using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CookComputing.XmlRpc;
using ServiceStack.Text;

namespace BitPool.BitMessage.PyBitMessage
{
    /// <summary>
    /// Provides BitMessage services through interop with PyBitMessage using XML-RPC. 
    /// 
    /// </summary>
    class PyBitMessageClient
    {
        private const string BroadcastAddress = "BROADCAST";

        private readonly IPyBitMessageServerAPI _api;

        /// <summary>
        /// Create a client with a connection to a PyBitMessage instance to interop with.
        /// </summary>
        /// <remarks>
        /// It is strongly recommended not to create connections to remote machines over unsecured networks.
        /// </remarks>
        /// <param name="username">Username to connect with.</param>
        /// <param name="password">Passowrd to connect with.</param>
        /// <param name="server">Path of the server. If null, default is http://127.0.0.1:1337 </param>
        public PyBitMessageClient (string username, string password, Uri server) {
            _api = XmlRpcProxyGen.Create<IPyBitMessageServerAPI>();
            _api.Url = server.ToString();
            _api.Headers.Add("Authorization", "Basic " + EncodeUTF8Base64Text(String.Format("{0}:{1}", username, password)));
        }

        #region Send/Receive messages & broadcasts
        /// <summary>
        /// Sends a message over the BitMessage network
        /// </summary>
        /// <param name="from">Sender</param>
        /// <param name="to">Address of the recipient.</param>
        /// <param name="subject">Message Subject</param>
        /// <param name="message">Message Body</param>
        public void SendMessage (string from, string to, string subject, string message) {
            if (to == BroadcastAddress) {
                _api.sendBroadcast(from, EncodeUTF8Base64Text(subject), EncodeUTF8Base64Text(message));
            } else {
                _api.sendMessage(to, from, EncodeUTF8Base64Text(subject), EncodeUTF8Base64Text(message));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        public void SendMessage (string from, string to, byte[] subject, byte[] message) {
            if (to == BroadcastAddress) {
                _api.sendBroadcast(from, Convert.ToBase64String(subject), Convert.ToBase64String(message));
            } else {
                _api.sendMessage(to, from, Convert.ToBase64String(subject), Convert.ToBase64String(message));
            }
            _api.
        }

        public void SendBroadcast (string from, string subject, string message) {
            SendMessage(from, BroadcastAddress, subject, message);
        }

        public void SendBroadcast (string from, byte[] subject, byte[] message) {
            SendMessage(from, BroadcastAddress, subject, message);
        }
        #endregion

        /// <summary>
        /// Gets all messages in Inbox.
        /// </summary>
        /// <returns>List of BitMessage Messages (BMMs), null if none.</returns>
        public List<BMMessage> GetAllInboxMessages () {
            BitMessages rawMessages;
            try {
                rawMessages = JsonSerializer.DeserializeFromString<BitMessages>(_api.getAllInboxMessages());
            } catch (Exception e) {
                Log.LogEvent("ERROR: PyBitMessageClient.CreateNeDeterministicAddress() threw exception: \n\t" + e.Message);
                return new List<BMMessage>();
            }
            var messageList = rawMessages.messages.Select(message => new BMMessage() {
                ID = message.msgid.Trim(), Data = Convert.FromBase64String(message.message.Trim()),
                Subject = Convert.FromBase64String(message.subject.Trim()), SourceAddress = message.fromAddress.Trim(), TargetAddress = message.toAddress.Trim()
            }).ToList();
            return messageList;
        }

        /// <summary>
        /// Get all addresses associated with local identities.
        /// </summary>
        /// <returns>List of BitMessage Addresses (BMAs), empty list if none.</returns>
        public List<BMAddress> GetLocalAddresses () {
            BitAddresses rawAddresses;
            try {
                rawAddresses = JsonSerializer.DeserializeFromString<BitAddresses>(_api.listAddresses());
            } catch (Exception e) {
                Log.LogEvent("ERROR: PyBitMessageClient.GetAddresses() threw exception: \n\t" + e.Message);
                return new List<BMAddress>();
            }
            var addressList = rawAddresses.addresses.Where(address => address.enabled).Select(address => new BMAddress() {
                Name = address.label, Address = address.address.Trim(),
                StreamNumber = address.stream
            }).ToList();
            return addressList;
        }

        public void TrashMessages (IEnumerable<string> msgids) {
            foreach (var msgid in msgids) {
                _api.trashMessage(msgid);
            }
        }

        #region Address generation
        /// <summary>
        /// Creates a random BitMessage Address
        /// </summary>
        /// <param name="label">Distinguishing name for the address.</param>
        /// <param name="shortAddress">Use extra computing power to get a shorter address.</param>
        /// <param name="totalDifficulty">Default is 1.</param>
        /// <param name="smallMessageDifficulty">Default is 1.</param>
        /// <returns>Bitmessage Address</returns>
        public string CreateAddress (string label, bool shortAddress = false, int totalDifficulty = 1, int smallMessageDifficulty = 1) {
            if (string.IsNullOrEmpty(label)) {
                var dt = DateTime.Now;
                label = String.Format("{0}-{1};{2}", Program.Name, dt.ToShortDateString(), dt.ToShortTimeString());
            }
            return _api.createRandomAddress(EncodeUTF8Base64Text(label), shortAddress, totalDifficulty, smallMessageDifficulty);
        }

        /// <summary>
        /// Create an address from a passphrase.
        /// </summary>
        /// <param name="passphrase">Passphrase for the address.</param>
        /// <param name="addressVersionNumber">Default is 0. This selects the best value known.</param>
        /// <param name="streamNumber">Default is 0. This selects the best value known.</param>
        /// <param name="eighteenByteRipe">Use extra computing power to get a shorter address.</param>
        /// <param name="totalDifficulty">Default is 1.</param>
        /// <param name="smallMessageDifficulty">Default is 1.</param>
        public BMAddress CreateNewDeterministicAddress (string passphrase, int addressVersionNumber = 0,
            int streamNumber = 0, bool eighteenByteRipe = false, int totalDifficulty = 1, int smallMessageDifficulty = 1) {
            List<BMAddress> addresses;
            try {
                addresses = CreateNewDeterministicAddresses(passphrase, 1, addressVersionNumber, streamNumber,
                                         eighteenByteRipe, totalDifficulty, smallMessageDifficulty);
            } catch (Exception e) {
                Log.LogEvent("ERROR: PyBitMessageClient.CreateNeDeterministicAddress() threw exception: \n\t" + e.Message);
                return null;
            }
            return addresses[0];
        }

        /// <summary>
        /// Create multiple addresses from a single passphrase.
        /// </summary>
        /// <param name="passphrase">Passphrase for the address.</param>
        /// <param name="amount">Number of addresses to generate from the one passphrase.</param>
        /// <param name="addressVersionNumber">Default is 0. This selects the best value known.</param>
        /// <param name="streamNumber">Default is 0. This selects the best value known.</param>
        /// <param name="eighteenByteRipe">Use extra computing power to get a shorter address.</param>
        /// <param name="totalDifficulty">Default is 1.</param>
        /// <param name="smallMessageDifficulty">Default is 1.</param>
        public List<BMAddress> CreateNewDeterministicAddresses (string passphrase, int amount = 1, int addressVersionNumber = 0,
            int streamNumber = 0, bool eighteenByteRipe = false, int totalDifficulty = 1, int smallMessageDifficulty = 1) {
            BitAddresses rawAddresses;
            try {
                rawAddresses = JsonSerializer.DeserializeFromString<BitAddresses>(_api.createDeterministicAddresses(EncodeUTF8Base64Text(passphrase),
                    amount, addressVersionNumber, streamNumber, eighteenByteRipe, totalDifficulty, smallMessageDifficulty));
            } catch (Exception e) {
                Log.LogEvent("ERROR: PyBitMessageClient.CreateNewDeterministicAddress() threw exception: \n\t" + e.Message);
                return new List<BMAddress>();
            }
            var addressList = rawAddresses.addresses.Select(address => new BMAddress() {
                Name = address.label, Address = address.address.Trim(),
                StreamNumber = address.stream
            }).ToList();
            return addressList;
        }

        /// <summary>
        /// Generates a deterministic BitMessage Address
        /// </summary>
        /// <remarks>Do not use this thinking it will be usable in sending a message with this client.</remarks>
        /// <param name="passphrase">Passphrase for the address</param>
        /// <param name="addressVersionNumber">Default is 0. This selects the best value known.</param>
        /// <param name="streamNumber">Default is 0. This selects the best value known.</param>
        /// <returns>BitMessage address, null on error</returns>
        public string GenerateAddress (string passphrase, int addressVersionNumber = 0, int streamNumber = 0) {
            string address = null;
            try {
                _api.getDeterministicAddress(EncodeUTF8Base64Text(passphrase), addressVersionNumber, streamNumber);
            } catch (Exception e) {
                Console.WriteLine(e);
            }

            return address;
        }
        #endregion

        /// <summary>
        /// Display a message in the PyBitMessage GUI.
        /// </summary>
        /// <param name="message"></param>
        public void DisplayGUIMessage(string message) { if (message != null) _api.statusBar(message); }

        #region Helper functions
        /// <summary>
        /// Encodes a string to UTF-8 inside a Base64 string.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="lineBreaks"></param>
        /// <returns></returns>
        static string EncodeUTF8Base64Text (string s, bool lineBreaks = true) {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(s), lineBreaks ? 
                Base64FormattingOptions.InsertLineBreaks : Base64FormattingOptions.None);
        }

        /// <summary>
        /// Decodes a string stored as UTF-8 inside a Base64 string.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        static string DecodeUTF8Base64Text (string s) {
            return Encoding.UTF8.GetString(Convert.FromBase64String(s));
        }
        #endregion
    }
}
