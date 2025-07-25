using Models.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Infrastructure
{
    public interface IDocument<T> where T : IEntity, ICloneable
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

        object Clone();
    }

    public class CustomerDocument : IDocument<Customer>
    {
        public string Id { get; set; }
        public Customer Draft { get; set; } = Customer.Empty;
        public int DraftVersion { get; set; }
        public Customer Submitted { get; set; } = Customer.Empty;
        public int SubmittedVersion { get; set; }
        public Customer Approved { get; set; } = Customer.Empty;
        public int ApprovedVersion { get; set; }
        public State CurrentState { get; set; } = State.New;

        public object Clone()
        {
            return new CustomerDocument
            {
                Id = Id,
                Draft = (Customer)Draft.Clone(),
                DraftVersion = DraftVersion,
                Submitted = (Customer)Submitted.Clone(),
                SubmittedVersion = SubmittedVersion,
                Approved = (Customer)Approved.Clone(),
                ApprovedVersion = ApprovedVersion,
                CurrentState = CurrentState
            };
        }
    }

    public class LegalEntityDocument : IDocument<LegalEntity>, ICloneable
    {
        public string Id { get; set; }
        public LegalEntity Draft { get; set; } = LegalEntity.Empty;
        public int DraftVersion { get; set; }
        public LegalEntity Submitted { get; set; } = LegalEntity.Empty;
        public int SubmittedVersion { get; set; }
        public LegalEntity Approved { get; set; } = LegalEntity.Empty;
        public int ApprovedVersion { get; set; }
        public State CurrentState { get; set; } = State.New;

        public object Clone()
        {
            return new LegalEntityDocument
            {
                Id = Id,
                Draft = (LegalEntity)Draft.Clone(),
                DraftVersion = DraftVersion,
                Submitted = (LegalEntity)Submitted.Clone(),
                SubmittedVersion = SubmittedVersion,
                Approved = (LegalEntity)Approved.Clone(),
                ApprovedVersion = ApprovedVersion,
                CurrentState = CurrentState
            };
        }
    }

    public enum State
    {
        New,
        EvaluationStarting,
        EvaluationStarted,
        EvaludationCompleted,
        Draft,
        Submitted,
        Approved,
        Rejected
    }
}
