using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using Newtonsoft.Json;
using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.ViewModels.DTOs;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.Data.Models;
using Aeon.HR.ViewModels;
using Aeon.HR.Infrastructure.Constants;
using Aeon.HR.Infrastructure.Enums;
using DocumentFormat.OpenXml.VariantTypes;
using System.Data.Entity;
using DocumentFormat.OpenXml.ExtendedProperties;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Globalization;
using Aeon.HR.ViewModels.PrintFormViewModel;
using OfficeOpenXml;
using System.Text.RegularExpressions;
using System.Security;
using TargetPlanTesting.ImportData;

namespace Aeon.HR.BusinessObjects.Handlers
{
    public partial class SettingBO
    {
        public async Task<ResultDTO> GetReasons(MasterdataArgs args)
        {
            var result = new ResultDTO();

            // var masterdataTypes = ConfigurationManager.GetSection("MasterDataTypes") as NameValueCollection;
            // lấy hết tất cả value trong masterdataTypes
            // List<string> valuesOfMasterdataType = new List<string>();
            // valuesOfMasterdataType.AddRange(masterdataTypes.AllKeys.SelectMany(masterdataTypes.GetValues).Where(getValues => getValues != null));

            // var type = GetKeyAndValueMasterdataTypeInWebConfig(args.Type);
            //SharePoint need to be checked
            args.QueryArgs.AddPredicate("metadataType.value = @[index]", args.Type);
            var reasons = await _uow.GetRepository<MasterData>().FindByAsync<ReasonViewModelForCABSetting>(args.QueryArgs.Order, args.QueryArgs.Page, args.QueryArgs.Limit, args.QueryArgs.Predicate, args.QueryArgs.PredicateParameters);
            //var reasons = await _uow.GetRepository<MasterData>().GetAllAsync<ReasonViewModelForCABSetting>();
            var total = reasons.Count();
            if (total > 0)
            {
                total = await _uow.GetRepository<MasterData>().CountAsync(args.QueryArgs.Predicate, args.QueryArgs.PredicateParameters);
            }

            result.Object = new ArrayResultDTO { Data = reasons, Count = total };
            return result;
        }

        public async Task<ResultDTO> SearchReason(string searchString, string type)
        {
            var args = new QueryArgs
            {
                Page = 1,
                Limit = Int32.MaxValue,
                Predicate = string.Empty,
                PredicateParameters = new object[] { },
                Order = string.Empty
            };
            args.AddPredicate("(name.contains(@[index]) || code.contains(@[index]))", searchString);
            return await GetReasons(new MasterdataArgs
            {
                QueryArgs = args,
                Type = type
            });
        }

        public async Task<ResultDTO> AddReason(object args)
        {
            var result = new ResultDTO();
            var viewModel = JsonConvert.DeserializeObject<ReasonViewModelForAddOfCABSetting>(args.ToString());

            viewModel.TryValidateModel(result);

            if (result.Messages.Count == 0)
            {

                var type = await _uow.GetRepository<MetadataType>().GetSingleAsync(m => m.Value == viewModel.Type);
                if (type == null)
                {
                    result.Messages.Add("Type is not define");
                    result.ErrorCodes.Add(2);
                }
                else
                {
                    await CheckDuplicateCodeMasterData(Guid.Empty, viewModel.Code, type.Id, result.Messages, result.ErrorCodes);
                }

                if (result.Messages.Count == 0)
                {
                    var nRecord = Mapper.Map<MasterData>(viewModel);
                    nRecord.MetaDataTypeId = type.Id;
                    _uow.GetRepository<MasterData>().Add(nRecord);
                    await _uow.CommitAsync();
                }
            }
            return result;
        }

        public async Task<ResultDTO> UpdateReason(object args)
        {
            var result = new ResultDTO();
            var viewModel = JsonConvert.DeserializeObject<ReasonViewModelForUpdateOfCABSetting>(args.ToString());

            //validation model dựa vào atrribute trong type của viewmodel
            viewModel.TryValidateModel(result);

            if (result.Messages.Count == 0)
            {
                var recordInDb = await _uow.GetRepository<MasterData>().GetSingleAsync(x => x.Id == viewModel.Id);
                if (recordInDb == null)
                {
                    result.Messages.Add("Id in valid, Not found record");
                    result.ErrorCodes.Add(3);
                }
                else
                {
                    await CheckDuplicateCodeMasterData(recordInDb.Id, viewModel.Code, recordInDb.MetaDataTypeId.Value, result.Messages, result.ErrorCodes);
                }

                if (result.Messages.Count == 0)
                {
                    Mapper.Map(viewModel, recordInDb);
                    _uow.GetRepository<MasterData>().Update(recordInDb);
                    await _uow.CommitAsync();
                }
            }
            return result;
        }

        public async Task<ResultDTO> DeleteReason(object args)
        {
            var result = new ResultDTO();
            var viewModel = JsonConvert.DeserializeObject<ReasonViewModelForCABSetting>(args.ToString());
            //validation model dựa vào atrribute trong type của viewmodel
            viewModel.TryValidateModel(result);

            if (result.Messages.Count == 0)
            {
                var recordInDb = await _uow.GetRepository<MasterData>().GetSingleAsync(x => x.Id == viewModel.Id);
                if (recordInDb == null)
                {
                    result.Messages.Add("Id is in valid, Not found record");
                    result.ErrorCodes.Add(3);
                }
                if (result.Messages.Count == 0)
                {
                    _uow.GetRepository<MasterData>().Delete(recordInDb);
                    await _uow.CommitAsync();
                }
            }

            return result;
        }

        // Phải truyền thêm currentId đẻ tránh trường hợp đang update một masterdata cũ và masterdata đầu tiên tìm thấy là masterdata đang update
        async Task CheckDuplicateCodeMasterData(Guid currentId, string code, Guid typeId, List<string> errorMessages, List<int> errorCodes)
        {
            var masterDataInDb = await _uow.GetRepository<MasterData>().GetSingleAsync(m => m.MetaDataTypeId == typeId && m.Code == code && m.Id != currentId, "", x => x.MetadataType);
            if (masterDataInDb != null)
            {
                string nameScreen = string.Empty;
                switch (masterDataInDb.MetadataType.Value)
                {
                    case MetadataTypeConstants.MISSING_TIME_CLOCK_REASON_TYPE_CODE:
                        nameScreen = "Missing Timeclock";
                        break;
                    case MetadataTypeConstants.OVERTIME_REASON_TYPE_CODE:
                        nameScreen = "Overtime";
                        break;
                    case MetadataTypeConstants.RESIGNATION_REASON_TYPE_CODE:
                        nameScreen = "Resignation";
                        break;
                    case MetadataTypeConstants.SHIFT_EXCHANGE_REASON_TYPE_CODE:
                        nameScreen = "Shift Exchange";
                        break;
                    default:
                        break;
                }

                errorMessages.Add($"Code {code} has existed in {nameScreen} Reasons list");
                errorCodes.Add(1); // tạm thời lưu số 1 tượng trưng cho duplicate value
            }
        }

        public async Task<ResultDTO> GetAllReason(MetadataTypeArgs arg)
        {
            var type = await _uow.GetRepository<MetadataType>().GetSingleAsync(x => x.Value == arg.nameType);
            var reasons = await _uow.GetRepository<MasterData>().FindByAsync<MasterDataViewModel>(m => m.MetaDataTypeId == type.Id);
            return new ResultDTO { Object = new ArrayResultDTO { Data = reasons, Count = reasons.Count() } };
        }

