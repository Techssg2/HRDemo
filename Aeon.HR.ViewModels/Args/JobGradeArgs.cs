using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels.Args
{
    public class JobGradeArgs
    {
        public Guid Id { get; set; }
        public int Grade { get; set; }
        public double UpGrade { get; set; }
        public string Caption { get; set; }
        [Required]
        public string Title { get; set; }
        public int ExpiredDayPosition { get; set; }
        public DepartmentType DepartmentType { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Max PRD + ERD must be greater than or equal to 0")]
        public double? MaxWFH { get; set; }
        public StorePositionType StorePosition { get; set; }
        public HQPositionType HQPosition { get; set; }
    }
}