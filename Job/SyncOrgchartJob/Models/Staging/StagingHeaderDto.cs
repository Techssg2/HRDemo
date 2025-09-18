using System;

namespace SyncOrgchartJob.Models
{
    public class StagingHeaderDto
    {
        public int Id { get; set; }
        public string SyncType { get; set; }
        public DateTime SyncTime { get; set; }
        public string Status { get; set; }
        public string JsonAllData { get; set; }
    }
}