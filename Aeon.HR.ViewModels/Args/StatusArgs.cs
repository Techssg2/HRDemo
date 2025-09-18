using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels.Args
{
    public class StatusArgs
    {
        public Guid RefNumber { get; set; }
        public StatusChange Status {get;set; }
        public string EmployeeCode { get; set; }
        public string Comment { get; set; }
    }
}