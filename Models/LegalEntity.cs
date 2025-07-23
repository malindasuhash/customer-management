using Models.Contract;

namespace Models
{
    public class LegalEntity : IEntity, ICloneable
    {
        public string CustomerId { get; set; }
        public string LegalName { get; set; }
        public string Name => EntityName.LegalEntity;

        public override string ToString() => string.Format($"CustomerId:'{CustomerId}', LegalName:'{LegalName}'");

        public object Clone()
        {
            return new LegalEntity
            {
                CustomerId = CustomerId,
                LegalName = LegalName
            };
        }
    }
}
