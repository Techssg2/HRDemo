using Aeon.HR.Data.Models;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Interfaces
{
    public interface ITrackingBO
    {
        Task<TrackingRequest> AddNewTrackingRequest(ResponseExternalDataMappingDTO mappingResponse);
        Task UpdateTrackingRequestByInstance(HttpResponseMessage responseMessage, TrackingRequest instance);
        Task<HttpResponseMessage> RetryRequest(string url, string payload);
        Task<HttpResponseMessage> RetryRequestNoAwait(string url, string payload);
    }
}
