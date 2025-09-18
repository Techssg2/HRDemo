using Aeon.HR.Infrastructure.Enums;
namespace Aeon.HR.API.ExternalItem
{
    public class FileMapping
    {
        public string SourceField { get; set; }
        public string TargetField { get; set; }
        public FieldType Type { get; set; }
       
    }
}