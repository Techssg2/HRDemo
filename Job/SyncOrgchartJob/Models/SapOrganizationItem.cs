using System.Collections.Generic;

namespace SyncOrgchartJob.Models
{
    public class SapOrganizationItem
    {
        public string ObjectName { get; set; }
        public string ObjectId { get; set; }
        public string ObjectType { get; set; }
        public string ParentId { get; set; }
        public string ParentName { get; set; }
        public string IsHead { get; set; }
        public string Level { get; set; }
        public string EmployeeJg { get; set; }
        public string OrgJobgrade { get; set; }
        public string PersonalArea { get; set; }     // <-- nên đổi sang string luôn
        public string SubArea { get; set; }          // <-- chính là dòng gây lỗi
        public string ValidFrom { get; set; }
        public string ValidTo { get; set; }
        public string EmployeeId { get; set; }
        public string AbbrName { get; set; }
        public string Concurrently { get; set; }
        public string ModifyDate { get; set; }
    }
    public class SapResponseWrapper
    {
        public SapResults d { get; set; }
    }

    public class SapResults
    {
        public List<SapOrganizationItem> results { get; set; }
    }
}