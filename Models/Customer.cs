using Models.Contract;

namespace Models
{
    public class Customer : IEntity, ICloneable
    {
        public string EmailAddress { get; set; }
        public string Name => EntityName.Customer;

        public override string ToString() => string.Format($"Name:'{Name}', EmailAddress: '{EmailAddress}'");

        public object Clone()
        {
            return new Customer
            {
                EmailAddress = EmailAddress
            };
        }
    }
}
