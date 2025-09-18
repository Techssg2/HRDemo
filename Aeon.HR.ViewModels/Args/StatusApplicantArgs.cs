using System;

namespace Aeon.HR.ViewModels.Args
{
    public class StatusApplicantArgs
    {
        public StatusApplicantArgs() { }

        public QueryArgs QueryArgs { get; set; }
        public string StatusName { get; set; }
    }
}