        public async Task<ResultDTO> DeleteShiftPlanSubmitPerson(Guid departmentId)
        {
            var result = new ResultDTO();
            var shiftPlanSubmitPersons = await _uow.GetRepository<UserSubmitPersonDeparmentMapping>().FindByAsync(x => x.DepartmentId == departmentId);
            foreach (var item in shiftPlanSubmitPersons)
            {
                item.IsSubmitPerson = false;
                _uow.GetRepository<UserSubmitPersonDeparmentMapping>().Update(item);
            }
            await _uow.CommitAsync();
            return result;
        }

        public async Task<ResultDTO> GetShiftPlanSubmitPersonById(Guid departmentId)
        {
            var result = new ResultDTO();
            var shiftPlanSubmitPerson = new ShiftPlanSubmitPersonViewModel();
            var userDepartments = await _uow.GetRepository<UserSubmitPersonDeparmentMapping>().FindByAsync(x => x.DepartmentId == departmentId, "", y => y.Department, u => u.User);
            var userGroupByDepartment = userDepartments.GroupBy(x => x.DepartmentId).ToList();
            foreach (var department in userGroupByDepartment)
            {
                List<Guid> userIds = new List<Guid>();
                List<string> userNames = new List<string>();
                shiftPlanSubmitPerson.DepartmentId = department.Key;
                foreach (var user in department)
                {
                    shiftPlanSubmitPerson.DepartmentName = user.Department.Name;
                    userIds.Add(user.UserId);
                    userNames.Add(user.User.FullName);
                }
                shiftPlanSubmitPerson.UserIds = userIds;
                shiftPlanSubmitPerson.UserNames = userNames;
            }
            result.Object = new ArrayResultDTO { Data = shiftPlanSubmitPerson };
            return result;
        }

        public async Task<ResultDTO> GetShiftPlanSubmitPersons(QueryArgs args)
        {
            var result = new ResultDTO();
            var shiftPlanSubmitPersons = new List<ShiftPlanSubmitPersonViewModel>();
            var userDepartments = (await _uow.GetRepository<UserSubmitPersonDeparmentMapping>() .FindByAsync(args.Predicate, args.PredicateParameters, "", x => x.Department, y => y.User)).ToList();
            if (userDepartments.Any())
            {
                var departmentGroups = userDepartments.Select(x => new { x.DepartmentId, x.IsIncludeChildren }).Distinct().ToList();
                var allUserDepartments = _uow.GetRepository<UserSubmitPersonDeparmentMapping>().GetAll("", x => x.Department, y => y.User).ToList();
                userDepartments = allUserDepartments.Where(x => departmentGroups.Any(g =>
                        g.DepartmentId == x.DepartmentId &&
                        x.IsSubmitPerson==true &&
                        g.IsIncludeChildren == x.IsIncludeChildren)) .ToList();
            }

            var grouped = userDepartments.GroupBy(x => new { x.DepartmentId, x.IsIncludeChildren }).ToList();
            foreach (var department in grouped)
            {
                var shiftPlanSubmitPerson = new ShiftPlanSubmitPersonViewModel
                {
                    DepartmentId = department.Key.DepartmentId,
                    IsIncludeChildren = department.Key.IsIncludeChildren,
                    DepartmentName = department.FirstOrDefault()?.Department?.Name,
                    UserIds = new List<Guid>(),
                    UserNames = new List<string>(),
                    UserListViews = new List<UserListViewModel>()
                };

                foreach (var user in department)
                {
                    shiftPlanSubmitPerson.UserIds.Add(user.UserId);
                    shiftPlanSubmitPerson.UserNames.Add(user.User?.FullName);
                    shiftPlanSubmitPerson.UserListViews.Add(new UserListViewModel
                    {
                        Id = user.UserId,
                        SAPCode = user.User?.SAPCode,
                        FullName = user.User?.FullName
                    });
                }
                shiftPlanSubmitPersons.Add(shiftPlanSubmitPerson);
            }
            result.Object = new ArrayResultDTO
            {
                Data = shiftPlanSubmitPersons.Skip((args.Page - 1) * args.Limit).Take(args.Limit),
                Count = shiftPlanSubmitPersons.Count
            };
            return result;
        }




        public async Task<ResultDTO> CreateShiftPlanSubmitPerson(ShiftPlanSubmitPersonArg arg)
        {
            var result = new ResultDTO();
            var userDepartments = await _uow.GetRepository<UserSubmitPersonDeparmentMapping>().FindByAsync(x => x.DepartmentId == arg.DepartmentId);
            //update user trong department thanh false
            if (userDepartments.Any())
            {
                foreach (var item in userDepartments)
                {
                    item.IsSubmitPerson = false;
                    item.IsIncludeChildren = arg.IsIncludeChildren;
                    _uow.GetRepository<UserSubmitPersonDeparmentMapping>().Update(item);
                }
            }
            //cap nhap lai user trong department
            foreach (var user in arg.UserIds)
            {
                var userExist = userDepartments.SingleOrDefault(x => x.UserId == user);
                if (userExist == null)
                {
                    var model = new UserSubmitPersonDeparmentMapping
                    {
                        DepartmentId = arg.DepartmentId,
                        UserId = user,
                        IsIncludeChildren = arg.IsIncludeChildren,
                        IsSubmitPerson = true
                    };
                    _uow.GetRepository<UserSubmitPersonDeparmentMapping>().Add(model);
                }
                else
                {
                    userExist.IsSubmitPerson = true;
                }
            }
            await _uow.CommitAsync();
            return result;
        }

        public async Task<ResultDTO> SaveShiftPlanSubmitPerson(ShiftPlanSubmitPersonArg arg)
        {
            var result = new ResultDTO();
            var userDepartments = await _uow.GetRepository<UserSubmitPersonDeparmentMapping>().FindByAsync(x => x.DepartmentId == arg.DepartmentIdOld);
            //update user trong department thanh false
            if (userDepartments.Any())
            {
                foreach (var item in userDepartments)
                {
                    item.IsSubmitPerson = false;
                    item.IsIncludeChildren = arg.IsIncludeChildren;
                    item.DepartmentId = arg.DepartmentId;
                    _uow.GetRepository<UserSubmitPersonDeparmentMapping>().Update(item);
                }
            }
            //cap nhap lai user trong department
            foreach (var user in arg.UserIds)
            {
                var userExist = userDepartments.FirstOrDefault(x => x.UserId == user);
                if (userExist == null)
                {
                    var model = new UserSubmitPersonDeparmentMapping
                    {
                        DepartmentId = arg.DepartmentId,
                        UserId = user,
                        IsIncludeChildren = arg.IsIncludeChildren,
                        IsSubmitPerson = true
                    };
                    _uow.GetRepository<UserSubmitPersonDeparmentMapping>().Add(model);
                }
                else
                {
                    userExist.IsSubmitPerson = true;
                }
            }
            await _uow.CommitAsync();
            return result;
        }
        public async Task<ResultDTO> CheckIsSubmitPersonOfDepartment(Guid divisionId, string currentUserSapCode)
        {
            var hasSubmit = await FindDepartmentHasSubmitPersonFromDepartmentId(divisionId);
            if (hasSubmit != null && hasSubmit.SubmitSAPCodes.Contains(currentUserSapCode))
            {
                return new ResultDTO { Object = new ArrayResultDTO { Data = hasSubmit } };
            }
            return new ResultDTO { ErrorCodes = { 204 }, Messages = { "Not found user submit for department" }, };
        }
        public async Task<ResultDTO> CheckDepartmentExist(Guid departmentId)
        {
            var result = new ResultDTO();
            var departments = await _uow.GetRepository<UserSubmitPersonDeparmentMapping>().FindByAsync(x => x.DepartmentId == departmentId && x.IsSubmitPerson);
            result.Object = new ArrayResultDTO { Count = departments.Count() };
            return result;
        }

