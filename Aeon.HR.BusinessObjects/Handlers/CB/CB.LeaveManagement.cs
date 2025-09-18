using Aeon.HR.ViewModels.DTOs;
using Aeon.HR.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Aeon.HR.Data.Models;
using AutoMapper;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.API.Helpers;
using Newtonsoft.Json;
using System.Collections.Generic;
using static Aeon.HR.ViewModels.CommonViewModel;
using Aeon.HR.BusinessObjects.Handlers.ExternalBO;

namespace Aeon.HR.BusinessObjects.Handlers
{
    public partial class CBBO
    {
        public async Task<ArrayResultDTO> GetListLeaveApplication(QueryArgs args)
        {
            var items = await _uow.GetRepository<LeaveApplication>().FindByAsync<LeaveApplicationViewModel>(args.Order, args.Page, args.Limit, args.Predicate, args.PredicateParameters);

            var count = await _uow.GetRepository<LeaveApplication>().CountAsync(args.Predicate, args.PredicateParameters);
            var result = new ArrayResultDTO { Data = items, Count = count };
            return result;
        }
        //Jira 632
        public async Task<ResultDTO> ValidateWhenCreateNewLeaveApp(LeaveApplicationForCreatingArgs data)
        {
            // old
            /*var newData = Mapper.Map<LeaveApplication>(data);
            var result = new ResultDTO { };
            if (newData.LeaveApplicationDetails != null && newData.LeaveApplicationDetails.Count > 0)
			{
                List<string> ignoreStatus = new List<string>() { Const.Status.cancelled, Const.Status.draft, Const.Status.rejected };
                foreach (var item in newData.LeaveApplicationDetails)
                {
                    var existLeave = await _uow.GetRepository<LeaveApplicationDetail>().GetSingleAsync<LeaveApplicantDetailDTO>(x => x.LeaveApplication.UserSAPCode == newData.UserSAPCode &&
                    x.FromDate == item.FromDate && x.ToDate == item.ToDate && !ignoreStatus.Contains(x.LeaveApplication.Status));
                    
                    if (existLeave != null)
					{
                        result.ErrorCodes.Add(1);
                        result.Object = existLeave;
                        result.Messages.Add(existLeave.LeaveApplicationReferenceNumber);
                        break;
					}
                }
            }*/
            // TamPV
            var newData = Mapper.Map<LeaveApplication>(data);
            var result = new ResultDTO { };
            if (newData.LeaveApplicationDetails != null && newData.LeaveApplicationDetails.Count > 0)
            {
                List<string> ignoreStatus = new List<string>() { Const.Status.cancelled, /*Const.Status.draft,*/ Const.Status.rejected };

                List<string> ignoreStatus_SE = new List<string>() { "Rejected", "Pending", "Draft", "Cancelled", "Completed" };
                var userinfo = await _uow.GetRepository<User>().GetSingleAsync(x => x.SAPCode == newData.UserSAPCode);
                if(userinfo == null)
                {
                    result.ErrorCodes.Add(2);
                    result.Messages.Add("User is not exist.");
                    return result;
                }

                foreach (var item in newData.LeaveApplicationDetails)
                {
                    var existLeave = (dynamic)null;

                    if (data.Id != null && !data.Id.Equals(""))
                    {
                        existLeave = await _uow.GetRepository<LeaveApplicationDetail>().GetSingleAsync<LeaveApplicantDetailDTO>(x => x.LeaveApplication.UserSAPCode == newData.UserSAPCode && x.LeaveApplication.Id != data.Id &&
                        ((item.FromDate <= x.FromDate && item.ToDate >= x.FromDate) || (item.ToDate >= x.ToDate && item.FromDate <= x.ToDate)) && x.ToDate <= item.ToDate && !ignoreStatus.Contains(x.LeaveApplication.Status));
                    }
                    else
                    {
                        existLeave = await _uow.GetRepository<LeaveApplicationDetail>().GetSingleAsync<LeaveApplicantDetailDTO>(x => x.LeaveApplication.UserSAPCode == newData.UserSAPCode &&
                        ((item.FromDate <= x.FromDate && item.ToDate >= x.FromDate) || (item.ToDate >= x.ToDate && item.FromDate <= x.ToDate)) && x.ToDate <= item.ToDate && !ignoreStatus.Contains(x.LeaveApplication.Status));
                    }

                    if (existLeave != null)
                    {
                        result.ErrorCodes.Add(1);
                        result.Object = existLeave;
                        result.Messages.Add(existLeave.LeaveApplicationReferenceNumber);
                        break;
                    }

                    var existShiftEx = await _uow.GetRepository<ShiftExchangeApplicationDetail>().GetSingleAsync(x => x.UserId == userinfo.Id && item.FromDate >= x.ShiftExchangeDate && item.ToDate <= x.ShiftExchangeDate && !ignoreStatus_SE.Contains(x.ShiftExchangeApplication.Status));
                    if (existShiftEx != null)
                    {
                        result.ErrorCodes.Add(2);
                        result.Messages.Add("UserSAPCode: " + newData.UserSAPCode + " already have Shift Exchange Application on " + existShiftEx.ShiftExchangeDate.ToString("dd/MM/yyyy"));
                        break;
                    }

                }
            }
            return result;
        }
        //===============
        public async Task<ResultDTO> CreateNewLeaveApplication(LeaveApplicationForCreatingArgs data)
        {
            //Jira 632
            ResultDTO invalid = await ValidateWhenCreateNewLeaveApp(data);
            if (invalid.ErrorCodes.Count > 0)
            {
                var result = new ResultDTO { };
                result.ErrorCodes.AddRange(invalid.ErrorCodes);
                result.Messages.AddRange(invalid.Messages);
                result.Object = invalid.Object;
                return result;
            }
            //===============

            var newData = Mapper.Map<LeaveApplication>(data);
            if (!string.IsNullOrEmpty(data.DeptCode))
            {
                var findDept = await _uow.GetRepository<Department>().GetSingleAsync(x => x.SAPCode.Equals(data.DeptCode) || x.Code.Equals(data.DeptCode));
                if (findDept != null)
                    newData.DeptId = findDept.Id;
            }
            UpdateIs2Approval(newData);
            _uow.GetRepository<LeaveApplication>().Add(newData);

            await _uow.CommitAsync();
            return new ResultDTO { Object = Mapper.Map<LeaveApplicationViewModel>(newData) };
        }

