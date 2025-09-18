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
    public class JobGradeController : SettingController
    {
        public JobGradeController(ILogger logger, ISettingBO setting) : base(logger, setting) { }
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
        [HttpPost]
        public async Task<IHttpActionResult> UpdateJobGrade(JobGradeArgs model)
        {
            try
            {
                var res = await _settingBO.UpdateJobGrade(model);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: UpdateJobGrade " + ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpGet]
        public async Task<IHttpActionResult> DeleteJobGrade(Guid Id)
        {
            try
            {
                var res = await _settingBO.DeleteJobGrade(Id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: DeleteJobGrade " + ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> CreateJobGrade(JobGradeArgs model)
        {
            try
            {
                var res = await _settingBO.CreateJobGrade(model);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: CreateJobGrade " + ex.Message);
                return BadRequest("System Error");
            }
        }

        [HttpPost]
        // id là id JobGrade
        public async Task<IHttpActionResult> GetItemRecruitmentsOfJobGrade(GetItemsArgs args)
        {
            try
            {
                var res = await _settingBO.GetItemRecruitmentsOfJobGrade(args.JobGradeId);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetItemRecruitmentsOfJobGrade " + ex.Message);
                return Ok(new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } });
            }
        }

        [HttpPost]
        // id là id JobGrade
        public async Task<IHttpActionResult> AddOrUpdateItemsOfJobGrade(JobGradeForAddOrUpdateItemViewModel viewModel)
        {
            try
            {
                var res = await _settingBO.AddOrUpdateItemsOfJobGrade(viewModel);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: AddOrUpdateItemsOfJobGrade " + ex.Message);
                return Ok(new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } });
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetAllItemRecruitments()
        {
            try
            {
                var res = await _settingBO.GetAllItemRecruitments();
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetAllItemRecruitments " + ex.Message);
                return Ok(new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } });
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetJobGradeById(Guid id)
        {
            try
            {
                var res = await _settingBO.GetJobGradeById(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetJobGradeById " + ex.Message);
                return BadRequest("System Error");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetItemRecruitmentsByJobGradeId(GetItemsArgs args)
        {
            try
            {
                var res = await _settingBO.GetItemRecruitmentsByJobGradeId(args.JobGradeId);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetItemRecruitmentsByJobGradeId " + ex.Message);
                return Ok(new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } });
            }
        }

        //===== CR11.2 =====
        [HttpGet]
        public async Task<IHttpActionResult> GetJobGradeByJobGradeValue(int jobGradeValue)
        {
            try
            {
                var res = await _settingBO.GetJobGradeByJobGradeValue(jobGradeValue);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetJobGradeByJobGradeValue " + ex.Message);
                return Ok(new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } });
            }
        }
    }
}
