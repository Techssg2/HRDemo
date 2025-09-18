using Aeon.HR.Infrastructure.Abstracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aeon.HR.Data.Models
{
    public class AttachmentFile : AuditableEntity
    {
        [StringLength(250)]
        public string Name { get; set; } // Ex: "MyFile"
        [StringLength(10)]
        public string Extension { get; set; } // Ex: ".docx"
        [StringLength(250)]
        public string FileDisplayName { get; set; } // Ex: "MyFile.docx"
        [StringLength(250)]
        public string FileUniqueName { get; set; } // Ex: "MyFile_13a2e2a8-b491-4edb-999d-560437648065.docx"
        public long Size { get; set; } // Byte - to keep same as FileInfo.Length    
        [StringLength(500)]
        public string Description { get; set; }
        public string ServerRelativeUrl { get; set; }  // lay file tren server
        public string Type { get; set; }
        public Guid? PromoteAndTransferId { get; set; }
        public Guid? RequestToHireId { get; set; }
        public Guid? LeaveApplicationId { get; set; }
        public virtual PromoteAndTransfer PromoteAndTransfer { get; set; }
        public virtual RequestToHire RequestToHire { get; set; }
        public virtual LeaveApplication LeaveApplication { get; set; }
    }
}