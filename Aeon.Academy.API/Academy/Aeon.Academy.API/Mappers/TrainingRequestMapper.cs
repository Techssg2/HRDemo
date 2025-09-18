using Aeon.Academy.API.DTOs;
using Aeon.Academy.API.Utils;
using Aeon.Academy.Common.Consts;
using Aeon.Academy.Common.Entities;
using Aeon.Academy.Data.Entities;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aeon.Academy.API.Mappers
{
    public static class TrainingRequestMapper
    {
        public static TrainingRequest ToEntity(this TrainingRequestDto dto, TrainingRequest request, User user)
        {
            if (request == null) request = new TrainingRequest();
            request.Id = dto.Id;
            if (request.Id == Guid.Empty)
            {
                request.Created = DateTimeOffset.Now;
                request.CreatedBy = user.LoginName;
                request.CreatedByFullName = user.FullName;
                request.CreatedById = user.Id;
            }
            request.Modified = DateTimeOffset.Now;
            request.ModifiedBy = user.LoginName;
            request.ModifiedByFullName = user.FullName;
            request.ModifiedById = user.Id;

            request.Status = string.IsNullOrEmpty(dto.Status) ? WorkflowStatus.Draft : dto.Status;

            request.ReferenceNumber = dto.ReferenceNumber;
            request.SapNo = dto.Requester.SapNumber;
            request.Affiliate = dto.Requester.Affiliate;
            request.Position = dto.Requester.Position;
            request.DepartmentId = dto.Requester.DepartmentId;
            request.DepartmentName = dto.Requester.DepartmentName;
            request.RegionId = dto.Requester.RegionId;
            request.RegionName = dto.Requester.RegionName;
            request.DateOfSubmission = dto.Requester.DateOfSubmission;

            request.TypeOfTraining = dto.TrainingDetails.TypeOfTraining;
            request.ExistingCourse = dto.TrainingDetails.ExistingCourse;
            request.CategoryId = dto.TrainingDetails.CategoryId != null ? dto.TrainingDetails.CategoryId.Value : Guid.NewGuid();
            request.CourseId = dto.TrainingDetails.CourseId != null ? dto.TrainingDetails.CourseId.Value : Guid.NewGuid();
            request.CourseName = dto.TrainingDetails.CourseName;
            request.SupplierName = dto.TrainingDetails.SupplierName;
            request.TrainingFee = dto.TrainingDetails.TrainingFee.HasValue ? dto.TrainingDetails.TrainingFee.Value : 0;
            request.TrainingNotTotalFee = dto.TrainingDetails.TrainingNotTotalFee.HasValue ? dto.TrainingDetails.TrainingNotTotalFee.Value : 0;
            request.AccommodationIfAny = dto.TrainingDetails.AccommodationIfAny.HasValue ? dto.TrainingDetails.AccommodationIfAny.Value : 0;
            request.AirTicketFeeIfAny = dto.TrainingDetails.AirTicketFeeIfAny.HasValue ? dto.TrainingDetails.AirTicketFeeIfAny.Value : 0;
            request.ReasonOfTrainingRequest = dto.TrainingDetails.ReasonOfTrainingRequest;
            request.SupplierCode = dto.TrainingDetails.SupplierCode;
            request.Currency = dto.TrainingDetails.Currency;
            request.CurrencyJson = dto.TrainingDetails.CurrencyJson;

            request.TrainingMethod = dto.TrainingDuration.TrainingMethod;
            request.EstimatedTrainingDays = dto.TrainingDuration.EstimatedTrainingDays;
            request.EstimatedTrainingHours = dto.TrainingDuration.EstimatedTrainingHours != null ? dto.TrainingDuration.EstimatedTrainingHours.Value : 0;
            if (dto.TrainingDuration.TrainingDurationItems != null)
            {
                request.TrainingDurationItems = new List<TrainingDurationItem>();
                foreach (var item in dto.TrainingDuration.TrainingDurationItems)
                {
                    request.TrainingDurationItems.Add(new TrainingDurationItem()
                    {
                        Id = Guid.Empty == item.Id ? Guid.NewGuid() : item.Id,
                        TrainingRequestId = dto.Id,
                        TrainingMethod = item.TrainingMethod,
                        Duration = item.Duration,
                        From = item.From,
                        To = item.To,
                        TrainingLocation = item.TrainingLocation
                    });
                }
            }

            request.DepartmentInCharge = dto.ParticipantBudget.DepartmentInCharge;
            request.DepartmentInChargeId = dto.ParticipantBudget.DepartmentInChargeId;
            request.RequestedDepartment = dto.ParticipantBudget.RequestedDepartment;
            request.RequestedDepartmentCode = dto.ParticipantBudget.RequestedDepartmentCode;
            request.RequestedDepartmentId = dto.ParticipantBudget.RequestedDepartmentId;
            request.DicDepartmentCode = dto.ParticipantBudget.DicDepartmentCode;
            request.DepartmentInChargeCode = dto.ParticipantBudget.DepartmentInChargeCode;
            request.MethodOfChoosingContractor = dto.ParticipantBudget.MethodOfChoosingContractor;
            request.TheProposalFor = dto.ParticipantBudget.TheProposalFor;
            request.Year = dto.ParticipantBudget.Year;
            request.Reference = dto.ParticipantBudget.Reference;

            if (dto.ParticipantBudget.Participants != null)
            {
                request.TrainingRequestParticipants = new List<TrainingRequestParticipant>();
                foreach (var item in dto.ParticipantBudget.Participants)
                {
                    request.TrainingRequestParticipants.Add(new TrainingRequestParticipant()
                    {
                        Id = Guid.Empty == item.Id ? Guid.NewGuid() : item.Id,
                        TrainingRequestId = dto.Id,
                        ParticipantId = item.UserId,
                        SapCode = item.SapCode,
                        Name = item.Name,
                        Email = item.Email,
                        PhoneNumber = item.PhoneNumber,
                        Position = item.Position,
                        Department = item.Department
                    });
                }
            }

            request.ApplySponsorship = dto.TrainingSponsorshipContract.ApplySponsorship;
            request.SponsorshipPercentage = dto.TrainingSponsorshipContract.SponsorshipPercentage;

            request.WorkingCommitment = dto.WorkingCommitment.WorkingCommitment;
            request.WorkingCommitmentFrom = dto.WorkingCommitment.From;
            request.WorkingCommitmentTo = dto.WorkingCommitment.To;
            request.SponsorshipContractNumber = dto.WorkingCommitment.SponsorshipContractNumber;
            request.CompensateAmount = dto.WorkingCommitment.CompensateAmount;

            request.Purpose = dto.Purpose;
            request.Reason = dto.Reason;
            request.HowApply = dto.HowApply;
            request.WhenExcute = dto.WhenExcute;
            request.WhatExpectedKPI = dto.WhatExpectedKPI;

            request.ActualTuitionReimbursementAmount = dto.TrainingSponsorshipContract.ActualTuitionReimbursementAmount;
            request.From = dto.ParticipantBudget.From;
            request.To = dto.ParticipantBudget.To;
            if (dto.CostCenters.Any())
            {
                request.CostCenters = new List<TrainingRequestCostCenter>();
                foreach (var item in dto.CostCenters)
                {
                    request.CostCenters.Add(new TrainingRequestCostCenter
                    {
                        Id = Guid.Empty == item.Id ? Guid.NewGuid() : item.Id,
                        TrainingRequestId = request.Id,
                        Amount = item.Amount,
                        BudgetBalanced = item.BudgetBalance,
                        RemainingBalance = item.RemainingBalance,
                        BudgetCode = item.BudgetCode,
                        CostCenterCode = item.CostCenterCode,
                        Currency = item.Currency,
                        Type = item.Type,
                        VAT = item.Vat,
                        VATPercentage = item.VatPercentage,
                        BudgetPlan = item.BudgetPlan,
                        TotalBudget = item.TotalBudget,
                        TransferIn = item.TransferIn,
                        TransferOut = item.TransferOut,
                        Refund = item.Refund
                    });
                }
            }
            return request;
        }

        public static TrainingRequestDto ToDto(this TrainingRequest entity)
        {
            var dto = new TrainingRequestDto
            {
                Id = entity.Id,
                Status = entity.Status,
                ReferenceNumber = entity.ReferenceNumber,
                Requester = new RequesterDto
                {
                    RequesterId = entity.CreatedById.Value,
                    SapNumber = entity.SapNo,
                    RequesterName = entity.CreatedByFullName,
                    Affiliate = entity.Affiliate,
                    DepartmentId = entity.DepartmentId,
                    DepartmentName = entity.DepartmentName,
                    RegionId = entity.RegionId,
                    RegionName = entity.RegionName,
                    Position = entity.Position,
                    DateOfSubmission = entity.DateOfSubmission
                },
                TrainingDetails = new TrainingDetailsDto()
                {
                    TypeOfTraining = entity.TypeOfTraining,
                    ExistingCourse = entity.ExistingCourse,
                    CategoryId = entity.CategoryId,
                    CategoryName = entity.Category != null ? entity.Category.Name : string.Empty,
                    CourseId = entity.CourseId,
                    CourseName = entity.Course != null ? entity.Course.Name : entity.CourseName,
                    SupplierName = entity.SupplierName,
                    TrainingFee = entity.TrainingFee,
                    TrainingNotTotalFee = entity.TrainingNotTotalFee,
                    AccommodationIfAny = entity.AccommodationIfAny,
                    AirTicketFeeIfAny = entity.AirTicketFeeIfAny,
                    Currency = entity.Currency,
                    CurrencyJson = entity.CurrencyJson,
                    ReasonOfTrainingRequest = entity.ReasonOfTrainingRequest,
                    SupplierCode = entity.SupplierCode
                },
                TrainingDuration = new TrainingDurationDto()
                {
                    TrainingMethod = entity.TrainingMethod,
                    EstimatedTrainingDays = entity.EstimatedTrainingDays,
                    EstimatedTrainingHours = entity.EstimatedTrainingHours,
                },
                ParticipantBudget = new ParticipantBudgetDto()
                {
                    DepartmentInCharge = entity.DepartmentInCharge,
                    Year = entity.Year,
                    From = entity.From,
                    To = entity.To,
                    RequestedDepartment = entity.RequestedDepartment,
                    RequestedDepartmentCode = entity.RequestedDepartmentCode,
                    DepartmentInChargeCode = entity.DepartmentInChargeCode,
                    MethodOfChoosingContractor = entity.MethodOfChoosingContractor,
                    TheProposalFor = entity.TheProposalFor,
                    Reference = entity.Reference
                },
                TrainingSponsorshipContract = new TrainingSponsorshipContractDto()
                {
                    ApplySponsorship = entity.ApplySponsorship,
                    SponsorshipPercentage = entity.SponsorshipPercentage,
                    ActualTuitionReimbursementAmount = entity.ActualTuitionReimbursementAmount
                },
                WorkingCommitment = new WorkingCommitmentDto()
                {
                    WorkingCommitment = entity.WorkingCommitment,
                    From = entity.WorkingCommitmentFrom,
                    To = entity.WorkingCommitmentTo,
                    SponsorshipContractNumber = entity.SponsorshipContractNumber,
                    CompensateAmount = entity.CompensateAmount
                },
                Purpose = entity.Purpose,
                Reason = entity.Reason,
                HowApply = entity.HowApply,
                WhenExcute = entity.WhenExcute,
                WhatExpectedKPI = entity.WhatExpectedKPI,
            };

            if (entity.TrainingDurationItems != null)
            {
                foreach (var item in entity.TrainingDurationItems)
                {
                    dto.TrainingDuration.TrainingDurationItems.Add(new TrainingDurationItemDto()
                    {
                        Id = item.Id,
                        TrainingMethod = item.TrainingMethod,
                        Duration = item.Duration,
                        From = item.From,
                        To = item.To,
                        TrainingLocation = item.TrainingLocation
                    });
                }
            }
            if (entity.TrainingRequestParticipants != null)
            {
                foreach (var item in entity.TrainingRequestParticipants)
                {
                    dto.ParticipantBudget.Participants.Add(new ParticipantDto()
                    {
                        Id = item.Id,
                        TrainingRequestId = entity.Id,
                        UserId = item.ParticipantId,
                        SapCode = item.SapCode,
                        Name = item.Name,
                        Email = item.Email,
                        PhoneNumber = item.PhoneNumber,
                        Position = item.Position,
                        Department = item.Department
                    });
                }
            }
            if (entity.CostCenters != null)
            {
                foreach (var item in entity.CostCenters)
                {
                    dto.CostCenters.Add(new CostCenterDto
                    {
                        Id = item.Id,
                        Amount = item.Amount,
                        BudgetBalance = item.BudgetBalanced,
                        RemainingBalance = item.RemainingBalance,
                        BudgetCode = item.BudgetCode,
                        CostCenterCode = item.CostCenterCode,
                        Currency = item.Currency,
                        Type = item.Type,
                        Vat = item.VAT,
                        VatPercentage = item.VATPercentage,
                        BudgetPlan = item.BudgetPlan,
                        TotalBudget = item.TotalBudget,
                        TransferIn = item.TransferIn,
                        TransferOut = item.TransferOut,
                        Refund = item.Refund
                    });
                }
            }

            dto.RealStatus = HttpUtil.ConvertStatus(entity.Status, entity.AssignedDepartmentPosition, entity.AssignedUserId);
            dto.F2ReferenceNumber = entity.F2ReferenceNumber;
            dto.F2Url = entity.F2URL;
            return dto;
        }
        public static List<TrainingRequestDto> ToDto(this IList<TrainingRequest> entities)
        {
            var list = new List<TrainingRequestDto>();
            foreach (var c in entities)
                list.Add(c.ToDto());
            return list;
        }

        public static IList<MyItemDto> ToMyItemDtos(this IList<TrainingRequest> requests)
        {
            var dtos = new List<MyItemDto>();

            if (requests == null) return dtos;

            var itemType = typeof(TrainingRequest).Name;

            foreach (var req in requests)
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
                dto.Status = HttpUtil.ConvertStatus(req.Status, req.AssignedDepartmentPosition, req.AssignedUserId);
                dtos.Add(dto);
            }

            return dtos;
        }

        public static IList<WorkflowTaskDto> ToMyTaskDtos(this IList<TrainingRequest> requests)
        {
            var dtos = new List<WorkflowTaskDto>();

            if (requests == null) return dtos;

            var itemType = typeof(TrainingRequest).Name;

            foreach (var req in requests)
            {
                var dto = new WorkflowTaskDto
                {
                    Created = req.Created,
                    DueDate = req.DueDate.GetValueOrDefault() != DateTimeOffset.MinValue ? req.DueDate.GetValueOrDefault() : DateTimeOffset.Now.AddDays(7),
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
                dto.Status = HttpUtil.ConvertStatus(req.Status, req.AssignedDepartmentPosition, req.AssignedUserId);

                dtos.Add(dto);
            }

            return dtos;
        }

        public static IList<TrainingRequestManagementDto> ToManagementDtos(this IList<TrainingRequest> requests)
        {
            var dtos = new List<TrainingRequestManagementDto>();
            if (requests == null) return dtos;
            foreach (var req in requests)
            {
                var dto = new TrainingRequestManagementDto
                {
                    Id = req.Id,
                    ReferenceNumber = req.ReferenceNumber,
                    RequesterName = req.CreatedByFullName,
                    DepartmentName = req.DepartmentName,
                    TypeOfTraining = req.TypeOfTraining,
                    TrainingFee = req.TrainingFee,
                    Status = req.Status
                };
                dto.Status = HttpUtil.ConvertStatus(req.Status, req.AssignedDepartmentPosition, req.AssignedUserId);
                dtos.Add(dto);
            }

            return dtos;
        }
        public static NotificationUserViewModel ToUserDto(this NotificationUser user)
        {
            var dto = new NotificationUserViewModel();
            if (user == null) return dto;
            dto = new NotificationUserViewModel
            {
                UserId = user.UserId,
                UserEmail = user.UserEmail,
                UserFullName = user.UserFullName,
                DepartmentId = user.DepartmentId,
                DepartmentGroup = (Group)user.DepartmentGroup,
            };

            return dto;
        }
        public static IList<TrainingRequestViewItemDto> ToViewItemDtos(this IList<TrainingRequest> requests)
        {
            var dtos = new List<TrainingRequestViewItemDto>();

            if (requests == null) return dtos;

            var itemType = typeof(TrainingRequest).Name;

            foreach (var req in requests)
            {
                var dto = new TrainingRequestViewItemDto
                {
                    Created = req.Created,
                    DueDate = req.DueDate.GetValueOrDefault(),
                    ItemId = req.Id,
                    ItemType = itemType,
                    ReferenceNumber = req.ReferenceNumber,
                    RequestedDepartmentName = req.DepartmentName,
                    RequestedDepartmentCode = string.Empty,
                    RegionName = req.RegionName,
                    RequestorFullName = req.CreatedByFullName,
                    CreatedByFullName = req.CreatedByFullName,
                    Status = req.Status,
                    Modified = req.Modified
                };
                dto.Status = HttpUtil.ConvertStatus(req.Status, req.AssignedDepartmentPosition, req.AssignedUserId);
                dtos.Add(dto);
            }
            return dtos;
        }
    }
}