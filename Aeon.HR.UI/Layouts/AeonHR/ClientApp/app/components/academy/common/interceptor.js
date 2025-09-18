var app = angular.module('ssg');
app.service('academy.interceptor', [function (TrainingRequestService) {
    var service = this;
    service.request = function (config) {
        //Add auth token for API
        if (config.url.indexOf("/api/") > 0) {
            config.headers.secret = sr;
            config.headers.uxr = uxr;
        }

        //Build path for partial views
        if (config.url.indexOf(".partial.html") > 0) {
            config.url = "ClientApp/app/components/academy/partials/" + config.url;
        }

        return config;
    }

    service.response = function (response) {
        var config = response.config;
        if (config.url.indexOf("/api/Workflow/GetAllItemTypes") > 0) {
            response.data.object.push("AcademyTrainingRequest");
        }

        return response;
    }
}])