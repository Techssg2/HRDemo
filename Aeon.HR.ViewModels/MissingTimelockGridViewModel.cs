using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels
{
    public class MissingTimelockGridViewModel: CBUserInfoViewModel
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public string ReferenceNumber { get; set; }
        public DateTimeOffset Created { get; set; }

    }
}