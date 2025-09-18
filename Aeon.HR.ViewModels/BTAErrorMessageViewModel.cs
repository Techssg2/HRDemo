using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels
{
    public class BTAErrorMessageViewModel
    {
        public Guid Id { get; set; }
        public BTAErrorEnums APIType { get; set; }
        public string ErrorCode { get; set; }
        public string MessageEN { get; set; }
        public string MessageVI { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
    }
}