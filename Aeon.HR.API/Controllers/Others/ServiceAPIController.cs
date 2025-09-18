using Microsoft.Extensions.Logging;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using Aeon.HR.Data.Models;
using System.Collections.Generic;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.BusinessObjects.Jobs;
using Aeon.HR.Infrastructure.Abstracts;
using Newtonsoft.Json;
using Aeon.HR.BusinessObjects.CompleteActions;
using Aeon.HR.ViewModels;
using System.Linq;

namespace SSG2.API.Controllers.Others
{
    public class ServiceAPIController : ApiController
    {
        protected readonly ILogger _logger;
        protected readonly IEmployeeBO _employee;
        protected readonly IUnitOfWork unitOf;
        private readonly ISSGExBO _exBO;
        private readonly IEmailNotification _emailNotification;
        private readonly IEdoc01BO _edoc01BO;
        private readonly IWorkflowBO _workflowBO;
        private readonly ISettingBO _settingBO;
        private readonly IDashboardBO _dashboardBO;

        public ServiceAPIController(
            ILogger logger,
            IEmployeeBO employee,
            IUnitOfWork unitOfWork,
            ISSGExBO exBO,
            IEmailNotification emailNotification,
            IEdoc01BO edoc01BO,
            IWorkflowBO workflowBO,
            ISettingBO settingBO
            )
        {
            _logger = logger;
            _employee = employee;
            unitOf = unitOfWork;
            _exBO = exBO;
            _emailNotification = emailNotification;
            _edoc01BO = edoc01BO;
            _workflowBO = workflowBO;
            _settingBO = settingBO;
        }

        #region Service API
        [HttpGet]
        public async Task<ResultDTO> UpdateUserInfoFromSapJob(int year)
        {
            try
            {
                _logger.LogError("-------- UpdateUserInfoFromSapJob --------");
                UpdateUserInfoFromSapJob runJob = new UpdateUserInfoFromSapJob(_logger, unitOf, _employee, _exBO);
                bool rs = await runJob.DoWork(year);
                return new ResultDTO() { Object = rs };
            }
            catch (Exception ex)
            {
                _logger.LogError("Ex: ", ex.Message);
                return null;
            }
        }

        [HttpGet]
        public async Task<ResultDTO> JobEmailDailyApproverEdoc1Edoc2()
        {
            try
            {
                _logger.LogInformation("-------- JobEmailDailyApproverEdoc1Edoc2 --------");
                ApproverNotificationJob runJob = new ApproverNotificationJob(_logger, unitOf, _emailNotification, _edoc01BO, _workflowBO);
                await runJob.SendNotifications();
                return new ResultDTO() { Object = true };
            }
            catch (Exception ex)
            {
                _logger.LogError("Ex: ", ex.Message);
                return null;
            }
        }
        [HttpGet]
        public async Task<ResultDTO> JobSendMail1STResignations()
        {
            try
            {
                _logger.LogInformation("-------- JobSendMail1STResignations --------");
                SendMail1STResignations runJob = new SendMail1STResignations(_logger, unitOf, _emailNotification);
                bool rs = await runJob.SendNotifications();
                return new ResultDTO() { Object = rs };
            }
            catch (Exception ex)
            {
                _logger.LogError("Ex: ", ex.Message);
                return null;
            }
        }

        [HttpGet]
        public async Task<ResultDTO> JobResignation()
        {
            try
            {
                _logger.LogInformation("-------- Start JobResignation --------");
                ResignationJob runJob = new ResignationJob(_logger, unitOf, _settingBO);
                bool rs = await runJob.InactiveUserOnResignationDate();
                return new ResultDTO() { Object = rs };
            }
            catch (Exception ex)
            {
                _logger.LogError("Ex: ", ex.Message);
                return null;
            }
        }
        [HttpGet]
        public async Task<ResultDTO> JobTargetPeriod()
        {
            try
            {
                _logger.LogInformation("-------- Start JobTargetPeriod --------");
                CreateTargetPlanPeriodJob runJob = new CreateTargetPlanPeriodJob(_logger, unitOf);
                runJob.DoCreateTargetPlanPeriod();
                return new ResultDTO() { Object = true};
            }
            catch (Exception ex)
            {
                _logger.LogError("Ex: ", ex.Message);
                return null;
            }
        }
        
        [HttpGet]
        public async Task<ResultDTO> RemindResponseInvitationJob()
        {
            try
            {
                _logger.LogInformation("-------- Start RemindResponseInvitationJob --------");
                RemindResponseInvitationJob runJob = new RemindResponseInvitationJob();
                runJob.SendInvitation();
                return new ResultDTO() { Object = true};
            }
            catch (Exception ex)
            {
                _logger.LogError("Ex: ", ex.Message);
                return null;
            }
        }
        
        
        [HttpGet]
        public async Task<ResultDTO> CancelOutOfPeriodTasksJob()
        {
            try
            {
                _logger.LogInformation("-------- Start CancelOutOfPeriodTasksJob --------");
                CancelOutOfPeriodTasksJob runJob = new CancelOutOfPeriodTasksJob(_logger, unitOf);
                await runJob.DoCancelOutOfPeriodTasksJob();
                return new ResultDTO() { Object = true};
            }
            catch (Exception ex)
            {
                _logger.LogError("Ex: ", ex.Message);
                return null;
            }
        }

        [HttpGet]
        public async Task<ResultDTO> JobPromoteAndTransfer()
        {
            try
            {
                _logger.LogInformation("-------- Start JobPromoteAndTransfer --------");
                PromoteAndTransferJob runJob = new PromoteAndTransferJob(_logger, unitOf, _settingBO);
                bool rs = await runJob.Sync();
                return new ResultDTO() { Object = rs };
            }
            catch (Exception ex)
            {
                _logger.LogError("Ex: ", ex.Message);
                return null;
            }
        }

        [HttpGet]
        public async Task<ResultDTO> JobAutoRetryPayloadSAP()
        {
            try
            {
                _logger.LogInformation("-------- Start JobAutoRetryPayloadSAP --------");
                SubmitPayloadSAP runJob = new SubmitPayloadSAP(_logger, unitOf, _exBO);
                 await runJob.DoWork();
                return new ResultDTO() { Object = true };
            }
            catch (Exception ex)
            {
                _logger.LogError("Ex: ", ex.Message);
                return null;
            }
        }
        [HttpGet]
        public async Task<ResultDTO> ClearCacheDepartment()
        {
            try
            {
                //_dashboardBO.ClearNode();
                bool clear = _settingBO.ResetAllDepartmentCache();
                return new ResultDTO() { Object = clear };
            }
            catch (Exception ex)
            {
                _logger.LogError("Ex: ", ex.Message);
                return null;
            }
        }
        #endregion
    }
}
