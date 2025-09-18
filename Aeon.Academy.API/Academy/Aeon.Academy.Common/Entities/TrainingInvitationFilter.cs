using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.Academy.Common.Entities
{
    public class TrainingInvitationFilter
    {
        private string[] status;
        private string referenceNumber;
        private string sapCode;
        private string courseType;
        private DateTimeOffset? createDateFrom = null;
        private DateTimeOffset? createDateTo = null;

        public Guid? UserId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string[] Status
        {
            get
            {
                return this.status;
            }
            set
            {
                this.status = value == null || value.Length == 0 ? new string[] { } : value;
            }
        }
        public string ReferenceNumber
        {
            get
            {
                return this.referenceNumber;
            }
            set
            {
                this.referenceNumber = string.IsNullOrEmpty(value) ? string.Empty : value.ToUpper();
            }
        }
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
        public string CourseType
        {
            get
            {
                return this.courseType;
            }
            set
            {
                this.courseType = string.IsNullOrEmpty(value) ? string.Empty : value.ToUpper();
            }
        }
        public DateTimeOffset? CreateDateFrom
        {
            get
            {
                return this.createDateFrom;
            }
            set
            {
                this.createDateFrom = value == null ? DateTimeOffset.MinValue : value;
            }
        }
        public DateTimeOffset? CreateDateTo
        {
            get
            {
                return this.createDateTo;
            }
            set
            {
                this.createDateTo = value == null ? DateTimeOffset.MaxValue : value.Value.AddDays(1);
            }
        }
    }
}
