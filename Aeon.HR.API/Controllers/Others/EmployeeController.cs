using Microsoft.Extensions.Logging;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.BusinessObjects.ExternalHelper.SAP;

namespace SSG2.API.Controllers.Others
{
    public class EmployeeController : BaseController
    {
        protected readonly IEmployeeBO _bo;
        public EmployeeController(ILogger logger, IEmployeeBO bo) : base(logger)
        {
            _bo = bo;
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetEmployeeInfo(string SAPCode)
        {
            try
            {
                return Ok(await _bo.GetFullEmployeeInfo(SAPCode));
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at: GetEmployeeInfo", ex.Message);
                return Ok(new ResultDTO { Object = new ArrayResultDTO { Data = Array.Empty<object>(), Count = 0 }, ErrorCodes = { 1001 }, Messages = { "Something went wrong!" } });
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetMasterDataEmployeeList(string empSubgroup) 
        {
            try
            {
                return Ok(await _bo.GetMasterDataEmployeeList(empSubgroup));
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at: GetMasterDataEmployeeList", ex.Message);
                return Ok(new ResultDTO { Object = new ArrayResultDTO { Data = Array.Empty<object>(), Count = 0 }, ErrorCodes = { 1001 }, Messages = { "Something went wrong!" } });
            }
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetNewWorkLocationList(string newWorkLocationText) {
            try
            {
                return Ok(await _bo.GetNewWorkLocationList(newWorkLocationText));
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at: GetNewWorkLocationList", ex.Message);
                return Ok(new ResultDTO { Object = new ArrayResultDTO { Data = Array.Empty<object>(), Count = 0 }, ErrorCodes = { 1001 }, Messages = { "Something went wrong!" } });
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetNewWorkLocationListV2(string newWorkLocationCode)
        {
            try
            {
                return Ok(await _bo.GetNewWorkLocationListV2(newWorkLocationCode));
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at: GetNewWorkLocationList", ex.Message);
                return Ok(new ResultDTO { Object = new ArrayResultDTO { Data = Array.Empty<object>(), Count = 0 }, ErrorCodes = { 1001 }, Messages = { "Something went wrong!" } });
            }
        }
    }

}
