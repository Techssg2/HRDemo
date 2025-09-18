using Aeon.HR.BusinessObjects.Handlers.File;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aeon.HR.Data.Models;
using Aeon.HR.ViewModels;
using AutoMapper;
using System.IO;
using System.Globalization;

namespace Aeon.HR.BusinessObjects.Handlers.FIle
{
    public class LeaveApplicationProcessingBO : BaseExcelProcessing, IExcelProcessingBO
    {

        protected override string Json => "export-mapping.json";
        protected override string JsonGroupName => "LeaveApplication";
        public LeaveApplicationProcessingBO(IUnitOfWork uow) : base(uow)
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
            var records = await _uow.GetRepository<LeaveApplication>().FindByAsync(parameters.Order, parameters.Page, parameters.Limit, parameters.Predicate, parameters.PredicateParameters);
            if (records.Any())
            {
                int leaveApplicationDetailsIndex = parameters.Predicate.IndexOf("leaveApplicationDetails");

                if (leaveApplicationDetailsIndex != -1)
                {
                    var leaveApplicationDetailsString = parameters.Predicate.Substring(leaveApplicationDetailsIndex);
                    var endOfCompareIndex = leaveApplicationDetailsString.IndexOf("and");
                    if (endOfCompareIndex != -1)
                    {
                        leaveApplicationDetailsString = leaveApplicationDetailsString.Substring(0, endOfCompareIndex - 1);
                    }
                    int fromIndex = leaveApplicationDetailsString.IndexOf("@") + 1;
                    int toIndex = leaveApplicationDetailsString.LastIndexOf("@") + 1;

                    int fromIndexParameter = int.Parse(leaveApplicationDetailsString.Substring(fromIndex, 1));
                    int toIndexParameter = int.Parse(leaveApplicationDetailsString.Substring(toIndex, 1));
                    if (fromIndex == toIndex)
                    {
                        if (leaveApplicationDetailsString.IndexOf(">=") != -1)
                        {
                            foreach (var record in records)
                            {
                                var stringDateCompareFrom = parameters.PredicateParameters.ElementAt(fromIndexParameter).ToString();
                                var dateCompareFrom = DateTimeOffset.ParseExact(stringDateCompareFrom, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                                record.LeaveApplicationDetails = record.LeaveApplicationDetails.Where(x => x.FromDate >= dateCompareFrom).ToList();
                            }
                        }
                        else if (leaveApplicationDetailsString.IndexOf("<") != -1)
                        {
                            foreach (var record in records)
                            {
                                var stringDateCompareTo = parameters.PredicateParameters.ElementAt(toIndexParameter).ToString();
                                var dateCompareTo = DateTimeOffset.ParseExact(stringDateCompareTo, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                                record.LeaveApplicationDetails = record.LeaveApplicationDetails.Where(x => x.ToDate < dateCompareTo).ToList();
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

                            record.LeaveApplicationDetails = record.LeaveApplicationDetails.Where(
                                x => (x.FromDate >= dateCompareFrom && x.FromDate < dateCompareTo) || (x.ToDate >= dateCompareFrom && x.ToDate < dateCompareTo)
                                || (dateCompareFrom >= x.FromDate && dateCompareFrom < x.ToDate) || (dateCompareTo > x.FromDate && dateCompareTo <= x.ToDate)
                                ).ToList();
                        }
                    }
                }
                var exportLeaveApplicationViewModels = new List<ExportLeaveApplicationViewModel>();
                foreach (var _record in records)
                {
                    var record = Mapper.Map<LeaveApplicationViewModel>(_record);
                    if (_record.LeaveApplicationDetails.Any())
                    {
                        var details = _record.LeaveApplicationDetails;
                        if (details.Any())
                        {
                            foreach (var item in details)
                            {
                                var detail = Mapper.Map<LeaveApplicantDetailDTO>(item);
                                exportLeaveApplicationViewModels.Add(new ExportLeaveApplicationViewModel
                                {
                                    ReferenceNumber = record.ReferenceNumber,
                                    Status = record.Status,
                                    DepartmentCode = !string.IsNullOrEmpty(record.DeptCode) ? record.DeptCode : record.DivisionCode,
                                    DepartmentName = !string.IsNullOrEmpty(record.DeptName) ? record.DeptName : record.DivisionName,
                                    FullName = record.CreatedByFullName,
                                    SAPCode = record.UserSAPCode,
                                    LeaveCode = detail.LeaveCode,
                                    LeaveName = string.Format("{0} - {1}",detail.LeaveName, detail.LeaveCode),
                                    FromDate = detail.FromDate,
                                    ToDate = detail.ToDate,
                                    Quantity = detail.Quantity,
                                    Reason = detail.Reason,
                                    CreateDate = record.Created
                                });
                            }
                        }
                    }
                }

                for (int rowNum = 0; rowNum < exportLeaveApplicationViewModels.Count(); rowNum++)
                {
                    DataRow row = tbl.Rows.Add();
                    var data = exportLeaveApplicationViewModels.ElementAt(rowNum);
                    for (int j = 0; j < fieldMappings.Count; j++)
                    {
                        try
                        {
                            var fieldMapping = fieldMappings[j];
                            var value = data.GetType().GetProperty(fieldMapping.Name).GetValue(data);
                            HandleCommonType(row, value, j, fieldMapping);
                        }
                        catch(Exception ex)
                        {

                        }
                        
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
