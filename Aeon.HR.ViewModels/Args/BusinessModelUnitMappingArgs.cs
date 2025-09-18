using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels.Args
{
    public class BusinessModelUnitMappingArgs
    {
        public Guid? Id { get; set; }
        public Guid? BusinessModelId { get; set; }
        public string BusinessModelCode { get; set; }
        public string BusinessUnitCode { get; set; }
        public bool? IsStore { get; set; }
    }
}