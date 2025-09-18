var ssgApp = angular.module('ssg.directiveBTAFlightTicketModule', []);
ssgApp.directive("flightTicket", [
    function () {
        return {
            restrict: "E",
            templateUrl: "ClientApp/app/shared/directives/bta/flight-ticket/flight-ticket.template.html?v=" + edocV,
            scope: {
                viewTicketScope: "="
            },
            controller: (["$rootScope", "$scope", "$translate", "$timeout", "bookingFlightService"],
                function ($rootScope, $scope, $translate, $timeout, bookingFlightService) {

                    $scope.setAccordion = function (currentVal, direcFlight) {
                        if (currentVal === $scope.accordion && $scope.directFlight === direcFlight) {
                            $scope.accordion = 0;
                        } else {
                            $scope.accordion = 0;
                            $scope.accordion = currentVal;
                            $scope.directFlight = direcFlight;
                        }
                    }

                    $scope.getFareRules = async function (queryArgs, targetdd) {
                        var getFareRules = await bookingFlightService.getFareRules(queryArgs);
                        if (!getFareRules || (getFareRules && Array.isArray(getFareRules))) {
                            // call tu api
                            $.each(getFareRules, function (key, val) {
                                if (val.fareRuleItems.length > 0) {
                                    var htmlRule = "";
                                    $.each(val.fareRuleItems, function (keyRule, valRule) {
                                        htmlRule += "<div class='title-rule'><span class='text-title-rule'>" + valRule.title + "</span></div>";
                                        htmlRule += "<div class='detail-rule'><span class='text-detail-rule'>" + valRule.detail + "</span></div>";
                                    });
                                    targetdd.find(".dropdown-body").html(htmlRule);
                                }
                            });
                        } else {
                            // call tu database
                            var htmlRule = "<div class='title-rule'><span class='text-title-rule'>" + getFareRules.title + "</span></div>";
                            htmlRule += "<div class='detail-rule'><span class='text-detail-rule'>" + getFareRules.detail + "</span></div>";
                            targetdd.find(".dropdown-body").html(htmlRule);
                        }

                        targetdd.css('display', 'block').removeClass('fadeOutUp').addClass('fadeInDown')
                            .on('animationend webkitAnimationEnd oanimationend MSAnimationEnd', function () {
                                angular.element(this).show();
                            });
                        //var res = await btaService.getInstance().bussinessTripApps.fareRules(queryArgs).$promise;
                        //if (res && res.isSuccess) {
                        //    $.each(res.object.fareRules, function (key, val) {
                        //        if (val.fareRuleItems.length > 0) {
                        //            var htmlRule = "";
                        //            $.each(val.fareRuleItems, function (keyRule, valRule) {
                        //                htmlRule += "<div class='title-rule'><span class='text-title-rule'>" + valRule.title + "</span></div>";
                        //                htmlRule += "<div class='detail-rule'><span class='text-detail-rule'>" + valRule.detail + "</span></div>";
                        //            });
                        //            targetdd.find(".dropdown-body").html(htmlRule);
                        //        }
                        //    });
                        //    targetdd.css('display', 'block').removeClass('fadeOutUp').addClass('fadeInDown')
                        //        .on('animationend webkitAnimationEnd oanimationend MSAnimationEnd', function () {
                        //            angular.element(this).show();
                        //        });
                        //}
                    }

                    $scope.showFareRule = function ($event, fareSourceCode, groupId, searchId) {
                        var targetdd = angular.element($event.target).closest('.baggage-parent').find('.dropdown-menu');
                        if (targetdd.hasClass('fadeInDown')) {
                            targetdd
                                .removeClass('fadeInDown')
                                .addClass('fadeOutUp')
                                .on('animationend webkitAnimationEnd oanimationend MSAnimationEnd', function () {
                                    angular.element(this).hide();
                                });
                        } else {
                            if (targetdd.find(".dropdown-body").html() === "") {
                                var queryArgs = {
                                    "fareSourceCode": fareSourceCode,
                                    "groupId": groupId,
                                    "searchId": searchId
                                };
                                $scope.getFareRules(queryArgs, targetdd);
                            } else {
                                targetdd.css('display', 'block').removeClass('fadeOutUp').addClass('fadeInDown')
                                    .on('animationend webkitAnimationEnd oanimationend MSAnimationEnd', function () {
                                        angular.element(this).show();
                                    });
                            }

                        }
                    }

                    $scope.showTicket = function (viewTicketScope) {
                        if (!$.isEmptyObject(viewTicketScope)) {
                            var myData = []
                            var sumAmount = 0;
                            $.each(viewTicketScope, function (key, val) {
                                if (key === 0) {
                                    $scope.flightType = val.flightType;
                                    $scope.adutsQtt = val.pricedItineraries.airItineraryPricingInfo.adultFare.passengerTypeQuantities.quantity;
                                }

                                var fightInfoServices = new aeon.flightInfo(val);
                                if ($scope.flightType === 'DOMESTIC') {

                                    sumAmount += val.pricedItineraries.airItineraryPricingInfo.itinTotalFare.totalFare.amount + (fightInfoServices.getTicketFee() * $scope.adutsQtt);
                                } else {
                                    if (key === viewTicketScope.length - 1) {
                                        sumAmount += val.pricedItineraries.airItineraryPricingInfo.itinTotalFare.totalFare.amount + (fightInfoServices.getTicketFee() * $scope.adutsQtt);
                                    }
                                }

                                myData.push(val);
                            });
                            $scope.sumAmount = sumAmount;
                            $scope.groupPricedItineraries = myData;

                            //if (viewTicketScope.length > 0) {
                            //    $.each(viewTicketScope, function (key, val) {
                            //        if (key === 0) {
                            //            $scope.flightType = val.flightType;

                            //        }
                            //        if ($scope.flightType === 'DOMESTIC') {
                            //            $scope.adutsQtt = val.pricedItineraries.airItineraryPricingInfo.adultFare.passengerTypeQuantities.quantity;
                            //            sumAmount += val.pricedItineraries.airItineraryPricingInfo.itinTotalFare.totalFare.amount;
                            //        }

                            //        myData.push(val);
                            //    });
                            //    $scope.sumAmount = sumAmount;
                            //    $scope.groupPricedItineraries = myData;
                            //}
                            //else {
                            //    $scope.groupPricedItineraries = viewTicketScope;
                            //}

                        }
                    }

                    function ngInit() {
                        $scope.viewTicketScope.showTicket = $scope.showTicket;
                    }

                    ngInit();
                })
        }
    }
]);