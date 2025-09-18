using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Data.Models
{
    public class CanViewDepartment : IEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid DeptLineId { get; set; }
        public string DeptLineCode { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
    }
}