        public async Task<ResultDTO> UpdateLeaveApplication(LeaveApplicationForCreatingArgs args)
        {
            //Jira 632
            ResultDTO invalid = await ValidateWhenCreateNewLeaveApp(args);
            if (invalid.ErrorCodes.Count > 0)
            {
                var result = new ResultDTO { };
                result.ErrorCodes.AddRange(invalid.ErrorCodes);
                result.Messages.AddRange(invalid.Messages);
                result.Object = invalid.Object;
                return result;
            }
            var existLeaveApplication = await _uow.GetRepository<LeaveApplication>().GetSingleAsync(x => x.Id == args.Id, string.Empty);
            if (existLeaveApplication == null)
            {
                return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Leave Application is not exist." } };
            }
            else
            {
                var existLeaveApplicationDetails = await _uow.GetRepository<LeaveApplicationDetail>().FindByAsync(x => x.LeaveApplicationId == args.Id);
                if (existLeaveApplicationDetails.Any())
                {
                    _uow.GetRepository<LeaveApplicationDetail>().Delete(existLeaveApplicationDetails);
                }
                var updatedLeaveApplicationDetails = JsonConvert.DeserializeObject<List<LeaveApplicationDetail>>(args.LeaveApplicantDetails);

                existLeaveApplication.ReferenceNumber = args.ReferenceNumber;
                existLeaveApplication.LeaveApplicationDetails = updatedLeaveApplicationDetails;
                existLeaveApplication.Documents = args.Documents;
                UpdateIs2Approval(existLeaveApplication);
                _uow.GetRepository<LeaveApplication>().Update(existLeaveApplication);
                await _uow.CommitAsync();
            }
            return new ResultDTO { Object = Mapper.Map<LeaveApplicationViewModel>(existLeaveApplication) };
        }

