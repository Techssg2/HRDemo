using Aeon.HR.BusinessObjects.Attributes;
using Aeon.HR.BusinessObjects.Handlers;
using Aeon.HR.BusinessObjects.Handlers.ExternalBO;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Re = System.Text.RegularExpressions;
using Aeon.HR.BusinessObjects.Helpers;
using Newtonsoft.Json;

namespace Aeon.HR.BusinessObjects.CompleteActions
{
    [Action(Type = typeof(RequestToHire))]
    public class RequestToHireCompleteAction : ICompleteAction
    {
        public async Task Execute(IUnitOfWork uow, Guid itemId, IDashboardBO dashboardBO, IWorkflowBO workflowBO, ILogger logger)
        {
            var item = await uow.GetRepository<RequestToHire>().FindByIdAsync(itemId);
            var deptDivisionid = Guid.Empty;
            var assignedToDept = await uow.GetRepository<Department>().GetSingleAsync(x => x.UserDepartmentMappings.Any(u => u.UserId == item.AssignToId && u.IsHeadCount && u.Role == Group.Member));
            var businessModelUnitMapping = await uow.GetRepository<BusinessModelUnitMapping>().FindByAsync(x => !x.IsDeleted);
            var latestPosition = new Position();
            if (item.ReplacementFor == TypeOfNeed.NewPosition)
            {
                /*var jobGrade = await uow.GetRepository<JobGrade>().GetSingleAsync(x => x.Caption == item.JobGradeCaption);*/
                var jobGrade = await uow.GetRepository<JobGrade>().GetSingleAsync(x => item.JobGradeId.HasValue && x.Id == item.JobGradeId.Value);
                if (jobGrade is null)
                {
                    jobGrade = await uow.GetRepository<JobGrade>().GetSingleAsync(x => item.JobGradeGrade.HasValue && x.Grade == item.JobGradeGrade.Value);
                }
                var dept = await uow.GetRepository<Department>().GetSingleAsync(x => x.Id == item.DeptDivisionId);
                var deptName = dept.Name;
                var r = new Re.Regex(@"\(+.+\)");
                var match = r.Match(deptName);
                if (match.Success)
                {
                    deptName = deptName.Replace(match.Value, string.Empty).Trim();
                }
                var countDept = await uow.GetRepository<Department>(true).CountAllAsync();
                //Fix ticket #322
                int latestDepCode = uow.GetLatestDepartmentCode(jobGrade.Grade);

                List<string> SAPCodeOfDepartments = new List<string>();
                if (!string.IsNullOrEmpty(item.ListDepartmentSAPCode))
                {
                    SAPCodeOfDepartments = JsonConvert.DeserializeObject<List<string>>(item.ListDepartmentSAPCode);
                }
                for (var i = 0; i < item.Quantity; i++)
                {
                    string name = $"{deptName} ({jobGrade.Caption})";

                    bool isStore = item.Operation == OperationOptions.Store ? true : (item.Operation == OperationOptions.HQ ? false : dept.IsStore);
                   
                    var newDept = new Department()
                    {
                        Name = GenarateDeptNameByContractType(name, item.ContractTypeCode),
                        PositionCode = item.PositionCode,
                        PositionName = item.PositionName,
                        Code = $"DEP{jobGrade.Grade}{String.Format("{0:D6}", (latestDepCode + i + 1))}",
                        //Type = jobGrade.Grade < 5 ? DepartmentType.Division : DepartmentType.Department,
                        Type = jobGrade.DepartmentType,
                        Color = dept.Color,
                        JobGradeId = jobGrade.Id,
                        ParentId = item.DeptDivisionId,
                        IsStore = isStore,
                        IsHR = dept.IsHR,
                        RequestToHireId = item.Id,
                        RTHReferenceNumber = item.ReferenceNumber
                    };
                    this.setBusinessModel(newDept, businessModelUnitMapping, item.LocationCode);

                    if (!string.IsNullOrEmpty(item.DepartmentName))
                    {
                        newDept.Name = item.DepartmentName;
                        newDept.EnableForPromoteActing = true;
                    }
                    if (SAPCodeOfDepartments.Any())
                    {
                        newDept.SAPCode = SAPCodeOfDepartments[i];
                    }
                    uow.GetRepository<Department>().Add(newDept);
                    deptDivisionid = newDept.Id;
                    //Fix bug 329
                    //Create request to hire - new position
                    var pos = new Position()
                    {
                        AssignToId = item.AssignToId,
                        DeptDivisionId = deptDivisionid,
                        RequestToHireId = item.Id,
                        RequestToHireNumber = item.ReferenceNumber,
                        ExpiredDay = item.ExpiredDayPosition,
                        ExpiredDate = DateTime.Now.AddDays(item.ExpiredDayPosition),
                        HasBudget = item.HasBudget == CheckBudgetOption.Budget,
                        Quantity = item.Quantity,
                        LocationCode = item.LocationCode,
                        LocationName = item.LocationName,
                        Status = PositionStatus.Opened,
                        PositionName = item.PositionName
                    };
                    uow.GetRepository<Position>().Add(pos);
                    latestPosition = pos;
                    uow.GetRepository<Permission>().Add(new Permission()
                    {
                        UserId = item.AssignToId,
                        DepartmentId = assignedToDept?.Id,
                        DepartmentType = Group.Member,
                        ItemId = pos.Id,
                        Perm = Right.Edit
                    });

                    //end
                }
                if (item.HasBudget == CheckBudgetOption.Budget)
                {
                    var headCount = await uow.GetRepository<HeadCount>().GetSingleAsync(x => x.DepartmentId == item.DeptDivisionId);
                    if (headCount != null)
                    {
                        if (headCount.Quantity > 0 && headCount.Quantity >= item.Quantity)
                        {
                            headCount.Quantity -= item.Quantity;
                        }
                        else
                        {
                            headCount.Quantity = 0;
                        }
                    }
                }
            }
            else
            {
                var dept = await uow.GetRepository<Department>().GetSingleAsync(x => x.Id == item.ReplacementForId);
                dept.RequestToHireId = item.Id;
                dept.RTHReferenceNumber = item.ReferenceNumber;
                dept.EnableForPromoteActing = true;
                dept.PositionCode = item.PositionCode;
                dept.PositionName = item.PositionName;
                this.setBusinessModel(dept, businessModelUnitMapping, item.LocationCode);
                deptDivisionid = dept.Id;

                //Fix bug 329
                //Create request to hire - new position
                var pos = new Position()
                {
                    AssignToId = item.AssignToId,
                    DeptDivisionId = deptDivisionid,
                    RequestToHireId = item.Id,
                    RequestToHireNumber = item.ReferenceNumber,
                    ExpiredDay = item.ExpiredDayPosition,
                    ExpiredDate = DateTime.Now.AddDays(item.ExpiredDayPosition),
                    HasBudget = item.HasBudget == CheckBudgetOption.Budget,
                    Quantity = item.Quantity,
                    LocationCode = item.LocationCode,
                    LocationName = item.LocationName,
                    Status = PositionStatus.Opened,
                    PositionName = item.PositionName
                };
                uow.GetRepository<Position>().Add(pos);
                latestPosition = pos;
                uow.GetRepository<Permission>().Add(new Permission()
                {
                    UserId = item.AssignToId,
                    DepartmentId = assignedToDept?.Id,
                    DepartmentType = Group.Member,
                    ItemId = pos.Id,
                    Perm = Right.Edit
                });
                //end
            }
            uow.GetRepository<Permission>().Add(new Permission()
            {
                UserId = item.AssignToId,
                DepartmentId = assignedToDept?.Id,
                DepartmentType = Group.Member,
                ItemId = item.Id,
                Perm = Right.View
            });
            await uow.CommitAsync();
            //Send mail to Assignee.
            await SendEmailNotificationToAssignee(item.AssignToId, latestPosition, uow, logger);
            try
            {
                // HR-1357
                if (item.JobGradeGrade != null && item.JobGradeGrade.HasValue && item.JobGradeGrade.Value != 1)
                {
                    MassBO massBO = new MassBO(logger, uow);
                    PositionMassViewModel data = new PositionMassViewModel
                    {
                        Name = latestPosition.PositionName,
                        RequiredQuantity = latestPosition.Quantity,
                        AlertQuantity = latestPosition.Quantity,
                        MassLocationCode = item.MassLocationCode,
                        Id = latestPosition.Id,
                        JobGrade = item.JobGradeGrade.Value,
                        ReferenceRTH = item.ReferenceNumber
                        /*Description = item.JobDescription + "<br/> <br/> <h3>REQUIREMENTS:</h3><br/>" + item.JobRequirement,*/
                    };
                    if (item.CategoryId != null)
                    {
                        data.CategoryId = item.CategoryId.Value;
                    }
                    await massBO.PushPositionToMass(data);
                }
            }
            catch (Exception ex)
            {

            }
            try
            {
                dashboardBO.ClearNode();
            }
            catch (Exception e)
            {

            }

        }
        private string GenarateDeptNameByContractType(string value, string contractType)
        {
            if (!String.IsNullOrEmpty(value) && contractType == "PT")
            {
                return String.Format("{0} - {1}", value, contractType);
            }
            return value;
        }
        private async Task SendEmailNotificationToAssignee(Guid? Id, Position position, IUnitOfWork uow, ILogger logger)
        {
            if (Id.HasValue)
            {
                try
                {
                    EmailNotification emailNotification = new EmailNotification(logger, uow);
                    EmailTemplateName type = EmailTemplateName.ForAssigneeCompleted;
                    User user = await uow.GetRepository<User>().FindByIdAsync(Id.Value);
                    //logger.LogInformation($"Send email notification to {user.Email}");
                    var mergeFields = new Dictionary<string, string>();
                    mergeFields["AssignmentName"] = user.FullName;
                    mergeFields["PositionRefereceNumber"] = position.ReferenceNumber;
                    mergeFields["Link"] = $"<a href=\"{ Convert.ToString(ConfigurationManager.AppSettings["siteUrl"])}/_layouts/15/AeonHR/Default.aspx#!/home/position/allRequests/view=\">Link</a>";
                    var recipients = new List<string>() { user.Email };
                    await emailNotification.SendEmail(type, EmailTemplateName.ForAssigneeCompleted, mergeFields, recipients);
                }
                catch (Exception ex)
                {
                    logger.LogError("Error at method: SendEmailNotificationToAssignee " + ex.Message);
                }
            }
        }

        private void setBusinessModel(Department department, IEnumerable<BusinessModelUnitMapping> businessModelUnitMappings,  string locationCode)
        {
            if (!string.IsNullOrEmpty(locationCode))
            {
                var currentBusinessModel = businessModelUnitMappings.Where(y => y.BusinessUnitCode == locationCode).FirstOrDefault();
                if (!(currentBusinessModel is null))
                {
                    department.BusinessModelId = currentBusinessModel.BusinessModelId;
                }
            }
        }

    }
}
