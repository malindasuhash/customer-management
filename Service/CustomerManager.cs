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

        public Customer AddCustomer(Customer customer, bool submit = false)
        {
            _changeHandler.Change(customer, submit);

            return customer;
        }
    }
}
