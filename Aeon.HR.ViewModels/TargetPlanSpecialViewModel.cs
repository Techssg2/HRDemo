using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class TargetPlanSpecialViewModel
    {
        public Guid Id { get; set; }
        public Guid DepartmentId { get; set; }
        public bool? IsIncludeChildren { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentCode { get; set; }
    }
}
