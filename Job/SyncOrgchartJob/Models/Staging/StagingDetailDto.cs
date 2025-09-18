using System;
using System.Collections.Generic;

namespace SyncOrgchartJob.Models
{
    public class StagingDetailDto
    {
        public int Id { get; set; }
        public int HeaderId { get; set; }
        public string ObjectId { get; set; }
        public string ObjectName { get; set; }
        public string ObjectType { get; set; }
        public string ParentId { get; set; }
        public string ParentName { get; set; }
        public string IsHead { get; set; }
        public string Level { get; set; }
        public string EmployeeJg { get; set; }
        public string OrgJobgrade { get; set; }
        public string PersonalArea { get; set; } // Vì có thể là text như "A009"
        public string SubArea { get; set; }      // Cũng nên là string để tránh lỗi convert
        public string ValidFrom { get; set; }
        public string ValidTo { get; set; }
        public string EmployeeId { get; set; }
        public string AbbrName { get; set; }
        public string Concurrently { get; set; }
        public string ModifyDate { get; set; }
        public string JsonData { get; set; }
        public string CreatedAt { get; set; }
        public List<StagingDetailDto> Children { get; set; } = new List<StagingDetailDto>();
        public List<StagingDetailDto> Persons { get; set; } = new List<StagingDetailDto>();
        public List<StagingDetailDto> Positions { get; set; } = new List<StagingDetailDto>();
    }
}