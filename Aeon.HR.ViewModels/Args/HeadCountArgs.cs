using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels.Args
{
    public class HeadCountArgs
    {
        public Guid? Id { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid JobGradeForHeadCountId { get; set; }
        public int Quantity { get; set; }
    }
}