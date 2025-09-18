using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Data.Models
{
    public class MetadataType : IEntity
    {
        public Guid Id { get; set ; }
        public DateTimeOffset Created { get ; set ; }
        public DateTimeOffset Modified { get ; set ; }
        public string Name { get; set; }
        public string Value { get; set; }
        
    }
}
