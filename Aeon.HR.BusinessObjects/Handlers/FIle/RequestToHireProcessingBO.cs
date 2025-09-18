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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aeon.HR.API.Helpers;
using Aeon.HR.BusinessObjects.Helpers;
using System.IO;

namespace Aeon.HR.BusinessObjects.Handlers.File
{
    public class RequestToHireProcessingBO : BaseExcelProcessing, IExcelProcessingBO
    {
        protected override string Json => "export-mapping.json";
        protected override string JsonGroupName => "RequestToHire";
        public RequestToHireProcessingBO(IUnitOfWork uow) : base(uow)
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
            var requestToHires = await _uow.GetRepository<RequestToHire>().FindByAsync<RequestToHireViewModel>(parameters.Predicate, parameters.PredicateParameters, "Created desc");
            if (requestToHires.Any())
            {
                for (int rowNum = 0; rowNum < requestToHires.Count(); rowNum++)
                {
                    DataRow row = tbl.Rows.Add();
                    var data = requestToHires.ElementAt(rowNum);
                    data.Type = data.ReplacementFor.GetEnumDescription();
                    if (data.Status == "Completed")
                    {
                        data.AssigneeUserName = data.AssignToSAPCode + " - " + data.AssignToFullName;
                    }
                    //lamnl
                    var getDepartment = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == data.DeptDivisionId);
                    if (getDepartment != null && getDepartment.RegionId != null)
                    {
                        data.RegionName = getDepartment.Region.RegionName;
                    }
                    // lamnl
                    if (data.ReplacementFor == TypeOfNeed.ReplacementFor)
                    {
                        data.DeptDivisionName = data.ReplacementForName;
                        data.HasBudget = CheckBudgetOption.Budget;
                        if (null != data.ReplacementForUserId)
                        {
                            data.ReplacementForUserName = data.ReplacementForUserSAPCode + " - " + data.ReplacementForUserFullName;
                        }
                    }
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
