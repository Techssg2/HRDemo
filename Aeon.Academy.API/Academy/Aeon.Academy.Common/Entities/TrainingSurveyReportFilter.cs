using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.Academy.Common.Entities
{
    public class TrainingSurveyReportFilter
    {

        private string sapCode;
        private string fullName;
        private string courseName;  
        private string trainerName;
        private DateTime? startDateFrom = null;
        private DateTime? startDateTo = null;
        private DateTime? endDateFrom = null;
        private DateTime? endDateTo = null;
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SapCode
        {
            get
            {
                return this.sapCode;
            }
            set
            {
                this.sapCode = string.IsNullOrEmpty(value) ? string.Empty : value.ToUpper();
            }
        }
        public string FullName
        {
            get
            {
                return this.fullName;
            }
            set
            {
                this.fullName = string.IsNullOrEmpty(value) ? string.Empty : value.ToUpper();
            }
        }      
        public string CourseName
        {
            get
            {
                return this.courseName;
            }
            set
            {
                this.courseName = string.IsNullOrEmpty(value) ? string.Empty : value.ToUpper();
            }
        }
        public string TrainerName
        {
            get
            {
                return this.trainerName;
            }
            set
            {
                this.trainerName = string.IsNullOrEmpty(value) ? string.Empty : value.ToUpper();
            }
        }
        public DateTime? StartDateFrom
        {
            get
            {
                return this.startDateFrom;
            }
            set
            {
                this.startDateFrom = value == null ? DateTime.MinValue : value;
            }
        }
        public DateTime? StartDateTo
        {
            get
            {
                return this.startDateTo;
            }
            set
            {
                this.startDateTo = value == null ? DateTime.MaxValue : value.Value.AddDays(1);
            }
        }
        public DateTime? EndDateFrom
        {
            get
            {
                return this.endDateFrom;
            }
            set
            {
                this.endDateFrom = value == null ? DateTime.MinValue : value;
            }
        }
        public DateTime? EndDateTo
        {
            get
            {
                return this.endDateTo;
            }
            set
            {
                this.endDateTo = value == null ? DateTime.MaxValue : value.Value.AddDays(1);
            }
        }
    }
}
