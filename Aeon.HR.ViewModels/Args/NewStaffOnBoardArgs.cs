using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels.Args
{
    public class NewStaffOnBoardArgs
    {
        public NewStaffOnBoardArgs() 
        {
            StatusCode = "AppStatus3";
        }

        public QueryArgs QueryArgs { get; set; }
        public string StatusCode { get; set; }
    }
}