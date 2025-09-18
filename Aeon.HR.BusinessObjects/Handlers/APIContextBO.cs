using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.BusinessObjects.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Aeon.HR.BusinessObjects.Handlers
{
    public class APIContextBO : IAPIContext
    {
        private const string SECRET = "secret";
        private const string TOKEN_EDOC2 = "token_edoc2";
        private const string USER_KEY = "uxr";
        public string CurrentUser
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    var items = HttpContext.Current.Request.Headers.GetValues(USER_KEY);
                    if (items != null)
                    {
                        var encryptedItem = items.FirstOrDefault();
                        string encry = StringCipher.Decrypt(encryptedItem, ConfigurationManager.AppSettings[SECRET]);
                        // fix code account hien.le
                        if (encry != null && !encry.Equals(""))
                        {
                            if (encry.Equals("hie"))
                            {
                                encry = "hien.le";
                            } else if (encry.Equals("canh.nguyenv"))
                            {
                                encry = "canh.nguyenvan";
                            } else if (encry.Equals("ngoclinh.nguyent"))
                            {
                                encry = "ngoclinh.nguyentrinh";
                            }
                            else if (encry.Equals("daiso"))
                            {
                                encry = "huong.du";
                            } else if (encry.Equals("thạo.nguyen"))
                            {
                                encry = "thao.nguyen";
                            } else if (encry.Equals("aono.keizo"))
                            {
                                encry = "keizo.aono";
                            } 
                            /*else if (encry.Equals("thuy.t.tran"))
                            {
                                encry = "thuthuy.tran";
                            }*/
                            else if (encry.Equals("thuyhongnhung.nguy"))
                            {
                                encry = "thuyhongnhung.nguyen";
                            }
                            else if (encry.Equals("vy.nguyentranhoa"))
                            {
                                encry = "vy.nguyentranhoai";
                            }
                            else if (encry.Equals("hiep"))
                            {
                                encry = "hiep.nguyen";
                            }
                        }
                        return encry;
                    }
                }
                return string.Empty;
            }
        }

        public bool ValidateContext(HttpRequestHeaders headers, string uri)
        {
            var _isValid = false;
            IEnumerable<string> items;
            headers.TryGetValues(SECRET, out items);
            var secret = ConfigurationManager.AppSettings[SECRET];
            //lamnl add api RE 
            bool inact = new string[] { "/api/APIEdoc2/GetItemByReferenceNumber",
                "/api/APIEdoc2/UpdateStatusBTA",
                "/api/APIEdoc2/GetAllUser_API",
                "/api/APIEdoc2/GetAllUser_APIV2",
                "/api/APIEdoc2/GetSpecificShiftPlan_API",
                "/api/APIEdoc2/GetActualShiftPlan_API",
                "/api/APIEdoc2/GetUsersForTargetPlanByDeptIdDWS",
                "/api/APIEdoc2/AccountVerification_API",
                "/api/APIEdoc2/GetDepartmentTree_API",
                "/api/APIEdoc2/GetAllBTA_API",
                "/api/SSGEx/CreatedPayload", 
                "/api/SSGEx/TestJob",
                "/api/SSGEx/ResetMultiplePassword",
                "/api/SSGEx/RemoveLogRequestToHireImportTracking",
                "/api/SSGEx/JobManualMoveUserPromoteAndTransfer",
                "/api/RequestToHire/AutoImportRequestToHire",
                "/api/User/ChangePassword",
                "/api/User/GetLinkImageUserByLoginName",
                "/api/SSGEx/GetTodoList",
                "/api/MasterData/GetMasterDataForOtherModules",
                "/api/SSGEx/ConvertCommitBTA",
                // DWS
                "/api/APIEdoc2/CreateTargetPlan_API",
                "/api/APIEdoc2/ValidateForEditDWS_API",
                "/api/APIEdoc2/GetActualShiftPlanForDWS_API",

                "/api/SSGEx/ConvertCommitBTA"
            }.Any(s => uri.Contains(s));
            bool consoleWindowAPI = new string[] {
                "/api/ServiceAPI/ClearCacheDepartment",
                "/api/ServiceAPI/UpdateUserInfoFromSapJob",
                "/api/ServiceAPI/JobEmailDailyApproverEdoc1Edoc2",
                "/api/ServiceAPI/JobResignation",
                "/api/ServiceAPI/JobPromoteAndTransfer",
                "/api/ServiceAPI/JobRemindActingNotification",
                "/api/ServiceAPI/JobCancelOutOfPeriodTasks",
                "/api/ServiceAPI/JobRemindResponseInvitation",
                "/api/ServiceAPI/JobMassSynchronization",
                "/api/ServiceAPI/JobSendMail1STResignations",
                "/api/ServiceAPI/JobAutoRetryPayloadSAP",
                "/api/ServiceAPI/JobTargetPeriod",
                "/api/ServiceAPI/CancelOutOfPeriodTasksJob",
                "/api/ServiceAPI/RemindResponseInvitationJob",
                "/api/ServiceAPI/JobEmailDailyApproverEdoc1Edoc2"
            }.Any(s => uri.Contains(s));
            if (inact)
            {
                var token = ConfigurationManager.AppSettings[TOKEN_EDOC2];
                if (token.Any())
                {
                    if (headers.Authorization != null && headers.Authorization.Parameter.Contains(token))
                    {
                        _isValid = true;
                    }
                }
                else
                {
                    _isValid = false;
                }
            }
            else if (consoleWindowAPI)
            {
                var token = ConfigurationManager.AppSettings[TOKEN_EDOC2];
                if (token.Any() && headers.Authorization != null && headers.Authorization.Parameter.Contains(token))
                    _isValid = true;
                else
                    _isValid = false;
            }
            else
            {
                if (items == null || !items.Contains(secret))
                {
                    _isValid = false;
                }
                else
                {
                    IEnumerable<string> _userItems;
                    headers.TryGetValues("uxr", out _userItems);
                    if (_userItems.Count() > 0)
                    {
                        _isValid = true;
                    }
                }
            }
            return _isValid;
        }
    }
}
