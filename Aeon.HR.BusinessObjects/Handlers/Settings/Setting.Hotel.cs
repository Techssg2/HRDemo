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
        #region Setting - Hotel
        public async Task<ResultDTO> GetListHotels(QueryArgs arg)
        {
            var hotels = await _uow.GetRepository<Hotel>().FindByAsync<HotelViewModel>(arg.Order, arg.Page, arg.Limit, arg.Predicate, arg.PredicateParameters);
            var totalList = await _uow.GetRepository<Hotel>().CountAsync(arg.Predicate, arg.PredicateParameters);
            return new ResultDTO { Object = new ArrayResultDTO { Data = hotels, Count = totalList } };
        }
        public async Task<ResultDTO> CreateHotel(HotelArgs data)
        {
            var hotels = Mapper.Map<Hotel>(data);
            _uow.GetRepository<Hotel>().Add(hotels);
            await _uow.CommitAsync();
            return new ResultDTO { Object = data };
        }
        public async Task<ResultDTO> DeleteHotel(HotelArgs data)
        {
            var hotels = await _uow.GetRepository<Hotel>().FindByAsync(x => x.Id == data.Id);
            if (hotels == null)
            {
                return new ResultDTO { Object = data };
            }
            else
            {
                _uow.GetRepository<Hotel>().Delete(hotels);
                await _uow.CommitAsync();
            }
            return new ResultDTO { Object = data };
        }
        public async Task<ResultDTO> UpdateHotel(HotelArgs data)
        {
            var hotels = await _uow.GetRepository<Hotel>().FindByAsync(x => x.Id == data.Id);
            if (hotels == null)
            {
                return new ResultDTO { Object = data };
            }
            else
            {
                hotels.FirstOrDefault().Code = data.Code;
                hotels.FirstOrDefault().Name = data.Name;
                hotels.FirstOrDefault().Address = data.Address;
                hotels.FirstOrDefault().Telephone = data.Telephone;
                hotels.FirstOrDefault().IsForeigner = data.IsForeigner;
                hotels.FirstOrDefault().BusinessTripLocationId = data.BusinessTripLocationId;
                _uow.GetRepository<Hotel>().Update(hotels);
                await _uow.CommitAsync();
            }
            return new ResultDTO { Object = data };
        }
        public async Task<ResultDTO> GetHotelByCode(QueryArgs arg)
        {
            var result = new ResultDTO();
            var data = arg.PredicateParameters[0].ToString().ToLower();
            if (arg.PredicateParameters[1] == null)
            {
                var hotelsettings = await _uow.GetRepository<Hotel>().FindByAsync<HotelViewModel>(x => x.Code.ToLower() == data);
                result.Object = new ArrayResultDTO { Data = hotelsettings, Count = hotelsettings.Count() };
            }
            else
            {
                var id = new Guid(arg.PredicateParameters[1].ToString().ToLower());
                var hotelsettings = await _uow.GetRepository<Hotel>().FindByAsync<HotelViewModel>(x => x.Code.ToLower() == data && x.Id != id);
                result.Object = new ArrayResultDTO { Data = hotelsettings, Count = hotelsettings.Count() };
            }

            return result;
        }
        #endregion
    }
}
