using AutoMapper;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Data.Models;
using Aeon.HR.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Transactions;
using Newtonsoft.Json;
using Aeon.HR.Infrastructure.Interfaces;
using System.Configuration;
using Microsoft.Extensions.Logging;

namespace Aeon.HR.BusinessObjects.Handlers
{

    public partial class RecruitmentBO
    {
        public async Task<ArrayResultDTO> GetAllListPositionForFilter()
        {
            var items = await _uow.GetRepository<Position>().GetAllAsync<PositionForFilterViewModel>();
            var result = new ArrayResultDTO { Data = items };
            return result;
        }

        public async Task<ArrayResultDTO> GetListPosition(QueryArgs args)
        {
            var items = await _uow.GetRepository<Position>().FindByAsync<PositionViewModel>(args.Order, args.Page, args.Limit, args.Predicate, args.PredicateParameters);
            var count = await _uow.GetRepository<Position>().CountAsync(args.Predicate, args.PredicateParameters);
            var result = new ArrayResultDTO { Data = items, Count = count };
            return result;
        }

        public async Task<ArrayResultDTO> GetOpenPositions()
        {
            var items = await _uow.GetRepository<Position>(true).FindByAsync<OpenPositionViewModel>(x => x.Status == PositionStatus.Opened, "Created desc");
            var result = new ArrayResultDTO { Data = items };
            return result;
        }


        public async Task<ArrayResultDTO> GetListPositionDetail(QueryArgs args)
        {
            var items = await _uow.GetRepository<Applicant>().FindByAsync<PrositionApplicantViewModel>(args.Order, args.Page, args.Limit, args.Predicate, args.PredicateParameters);
            var count = await _uow.GetRepository<Applicant>().CountAsync(args.Predicate, args.PredicateParameters);

            var result = new ArrayResultDTO { Data = items, Count = count };
            return result;
        }

        public async Task<ResultDTO> CreateNewPosition(PositionForCreatingArgs data)
        {
            //var existPosition = await _uow.GetRepository<Position>().FindByAsync(x => x.PositionName == data.PositionName);
            //if (existPosition.Any())
            //{
            //    return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Position is exist" } };
            //}
            //else
            //{
            var position = Mapper.Map<Position>(data);
            _uow.GetRepository<Position>().Add(position);
            await _uow.CommitAsync();
            //}
            return new ResultDTO { Object = Mapper.Map<PositionViewModel>(position) };
        }

        public async Task<ResultDTO> UpdatePosition(PositionForCreatingArgs args)
        {
            var existPosition = await _uow.GetRepository<Position>().GetSingleAsync(x => x.Id == args.Id, string.Empty);
            if (existPosition == null)
            {
                return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Position is not exist" } };
            }
            else
            {
                existPosition.PositionName = args.PositionName;
                existPosition.DeptDivisionId = args.DeptDivisionId;
                existPosition.LocationCode = args.LocationCode;
                existPosition.LocationName = args.LocationName;
                existPosition.ExpiredDay = args.ExpiredDay;
                existPosition.HasBudget = args.HasBudget;
                existPosition.Quantity = args.Quantity;
                existPosition.AssignToId = args.AssignToId;
                existPosition.Status = args.Status;

                await _uow.CommitAsync();
            }
            return new ResultDTO { Object = Mapper.Map<PositionViewModel>(existPosition) };
        }

        public async Task<ResultDTO> DeletePosition(Guid id)
        {
            bool existPosition = _uow.GetRepository<Position>().Any(x => x.Id == id);
            if (!existPosition)
            {
                return new ResultDTO
                {

                    ErrorCodes = { 404 },
                    Messages = { $"Position id {id} is not found!" },
                };
            }
            else
            {
                var position = _uow.GetRepository<Position>().FindById(id);
                await _uow.CommitAsync();
                return new ResultDTO { };
            }
        }

