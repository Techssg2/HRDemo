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
using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.Infrastructure.Enums;

namespace Aeon.HR.BusinessObjects.Handlers
{
    public partial class SettingBO
    {

        public async Task<ResultDTO> GetHeadCountList(QueryArgs args)
        {
            args.Order = "Department.Name asc,JobGradeForHeadCount.Grade desc";
            var headCountVm = await _uow.GetRepository<HeadCount>()
                .FindByAsync<ItemListHeadCountViewModel>(args.Order, args.Page, args.Limit, args.Predicate, args.PredicateParameters);
            var count = await _uow.GetRepository<HeadCount>().CountAsync(args.Predicate, args.PredicateParameters);
            var tempGroups = headCountVm.GroupBy(x => x.DepartmentName.Substring(3));
            var newList = new List<ItemListHeadCountViewModel>();
            foreach (var group in tempGroups)
            {
                newList.AddRange(group.OrderByDescending(y => y.JobGradeValue).ToList());
            }
            ResultDTO result = new ResultDTO
            {
                Object = new ArrayResultDTO { Data = newList, Count = count }
            };
            return result;
        }
        public async Task<ResultDTO> GetHeadCountByDepartmentId(Guid id, int jobGrade)
        {
            var currentDepartment = await _uow.GetRepository<Department>().FindByIdAsync(id);
            //if (currentDepartment.Type != DepartmentType.Department)
            //{
            //    var currentType = currentDepartment.Type;
            //    while (currentType != DepartmentType.Department && currentDepartment.ParentId.HasValue)
            //    {
            //        currentType = currentDepartment.Parent.Type;
            //        currentDepartment = currentDepartment.Parent;
            //    }
            //}
            //if (currentDepartment.Type == DepartmentType.Department)
            //{
            //    var headCounts = await _uow.GetRepository<HeadCount>().FindByAsync<ItemListHeadCountViewModel>(x => x.DepartmentId == currentDepartment.Id);
            //    if (!headCounts.Any())
            //    {
            //        var tempHeadCounts = new List<ItemListHeadCountViewModel>();
            //        while (!tempHeadCounts.Any())
            //        {

            //        }
            //    }
            //    if (headCounts.Any())
            //    {
            //        var pendingStatus = Utilities.PendingStatuses();
            //        foreach (var headCount in headCounts)
            //        {
            //            var pendingPositionForThisDepartments = await _uow.GetRepository<RequestToHire>().FindByAsync(x => x.DeptDivisionId == id && !pendingStatus.Contains(x.Status) && x.JobGradeId == headCount.JobGradeForHeadCountId);
            //            if (pendingPositionForThisDepartments.Any())
            //            {
            //                headCount.Quantity = headCount.Quantity - pendingPositionForThisDepartments.Sum(x => x.Quantity);
            //            }
            //        }

            //        return new ResultDTO { Object = new ArrayResultDTO { Data = headCounts, Count = headCounts.Count() } };
            //    }

            //}
            if (currentDepartment != null)
            {
                var headCounts = await _uow.GetRepository<HeadCount>().FindByAsync<ItemListHeadCountViewModel>(x => x.DepartmentId == currentDepartment.Id);
                if (!headCounts.Any())
                {
                    IEnumerable<ItemListHeadCountViewModel> tempHeadCounts = null;
                    while ((tempHeadCounts == null || !tempHeadCounts.Any()) && currentDepartment.ParentId.HasValue)
                    {
                        currentDepartment = currentDepartment.Parent;
                        tempHeadCounts = await _uow.GetRepository<HeadCount>().FindByAsync<ItemListHeadCountViewModel>(x => x.DepartmentId == currentDepartment.Id);
                    }
                    headCounts = tempHeadCounts;
                    if (headCounts != null)
                    {
                        var pendingStatus = Utilities.PendingStatuses();
                        foreach (var headCount in headCounts)
                        {
                            var pendingPositionForThisDepartments = await _uow.GetRepository<RequestToHire>().FindByAsync(x => !pendingStatus.Contains(x.Status) && x.DeptDivisionId == currentDepartment.Id && x.JobGradeId == headCount.JobGradeForHeadCountId && x.JobGradeGrade == jobGrade);
                            if (pendingPositionForThisDepartments.Any())
                            {

                                headCount.Quantity -= pendingPositionForThisDepartments.Sum(x => x.Quantity);
                                if (headCount.Quantity < 0)
                                {
                                    headCount.Quantity = 0;
                                }
                            }
                        }
                    }

                }
                else
                {
                    var pendingStatus = Utilities.PendingStatuses();
                    foreach (var headCount in headCounts)
                    {
                        var pendingPositionForThisDepartments = await _uow.GetRepository<RequestToHire>().FindByAsync(x => x.DeptDivisionId == id && !pendingStatus.Contains(x.Status) && x.JobGradeId == headCount.JobGradeForHeadCountId && x.JobGradeGrade == jobGrade && x.HasBudget == CheckBudgetOption.Budget);
                        if (pendingPositionForThisDepartments.Any())
                        {

                            headCount.Quantity = headCount.Quantity - pendingPositionForThisDepartments.Sum(x => x.Quantity);
                            if (headCount.Quantity < 0)
                            {
                                headCount.Quantity = 0;
                            }
                        }
                    }
                }
                return new ResultDTO { Object = new ArrayResultDTO { Data = headCounts, Count = headCounts != null ? headCounts.Count() : 0 } };
            }
            return new ResultDTO { Object = new ArrayResultDTO { Data = null, Count = 0 } };
        }
        public async Task<ResultDTO> CreateHeadCount(HeadCountArgs model)
        {
            var existing = await _uow.GetRepository<HeadCount>().GetSingleAsync(x => x.DepartmentId == model.DepartmentId && x.JobGradeForHeadCountId == model.JobGradeForHeadCountId);
            if (existing != null)
            {
                return new ResultDTO { ErrorCodes = { 2002 }, Messages = { "A department can only have 1 Headcount!" } };
            }
            var HeadCount = Mapper.Map<HeadCount>(model);
            _uow.GetRepository<HeadCount>().Add(HeadCount);
            await _uow.CommitAsync();
            return new ResultDTO { };
        }

