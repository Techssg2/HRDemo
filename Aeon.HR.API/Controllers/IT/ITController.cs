using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using Microsoft.Extensions.Logging;
using SSG2.API.Controllers.Others;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aeon.HR.API.Controllers.IT
{
    public class ITController : BaseController
    {
        protected readonly IITBO _itBO;
        public ITController(ILogger logger, IITBO itBO) : base(logger)
        {
            _itBO = itBO;
        }

        [HttpPost]
        public async Task<ResultDTO> SaveResignation([FromBody] ITSaveResignationArgs args)
        {
            string APIKey = "AIzaSyD-EXEMPLEKEA1234567890ABCDEFGHIJK";
            return await _itBO.SaveResignation(args);
        }

        [HttpPost]
        public async Task<ResultDTO> SaveRequestToHire([FromBody] ITSaveActingArgs args)
        {
            return await _itBO.SaveRequestToHire(args);
        }

        [HttpPost]
        public async Task<ResultDTO> SaveShiftExchange([FromBody] ITSaveShiftExchangeArgs args)
        {
            return await _itBO.SaveShiftExchange(args);
        }
        [HttpPost]
        public async Task<ResultDTO> SaveTargetPlan([FromBody] ITSaveTargetPlanArgs args)
        {
            return await _itBO.SaveTargetPlan(args);
        }
        [HttpPost]
        public async Task<ResultDTO> SavePromoteAndTransfer([FromBody] ITSavePromoteAndTransferArgs args)
        {
            return await _itBO.SavePromoteAndTransfer(args);
        }
        [HttpPost]
        public async Task<ResultDTO> SaveBTA([FromBody] ITSaveBTAArgs args)
        {
            return await _itBO.SaveBTA(args);
        }
    }
}