using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.Academy.Common.Entities
{
    public class TrainingTrackerReportFilter
    {
        private int pageNumber = 1;
        private int pageSize = 20;

        private string sapCode = "";
        private string fullName = "";
        private string courseName = "";
        private string supplierName = "";
        private string[] typeofTraining = new string [0];
        private DateTime? startTo = null;
        private DateTime? endTo = null;
        public string SapCode
        {
            get
            {
                return this.sapCode;
            }
            set
            {
                this.sapCode = string.IsNullOrEmpty(value) ? string.Empty : value.ToLower();
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
                this.fullName = string.IsNullOrEmpty(value) ? string.Empty : value.ToLower();
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
        public string SupplierName
        {
            get
            {
                return this.supplierName;
            }
            set
            {
                this.supplierName = string.IsNullOrEmpty(value) ? string.Empty : value.ToLower();
            }
        }
        public string[] TypeofTraining
        {
            get
            {
                return this.typeofTraining;
            }
            set
            {
                this.typeofTraining = value == null || value.Length == 0 ? new string[] { } : value;
            }
        }
        public DateTime? StartingDateFrom { get; set; }
        public DateTime? StartingDateTo
        {
            get
            {
                return this.startTo;
            }
            set
            {
                if (value != null)
                {
                    this.startTo = value.Value.AddDays(1);
                }
            }
        }
        public DateTime? EndingDateFrom { get; set; }
        public DateTime? EndingDateTo
        {
            get
            {
                return this.endTo;
            }
            set
            {
                if (value != null)
                {
                    this.endTo = value.Value.AddDays(1);
                }
            }
        }
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
                this.pageSize = value != null ? value.Value : 20;
            }
        }
    }
}
