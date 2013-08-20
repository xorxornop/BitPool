using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace BitPool
{
    public static class Log
    {
        static Log() {
            Debug.Print("Log culture: {0}\nDates shown in format: {1}", Program.Culture.Name, Program.Culture.DateTimeFormat.ShortDatePattern);
        }

        public static void LogEvent (string data) { LogEvent(data, DateTime.Now); }

        public static void LogEvent(string data, DateTime eventTime) {
            Debug.Print("[{0}]\t{1}", DateTime.Now, data);
        }
    }
}
