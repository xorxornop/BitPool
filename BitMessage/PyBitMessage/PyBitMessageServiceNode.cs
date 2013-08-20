using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace BitPool.BitMessage.PyBitMessage
{
    /// <summary>
    /// Provides access to the BitMessage network through interop with PyBitMessage.
    /// </summary>
    class PyBitMessageServiceNode : IBitMessageServiceNode
    {
        /// <summary>
        /// Do not set this to true if PyBitMessage will also be used for non-BitPool messaging. 
        /// If true, all messages not possessing the BitPool subject tag will be trashed upon receiving.
        /// </summary>
        public bool TrashIrrelevantMessages { get; set; }

        private readonly PyBitMessageClient _client;

        private readonly Timer _pollTimer;

        public event EventHandler<ReceivedMessagesArgs> ReceivedMessages;

        /// <summary>
        /// Start a new instance of a BitMessage node operating through interop with a PyBitMessage instance.
        /// </summary>
        /// <param name="server">Path of the server. If null, default is http://127.0.0.1:1337 </param>
        /// <param name="pollInterval">Interval in seconds between polling PyBitMessage for new inbox messages. Default is 1 minute (60).</param>
        public PyBitMessageServiceNode (string username, string password, Uri server = null, int pollInterval = 60) {
            _client = new PyBitMessageClient(username,password, server ?? new Uri("http://127.0.0.1:1337"));
            _pollTimer = new Timer(pollInterval * 1000);
            _pollTimer.Elapsed += PollTimerOnElapsed;
            _pollTimer.AutoReset = true;
            _pollTimer.Start();
        }

        /// <summary>
        /// Fired on poll timer Elapsed event invocation. Communicates 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="elapsedEventArgs"></param>
        private void PollTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs) {
            var messages = _client.GetAllInboxMessages();
            if(messages.Count == 0) return;
            
            var unrelated = messages.Where(message => Utilities.HasBitPoolSubjectTag(message.Subject)) as BMMessage[];

            if (unrelated == null) return;
            if (TrashIrrelevantMessages) {
                // Purge all those messages that are not BitPool-related from PyBitMessage, if enabled
                _client.TrashMessages(unrelated.Select(message => message.ID));
            }
            ReceivedMessages(this, new ReceivedMessagesArgs(messages.Except(unrelated)));
        }

        private void PurgeTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs) {

            Log.LogEvent("Purged PyBitMessage.");
        }



        

        public void RegisterReceiverHandler (EventHandler<ReceivedMessagesArgs> handler) { ReceivedMessages += handler; }


        public void Send (byte[] data, string recipient) {
            throw new NotImplementedException();
        }

        public string GenerateAddress (string label) {
            throw new NotImplementedException();
        }

        public string GenerateAddress (string label, string passphrase) {
            throw new NotImplementedException();
        }

        public IEnumerable<BMAddress> LocalAddresses {
            get { return _client.GetLocalAddresses(); }
        }
    }
}
