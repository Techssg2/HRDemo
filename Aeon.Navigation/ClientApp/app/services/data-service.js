angular
    .module('ssg')
    .factory('dataService', [function () {
        return {
            workflowStatus: {},
            permission: { right: 0 },
            trackingHistory: []
        };
    }]);