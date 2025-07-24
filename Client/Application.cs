using Models;
using Models.Infrastructure;
using Service;

namespace Client
{
    internal class Application
    {
        private readonly CustomerManager _service;

        public Application()
        {
            _service = new CustomerManager();
        }

        // POST /customer
        public void AddCustomer(string emailAddress)
        {
            var customer = new Customer
            {
                EmailAddress = emailAddress
            };

            EventAggregator.Log("Adding a new customer");

            
            _service.AddCustomer(customer); 
        }

        // GET /customer/{customerId}
        public IDocument<Customer> GetCustomer(int index) // For simplicity, using index as customerId
        {
            var customer = _service.GetCustomer(index);

            return customer;
        }

        // PUT /customer/{customerId} { emailAddress }
        public void UpdateCustomer(int index, string newEmailAddress)
        {
            var customer = _service.GetCustomers().ElementAt(index);
            ((Customer)customer.Draft).EmailAddress = newEmailAddress;

            _service.UpdateCustomer(customer);
        }

        // PUT /customer/{customerId}
        public void UpdateCustomer(int index)
        {
            var customer = _service.GetCustomers().ElementAt(index);

            _service.UpdateCustomer(customer);
        }

        // POST customer/{customer-id}/legal-entity
        public LegalEntity AddLegalEntity(int customerIndex, string legalName)
        {
            var customer = _service.GetCustomers().ElementAt(customerIndex);

            var legalEntity = new LegalEntity
            {
                LegalName = legalName,
                CustomerId = customer.Id
            };

            _service.AddLegalEntity(customer.Id, legalEntity);

            return legalEntity;
        }

        // PUT /customer/{customerId}/legal-entity/{legal-entity-id}
        public void UpdateLegalEntity(int legalEntityIndex, string newLegalName)
        {
            var legalEntity = _service.GetLegalEntity(legalEntityIndex);
            ((LegalEntity)legalEntity.Draft).LegalName = newLegalName;

            _service.UpdateLegalEntity(legalEntity);
        }


        // GET /customer/{customerId}/Legal-entity/{legalEntityId}
        public IDocument<LegalEntity> GetLegalEntity(int index) // For simplicity, using index
        {
            var legalEntity = _service.GetLegalEntity(index);

            return legalEntity;
        }

        public void Submit(int customerIndex)
        {
            var customer = _service.GetCustomers().ElementAt(customerIndex);
            _service.SubmitForEvaluation(customer.Id);
        }

        public void ShowData()
        {
            _service.ViewDatabase();
        }
    }
}
