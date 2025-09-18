using Aeon.HR.Infrastructure.Enums;
using System;

namespace Aeon.HR.ViewModels.Args
{
    public class PositionStatusArgs
    {
        public PositionStatusArgs()
        {
        }
        public Guid PositionId { get; set; }
        public PositionStatus Status { get; set; }
    }
}