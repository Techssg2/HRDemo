using AutoMapper;
using Aeon.HR.ViewModels.DTOs;
using Aeon.HR.Data.Models;
using Aeon.HR.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aeon.HR.ViewModels.Args;
using System.Web;
using System.Globalization;
using System.Security;
using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.ViewModels.PrintFormViewModel;
using System.Collections;

namespace Aeon.HR.BusinessObjects.Handlers
{
    public partial class CBBO
    {
        #region Resignation Application 
        public async Task<ResultDTO> GetAllResignationApplicantion(QueryArgs args)
        {
            //if (args.QueryArgs.PredicateParameters[0].ToString() != "")
            //{
            //    var userId = Guid.Parse(args.QueryArgs.PredicateParameters[0].ToString());
            //    var resignationApplications = await _uow.GetRepository<ResignationApplication>().FindByAsync<ResignationApplicantGridViewModel>(args.QueryArgs.Order, args.QueryArgs.Page, args.QueryArgs.Limit, args.QueryArgs.Predicate, args.QueryArgs.PredicateParameters);
            //    var count = await _uow.GetRepository<ResignationApplication>().CountAsync(args.QueryArgs.Predicate, args.QueryArgs.PredicateParameters);
            //    return new ResultDTO { Object = new ArrayResultDTO { Data = resignationApplications, Count = count } };
            //}
            //else
            //{
            //    var resignationApplications = await _uow.GetRepository<ResignationApplication>().GetAllAsync<ResignationApplicantGridViewModel>(args.QueryArgs.Order, args.QueryArgs.Page, args.QueryArgs.Limit);
            //    var count = await _uow.GetRepository<ResignationApplication>().CountAllAsync();
            //    return new ResultDTO { Object = new ArrayResultDTO { Data = resignationApplications, Count = count } };
            //}

            int resignationsCount = 0;
            var sapCodes = new List<string>();
            try
            {
                if (args.Object != null)
                {
                    List<Guid> jobGradesGuid = ((IEnumerable)args.Object).Cast<object>().Select(x => new Guid(x.ToString())).Where(x => x != null).ToList();
                    if (jobGradesGuid.Count() > 0)
                    {
                        List<Guid> jd = new List<Guid>();
                        foreach (var gradeId in jobGradesGuid)
                        {
                            jd.Add((await _uow.GetRepository<JobGrade>().GetSingleAsync(x => gradeId == x.Id)).Id);
                        }
                        if (jd.Any())
                        {
                            var sapCodess = (await _uow.GetRepository<User>().FindByAsync(x => x.UserDepartmentMappings.Where(y => jd.Contains(y.Department.JobGradeId) && !y.User.IsDeleted && y.IsHeadCount).ToList().Select(z => z.UserId).Contains(x.Id)));
                            if (sapCodess != null)
                            {
                                foreach (var it in sapCodess)
                                {
                                    sapCodes.Add(it.SAPCode);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                sapCodes = new List<string>();  
            }

            var resignationApplications = await _uow.GetRepository<ResignationApplication>()
            .FindByAsync<ResignationApplicantGridViewModel>(args.Predicate, args.PredicateParameters, args.Order);
            if (sapCodes.Any() || (args.Object != null && !sapCodes.Any()))
            {
                resignationsCount = resignationApplications.Where(x => sapCodes.Contains(x.UserSAPCode)).Count();
                //resignationApplications = (await _uow.GetRepository<ResignationApplication>().FindByAsync<ResignationApplicantGridViewModel>(x => sapCodes.Contains(x.UserSAPCode), args.Order)).Skip((args.Page - 1) * args.Limit).Take(args.Limit);
                resignationApplications = resignationApplications.Where(x => sapCodes.Contains(x.UserSAPCode)).Skip((args.Page - 1) * args.Limit).Take(args.Limit);
            }
            else
            {
                resignationsCount = resignationApplications.Count();
                resignationApplications = resignationApplications.Skip((args.Page - 1) * args.Limit).Take(args.Limit);
            }

            return new ResultDTO { Object = new ArrayResultDTO { Data = resignationApplications, Count = resignationsCount } };
        }
        public async Task<ResultDTO> CheckInProgressResignationApplicantion(string userSapCode)
        {
            var pendingStatus = Utilities.PendingStatuses();
            var exist = await _uow.GetRepository<ResignationApplication>().FindByAsync<ResignationApplicantGridViewModel>(x => x.UserSAPCode == userSapCode && (!pendingStatus.Contains(x.Status) || x.Status == "Completed"));
            return new ResultDTO { Object = exist.Any() };
        }
        public async Task<ResultDTO> CheckInProgressResignationWithIsActive(string userSapCode)
        {
            var pendingStatus = Utilities.PendingStatuses();
            var existResignationApplicant = await _uow.GetRepository<ResignationApplication>().FindByAsync<ResignationApplicantGridViewModel>(x => x.UserSAPCode == userSapCode && (!pendingStatus.Contains(x.Status) || x.Status == "Completed"));
            var isActiveUser = await _uow.GetRepository<User>(true).AnyAsync(x => x.IsActivated == true && x.SAPCode==userSapCode);
            return new ResultDTO { Object = existResignationApplicant.Any() && !isActiveUser };
        }
        public async Task<ResultDTO> GetResignationApplicantionByReferenceNumber(MasterdataArgs args)
        {
            var param = args.QueryArgs.PredicateParameters[0].ToString();
            Guid id = new Guid(param);
            var resignationApplications = await _uow.GetRepository<ResignationApplication>().FindByAsync<ResignationApplicationViewModel>(x => x.Id == id);
            return new ResultDTO { Object = resignationApplications.FirstOrDefault() };
        }

        public async Task<ResultDTO> SaveResignationApplicantion(ResignationApplicationArgs data)
        {
            var result = new ResultDTO();
            var resignationApplicationExist = await _uow.GetRepository<ResignationApplication>().GetSingleAsync(x => x.Id == data.Id);
            if (resignationApplicationExist == null)
            {

                var resignationApplication = Mapper.Map<ResignationApplication>(data);
                _uow.GetRepository<ResignationApplication>().Add(resignationApplication);
                result.Object = Mapper.Map<ResignationApplicationViewModel>(resignationApplication);
            }
            else
            {
                data.ReferenceNumber = resignationApplicationExist.ReferenceNumber;
                if (!resignationApplicationExist.SubmitDate.HasValue)
                {
                    try
                    {
                        // luu ngay subit dau tien
                        var submitedDate = await GetSubmittedFirstDate(resignationApplicationExist.Id);
                        if (submitedDate.IsSuccess)
                        {
                            var wfhistoriesModel = (WorkflowHistoryViewModel)submitedDate.Object;
                            if (wfhistoriesModel != null && wfhistoriesModel.Modified != null)
                            {
                                resignationApplicationExist.SubmitDate = wfhistoriesModel.Modified;
                            }
                        }
                    }
                    catch (Exception ex) { }
                }
                Mapper.Map(data, resignationApplicationExist);
                _uow.GetRepository<ResignationApplication>().Update(resignationApplicationExist);
                result.Object = Mapper.Map<ResignationApplicationViewModel>(resignationApplicationExist);
            }
            await _uow.CommitAsync();
            return result;
        }

        public async Task<ResultDTO> SubmitResignationApplicantion(ResignationApplicationArgs data)
        {
            var resignationApplicationExist = await _uow.GetRepository<ResignationApplication>().GetSingleAsync(x => x.Id == data.Id);
            if (resignationApplicationExist == null)
            {
                var resignationApplication = Mapper.Map<ResignationApplication>(data);

                _uow.GetRepository<ResignationApplication>().Add(resignationApplication);
                await _uow.CommitAsync();
                return new ResultDTO { Object = Mapper.Map<ResignationApplicationViewModel>(resignationApplication) };
            }
            else
            {
                Mapper.Map(data, resignationApplicationExist);
                _uow.GetRepository<ResignationApplication>().Update(resignationApplicationExist);
                await _uow.CommitAsync();
            }
            return new ResultDTO { Object = Mapper.Map<ResignationApplicationViewModel>(resignationApplicationExist) };
        }
        public async Task<ResignationApplicationViewModel> GetResignationApplicantionById(Guid id)
        {
            var currentUser = await _uow.GetRepository<User>().GetSingleAsync(x => x.Id == _uow.UserContext.CurrentUserId);
            var IsStoredepartmetnmapingCurrentUser = await _uow.GetRepository<UserDepartmentMapping>().AnyAsync(x => x.UserId == _uow.UserContext.CurrentUserId && x.IsHeadCount && x.Department.IsStore);
            var isaccess = false;
            if ((currentUser.Role & UserRole.HR) == UserRole.HR)
            {
                if (!IsStoredepartmetnmapingCurrentUser)
                    isaccess = true;
                else
                {
                    var recordCurrent = await _uow.GetRepository<ResignationApplication>(true).FindByIdAsync<ResignationApplicationViewModel>(id);
                    var currentUserRP = await _uow.GetRepository<User>().GetSingleAsync(x => x.SAPCode == recordCurrent.UserSAPCode);
                    var IsStoredepartmetnmapingCurrentUserRP = await _uow.GetRepository<UserDepartmentMapping>().AnyAsync(x => x.UserId == currentUserRP.Id && x.IsHeadCount && x.Department.IsStore);

                    if (IsStoredepartmetnmapingCurrentUserRP)
                    {
                        isaccess = true;
                    }
                }
            }
            if (isaccess)
            {
                return await _uow.GetRepository<ResignationApplication>(isaccess).FindByIdAsync<ResignationApplicationViewModel>(id);
            }
            var record = await _uow.GetRepository<ResignationApplication>().FindByIdAsync<ResignationApplicationViewModel>(id);
            return record;
        }
        public async Task<byte[]> PrintFormResignation(Guid Id)
        {
            byte[] result = null;
            var record = await _uow.GetRepository<ResignationApplication>().FindByIdAsync(Id);
            if (record != null)
            {
                var dataToPrint = Mapper.Map<ResignationApplicationPrintFormViewModel>(record);
                switch (record.ContractTypeCode)
                {
                    case ContractType.OneDays: // Hợp đồng học việc, thử việc báo trước 01 ngày - checkbox 1
                        dataToPrint.CheckedBoxs.Add("checkbox1");
                        break;
                    case ContractType.ThreeDays: // 03  days prior notice must be given  for definite - checkbox 2
                        dataToPrint.CheckedBoxs.Add("checkbox2");
                        break;
                    case ContractType.ThirtyDays: // 30  days prior notice must be given for definite term contract - checkbox 3
                        dataToPrint.CheckedBoxs.Add("checkbox3");
                        break;
                    case ContractType.FourtyFive: // 45  days prior notice must be given for indefinite term contract - checkbox 4
                        dataToPrint.CheckedBoxs.Add("checkbox4");
                        break;
                }
                if (record.IsAgree.Value)
                {
                    dataToPrint.CheckedBoxs.Add("checkbox5"); // 
                }
                if (record.IsNotifiedLastWorkingDate)
                {
                    dataToPrint.CheckedBoxs.Add("checkbox6"); // 
                }
                var properties = typeof(ResignationApplicationPrintFormViewModel).GetProperties();
                var pros = new Dictionary<string, string>();
                var tbPros = await GetWorkFlowHistories(Id, ObjectToPrintFromType.Resignation, dataToPrint);
                foreach (var property in properties)
                {
                    var value = Convert.ToString(property.GetValue(dataToPrint));
                    pros[property.Name] = SecurityElement.Escape(value);
                }
                result = WordAutomation.ExportPDF("Resignation.docx", pros, tbPros, dataToPrint.CheckedBoxs);
            }
            return result;
        }
        #endregion
        public async Task<List<Dictionary<string, string>>> GetWorkFlowHistories(Guid Id, ObjectToPrintFromType type, object dataToPrint = null)
        {
            string submitedDate = "";
            var wfStatus = await _workflowBO.GetWorkflowStatusByItemId(Id);
            var workflowStatus = (WorkflowStatusViewModel)wfStatus.Object;
            var tbPros = new List<Dictionary<string, string>>();
            if (workflowStatus != null && workflowStatus.WorkflowInstances != null)
            {
                var wfInstanceList = workflowStatus.WorkflowInstances.FirstOrDefault();

                if (type == ObjectToPrintFromType.Resignation)
                {
                    var resignationApplicationPrintForm = (ResignationApplicationPrintFormViewModel) dataToPrint;
                    submitedDate = await getFirstDateSubmit(wfInstanceList.ItemReferenceNumber);
                    resignationApplicationPrintForm.ConfirmDate = submitedDate;
                }

                UpdateAdditionInfo(wfInstanceList, type, dataToPrint);
                if (wfInstanceList != null && wfInstanceList.Histories != null)
                {
                    foreach (var item in wfInstanceList.Histories)
                    {
                        if (item != null && item.IsStepCompleted)
                        {
                            var rowPros = new Dictionary<string, string>();
                            if (type == ObjectToPrintFromType.Resignation && item.StepNumber == 1)
                            {
                                rowPros["AssignedDate"] = submitedDate;
                            }
                            else
                            {
                                rowPros["AssignedDate"] = SecurityElement.Escape(item.Modified.ToString("dd/MM/yyyy"));
                            }
                            rowPros["AssignedDate"] = SecurityElement.Escape(item.Modified.ToString("dd/MM/yyyy"));
                            rowPros["AssignedBy"] = SecurityElement.Escape(!string.IsNullOrEmpty(item.ApproverFullName) ? string.Format("{0} Signed", item.ApproverFullName) : string.Empty);
                            rowPros["Outcome"] = SecurityElement.Escape(item.Outcome);
                            rowPros["Comment"] = SecurityElement.Escape(item.Comment);
                            tbPros.Add(rowPros);
                        }
                    }
                }
                else
                {
                    CreateEmptyTbPros(tbPros);
                }
            }
            else
            {
                CreateEmptyTbPros(tbPros);
            }
            return tbPros;
        }

        private async Task<string> getFirstDateSubmit(string referenceNumber)
        {
            var items = (await _uow.GetRepository<WorkflowInstance>().FindByAsync(x => x.ItemReferenceNumber == referenceNumber, "created asc")).FirstOrDefault();
            if (items != null)
            {
                var wfhistorires = (await _uow.GetRepository<WorkflowHistory>().FindByAsync(x => x.InstanceId == items.Id, "created asc")).FirstOrDefault();
                if (wfhistorires != null)
                    return wfhistorires.Modified.ToString("dd/MM/yyyy");
            }
            return "";
        }

        private void CreateEmptyTbPros(List<Dictionary<string, string>> tbProsTemp)
        {
            var rowPros = new Dictionary<string, string>();
            rowPros["AssignedDate"] = "";
            rowPros["AssignedBy"] = "";
            rowPros["Outcome"] = "";
            rowPros["Comment"] = "";
            tbProsTemp.Add(rowPros);
        }
        private void UpdateAdditionInfo(WorkflowInstanceViewModel workflowInstanceView, ObjectToPrintFromType type, object dataToPrint = null)
        {
            var histories = workflowInstanceView.Histories.ToList();
            if (type == ObjectToPrintFromType.Resignation)
            {
                var resignationApplicationPrintForm = (ResignationApplicationPrintFormViewModel)dataToPrint;
                int i = 0;
                foreach (var item in workflowInstanceView.WorkflowData.Steps)
                {
                    if (i < histories.Count)
                    {
                        var currentHistory = histories.ElementAt(i);
                        if (currentHistory.IsStepCompleted)
                        {
                            // HR-823
                           /* if (item.StepNumber == 1)
                            {
                                resignationApplicationPrintForm.ConfirmDate = currentHistory.Modified.ToString("dd/MM/yyyy");
                            }*/
                            if (item.StepNumber == 2)
                            {
                                resignationApplicationPrintForm.HrConfirmName = !string.IsNullOrEmpty(currentHistory.ApproverFullName) ? currentHistory.ApproverFullName + " Signed" : "";
                                resignationApplicationPrintForm.SignedDateHrConfirm = currentHistory.Modified.ToString("dd/MM/yyyy");
                            }
                            else if (item.StepNumber == 3)
                            {
                                resignationApplicationPrintForm.HeadLineManager = !string.IsNullOrEmpty(currentHistory.ApproverFullName) ? currentHistory.ApproverFullName + " Signed" : "";
                                resignationApplicationPrintForm.SignedDateHeadLineManager = currentHistory.Modified.ToString("dd/MM/yyyy");
                            }
                            else if (item.StepNumber == workflowInstanceView.WorkflowData.Steps.Count())        //minhlnd change
                            {
                                resignationApplicationPrintForm.StoreManager = !string.IsNullOrEmpty(currentHistory.ApproverFullName) ? currentHistory.ApproverFullName + " Signed" : "";
                                resignationApplicationPrintForm.SignedDateStoreManager = currentHistory.Modified.ToString("dd/MM/yyyy");
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                    i++;
                }
            }
            else if (type == ObjectToPrintFromType.Overtime)
            {
                var overtimeInfo = (OvertimePrintFormViewModel)dataToPrint;
                int i = 0;
                foreach (var item in workflowInstanceView.WorkflowData.Steps)
                {
                    if (i < histories.Count)
                    {
                        var currentHistory = histories.ElementAt(i);
                        if (currentHistory.IsStepCompleted)
                        {
                            if (overtimeInfo.IsHQ)
                            {
                                if (item.StepNumber == 2)
                                {
                                    overtimeInfo.ApprovedDepartmentHead = currentHistory.ApproverFullName;
                                }
                                else if (item.StepNumber == 3)
                                {
                                    overtimeInfo.ApprovedSeniorGeneralManager = currentHistory.ApproverFullName;
                                }
                                else if (item.StepNumber == 4)
                                {
                                    overtimeInfo.CheckByHR = currentHistory.ApproverFullName;
                                }
                            }
                            else
                            {
                                if (overtimeInfo.Grade <= 2)
                                {
                                    if (item.StepNumber == 2)
                                    {
                                        overtimeInfo.ApprovedDepartmentHead = currentHistory.ApproverFullName;
                                    }
                                    else if (item.StepNumber == 3)
                                    {
                                        overtimeInfo.ApprovedSeniorGeneralManager = currentHistory.ApproverFullName;
                                    }
                                    else if (item.StepNumber == 4)
                                    {
                                        overtimeInfo.CheckByHR = currentHistory.ApproverFullName;
                                    }

                                }
                                else if (overtimeInfo.Grade > 2)
                                {
                                    if (item.StepNumber == 2)
                                    {
                                        overtimeInfo.ApprovedDepartmentHead = currentHistory.ApproverFullName;
                                    }
                                    else if (item.StepNumber == 3)
                                    {
                                        overtimeInfo.CheckByHR = currentHistory.ApproverFullName;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                    i++;
                }
            }

        }
        public async Task<ResultDTO> GetSubmittedFirstDate(Guid ItemId)
        {
            var result = new ResultDTO();
            try
            {
                var instance = await _uow.GetRepository<WorkflowInstance>().FindByAsync(x => x.ItemId == ItemId);
                if (instance != null)
                {
                    var instanceListId = instance.Select(x => x.Id).ToList();
                    var histories = await _uow.GetRepository<WorkflowHistory>().FindByAsync<WorkflowHistoryViewModel>(x => instanceListId.Contains(x.InstanceId) && x.Outcome == "Submitted", "Created asc");
                    if (histories != null && histories.Any())
                    {
                        var firstHistory = histories.FirstOrDefault();
                        if (firstHistory != null)
                        {
                            result.Object = firstHistory;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                result.Object = null;
                return result;
            }
            return result;
        }

        public async Task<ResultDTO> CountExitInterview(Guid ItemId)
        {
            var result = new ResultDTO();
            try
            {
                var currentRes = await _uow.GetRepository<ResignationApplication>().GetSingleAsync(x => x.Id == ItemId);
                if (currentRes != null)
                {
                    currentRes.CountExitInterview += 1;
                    _uow.GetRepository<ResignationApplication>().Update(currentRes);
                    await _uow.CommitAsync();
                    result.Object = Mapper.Map<ResignationApplicationViewModel>(currentRes);
                }
            }
            catch (Exception e)
            {
                result.ErrorCodes = new List<int> { -1 };
                result.Messages = new List<string> { e.Message };
            }
            return result;
        }
    }
}