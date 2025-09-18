using AutoMapper;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using Aeon.HR.Data.Models;
using Aeon.HR.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Handlers
{
    public partial class RecruitmentBO
    {
        public async Task<ResultDTO> UpdateStatusNewStaffOnBoard(UpdateNewStaffOnBoardArgs arg)
        {
            var applicantExit = await _uow.GetRepository<Applicant>().GetSingleAsync(x => x.Id.Equals(arg.ApplicantId));
            applicantExit.ApplicantStatusId= arg.ApplicantStatusId;
            await _uow.CommitAsync();
            return new ResultDTO { Object = Mapper.Map<Applicant>(applicantExit) };
        }

        public async Task<ResultDTO> GetAllNewStaffOnboard(QueryArgs args)
        {
            var items = await _uow.GetRepository<Applicant>().FindByAsync<ApplicantViewModel>(args.Order, args.Page, args.Limit, args.Predicate, args.PredicateParameters);
            var count = await _uow.GetRepository<Applicant>().CountAsync(args.Predicate, args.PredicateParameters);
            return new ResultDTO { Object = new ArrayResultDTO { Data = items, Count = count } }; ;
        }
    }
}