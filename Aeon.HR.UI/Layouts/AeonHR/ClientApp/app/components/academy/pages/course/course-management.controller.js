angular
    .module('ssg.module.academy.course-management.ctrl', ["kendo.directives"])
    .controller('CourseManagementController', function ($rootScope, $scope, $stateParams, $state, $window, $translate, $controller, appSetting,
        Notification, CourseService) {
        $controller('BaseController', { $scope: $scope });

        $scope.title = 'Course  Management';

        this.$onInit = async function () {
            $window.scrollTo(0, 0);

        }

    })