        public async Task<ResultDTO> DeleteLeaveApplication(Guid id)
        {
            bool existPosition = _uow.GetRepository<LeaveApplication>().Any(x => x.Id == id);
            if (!existPosition)
            {
                return new ResultDTO
                {

                    ErrorCodes = { 404 },
                    Messages = { $"Leave Application id {id} is not found!" },
                };
            }
            else
            {
                var leaveApplication = _uow.GetRepository<LeaveApplication>().FindById(id);

                await _uow.CommitAsync();
                return new ResultDTO { };
            }
        }
        public async Task<ResultDTO> FinalCheckValidLeaveKind(ObjectToCheckValidLeaveManagemetDTO objectToCheck)
        {
            var result = new ResultDTO { };
            ObjectToCheckValidLeaveManagemetDTO objectToCheckPrevYear = new ObjectToCheckValidLeaveManagemetDTO();
            objectToCheckPrevYear.UserSapCode = objectToCheck.UserSapCode;
            objectToCheckPrevYear.Id = objectToCheck.Id;
            objectToCheckPrevYear.LeaveDetails = objectToCheck.LeaveDetails.Where(x => x.FromDate.Year == DateTime.Now.Year-1).ToList();
            if (objectToCheckPrevYear.LeaveDetails.Any())
            {
                var objectsleaveBalances = await _sSGExBO.GetLeaveBalanceSet(objectToCheckPrevYear.UserSapCode, DateTime.Now.Year - 1);

                if (objectsleaveBalances.Object is List<LeaveBalanceResponceSAPViewModel>)
                {
                    var leaveBalances = objectsleaveBalances.Object as List<LeaveBalanceResponceSAPViewModel>;

                    objectToCheckPrevYear.MyLeaveBalances = leaveBalances;
                    result = await CheckValidLeaveKind(objectToCheckPrevYear);
                    if (!result.IsSuccess)
                        return result;
                }

            }

            ObjectToCheckValidLeaveManagemetDTO objectToCheckCurrentYearYear = new ObjectToCheckValidLeaveManagemetDTO();
            objectToCheckCurrentYearYear.UserSapCode = objectToCheck.UserSapCode;
            objectToCheckCurrentYearYear.LeaveDetails = objectToCheck.LeaveDetails.Where(x => x.FromDate.Year == DateTime.Now.Year).ToList();
            objectToCheckCurrentYearYear.MyLeaveBalances = objectToCheck.MyLeaveBalances;
            objectToCheckCurrentYearYear.Id = objectToCheck.Id;
            result = await CheckValidLeaveKind(objectToCheckCurrentYearYear);
            return result;

        }

