namespace BitPool.Cryptography.MACs
{
    public interface IMacWithSalt
    {
        void Init (byte[] key, byte[] salt);
    }
}