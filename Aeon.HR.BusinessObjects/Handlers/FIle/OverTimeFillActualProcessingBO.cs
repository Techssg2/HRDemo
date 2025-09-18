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
    public class OverTimeFillActualProcessingBO : BaseExcelProcessing, IExcelProcessingBO
    {
        protected override string Json => "export-mapping.json";
        protected override string JsonGroupName => "OvertimeFillActualDetails";
        public OverTimeFillActualProcessingBO(IUnitOfWork uow) : base(uow)
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
                DataColumn currentColumn = tbl.Columns.Add(headerItem);
                currentColumn.DataType = System.Type.GetType("System.String");
            }
            // Add Row
            var records = await _uow.GetRepository<OvertimeApplication>().GetSingleAsync(x=>x.ReferenceNumber == parameters.Predicate);
            if (records != null && records.OvertimeItems != null && records.OvertimeItems.Count > 0)
            {
                var exportOvertimeItems = records.OvertimeItems;
                int count = exportOvertimeItems.Count;

                for (int rowNum = 0; rowNum < count; rowNum++)
                {
                    DataRow row = tbl.Rows.Add();
                    var data = exportOvertimeItems.ElementAt(rowNum);
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
            var creatingExcelFileReslult = ExportExcel(tbl, true);
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
            if (isHQ && float.Parse(result) >= 5)
            {
                result = (float.Parse(result) - 1).ToString();
            }
            return result;
        }
    }
}
