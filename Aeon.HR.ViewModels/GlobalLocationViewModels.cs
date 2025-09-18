using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class GlobalLocationViewModels
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public Guid? BusinessTripLocationId { get; set; }
        public string BusinessTripLocationName { get; set; }
        public Guid? PartitionId { get; set; }
        public string PartitionName { get; set; }
        public string PartitionCode { get; set; }
    }
}
