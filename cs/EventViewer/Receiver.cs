using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EventViewer
{
    internal class Receiver
    {
        public void Receive()
        {
            int minutesToRun = 5;

            Console.WriteLine("Event Receiver starting.....\n\n");

            var server = new NamedPipeServerStream("CSEventPipe");

            var serverInfo = server.WaitForConnectionAsync();
            serverInfo.ContinueWith((task) =>
            {

                DateTime startTime = DateTime.Now;

                using var reader = new StreamReader(server);
                while ((DateTime.Now - startTime).TotalMinutes < minutesToRun)
                {
                    Console.WriteLine(string.Format("----------{0}----------", DateTime.Now.ToString("HH:mm:ss:ff")));
                    Console.WriteLine(reader.ReadLine());
                    Console.WriteLine("----------");
                }

            });

            Console.WriteLine("Press Enter to terminate");
            Console.ReadKey();
        }
    }
}
