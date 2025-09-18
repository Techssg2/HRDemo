using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class ApplicantFromMassDetailByIdViewModel
    {
        public object Data { get; set; }
        public bool Success { get; set; }
        public List<string> Messages { get; set; }
        public int Count { get; set; }
    }
}
