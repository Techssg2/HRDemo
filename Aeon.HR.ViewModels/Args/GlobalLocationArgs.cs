using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.Args
{
    public class GlobalLocationArgs
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public Guid BusinessTripLocationId { get; set; }
        public Guid PartitionId { get; set; }
    }
}
