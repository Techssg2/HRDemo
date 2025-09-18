using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.ViewModels.BTA;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels
{
    public class JobGradeViewModel
    {
        public Guid Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Caption { get; set; }
        public int Grade { get; set; }
        [Range(0, int.MaxValue)]
        public int ExpiredDayPosition { get; set; }
        public DepartmentType? DepartmentType { get; set; }
        public string _ExportDepartmentType
        {
            get
            {
                return (DepartmentType == null || DepartmentType == 0) ? "" : DepartmentType.ToString();
            }
        }
        [Range(0, double.MaxValue, ErrorMessage = "Max PRD + ERD must be greater than or equal to 0")]
        public double? MaxWFH { get; set; } 
        public StorePositionType? StorePosition { get; set; } = null;
        public HQPositionType? HQPosition { get; set; }
        public string _ExportStorePosition { 
            get
            {
                return (StorePosition == null || StorePosition == 0) ? "" : StorePosition.ToString();
            }
        }
        public string _ExportHQPosition
        {
            get
            {
                return (HQPosition == null || HQPosition == 0) ? "" : HQPosition.ToString();
            }
        }
        //public ICollection<ItemListRecruitmentViewModel> JobGradeItems { get; set; }
        public BTAPolicyViewModel BTAPolicy { get; set; }
    }
}