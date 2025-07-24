using Models;
using Models.Infrastructure;

namespace Service
{
    public class CustomerManager
    {
        private readonly EntityChangeHandler _changeHandler = new();

        public Customer AddCustomer(Customer customer)
        {
            var customerDocument = new CustomerDocument()
            {
                Draft = customer
            };

            _changeHandler.Manage(customerDocument);

            return customer;
        }

        public IDocument<Customer> GetCustomer(int index)
        {
            var layout = Database.Instance.CustomerDocuments.ElementAt(index);

            return layout;
        }

        public IEnumerable<IDocument<Customer>> GetCustomers()
        {
            var customers = Database.Instance.CustomerDocuments;

            return customers;
        }

        public void UpdateCustomer(IDocument<Customer> customer)
        {
            _changeHandler.Manage(customer);

            EventAggregator.Log("Customer updated:'{0}'", customer.Id);
        }

        public LegalEntity AddLegalEntity(string customerId, LegalEntity legalEntity)
        {
            var legalEntityDocument = new LegalEntityDocument
            {
                Draft = legalEntity
            };

            _changeHandler.Manage(legalEntityDocument);

            return legalEntity;
        }

        public IDocument<LegalEntity> GetLegalEntity(int legalEntityIndex)
        {
            var legalEntityDocument = Database.Instance.LegalEntityDocuments.ElementAt(legalEntityIndex);

            return legalEntityDocument;
        }

        public void UpdateLegalEntity(IDocument<LegalEntity> legalEntity)
        {
            _changeHandler.Manage(legalEntity);
            EventAggregator.Log("LegalEntity updated:'{0}'", legalEntity);
        }

        public void SubmitForEvaluation(string id)
        {
            _changeHandler.Submit(id, EntityName.Customer);
        }

        public void ViewDatabase()
        {
            var customers = Database.Instance.CustomerDocuments;

            EventAggregator.Log("Customers");
            EventAggregator.Log("---------");
            foreach (var customer in customers)
            {
                EventAggregator.Log("<...RECORD START....>");
                EventAggregator.Log("Id:{0}", customer.Id);
                EventAggregator.Log("Draft:'{0}'", customer.Draft == null ? "empty" : customer.Draft);
                EventAggregator.Log("Submitted:'{0}'", customer.Submitted == null ? "empty" : customer.Submitted);
                EventAggregator.Log("Approved:'{0}'", customer.Approved == null ? "empty" : customer.Approved);
                EventAggregator.Log("CurrentState:'{0}'", customer.CurrentState);
                EventAggregator.Log("<...RECORD END....>");
            }

            var leglEntities = Database.Instance.LegalEntityDocuments;

            EventAggregator.Log("Legal Entities");
            EventAggregator.Log("--------------");
            foreach (var legalEntity in leglEntities)
            {
                EventAggregator.Log("<...RECORD START....>");
                EventAggregator.Log("Id:{0}", legalEntity.Id);
                EventAggregator.Log("Draft:'{0}'", legalEntity.Draft == null ? "empty" : legalEntity.Draft);
                EventAggregator.Log("Submitted:'{0}'", legalEntity.Submitted == null ? "empty" : legalEntity.Submitted);
                EventAggregator.Log("Approved:'{0}'", legalEntity.Approved == null ? "empty" : legalEntity.Approved);
                EventAggregator.Log("CurrentState:'{0}'", legalEntity.CurrentState);
                EventAggregator.Log("<...RECORD END....>");
            }
        }
    }
}
