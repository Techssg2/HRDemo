using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class ExportApplicant
    {
        public string Status { get; set; }
        public string ReferenceNumber { get; set; }
        public string FullName { get; set; }
        public string Position { get; set; }
        public string IDCard9Number { get; set; }
        public string IDCard12Number { get; set; }
        public string Department { get; set; }
        public DateTimeOffset? DateOfBirth { get; set; }
        public string Created { get; set; }
    }
}
