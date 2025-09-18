using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.Args
{
    public class ITSaveBTAArgs
    {
        public string ReferenceNumber { get; set; }
        public bool? IsRoundTrip { get; set; }
        public string RequestorNote { get; set; }
        public string RequestorNoteDetail { get; set; }
        public string BTADetails { get; set; }
    }
}
