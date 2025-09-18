using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels.Args
{
    public class MasterDataArgs
    {  
        public string Name { get; set; } // is name of master data api with SourceFrom = External or Value of MetadataType with Source From  = Internal
        public string ParentCode { get; set; }
        //public MasterDataFrom SourceFrom { get; set; }      
        //public int Top { get; set; } // Limit record
        //public string Filter { get; set; } // Predicate with SourceFrom = Internal
        //public int Skip { get; set; } // Skip number records or Page with SourceFrom = Internal
        //public string Select { get; set; } // Name of columns which want to get from External
        //public object[] PredicateParameters { get; set; } // PredicateParameters  with SourceFrom = Internal
    }
}