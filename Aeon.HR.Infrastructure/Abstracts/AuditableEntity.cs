using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;

namespace Aeon.HR.Infrastructure.Abstracts
{

    public abstract class AuditableEntity :  IAuditableEntity
    {
        public Guid Id { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
        public Guid? CreatedById { get; set; }
        public Guid ModifiedById { get; set; }
        [StringLength(255)]
        public string CreatedBy { get; set; }
        [StringLength(255)]
        public string ModifiedBy { get; set; }

        [StringLength(255)]
        public string CreatedByFullName { get; set; }
        [StringLength(255)]
        public string ModifiedByFullName { get; set; }

        [StringLength(255)]
        public string AppService { get; set; }
    }
}