using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class ProvinceMasterDataViewModel
    {
        [JsonProperty("Zprovince")]
        public string ProvinceCode { get; set; }
        [JsonProperty("ZprovinceT")]
        public string ProvinceName { get; set; }
        [JsonProperty("ZdistrictT")]
        public string DistrictName { get; set; }
        [JsonProperty("Zdistrict")]
        public string DistrictCode { get; set; }
        [JsonProperty("ZwardsT")]
        public string WardName { get; set; }
        [JsonProperty("Zwards")]
        public string WardCode { get; set; }
    }
}
