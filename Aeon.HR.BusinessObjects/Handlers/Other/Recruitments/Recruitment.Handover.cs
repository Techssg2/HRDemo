using AutoMapper;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.BusinessObjects.Interfaces;
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
    public partial class RecruitmentBO
    {
        public async Task<ResultDTO> CreateHandover(HandoverDataForCreatingArgs data)
        {
            var existHandover = await _uow.GetRepository<Handover>().FindByAsync(x => x.Id == data.Id);
            if (existHandover.Any())
            {
                return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Handover is exist" } };
            }
            else
            {
                var handover = Mapper.Map<Handover>(data);
                _uow.GetRepository<Handover>().Add(handover);
                foreach (var item in handover.HandoverDetailItems)
                {
                    var handoverDetailItems = Mapper.Map<HandoverDetailItem>(item);
                    _uow.GetRepository<HandoverDetailItem>().Add(handoverDetailItems);
                }
                await _uow.CommitAsync();
                return new ResultDTO { Object = Mapper.Map<HandoverViewModel>(handover) };
            }
        }

        public async Task<ArrayResultDTO> GetListHandovers(QueryArgs args)
        {
            // findbyAsync UserDepartmentMapping truyền userId
            var handovers = await _uow.GetRepository<Handover>().FindByAsync<HandoverViewModel>(args.Order, args.Page, args.Limit, args.Predicate, args.PredicateParameters);
            var count = await _uow.GetRepository<Handover>().CountAsync(args.Predicate, args.PredicateParameters);
            var result = new ArrayResultDTO { Data = handovers, Count = count };
            return result;
        }

        public async Task<ResultDTO> GetListDetailHandover(HandoverDataForCreatingArgs arg)
        {
            var handover = await _uow.GetRepository<Handover>().FindByIdAsync<HandoverViewModel>(arg.Id.Value);
            if (handover != null)
            {
                var items = await _uow.GetRepository<HandoverDetailItem>().FindByAsync<HandoverDetailItemViewModel>(x=>x.HandoverId == arg.Id.Value, "Name");
                handover.HandoverDetailItems = items.ToList();
                return new ResultDTO { Object = handover };
            }
            return new ResultDTO { };
        }

        public async Task<ResultDTO> SearchListHandover(HandoverDataForCreatingArgs data)
        {
            var handovers = await _uow.GetRepository<Handover>().FindByAsync<HandoverViewModel>(x => x.ReferenceNumber == data.ReferenceNumber);
            if (handovers.Any())
            {
                return new ResultDTO { Object = handovers };
            }
            return new ResultDTO { };

        }

        public async Task<ResultDTO> DeleteHandover(HandoverDataForCreatingArgs data)
        {
            var handover = await _uow.GetRepository<Handover>().GetSingleAsync(x => x.Id == data.Id);
            if (handover == null)
            {
                return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Handover is not exist" } };
            }
            else
            {
                _uow.GetRepository<Handover>().Delete(handover);
                await _uow.CommitAsync();
            }
            return new ResultDTO { };
        }

        public async Task<ResultDTO> UpdateHandover(HandoverDataForCreatingArgs data)
        {
            var existHandover = await _uow.GetRepository<Handover>().GetSingleAsync(x => x.Id == data.Id);
            
            if (existHandover == null)
            {
                return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Handover is not exist" } };
            }
            else
            {
                existHandover.ReferenceNumber = data.ReferenceNumber;
                existHandover.ReceivedDate = data.ReceivedDate;
                existHandover.ApplicantId = data.ApplicantId;
                existHandover.UserDeptName = data.UserDeptName;
                existHandover.UserFullName = data.UserFullName;
                existHandover.PositionName = data.PositionName;
                existHandover.LocationName = data.LocationName;
                existHandover.LocationCode = data.LocationCode;
                existHandover.DepartmentType = data.DepartmentType;
                existHandover.JobGradeCaption = data.JobGradeCaption;
                existHandover.DeptDivision = data.DeptDivision;
                existHandover.DeptDivisionCode = data.DeptDivisionCode;
                existHandover.StartDate = data.StartDate;
                existHandover.IsCancel = data.IsCancel;
                _uow.GetRepository<HandoverDetailItem>().Delete(existHandover.HandoverDetailItems);
                foreach (var item in data.HandoverDetailItems)
                {
                    var handoverDetailItems = Mapper.Map<HandoverDetailItem>(item);
                    handoverDetailItems.HandoverId = data.Id.Value;
                    _uow.GetRepository<HandoverDetailItem>().Add(handoverDetailItems);
                }
                await _uow.CommitAsync();
            }
            return new ResultDTO { Object = Mapper.Map<HandoverViewModel>(existHandover) };
        }


        public async Task<ArrayResultDTO> GetHandovers(QueryArgs args)
        {
            // findbyAsync UserDepartmentMapping truyền userId
            var handovers = await _uow.GetRepository<Handover>().FindByAsync<HandoverViewModel>(args.Order, args.Page, args.Limit, args.Predicate, args.PredicateParameters);
            var count = await _uow.GetRepository<Handover>().CountAsync(args.Predicate, args.PredicateParameters);
            return  new ArrayResultDTO { Data = handovers, Count = count };
        }
    }
}