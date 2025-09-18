using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.ViewModels.DTOs;
using Microsoft.Extensions.Logging;
using SSG2.API.Controllers.Others;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Aeon.HR.API.Controllers.Maintenant
{
    public class MaintenantController: BaseController
    {
        protected readonly IMaintenantBO _maintenantBO;
        public MaintenantController(ILogger logger, IMaintenantBO maintenantBO) : base(logger)
        {
            _maintenantBO = maintenantBO;
        }


        [HttpGet]
        public ResultDTO GetItemsHasWrongStatus()
        {
            return _maintenantBO.GetItemsHasWrongStatus();
        }

        [HttpGet]
        public ResultDTO GetItemsNotHavePayload()
        {
            return _maintenantBO.GetItemsNotHavePayload();
        }

        [HttpGet]
        public ResultDTO GetItemsHadPending()
        {
            return _maintenantBO.GetItemsHadPending();
        }

        [HttpGet]
        public ResultDTO GetUserLockedStatus(string sapCode)
        {
            if(string.IsNullOrEmpty(sapCode))
            {
                return _maintenantBO.GetLockedUser();
            }
            else
            {
                return _maintenantBO.GetUserLockedStatus(sapCode);
            }
        }

        [HttpGet]
        public async Task<ResultDTO> SyncUserDataFromSAP(string sapCode)
        {
            return await _maintenantBO.SyncUserDataFromSAP(sapCode);
        }

        [HttpGet]
        public ResultDTO UnlockedUser(string sapCode)
        {
            return _maintenantBO.UnlockedUser(sapCode);
        }

        [HttpGet]
        public ResultDTO GetLockedUser()
        {
            return _maintenantBO.GetLockedUser();
        }

        [HttpGet]
        public async Task<ResultDTO> GetUserSend_OT_Holiday_Failed()
        {
            return await _maintenantBO.GetUserSend_OT_Holiday_Failed();
        }

        [HttpGet]
        public async Task<ResultDTO> SubmitPayload(Guid itemID)
        {
            return await _maintenantBO.SubmitPayload(itemID);
        }

        [HttpGet]
        public ResultDTO UpdateStatus(string referenceNumber) {
            return _maintenantBO.UpdateStatus(referenceNumber);
        }

        [HttpGet]
        public async Task<ResultDTO> GenerateOTPayload(string otReferenceNumber)
        {
            return await _maintenantBO.GenerateOTPayload(otReferenceNumber);
        }
    }
}