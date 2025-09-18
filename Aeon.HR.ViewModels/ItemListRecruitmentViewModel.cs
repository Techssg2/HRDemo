using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class ItemListRecruitmentViewModel
    {

        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
        public Guid CreatedById { get; set; }
        public Guid ModifiedById { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string AppService { get; set; }
    }
}
