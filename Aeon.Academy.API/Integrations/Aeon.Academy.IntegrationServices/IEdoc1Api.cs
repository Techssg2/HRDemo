using Aeon.Academy.Common.Entities;
using Refit;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Aeon.Academy.IntegrationServices
{
    public interface IEdoc1Api
    {
        [Get("/DepartmentsInCharge")]
        Task<HttpResponseMessage> GetDepartmentInCharges();
        [Post("/BudgetInformations")]
        Task<HttpResponseMessage> GetBudgetInformations(BudgetPlanRequest model);
        [Post("/GetDepartments")]
        Task<HttpResponseMessage> GetRequestedDepartments(RequestedDepartmentRequest model);
        [Post("/CreatePurchaseRequest")]
        Task<HttpResponseMessage> CreateF2MB([Body] CreateF2MBRequest model);
        [Post("/PurchaseRequestStatus")]
        Task<HttpResponseMessage> GetStatusF2MB(F2MBStatusRequest model);
        [Get("/Suppliers")]
        Task<HttpResponseMessage> GetSuppliers();
        [Get("/Years")]
        Task<HttpResponseMessage> GetYears();
        [Post("/GetCurrency")]
        Task<HttpResponseMessage> GetCurrency(CurrencyModel model);
        [Post("/GetBudgetItem")]
        Task<HttpResponseMessage> GetBudgetItem(BudgetBallanceModel model);
    }
}
