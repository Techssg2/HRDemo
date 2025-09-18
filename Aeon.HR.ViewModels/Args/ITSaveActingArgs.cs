using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.Args
{
    public class ITSaveActingArgs
    {
        public string ReferenceNumber { get; set; }
        public Guid? WorkingAddressRecruitmentId { get; set; }
        public CheckBudgetOption HasBudget { get; set; }
    }
}
