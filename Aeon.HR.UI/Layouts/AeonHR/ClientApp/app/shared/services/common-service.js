angular
    .module('ssg')
    .factory('commonService', function ($resource, $window, appSetting, $rootScope, interceptorService) {
        var _ = $window._;
        var instance = null;

        function createInstance() {
            instance = new createfactoryService();
            return instance;
        }

        function createfactoryService() {
            var url = baseUrlApi + "/:controller/:action/:id";
            var customHeaders = buildCustomHeader();
            return {
                users: $resource(url, null, {
                    getCreatedByUser: {
                        method: 'POST',
                        interceptor: interceptorService.getInstance().interceptor,
                        params: {
                            controller: "User",
                            action: 'GetUserById'
                        },
                        headers: customHeaders
                    },
                })
            }
        }
        return {
            getInstance() {
                if (!instance) {
                    instance = new createInstance();
                }
                return instance;
            }
        }
    });

