using AutoMapper;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using Aeon.HR.Data.Models;
using Aeon.HR.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Aeon.HR.Infrastructure.Enums;
using System.Globalization;
using System.Collections.Generic;
using System.Security;
using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.BusinessObjects.DataHandlers;
using Aeon.HR.ViewModels.PrintFormViewModel;
using System.Text.RegularExpressions;
using OfficeOpenXml;
using TargetPlanTesting.ImportData;
using Newtonsoft.Json;
using static Aeon.HR.ViewModels.UpdateApprovalWorkflowViewModel;
using Aeon.HR.BusinessObjects.CompleteActions;
using System.Data;
using System.IO;
using Aeon.HR.BusinessObjects.Handlers.FIle;
using System.Configuration;
using System.Net;
using System.Diagnostics;

namespace Aeon.HR.BusinessObjects.Handlers
{
    public partial class RecruitmentBO
    {
        private string _uploadedFilesFolder = System.Web.Hosting.HostingEnvironment.MapPath("~/Attachments");
        private string _domainRTH = ConfigurationManager.AppSettings["Domain_ImportRTH"];
        private string _userNameRTH = ConfigurationManager.AppSettings["UserrName_ImportRTH"];
        private string _passwordRTH = ConfigurationManager.AppSettings["Password_ImportRTH"];
        private static NetworkCredential credentials = null;
        public async Task<ResultDTO> CreateRequestToHire(RequestToHireDataForCreatingArgs data)
        {
            var existRequestToHire = await _uow.GetRepository<RequestToHire>().FindByAsync(x => x.Id == data.Id);
            if (existRequestToHire.Any())
            {
                return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Request To Hire is exist" } };
            }
            else
            {

                var requestToHire = Mapper.Map<RequestToHire>(data);
                if (requestToHire.ReplacementFor == TypeOfNeed.ReplacementFor)
                {
                    requestToHire.DeptDivisionId = requestToHire.ReplacementForId;
                    requestToHire.JobGradeCaption = requestToHire.ReplacementForGrade.HasValue ? requestToHire.ReplacementForGrade.Value.ToString() : "";
                }
                if (requestToHire.DeptDivisionId.HasValue)
                {
                    var dept = DbHelper.LoadDept(_uow, requestToHire.DeptDivisionId.Value);
                    requestToHire.DeptCode = dept.Code;
                    requestToHire.DeptName = dept.Name;
                }
                //await UpdateAdditionalInfo(requestToHire);
                _uow.GetRepository<RequestToHire>().Add(requestToHire);
                await _uow.CommitAsync();

                return new ResultDTO { Object = Mapper.Map<RequestToHireViewModel>(requestToHire) };
            }
        }

