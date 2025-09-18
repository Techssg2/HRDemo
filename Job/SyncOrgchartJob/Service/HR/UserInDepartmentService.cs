using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SyncOrgchartJob.Models.eDocHR;

namespace SyncOrgchartJob.Service
{
    public static class UserInDepartmentService
    {
        private static SqlConnection _connection;
        public static async Task ProcessSyncData(this List<UserDepartmentMapping> models, string action, SqlConnection conn, SqlTransaction sqlTransaction)
        {
            _connection = conn;
            foreach (var model in models)
            {
                var result = false;
                switch (action)
                {
                    case "Add":
                        await model.Create(sqlTransaction);
                        break;
                    case "Update":
                        await model.Update(sqlTransaction);
                        break;
                    case "Delete":
                        // await model.Delete(sqlTransaction);
                        await model.UpdatePrepareDeleteDepartment(sqlTransaction);
                        break;
                }
                await model.SaveLogsSyncUserInDepartment(action, conn, sqlTransaction);
            }
        }
        
        private static async Task Create(this UserDepartmentMapping model, SqlTransaction sqlTransaction)
        {
            try
            {
                if (model.NewData.DepartmentEdocSAPCode.Equals("50055931"))
                {
                    var daa = "";
                }
                
                var query = @"INSERT INTO UserDepartmentMappings (Id, DepartmentId, UserId, Role, IsHeadCount, Created, Modified, CreatedById, ModifiedById, CreatedByFullName, ModifiedByFullName)
                          VALUES (@Id, @DepartmentId, @UserId, @Role, @IsHeadCount, @Created, @Modified, @CreatedById, @ModifiedById, @CreatedByFullName, @ModifiedByFullName)";
                using (var cmd = new SqlCommand(query, _connection, sqlTransaction))
                {
                    cmd.Transaction = sqlTransaction;
                    cmd.Parameters.AddWithValue("@Id", Guid.NewGuid());
                    cmd.Parameters.AddWithValue("@DepartmentId", model.NewData.DepartmentId);
                    cmd.Parameters.AddWithValue("@UserId", model.NewData.UserId);
                    cmd.Parameters.AddWithValue("@Role", model.NewData.Role);
                    cmd.Parameters.AddWithValue("@IsHeadCount", model.NewData.IsHeadCount);
                    cmd.Parameters.AddWithValue("@Created", DateTime.Now);
                    cmd.Parameters.AddWithValue("@modified", DateTime.Now);
                    cmd.Parameters.AddWithValue("@CreatedById", Guid.Parse("99A2F491-3103-4C01-9F6E-43359F3FBCD7"));
                    cmd.Parameters.AddWithValue("@ModifiedById", Guid.Parse("99A2F491-3103-4C01-9F6E-43359F3FBCD7"));
                    cmd.Parameters.AddWithValue("@CreatedByFullName", "Sadmin");
                    cmd.Parameters.AddWithValue("@ModifiedByFullName", "Sadmin");
                    var rs = await cmd.ExecuteNonQueryAsync();
                    Logger.Write($"Create User In Department: {model.NewData.Id} - {model.NewData.UserSAPCode} - {model.NewData.DepartmentEdocSAPCode}", true);
                    model.NewData.Status = rs > 0;
                }
            }
            catch (Exception ex)
            {
                Logger.Write($"Create User In Department: {model.NewData.Id}", true);
                model.NewData.ErrorList.Add($"Error creating User In Department: {model.NewData.Id} - {model.NewData.UserSAPCode} - {model.NewData.DepartmentEdocSAPCode} - {ex.Message}");
                // throw new Exception($"Error creating User In Department: {model.NewData.Id} - {model.NewData.UserSAPCode} - {model.NewData.DepartmentEdocSAPCode} - {ex.Message}", ex);
            }
        }
        
        private static async Task Update(this UserDepartmentMapping model, SqlTransaction sqlTransaction)
        {
            try
            {
                
                var query = @"UPDATE UserDepartmentMappings SET IsHeadCount = @IsHeadCount, IsDeleted = @IsDeleted, IsPrepareDelete = @IsPrepareDelete WHERE Id = @id";
                using (var cmd = new SqlCommand(query, _connection, sqlTransaction))
                {
                    cmd.Transaction = sqlTransaction;
                    // cmd.Parameters.AddWithValue("@IsHeadCount", model.NewData.IsHeadCount);
                    cmd.Parameters.AddWithValue("@IsHeadCount", !model.NewData.IsConcurrently);
                    cmd.Parameters.AddWithValue("@IsPrepareDelete", false);
                    cmd.Parameters.AddWithValue("@IsDeleted", false);
                    cmd.Parameters.AddWithValue("@id", model.NewData.Id);
                    int rs = await cmd.ExecuteNonQueryAsync();
                    Logger.Write($"Update User In Department: {model.NewData.Id} - {model.NewData.UserSAPCode} - {model.NewData.DepartmentEdocSAPCode}", true);
                    model.NewData.Status = rs > 0;
                }   
            } catch (Exception ex)
            {
                model.NewData.ErrorList.Add($"Error deleting User In Department: {model.NewData.Id} - {model.NewData.UserSAPCode} - {model.NewData.DepartmentEdocSAPCode} - {ex.Message}");
                Logger.Write($"Error deleting User In Department: {model.NewData.Id} - {model.NewData.UserSAPCode} - {model.NewData.DepartmentEdocSAPCode} - {ex.Message}", true);
                throw ex;
            }
        }
        
