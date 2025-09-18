
using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Interfaces
{
    public interface ITrackingHistoryBO
    {
        Task<ResultDTO> GetTrackingHistoryByItemId(Guid ItemId);
        Task<ResultDTO> GetTrackingHistoryById(Guid Id);
        Task<ResultDTO> GetTrackingHistoryByTypeAndItemType(string type, string itemType);
        Task<ResultDTO> SaveTrackingHistory(TrackingHistoryArgs arg);
    }
}
