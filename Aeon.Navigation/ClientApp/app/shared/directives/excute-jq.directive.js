var ssgApp = angular.module('excuteJQModule', []);
ssgApp.directive("excutejq", [
    function () {
        return {
            restrict: "A",
            link: function ($scope, element, attr, modelCtrl) {
                $(element).click(function (ele) {
                    // debugger;

                    var $menu = $(this).find(".child-menu").slideToggle("fast");
                    $('ul.child-menu').not($menu).slideUp('fast');
                });
            },
        };
    },
]);
ssgApp.directive("addClass", [
    function () {
        return {
            restrict: "A",
            scope: {
                currentClass: "@",
                addedClass: "@"
            },
            link: function (scope, element, attr, modelCtrl) {
                $(element).click(function () {
                    var $menu = $(`.${scope.currentClass}`).addClass(`${scope.addedClass}`);
                });
                $(document).on("click", function (event) {
                    var $trigger = $(`.${scope.currentClass}`);
                    if ($trigger !== event.target && !$trigger.has(event.target).length) {
                        $trigger.removeClass(`${scope.addedClass}`);
                    }
                });
            },
        };
    },
]);
ssgApp.directive("collapseMenu", [
    function () {
        return {
            restrict: "A",
            scope: {
                currentClass: "@",
                addedClass: "@"
            },
            link: function (scope, element, attr, modelCtrl) {
                $(element).click(function () {
                    if ($('#navbar-collapse').hasClass('in')) {
                        $('#navbar-collapse').removeClass(`${scope.addedClass}`);
                    } else {
                        $('#navbar-collapse').addClass(`${scope.addedClass}`);
                    }
                });
            },
        };
    },
]);
ssgApp.directive("removePlaceholderDate", [
    function () {
        return {
            restrict: "A",
            scope: {
            },
            link: function (scope, element, attr, modelCtrl) {
                var timeOut = setTimeout(function () {
                    if (element[0].value === 'day/month/year') {
                        $(element).addClass('white-text');
                    }
                }, 0);

                $(element).focus(function () {
                    $(element).removeClass('white-text');
                });

                $(element).blur(function () {
                    //day/month/year
                    if (element.val().includes('day') || element.val().includes('month') || element.val().includes('year')) {
                        $(element).addClass('white-text');
                    }
                });
            },
        };
    },
]);

ssgApp.directive("rangeDateForFilter", function () {
    return {
        scope: {
            dFrom: "=",
            dTo: "="
        },
        template: `
      <input kendo-date-picker k-date-input="true" k-format="'dd/MM/yyyy'" ng-model="dFrom" k-on-change="onChangeDFrom()"/>
      - 
      <input kendo-date-picker k-date-input="true" k-format="'dd/MM/yyyy'" ng-model="dTo" k-on-change="onChangeDFrom()"/>
      `,
        link: function (scope, element, attr, modelCtrl) {
            scope.onChangeDFrom = function () {
                // from > to thì reset giá trị của to về null
                if (scope.dFrom && scope.dTo) {
                    let result = compare(scope.dFrom, scope.dTo);
                    if (result === 1) {
                        scope.dTo = null;
                    }
                }
            };

            function compare(dateTimeA, dateTimeB) {
                var momentA = moment(dateTimeA, "DD/MM/YYYY");
                var momentB = moment(dateTimeB, "DD/MM/YYYY");
                if (momentA > momentB) return 1;
                else if (momentA < momentB) return -1;
                else return 0;
            }
        }
    };
});

