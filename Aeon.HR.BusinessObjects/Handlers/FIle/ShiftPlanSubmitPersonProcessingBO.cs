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
    public class ShiftPlanSubmitPersonProcessingBO : BaseExcelProcessing, IExcelProcessingBO
    {
        protected override string Json => "export-mapping.json";
        protected override string JsonGroupName => "ShiftPlanSubmitPerson";
        public ShiftPlanSubmitPersonProcessingBO(IUnitOfWork uow) : base(uow)
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
            var records = new List<ShiftPlanSubmitPersonViewModel>();
            var userDepartments = await _uow.GetRepository<UserSubmitPersonDeparmentMapping>().FindByAsync(parameters.Predicate, parameters.PredicateParameters, parameters.Order, x => x.Department, y => y.User);
            var userGroupByDepartment = userDepartments.GroupBy(x => x.DepartmentId).ToList();
            foreach (var department in userGroupByDepartment)
            {
                var shiftPlanSubmitPerson = new ShiftPlanSubmitPersonViewModel();
                List<string> userNames = new List<string>();
                shiftPlanSubmitPerson.DepartmentId = department.Key;
                foreach (var user in department)
                {
                    shiftPlanSubmitPerson.DepartmentName = user.Department.Name;
                    userNames.Add(user.User.FullName);
                }
                shiftPlanSubmitPerson.UserNames = userNames;
                records.Add(shiftPlanSubmitPerson);
            }


            if (records.Any())
            {

                var exportShiftPlanSubmitPersonViewModels = new List<ExportShiftPlanSubmitPersonViewModel>();
                foreach (var record in records)
                {
                    exportShiftPlanSubmitPersonViewModels.Add(new ExportShiftPlanSubmitPersonViewModel
                    {
                        Department = record.DepartmentName,
                        Users = string.Join(", ", record.UserNames)
                    });
                }

                for (int rowNum = 0; rowNum < exportShiftPlanSubmitPersonViewModels.Count(); rowNum++)
                {
                    DataRow row = tbl.Rows.Add();
                    var data = exportShiftPlanSubmitPersonViewModels.ElementAt(rowNum);
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
