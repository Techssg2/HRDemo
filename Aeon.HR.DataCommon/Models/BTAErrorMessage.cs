using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Aeon.HR.Data.Models
{
    public class BTAErrorMessage : Entity
    {
        public BTAErrorEnums Type { get; set; }
        [StringLength(200)]
        public string ErrorCode { get; set; }
        [StringLength(200)]
        public string MessageEN { get; set; }
        [StringLength(200)]
        public string MessageVI { get; set; }
    }
}