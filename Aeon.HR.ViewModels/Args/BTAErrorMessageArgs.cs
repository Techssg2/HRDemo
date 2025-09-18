using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels.Args
{
    public class BTAErrorMessageArgs
    {
        public Guid Id { get; set; }
        public BTAErrorEnums APIType { get; set; }
        public string ErrorCode { get; set; }
        public string MessageEN { get; set; }
        public string MessageVI { get; set; }
    }
}