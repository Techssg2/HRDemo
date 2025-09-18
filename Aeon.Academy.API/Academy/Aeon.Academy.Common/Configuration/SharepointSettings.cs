using Aeon.Academy.Common.Consts;
using System.Configuration;

namespace Aeon.Academy.Common.Configuration
{
    public static class SharepointSettings
    {
        public static string FileName
        {
            get
            {
                return ConfigurationManager.AppSettings[CommonKeys.FileName] ?? string.Empty;
            }
        }
        public static string SiteUrl
        {
            get
            {
                return ConfigurationManager.AppSettings[CommonKeys.SiteUrl] ?? string.Empty;
            }
        }
        public static string CourseFolder
        {
            get
            {
                return ConfigurationManager.AppSettings[CommonKeys.CourseFolder] ?? string.Empty;
            }
        }
        public static string CourseDocumentLibrary
        {
            get
            {
                return ConfigurationManager.AppSettings[CommonKeys.CourseDocumentLibrary] ?? string.Empty;
            }
        }
        public static string ReportDocumentLibrary
        {
            get
            {
                return ConfigurationManager.AppSettings[CommonKeys.ReportDocumentLibrary] ?? string.Empty;
            }
        }
        public static string RequestDocumentLibrary
        {
            get
            {
                return ConfigurationManager.AppSettings[CommonKeys.RequestDocumentLibrary] ?? string.Empty;
            }
        }
        public static string InvitationDocumentLibrary
        {
            get
            {
                return ConfigurationManager.AppSettings[CommonKeys.InvitationDocumentLibrary] ?? string.Empty;
            }
        }
        public static string Password
        {
            get
            {
                return ConfigurationManager.AppSettings[CommonKeys.Password] ?? string.Empty;
            }
        }
        public static string UserName
        {
            get
            {
                return ConfigurationManager.AppSettings[CommonKeys.UserName] ?? string.Empty;
            }
        }
        public static string ServerFolder
        {
            get
            {
                return ConfigurationManager.AppSettings[CommonKeys.ServerFolder] ?? string.Empty;
            }
        }

    }
}

