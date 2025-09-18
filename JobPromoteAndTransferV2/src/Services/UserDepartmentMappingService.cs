
using JobPromoteAndTransfer.src.ModelEntity;
using JobPromoteAndTransferV2.src;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPromoteAndTransfer.src.Services
{
    public class UserDepartmentMappingService : JobPromoteAndTransferV2.src.SQLExcute.SQLQuery<UserDepartmentMappingEntity>
    {
        private readonly UserService _userService;
        public UserDepartmentMappingService() {
            _userService = new UserService();
        }

        public bool GetUserDepartmentMappingByUserId(Guid? currentId, Guid? newDeptOrLineId)
        {
            try
            {
                string query = $@"SELECT * FROM UserDepartmentMappings 
                          WHERE DepartmentId = '{newDeptOrLineId}' 
                          AND UserId = '{currentId}' 
                          AND IsHeadCount = 1";

                var users = this.GetItemsByQuery(query);

                bool hasResult = users != null && users.Any();
                Utilities.WriteLogError($"[INFO] Query result for UserId: {currentId}, DepartmentId: {newDeptOrLineId}. Found: {hasResult}");

                return hasResult;
            }
            catch (Exception ex)
            {
                Utilities.WriteLogError($"[ERROR] GetUserDepartmentMappingByUserId failed. UserId: {currentId}, DepartmentId: {newDeptOrLineId}. Exception: {ex}");
                return false;
            }
        }



        public bool DeleteUserDepartmentMappingByUserId(Guid? UserId, Guid? DepartmentId)
        {
            try
            {
                string query = $@"DELETE FROM UserDepartmentMappings 
                          WHERE UserId = '{UserId}' 
                          AND DepartmentId = '{DepartmentId}'";
                bool mappingResult = this.ExecuteRunQuery(query);

                Utilities.WriteLogError($"[INFO] Deleted UserDepartmentMapping. UserId: {UserId}, DepartmentId: {DepartmentId}. Success: {mappingResult}");
                return mappingResult;
            }
            catch (Exception ex)
            {
                Utilities.WriteLogError($"[ERROR] DeleteUserDepartmentMappingByUserId failed. UserId: {UserId}, DepartmentId: {DepartmentId}. Exception: {ex}");
                return false;
            }
        }

        public bool InsertUserDepartmentMapping(List<UserDepartmentMappingEntity> mappings)
        {
            try
            {
                var queries = new List<string>();
                UserEntity user = _userService.GetUserInfoSadmin();


                foreach (var item in mappings)
                {
                    string query = $@"
                                    INSERT INTO UserDepartmentMappings 
                                    (
                                        Id, UserId, DepartmentId, Role, IsHeadCount,
                                        Created, Modified, CreatedById, ModifiedById,
                                        CreatedBy, ModifiedBy, CreatedByFullName, ModifiedByFullName, AppService
                                    )
                                    VALUES (
                                        '{item.ID}',    
                                        '{item.UserId}', 
                                        '{item.DepartmentId}', 
                                        '4', 
                                        '{(item.IsHeadCount ? 1 : 0)}',
                                        GETDATE(), GETDATE(),
                                        '{user.ID}',
                                        '{user.ID}',
                                        '{user.LoginName}', '{user.LoginName}',
                                        '{user.FullName}', '{user.FullName}',
                                        ''
                                    )";

                                queries.Add(query);
                }

                bool result = true;
                foreach (var q in queries)
                {
                    try
                    {
                        bool success = this.ExecuteRunQuery(q);
                        if (!success)
                        {
                            Utilities.WriteLogError($"[ERROR] Insert query failed without exception:\n{q}");
                            result = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Utilities.WriteLogError($"[EXCEPTION] Failed to execute insert query:\n{q}\nException: {ex}");
                        result = false;
                    }
                }

                Utilities.WriteLogError($"[INFO] Insert completed. Total: {mappings.Count}, Success: {result}");
                return result;
            }
            catch (Exception ex)
            {
                Utilities.WriteLogError($"[ERROR] InsertUserDepartmentMapping failed. Exception: {ex}");
                return false;
            }
        }

    }
}
