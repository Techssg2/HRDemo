using AutoMapper;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.ViewModels.DTOs;
using Aeon.HR.Data.Models;
using Aeon.HR.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Aeon.HR.Infrastructure.Interfaces;
using System.Transactions;

namespace Aeon.HR.BusinessObjects.Handlers
{
    public partial class RecruitmentBO : IRecruitmentBO
    {
        private readonly IUnitOfWork _uow;
        private readonly IIntegrationExternalServiceBO _externalServiceBO;
        private readonly IWorkflowBO _workflowBO;
        private readonly ISettingBO _settingBO;
        private readonly IMasterDataB0 _masterDataB0;
        private readonly IDashboardBO _dashboardBO;
        private readonly IAttachmentFileBO _attachmentFileBO;
        private readonly IMassBO _massBO;

        public RecruitmentBO(IUnitOfWork uow, IIntegrationExternalServiceBO externalServiceBO, IWorkflowBO workflowBO, ISettingBO settingBO, IMasterDataB0 masterDataB0, IDashboardBO dashboardBO, IAttachmentFileBO attachmentFileBO, IMassBO massBO)
        {
            _uow = uow;
            _externalServiceBO = externalServiceBO;
            _workflowBO = workflowBO;
            _settingBO = settingBO;
            _masterDataB0 = masterDataB0;
            _dashboardBO = dashboardBO;
            _attachmentFileBO = attachmentFileBO;
            _massBO = massBO;
        }

        public Task<ResultDTO> CreateItemWithPermission<T>(T Data)
        {
            throw new NotImplementedException();
        }
    }
}