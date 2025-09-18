using System;

namespace Aeon.HR.ViewModels.Args
{
    public class PositionDetailArgs
    {
        public PositionDetailArgs()
        {
        }

        public string Position { get; set; }
        public string Status { get; set; }
        public DateTimeOffset CreatedDateFrom { get; set; }
        public DateTimeOffset CreatedDateTo { get; set; }
    }
}