        public async Task<ResultDTO> CheckValidLeaveKind(ObjectToCheckValidLeaveManagemetDTO objectToCheck)
        {
            var result = new ResultDTO { };
            if (!string.IsNullOrEmpty(objectToCheck.UserSapCode))
            {
                objectToCheck.LeaveDetails = objectToCheck.LeaveDetails.Where(x => x.LeaveCode != "ERD" || x.LeaveCode != "ERD1" || x.LeaveCode != "ERD2").ToList();
                var errorMessageFromCheckValidQuata = CheckValidQuota(objectToCheck);
                if (!string.IsNullOrEmpty(errorMessageFromCheckValidQuata))
                {
                    result.ErrorCodes.Add(1004);
                    result.Messages.Add(errorMessageFromCheckValidQuata);
                }
                else
                {
                    // AMOAEON-534
                    // Kiểm tra có 2 đơn nghỉ phép trong cùng 1 ngày hay không
                    bool doesOverlappingLeaveApplications = false;
                    List<LeaveApplication> overlappingLeaveApplications = objectToCheck.LeaveDetails.GetOverlappingLeaveApplications(objectToCheck.UserSapCode, _uow);
                    if (objectToCheck != null && objectToCheck.Id != null && objectToCheck.Id.HasValue)
                    {
                        overlappingLeaveApplications = overlappingLeaveApplications.Where(x => x.Id != objectToCheck.Id.Value).ToList();
                    }
                    if (overlappingLeaveApplications.Any())
                    {
                        doesOverlappingLeaveApplications = true;
                        result.ErrorCodes.Add(1004);
                        result.Messages.Add($"You cannot apply leave in the same day: {string.Join(",", overlappingLeaveApplications.Select(x=>x.ReferenceNumber))}");
                    }


                    if (!doesOverlappingLeaveApplications)
                    {
                        // Kiểm tra còn ngày phép hay không
                        var leaveCheck1 = new List<LeaveApplicantDetailDTO>();

                        var leaveCheck2 = new List<LeaveApplicantDetailDTO>();
                        var inValidStatus = new string[] { "completed", "draft", "rejected", "requested to change", "cancelled" };
                        int currentYear = DateTimeOffset.Now.Year;
                        var existItems = await _uow.GetRepository<LeaveApplicationDetail>().FindByAsync(x => (x.FromDate.Year == currentYear && x.ToDate.Year == currentYear) && x.LeaveApplicationId != objectToCheck.Id.Value && x.LeaveApplication.UserSAPCode == objectToCheck.UserSapCode && !inValidStatus.Contains(x.LeaveApplication.Status.ToLower()));
                        leaveCheck1.AddRange(objectToCheck.LeaveDetails);
                        if (existItems.Any())
                        {
                            foreach (var item in existItems)
                            {
                                leaveCheck1.Add(Mapper.Map<LeaveApplicantDetailDTO>(item));
                            }
                        }


                        var jsonContent = JsonHelper.GetJsonContentFromFile("Mappings", "leave-kind-mapping.json");
                        var jsonMappings = JsonConvert.DeserializeObject<List<LeaveKindQuotaMapping>>(jsonContent);
                        var groupsLeaveKind = leaveCheck1.GroupBy(x => x.LeaveCode).Select(x => new LeaveApplicantDetailDTO { LeaveCode = x.Key, Quantity = x.Sum(y => y.Quantity) });
                        groupsLeaveKind = groupsLeaveKind.Where(x => objectToCheck.LeaveDetails.Any(y => y.LeaveCode == x.LeaveCode));
                        Dictionary<string, List<LeaveApplicantDetailDTO>> dicLeaveKinds = new Dictionary<string, List<LeaveApplicantDetailDTO>>();
                        Dictionary<string, List<LeaveBalanceResponceSAPViewModel>> dicLeaveLeaveBalances = new Dictionary<string, List<LeaveBalanceResponceSAPViewModel>>();


                        if (jsonMappings.Any())
                        {
                            foreach (var item in groupsLeaveKind)
                            {
                                var mappingItem = jsonMappings.FirstOrDefault(x => x.LeaveKinds.Any(y => y == item.LeaveCode));
                                if (mappingItem != null)
                                {
                                    if (dicLeaveKinds.ContainsKey(mappingItem.Key))
                                    {
                                        dicLeaveKinds[mappingItem.Key].Add(item);
                                    }
                                    else
                                    {
                                        dicLeaveKinds[mappingItem.Key] = new List<LeaveApplicantDetailDTO> { item };
                                    }
                                }
                            }
                            if (objectToCheck.MyLeaveBalances.Any())
                            {
                                foreach (var item in objectToCheck.MyLeaveBalances)
                                {
                                    var mappingItem = jsonMappings.FirstOrDefault(x => x.QuotaKinds.Any(y => y == item.AbsenceQuotaType));
                                    if (mappingItem != null && dicLeaveKinds.ContainsKey(mappingItem.Key))
                                    {
                                        if (dicLeaveLeaveBalances.ContainsKey(mappingItem.Key))
                                        {
                                            dicLeaveLeaveBalances[mappingItem.Key].Add(item);
                                        }
                                        else
                                        {
                                            dicLeaveLeaveBalances[mappingItem.Key] = new List<LeaveBalanceResponceSAPViewModel> { item };
                                        }
                                    }
                                }
                                if (dicLeaveKinds.Any())
                                {
                                    foreach (var leave in dicLeaveKinds)
                                    {
                                        var key = leave.Key;
                                        if (dicLeaveLeaveBalances.ContainsKey(key))
                                        {
                                            if (dicLeaveLeaveBalances[key].Sum(x => x.Remain) < leave.Value.Sum(y => y.Quantity))
                                            {
                                                var leaveKindCodes = leave.Value.Select(x => x.LeaveCode);
                                                result.ErrorCodes.Add(1004);
                                                result.Messages.Add($"Leave quantity Exceeds Quota: {string.Join(", ", leaveKindCodes)}");
                                            }
                                        }

                                    }
                                }
                            }
                        }

                        // Kiểm tra Leave Kind ở các phiếu khác  nhau
                        var statusToCheckes = new string[] { "Rejected", "Cancelled", "Draft" };
                        var _existItems = await _uow.GetRepository<LeaveApplicationDetail>().FindByAsync(x => x.LeaveApplicationId != objectToCheck.Id.Value && x.LeaveApplication.UserSAPCode == objectToCheck.UserSapCode && (x.LeaveApplication.Status == "Completed" || !statusToCheckes.Contains(x.LeaveApplication.Status)), string.Empty);
                        if (_existItems.Any())
                        {
                            leaveCheck2.AddRange(objectToCheck.LeaveDetails);
                            foreach (var item in _existItems)
                            {
                                leaveCheck2.Add(Mapper.Map<LeaveApplicantDetailDTO>(item));
                            }
                            var quantityByDates = new List<SimpleLeaveApplicantDetail>();
                            leaveCheck2.ForEach(x =>
                            {
                                var startDate = x.FromDate.LocalDateTime.Date;
                                while (startDate <= x.ToDate.LocalDateTime.Date)
                                {
                                    var quantity = GetQuantityFromLeaveCode(x.LeaveCode);
                                    quantityByDates.Add(new SimpleLeaveApplicantDetail { Date = startDate, Quantity = quantity, LeaveCode = x.LeaveCode, LeaveApplicationReferenceNumber = x.LeaveApplicationReferenceNumber });
                                    startDate = startDate.AddDays(1);
                                }
                            });
                            var groupByDates = quantityByDates.GroupBy(x => x.Date);
                            var tempErrorMessageForHaftDay = new List<string>();
                            var tempErrorMessageForFullDay = new List<string>();
                            foreach (var group in groupByDates)
                            {
                                var existInLeaveDetails = objectToCheck.LeaveDetails.Where(x => x.FromDate.LocalDateTime.Date == group.Key.Date);
								if (existInLeaveDetails.Any())
								{
                                    var totalQuantity = group.Sum(x => x.Quantity);
                                    if (totalQuantity > 1)
                                    {
                                        var contentMessage = group.Where(y => !string.IsNullOrEmpty(y.LeaveApplicationReferenceNumber)).Select(z => string.Format("{0}({1})", z.Date.LocalDateTime.ToString("dd/MM/yyyy"), z.LeaveApplicationReferenceNumber)).FirstOrDefault();
                                        result.ErrorCodes.Add(1004);
                                        tempErrorMessageForFullDay.Add(contentMessage);

                                    }
                                    else
                                    {
                                        if (group.Sum(x => x.Quantity) == 1 && (group.Count(y => y.LeaveCode.Contains("1") && y.Quantity == 0.5) > 1 || group.Count(y => y.LeaveCode.Contains("2") && y.Quantity == 0.5) > 1))
                                        {
                                            var contentMessage = group.Where(y => !string.IsNullOrEmpty(y.LeaveApplicationReferenceNumber)).Select(z => string.Format("{0}({1})", z.Date.LocalDateTime.ToString("dd/MM/yyyy"), z.LeaveApplicationReferenceNumber)).FirstOrDefault();
                                            result.ErrorCodes.Add(1004);
                                            tempErrorMessageForHaftDay.Add(contentMessage);
                                        }
                                    }
                                } 
                            }
                            if (tempErrorMessageForFullDay.Any())
                            {
                                result.Messages.Add($"Leave quantity in a day is not more than 1: {string.Join(",", tempErrorMessageForFullDay)}");
                            }
                            if (tempErrorMessageForHaftDay.Any())
                            {
                                result.Messages.Add($"You cannot apply leave in the same half day: {string.Join(",", tempErrorMessageForHaftDay)}");
                            }
                        }
                    }
                }

            }
            return result;

        }

