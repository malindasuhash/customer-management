using Models.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Infrastructure
{
    public interface IDocument<T> where T : IEntity
    {
        public string Id { get; set; }
        public string Name => typeof(T).Name;

        public T Draft { get; set; }
        public int DraftVersion { get; set; }

        public T Submitted { get; set; }
        public int SubmittedVersion { get; set; }

        public T Approved { get; set; }
        public int ApprovedVersion { get; set; }

        public State CurrentState { get; set; }
    }

    public class CustomerDocument : IDocument<Customer>
    {
        public string Id { get; set; }
        public Customer Draft { get; set; }
        public int DraftVersion { get; set; }
        public Customer Submitted { get; set; }
        public int SubmittedVersion { get; set; }
        public Customer Approved { get; set; }
        public int ApprovedVersion { get; set; }
        public State CurrentState { get; set; } = State.New;
    }

    public class LegalEntityDocument : IDocument<LegalEntity>
    {
        public string Id { get; set; }
        public LegalEntity Draft { get; set; }
        public int DraftVersion { get; set; }
        public LegalEntity Submitted { get; set; }
        public int SubmittedVersion { get; set; }
        public LegalEntity Approved { get; set; }
        public int ApprovedVersion { get; set; }
        public State CurrentState { get; set; } = State.New;
    }

    public enum State
    {
        New,
        Draft,
        Submitted,
        Approved,
        Rejected
    }
}
