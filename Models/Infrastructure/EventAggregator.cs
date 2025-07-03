using Models.Infrastructure.Events;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Infrastructure
{
    public static class EventAggregator
    {
        private static readonly NamedPipeClientStream _client;
        static EventAggregator()
        {
            _client = new NamedPipeClientStream("CSPipe");
            _client.Connect();
            
        }
        public static void PublishEvent(IEventInfo eventInfo)
        {
           
        }

        public static void Log(string message, params object[] values) {
            var streamWriter = new StreamWriter(_client);
            streamWriter.WriteLine(String.Format("{0} -> {1}", DateTime.Now.ToString("HH:mm:ss"), string.Format(message, values)));
            streamWriter.AutoFlush = true;
        }
    }
}
