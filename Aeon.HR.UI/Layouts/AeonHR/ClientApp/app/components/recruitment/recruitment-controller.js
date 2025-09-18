var ssgApp = angular.module('ssg.recruitmentModule', ["kendo.directives"]);
ssgApp.controller('recruitmentController', function($rootScope, $scope, $location, appSetting) {
    // create a message to display in our view
    var ssg = this;
    $scope.title = 'Recruitment';
    $rootScope.isParentMenu = true;
});