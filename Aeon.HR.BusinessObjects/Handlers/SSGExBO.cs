using Aeon.HR.ViewModels.Args;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.ViewModels.DTOs;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Aeon.HR.Infrastructure.Interfaces;
using Newtonsoft.Json;
using Aeon.HR.ViewModels;
using System.Net.Http;
using System.Diagnostics;
using Aeon.HR.BusinessObjects.Jobs;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Aeon.HR.Infrastructure.Constants;
using Aeon.HR.ViewModels.ExternalItem;
using Aeon.HR.ViewModels.ExternalItem.ParseInfo;
using System.Globalization;
using System.Collections;

namespace Aeon.HR.BusinessObjects.Handlers
{
    public class SSGExBO : ISSGExBO
    {

        private readonly IUnitOfWork _uow;
        private readonly IIntegrationExternalServiceBO _exbo;
        private readonly ITrackingBO _tracking;
        private readonly ISettingBO _userSetting;
        private readonly IDashboardBO _dashboardBO;
        private readonly IIntegrationExternalServiceBO _externalServiceBO;
        private readonly IEmployeeBO _employeeBO;
        private readonly ILogger _log;
        public SSGExBO(IUnitOfWork uow, IIntegrationExternalServiceBO bo, ITrackingBO tracking, ISettingBO userSetting, IDashboardBO dashboardBO, IIntegrationExternalServiceBO externalServiceBO, IEmployeeBO employeeBO, ILogger log)
        {
            _uow = uow;
            _exbo = bo;
            _tracking = tracking;
            _userSetting = userSetting;
            _dashboardBO = dashboardBO;
            _externalServiceBO = externalServiceBO;
            _employeeBO = employeeBO;
            _log = log;
        }

        public Task<ResultDTO> GetUserInFoByCode(string SAPCode)
        {
            throw new NotImplementedException();
        }

        public async Task<ResultDTO> TeminateEmployee(string SAPCode)
        {
            ResultDTO result = null;
            var response = new ResponseExternalDataMappingDTO
            {
                Payload = SAPCode,
                Url = $"SSGEx/TeminateEmployee?SAPCode = {SAPCode}",
                Created = DateTime.Now,
                Modified = DateTime.Now,
                Action = ActionExposeAPI.Receive
            };
            TrackingRequest trackingRequest = await _tracking.AddNewTrackingRequest(response);
            if (!string.IsNullOrEmpty(SAPCode))
            {
                var currentUser = await _uow.GetRepository<User>(true).GetSingleAsync(x => x.SAPCode == SAPCode);
                if (currentUser != null)
                {
                    await _userSetting.ChangeStatus(currentUser.Id, false);
                    await _uow.CommitAsync();
                    result = new ResultDTO { Messages = { "User is teminated" } };
                    trackingRequest.Response = JsonConvert.SerializeObject(result);
                    _uow.GetRepository<TrackingRequest>().Update(trackingRequest);
                }
                else
                {
                    result = new ResultDTO { ErrorCodes = { 1003 }, Object = null, Messages = { "Current User is not found in database" } };
                }

            }
            else
            {
                result = new ResultDTO { ErrorCodes = { 1003 }, Object = null, Messages = { "Current User is not found in database" } };
            }
            return result;
        }

