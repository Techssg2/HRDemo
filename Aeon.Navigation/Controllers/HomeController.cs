using Aeon.HR.BusinessObjects.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace Aeon.Navigation.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Edoc1Url = ConfigurationManager.AppSettings["edoc1Url"];
            ViewBag.SR = ConfigurationManager.AppSettings["secret"];
            ViewBag.UXR = StringCipher.Encrypt("hiep.nguyen", ViewBag.SR);
            return View();
        }
    }
}
