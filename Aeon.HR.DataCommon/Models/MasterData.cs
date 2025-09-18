using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Enums;

namespace Aeon.HR.Data.Models
{
    public class MasterData: SoftDeleteEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public MasterDataFrom SourceFrom { get; set; }
        public Guid ? MetaDataTypeId { get; set; }
        public virtual MetadataType MetadataType { get; set; }
        public Guid? JobGradeId { get; set; }
        public virtual JobGrade JobGrade { get; set; }
    }
}