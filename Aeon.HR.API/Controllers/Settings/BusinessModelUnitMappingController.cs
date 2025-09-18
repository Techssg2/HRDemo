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
    public class BusinessModelUnitMappingController : SettingController
    {
        public BusinessModelUnitMappingController(ILogger logger, ISettingBO setting) : base(logger, setting) { }
        [HttpPost]
        public async Task<IHttpActionResult> CreateBusinessModelUnitMapping(BusinessModelUnitMappingArgs model)
        {
            try
            {
                var res = await _settingBO.CreateBusinessModelUnitMapping(model);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: CreateBusinessModel " + ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetBusinessModelUnitMappingList(QueryArgs args)
        {
            try
            {
                var res = await _settingBO.GetBusinessModelUnitMappingList(args);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetBusinessModelList " + ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> UpdateBusinessModelUnitMapping(BusinessModelUnitMappingArgs model)
        {
            try
            {
                var res = await _settingBO.UpdateBusinessModelUnitMapping(model);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: UpdateBusinessModel " + ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpGet]
        public async Task<IHttpActionResult> DeleteBusinessModelUnitMapping(Guid Id)
        {
            try
            {
                var res = await _settingBO.DeleteBusinessModelUnitMapping(Id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: DeleteBusinessModel " + ex.Message);
                return BadRequest("System Error");
            }
        }
    }
}
