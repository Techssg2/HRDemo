using Microsoft.Extensions.Logging;
using Aeon.HR.BusinessObjects.Interfaces;
using SSG2.API.Controllers.Others;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace SSG2.API.Controllers.Settings
{
    //[Authorize(Roles = "Administrator")]
    public class SettingController : BaseController
    {
        protected readonly ISettingBO _settingBO;
        public SettingController(ILogger logger, ISettingBO setting) : base(logger)
        {
            _settingBO = setting;
        }     
    }
}
