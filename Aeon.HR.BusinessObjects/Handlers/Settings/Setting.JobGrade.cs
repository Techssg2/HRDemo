using Aeon.HR.Data.Models;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.BTA;
using Aeon.HR.ViewModels.DTOs;
using AutoMapper;
using Microsoft.Office.Interop.Word;
using Newtonsoft.Json;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.Caching;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.Data;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels.eDocIT;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Aeon.HR.BusinessObjects.Handlers
{
    public partial class SettingBO
    {
        string connStr = ConfigurationManager.ConnectionStrings["HRDbContext"].ConnectionString;
        string connFaciStr = ConfigurationManager.ConnectionStrings["FacilitiesDbContext"].ConnectionString;
        private string Inprocess_JobGrade = "Inprocess_JobGrade";
        public async Task<ResultDTO> GetJobGradeList(QueryArgs args)
        {
            var jobGradeVm = await _uow.GetRepository<JobGrade>()
                                         .FindByAsync<JobGradeViewModel>(args.Order, args.Page, args.Limit, args.Predicate, args.PredicateParameters);
            var count = await _uow.GetRepository<JobGrade>().CountAsync(args.Predicate, args.PredicateParameters);
            var result = new ArrayResultDTO { Data = jobGradeVm, Count = count };
            return new ResultDTO { Object = result };

        }
        public async Task<ResultDTO> UpdateJobGrade(JobGradeArgs args)
        {
            var result = new ResultDTO() { };
            var existJobGrade = await _uow.GetRepository<JobGrade>().FindByIdAsync(args.Id);
            if (existJobGrade is null)
            {
                return new ResultDTO { ErrorCodes = { 1001 }, Messages = { "Grade is not exist" } };
            }
            else
            {
                if (args.DepartmentType == 0)
                {
                    result.ErrorCodes = new List<int> { -1 };
                    result.Messages = new List<string> { "Department Type is require!" };
                    goto Finish;
                }

                if (args.StorePosition == 0)
                {
                    result.ErrorCodes = new List<int> { -1 };
                    result.Messages = new List<string> { "StorePosition is require!" };
                    goto Finish;
                }

                if (args.HQPosition == 0)
                {
                    result.ErrorCodes = new List<int> { -1 };
                    result.Messages = new List<string> { "HQPosition is require!" };
                    goto Finish;
                }

                var existsGradeTitle = await _uow.GetRepository<JobGrade>().FindByAsync(x => args.Id != x.Id && x.Title.ToLower().Trim().Equals(args.Title.ToLower().Trim()));
                if (existsGradeTitle.Any())
                {
                    result.ErrorCodes = new List<int> { -1 };
                    result.Messages = new List<string> { "Title already exists!" };
                    goto Finish;
                }

                var existsGradeCaption = await _uow.GetRepository<JobGrade>().FindByAsync(x => args.Id != x.Id && x.Grade == args.Grade);
                if (existsGradeCaption.Any())
                {
                    result.ErrorCodes = new List<int> { -1 };
                    result.Messages = new List<string> { "Jobgrade already exists!" };
                    goto Finish;
                }

                var validateExistTitle = await _uow.GetRepository<JobGrade>().FindByAsync(x => args.Id != x.Id && x.Title.Replace(" ","").ToLower().Trim().Equals(args.Title.Replace(" ", "").ToLower().Trim()));
                if (validateExistTitle.Any())
                {
                    result.ErrorCodes = new List<int> { -1 };
                    result.Messages = new List<string> { "Title already exists!" };
                    goto Finish;
                }

                Mapper.Map(args, existJobGrade);
                _uow.GetRepository<JobGrade>().Update(existJobGrade);
                await _uow.CommitAsync();
                result.Object = Mapper.Map<JobGradeViewModel>(existJobGrade);
            }
        Finish:
            return result;
        }
        public async Task<ResultDTO> DeleteJobGrade(Guid Id)
        {
            var JobGrade = await _uow.GetRepository<JobGrade>().FindByIdAsync(Id);
            if (JobGrade == null)
            {
                return new ResultDTO { ErrorCodes = { 111 }, Messages = { "Not found JobGrade with id " + Id }, };
            }
            else
            {
                _uow.GetRepository<JobGrade>().Delete(JobGrade);
                await _uow.CommitAsync();
            }
            return new ResultDTO { };
        }

        public async Task<ResultDTO> CreateJobGrade(JobGradeArgs args)
        {
            var result = new ResultDTO() { };
            ObjectCache cache = MemoryCache.Default;
            try
            {
                bool inprocessJG = cache.Get(Inprocess_JobGrade) as bool? ?? false;
                if (inprocessJG)
                {
                    result.ErrorCodes = new List<int> { -1 };
                    result.Messages = new List<string> { "Grade is being processed, please try again later!" };
                    goto Finish;
                }
                cache.Set(Inprocess_JobGrade, true, DateTime.Now.AddHours(1));

                try
                {
                    using (var facilityConnection = new SqlConnection(connFaciStr))
                    {
                        await facilityConnection.OpenAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Failed to connect to Facilities Database: {Message}", ex.Message);
                    result.ErrorCodes = new List<int> { -1 };
                    result.Messages = new List<string> { "Connection Facility failed!" };
                    cache.Set(Inprocess_JobGrade, false, DateTime.Now.AddHours(1));
                    goto Finish;
                }

                if (args.UpGrade == 0)
                {
                    result.ErrorCodes = new List<int> { -1 };
                    result.Messages = new List<string> { "Column UpGrade is invalid!" };
                    goto Finish;
                }

                var existsGradeTitle = await _uow.GetRepository<JobGrade>()
                    .FindByAsync(x => x.Title.ToLower().Trim().Equals(args.Title.ToLower().Trim()));
                if (existsGradeTitle.Any())
                {
                    result.ErrorCodes = new List<int> { -1 };
                    result.Messages = new List<string> { "Title already exists!" };
                    goto Finish;
                }

                var existsGradeCaption =
                    await _uow.GetRepository<JobGrade>().FindByAsync(x => x.Grade.Equals(args.Grade));
                if (existsGradeCaption.Any())
                {
                    result.ErrorCodes = new List<int> { -1 };
                    result.Messages = new List<string> { "Jobgrade already exists!" };
                    goto Finish;
                }

                var validateExistTitle = await _uow.GetRepository<JobGrade>().FindByAsync(x =>
                    x.Title.Replace(" ", "").ToLower().Trim().Equals(args.Title.Replace(" ", "").ToLower().Trim()));
                if (validateExistTitle.Any())
                {
                    result.ErrorCodes = new List<int> { -1 };
                    result.Messages = new List<string> { "Title already exists!" };
                    goto Finish;
                }

                #region HR

                #region Xử lý JobGrade

                args.Grade = (int)(args.UpGrade + 0.5);
                double upFromGrade = args.UpGrade;
                double upToGrade = args.Grade;

                var findAllGradeNeedUp = await _uow.GetRepository<JobGrade>().FindByAsync(x => x.Grade > upFromGrade);
                foreach (var item in findAllGradeNeedUp.OrderBy(x => x.Grade))
                {
                    item.Grade++;
                    _uow.GetRepository<JobGrade>().Update(item);
                }

                var nJobGrade = Mapper.Map<JobGrade>(args);
                _uow.GetRepository<JobGrade>().Add(nJobGrade);
                //await _uow.CommitAsync();
                result.Object = Mapper.Map<JobGradeViewModel>(nJobGrade);

                #endregion

                var allWorkflowInstanceList =
                    await _uow.GetRepository<WorkflowInstance>(true).FindByAsync(x => !x.IsCompleted);
                var allWorkflowTemplates = await _uow.GetRepository<WorkflowTemplate>(true).GetAllAsync();
                if (allWorkflowTemplates.Any())
                {
                    foreach (var item in allWorkflowTemplates)
                    {
                        // Lưu histories cho an toàn
                        item.OldWorkflowDataStr = item.WorkflowDataStr;
                        _uow.GetRepository<WorkflowTemplate>().Update(item);
                        // await _uow.CommitAsync();
                        bool isUpdateWf = false;

                        var currentItem = item;
                        if (currentItem.WorkflowData != null)
                        {
                            // Start conditions
                            if (currentItem.WorkflowData.StartWorkflowConditions.Any())
                            {
                                foreach (var condition in currentItem.WorkflowData.StartWorkflowConditions)
                                {
                                    if (!string.IsNullOrEmpty(condition.FieldName) &&
                                        Const.FieldNameGrades.Contains(condition.FieldName) &&
                                        !condition.FieldValues.Contains("Notused"))
                                    {
                                        List<string> fieldValues = new List<string>();

                                        var intList = condition.FieldValues.Select(x => int.Parse(x)).ToList();
                                        intList.OrderBy(x => x);

                                        #region Nâng Grade

                                        condition.FieldValues = condition.FieldValues
                                            .Select(gradeStr =>
                                            {
                                                int grade = int.Parse(gradeStr);
                                                if (grade > upFromGrade)
                                                {
                                                    grade++;
                                                }

                                                return grade.ToString();
                                            })
                                            .ToList();

                                        int minGrade = intList[0];

                                        #endregion

                                        #region Chèn thêm Grade

                                        if (minGrade < upFromGrade)
                                        {
                                            // Thêm grade mới vào đầu danh sách
                                            intList = condition.FieldValues.Select(x => int.Parse(x)).ToList();
                                            int existNextGrade = intList.FirstOrDefault(x => x == (upToGrade + 1));
                                            int existPrevGrade = intList.FirstOrDefault(x => x == (upToGrade - 1));
                                            if (existNextGrade != 0 && existPrevGrade != 0)
                                            {
                                                condition.FieldValues.Add(upToGrade.ToString());
                                                condition.FieldValues = condition.FieldValues.OrderBy(X => int.Parse(X))
                                                    .ToList();
                                            }
                                        }

                                        isUpdateWf = true;

                                        #endregion
                                    }
                                }
                            }

                            // Step conditions
                            if (currentItem?.WorkflowData?.Steps?.Any() == true)
                            {
                                foreach (var step in currentItem.WorkflowData.Steps)
                                {
                                    if (int.TryParse(step.JobGrade, out int jobGrade) && jobGrade > upFromGrade)
                                    {
                                        step.JobGrade = (jobGrade + 1).ToString();
                                        isUpdateWf = true;
                                    }

                                    if (int.TryParse(step.MaxJobGrade, out int maxJobGrade) &&
                                        maxJobGrade > upFromGrade)
                                    {
                                        step.MaxJobGrade = (maxJobGrade + 1).ToString();
                                        isUpdateWf = true;
                                    }

                                }
                            }
                        }

                        if (isUpdateWf)
                        {
                            item.WorkflowDataStr = JsonConvert.SerializeObject(currentItem.WorkflowData);
                            _uow.GetRepository<WorkflowTemplate>().Update(item);

                            // Đồng bộ các workflow đang chạy
                            var findAllWorkflowInstance =
                                allWorkflowInstanceList.Where(x => x.TemplateId == item.Id).ToList();
                            if (findAllWorkflowInstance.Any())
                            {
                                foreach (var currentInstance in findAllWorkflowInstance)
                                {
                                    currentInstance.OldWorkflowDataStr = currentInstance.WorkflowDataStr;
                                    currentInstance.WorkflowDataStr = item.WorkflowDataStr;
                                    _uow.GetRepository<WorkflowInstance>().Update(currentInstance);
                                }
                            }
                        }
                    }
                }

                #region Xử lý các phiếu

                /*
                    Acting => JobGradeName, JobGradeValue, CurrentJobGrade (Lưu Jobgrade hiện tại của user đó)
                    BTA => MaxGrade (Workflow)
                    BTA Detail => UserGradeValue (Jobgrade của user đó)
                    BOB => MaxGrade (Workflow)
                    BOB Detail => UserGradeValue (Jobgrade của user đó)
                    Hanover => JobGradeCaption (Hiển thị ở cột Grade)
                    Promote and Transfer => CurrentJobGrade (Hiển thị ở cột Current Job Grade)
                */
                List<string> ignoreStatus = new List<string> { "Cancelled", "Rejected", "Completed" };
                List<string> ignoreStatusForBTA = new List<string> { "Cancelled", "Rejected" };

                #region Acting

                //var allActings = await _uow.GetRepository<Acting>(true).FindByAsync(x => (x.JobGradeValue > upFromGrade || int.Parse(x.CurrentJobGrade.Replace("G", "")) > upFromGrade) && !ignoreStatus.Contains(x.Status));
                var allActings = await _uow.GetRepository<Acting>(true)
                    .FindByAsync(x => !ignoreStatus.Contains(x.Status));
                // Filter in memory to handle parsing and complex logic
                allActings = allActings.Where(x =>
                {
                    // Handle JobGradeValue
                    bool isJobGradeValid = x.JobGradeValue > upFromGrade;

                    // Handle CurrentJobGrade parsing
                    bool isCurrentJobGradeValid = false;
                    if (!string.IsNullOrEmpty(x.CurrentJobGrade))
                    {
                        if (int.TryParse(x.CurrentJobGrade.Replace("G", ""), out int currentGrade))
                        {
                            isCurrentJobGradeValid = currentGrade > upFromGrade;
                        }
                    }

                    return isJobGradeValid || isCurrentJobGradeValid;
                }).ToList();
                if (allActings.Any())
                {
                    foreach (var acting in allActings)
                    {
                        if (acting.JobGradeValue > upFromGrade)
                        {
                            acting.OldJobGradeValue = acting.JobGradeValue;
                            acting.JobGradeValue = (acting.JobGradeValue + 1);
                            _uow.GetRepository<Acting>().Update(acting);
                        }

                        if (int.Parse(acting.CurrentJobGrade.Replace("G", "")) > upFromGrade)
                        {
                            acting.OldCurrentJobGrade = acting.CurrentJobGrade;
                            acting.CurrentJobGrade = string.Format("G{0}",
                                (int.Parse(acting.CurrentJobGrade.Replace("G", "")) + 1));
                            _uow.GetRepository<Acting>().Update(acting);
                        }
                    }
                }

                #endregion

                #region BTA Header

                var allBTAs = await _uow.GetRepository<BusinessTripApplication>(true)
                    .FindByAsync(x => !ignoreStatusForBTA.Contains(x.Status));

                allBTAs = allBTAs.Where(x =>
                {
                    if (string.IsNullOrEmpty(x.MaxGrade))
                        return false;

                    return int.TryParse(x.MaxGrade.Replace("G", ""), out int maxGrade) && maxGrade > upFromGrade;
                }).ToList();

                var updatedBTAs = new List<BusinessTripApplication>();
                foreach (var btaHeader in allBTAs)
                {
                    if (int.Parse(btaHeader.MaxGrade.Replace("G", "")) > upFromGrade)
                    {
                        btaHeader.OldMaxGrade = btaHeader.MaxGrade;
                        btaHeader.MaxGrade =
                            string.Format("G{0}", (int.Parse(btaHeader.MaxGrade.Replace("G", "")) + 1));
                        updatedBTAs.Add(btaHeader);
                    }
                }

                if (updatedBTAs.Any())
                {
                    _uow.GetRepository<BusinessTripApplication>().Update(updatedBTAs);
                }

                #endregion

                #region BTA Details

                var allBTADetails = await _uow.GetRepository<BusinessTripApplicationDetail>(true)
                    .FindByAsync(x =>
                        x.UserGradeValue > upFromGrade && !ignoreStatus.Contains(x.BusinessTripApplication.Status));
                var updatedBTADetails = new List<BusinessTripApplicationDetail>();
                foreach (var btaDetail in allBTADetails)
                {
                    if (btaDetail.UserGradeValue > upFromGrade)
                    {
                        btaDetail.OldUserGradeValue = btaDetail.UserGradeValue;
                        btaDetail.UserGradeValue = btaDetail.UserGradeValue++;
                        updatedBTADetails.Add(btaDetail);
                    }
                }

                if (updatedBTADetails.Any())
                {
                    _uow.GetRepository<BusinessTripApplicationDetail>().Update(updatedBTADetails);
                }

                #endregion

                #region BOBHeader

                var allBOBs = await _uow.GetRepository<BusinessTripOverBudget>(true)
                    .FindByAsync(x => !ignoreStatus.Contains(x.Status));

                allBOBs = allBOBs.Where(x =>
                {
                    if (string.IsNullOrEmpty(x.MaxGrade))
                        return false;

                    return int.TryParse(x.MaxGrade.Replace("G", ""), out int maxGrade) && maxGrade > upFromGrade;
                }).ToList();
                var updatedBOB = new List<BusinessTripOverBudget>();
                foreach (var bobHeader in allBOBs)
                {
                    if (int.Parse(bobHeader.MaxGrade.Replace("G", "")) > upFromGrade)
                    {
                        bobHeader.MaxGrade = $"G{(int.Parse(bobHeader.MaxGrade.Replace("G", "")) + 1)}";
                        updatedBOB.Add(bobHeader);
                    }
                }

                if (updatedBOB.Any())
                {
                    _uow.GetRepository<BusinessTripOverBudget>().Update(updatedBOB);
                }

                #endregion

                #region BOBDetails

                var allBOBDetails = await _uow.GetRepository<BusinessTripOverBudgetsDetail>(true).FindByAsync(x =>
                    x.UserGradeValue > upFromGrade && !ignoreStatus.Contains(x.BusinessTripOverBudget.Status));
                var updatedBOBDetails = new List<BusinessTripOverBudgetsDetail>();
                foreach (var bobDetail in allBOBDetails)
                {
                    if (bobDetail.UserGradeValue > upFromGrade)
                    {
                        bobDetail.OldUserGradeValue = bobDetail.UserGradeValue;
                        bobDetail.UserGradeValue = bobDetail.UserGradeValue++;
                        updatedBOBDetails.Add(bobDetail);
                    }
                }

                if (updatedBOBDetails.Any())
                {
                    _uow.GetRepository<BusinessTripOverBudgetsDetail>().Update(updatedBOBDetails);
                }

                #endregion

                #region Hanover

                var allHanovers = await _uow.GetRepository<Handover>(true)
                    .FindByAsync(x => x.JobGradeCaption != null);
                allHanovers = allHanovers.Where(x =>
                {
                    if (string.IsNullOrEmpty(x.JobGradeCaption))
                        return false;

                    return int.TryParse(x.JobGradeCaption.Replace("G", ""), out int jobGrade) && jobGrade > upFromGrade;
                }).ToList();
                var updatedHanovers = new List<Handover>();
                foreach (var hanover in allHanovers)
                {
                    if (int.Parse(hanover.JobGradeCaption.Replace("G", "")) > upFromGrade)
                    {
                        hanover.OldJobGradeCaption = hanover.JobGradeCaption;
                        hanover.JobGradeCaption = string.Format("G{0}",
                            (int.Parse(hanover.JobGradeCaption.Replace("G", "")) + 1));
                        updatedHanovers.Add(hanover);
                    }
                }

                if (updatedHanovers.Any())
                {
                    _uow.GetRepository<Handover>().Update(updatedHanovers);
                }

                #endregion

                #region Promote and Transfer

                var allPromoteAndTransfers = await _uow.GetRepository<PromoteAndTransfer>(true)
                    .FindByAsync(x => x.CurrentJobGrade != null);
                allPromoteAndTransfers = allPromoteAndTransfers.Where(x =>
                {
                    if (string.IsNullOrEmpty(x.CurrentJobGrade))
                        return false;

                    return int.TryParse(x.CurrentJobGrade.Replace("G", ""), out int jobGrade) && jobGrade > upFromGrade;
                }).ToList();
                var updatedPromoteAndTransfers = new List<PromoteAndTransfer>();
                foreach (var promoteAndTransfer in allPromoteAndTransfers)
                {
                    if (int.Parse(promoteAndTransfer.CurrentJobGrade.Replace("G", "")) > upFromGrade)
                    {
                        promoteAndTransfer.OldCurrentJobGrade = promoteAndTransfer.CurrentJobGrade;
                        promoteAndTransfer.CurrentJobGrade = string.Format("G{0}",
                            (int.Parse(promoteAndTransfer.CurrentJobGrade.Replace("G", "")) + 1));
                        updatedPromoteAndTransfers.Add(promoteAndTransfer);
                    }
                }

                if (updatedPromoteAndTransfers.Any())
                {
                    _uow.GetRepository<PromoteAndTransfer>().Update(updatedPromoteAndTransfers);
                }

                #endregion

                #endregion

                #endregion

                //await _uow.CommitAsync();

                #region Workflow IT

                var itWorkflowTemplateList = new List<ITWorkflowTemplate>();
                var total = 0;
                var itWorkflowTemplates =
                    _uow.QuerySQL("select * from ITWorkflowTemplates", null);
                if (itWorkflowTemplates != null)
                {
                    total = itWorkflowTemplates.Tables[0].Rows.Count;
                    for (int i = 0; i < total; i++)
                    {
                        var item = new ITWorkflowTemplate();
                        if (item.ParseData(itWorkflowTemplates.Tables[0].Rows[i]))
                            if (item != null)
                                itWorkflowTemplateList.Add(item);
                    }
                }

                #region Get Workflow Instance

                var itWorkflowInstanceList = new List<ITWorkflowInstance>();
                total = 0;
                var itWorkflowInstances =
                    _uow.QuerySQL("select * from ITWorkflowInstances where IsCompleted = 0", null);
                if (itWorkflowInstances != null)
                {
                    total = itWorkflowInstances.Tables[0].Rows.Count;
                    for (int i = 0; i < total; i++)
                    {
                        var item = new ITWorkflowInstance();
                        if (item.ParseData(itWorkflowInstances.Tables[0].Rows[i]))
                            if (item != null)
                                itWorkflowInstanceList.Add(item);
                    }
                }

                #endregion

                /*string sqlInstanceOld = "UPDATE ITWorkflowInstances SET ITOldWorkflowDataStr = @ITOldWorkflowDataStr  WHERE Id = @Id";
                var sqlParametersOld = new Dictionary<string, object>
                {
                    { "@ITOldWorkflowDataStr", instances.ITWorkflowDataStr },
                    { "@Id", instances.Id }
                };*/
                var loopSQLInstanceOld = new List<Dictionary<string, object>>();
                var loopSQLInstance = new List<Dictionary<string, object>>();
                var loopSQL = new List<Dictionary<string, object>>();
                var loopSQLOld = new List<Dictionary<string, object>>();

                foreach (var currentTemplate in itWorkflowTemplateList)
                {
                    currentTemplate.WorkflowDataStr = currentTemplate.WorkflowDataStr
                        .Replace(@"\r\n", "")   // bỏ xuống dòng escape
                        .Replace(@"\""", @"""") // đổi \" thành "
                        .Replace(@"""[", "[")   // đổi "[ thành [
                        .Replace(@"]""", "]");  // đổi ]" thành ]
                    JObject obj = JObject.Parse(currentTemplate.WorkflowDataStr);

                    bool isUpdateStartConditions = false;
                    JArray startWorkflowConditions = (JArray)obj["StartWorkflowConditions"];
                    foreach (var condition in startWorkflowConditions)
                    {
                        if (condition == null) continue; // Bỏ qua nếu không phải JObject

                        string fieldName = condition["FieldName"]?.ToString();
                        JArray fieldValues = condition["FieldValues"] as JArray;

                        if (!string.IsNullOrEmpty(fieldName) &&
                            Const.FieldNameGradesIT.Contains(fieldName) &&
                            fieldValues != null && fieldValues.All(v => v.ToString() != "Notused"))
                        {
                            List<int> intList;
                            try
                            {
                                intList = fieldValues.Select(x => int.Parse(x.ToString())).ToList();
                            }
                            catch (FormatException)
                            {
                                // Xử lý trường hợp giá trị không parse được thành int
                                continue;
                            }

                            intList = intList.OrderBy(x => x).ToList();

                            #region Nâng Grade

                            condition["FieldValues"] = new JArray(
                                fieldValues.Select(gradeStr =>
                                {
                                    int grade = int.Parse(gradeStr.ToString());
                                    if (grade > upFromGrade)
                                    {
                                        grade++;
                                    }

                                    return grade.ToString();
                                })
                            );
                            if (!isUpdateStartConditions) isUpdateStartConditions = true;
                            int minGrade = intList.Any() ? intList[0] : 0;

                            #endregion

                            #region Chèn thêm Grade

                            if (minGrade != 0 && minGrade < upFromGrade)
                            {
                                // Cập nhật lại intList sau khi nâng grade
                                intList = condition["FieldValues"].Select(x => int.Parse(x.ToString())).ToList();
                                int existNextGrade = intList.FirstOrDefault(x => x == (upToGrade + 1));
                                int existPrevGrade = intList.FirstOrDefault(x => x == (upToGrade - 1));

                                if (existNextGrade != 0 && existPrevGrade != 0)
                                {
                                    fieldValues.Add(upToGrade.ToString());
                                    condition["FieldValues"] = new JArray(
                                        fieldValues.OrderBy(x => int.Parse(x.ToString()))
                                    );
                                }

                                if (!isUpdateStartConditions) isUpdateStartConditions = true;
                            }

                            #endregion
                        }
                    }

                    if (isUpdateStartConditions)
                    {
                        obj["StartWorkflowConditions"] =
                            startWorkflowConditions.ToString(Newtonsoft.Json.Formatting.Indented);
                    }

                    JArray steps = (JArray)obj["Steps"];
                    if (steps != null)
                    {
                        bool isUpdateConditions = false;
                        foreach (JObject step in steps)
                        {
                            var jobGradeToken = step["JobGrade"];
                            if (jobGradeToken != null && int.TryParse(jobGradeToken.ToString(), out int jobGrade) &&
                                jobGrade > upFromGrade)
                            {
                                step["JobGrade"] = (jobGrade + 1).ToString();
                                isUpdateConditions = true;
                            }

                            var maxJobGradeToken = step["MaxJobGrade"];
                            if (maxJobGradeToken != null &&
                                int.TryParse(maxJobGradeToken.ToString(), out int maxJobGrade) &&
                                maxJobGrade > upFromGrade)
                            {
                                step["MaxJobGrade"] = (maxJobGrade + 1).ToString();
                                isUpdateConditions = true;
                            }
                        }

                        if (isUpdateConditions)
                        {
                            obj["Steps"] = steps;
                        }
                    }

                    string updatedJson = JsonConvert.SerializeObject(obj);

                    // Sử dụng parameterized query để tránh SQL Injection
                    var parameters = new Dictionary<string, object>
                    {
                        { "@WorkflowDataStr", updatedJson },
                        { "@Id", currentTemplate.Id }
                    };
                    loopSQL.Add(parameters);
                    //_uow.ExecuteQuerySQL(sql, parameters);


                    var parameterOld = new Dictionary<string, object>
                    {
                        { "@OldWorkflowDataStr", currentTemplate.WorkflowDataStr },
                        { "@Id", currentTemplate.Id }
                    };
                    // _uow.ExecuteQuerySQL(sqlOld, parameterOld);
                    loopSQLOld.Add(parameterOld);

                    var wfInstanceList = itWorkflowInstanceList.Where(x => x.ITTemplateId == currentTemplate.Id)
                        .ToList();
                    foreach (var instances in wfInstanceList)
                    {
                        var sqlParameters = new Dictionary<string, object>
                        {
                            { "@ITWorkflowDataStr", updatedJson },
                            { "@ITOldWorkflowDataStr", instances.ITWorkflowDataStr },
                            { "@Id", instances.Id }
                        };
                        loopSQLInstance.Add(sqlParameters);
                        // _uow.ExecuteQuerySQL(sqlInstance, sqlParameters);

                        var sqlParametersOld = new Dictionary<string, object>
                        {
                            { "@ITOldWorkflowDataStr", instances.ITWorkflowDataStr },
                            { "@Id", instances.Id }
                        };

                        loopSQLInstanceOld.Add(sqlParametersOld);
                        // _uow.ExecuteQuerySQL(sqlInstanceOld, sqlParametersOld);
                    }
                }
                #endregion

                #region Workflow Facility

                var facilityWorkflowTemplateList = new List<DataRow>();
                var facilityTotal = 0;
                DataSet facilityWorkflowTemplates = null;

                using (var facilityConnection = new SqlConnection(connFaciStr))
                {
                    await facilityConnection.OpenAsync();
                    using (var command = new SqlCommand("select * from WorkflowTemplates", facilityConnection))
                    {
                        using (var adapter = new SqlDataAdapter(command))
                        {
                            facilityWorkflowTemplates = new DataSet();
                            adapter.Fill(facilityWorkflowTemplates);
                        }
                    }
                }

                if (facilityWorkflowTemplates != null && facilityWorkflowTemplates.Tables.Count > 0)
                {
                    facilityTotal = facilityWorkflowTemplates.Tables[0].Rows.Count;
                    for (int i = 0; i < facilityTotal; i++)
                    {
                        var row = facilityWorkflowTemplates.Tables[0].Rows[i];
                        facilityWorkflowTemplateList.Add(row);
                    }
                }

                #region Get Facility Workflow Instance

                var facilityWorkflowInstanceList = new List<DataRow>();
                facilityTotal = 0;
                DataSet facilityWorkflowInstances = null;

                using (var facilityConnection = new SqlConnection(connFaciStr))
                {
                    await facilityConnection.OpenAsync();
                    using (var command = new SqlCommand("select * from WorkflowInstances where IsCompleted = 0", facilityConnection))
                    {
                        using (var adapter = new SqlDataAdapter(command))
                        {
                            facilityWorkflowInstances = new DataSet();
                            adapter.Fill(facilityWorkflowInstances);
                        }
                    }
                }

                if (facilityWorkflowInstances != null && facilityWorkflowInstances.Tables.Count > 0)
                {
                    facilityTotal = facilityWorkflowInstances.Tables[0].Rows.Count;
                    for (int i = 0; i < facilityTotal; i++)
                    {
                        var row = facilityWorkflowInstances.Tables[0].Rows[i];
                        facilityWorkflowInstanceList.Add(row);
                    }
                }

                #endregion

                var facilityLoopSQLInstance = new List<Dictionary<string, object>>();
                var facilityLoopSQL = new List<Dictionary<string, object>>();

                foreach (var currentTemplate in facilityWorkflowTemplateList)
                {
                    string workflowDataStr = currentTemplate["WorkflowDataStr"].ToString();
                    Guid templateId = (Guid)currentTemplate["Id"];

                    JObject obj = JObject.Parse(workflowDataStr);

                    bool isUpdateStartConditions = false;
                    JArray startWorkflowConditions = (JArray)obj["StartWorkflowConditions"];

                    foreach (var condition in startWorkflowConditions)
                    {
                        if (condition == null) continue;

                        string fieldName = condition["FieldName"]?.ToString();
                        JArray fieldValues = condition["FieldValues"] as JArray;

                        if (!string.IsNullOrEmpty(fieldName) &&
                            Const.FieldNameGradesFacility.Contains(fieldName) &&
                            fieldValues != null && fieldValues.All(v => v.ToString() != "Notused"))
                        {
                            List<int> intList;
                            try
                            {
                                intList = fieldValues.Select(x => int.Parse(x.ToString())).ToList();
                            }
                            catch (FormatException)
                            {
                                continue;
                            }

                            intList = intList.OrderBy(x => x).ToList();

                            #region Nâng Grade

                            condition["FieldValues"] = new JArray(
                                fieldValues.Select(gradeStr =>
                                {
                                    int grade = int.Parse(gradeStr.ToString());
                                    if (grade > upFromGrade)
                                    {
                                        grade++;
                                    }

                                    return grade.ToString();
                                })
                            );
                            if (!isUpdateStartConditions) isUpdateStartConditions = true;
                            int minGrade = intList.Any() ? intList[0] : 0;

                            #endregion

                            #region Chèn thêm Grade

                            if (minGrade != 0 && minGrade < upFromGrade)
                            {
                                intList = condition["FieldValues"].Select(x => int.Parse(x.ToString())).ToList();
                                int existNextGrade = intList.FirstOrDefault(x => x == (upToGrade + 1));
                                int existPrevGrade = intList.FirstOrDefault(x => x == (upToGrade - 1));

                                if (existNextGrade != 0 && existPrevGrade != 0)
                                {
                                    fieldValues.Add(upToGrade.ToString());
                                    condition["FieldValues"] = new JArray(
                                        fieldValues.OrderBy(x => int.Parse(x.ToString()))
                                    );
                                }

                                if (!isUpdateStartConditions) isUpdateStartConditions = true;
                            }

                            #endregion
                        }
                    }

                    if (isUpdateStartConditions)
                    {
                        obj["StartWorkflowConditions"] =
                            startWorkflowConditions.ToString(Newtonsoft.Json.Formatting.Indented);
                    }

                    JArray steps = (JArray)obj["Steps"];
                    if (steps != null)
                    {
                        bool isUpdateConditions = false;
                        foreach (JObject step in steps)
                        {
                            var jobGradeToken = step["JobGrade"];
                            if (jobGradeToken != null && int.TryParse(jobGradeToken.ToString(), out int jobGrade) &&
                                jobGrade > upFromGrade)
                            {
                                step["JobGrade"] = (jobGrade + 1).ToString();
                                isUpdateConditions = true;
                            }

                            var maxJobGradeToken = step["MaxJobGrade"];
                            if (maxJobGradeToken != null &&
                                int.TryParse(maxJobGradeToken.ToString(), out int maxJobGrade) &&
                                maxJobGrade > upFromGrade)
                            {
                                step["MaxJobGrade"] = (maxJobGrade + 1).ToString();
                                isUpdateConditions = true;
                            }
                        }

                        if (isUpdateConditions)
                        {
                            obj["Steps"] = steps;
                        }
                    }

                    string updatedJson = JsonConvert.SerializeObject(obj);

                    var parameters = new Dictionary<string, object>
                    {
                        { "@WorkflowDataStr", updatedJson },
                        { "@Id", templateId }
                    };
                    facilityLoopSQL.Add(parameters);

                    var wfInstanceList = facilityWorkflowInstanceList.Where(x => (Guid)x["TemplateId"] == templateId).ToList();

                    foreach (var instances in wfInstanceList)
                    {
                        string instanceWorkflowDataStr = instances["WorkflowDataStr"].ToString();
                        Guid instanceId = (Guid)instances["Id"];

                        var sqlParameters = new Dictionary<string, object>
                        {
                            { "@WorkflowDataStr", updatedJson },
                            { "@Id", instanceId }
                        };
                        facilityLoopSQLInstance.Add(sqlParameters);
                    }
                }

                #endregion

                using (var con = new SqlConnection(connStr))                    //HR
                using (var facilityConnection = new SqlConnection(connFaciStr)) //Facility
                {
                    await con.OpenAsync();
                    await facilityConnection.OpenAsync();

                    using (var tran = con.BeginTransaction())
                    using (var facilityTran = facilityConnection.BeginTransaction())
                    {
                        try
                        {
                            #region HR
                            foreach (var item in loopSQL)
                            {
                                var sql =
                                    "UPDATE ITWorkflowTemplates SET WorkflowDataStr = @WorkflowDataStr WHERE Id = @Id";
                                _uow.ExecuteQuerySQL(con, tran, sql, item);
                            }

                            foreach (var item in loopSQLOld)
                            {
                                var sqlOld =
                                    "UPDATE ITWorkflowTemplates SET OldWorkflowDataStr = @OldWorkflowDataStr WHERE Id = @Id";
                                _uow.ExecuteQuerySQL(con, tran, sqlOld, item);
                            }

                            foreach (var item in loopSQLInstance)
                            {
                                var sqlInstance =
                                    "UPDATE ITWorkflowInstances SET ITWorkflowDataStr = @ITWorkflowDataStr WHERE Id = @Id";
                                _uow.ExecuteQuerySQL(con, tran, sqlInstance, item);
                            }

                            foreach (var item in loopSQLInstanceOld)
                            {
                                var sqlInstanceOld =
                                    "UPDATE ITWorkflowInstances SET ITOldWorkflowDataStr = @ITOldWorkflowDataStr  WHERE Id = @Id";
                                _uow.ExecuteQuerySQL(con, tran, sqlInstanceOld, item);
                            }
                            #endregion

                            #region Facility
                            foreach (var item in facilityLoopSQL)
                            {
                                var sql = "UPDATE WorkflowTemplates SET WorkflowDataStr = @WorkflowDataStr WHERE Id = @Id";
                                _uow.ExecuteQuerySQL(facilityConnection, facilityTran, sql, item);
                            }

                            foreach (var item in facilityLoopSQLInstance)
                            {
                                var sqlInstance = "UPDATE WorkflowInstances SET WorkflowDataStr = @WorkflowDataStr WHERE Id = @Id";
                                _uow.ExecuteQuerySQL(facilityConnection, facilityTran, sqlInstance, item);
                            }
                            #endregion

                            await _uow.CommitAsync();
                            tran.Commit();
                            facilityTran.Commit();
                            cache.Set(Inprocess_JobGrade, false, DateTime.Now.AddHours(1));
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            facilityTran.Rollback();
                            _logger.LogError("Error while updating IT Workflow and Facility Workflow: {Message}", ex.Message);
                            result.ErrorCodes = new List<int> { -1 };
                            result.Messages = new List<string>
                                { "An error occurred while updating IT Workflow and Facility Workflow. Please try again later." };
                            throw new Exception("An error occurred while updating IT Workflow and Facility Workflow. Please try again later.",
                                ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error while creating/updating JobGrade: {Message}", ex.Message);
                cache.Set(Inprocess_JobGrade, false, DateTime.Now.AddHours(1));
                result.ErrorCodes = new List<int> { -1 };
                result.Messages = new List<string>
                    { "An error occurred while processing your request. Please try again later." };
                throw new Exception("An error occurred while creating/updating JobGrade. Please try again later.", ex);
            }

        Finish:
            return result;
        }

        public async Task<ResultDTO> GetJobGradeById(Guid id)
        {
            var existJobGrade = await _uow.GetRepository<JobGrade>().FindByIdAsync<JobGradeViewModel>(id);
            return new ResultDTO { Object = existJobGrade };
        }

        public async Task<ResultDTO> AddOrUpdateItemsOfJobGrade(JobGradeForAddOrUpdateItemViewModel jobGradeItemViewModel)
        {
            var resultDto = new ResultDTO();
            var jobGradeItemRecruitmentMappingInDbs = await _uow.GetRepository<JobGradeItemRecruitmentMapping>().FindByAsync(i => i.JobGradeId == jobGradeItemViewModel.Id);

            // xóa hết các item cũ. thêm vào các item mới
            if (jobGradeItemRecruitmentMappingInDbs.Count() != 0)
            {
                _uow.GetRepository<JobGradeItemRecruitmentMapping>().Delete(jobGradeItemRecruitmentMappingInDbs);
            }

            // loại bỏ những dòng không chọn gì
            jobGradeItemViewModel.ItemListRecruitmentIds = jobGradeItemViewModel.ItemListRecruitmentIds.Where(id => id != Guid.Empty).ToList();

            // thêm mới các item
            foreach (var itemRecruitmentId in jobGradeItemViewModel.ItemListRecruitmentIds)
            {
                var newJobGradeItemRecruitmentMapping = new JobGradeItemRecruitmentMapping
                {
                    JobGradeId = jobGradeItemViewModel.Id,
                    ItemListRecruitmentId = itemRecruitmentId
                };
                _uow.GetRepository<JobGradeItemRecruitmentMapping>().Add(newJobGradeItemRecruitmentMapping);
            }

            await _uow.CommitAsync();

            return resultDto;
        }

        public async Task<ResultDTO> GetItemRecruitmentsOfJobGrade(Guid jobGradeId)
        {
            var itemRecruitmentsOfJobGrade = await _uow.GetRepository<JobGradeItemRecruitmentMapping>().FindByAsync<JobGradeItemRecruitmentMappingForAddOrUpdateItemViewModel>(i => i.JobGradeId == jobGradeId);
            return new ResultDTO { Object = new ArrayResultDTO { Count = itemRecruitmentsOfJobGrade.Count(), Data = itemRecruitmentsOfJobGrade } };
        }

        public async Task<ResultDTO> GetAllItemRecruitments()
        {
            var itemRecruitmentInDbs = await _uow.GetRepository<ItemListRecruitment>().GetAllAsync<ItemListRecruitmentForDropDownOfJobGradeViewModel>();
            return new ResultDTO { Object = new ArrayResultDTO { Count = itemRecruitmentInDbs.Count(), Data = itemRecruitmentInDbs } };
        }

        public async Task<ResultDTO> GetItemRecruitmentsByJobGradeId(Guid jobGradeId)
        {
            var itemRecruitmentsOfJobGradeId = await _uow.GetRepository<JobGradeItemRecruitmentMapping>().FindByAsync<JobGradeItemRecruitmentMappingInHandoverViewModel>(i => i.JobGradeId == jobGradeId);
            return new ResultDTO { Object = new ArrayResultDTO { Count = itemRecruitmentsOfJobGradeId.Count(), Data = itemRecruitmentsOfJobGradeId } };
        }

        //===== CR11.2 =====
        public async Task<ResultDTO> GetJobGradeByJobGradeValue(int jobGradeValue)
        {
            var itemJobGradeValue = await _uow.GetRepository<JobGrade>().GetSingleAsync<JobGradeViewModel>(i => i.Grade == jobGradeValue);
            return new ResultDTO { Object = new ArrayResultDTO { Count = 1, Data = itemJobGradeValue } };
        }
    }
}
