using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SyncOrgchartJob.Models.eDocHR;

namespace SyncOrgchartJob.Service
{
    public class BackupService
    {
        private List<User> _userLists = null;
        private List<Department> _departments = null;
        private List<UserDepartmentMapping> _userDepartmentMappings = null;
        private readonly string _connStr = ConfigurationManager.AppSettings["StagingDb"];
        
        public async Task RunAsync()
        {
            _userLists = await DataService.GetListUsers();
            _departments = await DataService.GetAllDepartmentsAsync();
            _userDepartmentMappings = await DataService.GetAllUserInDepartmentMappings();
            
            if (_userLists == null || _userLists.Count == 0)
            {
                return;
            }

            using (var con = new SqlConnection(_connStr))
            {
                await con.OpenAsync();

                using (var tran = con.BeginTransaction())
                {
                    var headerId = await CreateHeader(con, tran);

                    try
                    {
                        await InsertBackupUser(con, tran, headerId, _userLists);
                        await InsertBackupDepartment(con, tran, headerId, _departments);
                        await InsertBackupUserDepartmentMappings(con, tran, headerId, _userDepartmentMappings);   
                        tran.Commit();
                    } catch (Exception ex)
                    {
                        Console.WriteLine($"Error creating backup header: {ex.Message}");
                        tran.Rollback();
                        return;
                    }
                    
                }
            }
        }
        
