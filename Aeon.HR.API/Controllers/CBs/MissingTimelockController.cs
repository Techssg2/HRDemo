using Microsoft.Extensions.Logging;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.ViewModels.DTOs;
using Aeon.HR.Data.Models;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using Aeon.HR.ViewModels.Args;

namespace SSG2.API.Controllers.CBs
{
    public class MissingTimelockController : CandBController
    {
        public MissingTimelockController(ILogger logger, ICBBO cbBO) : base(logger, cbBO) { }

        [HttpPost]
        public async Task<ResultDTO> GetAllMissingTimelockByUser([FromBody] QueryArgs data)
        {
            try
            {
                var res = await _cbBO.GetAllMissingTimelockByUser(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetAllMissingTimelockByReferenceNumber", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }

        [HttpPost]
        public async Task<ResultDTO> GetMissingTimelockByReferenceNumber([FromBody] MasterdataArgs data)
        {
            try
            {
                var res = await _cbBO.GetMissingTimelockByReferenceNumber(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetMissingTimelockBySapCode", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }

        [HttpPost]
        public async Task<ResultDTO> GetMissingTimelocks([FromBody] QueryArgs data)
        {
            try
            {
                var res = await _cbBO.GetMissingTimelocks(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetMissingTimelocks", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }

        [HttpPost]
        public async Task<ResultDTO> SaveMissingTimelock([FromBody] MissingTimelockArgs data)
        {
            try
            {
                var res = await _cbBO.SaveMissingTimelock(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: SaveMissingTimelock", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }

        [HttpPost]
        public async Task<ResultDTO> SubmitMissingTimelock([FromBody] MissingTimelockArgs data)
        {
            try
            {
                var res = await _cbBO.SubmitMissingTimelock(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: SaveMissingTimelock", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
    }
}
