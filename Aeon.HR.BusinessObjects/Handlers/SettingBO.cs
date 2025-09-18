using Aeon.HR.ViewModels.Args;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.ViewModels.DTOs;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Data.Models;
using Aeon.HR.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Aeon.HR.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
namespace Aeon.HR.BusinessObjects.Handlers
{
    public partial class SettingBO : ISettingBO
    {
        private readonly IUnitOfWork _uow;
        private readonly IAPIContext _apiCtx;
        private readonly ILogger _logger;
        private readonly IEmployeeBO _employee;
        private readonly ISharePointBO _sharePointBO;
        private readonly IEmailNotification _emailNotification;
        private readonly IDashboardBO _dashboardBO;
        private readonly IIntegrationExternalServiceBO _externalServiceBO;
        private readonly ITrackingHistoryBO _trackingHistoryBO;
        public SettingBO(IUnitOfWork uow, ILogger logger, IEmployeeBO employeeBO, IAPIContext apiCtx, ISharePointBO sharePointBO, IEmailNotification emailNotification, IDashboardBO dashboardBO, IIntegrationExternalServiceBO externalServiceBO, ITrackingHistoryBO trackingHistoryBO)
        {
            _uow = uow;
            _apiCtx = apiCtx;
            _employee = employeeBO;
            _sharePointBO = sharePointBO;
            _emailNotification = emailNotification;
            _dashboardBO = dashboardBO;
            _externalServiceBO = externalServiceBO;
            _trackingHistoryBO = trackingHistoryBO;
            _logger = logger;
        }
    }
}