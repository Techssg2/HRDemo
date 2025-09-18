angular
  .module("ssg.module.academy.f2-integration.svc", [])
  .factory("F2IntegrationService", function ($resource) {
    return $resource(null, null, {
      getDepartmentInCharges: {
        method: "GET",
        isArray: true,
        url: baseUrlApi + "/F2Integration/GetDepartmentInCharges",
      },
      getBudgetInformations: {
        method: "GET",
        isArray: true,
        url: baseUrlApi + "/F2Integration/GetBudgetInformations",
      },
      getRequestedDepartments: {
        method: "GET",
        isArray: true,
        url: baseUrlApi + "/F2Integration/GetRequestedDepartments",
      },
      getSuppliers: {
        method: "GET",
        isArray: true,
        url: baseUrlApi + "/F2Integration/GetSuppliers",
      },
      getYears: {
        method: "GET",
        isArray: true,
        url: baseUrlApi + "/F2Integration/GetYears",
      },
    });
  });
