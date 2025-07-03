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
        public void Receive()
        {
            int minutesToRun = 5;

            Console.WriteLine("Receiver starting.....");

            var server = new NamedPipeServerStream("CSPipe");

            var serverInfo = server.WaitForConnectionAsync();
            serverInfo.ContinueWith((task) =>
            {

                DateTime startTime = DateTime.Now;

                using var reader = new StreamReader(server);
                while ((DateTime.Now - startTime).TotalMinutes < minutesToRun)
                {
                    Console.WriteLine(reader.ReadLine());
                }

            });

            Console.WriteLine("Press Enter to terminate");
            Console.ReadKey();
        }
    }
}


