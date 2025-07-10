using Models;
using Models.Contract;
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

        public EntityLayout<Customer, CustomerClient> GetCustomer(int index)
        {
            var layout = Database.Instance.CustomerCollection.ElementAt(index);

            return layout;
        }

        public IEnumerable<CustomerClient> GetCustomers()
        {
            var customers = Database.Instance.CustomerCollection.Select(c => c.ClientCopy);
            return customers;
        }

        public void UpdateCustomer(CustomerClient customer, bool submit = false)
        {
            _changeHandler.Change(customer, submit);
            EventAggregator.Log("Customer updated:'{0}'", customer.ToString());
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
                EventAggregator.Log("WorkingCopy:'{0}'", customer.WorkingCopy == null ? string.Empty : DoFormat(customer.WorkingCopy));
                EventAggregator.Log("ReadyCopy:'{0}'", customer.ReadyCopy == null ? string.Empty : customer.ReadyCopy.ToString());
                EventAggregator.Log("<...RECORD END....>");
            }

            var leglEntities = Database.Instance.LegalEntityCollection;

            EventAggregator.Log("Legal Entities");
            EventAggregator.Log("--------------");
            foreach (var legalEntity in leglEntities)
            {
                EventAggregator.Log("<...RECORD START....>");
                EventAggregator.Log("Id:{0}", legalEntity.Id);
                EventAggregator.Log("ClientCopy:'{0}'", legalEntity.ClientCopy == null ? string.Empty : legalEntity.ClientCopy.ToString());
                EventAggregator.Log("SubmittedCopy:'{0}'", legalEntity.LastestSubmittedCopy == null ? string.Empty : legalEntity.LastestSubmittedCopy.ToString());
                EventAggregator.Log("WorkingCopy:'{0}'", legalEntity.WorkingCopy == null ? "No data" : "Has data");
                EventAggregator.Log("ReadyCopy:'{0}'", legalEntity.ReadyCopy == null ? string.Empty : legalEntity.ReadyCopy.ToString());
                EventAggregator.Log("<...RECORD END....>");
            }
        }

        private static string DoFormat(IList<Customer?> workingCopy)
        {
            StringBuilder sb = new();

            foreach (var customer in workingCopy) {
                sb.AppendLine(customer.ToString());
            }

            return sb.ToString();
        }

        public LegalEntityClient AddLegalEntity(string customerId, LegalEntityClient legalEntityClient, bool submit)
        {
            _changeHandler.Change(legalEntityClient, submit);

            return legalEntityClient;
        }
    }
}
