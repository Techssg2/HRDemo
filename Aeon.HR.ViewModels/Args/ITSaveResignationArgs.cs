using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.Args
{
    public class ITSaveResignationArgs
    {
        public bool IsUpdatePayload { get; set; }
        public string ReferenceNumber { get; set; }
        public DateTimeOffset? StartingDate { get; set; }
        public DateTimeOffset? OfficialResignationDate { get; set; }
        public DateTimeOffset ? SuggestionForLastWorkingDay { get; set; }
    }
}
