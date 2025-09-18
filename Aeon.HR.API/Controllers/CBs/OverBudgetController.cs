using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Infrastructure;
using Aeon.HR.ViewModels.DTOs;
using Microsoft.Extensions.Logging;
using SSG2.API.Controllers.Others;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Aeon.HR.API.Controllers.CBs
{
    public class OverBudgetController : BaseController
    {
        protected readonly IOverBudgetBO bo;
        public OverBudgetController(ILogger _logger, IOverBudgetBO _bo) : base(_logger)
        {
            bo = _bo;
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetListOverBudget(QueryArgs arg)
        {
            try
            {
                var res = await bo.GetListOverBudget(arg);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetList Over Budget", ex.Message);
                return BadRequest("Error System");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> SaveRequestOverBudget(RequestOverBudgetDTO data)
        {
            try
            {
                var res = await bo.SaveRequestOverBudget(data);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: Save Request Over Budget", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> SaveBudget(BusinessOverBudgetDTO data)
        {
            try
            {
                var res = await bo.SaveBudget(data);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: Save Over Budget", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetItemOverBudgetById(Guid id)
        {
            try
            {
                var res = await bo.GetItemOverBudgetById(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetItemById OverBudget", ex.Message);
                return BadRequest("Error System");
            }
        }
        [HttpGet]
        public IHttpActionResult GetTripOverBudgetGroups(Guid id)
        {
            try
            {
                var res = bo.GetTripOverBudgetGroups(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error at Export File: GetTripOverBudgetGroups", ex.Message);
                return Ok(new ResultDTO { ErrorCodes = { 1003 }, Messages = { "No Data" } });
            }
        }

    }
}