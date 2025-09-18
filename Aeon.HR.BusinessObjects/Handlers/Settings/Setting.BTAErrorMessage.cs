using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Aeon.HR.BusinessObjects.Handlers
{
    public partial class SettingBO
    {
        public async Task<ResultDTO> GetBTAErrorMessageList(QueryArgs arg)
        {
            var itemList = await _uow.GetRepository<BTAErrorMessage>().FindByAsync<BTAErrorMessageViewModel>(arg.Order, arg.Page, arg.Limit, arg.Predicate, arg.PredicateParameters);
            var totalList = await _uow.GetRepository<BTAErrorMessage>().CountAsync(arg.Predicate, arg.PredicateParameters);
            return new ResultDTO { Object = new ArrayResultDTO { Data = itemList, Count = totalList } };
        }
        public async Task<ResultDTO> CreateBTAErrorMessageList(BTAErrorMessageArgs data)
        {

           var existError = _uow.GetRepository<BTAErrorMessage>().GetSingle(x => x.ErrorCode.Equals(data.ErrorCode) && x.Type == data.APIType);
           if (existError != null)
            {
                return new ResultDTO
                {
                    ErrorCodes = new List<int> { 1002 },
                    Messages = new List<string> { "Error Code already exists!" }
                };
            }
            var item = Mapper.Map<BTAErrorMessage>(data);
            item.Type = data.APIType;
            _uow.GetRepository<BTAErrorMessage>().Add(item);
            await _uow.CommitAsync();
            return new ResultDTO { Object = data };
        }
        public async Task<ResultDTO> DeleteBTAErrorMessage(BTAErrorMessageArgs data)
        {
            var item = await _uow.GetRepository<BTAErrorMessage>().FindByAsync(x => x.Id == data.Id);
            if (item == null)
            {
                return new ResultDTO { 
                    ErrorCodes = new List<int> { 1002 },
                    Messages = new List<string> { "Cannot find Item!" }
                };
            }
            else
            {
                _uow.GetRepository<BTAErrorMessage>().Delete(item);
                await _uow.CommitAsync();
            }
            return new ResultDTO { Object = data };
        }
        public async Task<ResultDTO> UpdateBTAErrorMessage(BTAErrorMessageArgs data)
        {
            var item = await _uow.GetRepository<BTAErrorMessage>().GetSingleAsync(x => x.Id == data.Id);
            if (item == null)
            {
                return new ResultDTO
                {
                    ErrorCodes = new List<int> { 1002 },
                    Messages = new List<string> { "Cannot find Item!" }
                };
            }
            else
            {
                var existError = _uow.GetRepository<BTAErrorMessage>().GetSingle(x => x.Id != data.Id && x.ErrorCode.Equals(data.ErrorCode) && x.Type == data.APIType);
                if (existError != null)
                {
                    return new ResultDTO
                    {
                        ErrorCodes = new List<int> { 1002 },
                        Messages = new List<string> { "Error Code already exists!" }
                    };
                }

                item.Type = data.APIType;
                item.ErrorCode = data.ErrorCode;
                item.MessageEN = data.MessageEN;
                item.MessageVI = data.MessageVI;
                _uow.GetRepository<BTAErrorMessage>().Update(item);
                await _uow.CommitAsync();
            }
            return new ResultDTO { Object = data };
        }

        public async Task<BTAErrorMessageViewModel> GetBTAErrorMessageByCode(BTAErrorEnums APIType, string errorCode)
        {
            var item = await _uow.GetRepository<BTAErrorMessage>().GetSingleAsync<BTAErrorMessageViewModel>(x => x.ErrorCode.Equals(errorCode) && x.Type == APIType);
            if (item == null)
            {
                return null;
            }
            return item;
        }
    }
}
