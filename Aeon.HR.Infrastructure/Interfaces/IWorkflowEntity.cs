using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Infrastructure.Interfaces
{
    public interface IWorkflowEntity
    {
        string Status { get; set; }
        DateTimeOffset SignedDate { get; set; }
        string SignedBy { get; set; }
        string DeptCode { get; set; }
        string DeptName { get; set; }
    }
}
