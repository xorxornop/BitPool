namespace BitPool.BitMessage.PyBitMessage
{
    // Data transfer objects for XML-RPC with PyBitMessage

    public struct BitAddresses
    {
        public BitAddress[] addresses;
    }

    public struct BitAddress
    {
        public string label;
        public string address;
        public int stream;
        public bool enabled;
    }

    public struct BitMessages
    {
        public BitMessage[] messages;
    }

    public struct BitMessage
    {
        public int encodingType;
        public string toAddress;
        public string msgid;
        public int receivedTime;
        public string message;
        public string fromAddress;
        public string subject;
    }

    public struct Subscriptions
    {
        public Subscription[] Sub;
    }

    public struct Subscription
    {
        public string label;
        public string address;
        public bool enabled;
    }
}