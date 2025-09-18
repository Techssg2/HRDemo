var ssgApp = angular.module('ssg.cbModule', []);
ssgApp.controller('cbController', function($rootScope, $scope, $location, appSetting) {
    // create a message to display in our view
    var ssg = this;
    $scope.title = 'C&B';
    $rootScope.isParentMenu = true;
});