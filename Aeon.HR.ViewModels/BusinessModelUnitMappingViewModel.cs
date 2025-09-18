using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Data.Models
{
    public class BusinessModelUnitMappingViewModel
    {
        public Guid Id { get; set; }
        public Guid BusinessModelId { get; set; }
        public string BusinessModelCode { get; set; }
        public string BusinessUnitCode { get; set; } // lay tu API
        public bool? IsStore { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
    }
}
