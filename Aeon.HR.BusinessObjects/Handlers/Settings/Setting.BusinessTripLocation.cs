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
        #region Setting - Business Trip Location
        public async Task<ResultDTO> GetListBusinessTripLocation(QueryArgs arg)
        {
            var businessTripLocations = await _uow.GetRepository<BusinessTripLocation>().FindByAsync<BusinessTripLocationViewModel>(arg.Order, arg.Page, arg.Limit, arg.Predicate, arg.PredicateParameters);
            var totalList = await _uow.GetRepository<BusinessTripLocation>().CountAsync(arg.Predicate, arg.PredicateParameters);
            return new ResultDTO { Object = new ArrayResultDTO { Data = businessTripLocations, Count = totalList } };
        }
        public async Task<ResultDTO> CreateBusinessTripLocation(BusinessTripLocationArgs data)
        {
            var businessTripLocation = Mapper.Map<BusinessTripLocation>(data);
            _uow.GetRepository<BusinessTripLocation>().Add(businessTripLocation);
            await _uow.CommitAsync();
            return new ResultDTO { Object = data };
        }
        public async Task<ResultDTO> DeleteBusinessTripLocation(BusinessTripLocationArgs data)
        {
            // Thêm xử lý dò tìm xem có hotel nào đang có businessTripLocation này không
            var haveBusinessTripLocationInHotel = await _uow.GetRepository<Hotel>().FindByAsync<HotelViewModel>(x => x.BusinessTripLocationId == data.Id);
            if (haveBusinessTripLocationInHotel.Any())
            {
                return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "This data has been used in Hotel. You have to change Business Trip Location in Hotel first" } };
            }
            //end
            else
            {
                var businessTripLocation = await _uow.GetRepository<BusinessTripLocation>().FindByAsync(x => x.Id == data.Id);
                if (businessTripLocation == null)
                {
                    return new ResultDTO { Object = data };
                }
                else
                {
                    _uow.GetRepository<BusinessTripLocation>().Delete(businessTripLocation);
                    await _uow.CommitAsync();
                    
                }
                return new ResultDTO { Object = data };
            }
        }
        public async Task<ResultDTO> UpdateBusinessTripLocation(BusinessTripLocationArgs data)
        {
            var businessTripLocations = await _uow.GetRepository<BusinessTripLocation>().FindByAsync(x => x.Id == data.Id);
            if (businessTripLocations == null)
            {
                return new ResultDTO { Object = data };
            }
            else
            {
                businessTripLocations.FirstOrDefault().Code = data.Code;
                businessTripLocations.FirstOrDefault().Name = data.Name;
                _uow.GetRepository<BusinessTripLocation>().Update(businessTripLocations);
                await _uow.CommitAsync();
            }
            return new ResultDTO { Object = data };
        }
        public async Task<ResultDTO> GetBusinessTripLocationByCode(QueryArgs arg)
        {
            var result = new ResultDTO();
            var data = arg.PredicateParameters[0].ToString().ToLower();
            if (arg.PredicateParameters[1] == null)
            {
                var businessTripLocations = await _uow.GetRepository<BusinessTripLocation>().FindByAsync<BusinessTripLocationViewModel>(x => x.Code.ToLower() == data);
                result.Object = new ArrayResultDTO { Data = businessTripLocations, Count = businessTripLocations.Count() };
            }
            else
            {
                var id = new Guid(arg.PredicateParameters[1].ToString().ToLower());
                var businessTripLocations = await _uow.GetRepository<BusinessTripLocation>().FindByAsync<BusinessTripLocationViewModel>(x => x.Code.ToLower() == data && x.Id != id);
                result.Object = new ArrayResultDTO { Data = businessTripLocations, Count = businessTripLocations.Count() };
            }

            return result;
        }
        #endregion
    }
}
