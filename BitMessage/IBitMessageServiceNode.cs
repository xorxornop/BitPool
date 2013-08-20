using System;
using System.Collections.Generic;

namespace BitPool.BitMessage
{
    internal interface IBitMessageServiceNode
    {
        void Send(byte[] data, string recipient);
        string GenerateAddress (string label);
        string GenerateAddress(string label, string passphrase);

        /// <summary>
        /// Get pool-owned addresses that can be used as egress points to other pools and/or the wider BitMessage network. 
        /// </summary>
        /// <remarks>
        /// If messages are to be sent from pool to pool, in absence of an existing MatryoshkaClient connection to that pool,
        /// they are sent via this way.
        /// </remarks>
        IEnumerable<BMAddress> LocalAddresses { get; }

        /// <summary>
        /// Register a event handler that will fire when messages are received by the node, in order to process them.
        /// </summary>
        /// <param name="handler"></param>
        void RegisterReceiverHandler(EventHandler<ReceivedMessagesArgs> handler);
    }
}