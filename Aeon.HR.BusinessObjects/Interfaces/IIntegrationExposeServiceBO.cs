using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Interfaces
{
    public interface IIntegrationExternalServiceBO
    {
        Task SubmitData(IIntegrationEntity entity, bool allowSendToSAP);
        ExternalExcution BuildAPIService(ExtertalType extertalType);
    }
}
