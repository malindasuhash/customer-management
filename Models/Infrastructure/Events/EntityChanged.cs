using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Infrastructure.Events
{
    public class EntityChanged : IEventInfo
    {
        public string EventName => nameof(EntityChanged);

        public object Document { get; }

        public EntityChanged(object documentToSubmit)
        {
            Document = documentToSubmit;
        }
    }
}
