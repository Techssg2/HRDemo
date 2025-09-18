using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SyncOrgchartJob.Models;
using SyncOrgchartJob.Models.eDocHR;

namespace SyncOrgchartJob.Service
{
    public static class UserService
    {
        private static SqlConnection _connection;
        public static async Task ProcessSyncData(this List<User> models, string action, SqlConnection conn, SqlTransaction sqlTransaction)
        {
            _connection = conn;
            foreach (var model in models)
            {
                var result = false;
                switch (action)
                {
                    case "Add":
                        // result = await model.Create(sqlTransaction);
                        break;
                    case "Update":
                        await model.Update(sqlTransaction);
                        break;
                    case "Delete":
                        // await model.Delete(sqlTransaction);
                        break;
                }

                await model.SaveLogs(action, conn, sqlTransaction);
            }
        }
        
        private static async Task<bool> Create(this User model, SqlTransaction sqlTransaction)
        {
            Logger.Write($"Create: {model.SAPCode} - {model.LoginName} - {model.FullName}");
            return true;
        }
        
        private static async Task Update(this User model, SqlTransaction sqlTransaction)
        {
            try
            {
                Logger.Write($"Update User: {model.NewData.SAPCode} - {model.NewData.FullName}");
                var query = @"UPDATE Users SET IsActivated = @IsActivated WHERE Id = @id";
                using (var cmd = new SqlCommand(query, _connection, sqlTransaction))
                {
                    cmd.Transaction = sqlTransaction;
                    cmd.Parameters.AddWithValue("@IsActivated", false);
                    cmd.Parameters.AddWithValue("@id", model.NewData.Id);
                    int rs = await cmd.ExecuteNonQueryAsync();
                    Logger.Write($"Update User: {model.Id} - {model.SAPCode} => IsActivated: true", true);
                    model.NewData.Status = rs > 0;
                }   
            } catch (Exception ex)
            {
                model.NewData.ErrorList.Add("Update Department" + ex.Message);
                Logger.Write($"Error Updating Department: {model.NewData.SAPCode} - {model.NewData.FullName} - {ex.Message}");
                throw new Exception($"Error Updating Department: {model.NewData.SAPCode} - {model.NewData.FullName} - {ex.Message}");
            }
        }
        
        private static async Task<bool> Delete(this User model, SqlTransaction sqlTransaction)
        {
            Logger.Write($"Delete: {model.SAPCode} - {model.LoginName} - {model.FullName}");
            return true;
        }
        
        private static async Task SaveLogs(this User model, string action, SqlConnection conn, SqlTransaction sqlTransaction)
        {
            try
            {
                var insertCmd = new SqlCommand(@"
                INSERT INTO UserSyncHistories (
                    Id, Action, SapCode, FullName, ErrorList, StartDate, Created, Modified, Status
                )
                VALUES (
                    @Id, @Action, @SapCode, @FullName, @ErrorList, @StartDate, @Created, @Modified, @Status
                )", conn, sqlTransaction);

                insertCmd.Parameters.AddWithValue("@Id", Guid.NewGuid());
                insertCmd.Parameters.AddWithValue("@Action", action);
                insertCmd.Parameters.AddWithValue("@SapCode", (object) model.NewData.SAPCode.ConvertToSAP() ?? DBNull.Value);
                insertCmd.Parameters.AddWithValue("@FullName", (object) model.NewData.FullName ?? DBNull.Value);
                insertCmd.Parameters.AddWithValue("@StartDate", (object) model.NewData.StartDate ?? DBNull.Value);
                insertCmd.Parameters.AddWithValue("@ErrorList", (model.NewData.ErrorList != null && model.NewData.ErrorList.Any()) ? JsonConvert.SerializeObject(model.NewData.ErrorList) : (object) DBNull.Value);
                insertCmd.Parameters.AddWithValue("@Created", DateTime.Now);
                insertCmd.Parameters.AddWithValue("@Modified", DateTime.Now);
                insertCmd.Parameters.AddWithValue("@Status", model.NewData.Status);
                await insertCmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving sync log for user {model.SAPCode}: {ex.Message}", ex);
            }
        }
    }
}