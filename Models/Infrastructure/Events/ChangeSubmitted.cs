using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Infrastructure.Events
{
    public class ChangeSubmitted : IEventInfo
    {
        public ChangeSubmitted(IEntity newState)
        {
            NewState = newState;
        }

        public IEntity NewState { get; }
    }
}
