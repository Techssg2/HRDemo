using Refit;
using System.Threading.Tasks;
using Aeon.Academy.Common.Entities;
using System.Net.Http;

namespace Aeon.Academy.IntegrationServices
{
    public interface ISapApi
    {
        [Post("")]
        Task<HttpResponseMessage> SyncData([Body] AcademyTrainingRequest model);
    }
}
