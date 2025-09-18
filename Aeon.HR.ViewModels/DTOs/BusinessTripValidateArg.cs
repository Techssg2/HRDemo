using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.DTOs
{
    public class BusinessTripValidateArg
    {
        public string BusinessTripDetails { get; set; }
        public Guid? BusinessTripApplicationId { get; set; }
    }
}