        private double GetQuantityFromLeaveCode(string code)
        {
            var quantity = 0.0;
            var quantiesForHafts = new string[] { "ERD1",
        "ERD2",
        "DOH1",
        "DOH2",
        "ALH1",
        "ALH2",
        "NPH1",
        "NPH2",
        "TRN1",
        "TRN2",
        "BTD1",
        "BTD2",
        "NPA1",
        "NPA2",
        "RAC1",
        "RAC2",
        "CE1",
        "CE2"
            };
            if (!string.IsNullOrEmpty(code))
            {

                if (quantiesForHafts.Any(x => x == code))
                {
                    quantity = 0.5;
                }
                else
                {
                    quantity = 1;
                }
            }
            return quantity;
        }
        private string CheckValidQuota(ObjectToCheckValidLeaveManagemetDTO objectToCheck)
        {
            string errorMessage = "";
            var invalidLeaveKind = new List<string>();
            var allLeaveKindToCheck = new List<string>();
            var allLeaveCodeSubmited = objectToCheck.LeaveDetails.GroupBy(x => x.LeaveCode).Select(y => y.Key);
            var jsonContent = JsonHelper.GetJsonContentFromFile("Mappings", "leave-kind-mapping.json");
            var jsonMappings = JsonConvert.DeserializeObject<List<LeaveKindQuotaMapping>>(jsonContent);
            jsonMappings.ForEach(x =>
            {
                allLeaveKindToCheck.AddRange(x.LeaveKinds);
            });
            foreach (var item in allLeaveCodeSubmited)
            {
                if (allLeaveKindToCheck.Any(x => x == item))
                {
                    var currentQuotaType = jsonMappings.Where(y => y.LeaveKinds.Any(z => z == item)).FirstOrDefault();
                    if (!objectToCheck.MyLeaveBalances.Any(t => currentQuotaType.QuotaKinds.Any(m => m == t.AbsenceQuotaType)))
                    {
                        invalidLeaveKind.Add(item);
                    }
                }
            }
            // 
            if (invalidLeaveKind.Any())
            {
                errorMessage = string.Format("{0}: {1}", "You have not been granted this leave kind", string.Join(",", invalidLeaveKind));
            }

            return errorMessage;
        }

