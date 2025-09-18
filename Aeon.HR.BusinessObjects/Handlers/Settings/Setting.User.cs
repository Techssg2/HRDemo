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
using Aeon.HR.Infrastructure.Utilities;
using System.Web.Security;
using Aeon.HR.Infrastructure.Enums;
using System.Configuration;
using System.Text;
using Aeon.HR.ViewModels.Tree;
using AutoMapper.QueryableExtensions;
using Aeon.HR.Infrastructure.Abstracts;
using Newtonsoft.Json;
using System.Threading;
using Aeon.HR.Infrastructure.Constants;
using Extensions = Aeon.HR.Infrastructure.Utilities.Extensions;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Security.Cryptography;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System.Runtime.CompilerServices;

namespace Aeon.HR.BusinessObjects.Handlers
{
    public partial class SettingBO
    {
        private const int NON_CHARACTER_PASSWORD_LENGTH = 1;
        private const string PASSWORDSALT = "Minhtu@#123456";

        public async Task ActiveAllUser(string password)
        {
            try
            {

                //abc
                var users = await _uow.GetRepository<User>().GetAllAsync();
                foreach (var user in users)
                {
                    if (user.Type == LoginType.Membership)
                    {
                        user.IsActivated = true;
                        if (!string.IsNullOrEmpty(user.Email))
                        {
                            var pwd = Membership.GeneratePassword(8, NON_CHARACTER_PASSWORD_LENGTH);
                            var mUser = Membership.GetUser(user.LoginName);
                            if (mUser == null)
                            {
                                mUser = Membership.CreateUser(user.LoginName, pwd);
                            }
                            else
                            {
                                mUser.UnlockUser();
                                var resetPwd = mUser.ResetPassword();
                                // Change the user's password
                                mUser.ChangePassword(resetPwd, pwd);
                            }
                            await SendPasswordNotification(EmailTemplateName.NewMSAccount, user, pwd);
                        }
                        else
                        {
                            if (password.Length >= 8)
                            {
                                var mUser = Membership.GetUser(user.LoginName);
                                if (mUser == null)
                                {
                                    mUser = Membership.CreateUser(user.LoginName, password);
                                }
                                else
                                {
                                    mUser.UnlockUser();
                                    var resetPwd = mUser.ResetPassword();
                                    // Change the user's password
                                    mUser.ChangePassword(resetPwd, password);
                                }
                            }
                        }
                    }
                }
                await _uow.CommitAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Add new User
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<ResultDTO> SAPCreateUser(SAPUserDataForCreatingArgs data)
        {
            var result = new ResultDTO();

            try
            {
                Action<object, string> SaveTrackingRequest = (object response, string status) =>
                {
                    TrackingRequest trackingRequest = new TrackingRequest();
                    trackingRequest.Id = new Guid();
                    trackingRequest.Url = "/api/User/SAPCreateUser";
                    trackingRequest.Payload = JsonConvert.SerializeObject(data);
                    trackingRequest.Created = DateTime.Now;
                    trackingRequest.Modified = DateTime.Now;
                    trackingRequest.HttpStatusCode = "Create";
                    trackingRequest.Status = status;
                    trackingRequest.DeptCode = trackingRequest.DeptCode;

                    trackingRequest.Response = JsonConvert.SerializeObject(response);
                    _uow.GetRepository<TrackingRequest>().Add(trackingRequest);
                    _uow.Commit();
                };



                if (string.IsNullOrEmpty(data.Email))
                {
                    ResultDTO returnData = new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Email is required !" } };
                    SaveTrackingRequest(returnData, "Fail");
                    return returnData;
                }
                if (string.IsNullOrEmpty(data.SAPCode))
                {
                    ResultDTO returnData = new ResultDTO { ErrorCodes = { 1001 }, Messages = { "SAPCode is required !" } };
                    SaveTrackingRequest(returnData, "Fail");
                    return returnData;
                }
                if (string.IsNullOrEmpty(data.FullName))
                {
                    ResultDTO returnData = new ResultDTO { ErrorCodes = { 1001 }, Messages = { "FullName is required !" } };
                    SaveTrackingRequest(returnData, "Fail");
                    return returnData;
                }
                if (string.IsNullOrEmpty(data.DeptCode))
                {
                    ResultDTO returnData = new ResultDTO { ErrorCodes = { 1001 }, Messages = { "DeptCode is required !" } };
                    SaveTrackingRequest(returnData, "Fail");
                    return returnData;
                }
                if (!data.Email.IsValidEmail())
                {
                    ResultDTO returnData = new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Email is not wellform !" } };
                    SaveTrackingRequest(returnData, "Fail");
                    return returnData;
                }

                var existUser = await _uow.GetRepository<User>().FindByAsync(x => x.SAPCode == data.SAPCode);

                #region Create/Re-active membership account
                var generatedPassword = string.Empty;
                var emailType = EmailTemplateName.NewADAccount;
                //If login type is membership, make sure the user will be added
                if (data.Type == LoginType.Membership)
                {
                    var mUser = Membership.GetUser(data.LoginName);
                    if (mUser == null)
                    {
                        generatedPassword = Membership.GeneratePassword(8, NON_CHARACTER_PASSWORD_LENGTH);
                        if (string.IsNullOrEmpty(data.Email))
                        {
                            generatedPassword = data.SAPCode;
                        }
                        var member = Membership.CreateUser(data.LoginName, generatedPassword, data.Email);
                        emailType = EmailTemplateName.NewMSAccount;
                    }
                    else
                    {
                        mUser.Email = data.Email;
                        Membership.UpdateUser(mUser);
                        mUser.UnlockUser();
                        // They exist, so attempt to reset their password
                        var resetPwd = mUser.ResetPassword();
                        generatedPassword = Membership.GeneratePassword(8, NON_CHARACTER_PASSWORD_LENGTH);
                        // Change the user's password
                        mUser.ChangePassword(resetPwd, generatedPassword);
                        emailType = EmailTemplateName.NewMSAccount;
                    }
                }
                #endregion

                //Assign permission to Sharepoint
                await _sharePointBO.AssignUser(data.LoginName);

                #region Create/Update User info
                var user = Mapper.Map<UserListViewModel>(data);
                User userModel = null;
                if (existUser.Any())
                {
                    userModel = existUser.First();
                }
                else
                {
                    userModel = Mapper.Map<User>(user);
                }

                try
                {
                    userModel.IsActivated = true;
                    //
                    var resultJoiningDate = await _employee.GetJoiningDateOfEmployee(userModel.SAPCode);
                    if (resultJoiningDate != null)
                    {
                        userModel.StartDate = resultJoiningDate;
                    }
                    int[] years = new int[] { 2020, 2021 };
                    var sapCodesForGet = new List<string>() { userModel.SAPCode };

                    var allUsers = new List<User>() { userModel };
                    foreach (var year in years)
                    {
                        var leave = await GetMultipleLeaveBalanceSet(sapCodesForGet, year);
                        await UpdateErdRemain(allUsers, leave, year);
                    }

                    if (existUser.Any())
                    {
                        userModel.IsActivated = true;
                        _uow.GetRepository<User>().Update(userModel);
                    }
                    else
                    {
                        _uow.GetRepository<User>().Add(userModel);
                    }
                    result.Object = userModel.Id;

                }
                catch (Exception ex)
                {
                    ResultDTO returnData = new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Create new user failed. " + ex.Message } };
                    SaveTrackingRequest(returnData, "Fail");
                    return returnData;
                }
                await _uow.CommitAsync();
                #endregion

                #region Assign User to department
                try
                {
                    User oUser = _uow.GetRepository<User>().GetSingle(x => x.SAPCode == userModel.SAPCode);
                    Department oDept = _uow.GetRepository<Department>().GetSingle(x => x.SAPCode == data.DeptCode);
                    if (oDept != null)
                    {
                        UserInDepartmentArgs userInDepartmentArgs = new UserInDepartmentArgs();
                        userInDepartmentArgs.IsHeadCount = true;
                        userInDepartmentArgs.Role = Group.Member;
                        userInDepartmentArgs.UserId = oUser.Id;
                        userInDepartmentArgs.DepartmentId = oDept.Id;
                        await MoveHeadCountAdd(userInDepartmentArgs);


                        //Send invitation email to user here for new password 
                        //Khiem - fix bug 453
                        if (emailType == EmailTemplateName.NewMSAccount)
                        {
                            if (!string.IsNullOrEmpty(data.Email))
                            {
                                await SendPasswordNotification(emailType, userModel, generatedPassword);
                            }
                            else
                            {
                                var forceEmail = ConfigurationManager.AppSettings["aeonEmail"];
                                await SendPasswordNotification(emailType, userModel, generatedPassword, forceEmail);
                            }
                        }
                    }
                    else
                    {
                        ResultDTO returnData = new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Cannot find any department with SAP code is  " + data.DeptCode } };
                        SaveTrackingRequest(returnData, "Fail");
                        return returnData;
                    }
                }
                catch (Exception ex)
                {
                    ResultDTO returnData = new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Assign new user to department failed. " + ex.Message } };
                    SaveTrackingRequest(returnData, "Fail");
                    return returnData;
                }
                #endregion
                SaveTrackingRequest(result, "Success");
            }
            catch (MembershipCreateUserException e)
            {
                result.ErrorCodes.Add((int)e.StatusCode);
                result.Messages.Add(e.StatusCode.ToString());
            }
            catch (Exception ex)
            {
                result.ErrorCodes = new List<int>() { 1001 };
                result.Messages = new List<string>() { ex.Message };
            }
            return result;
        }

        public async Task<ResultDTO> CreateUser(UserDataForCreatingArgs data)
        {
            var result = new ResultDTO();
            try
            {
                var existUser = await _uow.GetRepository<User>().FindByAsync(x => x.LoginName == data.LoginName || x.SAPCode == data.SAPCode || x.Email == data.Email && !string.IsNullOrEmpty(data.Email));
                if (existUser.Any())
                {
                    return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "The login name/sap code/email exists !" } };
                }

                if (string.IsNullOrEmpty(data.LoginName))
                {
                    return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Login name is required !" } };
                }
                //if (string.IsNullOrEmpty(data.Email))
                //{
                //    return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Email is required !" } };
                //}
                var generatedPassword = string.Empty;
                var emailType = EmailTemplateName.NewADAccount;
                //If login type is membership, make sure the user will be added
                if (data.Type == LoginType.Membership)
                {
                    var mUser = Membership.GetUser(data.LoginName);
                    if (mUser == null)
                    {
                        generatedPassword = Membership.GeneratePassword(8, NON_CHARACTER_PASSWORD_LENGTH);
                        if (string.IsNullOrEmpty(data.Email))
                        {
                            generatedPassword = data.SAPCode;
                        }
                        var member = Membership.CreateUser(data.LoginName, generatedPassword);
                        emailType = EmailTemplateName.NewMSAccount;
                    }
                    else
                    {
                        mUser.UnlockUser();
                        // They exist, so attempt to reset their password
                        var resetPwd = mUser.ResetPassword();
                        generatedPassword = Membership.GeneratePassword(8, NON_CHARACTER_PASSWORD_LENGTH);
                        // Change the user's password
                        mUser.ChangePassword(resetPwd, generatedPassword);
                        emailType = EmailTemplateName.NewMSAccount;
                    }
                }
                //Assign permission to Sharepoint
                _sharePointBO.AssignUser(data.LoginName);

                var user = Mapper.Map<UserListViewModel>(data);
                var userModel = Mapper.Map<User>(user);
                userModel.IsActivated = true;
                //
                var resultJoiningDate = await _employee.GetJoiningDateOfEmployee(userModel.SAPCode);
                if (resultJoiningDate != null)
                {
                    userModel.StartDate = resultJoiningDate;
                }
                int[] years = new int[] { 2020, 2021 };
                var sapCodesForGet = new List<string>() { userModel.SAPCode };

                var allUsers = new List<User>() { userModel };
                foreach (var year in years)
                {
                    var leave = await GetMultipleLeaveBalanceSet(sapCodesForGet, year);
                    await UpdateErdRemain(allUsers, leave, year);
                }

                _uow.GetRepository<User>().Add(userModel);
                result.Object = userModel.Id;
                // save log
                await _uow.CommitAsync();
                try
                {
                    userModel.HasTrackingLog = true;
                    await _trackingHistoryBO.SaveTrackingHistory(new TrackingHistoryArgs()
                    {
                        DataStr = JsonConvert.SerializeObject(Mapper.Map<CommonViewModel.LogHistories.UserLogViewModel>(userModel)),
                        ItemId = userModel.Id,
                        ItemType = ItemTypeContants.User,
                        Type = TrackingHistoryTypeContants.Create,
                        ItemRefereceNumberOrCode = user.SAPCode,
                    });
                    await _uow.CommitAsync();
                }
                catch (Exception e) { }
                //Send invitation email to user here for new password 
                //Khiem - fix bug 453
                if (emailType == EmailTemplateName.NewMSAccount)
                {
                    if (!string.IsNullOrEmpty(data.Email))
                    {
                        await SendPasswordNotification(emailType, userModel, generatedPassword);
                    }
                    else
                    {
                        var forceEmail = ConfigurationManager.AppSettings["aeonEmail"];
                        await SendPasswordNotification(emailType, userModel, generatedPassword, forceEmail);
                    }
                }
            }
            catch (MembershipCreateUserException e)
            {
                result.ErrorCodes.Add((int)e.StatusCode);
                result.Messages.Add(e.StatusCode.ToString());
            }
            return result;
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
        private async Task UpdateErdRemain(IEnumerable<User> userList, ResultDTO result, int year)
        {

            if ((result != null) && (result.Object != null))
            {
                var listTemp = (List<LeaveBalanceResponceSAPViewModel>)(result.Object);
                if (listTemp != null)
                {
                    string[] types = { "11", "12", "13" };
                    //bool commit = false;
                    var listResult = listTemp.Where(x => types.Contains(x.AbsenceQuotaType)).GroupBy(x => new { x.EmployeeCode, x.Year });
                    foreach (var item in listResult)
                    {
                        //compare remain with sapcode + year remain
                        var user = userList.FirstOrDefault(x => x.SAPCode == item.Key.EmployeeCode && item.Key.Year == year.ToString());
                        if (user != null)
                        {
                            double AlRemain = 0;
                            double ErdRemain = 0;
                            double DoflRemain = 0;
                            QuotaDataJsonDTO quotaData = string.IsNullOrEmpty(user.QuotaDataJson) ? null : JsonConvert.DeserializeObject<QuotaDataJsonDTO>(user.QuotaDataJson);
                            foreach (var rd in item.ToList())
                            {
                                switch (rd.AbsenceQuotaType)
                                {
                                    case "11":
                                        AlRemain = rd.Remain;
                                        break;
                                    case "12":
                                        ErdRemain = rd.Remain;
                                        break;
                                    case "13":
                                        DoflRemain = rd.Remain;
                                        break;
                                }
                            }
                            if (quotaData == null)
                            {
                                quotaData = new QuotaDataJsonDTO();
                                quotaData.JsonData.Add(new QuotaDataJsonDetailDTO
                                {
                                    Year = year,
                                    ALRemain = AlRemain,
                                    DOFLRemain = DoflRemain,
                                    ERDRemain = ErdRemain
                                });
                                //user.AlRemain = AlRemain;
                                //user.DoflRemain = DoflRemain;
                                //user.ErdRemain = ErdRemain;
                            }
                            user.QuotaDataJson = JsonConvert.SerializeObject(quotaData);
                            //_uow.GetRepository<User>().Update(user);
                        }
                    }
                    //await _uow.CommitAsync();
                }
            }
        }
        private async Task SendPasswordNotification(EmailTemplateName type, User user, string password, string forceEmail = "")
        {
            try
            {
                var mergeFields = new Dictionary<string, string>();
                mergeFields["FullName"] = user.FullName;
                mergeFields["LoginName"] = user.LoginName;
                mergeFields["Password"] = password;
                mergeFields["Link"] = $"<a href=\"{ Convert.ToString(ConfigurationManager.AppSettings["siteUrl"])}\">Link</a>";
                var recipients = new List<string>();
                if (string.IsNullOrEmpty(forceEmail))
                {
                    recipients.Add(user.Email);
                }
                else
                {
                    recipients.Add(forceEmail);
                }
                await _emailNotification.SendEmail(type, EmailTemplateName.MainLayout, mergeFields, recipients);
            }
            catch
            {

            }
        }
        /// <summary>
        /// Get List User
        /// </summary>
        /// <returns></returns>
        public async Task<ArrayResultDTO> GetListUsers(QueryArgs args)
        {
            try
            {
                var items = await _uow.GetRepository<User>().FindByAsync<UserListViewModel>(args.Order, args.Page, args.Limit, args.Predicate, args.PredicateParameters);
                var count = await _uow.GetRepository<User>().CountAsync(args.Predicate, args.PredicateParameters);
                var result = new ArrayResultDTO { Data = items, Count = count };
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// Get danh sách user theo từng department
        /// Có thể select trong department hiện tại hoặc bao gồm cả những department con nữa
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<ArrayResultDTO> GetUsersByDivision(Guid? departmentId, GetUserByDivisionEnum type, string currentSapCode)
        {
            var departmentList = new List<Guid?>();
            if (type == GetUserByDivisionEnum.InCurrentDepartment)
            {
                departmentList.Add(departmentId);
            }
            else //GetUserByDivisionEnum.AllChildDepartment
            {
                //Lọc ra division
                if (departmentId.HasValue)
                {
                    var lstDepartment = await _uow.GetRepository<Department>().FindByAsync(x => x.Id == departmentId || x.ParentId == departmentId);
                    foreach (var item in lstDepartment)
                    {
                        if (item.JobGrade.Grade < 5)
                        {
                            departmentList.Add(item.Id);
                        }
                    }
                }
                else
                {
                    var lstDepartment = await _uow.GetRepository<Department>().FindByAsync(x => x.JobGrade.Grade < 5);
                    foreach (var item in lstDepartment)
                    {
                        if (item.JobGrade.Grade < 5)
                        {
                            departmentList.Add(item.Id);
                        }
                    }
                }

            }
            var items = await _uow.GetRepository<User>().FindByAsync<UserListViewModel>(x => x.UserDepartmentMappings.Any(j => j.User.SAPCode == currentSapCode) || x.UserDepartmentMappings.Any(y => departmentList.Contains(y.DepartmentId)));
            var count = items.Count();//await _uow.GetRepository<User>().CountAsync(args.Predicate, args.PredicateParameters);
            var result = new ArrayResultDTO { Data = items, Count = count };
            return result;
        }

        /// <summary>
        /// Get danh sách user theo từng department
        /// Có thể select trong department hiện tại hoặc bao gồm cả những department con nữa
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<ArrayResultDTO> GetChildUsers(Guid departmentId, int limit, int page, string searchText, bool isAll = false)
        {
            var dept = await _uow.GetRepository<Department>().FindByIdAsync(departmentId);
            var allDepts = await _uow.GetRepository<Department>().GetAllAsync();
            if (dept != null)
            {
                var departmentList = new List<Guid>();
                departmentList.Add(departmentId);
                ExpandAllNodes(departmentList, dept, allDepts);
                if (searchText == null)
                {
                    searchText = "";
                }
                var items = await _uow.GetRepository<User>().FindByAsync<UserForTreeViewModel>("FullName", page, limit, x => x.UserDepartmentMappings.Any(y => departmentList.Contains(y.DepartmentId.Value) && (isAll ? true : y.IsHeadCount) && !y.IsFromIT) && (x.SAPCode.Contains(searchText) || x.FullName.ToLower().Contains(searchText.ToLower())) && x.IsActivated);
                //var count = items.Count();//await _uow.GetRepository<User>().CountAsync(args.Predicate, args.PredicateParameters);
                var count = await _uow.GetRepository<User>().CountAsync(x => x.UserDepartmentMappings.Any(y => departmentList.Contains(y.DepartmentId.Value) && (isAll ? true : y.IsHeadCount) && !y.IsFromIT) && (x.SAPCode.Contains(searchText) || x.FullName.ToLower().Contains(searchText.ToLower())) && x.IsActivated);
                var result = new ArrayResultDTO { Data = items, Count = count };
                return result;
            }
            return new ArrayResultDTO { Data = new List<UserListViewModel>(), Count = 0 };
        }
        public async Task<ArrayResultDTO> GetUsersForReportTargetPlan(Guid departmentId, Guid periodId, int limit, int page, string searchText, bool isMade = false)
        {
            var result = new ArrayResultDTO { };
            int count = 0;
            var dept = await _uow.GetRepository<Department>().FindByIdAsync(departmentId);
            var allDepts = await _uow.GetRepository<Department>().GetAllAsync();
            if (dept != null)
            {
                var departmentList = new List<Guid>();
                departmentList.Add(departmentId);
                ExpandAllNodes(departmentList, dept, allDepts);
                if (searchText == null)
                {
                    searchText = "";
                }
                var items = await _uow.GetRepository<User>().FindByAsync<UserForTreeViewModel>(x => x.UserDepartmentMappings.Any(y => departmentList.Contains(y.DepartmentId.Value)) && (x.SAPCode.Contains(searchText) || x.FullName.ToLower().Contains(searchText.ToLower())));
                var sapCodes = items.OrderByDescending(x => x.JobGradeValue).Select(x => x.SAPCode);
                if (sapCodes.Any())
                {
                    var pendingTargetPlans = await _uow.GetRepository<PendingTargetPlanDetail>().FindByAsync<PendingTargetPlanDetailViewModel>(x => sapCodes.Contains(x.SAPCode) && x.PeriodId == periodId);

                    if (isMade)
                    {
                        pendingTargetPlans = pendingTargetPlans.Where(x => x.IsSent);
                        if (pendingTargetPlans.Any())
                        {
                            //code moi
                            var pendingGroupBySAPCode = pendingTargetPlans.GroupBy(x => x.SAPCode);
                            count = pendingGroupBySAPCode.Count();
                            var data = pendingGroupBySAPCode.Skip((page - 1) * limit).Take(limit).SelectMany(x => x.ToList());
                            result.Data = data;
                            result.Count = count;
                            //

                            //count = pendingTargetPlans.Count() / 2;
                            //pendingTargetPlans = pendingTargetPlans.Skip((page - 1) * limit*2).Take(limit*2);
                            //result.Data = pendingTargetPlans;
                            //result.Count = count;
                        }
                    }
                    else
                    {
                        var hasMade = pendingTargetPlans.Where(x => x.IsSent).Select(x => x.SAPCode);
                        var notMade = pendingTargetPlans.Where(x => !x.IsSent).Select(x => x.SAPCode);
                        var notCreateTargets = sapCodes.Where(x => !hasMade.Contains(x) && !notMade.Contains(x));
                        sapCodes = notMade.Concat(notCreateTargets).Distinct();
                        //var cloneSAPCodes = sapCodes;
                        result.Count = sapCodes.Count();
                        var currentUsersInPage = sapCodes.Skip((page - 1) * limit).Take(limit);
                        //var tempData = items.Where(x => currentUsersInPage.Contains(x.SAPCode)).ToList();
                        var tempData = await _uow.GetRepository<User>().FindByAsync<UserForTreeViewModel>(x => x.UserDepartmentMappings.Any(y => departmentList.Contains(y.DepartmentId.Value)) && (x.SAPCode.Contains(searchText) || x.FullName.ToLower().Contains(searchText.ToLower())) && currentUsersInPage.Contains(x.SAPCode));
                        foreach (var item in tempData)
                        {
                            var existItem = pendingTargetPlans.FirstOrDefault(x => x.SAPCode == item.SAPCode);
                            if (existItem != null)
                            {
                                item.Created = existItem.Created;
                            }
                        }
                        result.Data = tempData;
                    }
                }
                //var count = items.Count();//await _uow.GetRepository<User>().CountAsync(args.Predicate, args.PredicateParameters);
                //var count = await _uow.GetRepository<User>().CountAsync(x => x.UserDepartmentMappings.Any(y => departmentList.Contains(y.DepartmentId.Value)) && (x.SAPCode.Contains(searchText) || x.FullName.ToLower().Contains(searchText.ToLower())));

                return result;
            }
            return new ArrayResultDTO { Data = new List<UserListViewModel>(), Count = 0 };
        }

        public async Task<ArrayResultDTO> GetUsersForTargetPlanByDeptId(UserForTargetArg arg)
        {
           var arrayResult = new ArrayResultDTO();
            string message = "";
           try
            {
                bool isContinue = false;
                IEnumerable<ResignationApplicationViewModel> allResinations = null;
                var ids = arg.Items.Select(s => s.Id);
                var currentPeriod = await _uow.GetRepository<TargetPlanPeriod>().FindByIdAsync<TargetPlanPeriodViewModel>(arg.PeriodId.Value);
                int totalItems = 0;
                var res = new List<UserForTreeViewModel>();
                var tempDatas = new List<UserForTreeViewModel>();
                var currentUser = await _uow.GetRepository<User>().GetSingleAsync<UserForTreeViewModel>(x => x.Id == _uow.UserContext.CurrentUserId);
                var jobGrade45 = await _uow.GetRepository<JobGrade>().GetAllAsync();
                int g4 = jobGrade45.FirstOrDefault(x => x.Title.ToUpper().Equals("G4"))?.Grade ?? 4;
                var currentJobGrade = jobGrade45.FirstOrDefault(x => x.Id == currentUser.JobGradeId);
                
                if (currentUser != null)
                {

                    // if (currentUser.IsStore.Value && currentUser.JobGradeValue >= g4 || !currentUser.IsStore.Value && currentUser.JobGradeValue >= g5)
                    if ((currentUser.IsStore.Value && currentJobGrade.StorePosition == StorePositionType.MNG_STORE) || (!currentUser.IsStore.Value && currentJobGrade.HQPosition == HQPositionType.MNG_HQ))
                    {
                        if (arg.PeriodId.HasValue)
                        {
                            //code moi
                            if (arg.Type == TypeTaget.ShiftPlan)
                            {
                                var sapCodes = new List<UserForTreeViewModel>();
                                var sapCodesCompare = new List<string>();
                                var departmentList = new List<Guid>();
                                var dept = new Department();
                                if (arg.DivisionId != null)
                                {
                                    dept = await _uow.GetRepository<Department>().FindByIdAsync(arg.DivisionId.Value);
                                    departmentList.Add(arg.DivisionId.Value);
                                }
                                else
                                {
                                    dept = await _uow.GetRepository<Department>().FindByIdAsync(currentUser.DepartmentId.Value);
                                    departmentList.Add(currentUser.DepartmentId.Value);
                                }
                                if (dept != null)
                                {
                                    var allDepts = await _uow.GetRepository<Department>().GetAllAsync();
                                    ExpandAllNodes(departmentList, dept, allDepts);
                                    foreach (var item in departmentList)
                                    {
                                        var userDepartments = await _uow.GetRepository<UserDepartmentMapping>().FindByAsync(x => x.DepartmentId == item && x.User.IsActivated);
                                        if (userDepartments.Any())
                                        {
                                            var users = userDepartments.Select(x => x.User).ToList();
                                            sapCodes.AddRange(Mapper.Map<List<UserForTreeViewModel>>(users));
                                            sapCodesCompare.AddRange(userDepartments.Select(x => x.User.SAPCode));
                                        }
                                    }
                                }
                                if (sapCodes.Any())
                                {
                                    tempDatas.AddRange(sapCodes);
                                    //tempDatas = tempDatas.Skip((arg.Query.Page - 1) * arg.Query.Limit).Take(arg.Query.Limit).ToList();
                                }
                                var allPendingTargetPlans = await _uow.GetRepository<PendingTargetPlanDetail>().FindByAsync<PendingTargetPlanDetailViewModel>(x => x.PendingTargetPlan.PeriodId == arg.PeriodId && sapCodesCompare.Contains(x.SAPCode) || ids.Contains(x.PendingTargetPlan.DeptId.Value) || ids.Contains(x.PendingTargetPlan.DivisionId.Value));
                                foreach (var item in tempDatas)
                                {
                                    var currentDetailItem = allPendingTargetPlans.FirstOrDefault(x => x.SAPCode == item.SAPCode);
                                    if (currentDetailItem != null)
                                    {
                                        item.IsSent = currentDetailItem.IsSent;
                                        item.IsSubmitted = currentDetailItem.IsSubmitted;
                                    }
                                }
                                //
                                tempDatas.ForEach(x =>
                                {
                                    if (!x.IsSent.HasValue)
                                    {
                                        x.IsSent = false;
                                    }
                                    if (!x.IsSubmitted.HasValue)
                                    {
                                        x.IsSubmitted = false;
                                    }
                                });
                                if (!String.IsNullOrEmpty(arg.Query.Predicate))
                                {
                                    tempDatas = tempDatas.AsQueryable().Where(arg.Query.Predicate, arg.Query.PredicateParameters).ToList();
                                }
                                if (tempDatas.Any())
                                {
                                    res.AddRange(tempDatas);
                                }
                            }
                            else
                            {
                                var isValid = true;
                                if (!currentUser.StartDate.HasValue)
                                {
                                    currentUser.StartDate = new DateTime(2016, 01, 01);
                                }
                                if (currentUser.StartDate <= currentPeriod.ToDate)
                                {
                                    tempDatas.Add(currentUser);
                                    totalItems = 1;
                                }
                                var lastResignation = await _uow.GetRepository<ResignationApplication>(true).GetSingleAsync<ResignationApplicationViewModel>(x => x.Status.Contains("Completed") && currentUser.SAPCode == x.UserSAPCode, "Created desc");
                                if (lastResignation != null)
                                {
                                    if (currentUser.StartDate.HasValue && lastResignation.OfficialResignationDate.Date >= currentUser.StartDate.Value.Date)
                                    {
                                        if (lastResignation.OfficialResignationDate.Date >= currentPeriod.FromDate.Date && lastResignation.OfficialResignationDate.Date <= currentPeriod.ToDate.Date)
                                        {
                                            currentUser.OfficialResignationDate = lastResignation.OfficialResignationDate;
                                        }
                                        else if (currentUser.StartDate.Value.Date <= currentPeriod.ToDate.Date && lastResignation.OfficialResignationDate.Date.Date > currentPeriod.FromDate.Date)
                                        {
                                            currentUser.OfficialResignationDate = null;
                                        }
                                        else
                                        {
                                            isValid = false;
                                        }
                                    }
                                }
                                if (isValid)
                                {
                                    var allPendingTargetPlans = await _uow.GetRepository<PendingTargetPlanDetail>().FindByAsync<PendingTargetPlanDetailViewModel>(x => x.PendingTargetPlan.PeriodId == arg.PeriodId && x.SAPCode == currentUser.SAPCode || ids.Contains(x.PendingTargetPlan.DeptId.Value) || ids.Contains(x.PendingTargetPlan.DivisionId.Value));
                                    foreach (var item in tempDatas)
                                    {
                                        var currentDetailItem = allPendingTargetPlans.FirstOrDefault(x => x.SAPCode == item.SAPCode);
                                        if (currentDetailItem != null)
                                        {
                                            item.IsSent = currentDetailItem.IsSent;
                                            item.IsSubmitted = currentDetailItem.IsSubmitted;
                                        }
                                    }
                                    tempDatas.ForEach(x =>
                                    {
                                        if (!x.IsSent.HasValue)
                                        {
                                            x.IsSent = false;
                                        }
                                        if (!x.IsSubmitted.HasValue)
                                        {
                                            x.IsSubmitted = false;
                                        }
                                    });
                                    if (!String.IsNullOrEmpty(arg.Query.Predicate))
                                    {
                                        tempDatas = tempDatas.AsQueryable().Where(arg.Query.Predicate, arg.Query.PredicateParameters).ToList();
                                    }
                                    if (tempDatas.Any())
                                    {
                                        res.AddRange(tempDatas);
                                    }
                                }
                            }

                        }
                        else
                        {
                            res.AddRange(tempDatas);
                        }
                    }
                    else
                    {
                        isContinue = true;
                    }

                }
                if (isContinue)
                {
                    message = "isContinue";
                    var invalidUsers = new List<string>(); // check resignation date
                    var allSubmitPersons = await _uow.GetRepository<UserSubmitPersonDeparmentMapping>().FindByAsync<UserSubmitPersonDepartmentMappingViewModel>(x => x.IsSubmitPerson);
                    var allDepartments = await _uow.GetRepository<Department>().GetAllAsync<DepartmentTreeViewModel>();

                    message = "allDepartments";
                    try
                    {
                        foreach (var department in arg.Items)
                        {
                            var dept = allDepartments.FirstOrDefault(x => x.Id == department.Id);
                            DepartmentTreeViewModel forcusDerpartment = null;
                            if (dept != null && dept.Type == DepartmentType.Department)
                            {
                                // Tìm department có người submit
                                var hasSubmitPerson = allSubmitPersons.Any(x => x.DepartmentId == dept.Id);
                                while (!hasSubmitPerson && dept != null && dept.ParentId.HasValue)
                                {
                                    dept = allDepartments.FirstOrDefault(x => x.Id == dept.ParentId);
                                    if (dept != null)
                                    {
                                        hasSubmitPerson = allSubmitPersons.Any(x => x.DepartmentId == dept.Id);
                                    }
                                }
                                if (hasSubmitPerson)
                                {
                                    forcusDerpartment = dept;
                                }

                            }
                            else
                            {
                                forcusDerpartment = dept;
                            }
                            var currentUsers = new List<UserForTreeViewModel>();
                            if (forcusDerpartment != null)
                            {
                                forcusDerpartment.IsIncludeChildren = department.IsIncludeChildren;
                                var departmentList = new List<Guid>();
                                if (arg.IsNoDivisionChosen)
                                {
                                    if (forcusDerpartment.JobGradeGrade < g4)
                                    {
                                        while (forcusDerpartment.JobGradeGrade < g4)
                                        {
                                            forcusDerpartment = await _uow.GetRepository<Department>().FindByIdAsync<DepartmentTreeViewModel>(forcusDerpartment.ParentId.Value);
                                        }
                                    }
                                    if (forcusDerpartment.ParentId.HasValue)
                                    {
                                        departmentList.Add(forcusDerpartment.ParentId.Value);
                                    }
                                }
                                departmentList.Add(forcusDerpartment.Id);
                                currentUsers = await GetDetailUserInTargetPlan(departmentList, forcusDerpartment, allDepartments, allSubmitPersons);
                            }
                            else
                            {
                                var departmentList = new List<Guid>();
                                var currentDept = allDepartments.FirstOrDefault(x => x.Id == department.Id);
                                departmentList.Add(department.Id);
                                currentUsers = await GetDetailUserInTargetPlan(departmentList, currentDept, allDepartments, allSubmitPersons);
                            }
                            if (currentUsers.Count > 0)
                            {
                                //Khiem - Fix target plan load user co start date sau period
                                currentUsers.RemoveAll(x => x.StartDate > currentPeriod.ToDate);
                                tempDatas.AddRange(currentUsers);
                            }
                        }
                    } catch (Exception e)
                    {
                        message = e.Message;
                    }
                    var sapCodes = tempDatas.Select(x => x.SAPCode);
                    allResinations = await _uow.GetRepository<ResignationApplication>(true).FindByAsync<ResignationApplicationViewModel>(x => x.Status.Contains("Completed") && sapCodes.Contains(x.UserSAPCode), "Created desc");
                    if (allResinations.Any())
                    {
                        foreach (var item in allResinations)
                        {
                            var condition1 = item.OfficialResignationDate < currentPeriod.FromDate;
                            var condition2 = tempDatas.FirstOrDefault(x => x.SAPCode == item.UserSAPCode)?.StartDate.Value.Date < item.OfficialResignationDate.Date;
                            if (condition1 && condition2)
                            {
                                invalidUsers.Add(item.UserSAPCode);
                            }
                        }
                    }
                    message = "arg.PeriodId.HasValue";
                    if (arg.PeriodId.HasValue)
                    {
                        var allPendingTargetPlans = await _uow.GetRepository<PendingTargetPlanDetail>().FindByAsync<PendingTargetPlanDetailViewModel>(x => x.PendingTargetPlan.PeriodId == arg.PeriodId && (sapCodes.Contains(x.SAPCode) || ids.Contains(x.PendingTargetPlan.DeptId.Value) || ids.Contains(x.PendingTargetPlan.DivisionId.Value)));
                        foreach (var item in tempDatas)
                        {
                            var currentDetailItem = allPendingTargetPlans.FirstOrDefault(x => x.SAPCode == item.SAPCode);
                            if (currentDetailItem != null)
                            {
                                item.IsSent = currentDetailItem.IsSent;
                                item.IsSubmitted = currentDetailItem.IsSubmitted;
                            }
                        }
                        tempDatas.ForEach(x =>
                        {
                            if (!x.IsSent.HasValue)
                            {
                                x.IsSent = false;
                            }
                            if (!x.IsSubmitted.HasValue)
                            {
                                x.IsSubmitted = false;
                            }
                        });
                        if (arg.Query != null && !String.IsNullOrEmpty(arg.Query.Predicate))
                        {
                            tempDatas = tempDatas.AsQueryable().Where(arg.Query.Predicate, arg.Query.PredicateParameters).ToList();
                        }

                    }

                    if (arg.ActiveUsers != null && arg.ActiveUsers.Length > 0)
                    {
                        message = "arg.ActiveUsers";
                        var currenrtActiveUsers = tempDatas.Where(x => arg.ActiveUsers.ToList().Contains(x.SAPCode));
                        if (currenrtActiveUsers.Any())
                        {

                            tempDatas = tempDatas.Where(x => !currenrtActiveUsers.Contains(x)).ToList();
                            res.AddRange(currenrtActiveUsers);
                            res.AddRange(tempDatas);
                        }
                        else
                        {
                            res.AddRange(tempDatas);
                        }
                    }
                    else
                    {
                        res.AddRange(tempDatas);
                    }
                    message = " res = res.Where(x => !invalidUsers.Contains(x.SAPCode)).";
                    res = res.Where(x => !invalidUsers.Contains(x.SAPCode)).OrderByDescending(x => x.JobGradeValue).ToList();
                    message = " res = res.Where(x => !invalidUsers.Contains(x.SAPCode)).";
                    var notIncludeSAPCodes = new List<string>();
                    foreach (var item in res)
                    {
                        if ((await IsG5UpHQOrG4Store(item.SAPCode, currentUser.SAPCode)))
                        {
                            notIncludeSAPCodes.Add(item.SAPCode);
                        }
                    }
                    if (notIncludeSAPCodes.Any())
                    {
                        res = res.Where(x => !notIncludeSAPCodes.Contains(x.SAPCode)).ToList();
                    }
                    totalItems = res.Count();
                    //if (arg.Query != null)
                    //{
                    //    res = res.Skip((arg.Query.Page - 1) * arg.Query.Limit).Take(arg.Query.Limit).ToList();
                    //}

                }
                if (res.Any() && allResinations != null)
                {
                    message = "allResinations";
                    var removeUsers = new List<string>();
                    foreach (var item in res)
                    {
                        var currentResignation = allResinations.FirstOrDefault(x => x.UserSAPCode == item.SAPCode);
                        if (currentResignation != null)
                        {

                            if (currentResignation.OfficialResignationDate >= item.StartDate)
                            {
                                if (currentResignation.OfficialResignationDate.Date <= currentPeriod.FromDate.Date)
                                {
                                    removeUsers.Add(item.SAPCode);
                                }
                                else
                                {
                                    item.OfficialResignationDate = currentResignation.OfficialResignationDate;
                                }
                            }
                        }
                    }
                    if (removeUsers.Count > 0)
                    {
                        res = res.Where(x => !removeUsers.Contains(x.SAPCode)).ToList();
                        totalItems = res.Count();
                    }
                }
                message = "// kiem tra them nhan vien co can lam targetplant hay khong";
                // kiem tra them nhan vien co can lam targetplant hay khong
                if (res.Any())
                {
                    message = "if (res.Any())";
                    res = res.Where(x => x.IsNotTargetPlan == false).ToList();
                    // kiem tra user hien tai co quyen lam target lan hay khong
                    if (currentUser.IsNotTargetPlan == true)
                    {
                        res = new List<UserForTreeViewModel>();
                    }
                    totalItems = res.Count();
                }
                //totalItems = res.Count();
                //if (arg.Type == TypeTaget.ShiftPlan)
                //{
                //    totalItems = res.Count();
                //    res = res.Skip((arg.Query.Page - 1) * arg.Query.Limit).Take(arg.Query.Limit).ToList();
                //}
                if (arg.Query != null)
                {
                    res = res.Skip((arg.Query.Page - 1) * arg.Query.Limit).Take(arg.Query.Limit).ToList();
                }
                arrayResult = new ArrayResultDTO { Data = res, Count = totalItems };
            }
            catch (Exception e)
            {
                arrayResult = new ArrayResultDTO { Data = message + " - " + e.Message, Count = 0 };
            }
            return arrayResult;
        }

        private async Task<bool> IsG5UpHQOrG4Store(string checkSAPCode, string currentLoginSapCode)
        {
            var result = false;
            try
            {
                var jobGrade45 = await _uow.GetRepository<JobGrade>().FindByAsync(x => x.Title.ToUpper().Equals("G4") || x.Title.ToUpper().Equals("G5"));
                int g4 = jobGrade45.FirstOrDefault(x => x.Title.ToUpper().Equals("G4"))?.Grade ?? 4;
                int g5 = jobGrade45.FirstOrDefault(x => x.Title.ToUpper().Equals("G5"))?.Grade ?? 5;
                var currentUser = await _uow.GetRepository<User>().GetSingleAsync<UserForTreeViewModel>(x => x.SAPCode == checkSAPCode && x.IsActivated);
                // if (currentUser != null && currentUser.SAPCode != currentLoginSapCode && ((currentUser.JobGradeValue.Value >= 5 && !currentUser.IsStore.Value) || (currentUser.JobGradeValue.Value >= 4 && currentUser.IsStore.Value)))
                if (currentUser != null && currentUser.SAPCode != currentLoginSapCode && ((currentUser.JobGradeValue.Value >= g5 && !currentUser.IsStore.Value) || (currentUser.JobGradeValue.Value >= g4 && currentUser.IsStore.Value)))
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {

            }

            return result;
        }
        public async Task<List<string>> GetValidUsersForSubmitTargetPlan(UserForTargetArg arg)
        {
            var res = new List<UserForTreeViewModel>();
            List<string> resultStr = new List<string>();
            try
            {
                var tempDatas = new List<UserForTreeViewModel>();
                var isContinue = false;
                var ids = arg.Items.Select(s => s.Id);
                var invalidUsers = new List<string>(); // check resignation date
                var allSubmitPersons = await _uow.GetRepository<UserSubmitPersonDeparmentMapping>().FindByAsync<UserSubmitPersonDepartmentMappingViewModel>(x => x.IsSubmitPerson);
                var allDepartments = await _uow.GetRepository<Department>().GetAllAsync<DepartmentTreeViewModel>();
                var currentPeriod = await _uow.GetRepository<TargetPlanPeriod>().FindByIdAsync<TargetPlanPeriodViewModel>(arg.PeriodId.Value);
                var currentUser = await _uow.GetRepository<User>().GetSingleAsync<UserForTreeViewModel>(x => x.Id == _uow.UserContext.CurrentUserId);

                var allJobGrade = await _uow.GetRepository<JobGrade>().GetAllAsync();
                var currentJobGrade = allJobGrade.FirstOrDefault(x => x.Id == currentUser.JobGradeId);
                if (currentUser != null)
                {
                    // if (currentUser.IsStore.Value && currentUser.JobGradeValue >= g4 || !currentUser.IsStore.Value && currentUser.JobGradeValue >= g5)
                    if ((currentUser.IsStore.Value && currentJobGrade.StorePosition == StorePositionType.MNG_STORE) || (!currentUser.IsStore.Value && currentJobGrade.HQPosition == HQPositionType.MNG_HQ))
                    {
                        if (currentUser.StartDate <= currentPeriod.ToDate)
                        {
                            tempDatas.Add(currentUser);
                        }
                        if (arg.PeriodId.HasValue)
                        {
                            var allPendingTargetPlans = await _uow.GetRepository<PendingTargetPlanDetail>().FindByAsync<PendingTargetPlanDetailViewModel>(x => x.PeriodId == arg.PeriodId.Value && x.PendingTargetPlan.PeriodId == arg.PeriodId && x.SAPCode == currentUser.SAPCode || ids.Contains(x.PendingTargetPlan.DeptId.Value) || ids.Contains(x.PendingTargetPlan.DivisionId.Value));
                            foreach (var item in tempDatas)
                            {
                                var currentDetailItem = allPendingTargetPlans.FirstOrDefault(x => x.SAPCode == item.SAPCode);
                                if (currentDetailItem != null && currentDetailItem.PeriodId == arg.PeriodId)
                                {
                                    item.IsSent = currentDetailItem.IsSent;
                                    item.IsSubmitted = currentDetailItem.IsSubmitted;
                                }
                            }
                            tempDatas.ForEach(x =>
                            {
                                if (!x.IsSent.HasValue)
                                {
                                    x.IsSent = false;
                                }
                                if (!x.IsSubmitted.HasValue)
                                {
                                    x.IsSubmitted = false;
                                }
                            });
                            if (!String.IsNullOrEmpty(arg.Query.Predicate))
                            {
                                tempDatas = tempDatas.AsQueryable().Where(arg.Query.Predicate, arg.Query.PredicateParameters).ToList();
                            }
                            if (tempDatas.Any())
                            {
                                res.AddRange(tempDatas);
                            }
                        }
                        else
                        {
                            res.AddRange(tempDatas);
                        }
                    }
                    else
                    {
                        isContinue = true;
                    }
                }
                if (isContinue)
                {
                    try
                    {
                        foreach (var department in arg.Items)
                        {
                            var dept = allDepartments.FirstOrDefault(x => x.Id == department.Id);
                            DepartmentTreeViewModel forcusDerpartment = null;
                            if (dept != null && dept.Type == DepartmentType.Department)
                            {
                                // Tìm department có người submit
                                var hasSubmitPerson = allSubmitPersons.Any(x => x.DepartmentId == dept.Id);
                                while (!hasSubmitPerson && dept != null && dept.ParentId.HasValue)
                                {
                                    dept = allDepartments.FirstOrDefault(x => x.Id == dept.ParentId);
                                    if (dept != null)
                                    {
                                        hasSubmitPerson = allSubmitPersons.Any(x => x.DepartmentId == dept.Id);
                                    }
                                }
                                if (hasSubmitPerson)
                                {
                                    forcusDerpartment = dept;
                                }

                            }
                            else
                            {
                                forcusDerpartment = dept;
                            }
                            var currentUsers = new List<UserForTreeViewModel>();
                            if (forcusDerpartment != null)
                            {
                                var departmentList = new List<Guid>();
                                var currentJobGradeDepartment = allJobGrade.FirstOrDefault(x => x.Id == forcusDerpartment.JobGradeId);
                                //if ((!forcusDerpartment.IsStore && forcusDerpartment.JobGradeGrade < g5) || (forcusDerpartment.IsStore && forcusDerpartment.JobGradeGrade < g4))
                                if ((!forcusDerpartment.IsStore && currentJobGradeDepartment.HQPosition == HQPositionType.EXE_HQ) || (forcusDerpartment.IsStore && currentJobGradeDepartment.StorePosition == StorePositionType.EXE_STORE))
                                {
                                    departmentList.Add(forcusDerpartment.Id);
                                }
                                currentUsers = await GetDetailUserInTargetPlan(departmentList, forcusDerpartment, allDepartments, allSubmitPersons);
                            }
                            else
                            {
                                var departmentList = new List<Guid>();
                                var currentDept = allDepartments.FirstOrDefault(x => x.Id == department.Id);
                                departmentList.Add(department.Id);
                                currentUsers = await GetDetailUserInTargetPlan(departmentList, currentDept, allDepartments, allSubmitPersons);
                            }
                            if (currentUsers.Count > 0)
                            {
                                //Khiem - Fix target plan load user co start date sau period
                                currentUsers.RemoveAll(x => x.StartDate > currentPeriod.ToDate);

                                tempDatas.AddRange(currentUsers);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        resultStr.Add("Error: " + e.Message);
                    }
                }
                var sapCodes = tempDatas.Select(x => x.SAPCode);
                var allResinations = await _uow.GetRepository<ResignationApplication>(true).FindByAsync<ResignationApplicationViewModel>(x => x.Status.Contains("Completed") && sapCodes.Contains(x.UserSAPCode));
                if (allResinations.Any())
                {

                    foreach (var item in allResinations)
                    {
                        var condition1 = item.OfficialResignationDate.Date <= currentPeriod.FromDate.Date;
                        var condition2 = tempDatas.FirstOrDefault(x => x.SAPCode == item.UserSAPCode)?.StartDate.Value.Date < item.OfficialResignationDate.Date;
                        if (condition1 && condition2)
                        {
                            invalidUsers.Add(item.UserSAPCode);
                        }
                    }
                }
                if (arg.PeriodId.HasValue)
                {
                    var allPendingTargetPlans = await _uow.GetRepository<PendingTargetPlanDetail>().FindByAsync<PendingTargetPlanDetailViewModel>(x => x.PeriodId == arg.PeriodId && (sapCodes.Contains(x.SAPCode)));
                    foreach (var item in tempDatas)
                    {
                        var currentDetailItem = allPendingTargetPlans.FirstOrDefault(x => x.SAPCode == item.SAPCode);
                        if (currentDetailItem != null)
                        {
                            item.IsSent = currentDetailItem.IsSent;
                            item.IsSubmitted = currentDetailItem.IsSubmitted;
                        }
                    }
                    tempDatas.ForEach(x =>
                    {
                        if (!x.IsSent.HasValue)
                        {
                            x.IsSent = false;
                        }
                        if (!x.IsSubmitted.HasValue)
                        {
                            x.IsSubmitted = false;
                        }
                    });
                    if (!String.IsNullOrEmpty(arg.Query.Predicate))
                    {
                        tempDatas = tempDatas.AsQueryable().Where(arg.Query.Predicate, arg.Query.PredicateParameters).ToList();
                    }

                }
                if (arg.ActiveUsers != null && arg.ActiveUsers.Length > 0)
                {
                    var currenrtActiveUsers = tempDatas.Where(x => arg.ActiveUsers.ToList().Contains(x.SAPCode));
                    if (currenrtActiveUsers.Any())
                    {

                        tempDatas = tempDatas.Where(x => !currenrtActiveUsers.Contains(x)).ToList();
                        res.AddRange(currenrtActiveUsers);
                        res.AddRange(tempDatas);
                    }
                    else
                    {
                        res.AddRange(tempDatas);
                    }
                }
                else
                {
                    res.AddRange(tempDatas);
                }
                resultStr = res.Where(x => !invalidUsers.Contains(x.SAPCode) && !x.IsSubmitted.Value && x.IsNotTargetPlan == false).OrderByDescending(x => x.JobGradeValue).Select(x => x.SAPCode).Distinct().ToList();
            } catch (Exception e)
            {
                resultStr.Add("Error: " + e.Message);
            }
            return resultStr;
        }

        private async Task<List<UserForTreeViewModel>> GetDetailUserInTargetPlan(List<Guid> departmentList, DepartmentTreeViewModel focusDepartment, IEnumerable<DepartmentTreeViewModel> allDepartments, IEnumerable<UserSubmitPersonDepartmentMappingViewModel> allSubmitPersons)
        {
            if (focusDepartment != null)
            {
                if (focusDepartment.IsIncludeChildren.HasValue)
                {
                    if (focusDepartment.IsIncludeChildren == true)
                    {
                        CustomExpandAllNodes(departmentList, focusDepartment, allDepartments);
                    }
                    else
                    {
                        departmentList = new List<Guid> { focusDepartment.Id };
                    }
                }
                else
                {
                    CustomExpandAllNodes(departmentList, focusDepartment, allDepartments);
                }
            }
            
            var items = (await _uow.GetRepository<User>().FindByAsync<UserForTreeViewModel>(x => x.IsActivated && x.UserDepartmentMappings.Any(y => departmentList.Contains(y.DepartmentId.Value) && y.IsHeadCount))).ToList();
            //if (items.Any())
            //{
            //    var removeUsers = new List<UserForTreeViewModel>();
            //    var currentSubmitPersons = allSubmitPersons.Where(x => x.DepartmentId == focusDepartment.Id);
            //    foreach (var item in items)
            //    {
            //        var submitPersonItems = allSubmitPersons.Where(x => x.DepartmentId == item.DepartmentId && currentSubmitPersons.All(t => t.UserId != x.UserId) && item.Id != _uow.UserContext.CurrentUserId);
            //        if (submitPersonItems.Any())
            //        {
            //            if (item.DepartmentId != focusDepartment.ParentId)
            //            {
            //                removeUsers.Add(item);
            //            }
            //        }
            //    }
            //    if (removeUsers.Any())
            //    {
            //        foreach (var item in removeUsers)
            //        {
            //            items.Remove(item);
            //        }
            //    }
            //}
            return items;
        }

        private void ExpandAllNodes(List<Guid> deptList, Department parentNode, IEnumerable<Department> allNodes)
        {
            var childNodes = allNodes.Where(x => x.ParentId == parentNode.Id).ToList();
            if (childNodes.Count() > 0)
            {
                deptList.AddRange(childNodes.Select(x => x.Id).ToList());
                foreach (var childNode in childNodes)
                {
                    ExpandAllNodes(deptList, childNode, allNodes);
                }
            }
        }

        private void CustomExpandAllNodes(List<Guid> deptList, DepartmentTreeViewModel parentNode, IEnumerable<DepartmentTreeViewModel> allNodes)
        {
            var childNodes = allNodes.Where(x => x.ParentId == parentNode.Id).ToList();
            if (childNodes.Count() > 0)
            {
                deptList.AddRange(childNodes.Select(x => x.Id).ToList());
                foreach (var childNode in childNodes)
                {
                    CustomExpandAllNodes(deptList, childNode, allNodes);
                }
            }
        }

        private bool CheckIsChild(Dictionary<Guid, DepartmentForTreeViewModel> deptDic, DepartmentForTreeViewModel dept)
        {
            DepartmentForTreeViewModel deptValue = null;
            if (!deptDic.TryGetValue(dept.Id, out deptValue)) // Nếu tìm không thấy dept, chứng tỏ đã bị remove
            {
                return false;
            }

            if (deptValue.isFlag == true) // Nếu node đã là 1 phần của cây.
            {
                return true;
            }

            if (deptValue.ParentId == null) // Nếu node là 1 cây khác hoặc nằm ngoài phạm vi.
            {
                deptDic.Remove(dept.Id); // Remove node này.
                return false;
            }

            DepartmentForTreeViewModel parentDeptValue = null;
            if (!deptDic.TryGetValue(deptValue.ParentId.Value, out parentDeptValue)) // Nếu tìm không thấy dept, chứng tỏ đã bị remove
            {
                deptDic.Remove(dept.Id); // Remove node này.
                return false;
            }

            if (CheckIsChild(deptDic, parentDeptValue))  // Thử check parent xem có đạt đủ điều kiện hay không.
            {
                deptValue.isFlag = true;
                return true;
            }
            else
            {
                deptDic.Remove(dept.Id); // Remove node này.
                return false;
            }
        }

        public async Task<ArrayResultDTO> GetUsersByOnlyDeptLine(Guid depLineId, int limit, int page, string searchText = "", bool isAll = true)
        {
            var allDeptList = await _uow.GetRepository<Department>().FindByAsync<DepartmentForTreeViewModel>(x => x.Type == DepartmentType.Division || x.Id == depLineId);
            Dictionary<Guid, DepartmentForTreeViewModel> deptDic = allDeptList.ToDictionary(x => x.Id, x => x); // Tạo 1 dictionary từ dept list
            deptDic[depLineId].isFlag = true; // Đánh dấu GUID truyền lên có trong dict là 1 điểm gốc của cây

            foreach (var dept in allDeptList) // Tiến hành lọc các dept trong dictionary
            {
                CheckIsChild(deptDic, dept);
            }
            if (searchText == null)
            {
                searchText = "";
            }
            var user = await _uow.GetRepository<User>().FindByAsync<UserForTreeViewModel>("FullName", page, limit, (x => x.UserDepartmentMappings.Any(y => deptDic.Keys.Contains(y.DepartmentId.Value) && (isAll ? true : y.IsHeadCount)) && (x.SAPCode.Contains(searchText) || x.FullName.ToLower().Contains(searchText.ToLower()))));
            var count = await _uow.GetRepository<User>().CountAsync(x => (x.UserDepartmentMappings.Any(y => deptDic.Keys.Contains(y.DepartmentId.Value) && (isAll ? true : y.IsHeadCount))) && (x.SAPCode.Contains(searchText) || x.FullName.ToLower().Contains(searchText.ToLower())));
            return new ArrayResultDTO { Data = user, Count = count };
        }

        public async Task<ArrayResultDTO> GetUsersByDeptLines(CommonArgs.Department.GetUsersByDeptLines args, bool isAll = false)
        {
            if (args.DepLineIds.Any())
            {
                Dictionary<Guid, DepartmentForTreeViewModel> deptDictionary = new Dictionary<Guid, DepartmentForTreeViewModel>();
                foreach (var depLineId in args.DepLineIds)
                {
                    var allDeptList = await _uow.GetRepository<Department>().FindByAsync<DepartmentForTreeViewModel>(x => x.Type == DepartmentType.Division || x.Id == depLineId);
                    Dictionary<Guid, DepartmentForTreeViewModel> deptDic = allDeptList.ToDictionary(x => x.Id, x => x); // Tạo 1 dictionary từ dept list
                    deptDic[depLineId].isFlag = true; // Đánh dấu GUID truyền lên có trong dict là 1 điểm gốc của cây

                    foreach (var dept in allDeptList) // Tiến hành lọc các dept trong dictionary
                    {
                        CheckIsChild(deptDic, dept);
                    }
                    if (deptDic.Any())
                    {
                        foreach(var it in deptDic)  deptDictionary.Add(it.Key, it.Value);
                    }
                }
                if (args.SearchText == null)
                {
                    args.SearchText = "";
                }
                var user = await _uow.GetRepository<User>().FindByAsync<UserForTreeViewModel>("FullName", args.Page, args.Limit, (x => x.UserDepartmentMappings.Any(y => deptDictionary.Keys.Contains(y.DepartmentId.Value) && (isAll ? true : y.IsHeadCount)) && (x.SAPCode.Contains(args.SearchText) || x.FullName.ToLower().Contains(args.SearchText.ToLower()))));
                var count = await _uow.GetRepository<User>().CountAsync(x => (x.UserDepartmentMappings.Any(y => deptDictionary.Keys.Contains(y.DepartmentId.Value) && (isAll ? true : y.IsHeadCount))) && (x.SAPCode.Contains(args.SearchText) || x.FullName.ToLower().Contains(args.SearchText.ToLower())));
                return new ArrayResultDTO { Data = user, Count = count };
            }

            return new ArrayResultDTO { Data = null, Count = 0 };
        }

        public static T DeepCopy<T>(T item)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, item);
            stream.Seek(0, SeekOrigin.Begin);
            T result = (T)formatter.Deserialize(stream);
            stream.Close();
            return result;
        }

        public async Task<ResultDTO> Update(UserDataForCreatingArgs args)
        {
            var result = new ResultDTO();
            try
            {
                var existUser = await _uow.GetRepository<User>().GetSingleAsync(x => x.Id == args.Id, string.Empty);
                if (existUser == null)
                {
                    return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "User is not exist" } };
                }

                if (string.IsNullOrEmpty(args.LoginName))
                {
                    return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Login name is required !" } };
                }
                if (string.IsNullOrEmpty(args.Email))
                {
                    return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Email is required !" } };
                }
                var userBeforeUpdated = Mapper.Map<CommonViewModel.LogHistories.UserLogViewModel>(existUser); // luu log
                //Remove data in the membership database if user change the login name
                var requireToSendUpdateEmail = false;
                var generatedPassword = string.Empty;
                if (existUser.LoginName.ToLower().Trim() != args.LoginName.ToLower().Trim())
                {
                    //Remove permission from SP first
                    _sharePointBO.RemoveUser(existUser.LoginName);
                    if (args.Type == LoginType.Membership)
                    {
                        generatedPassword = Membership.GeneratePassword(8, NON_CHARACTER_PASSWORD_LENGTH);

                        var enumMemberships = Membership.FindUsersByName(args.LoginName).GetEnumerator();

                        var listMembers = new List<MembershipUser>();
                        while (enumMemberships.MoveNext())
                        {
                            listMembers.Add((MembershipUser)enumMemberships.Current);
                        }

                        if (!listMembers.Any(x => x.UserName.ToLower() == existUser.LoginName.ToLower()))
                        {
                            var existsUserMemberShip = Membership.GetUser(args.LoginName);
                            if (existsUserMemberShip != null)
                            {
                                var resetPwd = existsUserMemberShip.ResetPassword();
                                existsUserMemberShip.ChangePassword(resetPwd, generatedPassword);
                            } else
                            {
                                MembershipUser member = Membership.CreateUser(args.LoginName, generatedPassword);
                            }
                            requireToSendUpdateEmail = true;
                        }

                        //Send invitation email to user here for new password 
                    }
                    //Assign permission to SharepOint
                    _sharePointBO.AssignUser(args.LoginName);
                }
                var user = Mapper.Map<UserListViewModel>(args);
                //user.ProfilePicture = !string.IsNullOrEmpty(args.ProfilePicture) ? UtilitiesHelper.ConvertToByteArrayFromBase64(args.ProfilePicture) : null;
                Mapper.Map(user, existUser);
                existUser.LoginName = user.LoginName;
                existUser.SAPCode = user.SAPCode;
                existUser.FullName = user.FullName;
                existUser.Role = user.Role;
                existUser.Type = user.Type;
                existUser.Gender = user.Gender;
                existUser.Email = user.Email;
                existUser.IsActivated = user.IsActivated;
                existUser.Modified = DateTimeOffset.Now;
                existUser.CheckAuthorizationUSB = user.CheckAuthorizationUSB;
                existUser.IsTargetPlan = user.IsTargetPlan; // old
                existUser.IsNotTargetPlan = user.IsNotTargetPlan; // new

                // save tracking histories
                try {
                    existUser.HasTrackingLog = true;
                    await _trackingHistoryBO.SaveTrackingHistory(new TrackingHistoryArgs()
                    {
                        DataStr = JsonConvert.SerializeObject(userBeforeUpdated),
                        ItemId = userBeforeUpdated.Id,
                        ItemType = ItemTypeContants.User,
                        Type = TrackingHistoryTypeContants.Update,
                        ItemRefereceNumberOrCode = user.SAPCode,
                    });
                }
                catch (Exception e) {}

                await _uow.CommitAsync();
                _dashboardBO.ClearNode();
                if (requireToSendUpdateEmail)
                {
                    await SendPasswordNotification(EmailTemplateName.NewMSAccount, existUser, generatedPassword);
                }
            }
            catch (MembershipCreateUserException e)
            {
                result.ErrorCodes.Add((int)e.StatusCode);
                result.Messages.Add(e.StatusCode.ToString());
            }
            return result;
        }
        public async Task<ResultDTO> ChangeStatus(Guid userId, bool isActivated)
        {
            var existUser = await _uow.GetRepository<User>().GetSingleAsync(x => x.Id == userId);
            if (existUser == null)
            {
                return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "User is not exist" } };
            }
            else
            {
                var userBeforeUpdated = Mapper.Map<CommonViewModel.LogHistories.UserLogViewModel>(existUser);
                if (isActivated)
                {
                    var user = Membership.GetUser(existUser.LoginName);
                    if (user != null)
                    {
                        user.UnlockUser();
                        user.IsApproved = true;
                        Membership.UpdateUser(user);
                    }
                    else
                    {
                        var pwd = Membership.GeneratePassword(8, NON_CHARACTER_PASSWORD_LENGTH);
                        var member = Membership.CreateUser(existUser.LoginName, pwd);
                        await SendPasswordNotification(EmailTemplateName.NewMSAccount, existUser, pwd);
                    }
                }
                else
                {
                    //try
                    //{
                    //                   var user = Membership.GetUser(existUser.LoginName);
                    //                   if (user != null)
                    //                   {
                    //                       user.IsApproved = false;
                    //                       Membership.UpdateUser(user);
                    //                   }
                    //               }
                    //catch (Exception ex)
                    //{
                    //}
                    Func<string, bool> DeactiveUser = (loginName) =>
                    {
                        try
                        {
                            Thread.Sleep(100);
                            var user = Membership.GetUser(loginName);
                            if (user != null)
                            {
                                user.IsApproved = false;
                                Membership.UpdateUser(user);
                            }
                        }
                        catch (Exception ex)
                        {
                            return false;
                        }
                        return true;
                    };
                    Func<string, int, bool> TryDeactiveUser = (loginName, retryTime) =>
                    {
                        try
                        {
                            while (!DeactiveUser(loginName) && retryTime > 0)
                            {
                                retryTime = retryTime--;
                            }
                        }
                        catch (Exception ex)
                        {
                            return false;
                        }
                        return true;
                    };
                    TryDeactiveUser(existUser.LoginName, 20);
                    if (existUser.UserDepartmentMappings.Count > 0)
                    {
                        await RemoveUserFromDepartmentByUser(existUser.Id);
                    }
                }
                existUser.IsActivated = isActivated;

                // save tracking histories
                try
                {
                    existUser.HasTrackingLog = true;
                    await _trackingHistoryBO.SaveTrackingHistory(new TrackingHistoryArgs()
                    {
                        DataStr = JsonConvert.SerializeObject(userBeforeUpdated),
                        ItemId = userBeforeUpdated.Id,
                        ItemType = ItemTypeContants.User,
                        Type = TrackingHistoryTypeContants.Update,
                        ItemRefereceNumberOrCode = existUser.SAPCode,
                    });
                }
                catch (Exception e) { }
                _uow.Commit();
            }
            return new ResultDTO { };
        }

