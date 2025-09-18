using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class JobGradeItemRecruitmentMappingForAddOrUpdateItemViewModel
    {
        public Guid Id { get; set; }
        public Guid JobGradeId { get; set; }
        public Guid ItemRecruitmentId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ItemUnit { get; set; }
    }
}
