using Models.Contract;
using Models.Infrastructure.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Infrastructure
{
    public class DocumentStateManager
    {
        public void Transition<T>(IDocument<T> document) where T: class, IEntity, ICloneable, new()
        {
            if (document.Id == null)
            {
                document.CurrentState = State.New;
                return;
            }

            switch (document.CurrentState)
            {
                case State.New:
                    document.Submitted = (T)document.Draft.Clone();
                    document.SubmittedVersion = document.DraftVersion;
                    document.CurrentState = State.EvaluationStarting;
                    // Store document in database
                    
                    // Clone and submit
                    var documentToSubmit = document.Clone();
                    EventAggregator.Publish(new EntityChanged(documentToSubmit));
                    break;
            }
        }

    }
}
