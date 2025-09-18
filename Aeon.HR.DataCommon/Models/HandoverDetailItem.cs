using Aeon.HR.Infrastructure.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Data.Models
{
    public class HandoverDetailItem : SoftDeleteEntity
    {   
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        public string Unit { get; set; }
        public int Quantity { get; set; }
        public string Notes { get; set; }
        public Guid ItemListRecruitmentId { get; set; }
        public virtual ItemListRecruitment ItemListRecruitment { get; set; }
        public Guid HandoverId { get; set; }
        public virtual Handover Handover { get; set; }
    }
}
