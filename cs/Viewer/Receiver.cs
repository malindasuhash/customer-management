using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viewer
{
    internal class Receiver
    {
        private static ConsoleColor DefaultColour = ConsoleColor.DarkBlue;
        private static Dictionary<string, ConsoleColor> Tokens = new()
        {
            { "<red>", ConsoleColor.Red }
        };

        public void Receive()
        {
            int minutesToRun = 5;

            Console.WriteLine("Log Receiver starting.....\n\n");

            var server = new NamedPipeServerStream("CSLogPipe");

            var serverInfo = server.WaitForConnectionAsync();
            serverInfo.ContinueWith((task) =>
            {

                DateTime startTime = DateTime.Now;

                using var reader = new StreamReader(server);
                while ((DateTime.Now - startTime).TotalMinutes < minutesToRun)
                {
                    var line = reader.ReadLine(); if (line == null) continue;
                    var printableLineInfo = ParseText(line);
                    Console.BackgroundColor = printableLineInfo.Item1;
                    Console.Write(printableLineInfo.Item3); Console.WriteLine();
                    Console.BackgroundColor = printableLineInfo.Item2;
                }
            });

            Console.WriteLine("Press Enter to terminate");
            Console.ReadKey();
        }

        private Tuple<ConsoleColor, ConsoleColor, string> ParseText(string lineToPrint)
        {
            foreach (var token in Tokens)
            {
                if (lineToPrint.Contains(token.Key, StringComparison.InvariantCultureIgnoreCase))
                {
                    return Tuple.Create(token.Value, DefaultColour, lineToPrint.Replace(token.Key, string.Empty));
                }
            }

            return Tuple.Create(DefaultColour, DefaultColour, lineToPrint);
        }
    }
}