        public async Task<ArrayResultDTO> GetListRequestToHires(QueryArgs args)
        {
            /*var requestToHires = await _uow.GetRepository<RequestToHire>().FindByAsync<RequestToHireViewModel>(args.Order, args.Page, args.Limit, args.Predicate, args.PredicateParameters);
            foreach (var item in requestToHires)
            {
                var getDepartment = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == item.DeptDivisionId);
                if (getDepartment != null && getDepartment.RegionId != null)
                {
                    item.RegionName = getDepartment.Region.RegionName;
                }
            }
            var count = await _uow.GetRepository<RequestToHire>().CountAsync(args.Predicate, args.PredicateParameters);
            var result = new ArrayResultDTO { Data = requestToHires, Count = count };
            return result;*/
            if (args != null)
            {
                if (args.Object != null)
                {
                    var requestToHires = await _uow.GetRepository<RequestToHire>().FindByAsync<RequestToHireViewModel>(args.Predicate, args.PredicateParameters);
                    var requestToHires2 = (await _uow.GetRepository<RequestToHire>().FindByAsync<RequestToHireViewModel>(args.Predicate, args.PredicateParameters)).Select(x => x.Id).ToList();
                    var count = await _uow.GetRepository<RequestToHire>().CountAsync(args.Predicate, args.PredicateParameters);

                    var sapCodes = new List<string>();
                    try
                    {
                        // them de quy khi search department
                        Guid departmentId = new Guid((string)args.Object);
                        HashSet<Guid?> depChilds = new HashSet<Guid?>();
                        await getListDepartmentChild(departmentId, depChilds);
                        count = requestToHires.Where(x => depChilds.Contains(x.DeptDivisionId)).Count();
                        requestToHires = (await _uow.GetRepository<RequestToHire>().FindByAsync<RequestToHireViewModel>(x => requestToHires2.Contains(x.Id) && depChilds.Contains(x.DeptDivisionId), args.Order)).Skip((args.Page - 1) * args.Limit).Take(args.Limit);
                    }
                    catch (Exception e)
                    {
                        requestToHires = (await _uow.GetRepository<RequestToHire>().FindByAsync<RequestToHireViewModel>(x => requestToHires2.Contains(x.Id), args.Order)).Skip((args.Page - 1) * args.Limit).Take(args.Limit);
                    }

                    if (requestToHires.Any())
                    {
                        foreach (var item in requestToHires)
                        {
                            var getDepartment = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == item.DeptDivisionId);
                            if (getDepartment != null && getDepartment.RegionId != null)
                            {
                                item.RegionName = getDepartment.Region.RegionName;
                            }
                        }
                    }
                    return new ArrayResultDTO { Data = requestToHires, Count = count };
                }
                else
                {
                    // old code
                    var requestToHires = await _uow.GetRepository<RequestToHire>().FindByAsync<RequestToHireViewModel>(args.Order, args.Page, args.Limit, args.Predicate, args.PredicateParameters);
                    foreach (var item in requestToHires)
                    {
                        var getDepartment = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == item.DeptDivisionId);
                        if (getDepartment != null && getDepartment.RegionId != null)
                        {
                            item.RegionName = getDepartment.Region.RegionName;
                        }
                    }
                    var count = await _uow.GetRepository<RequestToHire>().CountAsync(args.Predicate, args.PredicateParameters);
                    var result = new ArrayResultDTO { Data = requestToHires, Count = count };
                    return new ArrayResultDTO { Data = requestToHires, Count = count };
                }
            }
            return new ArrayResultDTO { Data = null, Count = 0 };
        }

        public async Task<ResultDTO> GetListDetailRequestToHire(RequestToHireDataForCreatingArgs data)
        {
            var requestToHire = await _uow.GetRepository<RequestToHire>().FindByAsync<RequestToHireViewModel>(x => x.Id == data.Id);
            if (requestToHire.Any())
            {
                return new ResultDTO { Object = requestToHire };
            }
            return new ResultDTO { };
        }

        public async Task<ResultDTO> SearchListRequestToHire(RequestToHireDataForCreatingArgs data)
        {
            var requestToHires = await _uow.GetRepository<RequestToHire>().FindByAsync<RequestToHireViewModel>(x => (x.PositionName.Contains(data.PositionName)) || (x.LocationName.Contains(data.LocationName)) && x.DeptDivisionId == data.DeptDivisionId);
            if (requestToHires.Any())
            {
                return new ResultDTO { Object = requestToHires };
            }
            return new ResultDTO { };

        }

        public async Task<ResultDTO> DeleteRequestToHire(RequestToHireDataForCreatingArgs data)
        {
            var requestToHire = await _uow.GetRepository<RequestToHire>().GetSingleAsync(x => x.Id == data.Id);
            if (requestToHire == null)
            {
                return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "RequestToHire is not exist" } };
            }
            else
            {
                _uow.GetRepository<RequestToHire>().Delete(requestToHire);
                await _uow.CommitAsync();
            }
            return new ResultDTO { };
        }

        public async Task<ResultDTO> UpdateRequestToHire(RequestToHireDataForCreatingArgs data)
        {
            var isExist = await _uow.GetRepository<RequestToHire>().AnyAsync(x => x.Id == data.Id);
            if (!isExist)
            {
                return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "RequestToHire is not exist" } };
            }
            var existRequestToHire = Mapper.Map<RequestToHire>(data);
            // await UpdateAdditionalInfo(existRequestToHire);
            if (existRequestToHire.ReplacementFor == TypeOfNeed.ReplacementFor)
            {
                existRequestToHire.DeptDivisionId = existRequestToHire.ReplacementForId;
                existRequestToHire.JobGradeCaption = $"G{existRequestToHire.ReplacementForGrade}";
            }

            existRequestToHire.RecruitmentCategory = null;
            if (existRequestToHire.DeptDivisionId.HasValue)
            {
                var dept = DbHelper.LoadDept(_uow, existRequestToHire.DeptDivisionId.Value);
                if (dept != null)
                {
                    existRequestToHire.DeptCode = dept.Code;
                    existRequestToHire.DeptName = dept.Name;
                }
            }
            _uow.GetRepository<RequestToHire>().Update(existRequestToHire);
            await _uow.CommitAsync();
            return new ResultDTO { Object = Mapper.Map<RequestToHireViewModel>(existRequestToHire) };
        }

        private async Task UpdateRecruitmentLeaderFromRecruitmentStaffId(RequestToHire requestToHire, int maxGradeLeader)
        {
            if (requestToHire.RecruitmentStaffId.HasValue)
            {
                var currentRecruitmentStaff = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == requestToHire.RecruitmentStaffId.Value && x.IsHR); // Check Is Hr va chua check IHr ?
                if (currentRecruitmentStaff.JobGrade.Grade == maxGradeLeader)
                {
                    requestToHire.RecruitmentLeaderId = currentRecruitmentStaff.Id;
                }
                else
                {
                    var leaderDepartment = await _uow.GetRepository<Department>().GetSingleAsync(x => x.ParentId == requestToHire.RecruitmentStaffId.Value && x.IsHR && x.JobGrade.Grade == maxGradeLeader);
                    if (leaderDepartment != null)
                    {
                        requestToHire.RecruitmentLeaderId = leaderDepartment.Id;
                    }
                    else
                    {
                        requestToHire.RecruitmentLeaderId = requestToHire.RecruitmentStaffId;
                    }
                }
            }
        }
        private async Task GetRecruitmentStaffForOverG2(RequestToHire requestToHire, Department department, int maxGradeStaff, int maxGradeLeader, int g6Grade)
        {
            if (department.JobGrade.Grade == maxGradeStaff)
            {
                var g6Department = department;
                if (g6Department != null)
                {
                    if (requestToHire.RecruitmentStaffId.HasValue)
                    {
                        requestToHire.RecruitmentStaffId = g6Department.HrDepartmentId;
                        requestToHire.RecruitmentManagerId = g6Department.HrDepartmentId;
                        await UpdateRecruitmentLeaderFromRecruitmentStaffId(requestToHire, maxGradeLeader);
                    }

                }
            }
            else if (department.JobGrade.Grade < maxGradeStaff)
            {
                var grade = department.ParentId.HasValue ? department.JobGrade.Grade : 0;
                var parentId = department.ParentId;
                while (grade > 0 && grade <= maxGradeStaff && parentId.HasValue)
                {
                    var parent = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == parentId);
                    if (parent != null)
                    {
                        if (parent.JobGrade.Grade == g6Grade)
                        {
                            if (parent.HrDepartmentId.HasValue)
                            {
                                requestToHire.RecruitmentStaffId = parent.HrDepartmentId.Value;
                                requestToHire.RecruitmentManagerId = parent.HrDepartmentId.Value;
                                await UpdateRecruitmentLeaderFromRecruitmentStaffId(requestToHire, maxGradeLeader);
                            }
                            grade = maxGradeStaff + 1; // Exit while loop
                        }
                        else if (parent.JobGrade.Grade != maxGradeStaff)
                        {
                            if (parent.JobGrade.Grade > maxGradeStaff)
                            {
                                if (parent.HrDepartmentId.HasValue)
                                {
                                    requestToHire.RecruitmentStaffId = parent.HrDepartmentId.Value;
                                    requestToHire.RecruitmentManagerId = parent.HrDepartmentId.Value;
                                    await UpdateRecruitmentLeaderFromRecruitmentStaffId(requestToHire, maxGradeLeader);
                                }

                            }
                            else
                            {
                                parentId = parent.ParentId.Value;
                                grade = parent.JobGrade.Grade;
                            }
                        }
                    }
                    else
                    {
                        grade = maxGradeStaff + 1; // Exit while loop
                    }

                }

            }

        }
        private async Task GetRecruitmentStaffForG1G2(RequestToHire requestToHire, Department department, int maxGradeStaff, int maxGradeLeader)
        {
            if (department.JobGrade.Grade == maxGradeStaff)
            {
                if (department.HrDepartmentId.HasValue)
                {
                    requestToHire.RecruitmentStaffId = department.HrDepartmentId.Value;
                    requestToHire.RecruitmentManagerId = department.HrDepartmentId.Value;
                    await UpdateRecruitmentLeaderFromRecruitmentStaffId(requestToHire, maxGradeLeader);
                }

            }
            else
            {
                var grade = department.ParentId.HasValue ? department.JobGrade.Grade : 0;
                var parentId = department.ParentId;
                while (grade > 0 && grade <= maxGradeStaff && parentId.HasValue)
                {
                    var parent = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == parentId);
                    if (parent != null)
                    {
                        if (parent.JobGrade.Grade == maxGradeStaff)
                        {
                            if (parent.HrDepartmentId.HasValue)
                            {
                                requestToHire.RecruitmentStaffId = parent.HrDepartmentId.Value;
                                requestToHire.RecruitmentManagerId = parent.HrDepartmentId.Value;
                                await UpdateRecruitmentLeaderFromRecruitmentStaffId(requestToHire, maxGradeLeader);
                            }
                            grade = maxGradeStaff + 1; // exit while loop
                        }
                        else if (parent.JobGrade.Grade != maxGradeStaff)
                        {
                            if (parent.JobGrade.Grade > maxGradeStaff)
                            {
                                if (parent.HrDepartmentId.HasValue)
                                {
                                    requestToHire.RecruitmentStaffId = parent.HrDepartmentId.Value;
                                    requestToHire.RecruitmentManagerId = parent.HrDepartmentId.Value;
                                    await UpdateRecruitmentLeaderFromRecruitmentStaffId(requestToHire, maxGradeLeader);
                                }
                            }
                            else
                            {
                                parentId = parent.ParentId.Value;
                                grade = parent.JobGrade.Grade;
                            }
                        }
                    }
                    else
                    {
                        grade = maxGradeStaff + 1;
                        // exit while loop
                    }


                }
            }

        }
        public async Task<byte[]> PrintRequestToHire(Guid Id)
        {
            byte[] result = null;
            var res = await _uow.GetRepository<RequestToHire>().FindByIdAsync(Id);
            var record = Mapper.Map<RequestToHireForPrintViewModel>(res);
            if (record != null)
            {
                var dataToPrint = Mapper.Map<RequestToHireForPrintViewModel>(record);
                if (record.ContractTypeCode == "FT")
                {
                    dataToPrint.CheckedBoxs.Add("checkbox1");
                }
                else if (record.ContractTypeCode == "PT")
                {
                    dataToPrint.CheckedBoxs.Add("checkbox2");
                }
                if (record.WorkingTimeCode == "HQ")
                {
                    dataToPrint.CheckedBoxs.Add("checkbox3");
                }
                else if (record.WorkingTimeCode == "STORE")
                {
                    dataToPrint.CheckedBoxs.Add("checkbox4");
                }
                if (record.ReplacementFor == TypeOfNeed.NewPosition)
                {
                    dataToPrint.CheckedBoxs.Add("checkbox5");
                    if (!string.IsNullOrEmpty(record.DepartmentName))
                    {
                        dataToPrint.Position = record.DepartmentName;
                    }
                }
                else if (record.ReplacementFor == TypeOfNeed.ReplacementFor)
                {
                    dataToPrint.CheckedBoxs.Add("checkbox6");
                    if (!string.IsNullOrEmpty(record.ReplacementForName))
                    {
                        dataToPrint.Position = record.ReplacementForName;
                    }
                }
                if (record.HasBudget == CheckBudgetOption.Budget)
                {
                    dataToPrint.CheckedBoxs.Add("checkbox7");
                }
                else if (record.HasBudget == CheckBudgetOption.Non_Budget)
                {
                    dataToPrint.CheckedBoxs.Add("checkbox8");
                }
                var tbPros = await GetWorkFlowHistories(Id, ObjectToPrintFromType.RequestToHire, dataToPrint);
                CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
                var salaryFrom = record.EstimateSalaryStart.ToString("#,###", cul.NumberFormat);
                var salaryTo = record.EstimateSalaryEnd.ToString("#,###", cul.NumberFormat);
                if (!string.IsNullOrEmpty(salaryFrom) && !string.IsNullOrEmpty(salaryTo))
                {
                    dataToPrint.SalaryFromTo = string.Format("{0}-{1}", salaryFrom, salaryTo);
                }
                else if (!string.IsNullOrEmpty(salaryFrom))
                {
                    dataToPrint.SalaryFromTo = string.Format("From {0}", salaryFrom);
                }
                var fromDate = record.FromDate.HasValue && record.FromDate != DateTimeOffset.FromUnixTimeSeconds(0) ? record.FromDate.Value.ToLocalTime().ToString("dd/MM/yyyy") : "";
                var toDate = record.ToDate.HasValue && record.FromDate != DateTimeOffset.FromUnixTimeSeconds(0) ? record.ToDate.Value.ToLocalTime().ToString("dd/MM/yyyy") : "";

                if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(toDate))
                {
                    dataToPrint.RequiredTime = string.Format("From {0} to {1}", fromDate, toDate);
                }
                else if (!string.IsNullOrEmpty(fromDate))
                {
                    dataToPrint.RequiredTime = string.Format("From {0}", fromDate);
                }
                if (res.Operation == OperationOptions.HQ)
                {
                    dataToPrint.HQOperation = "HQ";
                }
                else if (res.Operation == OperationOptions.Store)
                {
                    dataToPrint.HQOperation = "Store";
                }


                #region Task #10671 get title instead grade 
                if (record.JobGradeId.HasValue)
                {
                    var newJobgrade = await _uow.GetRepository<JobGrade>().GetSingleAsync(x => x.Id == record.JobGradeId.Value);
                    if (!(newJobgrade is null))
                        dataToPrint.JobGradeTitle = !string.IsNullOrEmpty(newJobgrade.Title) ? newJobgrade.Title : "";
                }
                #endregion

                var properties = typeof(RequestToHireForPrintViewModel).GetProperties();
                var pros = new Dictionary<string, string>();
                foreach (var property in properties)
                {
                    var value = Convert.ToString(property.GetValue(dataToPrint));
                    pros[property.Name] = SecurityElement.Escape(value);
                }
                result = WordAutomation.ExportPDF("RequestToHire.docx", pros, tbPros, dataToPrint.CheckedBoxs);
            }
            return result;
        }


        public async Task<ResultDTO> GetResignationApplicantionCompletedBySapCode(string sapcode)
        {
            var resignationApplications = await _uow.GetRepository<ResignationApplication>(true)
            .FindByAsync<ResignationApplicantGridViewModel>(x => x.UserSAPCode == sapcode && x.Status == "Completed");

            return new ResultDTO { Object = new ArrayResultDTO { Data = resignationApplications, Count = resignationApplications.Count() } };
        }

        public async Task getListDepartmentChild(Guid divisionId, HashSet<Guid?> ids)
        {
            try
            {
                ids.Add(divisionId);
                var deptIds = (await _uow.GetRepository<Department>().FindByAsync(x => x.ParentId == divisionId)).ToList().Select(x => x.Id);
                if (deptIds != null)
                {
                    foreach (var itemId in deptIds)
                    {
                        if (!ids.Contains(itemId))
                        {
                            ids.Add(itemId);
                            await getListDepartmentChild(itemId, ids);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ids = new HashSet<Guid?>();
            }
        }

        public async Task<byte[]> DownloadTemplateImportRequestToHire(DataToImportRequestToHireTemplateArgs args)
        {
            var dataToPrint = new TargetPlanPrintFormViewModel();
            var items = new List<TargetPlanDetailPrintFormViewModel>();

            byte[] result = null;
            if (dataToPrint != null)
            {
                try
                {
                    var pros = new Dictionary<string, string>();
                    result = ExportXLS("REQ_Template.xlsx", pros, items);
                }
                catch (Exception ex)
                {
                    //_logger.LogError("Download Template {0}", string.IsNullOrEmpty(ex.Message) ? ex.InnerException.Message : ex.Message);
                }
            }
            return result;
        }

        public byte[] ExportXLS(string template, Dictionary<string, string> pros, List<TargetPlanDetailPrintFormViewModel> tbTPDetail)
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
                    //WorkBook.CalcMode = ExcelCalcMode.Manual;

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

        public List<RequestToHireFromImportDetailDTO> ReadBySheet(ExcelWorksheet workSheet)
        {
            var listData = new List<RequestToHireFromImportDetailDTO>();
            for (var rowNumber = 2; rowNumber <= workSheet.Dimension.End.Row; rowNumber++)
            {
                RequestToHireFromImportDetailDTO data = new RequestToHireFromImportDetailDTO();
                data.ID = workSheet.Cells[rowNumber, 2, rowNumber, workSheet.Dimension.End.Column][$"A{rowNumber}"].Text.Trim();
                data.PositionCode = workSheet.Cells[rowNumber, 3, rowNumber, workSheet.Dimension.End.Column][$"B{rowNumber}"].Text.Trim();
                data.TypeOfNeed = workSheet.Cells[rowNumber, 4, rowNumber, workSheet.Dimension.End.Column][$"C{rowNumber}"].Text;
                data.CheckBudget = workSheet.Cells[rowNumber, 5, rowNumber, workSheet.Dimension.End.Column][$"D{rowNumber}"].Text;
                data.Quantity = workSheet.Cells[rowNumber, 6, rowNumber, workSheet.Dimension.End.Column][$"E{rowNumber}"].Text;
                data.DepartmentCode = workSheet.Cells[rowNumber, 7, rowNumber, workSheet.Dimension.End.Column][$"F{rowNumber}"].Text.Trim();
                data.ExpiredDay = workSheet.Cells[rowNumber, 8, rowNumber, workSheet.Dimension.End.Column][$"G{rowNumber}"].Text;
                data.ReplacementForUser = workSheet.Cells[rowNumber, 9, rowNumber, workSheet.Dimension.End.Column][$"H{rowNumber}"].Text;
                data.Reason = workSheet.Cells[rowNumber, 10, rowNumber, workSheet.Dimension.End.Column][$"I{rowNumber}"].Text;
                data.CostCenterCode = workSheet.Cells[rowNumber, 11, rowNumber, workSheet.Dimension.End.Column][$"J{rowNumber}"].Text.Trim();
                data.BusinessUnit = workSheet.Cells[rowNumber, 12, rowNumber, workSheet.Dimension.End.Column][$"K{rowNumber}"].Text;
                data.HQ_Operation = workSheet.Cells[rowNumber, 13, rowNumber, workSheet.Dimension.End.Column][$"L{rowNumber}"].Text.Trim();
                data.WorkingAddressCode = workSheet.Cells[rowNumber, 14, rowNumber, workSheet.Dimension.End.Column][$"M{rowNumber}"].Text.Trim();
                data.StartingDate = workSheet.Cells[rowNumber, 15, rowNumber, workSheet.Dimension.End.Column][$"N{rowNumber}"].Text;
                data.WorkingTime = workSheet.Cells[rowNumber, 16, rowNumber, workSheet.Dimension.End.Column][$"O{rowNumber}"].Text.Trim();
                data.ContractType = workSheet.Cells[rowNumber, 17, rowNumber, workSheet.Dimension.End.Column][$"P{rowNumber}"].Text;
                data.WorkingHourPerWeek = workSheet.Cells[rowNumber, 18, rowNumber, workSheet.Dimension.End.Column][$"Q{rowNumber}"].Text;
                data.WagePerHour = workSheet.Cells[rowNumber, 19, rowNumber, workSheet.Dimension.End.Column][$"R{rowNumber}"].Text;
                data.FromDate = workSheet.Cells[rowNumber, 20, rowNumber, workSheet.Dimension.End.Column][$"S{rowNumber}"].Text;
                data.ToDate = workSheet.Cells[rowNumber, 21, rowNumber, workSheet.Dimension.End.Column][$"T{rowNumber}"].Text;
                data.AssignTo = workSheet.Cells[rowNumber, 22, rowNumber, workSheet.Dimension.End.Column][$"U{rowNumber}"].Text.Trim();
                data.DepartmentName = workSheet.Cells[rowNumber, 23, rowNumber, workSheet.Dimension.End.Column][$"V{rowNumber}"].Text;
                data.Category = workSheet.Cells[rowNumber, 24, rowNumber, workSheet.Dimension.End.Column][$"W{rowNumber}"].Text;
                data.WorkingLocation = workSheet.Cells[rowNumber, 25, rowNumber, workSheet.Dimension.End.Column][$"X{rowNumber}"].Text;
                data.Remark = workSheet.Cells[rowNumber, 26, rowNumber, workSheet.Dimension.End.Column][$"Y{rowNumber}"].Text;
                data.DepartmentSAPCode = workSheet.Cells[rowNumber, 27, rowNumber, workSheet.Dimension.End.Column][$"Z{rowNumber}"].Text;
                data.Preventive1 = workSheet.Cells[rowNumber, 28, rowNumber, workSheet.Dimension.End.Column][$"AA{rowNumber}"].Text;
                data.Preventive2 = workSheet.Cells[rowNumber, 29, rowNumber, workSheet.Dimension.End.Column][$"AB{rowNumber}"].Text;
                data.Preventive3 = workSheet.Cells[rowNumber, 30, rowNumber, workSheet.Dimension.End.Column][$"AC{rowNumber}"].Text;
                data.Preventive4 = workSheet.Cells[rowNumber, 31, rowNumber, workSheet.Dimension.End.Column][$"AD{rowNumber}"].Text;
                data.Preventive5 = workSheet.Cells[rowNumber, 32, rowNumber, workSheet.Dimension.End.Column][$"AE{rowNumber}"].Text;
                listData.Add(data);
            }
            return listData;
        }

        // doc file tu import manual
        public RequestToHireFromImportFileDTO ReadDataFromStream(Stream stream)
        {
            RequestToHireFromImportFileDTO result = new RequestToHireFromImportFileDTO();
            using (var pck = new ExcelPackage(stream))
            {
                ExcelWorkbook WorkBook = pck.Workbook;
                ExcelWorksheet workSheet = WorkBook.Worksheets.ElementAt(0);
                var listData = this.ReadBySheet(workSheet);
                result.Data = listData;
            }
            return result;
        }
        public bool IsNumeric(string value)
        {
            return value.All(char.IsNumber);
        }

        public async Task<ResultDTO> UploadData(ImportRequestToHireArg arg, Stream stream)
        {
            var dataFromFile = this.ReadDataFromStream(stream);
            var result = await this.InserData(dataFromFile, arg);
            return result;
        }

        // doc file tu folder
        private RequestToHireFromImportFileDTO ReadDataFromRoot(string root)
        {
            RequestToHireFromImportFileDTO result = new RequestToHireFromImportFileDTO();
            using (var stream = System.IO.File.OpenRead(root))
            {
                using (var pck = new ExcelPackage(stream))
                {
                    ExcelWorkbook WorkBook = pck.Workbook;
                    ExcelWorksheet workSheet = WorkBook.Worksheets.First();
                    var listData = this.ReadBySheet(workSheet);

                    result.Data = listData;
                }
            }

            return result;
        }

        private DataTable CreateDataTableFile()
        {
            DataTable detailTable = new DataTable();
            detailTable.Columns.AddRange(
            new DataColumn[]{
                     new DataColumn("ID", typeof(int)),
                     new DataColumn("Root", typeof(string)),
                     new DataColumn("FileName", typeof(string))
            });
            return detailTable;
        }

        private void GetDataFiles(string sDir, List<DataFileDTO> _dataFiles)
        {
            try
            {
                using (new NetworkConnection(sDir, credentials))
                {
                    foreach (string dir in Directory.GetFiles(sDir))
                    {
                        try
                        {
                            if (!dir.Contains(".xlsm"))
                                _dataFiles.Add(new DataFileDTO()
                                {
                                    FileName = Path.GetFileName(dir),
                                    Root = dir
                                });
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: " + ex.Message);
                        }
                    }
                    foreach (string dir in Directory.GetDirectories(sDir))
                    {
                        GetDataFiles(dir, _dataFiles);
                    }
                }    
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                _dataFiles.Add(new DataFileDTO() { Root = "UserName : " + _userNameRTH + " - " + "Password: " + _passwordRTH,  FileName = ex.Message });
            }
        }

        public async Task<ResultDTO> InserData(RequestToHireFromImportFileDTO dataFromFile, ImportRequestToHireArg arg)
        {
            var result = new ResultDTO();
            var data = new ArrayResultDTO();
            // Get List position
            QueryArgs queryPositionRecruimentsArgs = new QueryArgs();
            List<ReasonViewModelForCABSetting> positionListInvalid = new List<ReasonViewModelForCABSetting>();
            // {"predicate":"MetadataType.Value = @0 & !Name.contains(@1)","predicateParameters":["Position","Acting"],"order":"created desc","limit":1000,"page":1}
            queryPositionRecruimentsArgs.Predicate = "MetadataType.Value = @0 & !Name.contains(@1)";
            queryPositionRecruimentsArgs.PredicateParameters = new object[] { "Position", "Acting" };
            queryPositionRecruimentsArgs.Order = "created desc";
            queryPositionRecruimentsArgs.Limit = 10000;
            queryPositionRecruimentsArgs.Page = 1;
            var positions = await _settingBO.GetPositionRecruiments(queryPositionRecruimentsArgs);
            if (positions.IsSuccess && !(positions.Object is null))
            {
                positionListInvalid = Mapper.Map<List<ReasonViewModelForCABSetting>>(positions.Object.GetPropertyValue("Data"));
            }

            if (!positionListInvalid.Any())
            {
                // show error
            }
            var positionCodeList = positionListInvalid.Select(x => x.Code).ToList();

            // get department code
            var allDepartments = await _uow.GetRepository<Department>(true).FindByAsync<DepartmentTreeViewModel>(x => !x.IsDeleted);

            // get all jobgrade
            var allJobgGrades = await _uow.GetRepository<JobGrade>(true).FindByAsync<JobGradeViewModel>(x => !x.IsDeleted);

            var allCostcenters = await _uow.GetRepository<CostCenterRecruitment>().FindByAsync(x => !x.IsDeleted);

            // Business unit
            var businessUnitList = new List<MasterExternalDataViewModel>();
            var businessUnit = await _masterDataB0.GetMasterDataValues(new MasterDataArgs() { Name = "WorkLocation" });
            if (!(businessUnit is null) && !(businessUnit.Object is null))
            {
                businessUnitList = Mapper.Map<List<MasterExternalDataViewModel>>(businessUnit.Object.GetPropertyValue("Data"));
            }

            // allWorkingAddress
            var workingAddressInvalid = new List<WorkingAddressRecruitmentViewModel>();
            QueryArgs queryWorkingAddress = new QueryArgs() { };
            // {"predicate":"code.contains(@0) || address.contains(@1)","predicateParameters":["",""],"page":1,"limit":10000,"order":"Modified desc"}
            queryWorkingAddress.Page = 1;
            queryWorkingAddress.Limit = 1000;
            queryWorkingAddress.Order = "Modified desc";
            queryWorkingAddress.Predicate = "code.contains(@0) || address.contains(@1)";
            queryWorkingAddress.PredicateParameters = new object[] { "", "" };
            var allWorkingAddress = await _settingBO.GetWorkingAddressRecruiments(queryWorkingAddress);

            if (allWorkingAddress.IsSuccess && !(allWorkingAddress.Object is null))
            {
                workingAddressInvalid = Mapper.Map<List<WorkingAddressRecruitmentViewModel>>(allWorkingAddress.Object.GetPropertyValue("Data"));
            }

            //  Working time
            var allWorkingTimes = await _uow.GetRepository<WorkingTimeRecruitment>(true).FindByAsync<WorkingTimeRecruimentViewModel>(x => !x.IsDeleted);

            // All Users
            var allUsers = await _uow.GetRepository<User>(true).FindByAsync(x => !x.IsDeleted);


            var allCategories = await _uow.GetRepository<RecruitmentCategory>(true).FindByAsync(x => !x.IsDeleted);

            //var massLocationCode = await _massBO.GetMassLocations();
            var massLocationList = await _massBO.GetMassLocationsPRD();

            // Contract type
            List<string> allContractTypeList = new List<string>() { "FT", "PT" };

            
            if (dataFromFile.Data.Count > 0)
            {
                ImportTracking tracking = new ImportTracking();
                List<ImportRequestToHireError> importTrackingInfo = new List<ImportRequestToHireError>();
                bool hasErrors = false;
                int rowNum = 0;
                foreach (var item in dataFromFile.Data)
                {
                    rowNum += 1;
                    ImportRequestToHireError inforTracking = new ImportRequestToHireError();
                    List<string> errorMessage = new List<string>();
                    var requestToHire = new RequestToHire() { };
                    inforTracking.RowNum = rowNum;
                    if (string.IsNullOrEmpty(item.ID))
                        errorMessage.Add("ID is null!");
                    else
                        inforTracking.ReferenceId = item.ID;

                    // kiem tra ton tai cua ID)
                    var checkExistReferenceNumber = await CheckExistReferenID(item.ID);
                    if (!string.IsNullOrEmpty(checkExistReferenceNumber))
                    {
                        errorMessage.Add("ID has been created successfully. (" + checkExistReferenceNumber + ")");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(item.PositionCode))
                        {
                            errorMessage.Add("PositionCode is null!");
                        } else
                        {
                            if (!positionCodeList.Contains(item.PositionCode))
                                errorMessage.Add("PositionCode is Invalid");
                            else
                            {
                                // Position
                                var currentPosition = positionListInvalid.Where(x => x.Code == item.PositionCode).FirstOrDefault();
                                requestToHire.PositionId = currentPosition.Id;
                                requestToHire.PositionCode = currentPosition.Code;
                                requestToHire.PositionName = currentPosition.Name;

                                // lay jobgrade
                                requestToHire.JobGradeGrade = currentPosition.JobGradeGrade;
                                requestToHire.JobGradeId = currentPosition.JobGradeId;
                                requestToHire.JobGradeCaption = currentPosition.JobGradeCaption;
                            }
                        }

                        // Type Of Need
                        if (string.IsNullOrEmpty(item.TypeOfNeed))
                        {
                            errorMessage.Add("Type Of Need is null!");
                        }
                        else
                        {
                            try
                            {
                                requestToHire.ReplacementFor = (TypeOfNeed)Enum.Parse(typeof(TypeOfNeed), item.TypeOfNeed);
                            }
                            catch (FormatException e)
                            {
                                errorMessage.Add("TypeOfNeed is wrong format!");
                                goto Finish;
                            }
                        }


                        if (TypeOfNeed.NewPosition == requestToHire.ReplacementFor)
                        {
                            // CheckBudget
                            if (string.IsNullOrEmpty(item.CheckBudget))
                            {
                                errorMessage.Add("Checkbudget is not exists!");
                            }
                            else
                            {
                                try
                                {
                                    requestToHire.HasBudget = (CheckBudgetOption)Enum.Parse(typeof(CheckBudgetOption), item.CheckBudget);
                                }
                                catch { errorMessage.Add("Checkbudget is wrong format!"); }
                            }

                            // value 1: NewPosition
                            var departments = allDepartments.Where(x => x.SAPCode == item.DepartmentCode).ToList().FirstOrDefault();
                            if ((departments is null))
                                errorMessage.Add("Department is not exists!");
                            else
                            {
                                requestToHire.DeptDivisionId = departments.Id;
                                requestToHire.DeptDivisionCode = departments.Code;
                                requestToHire.DeptDivisionName = departments.Name;
                                requestToHire.DeptName = departments.Name;
                                requestToHire.DeptCode = departments.Code;
                                requestToHire.DepartmentName = requestToHire.PositionName;
                                requestToHire.DeptDivisionGrade = departments.JobGradeGrade.ToString();

                                // lay ra cost center
                                if (!string.IsNullOrEmpty(item.CostCenterCode))
                                {
                                    var currentCostCenter = allCostcenters.Where(x => x.Code == item.CostCenterCode).OrderByDescending(y => y.Created).ToList().FirstOrDefault();
                                    if (!(currentCostCenter is null))
                                        requestToHire.CostCenterRecruitmentId = currentCostCenter.Id;
                                    else
                                        errorMessage.Add("CostCenterRecruitment is not exists!");
                                }
                                else
                                {
                                    /*if (departments.CostCenterRecruitmentId.HasValue)
                                    {
                                        var currentCostCenter = allCostcenters.Where(x => x.Id == departments.CostCenterRecruitmentId.Value).OrderByDescending(y => y.Created).ToList().FirstOrDefault();
                                        if (!(currentCostCenter is null))
                                            requestToHire.CostCenterRecruitmentId = currentCostCenter.Id;
                                    }
                                    else
                                        errorMessage.Add("CostCenterRecruitment is not exists!");*/
                                }
                            }

                            if (string.IsNullOrEmpty(item.Quantity) || !IsNumeric(item.Quantity))
                                errorMessage.Add("Quantity is Invalid!");
                            else
                            {
                                requestToHire.Quantity = int.Parse(item.Quantity);
                                // SAPCode of Department
                                if (!string.IsNullOrEmpty(item.DepartmentSAPCode))
                                {
                                    List<string> listSAPCode = new List<string>();
                                    if (item.DepartmentSAPCode.Contains(","))
                                    {
                                        var list = item.DepartmentSAPCode.Split(',').Trim();
                                        if (list.Any())
                                            listSAPCode = list.ToList();
                                    }
                                    else
                                        listSAPCode.Add(item.DepartmentSAPCode);

                                    if (requestToHire.Quantity != listSAPCode.Count)
                                    {
                                        errorMessage.Add("Quantity not equals SAPCode of department");
                                    }
                                    else
                                    {
                                        var checkSAPCodeDepartment = await _uow.GetRepository<Department>(true).FindByAsync(x => !string.IsNullOrEmpty(x.SAPCode) && listSAPCode.Contains(x.SAPCode));
                                        if (checkSAPCodeDepartment.Any())
                                        {
                                            foreach (var code in checkSAPCodeDepartment)
                                            {
                                                errorMessage.Add("SAPCode of department: " + code.SAPCode + " is exists!");
                                            }
                                        }
                                        if (!errorMessage.Any())
                                            requestToHire.ListDepartmentSAPCode = JsonConvert.SerializeObject(listSAPCode);
                                    }
                                }
                                else
                                    errorMessage.Add("SAPCode of department is null!");
                            }

                            if (requestToHire.DeptDivisionId.HasValue)
                            {
                                if (requestToHire.JobGradeId.HasValue && requestToHire.JobGradeGrade.HasValue && requestToHire.JobGradeGrade.HasValue && requestToHire.JobGradeGrade < departments.JobGradeGrade)
                                {
                                    var currentJobGrade = allJobgGrades.Where(x => x.Id == requestToHire.JobGradeId.Value).FirstOrDefault();
                                    if (!(currentJobGrade is null))
                                        requestToHire.ExpiredDayPosition = currentJobGrade.ExpiredDayPosition;
                                }/* else errorMessage.Add("ExpiredDayPosition is Invalid!");*/
                            }

                            if (!string.IsNullOrEmpty(item.DepartmentName))
                                requestToHire.DepartmentName = item.DepartmentName;

                        }
                        else
                        {
                            // value 2
                            requestToHire.HasBudget = CheckBudgetOption.Budget; // mac dinh
                            requestToHire.Quantity = 1;
                            requestToHire.CurrentBalance = 1;

                            if (!string.IsNullOrEmpty(item.CostCenterCode))
                            {
                                var currentCostCenter = allCostcenters.Where(x => x.Code == item.CostCenterCode).OrderByDescending(y => y.Created).ToList().FirstOrDefault();
                                if (!(currentCostCenter is null))
                                    requestToHire.CostCenterRecruitmentId = currentCostCenter.Id;
                                else
                                    errorMessage.Add("CostCenterRecruitment is not exists!");
                            }

                            // ReasonOptions
                            if (!string.IsNullOrWhiteSpace(item.Reason))
                            {
                                try
                                {
                                    requestToHire.Reason = (ReasonOptions)Enum.Parse(typeof(ReasonOptions), item.Reason);
                                }
                                catch (FormatException e) { errorMessage.Add("HQ/Operation is wrong format!"); }
                            }
                            else
                                errorMessage.Add("Reason is null!");

                            if (requestToHire.PositionId.HasValue && requestToHire.JobGradeId.HasValue)
                            {
                                if (!string.IsNullOrEmpty(item.DepartmentCode))
                                {
                                    var departments = allDepartments.Where(x => x.SAPCode == item.DepartmentCode).ToList().FirstOrDefault();
                                    if (!(departments is null))
                                    {
                                        if (departments.JobGradeId == requestToHire.JobGradeId.Value)
                                        {
                                            requestToHire.ReplacementForId = departments.Id;
                                            requestToHire.ReplacementForName = departments.Name;
                                            requestToHire.ReplacementForCode = departments.Code;
                                            requestToHire.ReplacementForGrade = departments.JobGradeGrade;

                                            var resultHeadCount = await _settingBO.GetUserCheckedHeadCount(departments.Id, "");
                                            if (!(resultHeadCount.Data is null))
                                            {
                                                var userHeadcount = resultHeadCount.Data as List<UserForTreeViewModel>;
                                                if (!(userHeadcount is null))
                                                {
                                                    //requestToHire.ReplacementForUserId = userHeadcount[0].Id;
                                                    if (!string.IsNullOrEmpty(item.ReplacementForUser))
                                                    {
                                                        var replacementForUser = allUsers.Where(x => x.SAPCode == item.ReplacementForUser).FirstOrDefault();
                                                        if (replacementForUser != null)
                                                        {
                                                            requestToHire.ReplacementForUserId = replacementForUser.Id;
                                                            var resignations = await _uow.GetRepository<ResignationApplication>().GetSingleAsync(x => x.UserSAPCode == replacementForUser.SAPCode && x.Status.Equals("Completed"), "created desc");
                                                            if (resignations != null)
                                                            {
                                                                requestToHire.ResignationId = resignations.Id;
                                                                requestToHire.ResignationNumber = resignations.ReferenceNumber;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            errorMessage.Add("ReplacementForUser is not exists!");
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                            errorMessage.Add("Department is invalid beacause jobgrade department not equal jobgrade position!");
                                    }
                                    else
                                        errorMessage.Add("Department is not exists!");
                                }
                            }
                            else
                                errorMessage.Add("Cannot get department id beacause position is null!");
                        }

                    Finish:
                        // validate department

                        // ExpiredDay
                        if (string.IsNullOrEmpty(item.ExpiredDay))
                            errorMessage.Add("Quantity is Invalid!");
                        else
                            requestToHire.ExpiredDayPosition = int.Parse(item.ExpiredDay); // mac dinh

                        // BusinessUnit
                        if (!string.IsNullOrEmpty(item.BusinessUnit))
                        {
                            var currentBusinessUnit = businessUnitList.Where(x => x.Code == item.BusinessUnit).ToList().FirstOrDefault();
                            if (!(currentBusinessUnit is null))
                            {
                                requestToHire.LocationCode = currentBusinessUnit.Code;
                                requestToHire.LocationName = currentBusinessUnit.Name;
                            }
                            else
                                errorMessage.Add("BusinessUnit is invalid!" + " - " + item.BusinessUnit);
                        }
                        else
                            errorMessage.Add("BusinessUnit is null!");

                        // HQ_Operation
                        if (!string.IsNullOrWhiteSpace(item.HQ_Operation))
                        {
                            try
                            {
                                requestToHire.Operation = (OperationOptions)Enum.Parse(typeof(OperationOptions), item.HQ_Operation);
                            }
                            catch (FormatException e) { errorMessage.Add("HQ/Operation is wrong format!"); }
                        }
                        else
                            errorMessage.Add("HQ/Operation is null!");

                        // Working address
                        if (!string.IsNullOrEmpty(item.WorkingAddressCode))
                        {
                            var currentWorkingAddress = workingAddressInvalid.Where(x => x.Code == item.WorkingAddressCode).ToList().FirstOrDefault();
                            if (!(currentWorkingAddress is null))
                            {
                                requestToHire.WorkingAddressRecruitmentId = currentWorkingAddress.Id;
                            }
                            else
                                errorMessage.Add("WorkingAddressCode is invalid!");
                        }
                        else
                            errorMessage.Add("WorkingAddressCode is null!");

                        // StartingDate
                        if (!string.IsNullOrEmpty(item.StartingDate))
                        {
                            try
                            {
                                DateTimeOffset date;
                                DateTimeOffset.TryParseExact(item.StartingDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
                                requestToHire.StartingDateRequire = date;
                            }
                            catch (Exception e)
                            {
                                errorMessage.Add("StartingDate Error Format: " + item.StartingDate + " | " + e.Message);
                            }
                        }

                        if (!(string.IsNullOrEmpty(item.WorkingTime)))
                        {
                            if (allWorkingTimes.Any())
                            {
                                var currentWorkingTime = allWorkingTimes.Where(x => x.Code.Equals(item.WorkingTime, StringComparison.OrdinalIgnoreCase)).ToList().FirstOrDefault();
                                if (!(currentWorkingTime is null))
                                {
                                    requestToHire.WorkingTimeId = currentWorkingTime.Id;
                                    requestToHire.WorkingTimeCode = currentWorkingTime.Code;
                                    requestToHire.WorkingTimeName = currentWorkingTime.Name;
                                }
                                else
                                    errorMessage.Add("WorkingTime is invalid!");
                            }
                            else
                                errorMessage.Add("Could not get master data WorkingTime!");
                        }
                        else
                            errorMessage.Add("WorkingTime is null!");

                        // ContractType
                        if (!string.IsNullOrWhiteSpace(item.ContractType))
                        {
                            if (allContractTypeList.Contains(item.ContractType))
                            {
                                requestToHire.ContractTypeCode = item.ContractType;
                                requestToHire.ContractTypeName = item.ContractType.Equals("FT") ? "Full time" : "Part time";
                                
                                if (requestToHire.ContractTypeCode.Equals("PT"))
                                {
                                    if (string.IsNullOrEmpty(item.WorkingHourPerWeek))
                                    {
                                        errorMessage.Add("Working hour per week is required");
                                    } else
                                    {
                                        if (!IsNumeric(item.WorkingHourPerWeek))
                                        {
                                            errorMessage.Add("Working hour per week is invalid");
                                        }
                                        else
                                        {
                                            requestToHire.WorkingHoursPerWeerk = int.Parse(item.WorkingHourPerWeek);
                                            if (!string.IsNullOrEmpty(item.WagePerHour))
                                            {
                                                if (!IsNumeric(item.WagePerHour))
                                                {
                                                    errorMessage.Add("Wage Per Hour is invalid");
                                                } else
                                                {
                                                    requestToHire.WagePerHour = double.Parse(item.WagePerHour);
                                                }
                                            }
                                        }
                                    }

                                    if (string.IsNullOrEmpty(item.FromDate))
                                    {
                                        errorMessage.Add("From date is required");
                                    } else if (string.IsNullOrEmpty(item.ToDate))
                                    {
                                        errorMessage.Add("To date is required");
                                    } else
                                    {
                                        string[] formats = { "dd/MM/yyyy", "d/M/yyyy" };
                                        DateTimeOffset parsedDate;
                                        // From date
                                        try
                                        {
                                            if (DateTimeOffset.TryParseExact(item.FromDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
                                                requestToHire.FromDate = parsedDate;

                                        } catch (Exception e)
                                        {
                                            errorMessage.Add("Parse From date is error! " + e.Message);
                                        }
                                        // To date
                                        try
                                        {
                                            if (DateTimeOffset.TryParseExact(item.ToDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
                                                requestToHire.ToDate = parsedDate;
                                        }
                                        catch (Exception e)
                                        {
                                            errorMessage.Add("Parse To date is error! " + e.Message);
                                        }
                                    }
                                }
                            }
                            else
                                errorMessage.Add("ContractType is invalid!");
                        }
                        else
                            errorMessage.Add("ContractType is null!");

                        if (!string.IsNullOrEmpty(item.AssignTo))
                        {
                            var assignToUser = allUsers.Where(x => x.SAPCode.Equals(item.AssignTo)).OrderByDescending(x => x.Created).FirstOrDefault();
                            if (!(assignToUser is null))
                                requestToHire.AssignToId = assignToUser.Id;
                        }
                        else
                            errorMessage.Add("AssignTo is null!");

                        // categories
                        if (!string.IsNullOrEmpty(item.Category))
                        {
                            var categories = allCategories.Where(x => x.Name.ToLower().Trim().Equals(item.Category.ToLower().Trim())).OrderByDescending(y => y.Created).FirstOrDefault();
                            if (!(categories is null))
                            {
                                requestToHire.CategoryId = categories.Id;
                                requestToHire.CategoryName = categories.Name;
                            }
                        }

                        // Work location 
                        if (!string.IsNullOrEmpty(item.WorkingLocation))
                        {
                            if (massLocationList.IsSuccess)
                            {
                                var currentLocation = massLocationList.Items.ToList().Where(x => x.value.ToLower().Equals(item.WorkingLocation.ToLower())).FirstOrDefault();
                                if (!(currentLocation is null))
                                {
                                    requestToHire.MassLocationCode = currentLocation.value;
                                    requestToHire.MassLocationName = currentLocation.name;
                                }
                                else
                                    errorMessage.Add("Worklocation is not exists!");
                            }
                            else
                                errorMessage.Add("Worklocation cannot get from API!");
                        }
                        else
                            errorMessage.Add("Worklocation is null!");

                        // JobDescription
                        if (!string.IsNullOrEmpty(item.Remark))
                            requestToHire.JobDescription = item.Remark;


                        // Column Preventive (support)
                        if (!string.IsNullOrEmpty(item.Preventive1))
                            requestToHire.Preventive1 = item.Preventive1;

                        if (!string.IsNullOrEmpty(item.Preventive2))
                            requestToHire.Preventive2 = item.Preventive2;

                        if (!string.IsNullOrEmpty(item.Preventive3))
                            requestToHire.Preventive3 = item.Preventive3;

                        if (!string.IsNullOrEmpty(item.Preventive4))
                            requestToHire.Preventive4 = item.Preventive4;

                        if (!string.IsNullOrEmpty(item.Preventive5))
                            requestToHire.Preventive5 = item.Preventive5;

                    }

                    // Check lai ReferenceID
                    checkExistReferenceNumber = await CheckExistReferenID(item.ID);
                    if (!string.IsNullOrEmpty(checkExistReferenceNumber))
                    {
                        errorMessage.Add("ID has been created successfully. (" + checkExistReferenceNumber + ")");
                    }

                    if (!errorMessage.Any())
                    {
                        requestToHire.IsImport = true;
                        _uow.GetRepository<RequestToHire>().Add(requestToHire);
                        await _uow.CommitAsync();
                        inforTracking.ReferenceNumber = requestToHire.ReferenceNumber;
                        inforTracking.RequestToHireId = requestToHire.Id;
                        inforTracking.ReferenceId = item.ID;
                        inforTracking.Status = "Success";
                        requestToHire.Status = "Completed";

                        // Sinh ra position
                        try
                        {
                            RequestToHireCompleteAction action = new RequestToHireCompleteAction();
                            await action.Execute(_uow, requestToHire.Id, _dashboardBO, _workflowBO, null);
                        }
                        catch (Exception e) {}

                        await this.UpdatePermission(requestToHire.Id);
                        CreateHistoricalWorkflow(requestToHire.Id, requestToHire.ReferenceNumber);
                    }
                    else
                    {
                        // luu log cua line do lai
                        inforTracking.ErrorMessage = errorMessage;
                        inforTracking.Status = "Failure";
                    }
                    importTrackingInfo.Add(inforTracking);
                    await _uow.CommitAsync();
                    if (!hasErrors && errorMessage.Any())
                        hasErrors = true;
                }

                tracking.JsonDataStr = JsonConvert.SerializeObject(importTrackingInfo);
                tracking.Module = arg.Module;
                tracking.IsImportManual = arg.IsImportManual;
                tracking.Status = hasErrors ? "Error" : "Success";

                // attachments
                if (!string.IsNullOrEmpty(arg.FileName))
                    tracking.FileName = arg.FileName;

                if (arg.AttachmentFileId.HasValue)
                {
                    tracking.Documents = JsonConvert.SerializeObject(new AttachmentDetail()
                    {
                        id = arg.AttachmentFileId.Value,
                        fileDisplayName = arg.FileName
                    });
                }
                _uow.GetRepository<ImportTracking>().Add(tracking);
                data.Data = tracking;
                await _uow.CommitAsync();
            }
            else
            {
                result.ErrorCodes.Add(1001);
                result.Messages.Add("REQUEST_TO_HIRE_VALIDATE_IMPORT_FILE");
            }
            result.Object = data;
            return result;
        }

        public async Task UpdatePermission(Guid ItemId)
        {
            var permission = await _uow.GetRepository<Permission>(true).FindByAsync(x => x.ItemId == ItemId);
            if (permission.Any())
            {
                foreach (var item in permission)
                    item.Perm = Right.View;
            }
        }
        public void CreateHistoricalWorkflow(Guid ItemId, string ReferencenNumber)
        {
            WorkflowInstance instance = new WorkflowInstance
            {
                ItemId = ItemId,
                ItemReferenceNumber = ReferencenNumber,
                WorkflowName = "Import Request To Hire",
                TemplateId = new Guid("FCA36DB0-DFAF-4626-BA15-D6634C2DCFC3"),
                WorkflowDataStr = "{\"OverwriteRequestedDepartment\":true,\"IgnoreValidation\":false,\"RequestedDepartmentField\":\"DeptDivisionId\",\"OnCancelled\":null,\"onRequestToChange\":null,\"DefaultCompletedStatus\":null,\"StartWorkflowConditions\":[{\"FieldName\":\"JobGradeCaption\",\"FieldValues\":[\"G2\",\"G3\"]},{\"FieldName\":\"HasBudget\",\"FieldValues\":[\"Non_Budget\"]},{\"FieldName\":\"Status\",\"FieldValues\":[\"Draft\",\"RequestedToChange\"]}],\"Steps\":[{\"StepName\":\"Submit\",\"OverwriteRequestedDepartment\":false,\"RequestedDepartmentField\":\"\",\"IsStatusFollowStepName\":true,\"SkipStepConditions\":[],\"RestrictedProperties\":[],\"StepConditions\":[],\"StepNumber\":1,\"OnSuccess\":\"Submitted\",\"OnFailure\":\"Rejected\",\"SuccessVote\":\"Submit\",\"FailureVote\":\"Reject\",\"DueDateNumber\":3,\"ParticipantType\":3,\"TraversingFromRoot\":false,\"IsHRHQ\":false,\"Level\":0,\"MaxLevel\":0,\"IgnoreIfNoParticipant\":false,\"PreventAutoPopulate\":false,\"DataField\":null,\"MaxJobGrade\":\"G9\",\"JobGrade\":\"G5\",\"ReverseJobGrade\":false,\"IncludeCurrentNode\":false,\"NextDepartmentType\":4,\"UserId\":null,\"UserFullName\":null,\"DepartmentId\":null,\"DepartmentName\":null,\"DepartmentType\":4,\"AllowRequestToChange\":true,\"IsCustomEvent\":false,\"CustomEventKey\":null,\"ReturnToStepNumber\":0,\"RequestorPerm\":1,\"ApproverPerm\":1,\"IsTurnedOffSendNotification\":false,\"IsAttachmentFile\":false,\"IsCustomRequestToChange\":false,\"IsStepWithConditions\":false}]}",
                IsCompleted = true
            };
            _uow.GetRepository<WorkflowInstance>().Add(instance);

            // tao workflow histories
            WorkflowHistory history = new WorkflowHistory
            {
                InstanceId = instance.Id,
                VoteType = VoteType.Approve,
                IsStepCompleted = true,
                Outcome = "Completed",
                StepNumber = 1,
                ApproverId = _uow.UserContext.CurrentUserId,
                ApproverFullName = _uow.UserContext.CurrentUserFullName,
                Comment = "Imported"
            };
            _uow.GetRepository<WorkflowHistory>().Add(history);

        }

        public async Task<string> CheckExistReferenID(string ReferenceID)
        {
            var valueReturn = "";
            var trackingImport = await _uow.GetRepository<ImportTracking>().FindByAsync(x => !x.IsDeleted);
            if (trackingImport.Any(x => !string.IsNullOrEmpty(x.JsonDataStr)))
            {
                foreach (var item in trackingImport)
                {
                    List<ImportRequestToHireError> jsonDataParsed = JsonConvert.DeserializeObject<List<ImportRequestToHireError>>(item.JsonDataStr);
                    if (jsonDataParsed.Any())
                    {
                        var existItem = jsonDataParsed.Where(x => !string.IsNullOrEmpty(x.ReferenceId) && x.ReferenceId.Equals(ReferenceID) && !string.IsNullOrEmpty(x.Status) && x.Status.ToUpper().Equals(Const.SAPStatus.SUCCESS)).FirstOrDefault();
                        if (!(existItem is null))
                        {
                            valueReturn = existItem.ReferenceNumber;
                            break;
                        }
                    }
                }
            }
            return valueReturn;
        }
        public async Task<ArrayResultDTO> GetImportTracking(TrackingImportArgs args)
        {               
            var result = new ArrayResultDTO();
            try
            {
                //var items = await _uow.GetRepository<ImportTracking>().FindByAsync<ImportTrackingViewModel>(args.Order, args.Page, args.Limit, args.Predicate, args.PredicateParameters);
                /*var items = await _uow.GetRepository<ImportTracking>().FindByAsync<ImportTrackingViewModel>(args.Predicate, args.PredicateParameters, args.Order);
                var count = await _uow.GetRepository<ImportTracking>().CountAsync(args.Predicate, args.PredicateParameters);
                var trackingimport = new List<ImportTrackingViewModel>();*/
                if (!string.IsNullOrEmpty(args.referenceId) || !string.IsNullOrEmpty(args.referenceNumber) || (args.referenceStatus != null && args.referenceStatus.Count > 0) )
                {
                    var items = await _uow.GetRepository<ImportTracking>().FindByAsync<ImportTrackingViewModel>(args.Predicate, args.PredicateParameters, args.Order);
                    var count = await _uow.GetRepository<ImportTracking>().CountAsync(args.Predicate, args.PredicateParameters);
                    var trackingimport = new List<ImportTrackingViewModel>();
                    count = 0;
                    if (items.Any(x => !string.IsNullOrEmpty(x.JsonDataStr)))
                    {
                        foreach (var item in items)
                        {
                            List<ImportRequestToHireError> jsonDataParsed = JsonConvert.DeserializeObject<List<ImportRequestToHireError>>(item.JsonDataStr);
                            if (jsonDataParsed.Any())
                            {
                                var existItem   = jsonDataParsed.Where(x => ((!string.IsNullOrEmpty(x.ReferenceId) && !string.IsNullOrEmpty(args.referenceId) && x.ReferenceId.Contains(args.referenceId)) || string.IsNullOrEmpty(args.referenceId))
                                                                                                 && ((!string.IsNullOrEmpty(x.ReferenceNumber) && !string.IsNullOrEmpty(args.referenceNumber) && x.ReferenceNumber.Equals(args.referenceNumber)) || string.IsNullOrEmpty(args.referenceNumber))
                                                                                                 && ((!string.IsNullOrEmpty(x.Status) && args.referenceStatus != null && args.referenceStatus.Count > 0 && args.referenceStatus.Contains(x.Status)) || (args.referenceStatus == null || args.referenceStatus.Count == 0))).FirstOrDefault();

                                if (!(existItem is null))
                                {
                                    trackingimport.Add(item);
                                    count = count + 1;
                                }
                            }
                        }
                    }
                    result = new ArrayResultDTO { Data = trackingimport, Count = count };
                }
                else
                {
                    var items = await _uow.GetRepository<ImportTracking>().FindByAsync<ImportTrackingViewModel>(args.Order, args.Page, args.Limit, args.Predicate, args.PredicateParameters);
                    var count = await _uow.GetRepository<ImportTracking>().CountAsync(args.Predicate, args.PredicateParameters);
                    result = new ArrayResultDTO { Data = items, Count = count };
                }
            }
            catch (Exception e)
            {
                result = new ArrayResultDTO { Data = "Error: " + e.Message, Count = 0 };
            }
            return result;
        }



        public async Task<ResultDTO> AutoImportRequestToHire(DataToImportRequestToHireTemplateArgs args)
        {
            var result = new ResultDTO();
            try
            {
                if (string.IsNullOrEmpty(args.RootDirectory))
                {
                    result.ErrorCodes = new List<int>() { -1 };
                    result.Messages = new List<string>() { "Root directory is null" };
                    goto Finish;
                }

                /*try
                {
                    NetworkCredential credentials = new NetworkCredential(_userNameRTH, _passwordRTH);
                    CredentialCache credentialCache = new CredentialCache();
                    credentialCache.Add(new Uri(args.RootDirectory), "Basic", credentials);
                    string[] file = Directory.GetFiles(args.RootDirectory);
                    if (file.Length <= 0)
                    {
                        result.ErrorCodes = new List<int>() { -1 };
                        result.Messages = new List<string>() { "Couldn't find any files!" };
                        goto Finish;
                    }
                }
                catch (Exception e)
                {
                    error = "Er: " + e.Message;
                }*/

                if (string.IsNullOrEmpty(args.RootDirectoryLog))
                {
                    result.ErrorCodes = new List<int>() { -1 };
                    result.Messages = new List<string>() { "Root directory log is null" };
                    goto Finish;
                }

                if (string.IsNullOrEmpty(args.RootDirectoryReceive))
                {
                    result.ErrorCodes = new List<int>() { -1 };
                    result.Messages = new List<string>() { "Root directory receive log is null" };
                    goto Finish;
                }

                result.Object = await ReadFileInFolder(args);
            }
            catch (Exception e)
            {
                result.ErrorCodes = new List<int>() { -3 };
                result.Messages = new List<string>() { e.Message};
            }
        Finish:
            return result;
        }

        private async Task<AttachmentFileViewModel> InsertAttachment(string root, string rootDirectory, string fileName)
        {
            var resultDTO = new AttachmentFileViewModel();
            // Insert file vao database
            string originalFileName = String.Concat(_uploadedFilesFolder, "\\" + (fileName).Trim(new Char[] { '"' }));
            var fileUniqueName = $"{Path.GetFileNameWithoutExtension(originalFileName)}_{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
            var filePath = Path.Combine(_uploadedFilesFolder, fileUniqueName);
            using (new NetworkConnection(root, credentials))
            {
                System.IO.File.Copy(rootDirectory, @filePath);
            }
            var fileResult = await _attachmentFileBO.Save(new AttachmentFileViewModel
            {
                Extension = Path.GetExtension(originalFileName),
                FileDisplayName = $"{Path.GetFileNameWithoutExtension(originalFileName)}{Path.GetExtension(originalFileName)}",
                FileUniqueName = fileUniqueName,
                Name = $"{Path.GetFileNameWithoutExtension(originalFileName)}{Path.GetExtension(originalFileName)}",
                Size = (new System.IO.FileInfo(filePath).Length) / 1024,
                Type = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            });

            if (fileResult != null && fileResult.Object != null && fileResult.Object is AttachmentFileViewModel)
            {
                resultDTO = Mapper.Map<AttachmentFileViewModel>(fileResult.Object);
            }
            return resultDTO;
        }

        public async Task<List<string>> ReadFileInFolder(DataToImportRequestToHireTemplateArgs args)
        {
            credentials = new NetworkCredential(_userNameRTH, _passwordRTH);
            List<string> vReturn = new List<string>();
            List<DataFileDTO> _dataFiles = new List<DataFileDTO>();
            this.GetDataFiles(args.RootDirectory, _dataFiles);
            if (_dataFiles.Count > 0)
            {
                foreach (var item in _dataFiles)
                {
                    var dataFromFile = ReadDataFromRoot(item.Root);
                    try
                    {
                        if (dataFromFile.Data.Any())
                        {
                            var resultInsertAttachmentFile = await this.InsertAttachment(args.RootDirectory, item.Root, item.FileName);
                            if (!(resultInsertAttachmentFile is null))
                            {
                                var resultInsertData = await InserData(dataFromFile, new ImportRequestToHireArg() { AttachmentFileId = resultInsertAttachmentFile.Id, FileName = item.FileName, IsImportManual = false });
                                if (resultInsertData.IsSuccess)
                                {
                                    vReturn.Add(item.FileName);
                                    // Cut file
                                    ArrayResultDTO response = Mapper.Map<ArrayResultDTO>(resultInsertData.Object);
                                    ImportTracking importTracking = Mapper.Map<ImportTracking>(response.Data);
                                    List<ImportRequestToHireError> errorLogs = JsonConvert.DeserializeObject<List<ImportRequestToHireError>>(importTracking.JsonDataStr);
                                    this.CreateFileLog(args.RootDirectory, args.RootDirectoryLog, item.FileName, errorLogs);
                                    this.ProcessedFile(args.RootDirectory, item.Root, args.RootDirectoryReceive, item.FileName);
                                    // Create file log
                                }
                                else
                                {
                                    this.SaveLogs(args.RootDirectory, args.RootDirectoryLog, item.FileName, new List<string>() { "Read not line" });
                                    this.ProcessedFile(args.RootDirectory, item.Root, args.RootDirectoryLog, item.FileName);
                                }
                            }
                        }
                        else
                        {
                            this.SaveLogs(args.RootDirectory, args.RootDirectoryLog, item.FileName, new List<string>() { "Read not line" });
                            this.ProcessedFile(args.RootDirectory, item.Root, args.RootDirectoryLog, item.FileName);
                        }
                    }
                    catch (Exception e)
                    {
                        this.SaveLogs(args.RootDirectory, args.RootDirectoryLog, item.FileName, new List<string>() { e.Message, e.StackTrace });
                        this.ProcessedFile(args.RootDirectory, item.Root, args.RootDirectoryLog, item.FileName);
                    }
                }
            }
            return vReturn;
        }

        public void SaveLogs(string root, string directory, string fileName, List<string> messageErrors)
        {
            using (new NetworkConnection(root, credentials))
            {
                string directoryLog = directory + (fileName + "_" + "_ExceptionLog_" + DateTimeOffset.Now.ToString("ddMMyyyyHHmmss") + ".txt");
                System.IO.File.Create(@directoryLog).Dispose();
                TextWriter tw = new StreamWriter(@directoryLog, false);
                tw.WriteLine("Exception: ");
                messageErrors.ForEach(x => tw.WriteLine(" - " + x));
                tw.Close();
            }
        }

        public void ProcessedFile(string root, string rootDirectory, string rootDirectoryRecive, string fileName)
        {
            
            using (new NetworkConnection(root, credentials))
                {
                    string typeFile = ".xlsx";
                    string directoryRecive = rootDirectoryRecive + (fileName.Replace(typeFile, ("_processed_" + DateTimeOffset.Now.ToString("ddMMyyyyHHmmss") + typeFile)));
                    System.IO.File.Move(@rootDirectory, directoryRecive);
                }
        }

        public void RejctedFile(string root, string rootDirectory, string rootDirectoryRecive, string fileName)
        {
            using (new NetworkConnection(root, credentials))
            {
                string typeFile = ".xlsx";
                string directoryRecive = rootDirectoryRecive + (fileName.Replace(typeFile, ("_processed_" + DateTimeOffset.Now.ToString("ddMMyyyyHHmmss") + typeFile)));
                System.IO.File.Move(@rootDirectory, directoryRecive);
            }
        }

        public void CreateFileLog(string root, string rootDirectoryLog, string fileName, List<ImportRequestToHireError> errorLogs)
        {
            string typeFile = ".xlsx";
            string changeToTypeFile = ".txt";
            string directoryLog = rootDirectoryLog + (fileName.Replace(typeFile, ("_log_" + DateTimeOffset.Now.ToString("ddMMyyyyHHmmss") + changeToTypeFile)));
            using (new NetworkConnection(root, credentials))
            {
                System.IO.File.Create(@directoryLog).Dispose();
                TextWriter tw = new StreamWriter(@directoryLog, false);
                foreach (var itemLog in errorLogs)
                {
                    tw.WriteLine("Row: " + itemLog.RowNum);
                    tw.WriteLine("RefereceID: " + itemLog.ReferenceId);
                    tw.WriteLine("Status: " + itemLog.Status);
                    if (itemLog.ErrorMessage.Any())
                    {
                        tw.WriteLine("ErrorLog: ");
                        itemLog.ErrorMessage.ForEach(x => tw.WriteLine(" - " + x));
                    }
                    tw.WriteLine("-----------------------------------------------");
                }
                tw.Close();
            }
        }
    }
}