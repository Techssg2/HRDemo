var ssgApp = angular.module("ssg.targetPlanModule", [
    "kendo.directives"
]);
ssgApp.controller("targetPlanController", function (
    $rootScope,
    $scope,
    $location,
    appSetting,
    $stateParams,
    $state,
    moment,
    commonData,
    Notification,
    $sce,
    cbService,
    settingService,
    masterDataService,
    $timeout,
    workflowService,
    fileService,
    $translate, localStorageService,
    $compile,
    dataService,
    $q
) {
    $scope.titleHeader = 'TARGET PLAN';
    $scope.title = /*$translate.instant('TARGET_PLAN') + ': ' +*/ $stateParams.referenceValue;
    $scope.isITHelpdesk = $rootScope.currentUser && $rootScope.currentUser.isITHelpDesk;
    $scope.model = {};
    var activeUsers = [];
    var targetPlanData = [];
    var holidayDates = [];
    var voteModel = {};
    $scope.processingStageRound= 0;
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
    $scope.selectedTab = 0;
    $('#mySelect').on('change', function () {
        var selectedValue = $(this).val();
        if (selectedValue === "1") {
            $scope.selectedTab = 1;
            $state.go('home.targetPlan.allRequests')

        } else if (selectedValue === "0") {
            $state.go('home.targetPlan.myRequests')
        }
    });
    this.$onInit = async function () {
		var currentDate = new Date();
        $scope.localtimezone = -1 * currentDate.getTimezoneOffset();
		
        $scope.targetPlans = {
            placeholder: "",
            dataTextField: "name",
            dataValueField: "id",
            autoBind: false,
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
        $rootScope.showLoading();
        await getTargetPlanById();
        await getHolidays();
        await getWorkflowProcessingStage($stateParams.id);
        await getJobGradeList();
        findJobGrade();
        //if ($scope.model.status == 'Draft') {
        //    $state.reload();
        //}

        initTooltipContent();

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
        $rootScope.hideLoading();
    }

    let findJDG1, findJDG2, findJDG4, findJDG5 = null;
    function findJobGrade() {
        findJDG1 = (_.find($scope.jobGradeList, x => x.title?.toLowerCase() === "g1")?.grade) ?? 1;
        findJDG2 = (_.find($scope.jobGradeList, x => x.title?.toLowerCase() === "g2")?.grade) ?? 2;
        findJDG4 = (_.find($scope.jobGradeList, x => x.title?.toLowerCase() === "g4")?.grade) ?? 4;
        findJDG5 = (_.find($scope.jobGradeList, x => x.title?.toLowerCase() === "g5")?.grade) ?? 5;
    }



    async function getJobGradeList() {
        $scope.jobGradeList = [];
        let result = await settingService.getInstance().headcount.getJobGrades({
            predicate: "",
            predicateParameters: [],
            order: "Grade asc",
            limit: 200,
            page: 1,
        }).$promise;
        if (result.isSuccess) {
            $scope.jobGradeList = result.object.data;
        } else {
            Notification.error(result.messages[0]);
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
        let res = await cbService.getInstance().targetPlan.getItem({ id: $stateParams.id }, null).$promise;
        $scope.model = res.object;
        $scope.statusTranslate = $rootScope.getStatusTranslate($scope.model.status);
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
        var timezoneOffset = $scope.model.periodFromDate.getTimezoneOffset() * -1;
        var convertFromDate = new Date($scope.model.periodFromDate);
        if (timezoneOffset != $scope.localtimezone) {
            var diff = $scope.localtimezone - timezoneOffset;
            convertFromDate.setMinutes(convertFromDate.getMinutes() + diff);
        }

        var timezoneOffset = $scope.model.periodToDate.getTimezoneOffset() * -1;
        var convertToDate = new Date($scope.model.periodToDate);
        if (timezoneOffset != $scope.localtimezone) {
            var diff = $scope.localtimezone - timezoneOffset;
            convertToDate.setMinutes(convertToDate.getMinutes() + diff);
        }

        var columnInPeriods = getColumnFromPeriod({
            id: $scope.model.periodId,
            name: $scope.model.periodName,
            fromDate: convertFromDate,
            toDate: convertToDate
        });

        $scope.localtimezone = timezoneOffset;

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
            if (from.getTimezoneOffset() * -1 != 420) {
                from.setMinutes(from.getMinutes() + 420 - from.getTimezoneOffset() * -1);
            }
            var to = new Date(period.toDate);
            if (to.getTimezoneOffset() * -1 != 420) {
                to.setMinutes(to.getMinutes() + 420 - to.getTimezoneOffset() * -1);
            }
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
                        input_html = `<input ng-change="onChange(dataItem)" style="border: 1px solid #c9c9c9; text-align: center" class="k-input w100 targetSelect" ng-model="dataItem.targetField${moment(temp).format('YYYYMMDD')}" ng-readonly="${(disabledField || !allowEdit)}"/>`;
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
    function getTargetType(dataItem) {
        return dataItem.type == 'Target 1' ? 1 : 2;
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
    $scope.appove = async function () {
        var isSend = false; 
        if (arguments != null && arguments.length > 1) {
            isSend = true;
        } 
        var deferred = $q.defer();
        await $scope.save(isSend);
        var result = ($scope.errors != null && $scope.errors.length > 0) ? false : true;
        deferred.resolve({ object: $scope.model, isSuccess: result });
        return deferred.promise; 
    }
    $scope.showProcessingStages = function () {
        $rootScope.visibleProcessingStages($translate);
    }
    $scope.showTrackingHistory = function () {
        $rootScope.visibleTrackingHistory($translate, appSetting.TrackingLogDialogDefaultWidth);
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
            text: $translate.instant('COMMON_BUTTON_OK'),
            action: function (e) {
                if (!$scope.comment) {
                    $scope.target.errorMessage = $translate.instant('COMMON_COMMENT_VALIDATE');
                    $scope.$apply();
                    return false;
                } else if ($scope.arrayCheck && $scope.arrayCheck.length == 0) {
                    $scope.target.errorMessage = $translate.instant('TARGET_PLAN_VALIDATE_CHECK_USER');
                    $scope.$apply();
                    return false;
                }
                let selectedItems = [];
                debugger;
                $scope.arrayCheck.forEach(x => {
                    let users = _.filter($scope.model.targetPlanDetails, y => { return y.sapCode == x.sapCode });
                    if (users.length) {
                        users.forEach(t => {
                            selectedItems.push({ sapCode: t.sapCode, fullName: t.fullName, targetPlanDetailId: t.id, type: t.type, comment: '' });
                        });
                    }
                })
                //requestToChange(selectedItems);
                //console.log(selectedItems);
                voteModel.comment = $scope.comment;
                console.log(voteModel);
                requestToChange(selectedItems);
                return false;
            },
            primary: true
        }]
    };
    async function requestToChange(data) {
        workflowService.getInstance().workflows.vote(voteModel).$promise.then(async function (result) {
            if (result.messages.length == 0) {
                // Notification.success("Workflow has been processed");
                Notification.success($translate.instant('COMMON_WORKFLOW_PROCESSED'));
                let result = await cbService.getInstance().targetPlan.requestToChange({ periodId: $scope.model.periodId, targetPlanDetails: data }).$promise;
                $state.go('home.targetPlan.item', { id: voteModel.itemId, referenceValue: $scope.model.referenceNumber }, { reload: true });
            } else {
                if (result.messages && result.messages.length) {
                    Notification.error(result.messages[0]);
                }
                // $scope.voteDialog.close();
            }
        });
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
                title: //"<input type='checkbox' ng-model='allCheck' name='allCheck' id='allCheck' class='form-check-label enable-to-edit' ng-change='onChange(allCheck)'/> <label class='form-check-input' for='allCheck' style='padding-bottom: 10px;'></label>",
                "<label class='form-check-label' for='allCheck' style='padding-left: 30px; padding-bottom: 10px;'></label><input type='checkbox' ng-model='allCheck' name='allCheck' id='allCheck' class='form-check-input enable-to-edit' ng-change='onChange(allCheck)'/>",
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
    $scope.changeCheck = async function (dataItem) {
        if ($scope.allCheck) {
            $scope.allCheck = false;
        }
        $scope.userGrid.forEach(item => {
            if (item.sapCode == dataItem.sapCode) {
                if (item.isCheck) {
                    item.isCheck = false;
                    var arrayTemporary = $scope.arrayCheck.filter(x => x.sapCode != item.sapCode);
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
                await getSapCodeByDivison(1, 10000, $scope.keyWorkTemporary);
            }
            $scope.employeesDataSource.forEach(x => {
                let result = $scope.arrayCheck.find(y => y.sapCode == x.sapCode);
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
    $scope.gridUser = {
        keyword: '',
    }
    function popupToSelectEmployee() {
        $scope.keyWorkTemporary = '';
        $scope.target.errorMessage = '';
        $scope.limitDefaultGrid = 5;
        $scope.userGrid = [];
        $scope.arrayCheck = [];
        $scope.allCheck = false;
        $scope.gridUser.keyword = '';
        //$scope.employeesDataSource = [];
        let grid = $('#userGrid').data("kendoGrid");
        grid.dataSource.data($scope.employeesDataSource);
        // grid.dataSource.page(1);
        $scope.searchGridUser();
        enableElementToEdit();
        // set title cho cái dialog
        let dialog = $("#dialog_Detail").data("kendoDialog");
        dialog.title($translate.instant('COMMON_EMPLOYEE'));
        dialog.open();
        $rootScope.confirmDialogAddItemsUser = dialog;
    }
    function setGridUser(data, idGrid, total, pageIndex, pageSizes) {
        let grid = $(idGrid).data("kendoGrid");
        if (idGrid == '#userGrid') {
            let dataSource = new kendo.data.DataSource({
                data: data,
                pageSize: pageSizes,
                page: pageIndex,
                serverPaging: false,
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
    $scope.searchGridUser = async function () {
        $scope.userGrid = [];
        //$scope.allCheck = false;
        $scope.keyWorkTemporary = $scope.gridUser.keyword.trim().toLowerCase();
        let grid = $("#userGrid").data("kendoGrid");
        page = grid.pager.dataSource._page;
        pageSize = grid.pager.dataSource._pageSize;
        if ($scope.gridUser.keyword != null) {
            let result = {};
            let dataFilter = _.filter($scope.employeesDataSource, x => { return x.sapCode.toLowerCase().includes($scope.keyWorkTemporary) || x.fullName.toLowerCase().includes($scope.keyWorkTemporary) }).map(function (item) {
                return { ...item, showtextCode: item.sapCode }
            });
            $scope.total = dataFilter.length;
            let count = 0;
            let countCheck = 0;
            dataFilter.forEach(item => {
                item['no'] = count;
                item['isCheck'] = false;
                if ($scope.arrayCheck.length > 0) {
                    var result = $scope.arrayCheck.find(x => x.sapCode == item.sapCode);
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
        else {
            let count = 0;
            let countCheck = 0;
            $scope.userGrid.forEach(item => {
                item['no'] = count;
                item['isCheck'] = false;
                if ($scope.arrayCheck.length > 0) {
                    var result = $scope.arrayCheck.find(x => x.sapCode == item.sapCode);
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

    function isEnableFromDate(dataItem) {
        let result = [];
        let currentUser = _.find($scope.allUsers, x => { return x.sapCode == dataItem.sapCode });
        if (currentUser) {
            let tempStartDate = currentUser.startDate;
            let endDate = $scope.model.periodToDate;
            if (currentUser.officialResignationDate && (!dateFns.isAfter(new Date(dateFns.format(currentUser.officialResignationDate, 'MM/DD/YYYY')), new Date(dateFns.format($scope.model.periodToDate, 'MM/DD/YYYY'))))) {
                endDate = addDays(currentUser.officialResignationDate, -1);
            }
            if (tempStartDate) {
                var startDate = new Date();
                if (checkIsAfterDate($scope.model.periodFromDate, tempStartDate)) {
                    startDate = $scope.model.periodFromDate;
                } else {
                    startDate = tempStartDate;
                }
                if (endDate) {
                    while (checkIsBeforeDate(startDate, endDate)) {
                        result.push(`targetField${moment(startDate).format('YYYYMMDD')}`);
                        startDate = addDays(startDate, 1);
                    }
                }
            }
        }
        if (dataItem.sapCode == '00403029') {
            console.log(result);
        }
        return result;
    }

    async function validateERDG1(list) {
        $scope.errors = [];
        let sapCodes = _.map(_.uniqBy(list, 'sapCode'), 'sapCode').join(',');
        let res = await settingService.getInstance().users.getUsersByList({ sapCodes }, null).$promise;
        if (res.object && res.object.data) {
            let currentUsers = res.object.data;
            //console.log(currentUsers);
            let haveError = false;
            _.forEach(list, x => {
                let item = _.find(currentUsers, y => { return x.sapCode == y.sapCode });
                // if ((item.jobGradeValue == 1 || item.jobGradeValue == 2) && item.isStore) {
                if ((item.jobGradeValue == 1) && item.isStore) {
                    if (x.type == 'Target 1') {
                        Object.keys(x).forEach(fieldName => {
                            if (!haveError && fieldName.includes('targetField') && x[fieldName].includes('ERD')) {
                                $scope.errors.push({ controlName: `${x.sapCode}:`, message: $translate.instant('TARGET_PLAN_VALIDATE_STORE_G2') });
                                haveError = true;
                            }
                        })
                    } else {
                        Object.keys(x).forEach(fieldName => {
                            if (!haveError && fieldName.includes('targetField') && (x[fieldName].includes('ERD1') || x[fieldName].includes('ERD2'))) {
                                $scope.errors.push({ controlName: `${x.sapCode}:`, message: $translate.instant('TARGET_PLAN_VALIDATE_STORE_G2_ERD1_ERD2') });
                                haveError = true;
                            }
                        })
                    }
                }
            })
        }
        
        $scope.$applyAsync();
    }

    function validateTargetPlan(list) {
        // console.log(list);
        $scope.errors = [];
        let haveError = false;
        _.forEach(list, x => {
            if (x.type == 'Target 1') {
                let res = isEnableFromDate(x);
                if (res.length) {
                    Object.keys(x).forEach(fieldName => {
                        if (!haveError && fieldName.includes('targetField') && (x[fieldName] == null || x[fieldName] == undefined || x[fieldName].length == 0) && _.findIndex(res, t => { return t == fieldName }) > -1) {
                            let colDate = convertDateFieldToDate(fieldName);
                            $scope.errors.push({ controlName: `(${x.sapCode}) Shift Code on ${colDate}: `, message: $translate.instant('TARGET_PLAN_YOUR_EMPLOYEES_HAVE_NOT_MADE') });
                            haveError = true;
                        }
                    })
                } else {
                    Object.keys(x).forEach(fieldName => {
                        if (!haveError && fieldName.includes('targetField') && !x[fieldName]) {
                            $scope.errors.push({ controlName: 'Shift Code: ', message: $translate.instant('TARGET_PLAN_YOUR_EMPLOYEES_HAVE_NOT_MADE') });
                            haveError = true;
                        }
                    })
                }

            }
        })
    }

    function convertDateFieldToDate(fieldName) {
        let dateStr = fieldName.substring(fieldName.length - 8, fieldName.length);
        let year = dateStr.substring(0, 4);
        let date = dateStr.substring(8, 6);
        let month = dateStr.substring(4).substring(0, 2);
        return `${date}/${month}/${year}`;
    }

    $scope.onChange = function (item) {
        let ERDs = []
        let PRDs = [];
        let ALHs = [];
        let DOFLs = [];
        let DOHs = []; // target2
        Object.keys(item).forEach(function (fieldName) {
            if (fieldName.includes('targetField')) {
                if (item[fieldName].includes('ERD')) {
                    {
                        ERDs.push(item[fieldName]);
                    }
                }
                if (item[fieldName].includes('AL')) {
                    {
                        ALHs.push(item[fieldName]);
                    }
                }
                if (item[fieldName].includes('PRD')) {
                    {
                        PRDs.push(item[fieldName]);
                    }
                }
                if (item[fieldName].includes('DOFL')) {
                    {
                        DOFLs.push(item[fieldName]);
                    }
                }
                if (item[fieldName].includes('DOH')) {
                    {
                        DOHs.push(item[fieldName]);
                    }
                }
            }
        });
        if (item.type == 'Target 1') {
            item.erdQuality = ERDs.length;
            item.prdQuality = PRDs.length;
            item.alhQuality = ALHs.length;
            item.doflQuality = DOFLs.length;
        } else {
            item.erdQuality = ERDs.length / 2;
            item.prdQuality = PRDs.length / 2;
            item.alhQuality = ALHs.length / 2;
            item.doflQuality = DOHs.length / 2;
        }
        //khiem - fix bug 473 
        if (item) {
            if ($scope.userGrid && $scope.userGrid.length > 0) {
                $scope.userGrid.forEach(item => {
                    item.isCheck = false;
                    $scope.changeCheck(item);
                });
            }
            $scope.allCheck = true;
            setGridUser($scope.userGrid, '#userGrid', $scope.total, 1, $scope.limitDefaultGrid);
        }
        else {
            if ($scope.userGrid && $scope.userGrid.length > 0) {
                $scope.userGrid.forEach(item => {
                    item.isCheck = true;
                    $scope.changeCheck(item);
                });
            }
            $scope.allCheck = false;
            setGridUser($scope.userGrid, '#userGrid', $scope.total, 1, $scope.limitDefaultGrid);
        }
    }
    $scope.$on("customRequestToChange", async function (evt, data) {
        voteModel = data.voteModel;
        console.log(voteModel);
        popupToSelectEmployee();
    })
    $scope.$on("Send Request", async function (evt, data) {
        $scope.save(true);
    })
    async function validateTargetPlanDetail(model) {
        let errors = [];
        //validate ERD vs PRD
        let errorERDAndPRD = await validateERDUserSAP(model);
        errors = errors.concat(errorERDAndPRD);
        let error = await validateUserSAP(model);
        errors = errors.concat(error);
        return errors;
    }

    async function validateUserSAP(model) {
        let errors = [];
        let res = await cbService.getInstance().targetPlan.validateTargetPlanV2(model).$promise;
        if (res && res.isSuccess && res.object) {
            if (res.object.data) {
                res.object.data.forEach(item => {
                    if (item.typeName == commonData.validateTargetPlan.WFH) {
                        let text = item.sapCode + ' ' + $translate.instant('TARGET_PLAN_VALIDATE_WFH') + " (" + item.description + ") ";
                        let textError = {
                            controlName: $translate.instant('COMMON_ENTER_FIELD'),
                            message: text
                        }
                        errors.push(textError);
                    }
                });
            }
        }
        return errors;
    }

    function getCurrentDivisionByCurrentUserSubmit(currentItem) {
        if (currentItem && currentItem.divisions && currentItem.divisions.length) {
            return _.filter(currentItem.divisions, function (item) { return item.submitSAPCodes.includes($rootScope.currentUser.sapCode); });
        }
        return [];
    }
    async function validateERDUserSAP(model) {
        let errors = [];
        let res = await cbService.getInstance().targetPlan.validateTargetPlan(model).$promise;
        if (res && res.isSuccess && res.object) {
            if (res.object.data) {
                res.object.data.forEach(item => {
                    if (item.typeName == commonData.validateTargetPlan.ERD) {
                        let text = item.sapCode + ' ' + $translate.instant('TARGET_PLAN_VALIDATE_ERD')
                        let textError = {
                            controlName: $translate.instant('COMMON_ENTER_FIELD'),
                            message: text
                        }
                        errors.push(textError);
                    }
                    else if (item.typeName == commonData.validateTargetPlan.PRD) {
                        let text = item.sapCode + ' ' + $translate.instant('TARGET_PLAN_VALIDATE_PRD')
                        let textError = {
                            controlName: $translate.instant('COMMON_ENTER_FIELD'),
                            message: text
                        }
                        errors.push(textError);
                    } else if (item.typeName == commonData.validateTargetPlan.PRDSPECTICAL) {
                        let text = item.sapCode + ' ' + $translate.instant('TARGET_PLAN_VALIDATE_PRD_SPECIAL')
                        let textError = {
                            controlName: $translate.instant('COMMON_ENTER_FIELD'),
                            message: text
                        }
                        errors.push(textError);
                    }
                    else if (item.typeName == commonData.validateTargetPlan.G2) {
                        let text = item.sapCode + ' ' + $translate.instant('TARGET_PLAN_VALIDATE_STORE_G2')
                        let textError = {
                            controlName: $translate.instant('COMMON_ENTER_FIELD'),
                            message: text
                        }
                        errors.push(textError);
                    }
                    else if (item.typeName == commonData.validateTargetPlan.AL) {
                        let text = item.sapCode + ' ' + $translate.instant('TARGET_PLAN_VALIDATE_AL')
                        let textError = {
                            controlName: $translate.instant('COMMON_ENTER_FIELD'),
                            message: text
                        }
                        errors.push(textError);
                    }
                    else if (item.typeName == commonData.validateTargetPlan.DOFL) {
                        let text = item.sapCode + ' ' + $translate.instant('TARGET_PLAN_VALIDATE_DOFL')
                        let textError = {
                            controlName: $translate.instant('COMMON_ENTER_FIELD'),
                            message: text
                        }
                        errors.push(textError);
                    }
                });
            }
        }
        return errors;
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

    function convertJsonData(isSend = false) {
        let removeFieldFromModels = ['createdById', 'jsonData'];
        let model = $scope.model;
        let jsonData = model.jsonData;
        let list = [];
        var arrayDataFromJson = _.groupBy(JSON.parse(jsonData), 'sapCode');
        if (arrayDataFromJson) {
            Object.keys(arrayDataFromJson).forEach(function (targetPlan) {
                tempTarget1 = createData(arrayDataFromJson[targetPlan][0]);
                tempTarget2 = createData(arrayDataFromJson[targetPlan][1]);

                if (tempTarget1 && !tempTarget1.isSent) {
                    tempTarget1.isSent = isSend;
                }
                if (tempTarget2 && !tempTarget2.isSent) {
                    tempTarget2.isSent = isSend;
                }
                if (tempTarget1) {
                    tempTarget1.periodId = model.periodId;
                    list.push(tempTarget1);
                }
                if (tempTarget2) {
                    tempTarget2.periodId = model.periodId;
                    list.push(tempTarget2);
                }
            });
        }
        let viewModel = {};
        Object.keys(model).forEach(function (fieldName) {
            if (_.findIndex(removeFieldFromModels, x => { return x == fieldName }) == -1) {
                viewModel[fieldName] = model[fieldName];
            }
        });
        viewModel.list = list;
        console.log(viewModel);
        return viewModel;
    }

    function createData(item) {
        if (item) {
            var convertModel = { modified: new Date(), modifiedById: null, responseStatus: null, jsonData: '' };
            let tempJsonData = [];
            Object.keys(item).forEach(function (fieldName) {
                if (fieldName.includes('targetField')) {
                    tempJsonData.push({ date: fieldName.substring(fieldName.length - 8, fieldName.length), value: item[fieldName] });
                } else {
                    if (fieldName == 'type') {
                        convertModel[fieldName] = item[fieldName].substring(item[fieldName].length - 1, item[fieldName].length);
                    } else {
                        convertModel[fieldName] = item[fieldName];
                    }
                }

            });
            convertModel.jsonData = JSON.stringify(tempJsonData);
            return convertModel;
        }
    }

    $scope.save = async function (isSend = false) {
        $scope.errors = [];
        let data = null;

        if ($scope.visibleSubmitData) {
            data = $('#grid_user_id').data('kendoGrid').dataSource._data;
        } else {
            data = _.filter($('#grid_user_id').data('kendoGrid').dataSource._data, x => { return activeUsers.includes(x.sapCode) });
        }
        /*let userValidate = _.uniq(currentData.map(us => us.sapCode));
        if ($scope.model && userValidate && userValidate.length) {
            var currentItem = await cbService.getInstance().targetPlan.validateSubmitPendingTargetPlan({ periodId: $scope.model.periodId, sapCodes: userValidate }).$promise
            if (!currentItem.isSuccess && currentItem.errorCodes != null && currentItem.errorCodes.length > 0) {
                Notification.error($translate.instant(currentItem.messages[0]) + ": " + currentItem.object);
                return;
            }
        }*/
        await validateERDG1(data);
        if (isSend) {
            validateTargetPlan(data);
            $scope.$applyAsync();
        }
        // if (!$scope.errors.length) {
        //     validateTargetInDay(data);
        // }
        if (!$scope.errors.length) {
            $scope.model.jsonData = JSON.stringify(data);
            $scope.model.createdById = $rootScope.currentUser.id;
            //console.log($scope.model);
            // console.log(convertJsonData($scope.model.jsonData));
            let saveModel = convertJsonData(isSend);
            console.log(saveModel);
            $scope.errors = await validateTargetPlanDetail(saveModel);
            $scope.$applyAsync();
            //console.log(saveModel);
            if (!$scope.errors.length) {
                let res = await cbService.getInstance().targetPlan.sendRequest_TargetPlan({ isSend: isSend }, saveModel).$promise;
                if (res.isSuccess) {
                    Notification.success($translate.instant('COMMON_SAVE_SUCCESS'));
                    $state.go($state.current.name, {}, { reload: isSend });
                } else {
                    $timeout(function () {
                        if (res.errorCodes && res.errorCodes.length) {
                            $scope.errors = [];
                            for (let i = 0; i < res.errorCodes.length; i++) {
                                if (res.errorCodes[i] == 1) { // Code 1: validate shiftcode line 2 valid
                                    $scope.errors.push({ controlName: res.messages[i], message: $translate.instant('INVALID_SHIFT_CODE_TARGET_1') });
                                }
                                if (res.errorCodes[i] == 2) { // Code 2: validate shiftcode line 2 valid
                                    $scope.errors.push({ controlName: res.messages[i], message: $translate.instant('INVALID_SHIFT_CODE_TARGET_2') });
                                } else
                                if (res.errorCodes[i] == 3) { // Code 3: Validate cho TargetPlan Holidays 
                                    $scope.errors.push({ controlName: res.messages[i], message: $translate.instant('TARGET_PLAN_VALIDATE_HOLIDAYS') + `: ${res.object} `});
                                } else if (res.errorCodes[i] == 4) {
                                    $scope.errors.push({ controlName: res.messages[i], message: $translate.instant('TARGET_PLAN_YOUR_EMPLOYEES_HAVE_NOT_MADE') });
                                }
                                else if (res.errorCodes[i] == 5) { //
                                    var messArray = res.messages[i].split("|");
                                    $scope.errors.push({ controlName: `${messArray[0]}`, message: `: ${$translate.instant(messArray[1] == 'Target1' ? 'SHIFT_PLAN_TARGET_1' : 'SHIFT_PLAN_TARGET_2') + ' ' + $translate.instant('COMMON_IS_NOT_INVALID')}` });
                                }
                                else if (res.errorCodes[i] == 7) { //
                                    $scope.errors.push({ controlName: res.messages[i], message: $translate.instant('TARGET_PLAN_VALIDATE_HALFDAY') });
                                }
                                else if (res.errorCodes[i] == 9) { //
                                    $scope.errors.push({ controlName: res.messages[i], message: $translate.instant('TARGET_PLAN_NO_PRD') });
                                } else if (result.errorCodes[i] == 17) {
                                    $scope.errors.push({ controlName: res.messages[i], message: $translate.instant('INACTIVE_SHIFT_CODE_TARGET_1') });
                                } else if (result.errorCodes[i] == 18) {
                                    $scope.errors.push({ controlName: res.messages[i], message: $translate.instant('INACTIVE_SHIFT_CODE_TARGET_2') });
                                }
                                else {
                                    $scope.errors.push({ controlName: $translate.instant(`SHIFT_PLAN_TARGET_${res.errorCodes[i]}`), message: $translate.instant(res.messages[i]) });
                                }
                            }
                        }
                    }, 0);
                }
            }
        }

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