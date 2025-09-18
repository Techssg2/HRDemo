using Aeon.HR.BusinessObjects.Handlers.File;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
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
    public class ShiftExchangeApplicationProcessingBO : BaseExcelProcessing, IExcelProcessingBO
    {
        protected override string Json => "export-mapping.json";
        protected override string JsonGroupName => "ShiftExchangeApplication";
        public ShiftExchangeApplicationProcessingBO(IUnitOfWork uow) : base(uow)
        {

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
            var records = await _uow.GetRepository<ShiftExchangeApplication>().FindByAsync<ShiftExchangeViewByReferenceNumberViewModel>(parameters.Order, parameters.Page, parameters.Limit, parameters.Predicate, parameters.PredicateParameters);
            if (records.Any())
            {
                int shiftExchangeIndex = parameters.Predicate.IndexOf("exchangingShiftItems");

                if (shiftExchangeIndex != -1)
                {
                    var shiftExchangeString = parameters.Predicate.Substring(shiftExchangeIndex);
                    var endOfCompareIndex = shiftExchangeString.IndexOf("and");
                    if (endOfCompareIndex != -1)
                    {
                        shiftExchangeString = shiftExchangeString.Substring(0, endOfCompareIndex - 1);
                    }
                    int fromIndex = shiftExchangeString.IndexOf("@") + 1;
                    int toIndex = shiftExchangeString.LastIndexOf("@") + 1;

                    int fromIndexParameter = int.Parse(shiftExchangeString.Substring(fromIndex, 1));
                    int toIndexParameter = int.Parse(shiftExchangeString.Substring(toIndex, 1));
                    if (fromIndex == toIndex)
                    {
                        if (shiftExchangeString.IndexOf(">=") != -1)
                        {
                            foreach (var record in records)
                            {
                                var stringDateCompareFrom = parameters.PredicateParameters.ElementAt(fromIndexParameter).ToString();
                                var dateCompareFrom = DateTimeOffset.ParseExact(stringDateCompareFrom, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                                record.ExchangingShiftItems = record.ExchangingShiftItems.Where(x => x.ShiftExchangeDate >= dateCompareFrom).ToList();
                            }
                        }
                        else if (shiftExchangeString.IndexOf("<") != -1)
                        {
                            foreach (var record in records)
                            {
                                var stringDateCompareTo = parameters.PredicateParameters.ElementAt(toIndexParameter).ToString();
                                var dateCompareTo = DateTimeOffset.ParseExact(stringDateCompareTo, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                                record.ExchangingShiftItems = record.ExchangingShiftItems.Where(x => x.ShiftExchangeDate < dateCompareTo).ToList();
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

                            record.ExchangingShiftItems = record.ExchangingShiftItems.Where(x => x.ShiftExchangeDate >= dateCompareFrom && x.ShiftExchangeDate < dateCompareTo).ToList();
                        }
                    }
                }
                var exportShiftExchangeApplicationViewModels = new List<ExportShiftExchangeDetaiViewModel>();
                var users = await _uow.GetRepository<User>().GetAllAsync<UserDTO>();
                foreach (var record in records)
                {
                    var details = record.ExchangingShiftItems;
                    var last_history = new WorkflowHistoryViewModel();
                    DateTimeOffset? completedDate = null;
                    if (record.Status.ToLower().Equals("completed"))
                    {
                        var instances = await _uow.GetRepository<WorkflowInstance>().FindByAsync(x => x.ItemReferenceNumber == record.ReferenceNumber, "Created desc");
                        if (instances.Any())
                        {
                            var firstInstance = instances.FirstOrDefault();
                            var histories = await _uow.GetRepository<WorkflowHistory>().FindByAsync<WorkflowHistoryViewModel>(x => x.InstanceId == firstInstance.Id && x.IsStepCompleted == true, "Created desc");
                            last_history = histories.FirstOrDefault();
                            completedDate = last_history.Modified;
                        }
                    }



                    if (details.Any())
                    {
                        var sapCodes = details.Select(x => x.UserSAPCode);
                        //var users = await _uow.GetRepository<User>().FindByAsync<UserDTO>(x => sapCodes.Contains(x.SAPCode));
                        foreach (var detail in details)
                        {
                            exportShiftExchangeApplicationViewModels.Add(new ExportShiftExchangeDetaiViewModel
                            {
                                ReferenceNumber = record.ReferenceNumber,
                                Status = record.Status,
                                FullName = record.FullName,
                                SAPCode = record.SAPCode,
                                UserFullName = detail.UserFullName,
                                UserSAPCode = detail.UserSAPCode,
                                ReasonName = detail.ReasonName,
                                ShiftExchangeDate = detail.ShiftExchangeDate,
                                CurrentShiftCode = detail.CurrentShiftCode,
                                CurrentShiftName = detail.CurrentShiftName,
                                NewShiftCode = detail.NewShiftCode,
                                NewShiftName = detail.NewShiftName,
                                OtherReason = detail.OtherReason,
                                ReasonCode = detail.ReasonCode,
                                DepartmentCode = !string.IsNullOrEmpty(record.DeptLineCode) ? record.DeptLineCode : record.DeptDivisionCode,
                                DepartmentName = !string.IsNullOrEmpty(record.DeptLineName) ? record.DeptLineName : record.DeptDivisionName,
                                CreateDate = record.Created,
                                QuotaErd = users.FirstOrDefault(x => x.SAPCode == detail.UserSAPCode) != null ? users.FirstOrDefault(x => x.SAPCode == detail.UserSAPCode).ErdRemain : 0,
                                IsERD = detail.IsERD,
                                CompletedDate = completedDate

                            }); ;
                        }
                    }
                }

                for (int rowNum = 0; rowNum < exportShiftExchangeApplicationViewModels.Count(); rowNum++)
                {
                    DataRow row = tbl.Rows.Add();
                    var data = exportShiftExchangeApplicationViewModels.ElementAt(rowNum);
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

        public Task<ResultDTO> ImportAsync(FileStream stream)
        {
            throw new NotImplementedException();
        }
    }
}
