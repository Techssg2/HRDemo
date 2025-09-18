using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aeon.HR.Infrastructure.Abstracts;

namespace Aeon.HR.Data.Models
{
    public class DaysConfiguration : SoftDeleteEntity
    {
        public string Name { get; set; }
        public int Value { get; set; }
        //public int SalaryPeriodFrom { get; set; }
        //public int SalaryPeriodTo { get; set; }
        //public int DeadlineOfSubmittingCABApplication { get; set; }
    }
}
