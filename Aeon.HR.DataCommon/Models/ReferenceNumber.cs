using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Aeon.HR.Data.Models
{
    public class ReferenceNumber: AuditableEntity
    {
        [Index]
        [StringLength(255)]
        public string ModuleType { get; set; }
        public int CurrentNumber { get; set; }
        public bool IsNewYearReset { get; set; }    //nếu = true thì field CurrentNumber sẽ trả về 1 
        public string Formula { get; set; }
        public int CurrentYear { get; set; }

    }
}