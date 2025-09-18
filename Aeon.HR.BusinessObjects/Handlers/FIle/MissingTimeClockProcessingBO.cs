using Aeon.HR.BusinessObjects.Handlers.File;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Handlers.FIle
{
    public class MissingTimeClockProcessingBO : BaseExcelProcessing, IExcelProcessingBO
    {
        protected override string Json => "export-mapping.json";
        protected override string JsonGroupName => "MissingTimeClock";
        public MissingTimeClockProcessingBO(IUnitOfWork uow) : base(uow)
        {

        }

        public Task<ResultDTO> ImportAsync(FileStream stream)
        {
            throw new NotImplementedException();
        }

        public async Task<ResultDTO> ExportAsync(QueryArgs parameters)
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
            var records = await _uow.GetRepository<MissingTimeClock>().FindByAsync<MissingTimelockViewModel>(parameters.Order, parameters.Page, parameters.Limit, parameters.Predicate, parameters.PredicateParameters);
            if (records.Any())
            {
                int missingTimeClockIndex = parameters.Predicate.IndexOf("missingTimeClockDetails");
                DateTime? dateCompareFrom = null;
                DateTime? dateCompareTo = null;
                if (missingTimeClockIndex != -1)
                {
                    var missingTimeClockString = parameters.Predicate.Substring(missingTimeClockIndex);
                    var endOfCompareIndex = missingTimeClockString.IndexOf("and");
                    if (endOfCompareIndex != -1)
                    {
                        missingTimeClockString = missingTimeClockString.Substring(0, endOfCompareIndex - 1);
                    }
                    int fromIndex = missingTimeClockString.IndexOf("@") + 1;
                    int toIndex = missingTimeClockString.LastIndexOf("@") + 1;

                    int fromIndexParameter = int.Parse(missingTimeClockString.Substring(fromIndex, 1));
                    int toIndexParameter = int.Parse(missingTimeClockString.Substring(toIndex, 1));
                    if (fromIndex == toIndex)
                    {
                        if (missingTimeClockString.IndexOf(">=") != -1)
                        {
                            var stringDateCompareFrom = parameters.PredicateParameters.ElementAt(fromIndexParameter).ToString();
                            dateCompareFrom = DateTime.ParseExact(stringDateCompareFrom, "yyyy-MM-dd", null).AddHours(-7);
                        }
                        else if (missingTimeClockString.IndexOf("<") != -1)
                        {
                            var stringDateCompareTo = parameters.PredicateParameters.ElementAt(toIndexParameter).ToString();
                            dateCompareTo = DateTime.ParseExact(stringDateCompareTo, "yyyy-MM-dd", CultureInfo.InvariantCulture).AddHours(-7);
                        }
                    }
                    else
                    {
                        var stringDateCompareFrom = parameters.PredicateParameters.ElementAt(fromIndexParameter).ToString();
                        var stringDateCompareTo = parameters.PredicateParameters.ElementAt(toIndexParameter).ToString();
                        dateCompareFrom = DateTime.ParseExact(stringDateCompareFrom, "yyyy-MM-dd", null).AddHours(-7);
                        dateCompareTo = DateTime.ParseExact(stringDateCompareTo, "yyyy-MM-dd", null).AddHours(-7);
                    }
                }
                var missingTimeClockDetails = new List<MissingTimeClockExportDetailViewModel>();
                var exportMisstingTimeClockDetailViewModel = new List<ExportMissingTimeClockViewModel>();
                foreach (var record in records)
                {
                    if (!string.IsNullOrEmpty(record.ListReason))
                    {
                        var details = JsonConvert.DeserializeObject<List<MissingTimeClockExportDetailViewModel>>(record.ListReason);

                        if (details.Any())
                        {
                            if (dateCompareFrom.HasValue && dateCompareTo.HasValue)
                            {
                                details = details.Where(x => x.Date >= dateCompareFrom && x.Date < dateCompareTo).ToList();
                            }
                            else if (dateCompareFrom.HasValue)
                            {
                                details = details.Where(x => x.Date >= dateCompareFrom).ToList();
                            }
                            else if (dateCompareTo.HasValue)
                            {
                                details = details.Where(x => x.Date < dateCompareTo).ToList();
                            }
                            foreach (var detail in details)
                            {
                                var typeName = "";
                                if (!string.IsNullOrEmpty(detail.ActualTime))
                                {
                                    detail.ActualTime = DateTime.Parse(detail.ActualTime).ToLocalTime().ToString("HH:mm");
                                }
                                if(detail.TypeActualTime == TypeActualTime.In)
                                {
                                    typeName = "IN";
                                }
                                else
                                {
                                    typeName = "OUT";
                                }
                                exportMisstingTimeClockDetailViewModel.Add(new ExportMissingTimeClockViewModel
                                {
                                    ReferenceNumber = record.ReferenceNumber,
                                    Status = record.Status,
                                    Date = detail.Date,
                                    DivisionName = record.DivisionName,
                                    SapCode = record.UserSAPCode,
                                    ActualTime = detail.ActualTime,
                                    ShiftCode = detail.ShiftCode,
                                    Other = detail.Others,
                                    Reason = string.IsNullOrEmpty(detail.ReasonName) ? detail.Others : detail.ReasonName,
                                    DepartmentCode = !string.IsNullOrEmpty(record.DeptCode) ? record.DeptCode : record.DivisionCode,
                                    DepartmentName = !string.IsNullOrEmpty(record.DeptName) ? record.DeptName : record.DivisionName,
                                    CreateDate = record.Created,
                                    FullName = record.CreatedByFullName,
                                    TypeActualTime = typeName,
                                    Documents = record.Documents == null ? "No" : "Yes"
                                });
                            }
                        }
                    }
                }

                for (int rowNum = 0; rowNum < exportMisstingTimeClockDetailViewModel.Count(); rowNum++)
                {
                    DataRow row = tbl.Rows.Add();
                    var data = exportMisstingTimeClockDetailViewModel.ElementAt(rowNum);
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
    }
}