        public async Task<ResultDTO> GetDepartmentTargetPlansByUserId(Guid Id)
        {
            var userDepartmentMappings = new List<UserDepartmentMapping>();
            var currentUser = await _uow.GetRepository<User>().FindByIdAsync<SimpleUserDTO>(_uow.UserContext.CurrentUserId);
            var existings = await _uow.GetRepository<UserDepartmentMapping>().FindByAsync(x => x.UserId == Id && (x.IsHeadCount));
            userDepartmentMappings = existings.ToList();
            var userExistSubmit = await _uow.GetRepository<UserSubmitPersonDeparmentMapping>().FindByAsync(x => x.UserId == Id);
            foreach (var item in userExistSubmit)
            {
                var results = await _uow.GetRepository<UserSubmitPersonDeparmentMapping>().FindByAsync(x => x.UserId == Id && x.DepartmentId == item.DepartmentId);
                foreach (var x in results)
                {
                    var value = new UserDepartmentMapping
                    {
                        Id = x.Id,
                        DepartmentId = x.DepartmentId,
                        IsHeadCount = false,
                        UserId = x.UserId,
                        Department = x.Department
                    };
                    userDepartmentMappings.Add(value);
                }
            }
            existings = userDepartmentMappings;
            var userInAllDepartments = new List<UserInAllDepartmentViewModel>();
            if (existings != null && existings.Any())
            {
                foreach (var item in existings)
                {
                    if (!userInAllDepartments.Any(x => x.DeptLine != null && x.DeptLine.Id == item.DepartmentId))
                    {
                        var userInAllDepartment = new UserInAllDepartmentViewModel();
                        if (item.Department.Type == DepartmentType.Department)
                        {
                            userInAllDepartment.DeptLine = Mapper.Map<DepartmentViewModel>(item.Department);
                            userInAllDepartments.Add(userInAllDepartment);
                        }
                        else
                        {
                            var division = Mapper.Map<DepartmentViewModel>(item.Department);
                            var skip = false;
                            var departmentIdx = item.Department.ParentId;
                            while (!skip)
                            {
                                if (departmentIdx.HasValue)
                                {
                                    var dept = await _uow.GetRepository<Department>().GetSingleAsync<DepartmentViewModel>(x => x.Id == departmentIdx);
                                    if (dept != null)
                                    {
                                        if (dept.Type == DepartmentType.Department)
                                        {
                                            skip = true;
                                            var deptItem = userInAllDepartments.FirstOrDefault(x => x.DeptLine.Id == dept.Id);
                                            if (deptItem == null)
                                            {
                                                userInAllDepartment.DeptLine = dept;
                                                userInAllDepartment.Divisions.Add(division);
                                                userInAllDepartments.Add(userInAllDepartment);
                                            }
                                            else
                                            {
                                                deptItem.Divisions.Add(division);
                                            }
                                        }
                                        else
                                        {
                                            departmentIdx = dept.ParentId;
                                        }
                                    }
                                    else
                                    {

                                        skip = true;
                                    }
                                }
                                else
                                {
                                    skip = true;
                                }
                            }
                        }

                    }
                }
                if (userInAllDepartments.Any())
                {
                    foreach (var item in userInAllDepartments)
                    {
                        if (item.Divisions.Any())
                        {
                            await GetDepartmentHasSubmitPersonFromDivision(item);
                            await CheckValidDivision(item);
                        }
                    }
                }
                return new ResultDTO { Object = new ArrayResultDTO { Data = userInAllDepartments, Count = userInAllDepartments.Count() } };

            }
            else
            {
                return new ResultDTO { ErrorCodes = { 404 }, Messages = { "Not found department with id " + Id }, };

            }
        }
        private async Task GetDepartmentHasSubmitPersonFromDivision(UserInAllDepartmentViewModel item)
        {
            var divisons = new List<DepartmentViewModel>();
            var currentDivisions = item.Divisions;
            foreach (var division in currentDivisions)
            {
                var data = await FindDepartmentHasSubmitPersonFromDepartmentId(division.Id);
                if (data != null)
                {
                    var currentDivison = await _uow.GetRepository<Department>().GetSingleAsync<DepartmentViewModel>(x => x.Id == data.Id);
                    divisons.Add(currentDivison);
                }
                else if (division.UserCheckedHeadCountSAPId == _uow.UserContext.CurrentUserId)
                {
                    divisons.Add(division);
                }
            }
            if (divisons.Count > 0)
            {
                item.Divisions = divisons;
            }
        }

