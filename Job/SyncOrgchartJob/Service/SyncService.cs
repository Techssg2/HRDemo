using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SyncOrgchartJob.Enums;
using SyncOrgchartJob.Models;
using SyncOrgchartJob.Models.eDocHR;

namespace SyncOrgchartJob.Service
{
    public class SyncService
    {
        private readonly string _apiUrl = ConfigurationManager.AppSettings["ApiUrl"];
        private readonly string _connStr = ConfigurationManager.AppSettings["StagingDb"];
        private readonly string _syncType = ConfigurationManager.AppSettings["SyncType"];
        private readonly bool usingAPI = bool.Parse(ConfigurationManager.AppSettings["usingAPI"]);

        public async Task<int> RunAsync()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Logger.Write($"Test connection to API: {_apiUrl} ");
            Console.WriteLine($"🔗Testing connection to API: {_apiUrl} ");
            Console.WriteLine($"UsingAPI: {usingAPI}");
            List<SapOrganizationItem> orgItems = new List<SapOrganizationItem>();
            if (usingAPI)
            {
                orgItems = await FetchFromApi();
            }
            else
            {
                orgItems = await ReadDataFromFileAPI();
            }
            if (orgItems == null || orgItems.Count == 0)
            {
                Console.WriteLine("No data fetched from API.");
                throw new Exception("No data fetched from API.");
            }
            
            var validationErrors = await ValidationData(orgItems);
            if (validationErrors.Count > 0)
            {
                throw new Exception("Validation errors found: " + string.Join(", ", validationErrors));
            }

            int headerId = 0;
            var syncTime = DateTime.Now;
            var rawJson = JsonConvert.SerializeObject(orgItems); // hoặc lưu raw gốc từ API nếu có
            using (var conn = new SqlConnection(_connStr))
            {
                await conn.OpenAsync();

                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        // Insert Staging_Header
                        var insertHeaderCmd = new SqlCommand(@"
                        INSERT INTO Staging_Header (SyncType, SyncTime, Status, JsonAllData, CreatedAt)
                        OUTPUT INSERTED.Id
                        VALUES (@syncType, @syncTime, @status, @jsonAllData, GETDATE())", conn, tran);

                        insertHeaderCmd.Parameters.AddWithValue("@syncType","Department");
                        insertHeaderCmd.Parameters.AddWithValue("@syncTime", syncTime);
                        insertHeaderCmd.Parameters.AddWithValue("@status", "Pending");
                        insertHeaderCmd.Parameters.AddWithValue("@jsonAllData","rawJson");

                        var result = await insertHeaderCmd.ExecuteScalarAsync();
                        if (result == null || result == DBNull.Value)
                        {
                            throw new Exception("Insert Staging_Header failed. No Id returned.");
                        }
                        headerId = Convert.ToInt32(result);
                        Console.WriteLine($"Sync complete. Header ID: {headerId}");

                        // Insert từng dòng Staging_Detail
                        foreach (var item in orgItems)
                        {
                            var jsonItem = JsonConvert.SerializeObject(item);

                            var insertDetailCmd = new SqlCommand($@"
                            INSERT INTO Staging_Detail (
                                HeaderId, ObjectId, ObjectName, ObjectType,
                                ParentId, ParentName, IsHead, EmployeeJg,
                                OrgJobgrade, PersonalArea, SubArea,
                                ValidFrom, ValidTo, ModifyDate, JsonData, CreatedAt, Level, EmployeeId, AbbrName, Concurrently)
                            VALUES (
                                @headerId, @objectId, @objectName, @objectType,
                                @parentId, @parentName, @isHead, @employeeJg,
                                @orgJobgrade, @personalArea, @subArea,
                                @validFrom, @validTo, @modifyDate, @jsonData, GETDATE(), @level, @employeeId, @abbrName, @concurrently)", conn, tran);

                            insertDetailCmd.Parameters.AddWithValue("@headerId", headerId);
                            insertDetailCmd.Parameters.AddWithValue("@objectId", item.ObjectId);
                            insertDetailCmd.Parameters.AddWithValue("@objectName",
                                item.ObjectName ?? (object)DBNull.Value);
                            insertDetailCmd.Parameters.AddWithValue("@objectType",
                                item.ObjectType ?? (object)DBNull.Value);
                            insertDetailCmd.Parameters.AddWithValue("@parentId", item.ParentId);
                            insertDetailCmd.Parameters.AddWithValue("@parentName",
                                item.ParentName ?? (object)DBNull.Value);
                            insertDetailCmd.Parameters.AddWithValue("@isHead", item.IsHead);
                            insertDetailCmd.Parameters.AddWithValue("@employeeJg",
                                item.EmployeeJg ?? (object)DBNull.Value);
                            insertDetailCmd.Parameters.AddWithValue("@orgJobgrade",
                                item.OrgJobgrade ?? (object)DBNull.Value);
                            insertDetailCmd.Parameters.AddWithValue("@personalArea",
                                item.PersonalArea ?? (object)DBNull.Value);
                            insertDetailCmd.Parameters.AddWithValue("@subArea", item.SubArea ?? (object)DBNull.Value);
                            insertDetailCmd.Parameters.AddWithValue("@validFrom", item.ValidFrom);
                            insertDetailCmd.Parameters.AddWithValue("@validTo", item.ValidTo);
                            insertDetailCmd.Parameters.AddWithValue("@modifyDate", item.ModifyDate);
                            insertDetailCmd.Parameters.AddWithValue("@jsonData", jsonItem);
                            insertDetailCmd.Parameters.AddWithValue("@level", item.Level);
                            insertDetailCmd.Parameters.AddWithValue("@employeeId", item.EmployeeId);
                            insertDetailCmd.Parameters.AddWithValue("@abbrName", item.AbbrName);
                            insertDetailCmd.Parameters.AddWithValue("@concurrently", item.Concurrently);
                            await insertDetailCmd.ExecuteNonQueryAsync();
                        }

                        tran.Commit();
                        Console.WriteLine($"✅ Inserted {orgItems.Count} records to staging. HeaderId: {headerId}");
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        Console.WriteLine("❌ Error: " + ex.Message);
                        Logger.Write("Insert DB failed: " + ex.ToString());
                        throw new Exception("Insert DB failed", ex);
                    }
                }
            }
            return headerId;
        }

