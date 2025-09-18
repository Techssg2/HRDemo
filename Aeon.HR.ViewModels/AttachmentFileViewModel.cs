using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class AttachmentFileViewModel
    {
        public Guid Id { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string AppService { get; set; }
        public string Type { get; set; }
        public string Name { get; set; } // Ex: "MyFile"
        public string Extension { get; set; } // Ex: ".docx"
        public string FileDisplayName { get; set; } // Ex: "MyFile.docx"
        public string FileUniqueName { get; set; } // Ex: "MyFile_13a2e2a8-b491-4edb-999d-560437648065.docx"
        public long Size { get; set; } // KB
        public byte[] Base64ImageValue { get; set; }
        public string Description { get; set; }
    }
}