        public async Task<ArrayResultDTO> GetLeaveApplicantDetail(Guid Id)
        {
            ArrayResultDTO result = new ArrayResultDTO { Data = new List<LeaveApplicantDetailDTO>(), Count = 0 };
            var data = await _uow.GetRepository<LeaveApplicationDetail>().FindByAsync<LeaveApplicantDetailDTO>(x => x.LeaveApplicationId == Id);
            var count = await _uow.GetRepository<LeaveApplicationDetail>().CountAsync(x => x.LeaveApplicationId == Id);
            if (data.Any())
            {
                result.Data = data;
                result.Count = count;
            }
            return result;
        }
        public async Task<ResultDTO> GetLeaveApplicationById(Guid Id)
        {
            ResultDTO result = new ResultDTO();
            var record = await _uow.GetRepository<LeaveApplication>().FindByIdAsync<LeaveApplicationViewModel>(Id);
            if (record != null)
            {
                List<string> departmentAndDivisions = new List<string>() { record.DeptCode, record.DivisionCode };
                var deptList = await _uow.GetRepository<Department>(true).FindByAsync(x => departmentAndDivisions.Contains(x.Code));
                if (!(deptList is null))
                {
                    // department
                    var divisionCode = deptList.Where(x => x.Code == record.DivisionCode).FirstOrDefault();
                    if (!(divisionCode is null)) record.DivisionId = divisionCode.Id;

                    var deptCode = deptList.Where(x => x.Code == record.DeptCode).FirstOrDefault();
                    if (!(deptCode is null)) record.DeptId = deptCode.Id;
                }
                result.Object = record;
            }
            return result;
        }
        //public async Task<ArrayResultDTO> GetLeaveApplicantDetailFromUserId(Guid userId)
        //{
        //    var data = await _uow.GetRepository<LeaveApplicationDetail>().FindByAsync<ExportLeaveApplicationViewModel>(x => x.LeaveApplication.CreatedById == userId && x.LeaveApplication.Status == "Completed");
        //    if (data.Any())
        //    {
        //        return new ArrayResultDTO { Data = data, Count = data.Count() };
        //    }
        //    return new ArrayResultDTO { Data = new List<LeaveApplicantDetailDTO>() };
        //}

