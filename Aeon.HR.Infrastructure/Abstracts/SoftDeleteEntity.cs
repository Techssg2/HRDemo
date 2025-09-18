using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Infrastructure.Abstracts
{
    public class SoftDeleteEntity :AuditableEntity, ISoftDeleteEntity
    {
        public bool IsDeleted { get; set; }
    }
}
