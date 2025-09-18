using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Enums;

namespace Aeon.HR.Data.Models
{
    public class ChangeUser : SoftDeleteEntity
    {
        public string FromLoginName { get; set; }
        public string ToLoginName { get; set; }
        public Guid? UserId { get; set; }

    }
}