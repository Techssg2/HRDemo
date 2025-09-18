using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using Aeon.HR.BusinessObjects.Jobs;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using Microsoft.Owin;
using Owin;
[assembly: OwinStartup(typeof(Aeon.HR.API.Startup))]

namespace Aeon.HR.API
{
    public class Startup
    {
        private IEnumerable<IDisposable> GetHangfireServers()
        {
            //2222
            GlobalConfiguration.Configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(ConfigurationManager.ConnectionStrings["HRDbContext"].ConnectionString, new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    UsePageLocksOnDequeue = true,
                    DisableGlobalLocks = true
                })
                .UseUnityActivator(UnityConfig.GetConfiguredContainer());

            yield return new BackgroundJobServer();
        }

        public void Configuration(IAppBuilder app)
        {
            /*app.UseHangfireAspNet(GetHangfireServers);
            var filter = new BasicAuthAuthorizationFilter(
                new BasicAuthAuthorizationFilterOptions
                {
                    // Require secure connection for dashboard
                    RequireSsl = false,
                    // Case sensitive login checking
                    LoginCaseSensitive = true,
                    Users = new[]
                    {
                        new BasicAuthAuthorizationUser
                        {
                            Login = "jobadmin",
                            // Password as plain text, SHA1 will be used
                            PasswordClear = ConfigurationManager.AppSettings["JobAdminKey"],
                        }
                    }
                });
            var options = new DashboardOptions
            {
                Authorization = new[] { filter }
            };
            app.UseHangfireDashboard("/jobs");
            *//*RecurringJob.AddOrUpdate<ApproverNotificationJob>("4morning", (j) => j.SendNotifications(), Cron.Daily(1, 00));
            RecurringJob.AddOrUpdate<ApproverNotificationJob>("noon", (j) => j.SendNotifications(), Cron.Daily(9, 00));*/
            /*RecurringJob.AddOrUpdate<ResignationJob>("resignation-morning", (j) => j.InactiveUserOnResignationDate(), Cron.Daily(3, 00));*//*
            RecurringJob.AddOrUpdate<MassSynchronizationJob>("mass-sync-morning", (j) => j.Sync(), Cron.Daily(2, 00));
            //RecurringJob.AddOrUpdate<RemindActingNotificationJob>("remind-morning", (j) => j.SendNotifications(), Cron.Monthly(int.Parse(ConfigurationManager.AppSettings["dayOfMonthRemindManagerActing"]))); // 9:00 dayConfig/month/year
            //CR324 - run daily at 8:00 AM
            RecurringJob.AddOrUpdate<RemindActingNotificationJob>("remind-morning", (j) => j.SendNotifications(), Cron.Daily(07, 15));
            //===========================
            *//*RecurringJob.AddOrUpdate<CreateTargetPlanPeriodJob>("create-target-noon", (j) => j.DoCreateTargetPlanPeriod(), Cron.Daily(1, 00));*//*
            //RecurringJob.AddOrUpdate<UpdateUserInfoFromSapJob>("update-user-morning-2020", (j) => j.DoWork(2020), Cron.Daily(17, 00));
            //Khiem bug 471
            //RecurringJob.AddOrUpdate<UpdateUserInfoFromSapJob>("update-user-morning-2021", (j) => j.DoWork(2021), Cron.Daily(17, 00));

            //Sync Quota - current year
            *//*RecurringJob.AddOrUpdate<UpdateUserInfoFromSapJob>("update-user-morning-2022", (j) => j.DoWork(DateTime.Now.Year), Cron.Daily(21, 00));
            RecurringJob.AddOrUpdate<UpdateUserInfoFromSapJob>("update-user-afternoon-2022", (j) => j.DoWork(DateTime.Now.Year), Cron.Daily(5, 00));*//*

            //RecurringJob.AddOrUpdate<PromoteAndTransferJob>("promote-and-transfer-morning", (j) => j.Sync(), Cron.Daily(0, 00));
            //Update Task as Out Of Period
            RecurringJob.AddOrUpdate<CancelOutOfPeriodTasksJob>("Cancel-Out-Of-Period-Tasks-Job", (j) => j.DoCancelOutOfPeriodTasksJob(), Cron.Daily(16, 00));
            //Qle - Update task for Academy
            RecurringJob.AddOrUpdate<RemindResponseInvitationJob>("Remind-Response-Invitation-Job", (j) => j.SendInvitation(), Cron.Daily(0, 00));
            *//*RecurringJob.AddOrUpdate<SubmitPayloadSAP>("auto-retry-payload", (j) => j.DoWork(), Cron.Daily(18, 00));*//*
            //RecurringJob.AddOrUpdate<SubmitPayloadSAP>("auto-retry-payload", (j) => j.DoWork(), "0/5 1-3 * * *");
            // Send email for 1st resignation
            //RecurringJob.AddOrUpdate<SendMail1STResignations>("noon1ST", (j) => j.SendNotifications(), Cron.Daily(9, 05));
            //RecurringJob.AddOrUpdate<SendMail1STResignations>("4morning1ST", (j) => j.SendNotifications(), Cron.Daily(1, 05));

            RecurringJob.RemoveIfExists("update-user-morning-2022");
            RecurringJob.RemoveIfExists("update-user-afternoon-2022");
            RecurringJob.RemoveIfExists("4morning");
            RecurringJob.RemoveIfExists("noon");
            RecurringJob.RemoveIfExists("promote-and-transfer-morning");
            RecurringJob.RemoveIfExists("create-target-noon");
            RecurringJob.RemoveIfExists("resignation-morning");
            RecurringJob.RemoveIfExists("noon1ST");
            RecurringJob.RemoveIfExists("4morning1ST");
            RecurringJob.RemoveIfExists("auto-retry-payload");*/
            //RecurringJob.AddOrUpdate<CancelOutOfPeriodTasksJob>("Cancel-Out-Of-Period-Tasks-Job", (j) => j.DoCancelOutOfPeriodTasksJob(), Cron.Daily(16, 00));
            //RecurringJob.AddOrUpdate<RemindResponseInvitationJob>("Remind-Response-Invitation-Job", (j) => j.SendInvitation(), Cron.Daily(0, 00));
        }
    }
}
