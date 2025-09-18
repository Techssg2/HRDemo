using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Aeon.HR.Data.Models
{
    public class TrackingLogInitData: IEntity
    {
        public Guid Id { get; set; }
        public Guid? TrackingLogId { get; set; }
        public string Code { get; set; }
        public string FunctionType { get; set; }
        public string SAPCode { get; set; }
        public string CreatedByFullName { get; set; }
        public string ReferenceNumber { get; set; }
        public DateTimeOffset? FromDate { get; set; }
        public DateTimeOffset? ToDate { get; set; }  
        public DateTimeOffset Created { get; set; } = DateTime.Now;
        public DateTimeOffset Modified { get; set; } = DateTime.Now;      
        public virtual TrackingRequest TrackingRequest { get; set; }
    }
}
