namespace Models
{
    public class LegalEntity : ICloneable
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string LegalName { get; set; }
        public string Name => EntityName.LegalEntity;

        public override string ToString() => string.Format($"Id:'{Id}', CustomerId:'{CustomerId}', LegalName:'{LegalName}'");

        public object Clone()
        {
            return new LegalEntity
            {
                Id = Id,
                CustomerId = CustomerId,
                LegalName = LegalName
            };
        }
    }
}
