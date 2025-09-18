using Aeon.UpdateUserInfoFromSapUpdate.Utilities;
using UpdateUserInfoFromSapUpdate.src.SQLExcute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UpdateUserInfoFromSapJobV2.src.ModelEntity;
using Newtonsoft.Json;
using UpdateUserInfoFromSapJobV2.src.ViewModel;
using UpdateUserInfoFromSapJobV2.src.SAP;

namespace UpdateUserInfoFromSapUpdate.src
{
    public class UpdateUserInfoFromSap :  SQLQuery<UserEntity>
    {

        public async Task Run()
        {
            await ExcuteUpdateAsync();
        }
        public async Task<List<UserEntity>> ExcuteUpdateAsync()
        {
            int year = AppSettingsHelper.TargetYear;
            Utilities.WriteLogError("Start update user from SAP");
            List<UserEntity> r_list = new List<UserEntity>();
            string selectData = string.Format(@"select * from Users where IsDeleted = 0 and IsActivated = 1");

            r_list = this.GetItemsByQuery(selectData);
            DateTime now = DateTime.Now;
            int yearCustom = year;
            int commit = 0;
            int count = 0;
            foreach (var user in r_list)
            {
                try
                {
                    SAPBO sAPBO = new SAPBO();
                    var resultJoiningDate = await sAPBO.GetJoiningDateOfEmployee(user.SAPCode);
                    if (resultJoiningDate != null)
                    {
                        string updateJoinQuery = string.Format(@"Update Users set StartDate='{0}' where id='{1}'", resultJoiningDate, user.ID);
                        this.ExecuteRunQuery(updateJoinQuery);
                    }
                    Utilities.WriteLogError($"Update start date for SAP Code: {user.SAPCode} - {resultJoiningDate}");
                }
                catch (Exception ex)
                {
                    Utilities.WriteLogError($"Error at UpdateUserInfoFromSapJob.ExcuteUpdateAsync.UpdateUsers : {ex.Message}");
                }
            }
            string result1 = (DateTime.Now - now).ToString();
            now = DateTime.Now;
            List<string> sapCodesForGet = new List<string>();
            count = 0;
            foreach (var user in r_list)
            {
                try
                {
                    Utilities.WriteLogError($"Update Leave Balance for SAP Code: {user.SAPCode}");
                    count++;
                    var result = await GetMultipleLeaveBalanceSet(user.SAPCode, yearCustom);
                    await UpdateErdRemain(r_list, result, yearCustom);
                
                }
                catch (Exception ex)
                {
                    Utilities.WriteLogError($"Error at UpdateUserInfoFromSapJob.ExcuteUpdateAsync.UpdateLeave: {ex.Message}");
                }
            }
            string result2 = (DateTime.Now - now).ToString();
            return r_list;
        }

        public async Task<ResultDTO> GetMultipleLeaveBalanceSet(string sapCodes, int? yearCustom = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sapCodes))
                {
                    Utilities.WriteLogError("GetMultipleLeaveBalanceSet: No SAP code provided.");
                    return new ResultDTO
                    {
                        Object = null,
                        ErrorCodes = new List<int> { 1000 },
                        Messages = new List<string> { "No SAP code provided" }
                    };
                }

                string year = yearCustom?.ToString() ?? DateTimeOffset.Now.Year.ToString();
                string filter = $"$filter=Pernr eq '{sapCodes}' and Year eq '{year}'";

                SAPBO sAPBO = new SAPBO();
                ResultDTO res;

                try
                {
                    res = await sAPBO.GetLeaveBalanceSet(filter);
                }
                catch (Exception exCall)
                {
                    Utilities.WriteLogError($"GetMultipleLeaveBalanceSet: Error when calling GetLeaveBalanceSet - {exCall.Message}");
                    return new ResultDTO
                    {
                        Object = null,
                        ErrorCodes = new List<int> { 1001 },
                        Messages = new List<string> { "Lỗi khi lấy dữ liệu từ SAP", exCall.Message }
                    };
                }

                return new ResultDTO
                {
                    Object = res.Object,
                    ErrorCodes = res.ErrorCodes,
                    Messages = res.Messages
                };
            }
            catch (Exception ex)
            {
                Utilities.WriteLogError($"GetMultipleLeaveBalanceSet: Unexpected error - {ex.Message}");
                return new ResultDTO
                {
                    Object = null,
                    ErrorCodes = new List<int> { 9999 },
                    Messages = new List<string> { "Lỗi không xác định: GetMultipleLeaveBalanceSet: ", ex.Message }
                };
            }
        }


        public async Task UpdateErdRemain(List<UserEntity> userList, ResultDTO result, int year)
        {
            try
            {
                if ((result != null) && (result.Object != null))
                {
                    var listTemp = result.Object as List<LeaveBalanceResponceSAPViewModel>;
                    if (listTemp != null)
                    {
                        string[] types = { "11", "12", "13" };
                        var listResult = listTemp
                            .Where(x => types.Contains(x.AbsenceQuotaType))
                            .GroupBy(x => new { x.EmployeeCode, x.Year });

                        foreach (var item in listResult)
                        {
                            try
                            {
                                var user = userList.FirstOrDefault(x => x.SAPCode == item.Key.EmployeeCode && item.Key.Year == year.ToString());
                                if (user != null)
                                {
                                    double AlRemain = 0;
                                    double ErdRemain = 0;
                                    double DoflRemain = 0;

                                    QuotaDataJsonDTO quotaData = string.IsNullOrEmpty(user.QuotaDataJson)
                                        ? null
                                        : JsonConvert.DeserializeObject<QuotaDataJsonDTO>(user.QuotaDataJson);

                                    foreach (var rd in item)
                                    {
                                        switch (rd.AbsenceQuotaType)
                                        {
                                            case "11": AlRemain = rd.Remain; break;
                                            case "12": ErdRemain = rd.Remain; break;
                                            case "13": DoflRemain = rd.Remain; break;
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
                                    Utilities.WriteLogError($"[UpdateErdRemain] Updating SAPCode: {sapCode}, ALRemain: {AlRemain}, ERDRemain: {ErdRemain}, DOFLRemain: {DoflRemain}");

                                    string quota = JsonConvert.SerializeObject(quotaData);
                                    string updateJoinQuery = string.Format(
                                        @"Update Users set QuotaDataJson='{0}' where Id='{1}'",
                                        quota.Replace("'", "''"), // Escape single quote for SQL
                                        user.ID
                                    );

                                    this.ExecuteRunQuery(updateJoinQuery);
                                }
                                else
                                {
                                    Utilities.WriteLogError($"[UpdateErdRemain] User not found for SAPCode: {item.Key.EmployeeCode}, Year: {year}");
                                }
                            }
                            catch (Exception innerEx)
                            {
                                Utilities.WriteLogError($"[UpdateErdRemain] Error processing item for EmployeeCode: {item.Key.EmployeeCode}, Year: {item.Key.Year}. Message: {innerEx.Message}");
                            }
                        }
                    }
                    else
                    {
                        Utilities.WriteLogError("[UpdateErdRemain] Result.Object cannot be cast to List<LeaveBalanceResponceSAPViewModel>.");
                    }
                }
                else
                {
                    Utilities.WriteLogError("[UpdateErdRemain] Input result is null or result.Object is null.");
                }
            }
            catch (Exception ex)
            {
                Utilities.WriteLogError($"[UpdateErdRemain] Unexpected error: {ex.Message}");
            }
        }

    }
}
