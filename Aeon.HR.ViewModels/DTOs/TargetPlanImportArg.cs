using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.DTOs
{
    public class TargetPlanImportArg
    {
        public Guid DeptId { get; set; }
        public Guid? DivisionId { get; set; }
        public Guid PeriodId { get; set; }
        public string SapCodes { get; set; }
        public bool VisibleSubmit { get; set; }
    }
}
