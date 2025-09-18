using Aeon.HR.Infrastructure.Abstracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Aeon.HR.Data.Models
{
    public class WorkflowTemplate : AuditableEntity
    {
        private WorkflowData _data;
        [StringLength(255)]
        public string WorkflowName { get; set; }
        [StringLength(255)]
        public string ItemType { get; set; }
        public int Order { get; set; }
        public bool IsActivated { get; set; }
        public bool AllowMultipleFlow { get; set; }
        public string StartWorkflowButton { get; set; }
        public string DefaultCompletedStatus { get; set; }
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
                        if (_data != null)
                        {
                            if (_data.Steps != null && _data.Steps.Any())
                            {
                                _data.Steps = _data.Steps.OrderBy(x => x.StepNumber).ToList();
                            }
                        }

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
        public bool? HasTrackingLog { get; set; }
        public string OldWorkflowDataStr { get; set; }
    }
}