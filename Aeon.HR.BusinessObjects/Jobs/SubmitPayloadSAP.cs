using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.DTOs;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Jobs
{
    public class SubmitPayloadSAP
    {
        private readonly ILogger _logger;
        private readonly IUnitOfWork _uow;
        private readonly ISSGExBO _exBO;


        public SubmitPayloadSAP(ILogger logger, IUnitOfWork uow, ISSGExBO exBO)
        {
            _logger = logger;
            _uow = uow;
            _exBO = exBO;
        }

        public async Task DoWork()
        {
            _logger.LogInformation("SubmitPayloadSAP.DoWork: ");
            try
            {
                List<string> status = new List<string>() { "Save data success", "SUCCESS", "Success", "success", "succes", "Succes", "save data success", "savedatasuccess" };
                DateTimeOffset currentTime = DateTimeOffset.Now.AddDays(-1);
                var targetPlantPeriod = await _uow.GetRepository<TargetPlanPeriod>().GetSingleAsync(x => x.FromDate <= currentTime && currentTime <= x.ToDate);
                if (targetPlantPeriod != null)
                {
                    var trackingRequests = await _uow.GetRepository<TrackingRequest>().FindByAsync(x =>
                    x.TrackingLogInitDatas.Any(y => (y.FromDate.HasValue && (DateTimeOffset.Compare(y.FromDate.Value, targetPlantPeriod.FromDate) >= 0 && DateTimeOffset.Compare(y.FromDate.Value, targetPlantPeriod.ToDate) <= 0))
                        || (y.ToDate.HasValue && (DateTimeOffset.Compare(y.ToDate.Value, targetPlantPeriod.FromDate) >= 0 && (DateTimeOffset.Compare(y.ToDate.Value, targetPlantPeriod.ToDate) <= 0))))
                        && !status.Contains(x.Status), "created asc");
                    foreach (var trackingRequest in trackingRequests)
                    {
                        try
                        {
                            await _exBO.Retry(trackingRequest.Id);
                            _logger.LogInformation("Id: " + trackingRequest.Id +  " - " + trackingRequest.ReferenceNumber + " - " + trackingRequest.Status);
                            /*await Task.Delay(5000);*/
                        }
                        catch (TaskCanceledException e)
                        {
                            _logger.LogInformation("Run Job Submit Payload SAP ReferenceNumber: " + trackingRequest.ReferenceNumber + "|| Time: " + System.DateTimeOffset.Now + " || Status: " + trackingRequest.Status);
                            _logger.LogInformation("Error Submit Payload SAP: " + e.Message);
                        }
                    }
                    await _uow.CommitAsync();
                }
                else
                {
                    _logger.LogInformation("TargetPlantPeriod is null");
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error Submit Payload SAP: " + ex.Message + " | Time: " + System.DateTimeOffset.Now);
            }
        }
    }
}
