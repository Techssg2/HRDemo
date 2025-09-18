using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.DTOs
{
    public class ValidUserForTargetPlanDTO
    {
        public string SAPCode { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTimeOffset? OfficialResignationDate { get; set; }
    }
}
