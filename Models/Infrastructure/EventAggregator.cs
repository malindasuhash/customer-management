using Models.Infrastructure.Events;
using Models.Workflows;
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

        private static Dictionary<Type, Type> _workflowMappings;

        static EventAggregator()
        {
            _logClient = new NamedPipeClientStream("CSLogPipe");
            _logClient.Connect();

            _eventClient = new NamedPipeClientStream("CSEventPipe");
            _eventClient.Connect();

            SetWorkflowMappings();

        }
        public static void Publish(IEventInfo eventInfo)
        {
            _eventWriter ??= new StreamWriter(_eventClient);
            _eventWriter.WriteLine(eventInfo);
            _eventWriter.AutoFlush = true;
        }

        public static void Publish(IWorkflowEvent trigger)
        {
            var workflowType =_workflowMappings.GetValueOrDefault(trigger.GetType());

            var instance = Activator.CreateInstance(workflowType);
            instance.GetType().InvokeMember("Run", System.Reflection.BindingFlags.Instance, null, null, null);
        }

        public static void Log(string message, params object[] values)
        {
            _logWriter ??= new StreamWriter(_logClient);
            _logWriter.WriteLine(String.Format("{0} -> {1}", DateTime.Now.ToString("HH:mm:ss:ff"), string.Format(message, values)));
            _logWriter.AutoFlush = true;
        }

        private static void SetWorkflowMappings()
        {
            _workflowMappings = new Dictionary<Type, Type>()
            {
                {  typeof(CustomerWorkflowEvent), typeof(CustomerWorkflow) }
            };

        }
    }
}
