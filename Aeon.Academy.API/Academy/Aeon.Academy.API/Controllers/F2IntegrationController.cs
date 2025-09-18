using Aeon.Academy.API.Core;
using Aeon.Academy.Common.Entities;
using Aeon.Academy.Common.Utils;
using Aeon.Academy.IntegrationServices;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Aeon.Intergation.FakeApi.Controllers
{
    public class F2IntegrationController : BaseAuthApiController
    {
        private readonly IEdoc1Service _edoc1Service;
        public F2IntegrationController(IEdoc1Service edoc1Service)
        {
            this._edoc1Service = edoc1Service;
        }
        [HttpGet]
        public IHttpActionResult GetDepartmentInCharges()
        {
            var response = _edoc1Service.GetDepartmentInCharges();
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                var dto = new List<DepartmentInChargeDto>();
                var results = CommonUtil.DeserializeObject<DepartmentInChargeResponse>(result);
                if (results != null && results.Data != null)
                {
                    foreach (DepartmentInChargeModel model in results.Data)
                    {
                        dto.Add(new DepartmentInChargeDto()
                        {
                            Name = model.name,
                            Code = model.dicCode
                        });
                    }
                }
                return Ok(dto);
            }
            else
            {
                return ResponseMessage(response);
            }
        }
        [HttpGet]
        public IHttpActionResult GetBudgetInformations(int year, string dicCode)
        {
            var response = _edoc1Service.GetBudgetInformations(year, dicCode);
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                var dto = new List<BudgetPlanDto>();
                var results = CommonUtil.DeserializeObject<List<BudgetPlanResponse>>(result);
                if (results != null)
                {
                    foreach (BudgetPlanResponse model in results)
                    {
                        dto.Add(new BudgetPlanDto()
                        {
                            BudgetPlan = model.BudgetPlan,
                            BudgetName = model.BudgetName,
                            BudgetCode = model.BudgetCode,
                            CostCenterCode = model.CostCenterCode,
                            TotalBudget = model.TotalBudget,
                            RemainingBalance = model.CurrentBudget,
                            TransferIn = model.TransferIn,
                            TransferOut = model.TransferOut,
                            Refund = model.Refund
                        });
                    }
                }
                return Ok(dto);
            }
            else
            {
                return ResponseMessage(response);
            }
        }
        [HttpGet]
        public IHttpActionResult GetRequestedDepartments(string sapCode)
        {
            var response = _edoc1Service.GetRequestedDepartments(sapCode);
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                var dto = new List<DepartmentInChargeDto>();
                var results = CommonUtil.DeserializeObject<RequestedDepartmentResponse>(result);
                if (results != null && results.Data != null)
                {
                    foreach (RequestedDepartmentModel model in results.Data)
                    {
                        dto.Add(new DepartmentInChargeDto()
                        {
                            Name = model.Name,
                            Code = model.Code
                        });
                    }
                }
                return Ok(dto);
            }
            else
            {
                return ResponseMessage(response);
            }
        }
        [HttpGet]
        public IHttpActionResult GetSuppliers()
        {
            var response = _edoc1Service.GetSuppliers();
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                var dto = new List<DepartmentInChargeDto>();
                var results = CommonUtil.DeserializeObject<SupplierResponse>(result);
                if (results != null && results.Data != null)
                {
                    foreach (SupplierModel model in results.Data)
                    {
                        dto.Add(new DepartmentInChargeDto()
                        {
                            Name = model.SAPName,
                            Code = model.SAPCode,
                        });
                    }
                }
                return Ok(dto);
            }
            else
            {
                return ResponseMessage(response);
            }
        }
        public IHttpActionResult GetYears()
        {
            var response = _edoc1Service.GetYears();
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                var dto = new List<int>();
                var results = CommonUtil.DeserializeObject<YearResponse>(result);
                if (results != null && results.Data != null)
                {
                    dto.AddRange(results.Data);
                }
                return Ok(dto);
            }
            else
            {
                return ResponseMessage(response);
            }
        }
        [HttpGet]
        public IHttpActionResult GetCurrency(string symbol)
        {
            var response = _edoc1Service.GetCurrency(symbol);
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                var dto = new List<CurrencyDto>();
                var results = CommonUtil.DeserializeObject<CurrencyResponse>(result);
                if (results != null)
                {
                    dto.Add(new CurrencyDto()
                    {
                        Id = results.Currency.Id,
                        AmountInVND = results.Currency.AmountInVND,
                        Number = results.Currency.Number,
                        Month = results.Currency.Month,
                        Year = results.Currency.Year,
                        Name = results.Currency.Name,
                        Symbol = results.Currency.Symbol,
                        IsDeleted = results.Currency.IsDeleted,
                        Day = results.Currency.Day,
                    });
                }
                return Ok(dto);
            }
            else
            {
                return ResponseMessage(response);
            }
        }
        [HttpGet]
        public IHttpActionResult GetBudgetItem(int year, string departmentInChargeCode, string CFRCode, string budgetCodeCode, string budgetPlan)
        {
            var response = _edoc1Service.GetBudgetItem(year, departmentInChargeCode, CFRCode, budgetCodeCode, budgetPlan);
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                var dto = new List<BudgetPlanResponse>();
                var results = CommonUtil.DeserializeObject<BudgetPlanResponse>(result);
                if (results != null)
                {
                    dto.Add(new BudgetPlanResponse()
                    {
                        Refund = results.Refund,
                        TransferOut = results.TransferOut,
                        TransferIn = results.TransferIn,
                        CurrentBudget = results.CurrentBudget,
                        TotalBudget = results.TotalBudget,
                        CostCenterCode = results.CostCenterCode,
                        BudgetCode = results.BudgetCode,
                        BudgetName = results.BudgetName,
                        BudgetPlan = results.BudgetPlan,
                    });
                }
                return Ok(dto);
            }
            else
            {
                return ResponseMessage(response);
            }
        }

    }
}
