using Aeon.HR.Infrastructure.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Data.Models
{
    public class TrackingAPIIntegrationLog : Entity
    {
        public string Module { get; set; }
        public string APIName { get; set; }
        public string Payload { get; set; }
        public string Response { get; set; }
        public string ErrorMessage { get; set; }
    }
}
