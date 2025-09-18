////using Microsoft.SharePoint;
//using Microsoft.SharePoint;
//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Linq;
//using System.Security.Principal;
//using System.Text;
//using System.Threading.Tasks;

//namespace Aeon.HR.BusinessObjects.Helpers
//{
//    public static class SPInvoker
//    {
//        public static void Invoke(Action<SPSite, SPWeb> action)
//        {
//            SPSecurity.RunWithElevatedPrivileges(() =>
//            {
//                using (SPSite site = new SPSite(ConfigurationManager.AppSettings["siteUrl"]))
//                {
//                    using (SPWeb web = site.OpenWeb())
//                    {
//                        action(site, web);
//                    }
//                }
//            });
//        }
//    }
//}
