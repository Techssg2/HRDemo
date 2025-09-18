angular
    .module('ssg')
    .factory('bookingFlightService', ["btaService", "$translate", "settingService", function (btaService, $translate, settingService) {
        var flightType, routeType, airlinesGroup = [], isReturn = false, searchId = "", departureSearchId = "", returnSearchId = "", maxJobGrade = 0;

        var buildQuery = {
            queryArgs: {
                CabinClass: "E",
                ChildrenQtt: 0,
                InfantsQtt: 0,
                Page: 0,
                Size: 15,
                OriginCode: "",
                DestinationCode: "",
                DepartureDate: "",
                ReturntureDate: "",
                AdutsQtt: 0
            },
            filterArgs: {
                SearchId: "",
                Filter: {
                    CabinClassOptions: [],
                    //TicketPolicyOptions: [],
                    TicketPolicyOptions: ["PREMIUM", "ECONOMY"],
                    AirlineOptions: [],
                    StopOptions: [],
                    Step: "1",
                    FlightType: "DOMESTIC",
                    DepartureDateTimeOptions: [],
                    ArrivalDateTimeReturnOptions: [],
                    ArrivalDateTimeOptions: [],
                    PriceItineraryId: "",
                    LoadMore: false,
                    DepartureDateTimeReturnOptions: []
                },
                DepartureItinerary: null

                //Filter: {
                //    AirlineOptions: [],
                //    StopOptions: [],
                //    FlightType: "",
                //},
                //ExtractLowPrice: false,

            },
            // Quoc te khu hoi
            groupIntineraryArgs: {
                SearchId: "",
                Filter: {
                    AirlineOptions: [],
                    ArrivalDateTimeOptions: [],
                    ArrivalDateTimeReturnOptions: [],
                    CabinClassOptions: [],
                    DepartureDateTimeOptions: [],
                    DepartureDateTimeReturnOptions: [],
                    FlightType: "INTERNATIONAL",
                    GroupId: "",
                    PriceItineraryId: "",
                    Step: "2",
                    StopOptions: [],
                    TicketPolicyOptions: []
                },
                DepartureItinerary: {
                    FareSourceCode: ""
                }
            }
        }

        var btaId = "";

        var timeConvert = function (n) {
            var num = n;
            var hours = (num / 60);
            var rhours = Math.floor(hours);
            var minutes = (hours - rhours) * 60;
            var rminutes = Math.round(minutes);
            return rhours + "h" + rminutes + "m";
        }

        var formatDate = function (date) {
            var currentMonth = (date.getMonth() + 1);
            var currentDate = date.getDate()
            return (currentMonth < 10 ? "0" + currentMonth : currentMonth) + '-' + (currentDate < 10 ? "0" + currentDate : currentDate) + '-' + date.getFullYear();
        }

        var getFareRules = async function (queryArgs) {
            var returnValue = [];
            var res = await btaService.getInstance().bussinessTripApps.getFareRulesByFareSourceCode(queryArgs).$promise;
            if (res && res.isSuccess) {
                returnValue = res.object;
            } else {
                res = await btaService.getInstance().bussinessTripApps.fareRules({ language: $translate.use() === "en_US" ? "en" : "vi" }, queryArgs).$promise;
                if (res && res.isSuccess) {
                    returnValue = res.object.fareRules;
                }
            }
            return returnValue;
        }

        var loadInfoFlight = function (currentData) {
            return infoFlight = {
                originCode: !isReturn ? currentData.OriginCode : currentData.DestinationCode,
                originCity: !isReturn ? currentData.DepartureName : currentData.ArrivalName,
                destinationCode: !isReturn ? currentData.DestinationCode : currentData.OriginCode,
                destinationCity: !isReturn ? currentData.ArrivalName : currentData.DepartureName,
                dateSearch: !isReturn ? new Date(currentData.DepartureDate) : new Date(currentData.ReturntureDate),
                numberAdult: currentData.AdutsQtt,
                isReturn: isReturn
            }
        }

        var comparePrice = function (a, b) {
            if (a.airItineraryPricingInfo.itinTotalFare.totalFare.amount < b.airItineraryPricingInfo.itinTotalFare.totalFare.amount) {
                return -1;
            }
            if (a.airItineraryPricingInfo.itinTotalFare.totalFare.amount > b.airItineraryPricingInfo.itinTotalFare.totalFare.amount) {
                return 1;
            }
            return 0;
        }

        var addHours = function (currentDate, hour) {
            currentDate.setTime(currentDate.getTime() + (hour * 60 * 60 * 1000));
            return currentDate;
        }

        var prepareData = function (groupPricedItineraries) {
            var newGroupPricedItineraries = [];
            if (!$.isEmptyObject(groupPricedItineraries) && groupPricedItineraries.length > 0) {
                $.each(groupPricedItineraries, function (key, val) {
                    groupPricedItineraries[key].imgAirLogo = "ClientApp/assets/images/AirLogos/" + val.airline + ".gif";
                    groupPricedItineraries[key].searchId = searchId;
                    groupPricedItineraries[key].departureSearchId = departureSearchId;
                    groupPricedItineraries[key].returnSearchId = returnSearchId;

                    val.pricedItineraries.sort(comparePrice);

                    val.pricedItineraries = $.grep(val.pricedItineraries, function (value) {
                        //if (value.cabinClassName === "BUSINESS" || value.cabinClassName === "PREMIUM")
                        if (value.cabinClassName === "BUSINESS")
                            return false;
                        else
                            return true;
                    });
                    if (val.pricedItineraries && val.pricedItineraries[0] != undefined) {
                        // CR: Lay gia tri nho nhat
                        //val.pricedItineraries = [val.pricedItineraries[0]];

                        //if (maxJobGrade >= 5 && maxJobGrade < 8) {
                        //    val.pricedItineraries = $.grep(val.pricedItineraries, function (value) {
                        //        var cabinText = value.originDestinationOptions[0].flightSegments[0];
                        //        if (cabinText.cabinClassText === "Economy" || cabinText.cabinClassText === "ECONOMYSAVER" || cabinText.cabinClassText === "ECONOMYSAVERMAX")
                        //            return true;
                        //        else
                        //            return false;
                        //    });
                        //}
                        //else if (maxJobGrade < 5) {
                        //    val.pricedItineraries = $.grep(val.pricedItineraries, function (value) {
                        //        var cabinText = value.originDestinationOptions[0].flightSegments[0];
                        //        if (cabinText.cabinClassText === "ECONOMYSAVER" ||
                        //            cabinText.cabinClassText === "ECONOMYSMART" ||
                        //            cabinText.cabinClassText === "ECONOMYFLEX" ||
                        //            cabinText.cabinClassText === "PREMIUMSMART")
                        //            return false;
                        //        else
                        //            return true;
                        //    });
                        //}

                        $.each(val.pricedItineraries, function (keyPrice, valPrice) {
                            $.each(valPrice.originDestinationOptions, function (keyOrgin, valOrgin) {
                                valPrice.originDestinationOptions[keyOrgin].journeyDuration = timeConvert(valOrgin.journeyDuration);
                                if (valOrgin.flightSegments.length > 1) {
                                    $.each(valOrgin.flightSegments, function (keySegements, valSegement) {

                                        if (valPrice.flightNo === null || valPrice.flightNo === undefined) {
                                            groupPricedItineraries[key].flightNo = valSegement.operatingAirline.code + valSegement.operatingAirline.flightNumber
                                        }
                                        /*if (valSegement.stopQuantityInfo) {
                                            valOrgin.flightSegments[keySegements].stopQuantityInfo.duration = timeConvert(valSegement.stopQuantityInfo.duration);
                                        }

                                        var arrivalTime = addHours(new Date(valSegement.arrivalAirport.scheduledTime), parseFloat((valSegement.arrivalAirport.utcOffset.hours)));
                                        var departureTime = addHours(new Date(valSegement.departureAirport.scheduledTime), parseFloat((valSegement.departureAirport.utcOffset.hours)));

                                        valSegement.arrivalDateTime = arrivalTime;
                                        valSegement.departureDateTime = departureTime;

                                        groupPricedItineraries[key].arrivalDateTime = arrivalTime;
                                        groupPricedItineraries[key].departureDateTime = departureTime;

                                        valOrgin.flightSegments[keySegements].journeyDuration = timeConvert(valSegement.journeyDuration);*/
                                    });
                                }
                                else {
                                    $.each(valOrgin.flightSegments, function (keySegements, valSegement) {

                                        if (valPrice.flightNo === null || valPrice.flightNo === undefined) {
                                            groupPricedItineraries[key].flightNo = valSegement.operatingAirline.code + valSegement.operatingAirline.flightNumber
                                        }
                                        /*if (valSegement.stopQuantityInfo) {
                                            valOrgin.flightSegments[keySegements].stopQuantityInfo.duration = timeConvert(valSegement.stopQuantityInfo.duration);
                                        }

                                        var arrivalTime = addHours(new Date(valSegement.arrivalAirport.scheduledTime), parseFloat((valSegement.arrivalAirport.utcOffset.hours)));
                                        var departureTime = addHours(new Date(valSegement.departureAirport.scheduledTime), parseFloat((valSegement.departureAirport.utcOffset.hours)));

                                        valSegement.arrivalDateTime = arrivalTime;
                                        valSegement.departureDateTime = departureTime;

                                        groupPricedItineraries[key].arrivalDateTime = arrivalTime;
                                        groupPricedItineraries[key].departureDateTime = departureTime;

                                        valPrice.originDestinationOptions[keyOrgin].originDateTime = departureTime;
                                        valPrice.originDestinationOptions[keyOrgin].destinationDateTime = arrivalTime;

                                        valOrgin.flightSegments[keySegements].journeyDuration = timeConvert(valSegement.journeyDuration);*/
                                    });
                                }
                            });
                        });

                        if (val.pricedItineraries.length > 0) {
                            var myAirlines = airlinesGroup.filter(x => x.text === val.airlineName);
                            if (myAirlines.length === 0) {
                                airlinesGroup.push({ id: val.airline, text: val.airlineName });
                            }
                            newGroupPricedItineraries.push(groupPricedItineraries[key]);
                        }
                    }
                });
            }
            return newGroupPricedItineraries;
        }

        var filterAvailability = async function (searchId, pageNum, jobGrade, airlines, stops) {
            var returnValue = [];
            if (pageNum === undefined) {
                pageNum = 0;
            }

            if (jobGrade !== undefined) {
                maxJobGrade = jobGrade;
            }

            buildQuery.filterArgs.SearchId = searchId;
            buildQuery.filterArgs.Filter.AirlineOptions = airlines === undefined ? [] : airlines;
            buildQuery.filterArgs.Filter.StopOptions = stops === undefined ? [] : stops;
            var res = await btaService.getInstance().bussinessTripApps.filterAvailability({ PageNum: pageNum }, buildQuery.filterArgs).$promise;
            if (res && res.isSuccess) {
                res.object.groupPricedItineraries = prepareData(res.object.groupPricedItineraries);
                res.object.airlinesGroup = airlinesGroup
                returnValue = res.object;
            }
            return returnValue;
        }

        var groupIntinerary = async function (searchId, groupId, fareSourceCode, pageNum, jobGrade, airlines, stops) {
            var returnValue = [];
            if (pageNum === undefined) {
                pageNum = 0;
            }

            if (jobGrade !== undefined) {
                maxJobGrade = jobGrade;
            }

            buildQuery.groupIntineraryArgs.SearchId = searchId;
            buildQuery.groupIntineraryArgs.Filter.GroupId = groupId;
            buildQuery.groupIntineraryArgs.Filter.AirlineOptions = airlines === undefined ? [] : airlines;
            buildQuery.groupIntineraryArgs.Filter.StopOptions = stops === undefined ? [] : stops;
            buildQuery.groupIntineraryArgs.DepartureItinerary.fareSourceCode = fareSourceCode;
            var res = await btaService.getInstance().bussinessTripApps.groupIntinerary({ PageNum: pageNum }, buildQuery.groupIntineraryArgs).$promise;
            if (res && res.isSuccess) {
                res.object.groupPricedItineraries = prepareData(res.object.groupPricedItineraries);
                res.object.airlinesGroup = airlinesGroup
                returnValue = res.object;
            }
            return returnValue;
        }

        var searchFlightDetail = async function (queryArgs) {
            var returnValue = {};
            if (!$.isEmptyObject(queryArgs)) {
                buildQuery.queryArgs.OriginCode = queryArgs.OriginCode;
                buildQuery.queryArgs.DestinationCode = queryArgs.DestinationCode;
                buildQuery.queryArgs.DepartureDate = queryArgs.DepartureDate;
                buildQuery.queryArgs.ReturntureDate = queryArgs.ReturntureDate;
                buildQuery.queryArgs.AdutsQtt = queryArgs.AdutsQtt;
                buildQuery.queryArgs.Page = queryArgs.Page === undefined ? 0 : queryArgs.Page;
                buildQuery.queryArgs.RouteType = queryArgs.RouteType;
                returnValue = await btaService.getInstance().bussinessTripApps.searchFlight(buildQuery.queryArgs).$promise;
            }
            return returnValue;
        }

        var searchFlight = async function (searchQuery, isReturnFlight, btaItemId) {
            if (!$.isEmptyObject(searchQuery)) {
                btaId = btaItemId;
                flightType = searchQuery.FlightType;
                routeType = searchQuery.RouteType;
                isReturn = isReturnFlight;
                buildQuery.filterArgs.Filter.FlightType = flightType;
                maxJobGrade = searchQuery.JobGrade;
                var res = await searchFlightDetail(searchQuery);
                if (res && res.isSuccess) {
                    searchId = res.object.searchId;
                    departureSearchId = res.object.departureSearchId;
                    returnSearchId = res.object.returnSearchId === null ? res.object.departureSearchId : res.object.returnSearchId;
                    if (flightType === "Domestic") {
                        if (res.object.groupPricedItineraries && res.object.groupPricedItineraries.length > 0) {
                            var filterRes = await filterAvailability(res.object.departureSearchId);

                            res.object.groupPricedItineraries = [];
                            if (filterRes && filterRes.groupPricedItineraries.length > 0) {
                                res.object.groupPricedItineraries = filterRes.groupPricedItineraries;
                                res.object.page = filterRes.page;
                            }
                        }
                    } else {
                        if (res.object.groupPricedItineraries && res.object.groupPricedItineraries.length > 0) {
                            res.object.groupPricedItineraries = prepareData(res.object.groupPricedItineraries);
                        }
                    }

                    return {
                        pagingInfo: res.object.page,
                        searchId: res.object.searchId,
                        departureSearchId: res.object.departureSearchId,
                        returnSearchId: returnSearchId,//res.object.returnSearchId,
                        groupPricedItineraries: res.object.groupPricedItineraries,
                        isSuccess: true,
                        airlinesGroup: airlinesGroup
                    };
                } else {
                    return {
                        isSuccess: false,
                        message: "Please contact administrator."
                    };
                }
            }
        }

        var loadFlight = function (searchTicketModel, isReturnFlight) {
            if (!$.isEmptyObject(searchTicketModel)) {
                isReturn = isReturnFlight;
                return loadInfoFlight(searchTicketModel);
            }
        }


        //Booking Ticket Start
        var remove_unicode = function (str) {
            str = str.toLowerCase();
            str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
            str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
            str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
            str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
            str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
            str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
            str = str.replace(/đ/g, "d");
            str = str.replace(/!|@|%|\^|\*|\(|\)|\+|\=|\<|\>|\?|\/|,|\.|\:|\;|\'|\"|\&|\#|\[|\]|~|$|_/g, "-");
            //lamnl add new
            str = str.normalize('NFD').replace(/[\u0300-\u036f]/g, '');
            //
            str = str.replace(/-+-/g, "-"); //thay thế 2- thành 1- 
            str = str.replace(/^\-+|\-+$/g, "");

            return str;
        }

        var mainGroups, objValidate = {}, arrayDraftBooking = [], bookingInfo = {}, airlineCodeInfo = [];

        var departureDate = function (a, b) {
            if (a.departureDateTime < b.departureDateTime) {
                return -1;
            }
            if (a.departureDateTime > b.departureDateTime) {
                return 1;
            }
            return 0;
        }

        var reValidate = async function (myGroups) {
            //var returnValue = false;

            if (myGroups.length > 0) {
                var currentGroup = myGroups.pop();
                if (currentGroup && currentGroup.groupInfoPassengers.length > 0) {
                    var arrayPassenger = currentGroup.groupInfoPassengers;

                    $.each(arrayPassenger, function (key, val) {
                        val.firstName = remove_unicode(val.firstName).toUpperCase();
                        val.lastName = remove_unicode(val.lastName).toUpperCase();
                    });

                    flightData = arrayPassenger[0].flightDetails;
                    var objItineraryInfos = {
                        itineraryInfos: []
                    }
                    var validTicket = async function () {
                        if (flightData.length > 0) {
                            flightData.sort(departureDate);
                            var currentFlight = flightData.shift();

                            //lamnl
                            let modelAirline = {
                                direction: currentFlight.directFlight ? 'DEPARTURE' : 'RETURN',
                                airlineCode: currentFlight.airline,
                                tripGroup: currentGroup.group
                            };
                            airlineCodeInfo.push(modelAirline);

                            //end
                            var objValidTicket = {};
                            if (currentFlight.flightType === "DOMESTIC") {
                                objValidTicket = {
                                    fareSourceCode: currentFlight.pricedItineraries.airItineraryPricingInfo.fareSourceCode,
                                    groupId: currentFlight.groupId,
                                    searchId: !currentFlight.directFlight ? currentFlight.returnSearchId : currentFlight.searchId
                                }
                                var res = await btaService.getInstance().bussinessTripApps.reValidateBooking(objValidTicket).$promise;
                                if (res && res.isSuccess && res.object.valid) {
                                    objItineraryInfos.itineraryInfos.push(objValidTicket);
                                    await validTicket();
                                }
                                else {
                                    return objValidate = {
                                        isSuccess: false,
                                        message: "The ticket is not valid!"
                                    }
                                }

                            } else {
                                // International
                                if (currentFlight.roundType === 'ROUNDTRIP') {
                                    if (!currentFlight.directFlight) {
                                            objValidTicket = {
                                                fareSourceCode: currentFlight.pricedItineraries.airItineraryPricingInfo.fareSourceCode,
                                                groupId: currentFlight.groupId,
                                                searchId: !currentFlight.directFlight ? currentFlight.returnSearchId : currentFlight.searchId
                                            }
                                            var res = await btaService.getInstance().bussinessTripApps.reValidateBooking(objValidTicket).$promise;
                                            if (res && res.isSuccess) {
                                                objItineraryInfos.itineraryInfos.push(objValidTicket);
                                                await validTicket();
                                            }
                                            else {
                                                return objValidate = {
                                                    isSuccess: false,
                                                    message: "The ticket is not valid!"
                                                }
                                            }
                                    } else {
                                        await validTicket();
                                    }
                                } else {
                                    objValidTicket = {
                                        fareSourceCode: currentFlight.pricedItineraries.airItineraryPricingInfo.fareSourceCode,
                                        groupId: currentFlight.groupId,
                                        searchId: !currentFlight.directFlight ? currentFlight.returnSearchId : currentFlight.searchId
                                    }
                                    var res = await btaService.getInstance().bussinessTripApps.reValidateBooking(objValidTicket).$promise;
                                    if (res && res.isSuccess) {
                                        objItineraryInfos.itineraryInfos.push(objValidTicket);
                                        await validTicket();
                                    }
                                    else {
                                        return objValidate = {
                                            isSuccess: false,
                                            message: "The ticket is not valid!"
                                        }
                                    }
                                }
                            }
                        }
                        else {
                            arrayDraftBooking.push({ tripGroup: currentGroup.group, objItineraryInfos: objItineraryInfos, newPassengerInfo: arrayPassenger });
                            await reValidate(myGroups);
                        }
                    }
                    await validTicket();
                }
            } else {
                return objValidate = {
                    isSuccess: true,
                    message: ""
                }
            }
        }

        var prepareGroups = function (passengerInfos) {
            var groups = {}, myGroups = [];
            $.each(passengerInfos, function (key, val) {
                var groupName = val.tripGroup;
                if (!groups[groupName]) {
                    groups[groupName] = [];
                }
                groups[groupName].push(val);
            });

            for (var groupName in groups) {
                myGroups.push({ group: groupName, isOverBudget: groups[groupName][0].isOverBudget, groupInfoPassengers: groups[groupName] });
            }
            mainGroups = angular.copy(myGroups);
            return myGroups;
        }

        var bookingTicket = async function (passengerInfos, tripId) {
            btaId = tripId;
            var myGroups = prepareGroups(passengerInfos);
            //var findOverBudget = mainGroups.filter(x => x.isOverBudget);
            airlineCodeInfo = [];
            //Check the ticket valid
            await reValidate(myGroups);

            if (objValidate && objValidate.isSuccess) {
                var prepareBookingInfoBeforeSave = function (status) {
                    var returnValue = [];
                    var myGroup = mainGroups.filter(x => x.group === tripGroup);
                    if (myGroup && myGroup.length > 0) {
                        $.each(myGroup[0].groupInfoPassengers, function (key, val) {
                            $.each(val.flightDetails, function (keyFlight, valFlight) {
                                var objBooking = {
                                    flightDetailId: valFlight.id,
                                    bTADetailId: val.id,
                                    bookingCode: bookingInfo.bookingCode,
                                    bookingNumber: bookingInfo.bookingNumber,
                                    status: status,
                                    penaltyFree: 0,
                                    groupId: valFlight.groupId,
                                    directFlight: valFlight.directFlight,
                                    isCancel: false
                                }
                                returnValue.push(objBooking);
                            });
                        });
                    }
                    return returnValue;
                }

                var draftBooking = async function () {
                    if (arrayDraftBooking.length > 0) {
                        var currentTripGroup = arrayDraftBooking.pop();
                        tripGroup = currentTripGroup.tripGroup;
                        var draftBookingArgs = {
                            btaID: currentTripGroup && currentTripGroup.newPassengerInfo.length > 0 ? currentTripGroup.newPassengerInfo[0].businessTripApplicationId : null
                        };
                        var res = await btaService.getInstance().bussinessTripApps.draftBooking(draftBookingArgs,currentTripGroup.objItineraryInfos).$promise;
                        if (res && res.isSuccess) {
                            bookingInfo = {
                                bookingCode: res.object.bookingCode.bookingCode,
                                bookingNumber: res.object.bookingCode.bookingNumber
                            }
                            //lamnl 
                            let airlineCodeInfos = $.grep(airlineCodeInfo, function (e) { return e.tripGroup == tripGroup; });
                            var travellerArgs = {
                                id: btaId,
                                bookingNumber: bookingInfo.bookingNumber,
                                btaDetailItem: currentTripGroup.newPassengerInfo,
                                airlines: JSON.stringify(airlineCodeInfos)
                            }
                            var resTraveller = await btaService.getInstance().bussinessTripApps.addBookingTraveller(travellerArgs).$promise;
                            //Not over budget
                            if (resTraveller && resTraveller.isSuccess && resTraveller.object && resTraveller.object.success) {
                                var bookingData = {
                                    booking_number: bookingInfo.bookingNumber,
                                    partner_trans_id: bookingInfo.bookingNumber,
                                    btaDetailItem: currentTripGroup.newPassengerInfo,
                                    booking_code: bookingInfo.bookingCode
                                };
                                var resCommit = await btaService.getInstance().bussinessTripApps.commitBooking(bookingData).$promise;
                                if (resCommit && resCommit.isSuccess && resCommit.object && resCommit.object.success) {
                                    //Save Booking Contract
                                    var bookingInfos = prepareBookingInfoBeforeSave("Completed");
                                    var res = await btaService.getInstance().bussinessTripApps.saveBookingInfo(bookingInfos).$promise;
                                    if (res && res.isSuccess) {
                                        await draftBooking();
                                    }
                                } else {
                                    return objValidate = {
                                        isCommitBooking: true,
                                        isSuccess: false,
                                        bookingCode: bookingInfo.bookingCode,
                                        bookingNumber: bookingInfo.bookingNumber,
                                        errorCode: resCommit.errorCodeStr,
                                        message: resCommit.messages
                                    }
                                    /*
                                    return objValidate = {
                                        isCommitBooking: true,
                                        bookingNumber: bookingInfo.bookingNumber,
                                        bookingCode: bookingInfo.bookingCode,
                                        isSuccess: false,
                                        message: "(CommitBooking) Xuất vé không thành công, Vui lòng liên hệ Admin hoặc GTD để chờ xử lý!"
                                    }*/
                                }
                            } else {
                                // show log khi errors
                                var errorMessage = "";
                                if (resTraveller.object) {
                                    resTraveller.object.errors.forEach(x => {
                                        errorMessage += (x.message + " - ");
                                    });
                                }
                                if (resTraveller.messages) {
                                    resTraveller.messages.forEach(x => {
                                        errorMessage += (x + " - ");
                                    });
                                }

                                var queryArgs = {
                                    predicate: "Type == (@0)",
                                    predicateParameters: [true],
                                    order: "Modified desc",
                                    limit: 10000,
                                    page: 1
                                };

                                var result = await GetBTAErrorList();
                                if (result.isSuccess && resTraveller.object != null) {
                                    var errorMessages = PushErrorCode("(AddBookingTraveller)", result.object.data, resTraveller.object.errors, $translate.use())
                                    return objValidate = {
                                        isSuccess: false,
                                        bookingNumber: bookingInfo.bookingNumber,
                                        message: errorMessages
                                    }
                                }
                                return objValidate = {
                                    isSuccess: false,
                                    bookingNumber: bookingInfo.bookingNumber,
                                    //message: "(AddBookingTraveller) BookingNumber " + bookingInfo.bookingNumber +" Book vé không thành công, Vui lòng chọn vé khác book lại : " + errorMessage
                                    message: resTraveller.messages
                                }
                            }
                        } else {
                            var result = await GetBTAErrorList();
                            if (result.isSuccess && res.object != null && res.object.errors) {
                                var errorMessages = PushErrorCode("(DraftBooking)", result.object.data, res.object.errors, $translate.use())
                                return objValidate = {
                                    isSuccess: false,
                                    bookingNumber: res.object.bookingCode.bookingNumber,
                                    message: errorMessages
                                }
                            }

                            return objValidate = {
                                isSuccess: false,
                                // message: "(DraftBooking) Đặt vé thất bại xin vui lòng chọn vé khác và book lại!"
                                message: "(DraftBooking) Đặt vé thất bại xin vui lòng chọn vé khác và book lại!"
                            }
                        }
                    }
                    else {
                        objValidate = {
                            isCommitBooking: true,
                            bookingNumber: bookingInfo.bookingNumber,
                            bookingCode: bookingInfo.bookingCode,
                            isSuccess: true,
                            message: ""
                        }
                    }
                }
                await draftBooking();
            }
            else {
                objValidate = {
                    isSuccess: false,
                    message: objValidate.message
                }
            }
            return objValidate;
        }

        async function GetBTAErrorList() {
            var queryArgs = {
                predicate: "Type == (@0)",
                predicateParameters: [true],
                order: "Modified desc",
                limit: 10000,
                page: 1
            };
            var result = await settingService.getInstance().btaErrorMessage.getBTAErrorMessageList(queryArgs).$promise;
            return result;
        }

        function PushErrorCode(typeAPI, btaErrorMessageMasterData, errors, currentLanguage) {
            let errorMessages = [];
            for (let item of errors) {
                var findError = _.find(btaErrorMessageMasterData, x => { return x.errorCode == item.code });
                if (findError != null) {
                    var showErrorCode = typeAPI + " " + findError.errorCode + " => ";
                    if (currentLanguage == "en_US")
                        if (!errorMessages.includes(getErrorCodeByGotadi))
                            errorMessages.push(showErrorCode.concat(findError.messageEN));
                     else 
                        if (!errorMessages.includes(getErrorCodeByGotadi))
                            errorMessages.push(showErrorCode.concat(findError.messageVI));
                    
                } else {
                    var errorCodeByGotadi = (typeAPI == '(AddBookingTraveller)') ? GetErrorCodeByGotadi(item.code) : GetErrorCodeAPIDraftBookingByGotadi(item.code);
                    errorCodeByGotadi = typeAPI + " " + errorCodeByGotadi;
                    if (!errorMessages.includes(errorCodeByGotadi))
                        errorMessages.push(errorCodeByGotadi);
                }
            }
            return errorMessages;
        }

        function GetErrorCodeByGotadi(errorCode) {
            switch (errorCode) {
                case "5_BOOKING_PROCESS_PENDING":
                    return "5_BOOKING_PROCESS_PENDING => Không thể thay đổi thông tin booking sau khi giữ chỗ. Lỗi xảy ra do gọi API nhiều lần.";
                    break;
                case "5_BOOKING_RESERVE_FAILED_DUPLICATED":
                    return "5_BOOKING_RESERVE_FAILED_DUPLICATED => Thông tin hành trình và hành khách bị trùng với booking khác đã thanh toán thành công trước đó. Vui lòng liên hệ với đội kỹ thuật để biết chính xác thông tin booking bị trùng.";
                    break;
                case "5_BOOKING_PARAMS_NULL":
                    return "5_BOOKING_PARAMS_NULL => Thiếu thông tin liên hệ hoặc thông tin hành khách.";
                    break;
                case "5_BOOKING_TRANSACTION_STATUS_INFO_EMPTY":
                    return "5_BOOKING_TRANSACTION_STATUS_INFO_EMPTY => Lỗi không thể đặt vé từ hãng bay.";
                    break;
                case "8_TRAVELLER_DOB_INVALID_ADT":
                    return "8_TRAVELLER_DOB_INVALID_ADT => Ngày sinh người lớn không hợp lệ.";
                    break;
                case "8_TRAVELLER_DOB_INVALID_CHD":
                    return "8_TRAVELLER_DOB_INVALID_CHD => Ngày sinh trẻ em không hợp lệ.";
                    break;
                case "8_TRAVELLER_DOB_INVALID_INF":
                    return "8_TRAVELLER_DOB_INVALID_INF => Ngày sinh em bé không hợp lệ.";
                    break;
                case "FLEXI_INSURANCE_FAILED":
                    return "FLEXI_INSURANCE_FAILED => Tạo bảo hiểm du lịch thất bại.";
                    break;
                case "UNKNOWN_ERROR":
                    return "UNKNOWN_ERROR => Lỗi chưa xác định. Liên hệ đội kỹ thuật để biết thêm chi tiết.";
                default:
                    // return "UNKNOWN_ERROR => Lỗi chưa xác định. Liên hệ đội kỹ thuật để biết thêm chi tiết.";
                    return "Xảy ra lỗi trong quá trình đặt vé, vui lòng liên hệ IT Aeon để xử lý";
                    break;
            }
        }

        function GetErrorCodeAPIDraftBookingByGotadi(errorCode) {
            switch (errorCode) {
                case "8_TICKET_NOT_FOUND":
                    return "8_TICKET_NOT_FOUND => Không tìm thấy thông tin vé. Vui lòng thử lại.";
                    break;
                default:
                    return "Xảy ra lỗi trong quá trình đặt vé, vui lòng liên hệ IT Aeon để xử lý";
                    break;
            }
        }
        //Booking Ticket End

        return {
            loadFlight: loadFlight,
            searchFlight: searchFlight,
            getFareRules: getFareRules,
            filterAvailability: filterAvailability,
            formatDate: formatDate,
            bookingTicket: bookingTicket,
            groupIntinerary: groupIntinerary
        };
    }]);