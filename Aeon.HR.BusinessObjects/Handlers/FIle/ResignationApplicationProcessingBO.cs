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
using Aeon.HR.Infrastructure.Enums;
using System.IO;

namespace Aeon.HR.BusinessObjects.Handlers.FIle
{
    public class ResignationApplicationProcessingBO : BaseExcelProcessing, IExcelProcessingBO
    {
        protected override string Json => "export-mapping.json";
        protected override string JsonGroupName => "ResignationApplication";
        public ResignationApplicationProcessingBO(IUnitOfWork uow) : base(uow)
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
            var resignationApplications = await _uow.GetRepository<ResignationApplication>().FindByAsync<ResignationApplicationViewModel>(parameters.Order, parameters.Page, parameters.Limit, parameters.Predicate, parameters.PredicateParameters);
            if (resignationApplications.Any())
            {
                for (int rowNum = 0; rowNum < resignationApplications.Count(); rowNum++)
                {
                    DataRow row = tbl.Rows.Add();
                    var data = resignationApplications.ElementAt(rowNum);
                    for (int j = 0; j < fieldMappings.Count; j++)
                    {
                        var fieldMapping = fieldMappings[j];
                        var value = data.GetType().GetProperty(fieldMapping.Name).GetValue(data);
                        if(fieldMapping.Name.Equals("SubmitDate") && value == null)
                        {
                            var instance = await _uow.GetRepository<WorkflowInstance>().FindByAsync(x => x.ItemId == data.Id);
                            if (instance != null)
                            {
                                var instanceListId = instance.Select(x => x.Id).ToList();
                                var histories = await _uow.GetRepository<WorkflowHistory>().FindByAsync<WorkflowHistoryViewModel>(x => instanceListId.Contains(x.InstanceId) && x.Outcome == "Submitted", "Created asc");
                                if (histories != null && histories.Any())
                                {
                                    var firstHistory = histories.FirstOrDefault();
                                    if (firstHistory != null)
                                    {
                                        value = firstHistory.Modified.Date;
                                    }
                                }
                            }
                        }
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
