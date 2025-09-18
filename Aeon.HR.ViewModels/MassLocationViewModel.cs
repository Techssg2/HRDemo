using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class MassLocationViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public Guid TypeId { get; set; }
        public MasterDataFrom SourceFrom { get; set; } = MasterDataFrom.External;
        public string Description { get; set; }
    }
}
