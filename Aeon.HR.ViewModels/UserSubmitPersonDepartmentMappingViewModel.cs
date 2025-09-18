using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class UserSubmitPersonDepartmentMappingViewModel
    {
        public Guid DepartmentId { get; set; }
        public Guid UserId { get; set; }
        public string SAPCode { get; set; }
        public bool IsSubmitPerson { get; set; }
    }
}
