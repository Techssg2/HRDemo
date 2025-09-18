using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using Aeon.HR.Infrastructure.Abstracts;

namespace Aeon.HR.Data.Models
{
    public class GlobalLocation : SoftDeleteEntity
    {
        public string Code { get; set; }
        public Guid? BusinessTripLocationId { get; set; }
        public Guid PartitionId { get; set; }
        public virtual Partition Partition { get; set; }
        public virtual BusinessTripLocation BusinessTripLocation { get; set; }
    }
}
