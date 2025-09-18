using System;
using System.Collections.Generic;

namespace Aeon.Academy.API.DTOs
{
    public class TrainingRequestDto
    {
        public TrainingRequestDto()
        {
            EnabledActions = new List<string>();
            Requester = new RequesterDto();
            TrainingDetails = new TrainingDetailsDto();
            TrainingDuration = new TrainingDurationDto();
            ParticipantBudget = new ParticipantBudgetDto();
            TrainingSponsorshipContract = new TrainingSponsorshipContractDto();
            WorkingCommitment = new WorkingCommitmentDto();
            CostCenters = new List<CostCenterDto>();
        }
        public Guid Id { get; set; }
        public string ReferenceNumber { get; set; }
        public string Status { get; set; }
        public List<string> EnabledActions { get; set; }
        public RequesterDto Requester { get; set; }
        public TrainingDetailsDto TrainingDetails { get; set; }
        public TrainingDurationDto TrainingDuration { get; set; }        
        public ParticipantBudgetDto ParticipantBudget { get; set; }        
        public TrainingSponsorshipContractDto TrainingSponsorshipContract { get; set; }
        public WorkingCommitmentDto WorkingCommitment { get; set; }
        public string Purpose { get; set; }
        public string Reason { get; set; }
        public string HowApply { get; set; }
        public string WhenExcute { get; set; }
        public string WhatExpectedKPI { get; set; }
        public string RealStatus { get; set; }
        public string F2ReferenceNumber { get; set; }
        public string F2Url { get; set; }
        public List<CostCenterDto> CostCenters { get; set; }
    }
}