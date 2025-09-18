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
using Aeon.HR.Infrastructure.Enums;
using System.IO;

namespace Aeon.HR.BusinessObjects.Handlers.FIle
{
    public class MaintainPromoteAndTranferPrintProcessingBO : BaseExcelProcessing, IExcelProcessingBO
    {
        protected override string Json => "export-mapping.json";
        protected override string JsonGroupName => "MaintainPromoteAndTranferPrint";
        public MaintainPromoteAndTranferPrintProcessingBO(IUnitOfWork uow) : base(uow)
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
            var removingValue = await _uow.GetRepository<MaintainPromoteAndTranferPrint>().FindByAsync<PromoteAndTranferPrintViewModel>(parameters.Order, 1, Int32.MaxValue, parameters.Predicate, parameters.PredicateParameters);
            if (removingValue.Any())
            {
                for (int rowNum = 0; rowNum < removingValue.Count(); rowNum++)
                {
                    DataRow row = tbl.Rows.Add();
                    var data = removingValue.ElementAt(rowNum);
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