using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.Args
{
    public class ImportRequestToHireArg
    {
        public string Module { get; set; }
        public string FileName { get; set; }
        public Guid? AttachmentFileId { get; set; }
        public bool IsImportManual { get; set; }
    }
}
