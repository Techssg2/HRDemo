using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Aeon.HR.BusinessObjects.Handlers
{
    public partial class CBBO: ICBBO
    {
        private readonly IUnitOfWork _uow;
        private readonly IWorkflowBO _workflowBO;
        private readonly ISettingBO _settingBO;
        private readonly ISSGExBO _sSGExBO;
        private readonly IMasterDataB0 _masterData;
        private readonly ITargetPlanBO _targetPlanBO;
        public CBBO(IUnitOfWork uow, IWorkflowBO workflowBO, IMasterDataB0 masterData, ISettingBO settingBO, ISSGExBO sSGExBO, ITargetPlanBO targetPlanBO)
        {
            _uow = uow;
            _workflowBO = workflowBO;
            _settingBO = settingBO;
            _sSGExBO = sSGExBO;
            _masterData = masterData;
            _targetPlanBO = targetPlanBO;
        }
    }
}