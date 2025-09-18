using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels.Args
{
    public class OverTimeBalanceArgs
    {
        public string sapCode { get; set; }
        public string month { get; set; }
        public Guid? OvertimeApplicationid { get; set; }
        public double? OTHour { get; set; }
    }
}