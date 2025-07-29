using Models.Contract;

namespace Models
{
    public class Customer : IEntity, ICloneable
    {
        public static Customer Empty = new Customer();
        public string EmailAddress { get; set; }
        public string Name => EntityName.Customer;

        public override string ToString() => string.Format($"EmailAddress: '{EmailAddress}'");

        public Customer CloneCustomer()
        {
            return new Customer
            {
                EmailAddress = EmailAddress
            };
        }

        public object Clone()
        {
            return new Customer
            {
                EmailAddress = EmailAddress
            };
        }
    }
}
