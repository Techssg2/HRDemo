using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using Microsoft.Extensions.Logging;
using SSG2.API.Controllers.Others;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Aeon.HR.API.Controllers
{
    public class CommonController : BaseController
    {
        protected readonly ICommonBO _commonBO;
        public CommonController(ILogger logger, ICommonBO commonBO) : base(logger)
        {
            _commonBO = commonBO;
        }

        [HttpPost]
        public async Task<IHttpActionResult> DeleteItemById(Guid id)
        {
            return Ok(await _commonBO.DeleteItemById(id));
        }

        [HttpPost]
        public async Task<IHttpActionResult> UpdateStatusByReferenceNumber(UpdateStatusArgs args)
        {
            return Ok(await _commonBO.UpdateStatusByReferenceNumber(args));
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetItemById(Guid id)
        {
            return Ok(await _commonBO.GetItemById(id));
        }
    }
}