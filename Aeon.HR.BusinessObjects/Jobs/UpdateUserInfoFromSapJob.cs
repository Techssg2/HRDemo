using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.DTOs;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Jobs
{
    public class UpdateUserInfoFromSapJob
    {
        private readonly ILogger _logger;
        private readonly IUnitOfWork _uow;
        private readonly IEmployeeBO _employee;
        private readonly ISSGExBO _bo;
        private int dayRemindManagerActing { get; set; }
        public UpdateUserInfoFromSapJob(ILogger logger, IUnitOfWork uow, IEmployeeBO employee, ISSGExBO bo)
        {
            _logger = logger;
            _uow = uow;
            _employee = employee;
            _bo = bo;
        }
        public async Task<bool> DoWork(int year)
        {
            _logger.LogInformation("Start update user from SAP");
            //var testSAPCodes = new List<string> { "00405815", "00310578", "00263379", "00307760", "00262949", "00280872", "00405877", "00310973" };
            var allUsers = await _uow.GetRepository<User>().FindByAsync(x => x.IsActivated == true && x.IsDeleted == false);
            //var userList = allUsers.Where(x => testSAPCodes.Contains(x.SAPCode));
            //int threadSleep = (userList.Count() > 0) ? (3600 * 1000) / userList.Count() : 1000;
            //var allUsers = await _uow.GetRepository<User>().FindByAsync(x => x.SAPCode == "00405815" && x.IsActivated == true && x.IsDeleted == false);
            _logger.LogInformation($"Users count: {allUsers.Count()}");
            DateTime now = DateTime.Now;
            //int yearCustom = now.Month == 12 && now.Day >= 10 ? now.AddMonths(1).Year : now.Year;
            int yearCustom = year;
            int commit = 0;
            int count = 0;
            foreach (var user in allUsers)
            {
                try
                {
                    _uow.RefreshContext(user);
                     var resultJoiningDate = await _employee.GetJoiningDateOfEmployee(user.SAPCode);
                    if (resultJoiningDate != null)
                    {
                        user.StartDate = resultJoiningDate;
                        commit++;
                    }
                    _logger.LogInformation($"Update start date for SAP Code: {user.SAPCode} - {resultJoiningDate}");
                    count++;
                    _uow.GetRepository<User>().Update(user);
                    if (commit > 10)// save 10 row 1 lần
                    {
                        await _uow.CommitAsync();
                        commit = 0;
                    }
                    //sleep để đừng push api quá nhiều
                    //Thread.Sleep(threadSleep / 2);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error at UpdateUserInfoFromSapJob.DoWork.GetFullEmployeeInfo: {ex.Message}");
                }
            }
            if (commit > 0)
            {
                await _uow.CommitAsync();
                commit = 0;
            }
            string result1 = (DateTime.Now - now).ToString();
            now = DateTime.Now;
            List<string> sapCodesForGet = new List<string>();
            count = 0;
            foreach (var user in allUsers)
            {
                try
                {
                    sapCodesForGet.Add(user.SAPCode);
                    _logger.LogInformation($"Update Leave Balance for SAP Code: {user.SAPCode}");
                    count++;
                    if (sapCodesForGet.Count() >= 10)
                    {
                        var result = await _bo.GetMultipleLeaveBalanceSet(sapCodesForGet, yearCustom);
                        await UpdateErdRemain(allUsers, result, yearCustom);
                        sapCodesForGet.Clear();
                    }
                    //sleep để đừng push api quá nhiều
                    //Thread.Sleep(threadSleep / 2);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error at UpdateUserInfoFromSapJob.DoWork.GetFullEmployeeInfo: {ex.Message}");
                }
            }
            if (sapCodesForGet.Count() > 0)
            {
                var result = await _bo.GetMultipleLeaveBalanceSet(sapCodesForGet, yearCustom);
                await UpdateErdRemain(allUsers, result, yearCustom);
                sapCodesForGet.Clear();
            }
            string result2 = (DateTime.Now - now).ToString();
            
            return true;
        }
        public async Task UpdateErdRemain(IEnumerable<User> userList, ResultDTO result, int year)
        {

            if ((result != null) && (result.Object != null))
            {
                var listTemp = (List<LeaveBalanceResponceSAPViewModel>)(result.Object);
                if (listTemp != null)
                {
                    string[] types = { "11", "12", "13" };
                    bool commit = false;
                    var listResult = listTemp.Where(x => types.Contains(x.AbsenceQuotaType)).GroupBy(x=> new { x.EmployeeCode, x.Year });
                    foreach (var item in listResult)
                    {
                        //compare remain with sapcode + year remain
                        var user = userList.FirstOrDefault(x => x.SAPCode == item.Key.EmployeeCode && item.Key.Year == year.ToString());
                        if (user != null)
                        {
                            _uow.RefreshContext(user);
                            double AlRemain = 0;
                            double ErdRemain = 0;
                            double DoflRemain = 0;
                            QuotaDataJsonDTO quotaData = string.IsNullOrEmpty(user.QuotaDataJson) ? null : JsonConvert.DeserializeObject<QuotaDataJsonDTO>(user.QuotaDataJson);
                            foreach (var rd in item.ToList())
                            {                                
                                switch (rd.AbsenceQuotaType)
                                {
                                    case "11":
                                        AlRemain = rd.Remain;
                                        break;
                                    case "12":
                                        ErdRemain = rd.Remain;
                                        break;
                                    case "13":
                                        DoflRemain = rd.Remain;
                                        break;
                                }
                            }
                            if (quotaData == null)
                            {
                                quotaData = new QuotaDataJsonDTO();
                                quotaData.JsonData.Add(new QuotaDataJsonDetailDTO
                                {
                                    Year = year,
                                    ALRemain = AlRemain,
                                    DOFLRemain = DoflRemain,
                                    ERDRemain = ErdRemain
                                });
                            }
                            else
                            {
                                var currentQuotaData = quotaData.JsonData.FirstOrDefault(x => x.Year == year);
                                if (currentQuotaData != null)
                                {
                                    currentQuotaData.ALRemain = AlRemain;
                                    currentQuotaData.DOFLRemain = DoflRemain;
                                    currentQuotaData.ERDRemain = ErdRemain;
                                }
                                else
                                {
                                    quotaData.JsonData.Add(new QuotaDataJsonDetailDTO
                                    {
                                        Year = year,
                                        ALRemain = AlRemain,
                                        DOFLRemain = DoflRemain,
                                        ERDRemain = ErdRemain
                                    });
                                }
                            }
                            var sapCode = !string.IsNullOrEmpty(user.SAPCode) ? user.SAPCode : "";
                            _logger.LogInformation($"SAPCode: {sapCode}, ALRemain: {AlRemain}, ERDRemain: {ErdRemain}, DOFLRemain: {DoflRemain}");
                            user.QuotaDataJson = JsonConvert.SerializeObject(quotaData);
                            _uow.GetRepository<User>().Update(user);          
                        }
                    }
                    await _uow.CommitAsync();
                }
            }
        }
    }
}
