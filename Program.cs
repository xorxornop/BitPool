using System.Globalization;

namespace BitPool
{
    using System;
    using Nancy.Hosting.Self;

    class Program
    {
        public static readonly string Name = "BitPool";
        public static readonly CultureInfo Culture = CultureInfo.CurrentCulture;

        static void Main (string[] args) {
            var uri =
                new Uri("http://localhost:3579");

            using (var host = new NancyHost(uri)) {
                host.Start();

                Console.WriteLine("{0} is running on {1}", Name, uri);
                Console.WriteLine("PyBitMessage must also be running on this host for any I/O to take place with the wider BitMessage P2P network.");
                Console.WriteLine("Current configuration specifies non-automatic starting of PyBitMessage. To change this, modify 'config.json' .");
                Console.WriteLine("Any messages sent to {0} before starting PyBitMessage, where applicable, will be cached until started.", Name);
                Console.WriteLine("Press any key to close the host.");
                Console.ReadLine();
            }
        }
    }
}
