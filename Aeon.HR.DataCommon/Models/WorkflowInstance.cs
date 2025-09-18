using Aeon.HR.Infrastructure.Abstracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aeon.HR.Data.Models
{
    public class WorkflowInstance : Entity
    {
        private WorkflowData _data;
        public string WorkflowName { get; set; }
        public string DefaultCompletedStatus { get; set; }
        public Guid TemplateId { get; set; }
        [NotMapped]
        public WorkflowData WorkflowData
        {
            get
            {

                if (_data == null)
                {
                    if (!string.IsNullOrEmpty(WorkflowDataStr))
                    {
                        _data = JsonConvert.DeserializeObject<WorkflowData>(WorkflowDataStr);
                    }
                    else
                    {
                        _data = null;
                    }
                }
                return _data;
            }
            set
            {
                WorkflowDataStr = JsonConvert.SerializeObject(value);
            }
        }

        public string WorkflowDataStr { get; set; }
        public string OldWorkflowDataStr { get; set; }
        [Index]
        public Guid ItemId { get; set; }
        [Index]
        [StringLength(255)]
        public string ItemReferenceNumber { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsTerminated { get; set; }
        public bool IsITUpdate { get; set; } = false;
        public virtual ICollection<WorkflowHistory> Histories { get; set; }
    }
}