using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.DTOs
{
    public class RequestToChangeTargetDTO
    {
        public string SAPCode { get; set; }
        public string FullName { get; set; }
        public Guid TargetPlanDetailId { get; set; }
        public TypeTargetPlan Type { get; set; }
        public string Comment { get; set; }
    }
}
