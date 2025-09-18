using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels.Args
{
    public class UpdatePayloadArgs
    {
        public string ReferenceNumber { get; set; }
        public string UserSAPCode { get; set; }
    }
}