using Aeon.Academy.Common.Consts;
using Aeon.Academy.Common.Entities;
using Aeon.Academy.Data;
using Aeon.Academy.Data.Entities;
using Aeon.Academy.Data.Repository;
using Aeon.Academy.IntegrationServices;
using Aeon.Academy.Services;
using Newtonsoft.Json;
using NLog;
using System;
using System.Configuration;

namespace Aeon.Academy.Edoc1Scheduler
{
    class Program
    {
        private static readonly NLog.ILogger Logger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            try
            {
                var unitOfWork = new UnitOfWork<AppDbContext>(new AppDbContext());
                var unitOfWorkEdoc1 = new UnitOfWork<EDocDbContext>(new EDocDbContext());
                var trainingRequestService = new TrainingRequestService(unitOfWork, unitOfWorkEdoc1);

                IWorkflowService<TrainingRequest> workflowService = new WorkflowService<TrainingRequest>(unitOfWorkEdoc1, unitOfWork, null);

                var updatedItems = 0;
                var list = trainingRequestService.ListByPending();
                var edoc1Service = new Edoc1Service();
                foreach (var request in list)
                {
                    if (string.IsNullOrEmpty(request.F2ReferenceNumber)) continue;
                    var response = edoc1Service.GetStatusF2MB(request.F2ReferenceNumber ?? string.Empty);
                    if (response != null)
                    {
                        var httpResponseResult = response.Content.ReadAsStringAsync().Result;
                        if (response.IsSuccessStatusCode)
                        {
                            var result = JsonConvert.DeserializeObject<F2MBStatusResponse>(httpResponseResult);
                            if (result != null && result.Data != null)
                            {
                                try
                                {
                                    updatedItems = RunAction(workflowService, request, result.Data.Status, updatedItems);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Fatal(request.ReferenceNumber + " - " + ex);
                                }
                            }
                        }
                        else
                        {
                            Logger.Fatal(httpResponseResult);
                        }
                    }
                    else
                    {
                        Logger.Fatal("Connection error.");
                    }
                }
                Logger.Info(string.Format("Total item: {0}, Updated items: {1}", list.Count, updatedItems));
                AdminCancelATR(trainingRequestService, edoc1Service);
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }
        }
        static int RunAction(IWorkflowService<TrainingRequest> workflowService, TrainingRequest request, string action, int count)
        {
            var user = new User()
            {
                FullName = "F2MB",
                LoginName = "f2"
            };
            if (string.IsNullOrEmpty(action))
            {
                return count;
            }
            if (action.IndexOf("Completed", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                workflowService.Approve(request, "Completed", user);
                Logger.Info(string.Format("{0} item: {1}", action, request.ReferenceNumber));
                count = count + 1;
            }
            else if (action.IndexOf("Reject", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                workflowService.Reject(request, "Rejected", user);
                Logger.Info(string.Format("{0} item: {1}", action, request.ReferenceNumber));
                count = count + 1;
            }
            else if (action.IndexOf("Request", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                workflowService.RequestToChange(request, "Request to change", user);
                Logger.Info(string.Format("{0} item: {1}", action, request.ReferenceNumber));
                count = count + 1;
            }
            else if (action.IndexOf("Cancel", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                workflowService.Cancel(request, "Cancelled", user);
                Logger.Info(string.Format("{0} item: {1}", action, request.ReferenceNumber));
                count = count + 1;
            }
            return count;
        }
        static void AdminCancelATR(TrainingRequestService service, Edoc1Service edoc1Service)
        {
            var date = DateTime.Now.AddDays(Common.Configuration.ApplicationSettings.CreatedDateFrom);
            var list = service.ListByExternalCompleted(date);
            int cancel = 0;
            foreach (var request in list)
            {
                if (string.IsNullOrEmpty(request.F2ReferenceNumber)) continue;
                var response = edoc1Service.GetStatusF2MB(request.F2ReferenceNumber);
                if (response != null)
                {
                    var httpResponseResult = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var result = JsonConvert.DeserializeObject<F2MBStatusResponse>(httpResponseResult);
                        if (result != null && result.Data != null)
                        {
                            if (result.Data.Status.IndexOf("Cancel", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                request.Status = WorkflowStatus.Cancelled;
                                service.Save(request);
                                cancel++;
                                Logger.Info(string.Format("Change Completed to Cancelled --- ReferenceNumber: {0}", request.ReferenceNumber));
                            }
                        }
                    }
                    else
                    {
                        Logger.Fatal(httpResponseResult);
                    }
                }
                else
                {
                    Logger.Fatal("Connection error.");
                }
            }
            Logger.Info(string.Format("Admin Change Completed to Cancelled --- (Cancel/Total) {0}/{1}", cancel, list.Count));
        }
    }
}
