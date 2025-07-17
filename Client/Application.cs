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
        private string _lastCustomerId = string.Empty;

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
            _lastCustomerId = customerClient.Id;
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

        public void Submit(int customerIndex)
        {
            var customer = _service.GetCustomers().ElementAt(customerIndex);
            _service.SubmitForEvaluation(customer.Id);
        }

        public LegalEntityClient AddLegalEntity(int customerIndex, LegalEntityClient legalEntityClient)
        {
            var customer = _service.GetCustomers().ElementAt(customerIndex);
            legalEntityClient.CustomerId = customer.Id;

            _service.AddLegalEntity(customer.Id, legalEntityClient);

            return legalEntityClient;
        }

        public void ShowData()
        {
            _service.ViewDatabase();
        }

        
    }
}
