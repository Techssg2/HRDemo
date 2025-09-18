using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels
{
    public class ItemListOvertimeApplicationViewModel:CBUserInfoViewModel
    {
        public Guid Id { get; set; }
        public DateTimeOffset Created { get; set; }
        public string Status { get; set; }
        public string ReferenceNumber { get; set; }  
    }
}