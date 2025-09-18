using System;

namespace SyncOrgchartJob.Models.eDocHR
{
    public class BusinessModelUnitMapping
    {
        public Guid Id { get; set; }
        public Guid BusinessModelId { get; set; }
        public string BusinessModelCode { get; set; }
        public string BusinessUnitCode { get; set; }
        public bool IsStore { get; set; }
    }
}