        public async Task<ResultDTO> ChangeStatus(PositionStatusArgs data)
        {
            var existPosition = await _uow.GetRepository<Position>().FindByIdAsync(data.PositionId);
            if (existPosition == null)
            {
                return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Position is not exist." } };
            }
            else
            {
                if (data.Status.Equals(PositionStatus.Draft))
                {
                    _uow.GetRepository<Position>().Delete(existPosition);
                }
                else
                {
                    existPosition.Status = data.Status;
                }
                await _uow.CommitAsync();
            }
            return new ResultDTO { };
        }

        //public async Task<ArrayResultDTO> GetAllNewStaffOnboard(StatusApplicantArgs args)
        //{
        //    var items = await _uow.GetRepository<PositionApplicantMapping>().FindByAsync<PositionApplicantMappingViewModel>(args.QueryArgs.Order, args.QueryArgs.Page, args.QueryArgs.Limit, x => !x.IsDeleted && x.Applicant.ApplicantStatus.Code.Equals(args.StatusName));
        //    var count = await _uow.GetRepository<PositionApplicantMapping>().CountAsync(x => !x.IsDeleted && x.Applicant.ApplicantStatus.Code.Equals(args.StatusName));
        //    var result = new ArrayResultDTO { Data = items, Count = count };
        //    return result;
        //}

        public async Task<ArrayResultDTO> GetPositionMappingApplicant(QueryArgs args)
        {
            var items = await _uow.GetRepository<Position>().FindByAsync<PositionViewModel>(args.Predicate, args.PredicateParameters);
            var result = new ArrayResultDTO { Data = items, Count = items.Count() };
            return result;
        }
        public async Task<ArrayResultDTO> GetPositionForActing(QueryArgs args)
        {
            var items = await _uow.GetRepository<Position>().FindByAsync<PositionViewModel>(args.Predicate, args.PredicateParameters);
            var result = new ArrayResultDTO { Data = items, Count = items.Count() };
            return result;
        }

        public async Task<ResultDTO> ReAssigneeAsync(ReAssigneeInPositionArgs args)
        {
            var result = new ResultDTO();
            var positionInDb = await _uow.GetRepository<Position>().FindByIdAsync(args.PositionId);
            if (positionInDb == null)
            {
                result.Messages.Add("Position is not found");
                return result;
            }
            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var saveTrackingLogResult = await this.SaveTrackingLog(args.TrackingLogArgs);
                    if (saveTrackingLogResult.IsSuccess)
                    {
                        // update postion
                        positionInDb.AssignToId = args.UserId;
                        _uow.GetRepository<Position>().Update(positionInDb);
                        // update request to hire                   
                        dynamic oldAssignee = JsonConvert.DeserializeObject<Object>(args.TrackingLogArgs.OldAssignee);
                        Guid? oldUserId = oldAssignee["id"];
                        var assignedToDept = await _uow.GetRepository<Department>().GetSingleAsync(x => x.UserDepartmentMappings.Any(u => u.UserId == args.UserId && u.IsHeadCount && u.Role == Group.Member));

                        if (oldUserId.HasValue)
                        {
                            var permissionPosition = await _uow.GetRepository<Permission>().GetSingleAsync(i => i.ItemId == positionInDb.Id && i.UserId == oldUserId);
                            var permissionRTH = await _uow.GetRepository<Permission>().GetSingleAsync(i => i.ItemId == positionInDb.RequestToHireId.Value && i.UserId == oldUserId);
                            if (permissionPosition != null)
                            {
                                permissionPosition.UserId = args.UserId;
                                permissionPosition.DepartmentId = assignedToDept?.Id;
                                permissionPosition.DepartmentType = Group.Member;
                                permissionPosition.Perm = Right.Edit;
                                _uow.GetRepository<Permission>().Update(permissionPosition);

                            }
                            else
                            {
                                _uow.GetRepository<Permission>().Add(new Permission()
                                {
                                    UserId = args.UserId,
                                    DepartmentId = assignedToDept?.Id,
                                    DepartmentType = Group.Member,
                                    ItemId = positionInDb.Id,
                                    Perm = Right.Edit
                                });
                            }

                            if (permissionRTH != null)
                            {
                                permissionRTH.UserId = args.UserId;
                                permissionRTH.DepartmentId = assignedToDept?.Id;
                                permissionRTH.DepartmentType = Group.Member;
                                permissionRTH.Perm = Right.View;
                                _uow.GetRepository<Permission>().Update(permissionRTH);
                            }
                            else
                            {
                                _uow.GetRepository<Permission>().Add(new Permission()
                                {
                                    UserId = args.UserId,
                                    DepartmentId = assignedToDept?.Id,
                                    DepartmentType = Group.Member,
                                    ItemId = positionInDb.RequestToHireId.Value,
                                    Perm = Right.View
                                });
                            }

                        }

                        await _uow.CommitAsync();
                        result.Object = new { assigneeId = args.UserId, positionId = args.PositionId };
                        transactionScope.Complete();
                    }

                }
                catch (Exception ex)
                {
                    result.ErrorCodes.Add(1004);
                    result.Messages.Add("System Error");
                    return result;
                }

            }

