using Aeon.HR.BusinessObjects.DataHandlers;
using Aeon.HR.BusinessObjects.Handlers;
using Aeon.HR.BusinessObjects.Handlers.CB;
using Aeon.HR.BusinessObjects.Handlers.ExternalBO;
using Aeon.HR.BusinessObjects.Handlers.File;
using Aeon.HR.BusinessObjects.Handlers.Other;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data;
using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Filters;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Microsoft.Logging;
using Unity.WebApi;

namespace Aeon.Navigation
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = GetConfiguredContainer();
            // file Processing
            //container.BindInRequestScope<IExcuteFileProcessing, ExcuteFileProcessing>();
            //container.BindInRequestScope<IUserExcelProcessingBO, UserExcelProcessingBO>();
            //container.BindInRequestScope<IMasterDataExcelProcessingBO, MasterDataExcelProcessingBO>();
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
            var providers = GlobalConfiguration.Configuration.Services.GetFilterProviders();
            var defaultprovider = providers.Single(i => i is ActionDescriptorFilterProvider);
            GlobalConfiguration.Configuration.Services.Remove(typeof(IFilterProvider), defaultprovider);
            GlobalConfiguration.Configuration.Services.Add(typeof(IFilterProvider), new UnityFilterProvider(container));

            ServiceLocator.SetContainer(container);
        }

        /// </summary>
        public static UnityContainer GetConfiguredContainer()
        {
            var container = new UnityContainer();
            container.BindInRequestScope<DbContext, HRDbContext>();
            container.AddUnitOfWork<HRDbContext>();
            container.BindInRequestScope<IRecruitmentBO, RecruitmentBO>();
            container.BindInRequestScope<ISettingBO, SettingBO>();
            container.BindInRequestScope<ISSGExBO, SSGExBO>();
            container.BindInRequestScope<ICBBO, CBBO>();
            container.BindInRequestScope<ISAPBO, SAPBO>();
            container.BindInRequestScope<IIntegrationExternalServiceBO, IntegrationExternalServiceBO>();
            container.BindInRequestScope<IMasterDataB0, MasterDataBO>();
            container.BindInRequestScope<IWorkflowBO, WorkflowBO>();
            container.BindInRequestScope<IPermissionBO, PermissionBO>();
            container.RegisterInstance<IMemoryCache>(new MemoryCache(Options.Create<MemoryCacheOptions>(new MemoryCacheOptions())));
            container.BindInRequestScope<ICachingService, CachingService>();
            container.BindInRequestScope<ITrackingBO, TrackingBO>();
            container.BindInRequestScope<IAttachmentFileBO, AttachmentFileBO>();
            container.BindInRequestScope<IEmployeeBO, EmployeeBO>();
            container.BindInRequestScope<IEmailNotification, EmailNotification>();
            container.BindInRequestScope<IAPIContext, APIContextBO>();
            container.BindInRequestScope<ISharePointBO, SharePointBO>();
            container.BindInRequestScope<IDashboardBO, DashboardBO>();
            container.BindInRequestScope<IExcuteFileProcessing, ExcuteFileProcessing>();
            container.BindInRequestScope<ICommonBO, CommonBO>();
            container.BindInRequestScope<IMassBO, MassBO>();
            container.BindInRequestScope<IEdoc01BO, Edoc1BO>();
            container.BindInRequestScope<ITargetPlanBO, TargetPlanBO>();
            container.BindInRequestScope<IBusinessTripBO, BusinessTripBO>();
            container.BindInRequestScope<INavigationBO, NavigationBO>();

            container.BindInRequestScope<IBTAPolicyBO, BTAPolicyBO>();
            container.BindInRequestScope<IMaintenantBO, MaintenantBO>();
            container.BindInRequestScope<IAPIEdoc2BO, APIEdoc2BO>();
            container.BindInRequestScope<IFacilityBO, FacilityBO>();
            container.BindInRequestScope<IOverBudgetBO, OverBudgetBO>();


            container.BindInRequestScope<ITrackingHistoryBO, TrackingHistoryBO>();
            container.BindInRequestScope<IITBO, ITBO>();

            container.BindInRequestScope<ITradeContractBO, TradeContractBO>();
            container.BindInRequestScope<ISKUBO, SKUBO>();
            ILoggerFactory loggerFactory = new LoggerFactory();
            string basedir = HostingEnvironment.MapPath("~");
            loggerFactory.AddSerilog(new LoggerConfiguration().MinimumLevel.Debug().WriteTo.File(basedir + @"\logs\log.txt", rollingInterval: RollingInterval.Day).CreateLogger());
            container.AddExtension(new LoggingExtension(loggerFactory));
            return container;
        }
    }
}