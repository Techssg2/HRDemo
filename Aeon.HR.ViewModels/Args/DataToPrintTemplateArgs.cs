using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels.Args
{
    public class DataToPrintTemplateArgs
    {
        public List<string> ListSAPCodes { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid? DivisionId { get; set; }
        public string PeriodFromDate { get; set; }
    }
}