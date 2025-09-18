using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using SyncOrgchartJob.Models.Backup;
using SyncOrgchartJob.Models.eDocHR;

namespace SyncOrgchartJob.Service
{
    public class RollbackService
    {
        
        private List<User> _userLists = null;
        private List<Department> _departments = null;
        private List<UserDepartmentMapping> _userDepartmentMappings = null;
        private readonly string _connStr = ConfigurationManager.AppSettings["StagingDb"];
        private readonly string _connHRStr = ConfigurationManager.AppSettings["HRDb"];
        public async Task RunAsync(int orgchartId)
        {
            Console.WriteLine("");
            Logger.Write($"------------------- START ROLLBACK", true);
            _userLists = await DataService.GetListUsers(true);
            _departments = await DataService.GetAllDepartmentsAsync(true);
            _userDepartmentMappings = await DataService.GetAllUserInDepartmentMappings(true);
            
            using (var con = new SqlConnection(_connHRStr))
            {
                await con.OpenAsync();

                using (var tran = con.BeginTransaction())
                {
                    try
                    {
                        Logger.Write("Start creating rollback header: " + orgchartId, true);
                        var dataBackupUser = await GetBackupUserByHeaderId(orgchartId);
                        Logger.Write("Start creating rollback dataBackupUser: " + orgchartId + " - User count: " + dataBackupUser.Count, true);
                        var dataBackupDepartment = await GetBackupDepartmentByHeaderId(orgchartId);
                        Logger.Write("Start creating rollback dataBackupDepartment: " + orgchartId + " - Department count: " + dataBackupDepartment.Count, true);
                        var dataBackupUserInDepartment = await GetBackupUserDepartmentByHeaderId(orgchartId);
                        Logger.Write("Start creating rollback dataBackupUserInDepartment: " + orgchartId + " - UserInDepartment count: " + dataBackupUserInDepartment.Count, true);
                        // tran.Commit();
                        
                        Logger.Write($"-----------------------------------------------------------------------------------------------", true);
                        Logger.Write($"------------------- START PROCESS USER", true);
                        Logger.Write($"------------------- INACTIVE", true);
                        int countInactive = 0;
                        int countUserDelete = 0;
                        foreach (var userBackup in dataBackupUser)
                        {
                            var userHR = _userLists.FirstOrDefault(u => u.Id == userBackup.ItemId);
                            if (userHR != null)
                            {
                                var query = @"UPDATE Users SET IsActivated = @isActivated, IsDeleted = @isDeleted WHERE Id = @id";
                                using (var cmd = new SqlCommand(query, con))
                                {
                                    cmd.Transaction = tran; 
                                    cmd.Parameters.AddWithValue("@isActivated", userBackup.IsActivated);
                                    cmd.Parameters.AddWithValue("@isDeleted", false);
                                    cmd.Parameters.AddWithValue("@id", userHR.Id);
                                    await cmd.ExecuteNonQueryAsync();
                                    countInactive++;
                                    Logger.Write($"Updated User: {userHR.Id} - {userHR.SAPCode} - {userHR.FullName} => IsActivated: {userBackup.IsActivated}", true);
                                }
                            }
                        }
                        Logger.Write("Number: " + countInactive, true);

                        var UserIds = dataBackupUser.Select(x => x.ItemId).ToList();
                        var userInHR = _userLists.Where(u => !u.IsDeleted && !UserIds.Contains(u.Id)).ToList();
                        Logger.Write($"------------------- DELETE", true);
                        if (userInHR.Count > 0)
                        {
                            Logger.Write("DELETE: " + userInHR.Count.ToString(), true);
                            foreach (var userHR in userInHR)
                            {
                                if (userHR.IsDeleted) continue;
                                
                                var query = @"UPDATE Users SET IsDeleted = @isDeleted WHERE Id = @id";
                                using (var cmd = new SqlCommand(query, con))
                                {
                                    cmd.Transaction = tran; 
                                    cmd.Parameters.AddWithValue("@isDeleted", true);
                                    cmd.Parameters.AddWithValue("@id", userHR.Id);
                                    await cmd.ExecuteNonQueryAsync();
                                    countUserDelete++;
                                    Logger.Write($"Updated User: {userHR.Id} - {userHR.SAPCode} - {userHR.FullName} => IsDeleted: true", true);
                                }
                            }
                        }
                        Logger.Write("Number: " + countUserDelete, true);
                        Logger.Write($"------------------- END PROCESS USER", true);
                        Logger.Write($"-----------------------------------------------------------------------------------------------", true);
                        
                        Logger.Write($"------------------- START PROCESS DEPARTMENT", true);
                        Logger.Write($"------------------- UPDATE INFORMATION", true);
                        int countDepartment = 0;
                        int countDeleteDepartment = 0;
                        foreach (var departmentBackup in dataBackupDepartment)
                        {
                            var departmentHR = _departments.Find(u => u.Id == departmentBackup.ItemId);
                            if (departmentHR != null)
                            {
                                if (departmentBackup.SAPCode == departmentHR.SAPCode &&
                                    departmentBackup.Code == departmentHR.Code &&
                                    departmentBackup.Name == departmentHR.Name &&
                                    departmentBackup.PositionCode == departmentHR.PositionCode &&
                                    departmentBackup.PositionName == departmentHR.PositionName &&
                                    departmentBackup.JobGradeId == departmentHR.JobGradeId &&
                                    departmentBackup.RegionId == departmentHR.RegionId &&
                                    departmentBackup.ParentId == departmentHR.ParentId &&
                                    departmentBackup.BusinessModelId == departmentHR.BusinessModelId &&
                                    !departmentHR.IsDeleted &&
                                    (departmentBackup.SAPObjectId == departmentHR.SAPObjectId ||
                                     string.IsNullOrEmpty(departmentBackup.SAPObjectId)) &&
                                    (departmentBackup.SAPObjectType == departmentHR.SAPObjectType ||
                                     string.IsNullOrEmpty(departmentBackup.SAPObjectType)) &&
                                    (departmentBackup.SAPDepartmentParentId == departmentHR.SAPDepartmentParentId ||
                                     string.IsNullOrEmpty(departmentBackup.SAPDepartmentParentId)) &&
                                    (departmentBackup.SAPDepartmentParentName == departmentHR.SAPDepartmentParentName ||
                                     string.IsNullOrEmpty(departmentBackup.SAPDepartmentParentName)) &&
                                    (departmentBackup.SAPLevel == departmentHR.SAPLevel ||
                                     string.IsNullOrEmpty(departmentBackup.SAPLevel)) &&
                                    (departmentBackup.SAPValidFrom == departmentHR.SAPValidFrom ||
                                     string.IsNullOrEmpty(departmentBackup.SAPValidFrom)) &&
                                    (departmentBackup.SAPValidTo == departmentHR.SAPValidTo ||
                                     string.IsNullOrEmpty(departmentBackup.SAPValidTo)))
                                {
                                    continue;
                                }
                                
                                var query = @"
                                    UPDATE Departments SET
                                        SAPCode = @SAPCode,
                                        Code = @Code,
                                        Name = @Name,
                                        PositionCode = @PositionCode,
                                        PositionName = @PositionName,
                                        JobGradeId = @JobGradeId,
                                        RegionId = @RegionId,
                                        ParentId = @ParentId,
                                        BusinessModelId = @BusinessModelId,
                                        Type = @Type,
                                        IsDeleted = @IsDeleted,
                                        SAPObjectId = @SAPObjectId,
                                        SAPObjectType = @SAPObjectType,
                                        SAPDepartmentParentId = @SAPDepartmentParentId,
                                        SAPDepartmentParentName = @SAPDepartmentParentName,
                                        SAPLevel = @SAPLevel,
                                        SAPValidFrom = @SAPValidFrom,
                                        SAPValidTo = @SAPValidTo
                                    WHERE Id = @id";
                                
                                using (var cmd = new SqlCommand(query, con))
                                {
                                    cmd.Transaction = tran;
                                    Logger.Write($"SAPCode: {departmentHR.SAPCode} -> {departmentBackup.SAPCode}");
                                    cmd.Parameters.AddWithValue("@SAPCode",(object)  departmentBackup.SAPCode ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("@Code",(object)  departmentBackup.Code ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("@Name",(object)  departmentBackup.Name ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("@PositionCode", (object) departmentBackup.PositionCode ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("@PositionName",(object)  departmentBackup.PositionName ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("@JobGradeId",(object)  departmentBackup.JobGradeId ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("@RegionId", (object) departmentBackup.RegionId ??  DBNull.Value);
                                    cmd.Parameters.AddWithValue("@ParentId", (object) departmentBackup.ParentId ??  DBNull.Value);
                                    cmd.Parameters.AddWithValue("@BusinessModelId", (object) departmentBackup.BusinessModelId ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("@Type", (object) departmentBackup.Type ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("@IsDeleted", false);
                                    
                                    cmd.Parameters.AddWithValue("@SAPObjectId", (object) departmentBackup.SAPObjectId ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("@SAPObjectType", (object) departmentBackup.SAPObjectType ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("@SAPDepartmentParentId", (object) departmentBackup.SAPDepartmentParentId ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("@SAPDepartmentParentName", (object) departmentBackup.SAPDepartmentParentName ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("@SAPLevel", (object)departmentBackup.SAPLevel ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("@SAPValidFrom", (object) departmentBackup.SAPValidFrom ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("@SAPValidTo", (object) departmentBackup.SAPValidTo ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("@id", departmentBackup.ItemId);
                                    await cmd.ExecuteNonQueryAsync();
                                    countDepartment++;
                                    Logger.Write($"Updated Department: {departmentBackup.Id} - {departmentBackup.SAPCode} - {departmentBackup.Name} => Update thong tin", true);
                                }
                            }
                        }
                        Logger.Write("Number: " + countDepartment, true);
                        
                        var departmentIds = dataBackupDepartment.Select(x => x.ItemId).ToList();
                        var departmentInHR = _departments.Where(u =>  !u.IsDeleted && !departmentIds.Contains(u.Id)).ToList();
                        Logger.Write($"------------------- DELETE", true);
                        if (departmentInHR.Count > 0)
                        {
                            Logger.Write("DELETE: " + departmentInHR.Count.ToString(), true);
                            foreach (var data in departmentInHR)
                            {
                                if (data.IsDeleted) continue;
                                var query = @"UPDATE Departments SET IsDeleted = @isDeleted WHERE Id = @id";
                                using (var cmd = new SqlCommand(query, con))
                                {
                                    cmd.Transaction = tran; 
                                    cmd.Parameters.AddWithValue("@isDeleted", true);
                                    cmd.Parameters.AddWithValue("@id", data.Id);
                                    await cmd.ExecuteNonQueryAsync();
                                    countDeleteDepartment++;
                                    Logger.Write($"Updated Department: {data.Id} - {data.SAPCode} - {data.Code}- {data.Name} => IsDeleted: true", true);
                                }
                            }
                        }
                        Logger.Write("Number: " + countUserDelete, true);
                        Logger.Write($"------------------- END PROCESS DEPARTMENT", true);
                        Logger.Write($"-----------------------------------------------------------------------------------------------", true);
                        
                        Logger.Write($"------------------- START PROCESS USER IN DEPARTMENT", true);
                        Logger.Write($"------------------- UPDATE INFORMATION", true);
                        int countUserInDepartment = 0;
                        int countDeleteUserInDepartment = 0;

                        foreach (var userInDepartmentBackup in dataBackupUserInDepartment)
                        {
                            var userInDepartment = _userDepartmentMappings.FirstOrDefault(u => u.Id == userInDepartmentBackup.ItemId);
                            if (userInDepartment != null)
                            {
                                if (userInDepartment.DepartmentId == Guid.Empty || userInDepartment.UserId == Guid.Empty) continue;
                            
                                if (userInDepartmentBackup.DepartmentId == userInDepartment.DepartmentId
                                    && userInDepartmentBackup.UserId == userInDepartment.UserId
                                    && userInDepartmentBackup.Role == (int) userInDepartment.Role
                                    && userInDepartmentBackup.IsHeadCount == userInDepartment.IsHeadCount
                                    && !userInDepartment.IsDeleted)
                                {
                                    continue;
                                }
                                
                                var query = @"
                                    UPDATE UserDepartmentMappings SET
                                        DepartmentId = @DepartmentId,
                                        UserId = @UserId,
                                        Role = @Role,
                                        IsHeadCount = @IsHeadCount,
                                        IsDeleted = @IsDeleted
                                    WHERE Id = @id";
                                
                                using (var cmd = new SqlCommand(query, con))
                                {
                                    try
                                    {
                                        cmd.Transaction = tran;
                                        cmd.Parameters.AddWithValue("@DepartmentId",(object) userInDepartmentBackup.DepartmentId ?? DBNull.Value);
                                        cmd.Parameters.AddWithValue("@UserId",(object) userInDepartmentBackup.UserId ?? DBNull.Value);
                                        cmd.Parameters.AddWithValue("@Role",(object) userInDepartmentBackup.Role ?? DBNull.Value);
                                        cmd.Parameters.AddWithValue("@IsHeadCount", userInDepartmentBackup.IsHeadCount);
                                        cmd.Parameters.AddWithValue("@IsDeleted", false);
                                        cmd.Parameters.AddWithValue("@id", userInDepartmentBackup.ItemId);
                                        await cmd.ExecuteNonQueryAsync();
                                        countUserInDepartment++;
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Write($"Error updating User In Department: {userInDepartment.Id} - {userInDepartment.DepartmentId} - {userInDepartment.UserId} => {ex.Message}", true);
                                        throw new Exception($"Error updating User In Department: {userInDepartment.Id} - {userInDepartment.DepartmentId} - {userInDepartment.UserId} => {ex.Message}");
                                    }
                                    
                                    Logger.Write($"Updated User In Department: {userInDepartment.Id} - {userInDepartment.DepartmentId} - {userInDepartment.UserId} => Update thong tin", true);
                                }
                            }
                        }
                        Logger.Write("Number: " + countUserInDepartment, true);
                        
                        var userInDepartmentIds = dataBackupUserInDepartment.Select(x => x.ItemId).ToList();
                        var userInDepartmentHR = _userDepartmentMappings.Where(u => !u.IsDeleted && !userInDepartmentIds.Contains(u.Id)).ToList();
                        Logger.Write($"------------------- DELETE", true);
                        if (userInDepartmentHR.Count > 0)
                        {
                            Logger.Write("DELETE: " + userInDepartmentHR.Count.ToString(), true);
                            foreach (var data in userInDepartmentHR)
                            {
                                if (data.IsDeleted) continue;
                                var query = @"UPDATE UserDepartmentMappings SET IsDeleted = @isDeleted WHERE Id = @id";
                                using (var cmd = new SqlCommand(query, con))
                                {
                                    cmd.Transaction = tran; 
                                    cmd.Parameters.AddWithValue("@isDeleted", true);
                                    cmd.Parameters.AddWithValue("@id", data.Id);
                                    await cmd.ExecuteNonQueryAsync();
                                    countDeleteUserInDepartment++;
                                    Logger.Write($"Updated User in Department: {data.Id} - {data.DepartmentId} - {data.DepartmentCode} - {data.UserId} - {data.UserSAPCode} => IsDeleted: true", true);
                                }
                            }
                        }
                        Logger.Write("Number: " + countDeleteUserInDepartment, true);
                        Logger.Write($"------------------- END PROCESS USER IN DEPARTMENT", true);
                        Logger.Write($"-----------------------------------------------------------------------------------------------", true);
                        tran.Commit();
                        Logger.Write("Rollback header created successfully with Id: " + orgchartId, true);
                    } catch (Exception ex)
                    {
                        tran.Rollback();
                        Console.WriteLine("Error creating rollback header: " + ex.Message);
                        Logger.Write("Error creating rollback header: " + ex.Message);
                        throw ex;
                    }
                    
                }
            }
            await Task.CompletedTask; 
        }
        
        private async Task<List<BackupUserDetail>> GetBackupUserByHeaderId(int headerId)
        {
            var results = new List<BackupUserDetail>();
            using (var conn = new SqlConnection(_connStr))
            {
                await conn.OpenAsync();
                var query = $@"select * from Backup_User_Detail where BackupHeaderId = @headerId";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@headerId", headerId);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int id = 0;
                            int backupHeaderId = 0;
                            Guid itemId = Guid.Empty;
                            string fullName = reader["FullName"]?.ToString();
                            string sapCode = reader["SAPCode"]?.ToString();
                            bool isActivated = false;
                            DateTime createdAt = DateTime.MinValue;
                            int.TryParse(reader["Id"]?.ToString(), out id);
                            int.TryParse(reader["BackupHeaderId"]?.ToString(), out backupHeaderId);
                            Guid.TryParse(reader["ItemId"]?.ToString(), out itemId);
                            bool.TryParse(reader["IsActivated"]?.ToString(), out isActivated);
                            DateTime.TryParse(reader["CreatedAt"]?.ToString(), out createdAt);
                            results.Add(new BackupUserDetail()
                            {
                                Id = id,
                                BackupHeaderId = backupHeaderId,
                                ItemId = itemId,
                                FullName = fullName,
                                SAPCode = sapCode,
                                IsActivated = isActivated,
                                CreatedAt = createdAt
                            });
                        }
                    }
                }
            }
            return results;
        }
        
        public async Task<List<BackupUserInDepartmentDetail>> GetBackupUserDepartmentByHeaderId(int headerId)
        {
            var results = new List<BackupUserInDepartmentDetail>();
            using (var conn = new SqlConnection(_connStr))
            {
                await conn.OpenAsync();
                var query = $@"select * from Backup_UserInDepartment_Detail where BackupHeaderId = @headerId";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@headerId", headerId);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var itemIds = Guid.TryParse(reader["ItemId"]?.ToString(), out var itemId)
                                ? itemId
                                : Guid.Empty;
                            
                            results.Add(new BackupUserInDepartmentDetail
                            {
                                BackupHeaderId = int.TryParse(reader["BackupHeaderId"]?.ToString(), out var backupHeaderId) ? backupHeaderId : 0,
                                ItemId = itemId,
                                DepartmentId = Guid.TryParse(reader["DepartmentId"]?.ToString(), out var departmentId) ? departmentId : Guid.Empty,
                                UserId = Guid.TryParse(reader["UserId"]?.ToString(), out var userId) ? userId : Guid.Empty,
                                IsHeadCount = int.TryParse(reader["IsHeadCount"]?.ToString(), out var isHeadCount) && isHeadCount == 1,
                                Role = int.TryParse(reader["Role"]?.ToString(), out var role) ? role : 0,
                                CreatedAt = DateTime.TryParse(reader["CreatedAt"]?.ToString(), out var createdAt) ? createdAt : DateTime.MinValue
                            });
                        }
                    }
                }
            }
            return results;
        }
        
        public async Task<List<BackupDepartmentDetail>> GetBackupDepartmentByHeaderId(int headerId)
        {
            var results = new List<BackupDepartmentDetail>();
            using (var conn = new SqlConnection(_connStr))
            {
                await conn.OpenAsync();
                var query = $@"select * from Backup_Department_Detail where BackupHeaderId = @headerId";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@headerId", headerId);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            results.Add(new BackupDepartmentDetail
                            {
                                Id = reader["Id"] is int id ? id : 0,
                                BackupHeaderId = reader["BackupHeaderId"] is int bhId ? bhId : 0,
                                ItemId = Guid.TryParse(reader["ItemId"]?.ToString(), out var itemId) ? itemId : Guid.Empty,
                                Code = reader["Code"]?.ToString(),
                                Name = reader["Name"]?.ToString(),
                                Type = int.TryParse(reader["Type"]?.ToString(), out var type) ? type : (int?)null,
                                JobGradeId = Guid.TryParse(reader["JobGradeId"]?.ToString(), out var jobGradeId) ? jobGradeId : (Guid?)null,
                                ParentId = Guid.TryParse(reader["ParentId"]?.ToString(), out var parentId) ? parentId : (Guid?)null,
                                SAPCode = reader["SAPCode"]?.ToString(),
                                Grade = int.TryParse(reader["Grade"]?.ToString(), out var grade) ? grade : (int?)null,
                                GradeTile = reader["GradeTile"]?.ToString(),
                                PositionCode = reader["PositionCode"]?.ToString(),
                                PositionName = reader["PositionName"]?.ToString(),
                                RegionId = Guid.TryParse(reader["RegionId"]?.ToString(), out var regionId) ? regionId : (Guid?)null,
                                RegionName = reader["RegionName"]?.ToString(),
                                BusinessModelId = Guid.TryParse(reader["BusinessModelId"]?.ToString(), out var businessModelId) ? businessModelId : (Guid?)null,
                                IsFromIT = bool.TryParse(reader["IsFromIT"]?.ToString(), out var isFromIT) ? isFromIT : (bool?)null,
                                IsEdoc = bool.TryParse(reader["IsEdoc"]?.ToString(), out var isEdoc) ? isEdoc : (bool?)null,
                                IsEdoc1 = bool.TryParse(reader["IsEdoc1"]?.ToString(), out var isEdoc1) ? isEdoc1 : (bool?)null,
                                SAPObjectId = reader["SAPObjectId"]?.ToString(),
                                SAPObjectType = reader["SAPObjectType"]?.ToString(),
                                SAPDepartmentParentName = reader["SAPDepartmentParentName"]?.ToString(),
                                SAPDepartmentParentId = reader["SAPDepartmentParentId"]?.ToString(),
                                SAPLevel = reader["SAPLevel"]?.ToString(),
                                SAPValidFrom = reader["SAPValidFrom"]?.ToString(),
                                SAPValidTo = reader["SAPValidTo"]?.ToString(),
                                CreatedAt = DateTime.TryParse(reader["CreatedAt"]?.ToString(), out var createdAt) ? createdAt : DateTime.MinValue
                            });
                        }
                    }
                }
            }
            return results;
        }
    }
}