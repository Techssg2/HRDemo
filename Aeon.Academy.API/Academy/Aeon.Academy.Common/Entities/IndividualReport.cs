using System;
using System.Collections.Generic;
using Aeon.Academy.Common.Workflow;

namespace Aeon.Academy.Common.Entities
{
    public class IndividualReport
    {
        public string CourseName { get; set; }
        public DateTime? StartingDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string SupplierName { get; set; }
        public string TypeofTraining { get; set; }
        public string CourseAssessment { get; set; }
    }
}
