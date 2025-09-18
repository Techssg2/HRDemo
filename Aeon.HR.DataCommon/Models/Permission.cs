
using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Aeon.HR.Data.Models
{
    public class Permission : Entity
    {
        [Index]
        public Guid ItemId { get; set; }
        public Guid? UserId { get; set; }
        public Guid? DepartmentId { get; set; }
        public Group DepartmentType { get; set; }
        public Right Perm { get; set; }
        public virtual User User { get; set; }
        public virtual Department Department { get; set; }
    }
}