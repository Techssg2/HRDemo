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
    public class PositionProcessingBO : BaseExcelProcessing, IExcelProcessingBO
    {
        protected override string Json => "export-mapping.json";
        protected override string JsonGroupName => "Position";
        public PositionProcessingBO(IUnitOfWork uow) : base(uow)
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
            var records = await _uow.GetRepository<Position>().FindByAsync(parameters.Order, parameters.Page, parameters.Limit, parameters.Predicate, parameters.PredicateParameters);
            if (records.Any())
            {

                var exporPositionViewModels = new List<ExportPosition>();
                foreach (var _record in records)
                {
                    var record = Mapper.Map<PositionExportViewModel>(_record);
                    if(record.Status == PositionStatus.None)
                    {
                        record.StatusName = "None";
                    }
                    if (record.Status == PositionStatus.Opened)
                    {
                        record.StatusName = "Opened";
                    }
                    if (record.Status == PositionStatus.Closed)
                    {
                        record.StatusName = "Closed";
                    }
                    if (record.Status == PositionStatus.Draft)
                    {
                        record.StatusName = "Draft";
                    }
                    exporPositionViewModels.Add(new ExportPosition
                    {
                        Status = record.StatusName,
                        ReferenceNumber = record.ReferenceNumber,
                        Position = record.PositionName,
                        //UpdateDate = record.Created,
                        RequestToHireNumber = record.RequestToHireNumber,
                        Department = !String.IsNullOrEmpty(record.DeptDivisionName) ? record.DeptDivisionCode + " - " + record.DeptDivisionName : "",
                        Location = record.LocationName,
                        ExpiredDate = record.ExpiredDate,
                        Applicants = record.ApplicantsCount,
                        Hired = record.HiredApplicantsCount,
                        Required = record.Quantity,
                        Assignee = record.AssignToFullName
                    });
                }

                for (int rowNum = 0; rowNum < exporPositionViewModels.Count(); rowNum++)
                {
                    DataRow row = tbl.Rows.Add();
                    var data = exporPositionViewModels.ElementAt(rowNum);
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
