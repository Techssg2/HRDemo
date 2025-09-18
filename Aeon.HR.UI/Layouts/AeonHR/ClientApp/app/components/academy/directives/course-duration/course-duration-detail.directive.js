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
      controller: ["$scope", function ($scope) {}],
    };
  });
