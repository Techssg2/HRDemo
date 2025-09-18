using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aeon.Academy.API.DTOs
{
    public class TrainingReportDto
    {
        public TrainingReportDto()
        {
            EnabledActions = new List<string>();
            TrainingReportAttachments = new TrainingReportAttachmentDto();
            TrainingActionPlans = new List<TrainingActionPlanDto>();
            TrainingSurveyQuestions = new List<TrainingSurveyQuestionDto>(); 
        }
        public Guid Id { get; set; }
        public EmployeeInfoDto EmployeeInfo { get; set; }
        public Guid TrainingInvitationId { get; set; }        
        public string CourseName { get; set; }
        public string TrainerName { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? ActualAttendingDate { get; set; }
        public string OtherReasons { get; set; }
        public string OtherFeedback { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }

        public List<string> EnabledActions { get; set; }

        [StringLength(255)]
        public string ReferenceNumber { get; set; }
        public string RealStatus { get; set; }
        public string SupplierName { get; set; }

        public TrainingReportAttachmentDto TrainingReportAttachments { get; set; }
        public List<TrainingActionPlanDto> TrainingActionPlans { get; set; }
        public List<TrainingSurveyQuestionDto> TrainingSurveyQuestions { get; set; }
    }
}