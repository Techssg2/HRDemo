using Aeon.HR.Data.Models;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Interfaces
{
    public interface IMaintenantBO
    {
        ResultDTO GetItemsHasWrongStatus();
        ResultDTO GetItemsNotHavePayload();
        ResultDTO GetItemsHadPending();
        ResultDTO GetUserLockedStatus(string sapCode);
        ResultDTO UnlockedUser(string sapCode);
        ResultDTO UpdateStatus(string ReferenceNumber);
        ResultDTO GetLockedUser();
        Task<ResultDTO> GetUserSend_OT_Holiday_Failed();
        Task<ResultDTO> SubmitPayload(Guid itemID); 
        Task<ResultDTO> SyncUserDataFromSAP(string userSAPCode);
        Task<ResultDTO> GenerateOTPayload(string otReferenceNumber);
    }
}
