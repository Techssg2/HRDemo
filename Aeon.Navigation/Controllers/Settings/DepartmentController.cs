using Microsoft.Extensions.Logging;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Aeon.HR.Infrastructure.Enums;

namespace Aeon.Navigation.Controllers.Settings
{
    public class DepartmentController : SettingController
    {
        public DepartmentController(ILogger logger, ISettingBO setting) : base(logger, setting) { }

        [HttpPost]
        public async Task<IHttpActionResult> GetDepartmentByFilter(QueryArgs args)
        {
            try
            {
                var res = await _settingBO.GetDepartmentByFilter(args);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetDepartmentTree", ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpGet]
        public async Task<IHttpActionResult> getAllListDepartments()
        {
            try
            {
                var res = await _settingBO.GetAllListDepartments();
                return Ok(new ResultDTO { Object = res });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: getAllDepartments", ex.Message);
                return BadRequest("System Error");
            }
        }
    }
}
