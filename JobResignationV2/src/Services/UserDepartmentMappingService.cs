using JobResignationV2.src.ModelEntity;
using JobResignationV2.src.SQLExcute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobResignationV2.src.Services
{
    internal class UserDepartmentMappingService : SQLQuery<UserDepartmentMappingEntity>
    {
        private readonly UserService _userService;
        public UserDepartmentMappingService() {
            _userService = new UserService();
        }

        public bool GetUserDepartmentMappingByUserId(Guid currentId, bool isActived)
        {
            try
            {
                string query = $@"DELETE FROM UserDepartmentMappings WHERE UserId = '{currentId}'";
                bool mappingResult = this.ExecuteRunQuery(query);

                Utilities.WriteLogError($"[INFO] Deleted UserDepartmentMappings for UserId: {currentId}. Success: {mappingResult}");

                if (isActived)
                {
                    bool execUpdate = _userService.UpdateInActivated(currentId);

                    Utilities.WriteLogError($"[INFO] Updated user IsActivated = 0 for UserId: {currentId}. Success: {execUpdate}");

                    return execUpdate;
                }

                return mappingResult;
            }
            catch (Exception ex)
            {
                Utilities.WriteLogError($"[ERROR] GetUserDepartmentMappingByUserId failed. UserId: {currentId}. Exception: {ex.Message}");
                return false;
            }
        }




    }
}
