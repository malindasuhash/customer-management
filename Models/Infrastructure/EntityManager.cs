using Models.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Infrastructure
{
    public class EntityManager
    {
        public void Transition(IEntity entity, TransitionContext context = TransitionContext.Success)
        {
            var currentState = entity.State;
            entity.State = GetNextState(entity.State, context);

            EventAggregator.Log("'{3}' State change: from:'{0}' to '{1}', Entity Id: '{2}'", currentState, entity.State, entity.Id, entity.Name);
        }

        private string GetNextState(string currentState, TransitionContext context)
        {
            if (currentState == null)
            {
                return EntityState.Draft;
            }

            return currentState switch
            {
                EntityState.Draft => EntityState.Submitted,
                EntityState.Submitted => EntityState.Evaluating,
                EntityState.Evaluating =>
                   context switch
                   {
                       TransitionContext.Success => EntityState.Applying,
                       TransitionContext.Completed => EntityState.Approved,
                       _ => EntityState.Failed,
                   },
                EntityState.Applying => EntityState.Synchonised,
                _ => EntityState.Failed,
            };
        }
    }

   public enum TransitionContext
    {
        None,
        Success,
        Completed,
        Failed
    }
}
