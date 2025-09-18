using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.Args
{
    public class HotelArgs
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }
        //public bool IsForeigner { get; set; }
        public ForeignerOptions IsForeigner { get; set; }
        public Guid BusinessTripLocationId { get; set; }
    }
}
