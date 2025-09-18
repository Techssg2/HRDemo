using Microsoft.Extensions.Logging;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.BusinessObjects.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.DTOs;

namespace SSG2.API.Controllers.Settings
{
    public class BusinessModelController : SettingController
    {
        public BusinessModelController(ILogger logger, ISettingBO setting) : base(logger, setting) { }
        [HttpPost]
        public async Task<IHttpActionResult> GetBusinessModelList(QueryArgs args)
        {
            try
            {
                var res = await _settingBO.GetBusinessModelList(args);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetBusinessModelList " + ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> UpdateBusinessModel(BusinessModelArgs model)
        {
            try
            {
                var res = await _settingBO.UpdateBusinessModel(model);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: UpdateBusinessModel " + ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpGet]
        public async Task<IHttpActionResult> DeleteBusinessModel(Guid Id)
        {
            try
            {
                var res = await _settingBO.DeleteBusinessModel(Id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: DeleteBusinessModel " + ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> CreateBusinessModel(BusinessModelArgs model)
        {
            try
            {
                var res = await _settingBO.CreateBusinessModel(model);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: CreateBusinessModel " + ex.Message);
                return BadRequest("System Error");
            }
        }
    }
}
