angular
    .module('ssg')
    .factory('workflowService', function($resource, $window, appSetting, $rootScope, interceptorService) {
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
                workflows: $resource(url, null, {
                    startWorkflow: {
                        method: 'POST',
                        url: url,
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Workflow", action: 'StartWorkflow' },
                        headers: customHeaders
                    },
                    getWorkflowStatus: {
                        method: 'GET',
                        url: url,
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Workflow", action: 'GetWorkflowStatusByItemId' },
                        headers: customHeaders
                    },
                    vote: {
                        method: 'POST',
                        url: url,
                        cache: true,
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Workflow", action: 'Vote' },
                        headers: customHeaders
                    },
                    getTasks: {
                        method: 'POST',
                        url: url,
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Workflow", action: 'GetTasks' },
                        headers: customHeaders
                    }
                }),
                common: $resource(url, null, {
                    delete: {
                        method: 'POST',
                        url: url,
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "common", action: 'DeleteItemById' },
                        headers: customHeaders
                    }
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