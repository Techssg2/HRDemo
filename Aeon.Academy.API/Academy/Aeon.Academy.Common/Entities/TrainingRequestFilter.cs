using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.Academy.Common.Entities
{
    public class TrainingRequestFilter
    {
        private string[] status = new string[0];
        private string[] type = new string[0];
        private string courseName = "";
        private string keyword = "";
        private int pageNumber = 1;
        private int pageSize = 10;
        private DateTimeOffset? createDateFrom = DateTimeOffset.MinValue;
        private DateTimeOffset? createDateTo = DateTimeOffset.MaxValue;
        public Guid? UserId { get; set; }
        public int? PageNumber
        {
            get
            {
                return this.pageNumber;
            }
            set
            {
                this.pageNumber = value != null ? value.Value : 1;
            }
        }
        public int? PageSize
        {
            get
            {
                return this.pageSize;
            }
            set
            {
                this.pageSize = value != null ? value.Value : 10;
            }
        }
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
        public string[] Type
        {
            get
            {
                return this.type;
            }
            set
            {
                this.type = value == null || value.Length == 0 ? new string[] { } : value;
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
                this.courseName = string.IsNullOrEmpty(value) ? string.Empty : value.ToLower();
            }
        }
        public string Keyword
        {
            get
            {
                return this.keyword;
            }
            set
            {
                this.keyword = string.IsNullOrEmpty(value) ? string.Empty : value.ToLower();
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
