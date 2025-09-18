using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Aeon.HR.Data.Models
{
    public class ImportTracking : SoftDeleteEntity
    {
        public string Module { get; set; }
        public string FileName { get; set; }
        public string Documents { get; set; }
        public string JsonDataStr { get; set; }
        public string Status { get; set; }
        public bool IsImportManual { get; set; }
    }
}
