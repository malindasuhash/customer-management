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
        private readonly EntityChangeHandler _changeHandler = new();

        public CustomerClient AddCustomer(CustomerClient customerClient, bool submit = false)
        {
            _changeHandler.Change(customerClient, submit);

            return customerClient;
        }

        public CustomerClient GetCustomer(string customerId)
        {
            return default;
        }

        public void ViewDatabase()
        {
            var customers = Database.Instance.CustomerCollection;

            EventAggregator.Log("Customers");
            EventAggregator.Log("---------");
            foreach (var customer in customers)
            {
                EventAggregator.Log("<...RECORD START....>");
                EventAggregator.Log("Id:{0}", customer.Id);
                EventAggregator.Log("ClientCopy:'{0}'", customer.ClientCopy == null? string.Empty : customer.ClientCopy.ToString());
                EventAggregator.Log("SubmittedCopy:'{0}'", customer.LastestSubmittedCopy == null ? string.Empty : customer.LastestSubmittedCopy.ToString());
                EventAggregator.Log("WorkingCopy:'{0}'", customer.WorkingCopy == null ? string.Empty : customer.WorkingCopy.ToString());
                EventAggregator.Log("ReadyCopy:'{0}'", customer.ReadyCopy == null ? string.Empty : customer.ReadyCopy.ToString());
                EventAggregator.Log("<...RECORD END....>");
            }
        }
    }
}
