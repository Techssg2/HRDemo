using Aeon.HR.BusinessObjects.Handlers.File;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Handlers.FIle
{
    public class PositionDetailProcessingBO : BaseExcelProcessing, IExcelProcessingBO
    {
        protected override string Json => "export-mapping.json";

        protected override string JsonGroupName => "PositionDetail";
        public PositionDetailProcessingBO(IUnitOfWork uow) : base(uow)
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
            var records = await _uow.GetRepository<Applicant>().FindByAsync<PrositionApplicantViewModel>(parameters.Order, parameters.Page, parameters.Limit, parameters.Predicate, parameters.PredicateParameters);
            if (records.Any())
            {

                var exportPositionDetails = new List<ExportPrositionDetailViewModel>();
                foreach (var _record in records)
                {
                    exportPositionDetails.Add(new ExportPrositionDetailViewModel
                    {
                        ReferenceNumber = _record.ReferenceNumber,
                        FullName = _record.FullName,
                        Mobile = _record.Mobile,
                        ApplicantStatusName = _record.ApplicantStatusName,
                        AppreciationName = _record.AppreciationName,
                        InChargePerson = _record.InChargePerson,
                        SAPReviewStatus = _record.SAPReviewStatus,
                        Created = _record.Created.ToString("dd/MM/yyyy HH:mm:ss"),
                        StartDate = _record.StartDate.Value.ToString("dd/MM/yyyy HH:mm:ss"),
                        Email = _record.Email
                    }); ;
                }

                for (int rowNum = 0; rowNum < exportPositionDetails.Count(); rowNum++)
                {
                    DataRow row = tbl.Rows.Add();
                    var data = exportPositionDetails.ElementAt(rowNum);
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
