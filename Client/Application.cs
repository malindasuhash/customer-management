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
        public void AddCustomer(string emailAddress)
        {
            var customerClient = new CustomerClient
            {
                EmailAddress = emailAddress
            };

            EventAggregator.Log("Adding a new customer");

            
            _service.AddCustomer(customerClient, true); 
        }

        // GET /customer/{customerId}
        public CustomerClient GetCustomer(string customerId)
        {
            var customer = _service.GetCustomer(customerId);

            return customer;
        }

    }
}
