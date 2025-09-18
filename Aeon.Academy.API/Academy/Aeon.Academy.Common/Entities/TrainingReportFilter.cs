using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.Academy.Common.Entities
{
    public class TrainingReportFilter
    {
        private string[] status = new string[0];
        private string sapCode = "";
        private string createdBy = "";
        private string courseName = "";
        private int pageNumber = 1;
        private int pageSize = 10;
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
        public string CreatedBy
        {
            get
            {
                return this.createdBy;
            }
            set
            {
                this.createdBy = string.IsNullOrEmpty(value) ? string.Empty : value.ToUpper();
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
                this.sapCode = string.IsNullOrEmpty(value) ? string.Empty : value;
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
    }
}
