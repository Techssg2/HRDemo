using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.DTOs
{
    public class UploadTargetPlanDTO
    {
        public Guid? DeptId { get; set; }
        public Guid? DivisionId { get; set; }
        public Guid PeriodId { get; set; }
        public bool VisibleSubmit { get; set; }
        public string SAPCodes { get; set; }
    }
}
