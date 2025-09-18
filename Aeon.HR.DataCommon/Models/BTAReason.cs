using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aeon.HR.Infrastructure.Abstracts;

namespace Aeon.HR.Data.Models
{
    public class BTAReason : SoftDeleteEntity
    {
        public string Name { get; set; }
    }
}
