using Aeon.Academy.Common.Consts;
using Aeon.Academy.Common.Entities;
using Aeon.Academy.Data;
using Aeon.Academy.Data.Repository;
using Aeon.Academy.Services;
using NLog;
using System;

namespace Aeon.Academy.TaskScheduler
{
    class Program
    {
        private static readonly NLog.ILogger Logger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            var unitOfWork = new UnitOfWork<AppDbContext>(new AppDbContext());
            var service = new QueueService(unitOfWork);
            var invitationService = new TrainingInvitationService(unitOfWork, null);
            var integrationService = new IntegrationService(unitOfWork);
            Logger.Info("SAP Integration Start ................");
            try
            {
                //Retry
                var queues = service.ListByInstanceType(IntegraitionType.Sap);
                Logger.Info("Retry failed items ...");
                var update = 0;
                var fail = 0;
                foreach (var item in queues)
                {
                    if (item.NumberOfCall > 9) continue;
                    var participant = invitationService.GetParticipantBySapCode(item.SapCode, item.TrainingInvitationId);
                    if (participant != null && participant.SapStatusCode != (int)SAPStatusCode.Submit)
                    {
                        var result = integrationService.SyncSAP(participant, ref update, ref fail);
                        if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        {
                            Logger.Fatal(result.RequestMessage.Headers);
                            return;
                        }
                    }
                }
                Logger.Info(string.Format("Total: {0}, updated: {1}, failed: {2}", queues.Count, update, fail));
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }
            //Sync after training end
            Sync(invitationService, integrationService);
        }
        static void Sync(TrainingInvitationService invitationService, IntegrationService integrationService)
        {
            try
            {
                var update = 0;
                var fail = 0;
                Logger.Info("Sync new items ...");
                var partipants = invitationService.GetParticipantsWithNotSubmitted();
                foreach (var item in partipants)
                {
                    var result = integrationService.SyncSAP(item, ref update, ref fail, true);
                    if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        Logger.Fatal(result.RequestMessage.Headers);
                        return;
                    }
                }
                Logger.Info(string.Format("Total: {0}, updated: {1}, failed: {2}", partipants.Count, update, fail));
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }
        }
    }
}
