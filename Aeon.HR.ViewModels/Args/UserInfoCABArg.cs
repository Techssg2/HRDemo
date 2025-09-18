using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels.Args
{
    public class UserInfoCABArg
    {
        public Guid UserId { get; set; }
        public bool IsActivated { get; set; }
        public string LockType { get; set; }
    }
}