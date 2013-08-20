using CookComputing.XmlRpc;

namespace BitPool.BitMessage.PyBitMessage
{
    /// <summary>
    /// XML-RPC API for interop with PyBitMessage
    /// </summary>
    internal interface IPyBitMessageServerAPI : IXmlRpcProxy
    {
        /// <summary>
        /// Gets a response of 'firstWord-secondWord'. Used as a simple test of the API.
        /// </summary>
        /// <returns>'firstWord-secondWord'</returns>
        [XmlRpcMethod]
        string helloWorld(string firstWord, string secondWord);

        /// <summary>
        /// Returns the sum of the supplied integers.
        /// </summary>
        /// <returns>Single integer, sum = a + b</returns>
        [XmlRpcMethod]
        int add(int a, int b);

        /// <summary>
        /// Displays a message in the PyBitMessage GUI.
        /// </summary>
        /// <param name="message">Message to display.</param>
        [XmlRpcMethod]
        void statusBar(string message);

        /// <summary>
        /// Gets a list of addresses that are associated with local Identities ("Your Identities" in GUI).
        /// </summary>
        /// <returns>List as JSON object.</returns>
        [XmlRpcMethod]
        string listAddresses ();

        /// <summary>
        /// Creates a random address with random address and stream versions.
        /// </summary>
        /// <param name="label">Base64 encoded distinguishing name for the address.</param>
        /// <returns>Address as JSON object.</returns>
        [XmlRpcMethod]
        string createRandomAddress (string label);

        /// <summary>
        /// Creates a random address with a random address and stream versions.
        /// </summary>
        /// <param name="label">Base64 encoded distinguishing name for the address.</param>
        /// <param name="eighteenByteRipe">Whether to use extra computing power to get a shorter address.</param>
        /// <param name="totalDifficulty">Default is 1.</param>
        /// <param name="smallMessageDifficulty">Default is 1.</param>
        /// <returns>Address as JSON object.</returns>
        [XmlRpcMethod]
        string createRandomAddress (string label, bool eighteenByteRipe, int totalDifficulty, int smallMessageDifficulty);

        /// <summary>
        /// Create multiple addresses from a single passphrase.
        /// </summary>
        /// <param name="passphrase">Base64 encoded passphrase for the address.</param>
        /// <returns>List as JSON object.</returns>
        [XmlRpcMethod]
        string createDeterministicAddresses (string passphrase);

        /// <summary>
        /// Create multiple addresses from a single passphrase.
        /// </summary>
        /// <param name="passphrase">Base64 encoded passphrase for the address.</param>
        /// <param name="addressVersionNumber">Default is 0. This selects the best value known.</param>
        /// <param name="streamNumber">Default is 0. This selects the best value known.</param>
        /// <param name="eighteenByteRipe">Whether to use extra computing power to get a shorter address.</param>
        /// <param name="totalDifficulty">Default is 1.</param>
        /// <param name="smallMessageDifficulty">Default is 1.</param>
        /// <returns>List as JSON object.</returns>
        [XmlRpcMethod]
        string createDeterministicAddresses (string passphrase, int numberOfAddresses, int addressVersionNumber, int streamNumber,
                                                    bool eighteenByteRipe, int totalDifficulty, int smallMessageDifficulty);

        /// <summary>
        /// Create an address from a passphrase, but DOES NOT add it to the Address Book.
        /// </summary>
        /// <param name="passphrase">Base64 encoded passphrase for the address.</param>
        /// <param name="addressVersionNumber">Default is 0. This selects the best value known.</param>
        /// <param name="streamNumber">Default is 0. This selects the best value known.</param>
        /// <returns>Address as JSON object.</returns>
        [XmlRpcMethod]
        string getDeterministicAddress (string passphrase, int addressVersionNumber, int streamNumber);

