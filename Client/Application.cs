using Models;
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

        public void AddCustomer(string emailAddress)
        {
            var customer = new Customer
            {
                EmailAddress = emailAddress
            };

            // POST /customer
            _service.AddCustomer(customer); 
        }

    }
}
