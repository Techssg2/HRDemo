using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class MassResponseAPIViewModel
    {
        [JsonProperty("isSuccess")]
        public bool IsSuccess { get; set; }
        [JsonProperty("object")]
        public List<Data> Items { get; set; }
    }
    public class Data
    {
        public string id { get; set; }
        public string name { get; set; }
        public string value { get; set; }
        public bool isDisabled { get; set; }
        public string typeId { get; set; }
        public object parentItemId { get; set; }
        public string typeName { get; set; }
        public object parentItemName { get; set; }
        public object parentItemValue { get; set; }
    }
}
