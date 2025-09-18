var ssgApp = angular.module('ssg.directiveBTAModule', []);
ssgApp.directive("searchFlight", [
    function () {
        return {
            restrict: "E",
            templateUrl: "ClientApp/app/shared/directives/bta/search-flight.template.html?v=" + edocV,
            scope: {
                searchTicketScope: "=",
            },
            controller: (["$rootScope", "$scope", "bookingFlightService", "$timeout", "$stateParams"],
                function ($rootScope, $scope, bookingFlightService, $timeout, $stateParams) {

                    //New Code
                    //Define
                    $scope.searchTicketModel = {};
                    $scope.infoSearchFlight = {};
                    $scope.dataSearch = {};
                    $scope.filterData = {};
                    $scope.selectFlight = {};
                    $scope.dateSearch = {
                        isNextDate: true,
                        isPreviousDate: true,
                        flightType: "",
                        routeType: "",
                        isReturn: false,
                        bookingGroupId: 0,
                        isBooking: true,
                        maxJobGrade: 0,
                    };
                    $scope.stopsOptions = {
                        dataTextField: "text",
                        dataValueField: "id",
                        placeholder: "All",
                        valuePrimitive: true,
                        checkboxes: false,
                        autoBind: true,
                        filter: "contains",
                        change: async function (e) {
                            kendo.ui.progress($("#loading_bta"), true);
                            var stopValue = this.value();
                            $scope.filterData.stopData = stopValue;
                            $scope.dateSearch.isSearch = false;
                            $scope.dataSearch.groupPricedItineraries = []
                            var res = await bookingFlightService.filterAvailability(!$scope.dateSearch.isReturn ? $scope.dataSearch.searchId : $scope.dataSearch.returnSearchId, 0, $scope.dateSearch.maxJobGrade, $scope.filterData.airlineData, stopValue);
                            $scope.dataSearch.groupPricedItineraries = res.groupPricedItineraries === null ? [] : res.groupPricedItineraries;
                            $scope.dataSearch.pagingInfo = res.page === null ? {} : res.page;
                            setDataSource(res.airlinesGroup, "#airlines");
                            $scope.dateSearch.isSearch = true;
                            $scope.$apply();
                            kendo.ui.progress($("#loading_bta"), false);
                        }
                    }
                    $scope.airlinesOptions = {
                        dataTextField: "text",
                        dataValueField: "id",
                        placeholder: "Airlines",
                        valuePrimitive: true,
                        checkboxes: false,
                        autoBind: true,
                        filter: "contains",
                        change: async function (e) {
                            kendo.ui.progress($("#loading_bta"), true);
                            var airlineValue = this.value();
                            $scope.filterData.airlineData = airlineValue;
                            $scope.dateSearch.isSearch = false;
                            $scope.dataSearch.groupPricedItineraries = [];
                            var res = await bookingFlightService.filterAvailability(!$scope.dateSearch.isReturn ? $scope.dataSearch.searchId : $scope.dataSearch.returnSearchId, 0, $scope.dateSearch.maxJobGrade, airlineValue, $scope.filterData.stopData);
                            $scope.dataSearch.groupPricedItineraries = res.groupPricedItineraries === null ? [] : res.groupPricedItineraries;
                            $scope.dataSearch.pagingInfo = res.page === null ? {} : res.page;
                            setDataSource(res.airlinesGroup, "#airlines");
                            $scope.dateSearch.isSearch = true;
                            $scope.$apply();
                            kendo.ui.progress($("#loading_bta"), false);
                        }
                    }
                    $scope.stops = [
                        {
                            id: 0,
                            text: "Direct flights"
                        },
                        {
                            id: 1,
                            text: "Transit flights"
                        }
                    ];
                    $scope.error = {
                        isSuccess: true,
                        message: ""
                    };
                    //Code
                    $scope.clearData = function () {
                        $scope.infoSearchFlight = {};
                        $scope.dataSearch = {};
                        $scope.filterData = {};
                        $scope.selectFlight = {};
                        $scope.dateSearch = {
                            isNextDate: true,
                            isPreviousDate: true,
                            flightType: "",
                            routeType: "",
                            isReturn: false,
                            bookingGroupId: 0,
                            isBooking: true,
                            isSearch: false,
                        };
                        setDataSource([], "#stopsId");
                        setDataSource([], "#airlines");
                        $scope.error = {
                            isSuccess: true,
                            errorMessage: ""
                        };
                    }

                    var setDataSource = function (dataAirlines, idControl) {
                        var dataSource = new kendo.data.HierarchicalDataSource({
                            data: dataAirlines,
                            serverFiltering: true
                        });
                        var dropdownAirlines = $(idControl).data("kendoMultiSelect");
                        if (dropdownAirlines) {
                            dropdownAirlines.setDataSource(dataSource);
                        }
                    }

                    $scope.showFareRule = async function ($event, fareSourceCode, groupId) {
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
                                    "searchId": !$scope.dateSearch.isReturn ? $scope.dataSearch.departureSearchId : $scope.dateSearch.returnSearchId
                                };

                                var getFareRules = await bookingFlightService.getFareRules(queryArgs);
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
                            }

                            targetdd.css('display', 'block').removeClass('fadeOutUp').addClass('fadeInDown')
                                .on('animationend webkitAnimationEnd oanimationend MSAnimationEnd', function () {
                                    angular.element(this).show();
                                });

                        }
                    }

                    $scope.setAccordion = function (currentVal) {
                        if (currentVal === $scope.accordion) {
                            $scope.accordion = 0;
                        } else {
                            $scope.accordion = 0;
                            $scope.accordion = currentVal
                        }
                    }

                    $scope.selectTicket = async function (sequenceNumber, groupId) {
                        $scope.filterData.airlineData = [];
                        $scope.filterData.stopData = [];
                        $scope.dataSearch.pagingInfo = {};
                        var selectFlight = $scope.dataSearch.groupPricedItineraries.filter(x => x.groupId === groupId);
                        var myData = angular.copy(selectFlight)
                        if (selectFlight && selectFlight.length > 0) {
                            var currentSelect = selectFlight[0];
                            var currentSequenceNumber = currentSelect.pricedItineraries.filter(x => x.sequenceNumber === sequenceNumber);
                            if (currentSequenceNumber && currentSequenceNumber.length > 0) {
                                currentSelect.pricedItineraries = currentSequenceNumber[0];

                                //get Service Free
                                var fightInfoServices = new aeon.flightInfo(currentSelect);
                                if (!$scope.dateSearch.isReturn) {
                                    currentSelect.directFlight = true;
                                    $scope.dateSearch.isReturn = true;
                                    currentSelect.IncludeEquivfare = fightInfoServices.getTicketFee();
                                    $scope.selectFlight.directFlight = selectFlight[0];
                                } else {
                                    currentSelect.directFlight = false;
                                    //If isReturn is true and filghtType is International
                                    //ticketFree is 0
                                    currentSelect.IncludeEquivfare = 0;
                                    if ($scope.dateSearch.flightType === "Domestic") {
                                        currentSelect.IncludeEquivfare = fightInfoServices.getTicketFee();
                                    }
                                    $scope.selectFlight.returnFlight = selectFlight[0];
                                }
                            }
                        }

                        if ($scope.dateSearch.routeType === "ROUNDTRIP") {
                            if ($scope.dateSearch.isReturn && $.isEmptyObject($scope.selectFlight.returnFlight)) {
                                setDataSource($scope.stops, "#stopsId");
                                var loadFlight = bookingFlightService.loadFlight($scope.searchTicketModel, $scope.dateSearch.isReturn);
                                $scope.loadInfo(loadFlight);
                                $scope.dataSearch.groupPricedItineraries = [];
                                $scope.dateSearch.isSearch = false;
                                if ($scope.dateSearch.flightType === "Domestic") {
                                    //Process ticket return
                                    kendo.ui.progress($("#loading_bta"), true);
                                    var res = await bookingFlightService.filterAvailability($scope.dataSearch.returnSearchId, 0, $scope.dateSearch.maxJobGrade);
                                    $scope.dataSearch.groupPricedItineraries = res.groupPricedItineraries === null ? [] : res.groupPricedItineraries;
                                    $scope.dataSearch.pagingInfo = res.page === null ? {} : res.page;
                                    setDataSource(res.airlinesGroup, "#airlines");
                                    $scope.dateSearch.isSearch = true;
                                    $scope.$apply();
                                    kendo.ui.progress($("#loading_bta"), false);
                                } else {
                                    $scope.dateSearch.isBooking = false;
                                    var returnInternational = myData.filter(x => x.groupId === groupId);
                                    if (returnInternational && returnInternational.length > 0) {
                                        kendo.ui.progress($("#loading_bta"), true);
                                        $timeout(function () {
                                            $scope.dataSearch.groupPricedItineraries = returnInternational;
                                            $scope.dataSearch.pagingInfo = {
                                                nextPageNumber: 0,
                                                offset: 1,
                                                pageNumber: 0,
                                                previousPageNumber: -1,
                                                totalElements: returnInternational.length,
                                                totalPage: 1
                                            }
                                            kendo.ui.progress($("#loading_bta"), false);
                                        }, 2000);
                                    }

                                }
                            } else {
                                $scope.searchTicketScope.setTripGroupHasTicket($scope.dateSearch.bookingGroupId, $scope.selectFlight);
                            }
                        }
                        else {
                            $scope.searchTicketScope.setTripGroupHasTicket($scope.dateSearch.bookingGroupId, $scope.selectFlight);
                        }
                    }

                    $scope.nextDate = async function () {
                        $scope.error = {
                            isSuccess: true,
                            message: ""
                        };
                        var newDate = new Date($scope.infoSearchFlight.dateSearch);
                        newDate.setDate(newDate.getDate() + 1);
                        $scope.dateSearch.isNextDate = true;
                        $scope.dateSearch.isPreviousDate = true;
                        $scope.infoSearchFlight.dateSearch = newDate;
                        if (newDate.getTime() > new Date($scope.dateSearch.currentDate).getTime()) {
                            $scope.dateSearch.isNextDate = false;
                        }

                        if (!$scope.dateSearch.isReturn) {
                            $scope.searchTicketModel.DepartureDate = bookingFlightService.formatDate(newDate);
                        } else {
                            $scope.searchTicketModel.ReturntureDate = bookingFlightService.formatDate(newDate);
                        }
                        await $scope.loadFlight($scope.searchTicketModel, false);
                    }

                    $scope.previousDate = async function () {
                        $scope.error = {
                            isSuccess: true,
                            message: ""
                        };
                        var newDate = new Date($scope.infoSearchFlight.dateSearch);
                        newDate.setDate(newDate.getDate() - 1);
                        $scope.infoSearchFlight.dateSearch = newDate;
                        $scope.dateSearch.isPreviousDate = true;
                        $scope.dateSearch.isNextDate = true;
                        if (newDate.getTime() < new Date($scope.dateSearch.currentDate).getTime()) {
                            $scope.dateSearch.isPreviousDate = false;
                        }
                        if (!$scope.dateSearch.isReturn) {
                            $scope.searchTicketModel.DepartureDate = bookingFlightService.formatDate(newDate);
                        } else {
                            $scope.searchTicketModel.ReturntureDate = bookingFlightService.formatDate(newDate);
                        }
                        await $scope.loadFlight($scope.searchTicketModel, false);
                    }

                    $scope.pagingFlight = async function (pageNum) {
                        $scope.error = {
                            isSuccess: true,
                            message: ""
                        };
                        kendo.ui.progress($("#loading_bta"), true);
                        $scope.dateSearch.isSearch = false;
                        $scope.dataSearch.groupPricedItineraries = [];
                        var res = await bookingFlightService.filterAvailability(!$scope.dateSearch.isReturn ? $scope.dataSearch.searchId : $scope.dataSearch.returnSearchId, pageNum, $scope.dateSearch.maxJobGrade, $scope.filterData.airlineData, $scope.filterData.stopData);
                        $scope.dataSearch.groupPricedItineraries = res.groupPricedItineraries === null ? [] : res.groupPricedItineraries;
                        $scope.dataSearch.pagingInfo = res.page === null ? {} : res.page;
                        setDataSource(res.airlinesGroup, "#airlines");
                        $scope.dateSearch.isSearch = true;
                        $scope.$apply();
                        kendo.ui.progress($("#loading_bta"), false);
                    }

                    $scope.loadInfo = function (infoData) {
                        $scope.infoSearchFlight.originCode = infoData.originCode;
                        $scope.infoSearchFlight.originCity = infoData.originCity;
                        $scope.infoSearchFlight.destinationCode = infoData.destinationCode;
                        $scope.infoSearchFlight.destinationCity = infoData.destinationCity;
                        $scope.infoSearchFlight.dateSearch = infoData.dateSearch;
                        $scope.infoSearchFlight.numberAdult = infoData.numberAdult;
                        $scope.infoSearchFlight.isReturn = infoData.isReturn
                        $scope.dateSearch.currentDate = infoData.dateSearch;
                    }

                    $scope.dataFlight = function (infoFlight) {
                        $scope.dataSearch.searchId = infoFlight.searchId;
                        $scope.dataSearch.departureSearchId = infoFlight.departureSearchId;
                        $scope.dataSearch.returnSearchId = infoFlight.returnSearchId;
                        $scope.dataSearch.groupPricedItineraries = infoFlight.groupPricedItineraries === null ? [] : infoFlight.groupPricedItineraries;
                        $scope.dataSearch.pagingInfo = infoFlight.pagingInfo;
                        $scope.dateSearch.isSearch = true;
                    }

                    $scope.loadFlight = async function (searchTicketModel) {
                        if (!$.isEmptyObject(searchTicketModel)) {
                            kendo.ui.progress($("#loading_bta"), true);
                            $scope.clearData();
                            $scope.searchTicketModel = searchTicketModel;
                            $scope.dateSearch.flightType = searchTicketModel.FlightType;
                            $scope.dateSearch.routeType = searchTicketModel.RouteType;
                            $scope.dateSearch.bookingGroupId = searchTicketModel.GroupId;
                            $scope.dateSearch.maxJobGrade = searchTicketModel.JobGrade;
                            setDataSource($scope.stops, "#stopsId");

                            //Set Stops
                            var loadFlight = bookingFlightService.loadFlight(searchTicketModel, $scope.dateSearch.isReturn);
                            $scope.loadInfo(loadFlight);
                            $scope.dateSearch.isSearch = false;
                            var searchFlight = await bookingFlightService.searchFlight(searchTicketModel, false, $stateParams.id);

                            if (searchFlight.isSuccess) {
                                $scope.dataFlight(searchFlight);
                            }
                            else {
                                $scope.error.isSuccess = false;
                                $scope.error.message = searchFlight.message;
                            }
                            kendo.ui.progress($("#loading_bta"), false);

                            //Set Airlines
                            setDataSource(searchFlight.airlinesGroup, "#airlines");
                        }
                    }

                    function ngInit() {
                        $scope.searchTicketScope.loadFlight = $scope.loadFlight;
                    }

                    ngInit();

                })
        }
    }
]);