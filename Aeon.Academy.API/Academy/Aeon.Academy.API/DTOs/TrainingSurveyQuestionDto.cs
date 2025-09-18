using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Aeon.Academy.Common.Consts;

namespace Aeon.Academy.API.DTOs
{
    public class TrainingSurveyQuestionDto
    {
        public Guid Id { get; set; }

        public Guid TrainingReportId { get; set; }

        [StringLength(255)]
        public string SurveyQuestion { get; set; }

        public string ParentQuestion { get; set; }
        public string Value { get; set; }
    }
}