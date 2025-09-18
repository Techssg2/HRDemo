using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.BusinessObjects.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using Aeon.HR.ViewModels.DTOs;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Handlers
{
    public class SharePointBO : ISharePointBO
    {
        private readonly ILogger _logger;
        public SharePointBO(ILogger logger)
        {
            _logger = logger;
        }
        public async Task<bool> AssignUser(string loginName)
        {
            bool returnValue = false;
            try
            {
                HttpResponseMessage response = await eDocHandlerHelper.CalleDocAPI($"action=AssignUser&loginName={loginName}");
                if (response != null && response.IsSuccessStatusCode && response.Content != null)
                {
                    string httpResponseResult = await response.Content.ReadAsStringAsync();
                    BaseHandlerResultDTO result = JsonConvert.DeserializeObject<BaseHandlerResultDTO>(httpResponseResult);
                    returnValue = result.success;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return returnValue;
        }

        public async Task<bool> RemoveUser(string loginName)
        {
            bool returnValue = false;
            try
            {
                HttpResponseMessage response = await eDocHandlerHelper.CalleDocAPI($"action=RemoveUser&loginName={loginName}");
                if (response != null && response.IsSuccessStatusCode && response.Content != null)
                {
                    string httpResponseResult = await response.Content.ReadAsStringAsync();
                    BaseHandlerResultDTO result = JsonConvert.DeserializeObject<BaseHandlerResultDTO>(httpResponseResult);
                    returnValue = result.success;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return returnValue;
        }
    }
}
