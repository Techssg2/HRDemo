using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.Tree
{
    public class DepartmentForTreeViewModel
    {
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public DepartmentType Type { get; set; }
        public bool? isFlag { get; set; }
    }
}
