using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SSG2.API.Controllers.CBs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aeon.HR.API.Controllers.CBs
{
    [RoutePrefix("api/ShiftExchange")]
    public class ShiftExchangeController : CandBController
    {
        private readonly IIntegrationExternalServiceBO _externalServiceBO;
        public ShiftExchangeController(ILogger logger, ICBBO cbBO, IIntegrationExternalServiceBO externalServiceBO) : base(logger, cbBO)
        {
            _externalServiceBO = externalServiceBO;
        }

        private Guid _currentUserId { get; set; }
        [HttpPost]
        public async Task<ResultDTO> GetAvailableLeaveBalances(List<AvailableLeaveBalanceArg> args)
        {
            try
            {
                if (args.Any())
                {
                    var lstResult = new List<LeaveBalanceResponceSAPViewModel>();
                    var groups = args.GroupBy(g => g.ExchangeYear).Select(s => s.Key);
                    if (groups.Any())
                    {
                        var excution = _externalServiceBO.BuildAPIService(ExtertalType.LeaveBalance);
                        excution.APIName = "GetLeaveBalanceSet";
                        foreach (var group in groups)
                        {
                            var sapCodes = args.Where(i => i.ExchangeYear == group).GroupBy(g => g.SapCode).Select(s => s.Key).ToList();

                            string pern = "(";
                            for (var i = 0; i < sapCodes.Count; i++)
                            {
                                pern += "Pernr eq '{" + i + "}'";
                                if (sapCodes.Count - i > 1)
                                {
                                    pern += " or ";
                                }
                            }
                            pern += ")";
                            var predicate = "$filter=" + pern + " and Year eq '{" + sapCodes.Count + "}'";
                            sapCodes.Add(group);
                            var predicateParameters = sapCodes.ToArray();
                            var res = (List<LeaveBalanceResponceSAPViewModel>)await excution.GetData(predicate, predicateParameters);
                            if (res.Any())
                                lstResult.AddRange(res);
                        }
                        return new ResultDTO { Object = lstResult };
                    }
                }
                return new ResultDTO { ErrorCodes = { 1002 }, Messages = { "No Data" } };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetAvailableLeaveBalances", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "No Connection" } };
            }
        }

        [HttpPost]
        public async Task<ResultDTO> GetAllShiftExchange([FromBody] QueryArgs data)
        {
            try
            {
                var res = await _cbBO.GetAllShiftExchange(data);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetAllShiftExchange", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> GetShiftExchanges([FromBody] QueryArgs args)
        {
            try
            {
                var res = await _cbBO.GetShiftExchanges(args, _currentUserId);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetShiftExchanges", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }

        [HttpPost]
        //[Route("GetShiftExchange/{referenceNumber}")]
        public async Task<ResultDTO> GetShiftExchange([FromBody] QueryArgs data)
        {
            try
            {
                var res = await _cbBO.GetShiftExchange(data, _currentUserId);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetShiftExchangeById", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }

        [HttpPost]
        public async Task<ResultDTO> SaveShiftExchange([FromBody] ShifExchangeForAddOrUpdateViewModel args)
        {
            try
            {
                _currentUserId = args.currentUserId;
                var res = await _cbBO.SaveShiftExchange(args, _currentUserId);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: SaveShiftExchange", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }

        // AEON_658 
        [HttpPost]
        public async Task<ResultDTO> CheckTargetPlanComplete([FromBody] ShifExchangeForAddOrUpdateViewModel args)
        {
            try
            {
                _currentUserId = args.currentUserId;
                var res = await _cbBO.CheckTargetPlanComplete(args, _currentUserId);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: SaveShiftExchange", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        } 

        [HttpPost]
        public async Task<ResultDTO> SubmitShiftExchange([FromBody] ShifExchangeForAddOrUpdateViewModel args)
        {
            try
            {
                var res = await _cbBO.SubmitShiftExchange(args, _currentUserId);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: SubmitShiftExchange", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        //[Route("GetShiftExchange/{referenceNumber}")]
        public async Task<ResultDTO> GetShiftExchangeDetailById(Guid Id)
        {
            try
            {
                var res = await _cbBO.GetShiftExchangeDetailById(Id);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetShiftExchangeById", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }

        [HttpPost]
        //[Route("GetShiftExchange/{referenceNumber}")]
        public async Task<ResultDTO> ValidateERDShiftExchangeDetail(List<ValidateERDShiftExchangeViewModel> arg)
        {
            try
            {
                var res = await _cbBO.ValidateERDShiftExchangeDetail(arg);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: ValidateERDShiftExchangeDetail", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }

        [HttpPost]
        public async Task<ResultDTO> GetCurrentShiftCodeFromShiftPlan(List<CurrentShiftArg> arg)
        {
            try
            {
                var res = await _cbBO.GetCurrentShiftCodeFromShiftPlan(arg);
                if (res.Count > 0)
                {
                    return new ResultDTO { Object = res };
                }
                return new ResultDTO { ErrorCodes = new List<int> { 1004 }, Messages = new List<string> { "No Data" } };


            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: ValidateERDShiftExchangeDetail", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
    }
}
