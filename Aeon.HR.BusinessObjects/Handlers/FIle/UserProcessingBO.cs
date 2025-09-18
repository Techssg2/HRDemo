using Aeon.HR.BusinessObjects.Handlers.File;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Enums;
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

namespace Aeon.HR.BusinessObjects.Handlers.File
{
    public class UserProcessingBO : BaseExcelProcessing, IExcelProcessingBO
    {
        protected override string Json => "export-mapping.json";
        protected override string JsonGroupName => "User";
        public UserProcessingBO(IUnitOfWork uow) : base(uow)
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
            var users = await _uow.GetRepository<User>().FindByAsync<UserListViewModel>(parameters.Order, 1, Int32.MaxValue, parameters.Predicate, parameters.PredicateParameters);
            if (users.Any())
            {
                for (int rowNum = 0; rowNum < users.Count(); rowNum++)
                {
                    DataRow row = tbl.Rows.Add();
                    var data = users.ElementAt(rowNum);
                    for (int j = 0; j < fieldMappings.Count; j++)
                    {
                        var fieldMapping = fieldMappings[j];
                        if (fieldMapping.Name == "LoginType")
                        {
                            //set role 
                            if (data.Type == LoginType.ActiveDirectory)
                            {
                                data.LoginType = "AD";
                            }
                            if (data.Type == LoginType.Membership)
                            {
                                data.LoginType = "MS";
                            }
                        }
                        if (fieldMapping.Name == "ModulePermission")
                        {
                            // set permission
                            if (data.Role == UserRole.SAdmin)
                            {
                                data.ModulePermission = "SAdmin";
                            }
                            if (data.Role == UserRole.Admin)
                            {
                                data.ModulePermission = "Admin";
                            }
                            if (data.Role == UserRole.HR)
                            {
                                data.ModulePermission = "HR";
                            }
                            if (data.Role == UserRole.CB)
                            {
                                data.ModulePermission = "CB";
                            }
                            if (data.Role == UserRole.Member)
                            {
                                data.ModulePermission = "Member";
                            }
                        }
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
