using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Infrastructure
{
    internal class EntityManager
    {
        public void Transition(IEntity entity)
        {
            entity.State = GetNextState(entity.State);
        }

        private string GetNextState(string currentState)
        {
            switch (currentState)
            {
                case EntityState.Draft:
                    return EntityState.Submitted;

                case EntityState.Submitted:
                    return EntityState.Evaluating;
            }

            return EntityState.Failed;
        }
    }
}