            return result;
        }

        async Task<Position> FindPositionByIdAsync(Guid positionId)
            => await _uow.GetRepository<Position>().FindByIdAsync(positionId);
        public async Task<ResultDTO> SendEmailToAssignee(ILogger logger, Guid? assigneeId, Guid positionId)
        {
            var result = new ResultDTO();
            if (assigneeId.HasValue)
            {
                try
                {
                    var currentPosition = await _uow.GetRepository<Position>().FindByIdAsync(positionId);
                    if (currentPosition != null)
                    {
                        EmailNotification emailNotification = new EmailNotification(logger, _uow);
                        EmailTemplateName type = EmailTemplateName.ForAssigneeCompleted;
                        User user = await _uow.GetRepository<User>().FindByIdAsync(assigneeId.Value);
                        //logger.LogInformation($"Send email notification to {user.Email}");
                        var mergeFields = new Dictionary<string, string>();
                        mergeFields["AssignmentName"] = user.FullName;
                        mergeFields["PositionRefereceNumber"] = currentPosition.ReferenceNumber;
                        mergeFields["Link"] = $"<a href=\"{ Convert.ToString(ConfigurationManager.AppSettings["siteUrl"])}/_layouts/15/AeonHR/Default.aspx#!/home/position/allRequests/view=\" >Link</a>";
                        var recipients = new List<string>() { user.Email };
                        await emailNotification.SendEmail(type, EmailTemplateName.ForAssigneeCompleted, mergeFields, recipients);

                    }
                    else
                    {
                        logger.LogError($"Position is not found");
                        result.Messages.Add($"Position {positionId} is not found");
                    }

                }
                catch (Exception ex)
                {
                    logger.LogError($"Send email notification to assignee is fail: {ex.Message}");
                    result.Messages.Add(ex.Message);
                }
            }
            return result;
        }

        public async Task<ResultDTO> GetPositionById(Guid id)
        {
            var result = new ResultDTO();
            var position = await _uow.GetRepository<Position>().FindByIdAsync(id);
            if (position != null)
            {
                result.Object = Mapper.Map<PositionViewModel>(position);
            }
            else
            {
                var applicant = await _uow.GetRepository<Applicant>().FindByIdAsync(id, x => x.Position);
                if (applicant != null)
                {
                    position = applicant.Position;
                    if (position != null)
                    {
                        result.Object = Mapper.Map<PositionViewModel>(position);
                    }
                }
            }
            return result;
        }
        public async Task<ResultDTO> GetPositionByDepartmentId(Guid deptId)
        {
            var result = new ResultDTO();
            var position = await _uow.GetRepository<Position>().GetSingleAsync<PositionViewModel>(x => x.DeptDivisionId == deptId);
            result.Object = position;
            return result;
        }
    }
}