using Aeon.HR.Infrastructure.Enums;
using System;

namespace Aeon.HR.ViewModels.Args
{
    public class PositionForCreatingArgs
    {
        public PositionForCreatingArgs()
        {
        }
        public Guid Id { get; set; }
        public string PositionName { get; set; }
        public Guid DeptDivisionId { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public int ExpiredDay { get; set; }
        public bool HasBudget { get; set; }
        public int Quantity { get; set; }
        public Guid AssignToId { get; set; }
        public string ReferenceNumber { get; set; }
        public Guid? ApplicantId { get; set; }
        public PositionStatus Status { get; set; }
    }
}