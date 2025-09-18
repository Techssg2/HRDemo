using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using AutoMapper;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Handlers
{
    public class EmployeeBO : IEmployeeBO
    {
        private ISAPBO _sap;
        private IMasterDataB0 _masterbo;
        private IUnitOfWork _uow;
        private ILogger _logger;

        public EmployeeBO(ISAPBO sap, IMasterDataB0 master, IUnitOfWork uow, ILogger logger)
        {
            _sap = sap;
            _masterbo = master;
            _uow = uow;
            _logger = logger;
        }

        public async Task<ResultDTO> GetFullEmployeeInfo(Guid userId, bool isActivated = true)
        {
            var result = new ResultDTO();
            var employeeInfo = new EmployeeViewModel();
            var edocEmployee = await _uow.GetRepository<User>().GetSingleAsync(x => x.Id == userId && x.IsActivated == isActivated, string.Empty);

            var jobGradeG5 = await _uow.GetRepository<JobGrade>().GetSingleAsync(x => x.Title.Equals("G5"));
            if (jobGradeG5 == null)
            {
                jobGradeG5 = new JobGrade() { Grade = 5 };
            }

            if (edocEmployee != null)
            {
                employeeInfo.Id = edocEmployee.Id;
                employeeInfo.FullName = edocEmployee.FullName;
                employeeInfo.LoginName = edocEmployee.LoginName;
                employeeInfo.Email = edocEmployee.Email;
                employeeInfo.SAPCode = edocEmployee.SAPCode;
                employeeInfo.Role = edocEmployee.Role;
                employeeInfo.Type = edocEmployee.Type;
                employeeInfo.StartDate = edocEmployee.StartDate;
                employeeInfo.IsNotTargetPlan = edocEmployee.IsNotTargetPlan;
                //Ngan them
                employeeInfo.ProfilePictureId = edocEmployee.ProfilePictureId;
                employeeInfo.ProfilePicture = (edocEmployee.ProfilePicture != null && !string.IsNullOrEmpty(edocEmployee.ProfilePicture.FileUniqueName) ? " /Attachments/" + edocEmployee.ProfilePicture.FileUniqueName : string.Empty);

                //End
                var department = await _uow.GetRepository<Department>().GetSingleAsync(x => x.UserDepartmentMappings.Any(t => t.UserId == edocEmployee.Id && t.IsHeadCount), "", x => x.JobGrade);

                if (department != null)
                {
                    employeeInfo.PositionName = department.PositionName;
                    if (department.Type == DepartmentType.Department)
                    {
                        employeeInfo.DeptCode = department.Code;
                        employeeInfo.DeptName = department.Name;
                        employeeInfo.DeptId = department.Id;
                        employeeInfo.IsStore = department.IsStore;
                        employeeInfo.IsHR = department.IsHR;
                        //employeeInfo.IsAdmin = department.IsAdmin;
                        employeeInfo.IsAdmin = await CheckIsAdmin(userId, department.Id);
                        employeeInfo.IsFacility = await CheckIsFacility(userId, department.Id);
                        employeeInfo.IsCB = await CheckIsCB(employeeInfo.Id);
                        if (department.JobGrade.Grade >= jobGradeG5.Grade)
                        {
                            employeeInfo.DeptG5Id = department.Id;
                        }
                        else
                        {
                            employeeInfo.DeptG5Id = await FindG5Department(department.ParentId, jobGradeG5.Grade);
                        }
                    }
                    else
                    {
                        employeeInfo.DivisionCode = department.Code;
                        employeeInfo.DivisionName = department.Name;
                        employeeInfo.DivisionId = department.Id;
                        employeeInfo.IsStore = department.IsStore;
                        employeeInfo.IsHR = department.IsHR;
                        //employeeInfo.IsAdmin = department.IsAdmin;
                        employeeInfo.IsAdmin = await CheckIsAdmin(userId, department.Id);
                        employeeInfo.IsITHelpDesk = CheckIsITHelpDesk(edocEmployee.Role);
                        employeeInfo.IsHRAdmin = CheckIsHRAdmin(edocEmployee.Role);
                        employeeInfo.IsFacility = await CheckIsFacility(userId, department.Id);
                        employeeInfo.IsCB = await CheckIsCB(employeeInfo.Id);
                        if (department.JobGrade.Grade >= jobGradeG5.Grade)
                        {
                            employeeInfo.DeptG5Id = department.Id;
                        }
                        else
                        {
                            employeeInfo.DeptG5Id = await FindG5Department(department.ParentId, jobGradeG5.Grade);
                        }
                        var skip = false;
                        var departmentIdx = department.ParentId;
                        while (!skip)
                        {
                            if (departmentIdx.HasValue)
                            {
                                var dept = await _uow.GetRepository<Department>().GetSingleAsyncIsNotDeleted(x => x.Id == departmentIdx);
                                if (dept != null)
                                {
                                    if (dept.IsDeleted)
                                    {
                                        dept = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == dept.ParentId);
                                        if (dept != null)
                                        {
                                            if (dept.Type == DepartmentType.Department)
                                            {
                                                employeeInfo.DeptCode = dept.Code;
                                                employeeInfo.DeptName = dept.Name;
                                                employeeInfo.DeptId = dept.Id;
                                                skip = true;
                                                await AddCanViewDeptLine(dept);
                                            }
                                            else
                                            {
                                                departmentIdx = dept.ParentId;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (dept.Type == DepartmentType.Department)
                                        {
                                            employeeInfo.DeptCode = dept.Code;
                                            employeeInfo.DeptName = dept.Name;
                                            employeeInfo.DeptId = dept.Id;
                                            skip = true;
                                            await AddCanViewDeptLine(dept);
                                        }
                                        else
                                        {
                                            departmentIdx = dept.ParentId;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                skip = true;
                            }
                        }
                    }

                    employeeInfo.JobGradeTitle = department.JobGrade.Title; //===== CR11.2 =====
                    employeeInfo.JobGradeName = department.JobGrade.Caption;
                    employeeInfo.JobGradeId = department.JobGrade.Id;
                    employeeInfo.JobGradeValue = department.JobGrade.Grade;
                    employeeInfo.JobGradeTitle = department.JobGrade.Title;
                    employeeInfo.StorePosition = department.JobGrade?.StorePosition;
                    employeeInfo.HQPosition = department.JobGrade?.HQPosition;

                    var submitPersonsMappings = await _uow.GetRepository<UserSubmitPersonDeparmentMapping>().FindByAsync<UserSubmitPersonDepartmentMappingViewModel>(x => x.IsSubmitPerson && x.DepartmentId == department.Id);
                    if (submitPersonsMappings.Any())
                    {
                        employeeInfo.SubmitPersons = submitPersonsMappings.Select(x => x.SAPCode);
                    }
                }
            }
            employeeInfo.IsITHelpDesk = CheckIsITHelpDesk(edocEmployee.Role);
            employeeInfo.IsHRAdmin = CheckIsHRAdmin(edocEmployee.Role);
            if (edocEmployee != null && !string.IsNullOrEmpty(edocEmployee.SAPCode))
            {
                try
                {
                    var res = await _sap.SearchEmployee(edocEmployee.SAPCode);
                    // Connect to SAP is fail
                    if (res.IsSuccess)
                    {
                        var dataInfo = JsonConvert.DeserializeObject<EmployeeResponsSearchingViewModel>(JsonConvert.SerializeObject(res.Object));
                        if (dataInfo != null)
                        {
                            //employeeInfo = Mapper.Map<EmployeeViewModel>(dataInfo);
                            employeeInfo.FullName = string.Format("{0} {1}", dataInfo.LastName, dataInfo.FirstName);
                            employeeInfo.GenderName = await GetMasterNameDataByCode("GenderByCode", dataInfo.GenderCode);
                            employeeInfo.WorkLocationName = await GetMasterNameDataByCode("WorkLocationByCode", dataInfo.WorkLocationCode);
                            employeeInfo.JobTitle = await GetMasterNameDataByCode("JobTitleByCode", dataInfo.JobTitle);
                            employeeInfo.PositionCode = dataInfo.PositionCode;
                            //employeeInfo.PositionName = await GetMasterNameDataByCode("PositionByCode", dataInfo.PositionCode);
                            employeeInfo.WorkLocationCode = dataInfo.WorkLocationCode;
                            employeeInfo.GenderCode = dataInfo.GenderCode;
                            employeeInfo.JobGradeCode = dataInfo.JobGradeCode;
                            employeeInfo.StartDate = DateTime.ParseExact(dataInfo.JoiningDate, "yyyyMMdd", CultureInfo.InvariantCulture);
                        }

                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError("Error at SearchEmployee from SAP.", ex.Message);
                }
            }
            if (employeeInfo.StartDate == DateTime.MinValue || !employeeInfo.StartDate.HasValue)
            {
                employeeInfo.StartDate = (DateTime?)null;
            }

            try
            {
                if (employeeInfo.StartDate != null)
                {
                    var findResignations = await _uow.GetRepository<ResignationApplication>().GetSingleAsync(x => employeeInfo.StartDate < x.OfficialResignationDate && x.UserSAPCode.Equals(employeeInfo.SAPCode) && x.Status.Equals("Completed"), "modified desc");
                    if (findResignations != null)
                    {
                        employeeInfo.OfficialResignationDate = findResignations.OfficialResignationDate;
                    }

                    var findPromotes = await _uow.GetRepository<PromoteAndTransfer>().FindByAsync<PromoteAndTransferViewModel>(x => employeeInfo.StartDate < x.EffectiveDate && x.UserId == employeeInfo.Id && x.Status.Equals("Completed"), "modified desc");
                    if (findPromotes != null)
                    {
                        employeeInfo.PromoteAndTransfers = findPromotes.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error: (findResignations, findPromotes): " + ex.Message, ex);
            }

            result.Object = employeeInfo;
            return result;
        }

        public async Task<Guid?> FindG5Department(Guid? parentId, int g5Grade)
        {
            if (!parentId.HasValue || parentId == Guid.Empty)
                return null;
            var parentDept = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == parentId && !x.IsDeleted, "", x => x.JobGrade);
            if (parentDept == null)
                return null;
            if (parentDept.JobGrade != null && parentDept.JobGrade.Grade >= g5Grade)
            {
                return parentDept.Id;
            }
            return await FindG5Department(parentDept.ParentId, g5Grade);
        }

        private async Task AddCanViewDeptLine(Department department)
        {
            if (department != null)
            {
                var findExistDeptLine = await _uow.GetRepository<CanViewDepartment>().GetSingleAsync(x => x.UserId == _uow.UserContext.CurrentUserId && x.DeptLineId == department.Id);
                if (findExistDeptLine == null)
                {
                    var canViewDeptLine = new CanViewDepartment()
                    {
                        UserId = _uow.UserContext.CurrentUserId,
                        DeptLineCode = department.Code,
                        DeptLineId = department.Id,
                    };
                    _uow.GetRepository<CanViewDepartment>().Add(canViewDeptLine);
                    await _uow.CommitAsync();
                }
            }
        }

        public async Task<EmployeeViewModel> GetUserProfileAdditionData(EmployeeViewModel employeeInfo)
        {
            employeeInfo.LeaveApplications = await GetLeaveBySAPCode(employeeInfo.SAPCode);
            employeeInfo.MissingTimeClocks = await GetMissingTimeClocksBySAPCode(employeeInfo.SAPCode);
            employeeInfo.OvertimeApplications = await GetOvertimeBySAPCode(employeeInfo.SAPCode);
            employeeInfo.ShiftExchangeApplications = await GetShiftExchangeBySAPCode(employeeInfo.SAPCode);
            return employeeInfo;
        }
        private async Task<List<ExportLeaveApplicationViewModel>> GetLeaveBySAPCode(string SAPCode)
        {
            var leaves = await _uow.GetRepository<LeaveApplication>().FindByAsync(x => x.UserSAPCode == SAPCode);
            if (leaves.Any())
            {
                var exportLeaveApplicationViewModels = new List<ExportLeaveApplicationViewModel>();
                foreach (var _leave in leaves)
                {
                    var leave = Mapper.Map<LeaveApplicationViewModel>(_leave);
                    if (_leave.LeaveApplicationDetails.Any())
                    {
                        var details = _leave.LeaveApplicationDetails;
                        if (details.Any())
                        {
                            foreach (var item in details)
                            {
                                var detail = Mapper.Map<LeaveApplicantDetailDTO>(item);
                                exportLeaveApplicationViewModels.Add(new ExportLeaveApplicationViewModel
                                {
                                    Id = leave.Id,
                                    ReferenceNumber = leave.ReferenceNumber,
                                    Status = leave.Status,
                                    DepartmentCode = !string.IsNullOrEmpty(leave.DeptCode) ? leave.DeptCode : leave.DivisionCode,
                                    DepartmentName = !string.IsNullOrEmpty(leave.DeptName) ? leave.DeptName : leave.DivisionName,
                                    FullName = leave.CreatedByFullName,
                                    SAPCode = leave.UserSAPCode,
                                    LeaveCode = detail.LeaveCode,
                                    LeaveName = string.Format("{0} - {1}", detail.LeaveName, detail.LeaveCode),
                                    FromDate = detail.FromDate,
                                    ToDate = detail.ToDate,
                                    Quantity = detail.Quantity,
                                    Reason = detail.Reason,
                                    CreateDate = leave.Created
                                });
                            }
                        }
                    }
                }
                return exportLeaveApplicationViewModels;
            }
            //return result.ToList();
            return null;
        }
        private async Task<List<MissingTimelockViewModel>> GetMissingTimeClocksBySAPCode(string SAPCode)
        {
            var result = await _uow.GetRepository<MissingTimeClock>().FindByAsync<MissingTimelockViewModel>(x => x.UserSAPCode == SAPCode);
            return result.ToList();
        }
        private async Task<List<ExportOvertimeApplicationDetailViewModel>> GetOvertimeBySAPCode(string SAPCode)
        {
            var overtimes = await _uow.GetRepository<OvertimeApplication>().FindByAsync(x => x.UserSAPCode == SAPCode);
            if (overtimes.Any())
            {
                var exportOvertimeApplicationDetailViewModels = new List<ExportOvertimeApplicationDetailViewModel>();
                foreach (var _overtime in overtimes)
                {
                    try
                    {
                        //var overtime = Mapper.Map<OvertimeApplicationViewModel>(_overtime);
                        if (_overtime.OvertimeItems.Any())
                        {
                            var details = _overtime.OvertimeItems;
                            if (details.Any())
                            {
                                foreach (var detail in details)
                                {
                                    try
                                    {
                                        //var detail = Mapper.Map<LeaveApplicantDetailDTO>(item);
                                        exportOvertimeApplicationDetailViewModels.Add(new ExportOvertimeApplicationDetailViewModel
                                        {
                                            Id = _overtime.Id,
                                            ReferenceNumber = _overtime.ReferenceNumber,
                                            Status = _overtime.Status,
                                            Date = detail.Date,
                                            ProposalHoursFrom = detail.ProposalHoursFrom,
                                            ProposalHoursTo = detail.ProposalHoursTo,
                                            OtProposalHours = detail.ProposalHoursTo != null && !string.IsNullOrEmpty(detail.ProposalHoursTo) && detail.ProposalHoursFrom != null && !string.IsNullOrEmpty(detail.ProposalHoursFrom) ?
                                                                (DateTime.Parse(detail.ProposalHoursTo.Replace('h', ':')) < DateTime.Parse(detail.ProposalHoursFrom.Replace('h', ':')) ?
                                                                    (DateTime.Parse(detail.ProposalHoursTo.Replace('h', ':')).AddHours(24) - DateTime.Parse(detail.ProposalHoursFrom.Replace('h', ':'))).ToString() :
                                                                    (DateTime.Parse(detail.ProposalHoursTo.Replace('h', ':')) - DateTime.Parse(detail.ProposalHoursFrom.Replace('h', ':'))).ToString()) : null,
                                            ActualHoursFrom = detail.ActualHoursFrom,
                                            ActualHoursTo = detail.ActualHoursTo,
                                            ActualOtHours = detail.ActualHoursTo != null && !string.IsNullOrEmpty(detail.ActualHoursTo) && detail.ActualHoursFrom != null && !string.IsNullOrEmpty(detail.ActualHoursFrom) ?
                                                            (DateTime.Parse(detail.ActualHoursTo.Replace('h', ':')) < DateTime.Parse(detail.ActualHoursFrom.Replace('h', ':')) ?
                                                                    (DateTime.Parse(detail.ActualHoursTo.Replace('h', ':')).AddHours(24) - DateTime.Parse(detail.ActualHoursFrom.Replace('h', ':'))).ToString() :
                                                                    (DateTime.Parse(detail.ActualHoursTo.Replace('h', ':')) - DateTime.Parse(detail.ActualHoursFrom.Replace('h', ':'))).ToString()) : null,
                                            ReasonName = detail.ReasonName,
                                            DateOffInLieu = detail.DateOffInLieu,
                                            IsNoOT = detail.IsNoOT,
                                            DepartmentCode = !string.IsNullOrEmpty(_overtime.DeptCode) ? _overtime.DeptCode : _overtime.DivisionCode,
                                            DepartmentName = !string.IsNullOrEmpty(_overtime.DeptName) ? _overtime.DeptName : _overtime.DivisionName,
                                            CreateDate = _overtime.Created,
                                            WorkLocationCode = !string.IsNullOrEmpty(_overtime.WorkLocationCode) ? _overtime.WorkLocationCode : "",
                                            WorkLocationName = !string.IsNullOrEmpty(_overtime.WorkLocationName) ? _overtime.WorkLocationName : "",
                                            DetailReason = string.IsNullOrEmpty(detail.DetailReason) ? detail.ReasonName : detail.DetailReason,
                                            FullName = _overtime.CreatedByFullName
                                        });
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogError("Error: " + ex.Message);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Error: " + ex.Message);
                    }
                }
                return exportOvertimeApplicationDetailViewModels;
            }
            return null;
        }
        private async Task<List<ExportShiftExchangeDetaiViewModel>> GetShiftExchangeBySAPCode(string SAPCode)
        {
            var shiftExchanges = await _uow.GetRepository<ShiftExchangeApplication>().FindByAsync<ShiftExchangeViewByReferenceNumberViewModel>(x => x.UserSAPCode == SAPCode || x.UserCreatedBy.SAPCode == SAPCode);
            if (shiftExchanges.Any())
            {
                var exportShiftExchangeApplicationViewModels = new List<ExportShiftExchangeDetaiViewModel>();
                foreach (var _shiftExchange in shiftExchanges)
                {
                    if (_shiftExchange.ExchangingShiftItems.Any())
                    {
                        var details = _shiftExchange.ExchangingShiftItems;
                        if (details.Any())
                        {
                            foreach (var detail in details)
                            {
                                exportShiftExchangeApplicationViewModels.Add(new ExportShiftExchangeDetaiViewModel
                                {
                                    Id = _shiftExchange.Id,
                                    ReferenceNumber = _shiftExchange.ReferenceNumber,
                                    Status = _shiftExchange.Status,
                                    FullName = detail.UserFullName,
                                    SAPCode = _shiftExchange.SAPCode,
                                    ReasonName = detail.ReasonName,
                                    ShiftExchangeDate = detail.ShiftExchangeDate,
                                    CurrentShiftCode = detail.CurrentShiftCode,
                                    CurrentShiftName = detail.CurrentShiftName,
                                    NewShiftCode = detail.NewShiftCode,
                                    NewShiftName = detail.NewShiftName,
                                    OtherReason = detail.OtherReason,
                                    ReasonCode = detail.ReasonCode,
                                    DepartmentCode = !string.IsNullOrEmpty(_shiftExchange.DeptLineCode) ? _shiftExchange.DeptLineCode : _shiftExchange.DeptDivisionCode,
                                    DepartmentName = !string.IsNullOrEmpty(_shiftExchange.DeptLineName) ? _shiftExchange.DeptLineName : _shiftExchange.DeptDivisionName,
                                    CreateDate = _shiftExchange.Created,
                                    EmployeeCode = detail.EmployeeCode

                                });
                            }
                        }
                    }
                }
                return exportShiftExchangeApplicationViewModels;
            }
            return null;
        }
        public async Task<DateTime?> GetJoiningDateOfEmployee(string SAPCode)
        {
            DateTime? result = null;
            try
            {
                var res = await _sap.SearchEmployee(SAPCode);
                if (res.IsSuccess)
                {
                    var dataInfo = JsonConvert.DeserializeObject<EmployeeResponsSearchingViewModel>(JsonConvert.SerializeObject(res.Object));
                    if (dataInfo != null)
                    {
                        result = DateTime.ParseExact(dataInfo.JoiningDate, "yyyyMMdd", CultureInfo.InvariantCulture);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at SearchEmployee from SAP.", ex.Message);
            }
            return result;
        }
        public async Task<ResultDTO> GetFullEmployeeInfo(string SAPCode)
        {
            var result = new ResultDTO();
            var employeeInfo = new EmployeeViewModel()
            {
                SAPCode = SAPCode
            };
            try
            {
                var res = await _sap.SearchEmployee(SAPCode);
                if (res.IsSuccess)
                {
                    var dataInfo = JsonConvert.DeserializeObject<EmployeeResponsSearchingViewModel>(JsonConvert.SerializeObject(res.Object));
                    if (dataInfo != null)
                    {
                        employeeInfo = Mapper.Map<EmployeeViewModel>(dataInfo);
                        employeeInfo.FullName = string.Format("{0} {1}", dataInfo.LastName, dataInfo.FirstName);
                        employeeInfo.GenderName = await GetMasterNameDataByCode("GenderByCode", dataInfo.GenderCode);
                        employeeInfo.WorkLocationName = await GetMasterNameDataByCode("WorkLocationByCode", dataInfo.WorkLocationCode);
                        employeeInfo.JobTitle = await GetMasterNameDataByCode("JobTitleByCode", dataInfo.JobTitle);
                        employeeInfo.PositionCode = dataInfo.PositionCode;
                        employeeInfo.PositionName = await GetMasterNameDataByCode("PositionByCode", dataInfo.PositionCode);
                        employeeInfo.WorkLocationCode = dataInfo.WorkLocationCode;
                        employeeInfo.GenderCode = dataInfo.GenderCode;
                        employeeInfo.JobGradeCode = dataInfo.JobGradeCode;
                        employeeInfo.StartDate = DateTime.ParseExact(dataInfo.JoiningDate, "yyyyMMdd", CultureInfo.InvariantCulture);
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at SearchEmployee from SAP.", ex.Message);
            }

            var edocEmployee = await _uow.GetRepository<User>().GetSingleAsync(x => x.SAPCode == SAPCode, string.Empty);
            if (edocEmployee != null)
            {
                employeeInfo.Id = edocEmployee.Id;
                employeeInfo.FullName = edocEmployee.FullName;
                employeeInfo.Email = edocEmployee.Email;
                employeeInfo.Role = edocEmployee.Role;
                employeeInfo.Type = edocEmployee.Type;
                var department = await _uow.GetRepository<Department>().GetSingleAsync(x => x.UserDepartmentMappings.Any(t => t.UserId == edocEmployee.Id && t.IsHeadCount), "", x => x.JobGrade);
                if (department != null)
                {
                    if (department.Type == DepartmentType.Department)
                    {
                        employeeInfo.DeptCode = department.Code;
                        employeeInfo.DeptName = department.Name;
                        employeeInfo.DeptId = department.Id;
                        employeeInfo.IsStore = department.IsStore;
                        employeeInfo.IsHR = department.IsHR;
                        //employeeInfo.IsAdmin = department.IsAdmin;
                        employeeInfo.IsAdmin = await CheckIsAdmin(employeeInfo.Id, department.Id);
                        employeeInfo.IsCB = await CheckIsCB(employeeInfo.Id);
                    }
                    else
                    {
                        employeeInfo.DivisionCode = department.Code;
                        employeeInfo.DivisionName = department.Name;
                        employeeInfo.DivisionId = department.Id;
                        employeeInfo.IsStore = department.IsStore;
                        employeeInfo.IsHR = department.IsHR;
                        //employeeInfo.IsAdmin = department.IsAdmin;
                        employeeInfo.IsAdmin = await CheckIsAdmin(employeeInfo.Id, department.Id);
                        employeeInfo.IsCB = await CheckIsCB(employeeInfo.Id);
                        var skip = false;
                        var departmentIdx = department.ParentId;
                        while (!skip)
                        {
                            if (departmentIdx.HasValue)
                            {
                                var dept = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == departmentIdx);
                                if (dept.Type == DepartmentType.Department)
                                {
                                    employeeInfo.DeptCode = dept.Code;
                                    employeeInfo.DeptName = dept.Name;
                                    employeeInfo.DeptId = department.Id;
                                    skip = true;
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
                    }

                    employeeInfo.JobGradeTitle = department.JobGrade.Title; //===== CR11.2 =====
                    employeeInfo.JobGradeName = department.JobGrade.Caption;
                    employeeInfo.JobGradeId = department.JobGrade.Id;
                    employeeInfo.JobGradeValue = department.JobGrade.Grade;
                }
                // Kiem tra user co Acting hay chua?
                if (!string.IsNullOrEmpty(employeeInfo.SAPCode) && employeeInfo.Id != null)
                {
                    var currentActing = await _uow.GetRepository<Acting>(true).GetSingleAsync(x => x.UserSAPCode == employeeInfo.SAPCode && x.UserId == employeeInfo.Id && x.Status == "Completed", "Created desc");
                    if (currentActing != null)
                    {
                        /*var lastWfInstance = await _uow.GetRepository<WorkflowInstance>(true).GetSingleAsync(x => x.ItemId == currentActing.Id, "Created desc");
                        if (lastWfInstance != null)
                        {
                            var IsActing = await _uow.GetRepository<WorkflowHistory>(true).CountAsync(x => x.InstanceId == lastWfInstance.Id && !string.IsNullOrEmpty(x.Outcome) && x.Outcome == "Accepted Target" && x.IsStepCompleted) > 0;
                            if (IsActing)
                                employeeInfo.ActingDepartmentId = currentActing.DepartmentId;
                        }*/
                        employeeInfo.ActingDepartmentId = currentActing.DepartmentId;
                    }
                }
            }
            if (employeeInfo.StartDate == DateTime.MinValue || !employeeInfo.StartDate.HasValue)
            {
                employeeInfo.StartDate = (DateTime?)null;
            }

            result.Object = employeeInfo;
            return result;
        }
        public async Task<ResultDTO> GetUsers(UserSAPArg arg)
        {
            return await _sap.GetUsers(arg);
        }
        public async Task<ResultDTO> GetMasterDataEmployeeList(string empSubGroup)
        {
            return await _sap.GetMasterDataEmployeeList(empSubGroup);
        }
        public async Task<ResultDTO> GetNewWorkLocationList(string newWorkLocationText)
        {
            return await _sap.GetNewWorkLocationList(newWorkLocationText);
        }

        public async Task<ResultDTO> GetNewWorkLocationListV2(string newWorkLocationCode)
        {
            return await _sap.GetNewWorkLocationListV2(newWorkLocationCode);
        }
        private async Task<string> GetMasterNameDataByCode(string type, string code)
        {
            string result = "";
            if (!string.IsNullOrEmpty(code))
            {
                var masterData = await _masterbo.GetMasterDataValues(new MasterDataArgs { Name = type, ParentCode = code });
                if (masterData.IsSuccess)
                {
                    var arrayData = JsonConvert.DeserializeObject<ArrayResultDTO>(JsonConvert.SerializeObject(masterData.Object)).Data;
                    List<MasterDataViewModel> masterDataValues = Mapper.Map<List<MasterDataViewModel>>(arrayData);
                    result = masterDataValues.FirstOrDefault()?.Name;
                }
            }
            return result;

        }
        private async Task<bool> CheckIsAdmin(Guid userId, Guid departmentId)
        {
            var result = false;
            var userIsAdmin = await _uow.GetRepository<UserDepartmentMapping>().FindByAsync(x => x.UserId == userId && x.Department.IsAdmin == true);
            if (userIsAdmin.Any())
            {
                result = true;
            }
            return result;
        }
        private bool CheckIsITHelpDesk(UserRole role)
        {
            return (role & UserRole.ITHelpDesk) == UserRole.ITHelpDesk;
        }
        private bool CheckIsHRAdmin(UserRole role)
        {
            return (role & UserRole.HRAdmin) == UserRole.HRAdmin;
        }
        private async Task<bool> CheckIsFacility(Guid userId, Guid departmentId)
        {
            var result = false;
            var userIsFacilities = await _uow.GetRepository<UserDepartmentMapping>().FindByAsync(x => x.UserId == userId && x.Department.IsFacility == true);
            if (userIsFacilities.Any())
            {
                result = true;
            }
            return result;
        }
        private async Task<bool> CheckIsCB(Guid userId)
        {
            var result = false;
            var userIsCB = await _uow.GetRepository<UserDepartmentMapping>().FindByAsync(x => x.UserId == userId && x.Department.IsCB == true);
            if (userIsCB.Any())
            {
                result = true;
            }
            return result;
        }
    }
}
