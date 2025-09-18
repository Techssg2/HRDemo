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
using Aeon.HR.ViewModels.DTOs;

namespace SSG2.API.Controllers.Settings
{
    public class HeadCountSettingController : SettingController
    {
        public HeadCountSettingController(ILogger logger, ISettingBO setting) : base(logger, setting) { }
        [HttpGet]
        public async Task<IHttpActionResult> GetDepartmentListForHeadCount()
        {
            try
            {
                var res = await _settingBO.GetDepartmentListForHeadCount();
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetDepartmentListForHeadCount " + ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> CreateHeadCount(HeadCountArgs model)
        {
            try
            {
                var res = await _settingBO.CreateHeadCount(model);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: CreateHeadCount " + ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> UpdateHeadCount(HeadCountArgs model)
        {
            try
            {
                var res = await _settingBO.UpdateHeadCount(model);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: UpdateHeadCount " + ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpGet]
        public async Task<IHttpActionResult> DeleteHeadCount(Guid Id)
        {
            try
            {
                var res = await _settingBO.DeleteHeadCount(Id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: DeleteHeadCount " + ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetHeadCountList(QueryArgs args)
        {
            try
            {
                var res = await _settingBO.GetHeadCountList(args);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetHeadCountList " + ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetHeadCountByDepartmentId(Guid id, int jobGrade)
        {
            try
            {
                var res = await _settingBO.GetHeadCountByDepartmentId(id, jobGrade);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetHeadCountByDepartmentId " + ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetJobGradeList(QueryArgs args)
        {
            try
            {
                var res = await _settingBO.GetJobGradeList(args);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetJobGradeList " + ex.Message);
                return BadRequest("System Error");
            }
        }
    }
}
