using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.DTOs
{
    public class TargetPlanValidMapping
    {
        public TypeTargetPlan Type { get; set; }
        public string Name { get; set; }
        public List<string> Values { get; set; }
    }
}
