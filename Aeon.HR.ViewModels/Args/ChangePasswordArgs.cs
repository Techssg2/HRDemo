using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.Args
{
    public class ChangePasswordArgs
    {
        public Guid Id { get; set; }
        public string OldPassword { get; set;}
        public string NewPassword { get; set; }
    }
}
