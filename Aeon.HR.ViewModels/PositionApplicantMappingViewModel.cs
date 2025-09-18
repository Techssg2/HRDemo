using Aeon.HR.Infrastructure.Enums;
using System;

namespace Aeon.HR.ViewModels
{
    public class PositionApplicantMappingViewModel
    {
        public Guid Id { get; set; }

        public Guid PositionId { get; set; }
        public string PositionPositionName { get; set; }
        public Guid ApplicantId { get; set; }
        public Guid? AppreciationId { get; set; }

        public Priority Priority { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
    }
}