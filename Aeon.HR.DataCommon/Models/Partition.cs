using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aeon.HR.Infrastructure.Abstracts;

namespace Aeon.HR.Data.Models
{
    public class Partition : SoftDeleteEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
