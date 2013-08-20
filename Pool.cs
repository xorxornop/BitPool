using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;
using BitPool.BitMessage;
using BitPool.BitMessage.PyBitMessage;

namespace BitPool
{
    class Pool
    {
        /// <summary>
        /// Provider of BitMessage network connectivity and I/O
        /// </summary>
        private readonly IBitMessageServiceNode _bmServiceNode;

        /// <summary>
        /// Collection of all messages - indexed by msgid (hash in hex form)
        /// </summary>
        private readonly Dictionary<string, BMMessage> _items = new Dictionary<string, BMMessage>();
        private readonly Timer _livenessTimer;

        /// <summary>
        /// Create a new BitPool
        /// </summary>
        /// <param name="livenessInterval">Interval of of purging old messages in seconds.</param>
        public Pool (IBitMessageServiceNode serviceNode, TimeSpan livenessInterval) {
            _livenessTimer = new Timer(livenessInterval.TotalMilliseconds);
            _livenessTimer.Elapsed += (sender, args) => {
                foreach (var s in _items.Where(p => p.Value.TimeCreated.Add(LiveTime) > DateTime.UtcNow).ToList()) {
                    _items.Remove(s.Key);
                    Log.LogEvent(String.Format("Removed item {0} : Liveness time exceeded.", s.Value.ID));
                }
            };

            _bmServiceNode = serviceNode;
            _bmServiceNode.RegisterReceiverHandler(ReceiveHandler);
        }

        /// <summary>
        /// How long a message is kept in the pool before being purged.
        /// </summary>
        public TimeSpan LiveTime { get; private set; }

        /// <summary>
        /// How many messages the pool currently contains.
        /// </summary>
        public int Count { get { return _items.Count; } }

        private void ReceiveHandler (object sender, ReceivedMessagesArgs receivedMessagesArgs) {
            // TODO: Perform additional processing to support redirection, delay time etc, by checking for such metadata
            foreach (var message in receivedMessagesArgs.Messages) {
                _items.Add(message.ID, message);
            }
        }
    }

    internal class ReceivedMessagesArgs : EventArgs
    {
        public ReceivedMessagesArgs (IEnumerable<BMMessage> ds) { Messages = ds; }
        public IEnumerable<BMMessage> Messages { get; private set; }
    }
}
