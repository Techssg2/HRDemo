using Aeon.HR.BusinessObjects.Handlers.File;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aeon.HR.BusinessObjects.Helpers;


namespace Aeon.HR.BusinessObjects.Handlers.FIle
{
    public class OverTimeProcessingBO : BaseExcelProcessing, IExcelProcessingBO
    {
        protected override string Json => "export-mapping.json";
        protected override string JsonGroupName => "OvertimeApplication";
        public OverTimeProcessingBO(IUnitOfWork uow) : base(uow)
        {
        }

        public Task<ResultDTO> ImportAsync(FileStream stream)
        {
            throw new NotImplementedException();
        }

        public async Task<ResultDTO> ExportAsync(ViewModels.Args.QueryArgs parameters)
        {
            var fieldMappings = ReadConfigurationFromFile();
            var headers = fieldMappings.Select(y => y.DisplayName);
            // Create Headers
            DataTable tbl = new DataTable();
            foreach (var headerItem in headers)
            {
                tbl.Columns.Add(headerItem);
            }
            // Add Row
            var records = await _uow.GetRepository<OvertimeApplication>().FindByAsync<OvertimeApplicationViewModel>(parameters.Order, parameters.Page, parameters.Limit, parameters.Predicate, parameters.PredicateParameters);
            if (records.Any())
            {
                int overtimeIndex = parameters.Predicate.IndexOf("overtimeItems");

                if (overtimeIndex != -1)
                {
                    var overtimeString = parameters.Predicate.Substring(overtimeIndex);
                    var endOfCompareIndex = overtimeString.IndexOf("and");
                    if (endOfCompareIndex != -1)
                    {
                        overtimeString = overtimeString.Substring(0, endOfCompareIndex - 1);
                    }
                    int fromIndex = overtimeString.IndexOf("@") + 1;
                    int toIndex = overtimeString.LastIndexOf("@") + 1;

                    int fromIndexParameter = int.Parse(overtimeString.Substring(fromIndex, 1));
                    int toIndexParameter = int.Parse(overtimeString.Substring(toIndex, 1));
                    if (fromIndex == toIndex)
                    {
                        if (overtimeString.IndexOf(">=") != -1)
                        {
                            foreach (var record in records)
                            {
                                var stringDateCompareFrom = parameters.PredicateParameters.ElementAt(fromIndexParameter).ToString();
                                var dateCompareFrom = DateTimeOffset.ParseExact(stringDateCompareFrom, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                                record.OvertimeItems = record.OvertimeItems.Where(x => x.Date >= dateCompareFrom).ToList();
                            }
                        }
                        else if (overtimeString.IndexOf("<") != -1)
                        {
                            foreach (var record in records)
                            {
                                var stringDateCompareTo = parameters.PredicateParameters.ElementAt(toIndexParameter).ToString();
                                var dateCompareTo = DateTimeOffset.ParseExact(stringDateCompareTo, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                                record.OvertimeItems = record.OvertimeItems.Where(x => x.Date < dateCompareTo).ToList();
                            }
                        }
                    }
                    else
                    {

                        foreach (var record in records)
                        {
                            var stringDateCompareFrom = parameters.PredicateParameters.ElementAt(fromIndexParameter).ToString();
                            var stringDateCompareTo = parameters.PredicateParameters.ElementAt(toIndexParameter).ToString();

                            var dateCompareFrom = DateTimeOffset.ParseExact(stringDateCompareFrom, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                            var dateCompareTo = DateTimeOffset.ParseExact(stringDateCompareTo, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                            record.OvertimeItems = record.OvertimeItems.Where(x => x.Date >= dateCompareFrom && x.Date < dateCompareTo).ToList();
                        }
                    }
                }
                var exportOvertimeApplicationViewModels = new List<ExportOvertimeApplicationDetailViewModel>();
                var contentofOtherReason = string.Empty;
                foreach (var record in records)
                {
                    if (record.OvertimeItems.Any())
                    {
                        //jira - 597
                        var deptIsHQ = false;
                        if (record.DivisionCode is null)
						{
                            var currentDepartment = await _uow.GetRepository<Department>().FindByAsync<DepartmentViewModel>(x => x.Code == record.DeptCode);
                            if (currentDepartment != null && currentDepartment.Any())
                            {
                                deptIsHQ = !currentDepartment.FirstOrDefault().IsStore;
                            }
                        }
						else
						{
                            var currentDepartment = await _uow.GetRepository<Department>().FindByAsync<DepartmentViewModel>(x => x.Code == record.DivisionCode);
                            if (currentDepartment != null && currentDepartment.Any())
                            {
                                deptIsHQ = !currentDepartment.FirstOrDefault().IsStore;
                            }
                        }                      
                        
                        //========================
                        var details = record.OvertimeItems;
                        if (details.Any())
                        {
                            foreach (var detail in details)
                            {
                                try
                                {
                                    User user = null;
                                    bool isHQ = false;
                                    if (record.Type == OverTimeType.EmployeeSeftService)
                                    {
                                        detail.SAPCode = record.UserSAPCode;
                                        detail.FullName = record.CreatedByFullName;
                                        user = record.UserSAPCode.GetUserByUserSAP(_uow);
                                        isHQ = user.UserDepartmentMappings.Any(x => (x.Department != null) && !x.Department.IsStore && x.IsHeadCount);
                                    }
                                    if (record.Type == OverTimeType.ManagerApplyForEmployee)
                                    {
                                        detail.ReasonName = record.ReasonName;
                                        user = detail.SAPCode.GetUserByUserSAP(_uow);
                                        isHQ = user.UserDepartmentMappings.Any(x => (x.Department != null) && !x.Department.IsStore && x.IsHeadCount);
                                    }
                                    if (detail.IsStore.HasValue) {
                                        isHQ = !detail.IsStore.Value;
                                    }
                                    if (record.ContentOfOtherReason != null)
                                    {
                                        contentofOtherReason = record.ContentOfOtherReason;
                                    }
                                    else
                                    {
                                        contentofOtherReason = detail.DetailReason;
                                    }
                                    string otProposalHours = string.Empty, otActualHours = string.Empty;

                                    DateTime dateValue;
                                    if (DateTime.TryParse(detail.ProposalHoursTo, out dateValue) && DateTime.TryParse(detail.ProposalHoursFrom, out dateValue))
                                    {
                                        
                                        //jira - 597
										if (!user.UserDepartmentMappings.Any(x => x != null))
										{
                                            otProposalHours = GetHours(detail.ProposalHoursFrom, detail.ProposalHoursTo, isHQ);
                                        }
										else
										{
                                            //Fix ticket #320: user.IsStore()
                                            otProposalHours = GetHours(detail.ProposalHoursFrom, detail.ProposalHoursTo, isHQ);
                                        }
                                        //========================
                                    }
                                    if (DateTime.TryParse(detail.ActualHoursFrom, out dateValue) && DateTime.TryParse(detail.ActualHoursTo, out dateValue))
                                    {
										//jira - 597
										if (!user.UserDepartmentMappings.Any(x => x != null))
										{
                                            otActualHours = GetHours(detail.ActualHoursFrom, detail.ActualHoursTo, isHQ);
                                        }
										else
										{
                                            //Fix ticket #320: user.IsStore()
                                            otActualHours = GetHours(detail.ActualHoursFrom, detail.ActualHoursTo, isHQ);
                                        }
                                        //========================
                                    }
                                    exportOvertimeApplicationViewModels.Add(new ExportOvertimeApplicationDetailViewModel
                                    {
                                        ReferenceNumber = record.ReferenceNumber,
                                        Status = record.Status,
                                        Date = detail.Date,
                                        ProposalHoursFrom = detail.ProposalHoursFrom,
                                        ProposalHoursTo = detail.ProposalHoursTo,
                                        OtProposalHours = otProposalHours,
                                        ActualHoursFrom = detail.ActualHoursFrom,
                                        ActualHoursTo = detail.ActualHoursTo,
                                        ActualOtHours = otActualHours,
                                        ReasonName = detail.ReasonName,
                                        DateOffInLieu = detail.DateOffInLieu,
                                        IsNoOT = detail.IsNoOT,
                                        DepartmentCode = !string.IsNullOrEmpty(record.DeptCode) ? record.DeptCode : record.DivisionCode,
                                        DepartmentName = !string.IsNullOrEmpty(record.DeptName) ? record.DeptName : record.DivisionName,
                                        CreateDate = record.Created,
                                        WorkLocationCode = !string.IsNullOrEmpty(record.WorkLocationCode) ? record.WorkLocationCode : "",
                                        WorkLocationName = !string.IsNullOrEmpty(record.WorkLocationName) ? record.WorkLocationName : "",
                                        DetailReason = string.IsNullOrEmpty(detail.DetailReason) ? detail.ReasonName : detail.DetailReason,
                                        SubmitFullName = record.CreatedByFullName,
                                        SubmitSAPCode = record.UserSAPCode,
                                        SAPCodeUserOT = detail.SAPCode,
                                        FullNameUserOT = detail.FullName,
                                        ContentOfOtherReason = contentofOtherReason
                                    });
                                }
                                catch (Exception ex)
                                {
                                    throw new SystemException(ex.Message);
                                }
                            }
                        }
                    }
                }

                for (int rowNum = 0; rowNum < exportOvertimeApplicationViewModels.Count(); rowNum++)
                {
                    DataRow row = tbl.Rows.Add();
                    var data = exportOvertimeApplicationViewModels.ElementAt(rowNum);
                    for (int j = 0; j < fieldMappings.Count; j++)
                    {
                        var fieldMapping = fieldMappings[j];
                        var value = data.GetType().GetProperty(fieldMapping.Name).GetValue(data);
                        HandleCommonType(row, value, j, fieldMapping);
                    }
                }
            }
            else
            {
                return new ResultDTO { ErrorCodes = { 1003 }, Messages = { "No Data" } };
            }
            var creatingExcelFileReslult = ExportExcel(tbl);
            if (creatingExcelFileReslult == null)
            {
                return new ResultDTO { ErrorCodes = { 1003 }, Messages = { "No Data" } };
            }
            return new ResultDTO { Object = creatingExcelFileReslult };

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
            if (isHQ && float.Parse(result) > 4)
            {
                result = (float.Parse(result) - 1).ToString();
            }
            return result;
        }
    }
}
