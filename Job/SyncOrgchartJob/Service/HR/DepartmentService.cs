using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SyncOrgchartJob.Models.eDocHR;

namespace SyncOrgchartJob.Service
{
    public static class DepartmentService
    {
        private static SqlConnection _connection;

        public static async Task ProcessSyncData(this List<Department> models, string action, SqlConnection conn,
            SqlTransaction sqlTransaction)
        {
            _connection = conn;
            foreach (var model in models)
            {
                switch (action)
                {
                    case "Add":
                        await model.CreateDepartment(sqlTransaction);
                        break;
                    case "Update":
                        await model.UpdateDepartment(sqlTransaction);
                        break;
                    case "Delete":
                        // await model.DeleteDepartment(sqlTransaction);
                        await model.UpdatePrepareDeleteDepartment(sqlTransaction);
                        break;
                }
                await model.SaveLogsSyncDepartment(action, conn, sqlTransaction);
            }
        }
        
        private static async Task CreateDepartment(this Department model, SqlTransaction sqlTransaction)
        {
            try
            {
                if (model.NewData.ParentId == null || (model.NewData.ParentId != null && model.NewData.ParentId.Value == Guid.Empty))
                {
                    model.NewData.ErrorList.Add("ParentId is null or empty");
                    Logger.Write($"CreateDepartment: ParentId is null or empty for {model.NewData.Code} - {model.NewData.Name}", true);
                    return; 
                }
                
                if (model.SAPCode.Equals("50026270"))
                {
                    var data = "";
                }
                    
                if (model.SAPCode.Equals("50037997"))
                {
                    var data = "";
                }
                    
                if (model.SAPCode.Equals("40001021"))
                {
                    var data = "";
                }
                
                var query = @"
                    INSERT INTO Departments (
                        Id, Code, Name, PositionCode, PositionName, Type, JobGradeId, ParentId, IsStore, SAPCode, RegionId, SAPObjectId, SAPObjectType, SAPDepartmentParentName, SAPDepartmentParentId, SAPLevel, SAPValidFrom, SAPValidTo, BusinessModelId
                        ,IsHR, IsDeleted, IsPerfomance, IsCB, IsAdmin,IsFacility, EnableForPromoteActing, IsMD, IsSM, IsAcademy, IsFromIT, IsMMD, ApplyChildSMMDMMD, IsEdoc, Created, Modified, ModifiedById
                    ) VALUES (
                        @Id, @Code, @Name, @PositionCode, @PositionName, @Type, @JobGradeId, @ParentId, @IsStore, @SAPCode, @RegionId, @SAPObjectId, @SAPObjectType, 
                              @SAPDepartmentParentName, @SAPDepartmentParentId, @SAPLevel, @SAPValidFrom, @SAPValidTo, @BusinessModelId,
                              @IsHR, @IsDeleted, @IsPerfomance, @IsCB, @IsAdmin, @IsFacility, @EnableForPromoteActing, @IsMD, @IsSM, @IsAcademy, @IsFromIT, @IsMMD, @ApplyChildSMMDMMD, @IsEdoc,
                              @Created, @Modified, @ModifiedById
                    )";
                
                using (var cmd = new SqlCommand(query, _connection, sqlTransaction))
                {
                    cmd.Transaction = sqlTransaction;
                    cmd.Parameters.AddWithValue("@Id", model.NewData.Id);
                    cmd.Parameters.AddWithValue("@Code", model.NewData.Code ?? (object) DBNull.Value);
                    cmd.Parameters.AddWithValue("@Name", model.NewData.Name ?? (object) DBNull.Value);
                    cmd.Parameters.AddWithValue("@PositionCode", model.NewData.PositionCode ?? (object) DBNull.Value);
                    cmd.Parameters.AddWithValue("@PositionName", model.NewData.PositionName ?? (object) DBNull.Value);
                    cmd.Parameters.AddWithValue("@Type", (object) model.NewData.Type ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@JobGradeId", model.NewData.JobGradeId);
                    cmd.Parameters.AddWithValue("@ParentId", (object) model.NewData.ParentId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsStore", model.NewData.IsStore);
                    cmd.Parameters.AddWithValue("@SAPCode", model.NewData.SAPCode ?? (object) DBNull.Value);
                    cmd.Parameters.AddWithValue("@RegionId", (object)model.NewData.RegionId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@SAPObjectId", model.NewData.SAPObjectId ?? (object) DBNull.Value);
                    cmd.Parameters.AddWithValue("@SAPObjectType", model.NewData.SAPObjectType ?? (object) DBNull.Value);
                    cmd.Parameters.AddWithValue("@SAPDepartmentParentName", model.NewData.SAPDepartmentParentName ?? (object) DBNull.Value);
                    cmd.Parameters.AddWithValue("@SAPDepartmentParentId", model.NewData.SAPDepartmentParentId ?? (object) DBNull.Value);
                    cmd.Parameters.AddWithValue("@SAPLevel", model.NewData.SAPLevel ?? (object) DBNull.Value);
                    cmd.Parameters.AddWithValue("@SAPValidFrom", model.NewData.SAPValidFrom ?? (object) DBNull.Value);
                    cmd.Parameters.AddWithValue("@SAPValidTo", model.NewData.SAPValidTo ?? (object) DBNull.Value);
                    cmd.Parameters.AddWithValue("@BusinessModelId", (object) model.NewData.BusinessModelId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsHR", false);
                    cmd.Parameters.AddWithValue("@IsDeleted", false);
                    cmd.Parameters.AddWithValue("@IsPerfomance", false);
                    cmd.Parameters.AddWithValue("@IsAdmin", false);
                    cmd.Parameters.AddWithValue("@IsFacility", false);
                    cmd.Parameters.AddWithValue("@EnableForPromoteActing", false);
                    cmd.Parameters.AddWithValue("@IsCB", false);
                    cmd.Parameters.AddWithValue("@IsMD", false);
                    cmd.Parameters.AddWithValue("@IsSM", false);
                    cmd.Parameters.AddWithValue("@IsAcademy", false);
                    cmd.Parameters.AddWithValue("@IsFromIT", false);
                    cmd.Parameters.AddWithValue("@IsMMD", false);
                    cmd.Parameters.AddWithValue("@ApplyChildSMMDMMD", false);
                    cmd.Parameters.AddWithValue("@IsEdoc", false);
                    cmd.Parameters.AddWithValue("@Created", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Modified", DateTime.Now);
                    cmd.Parameters.AddWithValue("@ModifiedById", Guid.Parse("99A2F491-3103-4C01-9F6E-43359F3FBCD7"));
                    int rs = await cmd.ExecuteNonQueryAsync();
                    Logger.Write($"Insert Department: {model.Id} - {model.SAPCode} - {model.NewData.Code} - {model.NewData.Name}", true);
                    model.NewData.Status = rs > 0;
                }
            }
            catch (Exception ex)
            {
                model.NewData.ErrorList.Add("Insert Department" + ex.Message);
                Logger.Write($"Error inserting Department: {JsonConvert.SerializeObject(model)} - {ex.Message}", true);
                Logger.Write($"Stack Trace: {ex.StackTrace}", true); 
                //throw new Exception($"Error inserting Department: {model.SAPCode} - {model.Name} - {ex.Message}");
            }
        }
        
        private static async Task UpdateDepartment(this Department model, SqlTransaction sqlTransaction)
        {
            try
            {
                if (model.NewData.ParentId == null || model.NewData.ParentId == Guid.Empty)
                {
                    model.NewData.ErrorList.Add("ParentId is null or empty");
                    return;
                }

                var updates = new List<string>();
                var parameters = new List<SqlParameter>();

                void AddParam(string column, string paramName, object value)
                {
                    if (value != null)
                    {
                        updates.Add($"{column} = @{paramName}");
                        parameters.Add(new SqlParameter("@" + paramName, value));
                    }
                }

                //AddParam("Code", "Code", model.NewData.Code);
                AddParam("Name", "Name", model.NewData.Name);
                AddParam("PositionCode", "PositionCode", model.NewData.PositionCode);
                AddParam("PositionName", "PositionName", model.NewData.PositionName);
                AddParam("Type", "Type", model.NewData.Type);
                AddParam("JobGradeId", "JobGradeId", model.NewData.JobGradeId);
                AddParam("IsStore", "IsStore", model.NewData.IsStore);
                if (!model.NewData.IsParentIsEdoc)
                {
                    AddParam("ParentId", "ParentId", model.NewData.ParentId);
                }
                //AddParam("SAPCode", "SAPCode", model.NewData.SAPCode);
                AddParam("RegionId", "RegionId", model.NewData.RegionId);
                AddParam("SAPObjectId", "SAPObjectId", model.NewData.SAPObjectId);
                AddParam("SAPObjectType", "SAPObjectType", model.NewData.SAPObjectType);
                AddParam("SAPDepartmentParentName", "SAPDepartmentParentName", model.NewData.SAPDepartmentParentName);
                AddParam("SAPDepartmentParentId", "SAPDepartmentParentId", model.NewData.SAPDepartmentParentId);
                AddParam("SAPLevel", "SAPLevel", model.NewData.SAPLevel);
                AddParam("SAPValidFrom", "SAPValidFrom", model.NewData.SAPValidFrom);
                AddParam("SAPValidTo", "SAPValidTo", model.NewData.SAPValidTo);
                AddParam("BusinessModelId", "BusinessModelId", model.NewData.BusinessModelId);
                AddParam("IsPrepareDelete", "IsPrepareDelete", false);

                if (updates.Count == 0)
                {
                    model.NewData.ErrorList.Add("No fields to update");
                    return;
                }

                var updateClause = string.Join(", ", updates);
                var query = $"UPDATE Departments SET {updateClause} WHERE Id = @Id";
                parameters.Add(new SqlParameter("@Id", model.Id));

                using (var cmd = new SqlCommand(query, _connection, sqlTransaction))
                {
                    cmd.Transaction = sqlTransaction;
                    cmd.Parameters.AddRange(parameters.ToArray());

                    int rs = await cmd.ExecuteNonQueryAsync();
                    Logger.Write($"Update Department: {model.Id} - {model.SAPCode} - {model.Code} - {model.Name}", true);
                    model.NewData.Status = rs > 0;
                }
            }
            catch (Exception ex)
            {
                model.NewData.ErrorList.Add("Update Department: " + ex.Message);
                Logger.Write($"Error updating Department: {JsonConvert.SerializeObject(model)} - {ex.Message}", true);
                Logger.Write($"Stack Trace: {ex.StackTrace}", true);
                throw new Exception($"Error updating Department: {model.SAPCode} - {model.Name} - {ex.Message}");
            }
        }
        
        private static async Task UpdatePrepareDeleteDepartment(this Department model, SqlTransaction sqlTransaction)
        {
            try
            {
                Logger.Write($"DeleteDepartment: {model.SAPCode} - {model.Name} - {model.PositionName}");
                var query = @"UPDATE Departments SET IsPrepareDelete = @IsPrepareDelete WHERE Id = @id";
                using (var cmd = new SqlCommand(query, _connection, sqlTransaction))
                {
                    cmd.Transaction = sqlTransaction;
                    cmd.Parameters.AddWithValue("@IsPrepareDelete", true);
                    cmd.Parameters.AddWithValue("@id", model.Id);
                    int rs = await cmd.ExecuteNonQueryAsync();
                    Logger.Write($"Delete Department: {model.Id} - {model.SAPCode} - {model.Code}- {model.Name} => IsPrepareDelete: true", true);
                    model.NewData.Status = rs > 0;
                }   
            } catch (Exception ex)
            {
                model.NewData.ErrorList.Add("Delete Department" + ex.Message);
                Logger.Write($"Error deleting Department: {JsonConvert.SerializeObject(model)} - {ex.Message}", true);
                throw new Exception($"Error deleting Department: {model.SAPCode} - {model.Name} - {ex.Message}");
            }
        }
        private static async Task DeleteDepartment(this Department model, SqlTransaction sqlTransaction)
        {
            try
            {
                Logger.Write($"DeleteDepartment: {model.SAPCode} - {model.Name} - {model.PositionName}");
                var query = @"UPDATE Departments SET IsDeleted = @isDeleted WHERE Id = @id";
                using (var cmd = new SqlCommand(query, _connection, sqlTransaction))
                {
                    cmd.Transaction = sqlTransaction;
                    cmd.Parameters.AddWithValue("@isDeleted", true);
                    cmd.Parameters.AddWithValue("@id", model.Id);
                    int rs = await cmd.ExecuteNonQueryAsync();
                    Logger.Write($"Delete Department: {model.Id} - {model.SAPCode} - {model.Code}- {model.Name} => IsDeleted: true", true);
                    model.NewData.Status = rs > 0;
                }   
            } catch (Exception ex)
            {
                model.NewData.ErrorList.Add("Delete Department" + ex.Message);
                Logger.Write($"Error deleting Department: {JsonConvert.SerializeObject(model)} - {ex.Message}", true);
                throw new Exception($"Error deleting Department: {model.SAPCode} - {model.Name} - {ex.Message}");
            }
        }

        private static async Task SaveLogsSyncDepartment(this Department model, string action, SqlConnection conn, SqlTransaction sqlTransaction)
        {
            var insertCmd = new SqlCommand(@"
            INSERT INTO DepartmentSyncHistories (
                Id, Action, DeptCode, DeptName, SAPCode, ParentCode, ParentName, ParentSapCode, PositionCode,
                PositionName, GradeId, Grade, GradeTitle,
                RegionId, RegionName, SAPLevel, SAPObjectId, SAPObjectType, SAPDepartmentParentId, SAPDepartmentParentName, SAPValidFrom, SAPValidTo, SubArea, PersonalArea,
                BusinessModelId, BusinessModelCode, BusinessModelName, IsStore, Status,
                ErrorList, Created, Modified
            )
            VALUES (
                @Id, @Action, @DeptCode, @DeptName, @SAPCode, @ParentCode, @ParentName, @ParentSapCode, @PositionCode,
                @PositionName, @GradeId, @Grade, @GradeTitle,
                @RegionId, @RegionName, @SAPLevel, @SAPObjectId, @SAPObjectType, @SAPDepartmentParentId, @SAPDepartmentParentName, @SAPValidFrom, @SAPValidTo, @SubArea, @PersonalArea,
                @BusinessModelId, @BusinessModelCode, @BusinessModelName, @IsStore, @Status,
                @ErrorList, @Created, @Modified
            )", conn, sqlTransaction);

            insertCmd.Parameters.AddWithValue("@Id", Guid.NewGuid());
            insertCmd.Parameters.AddWithValue("@Action", action ?? (object) DBNull.Value);
            insertCmd.Parameters.AddWithValue("@DeptCode", model.NewData.Code ?? (object) DBNull.Value);
            insertCmd.Parameters.AddWithValue("@DeptName", model.NewData.Name ?? (object) DBNull.Value);
            insertCmd.Parameters.AddWithValue("@SAPCode", model.NewData.SAPCode ?? (object) DBNull.Value);
            insertCmd.Parameters.AddWithValue("@ParentCode", model.NewData.ParentCode ?? (object) DBNull.Value);
            insertCmd.Parameters.AddWithValue("@ParentName", model.NewData.ParentName ?? (object) DBNull.Value);
            insertCmd.Parameters.AddWithValue("@ParentSapCode", model.NewData.ParentSapCode ?? (object) DBNull.Value);
            insertCmd.Parameters.AddWithValue("@PositionCode", model.NewData.PositionCode ?? (object) DBNull.Value);
            insertCmd.Parameters.AddWithValue("@PositionName", model.NewData.PositionName ?? (object) DBNull.Value);
            insertCmd.Parameters.AddWithValue("@GradeId", model.NewData.JobGradeId);
            insertCmd.Parameters.AddWithValue("@Grade", model.NewData.JobGrade);
            insertCmd.Parameters.AddWithValue("@GradeTitle", model.NewData.JobGradeTitle ?? (object) DBNull.Value);
            insertCmd.Parameters.AddWithValue("@RegionId", model.NewData.RegionId ?? (object) DBNull.Value);
            insertCmd.Parameters.AddWithValue("@RegionName", model.NewData.RegionName ?? (object) DBNull.Value);
            insertCmd.Parameters.AddWithValue("@SAPLevel", model.NewData.SAPLevel ?? (object) DBNull.Value);
            insertCmd.Parameters.AddWithValue("@SAPObjectId", model.NewData.SAPObjectId ?? (object) DBNull.Value);
            insertCmd.Parameters.AddWithValue("@SAPObjectType", model.NewData.SAPObjectType ?? (object) DBNull.Value);
            insertCmd.Parameters.AddWithValue("@SAPDepartmentParentId", model.NewData.SAPDepartmentParentId ?? (object) DBNull.Value);
            insertCmd.Parameters.AddWithValue("@SAPDepartmentParentName", model.NewData.SAPDepartmentParentName ?? (object) DBNull.Value);
            insertCmd.Parameters.AddWithValue("@SAPValidFrom", model.NewData.SAPValidFrom ?? (object) DBNull.Value);
            insertCmd.Parameters.AddWithValue("@SAPValidTo", model.NewData.SAPValidTo ?? (object) DBNull.Value);
            insertCmd.Parameters.AddWithValue("@SubArea", model.NewData.SubArea ?? (object) DBNull.Value);
            insertCmd.Parameters.AddWithValue("@PersonalArea", model.NewData.PersonalArea ?? (object) DBNull.Value);
            insertCmd.Parameters.AddWithValue("@BusinessModelId", model.NewData.BusinessModelId ?? (object) DBNull.Value);
            insertCmd.Parameters.AddWithValue("@BusinessModelCode", model.NewData.BusinessModelCode ?? (object) DBNull.Value);
            insertCmd.Parameters.AddWithValue("@BusinessModelName", model.NewData.BusinessModelName ?? (object) DBNull.Value);
            insertCmd.Parameters.AddWithValue("@IsStore", model.NewData.IsStore);
            insertCmd.Parameters.AddWithValue("@Status", model.NewData.Status);
            insertCmd.Parameters.AddWithValue("@ErrorList", (model.NewData.ErrorList != null && model.NewData.ErrorList.Any()) ? JsonConvert.SerializeObject(model.NewData.ErrorList) : (object) DBNull.Value);
            insertCmd.Parameters.AddWithValue("@Created", DateTime.Now);
            insertCmd.Parameters.AddWithValue("@Modified", DateTime.Now);
            await insertCmd.ExecuteNonQueryAsync();
        }
        
    }
}