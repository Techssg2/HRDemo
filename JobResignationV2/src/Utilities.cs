using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobResignationV2.src
{
    public class Utilities
    {
        public static void WriteLogError(Exception ex)
        {
            try
            {
                string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                string logFilePath = Path.Combine(logDirectory, "LogFile.txt");
                using (StreamWriter sw = new StreamWriter(logFilePath, true))
                {
                    sw.WriteLine(DateTime.Now.ToString("g") + ": " + ex.Source + "; " + ex.Message);
                }
            }
            catch
            {
               
            }
        }

        public static void WriteLogError(string message)
        {
            try
            {
                string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                string logFilePath = Path.Combine(logDirectory, "LogFile.txt");
                using (StreamWriter sw = new StreamWriter(logFilePath, true))
                {
                    sw.WriteLine(DateTime.Now.ToString("g") + ": " + message);
                }
            }
            catch
            {
               
            }
        }


        public static void AddProperty(ExpandoObject expando, string propertyName, object propertyValue)
        {
            // ExpandoObject supports IDictionary so we can extend it like this
            var expandoDict = expando as IDictionary<string, object>;
            if (expandoDict.ContainsKey(propertyName))
                expandoDict[propertyName] = propertyValue;
            else
                expandoDict.Add(propertyName, propertyValue);
        }

        //public class SAPJsonConverterHelper
        //{
        //    /// <summary>
        //    /// Convert 1 instance Master data 
        //    /// </summary>
        //    /// <typeparam name="T">Kiểu muốn convert</typeparam>
        //    /// <param masterDataType="masterDataType">Tên Master Data muốn lấy</param>
        //    /// <param name="httpResponseResult">Instance muốn đổi convert</param>
        //    /// <returns></returns>
        //    public static IEnumerable<object> ConvertFromObjectToMasterData<T>(string groupName, string httpResponseResult, string folderName, string fileName)
        //    {
        //        IEnumerable<object> syncContact = null;
        //        if (string.IsNullOrEmpty(httpResponseResult))
        //        {
        //            return syncContact;
        //        }
        //        var jsonFileContent = JsonHelper.GetJsonContentFromFile(folderName, fileName);
        //        if (!string.IsNullOrEmpty(jsonFileContent))
        //        {
        //            var groupData = jsonFileContent.GetGroupDataByName(groupName);
        //            if (!string.IsNullOrEmpty(groupData))
        //            {
        //                var setting = SAPSerializerSettings.BuildJsonSerializerSettings<T>(JsonConvert.DeserializeObject<List<UpdateUserInfoFromSapJobV2.src.ViewModel.FieldMappingDTO>>(groupData));
        //                if (setting != null)
        //                {
        //                    syncContact = setting.SynceData as IEnumerable<object>;
        //                    var resultArray = JsonConvert.DeserializeObject<SAPAPIResultForArray>(httpResponseResult);
        //                    if (resultArray != null && resultArray.D != null)
        //                    {
        //                        var result = JsonConvert.SerializeObject(resultArray.D.Results);
        //                        JsonConvert.PopulateObject(result, syncContact, setting.Setting);
        //                    }
        //                    if (typeof(T).IsAssignableFrom(typeof(MasterDataViewModel)))
        //                    {
        //                        for (int i = 0; i < resultArray.D.Results.Count; i++)
        //                        {
        //                            ((MasterDataViewModel)syncContact.ElementAt(i)).RawData = resultArray.D.Results[i];
        //                        }
        //                    }
        //                }

        //            }
        //        }

        //        return syncContact;
        //    }
        //}
    }
}
