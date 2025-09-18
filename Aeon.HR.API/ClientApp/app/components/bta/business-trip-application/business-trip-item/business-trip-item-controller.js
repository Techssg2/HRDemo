var ssgApp = angular.module('ssg.businessTripItemModule', ["kendo.directives"]);
ssgApp.controller('businessTripItemController', function ($rootScope, $scope, $location, appSetting, localStorageService, $stateParams, $state, moment, Notification, settingService, $timeout, cbService, dataService, workflowService, ssgexService, fileService, commonData, $translate, attachmentService, attachmentFile, $compile, btaService, bookingFlightService) {
    $scope.titleHeader = 'BUSINESS TRIP APPLICATION';
    $scope.title = $stateParams.id ? /*$translate.instant('BUSINESS_TRIP_APPLICATION') + ': ' +*/ $stateParams.referenceValue : /*$translate.instant('BUSINESS_TRIP_APPLICATION_NEW_TITLE')*/'';
    this.$onInit = function () {
        $scope.isITHelpdesk = $rootScope.currentUser && $rootScope.currentUser.isITHelpDesk;
        $scope.disableChangeOrCancel = false;
        $scope.perm = true;
        $scope.forceChangeBusinessTrip = false;
        $scope.btaListDetailGridData = [];
        $scope.btaListOverBudgetData = [];
        $scope.btaListDetailGroupedGridData = [];
        $scope.btaListChangeOrCancelGridData = [];
        $scope.searchTicketScope = {};
        $scope.viewTicketScope = {};
        $scope.viewChangeCancelTicketScope = {};
        $scope.searchTicketModel = {};
        $scope.ticketPriceInfos = {};
        $scope.sapCodeDataSource = [];
        $scope.isViewMode = false;
        $scope.commentModel = [];
        $scope.deleteModel = [];
        $scope.checkBookingViewConfirms = false;
        $scope.checkInfoConfirms = 0;
        $scope.processingStageRound = 0;
        $scope.isITEdit = false;
        $scope.contactAdmin = $translate.instant("BTA_CONTACT_ITADMIN");

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
            { name: $translate.instant('BTA_INTERNATIONAL'), value: 2 },
            { name: $translate.instant('BTA_INTERNATIONAL_NO_FLIGHT'), value: 4 }
        ];
        $scope.roundTripOption = [
            { name: " ", value: "" },
            { name: $translate.instant('COMMON_ONEWAY'), value: 1 },
            { name: $translate.instant('COMMON_ROUNDTRIP'), value: 2 },
        ];
        $scope.model.type = ""; // set type Dosmetic
        $scope.model.isRoundTrip = ""; // set type Round Trip
        $scope.model.dataRoundTrip = "";

        $scope.readReason = true;

        $scope.btaTypeChange = function (type) {
            if (type == '1' || type == '3') {

                var result = _.find($scope.dataPartitions, x => { return x.code.toLowerCase() == 'domestic' });
                if (result) {
                    var dataGridBTADetail = getDataGrid('#btaListDetailGrid');
                    if (dataGridBTADetail.length > 0) {
                        dataGridBTADetail.forEach(item => {

                            $timeout(function () {
                                let id = '#partitionSelect' + item.no;
                                id.attr("disabled", "disabled");
                            }, 0);
                            item.partitionId = result.id;
                            item.partitionName = result.name;
                            item.partitionCode = result.code;
                        })
                        setDataSourceForGrid(`#btaListDetailGrid`, dataGridBTADetail);
                    }

                }
            } else {
                var dataGridBTADetail = getDataGrid('#btaListDetailGrid');
                if (dataGridBTADetail.length > 0) {
                    dataGridBTADetail.forEach(item => {
                        $timeout(function () {
                            let id = '#partitionSelect' + item.no;
                            id.removeAttr("disabled").removeAttr("readonly");
                        }, 0);
                        item.partitionId = null;
                        item.partitionName = null;
                        item.partitionCode = null;
                    })
                    setDataSourceForGrid(`#btaListDetailGrid`, dataGridBTADetail);
                }
            }
        };
        var btaListDetailGridColumns = [
            {
                field: "no",
                headerTemplate: $translate.instant('COMMON_NO'),
                width: "50px",
                locked: true,
                editable: function (e) {
                    return false;
                },
            },
            {
                field: "isBookingContact",
                headerTemplate: $translate.instant('BTA_IS_BOOKING_CONTACT'),
                width: "150px",
                locked: true,
                hidden: $scope.model.type != null && ($scope.model.type === '1' || $scope.model.type === '4'),
                editable: function (e) {
                    return true;
                },
                template: function (dataItem) {
                    return `<input type="checkbox" ng-if="model.type == 2 || model.type == 3" ng-model="dataItem.isBookingContact" name="isBookingContact{{dataItem.no}}"
                    id="isBookingContact{{dataItem.no}}" class="form-check-input" style="margin-left: 10px; vertical-align: middle;" ng-change='ctrlMembership.changeIsBookingContact(dataItem)'/>
                    <label class="form-check-label" for="isBookingContact{{dataItem.no}}"></label>`
                }
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
                field: "partitionCode",
                headerTemplate: $translate.instant('BTA_PARTITION') + '<span class="text-danger">*</span>',
                width: "200px",
                template: function (dataItem) {
                    if ($scope.model.type == 1 || $scope.model.type == 3) {
                        return `<select kendo-drop-down-list style="color: #333;width: 100%;"
                        id="partitionSelect${dataItem.no}"
                        data-k-ng-model="dataItem.partitionCode"
                        k-data-text-field="'name'"
                        k-data-value-field="'code'"
                        k-template="'<span><label>#: name# </label></span>'",
                        k-valueTemplate="'#: name #'",
                        k-auto-bind="'true'"
                        k-value-primitive="'false'"
                        k-data-source="dataPartitions"
                        k-filter="'contains'",
                        k-on-change="changeCommon('partition', dataItem)"
                        ng-disabled="true"
                        > </select>`;
                    }
                    else {
                        return `<select kendo-drop-down-list style="color: #333;width: 100%;"
                        id="partitionSelect${dataItem.no}"
                        data-k-ng-model="dataItem.partitionCode"
                        k-data-value-field="'code'"
                        k-template="'<span>#: data.name# (#: data.code#)</span>'",
                        k-value-template="'<span>#: bta_GetPartitionValueTemplate(data)#'",
                        k-auto-bind="'true'"
                        k-value-primitive="'false'"
                        k-data-source="dataPartitions"
                        k-filter="'contains'",
                        k-on-change="changeCommon('partition', dataItem)"
                        > </select>`;
                    }
                }
            },
            {
                field: "departureCode",
                headerTemplate: $translate.instant('BTA_DEPARTURE') + '<span class="text-danger">*</span>',
                width: "200px",
                template: function (dataItem) {
                    if ($scope.model.type == 1) {
                        return `<select kendo-drop-down-list  style="color: #333;width: 100%;"
                        id="departureSelect${dataItem.no}"
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
                        placeholder="Search airport (KUL,BKK,...)"
                        k-filter="'contains'"
                        k-on-change="changeCommon('departure', dataItem)"
                        > </select>`;
                    }
                }
            },
            {
                field: "arrivalCode",
                headerTemplate: $translate.instant('BTA_ARRIVAL') + '<span class="text-danger">*</span>',
                width: "200px",
                template: function (dataItem) {
                    if ($scope.model.type == 1) {
                        return `<select kendo-drop-down-list id="arrivalSelect${dataItem.no}" style="color: #333;width: 100%;"
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
                        placeholder="Search airport (KUL,BKK,...)"
                        k-filter="'contains'"
                        k-on-change="changeArrival(dataItem)"
                        > </select>`;
                    }
                }
            },
            {
                field: "fromDate",
                headerTemplate: $translate.instant('BTA_TRIP_FROM_DATE') + '<span class="text-danger">*</span>',
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
                headerTemplate: $translate.instant('BTA_TRIP_TO_DATE') + '<span class="text-danger">*</span>',
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
                width: "200px",
                template: function (dataItem) {
                    return `<input type="checkbox" ng-model="dataItem.isForeigner" name="isForeigner{{dataItem.no}}"
                    id="isForeigner{{dataItem.no}}" class="form-check-input" style="margin-left: 10px; vertical-align: middle;"  ng-change='changeForeigner(dataItem)'/>
                    <label class="form-check-label" for="isForeigner{{dataItem.no}}"></label>`
                }
            },
            {
                field: "gender",
                headerTemplate: $translate.instant('COMMON_GENDER') + '<span class="text-danger">*</span>',
                width: "150px",
                template: function (dataItem) {
                    return `<select kendo-drop-down-list style="color: #333;width: 100%;"
                        id="gender${dataItem.no}"
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
                width: "150px",
                template: function (dataItem) {
                    return `<input  type="checkbox" ng-model="dataItem.stayHotel" name="stayHotel{{dataItem.no}}"
                    id="stayHotel${dataItem.no}" class="form-check-input" style="margin-left: 10px; vertical-align: middle;"  ng-change="stayHotelChange(dataItem)"/>
                    <label class="form-check-label" for="stayHotel{{dataItem.no}}"></label>`
                }
            },
            {
                field: "hotelCode",
                headerTemplate: $translate.instant('BTA_HOTEL'),
                width: "220px",
                template: function (dataItem) {
                    return `<select kendo-drop-down-list style="color: #333;width: 90%;"
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
                        > </select>`+ '<span ng-if="dataItem.stayHotel == true"  class="text-danger"> *</span>';
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
                    style="color: #333;width: 90%;" />`+ '<span ng-if="dataItem.stayHotel == true"  class="text-danger"> *</span>';
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
                    style="color: #333;width: 90%;" />`+ '<span ng-if="dataItem.stayHotel == true"  class="text-danger"> *</span>';
                }
            },
            {
                field: "firstName",
                headerTemplate: $translate.instant('BTA_GIVEN_NAME') + '<span ng-if="model.type == 3 || model.type == 2 || model.type == 4" class="text-danger">*</span>',
                width: "200px",
                template: function (dataItem) {
                    return `<input id="firstName${dataItem.no}" name="firstName" autocomplete="do-not-autofill" ng-readonly="false" class="k-input" type="text" ng-model="dataItem.firstName" style="color: #333;width: 100%"/>`;
                }
            },
            {
                field: "lastName",
                headerTemplate: $translate.instant('BTA_SURNAME') + '<span ng-if="model.type == 3 || model.type == 2 || model.type == 4" class="text-danger">*</span>',
                width: "100px",
                template: function (dataItem) {
                    return `<input id="lastName${dataItem.no}" name="lastName" autocomplete="do-not-autofill" ng-readonly="false" class="k-input" type="text" ng-model="dataItem.lastName" style="color: #333;width: 100%"/>`;
                }
            },
            {
                field: "email",
                headerTemplate: $translate.instant('BTA_EMAIL') + '<span ng-if="model.type == 3 || model.type == 2 || model.type == 4" class="text-danger">*</span>',
                width: "200px",
                template: function (dataItem) {
                    return `<input id="email${dataItem.no}" name="email" autocomplete="off" ng-readonly="false" class="k-input" type="text" ng-model="dataItem.email" style="color: #333;width: 100%"/>`;
                }
            },
            {
                field: "mobile",
                headerTemplate: $translate.instant('COMMON_MOBILE') + '<span ng-if="model.type == 3 || model.type == 2 || model.type == 4" class="text-danger">*</span>',
                width: "120px",
                template: function (dataItem) {
                    return `<input id="mobile${dataItem.no}" name="mobile" autocomplete="off" class="k-input" type="text" ng-model="dataItem.mobile" style="color: #333;width: 92%"
                            ng-keyup="phoneNumberKeyPress(event.keyCode, dataItem)"/>`;
                }
            },
            {
                field: "idCard",
                //headerTemplate: $translate.instant('COMMON_MOBILE'),
                headerTemplate: $translate.instant('BTA_ID_CARD') + '<span ng-if="model.type == 3" class="text-danger">*</span>',
                width: "120px",
                template: function (dataItem) {
                    return `<input id="idCard${dataItem.no}"  name="idCard" autocomplete="off" class="k-input" maxlength="12" type="text" ng-model="dataItem.idCard" style="color: #333;width: 92%"
                    onkeypress="return event.charCode >= 48 && event.charCode <= 57"/>`
                        ;
                }
            },
            {
                field: "dateOfBirth",
                headerTemplate: $translate.instant('BTA_DATE_OF_BIRTH') + '<span ng-if="model.type == 3 || model.type == 2 || model.type == 4" class="text-danger">*</span>',
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
                headerTemplate: $translate.instant('BTA_PASSPORT') + '<span ng-if="model.type == 2 || model.type == 4" class="text-danger">*</span>',
                width: "120px",
                template: function (dataItem) {
                    return `<input id="passport${dataItem.no}" name="passport" autocomplete="off" class="k-input" type="text" ng-model="dataItem.passport" style="color: #333;width: 90%"/>`
                        + '<span ng-if="dataItem.isForeigner == true"  class="text-danger"> *</span>';
                }
            },
            {
                field: "Country",
                headerTemplate: $translate.instant('BTA_COUNTRY') + '<span ng-if="model.type == 2" class="text-danger">*</span>',
                width: "200px",
                template: function (dataItem) {
                    $scope.countryFromGotadiOption = new kendo.data.DataSource({
                        transport: {
                            read: async function (options) {
                                let searchParam = { "queryText": "" };
                                if (!$.isEmptyObject(options.data) && !$.isEmptyObject(options.data.filter) && !$.isEmptyObject(options.data.filter) && !$.isEmptyObject(options.data.filter.filters)) {
                                    let filter = options.data.filter.filters;
                                    if (filter.length > 0) {
                                        searchParam.queryText = filter[0].value;
                                    }
                                }
                                let searchCountryResult = await btaService.getInstance().bussinessTripApps.searchCountry(searchParam, null).$promise;
                                $scope.searchCountryResult = searchCountryResult.object;
                                let countryInfos = searchCountryResult.object;

                                let btaDetailItems = $("#btaListDetailGrid").data("kendoGrid").dataSource._data;
                                if (!$.isEmptyObject(btaDetailItems) && btaDetailItems.length > 0) {
                                    let countryInfoInfos = $(btaDetailItems).map(function (cIndex, cItem) {
                                        return JSON.parse(cItem.countryInfo);
                                    });
                                    departureInfos = $.grep(countryInfoInfos, function (el, i) {
                                        let checkResult = $(countryInfos).filter(function (cIndex, cItem) {
                                            return el.code == cItem.code;
                                        });
                                        if (checkResult.length == 0) {
                                            countryInfos.push(el);
                                        }
                                    });
                                }

                                options.success(countryInfos);
                            }
                        },
                        serverFiltering: true
                    });
                    return `<select kendo-drop-down-list style="color: #333;width: 90%;"
                        id="countrySelect${dataItem.no}"
                        data-k-ng-model="dataItem.countryCode"
                        k-data-value-field="'code'"
                        k-template="'<span> #: data.name# (#: data.code#) </span>'",
                        k-value-template="'<span>#: bta_GetCountryValueTemplate(data)#'",
                        k-auto-bind="'true'"
                        k-value-primitive="'false'"
                        k-data-source="countryFromGotadiOption"
                        placeholder="Search country (vietnam, ... )"
                        k-filter="'contains'"
                        k-on-change="changeCountry(dataItem)"
                        > </select>` + `<span ng-if="model.type == 2"  class="text-danger"> *</span>`;
                }
            },
            {
                field: "passportDateOfIssue",
                headerTemplate: $translate.instant('BTA_PASSPORT_DATE_OF_ISSUE') + '<span ng-if="model.type == 2 || model.type == 4" class="text-danger">*</span>',
                width: "200px",
                format: "{0:dd/MM/yyyy}",
                template: function (dataItem) {
                    return `<input kendo-date-picker
                    id="passportDateOfIssue${dataItem.no}"
                    autocomplete="off" 
                    k-ng-model="dataItem.passportDateOfIssue"
                    k-date-input="true"
                    k-format="'dd/MM/yyyy'"
                    style="color: #333;width: 90%;" />`
                        + '<span ng-if="(model.type == 2 || model.type == 4) && dataItem.isForeigner == true"  class="text-danger"> *</span>';
                }
            },
            {
                field: "passportExpiryDate",
                headerTemplate: $translate.instant('BTA_PASSPORT_EXPIRY_DATE_ISSUE') + '<span ng-if="model.type == 2 || model.type == 4" class="text-danger">*</span>',
                width: "200px",
                format: "{0:dd/MM/yyyy}",
                template: function (dataItem) {
                    return `<input kendo-date-picker
                    id="passportExpiryDate${dataItem.no}"
                    autocomplete="off"
                    k-ng-model="dataItem.passportExpiryDate"
                    k-date-input="true"
                    k-format="'dd/MM/yyyy'"
                    style="color: #333;width: 90%;" />`
                        + '<span ng-if="(model.type == 2 || model.type == 4) && dataItem.isForeigner == true"  class="text-danger"> *</span>';
                }
            },
            {
                field: "Check Budget",
                headerTemplate: 'Check Budget' + '<span class="text-danger">*</span>',
                width: "200px",
                template: function (dataItem) {
                    return `<select kendo-drop-down-list
                    id="hasBudget${dataItem.no}"
                    style="color: #333;width: 100%;"
                    data-k-ng-model="dataItem.hasBudget"
                    k-data-text-field="'name'"
                    k-data-value-field="'code'"
                    k-template="'<span><label>#: name# </label></span>'",
                    k-valueTemplate="'#: name #'",
                    k-auto-bind="'true'"
                    k-value-primitive="'false'"
                    k-data-source="dataBudget"
                    k-filter="'contains'"
                    > </select>`;
                }
            },
            {
                field: "membershipCount",
                headerTemplate: $translate.instant('BTA_MEMBERSHIP_INFORMATION'),
                width: "150px",
                template: function (dataItem) {
                    return `<a ng-click="ctrlMembership.showMembershipDialog(dataItem);">${dataItem.memberships && dataItem.memberships != undefined ? JSON.parse(dataItem.memberships).length : 0}</a>`;
                }
            },
            {
                field: "rememberInformation",
                headerTemplate: $translate.instant('BTA_REMEMBER_INFORMATION'),
                width: "200px",
                template: function (dataItem) {
                    return `<input type="checkbox" ng-model="dataItem.rememberInformation" name="rInfo{{dataItem.no}}"
                    id="rInfo{{dataItem.no}}" class="form-check-input" style="margin-left: 10px; vertical-align: middle;"/>
                    <label class="form-check-label" for="rInfo{{dataItem.no}}"></label>`
                }
            },
            {
                headerTemplate: $translate.instant('COMMON_ACTION'),
                width: "180px",
                template: function (dataItem) {
                    if ($scope.model.type === 3 || $scope.model.type === 2 || $scope.model.type === 4) {//booking flight WF
                        $scope.isBookingFlightWF = true;
                    }
                    if (($scope.isBookingFlightWF && ($scope.model.status === "Completed" || $scope.model.status === "Completed Changing" || $scope.model.status === "Cancelled Booking"))
                        || ($scope.model.status === 'Waiting for Admin Checker' || $scope.model.status === 'Waiting for Admin Manager Approval' || $scope.model.status === 'Waiting for Manager (G5) Approval')) {
                        return `<a class="btn btn-outline-dark rounded-3 btn-sm poppins-regular display-9 m-1" ng-class="{'display-none': !isBookingFlightWF}" ng-click="viewTicketForCurrentUser(dataItem.sapCode, dataItem.id, true, isChangingAdminCheckerStep, true)">{{'BTA_VIEW_TICKET'|translate}}</a>`;
                    }
                    return `<a class="btn-border-upgrade btn-delete-upgrade" ng-click="deleteRecord('btaListDetailGrid', dataItem.no)" ></a> `;
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
                width: "50px",
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
                        k-ng-disable="true"
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
                        k-disable="true"
                        k-format="'dd/MM/yyyy HH:mm'"
                        k-time-format="'HH:mm'"
                        style="color: #333;width: 100%;" 
                        ng-class="{'disabled': dataItem.isCancel}"/>`;
                }
            },
            {
                field: "newFromDate",
                headerTemplate: $translate.instant('BTA_TRIP_NEW_FROM_DATE'),
                width: "220px",
                //format: "{0:dd/MM/yyyy}",
                template: function (dataItem) {
                    return `<input kendo-date-time-picker
                        id="newFromDateCancel${dataItem.no}" 
                        k-ng-model="dataItem.newFromDate"
                        k-date-input="true"
                        k-format="'dd/MM/yyyy HH:mm'"
                        k-time-format="'HH:mm'"
                        k-on-change="fromDateGridChangingCancellingChanged(dataItem)"
                        style="color: #333;width: 100%;" 
                        ng-class="{'disabled': dataItem.isCancel}"/>`;
                }
            },
            {
                field: "newToDate",
                headerTemplate: $translate.instant('BTA_TRIP_NEW_TO_DATE'),
                width: "220px",
                //format: "{0:dd/MM/yyyy}",
                template: function (dataItem) {
                    return `<input kendo-date-time-picker
                        id="newToDateCancel${dataItem.no}" 
                        k-ng-model="dataItem.newToDate"
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
                    else if ($scope.model.type == 2 || $scope.model.type == 4 || $scope.model.type == 3) {

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
                    id="isCancel{{dataItem.no}}" class="form-check-input" style="margin-left: 10px; vertical-align: middle;" ng-change='changeCancel(dataItem)'/>
                    <label class="form-check-label" for="isCancel{{dataItem.no}}"></label>`
                }
            },
            {
                field: "reason",
                // headerTemplate: $translate.instant('BTA_DESTINATION'),
                headerTemplate: $translate.instant('COMMON_REASON'),
                width: "350px",
                template: function (dataItem) {
                    return `<input id="reason${dataItem.no}" ng-keyup="reasonKeyUp(dataItem)" name="others" class="k-input w100" type="text" style="color: #333;" ng-model="dataItem.reason" kendo-tooltip k-content="'${dataItem.reason}'"/>`;
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
                        return `<a class="btn btn-outline-dark rounded-3 btn-sm poppins-regular display-9 m-1" ng-class="{'display-none': !isBookingFlightWF}" ng-click="viewTicketForCurrentUser(dataItem.sapCode, dataItem.businessTripApplicationDetailId, true, isChangingAdminCheckerStep)">{{'BTA_VIEW_TICKET'|translate}}</a>`;
                    }
                    return `<a class="btn btn-outline-dark rounded-3 btn-sm poppins-regular display-9 m-1" ng-class="{'display-none': !isBookingFlightWF}" ng-click="viewTicketForCurrentUser(dataItem.sapCode, dataItem.businessTripApplicationDetailId, false, isChangingAdminCheckerStep)">{{'BTA_VIEW_TICKET'|translate}}</a><a id="delete{{dataItem.no}}" class="btn-border-upgrade btn-delete-upgrade"  ng-click="deleteRecord('btaListChangeOrCancelGrid', dataItem.no)" ></a>`;
                }
            },
        ]

        //lamnl add btaListOverBudgetGridColumns
        var btaListOverBudgetGridColumns = [
            {
                field: "no",
                headerTemplate: $translate.instant('COMMON_NO'),
                width: "40px",
                editable: function (e) {
                    return false;
                }
            },
            {
                field: "referenceNumber",
                headerTemplate: $translate.instant('COMMON_REFERENCE_NUMBER'),
                width: "180px",
                locked: false,
                template: function (dataItem) {
                    return `<a ui-sref="home.over-budget.item({referenceValue: '${dataItem.referenceNumber}', id: '${dataItem.id}'})" ui-sref-opts="{ reload: true }">${dataItem.referenceNumber}</a>`;
                }
            },
            {
                field: "comment",
                headerTemplate: $translate.instant('COMMON_REASON'),
                width: "200px",
            },
            {
                field: "created",
                headerTemplate: $translate.instant('COMMON_CREATED_DATE'),
                width: "200px",
                template: function (dataItem) {
                    return moment(dataItem.created).format(appSetting.longDateFormat);
                }
            },
            {
                field: "status",
                //title: "Status",
                headerTemplate: $translate.instant('COMMON_STATUS'),
                width: "300px",
                locked: false,
                template: function (data) {
                    var statusTranslate = $rootScope.getStatusTranslate(data.status);
                    return `<workflow-status status="${statusTranslate}"></workflow-status>`;
                }
            }
        ]
        //
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

        $scope.reasons = [];
        $scope.reasonSelected = {};

        $scope.reasonOfListOptions = {
            dataTextField: "name",
            dataValueField: "name",
            template: '<span><label>#: data.name# </label></span>',
            valueTemplate: '#: name #',
            filter: "contains",
            dataSource: $scope.reasons,
            filtering: function (ev) {
                var filterValue = ev.filter != undefined ? ev.filter.value : "";
                ev.preventDefault();

                this.dataSource.filter({
                    logic: "or",
                    filters: [{
                        field: "name",
                        operator: "contains",
                        value: filterValue
                    }
                    ]
                });
            },
        }

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
        //lamnl: Confirm Option btaListConfirmGridData
        $scope.btaListOverBudgetGridOptions = {
            dataSource: {
                data: $scope.btaListOverBudgetData
            },
            sortable: false,
            pageable: false,
            columns: btaListOverBudgetGridColumns
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

    window.bta_GetCountryValueTemplate = function (data) {
        if ($.isEmptyObject(data) || data == "") {
            return "";
        }
        return `${data.name} (${data.code})`;
    }

    window.bta_GetPartitionValueTemplate = function (data) {
        if ($.isEmptyObject(data) || data == "") {
            return "";
        }
        return `${data.name}(${data.code})`;
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
        if (dataItem.newFromDate) {
            if (dataItem.newFomDate) {
                $(id).data('kendoDateTimePicker').min(new Date(dataItem.newFomDate));
            }
            var resultDayToDate = dateFns.getDayOfYear(dataItem.newToDate);
            var resultDayFromDate = dateFns.getDayOfYear(dataItem.newFomDate);
            var resultHourToDate = dateFns.getHours(dataItem.newToDate);
            var resultHourFromDate = dateFns.getHours(dataItem.newFomDate);
            if (dataItem.newToDate && resultDayToDate < resultDayFromDate) {
                dataItem.newToDate = dataItem.newFromDate;
            }
            else if (dataItem.newToDate && resultDayToDate == resultDayFromDate && resultHourToDate <= resultHourFromDate) {
                dataItem.newToDate = dataItem.newFromDate;
            }
        }
        if (checkInId || checkOutId) {
            if (dataItem.newFromDate)
                $(checkInId).data('kendoDateTimePicker').min(new Date(dataItem.newFromDate));
            $(checkInId).data('kendoDateTimePicker').max(new Date(dataItem.newToDate));
            $(checkOutId).data('kendoDateTimePicker').min(new Date(dataItem.newFromDate));
            $(checkOutId).data('kendoDateTimePicker').max(new Date(dataItem.newToDate));
            if (!isAction) {
                if (dataItem.newCheckInHotelDate && dataItem.newCheckInHotelDate >= dataItem.newFromDate && dataItem.newCheckInHotelDate <= dataItem.newToDate) {

                } else dataItem.newCheckInHotelDate = null;

                if (dataItem.newCheckOutHotelDate && dataItem.newCheckOutHotelDate >= dataItem.newFromDate && dataItem.newCheckOutHotelDate <= dataItem.newToDate) {

                } else dataItem.newCheckOutHotelDate = null;
                /*dataItem.newCheckInHotelDate = null;
                dataItem.newCheckOutHotelDate = null;*/
            }
        }
    };

    $scope.toDateGridChangingCancellingChanged = function (dataItem) {
        let checkInId = '#newCheckInHotelDate' + dataItem.no;
        let checkOutId = '#newCheckOutHotelDate' + dataItem.no;
        if (checkInId || checkOutId) {
            $(checkInId).data('kendoDateTimePicker').min(new Date(dataItem.newFromDate));
            $(checkInId).data('kendoDateTimePicker').max(new Date(dataItem.newToDate));
            $(checkOutId).data('kendoDateTimePicker').min(new Date(dataItem.newFromDate));
            $(checkOutId).data('kendoDateTimePicker').max(new Date(dataItem.newToDate));
            /*dataItem.newCheckInHotelDate = null;
            dataItem.newCheckOutHotelDate = null;*/
            if (dataItem.newCheckInHotelDate && dataItem.newCheckInHotelDate >= dataItem.newFromDate && dataItem.newCheckInHotelDate <= dataItem.newToDate) {

            } else dataItem.newCheckInHotelDate = null;

            if (dataItem.newCheckOutHotelDate && dataItem.newCheckOutHotelDate >= dataItem.newFromDate && dataItem.newCheckOutHotelDate <= dataItem.newToDate) {

            } else dataItem.newCheckOutHotelDate = null;
        }

        if (dataItem.newToDate < dataItem.newFromDate) {
            dataItem.newToDate = dataItem.newFromDate;
        }
    }

    $scope.fromDateCheckInOutGridChangingCancellingChanged = function (dataItem) {
        let id = "#newCheckOutHotelDate" + dataItem.no;
        if (dataItem.newCheckInHotelDate) {
            $(id).data('kendoDateTimePicker').min(new Date(dataItem.newCheckInHotelDate));
            $(id).data('kendoDateTimePicker').max(new Date(dataItem.newToDate));
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
            if (dataItem.newToDate)
                $(id).data('kendoDateTimePicker').max(new Date(dataItem.newToDate));

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
            case 'roomNotBookingGrid':
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
                        let currentDataOnGrid = $("#roomNotBookingGrid").data("kendoGrid").dataSource._data;
                        let grid = $("#roomNotBookingGrid").data("kendoGrid");
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

    $scope.changeCountry = async function (dataItem, isEdit = false) {
        var result = _.find($scope.searchCountryResult, x => { return x.code == dataItem.countryCode });
        if (result) {
            dataItem.countryName = result.name;
            dataItem.countryInfo = JSON.stringify(result);
        }
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
            case 'partition':
                if ($scope.model.type == 1) {
                    var result = _.find($scope.dataPartitions, x => { return x.code == dataItem.partitionCode });
                    if (result) {
                        dataItem.partitionName = result.name;
                        dataItem.partitionCode = result.code;
                        dataItem.partitionId = result.id;
                    }
                }
                else {
                    var partitionSelect = $("#partitionSelect" + dataItem.no).data("kendoDropDownList");
                    if (!$.isEmptyObject(partitionSelect)) {
                        var partitionInfo = partitionSelect.dataItem();
                        if (!$.isEmptyObject(partitionInfo)) {
                            dataItem.partitionName = partitionInfo.name + " (" + partitionInfo.code + ")";
                            dataItem.partitionCode = partitionInfo.code;
                            dataItem.partitionId = partitionInfo.id;
                            dataItem.partitionInfo = JSON.stringify(partitionInfo);
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

    $scope.dataPartitions = [];
    async function getPartitions() {
        $scope.dataPartitions = [];
        let queryArgs = {
            predicate: '',
            predicateParameters: [],
            order: appSetting.ORDER_GRID_DEFAULT,
            page: 1,
            limit: 10000
        };

        var res = await settingService.getInstance().partition.getListPartitions(queryArgs).$promise;
        if (res.isSuccess) {
            $scope.dataPartitions = res.object.data;
        }
    }

    $scope.dataBudget = [
        {
            code: '',
            name: ''
        },
        {
            code: 1,
            name: 'Budget'
        },
        {
            code: 0,
            name: 'Non-Budget'
        }];

    async function getAirlines() {
        let queryArgs = {
            predicate: '',
            predicateParameters: [],
            order: appSetting.ORDER_GRID_DEFAULT,
            page: 1,
            limit: 10000000
        };
        var res = await settingService.getInstance().airline.getAirlines(queryArgs).$promise;
        if (res.isSuccess) {
            $scope.dataAirlines = [];
            res.object.data.forEach(element => {
                $scope.dataAirlines.push(element);
            });
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
            if ($scope.model && $scope.model.referenceNumber) {
                if ($scope.model.referenceNumber !== 'BTA-000001343-2023' && $scope.model.referenceNumber !== 'BTA-000001289-2023') {
                    var resValidate = await validateFromToDateBusinessDetail(dataGridBTADetail); // validate trên phiếu khác
                    $scope.errors = translateValidate(resValidate);
                }
            } else {
                var resValidate = await validateFromToDateBusinessDetail(dataGridBTADetail); // validate trên phiếu khác
                $scope.errors = translateValidate(resValidate);
            }
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
                //carRental Attach
                if ($scope.carRentalCtrl.carRentalAttachmentDetails.length || $scope.carRentalCtrl.removeFileCarRentals.length) {
                    let attachmentFilesJson = await $scope.carRentalCtrl.mergeAttachment();
                    $scope.model.carRentalAttachmentDetails = attachmentFilesJson;
                }
                //visa Attach
                if ($scope.visaCtrl.visaAttachmentDetails.length || $scope.visaCtrl.removeFileVisas.length) {
                    let attachmentFilesJson = await $scope.visaCtrl.mergeAttachment();
                    $scope.model.visaAttachmentDetails = attachmentFilesJson;
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
        else {
            var res = { messages: $scope.errors };
            return res;
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
            title: $translate.instant('COMMON_MOBILE_NUMBER_REQUIRED'),
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
        //$scope.model.carRental = !$scope.isChecked ? "" : $scope.model.carRental;
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
                //if ($scope.isChecked && !$scope.model.carRental) {
                //    errors.push({ controlName: $translate.instant('CAR_RENTAL'), message: ': ' + $translate.instant('COMMON_FIELD_IS_REQUIRED') });
                //}
                break;
            case 2:
                dataGrid.forEach(item => {
                    if (item.hotelCode) {
                        errors = errors.concat(validationForTable(item, propertiesBTADetailHaveHotel, item.no));
                    }
                    else {
                        errors = errors.concat(validationForTable(item, propertiesBTADetail, item.no));
                    }

                    if ($scope.model.type == 2 || $scope.model.type == 4 || $scope.model.type == 3) {
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

                    if ($scope.model.type === "2" && $.isEmptyObject(item.countryCode)) {
                        errors.push({ controlName: `${item.sapCode} - ${item.fullName}: ${$translate.instant('BTA_COUNTRY')}`, message: ': ' + $translate.instant('COMMON_FIELD_IS_REQUIRED') });
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

                    var checkBudget = true;
                    var givenTimestamp = new Date('2024-07-09T00:00:00.0000000+07:00');
                    if ($scope.model.created && $scope.model.created != null && $scope.model.created != '') {
                        var createdTimestamp = new Date($scope.model.created);
                        if (createdTimestamp < givenTimestamp) {
                            checkBudget = false;
                        }
                    }
                    if (($scope.model.type == 2 || $scope.model.type == 3) && (item.hasBudget == null || item.hasBudget === '') && checkBudget) {
                        errors.push({
                            controlName: $translate.instant('HAS_BUDGET_COLUMN'),
                            message: ': ' + $translate.instant('COMMON_FIELD_IS_REQUIRED')
                        });
                    }
                });

                if (!$scope.model.requestorNoteDetail) {
                    errors.push({
                        controlName: $translate.instant('COMMON_REQUESTOR_NOTE_DETAIL'),
                        message: ': ' + $translate.instant('COMMON_FIELD_IS_REQUIRED')
                    });
                }

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
                else if (!$scope.model.dataRoundTrip && ($scope.model.type == 2 || $scope.model.type == 3)) {
                    errors.push({
                        controlName: $translate.instant('COMMON_TICKETTYPE'),
                        message: ': ' + $translate.instant('COMMON_FIELD_IS_REQUIRED')
                    });
                }
                else if ($scope.isChecked) {
                    if (!$scope.carRentalCtrl.carRentalAttachmentDetails.length && !$scope.carRentalCtrl.oldCarRentalAttachmentDetails.length) {
                        errors.push({
                            controlName: $translate.instant('CAR_RENTAL_ATTACHMENT_DETAIL'),
                            message: ': ' + $translate.instant('CAR_RENTAL_ATTACHMENTFILE_IS_REQUIRED_VALIDATE')
                        });
                    }
                }
                else if ($scope.isVisaChecked && ($scope.model.id == null || $scope.model.created > '2023-07-20T06:08:57.6000444+07:00')) {
                    if (!$scope.visaCtrl.visaAttachmentDetails.length && !$scope.visaCtrl.oldVisaAttachmentDetails.length) {
                        errors.push({
                            controlName: $translate.instant('VISA_ATTACHMENT_DETAIL'),
                            message: ': ' + $translate.instant('VISA_ATTACHMENTFILE_IS_REQUIRED_VALIDATE')
                        });
                    }
                }

                if (errors.length <= 0 && ($scope.model.type == 2 || $scope.model.type == 3)) {
                    // Check BookingContact 
                    let dataBookingContactGrid = getDataGrid('#bookingContactGrid');
                    if (dataBookingContactGrid && dataBookingContactGrid.length > 0) {
                        let err = [];
                        err = validationForTable(dataBookingContactGrid[0], $scope.ctrlMembership.requiredBookingContactFields, '');
                        if (!err || err.length <= 0)
                            $scope.model.bookingContact = JSON.stringify(dataBookingContactGrid);
                        else if (err.length == 4) {
                            let checkIsContact = $.grep(dataGrid, function (item) {
                                return item.isBookingContact;
                            });
                            if (checkIsContact == null || checkIsContact == undefined || checkIsContact.length <= 0) {
                                errors.push({
                                    controlName: $translate.instant('BTA_BOOKING_CONTACT'),
                                    message: ': ' + $translate.instant('BTA_IS_BOOKING_CONTACT_REQUIRE')
                                });
                            }
                        } else {
                            errors = errors.concat(err);
                        }
                    } else {
                        let checkIsContact = $.grep(dataGrid, function (item) {
                            return item.isBookingContact;
                        });
                        if (checkIsContact == null || checkIsContact == undefined || checkIsContact.length <= 0) {
                            errors.push({
                                controlName: $translate.instant('BTA_BOOKING_CONTACT'),
                                message: ': ' + $translate.instant('BTA_IS_BOOKING_CONTACT_REQUIRE')
                            });
                        }
                    }
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

    function checkDuplicateNewFromToDate(item) {
        let errors = [];
        var resultDay = dateFns.differenceInDays(item.newToDate, item.newFromDate);
        var resultHours = dateFns.differenceInHours(item.newToDate, item.newFromDate);
        var resultMinutes = dateFns.differenceInMinutes(item.newToDate, item.newFromDate);
        if (resultDay == 0 && resultHours == 0 && resultMinutes == 0) {
            errors.push({ controlName: $translate.instant('COMMON_ROW_NO') + ' ' + item.no + ' ' + $translate.instant('BTA_NEW_FROM_DATE_AND_NEW_TO_DATE'), message: ': ' + moment(item.fromDate).format(appSetting.longDateFormat) + ' ' + $translate.instant('BTA_DUPLICATE_DATE') });
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

                if (allowAddError && !model.stayHotel && (field.property == 'checkInHotelDate' || field.property == 'checkOutHotelDate')) {
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
        await getPartitions();
        await getAirlines();
        await getBTAReasons();

        $scope.isBookingFlightWF = false;

        if ($stateParams.id) {
            await getBusinessTripLocationById($stateParams.id);
            await checkAdminDept();
            await getWorkflowProcessingStage($stateParams.id);

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
                $("#carAttachmentDetail").removeAttr("disabled").removeAttr("readonly");
                $("#visaAttachmentDetail").removeAttr("disabled").removeAttr("readonly");
                $("#attachmentChanging").attr("disabled", "disabled");
                $(".deleteattachmentChanging").addClass("display-none");
                $(".deleteattachmentDetail").removeClass("display-none");
                $(".deleteCarAttachmentDetail").removeClass("display-none");
                $(".deleteVisaAttachmentDetail").removeClass("display-none");
            }, 0);
        }

        setTimeout(function () {
            document.querySelectorAll('.tooltip-custom').forEach((tooltip) => {
                const tooltipText = tooltip.querySelector('.tooltip-text');
                const parentRect = document.querySelector('.portlet-body').getBoundingClientRect();
                tooltip.addEventListener('mouseenter', function () {
                    const tooltipRect = tooltipText.getBoundingClientRect();
                    if (tooltipRect.right > parentRect.right) {
                        tooltip.classList.add('tooltip-left');
                    }
                });
            });
        }, 10);

        $scope.$apply();
    }
    $scope.isRoom = false;
    $scope.statusDraf = true;
    async function getBusinessTripLocationById(id) {
        var res = await btaService.getInstance().bussinessTripApps.getItemById({ id: id }, null).$promise;
        if (res.isSuccess) {
            $scope.model = res.object;
            $scope.statusTranslate = $rootScope.getStatusTranslate($scope.model.status);
            $scope.businessTripApplicationStatus = res.object.status;
            $scope.businessApplicationStatus = $scope.model.status;
            $scope.model.userCreatedFullName = $scope.model.userFullName;
            var dropdownDeptLine = $("#dept_line_id").data("kendoDropDownList");
            var dropdownDivision = $("#division_id").data("kendoDropDownTree");
            var dropdownReason = $("#reasonDropdown").data("kendoDropDownList");
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
            //if ($scope.model.carRental) {
            //    $scope.isChecked = true;
            //}
            $scope.departureOption = businessTripLocations;
            $scope.arrivalOption = businessTripLocations;
            setDataSourceForGrid('#btaListDetailGrid', $scope.model.businessTripDetails);
            $scope.btaListDetailGridOptions.dataSource.data = $scope.model.businessTripDetails;
            //lamnl check room confirm
            let currentRoomType = _.find($scope.model.businessTripDetails, x => { return x.checkBookingCompleted });
            if (currentRoomType) {
                $scope.checkBookingViewConfirms = true;
            }


            //// lamnl
            //$scope.model.checkRoomNextStep = isCheckRoomHotel();
            if ($scope.model.businessOverBudgets) {
                $scope.btaListOverBudgetData = $scope.model.businessOverBudgets;
                let count = 0;
                $scope.btaListOverBudgetData.forEach(item => {
                    count++;
                    item.no = count;
                })
                setDataSourceForGrid('#btaOverBudgetGrid', $scope.btaListOverBudgetData);
            }
            if ($scope.model.bookingContact) {
                $scope.convertBookingContact = JSON.parse($scope.model.bookingContact);
                setDataSourceForGrid('#bookingContactGrid', $scope.convertBookingContact);
            } else {
                $scope.convertBookingContact = [{
                    firstName: '',
                    lastName: '',
                    email: '',
                    mobile: ''
                }];
                setDataSourceForGrid('#bookingContactGrid', $scope.convertBookingContact);
            }
            //
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
                    if ($rootScope.currentUser.isAdmin && ($scope.model.status === "Waiting for Admin Checker" || $scope.model.status === "Waiting for Admin Manager Approval" || $scope.model.status === "Waiting for Manager (G5) Approval")) {
                        $scope.oldAttachmentDetails = JSON.parse($scope.model.documentDetails);
                    } else if ($scope.model.status === "Completed" || $scope.model.status === "Completed Changing") {
                        $scope.oldAttachmentDetails = JSON.parse($scope.model.documentDetails);
                    }
                }
            }
            if ($scope.model.documentChanges) {
                $scope.oldAttachmentChanges = JSON.parse($scope.model.documentChanges);
            }
            if ($scope.model.carRentalAttachmentDetails) {
                $scope.carRentalCtrl.oldCarRentalAttachmentDetails = JSON.parse($scope.model.carRentalAttachmentDetails);
                $scope.isChecked = true;
            }
            if ($scope.model.visaAttachmentDetails) {
                $scope.visaCtrl.oldVisaAttachmentDetails = JSON.parse($scope.model.visaAttachmentDetails);
                $scope.isVisaChecked = true;
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
            if (dropdownReason) {
                dropdownReason.value($scope.model.requestorNote);
            }
            //$("#carRental_id").attr("disabled", "disabled");
            kendo.ui.progress($("#loading_bta"), false);

            if ($scope.model.type === 3 || $scope.model.type === 2 || $scope.model.type === 4) {//booking flight WF
                $scope.isBookingFlightWF = true;
            }
            if ($scope.model.type === 1 || $scope.model.type === 4) {
                $scope.btaListDetailGridOptions.columns[1].hidden = true;
                var grid = $("#btaListDetailGrid").data("kendoGrid");
                if (grid && grid != undefined) {
                    var column = grid.columns.find(column => column.field === "isBookingContact");
                    column.hidden = true;
                    grid.setOptions($scope.btaListDetailGridOptions);
                    grid.refresh();
                }
            }

            if (!$scope.model.isRoundTrip) {
                $scope.model.dataRoundTrip = 1;
            } else if ($scope.model.isRoundTrip) {
                $scope.model.dataRoundTrip = 2;
            }

            var grid = $("#btaListDetailGrid").data("kendoGrid");
            if (grid && grid != undefined) {
                grid.dataSource._data.forEach(dataItem => {
                    if (dataItem.stayHotel) {
                        let hotelId = '#hotel' + dataItem.no
                        getHotels(dataItem.isForeigner, dataItem.arrivalCode, hotelId, dataItem.hotelCode);
                    }
                });
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

    function filterArrayGroup(array1, array2) {
        var result = _.filter(array1, function (item) { return !array2.map(a => a.id).includes(item.id) });
        return result;
    }

    $scope.addMore = async function () {
        $scope.isEditViewTicket = true;
        $scope.userChangingCanceling = getDataGrid("#btaListDetailGrid");
        let dataBtaListChangeOrCancelGrid = getDataGrid('#btaListChangeOrCancelGrid');
        //if (dataBtaListChangeOrCancelGrid.length > 0) {
        //    $scope.userChangingCanceling = filterArray($scope.userChangingCanceling, dataBtaListChangeOrCancelGrid);
        //}
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
                        /*id: item.id,*/
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
                        newToDate: item.toDate,
                        newFromDate: item.fromDate,
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
                        setDataSourceForGrid('#roomNotBookingGrid', dataTemporary);
                    }
                    else {
                        setDataSourceForGrid('#roomNotBookingGrid', $scope.rooms);
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
                    setDataSourceForGrid('#roomNotBookingGrid', dataTemporary);
                }
                else {
                    setDataSourceForGrid('#roomNotBookingGrid', $scope.rooms);
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
            let dialog = $("#dialog_Room_NotBookingId").data("kendoDialog");
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
                        //if (!flag) {
                        //    let arrayChooseUser = [];
                        //    currentRoomOrganizations.forEach(item => {
                        //        item.peoples.forEach(x => {
                        //            arrayChooseUser.push(x);
                        //        });
                        //    });
                        //    if (arrayChooseUser.length < arrayFilterHotel.length) {
                        //        errorsDetail.push({ message: $translate.instant('BTA_CHOOSE_USER_MESSAGE') });
                        //        flag = true;
                        //    }
                        //}
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

    $scope.dialog_Room_NotBookingOption = {
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
                let gridRoom = $("#roomNotBookingGrid").data("kendoGrid");
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
                        //if (!flag) {
                        //    let arrayChooseUser = [];
                        //    currentRoomOrganizations.forEach(item => {
                        //        item.peoples.forEach(x => {
                        //            arrayChooseUser.push(x);
                        //        });
                        //    });
                        //    if (arrayChooseUser.length < arrayFilterHotel.length) {
                        //        errorsDetail.push({ message: $translate.instant('BTA_CHOOSE_USER_MESSAGE') });
                        //        flag = true;
                        //    }
                        //}
                    }
                    else {
                        // Notification.error('Table Business Trip Application Detail: ' + $translate.instant('COMMON_FIELD_IS_REQUIRED'));
                        errorsDetail.push({ message: $translate.instant('BTA_TABLE_DETAIL_REQUIRED') + '' + $translate.instant('COMMON_FIELD_IS_REQUIRED') });
                        flag = true;
                    }
                }
                if (!flag) {
                    let dialog = $("#dialog_RoomConfirmNotBooking").data("kendoDialog");
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
                let tripGroup = 0;
                gridRoom.dataSource._data.toJSON().forEach(item => {
                    if (item.peoples.length) {
                        let value = {
                            id: item.id,
                            roomTypeId: item.roomTypeId,
                            roomTypeCode: item.roomTypeCode,
                            roomTypeName: item.roomTypeName,
                            users: mapUser(item.peoples, idGrid),
                            tripGroup: item.tripGroup
                        }
                        roomDetail.push(value);
                    }
                    tripGroup = item.tripGroup;
                });
                let model = {
                    businessTripApplicationId: $stateParams.id,
                    data: JSON.stringify(roomDetail),
                    tripGroup: tripGroup,
                    isChange: isChangeRoom
                }

                //
                $scope.saveRoomBookingFlight(model);

                ///
                //luu thong tin BTA detail
                let dataGridBTADetail = getDataGrid('#btaListDetailGrid');
                dataGridBTADetail.forEach(item => {
                    item.fromDate = new Date(item.fromDate);
                    item.toDate = new Date(item.toDate);
                });
                $scope.model.businessTripDetails = JSON.stringify(dataGridBTADetail);
                //check Room hotel next step
                //$scope.model.checkRoomNextStep = isCheckRoomHotel();
                $scope.saveBTADetail($scope.model);

                //$scope.saveTicketBookingFlight(tripGroup);
                //saveRoomOrganization(model);
                let dialog = $("#dialog_Room").data("kendoDialog");
                dialog.close();
                //continueWorkFlow();
                return true;
            },
            primary: true
        }]
    };



    $scope.peoples = []
    $scope.peopleDataSourceArray = [];
    $scope.rooms = [];
    // lamnl Room not Booking
    $scope.roomNotBookingOptions = {
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
                    return `<a class="btn-border-upgrade btn-delete-upgrade" ng-click="deleteRecord('roomNotBookingGrid', dataItem.no)"></a>`;
                }
            }
        ]
    }
    $scope.dialog_RoomConfirmNotBookingOption = {
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
                let gridRoom = $("#roomNotBookingGrid").data("kendoGrid");
                gridRoom.dataSource._data.toJSON().forEach(item => {
                    if (item.peoples.length) {
                        let value = {
                            id: item.id,
                            roomTypeId: item.roomTypeId,
                            roomTypeCode: item.roomTypeCode,
                            roomTypeName: item.roomTypeName,
                            users: mapUser(item.peoples, idGrid),
                            tripGroup: 0
                        }
                        roomDetail.push(value);
                    }
                });
                let model = {
                    businessTripApplicationId: $stateParams.id,
                    data: JSON.stringify(roomDetail),
                    tripGroup: 0,
                    isChange: isChangeRoom
                }

                //
                //$scope.saveRoomBookingFlight(model);
                saveRoomOrganization(model);
                let dialog = $("#dialog_Room_NotBookingId").data("kendoDialog");
                dialog.close();
                continueWorkFlow();
                return true;
            },
            primary: true
        }]
    };

    //end 

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
                let count = 0;
                array.forEach(items => {
                    let sapCodess = _.groupBy(items.users, 'sapCode');
                    Object.keys(sapCodess).forEach(y => {
                        if (y == x) {
                            count++;
                        }
                    });
                });
                if (count > 1) {
                    error.push({
                        row: item.no,
                        message: $translate.instant('SAP_DUPLICATED')
                    });
                }
            });

        });
        return error;
    }


    //lamnl
    $scope.saveRoomBookingFlight = async function (model) {
        var res = await btaService.getInstance().bussinessTripApps.saveRoomBookingFlight(model).$promise;
        if (res.isSuccess) {
            // $state.go($state.current.name, { id: $scope.model.id, referenceValue: $scope.model.referenceNumber }, { reload: true });
            var ress = await btaService.getInstance().bussinessTripApps.getBtaRoomHotel({ id: $scope.model.id }, null).$promise;
            if (ress.isSuccess) {
                $scope.model.roomOrganizations = ress.object;
                if ($scope.model.roomOrganizations && $scope.model.roomOrganizations.length > 0) {
                    let dataTemporary = $scope.model.roomOrganizations;
                    let arrayUsers = [];
                    count = 1;
                    dataTemporary.forEach(item => {
                        item['no'] = count;
                        item['showUsers'] = splitNameUsers(item.users);
                        count++;

                    });
                    setDataSourceForGrid('#roomViewGrid', dataTemporary);

                }
            }
            Notification.success($translate.instant('COMMON_SAVE_SUCCESS'));
        } else {
            Notification.error("Error System");
        }
        return true;
    }
    //

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
                    return `<a class="btn-border-upgrade btn-delete-upgrade" ng-click="deleteRecord('roomGrid', dataItem.no)"></a>`;
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
        let dataGrid = getDataGrid("#roomNotBookingGrid");
        dataGrid.push(value)
        let count = 1;
        dataGrid.forEach(item => {
            item.no = count;
            count++;
        });
        let id = "#peopleOption" + value.no;
        //mapDataToPeopleOption(getDataGrid("#btaListDetailGrid"), id);
        mapDataToPeopleOption(arrayFilterHotel, id);
        setDataSourceForGrid('#roomNotBookingGrid', dataGrid);
    }

    arrayRoom = [];
    $scope.addRoomBookingFlight = function (tripGroup) {
        let value = {
            no: $scope.rooms.length + 1,
            roomTypeId: '',
            roomTypeCode: '',
            roomTypeName: '',
            peoples: '',
            tripGroup: tripGroup
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
                        userGradeId: item.jobGradeId,
                        userGradeValue: item.jobGradeValue,
                        departmentId: item.departmentId,
                        rememberInformation: false,
                        countryCode: item.countryCode,
                        countryName: item.countryName,
                        countryInfo: null,
                        memberships: item.memberships,
                        hasBudget: item.hasBudget
                    };
                    $scope.btaListDetailGridData.push(value);
                });
                let grid = $('#btaListDetailGrid').data("kendoGrid");
                if (grid) {
                    let i = 1;
                    $scope.btaListDetailGridData.forEach(item => {
                        if ($scope.model.type == 1 || $scope.model.type == 3) {
                            var result = _.find($scope.dataPartitions, x => { return x.code.toLowerCase() == 'domestic' });
                            if (result) {
                                item.partitionId = result.id;
                                item.partitionName = result.name;
                                item.partitionCode = result.code;
                            }
                        }
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
                title: "<input type='checkbox' ng-model='allCheck' name='allCheck' id='allCheck' class='form-check-input' ng-change='onChange(allCheck)' style='margin-left: 10px;'/> <label class='form-check-label' for='allCheck' style='padding-bottom: 10px;'></label>",
                width: "50px",
                template: function (dataItem) {
                    return `<input type="checkbox" ng-model="dataItem.isCheck" name="isCheck{{dataItem.no}}"
                    id="isCheck{{dataItem.no}}" class="form-check-input" style="margin-left: 10px; vertical-align: middle;" ng-change='changeCheck(dataItem)'/>
                    <label class="form-check-label" for="isCheck{{dataItem.no}}"></label>`
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
                //field: "jobGrade",
                field: "jobGradeTitle",
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
            await getSapCodeByDivison($scope.model.deptDivisionId, option.data.page, option.data.take, $scope.keyWorkTemporary, true);
        } else if ($scope.model.deptLineId && !$scope.model.deptDivisionId) {
            await getSapCodeByDivison($scope.model.deptLineId, option.data.page, option.data.take, $scope.keyWorkTemporary, true);
            /*await getUserCheckedHeadCountByDeptLine($scope.model.deptLineId, option.data.take, option.data.page, $scope.keyWorkTemporary);*/
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
                    result = await settingService.getInstance().users.getChildUsers({ departmentId: $scope.model.deptLineId, limit: 100000, page: 1, searchText: $scope.gridUser.keyword ? $scope.gridUser.keyword.trim() : "" }).$promise;
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
    async function getSapCodeByDivison(deptId, page, limit, searchText = "", isAll = false) {
        if (deptId) {
            let result = await settingService.getInstance().users.getChildUsers({ departmentId: deptId, limit: limit, page: page, searchText: searchText, isAll: isAll }).$promise;
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
                result = await settingService.getInstance().users.getChildUsers({ departmentId: $scope.model.deptLineId, limit: 100000, page: 1, searchText: $scope.gridUser.keyword ? $scope.gridUser.keyword.trim() : "" }).$promise;
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
                    await getSapCodeByDivison($scope.model.deptDivisionId, 1, 100000, $scope.keyWorkTemporary, true);
                } else if ($scope.model.deptLineId && !$scope.model.deptDivisionId) {
                    await getSapCodeByDivison($scope.model.deptLineId, 1, 100000, $scope.keyWorkTemporary, true);
                    /*await getUserCheckedHeadCountByDeptLine($scope.model.deptLineId, 10000, 1, $scope.keyWorkTemporary);*/
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
            await getSapCodeByDivison($scope.model.deptDivisionId, 1, 100000, $scope.keyWorkTemporary, true);
        } else if ($scope.model.deptLineId && !$scope.model.deptDivisionId) {
            await getSapCodeByDivison($scope.model.deptLineId, 1, 100000, $scope.keyWorkTemporary, true);
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
                        memberships: ""
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
            <div class="d-flex align-items-center">
                <div class="col-md-4" style="margin-top: -25px;">
                    <i class="k-icon k-i-arrow-chevron-left" style="margin-left: -10px;" ng-click="backPopupReview()"></i>
                    <span style="color: #006EFA !important;font-family: Nova-Regular !important;font-size: 18px !important;font-weight: 700; margin: 10px 0;margin: 0;=line-height: 1.42857143;=text-transform: uppercase !important;">${$translate.instant('COMMON_BUTTON_REVIEWUSER')}</span>
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
    $scope.showTrackingHistory = function () {
        $rootScope.visibleTrackingHistory($translate, appSetting.TrackingLogDialogDefaultWidth);
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
                let disableAttachmentItems = ['attachmentDetail', 'attachmentChanging', 'carAttachmentDetail', 'visaAttachmentDetail'];
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
                        $("#carAttachmentDetail").attr("disabled", "disabled");
                        $("#visaAttachmentDetail").attr("disabled", "disabled");
                        $("#attachmentChanging").attr("disabled", "disabled");
                        $(`.deleteattachmentDetail`).addClass("display-none");
                        $(`.deleteCarAttachmentDetail`).addClass("display-none");
                        $(`.deleteVisaAttachmentDetail`).addClass("display-none");
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
                $("#carAttachmentDetail").attr("disabled", "disabled");
                $("#visaAttachmentDetail").attr("disabled", "disabled");
                $("#attachmentChanging").attr("disabled", "disabled");
                $(`.deleteattachmentDetail`).addClass("display-none");
                $(`.deleteCarAttachmentDetail`).addClass("display-none");
                $(`.deleteVisaAttachmentDetail`).addClass("display-none");
                $(`.deleteattachmentChanging`).addClass("display-none");
            }, 0);
        }
        $timeout(function () {
            if (data != null && data.workflowInstances && data.workflowInstances.length) {
                customCssFieldWhenDisable();
            }
            disableCheckInOutAllGrid();
        }, 0);
        $timeout(function () {
            if (data && data.allowToVote && $scope.model.isRequestToChange && $scope.model.isRequestToChange == true && $scope.model.status.toLowerCase() == 'Waiting For Admin Checker'.toLowerCase()) {
                disableElement(true);
                $("#attachmentDetail").removeAttr("disabled");
                $("#carAttachmentDetail").removeAttr("disabled");
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
            if (!cancellationColumns.some(column => column.field === "penaltyFee")) {
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

        //lamnl: Edit column custom
        if ($scope.checkAdminDeptView) {
            $timeout(function () {
                let step = _.find(data.workflowInstances[0].workflowData.steps, x => {
                    return x.customEventKey == 'bookingFlights';
                });

                //data.workflowInstances.workflowData.steps
                //let enableItems = [];
                var dataGridBTADetail = getDataGrid('#btaListDetailGrid');
                if (dataGridBTADetail.length > 0) {
                    dataGridBTADetail.forEach(item => {
                        if (step.restrictedProperties && step.restrictedProperties.length) {
                            step.restrictedProperties.forEach(x => {
                                //enableItems.push(x.fieldPattern);
                                $(`#${x.fieldPattern + item.no}`).removeAttr("disabled").removeAttr("readonly");
                                $(`.delete${x.fieldPattern + item.no}`).removeClass("display-none");
                            })
                        }
                    })
                }


            }, 0)
        }
        if ($scope.model.status === 'Waiting for Booking Flight') {
            $timeout(function () {

                $(`#budgetNote`).removeAttr("disabled").removeAttr("readonly");
                //$(`.delete$budgetNote`).removeClass("display-none");
            }, 0)
        }

        // Enable checkbox is cancel when change business trip button appears
        if (data.workflowButtons && data.workflowButtons.length) {
            let changeBusinessTrip = _.find(data.workflowButtons, x => {
                return x.name === 'Change Business Trip';
            });
            if (changeBusinessTrip) {
                let dataChanging = getDataGrid('#btaListChangeOrCancelGrid');
                dataChanging.forEach(item => {
                    $("#isCancel" + item.no).removeAttr("disabled");
                })
            }
        }

        if (data.currentStatus == "Completed" || $scope.forceChangeBusinessTrip) {
            $("#addMore").show();
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
                                if (propValue == x.sapCode) {
                                    if (item['newFromDate'] == x.fromDate && item['newToDate'] == x.toDate && item['destinationCode'] == x.arrivalCode && item['newHotelCode'] == x.hotelCode && item['newFlightNumberCode'] == x.flightNumberCode && item['newComebackFlightNumberCode'] == x.comebackFlightNumberCode && !item['isCancel']) {
                                        errors.push({
                                            //fieldName: 'List Changing/ Cancelling Business Trip',
                                            controlName: $translate.instant('COMMON_ROW_NO') + ` ${item['no']}`,
                                            errorDetail: $translate.instant('BTA_NOT_CHANGE_ANY_TIMELINE'),
                                            isRule: false,
                                        });
                                    }
                                }
                            });
                        /*case 'fromDate':
                            if (!propValue) {
                                errors.push({
                                    controlName: $translate.instant('BTA_TRIP_FROM_DATE') + ` ${item['no']}`,
                                    errorDetail: $translate.instant('COMMON_FIELD_IS_REQUIRED'),
                                    isRule: false,
                                });
                            }
                            break;
                        case 'toDate':
                            if (!propValue) {
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
                            break;*/
                        case 'newFromDate':
                            if (!propValue) {
                                errors.push({
                                    controlName: $translate.instant('BTA_TRIP_NEW_FROM_DATE') + ` ${item['no']}`,
                                    errorDetail: $translate.instant('COMMON_FIELD_IS_REQUIRED'),
                                    isRule: false,
                                });
                            }
                            break;
                        case 'newToDate':
                            if (!propValue) {
                                errors.push({
                                    controlName: $translate.instant('BTA_TRIP_NEW_TO_DATE') + ` ${item['no']}`,
                                    errorDetail: $translate.instant('COMMON_FIELD_IS_REQUIRED'),
                                    isRule: false,
                                });
                            }
                            else {
                                let err = checkDuplicateNewFromToDate(item);
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
                            if (!propValue /*&& !item['isCancel']*/) {
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
        let newFromDateCancelId = '#newFromDateCancel' + item.no;
        let newToDateCancelId = '#newToDateCancel' + item.no;
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
        if ($scope.model && $scope.model.status && $scope.model.status === "Waiting for Change Business Trip") {
            $(fromDateCancelId).data("kendoDateTimePicker").enable(false);
            $(toDateCancelId).data("kendoDateTimePicker").enable(false);
            $(newFromDateCancelId).data("kendoDateTimePicker").enable(value);
            $(newToDateCancelId).data("kendoDateTimePicker").enable(value);
            $(destinationId).data("kendoDropDownList").enable(value);
            $(flightNumberId).data("kendoDropDownList").enable(value);
            $(comebackFlightNumber).data("kendoDropDownList").enable(value);
            $("#attachmentChanging").removeAttr("disabled", "disabled");
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

        $('button:contains("Request Budget")').removeAttr("disabled");
        $('button:contains("Delete")').removeAttr("disabled");

        $('button:contains("Confirm All")').removeAttr("disabled");

        var tabstrip = $("#tabstrip").kendoTabStrip().data("kendoTabStrip");
        tabstrip.select(0);

        if ($scope.model.roomOrganizations && $scope.model.roomOrganizations.length > 0) {
            let dataTemporary = $scope.model.roomOrganizations;
            let arrayUsers = [];
            count = 1;
            dataTemporary.forEach(item => {
                item.users.forEach(items => {
                    arrayUsers.push(items.userId);
                })

            });
            //lamnl 07/04
            let flag = 0;

            let gridbtaListDetailGrid = $("#btaListDetailGrid").data("kendoGrid");
            gridbtaListDetailGrid.dataSource._data.forEach(items => {
                let data = _.find(arrayUsers, x => { return x == items.userId });
                if (data && data != null) {
                    flag = flag;
                } else {
                    flag++;
                }
            });

            if (flag == 0) {
                $('button:contains("Confirm")').removeAttr("disabled");
            }
            //
        }


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
                        footerTemplate: "footerTemplate footerTemplate footerTemplate footerTemplate footerTemplate footerTemplate footerTemplate footerTemplate footerTemplate footerTemplate "
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
        if (actionName == 'reviewTicket' || actionName == 'removeTicket' || actionName == 'saveTicketBooking' || actionName == 'checkStatusBooking') {
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

        if (!$.isEmptyObject(groupItems) && groupItems.length > 0) { //!
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

        let griddataTicket = $("#btaTripGroupHasTicketGrid").data("kendoGrid").dataSource._data;
        griddataTicket = $(griddataTicket).map(function (cIndex, cItem) {
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
                    //tudm: 1 group chi 9 nguoi
                    if (passengerOfSourceTripGroup.length >= 9) {
                        returnValue = false;
                    }
                    else {
                        let firstPassenger = passengerOfSourceTripGroup[0];

                        returnValue = (firstPassenger.arrivalCode == passengerItem.arrivalCode
                            && firstPassenger.departureCode == passengerItem.departureCode
                            && moment(firstPassenger.fromDate).format('YYYYMMDD') == moment(passengerItem.fromDate).format('YYYYMMDD')
                            && moment(firstPassenger.toDate).format('YYYYMMDD') == moment(passengerItem.toDate).format('YYYYMMDD')
                        );
                    }

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
    $scope.tripGroupRoom = 0;
    //tudm
    $scope.roomBookingFlight = async function (group) {
        $scope.tripGroupRoom = group;
        var res = await btaService.getInstance().bussinessTripApps.getBtaRoomHotel({ id: $scope.model.id }, null).$promise;
        if (res.isSuccess) {
            $scope.model.roomOrganizations = res.object;
        }

        $scope.errors = validationBTADetail(); // validate trên cùng 1 phiếu
        $scope.$applyAsync();
        if (!$scope.errors.length) {
            $scope.roomOrganizationErrorsMessages = [];
            peoplesTotal = [];
            let passengerInfos = $scope.prepareTripGroupHasTicketRoomHotel(); // lay data da book ticket
            var passengerTripList = $.grep(passengerInfos, function (e) { return e.tripGroup == group && e.flightDetails.length > 0; });
            isChangeRoom = false;
            let gridbtaListDetailGrid = $("#btaListDetailGrid").data("kendoGrid");
            idGrid = "#btaListDetailGrid";
            arrayFilterHotel = [];
            gridbtaListDetailGrid.dataSource._data.forEach(item => {
                if (item.hotelCode && item.checkInHotelDate && item.checkOutHotelDate && _.find(passengerTripList, x => { return x.id == item.id })) {
                    arrayFilterHotel.push(item);
                }
            });
            mapDataToPeopleOption(arrayFilterHotel);

            if ($scope.model.roomOrganizations != null) {
                let dataTemporary = $.grep($scope.model.roomOrganizations, function (e) { return e.tripGroup == group });
                if (dataTemporary != null && dataTemporary.length > 0) {
                    let count = 1;
                    dataTemporary.forEach(item => {
                        item['no'] = count;
                        item['peoples'] = initArrayValueUsers(item.users);
                        item['tripGroup'] = group;
                        count++;
                    });
                    setDataSourceForGrid('#roomGrid', dataTemporary);

                }
                else {
                    setDataSourceForGrid('#roomGrid', $scope.rooms);
                    $scope.addRoomBookingFlight(group);
                }
                if ($scope.model.roomorganizations) {
                    $scope.model.roomorganizations.foreach(item => {
                        item.peoples.foreach(x => {
                            peoplestotal.push(x)
                        });
                    });
                }
            } else {
                setDataSourceForGrid('#roomGrid', $scope.rooms);
                $scope.addRoomBookingFlight(group);
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
            //{
            //    text: $translate.instant('COMMON_BUTTON_BOOK_TICKET'),
            //    action: function (e) {
            //        $scope.checkBooingTicket();
            //        return false;
            //    },
            //    primary: true
            //},
            {
                text: $translate.instant('COMMON_BUTTON_CONFIRM_ALL_GROUP'),
                action: function (e) {
                    $scope.confirmBeforeBookings(0);
                    return false;
                },
                primary: true
            },
            {
                text: $translate.instant('BTA_REQUEST_BUDGET'),
                action: function (e) {
                    $scope.requestOverBudget();
                    return false;
                },
                primary: true
            },
            {
                text: $translate.instant('COMMON_BUTTON_DELETE'),
                action: function (e) {
                    $scope.requestOverBudgetDelete();
                    return false;
                },
                primary: true
            }

            //,
            //{
            //    text: $translate.instant('COMMON_BUTTON_SAVE'),
            //    action: function (e) {
            //        $scope.saveTicket(true);
            //        return $.isEmptyObject($scope.tripGroupPopup.errorMessages) || $scope.tripGroupPopup.errorMessages.length == 0;
            //    },
            //    primary: true
            //}
        ]
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
                $scope.btaListTripGroupTicketGridData = [];
                $scope.tripGroupArray = new Array();
                $scope.allowSetTripGroupHasTicket = true;
                let tripGroupTicketInfo = {};

                // prepare data has ticket and non ticket
                $scope.btaListTripGroupTicketGridData = $.grep($scope.btaListTripGroupGridData, function (e) { return e.checkBookingCompleted; });

                if ($scope.btaListTripGroupTicketGridData.length > 0) {
                    $scope.btaListTripGroupGridData = filterArrayGroup($scope.btaListTripGroupGridData, $scope.btaListTripGroupTicketGridData);
                }


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
                        footerTemplate: "footerTemplate footerTemplate footerTemplate footerTemplate footerTemplate footerTemplate footerTemplate footerTemplate footerTemplate footerTemplate "
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
                                //clear data isCommit
                                if ($scope.btaListTripGroupGridData.length > 0) {
                                    let dataCommitBooking = $scope.btaListTripGroupGridData.find(e => e.tripGroup == cItem.tripGroup);
                                    if (dataCommitBooking) {
                                        cItem.isCommitBooking = dataCommitBooking.isCommitBooking;
                                        cItem.bookingCode = dataCommitBooking.bookingCode;
                                        cItem.bookingNumber = dataCommitBooking.bookingNumber;
                                    }
                                }
                            });
                        }
                        $scope.btaTripGroupBindDataComplted = true;
                    }
                };
                //tuhm group has ticket
                $scope.tripGroupHasTicketGridOptions = {
                    dataSource: {
                        data: $scope.btaListTripGroupTicketGridData,
                        sort: [{ field: "tripGroup", dir: "asc" }, { field: "fullName", dir: "desc" }],
                        group: [{ field: "tripGroup" }],
                        footerTemplate: "footerTemplate footerTemplate footerTemplate footerTemplate footerTemplate footerTemplate footerTemplate footerTemplate footerTemplate footerTemplate "
                    },
                    sortable: false,
                    pageable: false,
                    columns: tripGroupHasTicketColumns,
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
                                    $("#btaTripGroupHasTicketGrid tbody").find("tr[data-uid=" + uid + "]").addClass("rOverBudget");  //alarm's in my style and we call uid for each row
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
            width: "120px",
            headerTemplate: $translate.instant('COMMON_FULL_NAME'),
            editable: function (e) {
                return false;
            },
            tempalte: function (dataItem) {
                return `<input ng-readonly="true" class="k-input" type="text" ng-model="dataItem.fullName" style="width: 100%"/>`;
            }
        },
        {
            field: "partitionName",
            width: "160px",
            headerTemplate: $translate.instant('BTA_PARTITION'),
            editable: function (e) {
                return false;
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
            //field: "jobGrade",
            field: "userGradeTitle",
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
                    return `<button ng-show='doesAllowShowTripGroupButton(` + data.value + `, "reviewTicket")' class="btn btn-sm btn-primary"  ng-click='showViewTicketPopup(` + data.value + `)'>` + $translate.instant('COMMON_BUTTON_VIEW_TICKET') + `</button>`;
                }
                if (!data.items[0].isCommitBooking)
                    return `<button data-ng-disabled= 'disableButton(1,"removeTicket")' ng-show='doesAllowShowTripGroupButton(` + data.value + `, "removeTicket")' class="btn btn-sm btn-primary" style='line-height: 0.8 !important;' ng-click='showPopupConfirmRemoveTicket(` + data.value + `)'>` + $translate.instant('COMMON_BUTTON_DELETE_TICKET') + `</button>
                            <button data-ng-disabled= 'disableButton(1,"reviewTicket")' ng-show='doesAllowShowTripGroupButton(` + data.value + `, "reviewTicket")' class="btn btn-sm btn-primary" style='line-height: 0.8 !important;'  ng-click='showViewTicketPopup(` + data.value + `)'>` + $translate.instant('COMMON_BUTTON_VIEW_TICKET') + `</button>
                            <button data-ng-disabled= 'disableButton(1,"searchTicket")' ng-show='doesAllowShowTripGroupButton(` + data.value + `, "searchTicket")' class="btn btn-sm btn-primary" style='line-height: 0.8 !important;' ng-click='tripGroupSearchTicket(` + data.value + `)'>` + $translate.instant('COMMON_BUTTON_SEARCH_TICKET') + `</button>
                            <button data-ng-disabled= 'disableButton(1,"saveTicketBooking")' ng-show= 'doesAllowShowTripGroupButton(` + data.value + `, "saveTicketBooking")' class="btn btn-sm btn-primary" style='line-height: 0.8 !important;' ng-click='confirmBeforeBookings(` + data.value + `)'>` + $translate.instant('BTA_SAVE_TICKET_BUDGET') + `</button>`;
                else
                    return `<button data-ng-disabled= 'disableButton(1,"checkStatusBooking")' class="btn btn-sm btn-primary" style='line-height: 0.8 !important;' ng-click='checkStatusBookingCtrl.showDialog("` + data.items[0].bookingNumber + `",` + data.value + `,"` + data.items[0].bookingCode + `")'>` + $translate.instant('BTA_COMMITBOOKING_CHECK_STATUS_BTN') + `</button>`;
            },
            editable: function (e) {
                return false;
            },
            template: function (dataItem) {
                return `<input ng-readonly="true" class="k-input" type="text" ng-model="dataItem.comments" style="width: 100%"/>`;
            }
        },
        {
            field: "requestBudget",
            headerTemplate: $translate.instant('Request Budget'),
            format: "{0:n0}",
            width: "200px",
            editable: function (e) {
                return false;
            },
            template: function (dataItem) {
                if (dataItem.referenceNumberOverBudget != null) {
                    return `<a href="" ng-click = "clickLinkOverBudget(dataItem.businessTripOverBudgetId, dataItem.referenceNumberOverBudget)">${dataItem.referenceNumberOverBudget}</a>`;
                } else {
                    return ``;//ui-sref="home.over-budget.item({referenceValue: '${dataItem.referenceNumberOverBudget}', id: '${dataItem.businessTripOverBudgetId}'})" ui-sref-opts="{ reload: true }"
                }

            }
        }
    ]

    var tripGroupHasTicketColumns = [
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
            width: "120px",
            headerTemplate: $translate.instant('COMMON_FULL_NAME'),
            editable: function (e) {
                return false;
            },
            tempalte: function (dataItem) {
                return `<input ng-readonly="true" class="k-input" type="text" ng-model="dataItem.fullName" style="width: 100%"/>`;
            }
        },
        {
            field: "partitionName",
            width: "160px",
            headerTemplate: $translate.instant('BTA_PARTITION'),
            editable: function (e) {
                return false;
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
            field: "userGradeTitle",
            //field: "jobGrade",
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
                return `<label style="margin-left: .25em;">${dataItem.tripGroup}</label>`;

            }
        },
        {
            field: "comments",
            headerTemplate: $translate.instant('BTA_OVER_BUDGET_REASON'),
            width: "280px",
            groupFooterTemplate: function (data) {
                if ($scope.isViewMode) {
                    return `<kendo-button  data-ng-disabled= 'disableButton(2,"reviewTicket2")' ng-show='doesAllowShowTripGroupButton(` + data.value + `, "reviewTicket")' class="k-primary float-right"  ng-click='showViewTicketPopup(` + data.value + `)'>` + $translate.instant('COMMON_BUTTON_VIEW_TICKET') + `</kendo-button>`;
                }
                return `<kendo-button <kendo-button data-ng-disabled= 'disableButton(2,"reviewTicket2")' ng-show='true' class="k-primary float-right btn-sm" style='line-height: 0.8 !important;'  ng-click='showViewTicketPopup(` + data.value + `)'>` + $translate.instant('COMMON_BUTTON_VIEW_TICKET') + `</kendo-button>
                        <kendo-button <kendo-button data-ng-disabled= 'disableButton(2,"roomBookingFlight")' ng-show= 'true' class="k-primary float-right btn-sm" style='line-height: 0.8 !important;' ng-click='roomBookingFlight(` + data.value + `)'>` + $translate.instant('BTA_ROOM_BUDGET') + `</kendo-button>`;
            },
            editable: function (e) {
                return false;
            },
            template: function (dataItem) {
                return `<input ng-readonly="true" class="k-input" type="text" ng-model="dataItem.comments" style="width: 100%"/>`;
            }
        },
        {
            field: "requestBudget",
            headerTemplate: $translate.instant('Request Budget'),
            format: "{0:n0}",
            width: "200px",
            editable: function (e) {
                return false;
            },
            template: function (dataItem) {
                if (dataItem.referenceNumberOverBudget != null) {
                    return `<a href="" ng-click = "clickLinkOverBudget(dataItem.businessTripOverBudgetId, dataItem.referenceNumberOverBudget)">${dataItem.referenceNumberOverBudget}</a>`;
                } else {
                    return ``;//ui-sref="home.over-budget.item({referenceValue: '${dataItem.referenceNumberOverBudget}', id: '${dataItem.businessTripOverBudgetId}'})" ui-sref-opts="{ reload: true }"
                }

            }
        }
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
    //lamnl
    $scope.clickLinkOverBudget = function (id, referenceNumber) {
        let dialog = $("#dialog_TripGroup").data("kendoDialog")
        dialog.close();

        $state.go("home.over-budget.item", { id: id, referenceValue: referenceNumber }, { reload: true });
    }
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
                let dialog = $("#dialog_ViewTicket").data("kendoDialog");
                dialog.title('View Ticket');
                dialog.open();
            }
        }
    }
    function showSearchFlightPopup(searchTicketModel) {
        if (!$.isEmptyObject(searchTicketModel)) {
            if (!$.isEmptyObject($scope.searchTicketScope.loadFlight)) {//!
                $scope.searchTicketScope.setTripGroupHasTicket = $scope.setTripGroupHasTicket;
                $scope.searchTicketScope.loadFlight(searchTicketModel);
                // set title cho cái dialog
                //let dialog = $("#dialog_SearchTicket").data("kendoDialog")
                //dialog.title($translate.instant('COMMON_BUTTON_SEARCH_TICKET'));
                //dialog.open();
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
            var res = await bookingFlightService.bookingTicket(res.object, $stateParams.id);
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
                if ($scope.model.type === '1' || $scope.model.type === '2') {
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



    //==================================================== Request Budget =======================================================//
    $scope.requestOverBudgetDialogOption = {
        width: "600px",
        buttonLayout: "normal",
        closable: true,
        modal: true,
        visible: false,
        content: "",
        actions: [{
            text: $translate.instant('COMMON_BUTTON_OK'),
            action: function (e) {
                let passengerInfos = $scope.prepareTripGroupNoTicket();
                var passengerList = $.grep(passengerInfos, function (e) { return _.find($scope.commentModel.tripGroup, x => { return x == e.tripGroup }); });

                $scope.commentModel.errorMessages = new Array();
                $scope.commentModel.errorMessages = validationRequestBudget(1, passengerList);
                if (!$scope.commentModel.errorMessages.length > 0) {

                    passengerList.forEach(item => {
                        item.userGradeValue = item.jobGrade;
                        item.BTADetailId = item.id;

                        if (item.flightDetails) {
                            item.flightDetails.forEach(items => {
                                if (items && items.id == null) {
                                    items.id = '00000000-0000-0000-0000-000000000000';
                                }
                            })
                            item.flightDetails = item.flightDetails;

                            // luu thong tin ve may bay
                            if (item.flightDetails[0] != null) {
                                item.departureSearchId = item.flightDetails[0].departureSearchId;
                                item.titleDepartureFareRule = item.flightDetails[0].titleDepartureFareRule;
                                item.detailDepartureFareRule = item.flightDetails[0].detailDepartureFareRule;
                            }
                            if (item.flightDetails[1] != null) {
                                item.returnSearchId = item.flightDetails[1].returnSearchId;
                                item.titleReturnFareRule = item.flightDetails[1].titleReturnFareRule;
                                item.detailReturnFareRule = item.flightDetails[1].detailReturnFareRule;
                            }
                        } else {
                            Notification.error($translate.instant('COMMON_SAVE_SUCCESS'));
                            return false;
                        }
                    })
                    let model = {
                        Id: $scope.model.id,
                        OverBudgetInfos: JSON.stringify(passengerList),
                        Comment: $scope.commentModel.comment
                    }
                    $scope.saveRequestOverBudgets(model);

                    // set title cho cái dialog
                    /*let dialog = $("#requestOverBudgetDialogId").data("kendoDialog")
                    dialog.close();
                    let dialog2 = $("#dialog_TripGroup").data("kendoDialog");
                    dialog2.close();*/
                } else {
                    $scope.$apply();
                    return false;
                }

            },
            primary: true
        }],
        close: async function (e) {
        }
    };

    function validationRequestBudget(type, passengerInfo) {
        let errors = [];
        if (type == 1) {
            if (!$scope.commentModel.comment || $scope.commentModel.comment == null) {
                errors.push($translate.instant('COMMON_COMMENT_VALIDATE'));
            }
            if (!$scope.commentModel.tripGroup || $scope.commentModel.tripGroup.length == 0) {
                errors.push($translate.instant('BTA_TRIP_GROUP'));
            }
            if (passengerInfo) {
                let flag = true;
                passengerInfo.forEach(item => {
                    if (!item.flightDetails.length > 0) {
                        flag = false;
                    }
                });
                if (!flag) {
                    errors.push($translate.instant('COMMOM_REQUEST_BUDGET_BEFORE'));
                }
            }
        } else {
            if (!$scope.deleteModel.tripGroup || $scope.deleteModel.tripGroup.length == 0) {
                errors.push($translate.instant('BTA_TRIP_GROUP'));
            }
        }
        return errors;
    }

    $scope.saveRequestOverBudgets = async function (model) {
        var res = await btaService.getInstance().bussinessTripApps.saveRequestOverBudget(model).$promise;

        $scope.$apply();
        if (res.isSuccess) {
            $state.go('home.over-budget.item', { id: res.object.id, referenceValue: res.object.referenceNumber }, { reload: true });
            //continueWorkflowNotSave();
            Notification.success($translate.instant('COMMON_SAVE_SUCCESS'));
            return true;
        } else {
            //Notification.error("Group " + res.errorCodes.join(', ') + ': ' + $translate.instant(res.messages[0]));
            Notification.error("SAPCode " + res.errorCodeStr.join(', ') + ': ' + $translate.instant(res.messages[0]));
            return false;
        }

    }
    $scope.deleteTripGroup = async function (passengerList) {
        let dataGridBTADetail = getDataGrid('#btaListDetailGrid');
        var btaDetail = $.grep(dataGridBTADetail, function (e) { return _.find(passengerList, x => { return x.id == e.id }); });
        btaDetail.forEach(item => {
            item.fromDate = new Date(item.fromDate);
            item.toDate = new Date(item.toDate);
        });
        var checkHasBta = dataGridBTADetail.some(e => passengerList.every(x => x.id !== e.id));
        if (!checkHasBta) {
            // check neu xoa het data
            Notification.error("Can not delete all rows!");
            return false;
        }
        var data = JSON.stringify(btaDetail);
        var res = await btaService.getInstance().bussinessTripApps.deleteTripGroup({
            Id: $scope.model.id,
            BtaDetails: data
        }).$promise;

        $scope.$apply();
        if (res.isSuccess) {
            Notification.success($translate.instant('COMMON_DELETE_SUCCESS'));
            var resSendEmail = await btaService.getInstance().bussinessTripApps.sendEmailDeleteRows({ userID: $scope.model.createdById, id: $scope.model.id }).$promise;

            // check all booking ticket and next step
            var resBooking = await btaService.getInstance().bussinessTripApps.checkBookingCompleted({ id: $scope.model.id }).$promise;
            if (resBooking.isSuccess && resBooking.object == true) {
                var resHotel = await btaService.getInstance().bussinessTripApps.checkRoomHotel({ id: $scope.model.id }).$promise;
                if (resHotel.isSuccess) {
                    let vote = await workflowService.getInstance().workflows.vote($scope.wfVoteModel).$promise;
                    if (vote.isSuccess) {
                        $state.go($state.current.name, { id: $scope.model.id, referenceValue: $scope.model.referenceNumber }, { reload: true });
                        Notification.success($translate.instant('COMMON_WORKFLOW_PROCESSED'));
                    }
                }
                else {
                    $state.go($state.current.name, { id: $scope.model.id, referenceValue: $scope.model.referenceNumber }, { reload: true });
                }
            }
            $state.go($state.current.name, { id: res.object.id, referenceValue: res.object.referenceNumber }, { reload: true });
            //continueWorkflowNotSave();
            return true;
        } else {
            Notification.error("Error System");
            return false;
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
    //Delete Bta Detail
    $scope.requestOverBudgetDelete = async function () {
        let passengerInfos = $scope.prepareTripGroupBeforeSave();
        if (passengerInfos.length > 0) {

            $scope.tripGroupBudgetData = [];
            let grid = $('#btaTripGroupGrid').data("kendoGrid");
            let dataSource = grid.dataSource._data;
            var group = prepareGroups(dataSource);
            group.forEach(item => {
                let model = {
                    code: item.group,
                    name: item.group
                };
                $scope.tripGroupBudgetData.push(model);
            });
            $scope.deleteModel.tripGroup = [];
            $timeout(function () {
                $("#tripGroupBudgetDeleteId").removeAttr("disabled").removeAttr("readonly");
            }, 0)
            // set title cho cái dialog
            let dialog = $("#requestOverBudgetDeleteDialogId").data("kendoDialog")
            dialog.title($translate.instant('BTA_OVER_BUDGET_DELETE'));
            dialog.open();

        }
        else {
            let messageArray = passengerHasNoTicket.map((cIndex, cItem) => `${$translate.instant('BTA_TRIP_GROUP')} ${cItem.tripGroup} - ${cItem.sapCode} - ${cItem.fullName} - ${$translate.instant('BTA_HAS_NO_TICKET')}`);
            $scope.tripGroupPopup.errorMessages = messageArray;
            Notification.error($translate.instant('BTA_REQUIRED_CHOOSE_TICKET_FOR_ALL_GROUPS'));
            return false;
        }
    }

    $scope.requestOverBudgetDeleteOption = {
        width: "600px",
        buttonLayout: "normal",
        closable: true,
        modal: true,
        visible: false,
        content: "",
        actions: [{
            text: $translate.instant('COMMON_BUTTON_OK'),
            action: function (e) {
                $scope.deleteModel.errorMessages = new Array();
                $scope.deleteModel.errorMessages = validationRequestBudget(2, null);
                if (!$scope.deleteModel.errorMessages.length > 0) {
                    let passengerInfos = $scope.prepareTripGroupNoTicket();
                    var passengerList = $.grep(passengerInfos, function (e) { return _.find($scope.deleteModel.tripGroup, x => { return x == e.tripGroup }); });

                    passengerList.forEach(item => {
                        item.userGradeValue = item.jobGrade;
                    })

                    $scope.deleteTripGroup(passengerList);

                    // set title cho cái dialog
                    let dialog = $("#requestOverBudgetDialogId").data("kendoDialog")
                    dialog.close();
                    let dialog2 = $("#dialog_TripGroup").data("kendoDialog");
                    dialog2.close();
                } else {
                    $scope.$apply();
                    return false;
                }

            },
            primary: true
        }],
        close: async function (e) {
        }
    };

    $scope.tripGroupBudgetOptions = {
        dataTextField: 'name',
        dataValueField: 'id',
        autoBind: false,
        valuePrimitive: true,
        filter: "contains",
    }
    // tudm over budget
    $scope.requestOverBudget = async function () {
        let passengerInfos = $scope.prepareTripGroupBeforeSave();
        if (passengerInfos.length > 0) {

            $scope.tripGroupBudgetData = [];
            let grid = $('#btaTripGroupGrid').data("kendoGrid");
            let dataSource = grid.dataSource._data;
            var group = prepareGroups(dataSource);
            group.forEach(item => {
                let model = {
                    code: item.group,
                    name: item.group
                };
                $scope.tripGroupBudgetData.push(model);
            });
            $scope.commentModel.tripGroup = [];
            $scope.commentModel.comment = null;
            $timeout(function () {
                $("#tripGroupBudgetId").removeAttr("disabled").removeAttr("readonly");
            }, 0)
            // set title cho cái dialog
            let dialog = $("#requestOverBudgetDialogId").data("kendoDialog")
            dialog.title($translate.instant('BTA_REQUEST_BUDGET'));
            dialog.open();

        }
        else {
            let messageArray = passengerHasNoTicket.map((cIndex, cItem) => `${$translate.instant('BTA_TRIP_GROUP')} ${cItem.tripGroup} - ${cItem.sapCode} - ${cItem.fullName} - ${$translate.instant('BTA_HAS_NO_TICKET')}`);
            $scope.tripGroupPopup.errorMessages = messageArray;
            Notification.error($translate.instant('BTA_REQUIRED_CHOOSE_TICKET_FOR_ALL_GROUPS'));
            return false;
        }
    }
    //lamnl
    $scope.saveTicketBookingFlight = async function (tripGroup) {
        let passengerInfos = $scope.prepareTripGroupNoTicket();

        let passengetTripGroup = passengerInfos.filter(x => x.tripGroup == tripGroup);
        if (passengetTripGroup.length > 0) {
            let flag = true;
            passengetTripGroup.forEach(data => {
                data.businessTripApplicationId = $scope.model.id;

                if (data.isOverBudget) {
                    Notification.error($translate.instant('BTA_OVER_BUDGET_DO_NOT_SAVE_TICKET'));
                    flag = false;
                }
            });
            if (flag) {
                let dialogs = $("#dialog_LoadingBooking").data("kendoDialog");
                dialogs.open();
                var res = await btaService.getInstance().bussinessTripApps.savePassengerInfo(passengetTripGroup).$promise;
                if (res.isSuccess) {
                    var ress = await bookingFlightService.bookingTicket(passengetTripGroup, $stateParams.id);
                    if (ress && ress.isSuccess) {
                        let dialog = $("#dialog_TripGroup").data("kendoDialog");
                        dialog.close();
                        Notification.success($translate.instant('COMMON_SAVE_SUCCESS'));
                        var resBooking = await btaService.getInstance().bussinessTripApps.checkBookingCompleted({ Id: $scope.model.id }).$promise;
                        if (resBooking.isSuccess && resBooking.object == true) {
                            var resHotel = await btaService.getInstance().bussinessTripApps.checkRoomHotel({ Id: $scope.model.id }).$promise;
                            if (resHotel.isSuccess) {
                                let vote = await workflowService.getInstance().workflows.vote($scope.wfVoteModel).$promise;
                                if (vote.isSuccess) {
                                    dialogs.close();
                                    $state.go($state.current.name, { id: $scope.model.id, referenceValue: $scope.model.referenceNumber }, { reload: true });
                                    Notification.success($translate.instant('COMMON_WORKFLOW_PROCESSED'));
                                }
                            } else {
                                dialogs.close();
                                $state.go($state.current.name, { id: $scope.model.id, referenceValue: $scope.model.referenceNumber }, { reload: true });
                            }

                        } else {
                            dialogs.close();
                            $state.go($state.current.name, { id: $scope.model.id, referenceValue: $scope.model.referenceNumber }, { reload: true });
                        }
                        //continueWorkflowNotSave();
                        return true;

                    } else {
                        dialogs.close();
                        // Check Status Booking khi Success = false;
                        if (ress.isCommitBooking != undefined && ress.isCommitBooking) {
                            let griddata = $("#btaTripGroupGrid").data("kendoGrid").dataSource._data;
                            griddata = $(griddata).map(function (index, item) {
                                if (tripGroup == item.tripGroup) {
                                    item.isCommitBooking = ress.isCommitBooking;
                                    item.bookingNumber = ress.bookingNumber;
                                    item.bookingCode = ress.bookingCode;
                                    item.hasTicket = true;
                                }
                            });

                            if ($scope.btaListTripGroupGridData.length > 0) {
                                let index = _.findIndex($scope.btaListTripGroupGridData, x => {
                                    return x.tripGroup == tripGroup;
                                });
                                $scope.btaListTripGroupGridData[index].isCommitBooking = ress.isCommitBooking;
                                $scope.btaListTripGroupGridData[index].bookingNumber = ress.bookingNumber;
                                $scope.btaListTripGroupGridData[index].bookingCode = ress.bookingCode;
                            }
                            RefreshTripGroupGridData(griddata);
                            // $scope.checkStatusBookingCtrl.showDialog(ress.bookingNumber, tripGroup, ress.bookingCode);
                        }

                        $scope.tripGroupPopup.errorMessages = ress;
                        $scope.$apply();
                        return false;
                    }
                } else {
                    dialogs.close();
                    $scope.tripGroupPopup.errorMessages = [res.messages[0]];

                    $scope.$apply();
                    return false;
                }
                dialogs.close();
            }
            return true;
        }
    }

    $scope.disableButton = function (tab, actionName) {
        let returnValue = false;
        if (tab == 1) {
            if ($scope.model.employeeCode == $rootScope.currentUser.sapCode) {
                // Current
                if (actionName == 'reviewTicket' || actionName == 'removeTicket' || actionName == 'saveTicketBooking' || actionName == 'searchTicket' || actionName == 'checkStatusBooking') {
                    returnValue = false;
                }
                else {
                    returnValue = true;
                }
            }
            if ($scope.checkAdminDeptView) {
                //ADmin
                if (actionName == 'roomBookingFlight' || actionName == 'reviewTicket2') {
                    returnValue = false;
                }
            }
        } else {
            if ($scope.model.employeeCode == $rootScope.currentUser.sapCode) {
                if (actionName == 'reviewTicket2') {
                    returnValue = false;
                }
                else {
                    returnValue = true;
                }
            }
            if ($scope.checkAdminDeptView) {
                if (actionName == 'roomBookingFlight' || actionName == 'reviewTicket2') {
                    returnValue = false;
                }
            }
        }
        return returnValue;

    }



    $scope.roomAdminChecker = async function () {
        //var buttons = $('.k-dialog button.k-button');
        //$(buttons).attr('disabled', 'disabled');
        if (!$scope.checkAdminDeptView) {
            $('button:contains("Request Budget")').attr('disabled', 'disabled');
            $('button:contains("Delete")').attr('disabled', 'disabled');
            $('button:contains("Confirm All")').attr('disabled', 'disabled');
        }

        var tabstrip = $("#tabstrip").kendoTabStrip().data("kendoTabStrip");
        tabstrip.select(1);

        popupToSelectEmployeeGroup({ isValidDate: true, itemId: $stateParams.id, vote: 1 });
    }

    $scope.checkAdminDeptView = false;
    async function checkAdminDept() {
        if ($rootScope.currentUser && $scope.model.status == 'Waiting for Booking Flight' && $scope.model.stayHotel) {
            let model = {
                Id: $stateParams.id,
                UserId: $rootScope.currentUser.id
            };
            if ($rootScope.currentUser.isAdmin) {
                $scope.checkAdminDeptView = true;
            } else {
                $scope.checkAdminDeptView = false;
            }
            ////var resBooking = await btaService.getInstance().bussinessTripApps.checkAdminDept(model).$promise;
            //if (resBooking.isSuccess) {
            //    if (resBooking.object && resBooking.object == true) {
            //        $scope.checkAdminDeptView = true;
            //    } else {
            //        $scope.checkAdminDeptView = false;
            //    }
            //} else {
            //    $scope.checkAdminDeptView = false;
            //}

        }

    }
    $scope.saveBTADetail = async function (model) {
        $scope.saveBTADetailCheck = false;
        let errors = [];
        if (errors.length <= 0 && ($scope.model.type == 2 || $scope.model.type == 3)) {
            // Check BookingContact 
            let dataBookingContactGrid = getDataGrid('#bookingContactGrid');
            var dataGrid = getDataGrid('#btaListDetailGrid');
            if (dataBookingContactGrid && dataBookingContactGrid.length > 0) {
                let err = [];
                err = validationForTable(dataBookingContactGrid[0], $scope.ctrlMembership.requiredBookingContactFields, '');
                if (!err || err.length <= 0)
                    $scope.model.bookingContact = JSON.stringify(dataBookingContactGrid);
                else if (err.length == 4) {
                    let checkIsContact = $.grep(dataGrid, function (item) {
                        return item.isBookingContact;
                    });
                    if (checkIsContact == null || checkIsContact == undefined || checkIsContact.length <= 0) {
                        errors.push({
                            controlName: $translate.instant('BTA_BOOKING_CONTACT'),
                            message: ': ' + $translate.instant('BTA_IS_BOOKING_CONTACT_REQUIRE')
                        });
                    }
                } else {
                    errors = errors.concat(err);
                }
            } else {
                let checkIsContact = $.grep(dataGrid, function (item) {
                    return item.isBookingContact;
                });
                if (checkIsContact == null || checkIsContact == undefined || checkIsContact.length <= 0) {
                    errors.push({
                        controlName: $translate.instant('BTA_BOOKING_CONTACT'),
                        message: ': ' + $translate.instant('BTA_IS_BOOKING_CONTACT_REQUIRE')
                    });
                }
            }
        }
        if (errors.length > 0) {
            Notification.error('Booking Contact Invalid!');
            let dialog = $("#dialog_LoadingBooking").data("kendoDialog");
            dialog.close();
            $scope.saveBTADetailCheck = false;
        } else {
            $scope.saveBTADetailCheck = true;
            var res = await btaService.getInstance().bussinessTripApps.saveBTADetail(model).$promise;
            if (res.isSuccess) {
                //$state.go($state.current.name, { id: res.object.id, referenceValue: res.object.referenceNumber }, { reload: true });
                Notification.success($translate.instant('COMMON_SAVE_SUCCESS'));
            } else {
                Notification.error("Error System");
            }
        }
    }
    //lamnl
    $scope.confirmAllBooking = async function () {
        $scope.tripGroupPopup.errorMessages = [];
        let passengerInfos = $scope.prepareTripGroupBeforeSave();

        let passengerHasTicket = $.grep(passengerInfos, function (e) { return e.hasTicket; });
        if (passengerHasTicket.length > 0) {
            let flag = true;
            let passengerHasOverBudget = $(passengerHasTicket).filter(function (cIndex, cItem) {
                return cItem.isOverBudget;
            });
            if (passengerHasOverBudget.length > 0) {
                Notification.error($translate.instant('BTA_OVER_BUDGET_DO_NOT_SAVE_TICKET'));
                flag = false;
            }
            if (flag) {
                let dialogs = $("#dialog_LoadingBooking").data("kendoDialog");
                dialogs.open();
                let groupTrips = _.groupBy(passengerHasTicket, 'tripGroup');
                for (const x of Object.keys(groupTrips)) {
                    await saveTicketBookingFlightByTripGroup(x);
                }
                dialogs.close();
                $state.go($state.current.name, { id: $scope.model.id, referenceValue: $scope.model.referenceNumber }, { reload: true });
                Notification.success($translate.instant('COMMON_WORKFLOW_PROCESSED'));
                // return await $scope.saveConfirmAllBooking(passengerHasTicket);
            }
            return true;
        } else {
            return await $scope.saveConfirmAllCheckBooking();
        }
    }


    async function saveTicketBookingFlightByTripGroup (tripGroup) {
        let passengerInfos = $scope.prepareTripGroupNoTicket();

        let passengetTripGroup = passengerInfos.filter(x => x.tripGroup == tripGroup);
        if (passengetTripGroup.length > 0) {
            let flag = true;
            passengetTripGroup.forEach(data => {
                data.businessTripApplicationId = $scope.model.id;

                if (data.isOverBudget) {
                    Notification.error($translate.instant('BTA_OVER_BUDGET_DO_NOT_SAVE_TICKET'));
                    flag = false;
                }
            });
            if (flag) {
                var res = await btaService.getInstance().bussinessTripApps.savePassengerInfo(passengetTripGroup).$promise;
                if (res.isSuccess) {
                    var ress = await bookingFlightService.bookingTicket(passengetTripGroup, $stateParams.id);
                    if (ress && ress.isSuccess) {
                        Notification.success($translate.instant('COMMON_SAVE_SUCCESS'));
                        var resBooking = await btaService.getInstance().bussinessTripApps.checkBookingCompleted({ Id: $scope.model.id }).$promise;
                        if (resBooking.isSuccess && resBooking.object == true) {
                            var resHotel = await btaService.getInstance().bussinessTripApps.checkRoomHotel({ Id: $scope.model.id }).$promise;
                            if (resHotel.isSuccess) {
                                let vote = await workflowService.getInstance().workflows.vote($scope.wfVoteModel).$promise;
                                if (vote.isSuccess) {
                                    Notification.success($translate.instant('COMMON_WORKFLOW_PROCESSED'));
                                }
                            }
                        }
                    }
                } else {
                    $scope.tripGroupPopup.errorMessages = [res.messages[0]];
                    $scope.$apply();
                }
            }
        }
    }

    //end

    $scope.saveConfirmAllBooking = async function (passengerHasTicket) {
        passengerHasTicket.forEach(data => {
            data.businessTripApplicationId = $scope.model.id;
        });
        let passengerHasCommitBooking = $.grep(passengerHasTicket, function (e) { return e.isCommitBooking; });
        passengerHasTicket = $.grep(passengerHasTicket, function (e) { return !e.isCommitBooking; });
        let dialogs = $("#dialog_LoadingBooking").data("kendoDialog");
        dialogs.open();
        let checkStatusForNoCommitBooking = true;
        if (passengerHasTicket.length > 0) {
            for (var i = 0; i < passengerHasTicket.length; i++) {
                var res = await btaService.getInstance().bussinessTripApps.savePassengerInfo([passengerHasTicket[i]]).$promise;;
                if (res.isSuccess) {
                    var ress = await bookingFlightService.bookingTicket([passengerHasTicket[i]], $stateParams.id);
                    if (ress && ress.isSuccess) {
                        //let dialog = $("#dialog_TripGroup").data("kendoDialog");
                        //dialog.close();
                        //Notification.success($translate.instant('COMMON_SAVE_SUCCESS'));
                        $scope.$apply();
                        let griddata = $("#btaTripGroupGrid").data("kendoGrid").dataSource._data;
                        let passengerInfos = $scope.prepareTripGroupBeforeSave();
                        griddata = $(griddata).map(function (index, item) {
                            if (item.tripGroup == passengerHasTicket[i].tripGroup) {
                                item.checkBookingCompleted = true;
                                item.isCommitBooking = ress.isCommitBooking;
                                item.bookingNumber = ress.bookingNumber;
                                item.bookingCode = ress.bookingCode;
                            }
                        });
                        if ($scope.btaListTripGroupGridData.length > 0) {
                            let index = _.findIndex($scope.btaListTripGroupGridData, x => {
                                return x.tripGroup == passengerHasTicket[i].tripGroup;
                            });
                            if (index) {
                                $scope.btaListTripGroupGridData[index].checkBookingCompleted = true;
                                $scope.btaListTripGroupGridData[index].isCommitBooking = ress.isCommitBooking;
                                $scope.btaListTripGroupGridData[index].bookingNumber = ress.bookingNumber;
                                $scope.btaListTripGroupGridData[index].bookingCode = ress.bookingCode;
                            }
                        }

                        RefreshTripGroupGridData(griddata);
                        $scope.$apply();
                        //continueWorkflowNotSave();

                    } else {
                        dialogs.close();
                        $scope.$apply();
                        let griddata = $("#btaTripGroupGrid").data("kendoGrid").dataSource._data;
                        let passengerInfos = $scope.prepareTripGroupBeforeSave();
                        if (ress.isCommitBooking && ress.isCommitBooking != undefined) {
                            var res = await btaService.getInstance().bussinessTripApps.getItemById({ id: $stateParams.id }, null).$promise;
                            let modelDetail = null;
                            if (res.isSuccess) {
                                modelDetail = res.object.businessTripDetails;
                            }
                            griddata = $(griddata).map(function (index, item) {
                                if (item.tripGroup == passengerHasTicket[i].tripGroup) {
                                    item.isCommitBooking = ress.isCommitBooking;
                                    item.bookingNumber = ress.bookingNumber;
                                    item.bookingCode = ress.bookingCode;
                                } else {
                                    let commit = _.find(modelDetail, x => {
                                        return x.id == item.id && item.tripGroup == x.tripGroup && (x.isCommitBooking || x.checkBookingCompleted);
                                    });
                                    if (commit) {
                                        item.isCommitBooking = commit.isCommitBooking;
                                        item.bookingNumber = commit.bookingNumber;
                                        item.bookingCode = commit.bookingCode;
                                        item.checkBookingCompleted = commit.checkBookingCompleted;
                                    }
                                }
                            });
                            if ($scope.btaListTripGroupGridData.length > 0) {
                                let index = _.findIndex($scope.btaListTripGroupGridData, x => {
                                    return x.tripGroup == passengerHasTicket[i].tripGroup;
                                });
                                $scope.btaListTripGroupGridData[index].isCommitBooking = ress.isCommitBooking;
                                $scope.btaListTripGroupGridData[index].bookingNumber = ress.bookingNumber;
                                $scope.btaListTripGroupGridData[index].bookingCode = ress.bookingCode;
                            }
                            RefreshTripGroupGridData(griddata);
                            $scope.tripGroupPopup.errorMessages = [ress.message];
                            checkStatusForNoCommitBooking = false;
                            $scope.$apply();
                        } else {
                            var res = await btaService.getInstance().bussinessTripApps.getItemById({ id: $stateParams.id }, null).$promise;
                            if (res.isSuccess) {
                                let model = res.object;
                                let modelDetail = model.businessTripDetails;
                                if (modelDetail) {
                                    griddata = $(griddata).map(function (index, item) {
                                        let commit = _.find(modelDetail, x => {
                                            return x.id == item.id && item.tripGroup == x.tripGroup && (x.isCommitBooking || x.checkBookingCompleted);
                                        })
                                        if (commit) {
                                            item.isCommitBooking = commit.isCommitBooking;
                                            item.bookingNumber = commit.bookingNumber;
                                            item.bookingCode = commit.bookingCode;
                                            item.checkBookingCompleted = commit.checkBookingCompleted;
                                            if ($scope.btaListTripGroupGridData.length > 0) {
                                                let index = _.findIndex($scope.btaListTripGroupGridData, x => {
                                                    return x.tripGroup == commit.tripGroup;
                                                });
                                                $scope.btaListTripGroupGridData[index].isCommitBooking = commit.isCommitBooking;
                                                $scope.btaListTripGroupGridData[index].bookingNumber = commit.bookingNumber;
                                                $scope.btaListTripGroupGridData[index].bookingCode = commit.bookingCode;
                                            }
                                        }
                                    });
                                }
                            }
                            /*let passengerInfos = $scope.prepareTripGroupBeforeSave();*/

                            RefreshTripGroupGridData(griddata);
                            // $scope.tripGroupPopup.errorMessages = [ress.message
                            $scope.tripGroupPopup.errorMessages = ress;
                            // $scope.tripGroupPopup.errorMessages.message = JSON.parse($scope.tripGroupPopup.errorMessages);
                            checkStatusForNoCommitBooking = false;
                            $scope.$apply();
                        }

                        return false;
                    }
                } else {
                    dialogs.close();
                    $scope.tripGroupPopup.errorMessages = [res.messages[0]];
                    $scope.$apply();
                    checkStatusForNoCommitBooking = false;
                    return false;
                }
            }
        }
        if (checkStatusForNoCommitBooking) {
            if (passengerHasCommitBooking.length > 0) {
                for (var j = 0; j < passengerHasCommitBooking.length; j++) {
                    var res = await btaService.getInstance().bussinessTripApps.getBookingDetail({ bookingNumber: passengerHasCommitBooking[j].bookingNumber }).$promise;
                    if (res.isSuccess && res.object) {
                        if (res.object.bookingInfo.status === 'BOOKED' && res.object.bookingInfo.paymentStatus === 'SUCCEEDED' && res.object.bookingInfo.issuedStatus === 'SUCCEEDED') {
                            let returnValue = [];
                            if (passengerHasCommitBooking[j]) {
                                $.each(passengerHasCommitBooking[j], function (key, val) {
                                    if (val && val != null) {
                                        $.each(val.flightDetails, function (keyFlight, valFlight) {
                                            var objBooking = {
                                                flightDetailId: valFlight.id,
                                                bTADetailId: passengerHasCommitBooking[j].id,
                                                bookingCode: res.object.bookingInfo.bookingCode,
                                                bookingNumber: res.object.bookingInfo.bookingNumber,
                                                status: 'Completed',
                                                penaltyFree: 0,
                                                groupId: valFlight.groupId,
                                                directFlight: valFlight.directFlight,
                                                isCancel: false
                                            }
                                            returnValue.push(objBooking);
                                        });
                                    }
                                });
                            }
                            var res = await btaService.getInstance().bussinessTripApps.saveBookingInfo(returnValue).$promise;
                            if (res && res.isSuccess) {
                                let griddata = $("#btaTripGroupGrid").data("kendoGrid").dataSource._data;
                                griddata = $(griddata).map(function (index, item) {
                                    if (item.tripGroup == passengerHasCommitBooking[j].tripGroup) {
                                        item.checkBookingCompleted = true;
                                    }
                                });

                                RefreshTripGroupGridData(griddata);
                            }
                        } else {
                            dialogs.close();
                            checkStatusForNoCommitBooking = false;
                            $state.reload();
                            /*$scope.tripGroupPopup.errorMessages = ['Status Booking not Completed!'];
                            let griddata = $("#btaTripGroupGrid").data("kendoGrid").dataSource._data;
                            if (griddata) {
                                var res = await btaService.getInstance().bussinessTripApps.getItemById({ id: $stateParams.id }, null).$promise;
                                let modelDetail = null;
                                if (res.isSuccess) {
                                    modelDetail = res.object.businessTripDetails;
                                }
                                griddata = $(griddata).map(function (index, item) {
                                    let commit = _.find(modelDetail, x => {
                                            return x.id == item.id && item.tripGroup == x.tripGroup && (x.isCommitBooking || x.checkBookingCompleted);
                                    });
                                    if (commit) {
                                        item.checkBookingCompleted = commit.checkBookingCompleted;
                                        item.isCommitBooking = commit.isCommitBooking;
                                        item.bookingNumber = commit.bookingNumber;
                                        item.bookingCode = commit.bookingCode;
                                    }
                                });
                                RefreshTripGroupGridData(griddata);
                            }
                            $scope.$apply();*/
                        }
                    }
                }
            }
        }
        if (checkStatusForNoCommitBooking) {
            var resBooking = await btaService.getInstance().bussinessTripApps.checkBookingCompleted({ id: $scope.model.id }).$promise;
            if (resBooking.isSuccess && resBooking.object == true) {
                var resHotel = await btaService.getInstance().bussinessTripApps.checkRoomHotel({ id: $scope.model.id }).$promise;
                if (resHotel.isSuccess) {
                    let vote = await workflowService.getInstance().workflows.vote($scope.wfVoteModel).$promise;
                    if (vote.isSuccess) {
                        dialogs.close();
                        $state.go($state.current.name, { id: $scope.model.id, referenceValue: $scope.model.referenceNumber }, { reload: true });
                        Notification.success($translate.instant('COMMON_WORKFLOW_PROCESSED'));
                    }
                } else {
                    dialogs.close();
                    $state.go($state.current.name, { id: $scope.model.id, referenceValue: $scope.model.referenceNumber }, { reload: true });
                }

            } else {
                dialogs.close();
                $state.go($state.current.name, { id: $scope.model.id, referenceValue: $scope.model.referenceNumber }, { reload: true });
            }
        }
    }


    $scope.saveConfirmAllCheckBooking = async function () {
        var resBooking = await btaService.getInstance().bussinessTripApps.checkBookingCompleted({ id: $scope.model.id }).$promise;
        if (resBooking.isSuccess && resBooking.object == true) {
            var resHotel = await btaService.getInstance().bussinessTripApps.checkRoomHotel({ id: $scope.model.id }).$promise;
            if (resHotel.isSuccess) {

                let vote = await workflowService.getInstance().workflows.vote($scope.wfVoteModel).$promise;
                if (vote.isSuccess) {
                    $state.go($state.current.name, { id: $scope.model.id, referenceValue: $scope.model.referenceNumber }, { reload: true });
                    Notification.success($translate.instant('COMMON_WORKFLOW_PROCESSED'));
                }
            } else {
                $state.go($state.current.name, { id: $scope.model.id, referenceValue: $scope.model.referenceNumber }, { reload: true });
            }
            return true;
        } else {
            $scope.tripGroupPopup.errorMessages = [$translate.instant('COMMON_VALIDATE_NO_GROUP_HASTICKET')];
            $scope.$apply();
            return false;
        }
    }


    $scope.$on('Wait Booking', function (event, data) {
        $('button:contains("Request Budget")').removeAttr("disabled");
        $('button:contains("Delete")').removeAttr("disabled");
        $('button:contains("Confirm All")').removeAttr("disabled");
    });
    $scope.$on('Booked Tickets', function (event, data) {
        $('button:contains("Request Budget")').attr('disabled', 'disabled');
        $('button:contains("Delete")').attr('disabled', 'disabled');
        $('button:contains("Confirm All")').attr('disabled', 'disabled');
    });

    //loading Booking
    $scope.dialogLoadingBookingOption = {
        width: "600px",
        buttonLayout: "normal",
        height: "450px",
        closable: false,
        modal: true,
        visible: false,
        content: "",
        close: async function (e) {
        }
    };

    $scope.prepareTripGroupHasTicketRoomHotel = function () {
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
        let griddataTicket = $("#btaTripGroupHasTicketGrid").data("kendoGrid").dataSource._data;
        griddataTicket = $(griddataTicket).map(function (cIndex, cItem) {
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
    //  TripGroup has no ticket, request Over budget
    $scope.prepareTripGroupNoTicket = function () {
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
    //

    //lamnl BTA Guid
    $scope.dialog_BTAGuidOption = {
        width: "800px",
        buttonLayout: "normal",
        height: "600px",
        closable: true,
        modal: true,
        visible: false,
        title: $translate.instant('COMMON_BTA_GUID'),
        content: "",
        actions: [{
            text: $translate.instant('COMMON_BUTTON_OK'),
            action: function (e) {
                let dialogs = $("#dialog_BTAGuid").data("kendoDialog");
                dialogs.close();
            },
            primary: true
        }],
        close: async function (e) {
        }
    };

    $scope.viewBTAGuid = async function () {
        let dialogs = $("#dialog_BTAGuid").data("kendoDialog");
        dialogs.open();
    }

    $scope.tripGroupInfo = 0;
    $scope.confirmBeforeBookings = async function (tripGroup) {
        $scope.tripGroupInfo = tripGroup;
        let dialogs = $("#dialog_BTAConfirmInfo").data("kendoDialog");
        dialogs.open();
    }

    $scope.dialog_BTAConfirmInfoOption = {
        width: "800px",
        buttonLayout: "normal",
        height: "auto",
        closable: true,
        modal: true,
        visible: false,
        title: $translate.instant('COMMON_BTA_CONFIRM_INFO'),
        content: "",
        actions: [
            {
                text: $translate.instant('COMMON_BUTTON_CONFIRM'),
                action: function (e) {
                    //luu thong tin BTA detail
                    let dataGridBTADetail = getDataGrid('#btaListDetailGrid');
                    dataGridBTADetail.forEach(item => {
                        item.fromDate = new Date(item.fromDate);
                        item.toDate = new Date(item.toDate);
                        let passengerInfos = $scope.prepareTripGroupBeforeSave();
                        let dataBooking = _.find(passengerInfos, x => {
                            return x.id == item.id;
                        });
                        if (dataBooking) {
                            item.bookingCode = dataBooking.bookingCode;
                            item.isCommitBooking = dataBooking.isCommitBooking;
                            item.bookingNumber = dataBooking.bookingNumber;
                            item.checkBookingCompleted = dataBooking.checkBookingCompleted;
                        }
                    });

                    btaService.getInstance().bussinessTripApps.validatationBTADetails({ btaDetails: dataGridBTADetail }).$promise.then(function (result) {
                        if (result.isSuccess) {
                            $scope.model.businessTripDetails = JSON.stringify(dataGridBTADetail);
		                    //check Room hotel next step
		                    //$scope.model.checkRoomNextStep = isCheckRoomHotel();
		                    $scope.saveBTADetail($scope.model);
		                    //Notification.success($translate.instant('COMMON_SAVE_SUCCESS'));
		                    let dialogs = $("#dialog_BTAConfirmInfo").data("kendoDialog");
		                    dialogs.close();
		                    if ($scope.saveBTADetailCheck) {
		                        if ($scope.tripGroupInfo != 0) {
		                            $scope.saveTicketBookingFlight($scope.tripGroupInfo);
		                        } else {
		                            $scope.confirmAllBooking();
		                        }
		                    }
		                    return false;
                        } else {
                            Notification.error($translate.instant(result.messages[0]));
                        }
                    });

                    return false;
                },
                primary: true
            },
            {
                text: $translate.instant('COMMON_BUTTON_CANCEL'),
                action: function (e) {
                    let dialogs = $("#dialog_BTAConfirmInfo").data("kendoDialog");
                    dialogs.close();

                    let dialog2 = $("#dialog_TripGroup").data("kendoDialog");
                    dialog2.close();
                    return false;
                },
                primary: true
            }],
        close: async function (e) {
        }
    }

    $scope.reasonManagers = [];

    async function getBTAReasons() {
        let currentQuery = {
            predicate: "",
            predicateParameters: [],
            order: "Modified desc",
            limit: 10000,
            page: 1
        };

        var result = await settingService.getInstance().reason.getBTAReasons(currentQuery).$promise;
        if (result.isSuccess) {
            $scope.reasonOfListOptions.dataSource = result.object.data;
            $scope.reasons = result.object.data;
            $scope.reasons.forEach(item => {
                $scope.reasonManagers.push(item);
            });
            setDataReasonOfOT($scope.reasonManagers);
        }
    }

    $scope.changeReason = function (item) {
        if (item && !$scope.isITEdit) {
            $scope.model.requestorNote = item.name;
            $scope.model.requestorNoteDetail = "";
        }
        
    }

    function setDataReasonOfOT(dataPosition) {
        var dataSource = new kendo.data.HierarchicalDataSource({
            data: dataPosition
        });
        var dropdownlist = $("#reasonDropdown").data("kendoDropDownList");
        dropdownlist.setDataSource(dataSource);
    }

    //loading Search Ticket
    $scope.dialogLoadingSearchOption = {
        width: "500px",
        buttonLayout: "normal",
        height: "400px",
        closable: false,
        modal: true,
        visible: false,
        content: "",
        close: async function (e) {
        }
    };

    $scope.openWindow = function (url) {
        $rootScope.openWindow(url);
    }

    //=================================================== MEMBERSHIP & BOOKING CONTACT ==========================================================================//
    $scope.ctrlMembership = {
        save: async function (dataItem) {
            var dataGridBTADetail = getDataGrid('#btaListDetailGrid');
            let index = _.findIndex(dataGridBTADetail, y => {
                return y.no == $scope.addDetailMembershipId;
            });
            if (dataGridBTADetail[index].memberships !== null && dataGridBTADetail[index].memberships !== undefined && dataGridBTADetail[index].memberships !== '')
                dataGridBTADetail[index].memberships = JSON.parse(dataGridBTADetail[index].memberships);
            else
                dataGridBTADetail[index].memberships = [];
            //validate
            let error = this.validateBeforeSave(dataItem, dataGridBTADetail[index].memberships);
            if (!error) {
                //create or update
                let checkUpdate = false;
                if (dataGridBTADetail[index].memberships.length > 0) {
                    let indexc = _.findIndex(dataGridBTADetail[index].memberships, y => {
                        return y.id == dataItem.id;
                    });
                    if (indexc != null && dataItem !== undefined && indexc != -1) {
                        dataGridBTADetail[index].memberships[indexc] = {
                            airlineCode: dataItem.airlineCode,
                            airlineName: dataItem.airlineName,
                            airlineId: dataItem.airlineId,
                            idCard: dataItem.idCard,
                            id: dataItem.id
                        }
                        checkUpdate = true;
                    }
                }
                if (!checkUpdate) {
                    let model = {
                        airlineCode: dataItem.airlineCode,
                        airlineName: dataItem.airlineName,
                        airlineId: dataItem.airlineId,
                        idCard: dataItem.idCard,
                        id: dataItem.uid
                    }
                    dataGridBTADetail[index].memberships.push(model);
                }
                dataGridBTADetail[index].memberships = JSON.stringify(dataGridBTADetail[index].memberships);

                setDataSourceForGrid('#btaListDetailGrid', dataGridBTADetail);
                dataItem.select = false;
                if (dataItem.id == null || dataItem.id == '' || dataItem.id == undefined)
                    dataItem.id = dataItem.uid;
                refreshGrid("membershipGrid");
            }
        },
        cancel: function (model) {
            if (model.id && model.id !== undefined) {
                record = 0;
                model['select'] = false;
                var dataGridBTADetail = getDataGrid('#btaListDetailGrid');
                let index = _.findIndex(dataGridBTADetail, y => {
                    return y.no == $scope.addDetailMembershipId;
                });
                if (dataGridBTADetail[index].memberships !== null && dataGridBTADetail[index].memberships !== undefined && dataGridBTADetail[index].memberships !== '')
                    dataGridBTADetail[index].memberships = JSON.parse(dataGridBTADetail[index].memberships);

                var item = dataGridBTADetail[index].memberships.find(x => x.id === model.id);
                item.select = false;
                model.airlineCode = item.airlineCode;
                model.airlineName = item.airlineName;
                model.airlineId = item.airlineId;
                model.idCard = item.idCard;

                refreshGrid("membershipGrid");
            } else {
                let grid = $('#membershipGrid').data("kendoGrid");
                grid.dataSource.read();
            }
        },
        edit: function (model) {
            var dataGridMember = getDataGrid('#membershipGrid');
            var result = this.validateEdit(dataGridMember);
            if (result)
                Notification.error($translate.instant('COMMON_ERROR_MESSAGE_WHEN_EDIT'));
            else {
                model.select = true;
                refreshGrid("membershipGrid");
            }
        },
        deleteRecord: function (model) {
            $scope.dataDeleteMember = [];
            var dataGridMember = getDataGrid('#membershipGrid');
            var result = this.validateEdit(dataGridMember);
            if (result) {
                Notification.error($translate.instant('COMMON_ERROR_MESSAGE_WHEN_EDIT'));
            }
            else {
                $scope.dataDeleteMember = model;
                $scope.dialog = $rootScope.showConfirmDelete($translate.instant('COMMON_BUTTON_DELETE'), $translate.instant(commonData.confirmContents.remove), $translate.instant('COMMON_BUTTON_CONFIRM'));
                $scope.dialog.bind("close", this.confirmDelete);
            }
        },
        confirmDelete: function (e) {
            if (e.data && e.data.value) {
                var dataGridBTADetail = getDataGrid('#btaListDetailGrid');
                let index = _.findIndex(dataGridBTADetail, y => {
                    return y.no == $scope.addDetailMembershipId;
                });
                if (dataGridBTADetail[index].memberships !== null && dataGridBTADetail[index].memberships !== undefined && dataGridBTADetail[index].memberships !== '')
                    dataGridBTADetail[index].memberships = JSON.parse(dataGridBTADetail[index].memberships);

                _.remove(dataGridBTADetail[index].memberships, function (item) {
                    return item.id === $scope.dataDeleteMember.id;
                });

                dataGridBTADetail[index].memberships = JSON.stringify(dataGridBTADetail[index].memberships);
                setDataSourceForGrid('#btaListDetailGrid', dataGridBTADetail);

                var dataGridMember = getDataGrid('#membershipGrid');
                _.remove(dataGridMember, function (item) {
                    return item.id === $scope.dataDeleteMember.id;
                });

                dataGridBTADetail[index].memberships = JSON.stringify(dataGridBTADetail[index].memberships);
                setDataSourceForGrid('#membershipGrid', dataGridMember);

                refreshGrid("membershipGrid");
                //let grid = $("#grid").data("kendoGrid");
                //grid.dataSource.read();
                //grid.dataSource.page(1);
            }
        },
        validateEdit: function (data) {
            var result = false;
            data ? data.forEach(item => {
                if (item.select === true) {//&& item.id !== '' && item.id != null && item.id !== undefined
                    result = true;
                    return result;
                }
            }) : result = false;
            return result
        },
        validateBeforeSave: function (dataItem, memberships) {
            let errors = $rootScope.validateInRecruitment(this.requiredMembershipFields, dataItem);
            if (errors.length > 0) {
                let errorList = errors.map(x => {
                    return x.controlName + " " + x.errorDetail;
                });
                if (errorList.length > 0) {
                    Notification.error(`Some fields are required: </br>
                    <ul>${errorList.join('<br/>')}</ul>`);
                }
                return true;
            } else {
                if (memberships.length > 0) {
                    let indexc = _.findIndex(memberships, y => {
                        return y.airlineCode === dataItem.airlineCode && y.id !== dataItem.id;
                    });
                    if (indexc != null && dataItem !== undefined && indexc != -1) {
                        Notification.error(`Duplicate: Airline`);
                        return true;
                    }
                }
            }
        },
        validateBookingContract: async function () {
            if ($scope.model.type != null && ($scope.model.type === 2 || $scope.model.type === 3)) {
                let dataGrid = getDataGrid('#bookingContactGrid');
                if (dataGrid && dataGrid.length > 0)
                    $scope.model.bookingContact = JSON.stringify(dataGrid);
                var dataGridBTADetail = getDataGrid('#btaListDetailGrid');
                if (dataGridBTADetail && dataGridBTADetail.length > 0) {
                    let checkIsContact = $.grep(dataGridBTADetail, function (item) {
                        return item.isContact;
                    });
                    if (checkIsContact == null || checkIsContact == undefined || checkIsContact.length == 0) {
                        let dataGrid = getDataGrid('#bookingContactGrid');
                        if (dataGrid && dataGrid.length > 0) {
                            if (!dataGrid[0].fullName || !dataGrid[0].email || !dataGrid[0].mobile) {
                                Notification.error(`BOOKING CONTACT: Require field`);
                                return true;
                            } else
                                return false;
                        }
                    }
                }
            }
            return false;
        },
        changeAirline: function (dataItem) {
            let name = _.find($scope.dataAirlines, function (e) {
                return e.code == dataItem.airlineCode;
            });
            dataItem.airlineName = name.name;
            dataItem.airlineId = name.id;
        },
        changeIsBookingContact: function (dataItem) {
            var dataGridBTADetail = getDataGrid('#btaListDetailGrid');
            if (dataGridBTADetail) {
                let checkBookingContact = $.grep(dataGridBTADetail, function (item) {
                    return item.isBookingContact;
                });
                if (checkBookingContact && checkBookingContact.length > 1) {
                    dataItem.isBookingContact = false
                    Notification.error($translate.instant('BTA_IS_BOOKING_CONTACT_ERROR'));
                }
            }
        },
        addMembership: function () {
            let grid = $("#membershipGrid").data("kendoGrid");
            let countData = grid.dataSource.data();
            let isEditing = _.find(countData, function (item) {
                return !item.airlineCode;
            });
            if (!isEditing) {
                let newRow = {
                    no: grid.dataSource._data.length + 1,
                    airlineCode: null,
                    idCard: null,
                    select: true
                };
                let source = grid.dataSource;
                var length = source._data.length;
                var currentPage = source.page();
                var currentPageSize = source.pageSize();
                var insertIndex = currentPage * currentPageSize <= length ? (currentPage * currentPageSize - 1) : (length + 1);
                source.insert(insertIndex, newRow);
                grid.refresh();
            }
        },
        showMembershipDialog: function (dataItem) {
            // var item = $scope.dataBudgetLimitYearManagement.find(x => x.id === data.id);
            if (dataItem.memberships != null && dataItem.memberships != undefined && dataItem.memberships != "")
                $scope.dataMembership = JSON.parse(dataItem.memberships);
            else
                $scope.dataMembership = null;
            $scope.addDetailMembershipId = dataItem.no;
            //$scope.dataMembership.sort(function (a, b) {
            //    return a.airlineCode - b.airlineCode;
            //});
            let dialog = $("#membershipid").data("kendoDialog");
            dialog.title('MEMBERSHIP FOR ' + dataItem.fullName);
            dialog.open();
            this.setDataGrid($scope.dataMembership, '#membershipGrid', 5);
        },
        changeTypeBooking: function (type) {
            if ($scope.model.type != null && ($scope.model.type === '2' || $scope.model.type === '3')) {
                if ($scope.model.bookingContact === null || $scope.model.bookingContact === undefined || $scope.model.bookingContact.length <= 0) {
                    $scope.convertBookingContact = [{
                        firstName: '',
                        lastName: '',
                        email: '',
                        mobile: ''
                    }];
                    setDataSourceForGrid('#bookingContactGrid', $scope.convertBookingContact);
                    refreshGrid("bookingContactGrid");
                } else {
                    setDataSourceForGrid('#bookingContactGrid', $scope.convertBookingContact);
                    refreshGrid("bookingContactGrid");
                }
                $scope.btaListDetailGridOptions.columns[1].hidden = false;
                var grid = $("#btaListDetailGrid").data("kendoGrid");
                var column = grid.columns.find(column => column.field === "isBookingContact");
                column.hidden = false;
                grid.setOptions($scope.btaListDetailGridOptions);
                grid.refresh();
            } else {
                $scope.btaListDetailGridOptions.columns[1].hidden = true;
                var grid = $("#btaListDetailGrid").data("kendoGrid");
                var column = grid.columns.find(column => column.field === "isBookingContact");
                column.hidden = true;
                grid.setOptions($scope.btaListDetailGridOptions);
                grid.refresh();
            }
            if ($scope.model.type != null && ($scope.model.type === '2' || $scope.model.type === '4')) {
                $scope.isVisaChecked = true;
            } else {
                $scope.isVisaChecked = false;
            }
        },
        setDataGrid: function (data, gridId, pageSize) {
            let dataSource = new kendo.data.DataSource({
                data: data ? data.map((item, index) => {
                    return {
                        ...item,
                        no: index + 1
                    }
                }) : null,
                pageSize: pageSize,
                schema: {
                    model: {
                        id: "id"
                    }
                }
            });
            let grid = $(gridId).data("kendoGrid");
            grid.setDataSource(dataSource);
        },
        requiredMembershipFields: [
            {
                fieldName: "airlineCode",
                title: $translate.instant('BTA_AIRLINE')
            },
            {
                fieldName: "idCard",
                title: $translate.instant('BTA_ID_CARD_AIRLINE')
            }
        ],
        requiredBookingContactFields: [
            {
                property: "firstName",
                title: $translate.instant('BTA_BOOKING_CONTACT_FIRSTNAME_REQUIRE')
            },
            {
                property: "lastName",
                title: $translate.instant('BTA_BOOKING_CONTACT_SURNAME_REQUIRE')
            },
            {
                property: "email",
                title: $translate.instant('BTA_BOOKING_CONTACT_EMAIL_REQUIRE')
            },
            {
                property: "mobile",
                title: $translate.instant('BTA_BOOKING_CONTACT_MOBILE_REQUIRE')
            }
        ],
        hasEditingMemberShip: function () {
            var result = false;
            var dataGridMember = getDataGrid('#membershipGrid');
            if (dataGridMember && dataGridMember.length > 0) {
                let edit = this.validateEdit(dataGridMember);
                if (!edit)
                    result = true;
            } else
                result = true;
            return result;
        },
        membershipOptions: {
            dataSource: {
                serverPaging: true,
                data: $scope.dataMembership,
                schema: {
                    model: {
                        id: "id"
                    }
                }
            },
            editable: false,
            sortable: false,
            pageable: {
                pageSize: appSetting.pageSizeWindow,
                alwaysVisible: true,
                pageSizes: [5, 10, 20, 30, 40, 50],
                responsive: false,
            },
            columns: [
                {
                    field: "no",
                    title: $translate.instant('COMMON_NO'),
                    width: "60px",
                    locked: true,
                    editor: function (container, options) {
                        $(`<label>${options.model[options.field]}</label>`).appendTo(
                            container
                        );
                    }
                },
                {
                    field: "airLine",
                    title: $translate.instant('BTA_AIRLINE'),
                    width: "300px",
                    locked: true,
                    template: function (dataItem) {
                        if (dataItem.select) {
                            return `<select kendo-drop-down-list style="width: 280px"
                            data-k-ng-model="dataItem.airlineCode"
                            k-data-text-field="'name'"
                            k-data-value-field="'code'"
                            k-auto-bind="'false'"
                            k-value-primitive="'false'"
                            k-data-source="dataAirlines"
                            k-on-change="ctrlMembership.changeAirline(dataItem)"
                            > </select>`
                        } else {
                            return `<span>{{dataItem.airlineName}}</span>`
                        }
                    }
                },
                {
                    field: "idCard",
                    title: $translate.instant('BTA_ID_CARD_AIRLINE'),
                    width: "190px",
                    locked: true,
                    template: function (dataItem) {
                        if (dataItem.select) {
                            return `<input class="k-textbox w100" autoComplete="off" name="idCard" ng-model="dataItem.idCard"/>`;
                        } else {
                            return `<span>{{dataItem.idCard}}</span>`;
                        }
                    }
                },
                {
                    width: "300px",
                    title: "Actions",
                    template: function (dataItem) {
                        if (!$scope.model.id || $scope.model.status === 'Draft' || $scope.model.status === 'Waiting for Booking Flight') {
                            if (dataItem.select || dataItem.id === null) {
                                return ` <button type="button" ng-click="ctrlMembership.save(dataItem)"
                                            class="btn btn-outline-dark rounded-3 btn-sm poppins-regular display-9 m-1">
                                       <svg fill="#000000" width="18" height="18" viewBox="0 0 256 256" xmlns="http://www.w3.org/2000/svg">
                                        <g fill-rule="evenodd">
                                            <path d="M65.456 48.385c10.02 0 96.169-.355 96.169-.355 2.209-.009 5.593.749 7.563 1.693 0 0-1.283-1.379.517.485 1.613 1.67 35.572 36.71 36.236 37.416.665.707.241.332.241.332.924 2.007 1.539 5.48 1.539 7.691v95.612c0 7.083-8.478 16.618-16.575 16.618-8.098 0-118.535-.331-126.622-.331-8.087 0-16-6.27-16.356-16.1-.356-9.832.356-118.263.356-126.8 0-8.536 6.912-16.261 16.932-16.261zm-1.838 17.853l.15 121c.003 2.198 1.8 4.003 4.012 4.015l120.562.638a3.971 3.971 0 0 0 4-3.981l-.143-90.364c-.001-1.098-.649-2.616-1.445-3.388L161.52 65.841c-.801-.776-1.443-.503-1.443.601v35.142c0 3.339-4.635 9.14-8.833 9.14H90.846c-4.6 0-9.56-4.714-9.56-9.14s-.014-35.14-.014-35.14c0-1.104-.892-2.01-1.992-2.023l-13.674-.155a1.968 1.968 0 0 0-1.988 1.972zm32.542.44v27.805c0 1.1.896 2.001 2 2.001h44.701c1.113 0 2-.896 2-2.001V66.679a2.004 2.004 0 0 0-2-2.002h-44.7c-1.114 0-2 .896-2 2.002z"/>
                                            <path d="M127.802 119.893c16.176.255 31.833 14.428 31.833 31.728s-14.615 31.782-31.016 31.524c-16.401-.259-32.728-14.764-32.728-31.544s15.735-31.963 31.91-31.708zm-16.158 31.31c0 9.676 7.685 16.882 16.218 16.843 8.534-.039 15.769-7.128 15.812-16.69.043-9.563-7.708-16.351-15.985-16.351-8.276 0-16.045 6.52-16.045 16.197z"/>
                                        </g>
                                    </svg> &nbsp;{{'COMMON_BUTTON_SAVE' | translate}}
                                    </button>

                                    <a ng-click="ctrlMembership.cancel(dataItem)"  class='btn btn-outline-dark rounded-3 btn-sm poppins-regular display-9 m-1' >  <svg width="14" height="14" viewBox="0 0 512 512" version="1.1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink">
                                        <title>cancel</title>
                                        <g id="Page-1" stroke="none" stroke-width="1" fill="none" fill-rule="evenodd">
                                            <g id="work-case" fill="#000000" transform="translate(91.520000, 91.520000)">
                                                <polygon id="Close" points="328.96 30.2933333 298.666667 1.42108547e-14 164.48 134.4 30.2933333 1.42108547e-14 1.42108547e-14 30.2933333 134.4 164.48 1.42108547e-14 298.666667 30.2933333 328.96 164.48 194.56 298.666667 328.96 328.96 298.666667 194.56 164.48">

                                    </polygon>
                                            </g>
                                        </g>
                                    </svg> &nbsp; {{'COMMON_BUTTON_CANCEL' | translate}}</a>`;
                            }
                            if (!dataItem.select && dataItem.id !== null) {
                                return `
                                       <a ng-click="ctrlMembership.edit(dataItem)" class='btn btn-outline-dark rounded-3 btn-sm poppins-regular display-9 m-1'>   <svg
                                            xmlns="http://www.w3.org/2000/svg"
                                            width="13"
                                            height="13"
                                            fill="currentColor"
                                            class="bi bi-pencil"
                                            viewBox="0 0 16 16"
                                          >
                                            <path
                                              d="M12.146.146a.5.5 0 0 1 .708 0l3 3a.5.5 0 0 1 0 .708l-10 10a.5.5 0 0 1-.168.11l-5 2a.5.5 0 0 1-.65-.65l2-5a.5.5 0 0 1 .11-.168zM11.207 2.5 13.5 4.793 14.793 3.5 12.5 1.207zm1.586 3L10.5 3.207 4 9.707V10h.5a.5.5 0 0 1 .5.5v.5h.5a.5.5 0 0 1 .5.5v.5h.293zm-9.761 5.175-.106.106-1.528 3.821 3.821-1.528.106-.106A.5.5 0 0 1 5 12.5V12h-.5a.5.5 0 0 1-.5-.5V11h-.5a.5.5 0 0 1-.468-.325"
                                            ></path>
                                          </svg>
                                          &nbsp;Edit</a>
                                         <a ng-click="ctrlMembership.deleteRecord(dataItem)" class='btn btn-outline-dark rounded-3 btn-sm poppins-regular display-9 m-1'>   <svg xmlns="http://www.w3.org/2000/svg" width="13" height="13" fill="currentColor" viewBox="0 0 16 16">
                                                    <path d="M5.5 5.5A.5.5 0 0 1 6 6v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5m2.5 0a.5.5 0 0 1 .5.5v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5m3 .5a.5.5 0 0 0-1 0v6a.5.5 0 0 0 1 0z" />
                                                    <path d="M14.5 3a1 1 0 0 1-1 1H13v9a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V4h-.5a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1H6a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1h3.5a1 1 0 0 1 1 1zM4.118 4 4 4.059V13a1 1 0 0 0 1 1h6a1 1 0 0 0 1-1V4.059L11.882 4zM2.5 3h11V2h-11z" />
                                                </svg>
                                          &nbsp;Delete</a>`;
                               
                            }
                        } else {
                            return '';
                        }
                    },
                }
            ]
        },
        bookingContactOptions: {
            dataSource: {
                data: $scope.convertBookingContact
            },
            sortable: false,
            pageable: false,
            columns: [
                {
                    field: "firstName",
                    headerTemplate: $translate.instant('BTA_GIVEN_NAME'),
                    width: "200px",
                    template: function (dataItem) {
                        return `<input name="firstName" autocomplete="do-not-autofill" ng-readonly="false" class="k-input" type="text" ng-model="dataItem.firstName" style="color: #333;width: 100%; padding: 5px;"/>`;
                    }
                },
                {
                    field: "lastName",
                    headerTemplate: $translate.instant('BTA_SURNAME'),
                    width: "200px",
                    headerAttributes: { "style": "border-left-width: 5px;" },
                    template: function (dataItem) {
                        return `<input name="lastName" autocomplete="do-not-autofill" ng-readonly="false" class="k-input" type="text" ng-model="dataItem.lastName" style="color: #333;width: 100%; padding: 5px;"/>`;
                    }
                },
                {
                    field: "email",
                    headerTemplate: $translate.instant('BTA_EMAIL'),
                    width: "200px",
                    headerAttributes: { "style": "border-left-width: 10px;" },
                    template: function (dataItem) {
                        return `<input name="email" autocomplete="off" ng-readonly="false" class="k-input" type="text" ng-model="dataItem.email" style="color: #333;width: 100%; padding: 5px;"/>`;
                    }
                },
                {
                    field: "mobile",
                    headerTemplate: $translate.instant('COMMON_MOBILE'),
                    width: "200px",
                    headerAttributes: { "style": "border-left-width: 15px;" },
                    template: function (dataItem) {
                        return `<input name="mobile" autocomplete="off" class="k-input" type="text" ng-model="dataItem.mobile" style="color: #333;width: 100%; padding: 5px;"
                            ng-keyup="phoneNumberKeyPress(event.keyCode, dataItem)"/>`;
                    }
                }
            ]
        },
    }

    //CAR_RENTAL_ATTACHMENT_DETAIL
    $scope.carRentalCtrl = {
        carRentalAttachmentDetails: [],
        oldCarRentalAttachmentDetails: [],
        removeFileCarRentals: [],
        onSelect: function (e) {
            let message = $.map(e.files, function (file) {
                $scope.carRentalCtrl.carRentalAttachmentDetails.push(file);
                return file.name;
            }).join(", ");
        },
        removeAttach: function (e) {
            let file = e.files[0];
            $scope.carRentalCtrl.carRentalAttachmentDetails = $scope.carRentalCtrl.carRentalAttachmentDetails.filter(item => item.name !== file.name);
        },
        removeOldAttachment: function (model) {
            $scope.carRentalCtrl.oldCarRentalAttachmentDetails = $scope.carRentalCtrl.oldCarRentalAttachmentDetails.filter(item => item.id !== model.id);
            $scope.carRentalCtrl.removeFileCarRentals.push(model.id);
        },
        downloadAttachment: async function (id) {
            let result = await attachmentFile.downloadAndSaveFile({
                id
            });
        },
        uploadAction: async function () {
            var payload = new FormData();
            $scope.carRentalCtrl.carRentalAttachmentDetails.forEach(item => {
                payload.append('file', item.rawFile, item.name);
            });
            let result = await attachmentService.getInstance().attachmentFile.upload(payload).$promise;
            return result;
        },
        mergeAttachment: async function () {
            try {
                // Upload file lên server rồi lấy thông tin lưu thành chuỗi json
                let uploadResult = await this.uploadAction();
                let attachmentFilesJson = '';
                let allattachmentDetails = $scope.carRentalCtrl.oldCarRentalAttachmentDetails && $scope.carRentalCtrl.oldCarRentalAttachmentDetails.length ? $scope.carRentalCtrl.oldCarRentalAttachmentDetails.map(({
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

    };

    //VISA_ATTACHMENT_DETAIL
    $scope.visaCtrl = {
        visaAttachmentDetails: [],
        oldVisaAttachmentDetails: [],
        removeFileVisas: [],
        onSelect: function (e) {
            let message = $.map(e.files, function (file) {
                $scope.visaCtrl.visaAttachmentDetails.push(file);
                return file.name;
            }).join(", ");
        },
        removeAttach: function (e) {
            let file = e.files[0];
            $scope.visaCtrl.visaAttachmentDetails = $scope.visaCtrl.visaAttachmentDetails.filter(item => item.name !== file.name);
        },
        removeOldAttachment: function (model) {
            $scope.visaCtrl.oldVisaAttachmentDetails = $scope.visaCtrl.oldVisaAttachmentDetails.filter(item => item.id !== model.id);
            $scope.visaCtrl.removeFileVisas.push(model.id);
        },
        downloadAttachment: async function (id) {
            let result = await attachmentFile.downloadAndSaveFile({
                id
            });
        },
        uploadAction: async function () {
            var payload = new FormData();
            $scope.visaCtrl.visaAttachmentDetails.forEach(item => {
                payload.append('file', item.rawFile, item.name);
            });
            let result = await attachmentService.getInstance().attachmentFile.upload(payload).$promise;
            return result;
        },
        mergeAttachment: async function () {
            try {
                // Upload file lên server rồi lấy thông tin lưu thành chuỗi json
                let uploadResult = await this.uploadAction();
                let attachmentFilesJson = '';
                let allattachmentDetails = $scope.visaCtrl.oldVisaAttachmentDetails && $scope.visaCtrl.oldVisaAttachmentDetails.length ? $scope.visaCtrl.oldVisaAttachmentDetails.map(({
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

    };

    $scope.isArray = function (value) {
        return Array.isArray(value);
    };

    async function getWorkflowProcessingStage(itemId) {
        var result = await workflowService.getInstance().workflows.getWorkflowStatus({ itemId: itemId }).$promise;
        if (result.isSuccess && result.object && result.object.currentStatus != "Draft") {
            $scope.currentInstanceProcessingStage = result.object.workflowInstances[0];
            $scope.processingStageRound = result.object.workflowInstances.length;
            $scope.currentInstanceProcessingStage.workflowData.steps.map((item) => {
                var findHistories = _.find($scope.currentInstanceProcessingStage.histories, x => { return x.stepNumber == item.stepNumber });
                if (findHistories != null) {
                    item.histories = findHistories;
                    return item;
                }
            });
        }
    }

    $scope.$on("ITEdit", async function (evt, model) {
        if (model) {
            $scope.isITEdit = model.isEdit;
        }
    })

    //===================================== CHECK STATUS BOOKING ===============================================//
    $scope.checkStatusBookingCtrl = {
        showDialog: async function (data, group, bookingCode) {
            $scope.datacheckStatusBooking = null;
            var res = await btaService.getInstance().bussinessTripApps.getBookingDetail({ bookingNumber: data }).$promise;
            if (res.isSuccess && res.object) {
                $scope.datacheckStatusBooking = {
                    information: this.getStatusInformation(res.object.bookingInfo.status, res.object.bookingInfo.paymentStatus, res.object.bookingInfo.issuedStatus),
                    bookingStatus: res.object.bookingInfo.status,
                    paymentStatus: res.object.bookingInfo.paymentStatus,
                    issuedStatus: res.object.bookingInfo.issuedStatus,
                    meaning: this.getStatusMeaning(res.object.bookingInfo.status, res.object.bookingInfo.paymentStatus, res.object.bookingInfo.issuedStatus),
                    group: group
                };

                let dialog = $("#checkStatusBookingDialogId").data("kendoDialog");
                dialog.title('STATUS BOOKING ' + data + ' GROUP ' + group);
                dialog.open();
                if (res.object.bookingInfo.status === 'BOOKED' && res.object.bookingInfo.paymentStatus === 'SUCCEEDED' && res.object.bookingInfo.issuedStatus === 'SUCCEEDED') {
                    var timeoutId = setTimeout(this.completedBooking(group, bookingCode, data), 3000);
                    clearTimeout(timeoutId); // Hủy bỏ timeout

                }
            }
        },
        closeDialog: function () {
            let dialog = $("#checkStatusBookingDialogId").data("kendoDialog");
            dialog.close();
        },
        getStatusMeaning: function (bookingStatus, paymentStatus, issuedStatus) {
            let status = null;
            switch (bookingStatus) {
                case 'EXPRIED':
                    status = 'Vé đã hết hạn';
                    break;
                case 'BOOKED':
                    switch (paymentStatus) {
                        case 'SUCCEEDED':
                            switch (issuedStatus) {
                                case 'PENDING':
                                    status = 'Book thành công, thanh toán thành công, chờ xuất vé';
                                    break;
                                case 'SUCCEEDED':
                                    status = 'Book thành công, thanh toán thành công, xuất vé thành công';
                                    break;
                                case 'FAILED':
                                    status = 'Book thành công, thanh toán thành công, xuất vé thất bại trong quá trình làm việc trực tiếp';
                                    break;
                                case 'TICKET_ON_PROCESS':
                                    status = 'Book thành công, thanh toán thành công nhưng do lỗi bất kỳ (đường truyền, hạ tầng,..) dẫn tới không xuất vé';
                                    break;
                                case 'CANCELLED':
                                    status = 'Book thành công, thanh toán thành công, yêu cầu hủy đang được xử lý';
                                    break;
                            }
                            break;
                        case 'PENDING':
                            if (issuedStatus === 'PENDING')
                                status = 'Book thành công, chờ thanh toán, vé chưa phát hành';
                            break;
                        case 'FAILED':
                            if (issuedStatus === 'PENDING')
                                status = 'Book thành công, thanh toán thất bại, chưa xuất vé';
                            break;
                        case 'EXPRIED':
                            if (issuedStatus === 'PENDING')
                                status = 'Book thành công, chờ xuất vé, hết thời gian thanh toán';
                            if (issuedStatus === 'FAILED')
                                status = 'Book thành công, xuất vé thất bại, hết thời gian thanh toán';
                            break;
                        case 'REFUNDED':
                            if (issuedStatus === 'CANCELLED')
                                status = 'Book thành công, thanh toán thành công, đã hoàn tiền';
                            break;
                    }
                    break;
                case 'PENDING':
                    if (paymentStatus === 'PENDING' && issuedStatus === 'PENDING')
                        status = 'Chưa book thành công';
                    break;
                case 'CANCELLED':
                    if (paymentStatus === 'SUCCEEDED' && issuedStatus === 'TICKET_ON_PROCESS')
                        status = 'Book thành công, thanh toán thành công, yêu cầu xuất đang được xử lý, sau đó hủy booking';
                    if (paymentStatus === 'SUCCEEDED' && issuedStatus === 'SUCCEEDED')
                        status = 'Book thành công, thanh toán thành công, yêu cầu xuất đã thành công, sau đó hủy booking';
                    if (paymentStatus === 'REFUNDED' && issuedStatus === 'TICKET_ON_PROCESS')
                        status = 'Booking đã hủy khi vé lỗi, refund tiền cho khách';
                    if (paymentStatus === 'REFUNDED' && issuedStatus === 'SUCCEEDED')
                        status = 'Booking đã hủy khi xuất vé thành công, refund tiền cho khách';
                    break;
                default:
                    status = 'Không còn slot để book';
                    break;
            }
            return status;
        },
        getStatusInformation: function (bookingStatus, paymentStatus, issuedStatus) {
            let status = null;
            switch (bookingStatus) {
                case 'EXPRIED':
                    status = 'Vé đã hết hạn';
                    break;
                case 'BOOKED':
                    if (paymentStatus === 'SUCCEEDED') {
                        switch (issuedStatus) {
                            case 'PENDING':
                                status = 'Thanh toán thành công, chờ xuất vé';
                                break;
                            case 'SUCCEEDED':
                                status = 'Giao dịch thành công';
                                break;
                            case 'FAILED':
                                status = 'Thanh toán thành công, xuất vé lỗi';
                                break;
                            case 'TICKET_ON_PROCESS':
                                status = 'Giao dịch đang xử lý';
                                break;
                            case 'CANCELLED':
                                status = 'Đã hủy';
                                break;
                        }
                    } else if (paymentStatus === 'PENDING' && issuedStatus === 'PENDING') {
                        status = 'Giữ chỗ, chờ thanh toán';
                    } else if (paymentStatus === 'FAILED' && issuedStatus === 'PENDING') {
                        status = 'Thanh toán thất bại';
                    } else if (paymentStatus === 'EXPRIED') {
                        if (issuedStatus === 'PENDING' || issuedStatus === 'FAILED') {
                            status = 'Giao dịch hết hiệu lực';
                        }
                    } else if (paymentStatus === 'REFUNDED' && issuedStatus === 'CANCELLED') {
                        status = 'Đã hoàn tiền';
                    }
                    break;
                case 'PENDING':
                    if (paymentStatus === 'PENDING' && issuedStatus === 'PENDING') {
                        status = 'Chưa thực hiện đặt vé';
                    }
                    break;
                case 'CANCELLED':
                    if (paymentStatus === 'SUCCEEDED' && issuedStatus === 'TICKET_ON_PROCESS')
                        status = 'Đã hủy vé lỗi';
                    if (paymentStatus === 'SUCCEEDED' && issuedStatus === 'SUCCEEDED')
                        status = 'Đã hủy vé thành công';
                    if (paymentStatus === 'REFUNDED' && issuedStatus === 'TICKET_ON_PROCESS')
                        status = 'Đã hoàn tiền vé lỗi';
                    if (paymentStatus === 'REFUNDED' && issuedStatus === 'SUCCEEDED')
                        status = 'Đã hoàn tiền vé thành công';
                    break;
                default:
                    status = 'Giao dịch thất bại';
                    break;
            }
            return status;
        },
        newBooking: function () {
            let griddata = $("#btaTripGroupGrid").data("kendoGrid").dataSource._data;
            griddata = $(griddata).map(function (index, item) {
                if ($scope.datacheckStatusBooking.group == item.tripGroup) {
                    item.isCommitBooking = false;
                    item.bookingNumber = null;
                    item.bookingCode = null;
                    item.hasTicket = false;
                    item.isOverBudget = false;
                    item.flightDetails = {};
                    item.isDeleted = false;
                    item.ticketInfo = null;
                }
            });

            //$scope.removeTicketOfTripGroup($scope.datacheckStatusBooking.group);
            //clear data isCommit
            if ($scope.btaListTripGroupGridData.length > 0) {
                let index = _.findIndex($scope.btaListTripGroupGridData, x => {
                    return x.tripGroup == $scope.datacheckStatusBooking.group;
                });
                $scope.btaListTripGroupGridData[index].isCommitBooking = false;
            }
            RefreshTripGroupGridData(griddata);
            let dialog = $("#checkStatusBookingDialogId").data("kendoDialog");
            dialog.close();
        },
        completedBooking: async function (group, bookingCode, bookingNumber) {
            let griddataTicket = $("#btaTripGroupGrid").data("kendoGrid").dataSource._data;
            if (griddataTicket) {
                let groupInfoPassengers = $.grep(griddataTicket, function (e) { return e.tripGroup == group });
                let returnValue = [];
                $.each(groupInfoPassengers, function (key, val) {
                    $.each(val.flightDetails, function (keyFlight, valFlight) {
                        var objBooking = {
                            flightDetailId: valFlight.id,
                            bTADetailId: val.id,
                            bookingCode: bookingCode,
                            bookingNumber: bookingNumber,
                            status: 'Completed',
                            penaltyFree: 0,
                            groupId: valFlight.groupId,
                            directFlight: valFlight.directFlight,
                            isCancel: false
                        }
                        returnValue.push(objBooking);
                    });
                });

                var res = await btaService.getInstance().bussinessTripApps.saveBookingInfo(returnValue).$promise;
                if (res && res.isSuccess) {
                    let dialogs = $("#dialog_LoadingBooking").data("kendoDialog");
                    dialogs.open();
                    var resBooking = await btaService.getInstance().bussinessTripApps.checkBookingCompleted({ Id: $scope.model.id }).$promise;
                    if (resBooking.isSuccess && resBooking.object == true) {
                        let dialog = $("#dialog_TripGroup").data("kendoDialog");
                        dialog.close();
                        var resHotel = await btaService.getInstance().bussinessTripApps.checkRoomHotel({ Id: $scope.model.id }).$promise;
                        if (resHotel.isSuccess) {
                            let vote = await workflowService.getInstance().workflows.vote($scope.wfVoteModel).$promise;
                            if (vote.isSuccess) {
                                dialogs.close();
                                $state.go($state.current.name, { id: $scope.model.id, referenceValue: $scope.model.referenceNumber }, { reload: true });
                                Notification.success($translate.instant('COMMON_WORKFLOW_PROCESSED'));
                            }
                        } else {
                            dialogs.close();
                            $state.go($state.current.name, { id: $scope.model.id, referenceValue: $scope.model.referenceNumber }, { reload: true });
                        }

                    } else {
                        dialogs.close();
                        $state.go($state.current.name, { id: $scope.model.id, referenceValue: $scope.model.referenceNumber }, { reload: true });
                    }
                    let dialogssss = $("#checkStatusBookingDialogId").data("kendoDialog");
                    dialogssss.close();
                }
            }
        }
    }
});