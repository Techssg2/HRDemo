using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Data.Models
{
    public class UserPasswordHistories : SoftDeleteEntity
    {
        public Guid UserId { get; set; }
        public string PwdHistory { get; set; }
        public virtual User User { get; set; }
    }
}
