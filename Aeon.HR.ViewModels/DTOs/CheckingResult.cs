using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels
{
    public class CheckingResult
    {
        public bool success { get; set; }
        public string errorCode { get; set; }
        public string message { get; set; }
        public string description { get; set; }
        public string messageCode { get; set; }
    }
    public class CheckingResultCollection : List<CheckingResult>
    {
        public CheckingResultCollection() : base()
        {
        }

        public bool isSuccess()
        {
            return (this.Count == 0 || !(this.Any(x => !(x is null) && !x.success)));
        }
    }

    public class OTCheckingResult:CheckingResult
    {
        public string calculatedActualHoursFrom { get; set; }
        public string calculatedActualHoursTo { get; set; }
    }
    public class OTCheckingResultCollection : List<OTCheckingResult>
    {
        public OTCheckingResultCollection():base()
        {
        }

        public bool isSuccess()
        {
            return (this.Count == 0 || !(this.Any(x => !(x is null) && !x.success)));
        }
    }
}