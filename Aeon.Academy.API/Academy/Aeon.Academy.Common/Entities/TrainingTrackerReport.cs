using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.Academy.Common.Entities
{
    public class TrainingTrackerReport
    {
        public string SapCode { get; set; }
        public string FullName { get; set; }
        public string Division { get; set; }
        public string JobGrade { get; set; }
        public string CourseName { get; set; }
        public DateTime? StartingDate { get; set; }
        public DateTime? EndingDate { get; set; }
        public DateTime? ActualAttendingDate { get; set; }
        public string SupplierName { get; set; }
        public int TotalOnlineTrainingHours { get; set; }
        public int TotalOfflineTrainingHours { get; set; }
        public string TypeOfTraining { get; set; }
        public decimal? TrainingFee { get; set; }
        public Guid UserId { get; set; }
        public string DeptLine { get; set; }
    }
}
