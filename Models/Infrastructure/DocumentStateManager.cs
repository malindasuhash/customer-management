using Models.Contract;
using Models.Infrastructure.Events;
using Models.Workflows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Infrastructure
{
    public class DocumentStateManager
    {
        public static DocumentStateManager Instance { get; } = new DocumentStateManager();
        
        private DocumentStateManager()
        {
            
        }

        public void Transition<T>(IDocument<T> document, NextAction nextAction = NextAction.None) where T: class, IEntity, ICloneable, new()
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
                    document.CurrentState = State.Evaluating;
                    
                    // Update document in database
                    Database.Instance.UpsertDocument(document);

                    // Clone and submit
                    var documentToSubmit = (IDocument<T>)document.Clone();
                    EventAggregator.Publish(documentToSubmit.Changed());
                    break;

                case State.Evaluating:
                    switch (nextAction)
                    {
                        case NextAction.AwaitingDependency:
                            document.CurrentState = State.AwaitingDependency;
                            
                            // Update document in database
                            Database.Instance.UpsertDocument(document);
                            
                            break;
                    }
                    

                    break;
            }
        }

    }
}
