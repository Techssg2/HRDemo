using Aeon.HR.Infrastructure.Entities;
using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class StepConditionViewModel
    {
        public string FieldName { get; set; }
        public IList<string> FieldValues { get; set; }
    }
}