        private Task<List<Guid>> DepartmentExistsUserSubmit(string currentUserSapCode, IEnumerable<string> submitSapCodes, List<DepartmentTreeViewModel> treeModel)
        {
            // RULE: DUYỆT XUỐNG
            var excludeDivisonIds = new List<Guid>();
            if (treeModel.Any())
            {
                foreach (var item in treeModel)
                {
                    // Exclude những divison có người submit khác
                    var excludeParents = DescendantsAndSelf(item).Where(i => i.UserCheckedHeadCountSapCode != currentUserSapCode && i.SubmitSAPCodes.Any() && !submitSapCodes.Any(y => i.SubmitSAPCodes.Contains(y)));

                    if (excludeParents.Any())
                    {
                        excludeDivisonIds.AddRange(excludeParents.Select(s => s.Id));

                        // Exclude những division con có người submit khác
                        foreach (var child in excludeParents)
                        {
                            if (child.Items.Any())
                            {
                                var excludeChilds = DescendantsAndSelf(child).Where(i => i.UserCheckedHeadCountSapCode != currentUserSapCode && i.SubmitSAPCodes.Any() && !submitSapCodes.Any(y => i.SubmitSAPCodes.Contains(y)));
                                excludeDivisonIds.AddRange(excludeChilds.Select(s => s.Id));
                            }
                        }
                    }
                }
            }
            return Task.FromResult(excludeDivisonIds);
        }
        private async Task<List<Guid>> DepartmentNotExistsUserSubmit(string currentUserSapCode, List<DepartmentTreeViewModel> treeModel)
        {
            //RULE: DUYỆT LÊN
            var excludeDivisonIds = new List<Guid>();
            foreach (var item in treeModel)
            {
                var departmentHasSubmitPerson = await FindDepartmentHasSubmitPersonFromDepartmentId(item.Id);
                if (departmentHasSubmitPerson != null)
                {
                    excludeDivisonIds = await DepartmentExistsUserSubmit(currentUserSapCode, departmentHasSubmitPerson.SubmitSAPCodes, treeModel);
                }
            }
            return excludeDivisonIds;
        }
        public async Task<ResultDTO> FilterDivisionByUserNotSubmit(DepartmentTargetPlanViewModel model)
        {
            var excludeDivisonIds = new List<Guid>();
            var getDepartmentTree = await GetDepartmentByArg(model.Arg);
            if (getDepartmentTree.IsSuccess)
            {
                //ResetDepartments();
                var departmentSubmitByCurrentUser = await _uow.GetRepository<UserSubmitPersonDeparmentMapping>().FindByAsync<UserSubmitPersonDepartmentMappingViewModel>(x => x.UserId == _uow.UserContext.CurrentUserId && x.IsSubmitPerson);
                var mapper = Mapper.Map<List<DepartmentTreeViewModel>>(getDepartmentTree.Object.GetPropValue("Data"));

                if (mapper.Any())
                {
                    var dept = mapper.FirstOrDefault(i => i.SubmitSAPCodes.Any());
                    if (dept != null) // Đã có người submit
                    {
                        excludeDivisonIds = await DepartmentExistsUserSubmit(model.CurrentUserSapCode, dept.SubmitSAPCodes, mapper);
                    }
                    else // chưa có người submit
                    {
                        excludeDivisonIds = await DepartmentNotExistsUserSubmit(model.CurrentUserSapCode, mapper);
                    }
                    excludeDivisonIds = excludeDivisonIds.ToList();
                    return new ResultDTO { Object = new ArrayResultDTO { Data = excludeDivisonIds, Count = excludeDivisonIds.Count } };
                }

            }
            return new ResultDTO { Object = new ArrayResultDTO { Data = null, Count = 0 } };
        }
        public async Task<ResultDTO> CheckSubmitPersonFromDepartmentId(Guid departmentId)
        {
            bool isSuccess = false;
            if (departmentId != null)
            {
                Department hasSubmitPersonDepartment = null;
                var dept = await _uow.GetRepository<Department>().FindByIdAsync(departmentId);
                if (dept != null)
                {
                    hasSubmitPersonDepartment = dept.UserSubmitPersonDeparmentMappings.Any(i => i.IsSubmitPerson) ? dept : null;
                    if (hasSubmitPersonDepartment == null)
                    {
                        var isPause = false;
                        while (!isPause)
                        {
                            var parentDeptId = dept.ParentId;
                            if (parentDeptId.HasValue)
                            {
                                var parentDept = await _uow.GetRepository<Department>().FindByIdAsync((Guid)parentDeptId);
                                hasSubmitPersonDepartment = parentDept.UserSubmitPersonDeparmentMappings.Any(i => i.IsSubmitPerson) ? parentDept : null;
                                if (hasSubmitPersonDepartment == null)
                                {
                                    dept = parentDept;
                                }
                                else
                                {
                                    isSuccess = true;
                                    isPause = true;
                                }
                            }
                            else
                            {
                                isPause = true;

                            }
                        }
                    }
                    else
                    {
                        isSuccess = true;
                    }
                }
                if (!isSuccess)
                    return new ResultDTO { ErrorCodes = { 204 }, Messages = { "Not found user submit for department" }, };
                return new ResultDTO { Object = new ArrayResultDTO { Data = isSuccess } };
            }
            return new ResultDTO { ErrorCodes = { 404 }, Messages = { "Not found department" }, };
        }
        public async Task<DepartmentTreeViewModel> FindDepartmentHasSubmitPersonFromDepartmentId(Guid departmentId)
        {
            DepartmentTreeViewModel result = null;
            if (departmentId != null)
            {
                Department hasSubmitPersonDepartment = null;
                var dept = await _uow.GetRepository<Department>().FindByIdAsync(departmentId);
                if (dept != null)
                {
                    hasSubmitPersonDepartment = dept.UserSubmitPersonDeparmentMappings.Any(i => i.IsSubmitPerson) ? dept : null;
                    if (hasSubmitPersonDepartment == null)
                    {
                        var isPause = false;
                        while (!isPause)
                        {
                            var parentDeptId = dept.ParentId;
                            if (parentDeptId.HasValue)
                            {
                                var parentDept = await _uow.GetRepository<Department>().FindByIdAsync((Guid)parentDeptId);
                                hasSubmitPersonDepartment = parentDept.UserSubmitPersonDeparmentMappings.Any(i => i.IsSubmitPerson) ? parentDept : null;
                                if (hasSubmitPersonDepartment == null)
                                {
                                    dept = parentDept;
                                }
                                else
                                {
                                    isPause = true;
                                }
                            }
                            else
                            {
                                isPause = true;

                            }
                        }
                    }
                }
                if (hasSubmitPersonDepartment != null)
                {
                    result = Mapper.Map<DepartmentTreeViewModel>(hasSubmitPersonDepartment);
                }
            }

            return result;
        }
        private async Task GetParentDepartment(UserInAllDepartmentViewModel item)
        {
            var divisons = new List<DepartmentViewModel>(item.Divisions);
            var currentDivisions = item.Divisions;
            var grade = currentDivisions.OrderByDescending(o => o.JobGradeGrade).FirstOrDefault()?.JobGradeGrade;
            foreach (var division in currentDivisions)
            {
                if (currentDivisions.Any(x => x.Id == division.ParentId))
                {
                    divisons.Remove(division);
                }
                else
                {
                    var isPause = false;
                    var depTmp = division;
                    while (!isPause)
                    {
                        var parentDeptId = depTmp.ParentId;
                        if (depTmp.JobGradeGrade == grade || !parentDeptId.HasValue)
                        {
                            // nếu check đến grade lớn nhất trong currentDivisions
                            isPause = true;
                        }
                        else
                        {
                            var parentDept = await _uow.GetRepository<Department>().FindByIdAsync<DepartmentViewModel>((Guid)parentDeptId);
                            if (currentDivisions.Any(x => x.Id == parentDept.ParentId))
                            {
                                isPause = true;
                                divisons.Remove(division);
                            }
                            else
                            {
                                depTmp = parentDept;
                            }
                        }
                    }
                }
            }
            item.Divisions = divisons;
        }
        //public async Task<ResultDTO> GetDepartmentTargetPlans(Guid Id)
        //{
        //    var userDepartmentMappings = new List<UserDepartmentMapping>();
        //    var existings = await _uow.GetRepository<UserDepartmentMapping>().FindByAsync(x => x.UserId == Id && (x.IsHeadCount));
        //    userDepartmentMappings = existings.ToList();
        //    var userExistSubmit = await _uow.GetRepository<UserSubmitPersonDeparmentMapping>().FindByAsync(x => x.UserId == Id);
        //    foreach (var item in userExistSubmit)
        //    {
        //        var results = await _uow.GetRepository<UserSubmitPersonDeparmentMapping>().FindByAsync(x => x.UserId == Id && x.DepartmentId == item.DepartmentId && x.IsSubmitPerson);
        //        foreach (var x in results)
        //        {
        //            var value = new UserDepartmentMapping
        //            {
        //                Id = x.Id,
        //                DepartmentId = x.DepartmentId,
        //                IsHeadCount = false,
        //                UserId = x.UserId,
        //                Department = x.Department
        //            };
        //            userDepartmentMappings.Add(value);
        //        }
        //    }
        //    existings = userDepartmentMappings;
        //    var userInAllDepartments = new List<UserInAllDepartmentViewModel>();
        //    if (existings != null && existings.Any())
        //    {
        //        var departments = await _uow.GetRepository<Department>().GetAllAsync<DepartmentViewModel>();
        //        foreach (var item in existings)
        //        {
        //            if (!userInAllDepartments.Any(x => x.DeptLine != null && x.DeptLine.Id == item.DepartmentId))
        //            {
        //                var userInAllDepartment = new UserInAllDepartmentViewModel();
        //                if (!item.IsHeadCount)
        //                {
        //                    userInAllDepartment.Divisions = departments.Where(i => i.ParentId == item.Department.Id).ToList();
        //                }
        //                if (item.Department.Type == DepartmentType.Department)
        //                {
        //                    userInAllDepartment.DeptLine = Mapper.Map<DepartmentViewModel>(item.Department);
        //                    userInAllDepartments.Add(userInAllDepartment);
        //                }
        //                else
        //                {
        //                    var skip = false;
        //                    var departmentIdx = item.Department.ParentId;
        //                    var division = Mapper.Map<DepartmentViewModel>(item.Department);
        //                    while (!skip)
        //                    {
        //                        if (departmentIdx.HasValue)
        //                        {
        //                            var dept = departments.FirstOrDefault(x => x.Id == departmentIdx);
        //                            if (dept != null)
        //                            {
        //                                if (dept.Type == DepartmentType.Department)
        //                                {
        //                                    skip = true;
        //                                    var deptItem = userInAllDepartments.FirstOrDefault(x => x.DeptLine.Id == dept.Id);
        //                                    if (deptItem == null)
        //                                    {
        //                                        userInAllDepartment.DeptLine = dept;
        //                                        userInAllDepartment.Divisions.Add(division);
        //                                        userInAllDepartments.Add(userInAllDepartment);
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    departmentIdx = dept.ParentId;
        //                                }
        //                            }
        //                            else
        //                            {

