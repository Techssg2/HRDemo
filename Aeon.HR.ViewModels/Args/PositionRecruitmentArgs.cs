using Aeon.HR.Infrastructure.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.Args
{
    public class PositionRecruitmentArgs
    {
        public PositionRecruitmentArgs()
        {
            TypeName = "Position";
        }
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string TypeName { get; set; }
        public Guid JobGradeId { get; set; }
    }
}
