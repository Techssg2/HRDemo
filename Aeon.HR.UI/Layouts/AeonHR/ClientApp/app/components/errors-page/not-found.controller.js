var ssgApp = angular.module('ssg.errorModule', ["kendo.directives"]);
ssgApp.controller('notPageFoundController', function ($rootScope, $scope, $location, settingService, appSetting, localStorageService, $timeout, $state, Notification) {
    // create a message to display in our view
    var ssg = this;
    $scope.errorPermission = '';
    if ($rootScope.permissionErrorMessage) {
        // Notification.error({ message: $rootScope.permissionErrorMessage, delay: 15000 });
        $scope.errorPermission = $rootScope.permissionErrorMessage;
        sessionStorage.removeItem('currentUser');
        sessionStorage.removeItem('departments');
        localStorageService.remove("departments");
        localStorageService.set("invalidPermission", true);


    } else {
        $scope.errorPermission = '';
        if (!sessionStorage.currentUser) {
            localStorageService.set("invalidPermission", false);
            window.location.href = baseUrl + "_layouts/15/SignOut.aspx";
        }
    }

});