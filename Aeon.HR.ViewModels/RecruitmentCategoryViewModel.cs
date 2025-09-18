using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class RecruitmentCategoryViewModel
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public int Priority { get; set; }
        public string ParentName { get; set; }
        public Guid? ParentId { get; set; }
    }
}
