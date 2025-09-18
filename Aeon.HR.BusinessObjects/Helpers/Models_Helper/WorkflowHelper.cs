using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects
{
    #region WorkflowHelper
    public static class WorkflowHelper
    {
        public static WorkflowStep GetNextStep(this IList<WorkflowStep> steps, WorkflowStep currentStep, WorkflowEntity workflowEntity, Type itemType)
        {
            WorkflowStep returnValue = null;
            try
            {
                WorkflowStep nextStep = steps.OrderBy(x => x.StepNumber).FirstOrDefault(x=>x.StepNumber > currentStep.StepNumber);
                if(nextStep!= null)
                {
                    if(nextStep.IsStepWithConditions)
                    {
                        bool matched = nextStep.DoesStepMatchCondition(workflowEntity, itemType);
                        if(!matched)
                        {
                            returnValue = steps.GetNextStep(nextStep, workflowEntity, itemType);
                        }
                        else
                        {
                            returnValue = nextStep;
                        }
                    }
                    else
                    {
                        returnValue = nextStep;
                    }
                }
            }
            catch
            {
                returnValue = null;
            }
            return returnValue;
        }

        public static bool DoesStepMatchCondition(this WorkflowStep currentStep, WorkflowEntity workflowEntity, Type itemType)
        {
            bool returnValue = true;
            try
            {
                if (currentStep != null && workflowEntity != null && itemType != null)
                {
                    #region Func DoesMatchCondition
                    Func<StepCondition, bool> DoesMatchCondition = (StepCondition cStepCondition) =>
                    {
                        bool returnStatus = false;
                        try
                        {
                            if (cStepCondition != null && cStepCondition.FieldValues != null)
                            {
                                object itemValue = workflowEntity.GetPropertyValue(cStepCondition.FieldName);
                                if (itemValue != null)
                                {
                                    returnStatus = cStepCondition.FieldValues.Where(x => x.Equals(itemValue.GetAsString(), StringComparison.OrdinalIgnoreCase)).Any();
                                }
                            }
                        }
                        catch
                        {
                            returnStatus = false;
                        }
                        return returnStatus;
                    };
                    #endregion

                    foreach (StepCondition cStepCondition in currentStep.StepConditions)
                    {
                        if (!DoesMatchCondition(cStepCondition))
                        {
                            returnValue = false;
                            break;
                        }
                    }
                }
            }
            catch
            {
                returnValue = false;
            }
            return returnValue;
        }

        public static TrackingRequest GetTrackingRequestOfWFInstance(this WorkflowInstance wfInstance, IUnitOfWork uow)
        {
            TrackingRequest returnValue = null;
            try
            {
                if (wfInstance != null && wfInstance.ItemId != Guid.Empty && uow != null && wfInstance.IsCompleted && !wfInstance.IsTerminated)
                {
                    List<TrackingRequest> TrackingRequests = uow.GetRepository<TrackingRequest>().FindBy(x => x.ReferenceNumber == wfInstance.ItemReferenceNumber).OrderBy(x=>x.Created).ToList();
                    if(TrackingRequests != null && TrackingRequests.Any())
                    {
                        returnValue = TrackingRequests.FirstOrDefault(x => x.Created >= wfInstance.Modified);
                    }
                }
            }
            catch
            {
                returnValue = null;
            }
            return returnValue;
        }

        public static bool DoesWFHasPayload(this WorkflowInstance wfInstance, IUnitOfWork uow)
        {
            bool returnValue = true;
            //If Workflow entiry no need to send data to SAP => return value is true, No need to check the Tracking request
            try
            {
                if(wfInstance != null && wfInstance.ItemId != Guid.Empty && uow != null)
                {
                    
                    List<Type> needPayloadTypes = new List<Type>() {
                        typeof(Acting),
                        typeof(LeaveApplication),
                        typeof(PromoteAndTransfer),
                        typeof(Applicant),
                        typeof(MissingTimeClock),
                        typeof(OvertimeApplication),
                        typeof(ResignationApplication),
                        typeof(ShiftExchangeApplication),
                        typeof(TargetPlan)
                    };
                    Object wfEntity = wfInstance.ItemId.GetWorkflowItem(uow);
                    if(wfEntity != null)
                    {
                        if (needPayloadTypes.Contains(wfEntity.GetType()))
                        {
                            returnValue = wfInstance.GetTrackingRequestOfWFInstance(uow) != null;
                        }
                    }
                }
            }
            catch
            {
                returnValue = false;
            }
            return returnValue;
        }

        public static async Task<bool> IsCompleteWorkflow(this IEnumerable<WorkflowInstance> wfInstances, IUnitOfWork uow) { 
            foreach (var wfInstance in wfInstances)
            {
                var wfHistories = await uow.GetRepository<WorkflowHistory>(true).FindByAsync(x => x.InstanceId == wfInstance.Id);

                foreach (var wfHistory in wfHistories)
                {
                    if (!wfHistory.IsStepCompleted)
                    { 
                        return false;
                    }
                }
                
            }
            return true;
        }
    }
    #endregion
}
