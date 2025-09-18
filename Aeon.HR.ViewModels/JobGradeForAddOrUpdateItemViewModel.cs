using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class JobGradeForAddOrUpdateItemViewModel
    {
        public Guid Id { get; set; } // id job grade
        public List<Guid> ItemListRecruitmentIds { get; set; }

    }
}
