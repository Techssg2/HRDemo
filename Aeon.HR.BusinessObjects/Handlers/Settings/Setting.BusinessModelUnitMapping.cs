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
using System.Configuration;

namespace Aeon.HR.BusinessObjects.Handlers
{
    public partial class SettingBO
    {
        public async Task<ResultDTO> GetBusinessModelUnitMappingList(QueryArgs args)
        {
            var businessModelUnit = await _uow.GetRepository<BusinessModelUnitMapping>()
                                         .FindByAsync<BusinessModelUnitMappingViewModel>(args.Order, args.Page, args.Limit, args.Predicate, args.PredicateParameters);
            foreach (var VARIABLE in businessModelUnit)
            {
                var findMasterData = await _uow.GetRepository<MasterData>().FindByIdAsync(VARIABLE.BusinessModelId);
            }
            var count = await _uow.GetRepository<BusinessModelUnitMapping>().CountAsync(args.Predicate, args.PredicateParameters);
            var result = new ArrayResultDTO { Data = businessModelUnit, Count = count };
            return new ResultDTO { Object = result };
        }
        public async Task<ResultDTO> CreateBusinessModelUnitMapping(BusinessModelUnitMappingArgs args)
        {
            var result = new ResultDTO() { };
            if (args is null)
            {
                result.ErrorCodes = new List<int> { -1 };
                result.Messages = new List<string> { "Param is null!" };
                goto Finish;
            }

            if (!args.BusinessModelId.HasValue)
            {
                result.ErrorCodes = new List<int> { -1 };
                result.Messages = new List<string> { "Business Model is require!" };
                goto Finish;
            }

            if (string.IsNullOrEmpty(args.BusinessUnitCode))
            {
                result.ErrorCodes = new List<int> { -1 };
                result.Messages = new List<string> { "Business Unit is require!" };
                goto Finish;
            }

            if (!args.IsStore.HasValue)
            {
                result.ErrorCodes = new List<int> { -1 };
                result.Messages = new List<string> { "Is Store is require!" };
                goto Finish;
            }


            var existsBusinessModelUnit = await _uow.GetRepository<BusinessModelUnitMapping>().GetSingleAsync(x => x.BusinessUnitCode.Equals(args.BusinessUnitCode, StringComparison.OrdinalIgnoreCase) && x.BusinessModelId == args.BusinessModelId.Value);
            if (!(existsBusinessModelUnit is null))
            {
                result.ErrorCodes = new List<int> { -1 };
                result.Messages = new List<string> { "Business Model Unit Mapping is exists!" };
                goto Finish;
            }



            if (result.IsSuccess)
            {
                var businessModelUnitMapping = Mapper.Map<BusinessModelUnitMapping>(args);
                _uow.GetRepository<BusinessModelUnitMapping>().Add(businessModelUnitMapping);
                result.Object = Mapper.Map<BusinessModelUnitMappingViewModel>(businessModelUnitMapping);
                await _uow.CommitAsync();
            }
        Finish:
            return result;
        }
        public async Task<ResultDTO> UpdateBusinessModelUnitMapping(BusinessModelUnitMappingArgs args)
        {
            var result = new ResultDTO() { };
            if (!args.Id.HasValue)
            {
                result.ErrorCodes = new List<int> { -1 };
                result.Messages = new List<string> { "Business Model Unit ID is require!" };
                goto Finish;
            }
            var currentBusinessModelUnit = await _uow.GetRepository<BusinessModelUnitMapping>().GetSingleAsync(x => x.Id == args.Id.Value);
            if (currentBusinessModelUnit is null)
            {
                result.ErrorCodes = new List<int> () { -1 };
                result.Messages = new List<string> () { "Business Model Unit Mapping is not exist" };
                goto Finish;
            }
            else
            {
                var currentBusinessModelUnitMapping = await _uow.GetRepository<BusinessModelUnitMapping>().GetSingleAsync(x => x.Id != args.Id.Value && x.BusinessUnitCode.Equals(args.BusinessUnitCode, StringComparison.OrdinalIgnoreCase) && x.BusinessModelId == args.BusinessModelId.Value);
                if (!(currentBusinessModelUnitMapping is null))
                {
                    result.ErrorCodes = new List<int> { -1 };
                    result.Messages = new List<string> { "Business Model Unit Mapping is exists!" };
                    goto Finish;
                }
                if (args.BusinessModelId.HasValue)
                    currentBusinessModelUnit.BusinessModelId = args.BusinessModelId.Value;
                currentBusinessModelUnit.BusinessUnitCode = args.BusinessUnitCode;
                currentBusinessModelUnit.BusinessModelCode = args.BusinessModelCode;
                currentBusinessModelUnit.IsStore = args.IsStore != null && (bool) args.IsStore;
                currentBusinessModelUnitMapping = Mapper.Map<BusinessModelUnitMapping>(args);
                _uow.GetRepository<BusinessModelUnitMapping>().Update(currentBusinessModelUnit);
                await _uow.CommitAsync();
                result.Object = Mapper.Map<BusinessModelUnitMappingViewModel>(currentBusinessModelUnit);
            }
            Finish:
            return result;
        }
        public async Task<ResultDTO> DeleteBusinessModelUnitMapping(Guid Id)
        {
            var result = new ResultDTO { };
            var businessModelUnit = await _uow.GetRepository<BusinessModelUnitMapping>().FindByIdAsync(Id);
            if (businessModelUnit is null)
            {
                result.ErrorCodes = new List<int>() { -1 };
                result.Messages = new List<string>() { "Not found Business Model Unit with id: " + Id };
            }
            else
            {
                businessModelUnit.IsDeleted = true;
                _uow.GetRepository<BusinessModelUnitMapping>().Update(businessModelUnit);
                await _uow.CommitAsync();
                result.Object = Mapper.Map<BusinessModelUnitMappingViewModel>(businessModelUnit);
            }
            return result;
        }

        public async Task<ResultDTO> GetBusinessModelUnitMappingById(Guid id)
        {
            var businessModel = await _uow.GetRepository<BusinessModelUnitMapping>().FindByIdAsync<BusinessModelUnitMappingViewModel>(id);
            return new ResultDTO { Object = businessModel };
        }
    }
}