        private async Task<List<SapOrganizationItem>> FetchFromApi()
        {
            try
            {
                var apiUrl = ConfigurationManager.AppSettings["ApiUrl"];
                var username = ConfigurationManager.AppSettings["ApiUsername"];
                var password = ConfigurationManager.AppSettings["ApiPassword"];

                var credentials = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{username}:{password}"));
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    
                    var response = await client.GetAsync(apiUrl);
                    response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStringAsync();
                    var wrapper = JsonConvert.DeserializeObject<SapResponseWrapper>(content);
                    return wrapper?.d?.results ?? new List<SapOrganizationItem>();
                }
            }
            catch (Exception ex)
            {
                Logger.Write("API call failed: " + ex.Message);
                Console.WriteLine("❌ Lỗi khi gọi API: " + ex.Message);
                throw new Exception("Lỗi khi gọi API", ex);
            }
        }

        private async Task<List<SapOrganizationItem>> ReadDataFromFileAPI()
        {
            var result = new List<SapOrganizationItem>();

            try
            {
                string relativePath = @"read\file.txt";
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string filePath = Path.Combine(baseDirectory, relativePath);

                if (!File.Exists(filePath))
                {
                    Console.WriteLine("Không tìm thấy file: " + filePath);
                    return result;
                }

                string json = File.ReadAllText(filePath);
                var wrapper = JsonConvert.DeserializeObject<SapResponseWrapper>(json);
                result = wrapper?.d?.results ?? new List<SapOrganizationItem>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi đọc file JSON: " + ex.Message);
                Logger.Write("Lỗi khi đọc file JSON: " + ex.Message);
                throw new Exception("Lỗi khi đọc file JSON", ex);
            }

            return result;
        }
        
        private async Task InsertErrorLogs(List<string> errorMessages, List<SapOrganizationItem> orgItems)
        {
            using (var conn = new SqlConnection(_connStr))
            {
                await conn.OpenAsync();

                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        // Insert Staging_Header
                        var insertHeaderCmd = new SqlCommand(@"
                        INSERT INTO Staging_Header (SyncType, SyncTime, Status, JsonAllData, CreatedAt, Note)
                        OUTPUT INSERTED.Id
                        VALUES (@syncType, @syncTime, @status, @jsonAllData, GETDATE(), @Note)", conn, tran);

                        insertHeaderCmd.Parameters.AddWithValue("@syncType","Department");
                        insertHeaderCmd.Parameters.AddWithValue("@syncTime", DateTime.Now);
                        insertHeaderCmd.Parameters.AddWithValue("@status", "Failed");
                        insertHeaderCmd.Parameters.AddWithValue("@jsonAllData", JsonConvert.SerializeObject(orgItems));
                        insertHeaderCmd.Parameters.AddWithValue("@Note", JsonConvert.SerializeObject(errorMessages));

                        await insertHeaderCmd.ExecuteScalarAsync();
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        Console.WriteLine("❌ Error inserting error logs: " + ex.Message);
                        Logger.Write("Insert DB failed: " + ex.Message, true);
                        throw new Exception("Insert DB failed", ex);
                    }
                }
            }
        }
        
        private async Task<List<string>> ValidationData(List<SapOrganizationItem> orgItems)
        {
            var errors = new List<string>();
            try
            {
                // Chỉ đồng bộ các user có EmployeeJg khác "G0"
                orgItems = orgItems.Where(x => x.EmployeeJg != "G0" || x.OrgJobgrade != "G0").ToList();
                
                List<JobGrade> jobGrades = await DataService.GetListJobGrade();
                List<string> titleCodes = jobGrades.Select(x => x.Title).ToList();
                titleCodes.Add("G10"); // Thêm G10 vào danh sách title code
                int minQty = ConfigurationManager.AppSettings["MinQty"] != null
                    ? int.Parse(ConfigurationManager.AppSettings["MinQty"])
                    : 5000;

                var departments = await DataService.GetAllDepartmentsAsync();
                // var userInDepartments = await DataService.GetAllUserInDepartmentMappings();
                var users = await DataService.GetListUsers();

                var userSapCodeEdocIds = users.Where(x => x.IsActivated && !x.IsDeleted).ToList().Select(x => x.SAPCode)
                    .ToList();
                var departmentSapCodeEdocIds = departments.Where(x => !string.IsNullOrEmpty(x.SAPCode) && !x.IsDeleted)
                    .Select(x => x.SAPCode).ToList();

                var findDifferentOrgJobgrade = orgItems
                    .Where(x => !string.IsNullOrEmpty(x.OrgJobgrade) && !titleCodes.Contains(x.OrgJobgrade))
                    .Select(x => x.OrgJobgrade)
                    .Distinct()
                    .ToList();

                if (findDifferentOrgJobgrade.Any())
                    errors.Add($"Các OrgJobGrade không hợp lệ: {string.Join(", ", findDifferentOrgJobgrade)}");

                var findDifferentEmployeeJg = orgItems
                    .Where(x => !string.IsNullOrEmpty(x.EmployeeJg) && !titleCodes.Contains(x.EmployeeJg))
                    .Select(x => x.EmployeeJg)
                    .Distinct()
                    .ToList();

                if (findDifferentEmployeeJg.Any())
                    errors.Add($"Các EmployeeJg không hợp lệ: {string.Join(", ", findDifferentEmployeeJg)}");

                if (orgItems.Count <= (minQty * 2))
                    errors.Add($"<<{orgItems.Count}>>| Số lượng API SAP trả về ít hơn giới hạn {(minQty * 2)} bản ghi.");

                var countDepartmentSap = orgItems.Where(x =>
                    departmentSapCodeEdocIds.Contains(x.ObjectId) && x.ObjectType == ObjectType.Position).ToList();
                var countUserSap = orgItems
                    .Where(x => userSapCodeEdocIds.Contains(x.ObjectId) && x.ObjectType == ObjectType.Person).ToList();

                if (countDepartmentSap.Count <= minQty)
                    errors.Add($"<<{countDepartmentSap.Count}>> | Số lượng Department SAP bé hơn giới hạn {minQty} bản ghi.");

                if (countUserSap.Count <= minQty)
                    errors.Add($"<<{countUserSap.Count}>> | Số lượng User SAP bé hơn giới hạn {minQty} bản ghi.");
            }
            catch (NullReferenceException nullEx)
            {
                errors.Add($"NullReferenceException: {nullEx.Message}");
                Logger.Write("❌ NullReferenceException: " + nullEx.ToString(), true);
            }
            catch (FormatException formatEx)
            {
                errors.Add($"FormatException: {formatEx.Message}");
                Logger.Write("❌ FormatException: " + formatEx.ToString(), true);
            }
            catch (Exception ex)
            {
                errors.Add($"Lỗi khi kiểm tra dữ liệu: {ex.Message}");
                Logger.Write("ValidationData error: " + ex.Message, true);
                Console.WriteLine("❌ Lỗi khi kiểm tra dữ liệu: " + ex.Message);
            }
            finally
            {
                if (errors.Count > 0)
                {
                    await InsertErrorLogs(errors, orgItems);
                }
            }
            return errors;
        }
    }
}