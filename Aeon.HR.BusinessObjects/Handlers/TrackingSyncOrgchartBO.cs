using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Data.Models.SyncLog;
using Aeon.HR.Infrastructure;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.Infrastructure.Utilities;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.DTOs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Handlers
{
    public class TrackingSyncOrgchartBO : ITrackingSyncOrgchartBO
    {
        private readonly ILogger _logger;
        private readonly IUnitOfWork _uow;
        public TrackingSyncOrgchartBO(ILogger logger, IUnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
        }
        public async Task<ResultDTO> GetTrackingUserDepartmentsRequest(QueryArgs args)
        {
            var resultDto = new ResultDTO() { };
            try
            {
                var items = await _uow.GetRepository<UserDepartmentSyncHistory>().FindByAsync<UserDepartmentSyncHistoryViewModel>(args.Order, args.Page, args.Limit, args.Predicate, args.PredicateParameters);
                var count = await _uow.GetRepository<UserDepartmentSyncHistory>().CountAsync(args.Predicate, args.PredicateParameters);
                resultDto.Object = new ArrayResultDTO { Data = items.ToList(), Count = count };
                
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at GetTrackingUserDepartmentsRequest: {Message}", ex.Message);
            }
            return resultDto;
        }
        public async Task<ResultDTO> GetTrackingDepartmentsLogRequest(QueryArgs args)
        {
            var resultDto = new ResultDTO() { };
            try
            {
                var items = await _uow.GetRepository<DepartmentSyncHistory>().FindByAsync<DepartmentSyncHistoryViewModel>(args.Order, args.Page, args.Limit, args.Predicate, args.PredicateParameters);
                var count = await _uow.GetRepository<DepartmentSyncHistory>().CountAsync(args.Predicate, args.PredicateParameters);
                resultDto.Object = new ArrayResultDTO { Data = items.ToList(), Count = count };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at GetTrackingDepartmentsLogRequest: {Message}", ex.Message);
            }
            return resultDto;
        }
        public async Task<ResultDTO> GetTrackingUsersLogRequest(QueryArgs args)
        {
            var resultDto = new ResultDTO() { };
            try
            {
                var items = await _uow.GetRepository<UserSyncHistory>().FindByAsync<UserSyncHistoryViewModel>(args.Order, args.Page, args.Limit, args.Predicate, args.PredicateParameters);
                var count = await _uow.GetRepository<UserSyncHistory>().CountAsync(args.Predicate, args.PredicateParameters);
                resultDto.Object = new ArrayResultDTO { Data = items.ToList(), Count = count };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at GetTrackingUsersLogRequest: {Message}", ex.Message);
            }
            return resultDto;
        }
    }
}
