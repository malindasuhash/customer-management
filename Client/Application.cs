using Models;
using Models.Infrastructure;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public void AddCustomer(string emailAddress, bool submit = false)
        {
            var customerClient = new CustomerClient
            {
                EmailAddress = emailAddress
            };

            EventAggregator.Log("Adding a new customer");

            
            _service.AddCustomer(customerClient, submit); 
        }

        // GET /customer/{customerId}
        public CustomerClient GetCustomer(string customerId)
        {
            var customer = _service.GetCustomer(customerId);

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

            _service.UpdateCustomer(customer, true);
        }

        public void ShowData()
        {
            _service.ViewDatabase();
        }
    }
}
