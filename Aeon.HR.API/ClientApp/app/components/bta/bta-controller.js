var ssgApp = angular.module('ssg.btaModule', []);
ssgApp.controller('btaController', function($rootScope, $scope, $location, appSetting) {
    // create a message to display in our view
    var ssg = this;
    $scope.title = 'BTA';
    $rootScope.isParentMenu = true;
});