using Aeon.HR.Infrastructure.Abstracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Data.Models
{
    public class HRTrackingAPILog : Entity
    {
        public Guid ItemId { get; set; }

        [StringLength(50)] 
        public string Action { get; set; }

        [StringLength(2000)] 
        public string Payload { get; set; }

        [StringLength(2000)]
        public string Response { get; set; }
    }
}
