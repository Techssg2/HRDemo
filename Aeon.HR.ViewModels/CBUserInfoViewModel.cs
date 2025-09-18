using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class CBUserInfoViewModel
    {
        public string UserSAPCode { get; set; }
        public string CreatedByFullName { get; set; }
        public string DeptName { get; set; }
        public Guid? DeptId { get; set; }
        public string DeptCode { get; set; }
        public Guid? DivisionId { get; set; }
        public string DivisionCode { get; set; }
        public string DivisionName { get; set; }
        public string WorkLocationCode { get; set; }
        public string WorkLocationName { get; set; }
        public DateTimeOffset? StartingDate { get; set; }
    }
}
