using System;

namespace BitPool
{
    public static class UnixTime
    {
        private static DateTime UNIX = new DateTime(1970, 1, 1, 0, 0, 0);
        private static DateTime OVERFLOW = new DateTime(2038, 1, 19, 3, 14, 18);

        public static DateTime ConvertFrom (int unix) {
            if (unix > 0) {
                return UNIX.AddSeconds(unix);
            }
            //works until 7 Feb 2106
            return OVERFLOW.AddSeconds(int.MaxValue - unix);
        }

        public static int ConvertTo (DateTime DT) {
            return (int) DT.Subtract(UNIX).TotalSeconds;
        }
    }
}