        //                                skip = true;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            skip = true;
        //                        }
        //                    }
        //                }

        //            }
        //            if (userInAllDepartments.Any())
        //            {
        //                foreach (var _item in userInAllDepartments)
        //                {
        //                    if (_item.Divisions.Any())
        //                    {
        //                        await CheckValidDivision(_item);
        //                    }
        //                }
        //            }
        //        }
        //        return new ResultDTO { Object = new ArrayResultDTO { Data = userInAllDepartments, Count = userInAllDepartments.Count() } };

        //    }
        //    else
        //    {
        //        return new ResultDTO { ErrorCodes = { 404 }, Messages = { "Not found department with id " + Id }, };

        //    }
        //}

        public async Task<ResultDTO> GetDepartmentTargetPlans(Guid Id)
        {
            var userDepartmentMappings = new List<UserDepartmentMapping>();
            var existings = await _uow.GetRepository<UserDepartmentMapping>().FindByAsync(x => x.UserId == Id && (x.IsHeadCount));
            userDepartmentMappings = existings.ToList();
            var userExistSubmit = await _uow.GetRepository<UserSubmitPersonDeparmentMapping>().FindByAsync(x => x.UserId == Id && x.IsSubmitPerson);
            foreach (var item in userExistSubmit)
            {
                var results = await _uow.GetRepository<UserSubmitPersonDeparmentMapping>().FindByAsync(x => x.UserId == Id && x.DepartmentId == item.DepartmentId && x.IsSubmitPerson);
                foreach (var x in results)
                {
                    var value = new UserDepartmentMapping
                    {
                        Id = x.Id,
                        DepartmentId = x.DepartmentId,
                        IsHeadCount = false,
                        UserId = x.UserId,
                        Department = x.Department
                    };
                    userDepartmentMappings.Add(value);
                }
            }
            existings = userDepartmentMappings;
            var userInAllDepartments = new List<UserInAllDepartmentViewModel>();
            if (existings != null && existings.Any())
            {
                foreach (var item in existings)
                {
                    if (!userInAllDepartments.Any(x => x.DeptLine != null && x.DeptLine.Id == item.DepartmentId))
                    {
                        var userInAllDepartment = new UserInAllDepartmentViewModel();
                        if (item.Department.Type == DepartmentType.Department)
                        {
                            userInAllDepartment.DeptLine = Mapper.Map<DepartmentViewModel>(item.Department);
                            userInAllDepartments.Add(userInAllDepartment);
                        }
                        else
                        {
                            var division = Mapper.Map<DepartmentViewModel>(item.Department);
                            var skip = false;
                            var departmentIdx = item.Department.ParentId;

                            while (!skip)
                            {
                                if (departmentIdx.HasValue)
                                {
                                    var dept = await _uow.GetRepository<Department>().GetSingleAsync<DepartmentViewModel>(x => x.Id == departmentIdx);
                                    if (dept != null)
                                    {
                                        if (dept.Type == DepartmentType.Department)
                                        {
                                            skip = true;
                                            var deptItem = userInAllDepartments.FirstOrDefault(x => x.DeptLine.Id == dept.Id);
                                            if (deptItem == null)
                                            {
                                                userInAllDepartment.DeptLine = dept;
                                                userInAllDepartment.Divisions.Add(division);
                                                userInAllDepartments.Add(userInAllDepartment);
                                            }
                                            else
                                            {
                                                if (!deptItem.Divisions.Any(x => x.Id == division.Id))
                                                {
                                                    deptItem.Divisions.Add(division);
                                                }

                                            }
                                        }
                                        else
                                        {
                                            departmentIdx = dept.ParentId;
                                        }
                                    }
                                    else
                                    {

                                        skip = true;
                                    }
                                }
                                else
                                {
                                    skip = true;
                                }
                            }
                        }

                    }
                }
                if (userInAllDepartments.Any())
                {
                    foreach (var item in userInAllDepartments)
                    {
                        if (item.Divisions.Any())
                        {
                            //await GetDepartmentHasSubmitPersonFromDivision(item);
                            await GetParentDepartment(item);
                        }
                    }
                }
                return new ResultDTO { Object = new ArrayResultDTO { Data = userInAllDepartments, Count = userInAllDepartments.Count() } };

            }
            else
            {
                return new ResultDTO { ErrorCodes = { 404 }, Messages = { "Not found department with id " + Id }, };

            }
        }

        #region CABSetting - Holiday Schedule
        public async Task<ResultDTO> GetHolidaySchedules(QueryArgs arg)
        {
            var holidaySchedules = await _uow.GetRepository<HolidaySchedule>().FindByAsync<HolidayScheduleViewModel>(arg.Order, arg.Page, arg.Limit, arg.Predicate, arg.PredicateParameters);
            var totalList = await _uow.GetRepository<HolidaySchedule>().CountAsync(arg.Predicate, arg.PredicateParameters);
            return new ResultDTO { Object = new ArrayResultDTO { Data = holidaySchedules, Count = totalList } };
        }
        public async Task<ResultDTO> CreateHolidaySchedule(HolidayScheduleArgs data)
        {
            var holidaySchedules = Mapper.Map<HolidaySchedule>(data);
            _uow.GetRepository<HolidaySchedule>().Add(holidaySchedules);
            await _uow.CommitAsync();
            return new ResultDTO { Object = data };
        }
        public async Task<ResultDTO> UpdateHolidaySchedule(HolidayScheduleArgs data)
        {
            var holidaySchedules = await _uow.GetRepository<HolidaySchedule>().FindByAsync(x => x.Id == data.Id);
            if (holidaySchedules == null)
            {
                return new ResultDTO { Object = data };
            }
            else
            {
                holidaySchedules.FirstOrDefault().FromDate = data.FromDate;
                holidaySchedules.FirstOrDefault().ToDate = data.ToDate;
                holidaySchedules.FirstOrDefault().Title = data.Title;
                _uow.GetRepository<HolidaySchedule>().Update(holidaySchedules);
                await _uow.CommitAsync();
            }
            return new ResultDTO { Object = data };
        }
        public async Task<ResultDTO> DeleteHolidaySchedule(HolidayScheduleArgs data)
        {
            var holidaySchedules = await _uow.GetRepository<HolidaySchedule>().FindByAsync(x => x.Id == data.Id);
            if (holidaySchedules == null)
            {
                return new ResultDTO { Object = data };
            }
            else
            {
                _uow.GetRepository<HolidaySchedule>().Delete(holidaySchedules);
                await _uow.CommitAsync();
            }
            return new ResultDTO { Object = data };
        }
        public async Task<ResultDTO> GetYearHolidays()
        {
            var holidaySchedules = await _uow.GetRepository<HolidaySchedule>().GetAllAsync<HolidayScheduleViewModel>();
            var dateTimeList = new List<DateTimeOffset>();
            foreach (var item in holidaySchedules)
            {
                dateTimeList.Add(item.FromDate);
                dateTimeList.Add(item.ToDate);
            }
            var uniqueYears = dateTimeList.Select(s => new { code = s.Year }).Distinct();
            return new ResultDTO { Object = new ArrayResultDTO { Data = uniqueYears } };
        }
        #endregion
        #region CABSetting - ShiftCode
        public async Task<ResultDTO> GetDataShiftCode(QueryArgs arg)
        {
            var shiftcodes = await _uow.GetRepository<ShiftCode>().FindByAsync<ShiftCodeViewModel>(arg.Order, arg.Page, arg.Limit, arg.Predicate, arg.PredicateParameters);
            var totalList = await _uow.GetRepository<ShiftCode>().CountAsync(arg.Predicate, arg.PredicateParameters);
            return new ResultDTO { Object = new ArrayResultDTO { Data = shiftcodes, Count = totalList } };
        }

