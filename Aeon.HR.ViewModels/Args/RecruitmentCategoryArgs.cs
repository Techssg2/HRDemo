using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.Args
{
    public class RecruitmentCategoryArgs
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public int Priority { get; set; }
        public Guid? ParentId { get; set; }
        public bool IsEdoc2 { get; set; } = true;
    }
}
