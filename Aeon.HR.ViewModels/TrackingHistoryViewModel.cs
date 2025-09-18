using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Constants;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class TrackingHistoryViewModel
    {
        public Guid Id { get; set; }
        public string ItemType { get; set; }
        public string ItemName { get; set; }
        public string Type { get; set; }
        public Guid ItemID { get; set; }
        public string WorkflowDataStr { get; set; }
        public string ItemReferenceNumberOrCode { get; set; }
        public string DataStr { get; set; }
        public string ErrorLog { get; set; }
        public string Comment { get; set; }
        public string RoundNum { get; set; }
        public string Documents { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByFullName { get; set; }
        public DateTimeOffset Created { get; set; }
        // Parse Attachment
        public List<UpdateApprovalWorkflowViewModel.AttachmentDetail> TrackingHistoryAttachments
        {
            get
            {
                if (!string.IsNullOrEmpty(Documents))
                    return JsonConvert.DeserializeObject<List<UpdateApprovalWorkflowViewModel.AttachmentDetail>>(Documents);
                else return null;
            }
        }
        // Parse Attachment
        public WorkflowTemplateViewModel WorkflowData
        {
            get
            {
                if (!string.IsNullOrEmpty(ItemType) && !string.IsNullOrEmpty(DataStr) && ItemType.Equals(ItemTypeContants.Workflow))
                    return JsonConvert.DeserializeObject<WorkflowTemplateViewModel>(DataStr);
                else return null;
            }
        }
    }
}
