using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.DTOs;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Jobs
{
    /// <summary>
    /// Đồng bộ data với Mass<br/>
    /// Chạy cron job
    /// </summary>
    public class MassSynchronizationJob
    {
        private readonly ILogger _logger;
        private readonly IUnitOfWork _uow;
        private readonly string aeonAccount = ConfigurationManager.AppSettings["massAccount"];
        private readonly string aeonPassword = ConfigurationManager.AppSettings["massPassword"];
        private readonly string baseUrl = ConfigurationManager.AppSettings["massUrl"];
        private string getLocationApiName = "External/GetLocations";       
        private string getAllJobCategoriesApiName = "External/GetAllJobCategories";
        public MassSynchronizationJob(ILogger logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _uow = unitOfWork;
        }
        public async Task<bool> Sync()
        {
            await SyncCategories();
            await SyncLocations();
            return true;
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
        private async Task<bool> SyncCategories()
        {
            try
            {
                var url = string.Format("{0}/{1}", baseUrl, getAllJobCategoriesApiName);
                var client = ConfigAPI(url, aeonAccount, aeonPassword);
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string httpResponseResult = await response.Content.ReadAsStringAsync();
                    var resultData = JsonConvert.DeserializeObject<ResultDTO>(httpResponseResult);
                    if (resultData.IsSuccess)
                    {
                        IList<RecruitmentCategoryViewModel> itemList = new List<RecruitmentCategoryViewModel>();
                        if (resultData.Object != null)
                        {
                            JArray jsonResponse = JArray.Parse(resultData.Object.ToString());

                            foreach (var item in jsonResponse)
                            {
                                JObject itemValue = (JObject)item;
                                var category = JsonConvert.DeserializeObject<RecruitmentCategoryViewModel>(itemValue.ToString());
                                if (category.ParentId == Guid.Empty)
                                {
                                    category.ParentId = null;
                                }
                                itemList.Add(category);
                            }
                            // ưu tiên những item ko có parent trước. tránh lỗi reference.
                            itemList = itemList.OrderBy(x => x.ParentId != null).ThenByDescending(x => x.ParentId).ToList();
                            foreach (RecruitmentCategoryViewModel item in itemList)
                            {
                                RecruitmentCategory existCategory = await _uow.GetRepository<RecruitmentCategory>()
                                    .FindByIdAsync(item.Id.Value);
                                if (existCategory != null)
                                {
                                    existCategory.Name = item.Name;
                                    existCategory.ParentId = item.ParentId;
                                    existCategory.Priority = item.Priority;
                                }
                                else
                                {
                                    RecruitmentCategory recruitmentCategory = Mapper.Map<RecruitmentCategory>(item);
                                    recruitmentCategory.Parent = null;
                                    _uow.GetRepository<RecruitmentCategory>().AddWithId(recruitmentCategory);
                                }
                                _uow.Commit();
                            }
                        }
                    }
                    else
                    {
                        _logger.LogError("Error at method: SyncCategories: Lỗi không lấy được data từ Mass API");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: SyncCategories: " + ex.Message);
                return false;
            }
        }
        private async Task<bool> SyncLocations()
        {
            try
            {
                var url = string.Format("{0}/{1}", baseUrl, getLocationApiName);
                var client = ConfigAPI(url, aeonAccount, aeonPassword);
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string httpResponseResult = await response.Content.ReadAsStringAsync();
                    var resultData = JsonConvert.DeserializeObject<ResultDTO>(httpResponseResult);
                    if (resultData.IsSuccess)
                    {
                        IList<MassLocationViewModel> itemList = new List<MassLocationViewModel>();
                        if (resultData.Object != null)
                        {
                            JArray jsonResponse = JArray.Parse(resultData.Object.ToString());

                            foreach (var item in jsonResponse)
                            {
                                JObject itemValue = (JObject)item;
                                var massLocation = JsonConvert.DeserializeObject<MassLocationViewModel>(itemValue.ToString());

                                itemList.Add(massLocation);
                            }
                            // ưu tiên những item ko có parent trước. tránh lỗi reference.
                            foreach (var item in itemList)
                            {
                                MasterData existMasterData = _uow.GetRepository<MasterData>()
                                    .FindBy(x => x.Code == item.Value).FirstOrDefault();
                                if (existMasterData != null)
                                {
                                    existMasterData.Name = item.Name;
                                }
                                else
                                {
                                    MasterData masterData = Mapper.Map<MasterData>(item);
                                    masterData.SourceFrom = Infrastructure.Enums.MasterDataFrom.External;
                                    _uow.GetRepository<MasterData>().AddWithId(masterData);
                                }
                                _uow.Commit();
                            }
                        }
                    }
                    else
                    {
                        _logger.LogError("Error at method: SyncLocations: Lỗi không lấy được data từ Mass API");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: SyncLocations:" + ex.Message);
                return false;
            }
        }
    }
}