        public async Task<ArrayResultDTO> GetLeaveApplicantDetailFromUserId(Guid userId)
        {
            var data = await _uow.GetRepository<LeaveApplication>().FindByAsync(x => x.CreatedById == userId && x.Status == "Completed");
            if (data.Any())
            {
                var exportLeaveApplicationViewModels = new List<ExportLeaveApplicationViewModel>();
                foreach (var _record in data)
                {
                    var record = Mapper.Map<LeaveApplicationViewModel>(_record);
                    if (_record.LeaveApplicationDetails.Any())
                    {
                        var details = _record.LeaveApplicationDetails;
                        if (details.Any())
                        {
                            foreach (var item in details)
                            {
                                var detail = Mapper.Map<LeaveApplicantDetailDTO>(item);
                                exportLeaveApplicationViewModels.Add(new ExportLeaveApplicationViewModel
                                {
                                    ReferenceNumber = record.ReferenceNumber,
                                    Status = record.Status,
                                    DepartmentCode = !string.IsNullOrEmpty(record.DeptCode) ? record.DeptCode : record.DivisionCode,
                                    DepartmentName = !string.IsNullOrEmpty(record.DeptName) ? record.DeptName : record.DivisionName,
                                    FullName = record.CreatedByFullName,
                                    SAPCode = record.UserSAPCode,
                                    LeaveCode = detail.LeaveCode,
                                    LeaveName = string.Format("{0} - {1}", detail.LeaveName, detail.LeaveCode),
                                    FromDate = detail.FromDate,
                                    ToDate = detail.ToDate,
                                    Quantity = detail.Quantity,
                                    Reason = detail.Reason,
                                    CreateDate = record.Created
                                });
                            }
                        }
                    }
                }
                return new ArrayResultDTO { Data = exportLeaveApplicationViewModels, Count = exportLeaveApplicationViewModels.Count() };
            }
            return new ArrayResultDTO { Data = new List<LeaveApplicantDetailDTO>() };
        }

        public async Task<ArrayResultDTO> GetAllLeaveManagementByUserId(Guid userId)
        {
            var statusToCheckes = new string[] { "Rejected", "Cancelled", "Draft" };
            var data = await _uow.GetRepository<LeaveApplicationDetail>().FindByAsync<LeaveApplicantDetailDTO>(x => x.LeaveApplication.CreatedById == userId && x.LeaveApplication.Status == "Completed" || !statusToCheckes.Contains(x.LeaveApplication.Status));
            if (data.Any())
            {
                return new ArrayResultDTO { Data = data, Count = data.Count() };
            }
            return new ArrayResultDTO { Data = new List<LeaveApplicantDetailDTO>() };
        }
        private void UpdateIs2Approval(LeaveApplication leave)
        {
            var details = leave.LeaveApplicationDetails;
            leave.Is2ndApproval = false;
            var existIs2Approval = details.Where(x => x.LeaveCode.Contains("NP") || x.LeaveCode.Contains("AL"));
            if (existIs2Approval.Any())
            {
                var sumQuantities = existIs2Approval.Sum(x => x.Quantity);
                if (sumQuantities >= 5)
                {
                    leave.Is2ndApproval = true;
                }
            }
        }
    }

}