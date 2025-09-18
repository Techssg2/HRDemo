var ssgApp = angular.module('ssg.businessTripReportModule', ["kendo.directives"]);
ssgApp.controller('businessTripReportController', function ($rootScope, $scope, $location, appSetting, localStorageService, $stateParams, $state, moment, Notification, settingService, $timeout, cbService, masterDataService, workflowService, ssgexService, fileService, commonData, $translate, attachmentService, attachmentFile, btaService, $compile) {
    $scope.title = $translate.instant('BTA_REPORT');
    // phần khai báo chung
    $scope.actions = {
        ifEnterDialogReport: ifEnterDialogReport,
        ifEnter: ifEnter
    };
    let currentStatus = null;
    let currentParentStatus = null;
    $scope.Init = function () {
        $scope.searchInfo = {
            hotel: "",
            location: "",
            flightNumber: "",
            status: "",
            airline: "",
            fromDate: null,
            toDate: null,
            departmentId: '',
            userSapCode: ''
        };
    }

    this.$onInit = function () {
        $scope.total = 0;
        $scope.data = [];
        $scope.reportType = appSetting.btaReportType.HOTEL;

        $scope.currentQuery = {
            predicate: "",
            predicateParameters: [],
            order: "Created desc",
            limit: appSetting.pageSizeDefault,
            page: 1
        };
        $scope.searchInfo = {
            hotel: '',
            flightNumber: '',
            status: '',
            location: '',
            airline: '',
            fromDate: null,
            toDate: null,
            departmentId: '',
            userSapCode: ''
        };

        $scope.columnGridReport = {
            reportHotels: [{
                field: "no",
                headerTemplate: $translate.instant('COMMON_NO'),
                width: "80px",
                editable: function (e) {
                    return false;
                }
            },
            {
                field: "name ",
                headerTemplate: $translate.instant('BTA_HOTEL'),
                width: "180px",
            },
            {
                field: "totalRooms",
                headerTemplate: $translate.instant('BTA_TOTAL_ROOMS'),
                width: "150px",
            },
            {
                field: "totalDays",
                headerTemplate: $translate.instant('BTA_TOTAL_DAYS'),
                width: "150px",
            }
            ],
            reportFlightNumbers: [
                {
                    field: "no",
                    headerTemplate: $translate.instant('COMMON_NO'),
                    width: "50px",
                    editable: function (e) {
                        return false;
                    }
                },
                {
                    field: "flightNumber",
                    headerTemplate: $translate.instant('BTA_FLIGHT_NUMBER'),
                    width: "80px"
                },
                {
                    field: "totalStaffs",
                    headerTemplate: $translate.instant('BTA_TOTAL_STAFF'),
                    width: "100px",
                    template: function (dataItem) {
                        return `<a ng-click="reportOfHotel(dataItem, 'flightNumber')">${dataItem.totalStaffs}</a>`;
                    }
                }
            ],
            reportStatus: [
                {
                    field: "no",
                    headerTemplate: $translate.instant('COMMON_NO'),
                    width: "50px",
                    editable: function (e) {
                        return false;
                    }
                },
                {
                    field: "status",
                    headerTemplate: $translate.instant('COMMON_STATUS'),
                    width: "80px"
                },
                {
                    field: "totalRequest",
                    headerTemplate: $translate.instant('COMMON_TOTAL_REQUEST'),
                    width: "100px",
                    template: function (dataItem) {
                        return `<a ng-click="reportOfHotel(dataItem, 'status')">${dataItem.totalRequest}</a>`;
                    }
                }
            ],
            reportFlightsBooking: [
                {
                    field: "no",
                    headerTemplate: $translate.instant('COMMON_NO'),
                    width: 50,
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
                        return `<a ui-sref="home.business-trip-application.item({referenceValue: '${dataItem.referenceNumber}', id: '${dataItem.btaId}'})" ui-sref-opts="{ reload: true }">${dataItem.referenceNumber}</a>`;
                    }
                },
                {
                    field: "bookingDate",
                    headerTemplate: $translate.instant('BTA_BOOKING_DATE'),
                    width: 120,
                    template: function (dataItem) {
                        return moment(dataItem.bookingDate).format(appSetting.sortDateFormat);
                    }
                },
                {
                    field: "flightInformation",
                    headerTemplate: $translate.instant('BTA_FLIGHT_INFORMATION'),
                    width: 250
                },
                {
                    field: "sapCode",
                    headerTemplate: $translate.instant('COMMON_SAP_CODE'),
                    width: 120
                },
                {
                    field: "fullName",
                    headerTemplate: $translate.instant('COMMON_FULL_NAME'),
                    width: 200
                },
                {
                    field: "deptName",
                    headerTemplate: $translate.instant('COMMON_DEPARTMENT'),
                    sortable: false,
                    width: "240px"
                },
                {
                    field: "status",
                    headerTemplate: $translate.instant('BTA_FLIGHT_TICKET_STATUS'),
                    width: 180,
                    template: function (dataItem) {
                        var statusTranslate = $rootScope.getStatusTranslate(dataItem.status);
                        return `<workflow-status status="${statusTranslate}"></workflow-status>`;
                    }
                },
                {
                    field: "btaStatus",
                    headerTemplate: $translate.instant('BTA_STATUS'),
                    width: 300,
                    template: function (dataItem) {
                        var statusTranslate = $rootScope.getStatusTranslate(dataItem.btaStatus);
                        return `<workflow-status status="${statusTranslate}"></workflow-status>`;
                    }
                },
                {
                    field: "cost",
                    headerTemplate: $translate.instant('BTA_AIRFARE'),
                    width: 200,
                    template: function (dataItem) {
                        return `<span>{{dataItem.cost | number}}</span>`;
                    }
                },
                {
                    field: "serviceFee",
                    headerTemplate: $translate.instant('BTA_SERVICE_FEE'),
                    width: 200,
                    template: function (dataItem) {
                        return `<span>{{dataItem.serviceFee | number}}</span>`;
                    }
                },
                {
                    field: "reasonforBusinessTrip",
                    headerTemplate: $translate.instant('BTA_REASON_BT'),
                    width: 250
                },
                {
                    field: "reasonforCancel",
                    headerTemplate: $translate.instant('BTA_REASON_CANCEL'),
                    width: 250
                },
                {
                    field: "cancellationFee",
                    headerTemplate: $translate.instant('BTA_CANCELLATION_FEE'),
                    width: 200,
                    template: function (dataItem) {
                        if (dataItem.cancellationFee != "") {
                            return `<span>{{dataItem.cancellationFee | number}}</span>`;
                        }
                        else {
                            return "";
                        }
                    }
                }
            ]
        };

        $scope.reportTypeOptions = {
            dataTextField: "name",
            dataValueField: "value",
            valuePrimitive: true,
            checkboxes: false,
            autoBind: true,
            filter: "contains",
            dataSource: {
                data: [{
                    name: $translate.instant('BTA_HOTEL'),
                    value: 1
                },
                {
                    name: $translate.instant('BTA_FLIGHT_NUMBER'),
                    value: 2
                },
                {
                    name: $translate.instant('COMMON_STATUS'),
                    value: 3
                },
                {
                    name: $translate.instant('BTA_FLIGHT_BOOKING'),
                    value: 4
                }
                ]
            },
            change: function (e) {
                $scope.reportType = e.sender.value();
                var grid = $("#grid").data("kendoGrid");
                var gridDialogReport = $("#dialogReport").data("kendoGrid");
                $scope.Init();
                if ($scope.reportType == appSetting.btaReportType.HOTEL) {
                    grid.setOptions({
                        columns: $scope.columnGridReport.reportHotels
                    });

                    gridDialogReport.getOptions().columns.forEach(function (e) {
                        if (e.field != "referenceNumber" && e.field != "hotelName" && e.field != "flightNumberCode" && e.field != "flightNumberName") {
                            gridDialogReport.showColumn(e.field);
                        } else {
                            gridDialogReport.hideColumn(e.field);
                        }
                    });
                }
                else if ($scope.reportType == appSetting.btaReportType.FLIGHTNUMBER) {
                    grid.setOptions({
                        dataSource: {
                            serverPaging: true,
                            pageSize: 20,
                            transport: {
                                read: async function (e) {
                                    await getListReports(e, $scope.reportType);
                                }
                            },
                            schema: {
                                total: () => { return $scope.total },
                                data: () => { return $scope.data }
                            }
                        },
                        columns: $scope.columnGridReport.reportFlightNumbers
                    });

                    gridDialogReport.getOptions().columns.forEach(function (e) {
                        if (e.field == "sapCode" || e.field == "fullName" || e.field == "departmentName" || e.field == "fromDate" || e.field == "toDate") {
                            gridDialogReport.showColumn(e.field);
                        }
                        else {
                            gridDialogReport.hideColumn(e.field);
                        }
                    });
                }
                else if ($scope.reportType == appSetting.btaReportType.STATUS) { //$scope.reportType == 3
                    $timeout(function () {
                        grid.tbody.find("td.k-hierarchy-cell").remove();
                        grid.thead.find("th.k-header")[0].style.cssText = "display: none;";
                    }, 0);
                    grid.setOptions({
                        dataSource: {
                            serverPaging: true,
                            pageSize: 20,
                            transport: {
                                read: async function (e) {
                                    await getListReports(e, $scope.reportType);
                                }
                            },
                            schema: {
                                total: () => { return $scope.total },
                                data: () => { return $scope.data }
                            }
                        },
                        columns: $scope.columnGridReport.reportStatus
                    });

                    gridDialogReport.getOptions().columns.forEach(function (e) {
                        if (e.field != "totalDays") {
                            gridDialogReport.showColumn(e.field);
                        } else {
                            gridDialogReport.hideColumn(e.field);
                        }
                    });
                }
                // Flight booking on change
                else if ($scope.reportType == appSetting.btaReportType.FLIGHTSBOOKING) {
                    $scope.data = [];
                    $scope.getDepartments();
                    grid.setOptions({
                        
                        dataSource: {
                            serverPaging: true,
                            pageSize: 20,
                            transport: {
                                read: async function (e) {
                                    await getListReports(e, $scope.reportType);
                                }
                            },
                            schema: {
                                total: () => { return $scope.total },
                                data: () => { return $scope.data }
                            }
                        },
                        sortable: false,
                        columns: $scope.columnGridReport.reportFlightsBooking,

                        
                    });
                }
            }
        }
        $scope.hotelOptions = {
            dataTextField: "name",
            dataValueField: "code",
            valuePrimitive: true,
            checkboxes: false,
            autoBind: true,
            filter: "contains",
            filtering: $rootScope.dropdownFilter,
            dataSource: {
                serverPaging: false,
                pageSize: 100,
                transport: {
                    read: async function (e) {
                        await getHotels(e);
                    }
                },
                schema: {
                    data: () => {
                        return $scope.dataHotels
                    }
                }
            },
            change: function (e) {
                let locationofHotel = [];
                if (e.sender.dataItem()) {
                    var obj = {};
                    obj.id = e.sender.dataItem().businessTripLocationId,
                        obj.code = e.sender.dataItem().businessTripLocationCode,
                        obj.name = e.sender.dataItem().businessTripLocationName
                    locationofHotel.push(obj);
                    setDataSourceLocations(locationofHotel);
                    var dropdownlistLocations = $("#location_id").data("kendoDropDownList");
                    if (dropdownlistLocations) {
                        dropdownlistLocations.select(0);
                        $scope.searchInfo.location = locationofHotel[0].code;
                    }
                }
            }
        }
        $scope.flightNumberOptions = {
            dataTextField: "name",
            dataValueField: "code",
            valuePrimitive: true,
            checkboxes: false,
            autoBind: true,
            filter: "contains",
            filtering: $rootScope.dropdownFilter,
            dataSource: {
                serverPaging: false,
                pageSize: 100,
                transport: {
                    read: async function (e) {
                        await getFlightNumbers(e);
                    }
                },
                schema: {
                    data: () => {
                        return $scope.dataFlightNumbers
                    }
                }
            },
            change: function (e) {
                let airlineOfFlightNumber = [];
                if (e.sender.dataItem()) {
                    var obj = {};
                    obj.id = e.sender.dataItem().airlineId,
                        obj.code = e.sender.dataItem().airlineCode,
                        obj.name = e.sender.dataItem().airlineName
                    airlineOfFlightNumber.push(obj);
                    setDataSourceAirlines(airlineOfFlightNumber);
                    var dropdownlistAirlines = $("#airline_id").data("kendoDropDownList");
                    if (dropdownlistAirlines) {
                        dropdownlistAirlines.select(0);
                        $scope.searchInfo.airline = airlineOfFlightNumber[0].code;
                    }
                }
            }
        }
        $scope.locationOptions = {
            dataTextField: "name",
            dataValueField: "code",
            valuePrimitive: true,
            checkboxes: false,
            autoBind: true,
            filter: "contains",
            filtering: $rootScope.dropdownFilter,
            dataSource: {
                serverPaging: false,
                pageSize: 100,
                transport: {
                    read: async function (e) {
                        await getLocations(e);
                    }
                },
                schema: {
                    data: () => {
                        return $scope.dataLocations
                    }
                }
            }
        }
        $scope.airlineOptions = {
            dataTextField: "name",
            dataValueField: "code",
            valuePrimitive: true,
            checkboxes: false,
            autoBind: true,
            filter: "contains",
            filtering: $rootScope.dropdownFilter,
            dataSource: {
                serverPaging: false,
                pageSize: 100,
                transport: {
                    read: async function (e) {
                        await getAirlines(e);
                    }
                },
                schema: {
                    data: () => {
                        return $scope.dataAirlines
                    }
                }
            }
        }
        //Flights Booking
        $scope.statusFlightsBookingOptions = {
            dataTextField: "name",
            dataValueField: "code",
            valuePrimitive: true,
            checkboxes: false,
            autoBind: true,
            filter: "contains",
            dataSource: {
                data: translateStatus($translate, commonData.itemFlightsBookingStatuses)
            }
        }

        $scope.statusOptions = {
            dataTextField: "name",
            dataValueField: "code",
            valuePrimitive: true,
            checkboxes: false,
            autoBind: true,
            filter: "contains",
            dataSource: {
                data: translateStatus($translate, commonData.itemBtaStatuses)
            }
        }

        $scope.reportDataSource = {
            serverPaging: true,
            pageSize: 20,
            transport: {
                read: async function (e) {
                    await getListReports(e, $scope.reportType);
                }
            },
            schema: {
                total: () => { return $scope.total },
                data: () => { return $scope.data }
            }
        };
        $scope.reportGridOptions = {
            serverPaging: true,
            sortable: false,
            pageable: {
                alwaysVisible: true,
                pageSizes: appSetting.pageSizesArray,
                messages: {
                    display: "{0}-{1} " + $translate.instant('PAGING_OF') + " {2} " + $translate.instant('PAGING_ITEM'),
                    itemsPerPage: $translate.instant('PAGING_ITEM_PER_PAGE'),
                    empty: $translate.instant('PAGING_NO_ITEM')
                }
            },
            columns: [{
                field: "no",
                headerTemplate: $translate.instant('COMMON_NO'),
                width: "30px",
                editable: function (e) {
                    return false;
                }
            },
            {
                field: "name ",
                headerTemplate: $translate.instant('BTA_HOTEL'),
                width: "180px",
                hidden: $scope.reportType != appSetting.btaReportType.HOTEL ? true : false
            },
            {
                field: "totalRooms",
                headerTemplate: $translate.instant('BTA_TOTAL_ROOMS'),
                width: "150px",
            },
            {
                field: "totalDays",
                headerTemplate: $translate.instant('BTA_TOTAL_DAYS'),
                width: "150px",
            }
            ]
        };

        $scope.reportDetailGridOptions = function (hotel) {
            var currentRoom = _.find($scope.data, x => {
                return x.code == hotel.code;
            });
            if (currentRoom) {
                return {
                    dataSource: {
                        data: currentRoom.items,
                        pageSize: 10,

                    },
                    scrollable: true,
                    sortable: true,
                    //pageable: true,
                    pageable: {
                        alwaysVisible: true,
                        pageSizes: appSetting.pageSizesArray
                    },
                    columns: [
                        {
                            field: "roomTypeName",
                            headerTemplate: $translate.instant('BTA_ROOM_TYPE'),
                            width: "200px"
                        },
                        {
                            field: "totalStaffs",
                            headerTemplate: $translate.instant('BTA_TOTAL_STAFF'),
                            width: "150px",
                            template: function (dataItem) {
                                return `<a ng-click="detailUserInRoom(dataItem,'${hotel.code}')">${dataItem.totalStaffs}</a>`;
                            }
                        },
                        {
                            field: "totalDays",
                            headerTemplate: $translate.instant('BTA_TOTAL_DAYS'),
                            width: "150px"
                        }
                    ]
                }

            }

        };

        $scope.dialogReportOptions = {
            dataSource: {
                serverPaging: true,
                data: $scope.dataDialogReport,
                pageSize: 20,
                schema: {
                    total: () => { return $scope.totalDialogReport },
                    data: () => { return $scope.dataDialogReport }
                }
            },
            excel: {
                fileName: `Business Trip Application Export Requests_${kendo.toString(new Date, "dd_MM_yyyy HH:mm:ss")}`,
            },
            excelExport: function (e) {
                var sheet = e.workbook.sheets[0];
                for (var rowIndex = 1; rowIndex < sheet.rows.length; rowIndex++) {
                    var row = sheet.rows[rowIndex];
                    for (var cellIndex = 0; cellIndex < row.cells.length; cellIndex++) {
                        if (_.isDate(row.cells[cellIndex].value)) {
                            row.cells[cellIndex].format = "dd/MM/yyyy";
                        }
                    }
                }
            },
            sortable: false,
            autoBind: true,
            resizable: true,
            editable: {
                mode: "inline",
                confirmation: false
            },
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
                    field: "referenceNumber",
                    title: "Reference Number",
                    width: "180px",
                    hidden: true
                },
                {
                    field: "sapCode",
                    title: "Sap Code",
                    width: "150px"
                },
                {
                    field: "fullName",
                    title: "Full Name",
                    width: "200px"
                },
                {
                    field: "departmentName",
                    title: "Department",
                    width: "250px"
                },
                {
                    field: "hotelName",
                    title: "Hotel",
                    width: "150px",
                    hidden: true
                },
                {
                    field: "fromDate",
                    title: "From Date",
                    width: "180px",
                    template: function (dataItem) {
                        return moment(dataItem.fromDate).format(appSetting.longDateFormatAMPM);
                    }
                },
                {
                    field: "toDate",
                    title: "To Date",
                    width: "180px",
                    template: function (dataItem) {
                        return moment(dataItem.toDate).format(appSetting.longDateFormatAMPM);
                    }
                },
                {
                    field: "checkInDate",
                    title: "Check in Date",
                    width: "180px",
                    template: function (dataItem) {
                        if (dataItem.checkInDate)
                            return moment(dataItem.checkInDate).format(appSetting.longDateFormatAMPM);
                        return "";
                    }
                },
                {
                    field: "checkOutDate",
                    title: "Check out Date",
                    width: "180px",
                    template: function (dataItem) {
                        if (dataItem.checkOutDate)
                            return moment(dataItem.checkOutDate).format(appSetting.longDateFormatAMPM);
                        return "";
                    }
                },
                {
                    field: "created",
                    title: "Created Date",
                    width: "180px",
                    template: function (dataItem) {
                        return moment(dataItem.created).format(appSetting.longDateFormat);
                    }
                },
                {
                    field: "totalDays",
                    title: "Total Days",
                    width: "100px"
                },
                {
                    field: "flightNumberName",
                    title: "Flight Number",
                    width: "150px",
                    hidden: true
                },
            ]
        };
    }

    function setDataSourceLocations(data) {
        var dropdownlistLocations = $("#location_id").data("kendoDropDownList");
        var dataSource = new kendo.data.DataSource({
            data: data
        });

        if (dropdownlistLocations) {
            dropdownlistLocations.setDataSource(dataSource);
        }
    }

    function setDataSourceAirlines(data) {
        var dropdownlistAirlines = $("#airline_id").data("kendoDropDownList");
        var dataSource = new kendo.data.DataSource({
            data: data
        });

        if (dropdownlistAirlines) {
            dropdownlistAirlines.setDataSource(dataSource);
        }
    }
    function setDataSourceHotels(data) {
        var dropdownlistAirlines = $("#hotel_id").data("kendoDropDownList");
        var dataSource = new kendo.data.DataSource({
            data: data
        });

        if (dropdownlistAirlines) {
            dropdownlistAirlines.setDataSource(dataSource);
        }
    }
    function setDataSourceFlightNumbers(data) {
        var dropdownlistAirlines = $("#flight_number_id").data("kendoDropDownList");
        var dataSource = new kendo.data.DataSource({
            data: data
        });

        if (dropdownlistAirlines) {
            dropdownlistAirlines.setDataSource(dataSource);
        }
    }

    async function getHotels(option) {
        let queryArgs = {
            predicate: '',
            predicateParameters: [],
            order: appSetting.ORDER_GRID_DEFAULT,
            page: 1,
            limit: 10000000
        };
        var res = await settingService.getInstance().hotel.getListHotels(queryArgs).$promise;
        if (res.isSuccess) {
            $scope.dataHotels = [];
            $scope.originalHotels = _.cloneDeep(res.object.data);
            $scope.dataHotels = res.object.data.map((item, index) => {
                item.name = item.address && item.telephone ? item.name + '-' + item.address + '-' + item.telephone : item.address ? item.name + '-' + item.address : item.telephone ? item.name + '-' + item.telephone : item.name;
                return {
                    ...item
                }
            });
        }
        if (option) {
            option.success($scope.dataHotels);
        }
    }

    async function getFlightNumbers(option) {
        let queryArgs = {
            predicate: '',
            predicateParameters: [],
            order: appSetting.ORDER_GRID_DEFAULT,
            page: 1,
            limit: 10000000
        };
        var res = await settingService.getInstance().flightNumber.getFlightNumbers(queryArgs).$promise;
        if (res.isSuccess) {
            $scope.dataFlightNumbers = [];
            res.object.data.forEach(element => {
                $scope.dataFlightNumbers.push(element);
            });
        }
        if (option) {
            option.success($scope.dataFlightNumbers);
        }
    }

    async function getLocations(option) {
        let queryArgs = {
            predicate: '',
            predicateParameters: [],
            order: appSetting.ORDER_GRID_DEFAULT,
            page: 1,
            limit: 10000000
        };
        var res = await settingService.getInstance().businessTripLocation.getListBusinessTripLocation(queryArgs).$promise;
        if (res.isSuccess) {
            $scope.dataLocations = [];
            res.object.data.forEach(element => {
                $scope.dataLocations.push(element);
            });
        }
        if (option) {
            option.success($scope.dataLocations);
        }
    }

    async function getAirlines(option) {
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
        if (option) {
            option.success($scope.dataAirlines);
        }
    }

    function buildArgs(pageIndex, pageSize) {
        let predicate = [];
        let predicateParameters = [];
        if ($scope.keyword) {
            predicate.push('(Code.contains(@0) or Description.contains(@0))');
            predicateParameters.push($scope.keyword);
        }
        var option = QueryArgs(predicate, predicateParameters, appSetting.ORDER_GRID_DEFAULT, pageIndex, pageSize);
        return option;
    }
    $scope.totalDialogReport = 0;
    function buildExportArgs() {
        let option = buildArgForGetListReport('StatusItem');
        let revokingArg = {
            predicate: option.predicate,
            predicateParameters: option.predicateParameters,
            codes: $scope.searchInfo.status
        };
        if (!$scope.searchInfo.status) {
            revokingArg.codes = $scope.statusOptions.dataSource.data.map(x => x.code);
        }
        return revokingArg;
    }
    async function setDataInDetailUserInRoom(id, data, count) {
        $scope.dataDialogReport = data;
        $scope.oldDataDialogReport = data;
        // $scope.totalDialogReport = count;
        $scope.totalDialogReport = data.length;
        //setDataSourceForGrid(id, $scope.dataDialogReport, $scope.totalDialogReport);
        setDataGrid(id, $scope.dataDialogReport, $scope.totalDialogReport)
    }

    async function getListReports(option, reportType) {
        let queryArgs = {
            predicate: '',
            predicateParameters: [],
            order: appSetting.ORDER_GRID_DEFAULT,
            page: 1,
            limit: appSetting.pageSizeDefault
        };
        let args = buildArgForGetListReport();
        //let queryArgs = {};
        //let queryArgs = $scope.currentQuery;
        if (reportType == appSetting.btaReportType.HOTEL) {


            if (option) {
                queryArgs.limit = option.data.take;
                queryArgs.page = option.data.page;
            }
            queryArgs.predicate = args.predicate;
            queryArgs.predicateParameters = args.predicateParameters;
        }
        else if (reportType == appSetting.btaReportType.FLIGHTNUMBER) {

            if (option) {
                queryArgs.limit = option.data.take;
                queryArgs.page = option.data.page;
            }
            queryArgs.predicate = args.predicate;
            queryArgs.predicateParameters = args.predicateParameters;
        }
        else if (reportType == appSetting.btaReportType.STATUS){ 
            {
                if (option) {
                    queryArgs.limit = option.data.take;
                    queryArgs.page = option.data.page;
                }
                queryArgs.predicate = args.predicate;
                queryArgs.predicateParameters = args.predicateParameters;
            }
        }
        //get data report - Flights booking
        else if (reportType == appSetting.btaReportType.FLIGHTSBOOKING) {
            {
                if (option) {
                    queryArgs.limit = option.data.take;
                    queryArgs.page = option.data.page;
                }
                queryArgs.predicate = args.predicate;
                queryArgs.predicateParameters = args.predicateParameters;
            }
        }
        res = await btaService.getInstance().bussinessTripApps.getReports({ type: reportType }, queryArgs).$promise;

        if (res.object && res.isSuccess) {
            //let n = 0;
            //$scope.data = res.object.data.map(function (item) { return { ...item, no: ++n } });
            if (option && option.data.page > 1) {
                $scope.data = res.object.data.map((item, index) => {
                    //item.name = item.address && item.telePhone ? item.name + '-' + item.address + '-' + item.telePhone : item.address ? item.name + '-' + item.address : item.telePhone ? item.name + '-' + item.telePhone : item.name;
                    return {
                        ...item,
                        no: index + option.data.take + 1,
                    }
                });
            } else {
                $scope.data = res.object.data.map((item, index) => {
                    //item.name = item.address && item.telePhone ? item.name + '-' + item.address + '-' + item.telePhone : item.address ? item.name + '-' + item.address : item.telePhone ? item.name + '-' + item.telePhone : item.name;
                    return {
                        ...item,
                        no: index + 1
                    }
                });
            }
            $scope.total = res.object.count;
        }
        if (option) {
            option.success($scope.data);
            if (reportType == appSetting.btaReportType.FLIGHTNUMBER || reportType == appSetting.btaReportType.STATUS) {
                $timeout(function () {
                    var rows = $('.k-hierarchy-cell');
                    for (let i = 0; i < rows.length; i++) {
                        rows[i].style.cssText = "display: none !important;";
                    }
                    if (grid && grid.thead) {
                        grid.thead.find("th.k-header")[0].style.cssText = "display: none !important;";
                    }
                }, 0);
            }
            if (reportType == appSetting.btaReportType.FLIGHTSBOOKING) {
                var rows = $('.k-hierarchy-cell a');
                for (let i = 0; i < rows.length; i++) {
                    rows[i].style.cssText = "display: none !important;";
                }
            }
        }
    }

    $scope.titleModal = 'Title modal';
    $scope.reportOfHotel = async function (dataItem, reportType) {
        $scope.roomType = dataItem.roomType;
        $scope.keywordDialogReport = '';
        let dialogReportOfHotel = $("#dialogReportOfHotel").data("kendoDialog");
        if (reportType == 'flightNumber') {
            currentStatus = dataItem.flightNumberCode;
            dialogReportOfHotel.title(`REPORT OF FLIGHT NUMBER: ${dataItem.flightNumber}`);
            await showDetailUserInFlightNumber(dataItem);
        } else if ($scope.reportType == appSetting.btaReportType.STATUS) {
            currentStatus = dataItem.status;
            await dialogReportOfHotel.title(`REPORT OF STATUS: ${dataItem.status}`);
            await showDetailUserInStatusData(dataItem);
        }
        //dialogReportOfHotel.open();
    }
    async function showDetailUserInFlightNumber(dataItem) {
        let queryArgs = {
            predicate: '',
            predicateParameters: [],
            order: appSetting.ORDER_GRID_DEFAULT,
            page: 1,
            limit: 10000000
        };
        let option = buildArgForGetListReport();
        queryArgs.predicate = option.predicate ? option.predicate + `&& flightNumberCode = @${option.predicateParameters.length}` : `flightNumberCode = @0`;
        queryArgs.predicateParameters = option.predicateParameters ? option.predicateParameters.concat([dataItem.flightNumberCode]) : [dataItem.flightNumberCode];
        $scope.flightNumberInfo = dataItem;
        $scope.reportType = appSetting.btaReportType.FLIGHTNUMBER;
        let dialogReportOfHotel = $("#dialogReportOfHotel").data("kendoDialog");
        if (dialogReportOfHotel) {
            var res = await btaService.getInstance().bussinessTripApps.getDetailUsersInRoom({ type: $scope.reportType, code: dataItem.flightNumberCode }, queryArgs).$promise;
            if (res && res.isSuccess) {
                setDataInDetailUserInRoom('#dialogReport', res.object.data, res.object.count);
            }
            dialogReportOfHotel.title(`REPORT OF FLIGHT NUMBER: ${dataItem.flightNumber}`);
            dialogReportOfHotel.open();
            $rootScope.confirmDialogReport = dialogReportOfHotel;
        }

    }
    async function showDetailUserInStatusData(dataItem) {
        let queryArgs = {
            predicate: '',
            predicateParameters: [],
            order: appSetting.ORDER_GRID_DEFAULT,
            page: 1,
            limit: 1000000000
        };
        if ($scope.searchInfo.fromDate && isValidDate($scope.searchInfo.fromDate)) {
            let fromDate = moment($scope.searchInfo.fromDate, 'DD/MM/YYYY').format('MM/DD/YYYY');
            queryArgs.predicate = queryArgs.predicate ? queryArgs.predicate + `&& BusinessTripApplication.Created>= @${queryArgs.predicateParameters.length}` : `BusinessTripApplication.Created>= @${queryArgs.predicateParameters.length}`;
            queryArgs.predicateParameters.push(fromDate);
        }
        if ($scope.searchInfo.toDate && isValidDate($scope.searchInfo.toDate)) {
            let toDate = moment($scope.searchInfo.toDate, 'DD/MM/YYYY').add(1, 'days').format('MM/DD/YYYY');
            queryArgs.predicate = queryArgs.predicate ? queryArgs.predicate + `&& BusinessTripApplication.Created < @${queryArgs.predicateParameters.length}` : `BusinessTripApplication.Created < @${queryArgs.predicateParameters.length}`;
            queryArgs.predicateParameters.push(toDate);
        }
        $scope.reportType = appSetting.btaReportType.STATUS;
        let dialogReportOfHotel = $("#dialogReportOfHotel").data("kendoDialog");
        if (dialogReportOfHotel) {
            dialogReportOfHotel.title(`REPORT OF STATUS: ${dataItem.status}`);
            let args = genarateQueryForStatusDetail(dataItem.status, queryArgs);
            var params = { type: $scope.reportType, code: dataItem.status };
            var res = await btaService.getInstance().bussinessTripApps.getDetailUsersInRoom(params, args).$promise;
            if (res && res.isSuccess) {
                setDataInDetailUserInRoom('#dialogReport', res.object.data, res.object.count);
            }
            dialogReportOfHotel.open();
            $rootScope.confirmDialogReport = dialogReportOfHotel;
        }

    }
    function genarateQueryForStatusDetail(code, option, statusColumn = 'BusinessTripApplication.Status', i = 0) {
        if (code == "Revoking" || code == "In Progress") {
            option.predicate = option.predicate ? option.predicate + `&& ${statusColumn}.contains(@${option.predicateParameters.length})` : `${statusColumn}.contains(@0)`;
            option.predicateParameters.push("Waiting");
        } else if (code != "Changing/ Cancelling Business Trip") {
            option.predicate = option.predicate ? option.predicate + `&& ${statusColumn}=(@${option.predicateParameters.length})` : `${statusColumn}=@0`;
            option.predicateParameters.push(code);
        }
        return option;

    }
    $scope.detailUserInRoom = async function (dataItem, hotelCode) {
        let queryArgs = {
            predicate: '',
            predicateParameters: [],
            order: appSetting.ORDER_GRID_DEFAULT,
            page: 1,
            limit: 10000000
        };
        let hotel = _.find($scope.originalHotels, x => { return x.code == hotelCode });
        currentStatus = dataItem.roomTypeName;
        currentParentStatus = hotel.code;
        let option = buildArgForGetListReport();
        queryArgs.predicate = option.predicate ? option.predicate + `&& hotelCode = @${option.predicateParameters.length}` : `hotelCode = @0`;
        queryArgs.predicateParameters = option.predicateParameters ? option.predicateParameters.concat([hotel.code]) : [hotel.code];

        $scope.reportType = appSetting.btaReportType.HOTEL;
        $scope.hotel = { code: hotel.code, name: hotel.name, address: hotel.address, telephone: hotel.telephone };
        $scope.roomTypeName = dataItem.roomTypeName;
        $scope.keywordDialogReport = '';
        let dialogReportOfHotel = $("#dialogReportOfHotel").data("kendoDialog");
        if (dialogReportOfHotel) {
            dialogReportOfHotel.title(`REPORT OF HOTEL: ${hotel.name}`);
            var res = await btaService.getInstance().bussinessTripApps.getDetailUsersInRoom({ type: $scope.reportType, code: dataItem.roomTypeCode }, queryArgs).$promise;
            if (res && res.isSuccess) {
                res.object.data.forEach(item => {
                    item.created = new Date(item.created);
                });
                setDataInDetailUserInRoom('#dialogReport', res.object.data, res.object.count);
            }
            dialogReportOfHotel.open();
            $rootScope.confirmDialogReport = dialogReportOfHotel;
        }
    }

    $scope.search = async function () {
        //let arg = buildArgForGetListReport();
        //await getListReports(arg, $scope.reportType);
        loadPageOne($scope.reportType);
    }

    $scope.clearSearch = function () {
        $scope.Init();
        $scope.$broadcast('resetToDate', $scope.searchInfo.toDate);
        clearDropDownList(["#location_id", "#airline_id", "#hotel_id", "#flight_number_id"]);
        setDataSourceLocations($scope.dataLocations);
        setDataSourceAirlines($scope.dataAirlines);
        setDataSourceHotels($scope.dataHotels);
        setDataSourceFlightNumbers($scope.dataFlightNumbers);
        loadPageOne();
    }



    $scope.searchDialogReport = function (keyWord) {
        var gridDialogReport = $("#dialogReport").data("kendoGrid");
        if (keyWord) {
            var dataDialogReportFilter = $scope.dataDialogReport.filter(x => {
                return x.sapCode && x.sapCode.toLowerCase().includes(keyWord.toLowerCase()) || x.fullName && x.fullName.toLowerCase().includes(keyWord.toLowerCase()) || x.departmentName && x.departmentName.toLowerCase().includes(keyWord.toLowerCase()) || x.referenceNumber && x.referenceNumber.toLowerCase().includes(keyWord.toLowerCase()) || x.flightNumberName && x.flightNumberName.toLowerCase().includes(keyWord.toLowerCase()) || x.hotelName && x.hotelName.toLowerCase().includes(keyWord.toLowerCase());
            });
            // if (gridDialogReport) {
            //     gridDialogReport.dataSource.data(dataDialogReportFilter);
            // }
            var dataSource = new kendo.data.DataSource({
                data: dataDialogReportFilter,
                //pageSize: 20,
                pageSize: gridDialogReport.dataSource._pageSize,
                schema: {
                    total: () => { return dataDialogReportFilter.length },
                    data: () => { return dataDialogReportFilter }
                }
            });

            gridDialogReport.setDataSource(dataSource);
        }
        else {
            var dataSource = new kendo.data.DataSource({
                data: $scope.oldDataDialogReport,
                //pageSize: 20,
                pageSize: gridDialogReport.dataSource._pageSize,
                pageable: {
                    alwaysVisible: true,
                    pageSizes: appSetting.pageSizesArray
                },
                schema: {
                    total: () => { return $scope.oldDataDialogReport.length },
                    data: () => { return $scope.oldDataDialogReport }
                }
            });

            gridDialogReport.setDataSource(dataSource);
            // if (gridDialogReport) {
            //     gridDialogReport.dataSource.data($scope.oldDataDialogReport);
            // }
        }
    }

    $scope.exportDialogReport = async function () {
        let res = {
            isSuccess: false,
            object: null,
        }
        let args = genarateQueryforReportDetail();
        if (args != null) {
            switch ($scope.reportType) {
                case appSetting.btaReportType.HOTEL:
                    {
                        if ($state.current.name == commonData.leaveManagement.myRequests) {
                            args.predicate = (args.predicate ? args.predicate + ' and ' : args.predicate) + `CreatedById = @${args.predicateParameters.length}`;
                            args.predicateParameters.push($rootScope.currentUser.id);
                        }
                        res = await btaService.getInstance().bussinessTripApps.export({ type: $scope.reportType }, args).$promise;
                        break;
                    }
                case appSetting.btaReportType.FLIGHTNUMBER:
                    {
                        res = await btaService.getInstance().bussinessTripApps.export({ type: $scope.reportType }, args).$promise;
                        break;
                    }
                case appSetting.btaReportType.STATUS:
                    {
                        res = await btaService.getInstance().bussinessTripApps.exportTypeStatus({ type: $scope.reportType }, args).$promise;
                        break;
                    }
                case appSetting.btaReportType.FLIGHTSBOOKING:
                    {
                        res = await fileService.getInstance().processingFiles.export({
                            type: commonData.exportType.FLIGHTSBOOKING
                        }, model).$promise;
                        break;
                    }
            }
            if (res.isSuccess) {
                exportToExcelFile(res.object);
                Notification.success(appSetting.notificationExport.success);
            } else {
                Notification.error(appSetting.notificationExport.error);
            }
        }
        else {
            Notification.error(appSetting.notificationExport.error);
        }
    }
    function genarateQueryforReportDetail() {
        let createType = ["BusinessTripApplication.Created", "BusinessTripApplication.Created"]
        let args = {
            predicate: "",
            predicateParameters: [$scope.keywordDialogReport],
            codes: [currentStatus],
        };
        switch ($scope.reportType) {
            ////
            case appSetting.btaReportType.STATUS: {
                let queryParams = currentStatus;
                let queryMethod = "=";
                if (currentStatus == "Revoking" || currentStatus == "In Progress") {
                    queryParams = "Waiting";
                    queryMethod = ".contains";
                }
                args.predicate = `(BusinessTripApplication.ReferenceNumber.contains(@0) || sapCode.contains(@0) || fullName.contains(@0) || departmentName.contains(@0))`;
                args.predicateParameters.push(queryParams);
                if (currentStatus != "Changing/ Cancelling Business Trip") {
                    args.predicate = args.predicate + `&& BusinessTripApplication.Status${queryMethod}(@1)`;
                }
                break;
            }
            //////
            case appSetting.btaReportType.HOTEL: {
                createType = ["checkInHotelDate", "checkOutHotelDate"];
                args.predicate = `(sapCode.contains(@0) || fullName.contains(@0) || departmentName.contains(@0)) && hotelCode = @1`;
                args.predicateParameters.push(currentParentStatus);
                break;
            }

            /////
            case appSetting.btaReportType.FLIGHTNUMBER: {
                createType = ["fromDate", "toDate"];
                args.predicate = `(sapCode.contains(@0) || fullName.contains(@0) || departmentName.contains(@0)) && flightNumberCode = @1`;
                args.predicateParameters.push(currentStatus);
            }
        }
        if ($scope.searchInfo.fromDate && isValidDate($scope.searchInfo.fromDate)) {
            const fromDate = moment($scope.searchInfo.fromDate, 'DD/MM/YYYY').format('MM/DD/YYYY');
            args.predicate = args.predicate ? args.predicate + `&& ${createType[0]}>= @${args.predicateParameters.length}` : `${createType[0]}>= @${args.predicateParameters.length}`;
            args.predicateParameters.push(fromDate);
        }
        if ($scope.searchInfo.toDate && isValidDate($scope.searchInfo.toDate)) {
            const toDate = moment($scope.searchInfo.toDate, 'DD/MM/YYYY').add(1, 'days').format('MM/DD/YYYY');
            args.predicate = args.predicate ? args.predicate + `&& ${createType[1]} < @${args.predicateParameters.length}` : `${createType[1]}< @${args.predicateParameters.length}`;
            args.predicateParameters.push(toDate);
        }
        return args;
    }
    async function ifEnter($event) {
        var keyCode = $event.which || $event.keyCode;
        if (keyCode === 13) {
            loadPageOne($scope.reportType);
        }
    }

    async function ifEnterDialogReport($event, keyWord) {
        var keyCode = $event.which || $event.keyCode;
        if (keyCode === 13) {
            $scope.searchDialogReport(keyWord);
        }
    }

    function loadPageOne() {
        let grid = $("#grid").data("kendoGrid");
        if (grid) {
            grid.dataSource.page(1);
        }
    }

    function buildArgForGetListReport(stutusName) {
        let option = {
            predicate: "",
            codes: [],
            predicateParameters: [],
            order: "Modified desc",
            limit: appSetting.pageSizeDefault,
            page: 1
        }
        const fromDate = moment($scope.searchInfo.fromDate, 'DD/MM/YYYY').format('MM/DD/YYYY');
        const toDate = moment($scope.searchInfo.toDate, 'DD/MM/YYYY').add(1, 'days').format('MM/DD/YYYY');
        if ($scope.reportType == appSetting.btaReportType.HOTEL) {
            //option.predicate = `hotelCode.contains(@0) && (DepartureCode.contains(@1) || ArrivalCode.contains(@2))`;
            //option.predicateParameters = [$scope.searchInfo.hotel, $scope.searchInfo.location, $scope.searchInfo.location];
            if ($scope.searchInfo.hotel) {
                option.predicate = `(hotelCode=@0 && arrivalCode = @1)`;
                option.predicateParameters = [$scope.searchInfo.hotel.trim(), $scope.searchInfo.location.trim()];
            } else if ($scope.searchInfo.location) {
                option.predicate = `arrivalCode = @0`;
                option.predicateParameters = [$scope.searchInfo.location.trim()];
            }

            if ($scope.searchInfo.fromDate && isValidDate($scope.searchInfo.fromDate)) {
                option.predicate = option.predicate ? option.predicate + ` && checkInHotelDate>= @${option.predicateParameters.length}` : `checkInHotelDate>= @${option.predicateParameters.length}`;
                option.predicateParameters.push(fromDate);
            }
            if ($scope.searchInfo.toDate && isValidDate($scope.searchInfo.toDate)) {
                option.predicate = option.predicate ? option.predicate + ` && checkOutHotelDate<= @${option.predicateParameters.length}` : `checkOutHotelDate<= @${option.predicateParameters.length}`;
                option.predicateParameters.push(toDate);
            }
        }

        if ($scope.reportType == appSetting.btaReportType.FLIGHTNUMBER) {
            if ($scope.searchInfo.flightNumber) {
                option.predicate = `(flightNumberCode = @0 && airlineCode = @1)`;
                option.predicateParameters = [$scope.searchInfo.flightNumber.trim(), $scope.searchInfo.airline.trim()];
            } else if ($scope.searchInfo.airline) {
                option.predicate = `airlineCode = @0`;
                option.predicateParameters = [$scope.searchInfo.airline.trim()];
            }
            if ($scope.searchInfo.fromDate && isValidDate($scope.searchInfo.fromDate)) {
                option.predicate = option.predicate ? option.predicate + ` && fromDate>= @${option.predicateParameters.length}` : `fromDate>= @${option.predicateParameters.length}`;
                option.predicateParameters.push(fromDate);
            }
            if ($scope.searchInfo.toDate && isValidDate($scope.searchInfo.toDate)) {
                option.predicate = option.predicate ? option.predicate + ` && toDate < @${option.predicateParameters.length}` : `toDate < @${option.predicateParameters.length}`;
                option.predicateParameters.push(toDate);
            }

        }

        if ($scope.reportType == appSetting.btaReportType.STATUS) {
            // if (_.findIndex($scope.searchInfo.status, x => { return x == 'Revoking' }) == -1) {

            // } else {
            //     if ($scope.searchInfo.fromDate) {
            //         option.predicate = option.predicate ? option.predicate + `&& created>= @${option.predicateParameters.length}` : `created>= @${option.predicateParameters.length}`;
            //         option.predicateParameters.push(fromDate);
            //     }
            //     if ($scope.searchInfo.toDate) {
            //         option.predicate = option.predicate ? option.predicate + `&& created<= @${option.predicateParameters.length}` : `created<= @${option.predicateParameters.length}`;
            //         option.predicateParameters.push(toDate);
            //     }
            // }
            let dateType = '';
            if (stutusName == "BusinessTripApplication.Status") {
                dateType = "BusinessTripApplication.created";
                generatePredicateExport(option, $scope.searchInfo.status, stutusName);
            } else {
                dateType = "created";
                generatePredicateWithStatus(option, $scope.searchInfo.status, stutusName);
            }
            if ($scope.searchInfo.fromDate && isValidDate($scope.searchInfo.fromDate)) {
                option.predicate = option.predicate ? option.predicate + `&& ${dateType}>= @${option.predicateParameters.length}` : `${dateType}>= @${option.predicateParameters.length}`;
                option.predicateParameters.push(fromDate);
            }
            if ($scope.searchInfo.toDate && isValidDate($scope.searchInfo.toDate)) {
                option.predicate = option.predicate ? option.predicate + `&& ${dateType} < @${option.predicateParameters.length}` : `${dateType} < @${option.predicateParameters.length}`;
                option.predicateParameters.push(toDate);
            }
        }
        //Flight booking
        if ($scope.reportType == appSetting.btaReportType.FLIGHTSBOOKING) {
            if ($scope.searchInfo.fromDate && isValidDate($scope.searchInfo.fromDate)) {
                option.predicate = option.predicate ? option.predicate + ` && created >= @${option.predicateParameters.length}` : `created >= @${option.predicateParameters.length}`;
                option.predicateParameters.push(fromDate);
            }
            if ($scope.searchInfo.toDate && isValidDate($scope.searchInfo.toDate)) {
                option.predicate = option.predicate ? option.predicate + ` && created < @${option.predicateParameters.length}` : `created < @${option.predicateParameters.length}`;
                option.predicateParameters.push(toDate);
            }
            if ($scope.searchInfo.status && $scope.searchInfo.status.length > 0) {
                generatePredicateWithStatus(option, $scope.searchInfo.status, stutusName);
            }
            if ($scope.searchInfo.departmentId) {
                option.predicate = option.predicate ? option.predicate + ` && BTADetail.DepartmentCode == @${option.predicateParameters.length}` : `BTADetail.DepartmentCode == @${option.predicateParameters.length}`;
                option.predicateParameters.push($scope.searchInfo.departmentId);
            }
            if ($scope.searchInfo.userSapCode) {
                option.predicate = option.predicate ? option.predicate + ` && BTADetail.UserId == @${option.predicateParameters.length}` : `BTADetail.UserId == @${option.predicateParameters.length}`;
                option.predicateParameters.push($scope.searchInfo.userSapCode);
            }
        }

        return option;
    }
    function generatePredicateWithStatus(option, statuses, statusColum = 'Status') {

        let predicateStatus = [];
        let predicateParameters = option.predicateParameters;
        let resultPredicate = ''
        if (!statuses) {
            statuses = $scope.statusOptions.dataSource.data.map(x => x.code);
        }
        if (statuses) {
            statuses.forEach(x => {
                predicateStatus.push(`${statusColum} = @${predicateParameters.length}`);
                predicateParameters.push(x);
            });
            resultPredicate += `(${predicateStatus.join(' or ')})`;
            if (resultPredicate == "()") {
                resultPredicate = "";
            }
        }
        option.predicate = option.predicate ? `(${option.predicate}) and (${resultPredicate})` : resultPredicate;

        option.predicateParameters = predicateParameters;

    }
    function generatePredicateExport(option, statuses, statusColum = 'BusinessTripApplication.Status') {
        if (statuses.length > 0) {
            if (!statuses.includes("Changing/ Cancelling Business Trip")) {
                let waitingStatus = ["Revoking", "In Progress"];
                let predicateStatus = [];
                let predicateParameters = option.predicateParameters;
                let resultPredicate = ''
                let parametersStatus = statuses;
                if (parametersStatus) {
                    let i = 0;
                    parametersStatus.forEach(x => {
                        if (waitingStatus.includes(x) && !predicateParameters.includes("Waiting")) {
                            predicateStatus.push(`${statusColum}.contains(@${i})`);
                            predicateParameters.push("Waiting");
                            i++;
                        } else if (!waitingStatus.includes(x)) {
                            predicateStatus.push(`${statusColum} = @${i}`);
                            predicateParameters.push(x);
                            i++;
                        }
                    });

                    resultPredicate += `(${predicateStatus.join(' or ')})`;
                    if (resultPredicate == "()") {
                        resultPredicate = "";
                    }
                }
                option.predicate = option.predicate ? `(${option.predicate}) and (${resultPredicate})` : resultPredicate;
                option.predicateParameters = predicateParameters;
            }
        }
        else {
            statuses = $scope.statusOptions.dataSource.data.map(x => x.code);
        }
        option.codes = statuses;
    }
    $scope.export = async function () {
        let res = {
            isSuccess: false,
            object: null,
        }
        if ($scope.reportType == appSetting.btaReportType.STATUS) {
            let args = buildArgForGetListReport('BusinessTripApplication.Status');
            res = await btaService.getInstance().bussinessTripApps.exportTypeStatus({ type: $scope.reportType }, args).$promise;
        }
        else if ($scope.reportType == appSetting.btaReportType.FLIGHTSBOOKING)
        {
            let option = buildArgForGetListReport();
            res = await fileService.getInstance().processingFiles.export({
                type: commonData.exportType.FLIGHTSBOOKING
            }, option).$promise;
        }
        else {
            let option = buildArgForGetListReport();
            if ($state.current.name == commonData.leaveManagement.myRequests) {
                option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `CreatedById = @${option.predicateParameters.length}`;
                option.predicateParameters.push($rootScope.currentUser.id);
            }
            res = await btaService.getInstance().bussinessTripApps.export({ type: $scope.reportType }, option).$promise;
        }
        if (res.isSuccess) {
            exportToExcelFile(res.object);
            Notification.success(appSetting.notificationExport.success);
        } else {
            Notification.error(appSetting.notificationExport.error);
        }
    }

    $scope.buttonExport = function (e) {
        if (!e.dialogVisible) {
            document.getElementById('exportCommon').style.display = '';
            document.getElementById('exportCommonButton').className = "btn btn-default dropdown-toggle";
            document.getElementById('exportCommonButton').style.backgroundColor = '';
        }
        else {
            document.getElementById('exportCommon').style.display = 'none';
            document.getElementById('exportCommonButton').style.backgroundColor = 'White';
        }
    }

    $scope.cancel = function () {
        $rootScope.cancel();
    }

    function setDataGrid(id, data, total) {
        let grid = $(id).data("kendoGrid");
        let dataSource = new kendo.data.DataSource({
            data: data,
            pageSize: 5,
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
    //Get department
    $scope.departments = [];
    $scope.departmentForGetDetail = [];
    $scope.toggleFilterPanel = function (value) {
        $scope.advancedSearchMode = value;
        if (value) {
            setDropDownTree('departmentId', allDepartments);
        }
    }
    $scope.departmentOptions = {
        placeholder: "",
        dataTextField: "name",
        dataValueField: "code",
        template: showCustomDepartmentTitle,
        valuePrimitive: true,
        checkboxes: false,
        autoBind: true,
        filter: "contains",
        filtering: async function (option) {
            await getDepartmentByFilter(option);
        },
        loadOnDemand: true,
        valueTemplate: (e) => showCustomField(e, ['name']),
        select: async function (e) {
            let dropdownlist = $("#departmentId").data("kendoDropDownTree");
            let dataItem = dropdownlist.dataItem(e.node);
            if (dataItem) {
                $scope.detail.newDeptOrLineCode = dataItem.code;
                $scope.detail.newDeptOrLineName = dataItem.name;
                $scope.detail.isStoreNewDepartment = dataItem.isStore;


            }
        }
    };
    async function getDepartmentByFilter(option) {
        if (!option.filter) {
            option.preventDefault();
        } else {
            let filter = option.filter && option.filter.value ? option.filter.value : "";
            arg = {
                predicate: "name.contains(@0) or code.contains(@1) or UserDepartmentMappings.Any(User.FullName.contains(@2))",
                predicateParameters: [filter, filter, filter],
                page: 1,
                limit: appSetting.pageSizeDefault,
                order: ""
            }
            res = await settingService.getInstance().departments.getDepartmentByFilter(arg).$promise;
            if (res.isSuccess) {
                setDataDepartment(res.object.data);
            }
        }
    }
    function setDataDepartment(dataDepartment) {
        var dataSource = new kendo.data.HierarchicalDataSource({
            data: dataDepartment,
            schema: {
                model: {
                    children: "items"
                }
            }
        });
        var departmentTree = $("#departmentId").data("kendoDropDownTree");
        if (departmentTree) {
            departmentTree.setDataSource(dataSource);
        }
    }

    var allDepartments;
    $scope.getDepartments = function () {
        allDepartments = JSON.parse(sessionStorage.getItemWithSafe("departments"));
        $scope.toggleFilterPanel(true);
    }


    //Data SAP CODE
    function showCustomSapCodeTitle(model) {
        if (model.sapCode) {
            return `${model.sapCode} - ${model.fullName}`
        } else {
            return `${model.fullName}`
        }
    }
    $scope.sapCodesDataSource = {
        dataTextField: 'sapCode',
        dataValueField: 'id',
        template: showCustomSapCodeTitle,
        valueTemplate: '#: sapCode #',
        valuePrimitive: true,
        autoBind: false,
        filter: "contains",
        dataSource: {
            serverFiltering: true,
            transport: {
                read: async function (e) {
                    await getUsers(e);
                }
            }
        },
        change: async function (e) {
           
        }
    };
   
    async function getUsers(option) {
        var filter = option.data.filter && option.data.filter.filters.length ? option.data.filter.filters[0].value : "";
        var arg = {
            predicate: "(sapcode.contains(@0) or fullName.contains(@1)) and IsActivated = @2",
            predicateParameters: [filter, filter, true],
            page: 1,
            limit: appSetting.pageSizeDefault,
            order: "sapcode asc"
        }
        var res = await settingService.getInstance().users.getUsers(arg).$promise;
        if (res.isSuccess) {
            $scope.dataUser = [];
            res.object.data.forEach(element => {
                if (element.isActivated) {
                    $scope.dataUser.push(element);
                }
            });
            if ($scope.selectedUserId) {
                let index = _.findIndex($scope.dataUser, x => {
                    return x.id == $scope.selectedUserId;
                });
                if (index == -1) {
                    $scope.dataUser.push({ id: $scope.detail.userId, fullName: $scope.detail.fullName, sapCode: $scope.detail.employeeCode });
                }
                $scope.selectedUserId = '';
            }
        }
        if (option) {
            option.success($scope.dataUser);
        }
    }
});