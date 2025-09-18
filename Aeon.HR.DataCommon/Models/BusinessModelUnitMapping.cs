using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Enums;

namespace Aeon.HR.Data.Models
{
    public class BusinessModelUnitMapping : SoftDeleteEntity
    {
        public Guid BusinessModelId { get; set; }
        public string BusinessModelCode { get; set; }
        public string BusinessUnitCode { get; set; } // lay tu API
        public string Note { get; set; }
        public bool? IsStore { get; set; }
        public virtual BusinessModel BusinessModels { get; set; }
    }
}