using Models;
using Models.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class CustomerManager
    {
        private readonly EntityChangeHandler _changeHandler = new EntityChangeHandler();

        public CustomerClient AddCustomer(CustomerClient customerClient, bool submit = false)
        {
            _changeHandler.Change(customerClient, submit);

            return customerClient;
        }

        public CustomerClient GetCustomer(string customerId)
        {
            return default;
        }
    }
}
