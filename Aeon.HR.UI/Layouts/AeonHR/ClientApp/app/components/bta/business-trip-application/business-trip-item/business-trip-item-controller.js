var ssgApp = angular.module('ssg.businessTripItemModule', ["kendo.directives"]);
ssgApp.controller('businessTripItemController', function ($rootScope, $scope, $location, appSetting, localStorageService, $stateParams, $state, moment, Notification, settingService, $timeout, cbService, dataService, workflowService, ssgexService, fileService, commonData, $translate, attachmentService, attachmentFile, $compile, btaService, bookingFlightService) {

    $scope.title = $stateParams.id ? $translate.instant('BUSINESS_TRIP_APPLICATION') + ': ' + $stateParams.referenceValue : $translate.instant('BUSINESS_TRIP_APPLICATION_NEW_TITLE');
    this.$onInit = function () {
        $scope.disableChangeOrCancel = false;
        $scope.perm = true;
        $scope.forceChangeBusinessTrip = false;
        $scope.btaListDetailGridData = [];
        $scope.btaListDetailGroupedGridData = [];
        $scope.btaListChangeOrCancelGridData = [];
        $scope.searchTicketScope = {};
        $scope.viewTicketScope = {};
        $scope.viewChangeCancelTicketScope = {};
        $scope.searchTicketModel = {};
        $scope.ticketPriceInfos = {};
        $scope.sapCodeDataSource = [];
        $scope.isViewMode = false;
        var voteModel = {};
        var workflowStatus = {};
        $scope.hotels = [];
        $scope.genderOption = [
            { name: 'Nam/ Male', value: 1 },
            { name: 'Nữ/ Female', value: 2 },
        ];
        $scope.businessTypeOption = [
            { name: " ", value: "" },
            { name: $translate.instant('BTA_DOMESTIC'), value: 1 },
            { name: $translate.instant('BTA_DOMESTIC_BY_PLANE'), value: 3 },
            { name: $translate.instant('BTA_INTERNATIONAL'), value: 2 }
        ];
        $scope.roundTripOption = [
            { name: " ", value: "" },
            { name: $translate.instant('COMMON_ONEWAY'), value: 1 },
            { name: $translate.instant('COMMON_ROUNDTRIP'), value: 2 },
        ];
        $scope.model.type = ""; // set type Dosmetic
        $scope.model.isRoundTrip = ""; // set type Round Trip
        $scope.model.dataRoundTrip = "";

        var btaListDetailGridColumns = [
            {
                field: "no",
                headerTemplate: $translate.instant('COMMON_NO'),
                width: "40px",
                locked: true,
                editable: function (e) {
                    return false;
                },
            },
            {
                field: "sapCode",
                headerTemplate: $translate.instant('COMMON_SAP_CODE'),
                width: "90px",
                locked: true,
                editable: function (e) {
                    return false;
                },
                tempalte: function (dataItem) {
                    return `<input name="others" ng-readonly="true" class="k-input" type="text" ng-model="dataItem.sapCode" style="color: #333;width: 100%"/>`;
                }
            },
            {
                field: "fullName",
                headerTemplate: $translate.instant('COMMON_FULL_NAME'),
                width: "200px",
                locked: true,
                editable: function (e) {
                    return false;
                },
                tempalte: function (dataItem) {
                    return `<input name="others" ng-readonly="false" class="k-input" type="text" ng-model="dataItem.fullName" style="color: #333;width: 100%"/>`;
                }
            },
            {
                field: "departmentName",
                headerTemplate: $translate.instant('COMMON_DEPARTMENT'),
                width: "200px",
                locked: true,
                editable: function (e) {
                    return false;
                },
                tempalte: function (dataItem) {
                    return `<input name="others" autocomplete="off" ng-readonly="false" class="k-input" type="text" required ng-model="dataItem.departmentName" style="color: #333;width: 100%"/>`;
                }
            },
            {
                field: "departureCode",
                headerTemplate: $translate.instant('BTA_DEPARTURE') + '<span class="font-red-thunderbird"> *</span>',
                width: "200px",
                template: function (dataItem) {
                    if ($scope.model.type == 1) {
                        return `<select kendo-drop-down-list style="color: #333;width: 100%;"
                        data-k-ng-model="dataItem.departureCode"
                        k-data-text-field="'name'"
                        k-data-value-field="'code'"
                        k-template="'<span><label>#: name# </label></span>'",
                        k-valueTemplate="'#: name #'",
                        k-auto-bind="'true'"
                        k-value-primitive="'false'"
                        k-data-source="departureOption"
                        k-filter="'contains'",
                        k-on-change="changeCommon('departure', dataItem)"
                        > </select>`;
                    }
                    else {
                        $scope.departureFromGotadiOption = new kendo.data.DataSource({
                            transport: {
                                read: async function (options) {
                                    let searchParam = {
                                        "queryText": "",
                                        "country": ""
                                    };
                                    if (!$.isEmptyObject(options.data) && !$.isEmptyObject(options.data.filter) && !$.isEmptyObject(options.data.filter) && !$.isEmptyObject(options.data.filter.filters)) {
                                        let filter = options.data.filter.filters;
                                        if (filter.length > 0) {
                                            searchParam.queryText = filter[0].value;
                                        }
                                    }
                                    if ($scope.model.type == 3) {
                                        searchParam.country = "VN";
                                    }
                                    else {
                                        if (searchParam.queryText == "") {
                                            searchParam.country = "VN";
                                        }
                                    }
                                    let searchAirportResult = await btaService.getInstance().bussinessTripApps.searchAirport(searchParam, null).$promise;
                                    let airportInfos = searchAirportResult.object;


                                    let btaDetailItems = $("#btaListDetailGrid").data("kendoGrid").dataSource._data;
                                    if (!$.isEmptyObject(btaDetailItems) && btaDetailItems.length > 0) {
                                        let departureInfos = $(btaDetailItems).map(function (cIndex, cItem) {
                                            return JSON.parse(cItem.departureInfo);
                                        });
                                        departureInfos = $.grep(departureInfos, function (el, i) {
                                            let checkResult = $(airportInfos).filter(function (cIndex, cItem) {
                                                return el.code == cItem.code;
                                            });
                                            if (checkResult.length == 0) {
                                                airportInfos.push(el);
                                            }
                                        });

                                    }

                                    options.success(airportInfos);
                                }
                            },
                            serverFiltering: true
                        });
                        return `<select kendo-drop-down-list style="color: #333;width: 100%;"
                        id="departureSelect${dataItem.no}"
                        data-k-ng-model="dataItem.departureCode"
                        k-data-value-field="'code'"
                        k-template="'<span>#: data.city# (#: data.code#), #: data.country#</span>'",
                        k-value-template="'<span>#: bta_GetAirportValueTemplate(data)#'",
                        k-auto-bind="'true'"
                        k-value-primitive="'false'"
                        k-data-source="departureFromGotadiOption"
                        k-filter="'contains'"
                        k-on-change="changeCommon('departure', dataItem)"
                        > </select>`;
                    }
                }
            },
            {
                field: "arrivalCode",
                headerTemplate: $translate.instant('BTA_ARRIVAL') + '<span class="font-red-thunderbird"> *</span>',
                width: "200px",
                template: function (dataItem) {
                    if ($scope.model.type == 1) {
                        return `<select kendo-drop-down-list style="color: #333;width: 100%;"
                        data-k-ng-model="dataItem.arrivalCode"
                        k-data-text-field="'name'"
                        k-data-value-field="'code'"
                        k-template="'<span><label>#: name# </label></span>'",
                        k-valueTemplate="'#: name #'",
                        k-auto-bind="'true'"
                        k-value-primitive="'false'"
                        k-data-source="arrivalOption"
                        k-filter="'contains'",
                        k-on-change="changeArrival(dataItem)"
                        ></select>`;
                    }
                    else {
                        $scope.arrivalFromGotadiOption = new kendo.data.DataSource({
                            transport: {
                                read: async function (options) {
                                    let searchParam = {
                                        "queryText": "",
                                        "country": ""
                                    };
                                    if (!$.isEmptyObject(options.data) && !$.isEmptyObject(options.data.filter) && !$.isEmptyObject(options.data.filter) && !$.isEmptyObject(options.data.filter.filters)) {
                                        let filter = options.data.filter.filters;
                                        if (filter.length > 0) {
                                            searchParam.queryText = filter[0].value;
                                        }
                                    }
                                    if ($scope.model.type == 3) {
                                        searchParam.country = "VN";
                                    }
                                    else {
                                        if (searchParam.queryText == "") {
                                            searchParam.country = "VN";
                                        }
                                    }
                                    let searchAirportResult = await btaService.getInstance().bussinessTripApps.searchAirport(searchParam, null).$promise;
                                    let airportInfos = searchAirportResult.object;

                                    let btaDetailItems = $("#btaListDetailGrid").data("kendoGrid").dataSource._data;
                                    if (!$.isEmptyObject(btaDetailItems) && btaDetailItems.length > 0) {
                                        let arrivalInfos = $(btaDetailItems).map(function (cIndex, cItem) {
                                            return JSON.parse(cItem.arrivalInfo);
                                        });
                                        arrivalInfos = $.grep(arrivalInfos, function (el, i) {
                                            let checkResult = $(airportInfos).filter(function (cIndex, cItem) {
                                                return el.code == cItem.code;
                                            });
                                            if (checkResult.length == 0) {
                                                airportInfos.push(el);
                                            }
                                        });
                                    }
                                    options.success(airportInfos);
                                }
                            },
                            serverFiltering: true
                        });
                        return `<select kendo-drop-down-list style="color: #333;width: 100%;"
                        id="arrivalSelect${dataItem.no}"
                        data-k-ng-model="dataItem.arrivalCode"
                        k-data-value-field="'code'"
                        k-template="'<span>#: data.city# (#: data.code#), #: data.country#</span>'",
                        k-value-template="'<span>#: bta_GetAirportValueTemplate(data)#'",
                        k-value-primitive="'false'"
                        k-data-source="arrivalFromGotadiOption"
                        k-filter="'contains'"
                        k-on-change="changeArrival(dataItem)"
                        > </select>`;
                    }
                }
            },
            {
                field: "fromDate",
                headerTemplate: $translate.instant('BTA_TRIP_FROM_DATE') + '<span class="font-red-thunderbird"> *</span>',
                width: "220px",
                format: "{0:dd/MM/yyyy}",
                template: function (dataItem) {
                    return `<input kendo-date-time-picker
                    id="fromDate${dataItem.no}"
                    autocomplete="off" 
                    k-ng-model="dataItem.fromDate"
                    k-date-input="true"
                    k-format="'dd/MM/yyyy HH:mm'"
                    k-time-format="'HH:mm'"
                    k-on-change="fromDateGridChanged('#toDate', dataItem)"
                    style="color: #333;width: 100%;" />`;
                }
            },
            {
                field: "toDate",
                headerTemplate: $translate.instant('BTA_TRIP_TO_DATE') + '<span class="font-red-thunderbird"> *</span>',
                width: "220px",
                format: "{0:dd/MM/yyyy}",
                template: function (dataItem) {
                    return `<input kendo-date-time-picker
                    id="toDate${dataItem.no}"
                    autocomplete="off" 
                    k-ng-model="dataItem.toDate"
                    k-date-input="true"
                    k-format="'dd/MM/yyyy HH:mm'"
                    k-time-format="'HH:mm'"
                    k-on-change="toDateGridChange(dataItem)"
                    style="color: #333;width: 100%;" />`;
                }
            },
            {
                field: "isForeigner",
                headerTemplate: $translate.instant('BTA_IS_FOREIGNER'),
                width: "100px",
                template: function (dataItem) {
                    return `<input type="checkbox" ng-model="dataItem.isForeigner" name="isForeigner{{dataItem.no}}"
                    id="isForeigner{{dataItem.no}}" class="k-checkbox" style="width: 100%;" ng-change='changeForeigner(dataItem)'/>
                    <label class="k-checkbox-label cbox" for="isForeigner{{dataItem.no}}"></label>`
                }
            },
            {
                field: "gender",
                headerTemplate: $translate.instant('COMMON_GENDER') + '<span class="font-red-thunderbird"> *</span>',
                width: "150px",
                template: function (dataItem) {
                    return `<select kendo-drop-down-list style="color: #333;width: 100%;"
                        data-k-ng-model="dataItem.gender"
                        k-data-text-field="'name'"
                        k-data-value-field="'value'"
                        k-template="'<span><label>#: data.name#</label></span>'",
                        valueTemplate="'#: name #'",
                        k-auto-bind="'false'"
                        k-value-primitive="'false'"
                        k-data-source="genderOption"
                        k-filter="'contains'",
                        > </select>`;
                }
            },
            {
                field: "stayHotel",
                headerTemplate: $translate.instant('BTA_STAY_HOTEL'),
                width: "100px",
                template: function (dataItem) {
                    return `<input type="checkbox" ng-model="dataItem.stayHotel" name="stayHotel{{dataItem.no}}"
                    id="stayHotel{{dataItem.no}}" class="k-checkbox" style="width: 100%;" ng-change="stayHotelChange(dataItem)"/>
                    <label class="k-checkbox-label cbox" for="stayHotel{{dataItem.no}}"></label>`
                }
            },
            {
                field: "hotelCode",
                headerTemplate: $translate.instant('BTA_HOTEL'),
                width: "220px",
                template: function (dataItem) {
                    return `<select kendo-drop-down-list style="color: #333;width: 95%;"
                        id="hotel${dataItem.no}"
                        data-k-ng-model="dataItem.hotelCode"
                        k-data-text-field="'name'"
                        k-data-value-field="'code'"
                        k-template="'<span><label>#: data.name# </label></span>'",
                        k-valueTemplate="'#: name #'",
                        k-auto-bind="'true'"
                        k-value-primitive="'false'"
                        k-filter="'contains'",
                        k-ng-disabled="!dataItem.stayHotel"
                        k-ng-readonly="!dataItem.stayHotel"
                        k-on-change="changeCommon('hotel', dataItem)"
                        > </select>`+ '<span ng-if="dataItem.stayHotel == true"  class="font-red-thunderbird"> *</span>';
                }
            },
            {
                field: "checkInHotelDate",
                headerTemplate: $translate.instant('BTA_CHECK_IN_HOTEL_DATE'),
                width: "220px",
                format: "{0:dd/MM/yyyy}",
                template: function (dataItem) {
                    return `<input kendo-date-time-picker
                    id="checkInHotelDate${dataItem.no}" 
                    k-ng-model="dataItem.checkInHotelDate"
                    autocomplete="off" 
                    k-date-input="true"
                    k-format="'dd/MM/yyyy HH:mm'"
                    k-time-format="'HH:mm'"
                    k-ng-disabled="!dataItem.stayHotel"
                    k-ng-readonly="!dataItem.stayHotel"
                    k-on-change="fromDateCheckInOutChanged(dataItem)"
                    style="color: #333;width: 95%;" />`+ '<span ng-if="dataItem.stayHotel == true"  class="font-red-thunderbird"> *</span>';
                }
            },
            {
                field: "checkOutHotelDate",
                headerTemplate: $translate.instant('BTA_CHECK_OUT_HOTEL_DATE'),
                width: "220px",
                format: "{0:dd/MM/yyyy}",
                template: function (dataItem) {
                    return `<input kendo-date-time-picker                    
                    id="checkOutHotelDate${dataItem.no}"
                    autocomplete="off" 
                    k-ng-model="dataItem.checkOutHotelDate"
                    k-date-input="true"
                    k-format="'dd/MM/yyyy HH:mm'"
                    k-ng-disabled="!dataItem.stayHotel"
                    k-ng-readonly="!dataItem.stayHotel"
                    k-on-change="toDateCheckInOutChanged(dataItem)"
                    k-time-format="'HH:mm'"
                    style="color: #333;width: 95%;" />`+ '<span ng-if="dataItem.stayHotel == true"  class="font-red-thunderbird"> *</span>';
                }
            },
            {
                field: "firstName",
                headerTemplate: $translate.instant('BTA_GIVEN_NAME') + '<span ng-if="model.type == 3 || model.type == 2"  class="font-red-thunderbird"> *</span>',
                width: "200px",
                template: function (dataItem) {
                    return `<input name="firstName" autocomplete="off" ng-readonly="false" class="k-input" type="text" ng-model="dataItem.firstName" style="color: #333;width: 100%"/>`;
                }
            },
            {
                field: "lastName",
                headerTemplate: $translate.instant('BTA_SURNAME') + '<span ng-if="model.type == 3 || model.type == 2"  class="font-red-thunderbird"> *</span>',
                width: "100px",
                template: function (dataItem) {
                    return `<input name="lastName" autocomplete="off" ng-readonly="false" class="k-input" type="text" ng-model="dataItem.lastName" style="color: #333;width: 100%"/>`;
                }
            },
            {
                field: "email",
                headerTemplate: $translate.instant('BTA_EMAIL') + '<span ng-if="model.type == 3 || model.type == 2"  class="font-red-thunderbird"> *</span>',
                width: "200px",
                template: function (dataItem) {
                    return `<input name="email" autocomplete="off" ng-readonly="false" class="k-input" type="text" ng-model="dataItem.email" style="color: #333;width: 100%"/>`;
                }
            },
            {
                field: "mobile",
                headerTemplate: $translate.instant('COMMON_MOBILE') + '<span ng-if="model.type == 3 || model.type == 2"  class="font-red-thunderbird"> *</span>',
                width: "120px",
                template: function (dataItem) {
                    return `<input name="mobile" autocomplete="off" class="k-input" type="text" ng-model="dataItem.mobile" style="color: #333;width: 92%"
                            ng-keyup="phoneNumberKeyPress(event.keyCode, dataItem)"/>`;
                }
            },
            {
                field: "idCard",
                //headerTemplate: $translate.instant('COMMON_MOBILE'),
                headerTemplate: $translate.instant('BTA_ID_CARD') + '<span ng-if="(model.type == 3)"  class="font-red-thunderbird"> *</span>',
                width: "120px",
                template: function (dataItem) {
                    return `<input name="idCard" autocomplete="off" class="k-input" maxlength="12" type="text" ng-model="dataItem.idCard" style="color: #333;width: 92%"
                    onkeypress="return event.charCode >= 48 && event.charCode <= 57"/>`
                        ;
                }
            },
            {
                field: "dateOfBirth",
                headerTemplate: $translate.instant('BTA_DATE_OF_BIRTH') + '<span ng-if="(model.type == 3 || model.type == 2)"  class="font-red-thunderbird"> *</span>',
                width: "200px",
                format: "{0:dd/MM/yyyy}",
                template: function (dataItem) {
                    return `<input kendo-date-picker
                    id="dateOfBirth${dataItem.no}"
                    autocomplete="off" 
                    k-ng-model="dataItem.dateOfBirth"
                    k-date-input="true"
                    k-format="'dd/MM/yyyy'"
                    style="color: #333;width: 100%;" />`;
                }
            },
            {
                field: "passport",
                headerTemplate: $translate.instant('BTA_PASSPORT') + '<span ng-if="model.type == 2"  class="font-red-thunderbird"> *</span>',
                width: "120px",
                template: function (dataItem) {
                    return `<input name="passport" autocomplete="off" class="k-input" type="text" ng-model="dataItem.passport" style="color: #333;width: 92%"/>`
                        + '<span ng-if="dataItem.isForeigner == true"  class="font-red-thunderbird pull-right"> *</span>';
                }
            },
            {
                field: "passportDateOfIssue",
                headerTemplate: $translate.instant('BTA_PASSPORT_DATE_OF_ISSUE') + '<span ng-if="model.type == 2"  class="font-red-thunderbird"> *</span>',
                width: "200px",
                format: "{0:dd/MM/yyyy}",
                template: function (dataItem) {
                    return `<input kendo-date-picker
                    id="passportDateOfIssue${dataItem.no}"
                    autocomplete="off" 
                    k-ng-model="dataItem.passportDateOfIssue"
                    k-date-input="true"
                    k-format="'dd/MM/yyyy'"
                    style="color: #333;width: 95%;" />`
                        + '<span ng-if="model.type == 2 && dataItem.isForeigner == true"  class="font-red-thunderbird pull-right"> *</span>';
                }
            },
            {
                field: "passportExpiryDate",
                headerTemplate: $translate.instant('BTA_PASSPORT_EXPIRY_DATE_ISSUE') + '<span ng-if="model.type == 2"  class="font-red-thunderbird"> *</span>',
                width: "200px",
                format: "{0:dd/MM/yyyy}",
                template: function (dataItem) {
                    return `<input kendo-date-picker
                    id="passportExpiryDate${dataItem.no}"
                    autocomplete="off"
                    k-ng-model="dataItem.passportExpiryDate"
                    k-date-input="true"
                    k-format="'dd/MM/yyyy'"
                    style="color: #333;width: 95%;" />`
                        + '<span ng-if="model.type == 2 && dataItem.isForeigner == true"  class="font-red-thunderbird pull-right"> *</span>';
                }
            },
            {
                field: "rememberInformation",
                headerTemplate: $translate.instant('BTA_REMEMBER_INFORMATION'),
                width: "200px",
                template: function (dataItem) {
                    return `<input type="checkbox" ng-model="dataItem.rememberInformation" name="rInfo{{dataItem.no}}"
                    id="rInfo{{dataItem.no}}" class="k-checkbox" style="width: 100%;"/>
                    <label class="k-checkbox-label cbox" for="rInfo{{dataItem.no}}"></label>`
                }
            },
            {
                headerTemplate: $translate.instant('COMMON_ACTION'),
                width: "100px",
                template: function (dataItem) {
                    if ($scope.model.type === 3 || $scope.model.type === 2) {//booking flight WF
                        $scope.isBookingFlightWF = true;
                    }
                    if ($scope.isBookingFlightWF && ($scope.model.status === "Completed" || $scope.model.status === "Completed Changing" || $scope.model.status === "Cancelled Booking")) {
                        return `<a class="btn btn-sm default green-stripe" ng-click="viewTicketForCurrentUser(dataItem.sapCode, dataItem.id, true, isChangingAdminCheckerStep, true)">{{'BTA_VIEW_TICKET'|translate}}</a>`;
                    }
                    return `<a class="btn btn-sm default red-stripe" ng-click="deleteRecord('btaListDetailGrid', dataItem.no)">{{'COMMON_BUTTON_DELETE'|translate}}</a>`;
                }
            }
        ]


        $scope.phoneNumberKeyPress = function (keyCode, dataItem) {
            dataItem.mobile = dataItem.mobile.replace(/[^0-9.]/g, '');
        }

        $scope.isClickReviewUserForChanging = false;
        var btaListChangeOrCancelGridColumns = [
            {
                field: "no",
                headerTemplate: $translate.instant('COMMON_NO'),
                width: "40px",
                editable: function (e) {
                    return false;
                }
            },
            {
                field: "sapCode",
                headerTemplate: $translate.instant('COMMON_SAP_CODE'),
                width: "90px",
                template: function (dataItem) {
                    // return `<select kendo-drop-down-list class="w100"
                    // data-k-ng-model="dataItem.sapCode"
                    // k-data-text-field="'sapCode'"
                    // k-data-value-field="'sapCode'"
                    // k-template="'<span><label>#: data.sapCode# - #: data.fullName#</label></span>'",
                    // k-valueTemplate="'#: sapCode #'",
                    // k-auto-bind="'false'"                   
                    // k-value-primitive="'false'"
                    // k-data-source="sapCodeDataSource"
                    // k-filter="'contains'"
                    // k-on-change="changeSapCode(dataItem, sapCodeDataSource)"                        
                    // > </select>                    
                    // `;
                    return `<input name="sapCode" ng-readonly="true" class="k-input" type="text" ng-model="dataItem.sapCode" style="color: #333;width: 100%" ng-class="{'disabled': dataItem.isCancel}"/>`;
                }
            },
            {
                field: "fullName",
                headerTemplate: $translate.instant('COMMON_FULL_NAME'),
                width: "200px",
                template: function (dataItem) {
                    return `<input name="others" ng-readonly="true" class="k-input" type="text" ng-model="dataItem.fullName" style="color: #333;width: 100%" ng-class="{'disabled': dataItem.isCancel}"/>`;
                }
            },
            {
                field: "fromDate",
                headerTemplate: $translate.instant('BTA_TRIP_FROM_DATE'),
                width: "220px",
                //format: "{0:dd/MM/yyyy}",
                template: function (dataItem) {
                    return `<input kendo-date-time-picker
                        id="fromDateCancel${dataItem.no}" 
                        k-ng-model="dataItem.fromDate"
                        k-date-input="true"
                        k-format="'dd/MM/yyyy HH:mm'"
                        k-time-format="'HH:mm'"
                        k-on-change="fromDateGridChangingCancellingChanged(dataItem)"
                        style="color: #333;width: 100%;" 
                        ng-class="{'disabled': dataItem.isCancel}"/>`;
                }
            },
            {
                field: "toDate",
                headerTemplate: $translate.instant('BTA_TRIP_TO_DATE'),
                width: "220px",
                //format: "{0:dd/MM/yyyy}",
                template: function (dataItem) {
                    return `<input kendo-date-time-picker
                        id="toDateCancel${dataItem.no}" 
                        k-ng-model="dataItem.toDate"
                        k-date-input="true"
                        k-format="'dd/MM/yyyy HH:mm'"
                        k-time-format="'HH:mm'"
                        k-on-change="toDateGridChangingCancellingChanged(dataItem)"
                        style="color: #333;width: 100%;" 
                        ng-class="{'disabled': dataItem.isCancel}"/>`;
                }
            },
            {
                field: "destinationCode",
                headerTemplate: $translate.instant('BTA_NEW_DESTINATION'),
                width: "220px",
                template: function (dataItem) {
                    if ($scope.model.type == 1) {
                        return `<select kendo-drop-down-list class="w100"
                        id="destination${dataItem.no}"                    
                        data-k-ng-model="dataItem.destinationCode"
                        k-data-text-field="'name'"
                        k-data-value-field="'code'"
                        k-template="'<span><label>#: data.name# </label></span>'",
                        k-valueTemplate="'#: name #'",
                        k-auto-bind="'true'"                   
                        k-value-primitive="'false'"
                        k-data-source="arrivalOption"
                        k-filter="'contains'",
                        k-on-change="changeDestination(dataItem)"                       
                        ng-class="{'disabled': dataItem.isCancel}"> </select>                    
                    `;
                    }
                    else if ($scope.model.type == 2 || $scope.model.type == 3) {

                        return `<select kendo-drop-down-list style="color: #333;width: 100%;"
                        id="destination${dataItem.no}"
                        data-k-ng-model="dataItem.arrivalCode"
                        k-data-value-field="'code'"
                        k-template="'<span>#: data.city# (#: data.code#), #: data.country#</span>'",
                        k-value-template="'<span>#: bta_GetAirportValueTemplate(data)#'",
                        k-value-primitive="'false'"
                        k-data-source="arrivalFromGotadiOption"
                        k-filter="'contains'"
                        > </select>`;
                    }
                }
            },
            {
                field: "newHotelCode",
                headerTemplate: $translate.instant('BTA_NEW_HOTEL'),
                width: "220px",
                template: function (dataItem) {
                    return `<select kendo-drop-down-list style="color: #333;width: 100%;"
                    id="hotelChange${dataItem.no}"
                    data-k-ng-model="dataItem.newHotelCode"
                    k-data-text-field="'name'"
                    k-data-value-field="'code'"
                    k-template="'<span><label>#: data.name# </label></span>'",
                    k-valueTemplate="'#: name #'",
                    k-value-primitive="'false'"
                    k-data-source="hotelNewOption${dataItem.no}"
                    k-filter="'contains'",
                    k-on-change="changeCancelCommon('newHotel', dataItem)"
                    ng-class="{'disabled': dataItem.isCancel}"> </select>`;
                }
            },
            {
                field: "newCheckInHotelDate",
                headerTemplate: $translate.instant('BTA_NEW_CHECK_IN_HOTEL_DATE'),
                width: "220px",
                format: "{0:dd/MM/yyyy}",
                template: function (dataItem) {
                    return `<input kendo-date-time-picker
                    id="newCheckInHotelDate${dataItem.no}" 
                    k-ng-model="dataItem.newCheckInHotelDate"
                    k-date-input="true"
                    k-format="'dd/MM/yyyy HH:mm'"
                    k-time-format="'HH:mm'"
                    k-on-change="fromDateCheckInOutGridChangingCancellingChanged(dataItem)"
                    style="color: #333;width: 100%;" ng-class="{'disabled': dataItem.isCancel}"/>`;
                }
            },
            {
                field: "newCheckOutHotelDate",
                headerTemplate: $translate.instant('BTA_NEW_CHECK_OUT_HOTEL_DATE'),
                width: "220px",
                format: "{0:dd/MM/yyyy}",
                template: function (dataItem) {
                    return `<input kendo-date-time-picker                    
                    id="newCheckOutHotelDate${dataItem.no}"
                    k-ng-model="dataItem.newCheckOutHotelDate"
                    k-date-input="true"
                    k-format="'dd/MM/yyyy HH:mm'"
                    k-time-format="'HH:mm'"
                    k-on-change="toDateCheckInOutGridChangingCancellingChanged(dataItem)"
                    style="color: #333;width: 100%;" ng-class="{'disabled': dataItem.isCancel}"/>`;
                }
            },
            {
                field: "newFlightNumberCode",
                headerTemplate: $translate.instant('BTA_NEW_DEPART_FLIGHT'),
                width: "280px",
                template: function (dataItem) {
                    return `<select kendo-drop-down-list style="color: #333;width: 100%;"
                        class="enable-to-edit"
                        id="newFlightNumberCode${dataItem.no}"
                        data-k-ng-model="dataItem.newFlightNumberCode"
                        k-data-text-field="'name'"
                        k-data-value-field="'code'"
                        k-template="'<span><label>#: data.name# </label></span>'",
                        k-valueTemplate="'#: name #'",
                        k-auto-bind="'true'"
                        k-value-primitive="'false'"
                        k-data-source="flightNumberOption"
                        k-filter="'contains'",
                        k-on-change="changeCancelCommon('newFlightNumber', dataItem)"
                        ng-class="{'disabled': dataItem.isCancel}"> </select>`;
                }
            },
            {
                field: "newComebackFlightNumberCode",
                headerTemplate: $translate.instant('BTA_NEW_RETURN_FLIGHT'),
                width: "280px",
                template: function (dataItem) {
                    return `<select kendo-drop-down-list style="color: #333;width: 100%;"
                        class="enable-to-edit"
                        id="newComebackFlightNumberCode${dataItem.no}"
                        data-k-ng-model="dataItem.newComebackFlightNumberCode"
                        k-data-text-field="'name'"
                        k-data-value-field="'code'"
                        k-template="'<span><label>#: data.name# </label></span>'",
                        k-valueTemplate="'#: name #'",
                        k-auto-bind="'true'"
                        k-value-primitive="'false'"
                        k-data-source="flightNumberOption"
                        k-filter="'contains'",
                        k-on-change="changeCancelCommon('newComebackFlightNumber', dataItem)"
                        ng-class="{'disabled': dataItem.isCancel}"> </select>`;
                }
            },
            {
                field: "isCancel",
                //headerTemplate: $translate.instant('BTA_IS_FOREIGNER'),
                headerTemplate: $translate.instant('BTA_IS_CANCEL'),
                width: "100px",
                editable: function (e) {
                    // check box
                    return false;
                },
                template: function (dataItem) {
                    return `<input type="checkbox" ng-model="dataItem.isCancel" name="isCancel{{dataItem.no}}"
                    id="isCancel{{dataItem.no}}" class="k-checkbox" style="width: 100%;" ng-change='changeCancel(dataItem)'/>
                    <label class="k-checkbox-label cbox" for="isCancel{{dataItem.no}}"></label>`
                }
            },
            {
                field: "reason",
                // headerTemplate: $translate.instant('BTA_DESTINATION'),
                headerTemplate: $translate.instant('COMMON_REASON'),
                width: "350px",
                template: function (dataItem) {
                    return `<input id="reason${dataItem.no}" ng-keyup="reasonKeyUp(dataItem)" name="others" class="k-input w100" type="text" style="color: #333;" ng-model="dataItem.reason"/>`;
                }
            },
            {
                headerTemplate: $translate.instant('COMMON_ACTION'),
                width: "180px",
                editable: function (e) {
                    // date
                    return false;
                },
                template: function (dataItem) {
                    if (($scope.revokeClick && !$scope.isClickReviewUserForChanging) || !$scope.isEditViewTicket) {
                        return `<a class="btn btn-sm default green-stripe" ng-class="{'display-none': !isBookingFlightWF}" ng-click="viewTicketForCurrentUser(dataItem.sapCode, dataItem.businessTripApplicationDetailId, true, isChangingAdminCheckerStep)">{{'BTA_VIEW_TICKET'|translate}}</a>`;
                    }
                    return `<a class="btn btn-sm default green-stripe" ng-class="{'display-none': !isBookingFlightWF}" ng-click="viewTicketForCurrentUser(dataItem.sapCode, dataItem.businessTripApplicationDetailId, false, isChangingAdminCheckerStep)">{{'BTA_VIEW_TICKET'|translate}}</a><a id="delete{{dataItem.no}}" class="btn btn-sm default red-stripe" ng-click="deleteRecord('btaListChangeOrCancelGrid', dataItem.no)">{{'COMMON_BUTTON_DELETE'|translate}}</a>`;
                }
            },
        ]
        var roomViewGridColumns = [
            {
                field: "no",
                headerTemplate: $translate.instant('COMMON_NO'),
                width: "50px",
                editable: function (e) {
                    return false;
                }
            },
            {
                field: "roomTypeName",
                headerTemplate: $translate.instant('BTA_ROOM_TYPE'),
                width: "100px",
                editable: function (e) {
                    return false;
                },
                tempalte: function (dataItem) {
                    return `<input name="others" ng-readonly="true" class="k-input" type="text" ng-model="dataItem.roomTypeName" style="width: 100%"/>`;
                }
            },
            {
                field: "showUsers",
                headerTemplate: $translate.instant('PEOPLE'),
                width: "300px",
                editable: function (e) {
                    return false;
                },
                tempalte: function (dataItem) {
                    return `<input name="others" ng-readonly="true" class="k-input" type="text" ng-model="dataItem.showUsers" style="width: 100%"/>`;
                }
            }
        ]
        var changeRoomViewOptions = [
            {
                field: "no",
                headerTemplate: $translate.instant('COMMON_NO'),
                width: "50px",
                editable: function (e) {
                    return false;
                }
            },
            {
                field: "roomTypeName",
                headerTemplate: "Room Type",
                width: "100px",
                editable: function (e) {
                    return false;
                },
                tempalte: function (dataItem) {
                    return `<input name="others" ng-readonly="true" class="k-input" type="text" ng-model="dataItem.roomTypeName" style="width: 100%"/>`;
                }
            },
            {
                field: "showUsers",
                headerTemplate: "Users",
                width: "300px",
                editable: function (e) {
                    return false;
                },
                tempalte: function (dataItem) {
                    return `<input name="others" ng-readonly="true" class="k-input" type="text" ng-model="dataItem.showUsers" style="width: 100%"/>`;
                }
            }
        ];
        $scope.stayHotelChange = function (dataItem) {
            if (!$.isEmptyObject(dataItem) && !dataItem.stayHotel) {
                //hotelCode checkInHotelDate checkOutHotelDate
                dataItem.hotelCode = "";
                dataItem.checkInHotelDate = null;
                dataItem.checkOutHotelDate = null;
            }
        };

        $scope.deptLineOptions = {
            dataTextField: 'name',
            dataValueField: 'id',
            autoBind: false,
            valuePrimitive: true,
            filter: "contains",
            dataSource: [],
            change: async function (e) {
                $scope.total = 0;
                let i = e.sender;
                let deptId = i.value();
                var value = this.value();
                let currentItemDeptLine = _.find($scope.deptLineList, x => { return x.deptLine.id == value });
                $scope.model.deptLineId = currentItemDeptLine.deptLine.id;
                $scope.model.deptCode = currentItemDeptLine.deptLine.code;
                $scope.model.deptName = currentItemDeptLine.deptLine.name;
                $scope.errorsTwo = {};
                if (deptId) {
                    clearSearchTextOnDropdownList('dept_line_id');
                    clearGrid('btaListDetailGrid');
                    var currentItem = _.find($scope.deptLineList, x => { return x.deptLine.id == deptId });
                    if (!currentItem.divisions.length) { // User là member của DeptLine                      
                        await $scope.getChildDivison();
                        await getUserCheckedHeadCount($scope.model.deptLineId);
                    } else {
                        var ids = _.map(currentItem.divisions, 'id');
                        await getDivisionTreeByIds(ids);
                    }
                }
            },
        };
        $scope.divisionOptions = {
            dataValueField: "id",
            dataTextField: "name",
            dataSource: [],
            autoClose: false,
            valuePrimitive: true,
            checkboxes: false,
            autoBind: true,
            filter: "contains",
            customFilterFields: ['code', 'name', 'userCheckedHeadCount'],
            filtering: filterMultiField,
            template: function (dataItem) {
                return `<span class="${dataItem.item.jobGradeGrade > 4 ? 'k-state-disabled' : ''}">${showCustomDepartmentTitle(dataItem)}</span>`;
            },
            loadOnDemand: false,
            valueTemplate: (e) => showCustomField(e, ['name']),
            select: async function (e) {
                let dropdownlist = $("#division_id").data("kendoDropDownTree");
                var dataItem = e.sender.dataItem(e.node);
                dropdownlist.close();
                let grid = $("#btaListDetailGrid").data("kendoGrid");
                if (grid.dataSource._data.length > 0) {
                    $timeout(function () {
                        $scope.dialog = $rootScope.showConfirmDelete($translate.instant('COMMON_NOTIFY'), $translate.instant('SHIFT_EXCHANGE_NOTIFY'), $translate.instant('COMMON_BUTTON_CONFIRM'));
                        $scope.dialog.bind("close", function (e) {
                            if (e.data && e.data.value) {
                                $scope.temporaryDivision = dataItem.id;
                                $scope.originalDivision = null;
                                $scope.model.deptDivisionCode = dataItem.code;
                                $scope.model.deptDivisionName = dataItem.name;
                                $scope.model.isStore = dataItem.isStore;
                                if (dataItem.jobGradeGrade > 4) {
                                    e.preventDefault();
                                } else {
                                    $scope.model.deptDivisionId = dataItem.id;
                                }
                                grid.setDataSource([]);
                            }
                            else {
                                dropdownlist.value($scope.temporaryDivision);
                            }
                        });
                    }, 0);
                }
                $scope.model.deptDivisionCode = dataItem.code;
                $scope.model.deptDivisionName = dataItem.name;
                $scope.model.isStore = dataItem.isStore;
            }
        };
        $scope.flightNumberOptions = {
            dataValueField: "code",
            dataTextField: "name",
            dataSource: [],
            autoClose: false,
            valuePrimitive: true,
            checkboxes: false,
            autoBind: true,
            filter: "contains",
            filtering: filterMultiField,
            loadOnDemand: false,
            select: async function (e) {
                var flightNumberId = '#' + e.sender.element[0].id;
                let dropdownlist = $(flightNumberId).data("kendoDropDownTree");
                dropdownlist.close();
                flightNumerValue = e.sender.dataItem(e.node);
            }
        };
        $scope.comeBackFlightOptions = {
            dataValueField: "code",
            dataTextField: "name",
            dataSource: [],
            autoClose: false,
            valuePrimitive: true,
            checkboxes: false,
            autoBind: true,
            filter: "contains",
            filtering: filterMultiField,
            loadOnDemand: false,
            select: async function (e) {
                var comeBackFlightId = '#' + e.sender.element[0].id;
                let dropdownlist = $(comeBackFlightId).data("kendoDropDownTree");
                dropdownlist.close();
                comeBackFlightValue = e.sender.dataItem(e.node);
            }
        };

        $scope.roomViewOptions = {
            dataSource: {
                data: []
            },
            sortable: false,
            pageable: false,
            columns: roomViewGridColumns
        }
        $scope.changeRoomViewOptions = {
            dataSource: {
                data: []
            },
            sortable: false,
            pageable: false,
            columns: changeRoomViewOptions
        }
        $scope.btaListDetailGridOptions = {
            dataSource: {
                data: $scope.btaListDetailGridData
            },
            sortable: false,
            pageable: false,
            columns: btaListDetailGridColumns
        }
        $scope.btaListChangeOrCancelGridOptions = {
            dataSource: {
                data: $scope.btaListDetailGridData
            },
            sortable: false,
            pageable: false,
            columns: btaListChangeOrCancelGridColumns
        }
        ngOnInit();

        $scope.btaListChangeOrCancelGridColumnsCustom = btaListChangeOrCancelGridColumns
    }

    window.bta_GetAirportValueTemplate = function (data) {
        if ($.isEmptyObject(data) || data == "") {
            return "";
        }
        return `${data.city}(${data.code}, ${data.country})`;
    }

    $scope.getChildDivison = async function () {
        if ($scope.model.deptLineId) {
            var result = await settingService.getInstance().departments.getDivisionTree({
                departmentId: $scope.model.deptLineId
            }).$promise;
            if (result.isSuccess) {
                $scope.dataTemporaryArrayDivision = _.filter(result.object.data, x => { return x.items.length || x.jobGradeGrade <= 4 });
                setDropDownTree("division_id", $scope.dataTemporaryArrayDivision);
                if (!result.object.data.length) {
                    await getUsersByDeptLine($scope.model.deptLineId, 5, '');
                }
            } else {
                Notification.error(result.messages[0]);
            }
        }
    }

    async function getUsersByDeptLine(deptId, limit, searchText, page = 1) {
        let result = await settingService.getInstance().users.getChildUsers({ departmentId: deptId, limit: limit, page: page, searchText: searchText }).$promise;
        if (result.isSuccess) {
            $scope.employeesDataSource = result.object.data.map(function (item) {
                return { ...item, showtextCode: item.sapCode }
            });
            $scope.total = result.object.count;
        }
    }

    $scope.changeCancel = async function (dataItem, isKeep = false) {
        var currentBTAListDetail = $("#btaListDetailGrid").data("kendoGrid").dataSource._data;
        let isCancel = dataItem.isCancel;
        if (isCancel) {
            var result = currentBTAListDetail.find(x => x.id == dataItem.businessTripApplicationDetailId);
            if (result) {
                let dataChanging = getDataGrid('#btaListChangeOrCancelGrid');
                if (dataChanging.length) {
                    var itemChanging = dataChanging.find(x => x.id == dataItem.id);
                    if (itemChanging) {
                        if (!isKeep) {
                            itemChanging.fromDate = result.fromDate;
                            itemChanging.toDate = result.toDate;
                            itemChanging.destinationCode = result.arrivalCode;
                            itemChanging.destinationName = result.arrivalName;
                            await resetDataChangeBTA(itemChanging);
                            itemChanging.newCheckInHotelDate = result.checkInHotelDate;
                            itemChanging.newCheckOutHotelDate = result.checkOutHotelDate;
                            itemChanging.newFlightNumberCode = result.flightNumberCode;
                            itemChanging.newComebackFlightNumberCode = result.comebackFlightNumberCode;
                            itemChanging.newComebackFlightNumberName = result.comebackFlightNumberName;
                            itemChanging.newHotelCode = result.hotelCode;
                            itemChanging.newHotelName = result.hotelName;
                        }
                        if ($scope.isEditViewTicket && $scope.isBookingFlightWF) {
                            itemChanging["isCancelInBoundFlight"] = true;
                            itemChanging["isCancelOutBoundFlight"] = true;
                        }
                    }
                }
                setDataSourceForGrid('#btaListChangeOrCancelGrid', dataChanging);
                if (!$scope.revokeClick) {
                    setGridChangeCancelValue();
                }

                // dataItem.fromDate = result.fromDate;
                // dataItem.toDate = result.toDate;
                // dataItem.destinationCode = result.arrivalCode;
                // dataItem.destinationName = result.arrivalName;
                // await resetDataChangeBTA(dataItem);
                // dataItem.newCheckInHotelDate = result.checkInHotelDate;
                // dataItem.newCheckOutHotelDate = result.checkOutHotelDate;
                // dataItem.newFlightNumberCode = result.flightNumberCode;
                // dataItem.newComebackFlightNumberCode = result.comebackFlightNumberCode;
                // dataItem.newComebackFlightNumberName = result.comebackFlightNumberName;
            }
            if (!$scope.isBookingFlightWF) {
                let checkInId = '#newCheckInHotelDate' + dataItem.no;
                let checkOutId = '#newCheckOutHotelDate' + dataItem.no;
                let id = "#toDateCancel" + dataItem.no;
                $(id).data('kendoDateTimePicker').min(new Date(dataItem.fromDate));
                $(checkInId).data('kendoDateTimePicker').min(new Date(dataItem.fromDate));
                $(checkInId).data('kendoDateTimePicker').max(new Date(dataItem.toDate));
                $(checkOutId).data('kendoDateTimePicker').min(new Date(dataItem.fromDate));
                $(checkOutId).data('kendoDateTimePicker').max(new Date(dataItem.toDate));
            }
        }
        else {
            let reasonId = '#reason' + dataItem.no;
            $(reasonId).removeAttr("disabled");
            if ($scope.isEditViewTicket && $scope.isBookingFlightWF) {
                let dataChanging = getDataGrid('#btaListChangeOrCancelGrid');
                if (dataChanging.length) {
                    var itemChanging = dataChanging.find(x => x.id == dataItem.id);
                    if (itemChanging) {
                        if ($scope.isEditViewTicket && $scope.isBookingFlightWF) {
                            itemChanging["isCancelInBoundFlight"] = false;
                            itemChanging["isCancelOutBoundFlight"] = false;
                        }
                    }
                }
                setDataSourceForGrid('#btaListChangeOrCancelGrid', dataChanging);
                if (!$scope.revokeClick) {
                    setGridChangeCancelValue();
                }
            }
        }
        let dataChanging = getDataGrid('#btaListChangeOrCancelGrid');
        setTimeout(function () {
            enableGridChangingCancelling(dataChanging);
        }, 200);
        // let id = "#destination" + dataItem.no;
        // var dropdownlist = $(id).data("kendoDropDownList");
        // if (dropdownlist) {
        //     if (dataItem.isCancel) {
        //         dropdownlist.readonly(true);
        //     }
        //     else {
        //         dropdownlist.readonly(false);
        //     }
        // }
    }

    $scope.warningWeekBooking = [];
    $scope.fromDateGridChanged = function (typeId, dataItem, isAction = false) {
        let id = '';
        let checkInId = '';
        let checkOutId = '';
        switch (typeId) {
            case '#toDate':
                id = "#toDate" + dataItem.no;
                checkInId = '#checkInHotelDate' + dataItem.no;
                checkOutId = '#checkOutHotelDate' + dataItem.no;

                var newDate = new Date(dataItem.fromDate);
                newDate.setDate(newDate.getDate() - 10);
                var currentDate = new Date().getTime();
                if (newDate.getTime() > currentDate) {
                    $scope.warningWeekBooking = $scope.warningWeekBooking.filter(function (item) {
                        return item.id !== dataItem.no
                    })
                } else {
                    $scope.warningWeekBooking.push(
                        {
                            id: dataItem.no,
                            message: $translate.instant('BTA_FROM_DATE_REQUIRED') + ' ' + dataItem.no + ': ' + $translate.instant('BTA_WARNING_MESSAGE')
                        });
                }

                break;
            case '#toDateCancel':
                id = "#toDateCancel" + dataItem.no;
                break;
            default:
                break;
        }

        if (id) {
            if (dataItem.fromDate) {
                $(id).data('kendoDateTimePicker').min(new Date(dataItem.fromDate));
                var resultDayToDate = dateFns.getDayOfYear(dataItem.toDate);
                var resultDayFromDate = dateFns.getDayOfYear(dataItem.fromDate);
                var resultHourToDate = dateFns.getHours(dataItem.toDate);
                var resultHourFromDate = dateFns.getHours(dataItem.fromDate);
                if (dataItem.toDate && resultDayToDate < resultDayFromDate) {
                    dataItem.toDate = dataItem.fromDate;
                }
                else if (dataItem.toDate && resultDayToDate == resultDayFromDate && resultHourToDate <= resultHourFromDate) {
                    dataItem.toDate = dataItem.fromDate;
                }
            }
            if (checkInId || checkOutId) {
                $(checkInId).data('kendoDateTimePicker').min(new Date(dataItem.fromDate));
                $(checkInId).data('kendoDateTimePicker').max(new Date(dataItem.toDate));
                $(checkOutId).data('kendoDateTimePicker').min(new Date(dataItem.fromDate));
                $(checkOutId).data('kendoDateTimePicker').max(new Date(dataItem.toDate));
                if (!isAction) {
                    dataItem.checkInHotelDate = null;
                    dataItem.checkOutHotelDate = null;
                }
            }
        }
    };

    $scope.toDateGridChange = function (dataItem) {
        let checkInId = '#checkInHotelDate' + dataItem.no;
        let checkOutId = '#checkOutHotelDate' + dataItem.no;
        if (checkInId || checkOutId) {
            $(checkInId).data('kendoDateTimePicker').min(new Date(dataItem.fromDate));
            $(checkInId).data('kendoDateTimePicker').max(new Date(dataItem.toDate));
            $(checkOutId).data('kendoDateTimePicker').min(new Date(dataItem.fromDate));
            $(checkOutId).data('kendoDateTimePicker').max(new Date(dataItem.toDate));
            dataItem.checkInHotelDate = null;
            dataItem.checkOutHotelDate = null;
        }

        if (dataItem.toDate < dataItem.fromDate) {
            dataItem.toDate = dataItem.fromDate;
        }
    }

    $scope.fromDateGridChangingCancellingChanged = function (dataItem, isAction = false) {
        let id = "#toDateCancel" + dataItem.no;
        let checkInId = '#newCheckInHotelDate' + dataItem.no;
        let checkOutId = '#newCheckOutHotelDate' + dataItem.no;
        if (dataItem.fromDate) {
            $(id).data('kendoDateTimePicker').min(new Date(dataItem.fromDate));
            var resultDayToDate = dateFns.getDayOfYear(dataItem.toDate);
            var resultDayFromDate = dateFns.getDayOfYear(dataItem.fromDate);
            var resultHourToDate = dateFns.getHours(dataItem.toDate);
            var resultHourFromDate = dateFns.getHours(dataItem.fromDate);
            if (dataItem.toDate && resultDayToDate < resultDayFromDate) {
                dataItem.toDate = dataItem.fromDate;
            }
            else if (dataItem.toDate && resultDayToDate == resultDayFromDate && resultHourToDate <= resultHourFromDate) {
                dataItem.toDate = dataItem.fromDate;
            }
        }
        if (checkInId || checkOutId) {
            $(checkInId).data('kendoDateTimePicker').min(new Date(dataItem.fromDate));
            $(checkInId).data('kendoDateTimePicker').max(new Date(dataItem.toDate));
            $(checkOutId).data('kendoDateTimePicker').min(new Date(dataItem.fromDate));
            $(checkOutId).data('kendoDateTimePicker').max(new Date(dataItem.toDate));
            if (!isAction) {
                dataItem.newCheckInHotelDate = null;
                dataItem.newCheckOutHotelDate = null;
            }
        }
    };

    $scope.toDateGridChangingCancellingChanged = function (dataItem) {
        let checkInId = '#newCheckInHotelDate' + dataItem.no;
        let checkOutId = '#newCheckOutHotelDate' + dataItem.no;
        if (checkInId || checkOutId) {
            $(checkInId).data('kendoDateTimePicker').min(new Date(dataItem.fromDate));
            $(checkInId).data('kendoDateTimePicker').max(new Date(dataItem.toDate));
            $(checkOutId).data('kendoDateTimePicker').min(new Date(dataItem.fromDate));
            $(checkOutId).data('kendoDateTimePicker').max(new Date(dataItem.toDate));
            dataItem.newCheckInHotelDate = null;
            dataItem.newCheckOutHotelDate = null;
        }

        if (dataItem.toDate < dataItem.fromDate) {
            dataItem.toDate = dataItem.fromDate;
        }
    }

    $scope.fromDateCheckInOutGridChangingCancellingChanged = function (dataItem) {
        let id = "#newCheckOutHotelDate" + dataItem.no;
        if (dataItem.newCheckInHotelDate) {
            $(id).data('kendoDateTimePicker').min(new Date(dataItem.newCheckInHotelDate));
            $(id).data('kendoDateTimePicker').max(new Date(dataItem.toDate));
            var resultDayCheckIn = dateFns.getDayOfYear(dataItem.newCheckInHotelDate);
            var resultDayCheckOut = dateFns.getDayOfYear(dataItem.newCheckOutHotelDate);
            var resultHourCheckIn = dateFns.getHours(dataItem.newCheckInHotelDate);
            var resultHourCheckOut = dateFns.getHours(dataItem.newCheckOutHotelDate);
            if (dataItem.newCheckOutHotelDate && resultDayCheckOut < resultDayCheckIn) {
                dataItem.newCheckOutHotelDate = dataItem.newCheckInHotelDate;
            }
            else if (dataItem.newCheckOutHotelDate && resultDayCheckIn == resultDayCheckOut && resultHourCheckOut <= resultHourCheckIn) {
                dataItem.newCheckOutHotelDate = dataItem.newCheckInHotelDate;
            }
        }

        if (dataItem.newCheckInHotelDate == null) {
            dataItem.newCheckInHotelDate = dataItem.fromDate;
        }
    };

    $scope.fromDateCheckInOutChanged = function (dataItem) {
        let id = "#checkOutHotelDate" + dataItem.no;
        if (dataItem.checkInHotelDate) {
            $(id).data('kendoDateTimePicker').min(new Date(dataItem.checkInHotelDate));
            $(id).data('kendoDateTimePicker').max(new Date(dataItem.toDate));
            var resultDayCheckIn = dateFns.getDayOfYear(dataItem.checkInHotelDate);
            var resultDayCheckOut = dateFns.getDayOfYear(dataItem.checkOutHotelDate);
            var resultHourCheckIn = dateFns.getHours(dataItem.checkInHotelDate);
            var resultHourCheckOut = dateFns.getHours(dataItem.checkOutHotelDate);
            if (dataItem.checkOutHotelDate && resultDayCheckOut < resultDayCheckIn) {
                dataItem.checkOutHotelDate = dataItem.checkInHotelDate;
            }
            else if (dataItem.checkOutHotelDate && resultDayCheckIn == resultDayCheckOut && resultHourCheckOut <= resultHourCheckIn) {
                dataItem.checkOutHotelDate = dataItem.checkInHotelDate;
            }
        }
    };

    $scope.allowDisabledBTAType = function () {
        let btaDetailItems = $("#btaListDetailGrid").data("kendoGrid").dataSource._data;
        let btaItems = $scope.btaListDetailGridOptions.dataSource.data;
        return (!$.isEmptyObject(btaDetailItems) && btaDetailItems.length > 0) || (!$.isEmptyObject(btaItems) && btaItems.length > 0);
    }

    $scope.deleteRecord = function (typeGrid, no) {
        $scope.no = no;
        switch (typeGrid) {
            case 'btaListDetailGrid':
                $scope.dialog = $rootScope.showConfirmDelete($translate.instant('COMMON_BUTTON_DELETE'), $translate.instant('COMMON_DELETE_VALIDATE'), $translate.instant('COMMON_BUTTON_CONFIRM'));
                $scope.dialog.bind("close", function (e) {
                    if (e.data && e.data.value) {
                        let currentDataOnGrid = $("#btaListDetailGrid").data("kendoGrid").dataSource._data;
                        var index = _.findIndex(currentDataOnGrid, x => {
                            if (x.id) {
                                return x.id == currentDataOnGrid[$scope.no - 1].id;
                            } else if (x.uid) {
                                return x.uid == currentDataOnGrid[$scope.no - 1].uid;
                            }
                        });
                        currentDataOnGrid.splice(index, 1);
                        let count = 1;
                        currentDataOnGrid.forEach(item => {
                            item.no = count;
                            count++;
                        })
                        setDataSourceForGrid('#btaListDetailGrid', currentDataOnGrid);
                        $scope.btaListDetailGridOptions.dataSource.data = currentDataOnGrid;
                        //get hotel 
                        currentDataOnGrid.forEach(item => {
                            $scope.changeArrival(item, true);
                            if (!item.hotelCode) {
                                disableCheckInCheckOut(item.no);
                            }
                        });
                        //set giới hạn lại time cho 
                        $timeout(function () {
                            $scope.warningWeekBooking = [];
                            currentDataOnGrid.forEach(x => {
                                $scope.fromDateGridChanged('#toDate', x, true);
                                //flightNumber
                                let flightNumberId = 'flightNumber' + x.no;
                                setDropDownTree(flightNumberId, $scope.flightNumberOption);
                                var flightNumberOption = $("#" + flightNumberId).data("kendoDropDownTree");
                                if (flightNumberOption) {
                                    flightNumberOption.value(x.flightNumberCode);
                                }
                                //comeBackFlight
                                let comeBackFlightId = 'comeBackFlight' + x.no;
                                setDropDownTree(comeBackFlightId, $scope.flightNumberOption);
                                var comeBackFlightOption = $("#" + comeBackFlightId).data("kendoDropDownTree");
                                if (comeBackFlightOption) {
                                    comeBackFlightOption.value(x.comebackFlightNumberCode);
                                }
                            });

                            if ($scope.warningWeekBooking.length > 0) {
                                $scope.bookingDateTripWeekes = true;
                            }
                            else {
                                $scope.bookingDateTripWeekes = false;
                                $scope.warningWeekBooking = [];
                            }

                        }, 0);
                        Notification.success($translate.instant('COMMON_DELETE_SUCCESS'));
                    }
                });
                break;
            case 'btaListChangeOrCancelGrid':
                $scope.dialog = $rootScope.showConfirmDelete($translate.instant('COMMON_BUTTON_DELETE'), $translate.instant('COMMON_DELETE_VALIDATE'), $translate.instant('COMMON_BUTTON_CONFIRM'));
                $scope.dialog.bind("close", function (e) {
                    if (e.data && e.data.value) {
                        let getGridId = '#btaListChangeOrCancelGrid';
                        let currentDataOnGrid = $(`${getGridId}`).data("kendoGrid").dataSource._data;
                        _.remove(currentDataOnGrid, function (item) {
                            return item.no === $scope.no;
                        });
                        currentDataOnGrid.forEach((item, index) => {
                            item.no = index + 1;
                            $scope.changeDestination(item, true);
                        });
                        $timeout(function () {
                            setDataSourceForGrid(getGridId, currentDataOnGrid);
                            Notification.success($translate.instant('COMMON_DELETE_SUCCESS'));
                        }, 0);
                        setGridChangeCancelValue();
                    }
                });
                break;
            case 'roomGrid':
                $scope.dialog = $rootScope.showConfirmDelete($translate.instant('COMMON_BUTTON_DELETE'), $translate.instant('COMMON_DELETE_VALIDATE'), $translate.instant('COMMON_BUTTON_CONFIRM'));
                $scope.dialog.bind("close", function (e) {
                    if (e.data && e.data.value) {
                        let id = "#peopleOption" + no;
                        var multiselect = $(id).data("kendoMultiSelect");
                        if (multiselect) {
                            if (multiselect.dataItems().length > 0) {
                                let arrayTemporary = [];
                                peoplesTotal.forEach(item => {
                                    let result = multiselect.dataItems().find(x => x.businessTripApplicationDetailId == item);
                                    if (!result) {
                                        arrayTemporary.push(item);
                                    }
                                });
                                peoplesTotal = arrayTemporary;
                            }
                        }
                        //
                        let currentDataOnGrid = $("#roomGrid").data("kendoGrid").dataSource._data;
                        let grid = $("#roomGrid").data("kendoGrid");
                        var index = _.findIndex(currentDataOnGrid, x => {
                            if (x.id) {
                                return x.id == currentDataOnGrid[$scope.no - 1].id;
                            } else if (x.uid) {
                                return x.uid == currentDataOnGrid[$scope.no - 1].uid;
                            }
                        });
                        currentDataOnGrid.splice(index, 1);
                        let count = 1;
                        currentDataOnGrid.forEach(item => {
                            item.no = count;
                            count++;
                        });
                        grid.refresh();
                        Notification.success($translate.instant('COMMON_DELETE_SUCCESS'));
                    }
                });
                break;
            default:
                break;
        }


    }

    $scope.changeSapCode = function (dataItem, source) {
        let currentItem = _.find(source, x => { return x.sapCode == dataItem.sapCode });
        if (currentItem) {
            dataItem.fullName = currentItem.fullName;
            dataItem.fromDate = currentItem.fromDate;
            dataItem.toDate = currentItem.toDate;
            dataItem.destinationCode = currentItem.arrivalCode;
            dataItem.destinationName = currentItem.arrivalName;
            dataItem.isCancel = false;
            dataItem.reason = '';
        }
        return dataItem;
    }

    $scope.changeForeigner = async function (dataItem) {
        let hotelId = '#hotel' + dataItem.no
        dataItem.hotelCode = '';
        dataItem.hotelName = '';
        dataItem.checkInHotelDate = '';
        dataItem.checkOutHotelDate = '';
        disableCheckInCheckOut(dataItem.no);
        await getHotels(dataItem.isForeigner, dataItem.arrivalCode, hotelId);
    }

    $scope.changeArrival = async function (dataItem, isEdit = false) {
        if ($scope.model.type == 1) {
            var result = _.find($scope.arrivalOption, x => { return x.code == dataItem.arrivalCode });
            if (result) {
                dataItem.arrivalName = result.name;
            }
        }
        else {
            var arrivalSelect = $("#arrivalSelect" + dataItem.no).data("kendoDropDownList");
            if (!$.isEmptyObject(arrivalSelect)) {
                var arrivalInfo = arrivalSelect.dataItem();
                if (!$.isEmptyObject(arrivalInfo)) {
                    dataItem.arrivalName = arrivalInfo.city + " (" + arrivalInfo.code + ")";
                    dataItem.arrivalInfo = JSON.stringify(arrivalInfo);
                }
            }
        }

        if ($scope.model.type === 1) {
            var result = _.find($scope.arrivalOption, x => { return x.code === dataItem.arrivalCode });
            if (result) {
                dataItem.arrivalName = result.name;
            }
        }

        let hotelId = '#hotel' + dataItem.no
        if (!isEdit) {
            dataItem.hotelCode = '';
            dataItem.hotelName = '';
            dataItem.checkInHotelDate = '';
            dataItem.checkOutHotelDate = '';
            disableCheckInCheckOut(dataItem.no);
        }
        await getHotels(dataItem.isForeigner, dataItem.arrivalCode, hotelId, dataItem.hotelCode);
    }

    $scope.changeDestination = async function (dataItem, isEdit = false) {
        var result = _.find($scope.arrivalOption, x => { return x.code == dataItem.destinationCode });
        if (result) {
            dataItem.destinationName = result.name;
        }
        let hotelId = '#hotelChange' + dataItem.no
        if (!isEdit) {
            dataItem.newHotelCode = '';
            dataItem.newHotelName = '';
            dataItem.newCheckInHotelDate = '';
            dataItem.newCheckOutHotelDate = '';
            disableNewCheckInCheckOut(dataItem.no);
        }
        await getHotels(dataItem.isForeigner, dataItem.destinationCode, hotelId, dataItem.newHotelCode, false);
    }

    async function resetDataChangeBTA(dataItem) {
        var result = _.find($scope.model.businessTripDetails, x => { return x.id == dataItem.businessTripApplicationDetailId });
        if (result) {
            dataItem.newHotelCode = result.hotelCode;
            dataItem.newHotelName = result.hotelName;
        }
        let hotelId = '#hotelChange' + dataItem.no;
        await getHotels(dataItem.isForeigner, dataItem.destinationCode, hotelId, dataItem.newHotelCode, false);
    }

    flightNumerValue = '';
    comeBackFlightValue = '';
    $scope.changeCommon = async function (type, dataItem) {
        switch (type) {
            case 'departure':
                if ($scope.model.type == 1) {
                    var result = _.find($scope.departureOption, x => { return x.code == dataItem.departureCode });
                    if (result) {
                        dataItem.departureName = result.name;
                    }
                }
                else {
                    var departureSelect = $("#departureSelect" + dataItem.no).data("kendoDropDownList");
                    if (!$.isEmptyObject(departureSelect)) {
                        var departureInfo = departureSelect.dataItem();
                        if (!$.isEmptyObject(departureInfo)) {
                            dataItem.departureName = departureInfo.city + " (" + departureInfo.code + ")";
                            dataItem.departureInfo = JSON.stringify(departureInfo);
                        }
                    }
                }
                break;
            case 'hotel':
                let hotelId = '#hotel' + dataItem.no;
                var dropdownlist = $(hotelId).data("kendoDropDownList");
                var result = _.find(dropdownlist.dataSource._data, x => { return x.code == dataItem.hotelCode });
                if (result) {
                    dataItem.hotelName = result.name;
                }
                //enable check in/ out
                let checkInId = '#checkInHotelDate' + dataItem.no;
                let checkOutId = '#checkOutHotelDate' + dataItem.no;
                var checkIn = $(checkInId).data("kendoDateTimePicker");
                var checkOut = $(checkOutId).data("kendoDateTimePicker");
                checkIn.readonly(false);
                checkOut.readonly(false);
                $(checkInId).data('kendoDateTimePicker').min(new Date(dataItem.fromDate));
                $(checkInId).data('kendoDateTimePicker').max(new Date(dataItem.toDate));
                $(checkOutId).data('kendoDateTimePicker').min(new Date(dataItem.fromDate));
                $(checkOutId).data('kendoDateTimePicker').max(new Date(dataItem.toDate));
                break;
            case 'flightNumber':
                if (flightNumerValue) {
                    var result = _.find($scope.flightNumberOption, x => { return x.code == flightNumerValue.code });
                    if (result) {
                        dataItem.flightNumberCode = result.code;
                        dataItem.flightNumberName = result.name;
                        dataItem.airlineCode = result.airlineCode;
                        dataItem.airlineName = result.airlineName;
                    }
                    flightNumerValue = '';
                }
                else {
                    dataItem.flightNumberCode = '';
                    dataItem.flightNumberName = '';
                    dataItem.airlineCode = '';
                    dataItem.airlineName = '';
                }
                break;
            case 'comebackFlightNumber':
                if (comeBackFlightValue) {
                    var result = _.find($scope.flightNumberOption, x => { return x.code == comeBackFlightValue.code });
                    if (result) {
                        dataItem.comebackFlightNumberCode = result.code;
                        dataItem.comebackFlightNumberName = result.name;
                        dataItem.comebackAirlineCode = result.airlineCode;
                        dataItem.comebackAirlineName = result.airlineName;
                    }
                }
                else {
                    dataItem.comebackFlightNumberCode = '';
                    dataItem.comebackFlightNumberName = '';
                    dataItem.comebackAirlineCode = '';
                    dataItem.comebackAirlineName = '';
                }
                break;
            case 'roomType':
                var result = _.find($scope.roomTypes, x => { return x.id == dataItem.roomTypeId });
                if (result) {
                    dataItem.roomTypeCode = result.code;
                    dataItem.roomTypeName = result.name;
                    dataItem.quota = result.quota;
                }
                break;
            default:
                break;
        }
    }
    $scope.changeCancelCommon = async function (type, dataItem) {
        switch (type) {
            case 'newHotel':
                let hotelIds = '#hotelChange' + dataItem.no;
                var dropdownlist = $(hotelIds).data("kendoDropDownList");
                var result = _.find(dropdownlist.dataSource._data, x => { return x.code == dataItem.newHotelCode });
                if (result) {
                    dataItem.newHotelName = result.name;
                }
                let checkInId = '#newCheckInHotelDate' + dataItem.no;
                let checkOutId = '#newCheckOutHotelDate' + dataItem.no;
                var checkIn = $(checkInId).data("kendoDateTimePicker");
                var checkOut = $(checkOutId).data("kendoDateTimePicker");
                checkIn.readonly(false);
                checkOut.readonly(false);
                break;
            case 'newFlightNumber':
                var result = _.find($scope.flightNumberOption, x => { return x.code == dataItem.newFlightNumberCode });
                if (result) {
                    dataItem.newFlightNumberName = result.name;
                    dataItem.NewFlightNumberCode = result.code;
                    dataItem.newAirlineCode = result.airlineCode;
                    dataItem.newAirlineName = result.airlineName;

                }
                break;
            case 'newComebackFlightNumber':
                var result = _.find($scope.flightNumberOption, x => { return x.code == dataItem.newComebackFlightNumberCode });
                if (result) {
                    dataItem.newComebackFlightNumberName = result.name;
                    dataItem.newComebackFlightNumberCode = result.code;
                    dataItem.newComebackAirlineCode = result.airlineCode;
                    dataItem.newComebackAirlineName = result.airlineName;
                }
                break;
        }
    }
    //get dept/ Line
    //$scope.model.deptLineId = '';
    async function getDepartmentByUserId(userId, deptId) {
        $scope.deptLineList = [];
        if (!$stateParams.id) {
            $scope.model.deptLineId = deptId;
        }
        let res = await settingService.getInstance().departments.getDepartmentUpToG4ByUserId({ id: userId }, null).$promise;
        if (res && res.isSuccess) {
            $scope.deptLineList = res.object.data;
            var deptLines = _.map($scope.deptLineList, x => { return x['deptLine'] });
            var dropdownlist = $("#dept_line_id").data("kendoDropDownList");
            if (dropdownlist) {
                dropdownlist.setDataSource(deptLines);
            }
            if ($scope.model.deptLineId) {
                if (dropdownlist) {
                    dropdownlist.value(deptId);
                    let currentItemDeptLine = _.find($scope.deptLineList, x => { return x.deptLine.id == deptId });
                    //$scope.model.deptLineId = currentItemDeptLine.deptLine.id;
                    $scope.model.deptCode = currentItemDeptLine.deptLine.code;
                    $scope.model.deptName = currentItemDeptLine.deptLine.name;
                    $scope.model.isStore = currentItemDeptLine.deptLine.isStore;
                }
            }
            //if(!$stateParams.id) {
            await getDivisionsByDeptLine(deptId);
            //$scope.model.deptDivisionId
            //}
        }
    }

    async function getDivisionsByDeptLine(deptId, divisionId = '') {
        var currentItem = _.find($scope.deptLineList, x => { return x.deptLine.id == deptId });
        var dropdownlist = $("#division_id").data("kendoDropDownTree");
        if (dropdownlist) {
            if (currentItem && currentItem.divisions && currentItem.divisions.length) {
                var ids = _.map(currentItem.divisions, 'id');
                await getDivisionTreeByIds(ids);
            } else {
                await getDivisionTreeByIds([deptId]);
            }
            if (divisionId) {
                dropdownlist.value(divisionId);
                $scope.model.deptDivisionId = divisionId;
            }
        }
    }

    //get division theo dept/line
    $scope.deptDivisions = [];
    //$scope.model.deptDivisionId = '';
    async function getDivisionTreeByIds(ids) {
        arg = {
            predicate: "",
            predicateParameters: [],
            page: 1,
            limit: appSetting.pageSizeDefault,
            order: ""
        }
        for (let i = 0; i < ids.length; i++) {
            arg.predicate = arg.predicate ? arg.predicate + `||id = @${i}` : `id = @${i}`;
            arg.predicateParameters.push(ids[i]);
        }
        result = await settingService.getInstance().departments.getDepartmentByFilter(arg).$promise;
        if (result.isSuccess) {
            $scope.deptDivisions = result.object.data;
            $scope.dataTemporaryArrayDivision = _.filter(result.object.data, x => { return x.items.length || x.jobGradeGrade <= 4 });
            setDropDownTree('division_id', $scope.dataTemporaryArrayDivision)
            //if (_.findIndex(ids, x => { return x == $rootScope.currentUser.divisionId }) > -1) {
            //$scope.model.deptDivisionId = $rootScope.currentUser.divisionId;
            //}
        }
    }

    businessTripLocations = [];
    async function getBusinessTripLocations() {
        let currentQuery = {
            predicate: "",
            predicateParameters: [],
            order: "Name asc",
            limit: 10000,
            page: 1
        };

        var result = await settingService.getInstance().businessTripLocation.getListBusinessTripLocation(currentQuery).$promise;
        if (result.isSuccess) {
            businessTripLocations = result.object.data;
            $scope.departureOption = businessTripLocations;
            $scope.arrivalOption = businessTripLocations;
        }
    }
    function getPredicateParameters(isForeigner, arrivalCode, isAll) {
        let predicate = '';
        let predicateParameters = [];
        if (isAll) {
            predicate = 'BusinessTripLocation.Code=@0';
            predicateParameters = [arrivalCode]
        } else {
            predicate = "(IsForeigner=@0 || IsForeigner=@1)and BusinessTripLocation.Code=@2";
            predicateParameters = [isForeigner, 'both', arrivalCode];
        }
        let currentQuery = {
            predicate: predicate,
            predicateParameters: predicateParameters,
            order: "Name asc",
            limit: 10000,
            page: 1
        };
        return currentQuery;
    }
    async function getHotels(isForeigner, arrivalCode, hotelId, hotelValue, isAll = false) {
        const currentQuery = getPredicateParameters(isForeigner, arrivalCode, isAll);
        var result = await settingService.getInstance().hotel.getListHotels(currentQuery).$promise;
        if (result.isSuccess) {
            $scope.hotels = result.object.data;
            if (!$.isEmptyObject($scope.hotels) && $scope.hotels.length > 0) {
                $scope.hotels = $.merge([{
                    "id": "",
                    "code": "",
                    "name": "",
                    "address": "",
                    "telephone": "",
                    "isForeigner": 0,
                    "businessTripLocationId": "",
                    "businessTripLocationName": "",
                    "businessTripLocationCode": ""
                }], $scope.hotels)

                result.object.data = $scope.hotels;
            }

            let setHotelDataSource = function (hotelId, dataSource) {
                var hotelControl = $(hotelId);
                if (hotelControl.length > 0) {
                    var dropdownlist = $(hotelId).data("kendoDropDownList");
                    if (dropdownlist) {
                        dropdownlist.setDataSource(dataSource);
                        if (hotelValue) {
                            dropdownlist.value(hotelValue)
                        }
                    }
                }
                else {
                    $timeout(function () {
                        setHotelDataSource(hotelId, dataSource);
                    }, 200);
                }
            }

            setHotelDataSource(hotelId, result.object.data);
        }



    }
    async function getAllHotels() {
        let currentQuery = {
            predicate: "",
            predicateParameters: [],
            order: "Name asc",
            limit: 10000,
            page: 1
        };
        var result = await settingService.getInstance().hotel.getListHotels(currentQuery).$promise;
        if (result.isSuccess) {
            $scope.allHotels = result.object.data;

        }
    }
    flightNumbers = [];
    async function getFlightNumbers(isAdmin = false) {
        let predicate = '';
        let predicateParameters = [];
        if (!isAdmin) {
            predicate = 'IsPeakTime=@0';
            predicateParameters = [isAdmin];
        }
        let currentQuery = {
            predicate: predicate,
            predicateParameters: predicateParameters,
            //order: "Modified desc",
            order: "airline.Name, departureTime asc",
            limit: 10000,
            page: 1
        };
        var result = await settingService.getInstance().flightNumber.getFlightNumbers(currentQuery).$promise;
        if (result.isSuccess) {
            hotels = result.object.data;
            $scope.flightNumberOption = hotels;
        }
    }

    $scope.model = {};

    $scope.save = async function (form, perm) {
        let dataGridBTADetail = getDataGrid('#btaListDetailGrid');
        let gridBTAChangeOrCancel = $("#btaListChangeOrCancelGrid").data("kendoGrid");
        $scope.errors = [];
        $scope.errors = validationBTADetail(); // validate trên cùng 1 phiếu
        if (!$scope.errors.length) {
            var resValidate = await validateFromToDateBusinessDetail(dataGridBTADetail); // validate trên phiếu khác
            $scope.errors = translateValidate(resValidate);
            if (!$scope.errors.length) {
                let maxDepartment = getMaxGradeFromBTADetails(dataGridBTADetail);
                if (maxDepartment) {
                    $scope.model.maxGrade = maxDepartment.maxGrade;
                    $scope.model.maxDepartmentId = maxDepartment.maxDepartmentId;
                }
                dataGridBTADetail.forEach(item => {
                    item.fromDate = new Date(item.fromDate);
                    item.toDate = new Date(item.toDate);
                });
                $scope.model.businessTripDetails = JSON.stringify(dataGridBTADetail);
                if (gridBTAChangeOrCancel && gridBTAChangeOrCancel.dataSource && gridBTAChangeOrCancel.dataSource._data && gridBTAChangeOrCancel.dataSource._data.length) {
                    gridBTAChangeOrCancel.dataSource._data.forEach(function (item) {
                        if (item.isCancel) {
                            item["reasonForInBoundFlight"] = item.reason;
                            item["reasonForOutBoundFlight"] = item.reason;
                        }
                    });
                    $scope.model.changeCancelBusinessTripDetails = JSON.stringify(gridBTAChangeOrCancel.dataSource._data.toJSON());
                }
                //check stay hotel
                $scope.model.stayHotel = isUsingHotel();
                //detail
                if ($scope.attachmentDetails.length || $scope.removeFileDetails.length) {
                    let attachmentFilesJson = await mergeAttachment();
                    $scope.model.documentDetails = attachmentFilesJson;
                }
                //change
                if ($scope.attachmentChanges.length || $scope.removeFileChanges.length) {
                    let attachmentFilesJson = await mergeAttachmentChange();
                    $scope.model.documentChanges = attachmentFilesJson;
                }
                var res = await btaService.getInstance().bussinessTripApps.save($scope.model).$promise;
                if (res.isSuccess) {
                    $state.go($state.current.name, { id: res.object.id, referenceValue: res.object.referenceNumber }, { reload: true });
                    Notification.success($translate.instant('COMMON_SAVE_SUCCESS'));
                } else {
                    Notification.error("Error System");
                }
                return res;
            }
        }
    }

    $scope.changeRoundTrip = function () {
        if ($scope.model.dataRoundTrip && $scope.model.dataRoundTrip != "") {
            $scope.model.isRoundTrip = true;
            if ($scope.model.dataRoundTrip === "1") {
                $scope.model.isRoundTrip = false;
            }
        }
        else {
            $scope.model.isRoundTrip = "";
        }
    }

    $scope.saveTicket = async function (dialogClose) {
        let passengerInfos = $scope.prepareTripGroupBeforeSave();

        //Check overbudget need add comments
        let overBudgetHasNoComment = passengerInfos.filter(x => x.isOverBudget && ($.isEmptyObject(x.comments) || x.comments.trim().length == 0));
        if (overBudgetHasNoComment.length > 0) {
            //Show error message
            let messageArray = overBudgetHasNoComment.map((cItem, cIndex) => `${$translate.instant('BTA_TRIP_GROUP')} ${cItem.tripGroup} - ${cItem.sapCode} - ${cItem.fullName} - ${$translate.instant('BTA_TICKET_OVER_BUDGET_HAS_NO_COMMENTS')}`);
            $scope.tripGroupPopup.errorMessages = messageArray;
            $scope.$apply();
            return false;
        }
        else {
            var res = await btaService.getInstance().bussinessTripApps.savePassengerInfo(passengerInfos).$promise;
            if (res.isSuccess) {
                if (dialogClose) {
                    $scope.tripGroupPopup.errorMessages = null;
                    let dialog = $("#dialog_TripGroup").data("kendoDialog");
                    dialog.close();
                }
                return true;
            }
            else {
                $scope.tripGroupPopup.errorMessages = [result.messages[0]];
                $scope.$apply();
                return false;
            }
        }
    }


    function isUsingHotel() {
        let dataGridBTADetail = getDataGrid('#btaListDetailGrid');
        if (!$.isEmptyObject(dataGridBTADetail)) {
            let stayHotelItems = dataGridBTADetail.filter(x => x.stayHotel == true);
            return stayHotelItems.length > 0;
        }
        else {
            return false;
        }
    }

    function getMaxGradeFromBTADetails(details) {
        if (details && details.length) {
            var itemHasMaxGradeUser = _.maxBy(details, function (o) { return o.userGradeValue; });
            if (itemHasMaxGradeUser) {
                return { maxGrade: `G${itemHasMaxGradeUser.userGradeValue}`, maxDepartmentId: itemHasMaxGradeUser.departmentId };
            }
        }
    }

    async function validateFromToDateBusinessDetail(gridBTADetail) {
        let result = '';
        if (gridBTADetail && gridBTADetail.length) {
            let model = {
                businessTripApplicationId: $scope.model.id,
                BusinessTripDetails: JSON.stringify(gridBTADetail)
            }
            var res = await btaService.getInstance().bussinessTripApps.validate(model).$promise;
            if (res.isSuccess) {
                result = res.object.data;
            }
        }
        return result;
    }

    $scope.warning = [];
    function translateValidate(arrayResultValidate) {
        let errors = [];
        if (arrayResultValidate) {
            let sapCodes = _.groupBy(arrayResultValidate, 'sapCode');
            Object.keys(sapCodes).forEach(x => {
                let referenceNumbers = _.groupBy(sapCodes[x], 'referenceNumber');
                Object.keys(referenceNumbers).forEach(y => {
                    errors = errors.concat(checkFromToDateInTicketOther(referenceNumbers[y], x));
                });
                // errors = errors.concat(checkFromToDateInTicketOther(sapCodes[x], x));
            });
        }
        $scope.$applyAsync();
        return errors;
    }

    propertiesBTADetail = [
        {
            property: "gender",
            title: $translate.instant('COMMON_GENDER_REQUIRED'),
        },
        {
            property: "departureCode",
            title: $translate.instant('BTA_DEPARTURE_REQUIRED'),
        },
        {
            property: "arrivalCode",
            title: $translate.instant('BTA_ARRIVAL_REQUIRED'),
        },
        {
            property: 'fromDate',
            title: $translate.instant('BTA_FROM_DATE_REQUIRED'),
        },
        {
            property: 'toDate',
            title: $translate.instant('BTA_TO_DATE_REQUIRED'),
        }
    ]

    propertiesBTADetailHaveHotel = [
        {
            property: "gender",
            title: $translate.instant('COMMON_GENDER_REQUIRED'),
        },
        {
            property: "departureCode",
            title: $translate.instant('BTA_DEPARTURE_REQUIRED'),
        },
        {
            property: "arrivalCode",
            title: $translate.instant('BTA_ARRIVAL_REQUIRED'),
        },
        {
            property: 'fromDate',
            title: $translate.instant('BTA_FROM_DATE_REQUIRED'),
        },
        {
            property: 'toDate',
            title: $translate.instant('BTA_TO_DATE_REQUIRED'),
        },
        {
            property: 'hotelCode',
            title: $translate.instant('BTA_HOTEL_REQUIRED'),
        },
        {
            property: 'checkInHotelDate',
            title: $translate.instant('BTA_CHECK_IN_HOTEL_DATE_REQUIRED'),
        },
        {
            property: 'checkOutHotelDate',
            title: $translate.instant('BTA_CHECK_OUT_HOTEL_DATE_REQUIRED'),
        },
    ]

    propertiesBTADetailHaveFlight = [
        {
            property: "email",
            title: $translate.instant('COMMON_EMAIL_REQUIRED'),
        },
        {
            property: "mobile",
            title: $translate.instant('COMMON_MOBILE_NUMBER_REQUIRED'),
        },
        {
            property: "dateOfBirth",
            title: $translate.instant('COMMON_DATE_OF_BIRTH_REQUIRED'),
        }
    ];

    propertiesBTADetailHaveFlight_International = [
        {
            property: "email",
            title: $translate.instant('COMMON_EMAIL_REQUIRED'),
        },
        {
            property: "mobile",
            title: $translate.instant('COMMON_MOBILE_REQUIRED'),
        },
        {
            property: "dateOfBirth",
            title: $translate.instant('COMMON_DATE_OF_BIRTH_REQUIRED'),
        },
        {
            property: "passport",
            title: $translate.instant('COMMON_PASSPORT_REQUIRED'),
        },
        {
            property: "passportDateOfIssue",
            title: $translate.instant('COMMON_PASSPORT_DATE_OF_ISSUE_REQUIRED'),
        },
        {
            property: "passportExpiryDate",
            title: $translate.instant('COMMON_PASSPORT_EXPIRY_DATE_REQUIRED'),
        }
    ];

    $scope.errors = [];
    function validationBTADetail() {
        $scope.model.carRental = !$scope.isChecked ? "" : $scope.model.carRental;
        let errors = [];
        //level 1
        errors = dynamicValidateBTADetail(1);
        if (errors.length == 0) {
            //level 2
            errors = dynamicValidateBTADetail(2);
            if (errors.length == 0) {
                //level 3
                errors = dynamicValidateBTADetail(3);
            }
        }
        return errors;
    }

    function dynamicValidateBTADetail(level) {
        let errors = [];
        let dataGrid = getDataGrid('#btaListDetailGrid');
        switch (level) {
            case 1:
                if (!dataGrid.length) {
                    errors.push({ controlName: $translate.instant('BTA_TABLE_DETAIL_REQUIRED'), message: ': ' + $translate.instant('COMMON_FIELD_IS_REQUIRED') });
                }
                if ($scope.isChecked && !$scope.model.carRental) {
                    errors.push({ controlName: $translate.instant('CAR_RENTAL'), message: ': ' + $translate.instant('COMMON_FIELD_IS_REQUIRED') });
                }
                break;
            case 2:
                dataGrid.forEach(item => {
                    if (item.hotelCode) {
                        errors = errors.concat(validationForTable(item, propertiesBTADetailHaveHotel, item.no));
                    }
                    else {
                        errors = errors.concat(validationForTable(item, propertiesBTADetail, item.no));
                    }

                    if ($scope.model.type == 2 || $scope.model.type == 3) {
                        let requiredRules = $.merge([], $scope.model.type == 3 ? propertiesBTADetailHaveFlight : propertiesBTADetailHaveFlight_International);
                        if (item.isForeigner != true) {
                            //ADD ID CARD rule
                            requiredRules.push(
                                {
                                    property: "idCard",
                                    title: $translate.instant('COMMON_ID_CARD_NUMBER_REQUIRED'),
                                });
                        }
                        errors = errors.concat(validationForTable(item, requiredRules, item.no));
                    }

                    if (item.isForeigner == true) {
                        if (item.passport == null || item.passport.trim() == '') {
                            errors.push({ controlName: `${item.sapCode} - ${item.fullName}: ${$translate.instant('BTA_PASSPORT')}`, message: ': ' + $translate.instant('COMMON_FIELD_IS_REQUIRED') });
                        }
                        if ($scope.model.type === "2" && $.isEmptyObject(item.passportDateOfIssue)) {
                            errors.push({ controlName: `${item.sapCode} - ${item.fullName}: ${$translate.instant('BTA_PASSPORT_DATE_OF_ISSUE')}`, message: ': ' + $translate.instant('COMMON_FIELD_IS_REQUIRED') });
                        }
                        if ($scope.model.type === "2" && $.isEmptyObject(item.passportExpiryDate)) {
                            errors.push({ controlName: `${item.sapCode} - ${item.fullName}: ${$translate.instant('BTA_PASSPORT_EXPIRY_DATE_ISSUE')}`, message: ': ' + $translate.instant('COMMON_FIELD_IS_REQUIRED') });
                        }
                    }
                    else {
                        if (item.idCard) {
                            if (item.idCard.length < 9 || (item.idCard.length >= 10 && item.idCard.length < 12)) {
                                errors.push({ controlName: $translate.instant('BTA_ID_CARD_NOT_INVALID') + ': ' + item.no, message: ': ' + $translate.instant('COMMON_IS_NOT_INVALID') });
                            }
                        }
                    }

                });

                if (!$scope.model.requestorNote) {
                    errors.push({
                        controlName: $translate.instant('COMMON_REQUESTOR_NOTE'),
                        message: ': ' + $translate.instant('COMMON_FIELD_IS_REQUIRED')
                    });
                } else if (!$scope.model.type) {
                    errors.push({
                        controlName: $translate.instant('COMMON_TYPE'),
                        message: ': ' + $translate.instant('COMMON_FIELD_IS_REQUIRED')
                    });
                }
                else if (!$scope.model.dataRoundTrip) {
                    errors.push({
                        controlName: $translate.instant('COMMON_TICKETTYPE'),
                        message: ': ' + $translate.instant('COMMON_FIELD_IS_REQUIRED')
                    });
                }
                break;
            case 3:
                dataGrid.forEach(item => {
                    //Check in Hotel Date, Check out Hotel Date bị trùng nhau
                    errors = errors.concat(checkDuplicateInOutHotel(item));
                    //1 SAPCode khong duoc departure va arrival trùng nhau
                    errors = errors.concat(checkDuplicateDepartureArrival(item));
                    //1 SAPCode khong duoc from date va to date trùng nhau
                    errors = errors.concat(checkDuplicateFromToDate(item));
                });
                //from-date đã có nằm trong khoảng from-date mới
                let itemsCloneFromToDate = _.cloneDeep(dataGrid);
                let saoCodes = _.groupBy(itemsCloneFromToDate, 'sapCode');
                Object.keys(saoCodes).forEach(x => {
                    errors = errors.concat(checkFromToDate(saoCodes[x], x));
                });
                break;
            default:
                break;
        }
        return errors;
    }
    function validateChangeCancelDetail() {
        let errors = [];
        return errors;
    }
    $scope.printForm = async function () {
        let res = await btaService.getInstance().actings.printForm({ id: $scope.model.id }).$promise;
        if (res.isSuccess) {
            exportToPdfFile(res.object);
        }
    }

    function checkDuplicateFromToDate(item) {
        let errors = [];
        var resultDay = dateFns.differenceInDays(item.toDate, item.fromDate);
        var resultHours = dateFns.differenceInHours(item.toDate, item.fromDate);
        var resultMinutes = dateFns.differenceInMinutes(item.toDate, item.fromDate);
        if (resultDay == 0 && resultHours == 0 && resultMinutes == 0) {
            errors.push({ controlName: $translate.instant('COMMON_ROW_NO') + ' ' + item.no + ' ' + $translate.instant('BTA_FROM_DATE_AND_TO_DATE'), message: ': ' + moment(item.fromDate).format(appSetting.longDateFormat) + ' ' + $translate.instant('BTA_DUPLICATE_DATE') });
        }
        return errors;
    }

    function checkDuplicateDepartureArrival(item) {
        let errors = [];
        if (item.departureCode == item.arrivalCode) {
            errors.push({ controlName: $translate.instant('COMMON_ROW_NO') + ' ' + item.no + ' ' + $translate.instant('BTA_FROM_DEPARTURE_TO_ARRIVAL'), message: ': ' + item.arrivalName + ' ' + $translate.instant('BTA_DUPLICATE_DATE') });
        }
        return errors;
    }

    function checkDuplicateInOutHotel(item) {
        let errors = [];
        if (item.checkInHotelDate || item.checkOutHotelDate) {
            let resultDay = dateFns.differenceInDays(item.checkInHotelDate, item.checkOutHotelDate);
            let resultHours = dateFns.differenceInHours(item.checkInHotelDate, item.checkOutHotelDate);
            let resultMinutes = dateFns.differenceInMinutes(item.checkInHotelDate, item.checkOutHotelDate);
            if (resultDay == 0 && resultHours == 0 && resultMinutes == 0) {
                errors.push({ controlName: $translate.instant('COMMON_ROW_NO') + ' ' + item.no + ' ' + $translate.instant('BTA_CHECK_IN_AND_CHECK_OUT_DATE'), message: ': ' + moment(item.fromDate).format(appSetting.longDateFormat) + ' ' + $translate.instant('BTA_DUPLICATE_DATE') });
            }
        }
        return errors;
    }
    function checkDuplicateNewCheckInOutHotel(item) {
        let errors = [];
        if (item.newCheckInHotelDate || item.newCheckOutHotelDate) {
            let resultDay = dateFns.differenceInDays(item.newCheckInHotelDate, item.newCheckOutHotelDate);
            let resultHours = dateFns.differenceInHours(item.newCheckInHotelDate, item.newCheckOutHotelDate);
            let resultMinutes = dateFns.differenceInMinutes(item.newCheckInHotelDate, item.newCheckOutHotelDate);
            if (resultDay == 0 && resultHours == 0 && resultMinutes == 0) {
                errors.push({ controlName: $translate.instant('COMMON_ROW_NO') + ' ' + item.no + ' ' + $translate.instant('BTA_NEW_CHECK_IN_AND_NEW_CHECK_OUT_DATE'), message: ': ' + moment(item.fromDate).format(appSetting.longDateFormat) + ' ' + $translate.instant('BTA_DUPLICATE_DATE') });
            }
        }
        return errors;
    }

    function checkFromToDateInTicketOther(dataGridTicket, sapCode) {
        let errors = [];
        let arrayDayApply = [];
        let arrayDay = [];
        let arrayDayTicket = [];
        let arrayFromToTemporaryTicket = [];
        let arrayFromToTemporary = [];
        let referenceNumberString = '';
        let objectFromToDateTicket = [];
        //liet ke thanh array: 1/2 -> 3/2 => array = [1/2, 2/2, 3/2]
        dataGridTicket.forEach(item => {
            let value = {
                fromDate: dateFns.setSeconds(new Date(item.fromDate), 00),
                toDate: dateFns.setSeconds(new Date(item.toDate), 00),
            }
            objectFromToDateTicket.push(value);
            arrayFromToTemporaryTicket.push(dateFns.setSeconds(new Date(item.fromDate), 00));
            arrayFromToTemporaryTicket.push(dateFns.setSeconds(new Date(item.toDate), 00));
        });
        arrayFromToTemporaryTicket = arrayFromToTemporaryTicket.sort(function (a, b) {
            var dateA = new Date(a), dateB = new Date(b);
            return dateA - dateB;
        });
        dataGridTicket.forEach(item => {
            arrayDayTicket.push(item.fromDate);
            if (item.toDate) {
                let tempDate = item.fromDate;
                if (dateFns.getDayOfYear(tempDate) == dateFns.getDayOfYear(item.toDate)) {
                    arrayDayTicket.push(item.toDate);
                }
                else {
                    while (dateFns.getDayOfYear(tempDate) < dateFns.getDayOfYear(item.toDate)) {
                        tempDate = addDays(tempDate, 1);
                        let resultCheck = arrayFromToTemporaryTicket.find(x => moment(x).format(appSetting.longDateFormatAMPM) == moment(tempDate).format(appSetting.longDateFormatAMPM));
                        if (!resultCheck) {
                            tempDate = dateFns.setHours(tempDate, 0);
                            tempDate = dateFns.setMinutes(tempDate, 00);
                        }
                        if (moment(tempDate).format(appSetting.sortDateFormat) == moment(item.toDate).format(appSetting.sortDateFormat)) {
                            if (moment(tempDate).format(appSetting.longDateFormatAMPM) != moment(item.toDate).format(appSetting.longDateFormatAMPM)) {
                                tempDate = item.toDate;
                            }
                        }
                        arrayDayTicket.push(tempDate);
                    }
                }
            }
            if (item.referenceNumber) {
                referenceNumberString = item.referenceNumber;
            }
        });
        let dataGrid = getDataGrid('#btaListDetailGrid');
        let arrayGrid = [];
        let dataSapCodes = _.groupBy(dataGrid, 'sapCode');
        Object.keys(dataSapCodes).forEach(x => {
            if (x == sapCode) {
                arrayGrid = dataSapCodes[x];
            }
        });

        objectFromDate = [];
        arrayGrid.forEach(item => {
            let value = {
                fromDate: dateFns.setSeconds(new Date(item.fromDate), 00),
                toDate: dateFns.setSeconds(new Date(item.toDate), 00),
            }
            objectFromDate.push(value);
            arrayFromToTemporary.push(dateFns.setSeconds(new Date(item.fromDate), 00));
            arrayFromToTemporary.push(dateFns.setSeconds(new Date(item.toDate), 00));
        });
        arrayGrid.forEach(item => {
            arrayDay.push(item.fromDate);
            if (item.toDate) {
                let tempDate = item.fromDate;
                if (dateFns.getDayOfYear(tempDate) == dateFns.getDayOfYear(item.toDate)) {
                    arrayDay.push(item.toDate);
                }
                else {
                    while (dateFns.getDayOfYear(tempDate) < dateFns.getDayOfYear(item.toDate)) {
                        tempDate = addDays(tempDate, 1);
                        let resultCheck = arrayFromToTemporary.find(x => moment(x).format(appSetting.longDateFormatAMPM) == moment(tempDate).format(appSetting.longDateFormatAMPM));
                        if (!resultCheck) {
                            tempDate = dateFns.setHours(tempDate, 0);
                            tempDate = dateFns.setMinutes(tempDate, 00);
                        }
                        if (moment(tempDate).format(appSetting.sortDateFormat) == moment(item.toDate).format(appSetting.sortDateFormat)) {
                            if (moment(tempDate).format(appSetting.longDateFormatAMPM) != moment(item.toDate).format(appSetting.longDateFormatAMPM)) {
                                tempDate = item.toDate;
                            }
                        }
                        arrayDay.push(tempDate);
                    }
                }
            }
        });
        arrayDay = arrayDay.sort(function (a, b) {
            var dateA = new Date(a), dateB = new Date(b);
            return dateA - dateB;
        });
        // trường hợp from-to date của các phiếu # là con của phiếu hiện tại
        arrayFromToTemporaryTicket.forEach(item => {
            let result = checkDayDuplicate(item, objectFromDate);
            if (result && !arrayDayApply.find(x => moment(x).format(appSetting.longDateFormatAMPM) == moment(result).format(appSetting.longDateFormatAMPM))) {
                arrayDayApply.push(result);
            }
        });
        //trường hợp from-to date của các phiếu # là cha của phiếu hiện tại
        arrayFromToTemporary.forEach(item => {
            let result = checkDayDuplicate(item, objectFromToDateTicket);
            if (result && !arrayDayApply.find(x => moment(x).format(appSetting.longDateFormatAMPM) == moment(result).format(appSetting.longDateFormatAMPM))) {
                arrayDayApply.push(result);
            }
        });
        //trường hợp from-to date của các phiếu # giống phiếu hiện tại
        objectFromToDateTicket.forEach(item => {
            let result = checkDayOtherSameDayCheck(item, objectFromDate);
            result.forEach(x => {
                if (x && !arrayDayApply.find(y => moment(y).format(appSetting.longDateFormatAMPM) == moment(x).format(appSetting.longDateFormatAMPM))) {
                    arrayDayApply.push(x);
                }
            });
        });
        //bind ra thanh string
        if (arrayDayApply.length) {
            arrayDayApply = arrayDayApply.sort(function (a, b) {
                var dateA = new Date(a), dateB = new Date(b);
                return dateA - dateB;
            });
            let dayApply = '';
            arrayDayApply.forEach(item => {
                dayApply = (dayApply ? dayApply + ', ' : dayApply) + moment(item).format(appSetting.longDateFormat);
            });
            errors.push({
                controlName: `${sapCode}: `,
                message: $translate.instant('BTA_DAY_APPLY_MESSAGE') + ': ' + `${dayApply}` + ' - ' + `${referenceNumberString}`
            });
        }
        return errors;
    }

    function checkDayDuplicate(item, arrayDay) {
        let result = '';
        arrayDay.forEach(x => {
            if (item >= x.fromDate && item < x.toDate) {
                result = item;
            }
        });
        return result;
    }

    function checkDayOtherSameDayCheck(item, arrayDay) {
        let result = [];
        arrayDay.forEach(x => {
            if (moment(item.fromDate).format(appSetting.longDateFormatAMPM) == moment(x.fromDate).format(appSetting.longDateFormatAMPM)
                && moment(item.toDate).format(appSetting.longDateFormatAMPM) == moment(x.toDate).format(appSetting.longDateFormatAMPM)) {
                result.push(item.fromDate);
                result.push(item.toDate);
            }
        });
        return result;
    }


    function checkFromToDate(dataGrid, sapCode) {
        let errors = [];
        let arrayDayApply = [];
        let arrayDay = [];
        let arrayFromToTemporary = [];
        //liet ke thanh array: 1/2 -> 3/2 => array = [1/2, 2/2, 3/2]
        dataGrid.forEach(item => {
            let value = {
                fromDate: item.fromDate,
                toDate: item.toDate
            }
            arrayFromToTemporary.push(value);
            arrayDay.push(item.fromDate);
            arrayDay.push(item.toDate);
        });
        //code moi
        arrayDay.forEach(item => {
            let result = checkFromToDateInvalid(item, arrayFromToTemporary);
            if (result) {
                arrayDayApply.push(item);
            }
        });
        arrayDayApply = arrayDayApply.sort(function (a, b) {
            var dateA = new Date(a), dateB = new Date(b);
            return dateA - dateB;
        });
        //
        let arraySameToDateFromDate = [];
        //bind ra thanh string
        if (arrayDayApply.length) {
            let dayApply = '';
            arrayDayApply.forEach(item => {
                dayApply = (dayApply ? dayApply + ', ' : dayApply) + moment(item).format(appSetting.longDateFormat);
            });
            if (arraySameToDateFromDate.length == 0) {
                errors.push({
                    controlName: `${sapCode}: `,
                    message: $translate.instant('BTA_DUPLICATE_DAY_APPLY_MESSAGE') + ': ' + `${dayApply}`
                });
            }
        }
        return errors;
    }

    function checkFromToDateInvalid(item, arrayData) {
        let result = '';
        arrayData.forEach(x => {
            if (item > x.fromDate) {
                if (moment(item).format(appSetting.longDateFormat) != moment(x.fromDate).format(appSetting.longDateFormat)) {
                    if (item < x.toDate) {
                        if (moment(item).format(appSetting.longDateFormat) != moment(x.toDate).format(appSetting.longDateFormat)) {
                            result = item;
                        }
                    }
                }
            }
        });
        return result;
    }

    function validationForTable(model, requiredFields, no) {
        let errors = [];
        requiredFields.forEach(field => {
            if (!model[field.property]) {
                let allowAddError = true;
                if (field.property == 'fromDate' && !$.isEmptyObject(model[field.property])) {
                    allowAddError = false;
                }
                if (allowAddError) {
                    errors.push({
                        controlName: `${field.title} ${no}: `,
                        message: $translate.instant('COMMON_FIELD_IS_REQUIRED')
                    });
                }
            } else if (field.property === 'email' || field.property === 'mobile') {
                if (model[field.property] !== '********') {
                    if (field.property === 'email') {
                        var validRegex = /^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$/;
                        if (!model[field.property].match(validRegex)) {
                            errors.push({
                                controlName: `${field.title} ${no}: `,
                                message: $translate.instant('BTA_EMAIL_INVALID')
                            });
                        }
                    } else {
                        var validRegexMobile = /^(\+\d{1,3}[- ]?)?\d{10}$/
                        if (model[field.property].match(validRegexMobile) && !(model[field.property].match(/0{5,}/))) {

                        } else {
                            errors.push({
                                controlName: `${field.title} ${no}: `,
                                message: $translate.instant('BTA_MOBILE_INVALID')
                            });
                        }
                    }
                }
            }
        });
        return errors;
    }


    async function ngOnInit() {
        await getBusinessTripLocations();
        await getFlightNumbers();
        await getRoomTypes();
        await getAllHotels();

        $scope.isBookingFlightWF = false;

        if ($stateParams.id) {
            await getBusinessTripLocationById($stateParams.id);
        }
        else {
            if ($rootScope.currentUser) {
                //get dept/Line
                $scope.model.userSAPCode = $rootScope.currentUser.sapCode;
                $scope.model.userCreatedFullName = $rootScope.currentUser.fullName;
                await getDepartmentByUserId($rootScope.currentUser.id, $rootScope.currentUser.deptId);
            }
        }
        if ($scope.perm) {
            $timeout(function () {
                $("#attachmentDetail").removeAttr("disabled").removeAttr("readonly");
                $("#attachmentChanging").attr("disabled", "disabled");
                $(".deleteattachmentChanging").addClass("display-none");
                $(".deleteattachmentDetail").removeClass("display-none");
            }, 0);
        }

        $scope.$apply();
    }
    $scope.isRoom = false;
    $scope.statusDraf = true;
    async function getBusinessTripLocationById(id) {
        var res = await btaService.getInstance().bussinessTripApps.getItemById({ id: id }, null).$promise;
        if (res.isSuccess) {
            $scope.model = res.object;
            var dropdownDeptLine = $("#dept_line_id").data("kendoDropDownList");
            var dropdownDivision = $("#division_id").data("kendoDropDownTree");
            let count = 1;
            $scope.model.businessTripDetails.forEach(item => {
                item['no'] = count;
                count++;
                let resultArrival = businessTripLocations.find(x => x.code == item.arrivalCode);
                if (!resultArrival) {
                    businessTripLocations.push({ code: item.arrivalCode, name: item.arrivalName });
                }
                let resultDeparture = businessTripLocations.find(x => x.code == item.departureCode);
                if (!resultDeparture) {
                    businessTripLocations.push({ code: item.departureCode, name: item.departureName });
                }
            });
            if ($scope.model.carRental) {
                $scope.isChecked = true;
            }
            $scope.departureOption = businessTripLocations;
            $scope.arrivalOption = businessTripLocations;
            setDataSourceForGrid('#btaListDetailGrid', $scope.model.businessTripDetails);
            $scope.btaListDetailGridOptions.dataSource.data = $scope.model.businessTripDetails;
            $scope.model.businessTripDetails.forEach(item => {
                let toDateId = "#toDate" + item.no;
                if ($(toDateId).data('kendoDateTimePicker')) {
                    $(toDateId).data('kendoDateTimePicker').min(new Date(item.fromDate));
                }
                let checkInId = '#checkInHotelDate' + item.no;
                let checkOutId = '#checkOutHotelDate' + item.no;
                if ($(checkInId).data('kendoDateTimePicker') || $(checkOutId).data('kendoDateTimePicker')) {
                    $(checkInId).data('kendoDateTimePicker').min(new Date(item.fromDate));
                    $(checkInId).data('kendoDateTimePicker').max(new Date(item.toDate));
                    $(checkOutId).data('kendoDateTimePicker').min(new Date(item.fromDate));
                    $(checkOutId).data('kendoDateTimePicker').max(new Date(item.toDate));
                }
                //flight number
                let flightNumberId = 'flightNumber' + item.no;
                setDropDownTree(flightNumberId, $scope.flightNumberOption);
                var flightNumberOption = $("#" + flightNumberId).data("kendoDropDownTree");
                if (flightNumberOption) {
                    flightNumberOption.value(item.flightNumberCode);
                }
                //come back number
                let comeBackFlightId = 'comeBackFlight' + item.no;
                setDropDownTree(comeBackFlightId, $scope.flightNumberOption);
                var comeBackFlightOption = $("#" + comeBackFlightId).data("kendoDropDownTree");
                if (comeBackFlightOption) {
                    comeBackFlightOption.value(item.comebackFlightNumberCode);
                }
            });

            if ($scope.model.status != 'Draft') {
                initFormChangingCancle();
                $timeout(function () {
                    $scope.statusDraf = false;
                }, 0)
            }
            if ($scope.model.changeCancelBusinessTripDetails && $scope.model.changeCancelBusinessTripDetails.length > 0) {
                count = 1;
                $scope.model.changeCancelBusinessTripDetails.forEach(item => {
                    item['no'] = count;
                    count++;
                });
                setDataSourceForGrid('#btaListChangeOrCancelGrid', $scope.model.changeCancelBusinessTripDetails);
                $scope.model.changeCancelBusinessTripDetails.forEach(item => {
                    $scope.fromDateGridChangingCancellingChanged(item, true);
                });
            }

            if ($scope.model.roomOrganizations && $scope.model.roomOrganizations.length > 0) {
                let dataTemporary = $scope.model.roomOrganizations;
                count = 1;
                dataTemporary.forEach(item => {
                    item['no'] = count;
                    item['showUsers'] = splitNameUsers(item.users);
                    count++;
                });
                setDataSourceForGrid('#roomViewGrid', dataTemporary);
                //
            }
            if ($scope.model.changedRoomOrganizations && $scope.model.changedRoomOrganizations.length > 0) {
                let dataTemporary = $scope.model.changedRoomOrganizations;
                count = 1;
                dataTemporary.forEach(item => {
                    item['no'] = count;
                    item['showUsers'] = splitNameUsers(item.users);
                    count++;
                });
                setDataSourceForGrid('#changeRoomViewGrid', dataTemporary);
                //
            }
            if ($stateParams.id) {
                //$scope.model.isAgree = true;
                if ($scope.model.status != 'Draft') {
                    $('#argree_id').css('disabled', 'disabled');
                }
            }
            //get hotel 
            $scope.model.businessTripDetails.forEach(item => {
                $scope.changeArrival(item, true);
            });
            if ($scope.model.changeCancelBusinessTripDetails) {
                $scope.model.changeCancelBusinessTripDetails.forEach(item => {
                    $scope.changeDestination(item, true);
                })
            }
            // bind file attachment
            if ($scope.model.documentDetails) {
                if ($scope.model.roomOrganizations === undefined || $scope.model.roomOrganizations === null) {
                    $scope.oldAttachmentDetails = JSON.parse($scope.model.documentDetails);
                } else {
                    if ($rootScope.currentUser.isAdmin && ($scope.model.status === "Waiting for Admin Checker" || $scope.model.status === "Waiting for Admin Manager Approval")) {
                        $scope.oldAttachmentDetails = JSON.parse($scope.model.documentDetails);
                    } else if ($scope.model.status === "Completed") {
                        $scope.oldAttachmentDetails = JSON.parse($scope.model.documentDetails);
                    }
                }
            }
            if ($scope.model.documentChanges) {
                $scope.oldAttachmentChanges = JSON.parse($scope.model.documentChanges);
            }
            //load department
            if ($rootScope.currentUser) {
                if ($scope.model.employeeCode == $rootScope.currentUser.sapCode) {
                    kendo.ui.progress($("#loading_bta"), true);
                    await getDepartmentByUserId($rootScope.currentUser.id, $rootScope.currentUser.deptId);
                }
                else {
                    let valueDeptLine = [{ id: $scope.model.deptLineId, code: $scope.model.deptCode, name: $scope.model.deptName }];
                    if (dropdownDeptLine) {
                        dropdownDeptLine.setDataSource(valueDeptLine);
                    }
                    let valueDivision = [{ id: $scope.model.deptDivisionId, code: $scope.model.deptDivisionCode, name: $scope.model.deptDivisionName }];
                    if (dropdownDivision) {
                        setDropDownTree("division_id", valueDivision);
                    }
                }
            }

            //bind Data
            kendo.ui.progress($("#loading_bta"), true);
            if (dropdownDeptLine) {
                dropdownDeptLine.value($scope.model.deptLineId);
            }
            if (dropdownDivision) {
                $scope.temporaryDivision = $scope.model.deptDivisionId;
                dropdownDivision.value($scope.model.deptDivisionId);
            }
            //$("#carRental_id").attr("disabled", "disabled");
            kendo.ui.progress($("#loading_bta"), false);

            if ($scope.model.type === 3 || $scope.model.type === 2) {//booking flight WF
                $scope.isBookingFlightWF = true;
            }

            if (!$scope.model.isRoundTrip) {
                $scope.model.dataRoundTrip = 1;
            } else if ($scope.model.isRoundTrip) {
                $scope.model.dataRoundTrip = 2;
            }
        }
    }

    function disableCheckInCheckOut(no) {
        let checkInId = '#checkInHotelDate' + no;
        let checkOutId = '#checkOutHotelDate' + no;
        var checkIn = $(checkInId).data("kendoDateTimePicker");
        var checkOut = $(checkOutId).data("kendoDateTimePicker");
        if (checkIn || checkOut) {
            checkIn.readonly();
            checkOut.readonly();
        }
    }
    function disableNewCheckInCheckOut(no) {
        let checkInId = '#newCheckInHotelDate' + no;
        let checkOutId = '#newCheckOutHotelDate' + no;
        var checkIn = $(checkInId).data("kendoDateTimePicker");
        var checkOut = $(checkOutId).data("kendoDateTimePicker");
        if (checkIn || checkOut) {
            checkIn.readonly();
            checkOut.readonly();
        }
    }

    function disableCheckInOutAllGrid() {
        let dataGrid = getDataGrid("#btaListDetailGrid");
        dataGrid.forEach(item => {
            if (!item.hotelCode) {
                disableCheckInCheckOut(item.no);
            }
        });
    }

    function splitNameUsers(data) {
        var result = '';
        if (data.length) {
            data.forEach(item => {
                result = (result ? result + ', ' : result) + item.sapCode + ' - ' + item.fullName;
            });
        }
        return result;
    }

    //changing / canceling
    function initFormChangingCancle() {
        let resultDataSapCode = [];
        let grid = $("#btaListDetailGrid").data("kendoGrid");
        if (grid) {
            let currentDataOnGrid = grid.dataSource._data;
            let saoCodes = _.groupBy(currentDataOnGrid, 'sapCode');
            Object.keys(saoCodes).forEach(x => {
                resultDataSapCode.push(saoCodes[x][0]);
            });
            $scope.sapCodeDataSource = resultDataSapCode;
        }
    }

    function filterArray(array1, array2) {
        var result = _.filter(array1, function (item) { return !array2.map(a => a.sapCode).includes(item.sapCode) });
        return result;
    }

    $scope.addMore = async function () {
        $scope.isEditViewTicket = true;
        $scope.userChangingCanceling = getDataGrid("#btaListDetailGrid");
        let dataBtaListChangeOrCancelGrid = getDataGrid('#btaListChangeOrCancelGrid');
        if (dataBtaListChangeOrCancelGrid.length > 0) {
            $scope.userChangingCanceling = filterArray($scope.userChangingCanceling, dataBtaListChangeOrCancelGrid);
        }
        let count = 1;
        $scope.userChangingCanceling.forEach(item => {
            //item['no'] = 1;
            item['isCheck'] = false;
            //count++;
        });
        setGridUser($scope.userChangingCanceling, '#userGridChangingCancel', $scope.userChangingCanceling.length, 1, appSetting.pageSizeDefault);
        $scope.userChangingCancelingCheck = [];
        $scope.checkAllChanging = false;
        let dialog = $("#dialog_changing_cancel").data("kendoDialog");
        dialog.title($translate.instant('BTA_CHANGING_CACELLING_ROW'));
        dialog.open();
        $rootScope.confirmDialogAddItemsUser = dialog;
        $timeout(function () {
            $("#checkAllChanging").removeAttr("disabled").removeAttr("readonly");
        }, 0)
    }
    $scope.dialogUserGridChangingCancelOption = {
        buttonLayout: "normal",
        animation: {
            open: {
                effects: "fade:in"
            }
        },
        schema: {
            model: {
                id: "no"
            }
        },
        actions: [{
            text: $translate.instant('COMMON_BUTTON_OK'),
            action: function (e) {
                let dataGrid = getDataGrid('#btaListChangeOrCancelGrid');
                let count = dataGrid.length;
                $scope.userChangingCancelingCheck.forEach(async item => {
                    count++;
                    let value = {
                        no: count,
                        id: item.id,
                        departmentCode: item.departmentCode,
                        departmentName: item.departmentName,
                        destinationCode: item.arrivalCode,
                        destinationName: item.arrivalName,
                        isForeigner: item.isForeigner,
                        isCancel: false,
                        reason: '',
                        newFlightNumberCode: item.flightNumberCode,
                        newFlightNumberName: item.flightNumberName,
                        newComebackFlightNumberCode: item.comebackFlightNumberCode,
                        newComebackFlightNumberName: item.comebackFlightNumberName,
                        businessTripApplicationDetailId: item.id,
                        toDate: item.toDate,
                        fromDate: item.fromDate,
                        newAirlineCode: item.airlineCode,
                        newAirlineName: item.airlineName,
                        newComebackAirlineCode: item.comebackAirlineCode,
                        newComebackAirlineName: item.comebackAirlineName,
                        newHotelCode: item.hotelCode,
                        newHotelName: item.hotelName,
                        newCheckInHotelDate: item.checkInHotelDate,
                        newCheckOutHotelDate: item.checkOutHotelDate,
                        sapCode: item.sapCode,
                        fullName: item.fullName,
                        userId: item.userId,
                        email: item.email,
                        gender: item.gender,
                        rememberInformation: item.rememberInformation,
                        businessTripApplicationDetailId: item.id,
                        isCancelInBoundFlight: false,
                        isCancelOutBoundFlight: false,
                        reasonForInBoundFlight: "",
                        reasonForOutBoundFlight: ""
                    }
                    dataGrid.push(value);
                });

                $timeout(function () {
                    setDataSourceForGrid('#btaListChangeOrCancelGrid', dataGrid);
                    dataGrid.forEach(item => {

                        $scope.fromDateGridChangingCancellingChanged(item, false);
                        $scope.changeDestination(item, true);
                        if (!item.newHotelCode) {
                            disableNewCheckInCheckOut(item.no);
                        }
                        if ($scope.isBookingFlightWF) {
                            $timeout(function () {
                                disableFieldsForChanging(item);
                            }, 100);
                        }
                    });
                    setTimeout(function () {
                        enableGridChangingCancelling(dataGrid);
                    }, 200);
                    setGridChangeCancelValue();
                }, 0);
            },
            primary: true
        }]
    };

    $scope.checkAllChanging = false;
    $scope.userChangingCanceling = [];
    $scope.userChangingCancelingCheck = [];
    $scope.userGridChangingCancelOptions = {
        dataSource: {
            serverPaging: true,
            pageSize: 10,
            schema: {
                total: () => { return $scope.userChangingCanceling.length },
                data: () => { return $scope.userChangingCanceling }
            }
        },
        sortable: false,
        autoBind: true,
        resizable: true,
        pageable: {
            // pageSize: appSetting.pageSizeWindow,
            alwaysVisible: true,
            pageSizes: [5, 10, 20, 30, 40],
            responsive: false,
            messages: {
                display: "{0}-{1} " + $translate.instant('PAGING_OF') + " {2} " + $translate.instant('PAGING_ITEM'),
                itemsPerPage: $translate.instant('PAGING_ITEM_PER_PAGE'),
                empty: $translate.instant('PAGING_NO_ITEM')
            }
        },
        columns: [
            {
                field: "isCheck",
                title: "<input type='checkbox' ng-model='checkAllChanging' name='checkAllChanging' id='checkAllChanging' class='k-checkbox' ng-change='checkAllGridChangingCancel(checkAllChanging)'/> <label class='k-checkbox-label cbox' for='checkAllChanging' style='padding-bottom: 10px;'></label>",
                width: "50px",
                template: function (dataItem) {
                    return `<input type="checkbox" ng-model="dataItem.isCheck" name="isCheck{{dataItem.no}}"
                    id="isCheck{{dataItem.no}}" class="k-checkbox" style="width: 100%;" ng-change='checkGridChangingCancel(dataItem)'/>
                    <label class="k-checkbox-label cbox" for="isCheck{{dataItem.no}}"></label>`
                }
            },
            {
                field: "sapCode",
                // title: "SAP Code",
                headerTemplate: $translate.instant('COMMON_SAP_CODE'),
                width: "100px",
            },
            {
                field: "fullName",
                // title: "Full Name",
                headerTemplate: $translate.instant('COMMON_FULL_NAME'),
                width: "200px",
            },
            {
                field: "fromDate",
                title: $translate.instant('BTA_TRIP_FROM_DATE'),
                width: "200px",
                template: function (dataItem) {
                    return moment(dataItem.fromDate).format(appSetting.longDateFormat);
                }
            },
            {
                field: "toDate",
                title: $translate.instant('BTA_TRIP_TO_DATE'),
                width: "200px",
                template: function (dataItem) {
                    return moment(dataItem.toDate).format(appSetting.longDateFormat);
                }
            },
            {
                field: "arrivalName",
                title: $translate.instant('BTA_ARRIVAL'),
                width: "200px",
            }
        ],
    }

    $scope.checkAllGridChangingCancel = function (check) {
        let dataGrid = getDataGrid('#userGridChangingCancel');
        if (check) {
            $scope.checkAllChanging = true;
            dataGrid.forEach(item => {
                item.isCheck = true;
            });
            $scope.userChangingCancelingCheck = dataGrid;
        }
        else {
            $scope.checkAllChanging = false;
            dataGrid.forEach(item => {
                item.isCheck = false;
            });
            $scope.userChangingCancelingCheck = [];
        }
        $scope.userChangingCanceling = dataGrid;
        setGridUser($scope.userChangingCanceling, '#userGridChangingCancel', $scope.userChangingCanceling.length, 1, $scope.limitDefaultGrid);
    }

    $scope.checkGridChangingCancel = function (dataItem) {
        let currentDataOnGrid = getDataGrid('#userGridChangingCancel');
        if (dataItem.isCheck) {
            $scope.userChangingCancelingCheck.push(dataItem);
        }
        else {
            var arrayTemporary = $scope.userChangingCancelingCheck.filter(x => x.id != dataItem.id);
            $scope.userChangingCancelingCheck = arrayTemporary;
        }
        if ($scope.userChangingCancelingCheck.length == currentDataOnGrid.length) {
            $scope.checkAllChanging = true;
        }
        else {
            $scope.checkAllChanging = false;
        }
    }
    //end

    //Room Organization
    var arrayFilterHotel = [];
    var isChangeRoom = false;
    idGrid = ''
    $scope.setRooms = function () {
        $scope.errors = validationBTADetail(); // validate trên cùng 1 phiếu
        $scope.$applyAsync();
        if (!$scope.errors.length) {
            $scope.roomOrganizationErrorsMessages = [];
            peoplesTotal = [];
            let gridChangeOrCancelGrid = $("#btaListChangeOrCancelGrid").data("kendoGrid");
            if ($scope.model.roomOrganizations && $scope.model.roomOrganizations.length
                && !$.isEmptyObject(gridChangeOrCancelGrid) && gridChangeOrCancelGrid.dataSource._data.length > 0) {
                isChangeRoom = true;
                idGrid = "#btaListChangeOrCancelGrid";
                arrayFilterHotel = [];
                gridChangeOrCancelGrid.dataSource._data.forEach(item => {
                    if (!item.isCancel && item.newHotelCode) {
                        //item.businessTripApplicationDetailId = item.businessTripApplicationDetailId;
                        arrayFilterHotel.push(item);
                    }
                });
                mapDataToPeopleOption(arrayFilterHotel);
                if ($scope.model.changedRoomOrganizations) {
                    if ($scope.model.changedRoomOrganizations.length) {
                        let dataTemporary = $scope.model.changedRoomOrganizations;
                        let count = 1;
                        dataTemporary.forEach(item => {
                            item['no'] = count;
                            item['peoples'] = initArrayValueUsers(item.users);
                            count++;
                        });
                        setDataSourceForGrid('#roomGrid', dataTemporary);
                    }
                    else {
                        setDataSourceForGrid('#roomGrid', $scope.rooms);
                        $scope.addRoom();
                    }
                }
            } else {
                isChangeRoom = false;
                let gridbtaListDetailGrid = $("#btaListDetailGrid").data("kendoGrid");
                idGrid = "#btaListDetailGrid";
                arrayFilterHotel = [];
                gridbtaListDetailGrid.dataSource._data.forEach(item => {
                    if (item.hotelCode && item.checkInHotelDate && item.checkOutHotelDate) {
                        arrayFilterHotel.push(item);
                    }
                });
                mapDataToPeopleOption(arrayFilterHotel);
                if ($scope.model.roomOrganizations) {
                    let dataTemporary = $scope.model.roomOrganizations;
                    let count = 1;
                    dataTemporary.forEach(item => {
                        item['no'] = count;
                        item['peoples'] = initArrayValueUsers(item.users);
                        count++;
                    });
                    setDataSourceForGrid('#roomGrid', dataTemporary);
                }
                else {
                    setDataSourceForGrid('#roomGrid', $scope.rooms);
                    $scope.addRoom();
                }
                if ($scope.model.roomOrganizations) {
                    $scope.model.roomOrganizations.forEach(item => {
                        item.peoples.forEach(x => {
                            peoplesTotal.push(x)
                        });
                    });
                }
            }
            // set title cho cái dialog
            let dialog = $("#dialog_Room").data("kendoDialog");
            dialog.title($translate.instant('BTA_ROOM_ORGANIZATION'));
            // $scope.addRoom();
            dialog.open();
            $rootScope.confirmDialogAddItemsUser = dialog;
            return dialog;
        }
    }


    function initArrayValueUsers(dataArray) {
        let result = [];
        dataArray.forEach(item => {
            if (idGrid == '#btaListChangeOrCancelGrid') {
                result.push(item.changeCancelBusinessTripApplicationDetailId);
            }
            else {
                result.push(item.businessTripApplicationDetailId);
            }
        });
        return result;
    }

    function mapDataToPeopleOption(dataGrid, id) {
        let count = 1;
        let arrayTemporary = [];
        dataGrid.forEach(item => {
            let value = {
                no: count,
                sapCode: item.sapCode,
                fullName: item.fullName,
                department: item.departmentName,
                fromtoDate: moment(item.fromDate).format(appSetting.longDateFormat) + '-' + moment(item.toDate).format(appSetting.longDateFormat),
                checkInHotelDate: moment(item.checkInHotelDate).format(appSetting.longDateFormat),
                checkOutHotelDate: moment(item.checkOutHotelDate).format(appSetting.longDateFormat),
                businessTripApplicationDetailId: item.id,
                arrivalName: item.arrivalName ? item.arrivalName : item.destinationName,
                hotelName: item.hotelCode ? getHotelDetailsFromCode(item.hotelCode) : getHotelDetailsFromCode(item.newHotelCode),
                showText: item.sapCode + ' - ' + item.fullName,
                gender: item.gender && item.gender == 1 ? 'Nam/ Male' : item.gender && item.gender == 2 ? 'Nữ/ Female' : '',
            }
            if (idGrid == '#btaListChangeOrCancelGrid') {
                value.checkInHotelDate = moment(item.newCheckInHotelDate).format(appSetting.longDateFormatAMPM);
                value.checkOutHotelDate = moment(item.newCheckOutHotelDate).format(appSetting.longDateFormatAMPM);
            }
            arrayTemporary.push(value);
            count++;
        });
        if (id) {
            $timeout(function () {
                var dropdownlist = $(id).data("kendoMultiSelect");
                if (dropdownlist) {
                    let arrayFilter = [];
                    arrayTemporary.filter(item => {
                        let result = peoplesTotal.find(x => x == item.businessTripApplicationDetailId)
                        if (!result) {
                            arrayFilter.push(item);
                        }
                    });
                    dropdownlist.setDataSource(arrayFilter);
                }
            }, 0);
        }
        else {
            $scope.peoples = arrayTemporary;
        }

    }

    function splitUserNames(data) {
        var result = '';
        if (data.length) {
            data.forEach(item => {
                result = (result ? result + ', ' : result) + item;
            });
        }
        return result;
    }
    $scope.roomOrganizationErrorsMessages = [];
    $scope.dialogRoomOption = {
        buttonLayout: "normal",
        animation: {
            open: {
                effects: "fade:in"
            }
        },
        schema: {
            model: {
                id: "no"
            }
        },
        actions: [{
            text: $translate.instant('COMMON_BUTTON_OK'),
            action: function (e) {
                let errorsDetail = [];
                let flag = false;
                let gridRoom = $("#roomGrid").data("kendoGrid");
                if ($scope.peoples.length) {
                    if (gridRoom.dataSource._data.toJSON().length > 0) {
                        let currentRoomOrganizations = gridRoom.dataSource._data.toJSON();
                        for (let i = 0; i < currentRoomOrganizations.length; i++) {
                            let currentRowItem = currentRoomOrganizations[i];
                            let errors = $rootScope.validateInRecruitment(requiredFields, currentRowItem);
                            if (errors.length == 0) {
                                if (!currentRowItem.peoples.length) {
                                    let required = {
                                        controlName: "People",
                                        errorDetail: "is required",
                                        fieldName: "peoples"
                                    }
                                    errors.push(required);
                                }
                            }
                            if (errors.length > 0) {
                                let errorList = errors.map(x => {
                                    return x.controlName
                                });
                                errorsDetail.push({
                                    row: i + 1,
                                    message: errorList.join(', ') + ' ' + $translate.instant('IS_REQURED')
                                });
                                flag = true;
                            }
                            else {
                                let currentRoomType = _.find($scope.roomTypes, x => { return x.name == currentRowItem.roomTypeName });
                                if (currentRoomType) {
                                    if (currentRowItem.peoples && currentRowItem.peoples.length > currentRoomType.quota) {
                                        errorsDetail.push({
                                            row: i + 1,
                                            // message: $translate.instant('ROOM_QUOTA_VALIDATION')
                                            message: $translate.instant('ROOM_QUOTA_VALIDATION') + ' ' + currentRoomType.quota
                                        });
                                    }
                                }
                                if (errorsDetail.length) {
                                    flag = true;
                                }
                            }
                        }
                        //Chọn các nhân viên có SAP Code giống nhau trong 1 phòng.
                        if (!flag) {
                            let arrayTemporary = [];
                            currentRoomOrganizations.forEach(item => {
                                let value = {
                                    no: item.no,
                                    users: mapUser(item.peoples, idGrid)
                                }
                                arrayTemporary.push(value);
                            });
                            errorsDetail = checkUserDuplicateInRoom(arrayTemporary);
                            if (errorsDetail.length) {
                                flag = true;
                            }
                        }
                        //Chưa xếp được hết nhân viên vào phòng khách sạn.
                        if (!flag) {
                            let arrayChooseUser = [];
                            currentRoomOrganizations.forEach(item => {
                                item.peoples.forEach(x => {
                                    arrayChooseUser.push(x);
                                });
                            });
                            if (arrayChooseUser.length < arrayFilterHotel.length) {
                                errorsDetail.push({ message: $translate.instant('BTA_CHOOSE_USER_MESSAGE') });
                                flag = true;
                            }
                        }
                    }
                    else {
                        // Notification.error('Table Business Trip Application Detail: ' + $translate.instant('COMMON_FIELD_IS_REQUIRED'));
                        errorsDetail.push({ message: $translate.instant('BTA_TABLE_DETAIL_REQUIRED') + '' + $translate.instant('COMMON_FIELD_IS_REQUIRED') });
                        flag = true;
                    }
                }
                if (!flag) {
                    let dialog = $("#dialog_RoomConfirm").data("kendoDialog");
                    dialog.title($translate.instant('BTA_ROOM_ORGANIZATION_CONFIRM'));
                    dialog.open();
                    $rootScope.confirmDialog = dialog;
                    $timeout(function () {
                        $scope.roomOrganizationErrorsMessages = [];
                    })
                    //$scope.$emit("roomOrganized", false);
                    return false;
                }
                else {
                    $timeout(function () {
                        $scope.roomOrganizationErrorsMessages = errorsDetail;
                    })
                    //$scope.$emit("roomOrganized", false);
                    return false;

                }

            },
            primary: true
        }]
    };

    $scope.dialogRoomConfirmOption = {
        buttonLayout: "normal",
        animation: {
            open: {
                effects: "fade:in"
            }
        },
        schema: {
            model: {
                id: "no"
            }
        },
        actions: [{
            text: $translate.instant('COMMON_BUTTON_OK'),
            action: function (e) {
                let roomDetail = [];
                let gridRoom = $("#roomGrid").data("kendoGrid");
                gridRoom.dataSource._data.toJSON().forEach(item => {
                    if (item.peoples.length) {
                        let value = {
                            id: item.id,
                            roomTypeId: item.roomTypeId,
                            roomTypeCode: item.roomTypeCode,
                            roomTypeName: item.roomTypeName,
                            users: mapUser(item.peoples, idGrid)
                        }
                        roomDetail.push(value);
                    }
                });
                let model = {
                    businessTripApplicationId: $stateParams.id,
                    data: JSON.stringify(roomDetail),
                    isChange: isChangeRoom
                }
                saveRoomOrganization(model);
                let dialog = $("#dialog_Room").data("kendoDialog");
                dialog.close();
                continueWorkFlow();
                return true;
            },
            primary: true
        }]
    };

    async function continueWorkFlow() {
        var res = await $scope.save();
        if (res && res.isSuccess) {
            continueWorkflowNotSave();
        }
    }

    $scope.onChangeDivision = async function (value) {
        if ($scope.btaListDetailGridData.length > 0 && !value) {
            $timeout(function () {
                $scope.dialog = $rootScope.showConfirmDelete($translate.instant('COMMON_NOTIFY'), $translate.instant('SHIFT_EXCHANGE_NOTIFY'), $translate.instant('COMMON_BUTTON_CONFIRM'));
                $scope.dialog.bind("close", confirm);
            }, 0);
        }
        else {
            if (!value) {
                kendo.ui.progress($("#loading"), true);
                clearSearchTextOnDropdownTree('division_id');
                setDataDeptDivision($scope.dataTemporaryArrayDivision);
            }
            kendo.ui.progress($("#loading"), false);
        }
    }
    confirm = async function (e) {
        let dropdownlist = $("#division_id").data("kendoDropDownTree");
        if (e.data && e.data.value) {
            if (!$scope.model.deptDivisionId) {
                clearSearchTextOnDropdownTree('division_id');
                $scope.btaListDetailGridData = new kendo.data.ObservableArray([]);
                initBTAEditListGrid($scope.btaListDetailGridData);
                if ($scope.dataTemporaryArrayDivision.length) {
                    setDataDeptDivision($scope.dataTemporaryArrayDivision);
                } else {
                    await getDivisionsByDeptLine($scope.form.dept_Line);
                    setDataDeptDivision($scope.dataTemporaryArrayDivision);
                }
                $scope.employeesDataSource = [];

            } else {
                if (dropdownlist) {
                    dropdownlist.value($scope.model.deptDivisionId);
                }
                $scope.btaListDetailGridData = new kendo.data.ObservableArray([]);
                initBTAEditListGrid($scope.btaListDetailGridData);
            }
        } else {
            if (dropdownlist) {
                dropdownlist.value($scope.temporaryDivision);
                $scope.model.deptDivisionId = $scope.temporaryDivision;
            }
        }
    }
    function initBTAEditListGrid(dataSource) {
        let grid = $("#btaListDetailGrid").data("kendoGrid");
        let dataSourceBTA = new kendo.data.DataSource({
            data: dataSource,
        });
        if (grid) {
            grid.setDataSource(dataSourceBTA);
        }
        if (!$scope.perm) {
            disableElement(true);
        }

    }
    function setDataDeptDivision(dataDepartment) {
        var dataSource = new kendo.data.HierarchicalDataSource({
            data: dataDepartment
        });
        var dropdownlist = $("#division_id").data("kendoDropDownTree");
        if (dropdownlist) {
            dropdownlist.setDataSource(dataSource);
        }
    }
    function checkUserDuplicateInRoom(array) {
        let error = [];
        array.forEach(item => {
            let sapCodes = _.groupBy(item.users, 'sapCode');
            Object.keys(sapCodes).forEach(x => {
                if (sapCodes[x].length > 1) {
                    error.push({
                        row: item.no,
                        message: $translate.instant('SAP_DUPLICATED')
                    });
                }
            });
        });
        return error;
    }

    async function saveRoomOrganization(model) {
        var res = await btaService.getInstance().bussinessTripApps.saveRoomOrganization(model).$promise;
        if (res.isSuccess) {
            // $state.go($state.current.name, { id: $scope.model.id, referenceValue: $scope.model.referenceNumber }, { reload: true });
            Notification.success($translate.instant('COMMON_SAVE_SUCCESS'));
        }
    }

    function mapUser(users, idGrid) {
        let result = [];
        let dataSapCode = getDataGrid(idGrid);
        users.forEach(item => {
            var data = dataSapCode.find(x => x.id == item);
            if (data) {
                let value = {
                    id: data.userId,
                    sapCode: data.sapCode,
                    fullName: data.fullName,
                    roomOrganizationId: $stateParams.id,
                    businessTripApplicationDetailId: data.id,
                    userId: data.userId,
                }
                if (idGrid == "#btaListChangeOrCancelGrid") {
                    value.businessTripApplicationDetailId = data.businessTripApplicationDetailId;
                    value.changeCancelBusinessTripApplicationDetailId = data.id;
                }
                result.push(value);
            }
        });
        return result;
    }

    $scope.roomTypes = []

    async function getRoomTypes() {
        let currentQuery = {
            predicate: "",
            predicateParameters: [],
            order: "Modified desc",
            limit: 10000,
            page: 1
        };

        var result = await settingService.getInstance().roomType.getListRoomTypes(currentQuery).$promise;
        if (result.isSuccess) {
            $scope.roomTypes = result.object.data;
        }
    }

    $scope.peoples = []
    $scope.peopleDataSourceArray = [];
    $scope.rooms = [];
    $scope.roomOptions = {
        dataSource: {
            data: $scope.rooms
        },
        sortable: false,
        pageable: false,
        columns: [
            {
                field: "roomTypeId",
                headerTemplate: $translate.instant('BTA_ROOM_TYPE'),
                width: "100px",
                template: function (dataItem) {
                    // if (dataItem.select) {
                    //     return `<select kendo-drop-down-list style="width: 100%;"
                    //         id="roomTypeId"
                    //         name="roomTypeId"
                    //         k-ng-model="dataItem.roomTypeId"       
                    //         k-data-source="roomTypes"        
                    //         k-options="roomTypeOptions"
                    //         ></select>`;
                    // } else {
                    //     return `<span>{{dataItem.roomTypeName}}</span>`;
                    // }
                    return `<select kendo-drop-down-list style="width: 100%;"
                            id="roomTypeId"
                            name="roomTypeId"
                            k-ng-model="dataItem.roomTypeId"       
                            k-data-source="roomTypes"        
                            k-options="roomTypeOptions"
                            k-on-change="changeCommon('roomType', dataItem)" 
                            ></select>`;
                }
            },
            {
                field: "peoples",
                headerTemplate: $translate.instant('PEOPLE'),
                width: "500px",
                template: function (dataItem) {
                    // if (dataItem.select) {
                    //     return `<select kendo-multi-select 
                    //     id="peopleOption${dataItem.no}" 
                    //     k-data-source="peoples"
                    //     k-options="peopleOption"
                    //     k-ng-model="dataItem.peoples"></select>`
                    // } else {
                    //     return `<label>${dataItem.peopleNames}</label>`
                    // }
                    return `<select kendo-multi-select 
                        id="peopleOption${dataItem.no}" 
                        k-data-source="peoples"
                        k-options="peopleOption"
                        k-ng-model="dataItem.peoples"></select>`
                }
            },
            {
                headerTemplate: $translate.instant('COMMON_ACTION'),
                width: "100px",
                template: function (dataItem) {
                    return `<a class="btn btn-sm default red-stripe " ng-click="deleteRecord('roomGrid', dataItem.no)">${$translate.instant('COMMON_BUTTON_DELETE')}</a>`;
                }
            }
        ]
    }

    arrayRoom = [];
    $scope.addRoom = function () {
        let value = {
            no: $scope.rooms.length + 1,
            roomTypeId: '',
            roomTypeCode: '',
            roomTypeName: '',
            peoples: ''
        }
        let dataGrid = getDataGrid("#roomGrid");
        dataGrid.push(value)
        let count = 1;
        dataGrid.forEach(item => {
            item.no = count;
            count++;
        });
        let id = "#peopleOption" + value.no;
        //mapDataToPeopleOption(getDataGrid("#btaListDetailGrid"), id);
        mapDataToPeopleOption(arrayFilterHotel, id);
        setDataSourceForGrid('#roomGrid', dataGrid);
    }

    let requiredFields = [
        {
            fieldName: "roomTypeId",
            title: $translate.instant('BTA_ROOM_TYPE')
        },
        {
            fieldName: "peoples",
            title: $translate.instant('PEOPLE')
        }
    ];

    $scope.roomTypeOptions = {
        dataTextField: "name",
        dataValueField: "id",
        valuePrimitive: true,
        checkboxes: false,
        autoBind: true,
        filter: "contains",
        dataSource: $scope.roomTypes
    };

    peoplesTotal = [];
    $scope.peopleOption = {
        dataValueField: "businessTripApplicationDetailId",
        dataTextField: "showText",
        headerTemplate: '<div id="headerRoom" class="row">' +
            '<div class="col-md-1">' +
            '<span id="textHeader" style="width: 100%;">SAPCode</span>' +
            '</div>' +
            '<div class="col-md-1">' +
            '<span id="textHeader" style="width: 100%">Full Name</span>' +
            '</div>' +
            '<div class="col-md-2">' +
            '<span id="textHeader" style="width: 100%">Department</span>' +
            '</div>' +
            '<div class="col-md-1">' +
            '<span id="textHeader" style="width: 100%">Gender</span>' +
            '</div>' +
            '<div class="col-md-1">' +
            '<span id="textHeader" style="width: 100%">Arrival</span>' +
            '</div>' +
            '<div class="col-md-2">' +
            '<span id="textHeader" style="width: 100%">From-To Date</span>' +
            '</div>' +
            '<div class="col-md-1">' +
            '<span id="textHeader" style="width: 100%">Check in</span>' +
            '</div>' +
            '<div class="col-md-1">' +
            '<span id="textHeader" style="width: 100%">Check out</span>' +
            '</div>' +
            '<div class="col-md-2">' +
            '<span id="textHeader" style="width: 100%">Hotel</span>' +
            '</div>' +
            '</div>',
        footerTemplate: 'Total #: instance.dataSource.total() # items found',
        template: '<div id="peopleOption#: data.no #_#: data.no #" class="row" style=" max-height:400px">' +
            '<div class="col-md-1">' +
            '<span style="width: 100%">#: data.sapCode #</span>' +
            '</div>' +

            '<div class="col-md-1">' +
            '<span style="width: 100%">#: data.fullName #</span>' +
            '</div>' +

            '<div class="col-md-2">' +
            '<span style="width: 100%">#: data.department #</span>' +
            '</div>' +

            '<div class="col-md-1">' +
            '<span style="width: 100%">#: data.gender #</span>' +
            '</div>' +

            '<div class="col-md-1">' +
            '<span style="width: 100%">#: data.arrivalName # </span>' +
            '</div>' +

            '<div class="col-md-2">' +
            '<span style="width: 100%">#: data.fromtoDate #</span>' +
            '</div>' +

            '<div class="col-md-1">' +
            '<span style="width: 100%">#: data.checkInHotelDate #</span>' +
            '</div>' +

            '<div class="col-md-1">' +
            '<span style="width: 100%">#: data.checkOutHotelDate #</span>' +
            '</div>' +

            '<div class="col-md-2">' +
            '<span style="width: 100%">#: data.hotelName #</span>' +
            '</div>' +

            '</div>',
        height: 400,
        valuePrimitive: true,
        checkboxes: false,
        autoBind: true,
        filter: "contains",
        customFilterFields: ['sapCode', 'fullName', 'department'],
        filtering: filterMultiField,
        dataSource: {
            data: $scope.peoples
        },
        autoClose: false,
        change: function (e) {
            var value = this.value();
            value.forEach(item => {
                peoplesTotal.push(item);
            });
        },
        deselect: function (e) {
            var dataItem = e.dataItem;
            // Use the deselected data item or jQuery item
            let arrayTemporary = [];
            peoplesTotal.forEach(item => {
                if (item != dataItem.businessTripApplicationDetailId) {
                    arrayTemporary.push(item);
                }
            });
            peoplesTotal = arrayTemporary;
        },
        open: function (e) {
            // let gridRoom = $("#roomGrid").data("kendoGrid");
            // if (gridRoom && gridRoom.dataSource._data) {
            //     let selectedPeople = gridRoom.dataSource._data.toJSON();
            //     if (selectedPeople && selectedPeople.length) {
            //         selectedPeople = _.filter(selectedPeople, x => { return x.peoples && x.peoples.length });
            //         let allIds = [];
            //         _.forEach(selectedPeople, x => {
            //             allIds = allIds.concat(x.peoples);
            //         });
            //         if (allIds.length) {
            //             // let peoples = _.filter($scope.peoples, y => {
            //             //     return _.findIndex(allIds, z => { return y.businessTripApplicationDetailId == z }) == -1;
            //             // });
            //             // $timeout(function () {
            //             //     $scope.peoples = peoples;
            //             // })                        
            //         }
            //     }
            // }
            // $scope.model.roomOrganizations.forEach(item => {
            //     peoplesTotal.push(item.businessTripApplicationDetailId);
            // });
            let id = "#" + e.sender.element[0].id;
            let id_list = `${id}-list`;
            $(id_list).attr("style", "width:1300px !important")
            var multiselect = $(id).data("kendoMultiSelect");
            if (multiselect) {
                if (multiselect.dataItems().length > 0) {
                    let arrayTemporary = [];
                    peoplesTotal.forEach(item => {
                        let result = multiselect.dataItems().find(x => x.businessTripApplicationDetailId == item);
                        if (!result) {
                            arrayTemporary.push(item);
                        }
                    });
                    peoplesTotal = arrayTemporary;
                }
                mapDataToPeopleOption(arrayFilterHotel, id);
                multiselect.ul.addClass('hide-selected');
            }

        },
        close: function (e) {
            let id = "#" + e.sender.element[0].id;
            var multiselect = $(id).data("kendoMultiSelect");
            if (multiselect) {
                if (multiselect.dataItems().length > 0) {
                    multiselect.dataItems().forEach(item => {
                        peoplesTotal.push(item.businessTripApplicationDetailId);
                    });
                }
            }
        }
    }

    function getDataGrid(id) {
        let gridRoom = $(id).data("kendoGrid");
        return gridRoom.dataSource._data.toJSON();
    }
    //end


    //popup add user
    $scope.dialogDetailOption = {
        buttonLayout: "normal",
        animation: {
            open: {
                effects: "fade:in"
            }
        },
        schema: {
            model: {
                id: "no"
            }
        },
        actions: [{
            text: $translate.instant('COMMON_BUTTON_REVIEWUSER'),
            action: function (e) {
                $scope.reviewUser();
                $scope.reviewOk = false;
                return false;
            },
            primary: true
        }]
    };
    $scope.dialogDetailReviewOption = {
        buttonLayout: "normal",
        animation: {
            open: {
                effects: "fade:in"
            }
        },
        schema: {
            model: {
                id: "no"
            }
        },
        actions: [{
            text: $translate.instant('COMMON_BUTTON_OK'),
            action: function (e) {
                $scope.isClickReviewUserForChanging = true;
                $scope.reviewOk = true;
                $scope.isShow = false;
                $scope.originalDivision = $scope.model.deptDivisionId;
                $scope.closeReviewDialog(false);
                $scope.allCheck = false;
                $scope.arrayCheck = [];
                let count = 1;
                let dataExist = getDataGrid('#btaListDetailGrid');
                $scope.btaListDetailGridData = dataExist;
                $scope.userGridReview.forEach(item => {
                    let value = {
                        no: count,
                        userId: item.id,
                        sapCode: item.sapCode,
                        fullName: item.fullName,
                        firstName: item.firstName,
                        lastName: item.lastName,
                        departmentName: item.department,
                        departmentCode: item.departmentCode,
                        email: item.email,
                        mobile: item.mobile,
                        idCard: item.idCard,
                        passport: item.passport,
                        dateOfBirth: item.dateOfBirth,
                        passportDateOfIssue: item.passportDateOfIssue,
                        passportExpiryDate: item.passportExpiryDate,
                        isForeigner: false,
                        gender: 1,
                        departureCode: '',
                        departureName: '',
                        departureInfo: null,
                        arrivalCode: '',
                        arrivalName: '',
                        arrivalInfo: null,
                        fromDate: $scope.date.fromDate,
                        toDate: $scope.date.toDate,
                        hotelName: '',
                        hotelCode: '',
                        checkInHotelDate: null,
                        checkOutHotelDate: null,
                        flightNumberCode: '',
                        flightNumberName: '',
                        userGradeValue: item.jobGradeValue,
                        departmentId: item.departmentId,
                        rememberInformation: false,
                    };
                    $scope.btaListDetailGridData.push(value);
                });
                let grid = $('#btaListDetailGrid').data("kendoGrid");
                if (grid) {
                    let i = 1;
                    $scope.btaListDetailGridData.forEach(item => {
                        item.no = i;
                        i++;
                    });
                    grid.setDataSource($scope.btaListDetailGridData);
                    //get hotel 
                    $scope.btaListDetailGridData.forEach(item => {
                        $scope.changeArrival(item, true);
                        if (!item.hotelCode) {
                            disableCheckInCheckOut(item.no);
                        }
                    });
                    //set giới hạn lại time cho 
                    $timeout(function () {
                        $scope.warningWeekBooking = [];
                        $scope.btaListDetailGridData.forEach(x => {
                            $scope.fromDateGridChanged('#toDate', x, true);
                            //flightNumber
                            let flightNumberId = 'flightNumber' + x.no;
                            setDropDownTree(flightNumberId, $scope.flightNumberOption);
                            var flightNumberOption = $("#" + flightNumberId).data("kendoDropDownTree");
                            if (flightNumberOption) {
                                flightNumberOption.value(x.flightNumberCode);
                            }
                            //comeBackFlight
                            let comeBackFlightId = 'comeBackFlight' + x.no;
                            setDropDownTree(comeBackFlightId, $scope.flightNumberOption);
                            var comeBackFlightOption = $("#" + comeBackFlightId).data("kendoDropDownTree");
                            if (comeBackFlightOption) {
                                comeBackFlightOption.value(x.comebackFlightNumberCode);
                            }
                        });

                        if ($scope.warningWeekBooking.length > 0) {
                            $scope.bookingDateTripWeekes = true;
                        }
                        else {
                            $scope.bookingDateTripWeekes = false;
                            $scope.warningWeekBooking = [];
                        }

                    }, 0);
                }
                let dialog = $("#dialog_Detail").data("kendoDialog");
                dialog.close();
                return true;
            },
            primary: true
        }]
    };

    $scope.userGrid = [];
    $scope.userListGridOptions = {
        dataSource: {
            pageSize: 20,
            schema: {
                total: () => { return $scope.total },
                data: () => { return $scope.userGrid }
            }
        },
        sortable: false,
        autoBind: true,
        resizable: true,
        pageable: {
            // pageSize: appSetting.pageSizeWindow,
            alwaysVisible: true,
            pageSizes: [5, 10, 20, 30, 40],
            responsive: false,
            messages: {
                display: "{0}-{1} " + $translate.instant('PAGING_OF') + " {2} " + $translate.instant('PAGING_ITEM'),
                itemsPerPage: $translate.instant('PAGING_ITEM_PER_PAGE'),
                empty: $translate.instant('PAGING_NO_ITEM')
            }
        },
        columns: [
            {
                field: "isCheck",
                title: "<input type='checkbox' ng-model='allCheck' name='allCheck' id='allCheck' class='k-checkbox' ng-change='onChange(allCheck)'/> <label class='k-checkbox-label cbox' for='allCheck' style='padding-bottom: 10px;'></label>",
                width: "50px",
                template: function (dataItem) {
                    return `<input type="checkbox" ng-model="dataItem.isCheck" name="isCheck{{dataItem.no}}"
                    id="isCheck{{dataItem.no}}" class="k-checkbox" style="width: 100%;" ng-change='changeCheck(dataItem)'/>
                    <label class="k-checkbox-label cbox" for="isCheck{{dataItem.no}}"></label>`
                }
            },
            {
                field: "sapCode",
                // title: "SAP Code",
                headerTemplate: $translate.instant('COMMON_SAP_CODE'),
                width: "100px",
            },
            {
                field: "fullName",
                // title: "Full Name",
                headerTemplate: $translate.instant('COMMON_FULL_NAME'),
                width: "200px",
            },
            {
                field: "department",
                // title: "Department",
                headerTemplate: $translate.instant('COMMON_DEPARTMENT'),
                width: "200px",
            },
            {
                field: "position",
                // title: "Position",
                headerTemplate: $translate.instant('COMMON_POSITION'),
                width: "200px",
            },
            {
                field: "jobGrade",
                // title: "Job Grade",
                headerTemplate: $translate.instant('JOBGRADE_MENU'),
                width: "100px",
            },
        ]
    }

    $scope.userGridReview = [];
    $scope.userListGridReviewOptions = {
        dataSource: {
            serverPaging: true,
            pageSize: 10,
            schema: {
                total: () => { return $scope.userGridReview.length },
                data: () => { return $scope.userGridReview }
            }
        },
        sortable: false,
        autoBind: true,
        resizable: true,
        pageable: {
            // pageSize: appSetting.pageSizeWindow,
            alwaysVisible: true,
            pageSizes: [5, 10, 20, 30, 40],
            responsive: false,
            messages: {
                display: "{0}-{1} " + $translate.instant('PAGING_OF') + " {2} " + $translate.instant('PAGING_ITEM'),
                itemsPerPage: $translate.instant('PAGING_ITEM_PER_PAGE'),
                empty: $translate.instant('PAGING_NO_ITEM')
            }
        },
        columns: [
            {
                field: "sapCode",
                // title: "SAP Code",
                headerTemplate: $translate.instant('COMMON_SAP_CODE'),
                width: "100px",
            },
            {
                field: "fullName",
                // title: "Full Name",
                headerTemplate: $translate.instant('COMMON_FULL_NAME'),
                width: "200px",
            },
            {
                field: "department",
                // title: "Department",
                headerTemplate: $translate.instant('COMMON_DEPARTMENT'),
                width: "200px",
            },
            {
                field: "position",
                // title: "Position",
                headerTemplate: $translate.instant('COMMON_POSITION'),
                width: "200px",
            },
            {
                field: "jobGrade",
                // title: "Job Grade",
                headerTemplate: $translate.instant('JOBGRADE_MENU'),
                width: "100px",
            },
        ],
    }

    async function getSAPCode(option) {
        $scope.userGrid = [];
        $scope.limitDefaultGrid = option.data.take;
        let countCheck = 0;
        if ($scope.model.deptDivisionId) {
            await getSapCodeByDivison(option.data.page, option.data.take, $scope.keyWorkTemporary, true);
        } else if ($scope.model.deptLineId && !$scope.model.deptDivisionId) {
            await getUserCheckedHeadCountByDeptLine($scope.model.deptLineId, option.data.take, option.data.page, $scope.keyWorkTemporary);
        }
        // if ($scope.arrayCheck.length > 0) {
        //     $scope.employeesDataSource.forEach(item => {
        //         var result = $scope.arrayCheck.find(x => x.id == item.id);
        //         if (result) {
        //             item.isCheck = true;
        //             countCheck++;
        //         }
        //     });
        // }
        let grid = $("#userGrid").data("kendoGrid");
        if (grid) {
            let count = 1;

            if ($scope.allCheck && $scope.arrayCheck.length == $scope.total) {
                $scope.employeesDataSource.forEach(item => {
                    item['no'] = count;
                    item['isCheck'] = true;
                    $scope.userGrid.push(item);
                    count++;
                    var result = $scope.arrayCheck.find(x => x.id == item.id);
                    if (!result) {
                        $scope.arrayCheck.push(item);
                    }
                });
                countCheck = $scope.arrayCheck.length;
            }
            else {

                $scope.employeesDataSource.forEach(item => {
                    item['no'] = count;
                    item['isCheck'] = false;
                    if ($scope.arrayCheck.length > 0) {
                        var result = $scope.arrayCheck.find(x => x.id == item.id);
                        if (result) {
                            item.isCheck = true;
                        }
                    }
                    $scope.userGrid.push(item);
                    count++;
                });

                if ($scope.model.deptDivisionId) {
                    result = await settingService.getInstance().users.getChildUsers({ departmentId: $scope.model.deptDivisionId, limit: 100000, page: 1, searchText: $scope.gridUser.keyword, isAll: true }).$promise;
                } else {
                    result = await settingService.getInstance().users.getUsersByOnlyDeptLine({ depLineId: $scope.model.deptLineId, limit: 100000, page: 1, searchText: $scope.gridUser.keyword ? $scope.gridUser.keyword.trim() : "" }).$promise;
                }
                result.object.data.forEach(item => {
                    item['isCheck'] = false;
                    if ($scope.arrayCheck.length > 0) {
                        var result = $scope.arrayCheck.find(x => x.id == item.id);
                        if (result) {
                            item.isCheck = true;
                            countCheck++;
                        }
                    }
                });
            }
            if ($scope.total <= $scope.arrayCheck.length) {
                if (countCheck == $scope.total && countCheck > 0) {
                    $scope.allCheck = true;
                }
                else {
                    if ($scope.arrayCheck.length == $scope.total && countCheck == $scope.arrayCheck.length && $scope.arrayCheck.length > 0) {
                        $scope.allCheck = true;
                    }
                    else {
                        $scope.allCheck = false;
                    }
                }
            } else {
                $scope.allCheck = false;
            }
        }
        option.success($scope.userGrid);
        grid.refresh();
    }

    $scope.employeesDataSource = [];
    async function getSapCodeByDivison(page, limit, searchText = "", isAll = false) {
        if ($scope.model.deptDivisionId) {
            let result = await settingService.getInstance().users.getChildUsers({ departmentId: $scope.temporaryDivision, limit: limit, page: page, searchText: searchText, isAll: isAll }).$promise;
            if (result.isSuccess) {
                $scope.employeesDataSource = result.object.data.map(function (item) {
                    return { ...item, showtextCode: item.sapCode }
                });
                $scope.total = result.object.count;
            }
        }
    }

    async function getUserCheckedHeadCount(deptId, textSearch = "") {
        let result = await settingService.getInstance().users.getUserCheckedHeadCount({ departmentId: deptId, textSearch: textSearch }).$promise;
        if (result.isSuccess) {
            $scope.employeesDataSource = result.object.data.map(function (item) {
                return { ...item, showtextCode: item.sapCode }
            });
            $scope.total = result.object.count;
        }
    }
    async function getUserCheckedHeadCountByDeptLine(deptId, limit, page, textSearch = "") {
        let result = await settingService.getInstance().users.getUsersByOnlyDeptLine({ depLineId: deptId, limit: limit, page: page, searchText: textSearch }).$promise;
        if (result.isSuccess) {
            $scope.employeesDataSource = result.object.data.map(function (item) {
                return { ...item, showtextCode: item.sapCode }
            });
            $scope.total = result.object.count;
        }
    }

    $scope.arrayCheck = [];
    $scope.userGridReview = [];
    $scope.gridUser = {
        keyword: '',
    }
    $scope.addItemsUser = async function (model) {
        $scope.keyWorkTemporary = '';
        $scope.limitDefaultGrid = 20;
        $scope.userGrid = [];
        $scope.arrayCheck = [];
        $scope.userGridReview = [];
        $scope.allCheck = false;
        $scope.isShow = false;
        $scope.gridUser.keyword = '';
        $scope.employeesDataSource = [];
        let grid = $('#userGrid').data("kendoGrid");
        grid.dataSource.data($scope.employeesDataSource);
        setDefaultDateItem();
        $scope.searchGridUser();
        // set title cho cái dialog
        let dialog = $("#dialog_Detail").data("kendoDialog");
        dialog.title($translate.instant('COMMON_BUTTON_ADDUSER'));
        dialog.open();
        $rootScope.confirmDialogAddItemsUser = dialog;
    }

    function setDefaultDateItem() {
        $scope.date = {
            fromDate: new Date(),
        }
        $scope.date.fromDate.setHours(13, 00);
        let datePicker = $('#toDate').data('kendoDateTimePicker');
        if (datePicker) {
            datePicker.min(new Date($scope.date.fromDate));
        }
    }
    $scope.keyWorkTemporary = '';
    $scope.allCheck = false;
    $scope.searchGridUser = async function () {
        $scope.userGrid = [];
        $scope.keyWorkTemporary = $scope.gridUser.keyword;
        let grid = $("#userGrid").data("kendoGrid");
        page = grid.pager.dataSource._page;
        pageSize = grid.pager.dataSource._pageSize;
        if ($scope.gridUser.keyword != null) {
            let result = {};
            if ($scope.model.deptDivisionId) {
                result = await settingService.getInstance().users.getChildUsers({ departmentId: $scope.model.deptDivisionId, limit: 100000, page: 1, searchText: $scope.gridUser.keyword }).$promise;
            } else {
                result = await settingService.getInstance().users.getUsersByOnlyDeptLine({ depLineId: $scope.model.deptLineId, limit: 100000, page: 1, searchText: $scope.gridUser.keyword ? $scope.gridUser.keyword.trim() : "" }).$promise;
            }
            if (result.isSuccess) {
                let dataFilter = result.object.data.map(function (item) {
                    return { ...item, showtextCode: item.sapCode }
                });
                $scope.total = result.object.count;
                let count = 0;
                let countCheck = 0;
                dataFilter.forEach(item => {
                    item['no'] = count;
                    item['isCheck'] = false;
                    if ($scope.arrayCheck.length > 0) {
                        var result = $scope.arrayCheck.find(x => x.id == item.id);
                        if (result) {
                            item.isCheck = true;
                            countCheck++;
                        }
                    }
                    $scope.userGrid.push(item);
                    count++;
                });
                if ($scope.total) {
                    if (countCheck == $scope.total) {
                        $scope.allCheck = true;
                    }
                    else {
                        if ($scope.arrayCheck.length == $scope.total && countCheck == $scope.userGrid.length) {
                            $scope.allCheck = true;
                        }
                        else {
                            $scope.allCheck = false;
                        }
                    }
                } else {
                    $scope.allCheck = false;
                }
                setGridUser(dataFilter, '#userGrid', $scope.total, 1, $scope.limitDefaultGrid);
            }
        }
        else {
            let count = 0;
            let countCheck = 0;
            $scope.userGrid.forEach(item => {
                item['no'] = count;
                item['isCheck'] = false;
                if ($scope.arrayCheck.length > 0) {
                    var result = $scope.arrayCheck.find(x => x.id == item.id);
                    if (result) {
                        item.isCheck = true;
                        countCheck++;
                    }
                }
                count++;
            });
            if (countCheck == $scope.limitDefaultGrid) {
                $scope.allCheck = true;
            }
            setGridUser($scope.userGrid, '#userGrid', $scope.total, 1, $scope.limitDefaultGrid);
        }
    }

    function setGridUser(data, idGrid, total, pageIndex, pageSizes) {
        let grid = $(idGrid).data("kendoGrid");
        if (idGrid == '#userGrid') {
            let dataSource = new kendo.data.DataSource({
                transport: {
                    read: async function (e) {
                        await getSAPCode(e);
                    }
                },
                pageSize: pageSizes,
                page: pageIndex,
                serverPaging: true,
                schema: {
                    total: function () {
                        return total;
                    }
                },
            });
            if (grid) {
                grid.setDataSource(dataSource);
            }
        }
        // if (idGrid == '#userGridReview') {
        else {
            let dataSource = new kendo.data.DataSource({
                data: data,
                pageSize: pageSizes,
                page: 1,
                schema: {
                    total: function () {
                        return total;
                    }
                },
            });
            if (grid) {
                grid.setDataSource(dataSource);
            }

        }
    }
    function continueWorkflowNotSave(res) {
        workflowService.getInstance().workflows.vote(voteModel).$promise.then(function (result) {
            if (result.messages.length == 0) {
                // Notification.success("Workflow has been processed");
                Notification.success($translate.instant('COMMON_WORKFLOW_PROCESSED'));
                $state.go($state.current.name, { id: res.object.id, referenceValue: res.object.referenceNumber }, { reload: true });
            } else {
                Notification.error(result.messages[0]);
            }
        });
    }


    $scope.changeCheck = async function (dataItem) {
        if ($scope.allCheck) {
            $scope.allCheck = false;
        }
        $scope.userGrid.forEach(item => {
            if (item.id == dataItem.id) {
                if (item.isCheck) {
                    item.isCheck = false;
                    var arrayTemporary = $scope.arrayCheck.filter(x => x.id != item.id);
                    $scope.arrayCheck = arrayTemporary;
                }
                else {
                    item.isCheck = true;
                    $scope.arrayCheck.push(item);
                }
            }
        });
        if ($scope.keyWorkTemporary) {
            let arrayCheckTemporary = [];
            let grid = $("#userGrid").data("kendoGrid");
            pageSize = grid.pager.dataSource._pageSize;
            if (pageSize < $scope.total) {
                if ($scope.model.deptDivisionId) {
                    await getSapCodeByDivison(1, 100000, $scope.keyWorkTemporary, true);
                } else if ($scope.model.deptLineId && !$scope.model.deptDivisionId) {
                    await getUserCheckedHeadCountByDeptLine($scope.model.deptLineId, 10000, 1, $scope.keyWorkTemporary);
                }
            }
            $scope.employeesDataSource.forEach(x => {
                let result = $scope.arrayCheck.find(y => y.id == x.id);
                if (result) {
                    arrayCheckTemporary.push(result);
                }
            });

            if (arrayCheckTemporary.length == $scope.total) {
                $timeout(function () {
                    $scope.allCheck = true;
                }, 0);
            }
        }
        else {
            if ($scope.arrayCheck.length == $scope.total) {
                $scope.allCheck = true;
            }
        }
    }

    $scope.onChange = async function (isCheckAll) {
        let count = 1;
        if ($scope.model.deptDivisionId) {
            await getSapCodeByDivison(1, 100000, $scope.keyWorkTemporary, true);
        } else if ($scope.model.deptLineId && !$scope.model.deptDivisionId) {
            await getUserCheckedHeadCountByDeptLine($scope.model.deptLineId, 10000, 1, $scope.keyWorkTemporary);
        }
        if (isCheckAll) {
            $scope.employeesDataSource.forEach(item => {
                item['no'] = count;
                item['isCheck'] = true;
                count++;
                var result = $scope.arrayCheck.find(x => x.id == item.id);
                if (!result) {
                    $scope.arrayCheck.push(item);
                }
            });
            $scope.userGrid.forEach(item => {
                item.isCheck = true;
            });
            $scope.allCheck = true;
        }
        else {
            $scope.employeesDataSource.forEach(item => {
                item.isCheck = false;
            });
            $scope.userGrid.forEach(item => {
                item.isCheck = false;
            });
            let arrayTemporary = [];
            $scope.arrayCheck.forEach(item => {
                var result = $scope.employeesDataSource.find(x => x.id == item.id);
                if (!result) {
                    arrayTemporary.push(item);
                }
            });
            $scope.allCheck = false;
            $scope.arrayCheck = arrayTemporary;
        }
        let grid = $("#userGrid").data("kendoGrid");
        page = grid.pager.dataSource._page;
        pageSize = grid.pager.dataSource._pageSize;
        setGridUser($scope.userGrid, '#userGrid', $scope.total, page, pageSize);
    }

    $scope.backPopupReview = function () {
        let dialog = $("#dialog_Detail_Review").data("kendoDialog");
        dialog.close();
        $scope.isWarningMessage = false;
        $scope.closeReviewDialog(true);
    }

    $scope.reviewUser = async function () {
        $scope.userGridReview = [];
        $scope.gridUserReview.keyword = '';
        let dialog_Detail = $("#dialog_Detail").data("kendoDialog");
        dialog_Detail.close();
        let passengerSAPCodes = $scope.arrayCheck.map(item => {
            if (item.isCheck) {
                return item.sapCode;
            }
        });

        let getPassengerInfoResult = await btaService.getInstance().bussinessTripApps.getPassengerInformationBySAPCodes(null, passengerSAPCodes).$promise;
        let passengerInfoResult = new Array();
        if (getPassengerInfoResult.isSuccess && !$.isEmptyObject(getPassengerInfoResult.object)) {
            passengerInfoResult = getPassengerInfoResult.object;
        }

        let findPassengerInfo = function (sapCode) {
            let passengerInfos = passengerInfoResult.filter(cPassenger => {
                if (cPassenger.sapCode == sapCode) {
                    return cPassenger;
                }
            });
            return !$.isEmptyObject(passengerInfos) && passengerInfos.length > 0 ? passengerInfos[0] : null;
        }

        $scope.arrayCheck.forEach(item => {
            if (item.isCheck) {
                let passengerInfo = findPassengerInfo(item.sapCode);
                if ($.isEmptyObject(passengerInfo)) {
                    let userFullName = item.fullName;
                    let nameElements = userFullName.split(' ');
                    passengerInfo = {
                        lastName: nameElements.shift(),
                        firstName: nameElements.join(' '),
                        email: item.email,
                        mobile: "",
                        idCard: "",
                        passport: "",
                        dateOfBirth: "",
                        passportDateOfIssue: "",
                        passportExpiryDate: "",
                    };
                }
                delete passengerInfo.id;
                item = $.extend(item, passengerInfo);
                $scope.userGridReview.push(item);
            }
        });

        setGridUser($scope.userGridReview, '#userGridReview', $scope.userGridReview.length, 1, $scope.limitDefaultGrid);
        let dialog = $("#dialog_Detail_Review").data("kendoDialog");
        dialog.title("");
        angular.element('.k-dialog-title').append($compile(`
        <div class="row">
            <div class="col-md-4" style="margin-top: -25px;">
                <div class="form-group">
                        
                    <div class="col-md-7" style="margin-left: -10px;">
                        <i class="k-icon k-i-arrow-chevron-left" style="margin-left: -10px;" ng-click="backPopupReview()"></i><span style="padding-left: 10px;color: #b42e83 !important;font-family: Nova-Regular !important;font-size: 18px !important;font-weight: 700; margin: 10px 0;margin: 0;=line-height: 1.42857143;=text-transform: uppercase !important;">${$translate.instant('COMMON_BUTTON_REVIEWUSER')}</span>
                    </div>
                </div>
            </div>
        </div>
        `)($scope)
        )
        dialog.open();
        $rootScope.confirmDialogAddItemsUser = dialog;
    }

    $scope.fromDateChanged = function () {
        if ($scope.date.fromDate) {

            $('#toDate').data('kendoDateTimePicker').min(new Date($scope.date.fromDate));
            var resultDayToDate = dateFns.getDayOfYear($scope.date.toDate);
            var resultDayFromDate = dateFns.getDayOfYear($scope.date.fromDate);
            var resultHourToDate = dateFns.getHours($scope.date.toDate);
            var resultHourFromDate = dateFns.getHours($scope.date.fromDate)
            if ($scope.date.toDate && resultDayToDate <= resultDayFromDate && resultHourToDate <= resultHourFromDate) {
                $scope.date.toDate = $scope.date.fromDate;
            }

            var newDate = new Date($scope.date.fromDate);
            newDate.setDate(newDate.getDate() - 10);
            var currentDate = new Date().getTime();
            if (newDate.getTime() > currentDate) {
                $scope.isWarningMessage = true;
            } else {
                $scope.isWarningMessage = false;
            }
        }
    };


    $scope.closeReviewDialog = function (openDialog_Detail) {
        $scope.dialogVisible = false;
        if (openDialog_Detail && !$scope.reviewOk) {
            let dialog = $("#dialog_Detail").data("kendoDialog");
            dialog.title($translate.instant('COMMON_BUTTON_ADDUSER'));
            dialog.open();
        }
    }

    $scope.gridUserReview = {
        keyword: ''
    }
    $scope.searchGridUserReview = function () {
        let grid = $("#userGridReview").data("kendoGrid");
        pageSize = grid.pager.dataSource._pageSize;
        if ($scope.gridUserReview.keyword) {
            let dataTemporary = _.cloneDeep($scope.userGridReview);
            let dataFilter = dataTemporary.filter(item => {
                if (item.fullName.toLowerCase().includes($scope.gridUserReview.keyword.toLowerCase())) {
                    return item;
                }
            });
            setGridUser(dataFilter, '#userGridReview', dataFilter.length, 1, pageSize);
        }
        else {
            setGridUser($scope.userGridReview, '#userGridReview', $scope.userGridReview.length, 1, pageSize)
        }
    }
    $scope.ifEnter = function ($event, dialog) {
        var keyCode = $event.which || $event.keyCode;
        if (keyCode === 13) {
            if (dialog == 'review') {
                $scope.searchGridUserReview();
            } else {
                $scope.searchGridUser();
            }
        }
    }
    $scope.showProcessingStages = function () {
        $rootScope.visibleProcessingStages($translate);
    }
    $rootScope.$watch(function () { return dataService.permission }, function (newValue, oldValue) {

        if ($stateParams.id) {
            $scope.perm = (2 & dataService.permission.right) == 2;
        } else {
            $scope.perm = true;
        }

    }, true);
    $scope.$on('Room Organization', function (event, data) {
        $scope.errors = $scope.checkHotelInfo();
        if ($scope.errors.length == 0) {
            voteModel = data.voteModel;
            $scope.setRooms();
        }
    });
    $scope.$on('Confirm', async function (event, data) {
        voteModel = data.voteModel;
        let res = await $scope.save();
        await continueWorkFlow(res);
        //continueWorkflowNotSave($scope.model.id);
    });
    //
    $scope.$on('workflowStatus', async function (event, data) {
        if (data.isCustomEvent) {
            //enableElementToEdit();
            await getFlightNumbers(true);
        }
        if (data.approveFieldText && (data.approveFieldText == "Change Business Trip" || data.approveFieldText.toLowerCase() == "Change Business Trip".toLowerCase())) {
            if (data.isCustomEvent && data.customEventKey === 'chaningBookingFlight') {
                $scope.isEditViewTicket = true;
                $scope.isBookingFlightWF = true;
                setTimeout(function () {
                    let dataDetail = getDataGrid('#btaListDetailGrid');
                    dataDetail.forEach(item => {
                        let fromDateId = '#fromDate' + item.no;
                        let toDateId = '#toDate' + item.no;
                        $(fromDateId).attr("disabled", "disabled");
                        $(toDateId).attr("disabled", "disabled");
                    });
                }, 200);
            }
            $scope.forceChangeBusinessTrip = true;

            setTimeout(function () {
                enableGridChangingCancelling($scope.model.changeCancelBusinessTripDetails);
            }, 200);

            /*$timeout(function () {
                enableGridChangingCancelling($scope.model.changeCancelBusinessTripDetails);
            }, 200);*/
        }
        if (data.isAttachmentFile) {
            $timeout(function () {
                //$("#attachmentChanging").removeAttr("disabled").removeAttr("readonly");
                //$("#attachmentDetail").removeAttr("disabled").removeAttr("readonly");
                let disableAttachmentItems = ['attachmentDetail', 'attachmentChanging'];
                let enableItems = [];
                if (data.restrictedProperties && data.restrictedProperties.length) {
                    data.restrictedProperties.forEach(x => {
                        enableItems.push(x.fieldPattern);
                        $(`#${x.fieldPattern}`).removeAttr("disabled").removeAttr("readonly");
                        $(`.delete${x.fieldPattern}`).removeClass("display-none");
                    })
                }
                if (!enableItems.length) {
                    $timeout(function () {
                        $("#attachmentDetail").attr("disabled", "disabled");
                        $("#attachmentChanging").attr("disabled", "disabled");
                        $(`.deleteattachmentDetail`).addClass("display-none");
                        $(`.deleteattachmentChanging`).addClass("display-none");
                    }, 0);
                } else if (enableItems.length) {
                    disableAttachmentItems.forEach(x => {
                        if (_.findIndex(enableItems, y => { return y == x }) == -1) {
                            $(`#${x}`).attr("disabled", "disabled");
                            $(`.deleteatt${x}`).addClass("display-none");
                        }
                    })
                }
            }, 0)
            $timeout(function () {
                enableGridChangingCancelling($scope.model.changeCancelBusinessTripDetails, true);
            }, 0);
        } else {
            $timeout(function () {
                $("#attachmentDetail").attr("disabled", "disabled");
                $("#attachmentChanging").attr("disabled", "disabled");
                $(`.deleteattachmentDetail`).addClass("display-none");
                $(`.deleteattachmentChanging`).addClass("display-none");
            }, 0);
        }
        $timeout(function () {
            if (data.workflowInstances.length) {
                customCssFieldWhenDisable();
            }
            disableCheckInOutAllGrid();
        }, 0);
        $timeout(function () {
            if (data && data.allowToVote && $scope.model.isRequestToChange && $scope.model.isRequestToChange == true && $scope.model.status.toLowerCase() == 'Waiting For Admin Checker'.toLowerCase()) {
                disableElement(true);
                $("#attachmentDetail").removeAttr("disabled");
            }
        }, 0);
        //View Booking Flights
        if (data.isCustomEvent && (data.customEventKey === 'approvalOverBudget' || data.customEventKey === 'viewBookingFlights')) {
            if ($("#viewBookingFlights").has("a").length === 0) {
                $scope.isViewMode = true;
                let btnViewBookingFlightsName = $translate.instant('BTA_VIEW_BOOKING_FLIGHT');
                $('#viewBookingFlights').append($compile("<a class='btn btn-sm btn-primary btn-add-user' id='btn_viewBookingFlights' ng-click='popupToViewBookingFlights(\"" + $scope.model.id + "\")' style='background-color: #b42e83'>" + btnViewBookingFlightsName + "</a>")($scope));
            }
        }
        //Enable reason column in revoke step
        if (data.isCustomEvent && data.customEventKey === 'revokeBookingFlight') {
            $scope.revokeClickFirstTime = true;
            //$scope.isEditViewTicket = true;
            $scope.model.changeCancelBusinessTripDetails.forEach(item => {
                let reasonId = '#reason' + item.no;
                $(reasonId).removeAttr("disabled");
            });
        }
        // Add Cancellation Fee
        if (data.isCustomEvent && data.customEventKey === 'addCancellationFee') {
            //disable and enable column in Admin Checker step
            var changingGrid = $("#btaListChangeOrCancelGrid").data("kendoGrid");
            var cancellationColumns = $scope.btaListChangeOrCancelGridColumnsCustom;
            if (cancellationColumns.length == 14) {
                cancellationColumns.splice(cancellationColumns.length - 1, 0, {
                    field: "penaltyFee",
                    headerTemplate: $translate.instant('BTA_CANCELLATION_FEE'),
                    width: "150px",
                    template: function (dataItem) {
                        return `<input kendo-numeric-text-box k-min="0" id="penaltyFee${dataItem.no}" name="penaltyFee" k-ng-model="dataItem.penaltyFee" style="width: 100%;"/>`
                    }
                });
            }

            changingGrid.setOptions({
                columns: cancellationColumns
            });
            //disable fields
            setGridChangeCancelValue();
            setDataSourceForGrid('#btaListChangeOrCancelGrid', $scope.btaRevokeListUsers);
            setTimeout(function () {
                enableGridChangingCancelling($scope.btaRevokeListUsers);
            }, 200);

            if ($scope.btaRevokeListUsers.length > 0) {
                $scope.btaRevokeListUsers.forEach(item => {
                    //item["penaltyFee"] = "";
                    var reasonId = $("#reason" + item.no);
                    reasonId.attr("disabled", "disabled");

                    var isCancelId = $("isCancel" + item.no);
                    isCancelId.attr("disabled", "disabled");
                });
            }
        }

        //Change/Cancel Admin Checker Step
        if (data.isCustomEvent && data.customEventKey === 'addChangingCancellationFee') {
            $scope.isChangingAdminCheckerStep = true;
        }
        setGridChangeCancelValue();
    });
    function enableGridChangingCancelling(dataGrid, isChecker = false) {
        if (dataGrid && dataGrid.length) {
            dataGrid.forEach(item => {
                if (isChecker) {
                    if (item.isCancel) {
                        setValueEnable(item, false);
                    }
                    else {
                        if ($scope.isChangingAdminCheckerStep) {
                            setValueEnable(item, false);
                        }
                    }
                }
                else {
                    if (item.isCancel) {
                        setValueEnable(item, false);
                    }
                    else {
                        setValueEnable(item, true);
                    }
                }
            });
        }
    }

    function customCssFieldWhenDisable() {
        let dataDetail = getDataGrid('#btaListDetailGrid');
        dataDetail.forEach(item => {
            let fromDateId = 'fromDate' + item.no;
            let toDateId = 'toDate' + item.no;
            let checkInHotelId = 'checkInHotelDate' + item.no;
            let checkOutHotelId = 'checkOutHotelDate' + item.no;
            document.getElementById(fromDateId).style.color = 'black';
            document.getElementById(toDateId).style.color = 'black';
            document.getElementById(checkInHotelId).style.color = 'black';
            document.getElementById(checkOutHotelId).style.color = 'black';
        });
        let dataChanging = getDataGrid('#btaListChangeOrCancelGrid');
        if (dataChanging.length) {
            dataChanging.forEach(item => {
                let fromDateCancelId = 'fromDateCancel' + item.no;
                let toDateCancelId = 'toDateCancel' + item.no;
                let newCheckInHotelDateId = 'newCheckInHotelDate' + item.no;
                let newCheckOutHotelDateId = 'newCheckOutHotelDate' + item.no;
                document.getElementById(fromDateCancelId).style.color = 'black';
                document.getElementById(toDateCancelId).style.color = 'black';
                document.getElementById(newCheckInHotelDateId).style.color = 'black';
                document.getElementById(newCheckOutHotelDateId).style.color = 'black';
            });
        }
    }

    $scope.$watch('model.deptDivisionId', function (newValue, oldValue) {
        const firstValue = $scope.originalDivision;
        if (firstValue) {
            $scope.temporaryDivision = firstValue;
        }
        else if (newValue) {
            $scope.temporaryDivision = newValue;
        }
    });
    function getHotelDetailsFromCode(hotelCode) {
        let result = '';
        let hotel = _.find($scope.allHotels, x => {
            return x.code == hotelCode;
        });
        if (hotel) {
            result = `${hotel.name}`;
            if (hotel.address) {
                result = result + '-' + hotel.address;
            }
            if (hotel.telephone) {
                result = result + '-' + hotel.telephone;
            }
        }
        return result;
    }
    $scope.$on('Change Business Trip', function (event, data) {
        $scope.errorChangings = [];
        var result = { isSuccess: false };
        if ($scope.revokeClick) {
            Notification.error($translate.instant('BTA_IS_REVOKE_CLICK_ERROR_MESSAGE'));
            return;
        }
        $scope.errorChangings = validateChangeBusinessTrip();
        if ($scope.errorChangings.length > 0) {
            return result;
        } else {
            //$scope.errorChangings = validateCancelDetail();

            if (!$scope.errorChangings.length) {
                $scope.dialog = $rootScope.showConfirmDelete($translate.instant('BUSINESS_CHANGE_TRIP_APPLICATION'), $translate.instant('BTA_CHANGE_NOTIFY'), $translate.instant('COMMON_BUTTON_CONFIRM'));
                $scope.dialog.bind("close", function (e) {
                    if (e.data && e.data.value) {
                        /*$scope.save().then(async res => {
                            if (res && res.isSuccess) {
                                voteModel = data.voteModel;
                                if ($scope.forceChangeBusinessTrip) {
                                    await continueWorkflowNotSave(res);
                                } else {
                                    await changeWorkflow(data.voteModel, res);
                                    voteModel.vote = 1;
                                    await continueWorkflowNotSave(res);
                                }
                                Notification.success($translate.instant('COMMON_SAVE_SUCCESS'));
                            }
                        });*/

                        $scope.save().then(async res => {
                            if (res && res.isSuccess) {
                                voteModel = data.voteModel;
                                if ($scope.forceChangeBusinessTrip) {
                                    await continueWorkflowNotSave(res);
                                } else {
                                    await changeWorkflow(data.voteModel, res);
                                    voteModel.vote = 1;
                                    await continueWorkflowNotSave(res);
                                }
                                Notification.success($translate.instant('COMMON_SAVE_SUCCESS'));
                            }
                            else {
                                Notification.success($translate.instant(res.message));
                            }
                        });
                    }
                });
            }
        }
    });

    async function changeWorkflow(voteModel, model) {
        var result = await workflowService.getInstance().workflows.startWorkflow(voteModel, null).$promise;
        if (result.messages.length == 0) {
            Notification.success($translate.instant('COMMON_WORKFLOW_STARTED'));
            $state.go($state.current.name, { id: model.object.id, referenceValue: model.object.referenceNumber }, { reload: true });
        } else {
            Notification.error(result.messages[0]);
        }
    }
    validateChangeBusinessTrip = function () {
        let errors = [];
        let currentBTAListChangeOrCancel = $("#btaListChangeOrCancelGrid").data("kendoGrid").dataSource._data;
        let currentBTAListDetail = $("#btaListDetailGrid").data("kendoGrid").dataSource._data;

        if (currentBTAListChangeOrCancel.length > 0) {
            for (let i in currentBTAListChangeOrCancel) {
                let item = currentBTAListChangeOrCancel[i];
                Object.keys(item).forEach(function (fieldName) {
                    let propValue = item[fieldName];
                    switch (fieldName) {
                        case 'destinationCode':
                            if (!propValue && !$scope.isBookingFlightWF) {
                                errors.push({
                                    fieldName: 'List Changing/ Cancelling Business Trip',
                                    controlName: $translate.instant('BTA_DESTINATION'),
                                    errorDetail: $translate.instant('COMMON_FIELD_IS_REQUIRED'),
                                    isRule: false,
                                });
                            }
                            break;
                        case 'sapCode':
                            currentBTAListDetail.filter(x => {
                                if (propValue == x.sapCode && !$scope.isBookingFlightWF) {
                                    if (item['fromDate'] == x.fromDate && item['toDate'] == x.toDate && item['destinationCode'] == x.arrivalCode && item['newHotelCode'] == x.hotelCode && item['newFlightNumberCode'] == x.flightNumberCode && item['newComebackFlightNumberCode'] == x.comebackFlightNumberCode && !item['isCancel']) {
                                        errors.push({
                                            //fieldName: 'List Changing/ Cancelling Business Trip',
                                            controlName: $translate.instant('COMMON_ROW_NO') + ` ${item['no']}`,
                                            errorDetail: $translate.instant('BTA_NOT_CHANGE_ANY_TIMELINE'),
                                            isRule: false,
                                        });
                                    }
                                }
                            });
                        case 'fromDate':
                            if (!propValue && !$scope.isBookingFlightWF) {
                                errors.push({
                                    controlName: $translate.instant('BTA_TRIP_FROM_DATE') + ` ${item['no']}`,
                                    errorDetail: $translate.instant('COMMON_FIELD_IS_REQUIRED'),
                                    isRule: false,
                                });
                            }
                            break;
                        case 'toDate':
                            if (!propValue && !$scope.isBookingFlightWF) {
                                errors.push({
                                    controlName: $translate.instant('BTA_TRIP_TO_DATE') + ` ${item['no']}`,
                                    errorDetail: $translate.instant('COMMON_FIELD_IS_REQUIRED'),
                                    isRule: false,
                                });
                            }
                            else {
                                let err = checkDuplicateFromToDate(item);
                                if (!_.isEmpty(err)) {
                                    errors.push({
                                        controlName: err[0].controlName,
                                        errorDetail: err[0].message.replace(":", ""),
                                        isRule: false,
                                    })
                                }
                            }
                            break;
                        case 'newCheckInHotelDate':
                            if (item.newHotelCode) {
                                if (!propValue) {
                                    errors.push({
                                        controlName: $translate.instant('BTA_NEW_CHECK_IN_HOTEL_DATE') + ` ${item['no']}`,
                                        errorDetail: $translate.instant('COMMON_FIELD_IS_REQUIRED'),
                                        isRule: false,
                                    });
                                }
                            }
                            break;
                        case 'newCheckOutHotelDate':
                            if (item.newHotelCode) {
                                if (!propValue) {
                                    errors.push({
                                        controlName: $translate.instant('BTA_NEW_CHECK_OUT_HOTEL_DATE') + ` ${item['no']}`,
                                        errorDetail: $translate.instant('COMMON_FIELD_IS_REQUIRED'),
                                        isRule: false,
                                    });
                                }
                                else {
                                    let err = checkDuplicateNewCheckInOutHotel(item);
                                    if (!_.isEmpty(err)) {
                                        errors.push({
                                            controlName: err[0].controlName,
                                            errorDetail: err[0].message.replace(":", ""),
                                            isRule: false,
                                        })
                                    }
                                }
                            }
                            break;
                        case 'reason':
                            if (!propValue && !item['isCancel']) {
                                errors.push({
                                    controlName: $translate.instant('COMMON_REASON') + ` ${item['no']}`,
                                    errorDetail: $translate.instant('COMMON_FIELD_IS_REQUIRED'),
                                    isRule: false,
                                });
                            }
                            break;
                    }
                });
            }
            if (!$scope.isBookingFlightWF) {
                currentBTAListChangeOrCancel.forEach(item => {
                    //check duplicate check In/Out Date
                    //errors = errors.concat(checkDuplicateNewCheckInOutGridChanging(item));
                    //check from-to date trung với grid detail
                    errors = errors.concat(checkDuplicateFromtoDateGridChanging(item, currentBTAListDetail));
                });
            }
        }
        else {// dòng rỗng
            errors.push({
                fieldName: $translate.instant('BTA_LIST_CHANGING_CACELLING_BUSNESS_TRIP'),
                controlName: $translate.instant('BTA_CHANGING_CACELLING_BUSNESS_TRIP'),
                errorDetail: $translate.instant('COMMON_FIELD_IS_REQUIRED'),
                isRule: false,
            });
        }
        return errors;
    }

    function checkDuplicateFromtoDateGridChanging(item, dataGrid) {
        let errors = [];
        dataGrid.forEach(x => {
            if (x.id != item.businessTripApplicationDetailId && x.sapCode == item.sapCode) {
                if (moment(x.fromDate).format(appSetting.longDateFormatAMPM) == moment(item.fromDate).format(appSetting.longDateFormatAMPM)) {
                    errors.push({
                        controlName: $translate.instant('BTA_FROM_DATE_ROW') + ' ' + ` ${item['no']}`,
                        errorDetail: moment(item.fromDate).format(appSetting.longDateFormatAMPM) + ' ' + $translate.instant('BTA_DUPLICATE_DATE'),
                        isRule: false,
                    });
                }
                if (moment(x.toDate).format(appSetting.longDateFormatAMPM) == moment(item.toDate).format(appSetting.longDateFormatAMPM)) {
                    errors.push({
                        controlName: $translate.instant('BTA_TO_DATE_ROW') + ' ' + ` ${item['no']}`,
                        errorDetail: moment(item.toDate).format(appSetting.longDateFormatAMPM) + ' ' + $translate.instant('BTA_DUPLICATE_DATE'),
                        isRule: false,
                    });
                }
            }
        });
        return errors;
    }
    function disableFieldsForChanging(item) {
        var fromDateCancelId = $("#fromDateCancel" + item.no);
        var toDateCancelId = $("#toDateCancel" + item.no);
        fromDateCancelId.attr("disabled", "disabled");
        toDateCancelId.attr("disabled", "disabled");

        var destinationId = $("#destination" + item.no);
        var flightNumberId = $("#newFlightNumberCode" + item.no);
        var comebackFlightNumber = $("#newComebackFlightNumberCode" + item.no);
        destinationId.attr("disabled", "disabled");
        flightNumberId.attr("disabled", "disabled");
        comebackFlightNumber.attr("disabled", "disabled");
    }
    function setValueEnable(item, value) {
        let fromDateCancelId = '#fromDateCancel' + item.no;
        let toDateCancelId = '#toDateCancel' + item.no;
        let destinationId = '#destination' + item.no;
        let hotelId = '#hotelChange' + item.no;
        let newCheckInHotelDateId = '#newCheckInHotelDate' + item.no;
        let newCheckOutHotelDateId = '#newCheckOutHotelDate' + item.no;
        let flightNumberId = '#newFlightNumberCode' + item.no;
        let comebackFlightNumber = '#newComebackFlightNumberCode' + item.no;
        let isCancelId = "#isCancel" + item.no;
        let reasonId = '#reason' + item.no;

        if (!$scope.isBookingFlightWF) {
            if ($(fromDateCancelId).data("kendoDateTimePicker")) {
                //datetime
                $(fromDateCancelId).data("kendoDateTimePicker").enable(value);
                $(toDateCancelId).data("kendoDateTimePicker").enable(value);
                $(newCheckInHotelDateId).data("kendoDateTimePicker").enable(value);
                $(newCheckOutHotelDateId).data("kendoDateTimePicker").enable(value);
                //dropdown list
                $(destinationId).data("kendoDropDownList").enable(value);
                $(hotelId).data("kendoDropDownList").enable(value);
                $(flightNumberId).data("kendoDropDownList").enable(value);
                $(comebackFlightNumber).data("kendoDropDownList").enable(value);
            }
        }
        else {
            if ($scope.isChangingAdminCheckerStep) {
                value = false;
                $(isCancelId).attr("disabled", "disabled");
                $(reasonId).attr("disabled", "disabled");
            }
            $(fromDateCancelId).data("kendoDateTimePicker").enable(false);
            $(toDateCancelId).data("kendoDateTimePicker").enable(false);
            $(newCheckInHotelDateId).data("kendoDateTimePicker").enable(value);
            $(newCheckOutHotelDateId).data("kendoDateTimePicker").enable(value);
            //dropdown list
            $(destinationId).data("kendoDropDownList").enable(false);
            $(hotelId).data("kendoDropDownList").enable(value);
            $(flightNumberId).data("kendoDropDownList").enable(false);
            $(comebackFlightNumber).data("kendoDropDownList").enable(false);

            if ($scope.revokeClick) {
                $scope.btaRevokeListUsers.forEach(item => {
                    //var isCancelId = $("#isCancel" + item.no);
                    if ($(isCancelId).length > 0) {
                        $(isCancelId).attr("disabled", "disabled");
                    }
                });
            }
            /*if (item.isCancel && !$scope.revokeClick) {
                let reasonId = '#reason' + item.no;
                $(reasonId).attr("disabled", "disabled");
            }*/
        }

    }
    //detail
    $scope.attachmentDetails = [];
    $scope.oldAttachmentDetails = [];
    $scope.removeFileDetails = [];
    $scope.onSelect = function (e) {
        let message = $.map(e.files, function (file) {
            $scope.attachmentDetails.push(file);
            return file.name;
        }).join(", ");
    };

    $scope.removeAttach = function (e) {
        let file = e.files[0];
        $scope.attachmentDetails = $scope.attachmentDetails.filter(item => item.name !== file.name);
    }

    $scope.removeOldAttachment = function (model) {
        $scope.oldAttachmentDetails = $scope.oldAttachmentDetails.filter(item => item.id !== model.id);
        $scope.removeFileDetails.push(model.id);
    }

    $scope.downloadAttachment = async function (id) {
        let result = await attachmentFile.downloadAndSaveFile({
            id
        });
    }

    async function uploadAction() {
        var payload = new FormData();
        $scope.attachmentDetails.forEach(item => {
            payload.append('file', item.rawFile, item.name);
        });
        let result = await attachmentService.getInstance().attachmentFile.upload(payload).$promise;
        return result;
    }

    async function mergeAttachment() {
        try {
            // Upload file lên server rồi lấy thông tin lưu thành chuỗi json
            let uploadResult = await uploadAction();
            let attachmentFilesJson = '';
            let allattachmentDetails = $scope.oldAttachmentDetails && $scope.oldAttachmentDetails.length ? $scope.oldAttachmentDetails.map(({
                id,
                fileDisplayName
            }) => ({
                id,
                fileDisplayName
            })) : [];
            if (uploadResult.isSuccess) {
                let attachmentFiles = uploadResult.object.map(({
                    id,
                    fileDisplayName
                }) => ({
                    id,
                    fileDisplayName
                }));
                allattachmentDetails = allattachmentDetails.concat(attachmentFiles);
            }
            attachmentFilesJson = JSON.stringify(allattachmentDetails);
            return attachmentFilesJson;
        } catch (e) {
            console.log(e);
            return '';
        }
    }
    //changing
    $scope.attachmentChanges = [];
    $scope.oldAttachmentChanges = [];
    $scope.removeFileChanges = [];
    $scope.onSelectChange = function (e) {
        let message = $.map(e.files, function (file) {
            $scope.attachmentChanges.push(file);
            return file.name;
        }).join(", ");
    };

    $scope.removeAttachChange = function (e) {
        let file = e.files[0];
        $scope.attachmentChanges = $scope.attachmentChanges.filter(item => item.name !== file.name);
    }

    $scope.removeOldAttachmentChange = function (model) {
        $scope.oldAttachmentChanges = $scope.oldAttachmentChanges.filter(item => item.id !== model.id);
        $scope.removeFileChanges.push(model.id);
    }

    $scope.downloadAttachmentChange = async function (id) {
        let result = await attachmentFile.downloadAndSaveFile({
            id
        });
    }

    async function uploadActionChange() {
        var payload = new FormData();
        $scope.attachmentChanges.forEach(item => {
            payload.append('file', item.rawFile, item.name);
        });
        let result = await attachmentService.getInstance().attachmentFile.upload(payload).$promise;
        return result;
    }

    async function mergeAttachmentChange() {
        try {
            // Upload file lên server rồi lấy thông tin lưu thành chuỗi json
            let uploadResult = await uploadActionChange();
            let attachmentFilesJson = '';
            let allattachmentChanges = $scope.oldAttachmentChanges && $scope.oldAttachmentChanges.length ? $scope.oldAttachmentChanges.map(({
                id,
                fileDisplayName
            }) => ({
                id,
                fileDisplayName
            })) : [];
            if (uploadResult.isSuccess) {
                let attachmentFiles = uploadResult.object.map(({
                    id,
                    fileDisplayName
                }) => ({
                    id,
                    fileDisplayName
                }));
                allattachmentChanges = allattachmentChanges.concat(attachmentFiles);
            }
            attachmentFilesJson = JSON.stringify(allattachmentChanges);
            return attachmentFilesJson;
        } catch (e) {
            console.log(e);
            return '';
        }
    }

    $scope.toDateChanged = function () {
        if ($scope.date.toDate == null) {
            $scope.date.toDate = $scope.date.fromDate;
        }
    }

    $scope.toDateCheckInOutChanged = function (dataItem) {
        if (dataItem.checkOutHotelDate == null) {
            dataItem.checkOutHotelDate = dataItem.toDate;
        }
    }

    $scope.toDateCheckInOutGridChangingCancellingChanged = function (dataItem) {
        if (dataItem.newCheckOutHotelDate == null) {
            dataItem.newCheckOutHotelDate = dataItem.toDate;
        }
    }

    $scope.getFilePath = async function (id) {
        let result = await attachmentFile.getFilePath({
            id
        });

        return result.object;
    }

    $scope.owaClose = function () {
        if (!$.isEmptyObject($("#attachmentViewDialog").data("kendoDialog"))) {
            attachmentViewDialog = $("#attachmentViewDialog").data("kendoDialog");
            attachmentViewDialog.close();
        }
    }

    $scope.viewFileOnline = async function (id) {
        let filePath = await $scope.getFilePath(id);
        if ($.type(filePath) == "string" && filePath.length > 0) {
            callHandler({
                Action: "viewFileOnWeb",
                filePath: filePath,
                itemID: id
            },
                function (resultData) {
                    if (!isNullOrUndefined(resultData) && resultData.data.length > 0) {
                        let attachmentViewDialog = null;
                        if (!$.isEmptyObject($("#attachmentViewDialog").data("kendoDialog"))) {
                            attachmentViewDialog = $("#attachmentViewDialog").data("kendoDialog");
                        }
                        else {
                            attachmentViewDialog = $("#attachmentViewDialog").kendoDialog({
                                width: "70%",
                                height: "100%",
                                close: function (e) {
                                    $("#attachmentViewDialog").hide();
                                }
                            }).data("kendoDialog");
                        }
                        $("#attachmentViewDialog").show();
                        $("#attachment_owa")[0].setAttribute("src", resultData.data);
                        attachmentViewDialog.open();

                    }
                    else {
                        Notification.error($translate.instant('NOT_SUPPORT_VIEW_ONLINE'));
                        $scope.$apply();
                    }

                }
            );
        }
    }


    //=============================================  Booking Hotels Section =====================================================
    $scope.checkHotelInfo = function () {
        let returnValue = new Array();
        try {
            let btaDetails = getDataGrid('#btaListDetailGrid');
            btaDetails.filter(function (cBTADetail, index) {
                if (!$.isEmptyObject(cBTADetail) && cBTADetail.stayHotel == true) {
                    if (isEmptyString(cBTADetail.hotelCode)) {
                        returnValue.push({ controlName: `${cBTADetail.sapCode} - ${cBTADetail.fullName}: ${$translate.instant('BTA_HOTEL')}`, message: ': ' + $translate.instant('COMMON_FIELD_IS_REQUIRED') });
                    }
                    if (cBTADetail.checkInHotelDate == null) {
                        returnValue.push({ controlName: `${cBTADetail.sapCode} - ${cBTADetail.fullName}: ${$translate.instant('BTA_CHECK_IN_HOTEL_DATE')}`, message: ': ' + $translate.instant('COMMON_FIELD_IS_REQUIRED') });
                    }
                    if (cBTADetail.checkOutHotelDate == null) {
                        returnValue.push({ controlName: `${cBTADetail.sapCode} - ${cBTADetail.fullName}: ${$translate.instant('BTA_CHECK_OUT_HOTEL_DATE')}`, message: ': ' + $translate.instant('COMMON_FIELD_IS_REQUIRED') });
                    }
                }
            });

        } catch (e) {

        }
        return returnValue;
    }

    $scope.$on("bookingHotels", async function (evt, data, itemActionScope) {
        $scope.errors = $scope.checkHotelInfo();
        if ($scope.errors.length == 0) {
            voteModel = data.voteModel;
            $scope.setRooms();

            //let title = $translate.instant(itemActionScope.status.approveFieldText);
            //itemActionScope.voteDialog.title(title);
            //itemActionScope.voteDialog.open();
            //itemActionScope.confirmVoteDialog = $scope.voteDialog;
        }
    });

    //=============================================  Booking Flight Section =====================================================



    $scope.$on("bookingFlights", async function (evt, data) {
        $scope.allowEditPassengerCommentd = true;
        voteModel = data.voteModel;
        popupToSelectEmployeeGroup(voteModel);
    });

    $scope.popupToViewBookingFlights = async function (voteModelId) {
        //dialog with no button
        $scope.dialogTripGroup.setOptions({
            actions: [{
                text: $translate.instant('COMMON_BUTTON_CLOSE'),
                action: function (e) {
                    let dialog = $("#dialog_TripGroup").data("kendoDialog")
                    dialog.close();
                },
                primary: true
            }]
        });

        if (voteModelId) {
            var res = await btaService.getInstance().bussinessTripApps.getEmployeeTripGroup({ id: voteModelId }, null).$promise;
            if (!$.isEmptyObject(res) && res.isSuccess) {
                $scope.btaListTripGroupGridData = res.object;
                $scope.tripGroupArray = new Array();
                $scope.allowSetTripGroupHasTicket = true;
                let tripGroupTicketInfo = {};

                //Collect ticket info and build tripgroup array
                $(res.object).map(function (index, item) {

                    //Prepare the maximum tripgroup array
                    $scope.tripGroupArray.push(
                        {
                            "id": index + 1,
                            "value": index + 1,
                            "isDeleted": false
                        });

                    //Prepare Ticket Info for each Trip Group
                    if (!$.isEmptyObject(item.flightDetails) && item.flightDetails.length > 0 && $.isEmptyObject(tripGroupTicketInfo[item.tripGroup])) {
                        let flightInfoArray = new aeon.flightInfoArray(item.flightDetails);
                        let ticketInfo = {};
                        ticketInfo.directFlight = flightInfoArray.getDirectFlight();
                        ticketInfo.returnFlight = flightInfoArray.getReturnFlight();
                        tripGroupTicketInfo[item.tripGroup] = ticketInfo;
                    }
                });
                //Prepare Info for Trip Group Grid
                $scope.btaTripGroupGridOptions = {
                    dataSource: {
                        data: $scope.btaListTripGroupGridData,
                        sort: [{ field: "tripGroup", dir: "asc" }, { field: "fullName", dir: "desc" }],
                        group: [{ field: "tripGroup" }],
                        footerTemplate: "footerTemplate footerTemplate footerTemplate footerTemplate footerTemplate footerTemplate footerTemplate footerTemplate "
                    },
                    sortable: false,
                    pageable: false,
                    columns: btaTripGroupGridColumns,

                    dataBound: function () {

                        dataView = this.dataSource.view();
                        for (var i = 0; i < dataView.length; i++) {
                            $(dataView[i].items).map(function (cindex, cItem) {
                                let isOverBudget = false;
                                let currentItemTicketPriceInfo = $scope.ticketPriceInfos[cItem.id] || {};

                                if (!$.isEmptyObject(cItem.flightDetails)) {
                                    let flightInfoArray = new aeon.flightInfoArray(cItem.flightDetails);
                                    currentItemTicketPriceInfo.totalPrice = flightInfoArray.getTotalPrice($scope.model.type);
                                    currentItemTicketPriceInfo.limitBudget = getLimitBudget(cItem);
                                    currentItemTicketPriceInfo.isOverBudget = currentItemTicketPriceInfo.totalPrice > currentItemTicketPriceInfo.limitBudget;
                                    isOverBudget = currentItemTicketPriceInfo.isOverBudget;
                                }

                                if (isOverBudget) {
                                    var uid = cItem.uid;
                                    $("#btaTripGroupGrid tbody").find("tr[data-uid=" + uid + "]").addClass("rOverBudget");  //alarm's in my style and we call uid for each row
                                }
                                $scope.ticketPriceInfos[cItem.id] = currentItemTicketPriceInfo;
                            });
                        }
                        $scope.btaTripGroupBindDataComplted = true;
                    }
                };

                setTimeout(function () {
                    var grid = $("#btaTripGroupGrid").data("kendoGrid");
                    grid.hideColumn(0);
                }, 200);

                //Set tripgroup has ticket
                Object.keys(tripGroupTicketInfo).forEach(function (tripGroup) {
                    $scope.setTripGroupHasTicket(tripGroup * 1, tripGroupTicketInfo[tripGroup]);
                });

                // set title cho cái dialog
                let dialog = $("#dialog_TripGroup").data("kendoDialog")
                dialog.title($translate.instant('BTA_VIEW_BOOKING_FLIGHT'));
                dialog.open();
            }
        }
    }

    $scope.tripGroupTemplate = `<span class="#: isDeleted ? 'k-state-disabled': ''#">
                                    #: value #
                                </span>`;

    $scope.tripGroupPopup = {};

    $scope.setGroupIdAsUnselected = function (groupId) {
        $($scope.tripGroupArray).filter(function (cIndex, cItem) {
            if (cItem.value == groupId) {
                cItem.isDeleted = false;
                cItem.ticketInfo = null;
            }
        });
    }

    $scope.doesAllowShowTripGroupButton = function (groupId, actionName) {
        let returnValue = false;
        let griddata = $("#btaTripGroupGrid").data("kendoGrid").dataSource._data;
        let itemArray = $(griddata).filter(function (index, item) {
            if (groupId == item.tripGroup && item.hasTicket == true) {
                return true;
            }
        });
        if (actionName == 'reviewTicket' || actionName == 'removeTicket') {
            returnValue = itemArray.length > 0;
        }
        else {
            returnValue = itemArray.length == 0;
        }
        return returnValue;
    }

    $scope.tripGroupSearchTicket = function (groupId) {
        let griddata = $("#btaTripGroupGrid").data("kendoGrid").dataSource._data;
        let groupItems = $(griddata).filter(function (index, item) {
            return groupId == item.tripGroup;
        });

        if (!$.isEmptyObject(groupItems) && groupItems.length > 0) {
            groupItems = groupItems.sort((a, b) => (a.jobGrade < b.jobGrade) ? 1 : -1);
            let groupItem = groupItems[0];
            let routeType = "ONEWAY"; //"ROUNDTRIP";
            let departureDate = moment(groupItem.fromDate).format('MM-DD-YYYY');
            let returntureDate = moment(groupItem.toDate).format('MM-DD-YYYY');
            if ($scope.model.isRoundTrip == true) {
                routeType = "ROUNDTRIP";
            }
            let searchTicketInfo = {
                "GroupId": groupId,
                "ArrivalName": groupItem.arrivalName,
                "DepartureName": groupItem.departureName,
                "OriginCode": groupItem.departureCode,
                "DestinationCode": groupItem.arrivalCode,
                "DepartureDate": departureDate,
                "ReturntureDate": returntureDate,
                "RouteType": routeType,
                "AdutsQtt": groupItems.length,
                "FlightType": $scope.model.type === 3 ? "Domestic" : "International",
                "Budget": getLimitBudget(groupItem),
                "JobGrade": groupItem.jobGrade
            };
            showSearchFlightPopup(searchTicketInfo);

            //$scope.setTripGroupHasTicket(groupId, $scope.ticketInfoTemp);
        }
    }

    $scope.prepareTripGroupBeforeSave = function () {
        let returnValue = new Array();
        let getTicketOfTripGroup = function (tripGroupId, businessTripApplicationDetailId) {
            let returnTicketInfo = null;
            let tripGroupInfo = $($scope.tripGroupArray).filter(function (cIndex, cItem) {
                return cItem.id == tripGroupId;
            });
            returnTicketInfo = tripGroupInfo.length > 0 ? tripGroupInfo[0].ticketInfo : null;
            if (!$.isEmptyObject(returnTicketInfo)) {
                for (var i = 0; i < returnTicketInfo.length; i++) {
                    if (returnTicketInfo[i].length > 0) {
                        returnTicketInfo[i].businessTripApplicationDetailId = businessTripApplicationDetailId;
                    }
                }
            }
            return returnTicketInfo;
        }

        let griddata = $("#btaTripGroupGrid").data("kendoGrid").dataSource._data;
        griddata = $(griddata).map(function (cIndex, cItem) {
            if (cItem.hasTicket) {
                let ticketInfo = getTicketOfTripGroup(cItem.tripGroup, cItem.id);
                if (!$.isEmptyObject(ticketInfo)) {
                    if (ticketInfo.length > 0) {
                        let flightInfos = new aeon.flightInfoArray(ticketInfo);

                        //Compare and merge with old ticket info
                        if (!$.isEmptyObject(cItem.flightDetails)) {
                            let oldFlightInfos = new aeon.flightInfoArray(cItem.flightDetails);
                            flightInfos = flightInfos.mergeWithOldTicketInfo(oldFlightInfos);
                            ticketInfo = flightInfos.ticketInfos;
                        }
                        let totalPrice = flightInfos.getTotalPrice($scope.model.type);
                        cItem.isOverBudget = totalPrice > getLimitBudget(cItem);
                    }
                    cItem.flightDetails = ticketInfo;
                }
            }
            returnValue.push(cItem);
            return cItem;
        });
        return returnValue;
    }

    $scope.setTripGroupHasTicket = function (groupId, ticketInfo) {
        try {
            //reset error message
            $scope.tripGroupPopup.errorMessages = null;

            let allowRefreshGrid = false;
            let setGroupIdHasTicket = function (groupId, ticketInfo) {
                $($scope.tripGroupArray).filter(function (cIndex, cItem) {
                    if (cItem.value == groupId) {
                        cItem.isDeleted = true;
                        if (!$.isEmptyObject(ticketInfo)) {
                            cItem.ticketInfo = new Array();

                            if (!$.isEmptyObject(ticketInfo.directFlight)) {
                                cItem.ticketInfo.push(ticketInfo.directFlight);
                            }
                            if (!$.isEmptyObject(ticketInfo.returnFlight)) {
                                cItem.ticketInfo.push(ticketInfo.returnFlight);
                            }
                        }
                    }
                });
            }
            setGroupIdHasTicket(groupId, ticketInfo);
            let griddata = $("#btaTripGroupGrid").data("kendoGrid").dataSource._data;
            griddata = $(griddata).map(function (index, item) {
                if (groupId == item.tripGroup) {
                    item.hasTicket = true;
                    allowRefreshGrid = true;
                }
            });
            griddata = $scope.prepareTripGroupBeforeSave();
            if (allowRefreshGrid) {
                RefreshTripGroupGridData(griddata);
            }

            // Close search ticket dialog
            let dialog = $("#dialog_SearchTicket").data("kendoDialog")
            dialog.close();
        }
        catch (e) {
            setTimeout($scope.setTripGroupHasTicket, 100, groupId, ticketInfo);
        }
    }

    $scope.showPopupConfirmRemoveTicket = function (groupId) {
        $scope.removeTicket_TripGroupId = groupId;
        let dialog = $("#dialog_RemoveTicketConfirm").data("kendoDialog");
        dialog.title($translate.instant('BTA_REMOVE_TICKET_CONFIRM_MESSAGE'));
        dialog.open();
    }

    $scope.removeTicketOfTripGroup = function (groupId) {
        let allowRefreshGrid = false;
        let griddata = $("#btaTripGroupGrid").data("kendoGrid").dataSource._data;
        griddata = $(griddata).map(function (index, item) {
            if (groupId == item.tripGroup) {
                item.hasTicket = false;
                item.isOverBudget = false;
                item.flightDetails = {};
                allowRefreshGrid = true;
            }
        });
        $scope.setGroupIdAsUnselected(groupId);
        if (allowRefreshGrid) {
            RefreshTripGroupGridData(griddata);
        }
    }

    $scope.tripGroupOnSelect = function (e) {
        if (e.dataItem.isDeleted) {
            e.preventDefault();
        }
    }

    $scope.tripGroupOnOpen = function (e) {
        this.dataItem.originalTripGroup = this.dataItem.tripGroup;
    }

    $scope.doesAllowMovePassengerToGroup = function (passengerItem, groupNumber) {
        let returnValue = false;
        try {
            let btaTripGroupData = $("#btaTripGroupGrid").data("kendoGrid").dataSource._data;
            if (!$.isEmptyObject(btaTripGroupData) && !$.isEmptyObject(passengerItem)) {
                let passengerOfSourceTripGroup = btaTripGroupData.filter(x => x.tripGroup == groupNumber && x.id != passengerItem.id);
                if (!$.isEmptyObject(passengerOfSourceTripGroup) && passengerOfSourceTripGroup.length > 0) {
                    let firstPassenger = passengerOfSourceTripGroup[0];

                    returnValue = (firstPassenger.arrivalCode == passengerItem.arrivalCode
                        && firstPassenger.departureCode == passengerItem.departureCode
                        && moment(firstPassenger.fromDate).format('YYYYMMDD') == moment(passengerItem.fromDate).format('YYYYMMDD')
                        && moment(firstPassenger.toDate).format('YYYYMMDD') == moment(passengerItem.toDate).format('YYYYMMDD')
                    );
                }
                else {
                    //this is the new group. No need to check
                    returnValue = true;
                }

            }
        } catch (e) {

        }
        return returnValue;
    }

    $scope.tripGroupOnChanged = function (e) {
        let allowMovetoNewTripGroup = $scope.doesAllowMovePassengerToGroup(this.dataItem, this.dataItem.tripGroup);
        if (!allowMovetoNewTripGroup) {
            this.dataItem.tripGroup = this.dataItem.originalTripGroup;
            Notification.error($translate.instant('BTA_CAN_CHANGE_THE_TRIP_GROUP'));
        }

        RefreshTripGroupGridData();
    }

    $scope.checkBooingTicket = async function () {
        $scope.tripGroupPopup.errorMessages = [];
        let passengerInfos = $scope.prepareTripGroupBeforeSave();
        let passengerHasNoTicket = $(passengerInfos).filter(function (cIndex, cItem) {
            return !cItem.hasTicket;
        });
        if (passengerHasNoTicket.length == 0) {
            $scope.tripGroupPopup.errorMessages = new Array();
            var res = await $scope.startBookingTicket(passengerInfos);
            if (res && res.isSuccess) {
                return true;
            }
            else {
                return false;
            }
        }
        else {
            let messageArray = passengerHasNoTicket.map((cIndex, cItem) => `${$translate.instant('BTA_TRIP_GROUP')} ${cItem.tripGroup} - ${cItem.sapCode} - ${cItem.fullName} - ${$translate.instant('BTA_HAS_NO_TICKET')}`);
            $scope.tripGroupPopup.errorMessages = messageArray;
            Notification.error($translate.instant('BTA_REQUIRED_CHOOSE_TICKET_FOR_ALL_GROUPS'));
            return false;
        }
        //return $scope.tripGroupPopup.errorMessages.length == 0;
    }

    //popup Trip group
    $scope.dialogTripGroupOption = {
        buttonLayout: "normal",
        collision: "fit",
        animation: {
            open: {
                effects: "fade:in"
            }
        },
        schema: {
            model: {
                id: "no"
            }
        },
        actions: [
            {
                text: $translate.instant('COMMON_BUTTON_BOOK_TICKET'),
                action: function (e) {
                    $scope.checkBooingTicket();
                    return false;
                },
                primary: true
            },
            {
                text: $translate.instant('COMMON_BUTTON_SAVE'),
                action: function (e) {
                    $scope.saveTicket(true);
                    return $.isEmptyObject($scope.tripGroupPopup.errorMessages) || $scope.tripGroupPopup.errorMessages.length == 0;
                },
                primary: true
            }]
    };

    //popup confirm remove ticket
    $scope.dialogRemoveTicketConfirmOption = {
        buttonLayout: "normal",
        animation: {
            open: {
                effects: "fade:in"
            }
        },
        schema: {
            model: {
                id: "no"
            }
        },
        actions: [{
            text: $translate.instant('COMMON_BUTTON_OK'),
            action: function (e) {
                $scope.removeTicketOfTripGroup($scope.removeTicket_TripGroupId);
                return true;
            },
            primary: true
        }]
    };

    //refresh the tripgroup with new data
    function RefreshTripGroupGridData(gridData) {
        //reset error message
        $scope.tripGroupPopup.errorMessages = null;

        let grid = $("#btaTripGroupGrid").data("kendoGrid")
        grid.dataSource.sort({ field: "tripGroup", dir: "asc" }, { field: "fullName", dir: "desc" });
        if (!$.isEmptyObject(gridData)) {
            grid.dataSource.data = gridData;
        }
        grid.refresh();
    }

    //Show popup trip group
    async function popupToSelectEmployeeGroup(voteModel) {
        if (!$.isEmptyObject(voteModel)) {
            $scope.wfVoteModel = voteModel;
            var res = await btaService.getInstance().bussinessTripApps.getEmployeeTripGroup({ id: voteModel.itemId }, null).$promise;
            if (!$.isEmptyObject(res) && res.isSuccess) {
                $scope.btaListTripGroupGridData = res.object;
                $scope.tripGroupArray = new Array();
                $scope.allowSetTripGroupHasTicket = true;
                let tripGroupTicketInfo = {};

                //Collect ticket info and build tripgroup array
                $(res.object).map(function (index, item) {

                    //Prepare the maximum tripgroup array
                    $scope.tripGroupArray.push(
                        {
                            "id": index + 1,
                            "value": index + 1,
                            "isDeleted": false
                        });

                    //Prepare Ticket Info for each Trip Group
                    if (!$.isEmptyObject(item.flightDetails) && item.flightDetails.length > 0 && $.isEmptyObject(tripGroupTicketInfo[item.tripGroup])) {
                        let flightInfoArray = new aeon.flightInfoArray(item.flightDetails);
                        let ticketInfo = {};
                        ticketInfo.directFlight = flightInfoArray.getDirectFlight();
                        ticketInfo.returnFlight = flightInfoArray.getReturnFlight();
                        tripGroupTicketInfo[item.tripGroup] = ticketInfo;
                    }
                });

                //Prepare Info for Trip Group Grid
                $scope.btaTripGroupGridOptions = {
                    dataSource: {
                        data: $scope.btaListTripGroupGridData,
                        sort: [{ field: "tripGroup", dir: "asc" }, { field: "fullName", dir: "desc" }],
                        group: [{ field: "tripGroup" }],
                        footerTemplate: "footerTemplate footerTemplate footerTemplate footerTemplate footerTemplate footerTemplate footerTemplate footerTemplate "
                    },
                    sortable: false,
                    pageable: false,
                    columns: btaTripGroupGridColumns,
                    dataBound: function () {
                        dataView = this.dataSource.view();
                        for (var i = 0; i < dataView.length; i++) {
                            $(dataView[i].items).map(function (cindex, cItem) {
                                let isOverBudget = false;
                                let currentItemTicketPriceInfo = $scope.ticketPriceInfos[cItem.id] || {};

                                if (!$.isEmptyObject(cItem.flightDetails)) {
                                    let flightInfoArray = new aeon.flightInfoArray(cItem.flightDetails);
                                    currentItemTicketPriceInfo.totalPrice = flightInfoArray.getTotalPrice($scope.model.type);
                                    currentItemTicketPriceInfo.limitBudget = getLimitBudget(cItem);
                                    currentItemTicketPriceInfo.isOverBudget = currentItemTicketPriceInfo.totalPrice > currentItemTicketPriceInfo.limitBudget;
                                    isOverBudget = currentItemTicketPriceInfo.isOverBudget;
                                }

                                if (isOverBudget) {
                                    var uid = cItem.uid;
                                    $("#btaTripGroupGrid tbody").find("tr[data-uid=" + uid + "]").addClass("rOverBudget");  //alarm's in my style and we call uid for each row
                                }

                                $scope.ticketPriceInfos[cItem.id] = currentItemTicketPriceInfo;
                            });
                        }
                        $scope.btaTripGroupBindDataComplted = true;
                    }
                };

                //Set tripgroup has ticket
                Object.keys(tripGroupTicketInfo).forEach(function (tripGroup) {
                    $scope.setTripGroupHasTicket(tripGroup * 1, tripGroupTicketInfo[tripGroup]);
                });

                // set title cho cái dialog
                let dialog = $("#dialog_TripGroup").data("kendoDialog")
                dialog.title($translate.instant('BTA_TRIP_GROUP'));
                dialog.open();
            }
        }

    }

    var btaTripGroupGridColumns = [
        {
            field: "sapCode",
            headerTemplate: $translate.instant('COMMON_SAP_CODE'),
            width: "120px",
            editable: function (e) {
                return false;
            },
            tempalte: function (dataItem) {
                return `<input ng-readonly="true" class="k-input" type="text" ng-model="dataItem.sapCode" style="width: 100%"/>`;
            }
        },
        {
            field: "fullName",
            headerTemplate: $translate.instant('COMMON_FULL_NAME'),
            editable: function (e) {
                return false;
            },
            tempalte: function (dataItem) {
                return `<input ng-readonly="true" class="k-input" type="text" ng-model="dataItem.fullName" style="width: 100%"/>`;
            }
        },
        {
            field: "departureName",
            width: "160px",
            headerTemplate: $translate.instant('BTA_DEPARTURE'),
            editable: function (e) {
                return false;
            }
        },
        {
            field: "arrivalName",
            width: "160px",
            headerTemplate: $translate.instant('BTA_ARRIVAL'),
            editable: function (e) {
                return false;
            }
        },
        {
            field: "fromDate",
            width: "200px",
            headerTemplate: $translate.instant('BTA_TRIP_FROM_DATE'),
            format: "{0:dd/MM/yyyy}",
            editable: function (e) {
                return false;
            },
            template: function (dataItem) {
                return `<input kendo-date-time-picker
                    id="fromDate${dataItem.no}" ng-readonly="true"
                    k-ng-model="dataItem.fromDate"
                    k-date-input="true"
                    k-format="'dd/MM/yyyy HH:mm'"
                    k-time-format="'HH:mm'"
                    style="width: 100%;" />`;
            }
        },
        {
            field: "toDate",
            width: "200px",
            headerTemplate: $translate.instant('BTA_TRIP_TO_DATE'),
            format: "{0:dd/MM/yyyy}",
            editable: function (e) {
                return false;
            },
            template: function (dataItem) {
                return `<input kendo-date-time-picker
                    id="toDate${dataItem.no}" ng-readonly="true"
                    k-ng-model="dataItem.toDate"
                    k-date-input="true"
                    k-format="'dd/MM/yyyy HH:mm'"
                    k-time-format="'HH:mm'"
                    style="width: 100%;" />`;
            }
        },
        {
            field: "jobGrade",
            width: "100px",
            headerTemplate: $translate.instant('BTA_JOB_GRADE'),
            editable: function (e) {
                return false;
            }
        },
        {
            field: "maxBudgetAmount",
            headerTemplate: $translate.instant('BTA_TRIP_MAX_BUDGET'),
            format: "{0:n0}",
            width: "200px",
            editable: function (e) {
                return false;
            },
            template: function (dataItem) {

                let currentItemTicketPriceInfo = $scope.ticketPriceInfos[dataItem.id] || null;
                let returnValue = `<span>{{${getLimitBudget(dataItem)}| number}}/VND</span>`;
                if (!$.isEmptyObject(currentItemTicketPriceInfo) && currentItemTicketPriceInfo.isOverBudget == true && dataItem.hasTicket == true) {
                    returnValue += `<br/><span class='cOverBudgetNumber'>{{${currentItemTicketPriceInfo.limitBudget - currentItemTicketPriceInfo.totalPrice}| number}}/VND</span>`
                }
                return returnValue;
            }
        },
        {
            field: "tripGroup",
            headerTemplate: $translate.instant('BTA_GROUP'),
            groupHeaderTemplate: "Group #= value #",
            width: "120px",
            template: function (dataItem) {
                if ($scope.isViewMode) {
                    return `<label style="margin-left: .25em;">${dataItem.tripGroup}</label>`;
                }
                return `<select kendo-drop-down-list style="width: 100%;"
                        k-template='tripGroupTemplate'
                        k-ng-disabled='dataItem.hasTicket==true'
                        k-ng-model="dataItem.tripGroup"
                        k-auto-bind="'true'"
                        k-value-primitive="'false'"
                        k-data-text-field="'id'"
                        k-data-value-field="'value'"
                        k-data-source="tripGroupArray"
                        k-filter="'contains'"
                        k-select="tripGroupOnSelect"
                        k-on-open="tripGroupOnOpen()"
                        k-on-change="tripGroupOnChanged()"
                        > </select>`;
            }
        },
        {
            field: "comments",
            headerTemplate: $translate.instant('BTA_OVER_BUDGET_REASON'),
            width: "280px",
            groupFooterTemplate: function (data) {
                if ($scope.isViewMode) {
                    return `<kendo-button ng-show='doesAllowShowTripGroupButton(` + data.value + `, "reviewTicket")' class="k-primary float-right"  ng-click='showViewTicketPopup(` + data.value + `)'>` + $translate.instant('COMMON_BUTTON_VIEW_TICKET') + `</kendo-button>`;
                }
                return `<kendo-button ng-show='doesAllowShowTripGroupButton(` + data.value + `, "reviewTicket")' class="k-primary float-right btn-sm" style='line-height: 0.8 !important;'  ng-click='showViewTicketPopup(` + data.value + `)'>` + $translate.instant('COMMON_BUTTON_VIEW_TICKET') + `</kendo-button>
                        <kendo-button ng-show='doesAllowShowTripGroupButton(` + data.value + `, "removeTicket")' class="k-primary float-right btn-sm" style='line-height: 0.8 !important;' ng-click='showPopupConfirmRemoveTicket(` + data.value + `)'>` + $translate.instant('COMMON_BUTTON_DELETE_TICKET') + `</kendo-button>
                        <kendo-button ng-show='doesAllowShowTripGroupButton(` + data.value + `, "searchTicket")' class="k-primary float-right btn-sm" style='line-height: 0.8 !important;' ng-click='tripGroupSearchTicket(` + data.value + `)'>` + $translate.instant('COMMON_BUTTON_SEARCH_TICKET') + `</kendo-button>`;
            },
            editable: function (e) {
                if ($scope.allowEditPassengerCommentd == true) {
                    return true
                }
                else {
                    return false;
                }
            },
            template: function (dataItem) {
                return `<input ng-readonly="allowEditPassengerCommentd != true" class="k-input" type="text" ng-model="dataItem.comments" style="width: 100%"/>`;
            }
        },
    ]

    //popup Search Ticket dialogSearchTicket 
    $scope.dialogSearchTicketOption = {
        buttonLayout: "normal",
        collision: "fit",
        animation: {
            open: {
                effects: "fade:in"
            }
        },
        schema: {
            model: {
                id: "no"
            }
        }
    };

    //popup View Ticket dialogSearchTicket 
    $scope.dialogViewTicketOption = {
        buttonLayout: "normal",
        collision: "fit",
        animation: {
            open: {
                effects: "fade:in"
            }
        },
        schema: {
            model: {
                id: "no"
            }
        },
        actions: [{
            text: $translate.instant('COMMON_BUTTON_CLOSE'),
            action: function (e) {
                let dialog = $("#dialog_SearchTicket").data("kendoDialog")
                dialog.close();
            },
            primary: true
        }]
    };

    //popup View Ticket dialogSearchTicket 
    $scope.dialogViewChangeCancelTicketOption = {
        buttonLayout: "normal",
        collision: "fit",
        animation: {
            open: {
                effects: "fade:in"
            }
        },
        schema: {
            model: {
                id: "no"
            }
        },
        /*actions: [{
            text: $translate.instant('COMMON_BUTTON_CLOSE'),
            action: function (e) {
                let dialog = $("#dialog_ViewChangeCancelTicket").data("kendoDialog")
                dialog.close();
            },
            primary: true
        }]*/
    };

    $scope.showViewTicketPopup = function (groupId) {
        if ((groupId * 1) > 0) {
            if (!$.isEmptyObject($scope.viewTicketScope.showTicket)) {
                $scope.viewTicketScope.closePopup = function () {
                    let dialog = $("#dialog_ViewTicket").data("kendoDialog")
                    dialog.close();
                };
                let tripGroupInfo = $($scope.tripGroupArray).filter(function (cIndex, cItem) {
                    return cItem.id == groupId;
                });
                let ticketInfo = tripGroupInfo.length > 0 ? tripGroupInfo[0].ticketInfo : null;
                $scope.viewTicketScope.showTicket(ticketInfo);
                // set title cho cái dialog
                let dialog = $("#dialog_ViewTicket").data("kendoDialog")
                dialog.title('View Ticket');
                dialog.open();
            }
        }
    }
    function showSearchFlightPopup(searchTicketModel) {
        if (!$.isEmptyObject(searchTicketModel)) {
            if (!$.isEmptyObject($scope.searchTicketScope.loadFlight)) {
                $scope.searchTicketScope.setTripGroupHasTicket = $scope.setTripGroupHasTicket;
                $scope.searchTicketScope.loadFlight(searchTicketModel);

                // set title cho cái dialog
                let dialog = $("#dialog_SearchTicket").data("kendoDialog")
                dialog.title($translate.instant('COMMON_BUTTON_SEARCH_TICKET'));
                dialog.open();
            }
        }
    }

    $scope.startBookingTicket = async function (passengerInfos) {

        let saveTicketsStatus = await $scope.saveTicket(false);

        if (saveTicketsStatus) {
            let overBudgetPassenger = passengerInfos.filter(x => x.isOverBudget);
            //overBudgetPassenger = [];
            if (overBudgetPassenger.length > 0) {
                //OverBudget ==> send to Admin checker
                let vote = await workflowService.getInstance().workflows.vote($scope.wfVoteModel).$promise;
                if (vote.isSuccess) {
                    $scope.tripGroupPopup.errorMessages = null;
                    let dialog = $("#dialog_TripGroup").data("kendoDialog");
                    dialog.close();
                    $state.go($state.current.name, { id: $scope.model.id, referenceValue: $scope.model.referenceNumber }, { reload: true });
                    Notification.success($translate.instant('COMMON_SAVE_SUCCESS'));
                }
            }
            else {
                var res = await bookingFlightService.bookingTicket(passengerInfos, $stateParams.id);
                if (res && res.isSuccess) {
                    $scope.tripGroupPopup.errorMessages = null;
                    let dialog = $("#dialog_TripGroup").data("kendoDialog");
                    dialog.close();
                    let vote = await workflowService.getInstance().workflows.vote($scope.wfVoteModel).$promise;
                    if (vote.isSuccess) {
                        $state.go($state.current.name, { id: $scope.model.id, referenceValue: $scope.model.referenceNumber }, { reload: true });
                        Notification.success($translate.instant('COMMON_SAVE_SUCCESS'));
                    }
                } else {
                    $scope.tripGroupPopup.errorMessages = [res.message];
                    $scope.$apply();
                    return false;
                }
            }
        }
        else {
            return false;
        }
    }
    $scope.approvalOverBudget = { isValid: true };

    $scope.bookingTicket = async function () {
        var res = await btaService.getInstance().bussinessTripApps.getEmployeeTripGroup({ id: $scope.model.id }, null).$promise;
        if (!$.isEmptyObject(res) && res.isSuccess) {
            var res = await bookingFlightService.bookingTicket(res.object);
            if (res && res.isSuccess) {
                $scope.approvalOverBudget.isValid = true;
                $scope.approvalOverBudget.errorMessage = null;
                let dialog = $("#dialog_ApprovalOverBudget").data("kendoDialog");
                dialog.close();

                $scope.wfVoteModel = {
                    itemId: $scope.model.id,
                    Comment: $scope.approvalOverBudget.comment,
                    Vote: 1
                }

                let vote = await workflowService.getInstance().workflows.vote($scope.wfVoteModel).$promise;
                if (vote.isSuccess) {
                    $state.go($state.current.name, { id: $scope.model.id, referenceValue: $scope.model.referenceNumber }, { reload: true });
                    Notification.success($translate.instant('COMMON_SAVE_SUCCESS'));
                }
            } else {
                $scope.approvalOverBudget.isValid = false;
                $scope.approvalOverBudget.errorMessage = res.message;
                $scope.$apply();
                return false;
            }
        }
    }

    $scope.dialogApprovalOverBudgetOption = {
        width: "600px",
        buttonLayout: "normal",
        closable: true,
        modal: true,
        visible: false,
        content: "",
        actions: [{
            text: $translate.instant('COMMON_BUTTON_OK'),
            action: function (e) {
                if ($scope.approvalOverBudget.comment) {
                    $scope.approvalOverBudget.isValid = true;
                    $scope.approvalOverBudget.errorMessage = "";
                    $scope.bookingTicket();
                    return false;
                } else {
                    $scope.approvalOverBudget.isValid = false;
                    $scope.approvalOverBudget.errorMessage = $translate.instant('COMMON_COMMENT_VALIDATE');
                    $scope.$apply();
                    return false;
                }
            },
            primary: true
        }],
        close: async function (e) {
        }
    };

    $scope.$on("approvalOverBudget", async function (evt, data) {
        let dialog = $("#dialog_ApprovalOverBudget").data("kendoDialog");
        dialog.title($translate.instant(dataService.workflowStatus.approveFieldText));
        dialog.open();
    });

    //=============================================  End Booking Flight Section =====================================================
    //=============================================  Start Revoke Booking Flight ====================================================
    $scope.revokeClick = false;
    $scope.revokeClickFirstTime = false;
    $scope.isRevokeRTC = false;
    $scope.isEditViewTicket = false;
    $scope.btaRevokeListUsers = [];
    $scope.btaChangingListUsers = [];
    $scope.isChangingAdminCheckerStep = false;
    $scope.reasonKeyUp = function (dataItem) {
        let cRow = $scope.btaRevokeListUsers.filter(x => x.no === dataItem.no);
        if (cRow && cRow.length > 0) {
            if ($scope.revokeClick || ($scope.isEditViewTicket && dataItem.isCancel)) {
                cRow[0].reasonForInBoundFlight = dataItem.reason;
                cRow[0].reasonForOutBoundFlight = dataItem.reason;
                cRow[0].reason = dataItem.reason;
            }
            else {
                cRow[0].reason = dataItem.reason;
            }
        }
    }
    function setGridChangeCancelValue() {
        $scope.btaRevokeListUsers = getDataGrid('#btaListChangeOrCancelGrid');
    }
    $scope.returnChangeCancelObject = function (data, cancellationFeeObj) {
        if (!$.isEmptyObject(data) && data.length > 0) {

            var dataChange = {};
            data.forEach(function (item) {
                dataChange['sapCode'] = item.sapCode;
                if (item.directFlight) {
                    dataChange['isCancelOutBoundFlight'] = item.isCancelOutBoundFlight;
                    dataChange['reasonForOutBoundFlight'] = item.reasonForOutBoundFlight;
                }
                else {
                    dataChange['isCancelInBoundFlight'] = item.isCancelInBoundFlight;
                    dataChange['reasonForInBoundFlight'] = item.reasonForInBoundFlight;
                }
            });
            if ($scope.isChangingAdminCheckerStep) {
                dataChange["cancellationFeeObj"] = cancellationFeeObj;
            }
            var arrayCurrData = [];
            arrayCurrData.push(dataChange);

            $scope.btaRevokeListUsers = $scope.btaRevokeListUsers.map(item => {
                const obj = arrayCurrData.find(o => o.sapCode === item.sapCode);
                return { ...item, ...obj };
            });

            setDataSourceForGrid('#btaListChangeOrCancelGrid', $scope.btaRevokeListUsers);
            setTimeout(function () {
                enableGridChangingCancelling($scope.btaRevokeListUsers);
            }, 200);
        }
    }
    function validateCancelDetail() {
        var errors = [];
        $scope.btaChangingListUsers = $scope.btaRevokeListUsers;
        if ($scope.btaChangingListUsers.length > 0) {
            $scope.btaChangingListUsers.forEach(function (item) {
                if (item.isCancel) {
                    if ((item.isCancelInBoundFlight == false || item.reasonForInBoundFlight == "") &&
                        (item.isCancelOutBoundFlight == false || item.reasonForOutBoundFlight == "")) {
                        errors.push({
                            fieldName: $translate.instant('BTA_IS_CANCEL'),
                            controlName: $translate.instant('BTA_IS_CANCEL') + ` ${item.no}`,
                            errorDetail: $translate.instant('BTA_VALIDATE_IS_CANCEL'),
                            isRule: false,
                        });
                    }
                }
            });
        }
        return errors;
    }

    function validateCancellationFee(gridValues) {
        var errors = [];
        if (gridValues.length > 0) {
            gridValues.forEach(function (item) {
                if (item.isCancel) {
                    if (item.cancellationFeeObj == null) {
                        errors.push({
                            fieldName: $translate.instant('BTA_CANCELLATION_FEE'),
                            controlName: $translate.instant('BTA_CANCELLATION_FEE') + ` ${item.no}`,
                            errorDetail: $translate.instant('BTA_VALIDATE_CANCELLATIONFEE'),
                            isRule: false,
                        });
                    }
                }
                else if (item.isCancelInBoundFlight || item.isCancelOutBoundFlight) {
                    if (item.cancellationFeeObj == null) {
                        errors.push({
                            fieldName: $translate.instant('BTA_CANCELLATION_FEE'),
                            controlName: $translate.instant('BTA_CANCELLATION_FEE') + ` ${item.no}`,
                            errorDetail: $translate.instant('BTA_VALIDATE_CANCELLATIONFEE'),
                            isRule: false,
                        });
                    }
                }
            });
        }
        return errors;
    }

    function showViewChangeCancelTicketPopup(userSapCode, BTADetailId, viewMode, currentChangeOrCancelRow, isAdminCheckerStep, isEditViewTicket, viewWhenCompleted) {
        //setGridChangeCancelValue();
        $scope.viewChangeCancelTicketScope.viewUserTicket(userSapCode, BTADetailId, viewMode, currentChangeOrCancelRow, isAdminCheckerStep, isEditViewTicket, viewWhenCompleted);

        if (isAdminCheckerStep || !viewMode) {
            $scope.viewChangeCancelTicketScope.returnChangeCancelObject = $scope.returnChangeCancelObject;
        }

        let dialog = $("#dialog_ViewChangeCancelTicket").data("kendoDialog");
        var titlePopup = $translate.instant('BTA_CHANGE_CANCEL_VIEW_TICKET_TITLE') + " " + userSapCode;

        if (viewWhenCompleted) {
            titlePopup = $translate.instant('BTA_VIEW_TICKET_TITLE') + " " + userSapCode;
        }
        dialog.title(titlePopup);
        dialog.open();
    }
    $scope.viewTicketForCurrentUser = function (userSapCode, BTADetailId, viewMode, isAdminCheckerStep, viewWhenCompleted = false) {
        let cRow = [];

        if ($scope.btaRevokeListUsers.length == 0) {
            setGridChangeCancelValue();
        }
        cRow = $scope.btaRevokeListUsers.filter(x => x.sapCode === userSapCode);
        showViewChangeCancelTicketPopup(userSapCode, BTADetailId, viewMode, cRow[0], isAdminCheckerStep, $scope.isEditViewTicket, viewWhenCompleted);
    }

    function revoke_ValidateChangeBusinessTrip() {
        let errors = [];
        let currentBTAListChangeOrCancel = $("#btaListChangeOrCancelGrid").data("kendoGrid").dataSource._data;

        if (currentBTAListChangeOrCancel.length > 0) {
            for (let i in currentBTAListChangeOrCancel) {
                let item = currentBTAListChangeOrCancel[i];
                Object.keys(item).forEach(function (fieldName) {
                    let propValue = item[fieldName];

                    if (!propValue && fieldName == 'reason') {
                        errors.push({
                            controlName: $translate.instant('COMMON_REASON') + ` ${item['no']}`,
                            errorDetail: $translate.instant('COMMON_FIELD_IS_REQUIRED'),
                            isRule: false,
                        });
                    }
                });
            }
        }
        else {// dòng rỗng
            errors.push({
                fieldName: $translate.instant('BTA_LIST_CHANGING_CACELLING_BUSNESS_TRIP'),
                controlName: $translate.instant('BTA_CHANGING_CACELLING_BUSNESS_TRIP'),
                errorDetail: $translate.instant('COMMON_FIELD_IS_REQUIRED'),
                isRule: false,
            });
        }
        return errors;
    }
    $scope.processRevokeWF = async function (model) {
        if ($scope.model.status && $scope.model.status == 'Completed') {
            var result = await workflowService.getInstance().workflows.startWorkflow(model, null).$promise;
            if (result.messages.length == 0) {
                Notification.success($translate.instant('COMMON_WORKFLOW_STARTED'));
            } else {
                if (result.messages && result.messages.length) {
                    Notification.error(result.messages[0]);
                }
            }
        }
        else {
            var result = await workflowService.getInstance().workflows.vote(model.voteModel).$promise;
            if (result.messages.length == 0) {
                Notification.success($translate.instant('COMMON_WORKFLOW_PROCESSED'));
            } else {
                if (result.messages && result.messages.length) {
                    Notification.error(result.messages[0]);
                }
            }
        }
    }
    $scope.saveRevokeInfo = async function () {
        let gridBTAChangeOrCancel = $("#btaListChangeOrCancelGrid").data("kendoGrid");
        if (gridBTAChangeOrCancel && gridBTAChangeOrCancel.dataSource && gridBTAChangeOrCancel.dataSource._data && gridBTAChangeOrCancel.dataSource._data.length) {
            gridBTAChangeOrCancel.dataSource._data.forEach(function (item) {
                if (item.isCancel) {
                    item["isCancelInBoundFlight"] = true;
                    item["isCancelOutBoundFlight"] = true;
                    item["reasonForInBoundFlight"] = item.reason;
                    item["reasonForOutBoundFlight"] = item.reason;
                }
            });
            $scope.model.changeCancelBusinessTripDetails = JSON.stringify(gridBTAChangeOrCancel.dataSource._data.toJSON());
        }
        var argModel = { changeCancelBusinessTripDetailStr: $scope.model.changeCancelBusinessTripDetails, businessTripApplicationId: $scope.model.id };
        var res = await btaService.getInstance().bussinessTripApps.saveRevokeInfo(argModel).$promise;
        if (res.isSuccess) {
            $state.go($state.current.name, { id: res.object.id, referenceValue: res.object.referenceNumber }, { reload: true });
            Notification.success($translate.instant('COMMON_SAVE_SUCCESS'));
        } else {
            Notification.error(res.message);
        }
        return res;
    }
    $scope.revoke_Actions = async function () {
        $scope.errorChangings = [];
        var result = { isSuccess: false };
        $scope.errorChangings = revoke_ValidateChangeBusinessTrip();
        if ($scope.errorChangings.length > 0) {
            return result;
        }
        else {
            //Save model
            var saveModel = await $scope.saveRevokeInfo();
            if (saveModel.isSuccess) {
                result.isSuccess = true;
            }
        }
        return result;
    }

    $scope.revokeInit = function () {
        $scope.isClickReviewUserForChanging = false;

        //Disable add more button
        $("#addMore").attr("disabled", "disabled");

        //get all employees from grid
        let allUserDetails = getDataGrid('#btaListDetailGrid');
        let existUsers = getDataGrid('#btaListChangeOrCancelGrid');
        $scope.btaRevokeListUsers = allUserDetails;

        //Filter out the list of users that need to add to changing grid when reovke
        if (existUsers.length > 0) {

            //remove duplicate user
            allUserDetails = allUserDetails.filter(function (item) {
                for (var i = 0, len = existUsers.length; i < len; i++) {
                    if (existUsers[i].sapCode == item.sapCode) {
                        return false;
                    }
                }
                return true;
            });

            $scope.btaRevokeListUsers = allUserDetails;
            existUsers.forEach(item => {
                item["isCancel"] = true;
                item["isKeep"] = true;
            });
        }
        if ($scope.btaRevokeListUsers.length > 0) {
            $scope.btaRevokeListUsers.forEach(item => {
                item["isCancel"] = true;
                item["isKeep"] = false;
                item["reason"] = '';
                item["destinationCode"] = item.arrivalCode;
                item["destinationName"] = item.arrivalName;
                item["newHotelCode"] = '';
                item["newHotelName"] = '';
                item["newFlightNumberCode"] = '';
                item["newFlightNumberName"] = '';
                item["businessTripApplicationDetailId"] = item.id;
            });
        }
        //merge exist user and remain user
        $.merge($scope.btaRevokeListUsers, existUsers);
        setDataSourceForGrid('#btaListChangeOrCancelGrid', $scope.btaRevokeListUsers);

        $scope.btaRevokeListUsers.forEach(item => {
            item["isCancelInBoundFlight"] = true;
            item["isCancelOutBoundFlight"] = true;
            item["reasonForInBoundFlight"] = "";
            item["reasonForOutBoundFlight"] = "";
            $scope.changeCancel(item, item.isKeep);
        });
    }
    //Revoke BTA
    $scope.$on("revokeBookingFlight", async function (evt, data) {
        $scope.revokeClick = true;

        if (!$scope.revokeClickFirstTime && !$scope.isRevokeRTC) {
            $scope.revokeInit();
            $scope.dialog = $rootScope.showConfirmDelete($translate.instant('BTA_REVOKE_DIALOG'), $translate.instant('BTA_REVOKE_NOTIFY'), $translate.instant('COMMON_BUTTON_OK'));
            $scope.revokeClickFirstTime = true;
        }
        else {
            setGridChangeCancelValue();
            if ($scope.btaRevokeListUsers.length == 0) {
                Notification.error("No data");
                return;
            }
            //Validate reason in changing/cancelling grid and run actions
            var runRevokeActions = await $scope.revoke_Actions();
            if (runRevokeActions.isSuccess) {
                $scope.processRevokeWF(data);
            }
        }
    });
    //Add cancellation fee BTA when revoke=======================================
    function cancellationFee_ValidateChangeBusinessTrip() {
        let errors = [];
        let currentBTAListChangeOrCancel = $("#btaListChangeOrCancelGrid").data("kendoGrid").dataSource._data;

        if (currentBTAListChangeOrCancel.length > 0) {
            for (let i in currentBTAListChangeOrCancel) {
                let item = currentBTAListChangeOrCancel[i];
                Object.keys(item).forEach(function (fieldName) {
                    let propValue = item[fieldName];

                    if (!propValue && fieldName == 'penaltyFee' && propValue == 0) {
                        errors.push({
                            controlName: $translate.instant('BTA_CANCELLATION_FEE') + ` ${item['no']}`,
                            errorDetail: $translate.instant('COMMON_FIELD_IS_REQUIRED'),
                            isRule: false,
                        });
                    }
                });
            }
        }
        else {// dòng rỗng
            errors.push({
                fieldName: $translate.instant('BTA_LIST_CHANGING_CACELLING_BUSNESS_TRIP'),
                controlName: $translate.instant('BTA_CHANGING_CACELLING_BUSNESS_TRIP'),
                errorDetail: $translate.instant('COMMON_FIELD_IS_REQUIRED'),
                isRule: false,
            });
        }
        return errors;
    }
    function getLimitBudget(btaDetailItem) {
        let limitBudgetAmount = 0;
        try {
            if (!$.isEmptyObject(btaDetailItem)) {
                if ($scope.model.isRoundTrip) {
                    limitBudgetAmount = btaDetailItem.maxBudgetAmount;
                }
                else {
                    limitBudgetAmount = btaDetailItem.maxBudgetAmount / 2;
                }
            }
        } catch (e) {

        }
        return limitBudgetAmount;
    }
    //save model
    $scope.saveRevokeCancellationFee = async function () {
        let gridBTAChangeOrCancel = $("#btaListChangeOrCancelGrid").data("kendoGrid");
        if (gridBTAChangeOrCancel && gridBTAChangeOrCancel.dataSource && gridBTAChangeOrCancel.dataSource._data && gridBTAChangeOrCancel.dataSource._data.length) {
            gridBTAChangeOrCancel.dataSource._data.forEach(function (item) {
                if (item.isCancel) {
                    item["cancellationFeeObj"] = `{"InBound": ${item.penaltyFee}, "OutBound": ${item.penaltyFee}}`;
                }
            });
            $scope.model.changeCancelBusinessTripDetails = JSON.stringify(gridBTAChangeOrCancel.dataSource._data.toJSON());
        }
        var argModel = { changeCancelBusinessTripDetailStr: $scope.model.changeCancelBusinessTripDetails, businessTripApplicationId: $scope.model.id };
        var res = await btaService.getInstance().bussinessTripApps.saveCancellationFee_revoke(argModel).$promise;
        if (res.isSuccess) {
            $state.go($state.current.name, { id: res.object.id, referenceValue: res.object.referenceNumber }, { reload: true });
            Notification.success($translate.instant('COMMON_SAVE_SUCCESS'));
        } else {
            Notification.error("Error System");
        }
        return res;
    }
    $scope.processCFAction = async function () {
        $scope.errorChangings = [];
        var result = { isSuccess: false };
        $scope.errorChangings = cancellationFee_ValidateChangeBusinessTrip();
        if ($scope.errorChangings.length > 0) {
            return result;
        }
        else {
            //Save model
            var saveModel = await $scope.saveRevokeCancellationFee();
            if (saveModel.isSuccess) {
                result.isSuccess = true;
            }
        }
        return result;
    }

    $scope.$on("addCancellationFee", async function (evt, data) {

        //Validate cancellation fee in changing/cancelling grid and run actions
        var runCFActions = await $scope.processCFAction();
        if (runCFActions.isSuccess) {
            $scope.processRevokeWF(data);
        }

    });
    $scope.saveChangingCancellationFee = async function () {
        $scope.errorChangings = [];
        var result = { isSuccess: false };
        let gridBTAChangeOrCancel = $("#btaListChangeOrCancelGrid").data("kendoGrid");
        if (gridBTAChangeOrCancel && gridBTAChangeOrCancel.dataSource && gridBTAChangeOrCancel.dataSource._data && gridBTAChangeOrCancel.dataSource._data.length) {
            $scope.model.changeCancelBusinessTripDetails = JSON.stringify(gridBTAChangeOrCancel.dataSource._data.toJSON());
            $scope.errorChangings = validateCancellationFee(gridBTAChangeOrCancel.dataSource._data);
        }
        if ($scope.errorChangings.length > 0) {
            return result;
        }
        var argModel = { changeCancelBusinessTripDetailStr: $scope.model.changeCancelBusinessTripDetails, businessTripApplicationId: $scope.model.id };
        var res = await btaService.getInstance().bussinessTripApps.saveCancellationFee_changing(argModel).$promise;
        if (res.isSuccess) {
            $state.go($state.current.name, { id: res.object.id, referenceValue: res.object.referenceNumber }, { reload: true });
            Notification.success($translate.instant('COMMON_SAVE_SUCCESS'));
        } else {
            Notification.error("Error System");
        }
        return res;
    }
    $scope.$on("addChangingCancellationFee", async function (evt, data) {

        //Run actions and complete WF
        var result = await $scope.saveChangingCancellationFee();
        if (result.isSuccess) {
            $scope.processRevokeWF(data);
        }

    });

    //=============================================  End Revoke Booking Flight ======================================================
});