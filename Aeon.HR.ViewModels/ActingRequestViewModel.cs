using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels
{
    public class ActingRequestViewModel
    {
        public Guid Id { get; set; }
        public string ReferenceNumber { get; set; }
        public string UserFullName { get; set; }
        public string UserSAPCode { get; set; }
        public UserDepartmentMappingViewModel Mapping { get; set; }
        public string WorkLocationName { get; set; }
        public string CurrentPosition { get; set; }       //Current Title        
        public string TitleInActingPeriodName { get; set; } // master data 
        public DateTimeOffset Created { get; set; }
        public string DeptName { get; set; }
        public string DeptCode { get; set; }
        public string DivisionCode { get; set; }
        public string DivisionName { get; set; }
        public string Status { get; set; }
        public string RegionName { get; set; }
        public string CreatedByFullName { get; set; }
        public string CreatedByFullNameView { get; set; }// bug 651
    }
}