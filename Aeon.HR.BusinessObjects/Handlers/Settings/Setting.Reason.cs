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
        public async Task<ResultDTO> GetBTAReasons(QueryArgs arg)
        {
            var airlines = await _uow.GetRepository<BTAReason>().FindByAsync<BTAReasonViewModel>(arg.Order, arg.Page, arg.Limit, arg.Predicate, arg.PredicateParameters);
            var total = await _uow.GetRepository<BTAReason>().CountAsync(arg.Predicate, arg.PredicateParameters);
            return new ResultDTO { Object = new ArrayResultDTO { Data = airlines, Count = total } };
        }

        public async Task<ResultDTO> CheckValidateReasons(ReasonArg arg)
        {
            var result = new ResultDTO();
            var reasons = await _uow.GetRepository<BTAReason>().FindByAsync(x => x.Name.ToLower() == arg.Name.ToLower() && !x.IsDeleted);
            result.Object = new ArrayResultDTO { Data = reasons, Count = reasons.Count() };
            return result;
        }

        public async Task<ResultDTO> SaveBTAReason(ReasonArg arg)
        {
            var result = new ResultDTO();
            var ReasonExist = await _uow.GetRepository<BTAReason>().GetSingleAsync(x => x.Id == arg.Id);
            if (ReasonExist == null)
            {
                var reason = Mapper.Map<BTAReason>(arg);
                _uow.GetRepository<BTAReason>().Add(reason);
            }
            else
            {
                var reason = Mapper.Map(arg, ReasonExist);
                _uow.GetRepository<BTAReason>().Update(reason);
            }
            await _uow.CommitAsync();
            return result;
        }

        public async Task<ResultDTO> DeleteReason(Guid id)
        {
            var result = new ResultDTO();
            var reasonExist = await _uow.GetRepository<BTAReason>().GetSingleAsync(x => x.Id == id);
            if (reasonExist != null)
            {
                _uow.GetRepository<BTAReason>().Delete(reasonExist);
                await _uow.CommitAsync();
            }
            return result;
        }
    }
}