        public async Task<ResultDTO> LockUserMembership(Guid userId, bool isActivated, string lockType)
        {
            var existUser = await _uow.GetRepository<User>().GetSingleAsync(x => x.Id == userId);
            if (existUser == null)
            {
                return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "User is not exist" } };
            }
            else
            {
                if (isActivated)
                {
                    var user = Membership.GetUser(existUser.LoginName);
                    if (user != null)
                    {
                        switch (lockType)
                        {
                            case "lockUser":
                                user.IsApproved = false;  //0
                                Membership.UpdateUser(user);
                                break;
                            case "unLockUser":
                                user.IsApproved = true;  //1
                                Membership.UpdateUser(user);
                                break;
                        }
                    }
                }
                else
                {
                    existUser.IsActivated = isActivated;
                    _uow.Commit();
                }
            }
            return new ResultDTO { };
        }

        public async Task<ResultDTO> CheckLockUser(Guid userId)
        {
            bool result = true;
            var existUser = await _uow.GetRepository<User>().GetSingleAsync(x => x.Id == userId);
            if (existUser == null)
            {
                result = false;
            }
            else
            {
                var user = Membership.GetUser(existUser.LoginName);
                if (user != null)
                {
                    if (!user.IsApproved)
                        result = false; // Da lock
                    else
                        result = true; // Chua lock

                }
                else
                {
                    result = true;// không có tk trong membership mặc định nó là AD
                }
            }
            return new ResultDTO { CheckLock = result };
        }

        public async Task<ResultDTO> ChangePassword(ChangePasswordArgs args)
        {
            var existUser = await _uow.GetRepository<User>().GetSingleAsync(x => x.Id == args.Id);
            if (existUser == null)
            {
                return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "User is not exist" } };
            }
            try
            {
                var validatePasswordMinimumLength = await _uow.GetRepository<CompanyPolicy>().GetSingleAsync(x => x.Type == CompanyPolicyEnums.MINUMUM_LENGTH);
                if (validatePasswordMinimumLength != null)
                {
                    string newPassword = args.NewPassword;
                    if (newPassword.Length < validatePasswordMinimumLength.Value)
                    {
                        return new ResultDTO { ErrorCodes = { 1001 }, Messages = { string.Format("Password must be at least {0} characters long", validatePasswordMinimumLength.Value) } };
                    }

                    if (!System.Text.RegularExpressions.Regex.IsMatch(newPassword, "[a-z]"))
                    {
                        return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "CHANGE_PASSWORD_LOWERCASE_VALIDATE" } };
                    }

                    if (!System.Text.RegularExpressions.Regex.IsMatch(newPassword, "[A-Z]"))
                    {
                        return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "CHANGE_PASSWORD_UPPERCASE_VALIDATE" } };
                    }

                    if (!System.Text.RegularExpressions.Regex.IsMatch(newPassword, "\\d"))
                    {
                        return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "CHANGE_PASSWORD_NUMBER_VALIDATE" } };
                    }

                    if (!System.Text.RegularExpressions.Regex.IsMatch(newPassword, "[^a-zA-Z0-9]"))
                    {
                        return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "CHANGE_PASSWORD_SPECIAL_CHAR_VALIDATE" } };
                    }
                }
                

                // Attempt to find the user
                var validatePasswordNotMatch = await _uow.GetRepository<CompanyPolicy>().GetSingleAsync(x => x.Type == CompanyPolicyEnums.PASSWORD_CANNOT_MATCH);
                if (validatePasswordNotMatch != null)
                {
                    var findUserPasswordHistories = await _uow.GetRepository<UserPasswordHistories>().FindByAsync(x => x.UserId == existUser.Id, "created desc");
                    var takePasswordNotMatch = findUserPasswordHistories.ToList().OrderByDescending(x => x.Created).Take(validatePasswordNotMatch.Value).ToList();
                    foreach(var takeOneItem in takePasswordNotMatch)
                    {
                        var decrype = DecryptPassword(takeOneItem.PwdHistory);
                        if (decrype.Equals(args.NewPassword))
                        {
                            return new ResultDTO { ErrorCodes = { 1001 }, Messages = { string.Format("Password cannot be the same as the last {0} passwords", validatePasswordNotMatch.Value) } };
                        }
                    }
                }

                var user = Membership.GetUser(existUser.LoginName);
                var result = user.ChangePassword(args.OldPassword, args.NewPassword);
                if (result)
                {
                    await this.AddUserHistoriesPassword(existUser.Id, args.NewPassword);
                }
                return new ResultDTO { Object = result };
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task AddUserHistoriesPassword(Guid userId, string passwordParam)
        {
            var encryptPassword = EncryptPassword(passwordParam);
            var newUserPasswordHistories = new UserPasswordHistories()
            {
                UserId = userId,
                PwdHistory = encryptPassword
            };
            _uow.GetRepository<UserPasswordHistories>().Add(newUserPasswordHistories);
            await _uow.CommitAsync();
        }


        public string EncryptPassword(string passwordParam)
        {
            int keysize = 128;
            string initVector = "t!r@a#n$n%g^u&ye";
            byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(passwordParam);
            PasswordDeriveBytes password = new PasswordDeriveBytes(PASSWORDSALT, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] cipherTextBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            return Convert.ToBase64String(cipherTextBytes);
        }

        public string DecryptPassword(string passwordParam)
        {
            int keysize = 128;
            string initVector = "t!r@a#n$n%g^u&ye";
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] cipherTextBytes = Convert.FromBase64String(passwordParam);
            PasswordDeriveBytes password = new PasswordDeriveBytes(PASSWORDSALT, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];
            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
        }


        public async Task<ResultDTO> ForgotPassword(string username, string email)
        {
            var existUser = await _uow.GetRepository<User>().GetSingleAsync(x => x.LoginName == username && x.Email == email);
            if (existUser == null)
            {
                return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Tên đăng nhập hoặc email không chính xác" } };
            }
            else
            {
                #region AE-2260
                if (existUser.Type == LoginType.ActiveDirectory)
                {
                    return new ResultDTO { ErrorCodes = { -1 }, Messages = { "Tài khoản của bạn do IT Support quản lý. Vui lòng liên hệ IT Support để được hỗ trợ!" } };
                }
                /*
                 * Câu hình ở forgot password
                 * 
                 if (result.errorCodes[0] == -1) {
				    changeColorRequired("#bf0000");
				    document.getElementById('displayErrorMessage').innerHTML = result.messages[0];
			    } else {
				    changeColorRequired("#bf0000");
				    document.getElementById('displayErrorMessage').innerHTML = "Mã SAP hoặc thư điện tử của bạn không tồn tại/Your SAP code or email is not valid";
			    }
                 * 
                 */
                #endregion

                var pwd = string.Empty;
                // Attempt to find the user
                var user = Membership.GetUser(existUser.LoginName);
                // Check if the user actually was found
                if (user == null)
                {
                    pwd = Membership.GeneratePassword(8, NON_CHARACTER_PASSWORD_LENGTH);
                    var member = Membership.CreateUser(existUser.LoginName, pwd);
                }
                else
                {
                    user.UnlockUser();
                    // They exist, so attempt to reset their password
                    var resetPwd = user.ResetPassword();
                    pwd = Membership.GeneratePassword(8, NON_CHARACTER_PASSWORD_LENGTH);
                    // Change the user's password
                    user.ChangePassword(resetPwd, pwd);
                }
                await SendPasswordNotification(EmailTemplateName.ResetPassword, existUser, pwd);
            }
            return new ResultDTO { };
        }

        public async Task<ResultDTO> ResetPassword(Guid userId)
        {
            var existUser = await _uow.GetRepository<User>().GetSingleAsync(x => x.Id == userId);
            if (existUser == null)
            {
                return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "User is not exist" } };
            }
            else
            {
                var pwd = string.Empty;
                // Attempt to find the user
                var user = Membership.GetUser(existUser.LoginName);
                // Check if the user actually was found
                if (user == null)
                {
                    pwd = Membership.GeneratePassword(8, NON_CHARACTER_PASSWORD_LENGTH);
                    var member = Membership.CreateUser(existUser.LoginName, pwd);
                }
                else
                {
                    user.UnlockUser();
                    // They exist, so attempt to reset their password
                    var resetPwd = user.ResetPassword();
                    pwd = Membership.GeneratePassword(8, NON_CHARACTER_PASSWORD_LENGTH);
                    // Change the user's password
                    user.ChangePassword(resetPwd, pwd);
                }
                await SendPasswordNotification(EmailTemplateName.ResetPassword, existUser, pwd);
            }
            return new ResultDTO { };
        }


        public async Task<ResultDTO> GetUserById(UserInfoCABArg arg)
        {
            var result = await _employee.GetFullEmployeeInfo(arg.UserId);
            return result;
        }
        public async Task<ResultDTO> GetUsers()
        {
            var items = await _uow.GetRepository<User>().GetAllAsync<UserListViewModel>();
            return new ResultDTO { Object = new ArrayResultDTO { Data = items, Count = items.Count() } };
        }
        public async Task<ResultDTO> GetCurrentUser()
        {
            var loginName = string.Empty;
            if (_apiCtx == null || String.IsNullOrEmpty(_apiCtx.CurrentUser))
            {
                loginName = "SAdmin";
            }
            else
            {
                loginName = _apiCtx.CurrentUser;
            }

            /*var changeLogin = await _uow.GetRepository<ChangeUser>().GetSingleAsync(x => x.FromLoginName.Equals(loginName),"Created desc");
            if(changeLogin != null)
            {
                loginName = changeLogin.ToLoginName;
            }*/

            var user = await _uow.GetRepository<User>().GetSingleAsync<UserListViewModel>(x => x.LoginName == loginName && x.IsActivated && !x.IsDeleted);
            if (user != null)
            {
                var item = await GetUserById(new UserInfoCABArg() { UserId = user.Id });
                return item;
            }
            return new ResultDTO { Object = null, Messages = { loginName } };
        }

        public async Task<ResultDTO> GetCurrentUserV2()
        {
            var result = new ResultDTO() { };
            var loginName = string.Empty;
            if (_apiCtx == null || String.IsNullOrEmpty(_apiCtx.CurrentUser))
            {
                loginName = "SAdmin";
            }
            else
            {
                loginName = _apiCtx.CurrentUser;
            }
            var user = await _uow.GetRepository<User>().GetSingleAsync<UserListViewModel>(x => x.LoginName == loginName && x.IsActivated && !x.IsDeleted);
            if (user != null)
            {
                result.Object = user;
                return result;
            }
            return new ResultDTO { Object = null, Messages = { loginName } };
        }

        public async Task<ResultDTO> GetLinkImageUserByLoginName(string loginName)
        {
            var user = await _uow.GetRepository<User>().GetSingleAsync<UserListViewModel>(x => x.LoginName == loginName && x.IsActivated && !x.IsDeleted);
            if (user != null)
            {
                var profilePicture = user.ProfilePicture;
                if (!String.IsNullOrEmpty(profilePicture))
                {
                    string hrApiUrl = ConfigurationManager.AppSettings["HRApiUrl"];
                    return new ResultDTO { Object = hrApiUrl + profilePicture.Trim() };
                }
                else
                {
                    return new ResultDTO { Object = null, Messages = { loginName } };
                }
            }
            return new ResultDTO { Object = null, Messages = { loginName } };
        }

        public async Task<ResultDTO> UpdateImageUser(UserDataForCreatingArgs data)
        {
            var result = new ResultDTO();
            try
            {
                var existUser = await _uow.GetRepository<User>().GetSingleAsync(x => x.Id == data.Id);
                if (existUser == null)
                {
                    return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "User not exist !" } };
                }
                existUser.ProfilePictureId = data.ProfilePictureId;
                _uow.GetRepository<User>().Update(existUser);
                await _uow.CommitAsync();
                _dashboardBO.ClearNode();
                result.Object = Mapper.Map<UserListViewModel>(existUser);

            }
            catch (MembershipCreateUserException e)
            {
                result.ErrorCodes.Add((int)e.StatusCode);
                result.Messages.Add(e.StatusCode.ToString());
            }
            return result;
        }

        public async Task<ResultDTO> GetImageUserById(UserInfoCABArg data)
        {
            var items = await _uow.GetRepository<User>().GetSingleAsync<UserListViewModel>(x => x.Id == data.UserId);
            return new ResultDTO { Object = new ArrayResultDTO { Data = items } };
        }

        public async Task<ResultDTO> FindUserForDataInvalid(UserForCheckDataArg data)
        {
            var result = new ResultDTO();
            var messages = new List<string>();
            if (data.Id == Guid.Empty)
            {
                var userSAPCodes = await _uow.GetRepository<User>().FindByAsync<UserListViewModel>(x => x.SAPCode == data.SAPCode && x.IsActivated);
                var userLoginName = await _uow.GetRepository<User>().FindByAsync<UserListViewModel>(x => x.LoginName == data.LoginName && x.IsActivated);
                var userEmail = await _uow.GetRepository<User>().FindByAsync<UserListViewModel>(x => x.Email == data.Email && x.IsActivated);
                if (userSAPCodes.Any())
                {
                    messages.Add("SAP Code " + data.SAPCode + " is valid in User list");
                }
                if (userLoginName.Any())
                {
                    messages.Add("Login Name " + data.LoginName + " is valid in User list");
                }
                if (userEmail.Any())
                {
                    messages.Add("Email " + data.Email + " is valid in User list");
                }
                result.Object = new ArrayResultDTO { Data = messages, Count = messages.Count() };
            }
            else
            {
                var userSAPCodes = await _uow.GetRepository<User>().FindByAsync<UserListViewModel>(x => x.SAPCode == data.SAPCode && x.Id != data.Id && x.IsActivated);

                // var userSAPCodes = await _uow.GetRepository<User>().FindByAsync<UserListViewModel>()



                var userLoginName = await _uow.GetRepository<User>().FindByAsync<UserListViewModel>(x => x.LoginName == data.LoginName && x.Id != data.Id && x.IsActivated);
                var userEmail = await _uow.GetRepository<User>().FindByAsync<UserListViewModel>(x => x.Email == data.Email && x.Id != data.Id && x.IsActivated);
                if (userSAPCodes.Any())
                {
                    messages.Add("SAP Code " + data.SAPCode + " is valid in User list");
                }
                if (userLoginName.Any())
                {
                    messages.Add("Login Name " + data.LoginName + " is valid in User list");
                }
                if (userEmail.Any())
                {
                    messages.Add("Email " + data.Email + " is valid in User list");
                }
                result.Object = new ArrayResultDTO { Data = messages, Count = messages.Count() };
            }
            return result;
        }

        public async Task<ResultDTO> GetUserProfileDataById(UserInfoCABArg arg)
        {
            var result = await _employee.GetFullEmployeeInfo(arg.UserId);
            await _employee.GetUserProfileAdditionData((EmployeeViewModel)result.Object);
            return result;
        }
        public async Task<ResultDTO> GetUserProfileCustomById(UserInfoCABArg arg)
        {
            var result = new ResultDTO();
            var user = await _uow.GetRepository<User>().FindByIdAsync(arg.UserId);
            if (user != null)
            {
                result = await _employee.GetFullEmployeeInfo(arg.UserId, user.IsActivated);
                await _employee.GetUserProfileAdditionData((EmployeeViewModel)result.Object);
            }
            return result;
        }
        public async Task<ArrayResultDTO> GetUserCheckedHeadCount(Guid departmentId, string textSearch)
        {
            if (string.IsNullOrEmpty(textSearch))
            {
                textSearch = "";
            }
            var items = await _uow.GetRepository<User>().FindByAsync<UserForTreeViewModel>(x => x.UserDepartmentMappings.Any(y => y.UserId.HasValue && y.DepartmentId == departmentId && y.IsHeadCount && (!string.IsNullOrEmpty(y.User.FullName) && (y.User.FullName.Contains(textSearch) || y.User.SAPCode.Contains(textSearch)))));
            return new ArrayResultDTO { Data = items, Count = items.Count() };
        }
        public async Task<ResultDTO> CheckUserIsStore(string departmentCode)
        {
            var isStore = false;
            if (!string.IsNullOrEmpty(departmentCode))
            {
                var currentDepartment = await _uow.GetRepository<Department>().FindByAsync<DepartmentViewModel>(x => x.Code == departmentCode);
                isStore = currentDepartment != null && currentDepartment.Any() && currentDepartment.FirstOrDefault().IsStore;
            }
            return new ResultDTO { Object = isStore };
        }
        public async Task<ArrayResultDTO> GetUsersByListSAPs(string sapCodes)
        {
            ArrayResultDTO res = new ArrayResultDTO { };
            var sapCodeArray = sapCodes.Split(',');
            var users = await _uow.GetRepository<User>().FindByAsync<UserForTreeViewModel>(x => sapCodeArray.Contains(x.SAPCode));
            res.Data = users;
            return res;
        }

        public async Task<ResultDTO> CheckUserBySAPCode(string sapCode)
        {
            var result = new ResultDTO();
            var user = await _uow.GetRepository<User>().GetSingleAsync<UserForTreeViewModel>(x => x.SAPCode.Contains(sapCode.Trim()));
            result.Object = user;
            return result;
        }
        public async Task<ArrayResultDTO> GetAllUsers()
        {
            try
            {
                var items = await _uow.GetRepository<User>().FindByAsync<NavigationListUserDepartmentViewModel.User>(x => !x.IsDeleted && x.IsActivated);
                var count = await _uow.GetRepository<User>().CountAsync(x => !x.IsDeleted && x.IsActivated);
                var result = new ArrayResultDTO { Data = items, Count = count };
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
		}
        public async Task<ArrayResultDTO> GetAllUsersByKeyword(CommonArgs.Member.User.GetAllUserByKeyword args)
        {
            try
            {
                var items = await _uow.GetRepository<User>().FindByAsync<UserListViewModel>(args.Predicate, args.PredicateParameters);
                var count = await _uow.GetRepository<User>().CountAsync(args.Predicate, args.PredicateParameters);
                var result = new ArrayResultDTO { Data = items, Count = count };
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}