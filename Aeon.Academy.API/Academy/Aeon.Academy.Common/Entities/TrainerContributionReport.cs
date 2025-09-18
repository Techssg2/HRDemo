using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.Academy.Common.Entities
{
    public class TrainerContributionReport
    {
        public string SapCode { get; set; }
        public string FullName { get; set; }
        public string Department { get; set; }
        public string CourseName { get; set; }
        public int CourseDuration { get; set; }
        public int TotalCourseAttended { get; set; }
        public int TotalTimeAttended { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
