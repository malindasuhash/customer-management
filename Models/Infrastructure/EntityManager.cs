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
            entity.State = GetNextState(entity.State);
        }

        private string GetNextState(string currentState)
        {
            if (currentState == null)
            {
                return EntityState.Draft;
            }

            return currentState switch
            {
                EntityState.Draft => EntityState.Submitted,
                EntityState.Submitted => EntityState.Evaluating,
                _ => EntityState.Failed,
            };
        }
    }
}
