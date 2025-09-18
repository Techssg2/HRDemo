using Aeon.HR.BusinessObjects.Handlers.FIle;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.BTA;
using Aeon.HR.ViewModels.DTOs;
using Aeon.HR.ViewModels.PrintFormViewModel;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Aeon.HR.ViewModels.Args;

namespace Aeon.HR.BusinessObjects.Handlers.CB
{
    public class BTAPolicyBO : IBTAPolicyBO
    {
        private readonly IUnitOfWork _uow;
        private readonly IWorkflowBO _workflowBO;
        private readonly ILogger logger;
        public BTAPolicyBO(IUnitOfWork uow, ILogger _logger, IWorkflowBO workflowBO)
        {
            _uow = uow;
            logger = _logger;
            _workflowBO = workflowBO;
        }
        /// <summary>
        /// Trả về phiếu BTA từ Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        //TODO///////
        //...........
        //Code function for interface here.
        #region BTA Policy
        public async Task<ResultDTO> GetBTAPolicyByDepartment(string typeDepartment)
        {
            var result = new ResultDTO();
            if (typeDepartment == "HQ")
            {
                var item = await _uow.GetRepository<BTAPolicy>().FindByAsync<BTAPolicyViewModel>(x => x.IsStore == false);
                result.Object = new ArrayResultDTO { Data = item, Count = item.Count() };
            }
            else
            {
                var item = await _uow.GetRepository<BTAPolicy>().FindByAsync<BTAPolicyViewModel>(x => x.IsStore == true);
                result.Object = new ArrayResultDTO { Data = item, Count = item.Count() };
            }

            return result;
        }

        public async Task<ResultDTO> GetBTAPolicyList(QueryArgs arg)
        {
            var allBTAPolicy = await _uow.GetRepository<BTAPolicy>().FindByAsync<BTAPolicyViewModel>(arg.Order, arg.Page, arg.Limit, arg.Predicate, arg.PredicateParameters);
            var count = await _uow.GetRepository<BTAPolicy>().CountAsync(arg.Predicate, arg.PredicateParameters);
            return new ResultDTO
            {
                Object = new ArrayResultDTO
                {
                    Data = allBTAPolicy,
                    Count = count
                }
            };
        }
        //public async Task<ResultDTO> GetBTAPolicyByJobGradeId(QueryArgs arg)
        //{
        //    var result = new ResultDTO();
        //    var data = arg.PredicateParameters[0] != null ? new Guid(arg.PredicateParameters[0].ToString().ToUpper()) : new Guid();
        //    if (arg.PredicateParameters[1] == null)
        //    {
        //        var item = await _uow.GetRepository<BTAPolicy>().FindByAsync<BTAPolicyViewModel>(x => x.JobGradeId == data);
        //        result.Object = new ArrayResultDTO { Data = item, Count = item.Count() };
        //    }
        //    else
        //    {
        //        var isStore = Convert.ToBoolean(arg.PredicateParameters[1].ToString());
        //        var item = await _uow.GetRepository<BTAPolicy>().FindByAsync<BTAPolicyViewModel>(x => x.JobGradeId == data && x.IsStore == isStore);
        //        result.Object = new ArrayResultDTO { Data = item, Count = item.Count() };
        //    }
        //    /*else 
        //    {
        //        var isStore = Convert.ToBoolean(arg.PredicateParameters[1].ToString());
        //        var budgetFrom = Convert.ToDecimal(arg.PredicateParameters[2].ToString());
        //        var budgetTo = Convert.ToDecimal(arg.PredicateParameters[3].ToString());
        //        var item = await _uow.GetRepository<BTAPolicy>().FindByAsync<BTAPolicyViewModel>(x => x.JobGradeId == data && x.IsStore == isStore
        //        && x.BudgetFrom == budgetFrom && x.BudgetTo == budgetTo);
        //        result.Object = new ArrayResultDTO { Data = item, Count = item.Count() };
        //    }*/

        //    return result;
        //}
        public async Task<ResultDTO> CreateBTAPolicy(BTAPolicyArgs data)
        {
            var BTAPolicyItem = Mapper.Map<BTAPolicy>(data);
            _uow.GetRepository<BTAPolicy>().Add(BTAPolicyItem);
            await _uow.CommitAsync();
            return new ResultDTO { Object = data };
        }
        public async Task<ResultDTO> UpdateBTAPolicy(BTAPolicyArgs data)
        {
            var item = await _uow.GetRepository<BTAPolicy>().GetSingleAsync(x => x.Id == data.Id);
            if (item == null)
            {
                return new ResultDTO { Object = data };
            }
            else
            {
                item.BudgetFrom = data.BudgetFrom;
                item.BudgetTo = data.BudgetTo;
                item.PartitionId = data.PartitionId;
                item.JobGradeId = data.JobGradeId;
                item.PartitionName = data.PartitionName;
                item.PartitionCode = data.PartitionCode;
                _uow.GetRepository<BTAPolicy>().Update(item);
                await _uow.CommitAsync();
            }
            return new ResultDTO { Object = data };
        }

