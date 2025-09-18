using Aeon.Academy.Common.Entities;
using System.Collections.Generic;
using System.Net.Http;

namespace Aeon.Academy.IntegrationServices
{
    public interface IEdoc1Service
    {
        HttpResponseMessage GetDepartmentInCharges();
        HttpResponseMessage GetBudgetInformations(int Year, string dicCode);
        HttpResponseMessage GetRequestedDepartments(string sapCode);
        HttpResponseMessage CreateF2MB(CreateF2MBRequest request);
        HttpResponseMessage GetStatusF2MB(string referenceNumber);
        HttpResponseMessage GetSuppliers();
        HttpResponseMessage GetYears();
        HttpResponseMessage GetCurrency(string symbol);
        HttpResponseMessage GetBudgetItem(int year, string departmentInChargeCode, string CFRCode, string budgetCodeCode, string budgetPlan);
    }
}
