using Aeon.HR.API.ExternalItem;
using Aeon.HR.BusinessObjects.Handlers.ExternalBO;
using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.DTOs;
using Aeon.HR.ViewModels.ExternalItem;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using Aeon.HR.ViewModels.Args;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.InkML;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.IO;
using System.Text;
using TargetPlanTesting.ImportData;
using Aeon.HR.API.Helpers;
using Aeon.HR.ViewModels.PrintFormViewModel;
using System.Security;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Data;
using System.Drawing;

namespace Aeon.HR.BusinessObjects.Handlers
{
    public class IntegrationExternalServiceBO : IIntegrationExternalServiceBO
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly ILogger _log;
        protected readonly ITrackingBO _trackingBO;
        protected readonly IMasterDataB0 _masterDataB0;
        protected readonly IEmployeeBO _employeeBO;
        private readonly IWorkflowBO _workflowBO;
        private readonly ITargetPlanBO _targetPlanBO;
        private readonly ISettingBO _settingBO;
        private readonly ITrackingHistoryBO _trackingHistoryBO;
        public IntegrationExternalServiceBO(ILogger log, IUnitOfWork unitOfWork, ITrackingBO trackingBO, IMasterDataB0 masterDataB0, IEmployeeBO employeeBO, IEmailNotification emailNotification, IDashboardBO dashboardBO, IEdoc01BO edoc01BO, IAPIContext apiContextBO, ISharePointBO sharePointBO, IFacilityBO facilityBO, ITradeContractBO tradeContractBO, ISKUBO skuBO, ITrackingHistoryBO trackingHistoryBO)
        {
            _unitOfWork = unitOfWork;
            _log = log;
            _trackingBO = trackingBO;
            _masterDataB0 = masterDataB0;
            _employeeBO = employeeBO;
            _trackingHistoryBO = trackingHistoryBO;
            _workflowBO = new WorkflowBO(log, unitOfWork, this, emailNotification, dashboardBO, edoc01BO, facilityBO, tradeContractBO, skuBO, _trackingHistoryBO);
            _settingBO = new SettingBO(unitOfWork, log, employeeBO,apiContextBO,sharePointBO, emailNotification,dashboardBO, this, _trackingHistoryBO);
            _targetPlanBO = new CB.TargetPlanBO(unitOfWork, _workflowBO, log, employeeBO, _settingBO);
        }

        public ExternalExcution BuildAPIService(ExtertalType extertalType)
        {
            switch (extertalType)
            {
                case ExtertalType.LeaveBalance:
                    return new LeaveBalanceBO(_log, _unitOfWork, null, _trackingBO, _targetPlanBO);
                case ExtertalType.PromoteAndTransfer:
                    return new PromoteAndTransferBO(_log, _unitOfWork, null, _trackingBO, _masterDataB0);
                case ExtertalType.Acting:
                    return new ActingInfoBO(_log, _unitOfWork, null, _trackingBO, _masterDataB0);
                case ExtertalType.Resignation:
                    return new ResignationBO(_log, _unitOfWork, null, _trackingBO);
                case ExtertalType.ShiftSet:
                    return new ShiftSetBO(_log, _unitOfWork, null, _trackingBO);
            }
            return null;
        }

        public async Task SubmitData(IIntegrationEntity entity, bool allowSendToSAP)
        {
            ExternalExcution excution = null;
            if (entity is Acting actingItem)
            {
                excution = new ActingInfoBO(_log, _unitOfWork, actingItem, _trackingBO, _masterDataB0);
                excution.AdditionalItem = Mapper.Map<AdditionalItem>(actingItem);
            }
            else
            if (entity is LeaveApplication leaveBalanceItem)
            {
                excution = new LeaveBalanceBO(_log, _unitOfWork, leaveBalanceItem, _trackingBO, _targetPlanBO);
                excution.AdditionalItem = Mapper.Map<AdditionalItem>(leaveBalanceItem);
            }
            else
            if (entity is PromoteAndTransfer promoteAndTransfer)
            {
                excution = new PromoteAndTransferBO(_log, _unitOfWork, promoteAndTransfer, _trackingBO, _masterDataB0);
                excution.AdditionalItem = Mapper.Map<AdditionalItem>(promoteAndTransfer);
            }
            else
            if (entity is Applicant applicant)
            {
                excution = new EmployeeInfoBO(_log, _unitOfWork, applicant, _trackingBO, _masterDataB0);
                excution.AdditionalItem = Mapper.Map<AdditionalItem>(applicant);
                //if(applicant.Position.RequestToHire.ReplacementFor == TypeOfNeed.NewPosition)
                //{
                //    excution.AdditionalItem.DeptCode = applicant.Position.RequestToHire.DeptDivisionCode;
                //}
            }
            else
            if (entity is MissingTimeClock missingTimeClock)
            {
                excution = new MissingTimeClockBO(_log, _unitOfWork, missingTimeClock, _trackingBO, _masterDataB0);
                excution.AdditionalItem = Mapper.Map<AdditionalItem>(missingTimeClock);
            }
            else
            if (entity is OvertimeApplication overtime)
            {
                excution = new OvertimeBO(_log, _unitOfWork, overtime, _trackingBO, _masterDataB0);
                excution.AdditionalItem = Mapper.Map<AdditionalItem>(overtime);
            }
            else
            if (entity is ResignationApplication resignation)
            {
                excution = new ResignationBO(_log, _unitOfWork, resignation, _trackingBO);
                excution.AdditionalItem = Mapper.Map<AdditionalItem>(resignation);
            }
            else
            if (entity is ShiftExchangeApplication shiftExchange)
            {
                excution = new ShiftExchangeBO(_log, _unitOfWork, shiftExchange, _trackingBO);
                excution.AdditionalItem = Mapper.Map<AdditionalItem>(shiftExchange);
            }
            else if (entity is TargetPlan targetPlan)
            {
                excution = new TargetPlanExcutionBO(_log, _unitOfWork, targetPlan, _trackingBO);
                excution.AdditionalItem = Mapper.Map<AdditionalItem>(targetPlan);
            }
            if (excution != null)
            {
                excution.ConvertToPayload();
                await excution.SubmitData(allowSendToSAP);
            }
        }


    }
}