using AutoMapper;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using Aeon.HR.Data.Models;
using Aeon.HR.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.ViewModels.PrintFormViewModel;
using System.Security;
using System.IO;
using OfficeOpenXml;
using System.Text.RegularExpressions;
using Aeon.HR.ViewModels.ImportData;
using Aeon.HR.BusinessObjects.Helpers;

namespace Aeon.HR.BusinessObjects.Handlers
{
    public partial class CBBO
    {
        #region Overtime Application
        public Task<ResultDTO> ApproveOvertimeApplication(Guid Id)
        {
            throw new NotImplementedException();
        }

        public async Task<ResultDTO> SaveOvertimeApplication(OvertimeApplicationArgs arg)
        {
            var result = new ResultDTO();
            bool isValid_OTTime = true;
            List<string> messages = new List<string>();
            #region Get ShiftCode MasterData
            ResultDTO ShiftCodeData = await _masterData.GetMasterDataValues(new MasterDataArgs() { Name = "ShiftCode", ParentCode = string.Empty });
            ArrayResultDTO ShiftCodeArray = ShiftCodeData.Object as ArrayResultDTO;
            List<MasterExternalDataViewModel> ShiftCode_MasterData = ShiftCodeArray.Data as List<MasterExternalDataViewModel>;
            OTCheckingResultCollection checkResults = new OTCheckingResultCollection();
            #endregion

            #region check OT time
            if (!(arg is null) && !string.IsNullOrEmpty(arg.Status) && arg.Status.Equals("Waiting for Fill Actual Hour", StringComparison.Ordinal))
            {
                ShiftCodeDetailCollection shiftCodeCollection = new ShiftCodeDetailCollection(ShiftCode_MasterData);

                var overtimeListCount = arg.OvertimeList.Count;
                for (int i = 0; i < overtimeListCount; i++)
                {
                    OvertimeApplicationDetailArgs current_OT_Details = arg.OvertimeList[i];

                    DateTime otDate = current_OT_Details.Date.GetAsDateTime();
                    if (current_OT_Details.DateOffInLieu || current_OT_Details.IsNoOT || (otDate != DateTime.MinValue && otDate.IsPublicHoliday(_uow)))
                    {
                        //If DateOffInLieu with NOT check actual OT time overlapse the working time
                        //
                        //If public holiday will NOT check actual OT time overlapse the working time
                        //
                        OTCheckingResult currentCheckingResult = new OTCheckingResult();
                        currentCheckingResult.success = true;
                        arg.OvertimeList[i].CalculatedActualHoursFrom = current_OT_Details.ActualHoursFrom;
                        arg.OvertimeList[i].CalculatedActualHoursTo = current_OT_Details.ActualHoursTo;
                        checkResults.Add(currentCheckingResult);                       
                    }
                    else
                    {
                        #region Get user of OT details Item
                        User user = null;
                        //Get User of OT details
                        if (arg.Type == OverTimeType.ManagerApplyForEmployee)
                        {
                            user = current_OT_Details.SAPCode.GetUserByUserSAP(_uow);
                        }
                        else
                        {
                            user = arg.UserSAPCode.GetUserByUserSAP(_uow);
                        }
                        #endregion
                        OTCheckingResult currentCheckingResult = current_OT_Details.Check_OT_Valid(user, _uow, shiftCodeCollection, true);
                        if (currentCheckingResult.success)
                        {
                            arg.OvertimeList[i].CalculatedActualHoursFrom = currentCheckingResult.calculatedActualHoursFrom;
                            arg.OvertimeList[i].CalculatedActualHoursTo = currentCheckingResult.calculatedActualHoursTo;
                        }
                        checkResults.Add(currentCheckingResult);
                    }
                }

                isValid_OTTime = checkResults.isSuccess();
                if(!isValid_OTTime)
                {
                    result.Object = checkResults;
                }
            }
            else
            {
                var overtimeListCount = arg.OvertimeList.Count;
                for (int i = 0; i < overtimeListCount; i++)
                {
                    OvertimeApplicationDetailArgs current_OT_Details = arg.OvertimeList[i];
                    if (!current_OT_Details.IsNoOT) {
                        User user = null;
                        if (arg.Type == OverTimeType.ManagerApplyForEmployee)
                            user = current_OT_Details.SAPCode.GetUserByUserSAP(_uow);
                        else
                            user = arg.UserSAPCode.GetUserByUserSAP(_uow);

                        checkResults.Add(current_OT_Details.Validate_OT_Valid(user, _uow, true));
                    }
                }

                if (!checkResults.isSuccess())
                {
                    result.ErrorCodes = new List<int>() { 1006 };
                    result.Object = checkResults;
                    goto Finish;
                }

                //ignore check OT
                isValid_OTTime = true;
            }
            #endregion


            if (isValid_OTTime)
            {
                var overtimeExist = await _uow.GetRepository<OvertimeApplication>().GetSingleAsync(x => x.Id == arg.Id);
                if (overtimeExist == null)
                {
                    var overtimeApplication = Mapper.Map<OvertimeApplication>(arg);

                    arg.OvertimeList.ForEach(x =>
                    {
                        x.Id = Guid.NewGuid();
                        var item = Mapper.Map<OvertimeApplicationDetailArgs, OvertimeApplicationDetail>(x);
                        overtimeApplication.OvertimeItems.Add(item);

                    });
                    _uow.GetRepository<OvertimeApplication>().Add(overtimeApplication);
                    result.Object = Mapper.Map<OvertimeApplicationViewModel>(overtimeApplication);
                }
                else
                {
                    overtimeExist.Status = arg.Status;
                    if (overtimeExist.Type == OverTimeType.ManagerApplyForEmployee)
                    {
                        overtimeExist.DivisionId = arg.DivisionId;
                        overtimeExist.DivisionCode = arg.DivisionCode;
                        overtimeExist.DivisionName = arg.DivisionName;
                        overtimeExist.ReasonCode = arg.ReasonCode;
                        overtimeExist.ApplyDate = arg.ApplyDate;
                        overtimeExist.ReasonName = arg.ReasonName;
                        overtimeExist.ContentOfOtherReason = arg.ContentOfOtherReason;
                    }
                    if (!string.IsNullOrEmpty(arg.TimeInRound))
                    {
                        overtimeExist.TimeInRound = arg.TimeInRound;
                    }
                    if (!string.IsNullOrEmpty(arg.TimeOutRound))
                    {
                        overtimeExist.TimeOutRound = arg.TimeOutRound;
                    }
                    _uow.GetRepository<OvertimeApplication>().Update(overtimeExist);
                    var overtimeListExit = await _uow.GetRepository<OvertimeApplicationDetail>().FindByAsync(x => x.OvertimeApplicationId == overtimeExist.Id);
                    _uow.GetRepository<OvertimeApplicationDetail>().Delete(overtimeListExit);

                    foreach (var item in arg.OvertimeList)
                    {

                        item.OvertimeApplicationId = overtimeExist.Id;
                        _uow.GetRepository<OvertimeApplicationDetail>().Add(Mapper.Map<OvertimeApplicationDetailArgs, OvertimeApplicationDetail>(item));
                    }
                    result.Object = Mapper.Map<OvertimeApplicationViewModel>(overtimeExist);
                }
                await _uow.CommitAsync();
            }
            else
            {
                result.ErrorCodes = new List<int>() { 1007 };
                result.Messages = messages;

            }
            Finish:
            return result;
        }
        public async Task<ResultDTO> UpdateOvertimeApplication(OvertimeApplicationArgs model)
        {
            OvertimeApplication overtimeApplication = await _uow.GetRepository<OvertimeApplication>().FindByIdAsync(model.Id);
            if (overtimeApplication == null)
            {
                return new ResultDTO { ErrorCodes = { 404 }, Messages = { "Overtime application not found" } };
            }
            else
            {
                overtimeApplication.Modified = DateTimeOffset.Now;
                //remove overtime items
                foreach (var item in overtimeApplication.OvertimeItems)
                {
                    OvertimeApplicationDetail detail = await _uow.GetRepository<OvertimeApplicationDetail>().FindByIdAsync(item.Id);
                    _uow.GetRepository<OvertimeApplicationDetail>().Delete(detail);
                }
                //add overtime items
                model.OvertimeList.ForEach(x =>
                {
                    var item = Mapper.Map<OvertimeApplicationDetailArgs, OvertimeApplicationDetail>(x);
                    overtimeApplication.OvertimeItems.Add(item);

                });
                _uow.GetRepository<OvertimeApplication>().Add(overtimeApplication);

                await _uow.CommitAsync();
                return new ResultDTO { Object = Mapper.Map<OvertimeApplicationViewModel>(overtimeApplication) };
            }

        }
        public async Task<ResultDTO> GetOvertimeApplicationById(Guid Id)
        {
            var resultDto = new ResultDTO();
            var overTime = _uow.GetRepository<OvertimeApplication>().GetSingle<OvertimeApplicationViewModel>(x => x.Id == Id);
            if (overTime == null)
            {
                resultDto.Messages.Add("ID IS IN VALID");
                resultDto.ErrorCodes.Add(0);
            }
            else
            {
                var groupSapCodes = overTime.OvertimeItems.GroupBy(x => x.SAPCode);
                var overtimeDetails = new List<OvertimeApplicationDetailViewModel>();
                foreach (var group in groupSapCodes)
                {
                    group.OrderBy(x => x.Date);
                    overtimeDetails.AddRange(group.ToList().OrderBy(x => x.Date));
                }
                foreach (var dtl in overtimeDetails)
                {
                    try
                    {
                        if (dtl.IsStore == null || !dtl.IsStore.HasValue)
                        {
                            string sapCode = (!string.IsNullOrEmpty(dtl.SAPCode) && overTime.Type == OverTimeType.ManagerApplyForEmployee) ? dtl.SAPCode : (!string.IsNullOrEmpty(overTime.UserSAPCode) && overTime.Type == OverTimeType.EmployeeSeftService) ? overTime.UserSAPCode : "" ;
                            if (!string.IsNullOrEmpty(sapCode))
                            {
                                var users = await _uow.GetRepository<User>(true).GetSingleAsync(x => x.SAPCode == sapCode);
                                if (users != null)
                                    dtl.IsStore = users.IsStore();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        resultDto.Messages = new List<string>() { e.Message + " - " + dtl.SAPCode };
                    }
                }

                overTime.OvertimeItems = overtimeDetails.OrderBy(x => x.SAPCode).ToList();
            }
            resultDto.Object = overTime;
            return resultDto;
        }

        public async Task<ResultDTO> GetOvertimeApplicationList(QueryArgs args)
        {
            var overTimes = await _uow.GetRepository<OvertimeApplication>()
                .FindByAsync<ItemListOvertimeApplicationViewModel>(args.Order, args.Page, args.Limit, args.Predicate, args.PredicateParameters);
            var count = await _uow.GetRepository<OvertimeApplication>().CountAsync(args.Predicate, args.PredicateParameters);
            return new ResultDTO { Object = new ArrayResultDTO { Data = overTimes, Count = count } };
        }

        public async Task<ResultDTO> GetOvertimeApplications(QueryArgs args)
        {
            var overTimes = await _uow.GetRepository<OvertimeApplication>().FindByAsync<OvertimeApplicationViewModel>(args.Order, args.Page, args.Limit, args.Predicate, args.PredicateParameters);
            var count = await _uow.GetRepository<OvertimeApplication>().CountAsync(args.Predicate, args.PredicateParameters);
            return new ResultDTO { Object = new ArrayResultDTO { Data = overTimes, Count = count } };
        }

        public async Task<ResultDTO> RejectOvertimeApplication(Guid Id)
        {
            throw new NotImplementedException();
        }

        public async Task<ResultDTO> RequestToChangeOvertimeApplication(Guid Id)
        {
            throw new NotImplementedException();
        }

        public async Task<ResultDTO> SubmitOvertimeApplication(Guid Id)
        {
            throw new NotImplementedException();
        }
        public async Task<byte[]> PrintFormOvertime(Guid Id)
        {
            byte[] result = null;
            var record = await _uow.GetRepository<OvertimeApplication>().FindByIdAsync(Id);
            if (record != null)
            {
                var dataToPrint = Mapper.Map<OvertimePrintFormViewModel>(record);
                var currentDepartment = await _uow.GetRepository<Department>().FindByAsync<DepartmentViewModel>(x => x.Code == dataToPrint.DepartmentCode);
                if (currentDepartment != null && currentDepartment.Any())
                {
                    dataToPrint.IsHQ = !currentDepartment.FirstOrDefault().IsStore;
                    dataToPrint.Grade = currentDepartment.FirstOrDefault().JobGradeGrade;
                }

                var detailItems = await _uow.GetRepository<OvertimeApplicationDetail>().FindByAsync<OvertimeApplicationDetaiForPrintlViewModel>(x => x.OvertimeApplicationId == record.Id, "Date asc");
                if (detailItems.Any())
                {
                    foreach (var item in detailItems)
                    {
                        item.ProposalHours = GetHours(item.ProposalHoursFrom, item.ProposalHoursTo, dataToPrint.IsHQ);
                        item.ActualHours = GetHours(item.ActualHoursFrom, item.ActualHoursTo, dataToPrint.IsHQ);

                    }
                }
                var properties = typeof(OvertimePrintFormViewModel).GetProperties();
                var pros = new Dictionary<string, string>();
                var tbHistories = await GetWorkFlowHistories(Id, ObjectToPrintFromType.Overtime, dataToPrint);
                foreach (var property in properties)
                {
                    var value = Convert.ToString(property.GetValue(dataToPrint));
                    pros[property.Name] = SecurityElement.Escape(value);
                }
                result = ExportXLS("Overtime.xlsx", pros, detailItems.ToList(), tbHistories);
            }
            return result;
        }
        //jira - 597
        
        //================================================================
        public async Task<List<Dictionary<string, string>>> GetWorkFlowHistories(Guid Id, OvertimePrintFormViewModel dataToPrint)
        {
            var wfStatus = await _workflowBO.GetWorkflowStatusByItemId(Id);
            var workflowStatus = (WorkflowStatusViewModel)wfStatus.Object;
            var tbPros = new List<Dictionary<string, string>>();
            if (workflowStatus != null && workflowStatus.WorkflowInstances != null)
            {
                var wfInstanceList = workflowStatus.WorkflowInstances.FirstOrDefault();
                UpdateAdditionInfo(wfInstanceList, ObjectToPrintFromType.Overtime, dataToPrint);
                if (wfInstanceList != null && wfInstanceList.Histories != null)
                {
                    foreach (var item in wfInstanceList.Histories)
                    {
                        if (item != null && item.IsStepCompleted)
                        {
                            var rowPros = new Dictionary<string, string>();
                            rowPros["AssignedDate"] = SecurityElement.Escape(item.Modified.ToString("dd/MM/yyyy"));
                            rowPros["AssignedBy"] = SecurityElement.Escape(item.ApproverFullName);
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
        public byte[] ExportXLS(string template, Dictionary<string, string> pros, List<OvertimeApplicationDetaiForPrintlViewModel> tbOTDetails, List<Dictionary<string, string>> tbHistories)
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
                    //range.Merge = true;  
                    InsertTargetData(worksheet, 10, tbOTDetails);
                    InsertWorkflowHistoriesData(worksheet, (26 + tbOTDetails.Count - 1), tbHistories);
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
                    pck.SaveAs(memoryStream);

                }
            }
            return memoryStream.ToArray();
        }
        private void InsertWorkflowHistoriesData(ExcelWorksheet worksheet, int styleRow, List<Dictionary<string, string>> tblPros)
        {
            var index = 0;
            var fromRow = styleRow + 1;
            var toRow = fromRow + tblPros.Count;
            for (int i = fromRow; i < toRow; i++)
            {
                var target = tblPros.ElementAt(index);
                worksheet.InsertRow(i, 1, styleRow);
                var row = worksheet.Row(i);
                row.Height = 26;
                // Style               
                //worksheet.Cells[$"B{i}"].Style.WrapText = true;
                //worksheet.Cells[$"B{i}"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                //worksheet.Cells[$"B{i}"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                //worksheet.Cells[$"B{i}"].AutoFitColumns();
                // Update Data
                ++index;
                worksheet.Cells[$"B{i}"].Value = target["AssignedBy"];
                worksheet.Cells[$"C{i}"].Value = target["AssignedDate"];
                worksheet.Cells[$"D{i}"].Value = target["Outcome"];
                worksheet.Cells[$"E{i}"].Value = target["Comment"];
            }
            worksheet.DeleteRow(styleRow);
        }
        private void InsertTargetData(ExcelWorksheet worksheet, int styleRow, List<OvertimeApplicationDetaiForPrintlViewModel> tblPros)
        {
            var index = 0;
            var fromRow = styleRow + 1;
            var toRow = fromRow + tblPros.Count;
            for (int i = fromRow; i < toRow; i++)
            {
                var otDetail = tblPros.ElementAt(index);
                worksheet.InsertRow(i, 1, styleRow);
                var row = worksheet.Row(i);
                row.Height = 26;
                // Style               
                //worksheet.Cells[$"B{i}"].Style.WrapText = true;
                //worksheet.Cells[$"B{i}"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                //worksheet.Cells[$"B{i}"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                //worksheet.Cells[$"B{i}"].AutoFitColumns();
                // Update Data
                worksheet.Cells[$"A{i}"].Value = ++index;
                worksheet.Cells[$"B{i}"].Value = otDetail.FullName;
                worksheet.Cells[$"C{i}"].Value = otDetail.SAPCode;
                worksheet.Cells[$"D{i}"].Value = otDetail.Date.ToString("dd/MM/yyyy");
                worksheet.Cells[$"E{i}"].Value = otDetail.ProposalHours;
                worksheet.Cells[$"F{i}"].Value = otDetail.ProposalHoursFrom;
                worksheet.Cells[$"G{i}"].Value = otDetail.ProposalHoursTo;
                worksheet.Cells[$"I{i}"].Value = otDetail.ActualHours;
                worksheet.Cells[$"J{i}"].Value = otDetail.ActualHoursFrom;
                worksheet.Cells[$"K{i}"].Value = otDetail.ActualHoursTo;
                worksheet.Cells[$"L{i}"].Value = otDetail.DateOffInLieu ? "x" : "";
            }
            worksheet.DeleteRow(styleRow);
        }
        private string GetHours(string from, string to, bool isHQ)
        {
            var result = "";
            if (!string.IsNullOrEmpty(to) && !string.IsNullOrEmpty(from))
            {
                //ngan
                if (DateTime.Parse(from) > DateTime.Parse(to))
                {
                    var tempHours = (DateTime.Parse(to).AddDays(1) - DateTime.Parse(from)).TotalMinutes / 60;
                    result = tempHours.ToString();
                }
                else
                {
                    result = ((DateTime.Parse(to) - DateTime.Parse(from)).TotalMinutes / 60).ToString();
                }
            }
            if (isHQ && float.Parse(result) >= 5)
            {
                result = (float.Parse(result) - 1).ToString();
            }
            return result;
        }
        #endregion

        #region Import
        public async Task<ResultDTO> UploadDataForOvertime(OvertimeQuerySAPCodeArg arg, Stream stream)
        {
            var result = new ResultDTO();
            var data = new ArrayResultDTO();
            var errorSAPCode = new List<string>();
            List<OvertimeApplicationDetailArgs> overtimeDetails = new List<OvertimeApplicationDetailArgs>();

            var users = await _uow.GetRepository<User>().FindByAsync<UserForTreeViewModel>(x => arg.SAPCodes.Contains(x.SAPCode) && !x.IsDeleted && x.IsActivated);

            var dataFromFile = ReadDataFromStream(stream, users);
            if (dataFromFile.Data.Count > 0)
            {
                var groupSapcodes = dataFromFile.Data.GroupBy(x => x.SAPCode);
                foreach (var group in groupSapcodes)
                {
                    foreach (var item in group)
                    {
                        if (string.IsNullOrWhiteSpace(item.SAPCode))
                        {
                            result.ErrorCodes.Add(1006);
                            result.Messages.Add(item.LineOfExcel);
                        }
                        else
                        {
                            //Fixed #200
                            //  if (item.DateOfOT == null || !Utilities.IsValidDatetimeFormat(item.DateOfOT.ToString()))
                            if (item.DateOfOT == null)
                            {
                                result.ErrorCodes.Add(1003);
                                result.Messages.Add(item.LineOfExcel);
                            }
                            else
                            {
                                var arrFrom = item.From.Split(':');
                                int hourFrom = -1;
                                if (arrFrom.Length > 1)
                                {
                                    hourFrom = Convert.ToInt32(arrFrom[0]);
                                }

                                if (arrFrom.Length <= 1 || (arrFrom[1] != "30" && arrFrom[1] != "00") || !Utilities.IsValidTimeFormat(item.From) || (hourFrom >= 25 || hourFrom < 0))
                                {
                                    result.ErrorCodes.Add(1002);
                                    result.Messages.Add(item.LineOfExcel);
                                }
                                else
                                {
                                    var arrTo = item.From.Split(':');
                                    int hourTo = -1;
                                    if (arrFrom.Length > 1)
                                    {
                                        hourTo = Convert.ToInt32(arrTo[0]);
                                    }
                                    if (arrTo.Length <= 1 || (arrTo[1] != "30" && arrTo[1] != "00") || !Utilities.IsValidTimeFormat(item.To) || (hourTo >= 25 || hourTo < 0))
                                    {
                                        result.ErrorCodes.Add(1005);
                                        result.Messages.Add(item.LineOfExcel);
                                    }
                                    else
                                    {
                                        var currentUser = users.Where(x => x.SAPCode == item.SAPCode).FirstOrDefault();
                                        if (currentUser != null)
                                        {
                                            if (item.DateOfInLieu == "1")
                                            {
                                                item.DateOfInLieu = "true";
                                            }
                                            else
                                            {
                                                item.DateOfInLieu = "false";
                                            }

                                            bool isNotOT = false;
                                            bool.TryParse(item.IsNoOT, out isNotOT);
                                            overtimeDetails.Add(new OvertimeApplicationDetailArgs
                                            {
                                                SAPCode = item.SAPCode,
                                                FullName = currentUser.FullName,
                                                DateOffInLieu = bool.Parse(item.DateOfInLieu),
                                                IsNoOT = isNotOT,
                                                ProposalHoursFrom = item.From,
                                                ProposalHoursTo = item.To,
                                                // Date =  Utilities.ConvertStringToDatetime(item.DateOfOT.ToString()).ToString("yyyy-MM-dd"),
                                                //Fixed #200
                                                Date = item.DateOfOT?.ToString("yyyy-MM-dd"),
                                                UserId = currentUser.Id,
                                                Department = currentUser.Department
                                            });

                                            if (!Utilities.IsValidTimeFormat(item.From) || !Utilities.IsValidTimeFormat(item.To))
                                            {
                                                result.ErrorCodes.Add(1002);
                                                result.Messages.Add(item.LineOfExcel);
                                            }
                                        }
                                        else
                                        {
                                            result.ErrorCodes.Add(1004);
                                            result.Messages.Add(item.LineOfExcel);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                data.Data = overtimeDetails;
                data.Count = overtimeDetails.Count;
            }
            else
            {
                result.ErrorCodes.Add(1001);
                result.Messages.Add("OVERTIME_APPLICATION_VALIDATE_IMPORT_FILE");
            }
            result.Object = data;
            return result;
        }
        public async Task<ResultDTO> UploadActualDataForOvertime(OvertimeQuerySAPCodeArg arg, Stream stream)
        {
            var result = new ResultDTO();
            var data = new ArrayResultDTO();
            var errorSAPCode = new List<string>();
            List<OvertimeApplicationDetailArgs> overtimeDetails = new List<OvertimeApplicationDetailArgs>();

            var users = await _uow.GetRepository<User>().FindByAsync<UserForTreeViewModel>(x => arg.SAPCodes.Contains(x.SAPCode));

            var dataFromFile = ReadActualDataFromStream(stream, users);
            if (dataFromFile.Data.Count > 0)
            {

                Func<string, string, bool> CheckTimeIsValid = (string strTime, string lineNumber) =>
                {
                    bool checkTimeResult = true;
                    try
                    {
                        var arrTime = strTime.Split(':');
                        int hour = -1;
                        if (arrTime.Length > 1)
                        {
                            hour = Convert.ToInt32(arrTime[0]);
                        }
                        if (arrTime.Length <= 1 || (arrTime[1] != "30" && arrTime[1] != "00") || !Utilities.IsValidTimeFormat(strTime) || (hour >= 25 || hour < 0))
                        {
                            result.ErrorCodes.Add(1005);
                            result.Messages.Add(lineNumber);
                        }
                    }
                    catch
                    {
                        checkTimeResult = false;
                    }
                    return checkTimeResult;
                };

                var groupSapcodes = dataFromFile.Data.GroupBy(x => x.SAPCode);
                foreach (var group in groupSapcodes)
                {
                    foreach (var item in group)
                    {
                        if (string.IsNullOrWhiteSpace(item.SAPCode))
                        {
                            result.ErrorCodes.Add(1006);
                            result.Messages.Add(item.LineOfExcel);
                        }
                        else
                        {
                            //Fixed #200
                            //  if (item.DateOfOT == null || !Utilities.IsValidDatetimeFormat(item.DateOfOT.ToString()))
                            if (item.DateOfOT == null)
                            {
                                result.ErrorCodes.Add(1003);
                                result.Messages.Add(item.LineOfExcel);
                            }
                            else
                            {
                                List<bool> checkTime = new List<bool>();
                                checkTime.Add(CheckTimeIsValid(item.From, item.LineOfExcel));
                                checkTime.Add(CheckTimeIsValid(item.To, item.LineOfExcel));
                                checkTime.Add(CheckTimeIsValid(item.ActualFrom, item.LineOfExcel));
                                checkTime.Add(CheckTimeIsValid(item.ActualTo, item.LineOfExcel));

                                if(!checkTime.Contains(false))
                                {
                                    var currentUser = users.Where(x => x.SAPCode == item.SAPCode).FirstOrDefault();
                                    if (currentUser != null)
                                    {
                                        if (item.DateOfInLieu == "1")
                                        {
                                            item.DateOfInLieu = "true";
                                        }
                                        else
                                        {
                                            item.DateOfInLieu = "false";
                                        }

                                        if (item.IsNoOT == "1")
                                        {
                                            item.IsNoOT = "true";
                                        }
                                        else
                                        {
                                            item.IsNoOT = "false";
                                        }

                                        bool isNotOT = false;
                                        bool.TryParse(item.IsNoOT, out isNotOT);
                                        OvertimeApplicationDetailArgs current_OT_Detail = new OvertimeApplicationDetailArgs
                                        {
                                            SAPCode = item.SAPCode,
                                            FullName = currentUser.FullName,
                                            DateOffInLieu = bool.Parse(item.DateOfInLieu),
                                            IsNoOT = isNotOT,
                                            ProposalHoursFrom = item.From,
                                            ProposalHoursTo = item.To,
                                            ActualHoursFrom = item.ActualFrom,
                                            ActualHoursTo = item.ActualTo,
                                            Date = item.DateOfOT?.ToString("yyyy-MM-dd"),
                                            UserId = currentUser.Id,
                                            Department = currentUser.Department
                                        };
                                        bool dataNotValid = (string.IsNullOrEmpty(current_OT_Detail.SAPCode) || string.IsNullOrEmpty(current_OT_Detail.Date) || string.IsNullOrEmpty(current_OT_Detail.ActualHoursFrom) || string.IsNullOrEmpty(current_OT_Detail.ActualHoursTo));

                                        if (!dataNotValid)
                                        {
                                            overtimeDetails.Add(current_OT_Detail);
                                        }

                                        if (!Utilities.IsValidTimeFormat(item.From) || !Utilities.IsValidTimeFormat(item.To))
                                        {
                                            result.ErrorCodes.Add(1002);
                                            result.Messages.Add(item.LineOfExcel);
                                        }
                                    }
                                    else
                                    {
                                        result.ErrorCodes.Add(1004);
                                        result.Messages.Add(item.LineOfExcel);
                                    }
                                }
                            }
                        }
                    }
                }
                data.Data = overtimeDetails;
                data.Count = overtimeDetails.Count;
            }
            else
            {
                result.ErrorCodes.Add(1001);
                result.Messages.Add("OVERTIME_APPLICATION_VALIDATE_IMPORT_FILE");
            }
            result.Object = data;
            return result;
        }
        public OvertimeApplicationImportFileDTO ReadDataFromStream(Stream stream, IEnumerable<UserForTreeViewModel> users)
        {
            OvertimeApplicationImportFileDTO result = new OvertimeApplicationImportFileDTO();
            using (var pck = new ExcelPackage(stream))
            {
                ExcelWorkbook WorkBook = pck.Workbook;
                ExcelWorksheet workSheet = WorkBook.Worksheets.ElementAt(0);
                var listData = new List<OvertimeApplicationImportDetailDTO>();
                for (var rowNumber = 4; rowNumber <= workSheet.Dimension.End.Row; rowNumber++)
                {
                    OvertimeApplicationImportDetailDTO data = new OvertimeApplicationImportDetailDTO();
                    var objdate = workSheet.Cells[rowNumber, 4, rowNumber, workSheet.Dimension.End.Column][$"B{rowNumber}"];
                    //Fixed #200
                    string dateValueFormat = objdate is null ? string.Empty : objdate.Text;

                   /* if (!string.IsNullOrEmpty(dateValueFormat))
                    {
                        DateTime dateValue;
                        if (DateTime.TryParse(dateValueFormat, out dateValue))
                        {
                            dateValueFormat = dateValue.ToString("dd/MM/yyyy");
                        }
                    }*/
                    if (Utilities.IsValidDatetimeFormat(dateValueFormat))
                    {
                        // data.DateOfOT = DateTimeOffset.Parse(objdate.Text.ToString());
                        data.DateOfOT = Utilities.ConvertStringToDatetime(dateValueFormat);

                    }
                    data.LineOfExcel = rowNumber.ToString();
                    data.SAPCode = workSheet.Cells[rowNumber, 2, rowNumber, workSheet.Dimension.End.Column][$"A{rowNumber}"].Text.ToSAPFormatString();
                    data.From = workSheet.Cells[rowNumber, 5, rowNumber, workSheet.Dimension.End.Column][$"C{rowNumber}"].Text;
                    data.To = workSheet.Cells[rowNumber, 6, rowNumber, workSheet.Dimension.End.Column][$"D{rowNumber}"].Text;
                    data.DateOfInLieu = workSheet.Cells[rowNumber, 6, rowNumber, workSheet.Dimension.End.Column][$"E{rowNumber}"].Text;
                    listData.Add(data);
                }
                result.Data = listData;
            }
            return result;
        }
        public OvertimeApplicationImportActualFileDTO ReadActualDataFromStream(Stream stream, IEnumerable<UserForTreeViewModel> users)
        {
            OvertimeApplicationImportActualFileDTO result = new OvertimeApplicationImportActualFileDTO();
            using (var pck = new ExcelPackage(stream))
            {
                ExcelWorkbook WorkBook = pck.Workbook;
                ExcelWorksheet workSheet = WorkBook.Worksheets.ElementAt(0);
                var listData = new List<OvertimeApplicationImportActualDetailDTO>();
                for (var rowNumber = 2; rowNumber <= workSheet.Dimension.End.Row; rowNumber++)
                {
                    OvertimeApplicationImportActualDetailDTO data = new OvertimeApplicationImportActualDetailDTO();
                    DateTime dateOfOT = DateTime.MinValue;
                    DateTime.TryParseExact(workSheet.Cells[rowNumber, 3].Text, "dd/MM/yyyy", null, DateTimeStyles.None, out dateOfOT);

                    data.LineOfExcel = rowNumber.ToString();
                    data.SAPCode = workSheet.Cells[rowNumber,1].Text.ToSAPFormatString();
                    data.FullName = workSheet.Cells[rowNumber, 2].Text;
                    if(dateOfOT != DateTime.MinValue)
                    {
                        data.DateOfOT = new DateTimeOffset(dateOfOT);
                    }
                    data.From = workSheet.Cells[rowNumber, 4].Text;
                    data.To = workSheet.Cells[rowNumber, 5].Text;
                    data.ActualFrom = workSheet.Cells[rowNumber, 6].Text;
                    data.ActualTo = workSheet.Cells[rowNumber, 7].Text;
                    data.DateOfInLieu = workSheet.Cells[rowNumber, 8].Text;
                    data.IsNoOT = workSheet.Cells[rowNumber, 9].Text;

                    if(string.IsNullOrEmpty(data.SAPCode) || data.DateOfOT == null || string.IsNullOrEmpty(data.ActualFrom) || string.IsNullOrEmpty(data.ActualTo))
                    {
                        continue;
                    }
                    listData.Add(data);
                }
                result.Data = listData;
            }
            return result;
        }
        #endregion
    }
}