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
    public class PromoteAndTransferProcessingBO : BaseExcelProcessing, IExcelProcessingBO
    {
        protected override string Json => "export-mapping.json";
        protected override string JsonGroupName => "PromteAndTransfer";
        public PromoteAndTransferProcessingBO(IUnitOfWork uow) : base(uow)
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
            var records = await _uow.GetRepository<PromoteAndTransfer>().FindByAsync(parameters.Order, parameters.Page, parameters.Limit, parameters.Predicate, parameters.PredicateParameters, x => x.NewDeptOrLine);
            if (records.Any())
            {

                var exportPromoteAndTransferViewModels = new List<ExportPromoteAndTransfer>();
                foreach (var _record in records)
                {
                    var record = Mapper.Map<PromoteAndTransferViewModel>(_record);
                    if (record.RequestFrom == "EMP")
                    {
                        record.RequestFrom = "Employee";
                    }
                    else
                    {
                        record.RequestFrom = "Manager";
                    }
                    if (record.TypeCode == "Pro")
                    {
                        record.TypeName = "Promotion";
                    }
                    else if (record.TypeCode == "Tran")
                    {
                        record.TypeName = "Transfer";
                    }
                    else if (record.TypeCode == "ProAndTran")
                    {
                        record.TypeName = "Promote and Transfer";
                    }
                    exportPromoteAndTransferViewModels.Add(new ExportPromoteAndTransfer
                    {
                        Status = record.Status,
                        ReferenceNumber = record.ReferenceNumber,
                        Type = record.TypeName,
                        RequestFrom = record.RequestFrom,
                        FullName = record.UserFullName,
                        SapCode = record.UserSAPCode,
                        CurrentPosittion = record.CurrentTitle,
                        CurrentJobGrade = record.CurrentJobGrade,
                        CurrentDepartment = record.CurrentDepartment,
                        CurrentWorkLocation = record.CurrentWorkLocation,
                        //CR222============================================
                        PersonnelArea = record.PersonnelArea,
                        PersonnelAreaText = record.PersonnelAreaText,
                        EmployeeGroup = record.EmployeeGroup,
                        EmployeeGroupDescription = record.EmployeeGroupDescription,
                        EmployeeSubgroup = record.EmployeeSubgroup,
                        EmployeeSubgroupDescription = record.EmployeeSubgroupDescription,
                        PayScaleArea = record.PayScaleArea,
                        //=================================================
                        EffectiveDate = record.EffectiveDate,
                        NewSalaryBenefits = record.NewSalaryOrBenefit,
                        NewPosition = record.PositionName,
                        NewDeptLine = record.NewDeptOrLineName,
                        NewJobGrade = record.NewJobGradeName,
                        NewWorkLocation = record.NewWorkLocationName,
                        ReportTo = record.ReportToSAPCode + " - " + record.ReportToFullName,
                        ReasonOfPromotionTransfer = record.ReasonOfPromotion,
                        CreatedDate = record.Created.ToString("dd/MM/yyyy HH:mm:ss"),
                        CreatedByFullName = record.CreatedByFullName
                    });
                }

                for (int rowNum = 0; rowNum < exportPromoteAndTransferViewModels.Count(); rowNum++)
                {
                    DataRow row = tbl.Rows.Add();
                    var data = exportPromoteAndTransferViewModels.ElementAt(rowNum);
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
