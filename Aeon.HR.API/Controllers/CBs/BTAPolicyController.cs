using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.ViewModels.DTOs;
using Microsoft.Extensions.Logging;
using SSG2.API.Controllers.CBs;
using SSG2.API.Controllers.Others;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.ViewModels;
using Aeon.HR.API.Helpers;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.BusinessObjects.Handlers;

namespace Aeon.HR.API.Controllers.CBs
{
    public class BTAPolicyController : BaseController
    {
        protected readonly IBTAPolicyBO bo;
        public BTAPolicyController(ILogger _logger, IBTAPolicyBO _bo) : base(_logger)
        {
            bo = _bo;
        }
        #region  BTAPolicy API
        [HttpGet]
        public async Task<IHttpActionResult> GetBTAPolicyByDepartment(string typeDepartment)
        {
            try
            {
                var res = await bo.GetBTAPolicyByDepartment(typeDepartment);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetBTAPolicyByDepartment", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetBTAPolicyList(QueryArgs arg)
        {
            try
            {
                var res = await bo.GetBTAPolicyList(arg);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetBTAPolicyList", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> CreateBTAPolicy(BTAPolicyArgs arg)
        {
            try
            {
                var res = await bo.CreateBTAPolicy(arg);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: CreateBTAPolicy", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> UpdateBTAPolicy(BTAPolicyArgs arg)
        {
            try
            {
                var res = await bo.UpdateBTAPolicy(arg);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: UpdateBTAPolicy", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> DeleteBTAPolicy(BTAPolicyArgs arg)
        {
            try
            {
                var res = await bo.DeleteBTAPolicy(arg);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: DeleteBTAPolicy", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public async Task<ResultDTO> GetBTAPolicyByJobGradePartition(QueryArgs arg)
        {
            try
            {
                var res = await bo.GetBTAPolicyByJobGradePartition(arg);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetPartitionByCode", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        #endregion
        #region BTA Policy Special Cases - API
        [HttpPost]
        public async Task<IHttpActionResult> GetListBTAPolicySpecialCases(QueryArgs arg)
        {
            try
            {
                var res = await bo.GetListBTAPolicySpecialCases(arg);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetListBTAPolicySpecialCases", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> CreateBTAPolicySpecialCases(BTAPolicySpecialArgs arg)
        {
            try
            {
                var res = await bo.CreateBTAPolicySpecialCases(arg);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: CreateBTAPolicySpecialCases", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> UpdateBTAPolicySpecialCases(BTAPolicySpecialArgs arg)
        {
            try
            {
                var res = await bo.UpdateBTAPolicySpecialCases(arg);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: UpdateBTAPolicySpecialCases", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> DeleteBTAPolicySpecialCases(BTAPolicySpecialArgs arg)
        {
            try
            {
                var res = await bo.DeleteBTAPolicySpecialCases(arg);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: DeleteBTAPolicySpecialCases", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetBTAPolicySpecialCasesByUserSAPCode(string userSAPCode, Guid partitionId)
        {
            try
            {
                var res = await bo.GetBTAPolicySpecialCasesByUserSAPCode(userSAPCode, partitionId);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetBTAPolicySpecialCasesByUserSAPCode", ex.Message);
                return BadRequest("Error System");
            }
        }
        #endregion
    }
}