        public async Task<ResultDTO> DeleteBTAPolicy(BTAPolicyArgs data)
        {
            var item = await _uow.GetRepository<BTAPolicy>().FindByAsync(x => x.Id == data.Id);
            if (item == null)
            {
                return new ResultDTO { Object = data };
            }
            else
            {
                _uow.GetRepository<BTAPolicy>().Delete(item);
                await _uow.CommitAsync();
            }
            return new ResultDTO { Object = data };
        }

        public async Task<ResultDTO> GetBTAPolicyByJobGradePartition(QueryArgs arg)
        {
            var result = new ResultDTO();
            var dataJobGradeId = arg.PredicateParameters[0].ToString().ToLower();
            var dataPartitionId = arg.PredicateParameters[1].ToString().ToLower();
            bool isStore = Convert.ToBoolean(arg.PredicateParameters[3]);
            if (arg.PredicateParameters[2] == null)
            {
                var btaPolicySettings = await _uow.GetRepository<BTAPolicy>().FindByAsync<BTAPolicyViewModel>(x => x.JobGradeId.ToString().ToLower() == dataJobGradeId && x.PartitionId.ToString().ToLower() == dataPartitionId && x.IsStore == isStore);
                result.Object = new ArrayResultDTO { Count = btaPolicySettings.Count() };
            }
            else
            {
                var id = arg.PredicateParameters[2].ToString().ToLower();
                var btaPolicySettings = await _uow.GetRepository<BTAPolicy>().FindByAsync<BTAPolicyViewModel>(x => x.JobGradeId.ToString().ToLower() == dataJobGradeId && x.PartitionId.ToString().ToLower() == dataPartitionId && x.Id.ToString().ToLower() != id && x.IsStore == isStore);
                result.Object = new ArrayResultDTO { Count = btaPolicySettings.Count() };
            }
            return result;
        }
        #endregion
        #region BTA Policy Special Cases
        public async Task<ResultDTO> GetListBTAPolicySpecialCases(QueryArgs arg)
        {
            var allBTAPolicySpecial = await _uow.GetRepository<BTAPolicySpecial>().FindByAsync<BTAPolicySpecialViewModel>(arg.Order, arg.Page, arg.Limit, arg.Predicate, arg.PredicateParameters);
            var count = await _uow.GetRepository<BTAPolicySpecial>().CountAsync(arg.Predicate, arg.PredicateParameters);
            return new ResultDTO
            {
                Object = new ArrayResultDTO
                {
                    Data = allBTAPolicySpecial,
                    Count = count
                }
            };
        }
        public async Task<ResultDTO> GetBTAPolicySpecialCasesByUserSAPCode(string userSapCode, Guid partitionId)
        {
            var result = new ResultDTO();

            var item = await _uow.GetRepository<BTAPolicySpecial>().FindByAsync<BTAPolicySpecialViewModel>(x => x.SapCode == userSapCode && x.PartitionId.ToString() == partitionId.ToString());
            result.Object = new ArrayResultDTO { Data = item, Count = item.Count() };

            return result;
        }
        public async Task<ResultDTO> CreateBTAPolicySpecialCases(BTAPolicySpecialArgs data)
        {
            BTAPolicySpecial detail = new BTAPolicySpecial
            {
                SapCode = data.SapCode,
                FullName = data.FullName,
                UserId = data.UserId,
                DepartmentId = data.DepartmentId,
                PositionName = data.PositionName,
                JobGradeId = data.JobGradeId,
                BudgetFrom = data.BudgetFrom,
                BudgetTo = data.BudgetTo,
                PartitionId = data.PartitionId
            };

            _uow.GetRepository<BTAPolicySpecial>().Add(detail);
            _uow.Commit();

            return new ResultDTO { Object = data };
        }
        public async Task<ResultDTO> UpdateBTAPolicySpecialCases(BTAPolicySpecialArgs data)
        {
            var item = await _uow.GetRepository<BTAPolicySpecial>().GetSingleAsync(x => x.Id == data.Id);
            if (item == null)
            {
                return new ResultDTO { Object = data };
            }
            else
            {
                item.BudgetFrom = data.BudgetFrom;
                item.BudgetTo = data.BudgetTo;
                item.PartitionId = data.PartitionId;
                _uow.GetRepository<BTAPolicySpecial>().Update(item);
                await _uow.CommitAsync();
            }
            return new ResultDTO { Object = data };
        }
        public async Task<ResultDTO> DeleteBTAPolicySpecialCases(BTAPolicySpecialArgs data)
        {
            var item = await _uow.GetRepository<BTAPolicySpecial>().FindByAsync(x => x.Id == data.Id);
            if (item == null)
            {
                return new ResultDTO { Object = data };
            }
            else
            {
                _uow.GetRepository<BTAPolicySpecial>().Delete(item);
                await _uow.CommitAsync();
            }
            return new ResultDTO { Object = data };
        }

        public Task<ResultDTO> GetBTAPolicyByJobGradeId(QueryArgs arg)
        {
            throw new NotImplementedException();
        }

        public Task<ResultDTO> GetBTAPolicySpecialCasesByUserSAPCode(string userSapCode)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
