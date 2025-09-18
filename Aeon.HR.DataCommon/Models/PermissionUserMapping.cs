using Aeon.HR.Infrastructure.Abstracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Data.Models
{
    public class PermissionUserMapping : Entity
    {
        [Index]
        public Guid OwnerId { get; set; }
        [Index]
        public Guid AssignedId { get; set; }
    }
}
