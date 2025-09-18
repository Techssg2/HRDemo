using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.Args
{
    public class UpdateStatusArgs
    {
        public string Status { get; set; }
        public string ReferenceNumber { get; set; }
        public string Comment { get; set; }
        public Guid Id { get; set; }
    }
}
