using Aeon.HR.ViewModels.DTOs;
using Aeon.HR.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Aeon.HR.Data.Models;
using AutoMapper;
using Aeon.HR.ViewModels.Args;
using Newtonsoft.Json;
using System.Collections.Generic;
using ServiceStack;
using Aeon.HR.Infrastructure.Enums;

namespace Aeon.HR.BusinessObjects.Handlers
{
    public partial class CBBO
    {
        public async Task<ResultDTO> GetAllMissingTimelockByUser(QueryArgs args)
        {
            //var result = new ResultDTO {  };
            //if (args.QueryArgs.PredicateParameters[0].ToString() != "")
            //{
            //    var userId = Guid.Parse(args.QueryArgs.PredicateParameters[0].ToString());
            //    var missingTimelocks = await _uow.GetRepository<MissingTimeClock>().FindByAsync<MissingTimelockGridViewModel>(args.QueryArgs.Order, args.QueryArgs.Page, args.QueryArgs.Limit, args.QueryArgs.Predicate, args.QueryArgs.PredicateParameters);
            //    var count = await _uow.GetRepository<MissingTimeClock>().CountAsync(args.QueryArgs.Predicate, args.QueryArgs.PredicateParameters);
            //    result.Object = new ArrayResultDTO { Data = missingTimelocks, Count = count };
            //}
            //else
            //{
            //    var missingTimelocks = await _uow.GetRepository<MissingTimeClock>().GetAllAsync<MissingTimelockGridViewModel>(args.QueryArgs.Order, args.QueryArgs.Page, args.QueryArgs.Limit);
            //    var count = await _uow.GetRepository<MissingTimeClock>().CountAllAsync();
            //    result.Object = new ArrayResultDTO { Data = missingTimelocks, Count = count };
            //}
            //return result;
            var missingTimelocks = await _uow.GetRepository<MissingTimeClock>()
                .FindByAsync<MissingTimelockGridViewModel>(args.Order, args.Page, args.Limit, args.Predicate, args.PredicateParameters);
            var count = await _uow.GetRepository<MissingTimeClock>().CountAsync(args.Predicate, args.PredicateParameters);
            return new ResultDTO { Object = new ArrayResultDTO { Data = missingTimelocks, Count = count } };


        }
        public async Task<ResultDTO> GetMissingTimelockByReferenceNumber(MasterdataArgs args)
        {
            var param = args.QueryArgs.PredicateParameters[0].ToString();
            Guid id = new Guid(param);
            var missingTimelocks = await _uow.GetRepository<MissingTimeClock>().FindByAsync<MissingTimelockViewModel>(x => x.Id == id);
            return new ResultDTO { Object = missingTimelocks.FirstOrDefault() };
        }

        public async Task<ResultDTO> SaveMissingTimelock(MissingTimelockArgs data)
        {
            var result = new ResultDTO();
            var department = await _uow.GetRepository<Department>().GetSingleAsync(x => x.UserDepartmentMappings.Any(t => t.User.SAPCode == data.UserSAPCode && t.IsHeadCount), "", x => x.JobGrade);
            
            if (string.IsNullOrEmpty(data.UserSAPCode)  || string.IsNullOrEmpty(data.DeptCode) || string.IsNullOrEmpty(data.DeptName) || (department != null && department.Type == DepartmentType.Division && string.IsNullOrEmpty(data.DivisionCode)))
            {
                return new ResultDTO
                {
                    ErrorCodes = new List<int> { 1002 },
                    Messages = new List<string> { "User information data for ticket creation in the header is missing!" }
                };
            }

            var missingTimelockExist = await _uow.GetRepository<MissingTimeClock>().GetSingleAsync(x => x.Id == data.Id);
            if (missingTimelockExist == null)
            {
                var missingTimelock = Mapper.Map<MissingTimeClock>(data);
                _uow.GetRepository<MissingTimeClock>().Add(missingTimelock);
                missingTimelockExist = missingTimelock;
                result.Object = Mapper.Map<MissingTimelockViewModel>(missingTimelock); ;
            }
            else
            {
                missingTimelockExist.ListReason = data.ListReason;
                missingTimelockExist.Documents = data.Documents;
                var missingTimelockExistDetails = await _uow.GetRepository<MissingTimeClockDetail>().FindByAsync(x => x.MissingTimeClockId == missingTimelockExist.Id);
                if (missingTimelockExistDetails.Any())
                {
                    _uow.GetRepository<MissingTimeClockDetail>().Delete(missingTimelockExistDetails);
                }
                _uow.GetRepository<MissingTimeClock>().Update(missingTimelockExist);
                result.Object = Mapper.Map<MissingTimelockViewModel>(missingTimelockExist);
            }
            var missingTimeClockDetails = JsonConvert.DeserializeObject<List<MissingTimeClockDetail>>(data.ListReason);
            if (missingTimeClockDetails.Any())
            {
                foreach (var detailItem in missingTimeClockDetails)
                {
                    detailItem.MissingTimeClockId = missingTimelockExist.Id;
                    detailItem.Id = Guid.NewGuid();
                }
                _uow.GetRepository<MissingTimeClockDetail>().Add(missingTimeClockDetails);
            }
            await _uow.CommitAsync();
            return result;
        }

        public async Task<ResultDTO> SubmitMissingTimelock(MissingTimelockArgs data)
        {
            var missingTimelockExist = await _uow.GetRepository<MissingTimeClock>().GetSingleAsync(x => x.Id == data.Id);
            if (missingTimelockExist == null)
            {
                var missingTimelock = Mapper.Map<MissingTimeClock>(data);
                _uow.GetRepository<MissingTimeClock>().Add(missingTimelock);
            }
            else
            {
                missingTimelockExist.ListReason = data.ListReason;
                //missingTimelockExist.StatusString = data.Status;
                _uow.GetRepository<MissingTimeClock>().Update(missingTimelockExist);
            }
            await _uow.CommitAsync();
            return new ResultDTO { Object = Mapper.Map<MissingTimelockViewModel>(missingTimelockExist) };
        }

        public async Task<ResultDTO> GetMissingTimelocks(QueryArgs args)
        {
            var missingTimelocks = await _uow.GetRepository<MissingTimeClock>().FindByAsync<MissingTimelockViewModel>(args.Order, args.Page, args.Limit, args.Predicate, args.PredicateParameters);
            var count = await _uow.GetRepository<MissingTimeClock>().CountAsync(args.Predicate, args.PredicateParameters);
            return new ResultDTO { Object = new ArrayResultDTO { Data = missingTimelocks, Count = count } };
        }
    }
}