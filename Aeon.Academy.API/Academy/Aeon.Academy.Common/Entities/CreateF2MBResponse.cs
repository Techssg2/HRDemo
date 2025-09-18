using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Aeon.Academy.Common.Entities
{
    public class CreateF2MBResponse
    {
        public CreateF2MBModel Data  { get; set; }
        public object Errors { get; set; }
    }    
    public class CreateF2MBModel
    {
        public string ReferenceNumber  { get; set; }
        public string URL { get; set; }
    }
}
