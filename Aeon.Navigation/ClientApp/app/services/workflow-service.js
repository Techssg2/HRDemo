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
                    },
                    syncWorkflowById: {
                        method: 'GET',
                        url: url,
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Workflow", action: 'SyncWorkflowById' },
                        headers: customHeaders
                    },
                    updateAssignToByItemId: {
                        method: 'POST',
                        url: url,
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Workflow", action: 'UpdateAssignToByItemId' },
                        headers: customHeaders
                    },
                    changeStepWorkflow: {
                        method: 'POST',
                        url: url,
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Workflow", action: 'ChangeStepWorkflow' },
                        headers: customHeaders
                    },
                    voteAdminBTA: {
                        method: 'POST',
                        url: url,
                        cache: true,
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "Workflow", action: 'VoteAdminBTA' },
                        headers: customHeaders
                    },
                }),
                common: $resource(url, null, {
                    delete: {
                        method: 'POST',
                        url: url,
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "common", action: 'DeleteItemById' },
                        headers: customHeaders
                    },
                    updateStatusByReferenceNumber: {
                        method: 'POST',
                        url: url,
                        interceptor: interceptorService.getInstance().interceptor,
                        params: { controller: "common", action: 'UpdateStatusByReferenceNumber' },
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