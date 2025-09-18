using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateUserInfoFromSapJobV2.src.SAP
{
    public static class AppSettingsHelper
    {
        public static bool ApplyNewShiftExchange_Payload
        {
            get
            {
                bool returnValue = false;
                try
                {
                    string strValue = ConfigurationManager.AppSettings["ApplyNewShiftExchange_Payload"];
                    if (!string.IsNullOrEmpty(strValue) && strValue.Equals("true", StringComparison.OrdinalIgnoreCase))
                    {
                        returnValue = true;
                    }
                }
                catch
                {
                    returnValue = false;
                }
                return returnValue;
            }
        }

        public static double SAP_TimeOut
        {
            get
            {
                double returnValue = 180;
                try
                {
                    string strValue = ConfigurationManager.AppSettings["SAP_TimeOut"];
                    if (string.IsNullOrEmpty(strValue) || !double.TryParse(strValue, out returnValue))
                    {
                        returnValue = 180;
                    }
                }
                catch
                {
                    returnValue = 180;
                }
                return returnValue;
            }
        }

        public static string SiteUrl
        {
            get
            {
                string returnValue = string.Empty;
                try
                {
                    string strValue = ConfigurationManager.AppSettings["siteUrl"];
                    if (!string.IsNullOrEmpty(strValue))
                    {
                        returnValue = strValue;
                    }
                }
                catch
                {
                    returnValue = string.Empty;
                }
                return returnValue;
            }
        }

        public static int TargetYear
        {
            get
            {
                int defaultYear = DateTime.Now.Year;

                try
                {
                    string configValue = ConfigurationManager.AppSettings["TargetYear"];
                    if (int.TryParse(configValue, out int parsedYear))
                    {
                        return parsedYear;
                    }
                }
                catch
                {
                    defaultYear = DateTime.Now.Year;
                }

                return defaultYear;
            }
        }

    }
}
