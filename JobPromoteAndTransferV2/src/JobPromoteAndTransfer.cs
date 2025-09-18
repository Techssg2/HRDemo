using JobPromoteAndTransfer.src.ModelEntity;
using JobPromoteAndTransfer.src.Services;
using JobPromoteAndTransferV2.src.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPromoteAndTransferV2.src
{
    public class JobPromoteAndTransfer
    {
        private readonly PromoteAndTransferService _promoteAndTransferService;
        private readonly UserDepartmentMappingService _userDepartmentMappingService;
        // Constructor
        public JobPromoteAndTransfer()
        {
            _promoteAndTransferService = new PromoteAndTransferService();
            _userDepartmentMappingService = new UserDepartmentMappingService();
        }

        public async Task Run()
        {
            var result = MoveUserDepartment();
        }

        public async Task MoveUserDepartment()
        {
            var newDate = new DateTimeOffset();
            Utilities.WriteLogError($"[INFO] Starting MoveUserDepartment job at {DateTime.Now}");

            var promoteAndTransfer = _promoteAndTransferService.GetAllPromoteAndTransfer();

            promoteAndTransfer = promoteAndTransfer
                .Where(x =>
                    x.EffectiveDate != newDate &&
                    (DateTime.Now.Date) == (x.EffectiveDate.Hour >= 17 ? x.EffectiveDate.ToLocalTime().Date : x.EffectiveDate.Date) &&
                    x.Status == "Completed")
                .ToList();

            Utilities.WriteLogError($"[INFO] Found {promoteAndTransfer.Count} promoteAndTransfer items to process");

            if (promoteAndTransfer.Any())
            {
                var userMappings = new List<UserDepartmentMappingEntity>();

                try
                {
                    foreach (var item in promoteAndTransfer)
                    {
                        Utilities.WriteLogError($"[INFO] Processing UserId: {item.UserId}, From Dept: {item.CurrentDepartmentId} -> To Dept: {item.NewDeptOrLineId}");

                        bool userDepartmentIsExist = _userDepartmentMappingService.GetUserDepartmentMappingByUserId(item.UserId, item.NewDeptOrLineId);

                        if (!userDepartmentIsExist)
                        {
                            var userMapping = new UserDepartmentMappingEntity
                            {
                                DepartmentId = item.NewDeptOrLineId,
                                UserId = item.UserId,
                                Role = Aeon.HR.Infrastructure.Enums.Group.Member,
                                IsHeadCount = true,
                                ID = Guid.NewGuid()
                            };

                            userMappings.Add(userMapping);
                            Utilities.WriteLogError($"[INFO] Prepared mapping insert for UserId: {item.UserId} to DeptId: {item.NewDeptOrLineId}");

                            // DELETE current mapping
                            var deleteResult = _userDepartmentMappingService.DeleteUserDepartmentMappingByUserId(item.UserId, item.CurrentDepartmentId);
                            Utilities.WriteLogError($"[INFO] Deleted old mapping UserId: {item.UserId}, DeptId: {item.CurrentDepartmentId}. Success: {deleteResult}");
                        }
                        else
                        {
                            Utilities.WriteLogError($"[INFO] Mapping already exists for UserId: {item.UserId}, DeptId: {item.NewDeptOrLineId}, skipping...");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Utilities.WriteLogError($"[ERROR] Exception in loop MoveUserDepartment: {ex}");
                }

                bool userDepartmentInsert = _userDepartmentMappingService.InsertUserDepartmentMapping(userMappings);
                Utilities.WriteLogError($"[INFO] Inserted {userMappings.Count} new mappings. Success: {userDepartmentInsert}");
            }
            else
            {
                Utilities.WriteLogError($"[INFO] No eligible promoteAndTransfer items found for today.");
            }

            Utilities.WriteLogError($"[INFO] MoveUserDepartment job completed at {DateTime.Now}");
        }

    }
}
