using Aeon.HR.BusinessObjects.Helpers;
using System.Configuration;
using System.Web.Mvc;

namespace Aeon.HR.API.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {// GetDepartmentUpToG4ByUserId
            ViewBag.Edoc1Url = ConfigurationManager.AppSettings["edoc1Url"];
            ViewBag.SR = ConfigurationManager.AppSettings["secret"];
            ViewBag.UXR = StringCipher.Encrypt("SAdmin"/*"297078  311018 284633 402080"*/, ViewBag.SR);
            //ViewBag.UXR = StringCipher.Encrypt(Regex.Replace(HttpContext.Request.LogonUserIdentity.Name, ".*\\\\(.*)", "$1", RegexOptions.None), ViewBag.SR);
            return View();
        }
    }
}