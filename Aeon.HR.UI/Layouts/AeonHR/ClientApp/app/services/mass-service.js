angular
    .module('ssg')
    .factory('massService', function($resource, $window, appSetting, $rootScope, interceptorService) {
        var _ = $window._;
        var instance = null;

        function createInstance() {
            instance = new createfactoryService();
            return instance;
        }

        function createfactoryService() {
            var url = baseUrlApi + "/:controller/:action/";
            var customHeaders = buildCustomHeader();
            return {
                mass: $resource(url, null, {
                    addCategory: {
                        method: 'POST',
                        url: url,
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Mass", action: 'CreateRecruitmentCategory' },
                        headers: customHeaders
                    },
                    updateCategory: {
                        method: 'POST',
                        url: url,
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Mass", action: 'UpdateRecruitmentCategory' },
                        headers: customHeaders
                    },
                    deleteCategory: {
                        method: 'POST',
                        url: url,
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Mass", action: 'DeleteRecruitmentCategory' },
                        headers: customHeaders
                    },
                }),
                
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