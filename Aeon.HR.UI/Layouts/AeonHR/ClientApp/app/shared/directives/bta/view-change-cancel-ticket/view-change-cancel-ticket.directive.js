var ssgApp = angular.module('ssg.directiveBTAChangeCancelModule', []);
ssgApp.directive("viewChangeCancelTicket", [
    function () {
        return {
            restrict: "E",
            templateUrl: "ClientApp/app/shared/directives/bta/view-change-cancel-ticket/view-change-cancel-ticket.template.html?v=" + edocV,
            scope: {
                viewChangeCancelTicketScope: "=",
            },
            controller: (["$rootScope", "$scope", "$cacheFactory", "dataService", "bookingFlightService", "workflowService", "permissionService", "Notification", "$translate", "$timeout", "btaService"],
                function ($rootScope, $scope, $translate, $timeout, btaService, Notification, dataService) {
                    function closeDialogViewTicket() {
                        let dialog = $("#dialog_ViewChangeCancelTicket").data("kendoDialog")
                        dialog.close();
                    }
                    $scope.changeCancel = function (dataItem) {
                        if (dataItem.no === 1) {//outbound ticket
                            if (!dataItem.isCancelOutBoundFlight) {
                                $("#reasonForOutBoundFlight").val("");
                            }
                        }
                        else {
                            if (!dataItem.isCancelInBoundFlight) {
                                $("#reasonForInBoundFlight").val("");
                            }
                        }
                    }
                    $scope.closeDialog = function () {
                        closeDialogViewTicket();
                    }

                    $scope.saveReasonCancel = function () {
                        var validateValue = validateCancelTicket();
                        if (validateValue.isPass) {
                            if (!$scope.isAdminCheckerStep) {
                                $scope.viewChangeCancelTicketScope.returnChangeCancelObject($scope.dataItems);
                            }
                            else {
                                var cancelationFeeStr = "";
                                if ($scope.dataItems.length === 2) {
                                    cancelationFeeStr = `{"InBound": ${$scope.dataItems[1].penaltyFeeInBoundFlight}, "OutBound": ${$scope.dataItems[0].penaltyFeeOutBoundFlight}}`
                                }
                                else {
                                    if ($scope.dataItems[0].no === 1) {//OutBoundFlight
                                        cancelationFeeStr = `{"InBound": 0, "OutBound": ${$scope.dataItems[0].penaltyFeeOutBoundFlight}}`
                                    }
                                    else {//InBoundFlight
                                        cancelationFeeStr = `{"InBound": ${$scope.dataItems[0].penaltyFeeInBoundFlight}, "OutBound": 0}`
                                    }
                                }
                                $scope.viewChangeCancelTicketScope.returnChangeCancelObject($scope.dataItems, cancelationFeeStr);
                            }
                            Notification.success($translate.instant('COMMON_SAVE_SUCCESS'));
                            closeDialogViewTicket();
                        }
                        else {
                            Notification.error(validateValue.message);
                            return false;
                        }
                    }

                    function prepareFields(isCancel, viewMode, isAdminCheckerStep) {

                        var isCancelInBoundId = $("#isCancelInBoundFlight");
                        var isCancelOutBoundId = $("#isCancelOutBoundFlight");
                        var reasonForOutBoundFlightId = $("#reasonForOutBoundFlight");
                        var reasonForInBoundFlightId = $("#reasonForInBoundFlight");
                        var penaltyFeeOutBoundFlight = $("#penaltyFeeOutBoundFlight");
                        var penaltyFeeInBoundFlight = $("#penaltyFeeInBoundFlight");

                        if (((!isCancel && viewMode) || (isCancel && viewMode) || (!viewMode && !isCancel && !$scope.isEditViewTicket) || (isCancel && !viewMode && $scope.isEditViewTicket)) && !isAdminCheckerStep) {
                            if (isCancelInBoundId.length > 0) {
                                isCancelInBoundId.attr('disabled', 'disabled');
                            }
                            if (isCancelOutBoundId.length > 0) {
                                isCancelOutBoundId.attr('disabled', 'disabled');
                            }
                            if (reasonForOutBoundFlightId.length > 0) {
                                reasonForOutBoundFlightId.attr('disabled', 'disabled');
                            }
                            if (reasonForInBoundFlightId.length > 0) {
                                reasonForInBoundFlightId.attr('disabled', 'disabled');
                            }
                            if (penaltyFeeInBoundFlight.length > 0) {
                                penaltyFeeInBoundFlight.attr('disabled', 'disabled');
                            }
                            if (penaltyFeeOutBoundFlight.length > 0) {
                                penaltyFeeOutBoundFlight.attr('disabled', 'disabled');
                            }
                        }
                        else {
                            if (!isAdminCheckerStep) {
                                if (isCancelInBoundId.length > 0) {
                                    isCancelInBoundId.removeAttr("disabled");
                                }
                                if (isCancelOutBoundId.length > 0) {
                                    isCancelOutBoundId.removeAttr("disabled");
                                }
                                if (reasonForOutBoundFlightId.length > 0) {
                                    reasonForOutBoundFlightId.removeAttr("disabled");
                                }
                                if (reasonForInBoundFlightId.length > 0) {
                                    reasonForInBoundFlightId.removeAttr("disabled");
                                }
                            }
                            else {
                                if (isCancelInBoundId.length > 0) {
                                    isCancelInBoundId.attr('disabled', 'disabled');
                                }
                                if (isCancelOutBoundId.length > 0) {
                                    isCancelOutBoundId.attr('disabled', 'disabled');
                                }
                                if (reasonForOutBoundFlightId.length > 0) {
                                    reasonForOutBoundFlightId.attr('disabled', 'disabled');
                                }
                                if (reasonForInBoundFlightId.length > 0) {
                                    reasonForInBoundFlightId.attr('disabled', 'disabled');
                                }
                                if (penaltyFeeInBoundFlight.length > 0) {
                                    penaltyFeeInBoundFlight.removeAttr("disabled");
                                }
                                if (penaltyFeeOutBoundFlight.length > 0) {
                                    penaltyFeeOutBoundFlight.removeAttr("disabled");
                                }
                            }
                        }
                    }
                    function SortByNo(a, b) {
                        var aNo = a.no;
                        var bNo = b.no;
                        return ((aNo < bNo) ? -1 : ((aNo > bNo) ? 1 : 0));
                    }
                    function prepareObj(flightTicketInfo, changeCancelInfo) {
                        var dataItemsTemp = [];
                        var cancellationFeeObj = '';
                       
                        if (!$.isEmptyObject(flightTicketInfo) && !$.isEmptyObject(changeCancelInfo)) {
                            if (flightTicketInfo.length > 0) {
                                if (changeCancelInfo.cancellationFeeObj) {
                                    cancellationFeeObj = JSON.parse(changeCancelInfo.cancellationFeeObj);
                                }
                                flightTicketInfo.forEach(cItem => {
                                    var pricedItinerariesObject = cItem.pricedItineraries;
                                    if (cItem.directFlight) {
                                        var dataObj = {
                                            no: 1,
                                            sapCode: changeCancelInfo.sapCode,
                                            imgAirLogo: cItem.imgAirLogo,
                                            departureAirportLocationCode: cItem.departureAirportLocationCode,
                                            arrivalAirportLocationCode: cItem.arrivalAirportLocationCode,
                                            isCancel: changeCancelInfo.isCancel,
                                            isCancelOutBoundFlight: changeCancelInfo.isCancelOutBoundFlight,
                                            reasonForOutBoundFlight: changeCancelInfo.reasonForOutBoundFlight,
                                            departureDateTime: cItem.departureDateTime,
                                            arrivalDateTime: cItem.arrivalDateTime,
                                            originLocationCode: cItem.originLocationCode,
                                            destinationLocationCode: cItem.destinationLocationCode,
                                            cabinClassName: pricedItinerariesObject.cabinClassName,
                                            flightNo: cItem.flightNo,
                                            directFlight: cItem.directFlight,
                                            reasonId: "#reasonForOutBoundFlight",
                                            sumAmount: cItem.includeEquivfare + cItem.pricedItineraries.airItineraryPricingInfo.adultFare.passengerFare.baseFare.amount + cItem.pricedItineraries.airItineraryPricingInfo.adultFare.passengerFare.serviceTax.amount,
                                            penaltyFeeOutBoundFlight: cancellationFeeObj === '' ? 0 : cancellationFeeObj.OutBound
                                        };
                                        dataItemsTemp.push(dataObj);
                                    }
                                    else {
                                        var dataObj = {
                                            no: 2,
                                            sapCode: changeCancelInfo.sapCode,
                                            imgAirLogo: cItem.imgAirLogo,
                                            departureAirportLocationCode: cItem.departureAirportLocationCode,
                                            arrivalAirportLocationCode: cItem.arrivalAirportLocationCode,
                                            isCancel: changeCancelInfo.isCancel,
                                            isCancelInBoundFlight: changeCancelInfo.isCancelInBoundFlight,
                                            reasonForInBoundFlight: changeCancelInfo.reasonForInBoundFlight,
                                            departureDateTime: cItem.departureDateTime,
                                            arrivalDateTime: cItem.arrivalDateTime,
                                            originLocationCode: cItem.originLocationCode,
                                            destinationLocationCode: cItem.destinationLocationCode,
                                            cabinClassName: pricedItinerariesObject.cabinClassName,
                                            flightNo: cItem.flightNo,
                                            directFlight: cItem.directFlight,
                                            reasonId: "#reasonForInBoundFlight",
                                            sumAmount: cItem.includeEquivfare + cItem.pricedItineraries.airItineraryPricingInfo.adultFare.passengerFare.baseFare.amount + cItem.pricedItineraries.airItineraryPricingInfo.adultFare.passengerFare.serviceTax.amount,
                                            penaltyFeeInBoundFlight: cancellationFeeObj === '' ? 0 : cancellationFeeObj.InBound
                                        };
                                        dataItemsTemp.push(dataObj);
                                    }
                                });
                                dataItemsTemp.sort(SortByNo);
                                $scope.dataItems = dataItemsTemp;
                            }
                        }
                        $scope.$apply();
                    }

                    $scope.viewUserTicket = async function (userSapCode, BTADetailId, viewMode, currentChangeOrCancelRow, isAdminCheckerStep, isEditViewTicket, viewWhenCompleted) {
                        $scope.isAdminCheckerStep = isAdminCheckerStep;
                        $scope.isViewMode = viewMode;
                        $scope.isEditViewTicket = isEditViewTicket;
                        $scope.viewWhenCompleted = viewWhenCompleted;
                        if (viewWhenCompleted) {
                            $scope.isCancel = false;
                        }
                        else {
                            $scope.isCancel = currentChangeOrCancelRow.isCancel;
                        }
                        var result = await btaService.getInstance().bussinessTripApps.getUserTicketsInfo({ BTADetailId: BTADetailId }).$promise;
                        if (result.isSuccess) {

                            var datas = result.object;
                            $scope.dataItems = [];

                            var flightTicketInfo = datas.flightTicketInfo;
                            var changeCancelInfo = currentChangeOrCancelRow ? currentChangeOrCancelRow : {};
                            if ($.isEmptyObject(changeCancelInfo) && viewWhenCompleted) { // ViewTicket when completed
                                changeCancelInfo["isCancel"] = false;
                                changeCancelInfo["isCancelInBoundFlight"] = false;
                                changeCancelInfo["reasonForInBoundFlight"] = "";
                                changeCancelInfo["isCancelOutBoundFlight"] = false;
                                changeCancelInfo["reasonForOutBoundFlight"] = "";
                                changeCancelInfo["cancellationFeeObj"] = null;
                            }

                            if (datas.changeCancelInfo !== null && !isEditViewTicket) {
                                changeCancelInfo = datas.changeCancelInfo;
                                if (isAdminCheckerStep && currentChangeOrCancelRow.cancellationFeeObj) {
                                    changeCancelInfo.cancellationFeeObj = currentChangeOrCancelRow.cancellationFeeObj;
                                }
                            }

                            if (viewMode) {
                                prepareObj(flightTicketInfo, changeCancelInfo);                                
                                prepareFields(changeCancelInfo.isCancel, viewMode, isAdminCheckerStep);
                            }
                            else { //edit mode 
                                prepareObj(flightTicketInfo, changeCancelInfo)
                                prepareFields(changeCancelInfo.isCancel, viewMode, isAdminCheckerStep);
                            }
                        }

                        //===========================================================
                    }

                    function validateCancelTicket() {

                        var message = "";
                        var isPass = false;

                        if ($scope.isAdminCheckerStep) {
                            if ($scope.dataItems.length === 2) {
                                if (($scope.dataItems[0].isCancelOutBoundFlight && $scope.dataItems[0].penaltyFeeOutBoundFlight == 0)
                                    || ($scope.dataItems[1].isCancelInBoundFlight && $scope.dataItems[1].penaltyFeeInBoundFlight == 0)) {
                                    message = $translate.instant('BTA_VALIDATE_CANCELLATIONFEE');
                                }
                                else {
                                    isPass = true;
                                }
                            }
                            else {
                                if ($scope.dataItems[0].no === 1) { //OutBoundFlight
                                    if ($scope.dataItems[0].isCancelOutBoundFlight && $scope.dataItems[0].penaltyFeeOutBoundFlight == 0) {
                                        message = $translate.instant('BTA_VALIDATE_CANCELLATIONFEE');
                                    }
                                    else {
                                        isPass = true;
                                    }
                                }
                                else {//InBoundFlight
                                    if ($scope.dataItems[0].isCancelInBoundFlight && $scope.dataItems[0].penaltyFeeInBoundFlight == 0) {
                                        message = $translate.instant('BTA_VALIDATE_CANCELLATIONFEE');
                                    }
                                    else {
                                        isPass = true;
                                    }
                                }
                            }
                        }
                        else {
                            if ($scope.dataItems.length === 2) {
                                if (!$scope.dataItems[0].isCancelOutBoundFlight && !$scope.dataItems[1].isCancelInBoundFlight) {
                                    message = $translate.instant('BTA_VALIDATE_IS_CANCEL');
                                }
                                else {
                                    if (($scope.dataItems[0].isCancelOutBoundFlight && $scope.dataItems[0].reasonForOutBoundFlight == "")
                                        || ($scope.dataItems[1].isCancelInBoundFlight && $scope.dataItems[1].reasonForInBoundFlight == "")) {
                                        message = $translate.instant('BTA_VALIDATE_REASON');
                                    }
                                    else {
                                        isPass = true;
                                    }
                                }
                            }
                            else {
                                if ($scope.dataItems[0].no === 1) { //OutBoundFlight
                                    if (!$scope.dataItems[0].isCancelOutBoundFlight) {
                                        message = $translate.instant('BTA_VALIDATE_IS_CANCEL');
                                    }
                                    else {
                                        if ($scope.dataItems[0].isCancelOutBoundFlight && $scope.dataItems[0].reasonForOutBoundFlight == "") {
                                            message = $translate.instant('BTA_VALIDATE_REASON');
                                        }
                                        else {
                                            isPass = true;
                                        }
                                    }
                                }
                                else {//InBoundFlight
                                    if (!$scope.dataItems[0].isCancelInBoundFlight) {
                                        message = $translate.instant('BTA_VALIDATE_IS_CANCEL');
                                    }
                                    else {
                                        if ($scope.dataItems[0].isCancelInBoundFlight && $scope.dataItems[0].reasonForInBoundFlight == "") {
                                            message = $translate.instant('BTA_VALIDATE_REASON');
                                        }
                                        else {
                                            isPass = true;
                                        }
                                    }
                                }
                            }
                        }

                        return { message: message, isPass: isPass };
                    }
                    async function ngInit() {
                        $scope.viewChangeCancelTicketScope.viewUserTicket = await $scope.viewUserTicket;
                    }
                    ngInit();

                })
        }
    }
]);