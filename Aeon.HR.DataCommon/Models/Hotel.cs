using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;

namespace Aeon.HR.Data.Models
{
    public class Hotel : SoftDeleteEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }
        //public bool IsForeigner { get; set; }
        public ForeignerOptions IsForeigner { get; set; }
        public Guid BusinessTripLocationId { get; set; }
        // Business Trip Location trong setting
        public virtual BusinessTripLocation BusinessTripLocation { get; set; }
    }
}
