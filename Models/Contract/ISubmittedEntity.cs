using Models.Infrastructure.Events;

namespace Models.Contract
{
    public interface ISubmittedEntity : IEntity
    {
        int SubmittedVersion { get; set; }

        IEventInfo GetChangedEvent();
    }
}
