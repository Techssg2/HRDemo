using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.Args
{
    public class RoomOrganizationArg
    {
        public Guid BusinessTripApplicationId { get; set; }
        public string Data { get; set; }
        public bool IsChange { get; set; }
        public int TripGroup { get; set; }
    }
}
