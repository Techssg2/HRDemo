using Aeon.Academy.API.DTOs;
using Aeon.Academy.API.Utils;
using Aeon.Academy.Common.Consts;
using Aeon.Academy.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aeon.Academy.API.Mappers
{
    public static class TrainingReportMapper
    {
        public static TrainingReport ToEntity(this TrainingReportDto dto, TrainingReport report, User user)
        {
            if (report == null) report = new TrainingReport
            {
                Id = dto.Id,
                Created = DateTimeOffset.Now,
                CreatedBy = user.LoginName,
                CreatedByFullName = user.FullName,
                CreatedById = user.Id,
                FullName = user.FullName
            };
 
            report.Modified = DateTimeOffset.Now;
            report.ModifiedBy = user.LoginName;
            report.ModifiedByFullName = user.FullName;
            report.ModifiedById = user.Id;
            report.Status = string.IsNullOrEmpty(dto.Status) ? WorkflowStatus.Draft : dto.Status;

            report.TrainingInvitationId = dto.TrainingInvitationId;
            report.SapCode = dto.EmployeeInfo.SapCode;
            report.Location = dto.EmployeeInfo.Location;
            report.Position = dto.EmployeeInfo.Position;
            report.DepartmentId = dto.EmployeeInfo.DepartmentId;
            report.DepartmentName = dto.EmployeeInfo.DepartmentName;
            report.RegionId = dto.EmployeeInfo.RegionId;
            report.RegionName = dto.EmployeeInfo.RegionName;
            report.TrainerName = dto.TrainerName;
            report.OtherReasons = dto.OtherReasons;
            report.OtherFeedback = dto.OtherFeedback;
            report.ActualAttendingDate = dto.ActualAttendingDate;

            if (dto.TrainingActionPlans.Any())
            {
                report.TrainingActionPlans = new List<TrainingActionPlan>();
                foreach (var item in dto.TrainingActionPlans)
                {
                    report.TrainingActionPlans.Add(new TrainingActionPlan()
                    {
                        Id = Guid.NewGuid(),
                        ActionPlanCode = item.ActionPlanCode,
                        Quarter1 = item.Quarter1,
                        Quarter2 = item.Quarter2,
                        Quarter3 = item.Quarter3,
                        Quarter4 = item.Quarter4,
                        TrainingReportId = report.Id
                    });
                }
            }

            if (dto.TrainingSurveyQuestions.Any())
            {
                report.TrainingSurveyQuestions = new List<TrainingSurveyQuestion>();
                foreach (var item in dto.TrainingSurveyQuestions)
                {
                    report.TrainingSurveyQuestions.Add(new TrainingSurveyQuestion()
                    {
                        Id = Guid.NewGuid(),
                        TrainingReportId = report.Id,
                        SurveyQuestion = item.SurveyQuestion,
                        ParentQuestion = item.ParentQuestion,
                        Value = item.Value
                    });
                }
            }

            return report;
        }

        public static TrainingReportDto ToDto(this TrainingReport entity)
        {
            var dto = new TrainingReportDto
            {
                Id = entity.Id,
                Status = entity.Status,
                ReferenceNumber = entity.ReferenceNumber,
                TrainingInvitationId = entity.TrainingInvitationId,
                EmployeeInfo = new EmployeeInfoDto
                {
                    SapCode = entity.SapCode,
                    Location = entity.Location,
                    Position = entity.Position,
                    DepartmentId = entity.DepartmentId,
                    DepartmentName = entity.DepartmentName,
                    RegionId = entity.RegionId,
                    RegionName = entity.RegionName,
                    FullName = entity.FullName
                },
                OtherReasons = entity.OtherReasons,
                OtherFeedback = entity.OtherFeedback,
                Remark = entity.Remark
            };
            if (entity.TrainingInvitation != null)
            {
                dto.TrainerName = string.IsNullOrEmpty(entity.TrainerName) ? entity.TrainingInvitation.TrainerName : entity.TrainerName;
                dto.ActualAttendingDate = entity.ActualAttendingDate;
                dto.CourseName = entity.TrainingInvitation.CourseName;
                if(entity.TrainingInvitation.TrainingRequest != null)
                {
                    dto.SupplierName = entity.TrainingInvitation.TrainingRequest.SupplierName;
                }
            }

            if (entity.TrainingSurveyQuestions != null)
            {
                foreach (var item in entity.TrainingSurveyQuestions)
                {
                    dto.TrainingSurveyQuestions.Add(new TrainingSurveyQuestionDto()
                    {
                        Id = item.Id,
                        TrainingReportId = item.TrainingReportId,
                        SurveyQuestion = item.SurveyQuestion,
                        ParentQuestion = item.ParentQuestion,
                        Value = item.Value
                    });
                }
            }
            if (entity.TrainingActionPlans != null)
            {
                foreach (var item in entity.TrainingActionPlans)
                {
                    dto.TrainingActionPlans.Add(new TrainingActionPlanDto()
                    {
                        Id = item.Id,
                        TrainingReportId = item.TrainingReportId,
                        ActionPlanCode = item.ActionPlanCode,
                        Quarter1 = item.Quarter1,
                        Quarter2 = item.Quarter2,
                        Quarter3 = item.Quarter3,
                        Quarter4 = item.Quarter4
                    });
                }
            }
            dto.RealStatus = HttpUtil.ConvertStatus(entity.Status, entity.AssignedDepartmentPosition, null);
            return dto;
        }
        public static List<TrainingReportDto> ToDto(this IList<TrainingReport> entities)
        {
            var list = new List<TrainingReportDto>();
            foreach (var c in entities)
                list.Add(c.ToDto());
            return list;
        }

        public static IList<MyItemDto> ToMyItemDtos(this IList<TrainingReport> reports)
        {
            var dtos = new List<MyItemDto>();

            if (reports == null) return dtos;

            var itemType = typeof(TrainingReport).Name;

            foreach (var req in reports)
            {
                var dto = new MyItemDto
                {
                    Id = req.Id,
                    Created = req.Created,
                    CreatedBy = req.CreatedBy,
                    CreatedByFullName = req.CreatedByFullName,
                    CreatedById = req.CreatedById,
                    ItemType = itemType,
                    Modified = req.Modified,
                    ModifiedBy = req.ModifiedBy,
                    ModifiedByFullName = req.ModifiedByFullName,
                    ModifiedById = req.ModifiedById,
                    ReferenceNumber = req.ReferenceNumber,
                    Status = req.Status
                };
                dto.Status = HttpUtil.ConvertStatus(req.Status, req.AssignedDepartmentPosition, null);
                dtos.Add(dto);
            }

            return dtos;
        }

        public static IList<WorkflowTaskDto> ToMyTaskDtos(this IList<TrainingReport> reports)
        {
            var dtos = new List<WorkflowTaskDto>();

            if (reports == null) return dtos;

            var itemType = typeof(TrainingReport).Name;

            foreach (var req in reports)
            {
                var dto = new WorkflowTaskDto
                {
                    Created = req.Created,
                    DueDate = req.DueDate.GetValueOrDefault(),
                    IsCompleted = req.Status == WorkflowStatus.Completed,
                    ItemId = req.Id,
                    ItemType = itemType,
                    ReferenceNumber = req.ReferenceNumber,
                    RequestedDepartmentId = req.DepartmentId,
                    RequestedDepartmentName = req.DepartmentName,
                    RequestedDepartmentCode = string.Empty,
                    RegionId = req.RegionId,
                    RegionName = req.RegionName,
                    RequestorFullName = req.CreatedByFullName,
                    RequestorUserName = req.CreatedBy,
                    RequestorId = req.CreatedById,
                    Status = req.Status
                };
                dto.Status = HttpUtil.ConvertStatus(req.Status, req.AssignedDepartmentPosition, null);
                dtos.Add(dto);
            }

            return dtos;
        }
        public static List<object> ToManagementDtos(this List<TrainingReport> reports)
        {
            var dtos = new List<object>();

            if (reports == null) return dtos;

            var itemType = typeof(TrainingReport).Name;

            foreach (var req in reports)
            {
                var dto = new
                {
                    DateSubmit = req.Created,
                    ItemId = req.Id,
                    ItemType = itemType,
                    req.ReferenceNumber,
                    RequestedDepartmentId = req.DepartmentId,
                    RequestedDepartmentName = req.DepartmentName,
                    req.TrainingInvitation.TrainingRequest.TypeOfTraining ,
                    req.TrainingInvitation.CourseName,
                    RequestorFullName = req.CreatedByFullName,
                    RequestorUserName = req.CreatedBy,
                    RequestorId = req.CreatedById,
                    Status = HttpUtil.ConvertStatus(req.Status, req.AssignedDepartmentPosition, null)
                };
                
                dtos.Add(dto);
            }

            return dtos;
        }

    }
}