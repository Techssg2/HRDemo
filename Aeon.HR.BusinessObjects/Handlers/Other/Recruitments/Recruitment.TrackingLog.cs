using AutoMapper;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.ViewModels.DTOs;
using Aeon.HR.Data.Models;
using Aeon.HR.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Aeon.HR.BusinessObjects.Handlers
{
    public partial class RecruitmentBO
    {
        public async Task<ResultDTO> SaveTrackingLog(TrackingLogArgs data)
        {
            var existTrackingLog = await _uow.GetRepository<TrackingLog>().FindByAsync(x => x.Id == data.Id);
            if (existTrackingLog.Any())
            {
                return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Tracking Log is exist" } };
            }
            else
            {
                var trackingLog = Mapper.Map<TrackingLog>(data);
                _uow.GetRepository<TrackingLog>().Add(trackingLog);
                await _uow.CommitAsync();
                return new ResultDTO { Object = Mapper.Map<TrackingLogViewModel>(trackingLog) };
            }
        }

        public async Task<ArrayResultDTO> GetListTrackingLog(QueryArgs args)
        {
            var trackingLogs = await _uow.GetRepository<TrackingLog>().FindByAsync<TrackingLogViewModel>(args.Order, args.Page, args.Limit, args.Predicate, args.PredicateParameters);
            var count = await _uow.GetRepository<TrackingLog>().CountAsync(args.Predicate, args.PredicateParameters);
            var result = new ArrayResultDTO { Data = trackingLogs, Count = count };
            return result;
        }
    }
}
