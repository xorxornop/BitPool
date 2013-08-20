using System;
using BitPool.Cryptography.BouncyCastle;
using BitPool.Cryptography.MACs.BLAKE2B;

namespace BitPool.Cryptography.Digests
{
	public class BLAKE2BDigest : IDigest
	{
		protected Blake2BHasher hasher;
		protected int outputSize;

		public BLAKE2BDigest (int size, bool bits) : this(size, bits, true)
		{
		}

		internal BLAKE2BDigest(int size, bool bits, bool init) {
			var config = new Blake2BConfig () {
				Key = null,
				Salt = null,
				Personalization = null,
				OutputSizeInBytes = size,
			};

			if (bits) size /= 8;
			outputSize = size;
			if (!init) return;
			hasher = new Blake2BHasher (config);
		}

		#region IDigest implementation

		public int GetDigestSize ()
		{
			return outputSize;
		}

		public int GetByteLength ()
		{
			return 128;
		}

		public void Update (byte input)
		{
			hasher.Update (new byte[] { input });
		}

		public void BlockUpdate (byte[] input, int inOff, int length)
		{
			hasher.Update (input, inOff, length);
		}

		public int DoFinal (byte[] output, int outOff)
		{
			var outputBytes = hasher.Finish ();
			Array.Copy (outputBytes, 0, output, outOff, outputBytes.Length);
			return outputBytes.Length;
		}

		public void Reset ()
		{
			hasher.Init ();
		}

		public string AlgorithmName {
			get {
				return "BLAKE2B" + outputSize * 8;
			}
		}

		#endregion
	}
}

