using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class TrackingRequestForGetListViewModel
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public string Payload { get; set; }
        public string Response { get; set; }
        public string ReferenceNumber { get; set; }
        public string UserName { get; set; }
        public string Status { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
        public string HttpStatusCode { get; set; }
        public ActionExposeAPI Action { get; set; }
        public string ActionDescription { get; set; }
        public bool? HasTrackingLog { get; set; }
    }
}
