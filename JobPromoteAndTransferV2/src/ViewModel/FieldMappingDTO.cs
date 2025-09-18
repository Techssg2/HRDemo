using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobResignationV2.src.ViewModel
{
    public class FieldMappingDTO
    {
        public string SourceField { get; set; }
        public string TargetField { get; set; }
        public FieldType Type { get; set; }
    }
}
