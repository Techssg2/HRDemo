var ssgApp = angular.module('ssg.accessDeniedModule', ["kendo.directives"]);
ssgApp.controller('accessDeniedController', function ($rootScope, $scope, localStorageService, $timeout) {
    $scope.errorAccessDenied = '401 - ACCESS DENIED';
});