using Microsoft.Extensions.Logging;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.ViewModels;
using System.Linq;
using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Enums;

namespace SSG2.API.Controllers.Others
{
    public class MasterDataController : BaseController
    {

        protected readonly IMasterDataB0 _masterDataBO;
        public MasterDataController(ILogger logger, ISAPBO sap, IMasterDataB0 mas) : base(logger)
        {               
            _masterDataBO = mas;
        }
        /// <summary>
        /// Lấy master data
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> GetMasterData(MasterDataArgs arg)
        {
            var result = new ResultDTO();
            try
            {
                result = await _masterDataBO.GetMasterDataValues(arg);
                return Ok(result);               
            }
            catch (Exception ex)
            {
                _logger.LogError("Error API:  GetMasterData ", ex.Message);
                return Ok(new ResultDTO { Object = new ArrayResultDTO { Data = Array.Empty<object>(), Count = 0 }, ErrorCodes = { 1001 }, Messages = { "Something went wrong!" } });
            }

        }

        [HttpPost]
        public async Task<IHttpActionResult> GetMasterDataForOtherModules(MasterDataArgs arg)
        {
            var result = new ResultDTO();
            try
            {
                result = await _masterDataBO.GetMasterDataValues(arg);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error API:  GetMasterData ", ex.Message);
                return Ok(new ResultDTO { Object = new ArrayResultDTO { Data = Array.Empty<object>(), Count = 0 }, ErrorCodes = { 1001 }, Messages = { "Something went wrong!" } });
            }

        }
    }



}
