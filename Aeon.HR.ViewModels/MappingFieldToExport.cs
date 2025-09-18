using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class MappingFieldToExport
    {
        public string Name { get; set; } // Property Name of ViewModel
        public string DisplayName { get; set; } // Name to show on header and get value form ViewModel
        public bool Visible { get; set; } // Config to show or hidden on Excel
        public FieldType Type { get; set; } // Type of Value

    }
}
