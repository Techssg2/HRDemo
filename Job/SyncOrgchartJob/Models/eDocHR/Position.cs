using System;

namespace SyncOrgchartJob.Service
{
    public class Position
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public Guid JobGradeId { get; set; }
    }
}