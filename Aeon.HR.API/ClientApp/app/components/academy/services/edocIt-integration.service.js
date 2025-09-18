angular
    .module("ssg.module.academy.edocIt-integration.svc", [])
    .factory("EdocItIntegrationService", function ($resource) {
        return $resource(null, null, {
            getDepartmentByLoginName: {
                method: "POST",
                isArray: true,
                url: baseUrlApi + "/EdocItIntegration/GetDepartmentByLoginName",
            },
            getDepartmentDIC: {
                method: "POST",
                isArray: true,
                url: baseUrlApi + "/EdocItIntegration/GetDepartmentDIC",
            },
        });
    });