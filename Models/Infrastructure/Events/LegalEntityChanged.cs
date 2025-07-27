namespace Models.Infrastructure.Events
{
    public class LegalEntityChanged : IEventInfo
    {
        public string EventName => nameof(LegalEntityChanged);

        public LegalEntityDocument Document { get; }

        public LegalEntityChanged(string legalEntityId, LegalEntityDocument document)
        {
            Document = document;
        }

        public override string? ToString()
        {
            return Document is null ? "empty": Document.ToString();
        }
    }
}
