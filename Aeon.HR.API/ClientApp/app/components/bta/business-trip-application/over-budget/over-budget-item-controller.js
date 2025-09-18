var ssgApp = angular.module('ssg.overBudgetModule', ["kendo.directives"]);
ssgApp.controller('overBudgetItemController', function ($rootScope, $scope, $location, appSetting, localStorageService, $stateParams, $state, moment, Notification, settingService, $timeout, cbService, dataService
    , workflowService, ssgexService, fileService, commonData, $translate, attachmentService, attachmentFile, $compile, btaService) {
    $scope.titleHeader = $translate.instant('BUSINESS_TRIP_OVER_BUDGET').toUpperCase();
    $scope.title = $stateParams.id ? /*$translate.instant('BUSINESS_TRIP_OVER_BUDGET') + ': ' +*/ $stateParams.referenceValue : /*$translate.instant('BUSINESS_TRIP_OVER_BUDGET_NEW_TITLE')*/'';
    this.$onInit = function () {
        $scope.isITHelpdesk = $rootScope.currentUser && $rootScope.currentUser.isITHelpDesk;
        $scope.disableChangeOrCancel = false;
        $scope.perm = true;
        $scope.btaListOverBudgetData = [];
        $scope.searchTicketScope = {};
        $scope.viewTicketScope = {};
        $scope.viewChangeCancelTicketScope = {};
        $scope.searchTicketModel = {};
        $scope.ticketPriceInfos = {};
        $scope.sapCodeDataSource = [];
        $scope.model = [];
        $scope.isViewMode = false;
        $scope.allowEditPassengerCommentd = true;
        $scope.BTAApprovedDay = "";
        $scope.processingStageRound = 0;
        var voteModel = {};
        var workflowStatus = {};
        $scope.genderOption = [
            { name: 'Nam/ Male', value: 1 },
            { name: 'Nữ/ Female', value: 2 },
        ];
        $scope.businessTypeOption = [
            { name: "", value: 0 },
            { name: $translate.instant('BTA_DOMESTIC'), value: 1 },
            { name: $translate.instant('BTA_DOMESTIC_BY_PLANE'), value: 3 },
            { name: $translate.instant('BTA_INTERNATIONAL'), value: 2 },
            { name: $translate.instant('BTA_INTERNATIONAL_NO_FLIGHT'), value: 4 }
        ];
        //$scope.model.type = 0; // set type Dosmetic


        $scope.roundTripOption = [
            { name: "One-way", value: 0 },
            { name: "Return", value: 1 },
            { name: "", value: 2 }
        ];
        //$scope.model.roundTrip = 2;
        //$scope.model.isRoundTrip = false;
        $scope.roundTripChange = function (roundTripType) {
            if (roundTripType == 1)
                $scope.model.isRoundTrip = true;
            else
                $scope.model.isRoundTrip = false;
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
                    clearGrid('btaListOverBudgetDetailGrid');
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
                let grid = $("#btaListOverBudgetDetailGrid").data("kendoGrid");
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
        loadBTATripGroup();

        ngOnInit();
    }

    //$scope.btaListOverBudgetOption = {
    //    dataSource: {
    //        data: $scope.btaListOverBudgetData,
    //    },
    //    sortable: false,
    //    pageable: false,
    //    columns: btaTripGroupGridColumns
    //}
    async function ngOnInit() {
        await getBusinessTripLocations();
        //await getFlightNumbers();
        //await getRoomTypes();
        //await getAllHotels();

        $scope.isBookingFlightWF = false;

        if ($stateParams.id) {
            await getBusinessTripLocationById($stateParams.id);
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
                $("#attachmentChanging").attr("disabled", "disabled");
                $(".deleteattachmentChanging").addClass("display-none");
                $(".deleteattachmentDetail").removeClass("display-none");
            }, 0);
        }
        await getBTAApprovedDay();

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

    async function getBusinessTripLocationById(id) {
        var res = await btaService.getInstance().bussinessTripApps.getItemOverBudgetById({ id: id }, null).$promise;
        if (res.isSuccess) {
            $scope.model = res.object;
            $scope.statusTranslate = $rootScope.getStatusTranslate($scope.model.status);
            $scope.model.userCreatedFullName = $scope.model.userFullName;
            var dropdownDeptLine = $("#dept_line_id").data("kendoDropDownList");
            var dropdownDivision = $("#division_id").data("kendoDropDownTree");
            let count = 1;
            $scope.model.businessTripOverBudgetDetails.forEach(item => {
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

                //lamnl add:
                $(item).map(function (cindex, cItem) {
                    let isOverBudget = false;
                    let currentItemTicketPriceInfo = $scope.ticketPriceInfos[cItem.id] || {};

                    if (!$.isEmptyObject(cItem.flightDetails)) {
                        let flightInfoArray = new aeon.flightInfoArray(cItem.flightDetails);
                        currentItemTicketPriceInfo.totalPrice = flightInfoArray.getTotalPrice($scope.model.type);
                        currentItemTicketPriceInfo.limitBudget = getLimitBudget(cItem);
                        currentItemTicketPriceInfo.isOverBudget = currentItemTicketPriceInfo.totalPrice > currentItemTicketPriceInfo.limitBudget;
                        sOverBudget = currentItemTicketPriceInfo.isOverBudget;
                    }
                    $scope.ticketPriceInfos[cItem.id] = currentItemTicketPriceInfo;
                });

            });
            $scope.departureOption = businessTripLocations;
            $scope.arrivalOption = businessTripLocations;

            //var res2 = await btaService.getInstance().bussinessTripApps.getTripOverBudgetGroups({ id: $stateParams.id }, null).$promise;
            //$scope.btaListOverBudgetData = res2.object;
            //setDataSourceForGrid('#btaListOverBudgetDetailGrid', $scope.model.businessTripOverBudgetDetails);
            loadBTATripGroup();


            if ($scope.model.isRoundTrip) {
                $scope.model.roundTrip = 1;
            } else {
                $scope.model.roundTrip = 0;
            }

            //
            $scope.model.businessTripOverBudgetDetails.forEach(item => {

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


            if ($stateParams.id) {
                //$scope.model.isAgree = true;
                if ($scope.model.status != 'Draft') {
                    $('#argree_id').css('disabled', 'disabled');
                }
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
                    kendo.ui.progress($("#loading_overbudget"), true);
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
            kendo.ui.progress($("#loading_overbudget"), true);
            if (dropdownDeptLine) {
                dropdownDeptLine.value($scope.model.deptLineId);
            }
            if (dropdownDivision) {
                $scope.temporaryDivision = $scope.model.deptDivisionId;
                dropdownDivision.value($scope.model.deptDivisionId);
            }
            //$("#carRental_id").attr("disabled", "disabled");
            kendo.ui.progress($("#loading_overbudget"), false);

            if ($scope.model.type === 3 || $scope.model.type === 2) {//booking flight WF
                $scope.isBookingFlightWF = true;
            }
        }
    }

    async function getWorkflowProcessingStage(itemId) {
        var result = await workflowService.getInstance().workflows.getWorkflowStatus({ itemId: itemId }).$promise;
        if (result.isSuccess && result.object) {
            $scope.currentInstanceProcessingStage = result.object.workflowInstances[0];
            $scope.processingStageRound = result.object.workflowInstances.length;
            if (!$scope.currentInstanceProcessingStage) return;
            $scope.currentInstanceProcessingStage.workflowData.steps.map((item) => {
                var findHistories = _.find($scope.currentInstanceProcessingStage.histories, x => { return x.stepNumber == item.stepNumber });
                if (findHistories != null) {
                    item.histories = findHistories;
                    return item;
                }
            });
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
    async function getBTAApprovedDay() {
        if ($scope.model.businessTripApplicationId)
        {
            $scope.BTAApprovedDay = await btaService.getInstance().bussinessTripApps.getBTAApprovedDay({ businessTripApplicationId: $scope.model.businessTripApplicationId }, null).$promise;
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
    function getDataGrid(id) {
        let gridRoom = $(id).data("kendoGrid");
        return gridRoom.dataSource._data.toJSON();
    }



    async function loadBTATripGroup() {
        //var res = await btaService.getInstance().bussinessTripApps.getTripOverBudgetGroups({ id: $stateParams.id }, null).$promise;
        $scope.btaListTripGroupGridData = $scope.model.businessTripOverBudgetDetails;
        if ($scope.btaListTripGroupGridData && $scope.btaListTripGroupGridData.length > 0) {
            //$scope.btaListTripGroupGridData = res.object;
            $scope.tripGroupArray = new Array();
            $scope.allowSetTripGroupHasTicket = true;
            let tripGroupTicketInfo = {};

            //Collect ticket info and build tripgroup array
            $($scope.btaListTripGroupGridData).map(function (index, item) {

                //Prepare the maximum tripgroup array
                $scope.tripGroupArray.push(
                    {
                        "id": item.tripGroup,
                        "value": item.tripGroup,
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

            $scope.btaListTripGroupGridData.forEach(item => {
                item.toDate = new Date(item.toDate),
                    item.fromDate = new Date(item.fromDate)
            })
            $scope.btaListOverBudgetOption = ({
                dataSource: {
                    data: $scope.btaListTripGroupGridData,
                    sort: [{ field: "tripGroup", dir: "asc" }, { field: "fullName", dir: "desc" }],
                    group: [{ field: "tripGroup" }],
                    footerTemplate: "footerTemplate footerTemplate footerTemplate footerTemplate footerTemplate footerTemplate footerTemplate footerTemplate footerTemplate footerTemplate "
                },
                sortable: false,
                pageable: false,
                columns: btaTripGroupGridColumns,
                dataBound: function (e) {
                    dataView = this.dataSource.view();
                    //lamnl add:
                    // for (var i = 0; i < dataView.length; i++) {
                    // $(dataView[i].items).map(function (cindex, cItem) {
                    // let isOverBudget = false;
                    // let currentItemTicketPriceInfo = $scope.ticketPriceInfos[cItem.id] || {};

                    // if (!$.isEmptyObject(cItem.flightDetails)) {
                    // let flightInfoArray = new aeon.flightInfoArray(cItem.flightDetails);
                    // currentItemTicketPriceInfo.totalPrice = flightInfoArray.getTotalPrice($scope.model.type);
                    // currentItemTicketPriceInfo.limitBudget = getLimitBudget(cItem);
                    // currentItemTicketPriceInfo.isOverBudget = currentItemTicketPriceInfo.totalPrice > currentItemTicketPriceInfo.limitBudget;
                    // isOverBudget = currentItemTicketPriceInfo.isOverBudget;
                    // }

                    // if (isOverBudget) {
                    // var uid = cItem.uid;
                    // $("#btaListOverBudgetDetailGrid tbody").find("tr[data-uid=" + uid + "]").addClass("rOverBudget");  //alarm's in my style and we call uid for each row
                    // }
                    // $scope.ticketPriceInfos[cItem.id] = currentItemTicketPriceInfo;
                    // });
                    // }



                    //mergeGridRows('btaListOverBudgetDetailGrid','extraBudget');
                    //var columnIndex = this.wrapper.find(".k-grid-header [data-field=" + "UnitsInStock" + "]").index();
                    let headerCell = null;
                    // iterate the table rows and apply custom row and cell styling
                    var rows = e.sender.tbody.children();
                    for (let row of rows) {
                        if (row.className == "k-grouping-row ng-scope")
                            continue;
                        const firstCell = row.cells[11];
                        if (headerCell === null || firstCell.innerText !== headerCell.innerText) {
                            headerCell = firstCell;
                        } else {
                            headerCell.rowSpan++;
                            firstCell.remove();
                        }
                    }
                }
            });
            //Set tripgroup has ticket
            Object.keys(tripGroupTicketInfo).forEach(function (tripGroup) {
                $scope.setTripGroupHasTicket(tripGroup * 1, tripGroupTicketInfo[tripGroup]);
            });
        }
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
            let griddata = $("#btaListOverBudgetDetailGrid").data("kendoGrid").dataSource._data;
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
    //refresh the tripgroup with new data
    function RefreshTripGroupGridData(gridData) {
        //reset error message
        $scope.tripGroupPopup.errorMessages = null;

        let grid = $("#btaListOverBudgetDetailGrid").data("kendoGrid")
        grid.dataSource.sort({ field: "tripGroup", dir: "asc" }, { field: "fullName", dir: "desc" });
        if (!$.isEmptyObject(gridData)) {
            grid.dataSource.data = gridData;
        }
        grid.refresh();
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
            width: "120px",
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
            width: "250px",
            headerTemplate: $translate.instant('BTA_TRIP_FROM_DATE'),
            format: "{0:dd/MM/yyyy HH:mm}",
            editable: function (e) {
                return false;
            }
            ,
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
            width: "250px",
            headerTemplate: $translate.instant('BTA_TRIP_TO_DATE'),
            format: "{0:dd/MM/yyyy HH:mm}",
            editable: function (e) {
                return false;
            }
            ,
            template: function (dataItem) {
                return `<input kendo-date-time-picker
                    id="todate${dataItem.no}" ng-readonly="true"
                    k-ng-model="dataItem.toDate"
                    k-format="'dd/MM/yyyy HH:mm'"
                    k-time-format="'HH:mm'"
                    style="width: 100%;" />`;
            }
        },
        {
            //field: "userGradeValue",
            field: "userJobgradeTitle",
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
                if (!$.isEmptyObject(currentItemTicketPriceInfo) && currentItemTicketPriceInfo.isOverBudget == true) {
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
            field: "extraBudget",
            headerTemplate: $translate.instant('BTA_OVER_NEW_BUDGET'),
            width: "200px",
            groupFooterTemplate: function (data) {
                //return `<a class="k-primary float-right btn-sm" style='line-height: 0.8 !important;' ng-click= "tripGroupSearchTicket(2)">` + $translate.instant('COMMON_BUTTON_SEARCH_TICKET') + `</a>`;
                return `<kendo-button ng-show='doesAllowShowTripGroupButton(` + data.value + `, "viewTicket")' class="k-primary float-right btn-sm" style='line-height: 0.8 !important;' ng--click='showViewTicketPopup(` + data.value + `)'>` + $translate.instant('COMMON_BUTTON_VIEW_SELECTED_TICKET') + `</kendo-button>
                        <kendo-button data-ng-disabled='disableButton(1,"searchTicket")' ng-show='doesAllowShowTripGroupButton(` + data.value + `, "searchTicket")' class="k-primary float-right btn-sm" style='line-height: 0.8 !important;' ng-click='tripGroupSearchTicket(` + data.value + `)'>` + $translate.instant('COMMON_BUTTON_SEARCH_TICKET') + `</kendo-button>`;

                
            },
            editable: true,
            template: function (dataItem) {
                return `<input kendo-numeric-text-box k-min="0" ng-readonly="false" ng-model="dataItem.extraBudget" k-on-change= "getDataItemNo(dataItem)" style="width: 100%"/>`;
            }
        }
        //,
        //{
        //    field: "comments",
        //    headerTemplate: $translate.instant('BTA_OVER_BUDGET_REASON'),
        //    width: "280px",
        //    groupFooterTemplate: function (data) {
        //        //return `<a class="k-primary float-right btn-sm" style='line-height: 0.8 !important;' ng-click= "tripGroupSearchTicket(2)">` + $translate.instant('COMMON_BUTTON_SEARCH_TICKET') + `</a>`;
        //        return `<kendo-button ng-show='doesAllowShowTripGroupButton(` + data.value + `, "searchTicket")' class="k-primary float-right btn-sm" style='line-height: 0.8 !important;' ng--click='tripGroupSearchTicket(` + data.value + `)'>` + $translate.instant('COMMON_BUTTON_SEARCH_TICKET') + `</kendo-button>`;
        //    },
        //    editable: function (e) {
        //        return true;
        //    },
        //    template: function (dataItem) {
        //        return `<input ng-readonly="allowEditPassengerCommentd != true" class="k-input" type="text" ng-model="dataItem.comments" style="width: 100%"/>`;
        //    }
        //}
    ]
    $scope.itemNo = 0;
    $scope.extraBudgetItem = 0;
    $scope.modelExtraBudget = [];
    $scope.getDataItemNo = function (dataItem) {
        //var tripGroupExtra = _.find($scope.modelExtraBudget, x => { return x.tripGroup == dataItem.tripGroup });
        if ($scope.modelExtraBudget && $scope.modelExtraBudget.length > 0) {
            $scope.modelExtraBudget.forEach(item => {
                if (item.tripGroup == dataItem.tripGroup) {
                    item.extraBudget = dataItem.extraBudget;
                } else {
                    $scope.modelExtraBudget.push({ tripGroup: dataItem.tripGroup, extraBudget: dataItem.extraBudget });
                }
            })
        } else {
            $scope.modelExtraBudget.push({ tripGroup: dataItem.tripGroup, extraBudget: dataItem.extraBudget });
        }

    }

    $scope.doesAllowShowTripGroupButton = function (groupId, actionName) {
        let returnValue = true;
        if ($scope.model.status == 'Completed') {
            returnValue = false;
        }

        return returnValue;
    }
    $scope.tripGroupTemplate = `<span class="#: isDeleted ? 'k-state-disabled': ''#">
                                    #: value #
                                </span>`;

    $scope.tripGroupPopup = {};
    $scope.tripGroupSearchTicket = function (groupId) {
        let griddata = $("#btaListOverBudgetDetailGrid").data("kendoGrid").dataSource._data;
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
    function showSearchFlightPopup(searchTicketModel) {
        if (!$.isEmptyObject(searchTicketModel)) {
            if (!$.isEmptyObject($scope.searchTicketScope.loadFlight)) {//!
                $scope.searchTicketScope.setTripGroupHasTicket = $scope.setTripGroupHasTicket;
                $scope.searchTicketScope.loadFlight(searchTicketModel);

                //$('button:contains("Select")').attr('disabled', 'disabled');

                //// set title cho cái dialog
                //let dialog = $("#dialog_SearchTicket").data("kendoDialog")
                //dialog.title($translate.instant('COMMON_BUTTON_SEARCH_TICKET'));
                //dialog.open();
            }
        }
    }
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

    $scope.showProcessingStages = function () {
        $rootScope.visibleProcessingStages($translate);
    }
    $scope.showTrackingHistory = function () {
        $rootScope.visibleTrackingHistory($translate, appSetting.TrackingLogDialogDefaultWidth);
    }

    //$scope.$on('workflowStatus', async function (event, data) {

    //}


    $scope.save = async function (form, perm) {
        let dataGridBTADetail = $("#btaListOverBudgetDetailGrid").data("kendoGrid");
        //let extraBudget = 0;
        //dataGridBTADetail.dataSource._data.forEach(item => {
        //    if (item.extraBudget > 0)
        //        extraBudget = item.extraBudget;
        //})
        if ($scope.modelExtraBudget && $scope.modelExtraBudget.length > 0) {
            dataGridBTADetail.dataSource._data.forEach(item => {
                var tripGroupExtraData = _.find($scope.modelExtraBudget, x => { return x.tripGroup == item.tripGroup });
                if (tripGroupExtraData) {
                    item.extraBudget = tripGroupExtraData.extraBudget;
                } else {
                    item.extraBudget = 0;
                }
            })
        }


        $scope.model.businessTripOverBudgetDetails = JSON.stringify(dataGridBTADetail.dataSource._data.toJSON());;
        $scope.errors = [];
        //$scope.errors = validationBTADetail(); // validate trên cùng 1 phiếu
        if (!$scope.errors.length) {
            //var resValidate = await validateFromToDateBusinessDetail(dataGridBTADetail); // validate trên phiếu khác
            $scope.errors = [];//translateValidate(resValidate);
            if (!$scope.errors.length) {

                //detail
                if ($scope.attachmentDetails.length || $scope.removeFileDetails.length) {
                    let attachmentFilesJson = await mergeAttachment();
                    $scope.model.documentDetails = attachmentFilesJson;
                }
                var res = await btaService.getInstance().bussinessTripApps.saveBudget($scope.model).$promise;
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

    //$scope.$on('approvedBudget', async function (event, data) {
    //    voteModel = data.voteModel;
    //    await continueWorkFlow();
    //    var ress = await btaService.getInstance().bussinessTripApps.updateStatusBTA({ id: $scope.model.businessTripApplicationId, Status: 'Completed Request Over Budget' }).$promise;
    //    if (ress.isSuccess) {
    //        Notification.success($translate.instant('COMMON_SAVE_SUCCESS'));
    //        return true;
    //    } else {
    //        Notification.error("Error System");
    //        return false;
    //    }
    //});
    async function continueWorkFlow() {
        continueWorkflowNotSave();
    }
    function continueWorkflowNotSave() {
        workflowService.getInstance().workflows.vote(voteModel).$promise.then(function (result) {
            if (result.messages.length == 0) {
                // Notification.success("Workflow has been processed");
                Notification.success($translate.instant('COMMON_WORKFLOW_PROCESSED'));
                $state.go($state.current.name, { id: $scope.model.id, referenceValue: $scope.model.referenceNumber }, { reload: true });
            } else {
                Notification.error(result.messages[0]);
            }
        });
    }
    //loading Search Ticket
    $scope.dialogLoadingSearchOption = {
        width: "500px",
        buttonLayout: "normal",
        height: "400px",
        closable: true,
        modal: true,
        visible: false,
        content: "",
        close: async function (e) {
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
    // view ticket
    $scope.showViewTicketPopup = function (groupId) {
        if ((groupId * 1) > 0) {
            //var tripGroupViewTicket = _.find($scope.btaListTripGroupGridData, x => { return x.tripGroup == groupId });
            //$scope.viewTicketScope = tripGroupViewTicket.flightDetails;
            // HR-851 View vé trên phiếu BOB bị lỗi không hiển thị điều kiện vé và hành lý
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

        }
        catch (e) {
            setTimeout($scope.setTripGroupHasTicket, 100, groupId, ticketInfo);
        }
    }

});