        public async Task<ResultDTO> CreateShiftCode(ShiftCodeAgrs data)
        {
            var existShiftCode = await _uow.GetRepository<ShiftCode>().FindByAsync(item => item.Code == data.Code);
            if (existShiftCode.Any())
            {
                return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "This item exists !" } };
            }
            data.Id = Guid.NewGuid();
            var shiftCode = Mapper.Map<ShiftCode>(data);
            _uow.GetRepository<ShiftCode>().Add(shiftCode);
            await _uow.CommitAsync();
            try
            {
                await _trackingHistoryBO.SaveTrackingHistory(new TrackingHistoryArgs()
                {
                    DataStr = JsonConvert.SerializeObject(Mapper.Map<ShiftCodeViewModel>(shiftCode)),
                    ItemId = shiftCode.Id,
                    ItemType = ItemTypeContants.ShiftCode,
                    Type = TrackingHistoryTypeContants.Create,
                    ItemRefereceNumberOrCode = shiftCode.Code,
                });
                await _uow.CommitAsync();
            }
            catch (Exception e)
            {
                _logger.LogError("Error: " + e.Message);
            }
            return new ResultDTO { Object = data };
        }
        public async Task<ResultDTO> UpdateShiftCode(ShiftCodeAgrs data)
        {
            var shiftCode = await _uow.GetRepository<ShiftCode>().GetSingleAsync(item => item.Id == data.Id);
            var existUser = await _uow.GetRepository<ShiftCode>().FindByAsync(item => item.Code == data.Code && item.Id != data.Id);
            if (existUser.Any())
            {
                return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Data shift code existed!" } };
            }
            var history = Mapper.Map<ShiftCodeViewModel>(shiftCode);
            shiftCode.Code = data.Code;
            shiftCode.Type = data.Type;
            shiftCode.TypeOfHaftDay = data.TypeOfHaftDay;
            shiftCode.Line = (ShiftLine)data.Line;
            shiftCode.IsHoliday = data.IsHoliday;
            shiftCode.IsActive = data.IsActive;
            _uow.GetRepository<ShiftCode>().Update(shiftCode);
            try
            {
                await _trackingHistoryBO.SaveTrackingHistory(new TrackingHistoryArgs()
                {
                    DataStr = JsonConvert.SerializeObject(history),
                    ItemId = shiftCode.Id,
                    ItemType = ItemTypeContants.ShiftCode,
                    Type = TrackingHistoryTypeContants.Update,
                    ItemRefereceNumberOrCode = shiftCode.Code,
                });
                await _uow.CommitAsync();
            }
            catch (Exception e)
            {
                _logger.LogError("Error: " + e.Message);
            }
            return new ResultDTO { Object = data };
        }
        public async Task<ResultDTO> DeleteShiftCode(ShiftCodeAgrs data)
        {
            var shiftCode = await _uow.GetRepository<ShiftCode>().FindByAsync(x => x.Id == data.Id);
            if (shiftCode == null)
            {
                return new ResultDTO { Object = data };
            }
            else
            {
                shiftCode.FirstOrDefault().IsDeleted = true;
                _uow.GetRepository<ShiftCode>().Delete(shiftCode);
                await _uow.CommitAsync();
                try
                {
                    await _trackingHistoryBO.SaveTrackingHistory(new TrackingHistoryArgs()
                    {
                        DataStr = JsonConvert.SerializeObject(Mapper.Map<ShiftCodeViewModel>(shiftCode)),
                        ItemId = shiftCode.FirstOrDefault().Id,
                        ItemType = ItemTypeContants.ShiftCode,
                        Type = TrackingHistoryTypeContants.Delete,
                        ItemRefereceNumberOrCode = shiftCode.FirstOrDefault().Code,

                    });
                    await _uow.CommitAsync();
                }
                catch (Exception e)
                {
                    _logger.LogError("Error: " + e.Message);
                }
            }
            return new ResultDTO { Object = data };
        }

        #region Download Template
        public async Task<byte[]> DownloadTemplate()
        {
            var dataToPrint = new ShiftCodeViewModel();
            var items = new List<ShiftCodePrintFormViewModel>();

            byte[] result = null;
            if (dataToPrint != null)
            {
                try
                {
                    var pros = new Dictionary<string, string>();
                    result = ExportXLS("ShiftcodeTemplate.xlsx", pros, items);
                }
                catch (Exception ex)
                {
                }
            }
            return result;
        }

        public byte[] ExportXLS(string template, Dictionary<string, string> pros, List<ShiftCodePrintFormViewModel> tbTPDetail)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory; // You get main rott
            var filePath = Path.Combine(path, "PrintDocument", template);
            var memoryStream = new MemoryStream();
            using (var stream = System.IO.File.OpenRead(filePath))
            {
                using (var pck = new ExcelPackage(stream))
                {
                    ExcelWorkbook WorkBook = pck.Workbook;
                    ExcelWorksheet worksheet = WorkBook.Worksheets.First();

                    InsertShiftCodeData(worksheet, 6, tbTPDetail);
                    var regex = new Regex(@"\[\[[\d\w\s]*\]\]", RegexOptions.IgnoreCase);
                    var tokens = worksheet.Cells.Where(x => x.Value != null && regex.Match(x.Value.ToString()).Success);
                    foreach (var token in tokens)
                    {
                        var fieldToken = token.Value.ToString().Trim(new char[] { '[', ']' });
                        if (pros.ContainsKey(fieldToken))
                        {
                            token.Value = pros[fieldToken];
                        }
                    }
                    worksheet.Calculate();

                    pck.SaveAs(memoryStream);

                }
            }
            return memoryStream.ToArray();
        }

        private void InsertShiftCodeData(ExcelWorksheet worksheet, int styleRow, List<ShiftCodePrintFormViewModel> tblPros)
        {
            var index = 0;
            var fromRow = styleRow + 1;
            var toRow = fromRow + tblPros.Count;
            for (int i = fromRow; i < toRow; i++)
            {
                try
                {
                    var tpDetail = tblPros.ElementAt(index);
                    worksheet.InsertRow(i, 1, styleRow);
                    var row = worksheet.Row(i);

                    // Update Data
                    worksheet.Cells[$"B{i}"].Value = tpDetail.Code;
                    worksheet.Cells[$"C{i}"].Value = tpDetail.Type;
                    worksheet.Cells[$"E{i}"].Value = tpDetail.TypeOfHaftDay;
                    worksheet.Cells[$"D{i}"].Value = tpDetail.Line;
                    worksheet.Cells[$"F{i}"].Value = tpDetail.IsHoliday;

                    index++;

                }
                catch (Exception ex)
                {

                }

            }
            worksheet.DeleteRow(styleRow);
        }
        #endregion


        #region  cr 9.9 target plan specical
        public async Task<ResultDTO> GetTargetPlanSpecial(QueryArgs args)
        {
            var result = new ResultDTO();
            var targetPlanSpecials = new List<TargetPlanSpecialViewModel>();
            var userDepartments = await _uow.GetRepository<TargetPlanSpecialDepartmentMapping>().FindByAsync(args.Predicate, args.PredicateParameters, args.Order, x => x.Department);
            var userGroupByDepartment = userDepartments.GroupBy(item => new { item.DepartmentId, item.IsIncludeChildren, item.DepartmentName, item.DepartmentCode, item.Id }).ToList();
            foreach (var department in userGroupByDepartment)
            {
                var targetPlanSpecial = new TargetPlanSpecialViewModel();
                targetPlanSpecial.Id = department.Key.Id;
                targetPlanSpecial.DepartmentId = department.Key.DepartmentId;
                targetPlanSpecial.DepartmentName = department.Key.DepartmentName;
                targetPlanSpecial.DepartmentCode = department.Key.DepartmentCode;
                targetPlanSpecial.IsIncludeChildren = department.Key?.IsIncludeChildren;

                targetPlanSpecials.Add(targetPlanSpecial);
            }
            result.Object = new ArrayResultDTO { Data = targetPlanSpecials.Skip((args.Page - 1) * args.Limit).Take(args.Limit), Count = targetPlanSpecials.Count() };
            return result;
        }
        public async Task<ResultDTO> CheckDepartmentExistInSpecial(Guid departmentId)
        {
            var result = new ResultDTO();
            var departments = await _uow.GetRepository<TargetPlanSpecialDepartmentMapping>().FindByAsync(x => x.DepartmentId == departmentId);
            result.Object = new ArrayResultDTO { Count = departments.Count() };
            return result;
        }
        public async Task<ResultDTO> CreateTargetPlanSpecial(TargetPlanSpecialArgs arg)
        {
            var result = new ResultDTO();
            var isInclude = await _uow.GetRepository<TargetPlanSpecialDepartmentMapping>().FindByAsync(x => x.DepartmentId == arg.DepartmentId);
            if (isInclude.Any())
            {
                return new ResultDTO { ErrorCodes = { 404 }, Messages = { "Department  is exists " }, };
            }
            {
                var department = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == arg.DepartmentId);   // không biết truyên code vào nên lấy nguyên con dept để lấy code  :) 
                var item = new TargetPlanSpecialDepartmentMapping();
                item.DepartmentCode = department.Code;
                item.IsIncludeChildren = arg.IsIncludeChildren;
                item.DepartmentId = arg.DepartmentId;
                item.DepartmentName = department.Name;
                _uow.GetRepository<TargetPlanSpecialDepartmentMapping>().Add(item);


                await _uow.CommitAsync();
                //data.Id = Guid.NewGuid();
                try
                {
                    await _trackingHistoryBO.SaveTrackingHistory(new TrackingHistoryArgs()
                    {
                        DataStr = JsonConvert.SerializeObject(Mapper.Map<TargetPlanSpecialViewModel>(item)),
                        ItemId = item.Id,
                        ItemType = ItemTypeContants.TargetPlanSpecial,
                        Type = TrackingHistoryTypeContants.Create,
                        ItemRefereceNumberOrCode = department.Code,
                    });
                    await _uow.CommitAsync();
                }
                catch (Exception e)
                {
                    _logger.LogError("Error: " + e.Message);
                }
            }

            return result;
        }
        public async Task<ResultDTO> DeleteTargetPlanSpecial(Guid departmentId)
        {
            var result = new ResultDTO();
            var shiftPlanSubmitPersons = await _uow.GetRepository<TargetPlanSpecialDepartmentMapping>().FindByAsync(x => x.DepartmentId == departmentId);
            foreach (var item in shiftPlanSubmitPersons)
            {
                _uow.GetRepository<TargetPlanSpecialDepartmentMapping>().Delete(item);
            }
            await _uow.CommitAsync();
            try
            {
                await _trackingHistoryBO.SaveTrackingHistory(new TrackingHistoryArgs()
                {
                    DataStr = JsonConvert.SerializeObject(Mapper.Map<TargetPlanSpecialViewModel>(shiftPlanSubmitPersons.FirstOrDefault())),
                    ItemId = shiftPlanSubmitPersons.FirstOrDefault().Id,
                    ItemType = ItemTypeContants.TargetPlanSpecial,
                    Type = TrackingHistoryTypeContants.Delete,
                    ItemRefereceNumberOrCode = shiftPlanSubmitPersons.FirstOrDefault().DepartmentCode,

                });
                await _uow.CommitAsync();
            }
            catch (Exception e)
            {
                _logger.LogError("Error: " + e.Message);
            }
            return result;
        }
        public async Task<ResultDTO> SaveTargetPlanSpecial(TargetPlanSpecialArgs arg)
        {
            var result = new ResultDTO();
            var departmentEdit = await _uow.GetRepository<TargetPlanSpecialDepartmentMapping>().FindByAsync(x => x.DepartmentId == arg.DepartmentIdOld);
            var department = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == arg.DepartmentId);   // không biết truyên code vào nên lấy nguyên con dept để lấy code  :) 

            //update user trong department thanh false
            if (departmentEdit.Any())
            {
                foreach (var item in departmentEdit)
                {
                    //item.IsSubmitPerson = false;
                    item.IsIncludeChildren = arg.IsIncludeChildren;
                    item.DepartmentCode = department.Code;
                    item.DepartmentName = department.Name;
                    item.DepartmentId = arg.DepartmentId;
                    _uow.GetRepository<TargetPlanSpecialDepartmentMapping>().Update(item);
                }
            }

            await _uow.CommitAsync();

            try
            {
                await _trackingHistoryBO.SaveTrackingHistory(new TrackingHistoryArgs()
                {
                    DataStr = JsonConvert.SerializeObject(Mapper.Map<TargetPlanSpecialViewModel>(departmentEdit.FirstOrDefault())),
                    ItemId = departmentEdit.FirstOrDefault().Id,
                    ItemType = ItemTypeContants.TargetPlanSpecial,
                    Type = TrackingHistoryTypeContants.Update,
                    ItemRefereceNumberOrCode = departmentEdit.FirstOrDefault().DepartmentCode,
                });
                await _uow.CommitAsync();
            }
            catch (Exception e)
            {
                _logger.LogError("Error: " + e.Message);
            }
            return result;
        }

        #endregion
        public async Task<ResultDTO> UploadData(ShiftCodeAgrs arg, Stream stream)
        {
            var dataFromFile = this.ReadDataFromStream(stream);
            var result = await this.InserData(dataFromFile, arg);
            return result;
        }


        public List<ShiftCodeImportFileDTO> ReadDataFromStream(Stream stream)
        {
            List<ShiftCodeImportFileDTO> result = new List<ShiftCodeImportFileDTO>();
            using (var pck = new ExcelPackage(stream))
            {
                ExcelWorkbook WorkBook = pck.Workbook;
                ExcelWorksheet workSheet = WorkBook.Worksheets.ElementAt(0);

                for (var rowNumber = 2; rowNumber <= workSheet.Dimension.End.Row; rowNumber++)
                {
                    var data = new ShiftCodeImportFileDTO();
                    data.Code = workSheet.Cells[rowNumber, 2, rowNumber, workSheet.Dimension.End.Column][$"B{rowNumber}"].Text;
                    data.Type = workSheet.Cells[rowNumber, 3, rowNumber, workSheet.Dimension.End.Column][$"C{rowNumber}"].Text;
                    data.TypeOfHaftDay = workSheet.Cells[rowNumber, 5, rowNumber, workSheet.Dimension.End.Column][$"E{rowNumber}"].Text;
                    data.Line = workSheet.Cells[rowNumber, 4, rowNumber, workSheet.Dimension.End.Column][$"D{rowNumber}"].Text.ToLower();
                    data.IsHoliday = workSheet.Cells[rowNumber, 6, rowNumber, workSheet.Dimension.End.Column][$"F{rowNumber}"].Text;
                    data.IsActive = workSheet.Cells[rowNumber, 7, rowNumber, workSheet.Dimension.End.Column][$"G{rowNumber}"].Text;
                    if (string.IsNullOrWhiteSpace(data.Code) && string.IsNullOrWhiteSpace(data.Type) && string.IsNullOrWhiteSpace(data.TypeOfHaftDay)
                        && string.IsNullOrWhiteSpace(data.Line) && string.IsNullOrWhiteSpace(data.IsHoliday))
                        break;
                    result.Add(data);
                }
            }
            return result;
        }

        public async Task<ResultDTO> InserData(List<ShiftCodeImportFileDTO> dataFromFile, ShiftCodeAgrs arg)
        {
            var result = new ResultDTO();
            var data = new ArrayResultDTO();
            string[] typeOfShift = { "NORM", "Absence", "Attendance" };
            List<string> holidayValid = new List<string> { "0", "1" };
            List<string> typeOfHaftDayValid = new List<string> { "0", "1", "2"};
            if (dataFromFile.Count > 0)
            {
                ImportTracking tracking = new ImportTracking();
                List<ImportShiftCodeError> importTrackingInfo = new List<ImportShiftCodeError>();
                int rowNum = 0;
                foreach (var item in dataFromFile)
                {
                    rowNum += 1;
                    ImportShiftCodeError inforTracking = new ImportShiftCodeError();
                    List<string> errorMessage = new List<string>();
                    var tempShiftCode = new ShiftCode() { };
                    inforTracking.RowNum = rowNum;

                    #region validate
                    if (string.IsNullOrEmpty(item.Code))
                    {
                        errorMessage.Add("ShiftCode is null!");
                    }
                    else tempShiftCode.Code = item.Code;

                    if (string.IsNullOrEmpty(item.Type))
                    {
                        errorMessage.Add("Type Of Shift is null!");
                    }
                    else if (!typeOfShift.Any(x => x.ToLower() == item.Type.ToLower()))
                    {
                        errorMessage.Add("Type " + item.Type + " is not defind!");
                    }
                    else tempShiftCode.Type = item.Type;

                    if (string.IsNullOrEmpty(item.TypeOfHaftDay))
                    {
                        errorMessage.Add("Type Of Half Day is null!");
                    }
                    else if (!typeOfHaftDayValid.Contains(item.TypeOfHaftDay))
                    {
                        errorMessage.Add("Type Of Half Day " + item.TypeOfHaftDay + " is not define !");
                    }
                    else if (item.Line.ToLower() == "fullday" && item.TypeOfHaftDay != "0")
                    {
                        errorMessage.Add("Line: Fullday - Type Of Half Day: 1 or 2 is not correct");
                    }
                    else if (item.Line.ToLower() == "halfday" && item.TypeOfHaftDay == "0")
                    {
                        errorMessage.Add("Line: halfday - Type Of Haft Day: 0 is not correct");
                    }
                    else tempShiftCode.TypeOfHaftDay = item.TypeOfHaftDay;

                    if (string.IsNullOrEmpty(item.Line))
                    {
                        errorMessage.Add("Line is null!");
                    }
                    else if (item.Line.ToLower() != "fullday" && item.Line.ToLower() != "halfday")
                    {
                        errorMessage.Add("Line " + item.Line + " is not define !");
                    }
                    else tempShiftCode.Line = item.Line.ToLower() == "fullday" ? ShiftLine.FULLDAY : ShiftLine.HAFTDAY;

                    if (string.IsNullOrEmpty(item.IsHoliday))
                        errorMessage.Add("Is Holiday is null!");
                    else if (!holidayValid.Contains(item.IsHoliday))
                        errorMessage.Add("Is Holiday " + item.IsHoliday + " is not define !");
                    else
                        tempShiftCode.IsHoliday = item.IsHoliday.Equals("1") ? true : false;

                    if (string.IsNullOrEmpty(item.IsActive))
                    {
                        item.IsActive = "1";
                    } else
                    {
                        if (!holidayValid.Contains(item.IsActive))
                        {
                            errorMessage.Add("Is Active " + item.IsActive + " is not define !");
                        }
                    }

                    #endregion
                    var shiftCode = await _uow.GetRepository<ShiftCode>().GetSingleAsync(i => i.Code == item.Code);
                    if (shiftCode != null && !errorMessage.Any())
                    {
                        try
                        {
                            if (string.IsNullOrWhiteSpace(item.Code) || string.IsNullOrWhiteSpace(item.Type) || string.IsNullOrWhiteSpace(item.TypeOfHaftDay)
                                || string.IsNullOrWhiteSpace(item.Line) || string.IsNullOrWhiteSpace(item.IsHoliday))
                            {
                                inforTracking.Status = "Failure";
                            }
                            else
                            {
                                var history = Mapper.Map<ShiftCodeViewModel>(shiftCode);
                                shiftCode.Code = item.Code;
                                shiftCode.Type = item.Type.ToUpper();
                                shiftCode.TypeOfHaftDay = item.Line == "1" ? item.TypeOfHaftDay = "0" : item.TypeOfHaftDay;
                                shiftCode.Line = item.Line.ToLower() == "fullday" ? ShiftLine.FULLDAY : ShiftLine.HAFTDAY;
                                shiftCode.IsHoliday = item.IsHoliday == "0" ? false : true;
                                shiftCode.IsActive = item.IsActive == "0" ? false : true;
                                _uow.GetRepository<ShiftCode>().Update(shiftCode);
                                await _trackingHistoryBO.SaveTrackingHistory(new TrackingHistoryArgs()
                                {
                                    DataStr = JsonConvert.SerializeObject(history),
                                    ItemId = shiftCode.Id,
                                    ItemType = ItemTypeContants.ShiftCode,
                                    Type = TrackingHistoryTypeContants.Update,
                                    ItemRefereceNumberOrCode = shiftCode.Code,
                                });
                                await _uow.CommitAsync();
                            }
                        }
                        catch (Exception e)
                        {
                            throw;
                        }
                    }
                    else
                    {
                        if (!errorMessage.Any())
                        {
                            tempShiftCode.Id = Guid.NewGuid();
                            _uow.GetRepository<ShiftCode>().Add(tempShiftCode);
                            await _uow.CommitAsync();
                            await _trackingHistoryBO.SaveTrackingHistory(new TrackingHistoryArgs()
                            {
                                DataStr = JsonConvert.SerializeObject(Mapper.Map<ShiftCodeViewModel>(tempShiftCode)),
                                ItemId = tempShiftCode.Id,
                                ItemType = ItemTypeContants.ShiftCode,
                                Type = TrackingHistoryTypeContants.Create,
                                ItemRefereceNumberOrCode = tempShiftCode.Code,
                            });

                            await _uow.CommitAsync();
                            inforTracking.Code = tempShiftCode.Code;
                            inforTracking.Type = tempShiftCode.Type.ToUpper();
                            inforTracking.TypeOfHaftDay = tempShiftCode.TypeOfHaftDay;
                            inforTracking.Line = tempShiftCode.Line;
                            inforTracking.IsHoliday = tempShiftCode.IsHoliday;
                            inforTracking.IsActive = tempShiftCode.IsActive;
                            inforTracking.Status = "Success";
                        }
                    }
                    inforTracking.ErrorMessage = errorMessage;
                    importTrackingInfo.Add(inforTracking);
                    await _uow.CommitAsync();
                }

                if (!string.IsNullOrEmpty(arg.FileName))
                    tracking.FileName = arg.FileName;
                tracking.JsonDataStr = JsonConvert.SerializeObject(importTrackingInfo);
                tracking.Module = "ShiftCode";
                tracking.IsImportManual = true;
                tracking.Status = "Success";
                if (!string.IsNullOrEmpty(arg.FileName))
                    tracking.FileName = arg.FileName;
                _uow.GetRepository<ImportTracking>().Add(tracking);
                data.Data = tracking;
                await _uow.CommitAsync();
            }
            else
            {
                result.ErrorCodes.Add(1001);
                result.Messages.Add("Cannot enter blank data");
            }
            result.Object = data;
            return result;
        }
        #endregion
    }
}