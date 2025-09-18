using Aeon.HR.Infrastructure.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Data.Models
{
    public class BTALog : Entity
    {
        public Guid? BusinessTripApplicationId { get; set; }
        public string Message { get; set; }
    }
}