        public async Task<ResultDTO> UpdateHeadCount(HeadCountArgs model)
        {
            var headCount = await _uow.GetRepository<HeadCount>().FindByIdAsync(model.Id.Value);
            if (headCount == null)
            {
                return new ResultDTO { ErrorCodes = { 2002 }, Messages = { "HeadCount not found !" } };
            }
            else
            {
                var existing = await _uow.GetRepository<HeadCount>()
                    .GetSingleAsync(x => x.DepartmentId == model.DepartmentId && x.Id != model.Id && x.JobGradeForHeadCountId == model.JobGradeForHeadCountId);
                if (existing != null)
                {
                    return new ResultDTO { ErrorCodes = { 2002 }, Messages = { "A department can only have 1 Headcount!" } };
                }
                headCount.Quantity = model.Quantity;
                headCount.DepartmentId = model.DepartmentId;
                headCount.JobGradeForHeadCountId = model.JobGradeForHeadCountId;

                await _uow.CommitAsync();
                return new ResultDTO { };
            }

        }

        public async Task<ResultDTO> DeleteHeadCount(Guid Id)
        {
            var HeadCount = await _uow.GetRepository<HeadCount>().FindByIdAsync(Id);
            if (HeadCount == null)
            {
                return new ResultDTO { ErrorCodes = { 111 }, Messages = { "Not found HeadCount with id " + Id }, };
            }
            else
            {
                _uow.GetRepository<HeadCount>().Delete(HeadCount);
                await _uow.CommitAsync();
            }
            return new ResultDTO { };
        }
        public async Task<ResultDTO> GetDepartmentListForHeadCount()
        {
            var lstDepartment = await _uow.GetRepository<Department>().GetAllAsync();
            List<ItemListDepartmentForHeadCountViewModel> vmLstDepartment = Mapper.Map<List<ItemListDepartmentForHeadCountViewModel>>(lstDepartment);
            ResultDTO result = new ResultDTO
            {
                Object = new ArrayResultDTO
                {
                    Data = vmLstDepartment,
                    Count = vmLstDepartment.Count
                },
            };
            return result;
        }

    }
}