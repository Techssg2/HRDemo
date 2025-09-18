using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.Academy.Common.Entities
{
    public class TrainingBudgetBalanceReportFilter
    {

        private Guid requestForDeptId;
        private Guid requestedDeptId;
        private string courseName;
        private string supplierName;
        private DateTimeOffset? requestedDateFrom = null;
        private DateTimeOffset? requestedDateTo = null;
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public Guid RequestForDeptId
        {
            get
            {
                return this.requestForDeptId;
            }
            set
            {
                this.requestForDeptId = value == null ? Guid.Empty : value;
            }
        }
        public Guid RequestedDeptId
        {
            get
            {
                return this.requestedDeptId;
            }
            set
            {
                this.requestedDeptId = value == null ? Guid.Empty : value;
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
        public string SupplierName
        {
            get
            {
                return this.supplierName;
            }
            set
            {
                this.supplierName = string.IsNullOrEmpty(value) ? string.Empty : value.ToUpper();
            }
        }
        public DateTimeOffset? RequestedDateFrom
        {
            get
            {
                return this.requestedDateFrom;
            }
            set
            {
                this.requestedDateFrom = value == null ? DateTimeOffset.MinValue : value;
            }
        }
        public DateTimeOffset? RequestedDateTo
        {
            get
            {
                return this.requestedDateTo;
            }
            set
            {
                this.requestedDateTo = value == null ? DateTimeOffset.MaxValue : value.Value.AddDays(1);
            }
        }
    }
}
