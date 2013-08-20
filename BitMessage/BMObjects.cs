using System;

namespace BitPool.BitMessage
{
    #region Messages
    /// <summary>
    /// BitMessage message (BMMessage) at high level for inclusion in pool intermixed with BPMessages.
    /// </summary>
    class BMMessage : IEquatable<BMMessage>, IEquatable<PyBitMessage.BitMessage>
    {
        public BMMessage () { TimeCreated = DateTime.UtcNow; }

        public BMMessage (DateTime created) {
            if (created >= DateTime.UtcNow) {
                Log.LogEvent("ERROR - BMMessage constructor - Passed DateTime is in the future. Set to UtcNow instead.");
                TimeCreated = DateTime.UtcNow;
            } else {
                TimeCreated = created;
            }
        }

        public string ID { get; internal set; } // Msgid

        public byte[] Subject { get; internal set; }
        public string SubjectText {
            get {
                string text;
                try {
                    text = System.Text.Encoding.UTF8.GetString(Data);
                } catch (Exception) {
                    throw new FormatException("Data could not be decoded to a UTF-8 string.");
                }
                return text;
            }
        }

        public byte[] Data { get; internal set; }
        public string DataText {
            get {
                string text;
                try {
                    text = System.Text.Encoding.UTF8.GetString(Data);
                } catch (Exception) {
                    throw new FormatException("Data could not be decoded to a UTF-8 string.");
                }
                return text;
            }
        }

        public string TargetAddress { get; internal set; } // Receiver
        public string SourceAddress { get; internal set; } // Sender

        /// <summary>
        /// Time that the message was created, either in a recieve or send context.
        /// </summary>
        /// <remarks>
        /// This property does NOT reflect the time the message was originally sent, 
        /// if originating outside local system.
        /// </remarks>
        public DateTime TimeCreated { get; private set; }

        public MessageEncoding Encoding { get; internal set; }

        public bool Equals (BMMessage other) {
            return ID.Equals(other.ID, StringComparison.OrdinalIgnoreCase);
        }

        public bool Equals (PyBitMessage.BitMessage other) {
            return ID.Equals(other.msgid, StringComparison.OrdinalIgnoreCase);
        }
    }

    /// <summary>
    /// BitPool message. Message instance being this type (in a mixed collection) 
    /// means it originated from within BitPool (sending from or redirecting through).
    /// </summary>
    class BPMessage : BMMessage, IEquatable<BMMessage>
    {
        /// <summary>
        /// How much time to wait at minimum before sending/redirecting, where applicable. Zero-span if not-applicable.
        /// </summary>
        public TimeSpan DelayTime { get; internal set; }

        /// <summary>
        /// Message has a requested minimum delay time for sending/redirection.
        /// </summary>
        public bool HasDelay { get { return DelayTime.CompareTo(TimeSpan.Zero) > 0; } }
    }
    #endregion

    #region Addresses
    class BMAddress : IEquatable<BMAddress>, IEquatable<PyBitMessage.BitAddress>
    {
        public string Name { get; internal set; } // Label

        /// <summary>
        /// The address itself.
        /// </summary>
        public string Address { get; internal set; }

        public int AddressVersion { get; internal set; }
        public int StreamNumber { get; internal set; }

        private const int AveragePOWFactor = 320, SmallMessagePOWFactor = 14000;
        private int _averageDifficulty, _smallMessageDifficulty;

        /// <summary>
        /// Average number of nonce trials that must be performed for PoW relative to a scaling factor, accurate to 1 decimal place. 
        /// Do NOT use this value for cryptographic/PoW computations, but only for PoW comparisons or display.
        /// </summary>
        public double AverageDifficultyFactor {
            get { return Math.Round((double) _averageDifficulty / AveragePOWFactor, 1); }
            internal set { _averageDifficulty = (int) (value * AveragePOWFactor); }
        }

        /// <summary>
        /// Number of extra bytes padded on to a message to increase small messages' PoW relative to a scaling factor, accurate to 1 decimal place. 
        /// Do NOT use this value for cryptographic/PoW computations, but only for PoW comparisons or display.
        /// </summary>
        public double SmallMessageDifficultyFactor {
            get { return Math.Round((double) _smallMessageDifficulty / SmallMessagePOWFactor, 1); }
            internal set { _smallMessageDifficulty = (int) (value * SmallMessagePOWFactor); }
        }

        /// <summary>
        /// Average number of nonce trials that must be performed for PoW to send a message.
        /// </summary>
        public int AverageDifficulty {
            get {
                if (!CanBeInteger(_averageDifficulty, AveragePOWFactor)) {
                    throw new ArithmeticException("The difficulty cannot be represented as an integer without loss of required precision.");
                }
                return _averageDifficulty / AveragePOWFactor;
            }
            internal set { _averageDifficulty = value * AveragePOWFactor; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int SmallMessageDifficulty {
            get {
                if (!CanBeInteger(_smallMessageDifficulty, SmallMessagePOWFactor)) {
                    throw new ArithmeticException("The difficulty cannot be represented as an integer without loss of required precision.");
                }
                return _smallMessageDifficulty / SmallMessagePOWFactor;
            }
            internal set { _averageDifficulty = value * AveragePOWFactor; }
        }

        private static bool CanBeInteger (double x, double y) {
            return Math.Abs(Math.IEEERemainder(x, y) - 0) > Double.Epsilon;
        }

        public byte[] PublicEncryptionKey { get { throw new NotImplementedException(); } }
        public byte[] PublicSigningKey { get { throw new NotImplementedException(); } }

        public bool Equals (BMAddress other) { return Address.Equals(other.Address, StringComparison.OrdinalIgnoreCase); }

        public bool Equals (PyBitMessage.BitAddress other) {
            return Address.Equals(other.address, StringComparison.OrdinalIgnoreCase);
        }
    }
    #endregion

    enum MessageEncoding : int
    {
        /// <summary>
        /// Any data with this number may be ignored. The sending node might simply be sharing its public key with you.
        /// </summary>
        Ignore = 0,
        /// <summary>
        /// UTF-8. No 'Subject' or 'Body' sections. Useful for simple strings of data, like URIs or magnet links.
        /// </summary>
        Trivial,
        /// <summary>
        /// UTF-8. Uses 'Subject' and 'Body' sections. No MIME is used.
        /// </summary>
        Simple
    }
}
