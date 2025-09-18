using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using Aeon.HR.BusinessObjects.Attributes;
using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.BusinessObjects.Jobs;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Constants;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Utilities;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using AutoMapper;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Aeon.HR.BusinessObjects.Handlers
{
    public class TrackingHistoryBO : ITrackingHistoryBO
    {
        private readonly ILogger _logger;
        private readonly IUnitOfWork _uow;
        public TrackingHistoryBO(ILogger logger, IUnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
        }

        public async Task<ResultDTO> GetTrackingHistoryByItemId(Guid ItemId)
        {
            var resultDto = new ResultDTO() { Object = new List<string> {}};
            var trackingHistory = await _uow.GetRepository<TrackingHistory>().FindByAsync<TrackingHistoryViewModel>(x => x.ItemID == ItemId && !x.IsDeleted, "Created desc");
            if (!(trackingHistory is null))
                resultDto.Object = trackingHistory;
            return resultDto;
        }

        public async Task<ResultDTO> GetTrackingHistoryById(Guid Id)
        {
            var resultDto = new ResultDTO() {};
            var trackingHistory = await _uow.GetRepository<TrackingHistory>().FindByIdAsync(Id);
            if (!(trackingHistory is null))
                resultDto.Object = Mapper.Map<TrackingHistoryViewModel>(trackingHistory);
            return resultDto;
        }

        public async Task<ResultDTO> GetTrackingHistoryByTypeAndItemType(string type, string itemType)
        {
            var resultDto = new ResultDTO() { };
            var trackingHistory = await _uow.GetRepository<TrackingHistory>().FindByAsync(x => x.ItemType == itemType && x.Type == type && !x.IsDeleted, "created desc");
            if (!(trackingHistory is null))
                resultDto.Object = Mapper.Map<List<TrackingHistoryViewModel>>(trackingHistory);
            return resultDto;
        }
            public async Task<ResultDTO> SaveTrackingHistory(TrackingHistoryArgs args)
        {
            var resultDto = new ResultDTO();
            string errorMsg = "";
            var trackingHistory = new TrackingHistory() {};
            try
            {
                trackingHistory = new TrackingHistory()
                {
                    ItemReferenceNumberOrCode = args.ItemRefereceNumberOrCode,
                    ItemType = args.ItemType,
                    Type = args.Type,
                    Comment = args.Comment
                };

                if (!string.IsNullOrEmpty(args.ItemName))
                    trackingHistory.ItemName = args.ItemName;

                // Round xu ly
                if (!string.IsNullOrEmpty(args.RoundNum))
                    trackingHistory.RoundNum = args.RoundNum;

                dynamic item = null;
                switch (args.ItemType)
                {
                    case ItemTypeContants.Acting:
                        item = await _uow.GetRepository<Acting>().GetSingleAsync(x => x.ReferenceNumber == args.ItemRefereceNumberOrCode);
                        break;
                    case ItemTypeContants.OvertimeApplication:
                        item = await _uow.GetRepository<OvertimeApplication>().GetSingleAsync(x => x.ReferenceNumber == args.ItemRefereceNumberOrCode);
                        break;
                    case ItemTypeContants.MissingTimeClock:
                        item = await _uow.GetRepository<MissingTimeClock>().GetSingleAsync(x => x.ReferenceNumber == args.ItemRefereceNumberOrCode);
                        break;
                    case ItemTypeContants.PromoteAndTransfer:
                        item = await _uow.GetRepository<PromoteAndTransfer>().GetSingleAsync(x => x.ReferenceNumber == args.ItemRefereceNumberOrCode);
                        break;
                    case ItemTypeContants.LeaveApplication:
                        item = await _uow.GetRepository<LeaveApplication>().GetSingleAsync(x => x.ReferenceNumber == args.ItemRefereceNumberOrCode);
                        break;
                    case ItemTypeContants.RequestToHire:
                        item = await _uow.GetRepository<RequestToHire>().GetSingleAsync(x => x.ReferenceNumber == args.ItemRefereceNumberOrCode);
                        break;
                    case ItemTypeContants.TargetPlan:
                        item = await _uow.GetRepository<TargetPlan>().GetSingleAsync(x => x.ReferenceNumber == args.ItemRefereceNumberOrCode);
                        break;
                    case ItemTypeContants.ShiftExchangeApplication:
                        item = await _uow.GetRepository<ShiftExchangeApplication>().GetSingleAsync(x => x.ReferenceNumber == args.ItemRefereceNumberOrCode);
                        break;
                    case ItemTypeContants.ShiftCode:
                        item = await _uow.GetRepository<ShiftCode>().GetSingleAsync(x => x.Code == args.ItemRefereceNumberOrCode);
                        break;
                    case ItemTypeContants.ResignationApplication:
                        item = await _uow.GetRepository<ResignationApplication>().GetSingleAsync(x => x.ReferenceNumber == args.ItemRefereceNumberOrCode);
                        break;
                    case ItemTypeContants.User:
                        item = await _uow.GetRepository<User>().GetSingleAsync(x => x.SAPCode == args.ItemRefereceNumberOrCode);
                        break;
                    case ItemTypeContants.Department:
                    case ItemTypeContants.UserDepartmentMapping:
                        item = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Code == args.ItemRefereceNumberOrCode);
                        break;
                    case ItemTypeContants.BusinessTripApplication:
                        item = await _uow.GetRepository<BusinessTripApplication>().GetSingleAsync(x => x.ReferenceNumber == args.ItemRefereceNumberOrCode);
                        break;
                    case ItemTypeContants.BusinessTripOverBudget:
                        item = await _uow.GetRepository<BusinessTripOverBudget>().GetSingleAsync(x => x.ReferenceNumber == args.ItemRefereceNumberOrCode);
                        break;
                    
                    case ItemTypeContants.TrackingRequest:
                        if (!args.ItemId.HasValue)
                        {
                            errorMsg = "TrackingRequest: Item Id is not null!";
                            trackingHistory.ErrorLog = errorMsg;
                            trackingHistory.DataStr = args.DataStr;
                            break;
                        }
                        item = await _uow.GetRepository<TrackingRequest>().GetSingleAsync(x => x.Id == args.ItemId);
                        break;
                    case ItemTypeContants.Workflow:
                        item = await _uow.GetRepository<WorkflowTemplate>().GetSingleAsync(x => x.Id == args.ItemId);
                        break;
                    case ItemTypeContants.TargetPlanSpecial:
                        item = await _uow.GetRepository<TargetPlanSpecialDepartmentMapping>().GetSingleAsync(x => x.DepartmentCode == args.ItemRefereceNumberOrCode);
                        break;
                    default:
                        errorMsg = "Item Type not exists!";
                        trackingHistory.ErrorLog = errorMsg;
                        trackingHistory.DataStr = args.DataStr;
                        break;
                }

                if (item != null)
                    trackingHistory.ItemID = item?.Id;

                // Danh cho log cua workflow
                if (args.InstanceId.HasValue)
                    trackingHistory.IntanceId = args.InstanceId.Value;

                if (!string.IsNullOrEmpty(args.WorkflowDataStr))
                {
                    trackingHistory.WorkflowDataStr = args.WorkflowDataStr;
                }
                else
                {
                    var workflowInstance = await _uow.GetRepository<WorkflowInstance>(true).GetSingleAsync(x => x.Id == args.InstanceId.Value);
                    if (!(workflowInstance is null))
                        trackingHistory.WorkflowDataStr = workflowInstance.WorkflowDataStr;
                }

                // parse data
                if (!string.IsNullOrEmpty(args.DataStr))
                {
                    if (!string.IsNullOrEmpty(args.Type))
                    {
                        switch (args.Type)
                        {
                            case TrackingHistoryTypeContants.SyncWorkflow:
                                break;
                            case TrackingHistoryTypeContants.UpdateInformation:
                                 if (args.ItemType.Equals(ItemTypeContants.RequestToHire))
                                {
                                    if (!(item is null) && !string.IsNullOrEmpty(args.DataStr))
                                    {
                                        var parseData = Mapper.Map<CommonViewModel.LogHistories.UpdateInformation.LeaveApplication>(JsonConvert.DeserializeObject<CommonViewModel.LogHistories.UpdateInformation.LeaveApplication>(args.DataStr));
                                        if (!(parseData is null))
                                        {
                                            var workingAddressRecruitment = await _uow.GetRepository<WorkingAddressRecruitment>().GetSingleAsync(x => x.Id == parseData.WorkingAddressRecruitmentId);
                                            if (!(workingAddressRecruitment is null))
                                            {
                                                parseData.WorkingAddressRecruitmentName = workingAddressRecruitment.Address;
                                                parseData.WorkingAddressRecruitmentCode = workingAddressRecruitment.Code;
                                            }
                                            args.DataStr = JsonConvert.SerializeObject(parseData);
                                        }
                                    }
                                }
                                break;
                            case TrackingHistoryTypeContants.UpdateApproval:
                                var dataStr = Mapper.Map<UpdateApprovalWorkflowViewModel>(JsonConvert.DeserializeObject<UpdateApprovalWorkflowViewModel>(args.DataStr));
                                if (!(dataStr is null))
                                {
                                    if (!string.IsNullOrEmpty(dataStr.Type))
                                    {
                                        if (dataStr.Type.Equals(ItemTypeContants.Department))
                                        {
                                            if (string.IsNullOrEmpty(dataStr.AssignToName))
                                            {
                                                var newDepartment = await _uow.GetRepository<Department>(true).GetSingleAsync(x => x.Id == dataStr.AssignToId);
                                                if (!(newDepartment is null))
                                                {
                                                    dataStr.AssignToName = newDepartment.Name;
                                                    dataStr.AssignToCode = newDepartment.Code;
                                                }
                                            }
                                        }
                                        else if (dataStr.Type.Equals(ItemTypeContants.User))
                                        {
                                            // Type users
                                            if (string.IsNullOrEmpty(dataStr.AssignToName))
                                            {
                                                var newUser = await _uow.GetRepository<User>(true).GetSingleAsync(x => x.Id == dataStr.AssignToId);
                                                if (!(newUser is null))
                                                {
                                                    dataStr.AssignToName = newUser.FullName;
                                                    dataStr.AssignToCode = newUser.SAPCode;
                                                }
                                            }
                                        }
                                    }
                                    args.DataStr = JsonConvert.SerializeObject(dataStr);
                                }
                                break;
                            case TrackingHistoryTypeContants.Create:
                            case TrackingHistoryTypeContants.Update:
                                if (!string.IsNullOrEmpty(args.ItemType))
                                {
                                    switch (args.ItemType)
                                    {
                                        case ItemTypeContants.Department:
                                            var parseData = Mapper.Map<DepartmentViewModel>(JsonConvert.DeserializeObject<DepartmentViewModel>(args.DataStr));
                                            if (parseData.ParentId.HasValue)
                                            {
                                                var parentDepartment = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == parseData.ParentId.Value);
                                                if (!(parentDepartment is null))
                                                {
                                                    parseData.ParentName = parentDepartment.Name;
                                                    parseData.ParentCode = parentDepartment.Code;
                                                }
                                            }
                                            if (parseData.HrDepartmentId.HasValue)
                                            {
                                                var hrDepartment = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == parseData.HrDepartmentId.Value);
                                                if (!(hrDepartment is null))
                                                {
                                                    parseData.HrDepartmentName = hrDepartment.Name;
                                                    parseData.HrDepartmentCode = hrDepartment.Code;
                                                }
                                            }

                                            if (parseData.RegionId.HasValue)
                                            {
                                                var region = await _uow.GetRepository<Region>().GetSingleAsync(x => x.Id == parseData.RegionId.Value);
                                                if (!(region is null))
                                                    parseData.RegionName = region.RegionName;
                                            }

                                            if (parseData.CostCenterRecruitmentId.HasValue)
                                            {
                                                var costCenterRecruitment = await _uow.GetRepository<CostCenterRecruitment>().GetSingleAsync(x => x.Id == parseData.CostCenterRecruitmentId.Value);
                                                if (!(costCenterRecruitment is null))
                                                {
                                                    parseData.CostCenterRecruitmentCode = costCenterRecruitment.Code;
                                                    parseData.CostCenterRecruitmentDescripion = costCenterRecruitment.Description;
                                                }
                                            }
                                            if (!string.IsNullOrEmpty(parseData.JobGradeCaption) && parseData.JobGradeId.HasValue)
                                            {
                                                var jobgrade = await _uow.GetRepository<JobGrade>().GetSingleAsync(x => x.Id == parseData.JobGradeId.Value);
                                                if (!(jobgrade is null))
                                                {
                                                    parseData.JobGradeCaption = jobgrade.Caption;
                                                    parseData.JobGradeGrade = jobgrade.Grade;
                                                }
                                            }

                                            if ( parseData.BusinessModelId.HasValue)
                                            {
                                                var businessModel = await _uow.GetRepository<BusinessModel>().GetSingleAsync(x => x.Id == parseData.BusinessModelId.Value);
                                                if (!(businessModel is null))
                                                {
                                                    parseData.BusinessModelCode = businessModel.Code ;
                                                    parseData.BusinessModelName = businessModel.Name ;
                                                }
                                            }
                                            args.DataStr = JsonConvert.SerializeObject(parseData);
                                            break;
                                    }
                                }
                                break;
                            // user department mapping
                            case TrackingHistoryTypeContants.UpdateUser:
                            case TrackingHistoryTypeContants.AddUser:
                            case TrackingHistoryTypeContants.DeleteUser:
                                if (!string.IsNullOrEmpty(args.DataStr))
                                {
                                    var parseData = Mapper.Map<CommonViewModel.LogHistories.UserDepartmentMappingViewModel>(JsonConvert.DeserializeObject<CommonViewModel.LogHistories.UserDepartmentMappingViewModel>(args.DataStr));
                                    if (!(parseData is null))
                                    {
                                        if (parseData.UserId.HasValue)
                                        {
                                            var user = await _uow.GetRepository<User>().GetSingleAsync(x => x.Id == parseData.UserId.Value);
                                            if (!(user is null))
                                                parseData.UserSAPCode = user.SAPCode;
                                                parseData.FullName = user.FullName;
                                        }
                                        args.DataStr = JsonConvert.SerializeObject(parseData);
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }

                trackingHistory.DataStr = !string.IsNullOrEmpty(args.DataStr) ? args.DataStr : "";
                trackingHistory.Documents = !string.IsNullOrEmpty(args.Documents) ? args.Documents : "";
            }
            catch(Exception e)
            {
                trackingHistory.ErrorLog = args + " - " + e.Message ;
            } finally
            {
                _uow.GetRepository<TrackingHistory>().Add(trackingHistory);
                await _uow.CommitAsync();
                resultDto.Object = trackingHistory;
                
            }
            return resultDto;
        }
    }
}
