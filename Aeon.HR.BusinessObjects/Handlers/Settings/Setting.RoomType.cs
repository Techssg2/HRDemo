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
        #region Setting - Room Type
        public async Task<ResultDTO> GetListRoomTypes(QueryArgs arg)
        {
            var roomTypes = await _uow.GetRepository<RoomType>().FindByAsync<RoomTypeViewModel>(arg.Order, arg.Page, arg.Limit, arg.Predicate, arg.PredicateParameters);
            var totalList = await _uow.GetRepository<RoomType>().CountAsync(arg.Predicate, arg.PredicateParameters);
            return new ResultDTO { Object = new ArrayResultDTO { Data = roomTypes, Count = totalList } };
        }
        public async Task<ResultDTO> CreateRoomType(RoomTypeArgs data)
        {
            var roomType = Mapper.Map<RoomType>(data);
            _uow.GetRepository<RoomType>().Add(roomType);
            await _uow.CommitAsync();
            return new ResultDTO { Object = data };
        }
        public async Task<ResultDTO> DeleteRoomType(RoomTypeArgs data)
        {
            var roomType = await _uow.GetRepository<RoomType>().FindByAsync(x => x.Id == data.Id);
            if (roomType == null)
            {
                return new ResultDTO { Object = data };
            }
            else
            {
                _uow.GetRepository<RoomType>().Delete(roomType);
                await _uow.CommitAsync();
            }
            return new ResultDTO { Object = data };
        }
        public async Task<ResultDTO> UpdateRoomType(RoomTypeArgs data)
        {
            var roomTypes = await _uow.GetRepository<RoomType>().FindByAsync(x => x.Id == data.Id);
            if (roomTypes == null)
            {
                return new ResultDTO { Object = data };
            }
            else
            {
                roomTypes.FirstOrDefault().Code = data.Code;
                roomTypes.FirstOrDefault().Name = data.Name;
                roomTypes.FirstOrDefault().Quota = data.Quota;
                _uow.GetRepository<RoomType>().Update(roomTypes);
                await _uow.CommitAsync();
            }
            return new ResultDTO { Object = data };
        }
        public async Task<ResultDTO> GetRoomTypeByCode(QueryArgs arg)
        {
            var result = new ResultDTO();
            var data = arg.PredicateParameters[0].ToString().ToLower();
            if (arg.PredicateParameters[1] == null)
            {
                var roomTypes = await _uow.GetRepository<RoomType>().FindByAsync<RoomTypeViewModel>(x => x.Code.ToLower() == data);
                result.Object = new ArrayResultDTO { Data = roomTypes, Count = roomTypes.Count() };
            }
            else
            {
                var id = new Guid(arg.PredicateParameters[1].ToString().ToLower());
                var roomTypes = await _uow.GetRepository<RoomType>().FindByAsync<RoomTypeViewModel>(x => x.Code.ToLower() == data && x.Id != id);
                result.Object = new ArrayResultDTO { Data = roomTypes, Count = roomTypes.Count() };
            }
            return result;
        }
        #endregion
    }
}