        private static async Task UpdatePrepareDeleteDepartment(this UserDepartmentMapping model, SqlTransaction sqlTransaction)
        {
            try
            {
                var query = @"UPDATE UserDepartmentMappings SET IsPrepareDelete = @IsPrepareDelete, IsHeadCount = @IsHeadCount WHERE Id = @id";
                using (var cmd = new SqlCommand(query, _connection, sqlTransaction))
                {
                    cmd.Transaction = sqlTransaction;
                    cmd.Parameters.AddWithValue("@IsPrepareDelete", true);
                    cmd.Parameters.AddWithValue("@IsHeadCount", false);
                    cmd.Parameters.AddWithValue("@id", model.NewData.Id);
                    int rs = await cmd.ExecuteNonQueryAsync();
                    Logger.Write($"Delete User In Department: {model.NewData.Id} - {model.NewData.UserSAPCode} - {model.NewData.DepartmentEdocSAPCode}", true);
                    model.NewData.Status = rs > 0;
                }   
            } catch (Exception ex)
            {
                model.NewData.ErrorList.Add($"Error deleting User In Department: {model.NewData.Id} - {model.NewData.UserSAPCode} - {model.NewData.DepartmentEdocSAPCode} - {ex.Message}");
                Logger.Write($"Error deleting User In Department: {model.NewData.Id} - {model.NewData.UserSAPCode} - {model.NewData.DepartmentEdocSAPCode} - {ex.Message}", true);
                throw ex;
            }
        }
        
        private static async Task Delete(this UserDepartmentMapping model, SqlTransaction sqlTransaction)
        {
            try
            {
                var query = @"UPDATE UserDepartmentMappings SET IsDeleted = @isDeleted WHERE Id = @id";
                using (var cmd = new SqlCommand(query, _connection, sqlTransaction))
                {
                    cmd.Transaction = sqlTransaction;
                    cmd.Parameters.AddWithValue("@isDeleted", true);
                    cmd.Parameters.AddWithValue("@id", model.NewData.Id);
                    int rs = await cmd.ExecuteNonQueryAsync();
                    Logger.Write($"Delete User In Department: {model.NewData.Id} - {model.NewData.UserSAPCode} - {model.NewData.DepartmentEdocSAPCode}", true);
                    model.NewData.Status = rs > 0;
                }   
            } catch (Exception ex)
            {
                model.NewData.ErrorList.Add($"Error deleting User In Department: {model.NewData.Id} - {model.NewData.UserSAPCode} - {model.NewData.DepartmentEdocSAPCode} - {ex.Message}");
                Logger.Write($"Error deleting User In Department: {model.NewData.Id} - {model.NewData.UserSAPCode} - {model.NewData.DepartmentEdocSAPCode} - {ex.Message}", true);
                throw ex;
            }
        }
        
        private static async Task SaveLogsSyncUserInDepartment(this UserDepartmentMapping model, string action, SqlConnection conn, SqlTransaction sqlTransaction)
        {
            try
            {
                var insertCmd = new SqlCommand(@"
                INSERT INTO UserDepartmentSyncHistories (
                    Id, Action, DepartmentId, DepartmentCode, DepartmentName, DepartmentSapCode,
                    UserId, UserSapCode, LoginName, FullName,
                    IsHeadCount, IsConcurrently, ErrorList, Created, Modified, Status
                )
                VALUES (
                    @Id, @Action, @DepartmentId, @DepartmentCode, @DepartmentName, @DepartmentSapCode,
                    @UserId, @UserSapCode, @LoginName, @FullName,
                    @IsHeadCount, @IsConcurrently, @ErrorList, @Created, @Modified, @Status
                )", conn, sqlTransaction);
                
                insertCmd.Parameters.AddWithValue("@Id", Guid.NewGuid());
                insertCmd.Parameters.AddWithValue("@Action", (object)action ?? DBNull.Value);
                insertCmd.Parameters.AddWithValue("@DepartmentId", (object) model.NewData.DepartmentId ?? DBNull.Value);
                insertCmd.Parameters.AddWithValue("@DepartmentCode", (object) model.NewData.DepartmentCode ?? DBNull.Value);
                insertCmd.Parameters.AddWithValue("@DepartmentName", (object) model.NewData.DepartmentName ?? DBNull.Value);
                insertCmd.Parameters.AddWithValue("@DepartmentSapCode", (object) model.NewData.DepartmentEdocSAPCode ?? DBNull.Value);
                insertCmd.Parameters.AddWithValue("@UserId", (object) model.NewData.UserId ?? DBNull.Value);
                insertCmd.Parameters.AddWithValue("@UserSapCode", (object) model.NewData.UserSAPCode ?? DBNull.Value);
                insertCmd.Parameters.AddWithValue("@LoginName", (object) model.NewData.LoginName ?? DBNull.Value);
                insertCmd.Parameters.AddWithValue("@FullName", (object) model.NewData.FullName ?? DBNull.Value);
                insertCmd.Parameters.AddWithValue("@IsHeadCount", (object) model.NewData.IsHeadCount ?? DBNull.Value);
                insertCmd.Parameters.AddWithValue("@IsConcurrently", false);
                insertCmd.Parameters.AddWithValue("@ErrorList", (model.NewData.ErrorList != null && model.NewData.ErrorList.Any()) ? JsonConvert.SerializeObject(model.NewData.ErrorList) : (object) DBNull.Value);
                insertCmd.Parameters.AddWithValue("@Created", DateTime.Now);
                insertCmd.Parameters.AddWithValue("@Modified", DateTime.Now);
                insertCmd.Parameters.AddWithValue("@Status", model.NewData.Status);

                await insertCmd.ExecuteNonQueryAsync();
            } catch (Exception ex)
            {
                throw new Exception($"Error saving sync log for user in department {model.NewData.Id} - {model.NewData.UserSAPCode} - {model.NewData.DepartmentEdocSAPCode}: {ex.Message}", ex);
            }
        }
    }
}