using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using static Aeon.HR.Infrastructure.Constants.JobgradeConst;

namespace Aeon.HR.Data.Models
{
    public class JobGrade : SoftDeleteEntity
    {
        public JobGrade()
        {
            // JobGradeItems = new HashSet<ItemListRecruitment>();
            Departments = new HashSet<Department>();
        }
        public int Grade { get; set; }
        public string Caption { get; set; }
        public string Title { get; set; }
        [Range(0, int.MaxValue)]
        public int ExpiredDayPosition { get; set; }
        public DepartmentType DepartmentType { get; set; }
        public double? MaxWFH { get; set; }
        public double? MaxPRDERD { get; set; }
        public StorePositionType StorePosition { get; set; }
        public HQPositionType HQPosition { get; set; }
        public virtual ICollection<HeadCount> HeadCounts { get; set; }
        public virtual ICollection<Department> Departments { get; set; }
        public virtual ICollection<JobGradeItemRecruitmentMapping> JobGradeItemRecruitmentMappings { get; set; }
        public virtual ICollection<RequestToHire> RequestToHires { get; set; }
        public virtual ICollection<BusinessTripOverBudgetsDetail> BusinessTripOverBudgetsDetails { get; set; }
        public virtual ICollection<BusinessTripApplicationDetail> BusinessTripApplicationDetails { get; set; }
    }
}