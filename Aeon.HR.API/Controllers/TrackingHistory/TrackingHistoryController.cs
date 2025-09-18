using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using Microsoft.Extensions.Logging;
using SSG2.API.Controllers.Others;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aeon.HR.API.Controllers.TrackingHitory
{
    public class TrackingHistoryController : BaseController
    {
        protected readonly ITrackingHistoryBO _trackingHistoryBO;
        public TrackingHistoryController(ILogger logger, ITrackingHistoryBO trackingHistoryBO) : base(logger)
        {
            _trackingHistoryBO = trackingHistoryBO;
        }

        [HttpGet]
        public async Task<ResultDTO> GetTrackingHistoryByItemId(Guid ItemId)
        {
            return await _trackingHistoryBO.GetTrackingHistoryByItemId(ItemId);
        }

        [HttpPost]
        public async Task<ResultDTO> GetTrackingHistoryByTypeAndItemType(string Type, string ItemType)
        {
            return await _trackingHistoryBO.GetTrackingHistoryByTypeAndItemType(Type, ItemType);
        }

        [HttpPost]
        public async Task<ResultDTO> SaveTrackingHistory(TrackingHistoryArgs args)
        {
            return await _trackingHistoryBO.SaveTrackingHistory(args);
        }

        [HttpGet]
        public async Task<ResultDTO> GetTrackingHistoryById(Guid Id)
        {
            return await _trackingHistoryBO.GetTrackingHistoryById(Id);
        }
    }
}