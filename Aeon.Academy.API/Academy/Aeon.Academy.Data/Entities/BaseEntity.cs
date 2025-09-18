using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.Academy.Data.Entities
{
    public interface IDataEntity
    {
        Guid Id { get; set; }
    }
    public abstract class BaseEntity : IDataEntity
    {
        public BaseEntity()
        {
            Id = new Guid();
        }
        [Key]
        public Guid Id { get; set; }
    }

    public abstract class BaseTrackingEntity : BaseEntity
    {
        public DateTimeOffset Created { get; set; }
        [StringLength(255)]
        public string CreatedBy { get; set; }
        [StringLength(255)]
        public string CreatedByFullName { get; set; }
        public DateTimeOffset Modified { get; set; }
        public Guid? CreatedById { get; set; }
        public Guid ModifiedById { get; set; }
        [StringLength(255)]
        public string ModifiedBy { get; set; }
        [StringLength(255)]
        public string ModifiedByFullName { get; set; }
    }

    public abstract class BaseWorkflowEntity : BaseTrackingEntity
    {
        [StringLength(255)]
        [Required]
        public string ReferenceNumber { get; set; }
        public string Status { get; set; }
        public Guid? AssignedDepartmentId { get; set; }
        [StringLength(255)]
        public string AssignedDepartmentName { get; set; }
        public int? AssignedDepartmentGroup { get; set; }
        public string AssignedDepartmentPosition { get; set; }
        public string AssignedUserId { get; set; }
        public int? WorkflowStep { get; set; }
        public string WorkflowData { get; set; }
        public DateTimeOffset? DueDate { get; set; }

    }
}
