using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.DTOs
{
    public class ResponseExternalDataMappingDTO
    { 
        public string Url { get; set; }
        public string Payload { get; set; }
        public string Response { get; set; }
        public string Status { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
        public string HttpStatusCode { get; set; }
        public ActionExposeAPI Action { get; set; }
        public Guid? ItemId { get; set; }
        public AdditionalItem AdditionalItem { get; set; }

    }
    public class AdditionalItem
    {
        public AdditionalItem()
        {

        }
        public AdditionalItem(string _deptName, string _depCode, string _divisionCode, string _divisionName, string _referenceNumber, string _userName)
        {
            DeptName = _deptName;
            DeptCode = _depCode;
            DivisionCode = _divisionCode;
            DivisionName = _divisionName;
            ReferenceNumber = _referenceNumber;
            UserName = _userName;

        }
        public string DeptName { get; set; }
        public string DeptCode { get; set; }
        public string DivisionCode { get; set; }
        public string DivisionName { get; set; }
        public string ReferenceNumber { get; set; }
        public string UserName { get; set; }
    }
}
