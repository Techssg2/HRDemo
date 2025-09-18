using Aeon.HR.Data.Migrations;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Interfaces;
using EFSecondLevelCache;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Aeon.HR.Data.Models.SyncLog;
using Z.EntityFramework.Plus;

namespace Aeon.HR.Data
{
    public class HRDbContext : DbContext
    {
        static HRDbContext()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<HRDbContext, Configuration>());
        }
        public HRDbContext() : base("HRDbContext")
        {
        }
        public DbSet<RecruitmentCategory> RecruitmentCategories { get; set; }
        public DbSet<JobGrade> JobGrades { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<PassengerInformation> PassengerInformations { get; set; }
        public DbSet<ApplicantStatusRecruitment> ApplicantStatusRecruitments { get; set; }
        public DbSet<AppreciationListRecruitment> AppreciationListRecruitments { get; set; }
        public DbSet<ItemListRecruitment> ItemListRecruitments { get; set; }
        public DbSet<WorkingTimeRecruitment> WorkingTimeRecruitments { get; set; }
        public DbSet<MissingTimeClock> MissingTimeClocks { get; set; }
        public DbSet<Position> Position { get; set; }
        public DbSet<ReferenceNumber> ReferenceNumbers { get; set; }
        public DbSet<RequestToHire> RequestToHires { get; set; }
        public DbSet<ResignationApplication> ResignationApplications { get; set; }
        public DbSet<HeadCount> HeadCounts { get; set; }
        public DbSet<AttachmentFile> AttachmentFiles { get; set; }
        public DbSet<Handover> Handovers { get; set; }
        public DbSet<PromoteAndTransfer> PromoteAndTransfers { get; set; }
        public DbSet<Period> Periods { get; set; }
        public DbSet<Acting> Actings { get; set; }
        public DbSet<LeaveApplication> LeaveApplications { get; set; }
        public DbSet<OvertimeApplicationDetail> OvertimeApplicationDetails { get; set; }
        public DbSet<OvertimeApplication> OvertimeApplications { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<UserDepartmentMapping> UserDepartmentMappings { get; set; }
        public DbSet<ShiftCode> ShiftCodes { get; set; }
        public DbSet<ShiftExchangeApplication> ShiftExchangeApplications { get; set; }
        public DbSet<ShiftExchangeApplicationDetail> ShiftExchangeApplicationDetails { get; set; }
        public DbSet<MasterData> MasterDatas { get; set; }
        public DbSet<MetadataType> MetadataTypes { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<CacheMasterData> CacheMasterDatas { get; set; }
        public DbSet<JobGradeItemRecruitmentMapping> JobGradeItemRecruitmentMappings { get; set; }
        public DbSet<WorkflowTemplate> WorkflowTemplates { get; set; }
        public DbSet<WorkflowInstance> WorkflowInstances { get; set; }
        public DbSet<WorkflowHistory> WorkflowHistories { get; set; }
        public DbSet<WorkflowTask> WorkflowTasks { get; set; }
        public DbSet<AddressInfomation> AddressInfomations { get; set; }
        public DbSet<TrackingRequest> TrackingRequests { get; set; }
        public DbSet<Education> Educations { get; set; }
        public DbSet<FamilyMember> FamilyMembers { get; set; }
        public DbSet<EmergencyContact> EmergencyContacts { get; set; }
        public DbSet<LeaveApplicationDetail> LeaveApplicationDetails { get; set; }
        public DbSet<Applicant> Applicants { get; set; }
        public DbSet<CostCenterRecruitment> CostCenterRecruitments { get; set; }
        public DbSet<TrackingLog> TrackingLogs { get; set; }
        public DbSet<WorkingAddressRecruitment> WorkingAddressRecruitments { get; set; }
        //public DbSet<TestDepartment> TestDepartments { get; set; }
        public DbSet<TargetPlanPeriod> TargetPlanPeriods { get; set; }
        public DbSet<HolidaySchedule> HolidaySchedules { get; set; }
        public DbSet<TargetPlan> TargetPlans { get; set; }
        public DbSet<TargetPlanDetail> TargetPlanDetails { get; set; }
        public DbSet<PendingTargetPlan> PendingTargetPlans { get; set; }
        public DbSet<PendingTargetPlanDetail> PendingTargetPlanDetails { get; set; }
        public DbSet<DaysConfiguration> DaysConfigurations { get; set; }
        public DbSet<UserSubmitPersonDeparmentMapping> userSubmitPersonDeparmentMappings { get; set; }
        public DbSet<TargetPlanSpecialDepartmentMapping> TargetPlanSpecialDepartmentMappings { get; set; }
        public DbSet<AuditEntry> AuditEntries { get; set; }
        public DbSet<AuditEntryProperty> AuditEntryProperties { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Airline> Airlines { get; set; }
        public DbSet<BusinessTripLocation> BusinessTripLocations { get; set; }
        public DbSet<FlightNumber> FlightNumbers { get; set; }        // BTA
        public DbSet<BusinessTripApplication> BusinessTripApplications { get; set; }
        public DbSet<BusinessTripApplicationDetail> BusinessTripApplicationDetails { get; set; }
        public DbSet<FlightDetail> FlightDetails { get; set; }
        public DbSet<BookingFlight> BookingFlight { get; set; }
        public DbSet<BTAReason> BTAReasons { get; set; }
        public DbSet<ChangeCancelBusinessTripDetail> ChangeCancelBusinessTripDetails { get; set; }
        public DbSet<RoomOrganization> RoomOrganizations { get; set; }
        public DbSet<RoomUserMapping> RoomUserMappings { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<MissingTimeClockDetail> MissingTimeClockDetails { get; set; }
        public DbSet<TrackingLogInitData> TrackingLogInitDatas { get; set; }
        public DbSet<Region> Region { get; set; }
        public DbSet<MaintainPromoteAndTranferPrint> MaintainPromoteAndTranferPrint { get; set; }
        public DbSet<BTAPolicy> BTAPolicy { get; set; }
        public DbSet<BTAPolicySpecial> BTAPolicySpecial { get; set; }
        public DbSet<BookingContract> BookingContract { get; set; }
        public DbSet<Navigation> Navigations { get; set; }
        public DbSet<BusinessTripOverBudget> BusinessTripOverBudgets { get; set; }
        public DbSet<BusinessTripOverBudgetsDetail> BusinessTripOverBudgetsDetails { get; set; }
        public DbSet<Partition> Partitions { get; set; }
        public DbSet<GlobalLocation> GlobalLocations { get; set; }
        public DbSet<EmailHistory> EmailHistories { get; set; }
        public DbSet<TrackingHistory> TrackingHistories { get; set; }
        public DbSet<ImportTracking> ImportTrackings { get; set; }
        public DbSet<BusinessModel> BusinessModels { get; set; }
        public DbSet<BusinessModelUnitMapping> BusinessModelUnitMappings { get; set; }
        public DbSet<BTALog> BTALogs { get; set; }
        public DbSet<CompanyPolicy> CompanyPolicys { get; set; }
        public DbSet<UserPasswordHistories> UserPasswordHistory { get; set; }
        public DbSet<ChangeUser> ChangeUsers { get; set; }
        public DbSet<TrackingAPIIntegrationLog> TrackingAPIIntegrationLogs { get; set; }
        public DbSet<BTAErrorMessage> BTAErrorMessages { get; set; }
        public DbSet<ItemWorkflowLog> ItemWorkflowLogs { get; set; }
        public DbSet<CanViewDepartment> CanViewDepartments { get; set; }
        #region Sync Log History
        public DbSet<UserSyncHistory> UserSyncHistories { get; set; }
        public DbSet<UserDepartmentSyncHistory> UserDepartmentSyncHistories { get; set; }
        public DbSet<DepartmentSyncHistory> DepartmentSyncHistories { get; set; }
        #endregion
        public DbSet<HRTrackingAPILog> HRTrackingAPILogs { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasMany(x => x.ShiftExchangeApplicationDetails)
                .WithRequired(X => X.User)
                .HasForeignKey(x => x.UserId)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<ShiftExchangeApplication>()
                .HasMany(x => x.ExchangingShiftItems)
                .WithOptional(x => x.ShiftExchangeApplication)
                .HasForeignKey(oi => oi.ShiftExchangeApplicationId)
                .WillCascadeOnDelete();
            modelBuilder.Entity<LeaveApplication>()
               .HasMany(x => x.LeaveApplicationDetails)
               .WithRequired(x => x.LeaveApplication)
               .HasForeignKey(oi => oi.LeaveApplicationId)
               .WillCascadeOnDelete();
            modelBuilder.Entity<MissingTimeClock>()
              .HasMany(x => x.MissingTimeClockDetails)
              .WithRequired(x => x.MissingTimeClock)
              .HasForeignKey(oi => oi.MissingTimeClockId)
              .WillCascadeOnDelete(false);
            modelBuilder.Entity<JobGrade>()
               .HasMany(x => x.HeadCounts)
               .WithRequired(x => x.JobGradeForHeadCount)
               .HasForeignKey(oi => oi.JobGradeForHeadCountId)
               .WillCascadeOnDelete(false);
            modelBuilder.Entity<Department>()
               .HasMany(x => x.Actings)
               .WithRequired(x => x.Department)
               .HasForeignKey(oi => oi.DepartmentId)
               .WillCascadeOnDelete(false);
            modelBuilder.Entity<Applicant>()
               .HasMany(x => x.Handovers)
               .WithRequired(x => x.Applicant)
               .HasForeignKey(oi => oi.ApplicantId)
               .WillCascadeOnDelete(false);
            modelBuilder.Entity<RecruitmentCategory>()
               .HasOptional(x => x.Parent)
               .WithMany(x => x.ChildCategories)
               .HasForeignKey(x => x.ParentId)
               .WillCascadeOnDelete(false);
            modelBuilder.Entity<TrackingRequest>()
             .HasMany(x => x.TrackingLogInitDatas)
             .WithOptional(x => x.TrackingRequest)
             .HasForeignKey(oi => oi.TrackingLogId)
             .WillCascadeOnDelete(false);
            modelBuilder.Entity<BusinessTripApplication>()
            .HasMany(x => x.BusinessTripApplicationDetails)
            .WithOptional(x => x.BusinessTripApplication)
            .HasForeignKey(oi => oi.BusinessTripApplicationId)
            .WillCascadeOnDelete(true);

            modelBuilder.Entity<User>()
            .HasMany(x => x.BusinessTripApplicationDetails)
            .WithRequired(x => x.User)
            .HasForeignKey(x => x.UserId)
            .WillCascadeOnDelete(false);
            modelBuilder.Entity<BusinessTripOverBudget>()
            .HasMany(x => x.BusinessTripOverBudgetDetails)
            .WithOptional(x => x.BusinessTripOverBudget)
            .HasForeignKey(oi => oi.BusinessTripOverBudgetId)
            .WillCascadeOnDelete(true);
        }
    }
}