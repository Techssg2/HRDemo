using Aeon.HR.Data.Models;
using JobResignationV2.src.ModelEntity;
using JobResignationV2.src.Services;
using JobResignationV2.src.SQLExcute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobResignationV2.src
{
    public class ResignationJob
    {
        private readonly ResignationApplicationService _resignationService;
        private readonly UserService _userService;
        private readonly UserDepartmentMappingService _userDepartmentMappingService;
        // Constructor
        public ResignationJob()
        {
            _resignationService = new ResignationApplicationService();
            _userService = new UserService();
            _userDepartmentMappingService = new UserDepartmentMappingService();
        }

        public async Task Run()
        {
            var result = await InactiveUserOnResignationDate();
        }

        public async Task<bool> InactiveUserOnResignationDate()
        {
            Utilities.WriteLogError("[INFO] Start job ResignationJob ");
            var rangeToDate = DateTime.Now.AddDays(-13).Date;
            List<ResignationApplicationEntity> resignations = _resignationService.GetCompletedResignations(rangeToDate);

            if (!resignations.Any())
            {
                Utilities.WriteLogError("[INFO] No resignations found to process.");
                return true;
            }

            try
            {
                foreach (var item in resignations)
                {
                    try
                    {
                        if (item == null || string.IsNullOrEmpty(item.UserSAPCode))
                        {
                            Utilities.WriteLogError($"[WARN] Skipped null or missing UserSAPCode. Resignation ID: {item?.ID}");
                            continue;
                        }

                        var resignationEffectiveDate = item.OfficialResignationDate.AddDays(14).ToLocalTime().DateTime.Date;

                        // Chỉ xử lý nếu ngày hiện tại >= ngày nghỉ chính thức + 14
                        if (resignationEffectiveDate <= DateTime.Now.Date)
                        {
                            UserEntity currentUser = _userService.GetUserBySapCode(item.UserSAPCode);

                            if (currentUser == null)
                            {
                                Utilities.WriteLogError($"[ERROR] User not found for SAPCode: {item.UserSAPCode}");
                                continue;
                            }

                            if (currentUser.StartDate.HasValue && currentUser.StartDate.Value.Date < item.OfficialResignationDate.ToLocalTime().DateTime.Date)
                            {
                                bool execDelete = _userDepartmentMappingService.GetUserDepartmentMappingByUserId(currentUser.ID, currentUser.IsActivated);

                                Utilities.WriteLogError($"[INFO] Deactivated and unlinked user: {currentUser.ID} - {item.UserSAPCode}. Success: {execDelete}");
                            }
                          
                        }
                    }
                    catch (Exception exItem)
                    {
                        Utilities.WriteLogError($"[ERROR] Failed processing resignation for SAPCode: {item.UserSAPCode}. Exception: {exItem.Message}. {exItem.StackTrace}");
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.WriteLogError($"[FATAL] Error in InactiveUserOnResignationDate. Exception: {ex.Message}. {ex.StackTrace}");
                return false;
            }

            return true;
        }

    }
}
