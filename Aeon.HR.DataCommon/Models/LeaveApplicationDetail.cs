using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Data.Models
{
    public class LeaveApplicationDetail : IEntity
    {
        public DateTimeOffset FromDate { get; set; }
        public DateTimeOffset ToDate { get; set; }
        public string SAPCode { get; set; }
        public string LeaveCode { get; set; }
        public string LeaveName { get; set; }
        public double Quantity { get; set; }
        public string Reason { get; set; }
        public bool IsApproved { get; set; }
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public DateTimeOffset Created { get; set; } = DateTime.Now;
        public DateTimeOffset Modified { get; set; } = DateTime.Now;
        public Guid LeaveApplicationId { get; set; }
        public virtual LeaveApplication LeaveApplication { get; set; }
    }
}
