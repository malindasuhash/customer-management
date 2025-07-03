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
        private static readonly NamedPipeClientStream _logClient;
        private static readonly NamedPipeClientStream _eventClient;

        private static StreamWriter _eventWriter;
        private static StreamWriter _logWriter;
        static EventAggregator()
        {
            _logClient = new NamedPipeClientStream("CSLogPipe");
            _logClient.Connect();

            _eventClient = new NamedPipeClientStream("CSEventPipe");
            _eventClient.Connect();

        }
        public static void Publish(IEventInfo eventInfo)
        {
            _eventWriter ??= new StreamWriter(_eventClient);
            _eventWriter.WriteLine(eventInfo);
            _eventWriter.AutoFlush = true;
        }

        public static void Log(string message, params object[] values)
        {
            _logWriter ??= new StreamWriter(_logClient);
            _logWriter.WriteLine(String.Format("{0} -> {1}", DateTime.Now.ToString("HH:mm:ss:ff"), string.Format(message, values)));
            _logWriter.AutoFlush = true;
        }
    }
}