ssgApp.directive("rangeDateOfDatePicker", [
    function ($rootScope) {
        return {
            restrict: "E",
            scope: {
                dFrom: "=",
                dTo: "=",
            },
            template: `
                <input kendo-date-picker id="fromDate_id" k-date-input="true" k-format="'dd/MM/yyyy'" ng-model="dFrom" k-rebind="maxDate" k-on-change="fromDateChanged()"/>
                - 
                <input kendo-date-picker id="toDate_id" k-date-input="true" k-format="'dd/MM/yyyy'" ng-model="dTo" k-rebind="minDate" k-on-change="toDateChanged()"/>
            `,
            link: function ($scope, element, attr, modelCtrl) {
                // $scope.dFrom = attr.dFrom,
                // $scope.dTo = attr.dTo
            },
            controller: [
                "$rootScope", "$scope", '$translate',
                function ($rootScope, $scope, $translate) {
                    $scope.fromDateChanged = function () {
                        if ($scope.dFrom) {
                            $("#toDate_id").data("kendoDatePicker").min(moment($scope.dFrom, 'DD/MM/YYYY').toDate());
                            if ($scope.dTo && (moment($scope.dTo, 'DD/MM/YYYY').dayOfYear()) < (moment($scope.dFrom, 'DD/MM/YYYY').dayOfYear())) {
                                $scope.dTo = null;
                            }
                        }
                    };

                    $scope.$on('resetToDate', function (event, data) {
                        $("#toDate_id").data("kendoDatePicker").min(moment(new Date(1970, 1, 1, 0, 0, 0), 'DD/MM/YYYY').toDate());
                    });
                },

            ]
        }
    }
]);
ssgApp.directive("customDateRangePicker", [
    function ($rootScope) {
        return {
            restrict: "E",
            scope: {
                dFrom: "=",
                dTo: "=",
            },
            template: `
                <input kendo-date-picker id="customFromDate_id" k-date-input="true" k-format="'dd/MM/yyyy'" ng-model="dFrom" k-rebind="maxDate"  k-on-change="fromDateChanged()"/>
                - 
                <input kendo-date-picker id="customToDate_id" k-date-input="true" k-format="'dd/MM/yyyy'" ng-model="dTo" k-rebind="minDate"/>
            `,
            link: function ($scope, element, attr, modelCtrl) {
                // $scope.dFrom = attr.dFrom,
                // $scope.dTo = attr.dTo
            },
            controller: [
                "$rootScope", "$scope", '$translate',
                function ($rootScope, $scope, $translate) {
                    $scope.fromDateChanged = function () {
                        if ($scope.dFrom) {
                            $("#customToDate_id").data("kendoDatePicker").min(moment($scope.dFrom, 'DD/MM/YYYY').toDate());
                            if ($scope.dTo && (moment($scope.dTo, 'DD/MM/YYYY').dayOfYear()) < (moment($scope.dFrom, 'DD/MM/YYYY').dayOfYear())) {
                                $scope.dTo = null;
                            }
                        }
                    };

                    $scope.$on('resetToDate', function (event, data) {
                        $("#customToDate_id").data("kendoDatePicker").min(moment(new Date(1970, 1, 1, 0, 0, 0), 'DD/MM/YYYY').toDate());
                    });
                },

            ]
        }
    }
]);

ssgApp.directive("rangeDateForFilterKendoModel", function () {
    return {
        scope: {
            dFrom: "=",
            dTo: "="
        },
        template: `
      <input kendo-date-picker k-date-input="true" k-format="'dd/MM/yyyy'" k-ng-model="dFrom" k-on-change="onChangeDFrom()"/>
      - 
      <input kendo-date-picker k-date-input="true" k-format="'dd/MM/yyyy'" k-ng-model="dTo" k-on-change="onChangeDFrom()"/>
      `,
        link: function (scope, element, attr, modelCtrl) {
            scope.onChangeDFrom = function () {
                // from > to thì reset giá trị của to về null
                if (scope.dFrom && scope.dTo) {
                    let result = compare(scope.dFrom, scope.dTo);
                    if (result === 1) {
                        //scope.dTo = null;
                        scope.dTo = scope.dFrom;
                    }
                }
            };

            function compare(dateTimeA, dateTimeB) {
                var momentA = moment(dateTimeA);
                var momentB = moment(dateTimeB);
                if (momentA > momentB) return 1;
                else if (momentA < momentB) return -1;
                else return 0;
            }
        }
    };
});

ssgApp.directive("restrictInput", [
    function () {
        return {
            restrict: "A",
            require: 'ngModel',
            scope: {
            },
            link: function (scope, element, attr, modelCtrl) {
                modelCtrl.$parsers.unshift(function (viewValue) {
                    var options = scope.$eval(attr.restrictInput);
                    if (!options.regex && options.type) {
                        options.regex = '^[0-9]*$';
                        //   switch (options.type) {
                        //     case 'digitsOnly': options.regex = '^[0-9]*$'; break;
                        //     case 'lettersOnly': options.regex = '^[a-zA-Z]*$'; break;
                        //     case 'lowercaseLettersOnly': options.regex = '^[a-z]*$'; break;
                        //     case 'uppercaseLettersOnly': options.regex = '^[A-Z]*$'; break;
                        //     case 'lettersAndDigitsOnly': options.regex = '^[a-zA-Z0-9]*$'; break;
                        //     case 'validPhoneCharsOnly': options.regex = '^[0-9 ()/-]*$'; break;
                        //     default: options.regex = '';
                        //   }
                    }
                    var reg = new RegExp(options.regex);
                    if (reg.test(viewValue)) { //if valid view value, return it
                        return viewValue;
                    } else { //if not valid view value, use the model value (or empty string if that's also invalid)
                        var overrideValue = (reg.test(modelCtrl.$modelValue) ? modelCtrl.$modelValue : '');
                        element.val(overrideValue);
                        return overrideValue;
                    }
                });
            },
        };
    },
]);