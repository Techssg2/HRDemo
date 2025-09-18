using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels.Args
{
    public class ApplicantSearchArgs
    {
        public QueryArgs query { get; set; }
        public string sapCode { get; set; }
        public bool getAll { get; set; }
    }
}