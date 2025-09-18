using Aeon.HR.API.Helpers;
using Aeon.HR.BusinessObjects.ExternalHelper.SAP;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.DTOs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Helpers
{
    public class SAPJsonConverterHelper
    {
        /// <summary>
        /// Convert 1 instance Master data 
        /// </summary>
        /// <typeparam name="T">Kiểu muốn convert</typeparam>
        /// <param masterDataType="masterDataType">Tên Master Data muốn lấy</param>
        /// <param name="httpResponseResult">Instance muốn đổi convert</param>
        /// <returns></returns>
        public static IEnumerable<object> ConvertFromObjectToMasterData<T>(string groupName, string httpResponseResult, string folderName, string fileName)
        {
            IEnumerable<object> syncContact = null;
            if (string.IsNullOrEmpty(httpResponseResult))
            {
                return syncContact;
            }
            var jsonFileContent = JsonHelper.GetJsonContentFromFile(folderName, fileName);
            if (!string.IsNullOrEmpty(jsonFileContent))
            {
                var groupData = jsonFileContent.GetGroupDataByName(groupName);
                if (!string.IsNullOrEmpty(groupData))
                {
                    var setting = SAPSerializerSettings.BuildJsonSerializerSettings<T>(JsonConvert.DeserializeObject<List<FieldMappingDTO>>(groupData));
                    if (setting != null)
                    {
                        syncContact = setting.SynceData as IEnumerable<object>;
                        var resultArray = JsonConvert.DeserializeObject<SAPAPIResultForArray>(httpResponseResult);
                        if (resultArray != null && resultArray.D != null)
                        {
                            var result = JsonConvert.SerializeObject(resultArray.D.Results);
                            JsonConvert.PopulateObject(result, syncContact, setting.Setting);
                        }
                        if (typeof(T).IsAssignableFrom(typeof(MasterDataViewModel)))
                        {
                            for (int i = 0; i < resultArray.D.Results.Count; i++)
                            {                               
                                ((MasterDataViewModel)syncContact.ElementAt(i)).RawData = resultArray.D.Results[i];
                            }
                        }
                    }

                }
            }
            
            return syncContact;
        }
    }

}
