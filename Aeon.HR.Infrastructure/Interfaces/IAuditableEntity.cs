using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Infrastructure.Interfaces
{
    public interface IAuditableEntity: IEntity
    {
        string CreatedBy { get; set; }
        string ModifiedBy { get; set; }
        string AppService { get; set; }
    }
}
