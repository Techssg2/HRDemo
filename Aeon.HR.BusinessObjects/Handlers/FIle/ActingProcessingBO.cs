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

namespace Aeon.HR.BusinessObjects.Handlers.FIle
{
    public class ActingProcessingBO : BaseExcelProcessing, IExcelProcessingBO
    {
        protected override string Json => "export-mapping.json";
        protected override string JsonGroupName => "Acting";
        public ActingProcessingBO(IUnitOfWork uow) : base(uow)
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
            // fix 652
            //var records = await _uow.GetRepository<Acting>().FindByAsync(parameters.Order, parameters.Page, parameters.Limit, parameters.Predicate, parameters.PredicateParameters);
            var records = await _uow.GetRepository<Acting>().FindByAsync(parameters.Order, parameters.Page, int.MaxValue, parameters.Predicate, parameters.PredicateParameters);
            //
            if (records.Any())
            {

                var exportActingViewModels = new List<ExportActing>();
                foreach (var _record in records)
                {
                    var record = Mapper.Map<ActingExportViewModel>(_record);
                    exportActingViewModels.Add(new ExportActing
                    {
                        Status = record.Status,
                        ReferenceNumber = record.ReferenceNumber,
                        FullName = record.FullName,
                        SapCode = record.UserSAPCode,
                        DeptLine = record.DeptName,
                        Division = record.DivisionName,
                        WorkLocation = record.WorkLocationName,
                        PositionInActingPeriod = record.TitleInActingPeriodName,
                        CreatedDate = record.Created.ToString("dd/MM/yyyy HH:mm:ss"),
                        CreatedByFullName = record.CreatedByFullName
                    }); ;
                }

                for (int rowNum = 0; rowNum < exportActingViewModels.Count(); rowNum++)
                {
                    DataRow row = tbl.Rows.Add();
                    var data = exportActingViewModels.ElementAt(rowNum);
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
