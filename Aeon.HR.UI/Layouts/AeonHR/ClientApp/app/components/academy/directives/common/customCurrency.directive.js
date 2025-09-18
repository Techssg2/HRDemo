"use strict";
var ssg = angular.module("ssg.module.academy.currency.directive", []);
ssg.directive("asCurrency", function ($filter) {
  return {
    restrict: "A",
    require: "?ngModel",
    link: function (scope, elem, attrs, ctrl) {
      if (!ctrl) return;

      var sanitize = function (s) {
        return s.replace(/[^\d|\-+|\.+]/g, "");
      };

      var convert = function () {
        var plain = sanitize(ctrl.$viewValue);
        ctrl.$setViewValue($filter("currency")(plain, "", 0));
        ctrl.$render();
      };

      elem.on("blur", convert);

      ctrl.$formatters.push(function (a) {
        return $filter("currency")(a, "", 0);
      });

      ctrl.$parsers.push(function (a) {
        return sanitize(a);
      });
    },
  };
});
