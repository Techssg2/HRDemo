using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.ExternalHelper.SAP
{
    public class SAPSerializerSettings
    {
        public static CustomJsonSerializerSettings BuildJsonSerializerSettings<T>(List<FieldMappingDTO> mappingFields)
        {
            CustomJsonSerializerSettings result = new CustomJsonSerializerSettings();
            var jsonResolver = new PropertyRenameSerializerContractResolver(); 
            foreach(var item in mappingFields)
            {
                jsonResolver.RenameProperty(typeof(T), item.SourceField, item.TargetField);
            }           
            result.SynceData = result.SynceData == null ? GetResultType<T>() : result.SynceData;
            result.Setting = new JsonSerializerSettings { ContractResolver = jsonResolver };
            return result;
        }
        private static List<T> GetResultType<T>()
        {
            return new List<T>();
        }
    }
    public class DataMappingDTO
    {
        public DataMappingDTO(string name, List<FieldMappingDTO> fieldMappings)
        {
            Name = name;
            FielMappings = fieldMappings;
        }
        public string Name { get; set; }
        public List<FieldMappingDTO> FielMappings { get; set; }
    }
}
