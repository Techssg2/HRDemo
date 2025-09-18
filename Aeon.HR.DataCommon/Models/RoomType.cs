using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Data.Models
{
    public class RoomType: SoftDeleteEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int Quota { get; set; }
    }
}
