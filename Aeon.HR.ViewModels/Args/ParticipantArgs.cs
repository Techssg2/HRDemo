using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.Args
{
    public class ParticipantArgs
    {
        public Guid DepartmentId { get; set; }
        public Group DepartmentGroup { get; set; }
    }
}
