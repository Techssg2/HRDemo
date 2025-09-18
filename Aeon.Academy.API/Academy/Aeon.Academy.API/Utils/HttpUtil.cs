using Aeon.Academy.Common.Configuration;
using Aeon.Academy.Common.Consts;
using Aeon.Academy.Common.Utils;
using Aeon.Academy.Services;
using Aeon.HR.Infrastructure.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web;

namespace Aeon.Academy.API.Utils
{
    public static class HttpUtil
    {
        private static readonly ILogger _logger = ServiceLocator.Resolve<ILogger>();
        public static bool TryGetLoginName(this HttpRequestHeaders headers, out string loginName)
        {
            loginName = string.Empty;

            if (headers.TryGetValues(CommonKeys.TOKEN, out IEnumerable<string> values))
            {
                var token = values.FirstOrDefault();
                _logger.LogInfo("token - " + token);
                loginName = StringCipher.Decrypt(token, ApplicationSettings.ApiSecret);
                if (!string.IsNullOrEmpty(loginName) && loginName.Equals("daiso")) return true;
                _logger.LogInfo("loginName - " + loginName);

                if (!string.IsNullOrEmpty(loginName)) return true;
            }
            return false;

        }
        public static string ConvertStatus(string status, string assigned, string assignedUserId)
        {
            if (status == WorkflowStatus.Pending && !string.IsNullOrEmpty(assigned))
            {
                return $"Waiting for {assigned} Approval";
            }
            if (status == WorkflowStatus.Completed && !string.IsNullOrEmpty(assignedUserId))
            {
                return "Waiting for Create Training Invitation";
            }
            return status;
        }
    }
}