using Aeon.HR.BusinessObjects.Handlers.File;
using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.BTA;
using Aeon.HR.ViewModels.DTOs;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TargetPlanTesting.ImportData;

namespace Aeon.HR.BusinessObjects.Handlers.FIle
{
    public class TargetPlanProcessingBO : BaseExcelProcessing, IExcelProcessingBO
    {
        protected override string Json => "export-mapping.json";
        protected override string JsonGroupName => "TargetPlan";
        public TargetPlanProcessingBO(IUnitOfWork uow) : base(uow)
        {

        }

        public Task<ResultDTO> ImportAsync(FileStream stream)
        {
            throw new NotImplementedException();
        }
        public async Task<ResultDTO> ExportAsync(QueryArgs arg)
        {
            var fieldMappings = ReadConfigurationFromFile();
            var headers = fieldMappings.Select(y => y.DisplayName);
            // Create Headers
            DataTable tbl = new DataTable();
            foreach (var headerItem in headers)
            {
                tbl.Columns.Add(headerItem);
            }
            //xử lý data
            var targetPlans = await _uow.GetRepository<TargetPlan>().FindByAsync<TargetPlanViewModel>(arg.Order, arg.Page, arg.Limit, arg.Predicate, arg.PredicateParameters);
            if (targetPlans.Any())
            {
                var exportTargetPlans = new List<TargetPlanExportViewModel>();
                var targetPlanIds = targetPlans.Select(x => x.Id).ToList();
                var allTargetPlanDetails = await _uow.GetRepository<TargetPlanDetail>().FindByAsync<TargetPlanDetailViewModel>(x => targetPlanIds.Contains(x.TargetPlanId));
                foreach (var item in allTargetPlanDetails)
                {
                    exportTargetPlans.Add(new TargetPlanExportViewModel
                    {
                        Status = item.Status,
                        ReferenceNumber = item.ReferenceNumber,
                        SubmitterSAPCode = item.SubmitterSAPCode,
                        SubmitterFullName = item.SubmitterFullName,
                        SAPCode = item.SAPCode,
                        FullName = item.FullName,
                        DeptLine = item.DeptLine,
                        DivisionGroup = item.DivisionGroup,
                        Department = item.DepartmentName,
                        Period = item.Period,
                        Created = String.Format("{0:d/M/yyyy}", item.Created.LocalDateTime),
                        Modified = String.Format("{0:d/M/yyyy}", item.Modified.LocalDateTime)
                    });
                }
                if (exportTargetPlans.Any())
                {
                    exportTargetPlans = exportTargetPlans.GroupBy(x => new { x.SAPCode, x.ReferenceNumber }).Select(y => y.FirstOrDefault()).ToList();
                    for (int rowNum = 0; rowNum < exportTargetPlans.Count(); rowNum++)
                    {
                        DataRow row = tbl.Rows.Add();
                        var data = exportTargetPlans.ElementAt(rowNum);
                        for (int j = 0; j < fieldMappings.Count; j++)
                        {
                            var fieldMapping = fieldMappings[j];
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
            //
            var creatingExcelFileReslult = ExportExcel(tbl);
            if (creatingExcelFileReslult == null)
            {
                return new ResultDTO { ErrorCodes = { 1003 }, Messages = { "No Data" } };
            }
            return new ResultDTO { Object = creatingExcelFileReslult };
        }
    }
}
