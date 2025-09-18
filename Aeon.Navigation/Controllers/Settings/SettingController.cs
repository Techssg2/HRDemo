using Microsoft.Extensions.Logging;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using Aeon.Navigation.Controllers.Others;

namespace Aeon.Navigation.Controllers.Settings
{
    public class SettingController : BaseController
    {
        protected readonly ISettingBO _settingBO;
        public SettingController(ILogger logger, ISettingBO setting) : base(logger)
        {
            _settingBO = setting;
        }     
    }
}
