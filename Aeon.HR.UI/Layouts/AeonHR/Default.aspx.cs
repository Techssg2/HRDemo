using System;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using Aeon.ManagementPortalUI.Encryption;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Linq;

namespace Aeon.ManagementPortalUI.Layouts.AeonHR
{
    public partial class Default : LayoutsPageBase
    {
        public string Version { get { return "1.2.35"; } }
        protected void Page_Load(object sender, EventArgs e)
        {
            var baseUrlApi = ConfigurationManager.AppSettings["baseUrlApi"];
            var secret = ConfigurationManager.AppSettings["secret"];
            var edoc1Url = ConfigurationManager.AppSettings["edoc1Url"];
            var edocITBaseUrl = ConfigurationManager.AppSettings["edocITBaseUrl"];
            var homeUpgradeUrlAPI = ConfigurationManager.AppSettings["homeUpgradeUrlAPI"];
            var subDomainEdoc1 = ConfigurationManager.AppSettings["subDomainEdoc1"];
            var subDomainSKU = ConfigurationManager.AppSettings["subDomainSKU"];
            var subDomainLiquor = ConfigurationManager.AppSettings["subDomainLiquor"];
            var subDomainHR = ConfigurationManager.AppSettings["subDomainHR"];
            var uxr = Regex.Replace(SPContext.Current.Web.CurrentUser.LoginName, ".*\\|(.*)", "$1", RegexOptions.None);
            if(!string.IsNullOrEmpty(uxr) && uxr.Contains("@"))
            {
                uxr = uxr.Split('@').FirstOrDefault();
            }

            uxr = Regex.Replace(uxr, ".*\\\\(.*)", "$1", RegexOptions.None);
            uxr = StringCipher.Encrypt(uxr, secret); 

            ClientScript.RegisterStartupScript(GetType(), "Javascript", $"var baseUrl ='/';var baseUrlApi='{baseUrlApi}'; var sr = '{secret}'; var uxr='{uxr}'; var edocV='{Version}'; var edoc1Url='{edoc1Url}'; var edocITBaseUrl='{edocITBaseUrl}'; var subDomainEdoc1='{subDomainEdoc1}';var homeUpgradeUrlAPI='{homeUpgradeUrlAPI}';var subDomainSKU='{subDomainSKU}';var subDomainLiquor='{subDomainLiquor}';var subDomainHR='{subDomainHR}'", true);
        }

    }
}