        public async Task<ResultDTO> UpdateEmployeeStatus(StatusArgs statusArgs)
        {
            ResultDTO result = null;
            //var currentUser = await _uow.GetRepository<User>().GetSingleAsync(x => x.Id == statusArgs.RefNumber);
            //if (currentUser != null)
            //{
            //    currentUser.Status = statusArgs.Status;
            //    result = new ResultDTO { Messages = { "Update Employee'status successfully" } };
            //    await _uow.CommitAsync();
            //    return result;
            //}           
            var response = new ResponseExternalDataMappingDTO
            {
                Payload = JsonConvert.SerializeObject(statusArgs),
                Url = "SSGEx/UpdateEmployeeStatus",
                Created = DateTime.Now,
                Modified = DateTime.Now,
                Action = ActionExposeAPI.Receive
            };
            TrackingRequest trackingRequest = await _tracking.AddNewTrackingRequest(response);
            if (statusArgs.RefNumber != Guid.Empty)
            {
                var applicant = await _uow.GetRepository<Applicant>(true).FindByIdAsync(statusArgs.RefNumber);
                if (applicant != null)
                {
                    if (statusArgs.Status == StatusChange.Approved)
                    {
                        applicant.IsApproved = true;
                        applicant.SAPReviewStatus = "Approved";
                        if (!string.IsNullOrEmpty(statusArgs.EmployeeCode))
                        {
                            var userResult = await CreateOrUpdateUser(statusArgs.EmployeeCode, applicant.FullName, applicant.Email);
                            if (userResult != null && userResult.IsSuccess)
                            {
                                var pos = await _uow.GetRepository<Position>(true).GetSingleAsync(x => x.Id == applicant.PositionId);
                                if (pos != null)
                                {
                                    var rth = await _uow.GetRepository<RequestToHire>(true).GetSingleAsync(x => x.Id == pos.RequestToHireId);
                                    if (rth != null)
                                    {
                                        if (rth.ReplacementFor == TypeOfNeed.ReplacementFor)
                                        {
                                            var dept = await _uow.GetRepository<Department>().GetSingleAsync(x => x.RequestToHireId == pos.RequestToHireId);
                                            if (dept != null)
                                            {
                                                var mappings = await _uow.GetRepository<UserDepartmentMapping>().FindByAsync(x => x.DepartmentId == dept.Id);
                                                _uow.GetRepository<UserDepartmentMapping>().Delete(mappings);
                                                _uow.GetRepository<UserDepartmentMapping>().Add(new UserDepartmentMapping()
                                                {
                                                    DepartmentId = dept.Id,
                                                    IsHeadCount = true,
                                                    Role = Group.Member,
                                                    UserId = (Guid)userResult.Object
                                                });
                                            }
                                        }
                                        else
                                        {
                                            var dept = await _uow.GetRepository<Department>().GetSingleAsync(x => x.RequestToHireId == pos.RequestToHireId && !x.UserDepartmentMappings.Any(t => t.Role == Group.Member));
                                            if (dept != null)
                                            {
                                                _uow.GetRepository<UserDepartmentMapping>().Add(new UserDepartmentMapping()
                                                {
                                                    DepartmentId = dept.Id,
                                                    IsHeadCount = true,
                                                    Role = Group.Member,
                                                    UserId = (Guid)userResult.Object
                                                });
                                            }
                                        }
                                        _dashboardBO.ClearNode();
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        applicant.IsApproved = false;
                        applicant.SAPReviewStatus = "Rejected";
                    }
                    result = new ResultDTO { Messages = { "Update Employee'status successfully" } };
                    trackingRequest.Response = JsonConvert.SerializeObject(result);
                    trackingRequest.ReferenceNumber = applicant.ReferenceNumber;
                    trackingRequest.Status = applicant.ApplicantStatus.Name;
                    trackingRequest.UserName = applicant.FullName;
                    _uow.GetRepository<TrackingRequest>().Update(trackingRequest);
                    await _uow.CommitAsync();
                }
                else
                {
                    result = new ResultDTO { Messages = { $"Not found Applicant with RefNumber : {statusArgs.RefNumber}" } };
                }
                return result;
            }
            result = new ResultDTO { ErrorCodes = { 1004 }, Object = null, Messages = { "Current User is not found in database" } };
            return result;
        }
        private async Task<ResultDTO> CreateOrUpdateUser(string sapcode, string fullName, string email)
        {
            var existUser = await _uow.GetRepository<User>().GetSingleAsync(x => x.SAPCode == sapcode);
            if (existUser != null)
            {
                existUser.IsActivated = true;
            }
            else
            {
                var user = new UserDataForCreatingArgs
                {
                    SAPCode = sapcode,
                    FullName = fullName,
                    Email = email,
                    Role = UserRole.Member,
                    Type = LoginType.Membership,
                    LoginName = sapcode
                };
                return await _userSetting.CreateUser(user);
            }
            return new ResultDTO();
        }
        public async Task<ResultDTO> Retry(Guid Id)
        {
            var trackingLog = await _uow.GetRepository<TrackingRequest>().FindByIdAsync(Id);
            if (trackingLog != null)
            {
                var url = trackingLog.Url;
                var payload = trackingLog.Payload;
                var response = await _tracking.RetryRequest(url, payload);
                if (response != null)
                {
                    if (response.IsSuccessStatusCode)
                    {
                        trackingLog.Status = "Success";
                    }
                    await _tracking.UpdateTrackingRequestByInstance(response, trackingLog);
                }
            }
            return new ResultDTO { Object = trackingLog };
        }
        public async Task<ResultDTO> TestPushTargetPlanToSAP(string url, string payload)
        {
            //var users = await _uow.GetRepository<User>(true).GetAllAsync<UserListViewModel>("SapCode", 1, 12);
            //var listSAPCodes = users.Select(x => x.SAPCode);
            //var data = new ArrayResultDTO();
            var stopwatch = Stopwatch.StartNew();
            var listResponses = new List<Task<HttpResponseMessage>>();
            var listSuccess = new List<string>();
            for (int i = 0; i < 60; i++)
            {
                var response = _tracking.RetryRequestNoAwait(url, payload);
                listResponses.Add(response);
            }
            int total = 0;
            while (listResponses.Any())
            {
                Task<HttpResponseMessage> finishedTask = await Task.WhenAny(listResponses);
                listSuccess.Add(JsonConvert.DeserializeObject(await finishedTask.Result.Content.ReadAsStringAsync()).ToString());
                listResponses.Remove(finishedTask);
                total++; ;
            }
            stopwatch.Stop();

            return new ResultDTO { Object = new { number = total, Data = listSuccess } };
        }
        public string GenerateName(int len)
        {
            Random r = new Random();
            string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "l", "n", "p", "q", "r", "s", "sh", "zh", "t", "v", "w", "x" };
            string[] vowels = { "a", "e", "i", "o", "u", "ae", "y" };
            string Name = "";
            Name += consonants[r.Next(consonants.Length)].ToUpper();
            Name += vowels[r.Next(vowels.Length)];
            int b = 2; //b tells how many times a new letter has been added. It's 2 right now because the first two letters are already in the name.
            while (b < len)
            {
                Name += consonants[r.Next(consonants.Length)];
                b++;
                Name += vowels[r.Next(vowels.Length)];
                b++;
            }

            return Name;
        }
        public async Task<ResultDTO> InserDepartment(int number)
        {
            var g9Department = new TestDepartment
            {
                Code = string.Format("DepartmentCode9-{0}", GenerateName(9)),
                Name = string.Format("DepartmentName9{0}", GenerateName(9)),
                JobGradeId = new Guid("670761A9-92F5-4E63-945F-4176AA324923"),
                Type = DepartmentType.Department,
                PositionName = $"Position9",
                PositionCode = $"Position9",
            };
            _uow.GetRepository<TestDepartment>().Add(g9Department);
            for (int i = 0; i < number; i++)
            {
                var g8Department = new TestDepartment
                {
                    Code = string.Format("DepartmentCode8-{0}", GenerateName(8)),
                    Name = string.Format("DepartmentName8-{0}", GenerateName(8)),
                    JobGradeId = new Guid("670761A9-92F5-4E63-945F-4176AA324923"),
                    Type = DepartmentType.Department,
                    PositionName = $"Position{i}",
                    PositionCode = $"Position{i}",
                    ParentId = g9Department.Id
                };
                _uow.GetRepository<TestDepartment>().Add(g8Department);
                for (int j = 0; j < 2; j++)
                {
                    var g7Department = new TestDepartment
                    {
                        Code = string.Format("DepartmentCode7-{0}", GenerateName(7)),
                        Name = string.Format("DepartmentName7-{0}", GenerateName(7)),
                        JobGradeId = new Guid("7958B0C4-9BD1-4AA4-8AA6-6F201E0A65F2"),
                        Type = DepartmentType.Department,
                        PositionName = $"Position{10 + i}",
                        PositionCode = $"Position{10 + i}",
                        ParentId = g8Department.Id
                    };
                    _uow.GetRepository<TestDepartment>().Add(g7Department);
                    for (int k = 0; k < 3; k++)
                    {
                        var g6Department = new TestDepartment
                        {
                            Code = string.Format("DepartmentCode6-{0}", GenerateName(6)),
                            Name = string.Format("DepartmentName6-{0}", GenerateName(6)),
                            JobGradeId = new Guid("333B2071-D76C-4FAC-9D6D-D64E2F2217B5"),
                            Type = DepartmentType.Department,
                            PositionName = $"Position{20 + i}",
                            PositionCode = $"Position{20 + i}",
                            ParentId = g7Department.Id
                        };
                        _uow.GetRepository<TestDepartment>().Add(g6Department);
                        for (int a = 0; a < 3; a++)
                        {
                            var g5Department = new TestDepartment
                            {
                                Code = string.Format("DepartmentCode5-{0}", GenerateName(5)),
                                Name = string.Format("DepartmentName5-{0}", GenerateName(5)),
                                JobGradeId = new Guid("A67CF0B7-44BD-437C-9B3E-5C95624BDBD4"),
                                Type = DepartmentType.Department,
                                PositionName = $"Position{30 + i}",
                                PositionCode = $"Position{30 + i}",
                                ParentId = g6Department.Id
                            };
                            _uow.GetRepository<TestDepartment>().Add(g5Department);
                            for (int b = 0; b < 3; b++)
                            {
                                var g4Department = new TestDepartment
                                {
                                    Code = string.Format("DepartmentCode4-{0}", GenerateName(4)),
                                    Name = string.Format("DepartmentName4-{0}", GenerateName(4)),
                                    JobGradeId = new Guid("5E7B0464-D2B2-446A-81D0-02281DE68431"),
                                    Type = DepartmentType.Division,
                                    PositionName = $"Position{40 + i}",
                                    PositionCode = $"Position{40 + i}",
                                    ParentId = g5Department.Id
                                };
                                _uow.GetRepository<TestDepartment>().Add(g4Department);
                                for (int c = 0; c < 3; c++)
                                {
                                    var g3Department = new TestDepartment
                                    {
                                        Code = string.Format("DepartmentCode3-{0}", GenerateName(3)),
                                        Name = string.Format("DepartmentName3-{0}", GenerateName(3)),
                                        JobGradeId = new Guid("53E9BC34-3F92-45B3-B1EE-606E7EE6A77C"),
                                        Type = DepartmentType.Division,
                                        PositionName = $"Position{50 + i}",
                                        PositionCode = $"Position{50 + i}",
                                        ParentId = g4Department.Id
                                    };
                                    _uow.GetRepository<TestDepartment>().Add(g3Department);
                                    for (int d = 0; d < 3; d++)
                                    {
                                        var g2Department = new TestDepartment
                                        {
                                            Code = string.Format("DepartmentCode2-{0}", GenerateName(2)),
                                            Name = string.Format("DepartmentName2-{0}", GenerateName(2)),
                                            JobGradeId = new Guid("F8A9694F-D369-43F5-8625-9C38FA36ABE4"),
                                            Type = DepartmentType.Division,
                                            PositionName = $"Position{60 + i}",
                                            PositionCode = $"Position{60 + i}",
                                            ParentId = g3Department.Id
                                        };
                                        _uow.GetRepository<TestDepartment>().Add(g2Department);
                                        for (int e = 0; e < 5; e++)
                                        {
                                            var g1Department = new TestDepartment
                                            {
                                                Code = string.Format("DepartmentCode1-{0}", GenerateName(1)),
                                                Name = string.Format("DepartmentName1-{0}", GenerateName(1)),
                                                JobGradeId = new Guid("EC1B3149-16BE-4BFB-A150-710B72FFD3B5"),
                                                Type = DepartmentType.Division,
                                                PositionName = $"Position{70 + i}",
                                                PositionCode = $"Position{70 + i}",
                                                ParentId = g2Department.Id
                                            };
                                            _uow.GetRepository<TestDepartment>().Add(g1Department);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            await _uow.CommitAsync();
            return new ResultDTO { };
        }

        public async Task<ResultDTO> GetMultipleLeaveBalanceSet(List<string> sapCodes, int? yearCustom = null)
        {
            string pern = "(";
            for (var i = 0; i < sapCodes.Count; i++)
            {
                pern += "Pernr eq '{" + i + "}'";
                if (sapCodes.Count - i > 1)
                {
                    pern += " or ";
                }
            }
            pern += ")";
            var predicate = "$filter=" + pern + " and Year eq '{" + sapCodes.Count + "}'";
            sapCodes.Add(yearCustom.HasValue ? yearCustom.ToString() : DateTimeOffset.Now.Year.ToString());
            var predicateParameters = sapCodes.ToArray();
            var excution = _externalServiceBO.BuildAPIService(ExtertalType.LeaveBalance);
            excution.APIName = "GetLeaveBalanceSet";
            var res = await excution.GetData(predicate, predicateParameters);
            return new ResultDTO { Object = res };
        }

        public async Task<ResultDTO> UpdateUserInformationQuoteFromSAP(CommonArgs.SAP.UpdateUserInformationQuoteFromSAP args)
        {
            var result = new ResultDTO();
            try
            {
                if (args != null)
                {
                    if (args.Year <= 0)
                    {
                        args.Year = DateTimeOffset.Now.Year;
                    }
                    if (args.SapCodes.Any())
                    {
                        var allUsers = await _uow.GetRepository<User>().FindByAsync(x => args.SapCodes.Contains(x.SAPCode));
                        if (allUsers.Any() && allUsers.Count() > 0)
                        {
                            var resultAPISAP = await GetMultipleLeaveBalanceSet(args.SapCodes, args.Year);
                            if (resultAPISAP.IsSuccess && resultAPISAP.Object != null)
                            {
                                await (new UpdateUserInfoFromSapJob(_log, _uow, _employeeBO, this)).UpdateErdRemain(allUsers, result, args.Year);
                                await _uow.CommitAsync();
                            }
                        }
                    }
                }
            } catch (Exception e)
            {
                result.Messages = new List<string>() { e.Message };
                result.ErrorCodes = new List<int>() {-1};
            }
            return result;
        }

        public async Task<ResultDTO> GetLeaveBalanceSet(string sapCode)
        {
            var predicate = "$filter=Pernr eq '{0}' and Year eq '{1}'";
            var predicateParameters = new string[] { sapCode, DateTimeOffset.Now.Year.ToString() };
            //var predicateParameters = new string[] { sapCode, yearForTest };
            var excution = _externalServiceBO.BuildAPIService(ExtertalType.LeaveBalance);
            excution.APIName = "GetLeaveBalanceSet";
            //fix - 439
            var res = await excution.GetData(predicate, predicateParameters);
            if (res != null)
            {
                List<LeaveBalanceResponceSAPViewModel> result = (List<LeaveBalanceResponceSAPViewModel>) res;
                result = result.Where(x => !string.IsNullOrEmpty(x.AbsenceQuotaType) && x.AbsenceQuotaType != AbsenceQuotaTypeConstants.QUOTA_OVERTIME).ToList();

                var predicateLeave = "UserSapCode = @0 and Status != @1 and Status != @2 and Status != @3 and Status != @4";
                var predicateParametersLeave = new string[] { sapCode, "Draft", "Rejected", "Completed", "Cancelled" };
                var leaveItems = await _uow.GetRepository<LeaveApplication>().FindByAsync<LeaveApplicationViewModel>("Created desc", 1, 1000, predicateLeave, predicateParametersLeave);
                if (leaveItems != null)
                {
                    double DOLF_InUsed = 0;
                    double AL_InUsed = 0;
                    double CF_InUsed = 0;
                    foreach (var item in leaveItems)
                    {
                        var leaveId = item.Id + string.Empty;
                        var predicateLeaveDetails = "LeaveApplicationId = @0";
                        var predicateParametersLeaveDetails = new string[] { leaveId };
                        DateTime startDate = new DateTime(DateTime.Now.Year, 1, 1, 0, 0, 0).ToUniversalTime();
                        DateTime endDate = new DateTime(DateTime.Now.Year + 1, 1, 1, 0, 0, 0).ToUniversalTime();
                        var leaveApplicationDetail = await _uow.GetRepository<LeaveApplicationDetail>().FindByAsync<LeaveApplicantDetailDTO>("Created desc", 1, 1000, predicateLeaveDetails, predicateParametersLeaveDetails);
                        leaveApplicationDetail = leaveApplicationDetail.Where(x => startDate <= x.FromDate && x.ToDate < endDate).ToList();

                        if (leaveApplicationDetail != null)
                        {
                            foreach (var leave in leaveApplicationDetail)
                            {
                                if (leave.LeaveCode == "DOLF" || leave.LeaveCode == "DOH1"
                                    || leave.LeaveCode == "DOH2")
                                {
                                    DOLF_InUsed += leave.Quantity;
                                }
                                else if (leave.LeaveCode == "AL" || leave.LeaveCode == "ALH1"
                                            || leave.LeaveCode == "ALH2")
                                {
                                    bool remainCF = false;
                                    for (int i = 0; i < result.Count; i++)
                                    {
                                        if (result[i].AbsenceQuotaName.Contains("Carry Forward"))
                                        {
                                            remainCF = true;
                                            if (result[i].Remain == CF_InUsed)
                                            {
                                                AL_InUsed += leave.Quantity;
                                            }
                                            else
                                            {
                                                CF_InUsed += leave.Quantity;
                                            }
                                        }
                                    }
									if (!remainCF)
									{
                                        AL_InUsed += leave.Quantity;
                                    }
                                }
                            }
                        }

                    }
                    for (int i = 0; i < result.Count; i++)
                    {
                        if (result[i].AbsenceQuotaName.Contains("Carry Forward"))
                        {
                            result[i].NewRemain = result[i].Remain + " (In Used: " + CF_InUsed + string.Empty + ")";
                        }
                        if (result[i].AbsenceQuotaName.Contains("Annual Leave"))
                        {
                            result[i].NewRemain = result[i].Remain + " (In Used: " + AL_InUsed + string.Empty + ")";
                        }
                        if (result[i].AbsenceQuotaName.Contains("Day Off In Lieu"))
                        {
                            result[i].NewRemain = result[i].Remain + " (In Used: " + DOLF_InUsed + string.Empty + ")";
                        }
                    }
                }
                res = result;
            }
            return new ResultDTO
            {
                Object = res
            };
        }
        public async Task<ResultDTO> GetLeaveBalanceSet(string sapCode, int year)
        {
            var predicate = "$filter=Pernr eq '{0}' and Year eq '{1}'";
            var predicateParameters = new string[] { sapCode, year.ToString() };
            //var predicateParameters = new string[] { sapCode, yearForTest };
            var excution = _externalServiceBO.BuildAPIService(ExtertalType.LeaveBalance);
            excution.APIName = "GetLeaveBalanceSet";
            var res = await excution.GetData(predicate, predicateParameters);
            if (res != null)
            {
                List<LeaveBalanceResponceSAPViewModel> result = (List<LeaveBalanceResponceSAPViewModel>) res;
                DateTime startDate = new DateTime(year, 1, 1, 0, 0, 0).ToUniversalTime();
                DateTime endDate = new DateTime(year+1, 1, 1, 0, 0, 0).ToUniversalTime();
                var predicateLeave = "UserSapCode = @0 and Status != @1 and Status != @2 and Status != @3 and Status != @4";
                var predicateParametersLeave = new string[] { sapCode, "Draft", "Rejected", "Completed", "Cancelled" };
                var leaveItems = await _uow.GetRepository<LeaveApplication>().FindByAsync<LeaveApplicationViewModel>("Created desc", 1, 1000, predicateLeave, predicateParametersLeave);
               
                if (leaveItems != null)
                {
                    double DOLF_InUsed = 0;
                    double AL_InUsed = 0;
                    double CF_InUsed = 0;
                    foreach (var item in leaveItems)
                    {
                        var leaveId = item.Id + string.Empty;
                        var predicateLeaveDetails = "LeaveApplicationId = @0";
                        var predicateParametersLeaveDetails = new string[] { leaveId };
                        var leaveApplicationDetail = await _uow.GetRepository<LeaveApplicationDetail>().FindByAsync<LeaveApplicantDetailDTO>("Created desc", 1, 1000, predicateLeaveDetails, predicateParametersLeaveDetails);
                        leaveApplicationDetail = leaveApplicationDetail.Where(x => startDate <= x.FromDate && x.ToDate < endDate).ToList();
                        if (leaveApplicationDetail != null)
                        {
                            foreach (var leave in leaveApplicationDetail)
                            {
                                if (leave.LeaveCode == "DOLF" || leave.LeaveCode == "DOH1"
                                    || leave.LeaveCode == "DOH2")
                                {
                                    DOLF_InUsed += leave.Quantity;
                                }
                                else if (leave.LeaveCode == "AL" || leave.LeaveCode == "ALH1"
                                            || leave.LeaveCode == "ALH2")
                                {
                                    bool remainCF = false;
                                    for (int i = 0; i < result.Count; i++)
                                    {
                                        if (result[i].AbsenceQuotaName.Contains("Carry Forward"))
                                        {
                                            remainCF = true;
                                            if (result[i].Remain == CF_InUsed)
                                            {
                                                AL_InUsed += leave.Quantity;
                                            }
                                            else
                                            {
                                                CF_InUsed += leave.Quantity;
                                            }
                                        }
                                    }
                                    if (!remainCF)
                                    {
                                        AL_InUsed += leave.Quantity;
                                    }
                                }
                            }
                        }

                    }
                    for (int i = 0; i < result.Count; i++)
                    {
                        if (result[i].AbsenceQuotaName.Contains("Carry Forward"))
                        {
                            result[i].NewRemain = result[i].Remain + " (In Used: " + CF_InUsed + string.Empty + ")";
                        }
                        if (result[i].AbsenceQuotaName.Contains("Annual Leave"))
                        {
                            result[i].NewRemain = result[i].Remain + " (In Used: " + AL_InUsed + string.Empty + ")";
                        }
                        if (result[i].AbsenceQuotaName.Contains("Day Off In Lieu"))
                        {
                            result[i].NewRemain = result[i].Remain + " (In Used: " + DOLF_InUsed + string.Empty + ")";
                        }
                    }
                }
                res = result;
            }
            return new ResultDTO
            {
                Object = res
            };
        }

        public async Task<ResultDTO> GetOvertimeBalanceSet(string sapCode, Guid? OvertimeApplicationid)
        {
            var predicate = "$filter=Pernr eq '{0}' and Year eq '{1}'";
            var predicateParameters = new string[] { sapCode, DateTimeOffset.Now.Year.ToString() };
            var excution = _externalServiceBO.BuildAPIService(ExtertalType.LeaveBalance);
            excution.APIName = "GetLeaveBalanceSet";
            var res = await excution.GetData(predicate, predicateParameters);
            if (res != null)
            {
                try
                {
                    List<LeaveBalanceResponceSAPViewModel> result = (List<LeaveBalanceResponceSAPViewModel>) res;
                    result = result.Where(x => !string.IsNullOrEmpty(x.AbsenceQuotaType) && x.AbsenceQuotaType == AbsenceQuotaTypeConstants.QUOTA_OVERTIME).ToList();

                    var predicateLeaveEmployee = "";
                    var predicateParametersEmployee = new string[] { };
                    if (OvertimeApplicationid == null)
                    {
                        predicateLeaveEmployee = "OvertimeApplication.Type = @5 and OvertimeApplication.UserSapCode = @0 and OvertimeApplication.Status != @1 and OvertimeApplication.Status != @2 and OvertimeApplication.Status != @3 and OvertimeApplication.Status != @4";
                        predicateParametersEmployee = new string[] { sapCode, "Draft", "Rejected", "Completed", "Cancelled", "1" };
                    }
                    else
                    {
                        predicateLeaveEmployee = "OvertimeApplication.Type = @5 and OvertimeApplication.UserSapCode = @0 and OvertimeApplication.Status != @1 and OvertimeApplication.Status != @2 and OvertimeApplication.Status != @3 and OvertimeApplication.Status != @4 and OvertimeApplication.Id != @6";
                        predicateParametersEmployee = new string[] { sapCode, "Draft", "Rejected", "Completed", "Cancelled", "1", OvertimeApplicationid.ToString() };
                    }
                    var overtimeItemEmployee = await _uow.GetRepository<OvertimeApplicationDetail>(true).FindByAsync("Created desc", 1, 1000, predicateLeaveEmployee, predicateParametersEmployee);

                    var predicateOvertimeManager = "";
                    var predicateParametersOvertimeManager = new string[] { };
                    if (OvertimeApplicationid == null)
                    {
                        predicateOvertimeManager = "OvertimeApplication.Type = @5 and SAPCode = @0 and OvertimeApplication.Status != @1 and OvertimeApplication.Status != @2 and OvertimeApplication.Status != @3 and OvertimeApplication.Status != @4";
                        predicateParametersOvertimeManager = new string[] { sapCode, "Draft", "Rejected", "Completed", "Cancelled", "2" };
                    }
                    else
                    {
                        predicateOvertimeManager = "OvertimeApplication.Type = @5 and SAPCode = @0 and OvertimeApplication.Status != @1 and OvertimeApplication.Status != @2 and OvertimeApplication.Status != @3 and OvertimeApplication.Status != @4 and OvertimeApplication.Id != @6";
                        predicateParametersOvertimeManager = new string[] { sapCode, "Draft", "Rejected", "Completed", "Cancelled", "2", OvertimeApplicationid.ToString() };
                    }
                    var overtimeItemManager = await _uow.GetRepository<OvertimeApplicationDetail>(true).FindByAsync("Created desc", 1, 1000, predicateOvertimeManager, predicateParametersOvertimeManager);

                    var allListOvertimes = new List<OvertimeApplicationDetail>();
                    allListOvertimes.AddRange(overtimeItemEmployee.ToList());
                    allListOvertimes.AddRange(overtimeItemManager.ToList());

                    if (allListOvertimes != null)
                    {
                        double Overtime_InUsed = 0;
                        DateTime startDate = new DateTime(DateTime.Now.Year, 1, 1, 0, 0, 0).ToUniversalTime();
                        DateTime endDate = new DateTime(DateTime.Now.Year + 1, 1, 1, 0, 0, 0).ToUniversalTime();
                        allListOvertimes = allListOvertimes.Where(x => startDate <= x.Date && x.Date < endDate).ToList();
                        foreach (var overtime in allListOvertimes)
                        {
                            DateTime? startTime = null;
                            DateTime? endTime = null;
                            if (!string.IsNullOrEmpty(overtime.ActualHoursFrom) && !string.IsNullOrEmpty(overtime.ActualHoursTo))
                            {
                                startTime = DateTime.ParseExact(overtime.ActualHoursFrom, "HH:mm", null);
                                endTime = DateTime.ParseExact(overtime.ActualHoursTo, "HH:mm", null);
                            }
                            else if (!string.IsNullOrEmpty(overtime.ProposalHoursFrom) && !string.IsNullOrEmpty(overtime.ProposalHoursTo))
                            {
                                startTime = DateTime.ParseExact(overtime.ProposalHoursFrom, "HH:mm", null);
                                endTime = DateTime.ParseExact(overtime.ProposalHoursTo, "HH:mm", null);
                            }
                            if (startTime != null && startTime.HasValue && endTime != null && endTime.HasValue)
                            {
                                TimeSpan duration = new TimeSpan();
                                if (startTime.Value > endTime.Value)
                                    duration = (endTime.Value - startTime.Value) + TimeSpan.FromHours(24);
                                else
                                    duration = (endTime.Value - startTime.Value);

                                if (overtime.IsStore == false && duration > TimeSpan.FromHours(4))
                                {
                                    duration = duration - TimeSpan.FromHours(1);
                                }
                                Overtime_InUsed += duration.TotalHours;
                            }
                        }

                        /*result = new List<LeaveBalanceResponceSAPViewModel>() { 
                            new LeaveBalanceResponceSAPViewModel() { 
                                Year = "2023",
                                EmployeeCode = sapCode,
                                Deduction = 13.000,
                                CurrentYearBalance = 300.000,
                                AbsenceQuotaName = "Overtime Application",
                                AbsenceQuotaType = "14"
                            } 
                        };*/

                        for (int i = 0; i < result.Count; i++)
                        {
                            result[i].NewRemain = result[i].OTRemain + " (In Used: " + Overtime_InUsed + string.Empty + ")";
                            result[i].EdocInUsed = Overtime_InUsed;
                        }
                    }
                    res = result;
                } catch (Exception e)
                {

                }
            }
            return new ResultDTO
            {
                Object = res
            };
        }

        public async Task<ResultDTO> GetMultiOvertimeBalanceSet(List<OverTimeBalanceArgs> model)
        {
            if (model.Any())
            {
                var result = new List<LeaveBalanceResponceSAPViewModel>();
                foreach (var item in model)
                {
                    ResultDTO res = await GetOvertimeBalanceSet(item.sapCode, item.OvertimeApplicationid);
                    result.AddRange((List<LeaveBalanceResponceSAPViewModel>)res.Object);
                }
                return new ResultDTO
                {
                    Object = result
                };
            }
            else
            {
                return new ResultDTO
                {
                    Object = null
                };
            }
        }

        public async Task<ResultDTO> GetOTHourInMonth(List<OverTimeBalanceArgs> model)
        {
            try
            {
                if (model.Any())
                {
                    foreach (var item in model) {
                        var OTHour = 0.0;

                        var predicateLeaveEmployee = "";
                        var predicateParametersEmployee = new string[] { };
                        if (item.OvertimeApplicationid == null)
                        {
                            predicateLeaveEmployee = "OvertimeApplication.Type = @4 and OvertimeApplication.UserSapCode = @0 and OvertimeApplication.Status != @1 and OvertimeApplication.Status != @2 and OvertimeApplication.Status != @3";
                            predicateParametersEmployee = new string[] { item.sapCode, "Draft", "Rejected", "Cancelled", "1" };
                        }
                        else
                        {
                            predicateLeaveEmployee = "OvertimeApplication.Type = @4 and OvertimeApplication.UserSapCode = @0 and OvertimeApplication.Status != @1 and OvertimeApplication.Status != @2 and OvertimeApplication.Status != @3 and OvertimeApplication.Id != @5";
                            predicateParametersEmployee = new string[] { item.sapCode, "Draft", "Rejected", "Cancelled", "1", item.OvertimeApplicationid.ToString() };
                        }
                        var overtimeItemEmployee = await _uow.GetRepository<OvertimeApplicationDetail>(true).FindByAsync("Created desc", 1, 1000, predicateLeaveEmployee, predicateParametersEmployee);

                        var predicateOvertimeManager = "";
                        var predicateParametersOvertimeManager = new string[] { };
                        if (item.OvertimeApplicationid == null)
                        {
                            predicateOvertimeManager = "OvertimeApplication.Type = @4 and SAPCode = @0 and OvertimeApplication.Status != @1 and OvertimeApplication.Status != @2 and OvertimeApplication.Status != @3 ";
                            predicateParametersOvertimeManager = new string[] { item.sapCode, "Draft", "Rejected", "Cancelled", "2" };
                        }
                        else
                        {
                            predicateOvertimeManager = "OvertimeApplication.Type = @4 and SAPCode = @0 and OvertimeApplication.Status != @1 and OvertimeApplication.Status != @2 and OvertimeApplication.Status != @3  and OvertimeApplication.Id != @5";
                            predicateParametersOvertimeManager = new string[] { item.sapCode, "Draft", "Rejected", "Cancelled", "2", item.OvertimeApplicationid.ToString() };
                        }
                        var overtimeItemManager = await _uow.GetRepository<OvertimeApplicationDetail>(true).FindByAsync("Created desc", 1, 1000, predicateOvertimeManager, predicateParametersOvertimeManager);

                        var allListOvertimes = new List<OvertimeApplicationDetail>();
                        allListOvertimes.AddRange(overtimeItemEmployee.Where(x=>!x.IsNoOT).ToList());
                        allListOvertimes.AddRange(overtimeItemManager.Where(x => !x.IsNoOT).ToList());

                        if (allListOvertimes != null)
                        {
                            double Overtime_InUsed = 0;

                            DateTime? startDate = null;
                            DateTime? endDate = null;
                            DateTime OTdate = DateTime.ParseExact(item.month, "M/yyyy", CultureInfo.InvariantCulture);

                            if (OTdate.Month == 1)
                            {
                                startDate = new DateTime(OTdate.Year - 1, 12, 26, 0, 0, 0);
                            }
                            else
                            {
                                startDate = new DateTime(OTdate.Year, OTdate.Month - 1, 26, 0, 0, 0);
                            }

                            endDate = new DateTime(OTdate.Year, OTdate.Month, 26, 0, 0, 0);



                            allListOvertimes = allListOvertimes.Where(x => startDate <= x.Date && x.Date < endDate).ToList();
                            foreach (var overtime in allListOvertimes)
                            {
                                DateTime? startTime = null;
                                DateTime? endTime = null;
                                if (!string.IsNullOrEmpty(overtime.ActualHoursFrom) && !string.IsNullOrEmpty(overtime.ActualHoursTo))
                                {
                                    startTime = DateTime.ParseExact(overtime.ActualHoursFrom, "HH:mm", null);
                                    endTime = DateTime.ParseExact(overtime.ActualHoursTo, "HH:mm", null);
                                }
                                else if (!string.IsNullOrEmpty(overtime.ProposalHoursFrom) && !string.IsNullOrEmpty(overtime.ProposalHoursTo))
                                {
                                    startTime = DateTime.ParseExact(overtime.ProposalHoursFrom, "HH:mm", null);
                                    endTime = DateTime.ParseExact(overtime.ProposalHoursTo, "HH:mm", null);
                                }
                                if (startTime != null && startTime.HasValue && endTime != null && endTime.HasValue)
                                {
                                    TimeSpan duration = new TimeSpan();
                                    if (startTime.Value > endTime.Value)
                                        duration = (endTime.Value - startTime.Value) + TimeSpan.FromHours(24);
                                    else
                                        duration = (endTime.Value - startTime.Value);

                                    if (overtime.IsStore == false && duration > TimeSpan.FromHours(4))
                                    {
                                        duration = duration - TimeSpan.FromHours(1);
                                    }
                                    Overtime_InUsed += duration.TotalHours;
                                }
                            }

                            OTHour = Overtime_InUsed;
                            item.OTHour = OTHour;

                        }
                    }
                }
            }
            catch (Exception e)
            {

            }

            return new ResultDTO
            {
                Object = model
            };
        }

        public async Task<ResultDTO> UpdatePayload(UpdatePayloadArgs args)
        {
            var trackingLogInitData = await _uow.GetRepository<TrackingLogInitData>().GetSingleAsync(x => !string.IsNullOrEmpty(x.SAPCode) && x.SAPCode == args.UserSAPCode && !string.IsNullOrEmpty(x.ReferenceNumber) && x.ReferenceNumber == args.ReferenceNumber);
            if (!(trackingLogInitData is null))
            {
                if (trackingLogInitData.TrackingLogId.HasValue)
                {
                    var trackingRequest = await _uow.GetRepository<TrackingRequest>().FindByIdAsync(trackingLogInitData.TrackingLogId.Value);
                    if (!(trackingRequest is null)) {
                        string refernceNumberSubStr = args.ReferenceNumber.Substring(0, 3);
                        switch(refernceNumberSubStr) {
                            case "RES":
                                var resinations = await _uow.GetRepository<ResignationApplication>().GetSingleAsync(x => x.ReferenceNumber == args.ReferenceNumber && x.UserSAPCode == args.UserSAPCode);
                                if (!(resinations is null))
                                {
                                    ResignationJsonInfo data = Mapper.Map<ResignationJsonInfo>(JsonConvert.DeserializeObject<ResignationJsonInfo>(trackingRequest.Payload));
                                    data.Begda = resinations.OfficialResignationDate.LocalDateTime.ToString("yyyyMMdd");
                                    trackingRequest.Payload = JsonConvert.SerializeObject(data);
                                }

                                break;
                            default:
                                return new ResultDTO { Object = null, Messages = new List<string>() { "Cannot ReferenceNumber Type!" }, ErrorCodes = new List<int>() { 400 } };
                        }
                        _uow.GetRepository<TrackingRequest>().Update(trackingRequest);
                        await _uow.CommitAsync();
                        return new ResultDTO { Object = Mapper.Map<TrackingRequestForGetListViewModel>(trackingRequest)};
                    }
                }
            }
            return new ResultDTO { Object = null, Messages = new List<string>() { "Tracking Request Not Found!" }, ErrorCodes = new List<int>() { 404 } };
        }
    }
}