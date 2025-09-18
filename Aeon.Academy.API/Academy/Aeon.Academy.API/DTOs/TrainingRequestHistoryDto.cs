using System;

namespace Aeon.Academy.API.DTOs
{
    public class TrainingRequestHistoryDto
    {
        public Guid Id { get; set; }
        public Guid TrainingRequestId { get; set; }
        public DateTimeOffset? Created { get; set; }
        public Guid CreatedById { get; set; }
        public string CreatebBy { get; set; }
        public string CreatedByFullName { get; set; }
        public string ReferenceNumber { get; set; }
        public string Comment { get; set; }
        public string Action { get; set; }
        public int? StepNumber { get; set; }
        public string AssignedToDepartmentName { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? DueDate { get; set; }
        public int? RoundNumber { get; set; }
    }
}