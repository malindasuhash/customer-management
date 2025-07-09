using Models.Infrastructure.Events;
using Models.Workflows;
using Models.Workflows.Events;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
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

        private static Dictionary<Type, Type> _typeMappings;

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

            if (eventInfo is EntitySubmitted submitted)
            {
                // Orchestrator is a special case, it handles the event directly
                Orchestrator.Instance.EntitySubmitted(submitted.EntityId, submitted.EntityName, submitted.Version);
                return;
            }

            // Find out whether there is a workflow mapping
            var workload = _typeMappings.GetValueOrDefault(eventInfo.GetType());

            if (workload != null && typeof(IWorkflow).IsAssignableFrom(workload))
            {
                Task.Run(() =>
                {
                    // Creates a new workflow dynamically
                    var instance = Activator.CreateInstance(workload);
                    instance.GetType().InvokeMember("Run", BindingFlags.InvokeMethod, null, instance, new object[] { eventInfo });
                });
            }
        }

        public static void Log(string message, params object[] values)
        {
            _logWriter ??= new StreamWriter(_logClient);
            _logWriter.WriteLine(string.Format("{0} -> {1}", DateTime.Now.ToString("HH:mm:ss:ff"), string.Format(message, values)));
            _logWriter.AutoFlush = true;
        }

        private static void SetWorkflowMappings()
        {
            _typeMappings = new Dictionary<Type, Type>()
            {
                // Customr Events
                {  typeof(CustomerChanged), typeof(CustomerEvaluationWorkflow) },
                {  typeof(CustomerEvaluationCompleteEvent), typeof(CustomerApplyWorkflow) },
                {  typeof(CustomerSynchonised), typeof(CustomerPostApplyWorkflow) },

                // Legal Entity Events
                {  typeof(LegalEntityChanged), typeof(LegalEntityEvaluationWorkflow) },
            };

        }
    }
}
