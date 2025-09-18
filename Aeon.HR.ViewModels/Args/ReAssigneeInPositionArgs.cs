using System;

namespace Aeon.HR.ViewModels.Args
{
    public class ReAssigneeInPositionArgs
    {
        public TrackingLogArgs TrackingLogArgs { get; set; }
        public Guid UserId { get; set; }
        public Guid PositionId { get; set; }
    }
}
