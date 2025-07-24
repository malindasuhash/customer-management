using Models.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Infrastructure
{
    public class DocumentStateManager
    {
        public void Transition<T>(IDocument<T> document) where T: class, IEntity, new()
        {
            if (document.Id == null)
            {
                document.CurrentState = State.New;
            }
        }

    }
}
