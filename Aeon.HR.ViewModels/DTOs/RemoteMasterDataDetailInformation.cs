using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.DTOs
{
    public class RemoteMasterDataDetailInformation
    {
        public string Name { get; set; }
        public string Filter { get; set; }
        public string ApiName { get; set; }
        public bool HasParentCode { get; set; }
        public RemoteMasterDataDetailInformation(string name, string value, string apiName, bool hasParentCode)
        {
            Name = name;
            Filter = value; 
            ApiName = apiName;
            HasParentCode = hasParentCode;
        }
    }
}
