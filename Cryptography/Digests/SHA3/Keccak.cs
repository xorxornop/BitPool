using System;
using BitPool.Cryptography.BouncyCastle;

namespace BitPool.Cryptography.Digests.SHA3
{
    public abstract class KeccakBase : IDigest {
        protected const int KeccakB = 1600;
		protected const int KeccakNumberOfRounds = 24;
		protected const int KeccakLaneSizeInBits = 8 * 8;

        protected static readonly ulong[] RoundConstants = new ulong[] {
            0x0000000000000001UL,
            0x0000000000008082UL,
            0x800000000000808aUL,
            0x8000000080008000UL,
            0x000000000000808bUL,
            0x0000000080000001UL,
            0x8000000080008081UL,
            0x8000000000008009UL,
            0x000000000000008aUL,
            0x0000000000000088UL,
            0x0000000080008009UL,
            0x000000008000000aUL,
            0x000000008000808bUL,
            0x800000000000008bUL,
            0x8000000000008089UL,
            0x8000000000008003UL,
            0x8000000000008002UL,
            0x8000000000000080UL,
            0x000000000000800aUL,
            0x800000008000000aUL,
            0x8000000080008081UL,
            0x8000000000008080UL,
            0x0000000080000001UL,
            0x8000000080008008UL
        };

		protected ulong[] state = new ulong[5 * 5];//1600 bits
        protected byte[] buffer;
        protected int buffLength;

        protected int HashSizeValue;

        protected int keccakR;

        public int KeccakR
        {
            get { return keccakR; }
            protected set { keccakR = value; }
        }

		// Modified on 7-Apr-2013 by _zenith to make code more readable + efficient
        protected KeccakBase (int size, bool bits) {
			if (!bits)
				size *= 8; 
            
            switch (size) {
                case 224:
                    KeccakR = 1152;
                    break;
                case 256:
                    KeccakR = 1088;
                    break;
                case 384:
                    KeccakR = 832;
                    break;
                case 512:
                    KeccakR = 576;
                    break;
				default:
					throw new ArgumentException("Output size must be 224, 256, 384, or 512 bits", "size");
            }

			Initialize();
			HashSizeValue = size;
        }

        protected ulong ROL (ulong a, int offset) {
            return (((a) << ((offset) % KeccakLaneSizeInBits)) ^ ((a) >> (KeccakLaneSizeInBits - ((offset) % KeccakLaneSizeInBits))));
        }

        protected void AddToBuffer (byte[] array, ref int offset, ref int count) {
            int amount = Math.Min(count, buffer.Length - buffLength);
            Buffer.BlockCopy(array, offset, buffer, buffLength, amount);
            offset += amount;
            buffLength += amount;
            count -= amount;
        }

		protected void Initialize () {
            buffLength = 0;
            //State = new ulong[5 * 5];//1600 bits
			Array.Clear (state, 0, state.Length); // optimised on 7-Apr-2013 by _zenith to avoid newing unnecessarily.
        }

        protected virtual void HashCore (byte[] array, int ibStart, int cbSize) {
            if (array == null)
                throw new ArgumentNullException("array");
            if (ibStart < 0)
                throw new ArgumentOutOfRangeException("ibStart");
            if (cbSize > array.Length)
                throw new ArgumentOutOfRangeException("cbSize");
            if (ibStart + cbSize > array.Length)
                throw new ArgumentOutOfRangeException("ibStart or cbSize");
        }

        // Added interface members



        public string AlgorithmName {  get { return "Keccak" + HashSizeValue; } }

        public int GetByteLength () { return KeccakR / 8; }

        public int GetDigestSize () { return HashSizeValue / 8; }

        public abstract void Update(byte input);

        public abstract void BlockUpdate(byte[] input, int inOff, int length);

        public abstract int DoFinal(byte[] output, int outOff);

		public virtual void Reset() { Initialize(); }
    }
}
