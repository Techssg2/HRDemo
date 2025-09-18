using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Aeon.Academy.Common.Entities
{
    public class AcademyTrainingResponse
    {
        [JsonProperty("d")]
        public Data Data { get; set; }
    }
    public class Metadata
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("uri")]
        public string Uri { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
    [DataContract(Name = "d")]
    public class Data
    {
        [JsonProperty("__metadata")]
        public Metadata Metadata { get; set; }

        [JsonProperty("Pernr")]
        public string Pernr { get; set; }

        [JsonProperty("Begda")]
        public string Begda { get; set; }

        [JsonProperty("Endda")]
        public string Endda { get; set; }

        [JsonProperty("Ztrain_loc")]
        public string ZtrainLoc { get; set; }

        [JsonProperty("Zprg_code")]
        public string ZprgCode { get; set; }

        [JsonProperty("Zprogram")]
        public string Zprogram { get; set; }

        [JsonProperty("Zhours_day")]
        public string ZhoursDay { get; set; }

        [JsonProperty("Znumofday")]
        public string Znumofday { get; set; }

        [JsonProperty("Ztotal_hours")]
        public string ZtotalHours { get; set; }

        [JsonProperty("Zin_ex")]
        public string ZinEx { get; set; }

        [JsonProperty("Ztrainer")]
        public string Ztrainer { get; set; }

        [JsonProperty("Zagency")]
        public string Zagency { get; set; }

        [JsonProperty("Ztrain_cost")]
        public string ZtrainCost { get; set; }

        [JsonProperty("Ztrain_cont")]
        public string ZtrainCont { get; set; }

        [JsonProperty("Zstart")]
        public string Zstart { get; set; }

        [JsonProperty("Zend")]
        public string Zend { get; set; }

        [JsonProperty("Status")]
        public string Status { get; set; }

        [JsonProperty("Message")]
        public string Message { get; set; }
    }
}
