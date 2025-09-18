using Aeon.Academy.API.Jobs;
using Aeon.Academy.API.Module;
using Aeon.Academy.Data;
using Aeon.Academy.Data.Entities;
using Aeon.Academy.Data.Repository;
using Aeon.Academy.IntegrationServices;
using Aeon.Academy.Services;
using System.Data.Entity;
using Unity;
using Unity.Lifetime;
namespace Aeon.Academy.API
{
    public static class UnityConfig
    {
        public static void RegisterComponents(UnityContainer container)
        {
            RegisterDataComponents(container);
            RegisterServiceComponents(container);
        }

        private static void RegisterDataComponents(UnityContainer container)
        {
            container.BindInRequestScope<DbContext, EDocDbContext>();
            container.BindInRequestScope<IUnitOfWork<EDocDbContext>, UnitOfWork<EDocDbContext>>();

            container.BindInRequestScope<DbContext, AppDbContext>();
            container.BindInRequestScope<IUnitOfWork<AppDbContext>, UnitOfWork<AppDbContext>>();
        }

        private static void RegisterServiceComponents(UnityContainer container)
        {
            container.RegisterType<HR.Infrastructure.Interfaces.IModuleDashboard, ModuleDashboard>(typeof(ModuleDashboard).FullName);
            container.RegisterType<HR.Infrastructure.Interfaces.IRemindResponseInvitationJob, RemindResponseInvitationJob>(typeof(RemindResponseInvitationJob).FullName);
            container.BindInRequestScope<IDoTaskService, DoTaskService>();

            container.BindInRequestScope<IUserService, UserService>();
            container.BindInRequestScope<ITrainingRequestService, TrainingRequestService>();
            container.BindInRequestScope<ICategoryService, CategoryService>();
            container.BindInRequestScope<ICourseService, CourseService>();
            container.BindInRequestScope<ITrainingInvitationService, TrainingInvitationService>();
            container.BindInRequestScope<ITrainingReportService, TrainingReportService>();
            container.BindInRequestScope<IReportService, ReportService>();
            container.BindInRequestScope<IReasonOfTrainingRequestService, ReasonOfTrainingRequestService>();

            container.BindInRequestScope<BaseWorkflowEntity, TrainingRequest>();
            container.BindInRequestScope<IWorkflowService<TrainingRequest>, WorkflowService<TrainingRequest>>();

            container.BindInRequestScope<BaseWorkflowEntity, TrainingReport>();
            container.BindInRequestScope<IWorkflowService<TrainingReport>, WorkflowService<TrainingReport>>();

            container.BindInRequestScope<ILogger, Logger>();
            container.BindInRequestScope<IEdoc1Service, Edoc1Service>();
        }

        public static void BindInRequestScope<T1, T2>(this IUnityContainer container) where T2 : T1
        {
            container.RegisterType<T1, T2>(new HierarchicalLifetimeManager());
        }
    }
}