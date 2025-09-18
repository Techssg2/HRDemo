using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.ViewModels.DTOs;
using Microsoft.Extensions.Logging;
using SSG2.API.Controllers.Others;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Aeon.HR.API.Controllers.Permission
{
    public class PermissionController : BaseController
    {
        protected readonly IPermissionBO _permBO;
        public PermissionController(ILogger logger, IPermissionBO permBO) : base(logger)
        {
            _permBO = permBO;
        }

        [HttpGet]
        public async Task<ResultDTO> GetPerm(Guid itemId)
        {
            var perm = await _permBO.GetItemPerm(itemId);
            return new ResultDTO() { Object = perm };
        }
    }
}