var ssgApp = angular.module('ssg.directiveNotiModule', []);
ssgApp.directive("notification", [
    function() {
        return {
            restrict: "E" ,
            templateUrl: "ClientApp/app/shared/directives/notification/notification.template.html?v=" + edocV,
            scope: {
                currentUser: "="
            },
            link: function($scope, element, attr, modelCtrl) {},
            controller: [
                "$rootScope", "$scope",
                function($rootScope, $scope) {
                    $scope.vm = {
                        currentUser: $scope.currentUser
                    };

                    $scope.notifications = [
                        {
                            name: 'PAYMENT REQUESTS',
                            childrens: [
                                { nameTask: 'PR 01', dateTask: moment(new Date(1994, 11, 24)).format('DD/MM/YYYY') },
                                { nameTask: 'PR 02', dateTask: moment(new Date(1994, 11, 24)).format('DD/MM/YYYY') }
                            ]
                        },
                        {
                            name: 'PURCHASING REQUESTS',
                            childrens: [
                                { nameTask: 'Purpose/Reason', dateTask: moment(new Date(1994, 11, 24)).format('DD/MM/YYYY') },
                                { nameTask: 'F2-6-Submit sau fix', dateTask: moment(new Date(1994, 11, 24)).format('DD/MM/YYYY') }
                            ]
                        }
                    ]

                    console.log($scope.notifications[0].childrens.length);






                },
            ],
        };
    },
]);