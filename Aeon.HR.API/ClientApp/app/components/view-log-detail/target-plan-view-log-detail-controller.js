var ssgApp = angular.module("ssg.targetPlanViewLogDetailModule", [
    "kendo.directives"
]);
ssgApp.controller("targetPlanViewLogDetailController", function (
    $rootScope,
    $scope,
    $location,
    appSetting,
    $stateParams,
    $state,
    moment,
    commonData,
    Notification,
    cbService,
    settingService,
    $timeout,
    workflowService,
    $translate,
    $q,
    trackingHistoryService
) {
    $scope.title = /*$translate.instant('TARGET_PLAN') + ': ' +*/ $stateParams.referenceValue;
    $scope.titleHeader = "TARGET PLAN";
    $scope.isITHelpdesk = $rootScope.currentUser && $rootScope.currentUser.isITHelpDesk;
    $scope.model = {};
    var activeUsers = [];
    var targetPlanData = [];
    var holidayDates = [];
    var voteModel = {};
    var columnsDaysInPeriod = [{
        field: "sapCode",
        //title: "Leave Kind",
        headerTemplate: $translate.instant('COMMON_SAP_CODE'),
        locked: true,
        width: 120
    }, {
        field: "fullName",
        //title: 'Leave Quota',
        headerTemplate: $translate.instant('COMMON_FULL_NAME'),
        locked: true,
        width: 180
    },
    {
        field: "departmentName",
        //title: 'Leave Quota',

        headerTemplate: $translate.instant('COMMON_DEPARTMENT'),
        locked: true,
        width: 200
    },
    {
        field: "type",
        //title: 'Leave Remains',
        headerTemplate: $translate.instant('MISSING_TIMECLOCK_TYPE'),
        locked: true,
        width: 120,
        template: function (dataItem) {
            let translateString = findStringTranslate(dataItem.type);
            return `<span>${$translate.instant(translateString)}</span>`
        }
    },
    {
        headerTemplate: `<div class="text-center">${$translate.instant('SHIFT_PLAN_COUNT_OFF_DAY')}</div>`,
        width: "200px",
        columns: [{
            field: "erdQuality",
            headerTemplate: '<div class="text-center">ERD</div>',
            width: 80,
            attributes: {
                "class": "table-cell"
            },
            template: function (dataItem) {
                return `<input style="border: 1px solid #c9c9c9; text-align: center" class="k-input w100" ng-model="dataItem.erdQuality" readonly/>`
            }
        }, {
            field: "prdQuality",
            headerTemplate: '<div class="text-center">PRD</div>',
            width: 80,
            attributes: {
                "class": "table-cell"
            },
            template: function (dataItem) {
                return `<input style="border: 1px solid #c9c9c9; text-align: center" class="k-input w100" ng-model="dataItem.prdQuality" readonly/>`
            }
        },
        {
            field: "alhQuality",
            headerTemplate: '<div class="text-center">AL</div>',
            width: 80,
            attributes: {
                "class": "table-cell"
            },
            template: function (dataItem) {
                return `<input style="border: 1px solid #c9c9c9; text-align: center" class="k-input w100" ng-model="dataItem.alhQuality" readonly/>`
            }
        },
        {
            field: "doflQuality",
            headerTemplate: '<div class="text-center">DOFL</div>',
            width: 80,
            attributes: {
                "class": "table-cell"
            },
            template: function (dataItem) {
                return `<input style="border: 1px solid #c9c9c9; text-align: center" class="k-input w100" ng-model="dataItem.doflQuality" readonly/>`
            }
        }]
    }];
    this.$onInit = async function () {
        $scope.targetPlans = {
            placeholder: "",
            dataTextField: "name",
            dataValueField: "id",
            autoBind: true,
            valuePrimitive: true,
            filter: "contains",
            dataSource: targetPlanData
        }
        $scope.deptLineOptions = {
            dataTextField: 'name',
            dataValueField: 'id',
            autoBind: false,
            valuePrimitive: true,
            filter: "contains",
            dataSource: []
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
            loadOnDemand: false,
            valueTemplate: (e) => showCustomField(e, ['name'])
        }
        $scope.gridUsers = {
            dataSource: {
                data: []
            },
            sortable: false,
            autoBind: true,
            valuePrimitive: true,
            columns: columnsDaysInPeriod,
            dataBound: function (e) {
                var items = e.sender.items();
                items.each(function (idx, item) {
                    if (e.sender.dataItem(item).isUserLastRecord) {
                        $(item).addClass('separate');
                    }
                });
            }
        }
        await getTargetPlanById();
        await getHolidays();
        //if ($scope.model.status == 'Draft') {
        //    $state.reload();
        //}

        initTooltipContent();
    }

    function doesRequestToChangeStatus() {
        try {
            return $scope.model.status.toLowerCase() == "requested to change";
        }
        catch (e) {
            return false;
        }
    }
    function doesDraftStatus() {
        try {
            return $scope.model.status.toLowerCase() == "draft";
        }
        catch (e) {
            return false;
        }
    }

    function doesCurrentUserAuthor() {
        try {
            return $scope.model.userSAPCode == $scope.currentUser.sapCode;

        } catch (e) {
            return false;
        }
    }

    async function getTargetPlanById() {
        let res = await await trackingHistoryService.getInstance().trackingHistory.getTrackingHistoryById({ Id: $stateParams.id }).$promise;
        $scope.model = JSON.parse(res.object.dataStr);
        $scope.title = /*$translate.instant('TARGET_PLAN') + ': ' +*/ $scope.model.referenceNumber;
        $scope.model.createdByFullName = $scope.model.userFullName;
        if (doesRequestToChangeStatus() || doesDraftStatus()) {
            let pendingTargetDetails = await cbService.getInstance().targetPlan.getSAPCode_PendingTargetPlanDetails({ id: $stateParams.id }, null).$promise;
            if (!$.isEmptyObject(pendingTargetDetails) && !$.isEmptyObject(pendingTargetDetails.object) && pendingTargetDetails.object.length > 0) {
                activeUsers = pendingTargetDetails.object.map(function (currentItem) {
                    return currentItem.sapCode;
                });
                $scope.allUsers = pendingTargetDetails.object;
            }
        }

        if ($scope.model.deptId) {
            deptItems = [{ id: $scope.model.deptId, name: $scope.model.deptName, code: $scope.model.deptCode }];
            divisionItems = [{ id: $scope.model.divisionId, name: $scope.model.divisionName, code: $scope.model.divisionCode }];
            periodItems = [{ id: $scope.model.periodId, name: $scope.model.periodName }]
            setDataDropdownList('#dept_line_id', deptItems, $scope.model.deptId);
            setDropDownTree('division_id', divisionItems, $scope.model.divisionId);
            setDataDropdownList('#period_source_id', periodItems, $scope.model.periodId);
        }
        console.log($scope.model);
        let targetPlanDetailItems = $scope.model.targetPlanDetails;

        if (targetPlanDetailItems && targetPlanDetailItems.length) {
            let currentUsers = [];
            let employeeData = [];
            _.forEach(targetPlanDetailItems, x => {
                var currentObject = { 'sapCode': x.sapCode, 'fullName': x.fullName, 'departmentName': x.departmentName, 'departmentCode': x.departmentCode, 'type': `${commonData.targetPlanType[`target${x.type}`]}`, erdQuality: x.erdQuality, prdQuality: x.prdQuality, alhQuality: x.alhQuality, doflQuality: x.doflQuality, isSent: x.isSent, isSubmitted: x.isSubmitted };
                let list = JSON.parse(x.jsonData);
                _.forEach(list, y => {
                    currentObject[`targetField${y.date}`] = y.value;
                })

                currentUsers.push(currentObject);
                if (_.findIndex(employeeData, y => { return y.sapCode == x.sapCode }) == -1) {
                    employeeData.push({ sapCode: x.sapCode, fullName: x.fullName, departmentName: x.departmentName })
                }
            });
            currentUsers = updateTargetPlanDetails(currentUsers);
            renderData(currentUsers);
            $scope.employeesDataSource = employeeData;
        }
    }
    function updateTargetPlanDetails(listUsers) {
        let currentUsers = [];
        var columnInPeriods = getColumnFromPeriod({
            id: $scope.model.periodId,
            name: $scope.model.periodName,
            fromDate: new Date($scope.model.periodFromDate),
            toDate: new Date($scope.model.periodToDate)
        });
        if (columnInPeriods.length) {
            let tempColumns = columnsDaysInPeriod.concat(columnInPeriods);
            refreshGridColumns(tempColumns);
        };
        let orderItems = _.sortBy(listUsers, ['sapCode', 'type']);
        for (var i = 1; i < orderItems.length; i += 2) {
            orderItems[i].isUserLastRecord = true;
        }
        _.forEach(orderItems, t => {
            currentUsers.push(t);
        });

        return currentUsers;
    }
    function renderData(currentData) {
        $timeout(function () {
            if (currentData.length) {
                setDataSourceForGrid('#grid_user_id', currentData);
            } else {
                setDataSourceForGrid('#grid_user_id', []);
            }
        }, 0);
    }
    async function getHolidays() {
        let res = await settingService.getInstance().cabs.getHolidaySchedules({
            predicate: "",
            predicateParameters: [],
            Order: "Created desc",
            Limit: 1000,
            Page: 1
        }).$promise;
        if (res && res.object && res.object.data) {
            _.forEach(res.object.data, x => {
                let tempDate = x.fromDate;
                while (tempDate <= x.toDate) {
                    holidayDates.push(tempDate);
                    tempDate = addDays(tempDate, 1);
                }
            })
        }
    }
    function getColumnFromPeriod(period) {
        let result = [];
        if (period && period.fromDate && period.toDate) {
            var from = new Date(period.fromDate);
            var to = new Date(period.toDate);
            var ite = from;
            var temp = from;
            var noTemp = 1;
            while (ite <= to) {
                result.push({
                    field: `targetField${moment(ite).format('YYYYMMDD')}`,
                    //title: `${getDayFromDate(ite)} ${moment(ite).format('DD/MM/YYYY')}`,
                    width: 100,
                    attributes: {
                        "class": "table-cell"
                    },
                    headerTemplate: `<div ng-class="{'is-header-sun-day': ${getDayFromDate(ite) == 'Sun'}, 'is-header-holiday-day': ${isHoliday(ite)}}" style="text-align: center"> <div ng-class="{'is-header-sun-day': ${getDayFromDate(ite) == 'Sun'}, 'is-header-holiday-day': ${isHoliday(ite)}}"> ${getDayFromDate(ite)}</div> <div ng-class="{'is-header-sun-day': ${getDayFromDate(ite) == 'Sun'}, 'is-header-holiday-day': ${isHoliday(ite)}}">${moment(ite).format('DD/MM/YYYY')}</div></div>`,
                    template: function (dataItem) {
                        var input_html = '';
                        var allowEdit = false;
                        if ((doesRequestToChangeStatus() || doesDraftStatus()) && doesCurrentUserAuthor()) {
                            allowEdit = activeUsers.indexOf(dataItem.sapCode) > -1;
                        }

                        let disabledField = false;

                        let validUser = _.find($scope.allUsers, x => { return x.sapCode == dataItem.sapCode });
                        if (validUser) {
                            if (validUser.startDate && validUser.officialResignationDate) {
                                disabledField = (new Date(dateFns.format(validUser.startDate, 'MM/DD/YYYY')) - new Date(dateFns.format(temp, 'MM/DD/YYYY')) > 0) || (new Date(dateFns.format(validUser.officialResignationDate, 'MM/DD/YYYY')) - new Date(dateFns.format(temp, 'MM/DD/YYYY')) < 0);
                            } else if (validUser.startDate) {
                                disabledField = new Date(dateFns.format(validUser.startDate, 'MM/DD/YYYY')) - new Date(dateFns.format(temp, 'MM/DD/YYYY')) > 0;
                            }
                        }
                        input_html = `<input ng-change="onChange(dataItem)" style="border: 1px solid #c9c9c9; text-align: center" class="k-input w100 targetSelect" ng-model="dataItem.targetField${moment(temp).format('YYYYMMDD')}" ng-readonly="true"/>`;
                        temp = temp >= to ? from : addDays(temp, 1);
                        noTemp = temp >= to ? noTemp : noTemp++;
                        return input_html;
                    }

                });
                ite = addDays(ite, 1);
            }
        }
        return result;
    }
    function getDayFromDate(date) {
        switch (date.getDay()) {
            case 0:
                return 'Sun';
            case 1:
                return 'Mon';
            case 2:
                return 'Tue';
            case 3:
                return 'Wed';
            case 4:
                return 'Thu';
            case 5:
                return 'Fri';
            case 6:
                return 'Sat';
        }
    }
    function refreshGridColumns(columns) {
        $timeout(function () {
            $scope.gridUsers.columns = columns;
        }, 0);
    }
    function isHoliday(date) {
        var index = _.findIndex(holidayDates, x => {
            return moment(x).format('DDMMYYYY') == moment(date).format('DDMMYYYY');
        });
        return index > -1;
    }
    function findStringTranslate(type) {
        let result = '';
        switch (type) {
            case commonData.targetPlanType.target1:
                result = 'SHIFT_PLAN_TARGET_1';
                break;
            case commonData.targetPlanType.target2:
                result = 'SHIFT_PLAN_TARGET_2';
                break;
            default:
                break;
        }
        return result;
    }
    $scope.target = {
        errorMessage: ''
    }
    $scope.userListGridOptions = {
        dataSource: {
            serverPaging: false,
            pageSize: 5,
            data: $scope.employeesDataSource,
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
                title: "<input type='checkbox' ng-model='allCheck' name='allCheck' id='allCheck' class='k-checkbox enable-to-edit' ng-change='onChange(allCheck)'/> <label class='k-checkbox-label cbox' for='allCheck' style='padding-bottom: 10px;'></label>",
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
                width: "150px",
            },
            {
                field: "fullName",
                // title: "Full Name",
                headerTemplate: $translate.instant('COMMON_FULL_NAME'),
                width: "200px",
            },
            {
                field: "departmentName",
                // title: "Department",
                headerTemplate: $translate.instant('COMMON_DEPARTMENT'),
                width: "200px",
            }
        ]
    }
    function initTooltipContent() {
        var iconURL = `<img src="ClientApp/assets/images/icon/[color].png" style="height:10px">`
        var redContent = "<div><span style='float: left'>" + iconURL.replaceAll("[color]", "red") + " " + $translate.instant("SHIFT_PLAN_RED_CONTENT") + "</span> </br>";
        var greenContent = "<span style='float: left'>" + iconURL.replaceAll("[color]", "green") + " " + $translate.instant("SHIFT_PLAN_GREEN_CONTENT") + "</span> </br>";
        var blueContent = "<span style='float: left'>" + iconURL.replaceAll("[color]", "blue") + " " + $translate.instant("SHIFT_PLAN_BLUE_CONTENT") + "</span> </br>";
        var yellowContent = "<span style='float: left'>" + iconURL.replaceAll("[color]", "yellow") + " " + $translate.instant("SHIFT_PLAN_YELLOW_CONTENT") + "</span> </br>";
        var orangeContent = "<span style='float: left'>" + iconURL.replaceAll("[color]", "orange") + " " + $translate.instant("SHIFT_PLAN_ORANGE_CONTENT") + "</span></div>";

        $scope.colorContent = redContent + greenContent + blueContent + yellowContent + orangeContent;
    }
});