using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateUserInfoFromSapJobV2.src.ViewModel
{
    public class EmployeeResponsSearchingViewModel
    {
        public string SAPCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string GenderCode { get; set; }
        public string WorkLocationCode { get; set; }
        public string JobGradeCode { get; set; }
        public string JobTitle { get; set; }
        public string PositionCode { get; set; }
        public string JoiningDate { get; set; }
    }
}
