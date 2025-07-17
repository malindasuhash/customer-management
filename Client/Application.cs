using Models;
using Models.Infrastructure;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
            var customerClient = new CustomerClient
            {
                EmailAddress = emailAddress
            };

            EventAggregator.Log("Adding a new customer");

            
            _service.AddCustomer(customerClient); 
        }

        // GET /customer/{customerId}
        public EntityLayout<Customer, CustomerClient> GetCustomer(int index) // For simplicity, using index as customerId
        {
            var customer = _service.GetCustomer(index);

            return customer;
        }

        // PUT /customer/{customerId} { emailAddress }
        public void UpdateCustomer(int index, string newEmailAddress)
        {
            var customer = _service.GetCustomers().ElementAt(index);
            customer.EmailAddress = newEmailAddress;

            _service.UpdateCustomer(customer);
        }

        // PUT /customer/{customerId}
        public void UpdateCustomer(int index)
        {
            var customer = _service.GetCustomers().ElementAt(index);

            _service.UpdateCustomer(customer);
        }

        // POST customer/{customer-id}/legal-entity
        public LegalEntityClient AddLegalEntity(int customerIndex, LegalEntityClient legalEntityClient)
        {
            var customer = _service.GetCustomers().ElementAt(customerIndex);
            legalEntityClient.CustomerId = customer.Id;

            _service.AddLegalEntity(customer.Id, legalEntityClient);

            return legalEntityClient;
        }

        // PUT /customer/{customerId}/legal-entity/{legal-entity-id}
        public void UpdateLegalEntity(int legalEntityIndex, string newLegalName)
        {
            var legalEntity = _service.GetLegalEntity(legalEntityIndex);
            legalEntity.ClientCopy.LegalName = newLegalName;

            _service.UpdateLegalEntity(legalEntity.ClientCopy);
        }


        // GET /customer/{customerId}/Legal-entity/{legalEntityId}
        public EntityLayout<LegalEntity, LegalEntityClient> GetLegalEntity(int index) // For simplicity, using index
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