        /// <summary>
        /// Get a list of all the messages in the Inbox.
        /// </summary>
        /// <returns>List as JSON object.</returns>
        [XmlRpcMethod]
        string getAllInboxMessages();

        /// <summary>
        /// Get a sent message by its message ID (msgid).
        /// </summary>
        /// <param name="msgid">Message ID in hex encoding.</param>
        /// <returns>Message as JSON object.</returns>
        [XmlRpcMethod]
        string getInboxMessageByID (string msgid);

        /// <summary>
        /// Gets a sent message by its acknowledgement data (ackdata).
        /// </summary>
        /// <param name="ackData">Acknowledgement data in hex encoding.</param>
        /// <returns>Message as JSON object.</returns>
        [XmlRpcMethod]
        string getSentMessageByAckData (string ackData);

        /// <summary>
        /// Gets a list of all sent messages.
        /// </summary>
        /// <returns>List as JSON object.</returns>
        [XmlRpcMethod]
        string getAllSentMessages ();

        /// <summary>
        /// Gets a sent message by its message ID (msgid).
        /// </summary>
        /// <param name="msgid">Message ID in hex encoding.</param>
        /// <returns>Message as JSON object.</returns>
        [XmlRpcMethod]
        string getSentMessageByID (string msgid);

        /// <summary>
        /// Gets a list of sent messages from an address.
        /// </summary>
        /// <returns>List as JSON object.</returns>
        [XmlRpcMethod]
        string getSentMessagesBySender (string fromAddress);

        /// <summary>
        /// Deletes a message by its message ID (msgid).
        /// </summary>
        /// <param name="msgid">Message ID in hex encoding.</param>
        [XmlRpcMethod]
        void trashMessage(string msgid);

        /// <summary>
        /// Sends a message over the BitMessage network.
        /// </summary>
        /// <param name="subject">Base64 encoded subject.</param>
        /// <param name="message">Base64 encoded message.</param>
        /// <returns>Acknowledgement data.</returns>
        [XmlRpcMethod]
        string sendMessage (string toAddress, string fromAddress, string subject, string message);

        /// <summary>
        /// Sends a message over the BitMessage network.
        /// </summary>
        /// <param name="subject">Base64 encoded subject for the message.</param>
        /// <param name="message">Base64 encoded message.</param>
        /// <param name="encodingType">Type of encoding used for subject and message. Set to 2 if Base64.</param>
        /// <returns>Acknowledgement data.</returns>
        [XmlRpcMethod]
        string sendMessage (string toAddress, string fromAddress, string subject, string message, int encodingType);

        /// <summary>
        /// Sends a broadcast over the BitMessage network.
        /// </summary>
        /// <param name="fromAddress">Address to send the broadcast from.</param>
        /// <param name="subject">Base64 encoded subject for the broadcast.</param>
        /// <param name="message">Base64 encoded message.</param>
        /// <returns>Acknowledgement data.</returns>
        [XmlRpcMethod]
        string sendBroadcast (string fromAddress, string subject, string message);

        ///// <summary>
        ///// Gets the status of a message.
        ///// </summary>
        ///// <param name="ackdata">Acknowledgement data for the message.</param>
        ///// <returns>Status code for the message.</returns>
        //[XmlRpcMethod]
        //string getStatus (string ackdata);

        /// <summary>
        /// Adds a subscription to an address.
        /// </summary>
        [XmlRpcMethod]
        void addSubscription (string address);

        /// <summary>
        /// Adds a subscription to an address.
        /// </summary>
        /// <param name="label">Base64 encoded label for the subscription.</param>
        [XmlRpcMethod]
        void addSubscription (string address, string label);

        /// <summary>
        /// Deletes a subscription to an address.
        /// </summary>
        [XmlRpcMethod]
        void deleteSubscription (string addr);

        /// <summary>
        /// Gets the list of addresses stored in Subscriptions.
        /// </summary>
        [XmlRpcMethod]
        string listSubscriptions (); 
    }
}
