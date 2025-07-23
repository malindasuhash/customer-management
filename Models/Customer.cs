namespace Models
{
    public class Customer : ICloneable
    {
        public string Id { get; set; }
        public string EmailAddress { get; set; }
        public string Name => EntityName.Customer;

        public override string ToString() => string.Format($"Id:'{Id}', Name:'{Name}', EmailAddress: '{EmailAddress}'");

        public object Clone()
        {
            return new Customer
            {
                Id = Id,
                EmailAddress = EmailAddress
            };
        }
    }
}
