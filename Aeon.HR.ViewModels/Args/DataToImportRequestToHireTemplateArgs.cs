using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels.Args
{
    public class DataToImportRequestToHireTemplateArgs
    {
        public bool IsImportManual { get; set; }
        public string Url { get; set; }
        public string RootDirectory { get; set; }
        public string RootDirectoryLog { get; set; }
        public string RootDirectoryReceive { get; set; }
    }
}