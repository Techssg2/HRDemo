using Aeon.HR.BusinessObjects.Handlers.File;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.BTA;
using Aeon.HR.ViewModels.DTOs;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Handlers.FIle
{
    public class OverBudgetProcessingBO : BaseExcelProcessing, IExcelProcessingBO
    {
        protected override string Json => "export-mapping.json";
        protected override string JsonGroupName => "OverBudget";
        public OverBudgetProcessingBO(IUnitOfWork uow) : base(uow)
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
            var items = await _uow.GetRepository<BusinessTripOverBudget>(true).FindByAsync(parameters.Order, 1, Int32.MaxValue, parameters.Predicate, parameters.PredicateParameters);
            if (items.Any())
            {
                for (int rowNum = 0; rowNum < items.Count(); rowNum++)
                {
                    var data = items.ElementAt(rowNum);
                    //var overBudgetsViewModel = Mapper.Map<BTAOverBudgetViewModel>(data);
                    var btaParent = await _uow.GetRepository<BusinessTripApplication>(true).FindByIdAsync(data.BusinessTripApplicationId.Value);
                    var allOverDetails = await _uow.GetRepository<BusinessTripOverBudgetsDetail>(true).FindByAsync(x => x.BusinessTripOverBudgetId == data.Id);
                    foreach (var detail in allOverDetails)
                    {
                        DataRow row = tbl.Rows.Add();
                        var pricedItineraries = new PricedItinerariesViewModel();
                        for (int j = 0; j < fieldMappings.Count; j++)
                        {
                            var fieldMapping = fieldMappings[j];
                            if (fieldMapping.Name == "SAPCode")
                            {
                                var valueItem = detail.SAPCode;
                                HandleCommonType(row, valueItem, j, fieldMapping);
                                continue;
                            }
                            if (fieldMapping.Name == "ReferenceNumber")
                            {
                                var valueItem = data.ReferenceNumber;
                                HandleCommonType(row, valueItem, j, fieldMapping);
                                continue;
                            }
                            if (fieldMapping.Name == "FullName")
                            {
                                var valueItem = detail.FullName;
                                HandleCommonType(row, valueItem, j, fieldMapping);
                                continue;
                            }
                            if (fieldMapping.Name == "BTA_ReferenceNumber")
                            {
                                var valueItem = btaParent.ReferenceNumber;
                                HandleCommonType(row, valueItem, j, fieldMapping);
                                continue;
                            }
                            if (fieldMapping.Name == "DeptDivisionName")
                            {
                                var valueItem = detail.DepartmentName;
                                HandleCommonType(row, valueItem, j, fieldMapping);
                                continue;
                            }
                            if (fieldMapping.Name == "DeptName")
                            {
                                var valueItem = data.DeptName;
                                HandleCommonType(row, valueItem, j, fieldMapping);
                                continue;
                            }
                            if (fieldMapping.Name == "DepartureName")
                            {
                                var valueItem = detail.DepartureName;
                                HandleCommonType(row, valueItem, j, fieldMapping);
                                continue;
                            }
                            if (fieldMapping.Name == "ArrivalName")
                            {
                                var valueItem = detail.ArrivalName;
                                HandleCommonType(row, valueItem, j, fieldMapping);
                                continue;
                            }
                            if (fieldMapping.Name == "FromDate")
                            {
                                //var valueItem = String.Format("{0:d/M/yyyy}", detail.FromDate.Value.LocalDateTime);
                                var valueItem = detail.FromDate.Value.LocalDateTime;
                                HandleCommonType(row, valueItem, j, fieldMapping);
                                continue;
                            }
                            if (fieldMapping.Name == "ToDate")
                            {
                                //var valueItem = String.Format("{0:d/M/yyyy}", detail.ToDate.Value.LocalDateTime);
                                var valueItem = detail.ToDate.Value.LocalDateTime;
                                HandleCommonType(row, valueItem, j, fieldMapping);
                                continue;
                            }
                            if (fieldMapping.Name == "OldBudget")
                            {
                                var valueItem = btaParent.IsRoundTrip ? detail.MaxBudgetAmount : detail.MaxBudgetAmount / 2;
                                HandleCommonType(row, valueItem, j, fieldMapping);
                                continue;
                            }
                            if (fieldMapping.Name == "NewBudget")
                            {
                                var valueItem = detail.ExtraBudget;
                                HandleCommonType(row, valueItem, j, fieldMapping);
                                continue;
                            }
                            if (fieldMapping.Name == "DeviantBudget")
                            {
                                var valueItem = detail.ExtraBudget - detail.MaxBudgetAmount;
                                HandleCommonType(row, valueItem, j, fieldMapping);
                                continue;
                            }
                            if (fieldMapping.Name == "Status")
                            {
                                var valueItem = data.Status;
                                HandleCommonType(row, valueItem, j, fieldMapping);
                                continue;
                            }
                            if (fieldMapping.Name == "BTAStatus")
                            {
                                var valueItem = btaParent.Status;
                                HandleCommonType(row, valueItem, j, fieldMapping);
                                continue;
                            }
                            if (fieldMapping.Name == "Created")
                            {
                                //var valueItem = String.Format("{0:d/M/yyyy}", data.Created.LocalDateTime);
                                var valueItem = data.Created.LocalDateTime;
                                HandleCommonType(row, valueItem, j, fieldMapping);
                                continue;
                            }

                            var value = data.GetType().GetProperty(fieldMapping.Name).GetValue(data);
                            HandleCommonType(row, value, j, fieldMapping);
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

        public Task<ResultDTO> ImportAsync(FileStream stream)
        {
            throw new NotImplementedException();
        }
    }
}
