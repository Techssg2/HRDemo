using Aeon.Academy.Common.Consts;
using Aeon.HR.ViewModels.CustomSection;
using System.Configuration;

namespace Aeon.Academy.Common.Configuration
{
    public static class ApplicationSettings
    {
        public static string ApiSecret
        {
            get
            {
                return ConfigurationManager.AppSettings[CommonKeys.SECRET];
            }
        }
        public static int AdminRole
        {
            get
            {
                var role = int.MinValue;
                var adminRole = ConfigurationManager.AppSettings[CommonKeys.AdminRole];
                if (!string.IsNullOrEmpty(adminRole))
                    int.TryParse(adminRole, out role);
                return role;
            }
        }
        public static int UserType
        {
            get
            {
                var role = -1;
                var adminRole = ConfigurationManager.AppSettings[CommonKeys.UserType];
                if (!string.IsNullOrEmpty(adminRole))
                    int.TryParse(adminRole, out role);
                return role;
            }
        }
        public static SAPSettingsSection Edoc1Settings
        {
            get
            {
                var sapSetting = (SAPSettingsSection)ConfigurationManager.GetSection("academyEdoc1Settings");
                return sapSetting;
            }
        }
        public static SAPSettingsSection SapSettings
        {
            get
            {
                var sapSetting = (SAPSettingsSection)ConfigurationManager.GetSection("sapSettings");
                return sapSetting;
            }
        }
        public static string FakeUrl
        {
            get
            {
                return ConfigurationManager.AppSettings[CommonKeys.FakeUrl];
            }
        }
        public static string AcademySAPAction
        {
            get
            {
                return ConfigurationManager.AppSettings[CommonKeys.AcademySapAction];
            }
        }
        public static string F2FakeUrl
        {
            get
            {
                return ConfigurationManager.AppSettings[CommonKeys.F2FakeUrl];
            }
        }
        public static string SapApi
        {
            get
            {
                if (!string.IsNullOrEmpty(FakeUrl))
                {
                    return "api/sapintegration/syncdata";
                }
                var sapApi = string.Empty;
                if (SapSettings != null)
                {
                    var sapSetting = SapSettings;
                    sapApi = $"{sapSetting.SAPGroupDataCollection["Employee"].Value}/{AcademySAPAction}";
                }
                return sapApi;
            }
        }
        public static string AcademySupportEmail
        {
            get
            {
                return ConfigurationManager.AppSettings[CommonKeys.AcademySupportEmail];
            }
        }
        public static string DeptAcademyCode
        {
            get
            {
                return ConfigurationManager.AppSettings[CommonKeys.DeptAcademyCode]?.Trim();
            }
        }
        public static string AcademyEdocFakeAction
        {
            get
            {
                return ConfigurationManager.AppSettings[CommonKeys.AcademyEdocFakeAction];
            }
        }
        public static string AcademyTrainingRequestUrl
        {
            get
            {
                return ConfigurationManager.AppSettings[CommonKeys.AcademyTrainingRequestUrl] ?? string.Empty;
            }
        }
        public static int CreatedDateFrom
        {
            get
            {
                var d = -180;
                var days = ConfigurationManager.AppSettings[CommonKeys.CreateFromDate];
                if (!string.IsNullOrEmpty(days))
                    int.TryParse(days, out d);
                return d;
            }
        }
    }
}
