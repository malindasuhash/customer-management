namespace Models.Infrastructure.Events
{
    public class LegalEntityChanged : IEventInfo
    {
        public string EventName => nameof(LegalEntityChanged);

        public IDocument<LegalEntity> LegalEntityDocument { get; }

        public LegalEntityChanged(IDocument<LegalEntity> legalEntityDocument)
        {
            LegalEntityDocument = legalEntityDocument;
        }

        public override string? ToString()
        {
            return LegalEntityDocument is null ? "empty": LegalEntityDocument.ToString();
        }
    }
}
