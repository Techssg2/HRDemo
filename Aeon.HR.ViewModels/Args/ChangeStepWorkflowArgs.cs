using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.Args
{
    public class ChangeStepWorkflowArgs
    {
        public Guid InstanceId { get; set; }
        public string ItemReferenceNumber { get; set; }
        public int NewStepNumber { get; set; }
        public int CurrentStepNumber { get; set; }
    }
}
