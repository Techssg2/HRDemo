using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Aeon.HR.Infrastructure.Enums;
using Newtonsoft.Json;
using SyncOrgchartJob.Enums;
using SyncOrgchartJob.Models;
using SyncOrgchartJob.Models.eDocHR;
using SyncOrgchartJob.ViewModel;

namespace SyncOrgchartJob.Service
{
    public static class DataService
    {
        private static readonly string _connHRStr = ConfigurationManager.AppSettings["HRDb"];
        private static readonly string _connStr = ConfigurationManager.AppSettings["StagingDb"];

        private static readonly Guid _metadataPositionId =
            ConfigurationManager.AppSettings["MetadataPositionId"] != null
                ? Guid.Parse(ConfigurationManager.AppSettings["MetadataPositionId"].ToString())
                : new Guid("11DF2C4C-65D2-4314-A552-850F6808F657");
        
        public static async Task<StagingHeaderDto> GetPendingHeaderAsync(int headerId, SqlConnection conn)
        {
            var query = @"SELECT TOP 1 * FROM Staging_Header WHERE Status = 'Pending' and Id = @headerId ORDER BY SyncTime DESC";
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@headerId", headerId);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new StagingHeaderDto
                        {
                            Id = (int) reader["Id"],
                            SyncType = reader["SyncType"].ToString(),
                            SyncTime = (DateTime)reader["SyncTime"],
                            JsonAllData = reader["JsonAllData"].ToString(),
                            Status = reader["Status"].ToString()
                        };
                    }
                }
            }
            return null;
        }

        public static  async Task<List<StagingDetailDto>> GetDetailsByHeaderIdAsync(SqlConnection conn, int headerId)
        {
            var results = new List<StagingDetailDto>();
            var query = @"SELECT * FROM Staging_Detail WHERE HeaderId = @headerId order by OrgJobgrade desc";
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@headerId", headerId);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        results.Add(new StagingDetailDto
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            HeaderId = Convert.ToInt32(reader["HeaderId"]),
                            ObjectId = reader["ObjectId"].ToString(),
                            ObjectName = reader["ObjectName"].ToString(),
                            ObjectType = reader["ObjectType"].ToString(),
                            ParentId = reader["ParentId"].ToString(),
                            ParentName = reader["ParentName"].ToString(),
                            IsHead = reader["IsHead"].ToString(),
                            EmployeeJg = reader["EmployeeJg"].ToString(),
                            OrgJobgrade = reader["OrgJobgrade"].ToString(),
                            PersonalArea = reader["PersonalArea"].ToString(),
                            SubArea = reader["SubArea"].ToString(),
                            ValidFrom = reader["ValidFrom"].ToString(),
                            ValidTo = reader["ValidTo"].ToString(),
                            ModifyDate = reader["ModifyDate"].ToString(),
                            JsonData = reader["JsonData"].ToString(),
                            CreatedAt = reader["CreatedAt"].ToString(),
                            Level = reader["Level"].ToString(),
                            EmployeeId = reader["EmployeeId"].ToString(),
                            AbbrName = reader["AbbrName"].ToString(),
                            Concurrently = reader["Concurrently"].ToString(),
                        });
                    }
                }
            }

            return results;
        }
        
        public static  async Task<List<Department>> GetAllDepartmentsAsync(bool isDeleted = false)
        {
            var results = new List<Department>();
            using (var conn = new SqlConnection(_connHRStr))
            {
                await conn.OpenAsync();
                var query = $@"
                    select 
	                    ide.IsEdoc1, 
	                    re.RegionName,
	                    jd.Grade Grade,
	                    jd.Title GradeTile,
	                    pade.Code ParentCode,
	                    pade.SAPCode ParentSapCode,
	                    pade.Name ParentName,
	                    de.* 
                    from 
	                    Departments de
	                    join ITDepartments ide on (de.Id = ide.Id)
	                    left join Regions re on (re.Id = de.RegionId)
	                    left join JobGrades jd on (de.JobGradeId = jd.Id)
	                    left join Departments pade on (de.ParentId = pade.Id)
                    where
	                    de.Id = ide.Id  ";

                if (!isDeleted)
                    query += $"and de.IsDeleted = 0";
                
                using (var cmd = new SqlCommand(query, conn))
                {
                    // cmd.Parameters.AddWithValue("@headerId", headerId);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var departmentId = Guid.Parse(reader["Id"].ToString());
                            results.Add(new Department
                            {
                                Id = Guid.Parse(reader["Id"].ToString()),
                                Code = reader["Code"]?.ToString(),
                                Name = reader["Name"]?.ToString(),
                                ParentCode = reader["ParentCode"]?.ToString(),
                                ParentSapCode = reader["ParentSapCode"]?.ToString(),
                                ParentName = reader["ParentName"]?.ToString(),
                                Type = (DepartmentType)reader.GetInt32(reader.GetOrdinal("Type")),
                                JobGradeId = reader.GetGuid(reader.GetOrdinal("JobGradeId")),
                                ParentId = reader["ParentId"] != DBNull.Value ? (Guid?)reader["ParentId"] : null,
                                SAPCode = reader["SAPCode"]?.ToString(),
                                Grade = reader.GetInt32(reader.GetOrdinal("Grade")),
                                GradeTile = reader["GradeTile"]?.ToString(),
                                PositionCode = reader["PositionCode"]?.ToString(),
                                PositionName = reader["PositionName"]?.ToString(),
                                RegionId = reader["RegionId"] as Guid?,
                                RegionName = reader["RegionName"]?.ToString(),
                                BusinessModelId = reader["BusinessModelId"] as Guid?,
                                IsFromIT = reader.GetBoolean(reader.GetOrdinal("IsFromIT")),
                                IsEdoc = HasColumn(reader, "IsEdoc") && !Convert.IsDBNull(reader["IsEdoc"]) && bool.Parse(reader["IsEdoc"].ToString()),
                                IsDeleted = HasColumn(reader, "IsDeleted") && !Convert.IsDBNull(reader["IsDeleted"]) && bool.Parse(reader["IsDeleted"].ToString()),
                                IsEdoc1 = !reader.IsDBNull(reader.GetOrdinal("IsEdoc1")) && reader.GetBoolean(reader.GetOrdinal("IsEdoc1")),
                                SAPObjectId = HasColumn(reader, "SAPObjectId") ? reader["SAPObjectId"]?.ToString() : null,
                                SAPObjectType = HasColumn(reader, "SAPObjectType") ? reader["SAPObjectType"]?.ToString() : null,
                                SAPDepartmentParentName = HasColumn(reader, "SAPDepartmentParentName") ? reader["SAPDepartmentParentName"]?.ToString() : null,
                                SAPDepartmentParentId = HasColumn(reader, "SAPDepartmentParentId") ? reader["SAPDepartmentParentId"]?.ToString() : null,
                                SAPLevel = HasColumn(reader, "SAPLevel") ? reader["SAPLevel"]?.ToString() : null,
                                SAPValidFrom = HasColumn(reader, "SAPValidFrom") ? reader["SAPValidFrom"]?.ToString() : null,
                                SAPValidTo = HasColumn(reader, "SAPValidTo") ? reader["SAPValidTo"]?.ToString() : null,
                                Persons = await GetUserMappingsByDepartmentId(departmentId, conn)
                            });
                        }
                    }
                }
            }

            return results;
        }
        
        
        public static async Task<DepartmentViewModel> GetDepartmentById(Guid id)
        {
            using (var conn = new SqlConnection(_connHRStr))
            {
                await conn.OpenAsync();
                var query = $@"select 
	                pa.Name ParentName,
	                pa.Code ParentCode,
	                hr.Code HrDepartmentCode,
                    hr.Name HrDepartmentName,
	                jg.Grade JobGradeGrade,
	                jg.Caption JobGradeCaption,
	                ccr.Code CostCenterRecruitmentCode,
	                re.RegionName,
	                de.* 
                from 
	                Departments de
	                left join Departments pa on (pa.Id = de.ParentId)
	                left join Departments hr on (de.HrDepartmentId = hr.id)
	                left join CostCenterRecruitments ccr on (de.CostCenterRecruitmentId = ccr.Id)
	                left join JobGrades jg on (jg.id = de.JobGradeId)
	                left join Regions re on (re.Id = de.JobGradeId)
                where de.IsDeleted = 0 and de.IsFromIT = 0 and de.Id = '{id}'";
                using (var cmd = new SqlCommand(query, conn))
                {
                    // cmd.Parameters.AddWithValue("@headerId", headerId);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            return new DepartmentViewModel
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                                ParentId = reader["ParentId"] != DBNull.Value ? (Guid?)reader["ParentId"] : null,
                                ParentName = reader["ParentName"]?.ToString(),
                                ParentCode = reader["ParentCode"]?.ToString(),
                                Code = reader["Code"]?.ToString(),
                                Name = reader["Name"]?.ToString(),
                                JobGradeCaption = reader["JobGradeCaption"]?.ToString(),
                                JobGradeId = reader["JobGradeId"] != DBNull.Value ? (Guid?)reader["JobGradeId"] : null,
                                JobGradeGrade = reader["JobGradeGrade"] != DBNull.Value ? Convert.ToInt32(reader["JobGradeGrade"]) : 0,
                                IsStore = reader["IsStore"] != DBNull.Value && (bool)reader["IsStore"],
                                IsHr = reader["IsHr"] != DBNull.Value && (bool)reader["IsHr"],
                                IsCB = reader["IsCB"] != DBNull.Value && (bool)reader["IsCB"],
                                IsPerfomance = reader["IsPerfomance"] != DBNull.Value && (bool)reader["IsPerfomance"],
                                IsAdmin = reader["IsAdmin"] != DBNull.Value && (bool)reader["IsAdmin"],
                                IsHR = reader["IsHR"] != DBNull.Value && (bool)reader["IsHR"],
                                IsFacility = reader["IsFacility"] != DBNull.Value && (bool)reader["IsFacility"],
                                IsMD = reader["IsMD"] != DBNull.Value && (bool)reader["IsMD"],
                                IsSM = reader["IsSM"] != DBNull.Value && (bool)reader["IsSM"],
                                HrDepartmentId = reader["HrDepartmentId"] != DBNull.Value ? (Guid?)reader["HrDepartmentId"] : null,
                                HrDepartmentName = reader["HrDepartmentName"]?.ToString(),
                                HrDepartmentCode = reader["HrDepartmentCode"]?.ToString(),
                                PositionCode = reader["PositionCode"]?.ToString(),
                                PositionName = reader["PositionName"]?.ToString(),
                                SAPCode = reader["SAPCode"]?.ToString(),
                                RegionId = reader["RegionId"] != DBNull.Value ? (Guid?)reader["RegionId"] : null,
                                RegionName = reader["RegionName"]?.ToString(),
                                Type = (DepartmentType)(reader["Type"] != DBNull.Value ? Convert.ToInt32(reader["Type"]) : 0),
                                Note = reader["Note"]?.ToString(),
                                CostCenterRecruitmentId = reader["CostCenterRecruitmentId"] != DBNull.Value ? (Guid?)reader["CostCenterRecruitmentId"] : null,
                                CostCenterRecruitmentCode = reader["CostCenterRecruitmentCode"]?.ToString(),
                                BusinessModelId = reader["BusinessModelId"] != DBNull.Value ? (Guid?)reader["BusinessModelId"] : null,
                                IsMMD = reader["IsMMD"] != DBNull.Value && (bool)reader["IsMMD"],
                                ApplyChildSMMDMMD = reader["ApplyChildSMMDMMD"] != DBNull.Value && (bool)reader["ApplyChildSMMDMMD"],
                                IsEdoc1 = !reader.IsDBNull(reader.GetOrdinal("IsEdoc1")) && reader.GetBoolean(reader.GetOrdinal("IsEdoc1")),
                                IsEdoc = HasColumn(reader, "IsEdoc") && !Convert.IsDBNull(reader["IsEdoc"]) && bool.Parse(reader["IsEdoc"].ToString()),
                                SAPObjectId = HasColumn(reader, "SAPObjectId") ? reader["SAPObjectId"]?.ToString() : null,
                                SAPObjectType = HasColumn(reader, "SAPObjectType") ? reader["SAPObjectType"]?.ToString() : null,
                                SAPDepartmentParentName = HasColumn(reader, "SAPDepartmentParentName") ? reader["SAPDepartmentParentName"]?.ToString() : null,
                                SAPDepartmentParentId = HasColumn(reader, "SAPDepartmentParentId") ? reader["SAPDepartmentParentId"]?.ToString() : null,
                                SAPLevel = HasColumn(reader, "SAPLevel") ? reader["SAPLevel"]?.ToString() : null,
                                SAPValidFrom = HasColumn(reader, "SAPValidFrom") ? reader["SAPValidFrom"]?.ToString() : null,
                                SAPValidTo = HasColumn(reader, "SAPValidTo") ? reader["SAPValidTo"]?.ToString() : null,
                            };

                        }
                    }
                }
            }
            return null;
        }

        private static  async Task<List<UserDepartmentMapping>> GetUserMappingsByDepartmentId(Guid departmentId, SqlConnection conn)
        {
            var list = new List<UserDepartmentMapping>();

            var query = @"
                    SELECT us.LoginName, us.SAPCode, us.FullName, udm.* 
                    FROM UserDepartmentMappings udm 
                    JOIN Users us ON udm.UserId = us.Id 
                    WHERE udm.DepartmentId = @DepartmentId";

            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@DepartmentId", departmentId);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        list.Add(new UserDepartmentMapping
                        {
                            Id = reader.GetGuid(reader.GetOrdinal("Id")),
                            DepartmentId = departmentId,
                            UserId = reader.GetGuid(reader.GetOrdinal("UserId")),
                            LoginName = reader["LoginName"]?.ToString(),
                            UserSAPCode = reader["SAPCode"]?.ToString(),
                            FullName = reader["FullName"]?.ToString(),
                            Role = (Group)reader.GetInt32(reader.GetOrdinal("Role")),
                            IsHeadCount = reader.GetBoolean(reader.GetOrdinal("IsHeadCount")),
                            IsEdoc = HasColumn(reader, "IsEdoc") && !Convert.IsDBNull(reader["IsEdoc"]) && bool.Parse(reader["IsEdoc"].ToString()),
                            Authorizated = reader["Authorizated"] != DBNull.Value ? (bool?)reader["Authorizated"] : null,
                            IsFromIT = reader.GetBoolean(reader.GetOrdinal("IsFromIT")),
                            IsDeleted = reader.GetBoolean(reader.GetOrdinal("IsDeleted")),
                            Created = reader.GetDateTimeOffset(reader.GetOrdinal("Created")),
                            Modified = reader.GetDateTimeOffset(reader.GetOrdinal("Modified")),
                            CreatedById = reader["CreatedById"] != DBNull.Value ? (Guid?)reader["CreatedById"] : null,
                        });
                    }
                }
            }

            return list;
        }
        
        public static async Task<List<UserDepartmentMapping>> GetAllUserInDepartmentMappings(bool isDeleted = false)
        {
            var list = new List<UserDepartmentMapping>();

            using (var conn = new SqlConnection(_connHRStr))
            {
                await conn.OpenAsync();
                var query = @"
                    SELECT us.LoginName, us.SAPCode, us.FullName, udm.* 
                    FROM UserDepartmentMappings udm 
                    JOIN Users us ON udm.UserId = us.Id";
                
                if (!isDeleted)
                    query += " where udm.IsDeleted = 0";
                
                using (var cmd = new SqlCommand(query, conn))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            if (!reader.HasRows)
                            {
                                // Ghi log nếu cần: logger.LogWarning("No rows returned from query");
                                return list; // hoặc return empty nếu không có dữ liệu
                            }
                            
                            list.Add(new UserDepartmentMapping
                            {
                                Id = reader.IsDBNull(reader.GetOrdinal("Id")) 
                                    ? (Guid) Guid.Empty 
                                    : reader.GetGuid(reader.GetOrdinal("Id")),

                                UserId = reader.IsDBNull(reader.GetOrdinal("UserId")) 
                                    ? (Guid)  Guid.Empty
                                    : reader.GetGuid(reader.GetOrdinal("UserId")),

                                DepartmentId = reader.IsDBNull(reader.GetOrdinal("DepartmentId")) 
                                    ? (Guid) Guid.Empty
                                    : reader.GetGuid(reader.GetOrdinal("DepartmentId")),
                                LoginName = HasColumn(reader, "LoginName") && !reader.IsDBNull(reader.GetOrdinal("LoginName"))
                                    ? reader.GetString(reader.GetOrdinal("LoginName"))
                                    : null,
                                UserSAPCode = HasColumn(reader, "SAPCode") && !reader.IsDBNull(reader.GetOrdinal("SAPCode"))
                                    ? reader.GetString(reader.GetOrdinal("SAPCode"))
                                    : null,
                                FullName = HasColumn(reader, "FullName") && !reader.IsDBNull(reader.GetOrdinal("FullName"))
                                    ? reader.GetString(reader.GetOrdinal("FullName"))
                                    : null,
                                Role = HasColumn(reader, "Role") && !reader.IsDBNull(reader.GetOrdinal("Role"))
                                    ? (Group)reader.GetInt32(reader.GetOrdinal("Role"))
                                    : default,
                                IsHeadCount = HasColumn(reader, "IsHeadCount") && !reader.IsDBNull(reader.GetOrdinal("IsHeadCount")) && reader.GetBoolean(reader.GetOrdinal("IsHeadCount")),
                                IsEdoc = HasColumn(reader, "IsEdoc") && !reader.IsDBNull(reader.GetOrdinal("IsEdoc")) && reader.GetBoolean(reader.GetOrdinal("IsEdoc")),
                                Authorizated = HasColumn(reader, "Authorizated") && !reader.IsDBNull(reader.GetOrdinal("Authorizated"))
                                    ? (bool?)reader.GetBoolean(reader.GetOrdinal("Authorizated"))
                                    : null,
                                IsFromIT = HasColumn(reader, "IsFromIT") && !reader.IsDBNull(reader.GetOrdinal("IsFromIT")) && reader.GetBoolean(reader.GetOrdinal("IsFromIT")),
                                IsDeleted = HasColumn(reader, "IsDeleted") && !reader.IsDBNull(reader.GetOrdinal("IsDeleted")) && reader.GetBoolean(reader.GetOrdinal("IsDeleted")),
                                Created = HasColumn(reader, "Created") && !reader.IsDBNull(reader.GetOrdinal("Created"))
                                    ? reader.GetDateTimeOffset(reader.GetOrdinal("Created"))
                                    : default,

                                Modified = HasColumn(reader, "Modified") && !reader.IsDBNull(reader.GetOrdinal("Modified"))
                                    ? reader.GetDateTimeOffset(reader.GetOrdinal("Modified"))
                                    : default,
                                CreatedById = HasColumn(reader, "CreatedById") && !reader.IsDBNull(reader.GetOrdinal("CreatedById"))
                                    ? (Guid?)reader.GetGuid(reader.GetOrdinal("CreatedById"))
                                    : null
                            });
                        }
                    }
                }
            }
            return list;
        }
        
        public static async Task<List<JobGrade>> GetListJobGrade()
        {
            var results = new List<JobGrade>();
            using (var conn = new SqlConnection(_connHRStr))
            {
                await conn.OpenAsync();
                var query = $@"select * from JobGrades where IsDeleted = 0";
                using (var cmd = new SqlCommand(query, conn))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            results.Add(new JobGrade
                            {
                                Id = Guid.Parse(reader["Id"].ToString()),
                                Grade = int.Parse(reader["Grade"].ToString()),
                                Title = reader["Title"]?.ToString(),
                                Caption = reader["Caption"]?.ToString(),
                                DepartmentType = (DepartmentType) int.Parse(reader["DepartmentType"].ToString()),
                            });
                        }
                    }
                }
            }
            return results;
        }
        
        public static async Task<List<User>> GetListUsers(bool isDeleted = false)
        {
            var results = new List<User>();
            using (var conn = new SqlConnection(_connHRStr))
            {
                await conn.OpenAsync();
                var query = $@"select * from Users";
                if (!isDeleted)
                {
                    query += " where IsDeleted = 0";
                }
                using (var cmd = new SqlCommand(query, conn))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            results.Add(new User()
                            {
                                Id = Guid.Parse(reader["Id"].ToString()),
                                FullName = reader["FullName"]?.ToString(),
                                SAPCode = reader["SAPCode"]?.ToString(),
                                Email = reader["Email"]?.ToString(),
                                Role = (UserRole) int.Parse(reader["Role"].ToString()),
                                Type = (LoginType) int.Parse(reader["Type"].ToString()),
                                IsActivated = bool.Parse(reader["IsActivated"].ToString()),
                                IsDeleted = bool.Parse(reader["IsDeleted"].ToString()),
                                IsEdoc = HasColumn(reader, "IsEdoc") && !Convert.IsDBNull(reader["IsEdoc"]) && bool.Parse(reader["IsEdoc"].ToString()),
                                IsFromIT = bool.Parse(reader["IsFromIT"].ToString()),
                            });
                        }
                    }
                }
            }
            return results;
        }
        
        private static bool HasColumn(SqlDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
        
        public static async Task<List<Position>> GetListPositions()
        {
            var results = new List<Position>();
            using (var conn = new SqlConnection(_connHRStr))
            {
                await conn.OpenAsync();
                var query = $@"select * from MasterDatas where MetaDataTypeId = '{_metadataPositionId}' and IsDeleted = 0";
                using (var cmd = new SqlCommand(query, conn))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            results.Add(new Position()
                            {
                                Id = Guid.Parse(reader["Id"].ToString()),
                                Code = reader["Code"]?.ToString(),
                                Name = reader["Name"]?.ToString()
                            });
                        }
                    }
                }
            }
            return results;
        }
        
        public static async Task InsertPositions(this List<Position> models, SqlConnection _connection, SqlTransaction sqlTransaction)
        {
            try
            {
                foreach (var model in models)
                {
                    if (model.JobGradeId == Guid.Empty)
                    {
                        Logger.Write($"JobGradeId is empty for Position: {model.Code} - {model.Name}");
                        throw new Exception($"JobGradeId is empty for Position: {model.Code} - {model.Name}");
                    }
                    
                    var query = @"
                        INSERT INTO MasterDatas (
                             Id, 
                             Code, 
                             Name, 
                             Description, 
                             SourceFrom, 
                             MetaDataTypeId, 
                             IsDeleted, 
                             Created, 
                             Modified, 
                             CreatedById,
                             CreatedBy,
                             CreatedByFullName,
                             ModifiedById,
                             ModifiedBy,
                             ModifiedByFullName,
                            JobGradeId
                        ) VALUES (
                             @Id, 
                             @Code, 
                             @Name, 
                             @Description, 
                             @SourceFrom, 
                             @MetaDataTypeId, 
                             @IsDeleted, 
                             @Created, 
                             @Modified, 
                             @CreatedById,
                             @CreatedBy,
                             @CreatedByFullName,
                             @ModifiedById,
                             @ModifiedBy,
                             @ModifiedByFullName,
                             @JobGradeId
                        )";

                    var newPosition = new Position()
                    {
                        Id = Guid.NewGuid(),
                        Code = model.Code,
                        Name = model.Name,
                        JobGradeId = model.JobGradeId
                    };
                    
                    using (var cmd = new SqlCommand(query, _connection, sqlTransaction))
                    {
                        cmd.Transaction = sqlTransaction;
                        cmd.Parameters.AddWithValue("@Id", newPosition.Id);
                        cmd.Parameters.AddWithValue("@Code", newPosition.Code);
                        cmd.Parameters.AddWithValue("@Name", newPosition.Name);
                        cmd.Parameters.AddWithValue("@Description", (object) DBNull.Value);
                        cmd.Parameters.AddWithValue("@SourceFrom", 0);
                        cmd.Parameters.AddWithValue("@MetaDataTypeId", _metadataPositionId);
                        cmd.Parameters.AddWithValue("@IsDeleted", false);
                        cmd.Parameters.AddWithValue("@Created", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Modified", DateTime.Now);
                        cmd.Parameters.AddWithValue("@CreatedById", Guid.Parse("99A2F491-3103-4C01-9F6E-43359F3FBCD7"));
                        cmd.Parameters.AddWithValue("@CreatedBy", "SAdmin");
                        cmd.Parameters.AddWithValue("@CreatedByFullName", "Administrator");
                        cmd.Parameters.AddWithValue("@ModifiedById", Guid.Parse("99A2F491-3103-4C01-9F6E-43359F3FBCD7"));
                        cmd.Parameters.AddWithValue("@ModifiedBy", "SAdmin");
                        cmd.Parameters.AddWithValue("@ModifiedByFullName", "Administrator");
                        cmd.Parameters.AddWithValue("@JobGradeId", model.JobGradeId);
                        int rs = await cmd.ExecuteNonQueryAsync();
                        Logger.Write($"Insert Position: {model.Id} - {model.Code} - {model.Name} - {model.JobGradeId}", true);
                        if (rs <= 0)
                        {
                            Logger.Write($"Insert Position failed: {model.Id} - {model.Code} - {model.Name} - {model.JobGradeId}");
                            throw new Exception($"Insert Position failed: {model.Id} - {model.Code} - {model.Name} - {model.JobGradeId}");
                        }
                    }
                }
            } catch (Exception ex)
            {
                Logger.Write($"InsertPosition Error: {ex.Message}", true);
                throw new Exception($"InsertPosition Error: {ex.Message}");
            }
        }
        
        public static async Task<List<BusinessModel>> GetListBusinessModels()
        {
            var results = new List<BusinessModel>();
            using (var conn = new SqlConnection(_connHRStr))
            {
                await conn.OpenAsync();
                var query = $@"select * from BusinessModels where IsDeleted = 0";
                using (var cmd = new SqlCommand(query, conn))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            results.Add(new BusinessModel()
                            {
                                Id = Guid.Parse(reader["Id"].ToString()),
                                Code = reader["Code"]?.ToString(),
                                Name = reader["Name"]?.ToString()
                            });
                        }
                    }
                }
            }
            return results;
        }
        
        public static async Task<List<BusinessModelUnitMapping>> GetListBusinessModelUnitMapping()
        {
            var results = new List<BusinessModelUnitMapping>();
            using (var conn = new SqlConnection(_connHRStr))
            {
                await conn.OpenAsync();
                var query = $@"select * from BusinessModelUnitMappings where IsDeleted = 0";
                using (var cmd = new SqlCommand(query, conn))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var idStr = reader["Id"]?.ToString();
                            var businessModelIdStr = reader["BusinessModelId"]?.ToString();
                            var isStoreStr = reader["IsStore"]?.ToString();
                            Guid id, businessModelId;
                            bool isStore;
                            if (!Guid.TryParse(idStr, out id)) continue;
                            if (!Guid.TryParse(businessModelIdStr, out businessModelId)) businessModelId = Guid.Empty;
                            if (!bool.TryParse(isStoreStr, out isStore)) isStore = false;
                            results.Add(new BusinessModelUnitMapping()
                            {
                                Id = id,
                                BusinessModelId = businessModelId,
                                BusinessModelCode = reader["BusinessModelCode"]?.ToString(),
                                BusinessUnitCode = reader["BusinessUnitCode"]?.ToString(),
                                IsStore = isStore,
                            });
                        }
                    }
                }
            }
            return results;
        }
        
        public static async Task<List<MasterData>> GetListCacheMasterDataWorkLocation()
        {
            var results = new List<MasterData>();
            using (var conn = new SqlConnection(_connHRStr))
            {
                await conn.OpenAsync();
                var query = $@"
                        select * from CacheMasterDatas where Name = 'WorkLocation' order by Created desc
                        OFFSET 0 ROWS FETCH NEXT 1 ROWS ONLY;";
                using (var cmd = new SqlCommand(query, conn))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var data = reader["Data"]?.ToString();
                            var parsedData = JsonConvert.DeserializeObject<List<MasterData>>(data);
                            results.AddRange(parsedData);
                        }
                    }
                }
            }
            return results;
        }
    }
}

