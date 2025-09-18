using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aeon.Academy.Data.Entities
{
    public class TrainingRequest : BaseWorkflowEntity, IDataEntity
    {

        [StringLength(255)]
        [Required]
        public string SapNo { get; set; }
        [StringLength(255)]
        [Required]
        public string Affiliate { get; set; }
        [StringLength(255)]
        [Required]
        public string Position { get; set; }
        public virtual Guid? DepartmentId { get; set; }
        [StringLength(255)]
        public string DepartmentName { get; set; }
        public Guid? RegionId { get; set; }
        [StringLength(255)]
        public string RegionName { get; set; }
        public DateTime DateOfSubmission { get; set; }

        [StringLength(50)]
        [Required]
        public string TypeOfTraining { get; set; }
        [StringLength(50)]
        public string ExistingCourse { get; set; }
        public virtual Guid CategoryId { get; set; }
        public virtual Guid CourseId { get; set; }
        [StringLength(255)]
        public string CourseName { get; set; }
        [StringLength(255)]
        public string SupplierName { get; set; }
        public decimal TrainingFee { get; set; }
        [Required]
        public string ReasonOfTrainingRequest { get; set; }
        [StringLength(50)]
        [Required]
        public string TrainingMethod { get; set; }
        [Required]
        public int EstimatedTrainingDays { get; set; }
        public int? EstimatedTrainingHours { get; set; }

        public string DepartmentInCharge { get; set; }
        public int? Year { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string F2ReferenceNumber { get; set; }
        public string F2URL { get; set; }

        public bool ApplySponsorship { get; set; }

        public int SponsorshipPercentage { get; set; }
        public decimal? ActualTuitionReimbursementAmount { get; set; }

        [Required]
        public bool WorkingCommitment { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? WorkingCommitmentFrom { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? WorkingCommitmentTo { get; set; }

        [StringLength(255)]
        public string SponsorshipContractNumber { get; set; }
        public decimal CompensateAmount { get; set; }

        public string Purpose { get; set; }
        public string Reason { get; set; }
        public string HowApply { get; set; }
        public string WhenExcute { get; set; }
        public string WhatExpectedKPI { get; set; }
        public string RequestedDepartment { get; set; }
        public string RequestedDepartmentCode { get; set; }
        public Guid? RequestedDepartmentId { get; set; }
        public string DepartmentInChargeCode { get; set; }
        public Guid? DepartmentInChargeId { get; set; }
        public string DicDepartmentCode {  get; set; }
        public string SupplierCode { get; set; }
        public string MethodOfChoosingContractor { get; set; }
        public string TheProposalFor { get; set; }
        public string Reference { get; set; }
        public string Currency { get; set; }
        public string CurrencyJson { get; set; }

        public virtual Category Category { get; set; }
        public virtual Course Course { get; set; }
        public decimal TrainingNotTotalFee { get; set; }
        public decimal AccommodationIfAny { get; set; }
        public decimal AirTicketFeeIfAny { get; set; }
        public virtual IList<TrainingDurationItem> TrainingDurationItems { get; set; }
        public virtual IList<TrainingRequestParticipant> TrainingRequestParticipants { get; set; }
        public virtual IList<TrainingRequestHistory> TrainingRequestHistories { get; set; }
        public virtual IList<TrainingRequestCostCenter> CostCenters { get; set; }
    }
}
