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
    public class TargetPlanSpecialProcessingBO : BaseExcelProcessing, IExcelProcessingBO
    {
        protected override string Json => "export-mapping.json";
        protected override string JsonGroupName => "TargetPlanSpecial";
        public TargetPlanSpecialProcessingBO(IUnitOfWork uow) : base(uow)
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
            var records = new List<TargetPlanSpecialViewModel>();
            var userDepartments = await _uow.GetRepository<TargetPlanSpecialDepartmentMapping>().FindByAsync(parameters.Predicate, parameters.PredicateParameters, parameters.Order, x => x.Department);
            var userGroupByDepartment = userDepartments.GroupBy(x => x.DepartmentId).ToList();
            foreach (var department in userGroupByDepartment)
            {
                var targetPlanSpecial = new TargetPlanSpecialViewModel();
                //List<string> userNames = new List<string>();
                targetPlanSpecial.DepartmentId = department.Key;
                foreach (var user in department)
                {
                    targetPlanSpecial.DepartmentName = user.Department.Name +"-"+ user.Department.Code;
                    //userNames.Add(user.User.FullName);
                }
                //targetPlanSpecial.UserNames = userNames;
                records.Add(targetPlanSpecial);
            }


            if (records.Any())
            {

                var exporttargetPlanSpecialViewModels = new List<ExportTargetPlanSpecialViewModel>();
                foreach (var record in records)
                {
                    exporttargetPlanSpecialViewModels.Add(new ExportTargetPlanSpecialViewModel
                    {
                        Department = record.DepartmentName,
                        //Users = string.Join(", ", record.UserNames)
                    });
                }

                for (int rowNum = 0; rowNum < exporttargetPlanSpecialViewModels.Count(); rowNum++)
                {
                    DataRow row = tbl.Rows.Add();
                    var data = exporttargetPlanSpecialViewModels.ElementAt(rowNum);
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
