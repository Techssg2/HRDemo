using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Data.Models
{
    public class CompanyPolicy : SoftDeleteEntity
    {
        public string Name { get; set; }
        public CompanyPolicyEnums Type { get; set; }
        public int Value { get; set; }
    }
}
