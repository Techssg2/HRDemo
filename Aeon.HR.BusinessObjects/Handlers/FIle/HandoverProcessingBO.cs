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
    public class HandoverProcessingBO : BaseExcelProcessing, IExcelProcessingBO
    {
        protected override string Json => "export-mapping.json";
        protected override string JsonGroupName => "Handover";
        public HandoverProcessingBO(IUnitOfWork uow) : base(uow)
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
            var records = await _uow.GetRepository<Handover>().FindByAsync(parameters.Order, parameters.Page, parameters.Limit, parameters.Predicate, parameters.PredicateParameters);
            if (records.Any())
            {

                var exportHandoverViewModels = new List<ExportHanvoderViewModel>();
                foreach (var _record in records)
                {
                    var record = Mapper.Map<HandoverViewModel>(_record);
                    if (_record.HandoverDetailItems.Any())
                    {
                        var details = _record.HandoverDetailItems;
                        if (details.Any())
                        {
                            foreach (var item in details)
                            {
                                if(item.IsDeleted == false)
                                {
                                    string cancel = "non-cancel";
                                    if(record.IsCancel)
                                    {
                                        cancel = "cancel";
                                    } 
                                    var detail = Mapper.Map<HandoverItemDetailViewModel>(item);
                                    exportHandoverViewModels.Add(new ExportHanvoderViewModel
                                    {
                                        ReferenceNumber = record.ReferenceNumber,
                                        ApplicantReferenceNumber = record.ApplicantReferenceNumber,
                                        ApplicantFullName = record.ApplicantFullName,
                                        Department = record.UserDeptName,
                                        ReceivedDate = record.ReceivedDate,
                                        Name = detail.Name,
                                        SerialNumber = detail.SerialNumber,
                                        Unit = detail.Unit,
                                        Quantity = detail.Quantity,
                                        Notes = detail.Notes,
                                        CreatedDate = record.Created.ToString("dd/MM/yyyy HH:mm:ss"),
                                        IsCancel = cancel
                                    }); ;
                                }
                            }
                        }
                    }
                }

                for (int rowNum = 0; rowNum < exportHandoverViewModels.Count(); rowNum++)
                {
                    DataRow row = tbl.Rows.Add();
                    var data = exportHandoverViewModels.ElementAt(rowNum);
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

