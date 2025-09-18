using Aeon.HR.API.ExternalItem;
using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.Data.Models;
using Aeon.HR.Data.Models.SyncLog;
using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.BTA;
using Aeon.HR.ViewModels.DTOs;
using Aeon.HR.ViewModels.ExternalItem;
using Aeon.HR.ViewModels.PrintFormViewModel;
using Aeon.HR.ViewModels.Tree;
using AutoMapper;
using Microsoft.Office.Interop.Word;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Aeon.HR.BusinessObjects.DataMappingProfile
{
    public class DefaultMapping : Profile
    {
        public DefaultMapping()
        {
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<User, UserDataForCreatingArgs>().ReverseMap();
            CreateMap<Region, RegionViewModel>().ReverseMap();
            CreateMap<MaintainPromoteAndTranferPrint, PromoteAndTranferPrintViewModel>().ReverseMap();
            CreateMap<MaintainPromoteAndTranferPrint, PromoteAndTranferPrintArgs>().ReverseMap();
            CreateMap<Applicant, ApplicantViewModel>()
                .ForMember(dest => dest.DeptDivision, opt => opt.MapFrom(src => src.Position.DeptDivision.Name))
                .ForMember(dest => dest.PositionName, opt => opt.MapFrom(src => src.Position.PositionName))
                .ReverseMap();
            CreateMap<Applicant, SimpleApplicantViewModel>()
                .ForMember(dest => dest.PositionName, opt => opt.MapFrom(src => src.Position.PositionName))
                .ForMember(dest => dest.PositionId, opt => opt.MapFrom(src => src.Position.Id))
                .ForMember(dest => dest.LocationName, opt => opt.MapFrom(src => src.Position.RequestToHire.LocationName))
                .ForMember(dest => dest.LocationCode, opt => opt.MapFrom(src => src.Position.RequestToHire.LocationCode))
                .ForMember(dest => dest.JobGradeCaption, opt => opt.MapFrom(src => src.Position.RequestToHire.JobGradeCaption))
                .ForMember(dest => dest.DeptDivision, opt => opt.MapFrom(src => src.Position.RequestToHire.ReplacementFor == TypeOfNeed.NewPosition ? src.Position.RequestToHire.DeptDivisionName : src.Position.RequestToHire.ReplacementForName))
                .ForMember(dest => dest.DeptDivisionId, opt => opt.MapFrom(src => src.Position.RequestToHire.ReplacementFor == TypeOfNeed.NewPosition ? src.Position.RequestToHire.DeptDivisionId.Value : src.Position.RequestToHire.ReplacementForId.Value))
                .ForMember(dest => dest.DeptDivisionCode, opt => opt.MapFrom(src => src.Position.RequestToHire.ReplacementFor == TypeOfNeed.NewPosition ? src.Position.RequestToHire.DeptDivisionCode : src.Position.RequestToHire.ReplacementForCode))
                .ForMember(dest => dest.DeptDivisionJobGradeId, opt => opt.MapFrom(src => src.Position.RequestToHire.JobGradeId))
                .ForMember(dest => dest.DepartmentType, opt => opt.MapFrom(src => src.Position.RequestToHire.ReplacementFor == TypeOfNeed.NewPosition ? "New Position" : "Replacement For"))
                .ReverseMap();
            CreateMap<CharacterReferee, CharacterRefereeViewModel>().ReverseMap();
            CreateMap<EmploymentHistory, EmploymentHistoryViewModel>().ReverseMap();
            CreateMap<EmergencyContact, EmergencyContactViewModel>().ReverseMap();
            CreateMap<FamilyMember, FamilyMemberViewModel>().ReverseMap();
            CreateMap<InterviewEvaluate, InterviewEvaluateViewModel>().ReverseMap();
            CreateMap<LanguageProficiencyEntry, LanguageProficiencyEntryViewModel>().ReverseMap();
            CreateMap<Activity, ActivityViewModel>();

            CreateMap<ApplicantRelativeInAeon, ApplicantRelativeInAeonViewModel>().ReverseMap();
            CreateMap<ApplicantEducation, ApplicantEducationViewModel>().ReverseMap();
            CreateMap<ApplicantWorkingProcess, ApplicantWorkingProcessViewModel>().ReverseMap();
            CreateMap<FamilyMember, FamilyMemberViewModel>().ReverseMap();
            CreateMap<Education, EducationViewModel>().ReverseMap();
            CreateMap<AddressInfomation, AddressInfomationViewModel>().ReverseMap();
            CreateMap<EmergencyContact, EmergencyContactViewModel>().ReverseMap();

            CreateMap<WorkingTimeRecruitment, WorkingTimeRecruimentArgs>().ReverseMap();
            CreateMap<RecruitmentCategory, RecruitmentCategoryArgs>().ReverseMap();
            CreateMap<UserDataForCreatingArgs, UserListViewModel>()
                .ForMember(dest => dest.ProfilePictureId, opt => opt.Ignore()).ReverseMap();
            CreateMap<SAPUserDataForCreatingArgs, UserListViewModel>()
                .ForMember(dest => dest.ProfilePictureId, opt => opt.Ignore()).ReverseMap();
            //tudm
            CreateMap<User, UserListAPIViewModel>();
            CreateMap<UserListViewModel, User>();
            CreateMap<User, UserListViewModel>()
                .ForMember(dest => dest.ProfilePicture, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.ProfilePicture.FileUniqueName) ? "/Attachments/" + src.ProfilePicture.FileUniqueName : string.Empty))
                .ForMember(x => x.UserDepartmentMappingsDepartmentCode, opt => opt.MapFrom(src => src.UserDepartmentMappings.Where(y => y.IsHeadCount).FirstOrDefault().Department.Code))
                .ForMember(x => x.UserDepartmentMappingsDepartmentName, opt => opt.MapFrom(src => src.UserDepartmentMappings.Where(y => y.IsHeadCount).FirstOrDefault().Department.Name))
                .ForMember(x => x.UserDepartmentMappingsJobGradeId, opt => opt.MapFrom(src => src.UserDepartmentMappings.Where(y => y.IsHeadCount).FirstOrDefault().Department.JobGradeId))
                .ForMember(x => x.UserDepartmentMappingsJobGradeGrade, opt => opt.MapFrom(src => src.UserDepartmentMappings.Where(y => y.IsHeadCount).FirstOrDefault().Department.JobGrade.Grade))
                .ForMember(x => x.UserDepartmentMappingsJobGradeCaption, opt => opt.MapFrom(src => src.UserDepartmentMappings.Where(y => y.IsHeadCount).FirstOrDefault().Department.JobGrade.Caption))
                .ForMember(x => x.UserDepartmentMappingsJobGradeTitle, opt => opt.MapFrom(src => src.UserDepartmentMappings.Where(y => y.IsHeadCount).FirstOrDefault().Department.JobGrade.Title))
            .ReverseMap();
            CreateMap<BookingContract, BookingContractViewModel>();
            CreateMap<BookingContract, BookingContractArgs>();
            // Recruitment
            CreateMap<Position, PositionViewModel>()
                .ForMember(x => x.ApplicantsCount, opt => opt.MapFrom(src => src.Applicants.Count()))
                .ForMember(x => x.HiredApplicantsCount, opt => opt.MapFrom(src => src.Applicants.Count(x => x.IsSignedOffer)))
                .ForMember(x => x.AssignToDepartmentCode, opt => opt.MapFrom(src => src.AssignTo.UserDepartmentMappings.Where(y => y.IsHeadCount).FirstOrDefault().Department.Code))
                .ForMember(x => x.AssignToDepartmentName, opt => opt.MapFrom(src => src.AssignTo.UserDepartmentMappings.Where(y => y.IsHeadCount).FirstOrDefault().Department.Name))
                .ForMember(x => x.AssignToJobGradeId, opt => opt.MapFrom(src => src.AssignTo.UserDepartmentMappings.Where(y => y.IsHeadCount).FirstOrDefault().Department.JobGradeId))
                .ForMember(x => x.AssignToJobGradeGrade, opt => opt.MapFrom(src => src.AssignTo.UserDepartmentMappings.Where(y => y.IsHeadCount).FirstOrDefault().Department.JobGrade.Grade))
                .ForMember(x => x.AssignToJobGradeCaption, opt => opt.MapFrom(src => src.AssignTo.UserDepartmentMappings.Where(y => y.IsHeadCount).FirstOrDefault().Department.JobGrade.Caption))
                .ReverseMap();
            CreateMap<Position, OpenPositionViewModel>()
                .ForMember(x => x.PositionGrade, opt => opt.MapFrom(src => src.RequestToHire.ReplacementFor == TypeOfNeed.NewPosition ? src.RequestToHire.JobGradeGrade.Value : src.RequestToHire.ReplacementForGrade.Value));
            CreateMap<Position, PositionForCreatingArgs>().ReverseMap();
            CreateMap<Position, PositionForFilterViewModel>();
            CreateMap<Position, PositionMassViewModel>()
                .ForMember(x => x.RequiredQuantity, x => x.MapFrom(y => y.Quantity))
                .ForMember(x => x.AlertQuantity, x => x.MapFrom(y => y.Quantity));
            CreateMap<Position, PositionExportViewModel>().ReverseMap();
            CreateMap<TrackingLog, TrackingLogViewModel>().ReverseMap();
            CreateMap<TrackingLog, TrackingLogArgs>().ReverseMap();
            CreateMap<WorkingTimeRecruitment, WorkingTimeRecruimentViewModel>().ReverseMap();
            CreateMap<RecruitmentCategory, RecruitmentCategoryViewModel>().ReverseMap();
            CreateMap<ItemListRecruitment, ItemListViewRecruitmentViewModel>().ReverseMap();
            CreateMap<ItemListRecruitment, ItemListViewRecruitmentArgs>().ReverseMap();
            CreateMap<ItemListRecruitment, ItemListRecruitmentViewModel>();
            CreateMap<ApplicantStatusRecruitment, ApplicantStatusRecruitmentViewModel>().ReverseMap();
            CreateMap<ApplicantStatusRecruitment, ApplicantStatusRecruitmentArgs>().ReverseMap();
            CreateMap<AppreciationListRecruitment, AppreciationListRecruitmentViewModel>().ReverseMap();
            CreateMap<RequestToHire, RequestToHireDataForCreatingArgs>()
                .ReverseMap();
            CreateMap<RequestToHire, RequestToHireViewModel>()
                .ForMember(x => x.AssignToFullName, opt => opt.MapFrom(src => src.AssignTo.FullName))
                .ForMember(x => x.AssignToSAPCode, opt => opt.MapFrom(src => src.AssignTo.SAPCode))
                .ForMember(x => x.JobGradeTitle, opt => opt.MapFrom(src => src.JobGrade.Title))
                .ReverseMap();
            CreateMap<RequestToHire, RequestToHireForPrintViewModel>()
                    .ForMember(x => x.Position, opt => opt.MapFrom(src => src.PositionName))
                    .ForMember(x => x.DepartmentName, opt => opt.MapFrom(src => src.ReplacementForName))
                      .ForMember(x => x.TotalCurrentBalance, opt => opt.MapFrom(src => src.CurrentBalance))
                      .ForMember(x => x.WorkLocation, opt => opt.MapFrom(src => src.LocationName))
                      .ForMember(x => x.JobGrade, opt => opt.MapFrom(src => src.JobGradeGrade))
                      .ForMember(x => x.StartingDate, opt => opt.MapFrom(src => src.StartingDateRequire.HasValue && src.StartingDateRequire != DateTimeOffset.FromUnixTimeSeconds(0) ? src.StartingDateRequire.Value.ToLocalTime().ToString("dd/MM/yyyy") : ""))
                    .ForMember(x => x.WHPerWeek, opt => opt.MapFrom(src => src.WorkingHoursPerWeerk.HasValue ? src.WorkingHoursPerWeerk.Value.ToString() : ""))
                    .ForMember(x => x.WagePerHours, opt => opt.MapFrom(src => src.WagePerHour.HasValue ? src.WagePerHour.Value.ToString() : ""))
                    .ForMember(x => x.ReplacementForUserFullName, opt => opt.MapFrom(src => src.ReplacementForUser.FullName))
                    .ForMember(x => x.CostCenter, opt => opt.MapFrom(src => src.CostCenterRecruitment.Description))
                .ReverseMap();
            CreateMap<CostCenterRecruitment, CostCenterRecruitmentViewModel>().ReverseMap();
            CreateMap<CostCenterRecruitment, CostCenterRecruitmentArgs>().ReverseMap();
            CreateMap<WorkingAddressRecruitment, WorkingAddressRecruimentArgs>().ReverseMap();
            CreateMap<WorkingAddressRecruitment, WorkingAddressRecruitmentViewModel>().ReverseMap();
            #region Handover
            CreateMap<Handover, HandoverDataForCreatingArgs>().ReverseMap();
            CreateMap<HandoverViewModel, Handover>();
            CreateMap<Handover, HandoverViewModel>()
                .ForMember(x => x.HandoverDetailItems, x => x.Ignore());
            //.ForMember(src => src.Mapping, opt => opt.MapFrom(x => x.User.UserDepartmentMappings.FirstOrDefault(y => y.IsHeadCount)))
            //.ForMember(x => x.SAPCode, opt => opt.MapFrom(x => x.User.SAPCode));
            CreateMap<HandoverDetailItem, HandoverDetailItemViewModel>().ReverseMap();
            CreateMap<HandoverDetailItem, HandoverItemDetailViewModel>().ReverseMap();
            #endregion
            CreateMap<AppreciationListRecruitment, AppreciationListRecruitmentArgs>().ReverseMap();
            CreateMap<ReferenceNumber, ReferencyNumberViewModel>().ReverseMap();
            CreateMap<ReferenceNumber, ReferencyNumberArgs>().ReverseMap();
            CreateMap<ResignationApplication, ResignationApplicationArgs>();
            CreateMap<ResignationApplicationArgs, ResignationApplication>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<ResignationApplication, ResignationApplicationViewModel>()
                .ReverseMap();
            CreateMap<ResignationApplication, ResignationApplicationPrintFormViewModel>()
                .ForMember(x => x.UnusedLeaveDate, opt => opt.MapFrom(y => y.UnusedLeaveDate.ToString()))
                .ForMember(x => x.CreatedDate, opt => opt.MapFrom(y => y.Created.LocalDateTime.ToString("dd/MM/yyyy")))
                .ForMember(x => x.StartingDate, opt => opt.MapFrom(y => (y.StartingDate.HasValue && y.StartingDate != DateTimeOffset.FromUnixTimeSeconds(0)) ? y.StartingDate.Value.LocalDateTime.ToString("dd/MM/yyyy") : ""))
                .ForMember(x => x.OffResDate, opt => opt.MapFrom(y => y.OfficialResignationDate != DateTimeOffset.FromUnixTimeSeconds(0) ? y.OfficialResignationDate.LocalDateTime.ToString("dd/MM/yyyy") : ""))
                .ForMember(x => x.SuggestionForLastWorkingDay, opt => opt.MapFrom(y => (y.SuggestionForLastWorkingDay.HasValue && y.SuggestionForLastWorkingDay != DateTimeOffset.FromUnixTimeSeconds(0)) ? y.SuggestionForLastWorkingDay.Value.LocalDateTime.ToString("dd/MM/yyyy") : ""))

                .ReverseMap();
            CreateMap<ResignationApplicantGridViewModel, ResignationApplication>();
            CreateMap<ResignationApplication, ResignationApplicantGridViewModel>()
                .ForMember(x => x.UserWorkLocationName, opt => opt.MapFrom(y => y.WorkLocationName)).ReverseMap();
            CreateMap<Applicant, ItemListApplicantViewModel>()
                //.ForMember(x => x.PositionName, opt => opt.MapFrom(src => src.AppliedPosition1StPriority.PositionName))
                .ReverseMap();
            CreateMap<Applicant, ApplicantArgs>().ReverseMap();
            CreateMap<Applicant, PrositionApplicantViewModel>()
                 .ForMember(x => x.FullName, opt => opt.MapFrom(y => y.FullName)).ReverseMap();

            #region DEPARTMENT SETTING
            CreateMap<Department, DepartmentArgs>().ReverseMap();
            CreateMap<Department, ItemListDepartmentViewModel>()
                .ForMember(x => x.ParentDepartmentType, opt => opt.MapFrom(x => x.Parent.Type))
                .ReverseMap();
            CreateMap<Department, DepartmentForTreeViewModel>();
            CreateMap<Department, DepartmentViewModel>()
                .ForMember(x => x.UserCheckedHeadCount, opt => opt.MapFrom(x => x.UserDepartmentMappings.Where(y => y.IsHeadCount).FirstOrDefault().User.FullName))
                .ForMember(x => x.UserCheckedHeadCountSAPCode, opt => opt.MapFrom(x => x.UserDepartmentMappings.Where(y => y.IsHeadCount).FirstOrDefault().User.SAPCode))
                 .ForMember(x => x.UserCheckedHeadCountSAPId, opt => opt.MapFrom(x => x.UserDepartmentMappings.Where(y => y.IsHeadCount).FirstOrDefault().User.Id))
                .ForMember(x => x.SubmitSAPCodes, opt => opt.MapFrom(x => x.UserSubmitPersonDeparmentMappings.Where(t => t.IsSubmitPerson).Select(y => y.User.SAPCode)))
                .ForMember(x => x.IsIncludeChildren, opt => opt.MapFrom(x => x.UserSubmitPersonDeparmentMappings.FirstOrDefault(i => i.DepartmentId == x.Id && i.IsSubmitPerson).IsIncludeChildren))
                .ForMember(x => x.HQPosition, opt => opt.MapFrom(x => x.JobGrade != null ? x.JobGrade.HQPosition : (HQPositionType?) null))
                .ForMember(x => x.StorePosition, opt => opt.MapFrom(x => x.JobGrade != null ? x.JobGrade.StorePosition : (StorePositionType?) null))
                .ReverseMap();
            CreateMap<Department, DepartmentTreeViewModel>()
                .ForMember(x => x.UserCheckedHeadCount, opt => opt.MapFrom(x => x.UserDepartmentMappings.Where(y => y.IsHeadCount && !y.IsFromIT).FirstOrDefault().User.FullName))
                .ForMember(x => x.IsIncludeChildren, opt => opt.MapFrom(x => x.UserSubmitPersonDeparmentMappings.FirstOrDefault(i => i.DepartmentId == x.Id && i.IsSubmitPerson).IsIncludeChildren))
                .ForMember(x => x.UserCheckedHeadCountSapCode, opt => opt.MapFrom(x => x.UserDepartmentMappings.Where(y => y.IsHeadCount).FirstOrDefault().User.SAPCode))
                .ForMember(x => x.Items, opt => opt.Ignore())
                .ForMember(x => x.SubmitSAPCodes, opt => opt.MapFrom(x => x.UserSubmitPersonDeparmentMappings.Where(t => t.IsSubmitPerson).Select(y => y.User.SAPCode)))
                .ReverseMap();
            //tudm
            CreateMap<Department, DepartmentTreeAPIViewModel>();
            //CreateMap<TestDepartment, DepartmentTreeViewModel>().ReverseMap();
            CreateMap<User, UserSelectViewModel>().ReverseMap();
            CreateMap<User, UserForTreeViewModel>()
                .ForMember(src => src.DepartmentId, opt => opt.MapFrom(x => x.UserDepartmentMappings.Where(y => y.IsHeadCount).FirstOrDefault().Department.Id))
                .ForMember(src => src.Department, opt => opt.MapFrom(x => x.UserDepartmentMappings.Where(y => y.IsHeadCount).FirstOrDefault().Department.Name))
                .ForMember(src => src.DepartmentCode, opt => opt.MapFrom(x => x.UserDepartmentMappings.Where(y => y.IsHeadCount).FirstOrDefault().Department.Code))
                .ForMember(src => src.DepartmentName, opt => opt.MapFrom(x => x.UserDepartmentMappings.Where(y => y.IsHeadCount).FirstOrDefault().Department.Name))
                .ForMember(src => src.Position, opt => opt.MapFrom(x => x.UserDepartmentMappings.Where(y => y.IsHeadCount).FirstOrDefault().Department.PositionName))
                .ForMember(src => src.JobGrade, opt => opt.MapFrom(x => x.UserDepartmentMappings.Where(y => y.IsHeadCount).FirstOrDefault().Department.JobGrade.Caption))
                .ForMember(src => src.JobGradeValue, opt => opt.MapFrom(x => x.UserDepartmentMappings.Where(y => y.IsHeadCount).FirstOrDefault().Department.JobGrade.Grade))
                .ForMember(src => src.JobGradeTitle, opt => opt.MapFrom(x => x.UserDepartmentMappings.Where(y => y.IsHeadCount).FirstOrDefault().Department.JobGrade.Title))
                .ForMember(src => src.JobGradeId, opt => opt.MapFrom(x => x.UserDepartmentMappings.Where(y => y.IsHeadCount).FirstOrDefault().Department.JobGrade.Id))
                .ForMember(src => src.IsStore, opt => opt.MapFrom(x => x.UserDepartmentMappings.Where(y => y.IsHeadCount).FirstOrDefault().Department.IsStore))
                .ForMember(src => src.OfficialResignationDate, opt => opt.MapFrom(x => x.ResignationApplications.Where(y => y.Status.Contains("Completed")).OrderByDescending(t => t.Created).FirstOrDefault().OfficialResignationDate))
                .ReverseMap();
            CreateMap<EmployeeViewModel, CreatedByUserViewModel>()
                  .ForMember(x => x.DeptLine, opt => opt.MapFrom(y => y.DeptName))
                  .ForMember(x => x.DivisionGroup, opt => opt.MapFrom(y => y.DivisionName))
                  .ForMember(x => x.StartingDate, opt => opt.MapFrom(y => y.StartDate != DateTime.MinValue ? y.StartDate : (DateTime?)null))
                .ReverseMap();
            CreateMap<UserDepartmentMapping, UserInDepartmentArgs>().ReverseMap();
            CreateMap<UserDepartmentMapping, UpdateUserDepartmentMappingArgs>().ReverseMap();
            CreateMap<UserDepartmentMapping, UserDepartmentMappingAPI_DTO>().ReverseMap();
            CreateMap<UserDepartmentMappingViewModel, UserDepartmentMappingAPI_DTO>().ReverseMap();
            CreateMap<UserDepartmentMapping, UserDepartmentMappingViewModel>()
                .ForMember(src => src.UserJobGradeGrade, opt => opt.MapFrom(x => x.Department.JobGrade.Grade))
                .ForMember(src => src.StartDate, opt => opt.MapFrom(x => x.User.StartDate))
                .ReverseMap();
            #endregion
            // setting c&b reason
            CreateMap<MasterData, ReasonViewModelForCABSetting>();
            CreateMap<MassLocationViewModel, MasterData>()
                .ForMember(x => x.MetaDataTypeId, opt => opt.MapFrom(x => x.TypeId))
                .ForMember(x => x.Code, opt => opt.MapFrom(x => x.Value));
            CreateMap<ReasonViewModelForAddOfCABSetting, MasterData>();
            CreateMap<ReasonViewModelForCABSetting, MasterData>().ForMember(x => x.Id, opt => opt.Ignore());
            CreateMap<ReasonViewModelForUpdateOfCABSetting, MasterData>().ForMember(x => x.Id, opt => opt.Ignore());
            CreateMap<HolidaySchedule, HolidayScheduleViewModel>().ReverseMap();
            CreateMap<HolidaySchedule, HolidayScheduleArgs>().ReverseMap();
            CreateMap<MissingTimeClock, MissingTimelockViewModel>()
                .ReverseMap();
            CreateMap<MissingTimeClock, MissingTimelockArgs>().ReverseMap();
            CreateMap<LeaveApplication, LeaveApplicationViewModel>()
                .ForMember(x => x.EmployeeCode, opt => opt.MapFrom(y => y.UserSAPCode))
                .ForMember(x => x.UserNameCreated, opt => opt.MapFrom(y => y.CreatedByFullName))
                .ReverseMap();
            CreateMap<LeaveApplicationForCreatingArgs, LeaveApplication>()
                .ForMember(source => source.LeaveApplicationDetails, opt => opt.MapFrom(x => JsonConvert.DeserializeObject<List<LeaveApplicationDetail>>(x.LeaveApplicantDetails))).ReverseMap();
            CreateMap<LeaveApplicationDetail, LeaveApplicantDetailDTO>().ReverseMap();
            CreateMap<MissingTimelockGridViewModel, MissingTimeClock>();
            CreateMap<MissingTimeClock, MissingTimelockGridViewModel>();

            #region HeadCount Setting
            CreateMap<Department, ItemListDepartmentForHeadCountViewModel>()
                .ForMember(x => x.JobGradeValue, opt => opt.MapFrom(y => y.JobGrade.Grade))
                .ReverseMap();
            CreateMap<HeadCountArgs, HeadCount>().ReverseMap();
            CreateMap<HeadCount, ItemListHeadCountViewModel>()
                .ForMember(x => x.JobGradeValue, opt => opt.MapFrom(y => y.JobGradeForHeadCount.Grade))
                .ForMember(x => x.JobGradeCaption, opt => opt.MapFrom(y => y.JobGradeForHeadCount.Caption))
                .ForMember(x => x.JobGradeTitle, opt => opt.MapFrom(y => y.JobGradeForHeadCount.Title))
                .ReverseMap();
            CreateMap<JobGradePairItem, JobGrade>().ReverseMap();
            #endregion
            #region acting
            CreateMap<Acting, ActingArgs>().ReverseMap();
            CreateMap<Period, PeriodArgs>().ReverseMap();
            CreateMap<ActingRequestViewModel, Acting>();
            CreateMap<Acting, ActingRequestViewModel>()
                .ForMember(x => x.UserSAPCode, opt => opt.MapFrom(y => y.User.SAPCode))
                .ForMember(x => x.UserFullName, opt => opt.MapFrom(y => y.User.FullName))
                .ForMember(src => src.Mapping, opt => opt.MapFrom(x => x.User.UserDepartmentMappings.FirstOrDefault(y => y.IsHeadCount)));
            CreateMap<Acting, ActingViewModel>().ReverseMap();
            CreateMap<Acting, ActingPrintFormViewModel>()
                 .ForMember(x => x.UserSAPCode, opt => opt.MapFrom(y => y.User.SAPCode))
                .ForMember(x => x.FullName, opt => opt.MapFrom(y => y.User.FullName))
                .ForMember(x => x.Appraiser2FullName, opt => opt.MapFrom(y => y.SecondAppraiser.FullName))
                .ForMember(x => x.Appraiser1FullName, opt => opt.MapFrom(y => y.FirstAppraiser.FullName))
                .ForMember(x => x.CurrentTitle, opt => opt.MapFrom(y => y.CurrentPosition))
                .ForMember(x => x.DeptLineName, opt => opt.MapFrom(y => y.Department.Name))
                .ForMember(x => x.TitleInActingPeriod, opt => opt.MapFrom(y => y.TitleInActingPeriodName))
                .ForMember(x => x.WorkLocation, opt => opt.MapFrom(y => y.WorkLocationName))
                .ForMember(x => x.FirstAppraiserNote, opt => opt.MapFrom(y => y.FirstAppraiserNote))
                .ForMember(x => x.SecondAppraiserNote, opt => opt.MapFrom(y => y.SecondAppraiserNote))
                .ReverseMap();
            CreateMap<Acting, ActingArgs>().ReverseMap();
            CreateMap<Period, PeriodViewModel>().ReverseMap();
            CreateMap<Acting, ActingExportViewModel>().ReverseMap();
            #endregion

            #region Overtime Application
            CreateMap<OvertimeApplicationDetail, OvertimeApplicationDetailArgs>();
            CreateMap<OvertimeApplicationDetailArgs, OvertimeApplicationDetail>()
                .ForMember(x => x.Date, opt => opt.MapFrom(y => DateTime.ParseExact(y.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture)));
            CreateMap<ItemListOvertimeApplicationViewModel, OvertimeApplication>();
            CreateMap<OvertimeApplication, ItemListOvertimeApplicationViewModel>().ReverseMap();
            CreateMap<OvertimeApplication, OvertimeApplicationViewModel>().ReverseMap();
            CreateMap<OvertimeApplicationDetail, OvertimeApplicationDetailViewModel>().ReverseMap();
            CreateMap<OvertimeApplication, OvertimeApplicationArgs>().ReverseMap();
            CreateMap<OvertimeApplicationDetail, OvertimeApplicationDetaiForPrintlViewModel>().ReverseMap();
            CreateMap<OvertimeApplication, OvertimePrintFormViewModel>()
                .ForMember(x => x.Department, opt => opt.MapFrom(y => string.IsNullOrEmpty(y.DivisionName) ? y.DeptName : y.DivisionName))
                .ForMember(x => x.DepartmentCode, opt => opt.MapFrom(y => string.IsNullOrEmpty(y.DivisionName) ? y.DeptCode : y.DivisionCode))
                .ForMember(x => x.Location, opt => opt.MapFrom(y => y.WorkLocationName))
                .ForMember(x => x.Reason, opt => opt.MapFrom(y => y.ReasonName))
                .ForMember(x => x.PreparedBy, opt => opt.MapFrom(y => y.CreatedByFullName))
                .ForMember(x => x.DeptLine, opt => opt.MapFrom(y => y.DeptName))
                .ReverseMap();

            #endregion

            #region JobGrade
            CreateMap<JobGrade, JobGradeViewModel>().ReverseMap();
            CreateMap<JobGradeArgs, JobGrade>().ForMember(src => src.Id, opt => opt.Ignore());
            CreateMap<JobGrade, JobGradeArgs>();

            CreateMap<JobGradeItemRecruitmentMapping, JobGradeItemRecruitmentMappingForAddOrUpdateItemViewModel>()
                .ForMember(des => des.ItemRecruitmentId, opt => opt.MapFrom(src => src.ItemListRecruitmentId))
                .ForMember(des => des.ItemCode, opt => opt.MapFrom(src => src.ItemListRecruitment.Code))
                .ForMember(des => des.ItemName, opt => opt.MapFrom(src => src.ItemListRecruitment.Name))
                .ForMember(des => des.ItemUnit, opt => opt.MapFrom(src => src.ItemListRecruitment.Unit));

            CreateMap<ItemListRecruitment, ItemListRecruitmentForDropDownOfJobGradeViewModel>();
            CreateMap<JobGradeItemRecruitmentMapping, JobGradeItemRecruitmentMappingInHandoverViewModel>()
                .ForMember(des => des.Id, opt => opt.MapFrom(src => src.ItemListRecruitmentId))
                .ForMember(des => des.Code, opt => opt.MapFrom(src => src.ItemListRecruitment.Code))
                .ForMember(des => des.Name, opt => opt.MapFrom(src => src.ItemListRecruitment.Name))
                .ForMember(des => des.Unit, opt => opt.MapFrom(src => src.ItemListRecruitment.Unit));
            #endregion

            #region Shift Exchange
            CreateMap<ShiftExchangeRequestViewModel, ShiftExchangeApplication>();
            CreateMap<ShiftExchangeApplication, ShiftExchangeRequestViewModel>()
                .ForMember(x => x.UserSAPCode, opt => opt.MapFrom(y => y.UserCreatedBy.SAPCode))
                .ForMember(x => x.UserFullName, opt => opt.MapFrom(y => y.UserCreatedBy.FullName))
                .ForMember(src => src.DeptName, opt => opt.MapFrom(x => x.DeptLine.Name))
                .ForMember(src => src.DivisionName, opt => opt.MapFrom(x => x.DeptDivision.Name)).ReverseMap();
            //.ForMember(x => x.UserDeptName, opt => opt.MapFrom(y => y.CreatedByUser.d))
            CreateMap<ShiftExchangeApplication, ShiftExchangeViewByReferenceNumberViewModel>()
                 .ForMember(x => x.SAPCode, opt => opt.MapFrom(y => y.UserCreatedBy.SAPCode))
                .ForMember(x => x.FullName, opt => opt.MapFrom(y => y.UserCreatedBy.FullName))
                .ReverseMap();
            CreateMap<ShiftExchangeApplication, ShifExchangeForAddOrUpdateViewModel>()
                 .ForMember(x => x.Id, opt => opt.MapFrom(y => y.Id));
            CreateMap<ShifExchangeForAddOrUpdateViewModel, ShiftExchangeApplication>()
                .ForMember(src => src.Id, opt => opt.Ignore()).ReverseMap();
            CreateMap<ShiftExchangeApplicationDetail, ShiftExchangeDetailForAddOrUpdateViewModel>()
                 .ForMember(x => x.EmployeeCode, opt => opt.MapFrom(y => y.User.SAPCode));
            CreateMap<ShiftExchangeApplicationDetail, ShiftExchangeDetailSimpleViewModel>()
                 .ForMember(x => x.Id, opt => opt.MapFrom(y => y.Id))
                 .ForMember(x => x.UserId, opt => opt.MapFrom(y => y.User.Id))
                 .ForMember(x => x.FullName, opt => opt.MapFrom(y => y.User.FullName))
                .ForMember(x => x.SAPCode, opt => opt.MapFrom(y => y.User.SAPCode))
                 .ForMember(x => x.EmployeeCode, opt => opt.MapFrom(y => y.User.SAPCode))
                .ReverseMap();
            CreateMap<ShiftExchangeDetailForAddOrUpdateViewModel, ShiftExchangeApplicationDetail>()
                .ForMember(src => src.Id, opt => opt.Ignore());
            CreateMap<ShiftExchangeApplication, ShiftExchangeListViewModel>()
                .ForMember(x => x.Status, opt => opt.MapFrom(y => y.Status))
                .ForMember(x => x.ReferenceNumber, opt => opt.MapFrom(y => y.ReferenceNumber))
                .ForMember(x => x.DeptCode, opt => opt.MapFrom(y => y.DeptLine.Code))
                .ForMember(x => x.DeptName, opt => opt.MapFrom(y => y.DeptLine.Name))
                .ForMember(x => x.DivisionCode, opt => opt.MapFrom(y => y.DeptDivision.Code))
                .ForMember(x => x.DivisionName, opt => opt.MapFrom(y => y.DeptDivision.Name))
                .ForMember(x => x.UserFullName, opt => opt.MapFrom(y => y.CreatedByFullName))
                .ForMember(x => x.UserSAPCode, opt => opt.MapFrom(y => y.UserCreatedBy.SAPCode))
                .ReverseMap();
            #endregion
            #region MasterData
            CreateMap<MasterData, MasterDataViewModel>();
            CreateMap<TrackingRequest, ResponseExternalDataMappingDTO>().ReverseMap();
            #endregion

            # region tracking request
            CreateMap<TrackingRequest, TrackingRequestForGetListViewModel>();
            CreateMap<TrackingLogInitData, TrackingLogInitDatasViewModel>();
            #endregion
            #region Promote And Transfer
            CreateMap<PromoteAndTransfer, PromoteAndTransferDataForCreatingArgs>().ReverseMap();
            CreateMap<PromoteAndTransferViewModel, PromoteAndTransfer>().ReverseMap();
            CreateMap<PromoteAndTransfer, PromoteTransferPrintFormViewModel>()
                .ForMember(x => x.UserSAPCode, opt => opt.MapFrom(x => x.User.SAPCode))
                .ForMember(x => x.CurrentPositionName, opt => opt.MapFrom(x => x.CurrentTitle))
                .ForMember(x => x.CurrentDepartmentName, opt => opt.MapFrom(x => x.CurrentDepartment))
                .ForMember(x => x.CurrentJobGradeName, opt => opt.MapFrom(x => x.CurrentJobGrade))
                .ForMember(x => x.CurrentWorkLocationName, opt => opt.MapFrom(x => x.CurrentWorkLocation))
                .ForMember(x => x.PersonnelArea, opt => opt.MapFrom(x => x.PersonnelArea))
                .ForMember(x => x.PersonnelAreaText, opt => opt.MapFrom(x => x.PersonnelAreaText))
                .ForMember(x => x.EmployeeGroup, opt => opt.MapFrom(x => x.EmployeeGroup))
                .ForMember(x => x.EmployeeGroupDescription, opt => opt.MapFrom(x => x.EmployeeGroupDescription))
                .ForMember(x => x.EmployeeSubgroup, opt => opt.MapFrom(x => x.EmployeeSubgroup))
                .ForMember(x => x.EmployeeSubgroupDescription, opt => opt.MapFrom(x => x.EmployeeSubgroupDescription))
                .ForMember(x => x.PayScaleArea, opt => opt.MapFrom(x => x.PayScaleArea))
                .ForMember(x => x.NewPositionName, opt => opt.MapFrom(x => x.PositionName))
                .ForMember(x => x.NewDepartmentName, opt => opt.MapFrom(x => x.NewDeptOrLine.Name))
                .ForMember(x => x.NewJobGradeName, opt => opt.MapFrom(x => x.NewJobGradeName))
                .ForMember(x => x.NewWorkLocationName, opt => opt.MapFrom(x => x.NewWorkLocationName))
                .ForMember(x => x.ReportToUser, opt => opt.MapFrom(x => x.ReportTo.FullName))
                .ForMember(x => x.EffectiveDate, opt => opt.MapFrom(x => x.EffectiveDate.LocalDateTime.ToString("dd/MM/yyyy")))
                .ForMember(x => x.RequestFrom, opt => opt.MapFrom(x => x.RequestFrom == "MNG" ? "Department Recommendation" : "Employee Volunteer"))
                .ForMember(x => x.RequestFromDes, opt => opt.MapFrom(x => x.RequestFrom == "MNG" ? @"Phòng ban đề xuất" : @"Nhân viên tự đề cử"))
                ;
            CreateMap<PromoteAndTransfer, PromoteAndTransferViewModel>()
                .ForMember(x => x.UserRole, opt => opt.MapFrom(x => x.User.Role))
                .ForMember(x => x.SAPCode, opt => opt.MapFrom(x => x.User.SAPCode));
            #endregion

            #region Workflow

            CreateMap<WorkflowStep, WorkflowStepViewModel>().ReverseMap();
            CreateMap<StepCondition, StepConditionViewModel>().ReverseMap();
            CreateMap<WorkflowData, WorkflowDataViewModel>().ReverseMap();
            CreateMap<WorkflowCondition, WorkflowConditionViewModel>().ReverseMap();
            CreateMap<WorkflowTemplate, WorkflowTemplateListViewModel>();
            CreateMap<WorkflowTemplate, WorkflowTemplateViewModel>().ReverseMap();
            CreateMap<WorkflowTemplateViewModel, WorkflowTemplate>().ReverseMap();
            CreateMap<WorkflowHistory, WorkflowHistoryViewModel>();
            CreateMap<WorkflowInstance, WorkflowInstanceViewModel>();
            CreateMap<WorkflowInstance, WorkflowInstanceListViewModel>();
            CreateMap<WorkflowEntity, MyItemViewModel>();
            CreateMap<WorkflowTask, WorkflowTaskViewModel>();
            #endregion

            #region attachmentFile
            CreateMap<AttachmentFile, AttachmentFileViewModel>().ReverseMap();
            #endregion

            #region SAP
            CreateMap<EmployeeResponsSearchingViewModel, EmployeeViewModel>()
                .ForMember(x => x.StartDate, opt => opt.MapFrom(y => DateTime.ParseExact(y.JoiningDate,
                                  "yyyyMMdd", CultureInfo.InvariantCulture))).ReverseMap();

            CreateMap<Applicant, EmployeeInfo>()
                .ForMember(dest => dest.EdocId, opt => opt.MapFrom(source => source.Id))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(source => source.FullName.Contains(" ") ? source.FullName.Substring(0, source.FullName.IndexOf(" ")) : source.FullName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(source => source.FullName.Contains(" ") ? source.FullName.Substring(source.FullName.IndexOf(" ") + 1) : ""))
                .ForMember(dest => dest.WorkLocation, opt => opt.MapFrom(source => source.Position.LocationCode))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(source => source.DateOfBirth.HasValue ? source.DateOfBirth.Value.DateTime.ToSAPFormat() : DateTime.Now.ToSAPFormat()))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(source => source.GenderCode))
                .ForMember(dest => dest.PlaceOfBirth, opt => opt.MapFrom(source => source.BirthPlaceCode))
                .ForMember(dest => dest.CityCode, opt => opt.MapFrom(source => source.NativeCode))
                .ForMember(dest => dest.EmailAdress, opt => opt.MapFrom(source => source.Email))
                .ForMember(dest => dest.ActionType, opt => opt.MapFrom(source => "Z1"))
                .ForMember(dest => dest.Nationality, opt => opt.MapFrom(source => source.NationalityCode))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(source => source.StartDate.HasValue ? source.StartDate.Value.LocalDateTime.ToSAPFormat() : DateTime.Now.ToSAPFormat()));

            CreateMap<ApplicantRelativeInAeon, FamilyMemberInfo>()
                .ForMember(dest => dest.FamilyMemberRelationShip, opt => opt.MapFrom(source => source.RelationCode))
                .ForMember(dest => dest.FamilyMemberFirstName, opt => opt.MapFrom(source => source.FullName.Contains(" ") ? source.FullName.Substring(0, source.FullName.IndexOf(" ")) : source.FullName))
                .ForMember(dest => dest.FamilyMemberLastName, opt => opt.MapFrom(source => source.FullName.Contains(" ") ? source.FullName.Substring(source.FullName.IndexOf(" ") + 1) : ""))
                .ForMember(dest => dest.FamilyMemberGender, opt => opt.MapFrom(source => ""));
            CreateMap<ApplicantEducation, EducationInfo>()
                .ForMember(dest => dest.EducationStartDate, opt => opt.MapFrom(source => source.FromDate.HasValue ? source.FromDate.Value.DateTime.ToLocalTime().ToSAPFormat() : ""))
                .ForMember(dest => dest.EducationEndDate, opt => opt.MapFrom(source => source.ToDate.HasValue ? source.ToDate.Value.DateTime.ToLocalTime().ToSAPFormat() : ""))
                .ForMember(dest => dest.EducationCertificate, opt => opt.MapFrom(source => source.Major));
            CreateMap<LeaveApplication, LeaveApplicationInfo>()
                .ForMember(dest => dest.EmployeeCode, opt => opt.MapFrom(source => source.UserSAPCode))
                .ForMember(dest => dest.UserEdoc, opt => opt.MapFrom(source => source.CreatedBy));

            CreateMap<Acting, ActingInfo>()
                .ForMember(dest => dest.EmployeeCode, opt => opt.MapFrom(source => source.User.SAPCode))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(source => "Y6"))
                //.ForMember(dest => dest.Position, opt => opt.MapFrom(source => source.TitleInActingPeriodCode))
                .ForMember(dest => dest.Reason, opt => opt.MapFrom(source => "87"))
                .ForMember(dest => dest.EmployeeGroupCode, opt => opt.MapFrom(source => ""))
                .ForMember(dest => dest.EmployeeSubGroupCode, opt => opt.MapFrom(source => ""))
                .ForMember(dest => dest.RequestFrom, opt => opt.MapFrom(source => source.User.CreatedBy))
                .ForMember(dest => dest.PersonelSubarea, opt => opt.MapFrom(source => source.WorkLocationCode));

            CreateMap<PromoteAndTransfer, PromoteAndTransferInfo>()
                .ForMember(dest => dest.EmployeeCode, opt => opt.MapFrom(source => source.User.SAPCode))
                .ForMember(dest => dest.EffectiveDate, opt => opt.MapFrom(source => source.EffectiveDate.LocalDateTime.ToSAPFormat()))
                .ForMember(dest => dest.NewTitle, opt => opt.MapFrom(source => ""))
                .ForMember(dest => dest.RequestFrom, opt => opt.MapFrom(source => source.User.CreatedBy))
                .ForMember(dest => dest.Position, opt => opt.MapFrom(source => source.NewTitleCode))
                .ForMember(dest => dest.PersonelSubarea, opt => opt.MapFrom(source => source.NewWorkLocationCode));

            CreateMap<MissingTimeClock, MissingTimeClockInfo>()
                 .ForMember(dest => dest.EmployeeCode, opt => opt.MapFrom(source => source.UserSAPCode))
                 .ForMember(dest => dest.RequestFrom, opt => opt.MapFrom(source => source.CreatedByFullName));
            CreateMap<OvertimeApplicationDetail, OvertimeInfo>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(source => source.Date.LocalDateTime.ToSAPFormat()))
                .ForMember(dest => dest.ActualHoursFrom, opt => opt.MapFrom(source => DateTime.Parse(source.ActualHoursFrom).ToString("HHmmss")))
                .ForMember(dest => dest.ActualHoursTo, opt => opt.MapFrom(source => DateTime.Parse(source.ActualHoursTo).ToString("HHmmss")));


            CreateMap<ResignationApplication, ResignationDataInfo>()
                 .ForMember(dest => dest.EmployeeCode, opt => opt.MapFrom(source => source.UserSAPCode))
                 .ForMember(dest => dest.Reason, opt => opt.MapFrom(source => source.ReasonForActionCode))
                 .ForMember(dest => dest.ActionType, opt => opt.MapFrom(source => "Z9"))
                 .ForMember(dest => dest.RequestFrom, opt => opt.MapFrom(source => source.CreatedByFullName));

            CreateMap<Acting, AdditionalItem>()
                   .ForMember(dest => dest.UserName, opt => opt.MapFrom(source => source.User.FullName));
            CreateMap<LeaveApplication, AdditionalItem>()
                   .ForMember(dest => dest.UserName, opt => opt.MapFrom(source => source.CreatedByFullName));
            CreateMap<PromoteAndTransfer, AdditionalItem>()
                   .ForMember(dest => dest.UserName, opt => opt.MapFrom(source => source.FullName))
                   .ForMember(dest => dest.DeptCode, opt => opt.MapFrom(source => source.NewDeptOrLineCode))
                   .ForMember(dest => dest.DeptName, opt => opt.MapFrom(source => source.NewDeptOrLineName));
            CreateMap<Applicant, AdditionalItem>()
                   .ForMember(dest => dest.UserName, opt => opt.MapFrom(source => source.FullName));

            CreateMap<MissingTimeClock, AdditionalItem>()
                 .ForMember(dest => dest.UserName, opt => opt.MapFrom(source => source.CreatedByFullName));
            CreateMap<OvertimeApplication, AdditionalItem>()
                 .ForMember(dest => dest.UserName, opt => opt.MapFrom(source => source.CreatedByFullName));
            CreateMap<ResignationApplication, AdditionalItem>()
                 .ForMember(dest => dest.UserName, opt => opt.MapFrom(source => source.CreatedByFullName));
            CreateMap<ShiftExchangeApplication, AdditionalItem>()
                 .ForMember(dest => dest.UserName, opt => opt.MapFrom(source => source.CreatedByFullName))
                 .ForMember(dest => dest.DeptCode, opt => opt.MapFrom(source => source.DeptLine.Code))
                   .ForMember(dest => dest.DeptName, opt => opt.MapFrom(source => source.DeptLine.Name))
                   .ForMember(dest => dest.DivisionCode, opt => opt.MapFrom(source => source.DeptDivision.Code))
                   .ForMember(dest => dest.DivisionName, opt => opt.MapFrom(source => source.DeptDivision.Name));
            #endregion           

            CreateMap<EmailTemplate, EmailTemplateViewModel>();

            CreateMap<LeaveApplicationDetail, ExportLeaveApplicationViewModel>().ReverseMap();

            #region CR-TargetPlan
            CreateMap<TargetPlan, PendingTargetPlanViewModel>()
                .ReverseMap();
            CreateMap<PendingTargetPlan, PendingTargetPlanViewModel>()
              .ReverseMap();
            CreateMap<TargetPlanDetail, PendingTargetPlanDetailViewModel>().ReverseMap();
            CreateMap<PendingTargetPlanDetail, PendingTargetPlanDetailViewModel>().ReverseMap();
            CreateMap<PendingTargetPlanDetail, PendingTargetPlanDetailViewModel>()
                .ForMember(dest => dest.PeriodName, opt => opt.MapFrom(source => source.PendingTargetPlan.PeriodName)).ReverseMap();
            CreateMap<TargetPlanPeriod, TargetPlanPeriodViewModel>().ReverseMap();
            CreateMap<TargetPlan, TargetPlanArg>().ReverseMap();
            CreateMap<TargetPlanArg, PendingTargetPlan>().ReverseMap();
            CreateMap<TargetPlanDetail, TargetPlanArgDetail>()
                .ForMember(dest => dest.TargetPlanId, opt => opt.MapFrom(source => source.TargetPlanId))
                .ForMember(dest => dest.ReferenceNumber, opt => opt.MapFrom(source => source.TargetPlan.ReferenceNumber))
                .ReverseMap();
            CreateMap<PendingTargetPlanDetail, TargetPlanArgDetail>().ReverseMap();
            CreateMap<PendingTargetPlanDetail, TargetPlanDetail>().ReverseMap();
            CreateMap<TargetPlan, TargetPlanViewModel>()
                   .ForMember(dest => dest.TargetPlanDetails, opt => opt.Ignore()).ReverseMap();
            CreateMap<TargetPlanDetail, TargetPlanDetailViewModel>()
                 .ForMember(dest => dest.ReferenceNumber, opt => opt.MapFrom(source => source.TargetPlan.ReferenceNumber))
                 .ForMember(dest => dest.DeptLine, opt => opt.MapFrom(source => source.TargetPlan.DeptName))
                 .ForMember(dest => dest.DivisionGroup, opt => opt.MapFrom(source => source.TargetPlan.DivisionName))
                 .ForMember(dest => dest.SubmitterSAPCode, opt => opt.MapFrom(source => source.TargetPlan.UserSAPCode))
                 .ForMember(dest => dest.SubmitterFullName, opt => opt.MapFrom(source => source.TargetPlan.UserFullName))
                 .ForMember(dest => dest.Status, opt => opt.MapFrom(source => source.TargetPlan.Status))
                 .ForMember(dest => dest.Period, opt => opt.MapFrom(source => source.TargetPlan.PeriodName))
                 .ForMember(dest => dest.PeriodFrom, opt => opt.MapFrom(source => source.TargetPlan.PeriodFromDate))
                 .ForMember(dest => dest.PeriodTo, opt => opt.MapFrom(source => source.TargetPlan.PeriodToDate))
                 .ForMember(dest => dest.CreatedById, opt => opt.MapFrom(source => source.TargetPlan.CreatedById))
                .ReverseMap();
            CreateMap<TargetPlan, AdditionalItem>()
                   .ForMember(dest => dest.UserName, opt => opt.MapFrom(source => source.CreatedByFullName));
            CreateMap<DateValueItem, DateValueArgs>()
                 .ForMember(dest => dest.Date, opt => opt.MapFrom(source => source.Zdate))
                  .ForMember(dest => dest.Value, opt => opt.MapFrom(source => source.Tprog))
                .ReverseMap();
            CreateMap<TargetPlanArgDetail, TargetPlanDetailPrintFormViewModel>().ReverseMap();

            #endregion
            #region Airline
            CreateMap<Airline, AirlineArg>()
               .ForMember(dest => dest.Id, opt => opt.Ignore()).ReverseMap();
            CreateMap<Airline, AirlineViewModel>().ReverseMap();
            #endregion
            #region Reason
            CreateMap<BTAReason, ReasonArg>()
               .ForMember(dest => dest.Id, opt => opt.Ignore()).ReverseMap();
            CreateMap<BTAReason, BTAReasonViewModel>().ReverseMap();
            #endregion
            #region Partition
            CreateMap<Partition, PartitionViewModel>().ReverseMap();
            CreateMap<Partition, PartitionArgs>().ReverseMap();
            #endregion
            #region Global Location
            CreateMap<GlobalLocation, GlobalLocationViewModels>().ReverseMap();
            CreateMap<GlobalLocation, GlobalLocationArgs>().ReverseMap();
            #endregion

            #region Flight Number
            CreateMap<FlightNumber, FlightNumberArg>()
               .ForMember(dest => dest.Id, opt => opt.Ignore()).ReverseMap();
            CreateMap<FlightNumber, FlightNumberViewModel>().ReverseMap();
            #endregion
            #region BTA           

            CreateMap<BTAViewModel, BusinessTripDTO>().ReverseMap();
            CreateMap<BusinessTripApplicationDetail, BTADetailViewModel>().ReverseMap();
            CreateMap<BusinessTripApplication, BusinessTripDTO>().ReverseMap();
            CreateMap<BTAViewModel, BusinessTripItemViewModel>().ReverseMap();
            CreateMap<BusinessTripDetailDTO, BusinessTripApplicationDetail>().ReverseMap();
            //tudm
            CreateMap<BusinessTripDetail_API_DTO, BusinessTripApplicationDetail>().ReverseMap();
            CreateMap<BusinessTripDetail_API_DTO, BusinessTripDetailDTO>().ReverseMap();
            CreateMap<BTAViewModel, BusinessTripItemViewModel_API>().ReverseMap();
            CreateMap<ChangeCancelBusinessTripDTO, ChangeCancelBusinessTripDetail>().ReverseMap();
            CreateMap<BusinessTripApplicationDetail, BtaDetailViewModel>()
                .ForMember(x => x.ReferenceNumber, opt => opt.MapFrom(y => y.BusinessTripApplication.ReferenceNumber))
                .ForMember(x => x.StatusItem, opt => opt.MapFrom(y => y.BusinessTripApplication.Status))
                .ForMember(x => x.CreatedByFullName, opt => opt.MapFrom(y => y.BusinessTripApplication.CreatedByFullName))
                .ForMember(x => x.CreatedBySapCode, opt => opt.MapFrom(y => y.BusinessTripApplication.UserSAPCode))
                .ForMember(x => x.DeptCode, opt => opt.MapFrom(y => y.BusinessTripApplication.DeptCode))
                .ForMember(x => x.DeptName, opt => opt.MapFrom(y => y.BusinessTripApplication.DeptName))
                .ForMember(x => x.DivisionCode, opt => opt.MapFrom(y => y.BusinessTripApplication.DeptDivisionCode))
                .ForMember(x => x.DivisionName, opt => opt.MapFrom(y => y.BusinessTripApplication.DeptDivisionName))
                .ForMember(x => x.CreatedById, opt => opt.MapFrom(y => y.BusinessTripApplication.CreatedById))
                .ForMember(x => x.UserGradeTitle, opt => opt.MapFrom(y => y.UserGrade != null ? y.UserGrade.Title : ""))
                .ReverseMap();
            CreateMap<BusinessTripApplication, BTAViewModel>()
                 .ForMember(x => x.EmployeeCode, opt => opt.MapFrom(y => y.UserSAPCode))
                 .ReverseMap();
            CreateMap<FlightDetail, FlightDetailViewModel>().ReverseMap();
            //
            CreateMap<FlightDetail, CommonViewModel.BusinessTripApplication.FareRuleViewModel>()
                .ForMember(x => x.Title, opt => opt.MapFrom(y => string.IsNullOrEmpty(y.TitleDepartureFareRule) ? y.TitleReturnFareRule : y.TitleDepartureFareRule))
                .ForMember(x => x.Detail, opt => opt.MapFrom(y => string.IsNullOrEmpty(y.DetailDepartureFareRule) ? y.DetailReturnFareRule : y.DetailDepartureFareRule))
                .ReverseMap();


            CreateMap<ChangeCancelBusinessTripDetail, ChangeCancelBusinessTripDetailViewModel>().ReverseMap();
            CreateMap<ChangeCancelBusinessTripDetail, ChangeCancelBTADetailViewModel>().ReverseMap();
            CreateMap<RoomOrganization, RoomOrganizationDTO>().ReverseMap();
            CreateMap<RoomUserMapping, SimpleUserDTO>()
                 .ForMember(x => x.UserId, opt => opt.MapFrom(y => y.UserId))
                 .ForMember(x => x.RoomOrganizationId, opt => opt.MapFrom(y => y.RoomOrganizationId))
                 .ForMember(x => x.FullName, opt => opt.MapFrom(y => y.User.FullName))
                 .ForMember(x => x.SAPCode, opt => opt.MapFrom(y => y.User.SAPCode))
                 .ForMember(x => x.BusinessTripApplicationDetailId, opt => opt.MapFrom(y => y.BusinessTripApplicationDetailId))
                 .ForMember(x => x.HotelCode, opt => opt.MapFrom(y => y.BusinessTripApplicationDetail.HotelCode))
                 .ForMember(x => x.HotelName, opt => opt.MapFrom(y => y.BusinessTripApplicationDetail.HotelName))
                .ReverseMap();
            //BTA Policy
            CreateMap<BTAPolicy, BTAPolicyViewModel>().ReverseMap();
            CreateMap<BTAPolicy, BTAPolicyArgs>().ReverseMap();
            //BTA Policy Special
            CreateMap<BTAPolicySpecial, BTAPolicySpecialViewModel>().ReverseMap();
            CreateMap<BTAPolicySpecial, BTAPolicySpecialArgs>().ReverseMap();
            //Booking flight
            CreateMap<BookingFlight, BookingFlightViewModel>().ReverseMap();
            CreateMap<LogHistory, LogHistoryViewModel>().ReverseMap();
            CreateMap<BookingFlight, BookingFlightArg>().ReverseMap();
            CreateMap<FlightDetail, FlightDetailViewModel>().ReverseMap();

            CreateMap<BTALog, BTALogDTO>().ReverseMap();
            #endregion

            #region Over Budget
            CreateMap<BTAOverBudgetViewModel, BusinessOverBudgetDTO>().ReverseMap();
            CreateMap<BusinessOverBudgetDTO, BusinessTripOverBudget>().ReverseMap();
            CreateMap<BTAOverBudgetViewModel, BusinessTripOverBudget>().ReverseMap();
            CreateMap<BusinessTripOverBudgetsDetail, BTAOverBudgetDetailViewModel>().ReverseMap();
            CreateMap<BTAOverBudgetViewModel, BusinessTripOverBudgetItemViewModel>().ReverseMap();

            CreateMap<BusinessTripOverBudgetsDetail, BusinessOverBudgetDetailDTO>()
                .ForMember(x => x.UserJobgradeTitle, opt => opt.MapFrom(y => y.UserGrade != null ? y.UserGrade.Title : ""))
                .ReverseMap();

            CreateMap<BusinessTripOverBudgetsDetail, BusinessTripOverBudgetDetailViewModel>()
                .ForMember(x => x.ReferenceNumber, opt => opt.MapFrom(y => y.BusinessTripOverBudget.ReferenceNumber))
                .ForMember(x => x.StatusItem, opt => opt.MapFrom(y => y.BusinessTripOverBudget.Status))
                .ForMember(x => x.CreatedByFullName, opt => opt.MapFrom(y => y.BusinessTripOverBudget.CreatedByFullName))
                .ForMember(x => x.CreatedBySapCode, opt => opt.MapFrom(y => y.BusinessTripOverBudget.UserSAPCode))
                .ForMember(x => x.DeptCode, opt => opt.MapFrom(y => y.BusinessTripOverBudget.DeptCode))
                .ForMember(x => x.DeptName, opt => opt.MapFrom(y => y.BusinessTripOverBudget.DeptName))
                .ForMember(x => x.DivisionCode, opt => opt.MapFrom(y => y.BusinessTripOverBudget.DeptDivisionCode))
                .ForMember(x => x.DivisionName, opt => opt.MapFrom(y => y.BusinessTripOverBudget.DeptDivisionName))
                .ForMember(x => x.CreatedById, opt => opt.MapFrom(y => y.BusinessTripOverBudget.CreatedById))
                .ReverseMap();
            #endregion
            #region Room Type
            CreateMap<RoomType, RoomTypeViewModel>().ReverseMap();
            CreateMap<RoomType, RoomTypeArgs>().ReverseMap();
            #endregion
            #region Hotel
            CreateMap<Hotel, HotelViewModel>().ReverseMap();
            CreateMap<Hotel, HotelArgs>().ReverseMap();
            #endregion
            #region TripLocation
            CreateMap<BusinessTripLocation, BusinessTripLocationViewModel>().ReverseMap();
            CreateMap<BusinessTripLocation, BusinessTripLocationArgs>().ReverseMap();
            #endregion


            #region Salary Day Configuration
            //CreateMap<DaysConfiguration, SalaryDayConfigurationArg>()
            //    .ForMember(dest => dest.Id, opt => opt.Ignore()).ReverseMap();
            //CreateMap<DaysConfiguration, SalaryDayConfigurationViewModel>().ReverseMap();
            #endregion
            #region Submit Person
            CreateMap<UserSubmitPersonDeparmentMapping, UserSubmitPersonDepartmentMappingViewModel>()
                 .ForMember(dest => dest.SAPCode, opt => opt.MapFrom(source => source.User.SAPCode))
                .ReverseMap();
            #endregion

            #region Navigation
            CreateMap<Navigation, NavigationViewModel>()
                .ForMember(dest => dest.ProfilePicture, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.ProfilePicture.FileUniqueName) ? "/Attachments/" + src.ProfilePicture.FileUniqueName : string.Empty))
                .ForMember(x => x.ParentId, opt => opt.MapFrom(y => y.ParentId))
                .ReverseMap();
            CreateMap<Navigation, NavigationDataForCreatingArgs>().ReverseMap();
            CreateMap<NavigationDataForCreatingArgs, NavigationViewModel>()
                .ForMember(dest => dest.ProfilePictureId, opt => opt.Ignore()).ReverseMap();
            CreateMap<NavigationViewModel, NavigationDataForCreatingArgs>();
            CreateMap<User, NavigationListUserDepartmentViewModel.User>()
                .ForMember(x => x.id, opt => opt.MapFrom(y => y.Id))
                .ForMember(x => x.FullName, opt => opt.MapFrom(y => y.FullName))
                .ForMember(x => x.SAPCode, opt => opt.MapFrom(y => y.SAPCode))
                .ReverseMap();
            CreateMap<NavigationListUserDepartmentViewModel.User, User>()
                .ForMember(x => x.Id, opt => opt.MapFrom(y => y.id))
                .ForMember(x => x.FullName, opt => opt.MapFrom(y => y.FullName))
                .ForMember(x => x.SAPCode, opt => opt.MapFrom(y => y.SAPCode))
                .ReverseMap();
            CreateMap<Department, NavigationListUserDepartmentViewModel.Department>()
                .ForMember(x => x.id, opt => opt.MapFrom(y => y.Id))
                .ForMember(x => x.Name, opt => opt.MapFrom(y => y.Name))
                .ForMember(x => x.Code, opt => opt.MapFrom(y => y.Code))
                .ReverseMap();
            CreateMap<NavigationListUserDepartmentViewModel.Department, Department>()
                .ForMember(x => x.Id, opt => opt.MapFrom(y => y.id))
                .ForMember(x => x.Name, opt => opt.MapFrom(y => y.Name))
                .ForMember(x => x.Code, opt => opt.MapFrom(y => y.Code))
                .ReverseMap();
            #endregion

            #region Tracking History
            CreateMap<TrackingHistory, TrackingHistoryViewModel>().ReverseMap();
            CreateMap<User, CommonViewModel.LogHistories.UserLogViewModel>().ReverseMap();
            CreateMap<UserDepartmentMapping, CommonViewModel.LogHistories.UserDepartmentMappingViewModel>().ReverseMap();
            #endregion

            #region Tracking Import
            CreateMap<ImportTracking, ImportTrackingViewModel>().ReverseMap();
            #endregion

            #region Business Model
            CreateMap<BusinessModel, BusinessModelViewModel>().ReverseMap();
            CreateMap<BusinessModelArgs, BusinessModel>().ReverseMap();
            #endregion

            #region Business Model Unit Mapping
            CreateMap<BusinessModelUnitMapping, BusinessModelUnitMappingViewModel>().ReverseMap();
            CreateMap<BusinessModelUnitMappingArgs, BusinessModelUnitMapping>().ReverseMap();
            #endregion
            #region Shift Code
            CreateMap<ShiftCode, ShiftCodeViewModel>().ReverseMap();
            CreateMap<ShiftCode, ShiftCodeAgrs>().ReverseMap();
            #endregion
            CreateMap<TargetPlanSpecialDepartmentMapping, TargetPlanSpecialViewModel>().ReverseMap();

            CreateMap<BTAErrorMessage, BTAErrorMessageViewModel>()
                .ForMember(x => x.APIType, opt => opt.MapFrom(x => x.Type))
                .ReverseMap();
            CreateMap<BTAErrorMessage, BTAErrorMessageArgs>().ReverseMap();

            CreateMap<UserDepartmentSyncHistory, UserDepartmentSyncHistoryViewModel>().ReverseMap();
            CreateMap<UserSyncHistory, UserSyncHistoryViewModel>().ReverseMap();
            CreateMap<DepartmentSyncHistory, DepartmentSyncHistoryViewModel>().ReverseMap();

            #region Department, ITDepartment

            CreateMap<Department, ITDepartmentDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.IsEdoc1, opt => opt.Ignore());

            CreateMap<ITDepartmentDTO, ItemListDepartmentViewModel>()
                 .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                 .ForMember(dest => dest.IsEdoc1, opt => opt.MapFrom(src => src.IsEdoc1));
            #endregion
        }
    }
}