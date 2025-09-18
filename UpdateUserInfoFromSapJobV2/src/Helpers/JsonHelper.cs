using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UpdateUserInfoFromSapJobV2.src.SAP
{
    public static class JsonHelper
    {
        public static string GetJsonContentFromFile(string folderName, string jsonFileName)
        {
            var result = "";
            string codeBase = Assembly.GetCallingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            string unescapedRepresentation = Uri.UnescapeDataString(uri.Path);
            using (StreamReader r = new StreamReader(Path.Combine(Path.GetDirectoryName(unescapedRepresentation), folderName, jsonFileName)))
            {
                result = r.ReadToEnd();
            };
            return result;
        }
        public static string GetPropertyValue(this string jsonData, string propertyName)
        {
            var result = "";
            return result;
        }
        public static string GetGroupDataByName(this string jsonData, string groupName)
        {
            var result = "";
            JObject group = JObject.Parse(jsonData);
            if (group != null)
            {
                result = group.GetValue(groupName).ToString();
            }
            return result;
        }
        public static string GetArrayData(this string jsonData, string arrayName)
        {
            var result = "";
            return result;
        }

        public static string SafeReplace(this string input, string find, string replace, bool matchWholeWord)
        {
            string textToFind = matchWholeWord ? string.Format(@"\b{0}\b", find) : find;
            return Regex.Replace(input, textToFind, replace);
        }
    }
}
