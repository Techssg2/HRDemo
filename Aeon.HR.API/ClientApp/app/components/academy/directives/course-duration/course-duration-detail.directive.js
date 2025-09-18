angular
  .module("ssg.directive.academy.courseDurationDetail", ["kendo.directives"])
  .directive("courseDurationDetail", function () {
    return {
      restrict: "E",
      replace: true,
      templateUrl:
        "ClientApp/app/components/academy/directives/course-duration/course-duration-detail.template.html",
      scope: {
        title: "@",
        isOfflineCourse: "=",
        courseDurationModel: "=",
        isDisabled: "=",
        onchange: "&",
      },
      link: function () {},
        controller: ["$scope", function ($scope) {

            $scope.changeFromToDate = function () {
                if ($scope.courseDurationModel.from != null && $scope.courseDurationModel.from != undefined) {
                    let id = "#trainingToDateId";
                    $(id).data('kendoDatePicker').min(new Date($scope.courseDurationModel.from));
                }
                if ($scope.courseDurationModel.to != null && $scope.courseDurationModel.to != undefined) {
                    let id = "#trainingFromDateId";
                    $(id).data('kendoDatePicker').max(new Date($scope.courseDurationModel.to));
                }
            }
        }],
    };
  });