        private static async Task<int> CreateHeader(SqlConnection conn, SqlTransaction trans)
        {
            var insertCmd = new SqlCommand(@"
                    INSERT INTO Backup_Header DEFAULT VALUES;
                    SELECT SCOPE_IDENTITY();
                ", conn, trans);

            var insertedId = await insertCmd.ExecuteScalarAsync();
            return int.Parse(insertedId.ToString());
        }
        
        private static async Task InsertBackupUser(
            SqlConnection conn,
            SqlTransaction transaction,
            int backupHeaderId, 
            List<User> users)
        {
            foreach (var user in users)
            {
                var insertCmd = new SqlCommand(@"
                    INSERT INTO Backup_User_Detail (
                        BackupHeaderId, ItemId, FullName, SAPCode, IsActivated, CreatedAt
                    )
                    VALUES (
                        @BackupHeaderId, @ItemId, @FullName, @SAPCode, @IsActivated, @CreatedAt
                    );", conn, transaction);

                insertCmd.Parameters.AddWithValue("@BackupHeaderId", backupHeaderId);
                insertCmd.Parameters.AddWithValue("@ItemId", user.Id);
                insertCmd.Parameters.AddWithValue("@FullName", user.FullName);
                insertCmd.Parameters.AddWithValue("@SAPCode", user.SAPCode);
                insertCmd.Parameters.AddWithValue("@IsActivated", user.IsActivated);
                insertCmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                await insertCmd.ExecuteNonQueryAsync();
            }
        }
        
        private static async Task InsertBackupUserDepartmentMappings(
            SqlConnection conn,
            SqlTransaction transaction,
            int backupHeaderId,
            List<UserDepartmentMapping> userDepartments)
        {
            foreach (var userDepartment in userDepartments)
            {
                var insertCmd = new SqlCommand(@"
                INSERT INTO Backup_UserInDepartment_Detail (
                    BackupHeaderId, ItemId, DepartmentId, UserId, IsHeadCount, Role, CreatedAt
                )
                VALUES (
                    @BackupHeaderId, @ItemId, @DepartmentId, @UserId, @IsHeadCount, @Role, @CreatedAt
                );", conn, transaction);

                insertCmd.Parameters.AddWithValue("@BackupHeaderId", backupHeaderId);
                insertCmd.Parameters.AddWithValue("@ItemId", userDepartment.Id);
                insertCmd.Parameters.AddWithValue("@DepartmentId", userDepartment.DepartmentId);
                insertCmd.Parameters.AddWithValue("@UserId", userDepartment.UserId);
                insertCmd.Parameters.AddWithValue("@IsHeadCount", userDepartment.IsHeadCount);
                insertCmd.Parameters.AddWithValue("@Role", (object) userDepartment.Role ?? DBNull.Value);
                insertCmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                await insertCmd.ExecuteNonQueryAsync();
            }
        }
        
        
        private static async Task InsertBackupDepartment(
            SqlConnection conn,
            SqlTransaction transaction,
            int backupHeaderId,
            List<Department> departments)
        {
            foreach (var dept in departments)
            {
                var insertCmd = new SqlCommand(@"
                    INSERT INTO Backup_Department_Detail (
                        BackupHeaderId, ItemId, Code, Name, Type, JobGradeId, ParentId,
                        SAPCode, Grade, GradeTile, PositionCode, PositionName, RegionId,
                        RegionName, BusinessModelId, IsFromIT, IsEdoc, IsEdoc1,
                        SAPObjectId, SAPObjectType, SAPDepartmentParentName, SAPDepartmentParentId,
                        SAPLevel, SAPValidFrom, SAPValidTo, CreatedAt
                    )
                    VALUES (
                        @BackupHeaderId, @ItemId, @Code, @Name, @Type, @JobGradeId, @ParentId,
                        @SAPCode, @Grade, @GradeTile, @PositionCode, @PositionName, @RegionId,
                        @RegionName, @BusinessModelId, @IsFromIT, @IsEdoc, @IsEdoc1,
                        @SAPObjectId, @SAPObjectType, @SAPDepartmentParentName, @SAPDepartmentParentId,
                        @SAPLevel, @SAPValidFrom, @SAPValidTo, @CreatedAt
                    );", conn, transaction);

                insertCmd.Parameters.AddWithValue("@BackupHeaderId", backupHeaderId);
                insertCmd.Parameters.AddWithValue("@ItemId", dept.Id);
                insertCmd.Parameters.AddWithValue("@Code", (object) dept.Code ?? DBNull.Value);
                insertCmd.Parameters.AddWithValue("@Name", (object)dept.Name ?? DBNull.Value);
                insertCmd.Parameters.AddWithValue("@Type", (int)dept.Type);
                insertCmd.Parameters.AddWithValue("@JobGradeId", (object)dept.JobGradeId ?? DBNull.Value);
                insertCmd.Parameters.AddWithValue("@ParentId", (object)dept.ParentId ?? DBNull.Value);
                insertCmd.Parameters.AddWithValue("@SAPCode", (object)dept.SAPCode ?? DBNull.Value);
                insertCmd.Parameters.AddWithValue("@Grade", dept.Grade);
                insertCmd.Parameters.AddWithValue("@GradeTile", (object)dept.GradeTile ?? DBNull.Value);
                insertCmd.Parameters.AddWithValue("@PositionCode", (object)dept.PositionCode ?? DBNull.Value);
                insertCmd.Parameters.AddWithValue("@PositionName", (object)dept.PositionName ?? DBNull.Value);
                insertCmd.Parameters.AddWithValue("@RegionId", (object)dept.RegionId ?? DBNull.Value);
                insertCmd.Parameters.AddWithValue("@RegionName", (object)dept.RegionName ?? DBNull.Value);
                insertCmd.Parameters.AddWithValue("@BusinessModelId", (object)dept.BusinessModelId ?? DBNull.Value);
                insertCmd.Parameters.AddWithValue("@IsFromIT", dept.IsFromIT);
                insertCmd.Parameters.AddWithValue("@IsEdoc", dept.IsEdoc);
                insertCmd.Parameters.AddWithValue("@IsEdoc1", dept.IsEdoc1);
                insertCmd.Parameters.AddWithValue("@SAPObjectId", (object)dept.SAPObjectId ?? DBNull.Value);
                insertCmd.Parameters.AddWithValue("@SAPObjectType", (object)dept.SAPObjectType ?? DBNull.Value);
                insertCmd.Parameters.AddWithValue("@SAPDepartmentParentName", (object)dept.SAPDepartmentParentName ?? DBNull.Value);
                insertCmd.Parameters.AddWithValue("@SAPDepartmentParentId", (object)dept.SAPDepartmentParentId ?? DBNull.Value);
                insertCmd.Parameters.AddWithValue("@SAPLevel", (object)dept.SAPLevel ?? DBNull.Value);
                insertCmd.Parameters.AddWithValue("@SAPValidFrom", (object)dept.SAPValidFrom ?? DBNull.Value);
                insertCmd.Parameters.AddWithValue("@SAPValidTo", (object)dept.SAPValidTo ?? DBNull.Value);
                insertCmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                await insertCmd.ExecuteNonQueryAsync();
            }
        }
    }
}