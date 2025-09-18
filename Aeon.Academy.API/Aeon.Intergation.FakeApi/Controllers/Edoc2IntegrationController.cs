using Aeon.Academy.Common.Configuration;
using Aeon.Academy.Common.Entities;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Aeon.Intergation.FakeApi.Controllers
{
    public class Edoc2IntegrationController : ApiController
    {
        [HttpGet]
        public IHttpActionResult DepartmentsInCharge()
        {
            var dto = new DepartmentInChargeResponse()
            {
                Total = 2,
                Data = new List<DepartmentInChargeModel>()
            };
            dto.Data.Add(new DepartmentInChargeModel()
            {
                code = "HR",
                name = "HR - Manpower Planning"
            });
            dto.Data.Add(new DepartmentInChargeModel()
            {
                code = "RE",
                name = "RE - Recruitment"
            });
            return Ok(dto);
        }

        [HttpPost]
        public IHttpActionResult BudgetInformations(BudgetPlanRequest model)
        {
            var dto = new List<BudgetPlanResponse>();
            dto.Add(new BudgetPlanResponse()
            {
                BudgetPlan = "Planned",
                BudgetName = "PL34-Domestic Training-6417240000",
                BudgetCode = "B21-0043",
                CostCenterCode = "Head Quater-HCM",
                TotalBudget = 177004455,
                CurrentBudget = 107004455
            });
            dto.Add(new BudgetPlanResponse()
            {
                BudgetPlan = "Planned",
                BudgetName = "PL34-Domestic Training-6417240001",
                BudgetCode = "B21-0043",
                CostCenterCode = "Canary",
                TotalBudget = 18473824,
                CurrentBudget = 10473824
            });
            dto.Add(new BudgetPlanResponse()
            {
                BudgetPlan = "Unplanned",
                BudgetName = "PL34-Domestic Training-6417240000",
                BudgetCode = "B21-0131",
                CostCenterCode = "Binh Tan",
                TotalBudget = 18473824,
                CurrentBudget = 10473824
            });
            dto.Add(new BudgetPlanResponse()
            {
                BudgetPlan = "Unplanned",
                BudgetName = "PL34-Domestic Training-6417240001",
                BudgetCode = "B21-0131",
                CostCenterCode = "Le Chan",
                TotalBudget = 177004455,
                CurrentBudget = 107004455
            });
            return Ok(dto);
        }
        [HttpPost]
        public IHttpActionResult GetDepartments(RequestedDepartmentRequest model)
        {
            var dto = new RequestedDepartmentResponse()
            {
                Total = 2,
                Data = new List<RequestedDepartmentModel>()
            };
            dto.Data.Add(new RequestedDepartmentModel()
            {
                Code = "50027361",
                Name = "SUPPORT (G2)"
            });
            dto.Data.Add(new RequestedDepartmentModel()
            {
                Code = "50026013",
                Name = "ASIA (G1)"
            });
            return Ok(dto);
        }
        [HttpPost]
        public HttpResponseMessage CreatePurchaseRequest(CreateF2MBRequest request)
        {
            var response = new CreateF2MBResponse()
            {
                Data = new CreateF2MBModel
                {
                    ReferenceNumber = "F2-ReferenceNumber",
                    URL = "f2.com"
                }
            };
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        [HttpPost]
        public IHttpActionResult PurchaseRequestStatus(F2MBStatusRequest model)
        {
            var action = ApplicationSettings.AcademyEdocFakeAction ?? "Completed";
            var dto = new F2MBStatusResponse()
            {
                Data = new F2MBStatusModel
                {
                    Status = action
                }
            };
            return Ok(dto);
        }
        [HttpGet]
        public IHttpActionResult Suppliers()
        {
            var dto = new SupplierResponse()
            {
                Total = 2,
                Data = new List<SupplierModel>()
            };
            dto.Data.Add(new SupplierModel()
            {
                SAPCode = "300089",
                SAPName = "CÔNG TY TNHH SHINRYO VIỆT NAM"
            });
            dto.Data.Add(new SupplierModel()
            {
                SAPCode = "100145",
                SAPName = "CÔNG TY TNHH TƯ VẤN GIẢI PHÁP VIỆT NAM"
            });
            return Ok(dto);
        }
        [HttpGet]
        public IHttpActionResult Years()
        {
            var dto = new YearResponse
            {
                Total = 2,
                Data = new List<int>() { 2021, 2022 }
            };
            return Ok(dto);
        }
    }
}
