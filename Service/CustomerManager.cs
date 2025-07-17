using Models;
using Models.Contract;
using Models.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class CustomerManager
    {
        private readonly EntityChangeHandler _changeHandler = new();

        public CustomerClient AddCustomer(CustomerClient customerClient)
        {
            _changeHandler.Manage(customerClient);

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

        public void UpdateCustomer(CustomerClient customer)
        {
            _changeHandler.Manage(customer);
            EventAggregator.Log("Customer updated:'{0}'", customer.ToString());
        }

        public void SubmitForEvaluation(string id)
        {
            _changeHandler.Submit(id, EntityName.Customer);
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
                EventAggregator.Log("SubmittedCopy:'{0}'", customer.LastestSubmittedCopy == null ? string.Empty : $"Email:{customer.LastestSubmittedCopy.EmailAddress}, State:'{customer.LastestSubmittedCopy.State}'");
                EventAggregator.Log("WorkingCopy:'{0}'", customer.WorkingCopy == null ? string.Empty : DoFormat(customer.WorkingCopy));
                EventAggregator.Log("ReadyCopy:'{0}'", customer.ReadyCopy == null ? string.Empty : $"Email:{customer.ReadyCopy.EmailAddress}, State:'{customer.ReadyCopy.State}'");
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
                EventAggregator.Log("SubmittedCopy:'{0}'", legalEntity.LastestSubmittedCopy == null ? string.Empty : $"Name:'{legalEntity.LastestSubmittedCopy.Name}', State:'{legalEntity.LastestSubmittedCopy.State}'");
                EventAggregator.Log("WorkingCopy:'{0}'", 
                    legalEntity.WorkingCopy == null || legalEntity.WorkingCopy.Count() == 0 ? "No data" : $"LegalEntity: '{legalEntity.WorkingCopy.First().Name}', State:'{legalEntity.WorkingCopy.First().State}'");
                EventAggregator.Log("ReadyCopy:'{0}'", legalEntity.ReadyCopy == null ? string.Empty : $"Name:'{legalEntity.ReadyCopy.Name}', State:'{legalEntity.ReadyCopy.State}'");
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

        public LegalEntityClient AddLegalEntity(string customerId, LegalEntityClient legalEntityClient)
        {
            _changeHandler.Manage(legalEntityClient);

            return legalEntityClient;
        }

        public EntityLayout<LegalEntity, LegalEntityClient> GetLegalEntity(int legalEntityIndex)
        {
            var layout = Database.Instance.LegalEntityCollection.ElementAt(legalEntityIndex);

            return layout;
        }

        public void UpdateLegalEntity(LegalEntityClient legalEntity)
        {
            _changeHandler.Manage(legalEntity);
            EventAggregator.Log("LegalEntity updated:'{0}'", legalEntity);
        }
    }
}
