var ssgApp = angular.module('ssg.directiveBTAModule', []);
ssgApp.directive("searchFlight", [
    function () {
        return {
            restrict: "E",
            templateUrl: "ClientApp/app/shared/directives/bta/search-flight.template.html?v=" + edocV,
            scope: {
                searchTicketScope: "=",
            },
            controller: (["$rootScope", "$scope", "bookingFlightService", "$timeout", "$stateParams", "$translate"],
                function ($rootScope, $scope, bookingFlightService, $timeout, $stateParams, $translate) {

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
                            if ($scope.dateSearch.isReturn && $scope.dateSearch.routeType === "ROUNDTRIP" && $scope.dateSearch.flightType === "International") {
                                // BUG: chọn Fillter vé về đi quốc tế
                                var res = await bookingFlightService.groupIntinerary($scope.dataSearch.departureSearchId, $scope.groupIdSelect, $scope.selectFlight.directFlight.fareSourceCode, 0, $scope.dateSearch.maxJobGrade);
                                $scope.dataSearch.groupPricedItineraries = res.groupPricedItineraries === null ? [] : res.groupPricedItineraries;
                                var draft = [];
                                if ($scope.dataSearch.groupPricedItineraries.length > 0) {
                                    $scope.dataSearch.groupPricedItineraries.forEach(item => {
                                        if (item.pricedItineraries.length > 0) {
                                            draft = item.pricedItineraries.reduce((a, c) => (a[c.originDestinationOptions[1].destinationDateTime] = (a[c.originDestinationOptions[1].destinationDateTime] || []).concat(c), a), {});
                                        }
                                    });
                                }
                                var groupPricedItineraries = [];
                                if (draft) {
                                    var index = 0;
                                    Object.entries(draft).forEach(([key, val]) => {
                                        groupPricedItineraries.push(copy($scope.dataSearch.groupPricedItineraries[0]));
                                        groupPricedItineraries[index].id = null;
                                        groupPricedItineraries[index].pricedItineraries = val;
                                        groupPricedItineraries[index].arrivalDateTime = val[0].originDestinationOptions[1].originDateTime;
                                        groupPricedItineraries[index].departureDateTime = val[0].originDestinationOptions[1].destinationDateTime;
                                        groupPricedItineraries[index].returnDateTime = val[0].originDestinationOptions[1].destinationDateTime;
                                        //groupPricedItineraries[index].index = index;
                                        index++;
                                    });
                                }
                                if (groupPricedItineraries) {
                                    $.each(groupPricedItineraries, function (key, val) {
                                        if (val.pricedItineraries && val.pricedItineraries[0] != undefined) {
                                            val.pricedItineraries = [val.pricedItineraries[0]];
                                        }
                                    })
                                }
                                $scope.dataSearch.groupPricedItineraries = groupPricedItineraries;
                                $scope.dataSearch.pagingInfo = res.page === null ? {} : res.page;
                            } else {
                                var res = await bookingFlightService.filterAvailability(!$scope.dateSearch.isReturn ? $scope.dataSearch.searchId : $scope.dataSearch.returnSearchId, 0, $scope.dateSearch.maxJobGrade, $scope.filterData.airlineData, stopValue);
                                $scope.dataSearch.groupPricedItineraries = res.groupPricedItineraries === null ? [] : res.groupPricedItineraries;
                                if ($scope.dataSearch.groupPricedItineraries) {
                                    $.each($scope.dataSearch.groupPricedItineraries, function (key, val) {
                                        if (val.pricedItineraries && val.pricedItineraries[0] != undefined) {
                                            val.pricedItineraries = [val.pricedItineraries[0]];
                                        }
                                    })
                                }
                                $scope.dataSearch.pagingInfo = res.page === null ? {} : res.page;
                            }
                            
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
                            if ($scope.dateSearch.isReturn && $scope.dateSearch.routeType === "ROUNDTRIP" && $scope.dateSearch.flightType === "International") {
                                // BUG: chọn Fillter vé về đi quốc tế
                                var res = await bookingFlightService.groupIntinerary($scope.dataSearch.departureSearchId, $scope.groupIdSelect, $scope.selectFlight.directFlight.fareSourceCode, 0, $scope.dateSearch.maxJobGrade);
                                $scope.dataSearch.groupPricedItineraries = res.groupPricedItineraries === null ? [] : res.groupPricedItineraries;
                                var draft = [];
                                if ($scope.dataSearch.groupPricedItineraries.length > 0) {
                                    $scope.dataSearch.groupPricedItineraries.forEach(item => {
                                        if (item.pricedItineraries.length > 0) {
                                            draft = item.pricedItineraries.reduce((a, c) => (a[c.originDestinationOptions[1].destinationDateTime] = (a[c.originDestinationOptions[1].destinationDateTime] || []).concat(c), a), {});
                                        }
                                    });
                                }
                                var groupPricedItineraries = [];
                                if (draft) {
                                    var index = 0;
                                    Object.entries(draft).forEach(([key, val]) => {
                                        groupPricedItineraries.push(copy($scope.dataSearch.groupPricedItineraries[0]));
                                        groupPricedItineraries[index].id = null;
                                        groupPricedItineraries[index].pricedItineraries = val;
                                        groupPricedItineraries[index].arrivalDateTime = val[0].originDestinationOptions[1].originDateTime;
                                        groupPricedItineraries[index].departureDateTime = val[0].originDestinationOptions[1].destinationDateTime;
                                        groupPricedItineraries[index].returnDateTime = val[0].originDestinationOptions[1].destinationDateTime;
                                        //groupPricedItineraries[index].index = index;
                                        index++;
                                    });
                                }
                                if (groupPricedItineraries) {
                                    $.each(groupPricedItineraries, function (key, val) {
                                        if (val.pricedItineraries && val.pricedItineraries[0] != undefined) {
                                            val.pricedItineraries = [val.pricedItineraries[0]];
                                        }
                                    })
                                }
                                $scope.dataSearch.groupPricedItineraries = groupPricedItineraries;
                                $scope.dataSearch.pagingInfo = res.page === null ? {} : res.page;
                            } else {
                                var res = await bookingFlightService.filterAvailability(!$scope.dateSearch.isReturn ? $scope.dataSearch.searchId : $scope.dataSearch.returnSearchId, 0, $scope.dateSearch.maxJobGrade, airlineValue, $scope.filterData.stopData);
                                $scope.dataSearch.groupPricedItineraries = res.groupPricedItineraries === null ? [] : res.groupPricedItineraries;
                                if ($scope.dataSearch.groupPricedItineraries) {
                                    $.each($scope.dataSearch.groupPricedItineraries, function (key, val) {
                                        if (val.pricedItineraries && val.pricedItineraries[0] != undefined) {
                                            val.pricedItineraries = [val.pricedItineraries[0]];
                                        }
                                    })
                                }
                                $scope.dataSearch.pagingInfo = res.page === null ? {} : res.page;
                            }
                            
                            
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
                                    "searchId": !$scope.dateSearch.isReturn ? $scope.dataSearch.departureSearchId : (!$scope.dataSearch.returnSearchId ? $scope.dataSearch.searchId : $scope.dataSearch.returnSearchId)
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

                    $scope.isSameDate = function (newDate, fromDate) {
                        return newDate.getFullYear() === fromDate.getFullYear() &&
                            newDate.getMonth() === fromDate.getMonth() &&
                            newDate.getDate() === fromDate.getDate();
                    };

                    $scope.selectTicket = async function (sequenceNumber, groupId) {
                        $scope.groupIdSelect = groupId;
                        $scope.filterData.airlineData = [];
                        $scope.filterData.stopData = [];
                        $scope.dataSearch.pagingInfo = {};
                        var selectFlight = $scope.dataSearch.groupPricedItineraries.filter(x => x.groupId === groupId);
                        var myData = angular.copy(selectFlight)
                        if (selectFlight && selectFlight.length > 0) {
                            //var currentSelect = selectFlight[0];
                            var currentDraft = $.grep(selectFlight, function (e) { return $.grep(e.pricedItineraries, function (x) { return x.sequenceNumber === sequenceNumber }).length > 0 });
                            var currentSelect = currentDraft[0];
                            var currentSequenceNumber = currentSelect ? currentSelect.pricedItineraries.filter(x => x.sequenceNumber === sequenceNumber) : null;
                            //var currentSequenceNumber = currentSelect.pricedItineraries.filter(x => x.sequenceNumber === sequenceNumber); //Code Cu
                            if (currentSequenceNumber && currentSequenceNumber.length > 0) {
                                currentSelect.pricedItineraries = currentSequenceNumber[0];

                                var queryArgs = {};
                                if (currentSelect && currentSelect.pricedItineraries && currentSelect.pricedItineraries.airItineraryPricingInfo && currentSelect.pricedItineraries.airItineraryPricingInfo.fareSourceCode
                                    && currentSelect.groupId && (currentSelect.departureSearchId || currentSelect.returnSearchId)) {
                                    queryArgs = {
                                        "fareSourceCode": currentSelect.pricedItineraries.airItineraryPricingInfo.fareSourceCode,
                                        "groupId": currentSelect.groupId,
                                        "searchId": !$scope.dateSearch.isReturn ? currentSelect.departureSearchId : currentSelect.returnSearchId
                                    };
                                    currentSelect.fareSourceCode = currentSelect.pricedItineraries.airItineraryPricingInfo.fareSourceCode;
                                    // call api
                                    if (queryArgs && queryArgs != null) {
                                        var getFareRules = await bookingFlightService.getFareRules(queryArgs);
                                        if (getFareRules && getFareRules != null) {
                                            $.each(getFareRules, function (key, val) {
                                                if (val.fareRuleItems.length > 0) {
                                                    $.each(val.fareRuleItems, function (keyRule, valRule) {
                                                        if (!$scope.dateSearch.isReturn) {
                                                            currentSelect.titleDepartureFareRule = valRule.title;
                                                            currentSelect.detailDepartureFareRule = valRule.detail;
                                                        } else {
                                                            currentSelect.titleReturnFareRule = valRule.title;
                                                            currentSelect.detailReturnFareRule = valRule.detail;
                                                        }
                                                    });
                                                }
                                            });
                                        }
                                    }
                                }
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
                                    //$scope.selectFlight.returnFlight = selectFlight[0];
                                    $scope.selectFlight.returnFlight = currentDraft[0];
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
                                    if ($scope.dataSearch.groupPricedItineraries) {
                                        $.each($scope.dataSearch.groupPricedItineraries, function (key, val) {
                                            if (val.pricedItineraries && val.pricedItineraries[0] != undefined) {
                                                val.pricedItineraries = [val.pricedItineraries[0]];
                                            }
                                        })
                                    }
                                    $scope.dataSearch.pagingInfo = res.page === null ? {} : res.page;
                                    setDataSource(res.airlinesGroup, "#airlines");
                                    $scope.dateSearch.isSearch = true;

                                    if ($scope.searchTicketModel.RouteType == 'ROUNDTRIP') {
                                        var searchToDateFormat = moment($scope.searchTicketModel.ReturntureDate, "MM-DD-YYYY").toDate();
                                        var searchToDate = new Date(searchToDateFormat);

                                        var nextToDate = new Date($scope.btaToDate);
                                        nextToDate.setDate(nextToDate.getDate() + 1);

                                        var previousToDate = new Date($scope.btaToDate);
                                        previousToDate.setDate(previousToDate.getDate() - 1);

                                        $scope.disableNextDate = false;
                                        $scope.disablePreviousDate = false;

                                        if ($scope.isSameDate(nextToDate, searchToDate)) {
                                            $scope.disableNextDate = true;
                                            $scope.disablePreviousDate = false;
                                        }

                                        if ($scope.isSameDate(previousToDate, searchToDate)) {
                                            $scope.disableNextDate = false;
                                            $scope.disablePreviousDate = true;
                                        }
                                    }
                                    $scope.$apply();
                                    kendo.ui.progress($("#loading_bta"), false);
                                }
                                else if ($scope.dateSearch.flightType === "International") {
                                    $scope.dateSearch.isBooking = false;
                                    kendo.ui.progress($("#loading_bta"), true);
                                    var res = await bookingFlightService.groupIntinerary($scope.dataSearch.departureSearchId, groupId, $scope.selectFlight.directFlight.fareSourceCode, 0, $scope.dateSearch.maxJobGrade);
                                    $scope.dataSearch.groupPricedItineraries = res.groupPricedItineraries === null ? [] : res.groupPricedItineraries;
                                    var draft = [];
                                    if ($scope.dataSearch.groupPricedItineraries.length > 0) {
                                        $scope.dataSearch.groupPricedItineraries.forEach(item => {
                                            if (item.pricedItineraries.length > 0) {
                                                draft = item.pricedItineraries.reduce((a, c) => (a[c.originDestinationOptions[1].destinationDateTime] = (a[c.originDestinationOptions[1].destinationDateTime] || []).concat(c), a), {});
                                            }
                                        });
                                    }
                                    var groupPricedItineraries = [];
                                    if (draft) {
                                        var index = 0;
                                        Object.entries(draft).forEach(([key, val]) => {
                                            groupPricedItineraries.push(copy($scope.dataSearch.groupPricedItineraries[0]));
                                            groupPricedItineraries[index].id = null;
                                            groupPricedItineraries[index].pricedItineraries = val;
                                            groupPricedItineraries[index].arrivalDateTime = val[0].originDestinationOptions[1].originDateTime;
                                            groupPricedItineraries[index].departureDateTime = val[0].originDestinationOptions[1].destinationDateTime;
                                            groupPricedItineraries[index].returnDateTime = val[0].originDestinationOptions[1].destinationDateTime;
                                            //groupPricedItineraries[index].index = index;
                                            index++;
                                        });
                                    }
                                    if (groupPricedItineraries) {
                                        $.each(groupPricedItineraries, function (key, val) {
                                            if (val.pricedItineraries && val.pricedItineraries[0] != undefined) {
                                                val.pricedItineraries = [val.pricedItineraries[0]];
                                            }
                                        })
                                    }
                                    $scope.dataSearch.groupPricedItineraries = groupPricedItineraries;
                                    $scope.dataSearch.pagingInfo = res.page === null ? {} : res.page;
                                    setDataSource(res.airlinesGroup, "#airlines");
                                    $scope.dateSearch.isSearch = true;
                                    $scope.$apply();
                                    kendo.ui.progress($("#loading_bta"), false);
                                }
                                else {
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
                                let dialog = $("#dialog_SearchTicket").data("kendoDialog");
                                dialog.close();
                            }
                        }
                        else {
                            $scope.searchTicketScope.setTripGroupHasTicket($scope.dateSearch.bookingGroupId, $scope.selectFlight);
                            let dialogss = $("#dialog_SearchTicket").data("kendoDialog");
                            dialogss.close();
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
                        if ($scope.dateSearch.isReturn && $scope.dateSearch.routeType === "ROUNDTRIP" && $scope.dateSearch.flightType === "International") {
                            // BUG: chọn Fillter vé về đi quốc tế
                            var res = await bookingFlightService.groupIntinerary($scope.dataSearch.departureSearchId, $scope.groupIdSelect, $scope.selectFlight.directFlight.fareSourceCode, 0, $scope.dateSearch.maxJobGrade);
                            $scope.dataSearch.groupPricedItineraries = res.groupPricedItineraries === null ? [] : res.groupPricedItineraries;
                            var draft = [];
                            if ($scope.dataSearch.groupPricedItineraries.length > 0) {
                                $scope.dataSearch.groupPricedItineraries.forEach(item => {
                                    if (item.pricedItineraries.length > 0) {
                                        draft = item.pricedItineraries.reduce((a, c) => (a[c.originDestinationOptions[1].destinationDateTime] = (a[c.originDestinationOptions[1].destinationDateTime] || []).concat(c), a), {});
                                    }
                                });
                            }
                            var groupPricedItineraries = [];
                            if (draft) {
                                var index = 0;
                                Object.entries(draft).forEach(([key, val]) => {
                                    groupPricedItineraries.push(copy($scope.dataSearch.groupPricedItineraries[0]));
                                    groupPricedItineraries[index].id = null;
                                    groupPricedItineraries[index].pricedItineraries = val;
                                    groupPricedItineraries[index].arrivalDateTime = val[0].originDestinationOptions[1].originDateTime;
                                    groupPricedItineraries[index].departureDateTime = val[0].originDestinationOptions[1].destinationDateTime;
                                    groupPricedItineraries[index].returnDateTime = val[0].originDestinationOptions[1].destinationDateTime;
                                    //groupPricedItineraries[index].index = index;
                                    index++;
                                });
                            }
                            if (groupPricedItineraries) {
                                $.each(groupPricedItineraries, function (key, val) {
                                    if (val.pricedItineraries && val.pricedItineraries[0] != undefined) {
                                        val.pricedItineraries = [val.pricedItineraries[0]];
                                    }
                                })
                            }
                            $scope.dataSearch.groupPricedItineraries = groupPricedItineraries;
                            $scope.dataSearch.pagingInfo = res.page === null ? {} : res.page;
                        } else {
                            var res = await bookingFlightService.filterAvailability(!$scope.dateSearch.isReturn ? $scope.dataSearch.searchId : $scope.dataSearch.returnSearchId, pageNum, $scope.dateSearch.maxJobGrade, $scope.filterData.airlineData, $scope.filterData.stopData);
                            $scope.dataSearch.groupPricedItineraries = res.groupPricedItineraries === null ? [] : res.groupPricedItineraries;
                            if ($scope.dataSearch.groupPricedItineraries) {
                                $.each($scope.dataSearch.groupPricedItineraries, function (key, val) {
                                    if (val.pricedItineraries && val.pricedItineraries[0] != undefined) {
                                        val.pricedItineraries = [val.pricedItineraries[0]];
                                    }
                                })
                            }
                            $scope.dataSearch.pagingInfo = res.page === null ? {} : res.page;
                        }
                        
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
                        $scope.infoSearchFlight.isReturn = infoData.isReturn;
                        $scope.dateSearch.currentDate = infoData.dateSearch;
                    }

                    $scope.dataFlight = function (infoFlight) {
                        $scope.dataSearch.searchId = infoFlight.searchId;
                        $scope.dataSearch.departureSearchId = infoFlight.departureSearchId;
                        $scope.dataSearch.returnSearchId = infoFlight.returnSearchId;
                        $scope.dataSearch.groupPricedItineraries = infoFlight.groupPricedItineraries === null ? [] : infoFlight.groupPricedItineraries;
                        if ($scope.dataSearch.groupPricedItineraries) {
                            $.each($scope.dataSearch.groupPricedItineraries, function (key, val) {
                                if (val.pricedItineraries && val.pricedItineraries[0] != undefined) {
                                    val.pricedItineraries = [val.pricedItineraries[0]];
                                }
                            })
                        }
                        $scope.dataSearch.pagingInfo = infoFlight.pagingInfo;
                        $scope.dateSearch.isSearch = true;
                    }

                    function copy(aObject) {
                        // Prevent undefined objects
                        // if (!aObject) return aObject;
                        let bObject = Array.isArray(aObject) ? [] : {};
                        let value;
                        for (const key in aObject) {
                            // Prevent self-references to parent object
                            // if (Object.is(aObject[key], aObject)) continue;
                            value = aObject[key];
                            bObject[key] = (typeof value === "object") ? copy(value) : value;
                        }
                        return bObject;
                    }

                    $scope.loadFlight = async function (searchTicketModel) {
                        if (!$.isEmptyObject(searchTicketModel)) {
                            /*let dialog = $("#dialog_LoadingSearchTicket").data("kendoDialog");
                            dialog.open();*/
                            //$rootScope.showLoading();
                            kendo.ui.progress($("#dialog_TripGroup"), true);
                            //kendo.ui.progress($("#loading_SearchAirTicket"), true);
                            $scope.clearData();
                            $scope.searchTicketModel = searchTicketModel;


                            var filterOtherFlight = ($scope.btaSearchModel != null && JSON.stringify(searchTicketModel) != JSON.stringify($scope.btaSearchModel)) ? true : false;
                            if ($scope.btaSearchModel == null || filterOtherFlight) {
                                
                                // set from date, to date from BTA
                                if ($scope.btaFromDate == null || ($scope.btaFromDate != null && filterOtherFlight)) {
                                    $scope.btaFromDate = moment(searchTicketModel.DepartureDate, "MM-DD-YYYY").toDate();
                                }
                                if ($scope.btaToDate == null || ($scope.btaToDate != null && filterOtherFlight)) {
                                    $scope.btaToDate = moment(searchTicketModel.ReturntureDate, "MM-DD-YYYY").toDate()
                                }
                                $scope.btaSearchModel = searchTicketModel;
                            }

                            $scope.disablePreviousDate = false;
                            $scope.disableNextDate = false;
                            if ($scope.btaFromDate != null && $scope.btaToDate) {
                                var currentSearchFromDate = moment(searchTicketModel.DepartureDate, "MM-DD-YYYY").toDate()
                                var currentSearchToDate = moment(searchTicketModel.ReturntureDate, "MM-DD-YYYY").toDate()

                                var fromDate = new Date($scope.btaFromDate);
                                var toDate = new Date($scope.btaToDate);

                                var fromDateAddOneDay = new Date(fromDate);
                                fromDateAddOneDay.setDate(fromDateAddOneDay.getDate() + 1);

                                var fromDateSubtractOneDay = new Date(fromDate);
                                fromDateSubtractOneDay.setDate(fromDateSubtractOneDay.getDate() - 1);

                                var toDateAddOneDay = new Date(toDate);
                                toDateAddOneDay.setDate(toDateAddOneDay.getDate() + 1);

                                var toDateSubtractOneDay = new Date(toDate);
                                toDateSubtractOneDay.setDate(toDateSubtractOneDay.getDate() - 1);

                                if ($scope.searchTicketModel.RouteType == 'ONEWAY') {
                                    if ($scope.isSameDate(fromDateSubtractOneDay, currentSearchFromDate)) {
                                        $scope.disableNextDate = false;
                                        $scope.disablePreviousDate = true;
                                    }
                                    if ($scope.isSameDate(fromDateAddOneDay, currentSearchFromDate)) {
                                        $scope.disableNextDate = true;
                                        $scope.disablePreviousDate = false;
                                    }
                                } else {
                                    if (!$scope.dateSearch.isReturn) {
                                        if ($scope.isSameDate(fromDateSubtractOneDay, currentSearchFromDate)) {
                                            $scope.disableNextDate = false;
                                            $scope.disablePreviousDate = true;
                                        }
                                        if ($scope.isSameDate(fromDateAddOneDay, currentSearchFromDate)) {
                                            $scope.disableNextDate = true;
                                            $scope.disablePreviousDate = false;
                                        }
                                    } else {

                                    }
                                }
                            }

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
                            //kendo.ui.progress($("#loading_SearchAirTicket"), false);
                            //dialog.close();
                            //$rootScope.hideLoading();
                            let dialogs = $("#dialog_SearchTicket").data("kendoDialog")
                            dialogs.title($translate.instant('COMMON_BUTTON_SEARCH_TICKET'));
                            dialogs.open();
                            //Set Airlines
                            setDataSource(searchFlight.airlinesGroup, "#airlines");
                            kendo.ui.progress($("#dialog_TripGroup"), false);
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