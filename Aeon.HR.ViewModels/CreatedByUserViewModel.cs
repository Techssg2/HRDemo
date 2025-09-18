using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class CreatedByUserViewModel
    {
        public Guid? Id { get; set; }
        public string SAPCode { get; set; }
        public string FullName { get; set; }
        public string DeptLine { get; set; }
        public string DeptCode { get; set; }
        public string DivisionGroup { get; set; }
        public string DivisionCode { get; set; }
        public string WorkLocationCode { get; set; }
        public string WorkLocationName { get; set; }
        public string JobGradeCode { get; set; }
        public string JobGradeName { get; set; }
        public DateTimeOffset? StartingDate { get; set; }
    }
}
