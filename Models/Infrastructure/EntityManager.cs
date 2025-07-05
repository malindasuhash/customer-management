using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Infrastructure
{
    public class EntityManager
    {
        public void Transition(IEntity entity)
        {
            var currentState = entity.State;
            entity.State = GetNextState(entity.State);

            EventAggregator.Log("State change: from:'{0}' to '{1}', Entity Id: '{2}'", currentState, entity.State, entity.Id);
        }

        private string GetNextState(string currentState, bool success = true)
        {
            if (currentState == null)
            {
                return EntityState.Draft;
            }

            return currentState switch
            {
                EntityState.Draft => EntityState.Submitted,
                EntityState.Submitted => EntityState.Evaluating,
                EntityState.Evaluating => EntityState.Applying,
                EntityState.Applying => EntityState.Ready,
                _ => EntityState.Failed,
            };
        }
    }
}
