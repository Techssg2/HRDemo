using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    [Serializable]
    public class EmployeeNodeViewModel
    {
        public EmployeeNodeViewModel()
        {

            Items = new List<EmployeeNodeViewModel>();
        }
        public Guid DepartmentId { get; set; }
        public Guid? ParentDepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentCaption { get; set; }
        public string JobGrade { get; set; }
        public int Grade { get; set; }
        public string ColorCode { get; set; }
        public string EmployeeName { get; set; }
        //public byte[] EmployeeImage { get; set; }
        public string EmployeeImage { get; set; }
        public bool HasChild { get { return Items.Any(); } }
        public List<EmployeeNodeViewModel> Items { get; set; }

    }
}
