using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.Args
{
    public class UserForCheckDataArg
    {
        public Guid Id { get; set; }
        public string SAPCode { get; set; }
        public string LoginName { get; set; }
        public string Email { get; set; }

    }
}
