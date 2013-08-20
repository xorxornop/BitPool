using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitPool.BitMessage
{
    internal static class Utilities
    {
        private static readonly byte[] SubjectTag = Encoding.ASCII.GetBytes("BITPOOL::");

        public static bool HasBitPoolSubjectTag (byte[] subject) {
            if (subject == null || subject.Length < SubjectTag.Length) return false;
            return !SubjectTag.Where((t, i) => !subject[i].Equals(t)).Any();
        }
    }
}
