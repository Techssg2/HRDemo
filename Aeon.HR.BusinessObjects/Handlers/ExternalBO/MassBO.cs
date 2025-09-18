using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.Infrastructure.Utilities;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Handlers.ExternalBO
{
    public class MassBO : IMassBO
    {
        private readonly ILogger _logger;
        private readonly IUnitOfWork _uow;
        private readonly Guid locationMassTypeId = Guid.Parse("DFFA32EA-3113-4CAB-9F4B-2A4CE84D63A2");
        private string aeonAccount = ConfigurationManager.AppSettings["massAccount"];
        private string aeonPassword = ConfigurationManager.AppSettings["massPassword"];
        private string baseUrl = ConfigurationManager.AppSettings["massUrl"];
        private string pushEdocJobApiName = "External/PushEdocJob";
        private string changePositionStatus = "External/ChangePositionStatus";
        private string addOrUpdateJobCategoryApiName = "External/AddOrUpdateJobCategory";
        private string deleteJobCategoryApiName = "External/DeleteJobCategory";
        private string getLocationApiName = "External/GetLocations";
        public MassBO(ILogger logger, IUnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
        }

        protected HttpClient ConfigAPI(string url, string userName, string pass)
        {
            var credentials = new NetworkCredential(userName, pass);
            var handler = new HttpClientHandler { Credentials = credentials };
            var client = new HttpClient(handler);
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;

        }

        public async Task<ResultDTO> UpdateCategory(RecruitmentCategoryArgs data)
        {
            var result = new ResultDTO();
            var url = string.Format("{0}/{1}", baseUrl, addOrUpdateJobCategoryApiName);

            var client = ConfigAPI(url, aeonAccount, aeonPassword);
            try
            {
                var keyValueContent = data.ToKeyValue();
                var content = new FormUrlEncodedContent(keyValueContent);
                var response = await client.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    string httpResponseResult = await response.Content.ReadAsStringAsync();
                    var resultData = JsonConvert.DeserializeObject<ResultDTO>(httpResponseResult);
                    if (resultData != null)
                    {
                        result = resultData;
                    }
                }

            }
            catch (Exception ex)
            {
                result.ErrorCodes.Add(1004);
                result.Messages.Add("Something went wrong");
                result.Messages.Add(ex.Message);
                _logger.LogError("Error at method UpdateCategory " + ex.Message);
            }
            return result;
        }

        public async Task<ResultDTO> DeleteCategory(Guid id)
        {
            var url = string.Format("{0}/{1}?id={2}", baseUrl, deleteJobCategoryApiName, id);
            var result = new ResultDTO();
            var client = ConfigAPI(url, aeonAccount, aeonPassword);
            try
            {
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string httpResponseResult = await response.Content.ReadAsStringAsync();
                    var resultData = JsonConvert.DeserializeObject<ResultDTO>(httpResponseResult);
                    if (resultData != null)
                    {
                        result = resultData;
                    }
                }

            }
            catch (Exception ex)
            {
                result.ErrorCodes.Add(1004);
                result.Messages.Add("Something went wrong");
                result.Messages.Add(ex.Message);
                _logger.LogError("Error at method DeleteCategory " + ex.Message);
            }
            return result;
        }
        public async Task<ResultDTO> GetMassLocations()
        {
            var result = new ResultDTO();
            try
            {
                var url = string.Format("{0}/{1}", baseUrl, getLocationApiName);
                var client = ConfigAPI(url, aeonAccount, aeonPassword);
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string httpResponseResult = await response.Content.ReadAsStringAsync();
                    var resultData = JsonConvert.DeserializeObject<ResultDTO>(httpResponseResult);
                    if (resultData != null)
                    {
                        result = resultData;
                    }
                }
            }
            catch (Exception ex)
            {
                result.ErrorCodes.Add(1004);
                result.Messages.Add("Something went wrong");
                result.Messages.Add(ex.Message);
                _logger.LogError("Error at method GetMassLocations " + ex.Message);
            }

            if (result.Object == null || result.IsSuccess == false)
            {
                _logger.LogInformation("result.Object == null || result.IsSuccess == false: ");
                result = new ResultDTO();
                var localItemList = await _uow.GetRepository<MasterData>().FindByAsync<MasterDataViewModel>(x => x.MetaDataTypeId == locationMassTypeId, "Created");
                result.Object = localItemList;
            }
            return result;
        }

        public async Task<MassResponseAPIViewModel> GetMassLocationsPRD()
        {
            var resultData = new MassResponseAPIViewModel();
            try
            {
                var _baseUrl = "http://massrecruit.aeon.com.vn/admin/api";
                var _getLocationApiName = "External/GetLocations";
                /*var _aeonAccount = "aeon";
                var _aeonPassword = "*:3BavqZ";*/
                var aeonAccount = ConfigurationManager.AppSettings["massAccount"] == null ? "aeon" : ConfigurationManager.AppSettings["massAccount"];
                var aeonPassword = ConfigurationManager.AppSettings["massPassword"] == null ? "*:5Ba3qZ" : ConfigurationManager.AppSettings["massPassword"];
                var url = string.Format("{0}/{1}", _baseUrl, _getLocationApiName);
                var client = ConfigAPI(url, aeonAccount, aeonPassword);
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string httpResponseResult = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("httpResponseResult: " + httpResponseResult);
                    resultData = Mapper.Map<MassResponseAPIViewModel>(JsonConvert.DeserializeObject<MassResponseAPIViewModel>(httpResponseResult));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method GetMassLocations " + ex.Message);
            }
            return resultData;
        }

        public async Task<bool> PushPositionToMass(PositionMassViewModel data)
        {
            var url = string.Format("{0}/{1}", baseUrl, pushEdocJobApiName);
            var result = new ResultDTO();
            var client = ConfigAPI(url, aeonAccount, aeonPassword);
            try
            {
                var keyValueContent = data.ToKeyValue();
                var content = new FormUrlEncodedContent(keyValueContent);
                var response = await client.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    string httpResponseResult = await response.Content.ReadAsStringAsync();
                    var resultData = JsonConvert.DeserializeObject<ResultDTO>(httpResponseResult);
                    if (resultData != null)
                    {
                        result = resultData;
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method PushPositionToMass " + ex.Message);
                return false;
            }
            return true;
        }
        public async Task<bool> ChangeStatusPositionToMass(PositionChangingStatus data)
        {
            var url = string.Format("{0}/{1}", baseUrl, changePositionStatus);
            var result = new ResultDTO();
            var client = ConfigAPI(url, aeonAccount, aeonPassword);
            try
            {
                var keyValueContent = data.ToKeyValue();
                var content = new FormUrlEncodedContent(keyValueContent);
                var response = await client.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    string httpResponseResult = await response.Content.ReadAsStringAsync();
                    var resultData = JsonConvert.DeserializeObject<ResultDTO>(httpResponseResult);
                    if (resultData != null)
                    {
                        result = resultData;
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method ChangePositionStatus " + ex.Message);
                return false;
            }
            return true;
        }

        public async Task<ResultDTO> GetMassPositions(QueryArgs queryArg)
        {
            var result = new ResultDTO();
            var aeonAccount = ConfigurationManager.AppSettings["massAccount"];
            var aeonPassword = ConfigurationManager.AppSettings["massPassword"];
            var baseUrl = ConfigurationManager.AppSettings["massUrl"];
            var url = string.Format("{0}/{1}", baseUrl, "Data/QueryData?entity=JobViewModel");
            var client = ConfigAPI(url, aeonAccount, aeonPassword);

            var payload = JsonConvert.SerializeObject(queryArg);
            payload = ReplaceNull(payload);
            var content = Utilities.StringContentObjectFromJson(payload);
            try
            {
                var response = await client.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    string httpResponseResult = await response.Content.ReadAsStringAsync();
                    var resultArray = JsonConvert.DeserializeObject<ItemsFromMassViewModel>(httpResponseResult);
                    if (resultArray != null)
                    {
                        result.Object = new ArrayResultDTO { Data = resultArray, Count = resultArray.Items.Count };
                    }
                }

            }
            catch (Exception ex)
            {
                result.ErrorCodes.Add(1004);
                result.Messages.Add("Something went wrong");
                result.Messages.Add(ex.Message);
            }

            return result;
        }
        private string ReplaceNull(string iText)
        {
            var result = iText.Replace("null", "\"\"");
            return result;
        }

    }

}
