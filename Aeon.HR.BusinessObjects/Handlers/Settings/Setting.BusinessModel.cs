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

        private readonly string FieldCheckWorkflow = ConfigurationManager.AppSettings["BM_CheckWorkflow"] != null ? ConfigurationManager.AppSettings["BM_CheckWorkflow"] : "BusinessModelCode";
        public async Task<ResultDTO> GetBusinessModelList(QueryArgs args)
        {
            var businessModel = await _uow.GetRepository<BusinessModel>()
                                         .FindByAsync<BusinessModelViewModel>(args.Order, args.Page, args.Limit, args.Predicate, args.PredicateParameters);
            var count = await _uow.GetRepository<BusinessModel>().CountAsync(args.Predicate, args.PredicateParameters);
            var result = new ArrayResultDTO { Data = businessModel, Count = count };
            return new ResultDTO { Object = result };
        }
        public async Task<ResultDTO> UpdateBusinessModel(BusinessModelArgs args)
        {
            var result = new ResultDTO() { };
            var currentBusinessModel = await _uow.GetRepository<BusinessModel>().GetSingleAsync(x => x.Id == args.Id);
            if (currentBusinessModel is null)
            {
                result.ErrorCodes = new List<int> () { -1 };
                result.Messages = new List<string> () { "Business Model is not exist" };
                goto Finish;
            }
            else
            {
                // check workflow
                if (!string.IsNullOrEmpty(currentBusinessModel.Code))
                {
                    bool isCanEditWorkflow = true;
                    var existsWorkflow = await this.CheckExistBusinessModelCodeInWorkflow(currentBusinessModel.Code);
                    if (existsWorkflow)
                        isCanEditWorkflow = false;

                    /*var existsDepartment = await this.CheckExistBusinessModelCodeInDepartment(currentBusinessModel.Id);
                    if (existsDepartment)
                        isCanEditWorkflow = false;*/

                    if (isCanEditWorkflow)
                    {
                        currentBusinessModel.Code = args.Code;
                    }
                }

                if (!string.IsNullOrEmpty(args.Name))
                {
                    currentBusinessModel.Name = args.Name;
                }
                var existsBusinessModel = await _uow.GetRepository<BusinessModel>().GetSingleAsync(x => x.Id != args.Id && !string.IsNullOrEmpty(x.Code) && x.Code.ToLower().Equals(args.Code.ToLower()));
                if (!(existsBusinessModel is null))
                {
                    result.ErrorCodes = new List<int> { -1 };
                    result.Messages = new List<string> { "Business Model Code is exists!" };
                    goto Finish;
                }
                _uow.GetRepository<BusinessModel>().Update(currentBusinessModel);
                await _uow.CommitAsync();
                result.Object = Mapper.Map<BusinessModelViewModel>(currentBusinessModel);
            }
            Finish:
            return result;
        }
        public async Task<ResultDTO> DeleteBusinessModel(Guid Id)
        {
            var result = new ResultDTO { };
            var businessModel = await _uow.GetRepository<BusinessModel>().FindByIdAsync(Id);
            if (businessModel is null)
            {
                result.ErrorCodes = new List<int>() { -1 };
                result.Messages = new List<string>() { "Not found Business Model with id: " + Id };
            }
            else
            {
                // check workflow
                var existsWorkflow = await this.CheckExistBusinessModelCodeInWorkflow(businessModel.Code);
                if (existsWorkflow)
                {
                    result.ErrorCodes = new List<int>() { -1 };
                    result.Messages = new List<string>() { "Business Model Code is used in Workflow!" };
                    goto Finish;
                }

                var existsDepartment = await this.CheckExistBusinessModelCodeInDepartment(businessModel.Id);
                if (existsDepartment)
                {
                    result.ErrorCodes = new List<int>() { -1 };
                    result.Messages = new List<string>() { "Business Model Code is used in Department!" };
                    goto Finish;
                }

                businessModel.IsDeleted = true;
                _uow.GetRepository<BusinessModel>().Update(businessModel);
                await _uow.CommitAsync();
                result.Object = Mapper.Map<BusinessModelViewModel>(businessModel);
            }
            Finish:
            return result;
        }

        public async Task<ResultDTO> CreateBusinessModel(BusinessModelArgs args)
        {
            var result = new ResultDTO() { };
            if (args is null)
            {
                result.ErrorCodes = new List<int> { -1 };
                result.Messages = new List<string> { "Param is null!" };
                goto Finish;
            }

            if (string.IsNullOrEmpty(args.Name))
            {
                result.ErrorCodes = new List<int> { -1 };
                result.Messages = new List<string> { "Name is required!" };
                goto Finish;
            }

            var existsBusinessModel = await _uow.GetRepository<BusinessModel>().GetSingleAsync(x => !string.IsNullOrEmpty(x.Code) && x.Code.ToLower().Equals(args.Code.ToLower()));
            if (!(existsBusinessModel is null))
            {
                result.ErrorCodes = new List<int> { -1 };
                result.Messages = new List<string> { "Business Model Code is exists!" };
                goto Finish;
            }

            if (result.IsSuccess)
            {
                var businessModel = Mapper.Map<BusinessModel>(args);
                _uow.GetRepository<BusinessModel>().Add(businessModel);
                result.Object = Mapper.Map<BusinessModelViewModel>(businessModel);
                await _uow.CommitAsync();
            }
            Finish:
            return result;
        }
        public async Task<ResultDTO> GetBusinessModelById(Guid id)
        {
            var businessModel = await _uow.GetRepository<BusinessModel>().FindByIdAsync<BusinessModelViewModel>(id);
            return new ResultDTO { Object = businessModel };
        }

        public async Task<bool> CheckExistBusinessModelCodeInWorkflow(string businessModelCode)
        {
            var rtValue = false;
            var allWorkflows = await _uow.GetRepository<WorkflowTemplate>().FindByAsync(x => x.IsActivated);
            if (!(allWorkflows is null))
            {
                rtValue = allWorkflows.Any(x => x.WorkflowData != null && x.WorkflowData.StartWorkflowConditions.Any(y => (!string.IsNullOrEmpty(y.FieldName)) && y.FieldName == FieldCheckWorkflow && y.FieldValues.Contains(businessModelCode))) 
                    ? true : false;
            }
            return rtValue;
        }

        public async Task<bool> CheckExistBusinessModelCodeInDepartment(Guid businessModelId)
        {
            var departments = await _uow.GetRepository<Department>().FindByAsync(x => x.BusinessModel != null && x.BusinessModelId == businessModelId);
            return departments.Any() ? true : false;
        }
    }
}