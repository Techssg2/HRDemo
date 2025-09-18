using AutoMapper;
using Aeon.HR.ViewModels.Args;
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
    public partial class SettingBO
    {
        public async Task<ResultDTO> GetListGlobalLocations(QueryArgs arg)
        {
            var globalLocations = await _uow.GetRepository<GlobalLocation>().FindByAsync<GlobalLocationViewModels>(arg.Order, arg.Page, arg.Limit, arg.Predicate, arg.PredicateParameters);
            var totalList = await _uow.GetRepository<GlobalLocation>().CountAsync(arg.Predicate, arg.PredicateParameters);
            return new ResultDTO { Object = new ArrayResultDTO { Data = globalLocations, Count = totalList } };
        }

        public async Task<ResultDTO> CreateGlobalLocation(GlobalLocationArgs data)
        {
            var globalLocations = Mapper.Map<GlobalLocation>(data);
            _uow.GetRepository<GlobalLocation>().Add(globalLocations);
            await _uow.CommitAsync();
            return new ResultDTO { Object = data };
        }

        public async Task<ResultDTO> DeleteGlobalLocation(GlobalLocationArgs data)
        {
            var globalLocations = await _uow.GetRepository<GlobalLocation>().FindByAsync(x => x.Id == data.Id);
            if (globalLocations == null)
            {
                return new ResultDTO { Object = data };
            }
            else
            {
                _uow.GetRepository<GlobalLocation>().Delete(globalLocations);
                await _uow.CommitAsync();
            }
            return new ResultDTO { Object = data };
        }

        public async Task<ResultDTO> UpdateGlobalLocation(GlobalLocationArgs data)
        {
            var globalLocations = await _uow.GetRepository<GlobalLocation>().FindByAsync(x => x.Id == data.Id);
            if (globalLocations == null)
            {
                return new ResultDTO { Object = data };
            }
            else
            {
                globalLocations.FirstOrDefault().Code = data.Code;
                globalLocations.FirstOrDefault().BusinessTripLocationId = data.BusinessTripLocationId;
                globalLocations.FirstOrDefault().PartitionId = data.PartitionId;
                _uow.GetRepository<GlobalLocation>().Update(globalLocations);
                await _uow.CommitAsync();
            }
            return new ResultDTO { Object = data };
        }

        public async Task<ResultDTO> GetGlobalLocationByCode(QueryArgs arg)
        {
            var result = new ResultDTO();
            var data = arg.PredicateParameters[0].ToString().ToLower();
            if (arg.PredicateParameters[1] == null)
            {
                var globalLocationSettings = await _uow.GetRepository<GlobalLocation>().FindByAsync<GlobalLocationViewModels>(x => x.Code.ToLower() == data);
                result.Object = new ArrayResultDTO { Data = globalLocationSettings, Count = globalLocationSettings.Count() };
            }
            else
            {
                var id = new Guid(arg.PredicateParameters[1].ToString().ToLower());
                var globalLocationSettings = await _uow.GetRepository<GlobalLocation>().FindByAsync<GlobalLocationViewModels>(x => x.Code.ToLower() == data && x.Id != id);
                result.Object = new ArrayResultDTO { Data = globalLocationSettings, Count = globalLocationSettings.Count() };
            }
            return result;
        }

        public async Task<ResultDTO> GetGlobalLocationByArrivalPartition(QueryArgs arg)
        {
            var result = new ResultDTO();
            var dataBusinessTripLocationId = arg.PredicateParameters[0].ToString().ToLower();
            var dataPartitionId = arg.PredicateParameters[1].ToString().ToLower();

            if (arg.PredicateParameters[0] == null && arg.PredicateParameters[1] == null)
            {
                var globalLocationSettings = await _uow.GetRepository<GlobalLocation>().FindByAsync<GlobalLocationViewModels>(x => x.BusinessTripLocationId.ToString().ToLower() == dataBusinessTripLocationId && x.PartitionId.ToString().ToLower() == dataPartitionId);
                result.Object = new ArrayResultDTO { Data = globalLocationSettings, Count = globalLocationSettings.Count() };
            }
            else
            {
                var id = new Guid(arg.PredicateParameters[1].ToString().ToLower());
                var globalLocationSettings = await _uow.GetRepository<GlobalLocation>().FindByAsync<GlobalLocationViewModels>(x => x.BusinessTripLocationId.ToString().ToLower() == dataBusinessTripLocationId && x.PartitionId.ToString().ToLower() == dataPartitionId && x.Id != id);
                result.Object = new ArrayResultDTO { Data = globalLocationSettings, Count = globalLocationSettings.Count() };
            }
            return result;
        }

    }
}
