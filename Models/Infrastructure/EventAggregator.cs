using Models.Infrastructure.Events;
using Models.Workflows;
using Models.Workflows.Events;
using Models.Workflows.Handlers;
using System.IO.Pipes;
using System.Reflection;

namespace Models.Infrastructure
{
    public static class EventAggregator
    {
        private static readonly NamedPipeClientStream _logClient;
        private static readonly NamedPipeClientStream _eventClient;

        private static StreamWriter _eventWriter;
        private static StreamWriter _logWriter;

        private static Dictionary<Type, Type> _typeMappings;
        private static Dictionary<Type, Type> _workfloweventMappings;

        static EventAggregator()
        {
            _logClient = new NamedPipeClientStream("CSLogPipe");
            _logClient.Connect();

            _eventClient = new NamedPipeClientStream("CSEventPipe");
            _eventClient.Connect();

            SetWorkflowMappings();
            SetWorkflowEventMappings();
        }

        public static void Publish(IEventInfo eventInfo)
        {
            _eventWriter ??= new StreamWriter(_eventClient);
            _eventWriter.WriteLine(eventInfo);
            _eventWriter.AutoFlush = true;

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

            // This is where the State Machine should run.
            var eventFromWorkflow = _workfloweventMappings.GetValueOrDefault(eventInfo.GetType());
            if (eventFromWorkflow == null)
            {
                Log($"No workflow mapping found for event type: {eventInfo.GetType().Name}");
                return;
            }

            var instance = Activator.CreateInstance(eventFromWorkflow);
            instance.GetType().InvokeMember("Handle", BindingFlags.InvokeMethod, null, instance, new object[] { eventInfo });
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
                // Customer Events
                {  typeof(CustomerChanged), typeof(CustomerEvaluationWorkflow) },
                {  typeof(CustomerEvaluationSuccessEvent), typeof(CustomerApplyWorkflow) },
                {  typeof(CustomerAppliedEvent), typeof(CustomerPostApplyWorkflow) },

                // Legal Entity Events
                {  typeof(LegalEntityChanged), typeof(LegalEntityEvaluationWorkflow) },
            };

        }

        private static void SetWorkflowEventMappings()
        {
            _workfloweventMappings = new Dictionary<Type, Type>()
            {
                { typeof(EvaluationRequireDependency), typeof(EvaluationRequireDependencyHandler) },
                { typeof(CustomerSynchonisedEvent), typeof(SynchonisedEventHandler) },
            };
        }
    }
}
