
using System.Threading.Tasks;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;
using UpdateUserInfoFromSapJobV2.src.SAP;

namespace UpdateUserInfoFromSapJobV2.src.Interfaces
{
    public interface IIntegrationExternalServiceBO
    {
        Task SubmitData(IIntegrationEntity entity, bool allowSendToSAP);
        ExternalExcution BuildAPIService(ExtertalType extertalType);
    }
}
