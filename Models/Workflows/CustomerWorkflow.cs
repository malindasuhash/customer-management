using Models.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Workflows
{
    internal class CustomerWorkflow : IWorkflow
    {
        public void Run()
        {
            EventAggregator.Log("START: Customer workflow");

            EventAggregator.Log("END: Customer workflow");
        }
    }
}
