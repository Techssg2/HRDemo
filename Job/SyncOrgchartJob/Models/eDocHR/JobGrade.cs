using System;
using SyncOrgchartJob.Enums;

namespace SyncOrgchartJob.Models.eDocHR
{
    public class JobGrade
    {
        public Guid Id { get; set; }
        public int Grade { get; set; }
        public string Caption { get; set; }
        public string Title { get; set; }
        public int ExpiredDayPosition { get; set; }
        public DepartmentType DepartmentType { get; set; }
        public double? MaxWFH { get; set; }
    }
}