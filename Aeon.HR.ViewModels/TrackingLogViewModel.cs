using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels
{
    public class TrackingLogViewModel
    {
        public Guid Id { get; set; }
        public string OldAssignee { get; set; }
        public string NewAssignee { get; set; }
        public Guid? ItemId { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
    }
    public class TrackingLogInitDatasViewModel
    {
        public Guid Id { get; set; }
        public Guid? TrackingLogId { get; set; }
        public string Code { get; set; }
        public string ReferenceNumber { get; set; }
        public string FunctionType { get; set; }
        public string SAPCode { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
        public virtual TrackingRequestForGetListViewModel TrackingRequest { get; set; }
    }
    public class TrackingPayLoad
    {
        public string RefNum { get; set; }
        public string EdocUser { get; set; }
    }
}
