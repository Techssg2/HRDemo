using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Jobs
{
    public class TestJob
    {
        private readonly ILogger _logger;
        private readonly IUnitOfWork _uow;
        private readonly IEmailNotification _emailNotification;
        private readonly IEdoc01BO _edoc01;
        private readonly IWorkflowBO _workflowBO;

        public TestJob(ILogger logger, IUnitOfWork uow, IEmailNotification emailNotification, IEdoc01BO edoc01, IWorkflowBO workflowBO)
        {
            _logger = logger;
            _uow = uow;
            _emailNotification = emailNotification;
            _edoc01 = edoc01;
            _workflowBO = workflowBO;
        }

        public void TestJobNofitications()
        {
            _logger.LogInformation("---------------------------------------------------------------------------------");
            _logger.LogInformation("---------------------------------------------------------------------------------");
            _logger.LogInformation("---------------------------------------------------------------------------------");
            _logger.LogInformation("-------------------------------------TestJob-------------------------------------");
            _logger.LogInformation("---------------------------------------------------------------------------------");
            _logger.LogInformation("---------------------------------------------------------------------------------");
        }
    }
}
