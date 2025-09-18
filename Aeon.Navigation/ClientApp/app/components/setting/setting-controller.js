var ssgApp = angular.module('ssg.settingModules', []);
ssgApp.controller('settingController', function($rootScope, $scope, $location, appSetting) {
    // create a message to display in our view
    var ssg = this;
    $scope.title = 'Setting';
    $rootScope.isParentMenu = true;
});