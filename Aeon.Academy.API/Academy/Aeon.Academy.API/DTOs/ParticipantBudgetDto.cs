using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aeon.Academy.API.DTOs
{
    public class ParticipantBudgetDto
    {
        public ParticipantBudgetDto()
        {
            Participants = new List<ParticipantDto>();
        }
        public string DepartmentInCharge { get; set; }
        [Range(0, 9999, ErrorMessage = "Year must be between 0 to 9999")]
        public int? Year { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string RequestedDepartment { get; set; }
        public string RequestedDepartmentCode { get; set; }
        public Guid? RequestedDepartmentId { get; set; }
        public string DepartmentInChargeCode { get; set; }
        public Guid? DepartmentInChargeId { get; set; }
        public string MethodOfChoosingContractor { get; set; }
        public string TheProposalFor { get; set; }
        [StringLength(255)]
        public string Reference { get; set; }

        public List<ParticipantDto> Participants { get; set; }
        public string DicDepartmentCode { get; set; }
    }
}