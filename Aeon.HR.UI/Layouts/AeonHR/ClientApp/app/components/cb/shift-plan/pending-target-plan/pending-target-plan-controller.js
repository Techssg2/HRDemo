var ssgApp = angular.module("ssg.pendingTargetPlanModule", [
    "kendo.directives"
]);

ssgApp.controller("pendingTargetPlanController", function (
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
    attachmentService,
    $translate, localStorageService,
    $compile,
    dataService,
    $window,
    $http
) {
    $scope.title = $stateParams.action.title;
    $scope.title = $translate.instant('TARGET_PLAN_NEW_TITLE');
    targetPlans = [];
    $scope.model = {};
    $scope.submitModel = {};
    $scope.warningDialog = {};
    $scope.importDialog = {};
    $scope.isSubmit = true;
    isGetData = false;
    $scope.isShowAllDivision = false;
    $scope.visibleSendData = false;
    $scope.visibleSubmitData = false;
    $scope.allUsers = [];
    $scope.attachmentDetails = [];
    $scope.disableDivison = [];
    var activeUsers = [];
    var holidayDates = [];
    var targetPlanData = [];
    var targetPlanDetailItems = [];
    var validUserMappings = [];
    var detailTargetPlan = [];
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
        width: 120
    },
    {
        field: "departmentName",
        //title: 'Leave Quota',
        headerTemplate: $translate.instant('COMMON_DEPARTMENT'),
        locked: true,
        width: 120
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
        }
        ]
    }];
    this.$onInit = async function () {
        setPagingDefault();
        $scope.errors = [];
        $scope.targetPlans = {
            placeholder: "",
            dataTextField: "name",
            dataValueField: "id",
            autoBind: true,
            valuePrimitive: true,
            filter: "contains",
            dataSource: targetPlanData,
            change: async function (e) {
                $scope.errors = [];
                let dropdownlist = $("#period_source_id").data("kendoDropDownList");
                let dataItem = dropdownlist.dataItem(e.node);
                $scope.model.periodId = dataItem.id;
                $scope.model.periodName = dataItem.name;
                $scope.model.periodFromDate = dataItem.fromDate;
                $scope.model.periodToDate = dataItem.toDate;
                await $scope.onCheckStatus();
            },
            select: function () {
                $scope.isShowAllDivision = false;
            }
        }
        $scope.deptLineOptions = {
            dataTextField: 'name',
            dataValueField: 'id',
            autoBind: false,
            valuePrimitive: true,
            filter: "contains",
            dataSource: [],
            change: async function (e) {
                $scope.errors = [];
                setPagingDefault();
                $scope.total = 0;
                $scope.errorsTwo = {};
                let dropdownlist = $("#dept_line_id").data("kendoDropDownList");
                let dataItem = dropdownlist.dataItem(e.node)
                if (dataItem) {
                    $scope.model.deptId = dataItem.id;
                    $scope.model.deptCode = dataItem.code;
                    $scope.model.deptName = dataItem.name;
                    clearGrid('grid_user_id');
                    var dropdownTree = $(`#division_id`).data("kendoDropDownTree");
                    if (dropdownTree) {
                        dropdownTree.setDataSource([]);
                    }
                    await loadUserByDeptLine(dataItem);
                }
            },
            select: function () {
                $scope.isShowAllDivision = false;
                $scope.allUsers = [];
            }
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
                return `<span class="${dataItem.item.jobGradeGrade > 4 || $scope.disableDivison.includes(dataItem.item.id) ? 'k-state-disabled' : ''}">${showCustomDepartmentTitle(dataItem)}</span>`;
            },
            loadOnDemand: false,
            valueTemplate: (e) => showCustomField(e, ['name']),
            select: async function (e) {
                $scope.isShowAllDivision = false;
                setPagingDefault(1, $scope.page.limit);
                $scope.errors = [];
                let dropdownlist = $("#division_id").data("kendoDropDownTree");
                let dataItem = dropdownlist.dataItem(e.node)
                if (dataItem.jobGradeGrade > 4 || $scope.disableDivison.includes(dataItem.id)) {
                    e.preventDefault();
                } else {
                    $scope.currentDivisionId = dataItem.id;
                    $scope.model.divisionId = dataItem.id;
                    $scope.model.divisionCode = dataItem.Code;
                    $scope.model.divisionName = dataItem.Name;
                }
                //await checkShowSubmitButton(dataItem);
                await getValidUserSAPForTargetPlan();
                await checkShowAction();
                dropdownlist.close();
            }
        }
        $scope.gridUsers = {
            dataSource: {
                serverPaging: true,
                pageSize: 100,
            },
            sortable: false,
            pageable: {
                alwaysVisible: true,
                pageSizes: appSetting.pageSizesArrayMax,
                messages: {
                    display: "{0}-{1} " + $translate.instant('PAGING_OF') + " {2} " + $translate.instant('PAGING_ITEM'),
                    itemsPerPage: $translate.instant('PAGING_ITEM_PER_PAGE'),
                    empty: $translate.instant('PAGING_NO_ITEM')
                }
            },
            autoBind: true,
            columns: columnsDaysInPeriod,
            dataBinding: async function (e) {
                let limitByPerPage = e.sender.dataSource._take;
                if (limitByPerPage && limitByPerPage != $scope.page.limit) {
                    setPagingDefault(1, limitByPerPage)
                    let divisionIds = !$scope.currentDivisonIds && $rootScope.currentUser ? [$rootScope.currentUser.divisionId] : $scope.currentDivisonIds;
                    await getSapCodeByDivision(divisionIds, null, true);
                }
            },
            dataBound: function (e) {
                var lockTableRows = e.sender.lockedContent.children()[0].tBodies[0].rows;
                for (var i = 0; i < lockTableRows.length; i++) {
                    let row = $(lockTableRows[i]);
                    let dataItem = e.sender.dataItem(row);
                    let inactive = dataItem.get("isSubmitted");
                    if (!inactive && $scope.visibleSubmitData) {
                        row.addClass("highlight-not-sent-target");
                    }
                }
                var rows = e.sender.tbody.children();
                for (var j = 0; j < rows.length; j++) {
                    let row = $(rows[j]);
                    let dataItem = e.sender.dataItem(row);
                    let inactive = dataItem.get("isSubmitted");
                    if (!inactive && $scope.visibleSubmitData) {
                        row.addClass("highlight-not-sent-target");
                    }
                }
                var items = e.sender.items();
                items.each(function (idx, item) {
                    if (e.sender.dataItem(item).isUserLastRecord) {
                        $(item).addClass('separate');
                    }
                });
            },
            page: async function (option) {
                let divisionIds = !$scope.currentDivisonIds && $rootScope.currentUser ? $rootScope.currentUser.divisionId : $scope.currentDivisonIds;
                await getSapCodeByDivision(divisionIds, option, true);

            }

        }
        initTooltipContent();
        await getHolidays();
    }
    function setPagingDefault(page, limit) {
        $scope.page = {
            current: page ? page : 1,
            limit: limit ? limit : 100
        }
    }
    async function loadUserByDeptLine(dataItem) {

        var currentItem = _.find($scope.deptLineList, x => { return x.deptLine.id == $scope.model.deptId });

        if (!currentItem.divisions.length || $scope.isShowAllDivision) {
            await getValidUserSAPForTargetPlan();
            await getDivisionsByDeptLine(dataItem.id);
        } else {
            var ids = _.map(_.uniqBy(currentItem.divisions, 'id'), 'id');
            await getDivisionTreeByIds(ids);
            await getValidUserSAPForTargetPlan();
            let currentData = await getDetailData();
            detailTargetPlan = currentData;
            if (currentData.length) {
                renderData(currentData);
            } else {
                let periodData = initPeriod($scope.model);
                let currentData = initData(periodData);
                renderData(currentData);
            }
        }
        await checkShowAction();

    }
    function getColumnFromPeriod(period) {
        let result = [];
        if (period && period.fromDate && period.toDate) {
            var from = period.fromDate;
            var to = period.toDate;
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
                    headerTemplate: `<div ng-class="{'is-header-sun-day': ${getDayFromDate(ite) == 'SHIFT_PLAN_SUN'}, 'is-header-holiday-day': ${isHoliday(ite)}}" style="text-align: center"> <div ng-class="{'is-header-sun-day': ${getDayFromDate(ite) == 'SHIFT_PLAN_SUN'}, 'is-header-holiday-day': ${isHoliday(ite)}}"> ${$translate.instant(getDayFromDate(ite))}</div> <div ng-class="{'is-header-sun-day': ${getDayFromDate(ite) == 'SHIFT_PLAN_SUN'}, 'is-header-holiday-day': ${isHoliday(ite)}}">${moment(ite).format('DD/MM/YYYY')}</div></div>`,
                    template: function (dataItem) {
                        dataItem['isActive'] = _.findIndex(activeUsers, x => { return x == dataItem.sapCode }) > -1;
                        let validUser = _.find($scope.allUsers, x => { return x.sapCode == dataItem.sapCode });
                        //dataItem['isDisable'] = false;                        
                        var input_html = '';
                        const targetField = `targetField${moment(temp).format('YYYYMMDD')}`;
                        let disabledField = false;
                        if (validUser) {
                            if (validUser.startDate && validUser.officialResignationDate) {
                                disabledField = (dateFns.isAfter(new Date(dateFns.format(validUser.startDate, 'MM/DD/YYYY')), new Date(dateFns.format(temp, 'MM/DD/YYYY')))) || (!dateFns.isAfter(new Date(dateFns.format(validUser.officialResignationDate, 'MM/DD/YYYY')), new Date(dateFns.format(temp, 'MM/DD/YYYY'))));
                            } else if (validUser.startDate) {
                                disabledField = (dateFns.isAfter(new Date(dateFns.format(validUser.startDate, 'MM/DD/YYYY')), new Date(dateFns.format(temp, 'MM/DD/YYYY'))));
                            }
                        }
                        disabledField = disabledField || ($scope.visibleSubmitData && !dataItem.isSubmitted ? false : (dataItem.isSent || !dataItem.isActive || dataItem.isDisable));
                        input_html = `<input ng-change="onChange(dataItem)" style="border: 1px solid #c9c9c9; text-align: center"  class="k-input w100" ng-model="dataItem.${targetField}" ng-readonly="${disabledField}"/>`;
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
    function isHoliday(date) {
        var index = _.findIndex(holidayDates, x => {
            return moment(x).format('DDMMYYYY') == moment(date).format('DDMMYYYY');
        });
        return index > -1;
    }
    $scope.checkStatus = {
        value: "all"
    }
    $scope.onCheckStatus = async function () {
        if ($scope.currentDivisonIds) {
            setPagingDefault(1, $scope.page.limit);
            await getSapCodeByDivision($scope.currentDivisonIds, null, true);
        }
    }
    function getDayFromDate(date) {
        switch (date.getDay()) {
            case 0:
                return 'SHIFT_PLAN_SUN';
            case 1:
                return 'SHIFT_PLAN_MON';
            case 2:
                return 'SHIFT_PLAN_TUE';
            case 3:
                return 'SHIFT_PLAN_WED';
            case 4:
                return 'SHIFT_PLAN_THU';
            case 5:
                return 'SHIFT_PLAN_FRI';
            case 6:
                return 'SHIFT_PLAN_SAT';
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
    async function getDepartmentByUserId(userId, deptId, divisionId) {
        kendo.ui.progress($("#loading"), true);
        $scope.deptLineList = [];
        let res = await settingService.getInstance().shiftPlanSubmitPerson.getDepartmentTargetPlansByUserId({ id: userId }, null).$promise;
        if (res && res.isSuccess) {
            GetDivisionsExitsSubmit(res.object.data);
            $scope.deptLineList = res.object.data;
            var deptLines = _.map($scope.deptLineList, x => { return x['deptLine'] });
            var deptLineList = $("#dept_line_id").data("kendoDropDownList");
            if (deptLineList)
                deptLineList.setDataSource(deptLines);
            if (deptId) {
                if (deptLineList) {
                    deptLineList.value(deptId);
                    $scope.model.deptId = deptId;
                }
                // Lấy Division cua
                // let divisionIds = await getDivisionsByDeptLine(deptId, divisionId);
                // if (divisionIds && divisionIds.length) {
                //     debugger;
                //     await getSapCodeByDivision(divisionIds, null, false);
                // }
                await getDivisionsByDeptLine(deptId, divisionId);
            }
            kendo.ui.progress($("#loading"), false);
        }
    }

    async function getDivisionsByDeptLine(deptId) {
        $scope.divisionList = [];
        let divisionIds = [];
        var currentItem = _.find($scope.deptLineList, x => { return x.deptLine.id == deptId });
        if (currentItem && currentItem.deptLine.submitSAPCodes.includes($rootScope.currentUser.sapCode) && $scope.isShowAllDivision) {
            divisionIds = [currentItem.deptLine.id];
            // return divisionIds;
        } else {
            divisionIds = _.map(currentItem.divisions, "id");
            // return divisionIds;
        }
        var divisionTree = $("#division_id").data("kendoDropDownTree");
        if (divisionTree) {
            if (currentItem && currentItem.divisions && currentItem.divisions.length && !$scope.isShowAllDivision) {
                await getDivisionTreeByIds(divisionIds);
                if (divisionIds && divisionIds.length == 1) {
                    divisionTree.value(divisionIds[0]);
                    $scope.model.divisionId = divisionIds[0];
                }
            } else {
                await getDivisionTreeByIds(divisionIds);
            }

        }

    }
    divisonNotUserSubmit = []
    async function getDivisionTreeByIds(ids) {
        const currentUser = $rootScope.currentUser;
        divisonNotUserSubmit = [];
        let arg = {
            predicate: "",
            predicateParameters: [],
            page: 1,
            limit: appSetting.pageSizeDefault,
            order: ""
        }
        for (let i = 0; i < ids.length; i++) {
            arg.predicate = arg.predicate ? arg.predicate + `||id = @${i}` : `id = @${i}`;
            arg.predicateParameters.push(ids[i]);
        };
        result = await settingService.getInstance().departments.getDepartmentByArg(arg).$promise;
        if (result.isSuccess) {
            if ((currentUser.isStore && currentUser.jobGradeValue <= 4) || (!currentUser.isStore && currentUser.jobGradeValue <= 5)) {
                $scope.dataTemporaryArrayDivision = _.filter(result.object.data, x => { return x.items.length || x.jobGradeGrade <= 4 });
                // CHECK USER SUBMIT
                var currentItem = _.find($scope.deptLineList, x => { return x.deptLine.id == $scope.model.deptId });
                setDropDownTree('division_id', $scope.dataTemporaryArrayDivision);
                // Nếu nhiều hơn 1 Division, thì chỉ show user theo division submit          
                if (currentItem.divisions.length == 1 && !$scope.isShowAllDivision) {
                    var divisionTree = $("#division_id").data("kendoDropDownTree");
                    if (divisionTree) {
                        let divisionId = currentItem.divisions[0].id;
                        $scope.currentDivisionId = divisionId;
                        $scope.model.divisionId = divisionId;
                        divisionTree.value(divisionId);
                    }
                } else {
                    if (currentItem && currentItem.deptLine.submitSAPCodes.includes($rootScope.currentUser.sapCode)) {
                        ids = [currentItem.deptLine.id];
                    } else {
                        let divisionSubmit = getCurrentDivisionByCurrentUserSubmit(currentItem);
                        ids = _.map(divisionSubmit, "id");
                    }
                    $scope.model.divisionId = '';
                    $scope.model.divisionName = '';
                    $scope.isShowAllDivision = true;
                }
            }

            await getSapCodeByDivision(ids, null);

        } else {
            Notification.error(result.messages[0]);
        }
    }
    async function checkDepartmentIsSubmit(currentItem) // Department or Division
    {
        let ids = null;

        //Division này đã có người submit
        let divsionIsSubmit = _.map(currentItem.divisions, "submitSAPCodes").filter(x => x.length > 0);

        if (divsionIsSubmit && divsionIsSubmit.length > 0) {
            ids = _.map(currentItem.divisions, "id");
        } else {
            ids = [currentItem.deptLine.id];
        }

        let model = genarateDepartmentTargetPlan(ids);
        if (model) {
            //$scope.disableDivison = await filterDivisionByUserNotSubmit(model);
        }
    }
    function genarateDepartmentTargetPlan(dataList) {
        let model = {
            arg: {
                predicate: "",
                predicateParameters: []
            },
            currentUserSapCode: $rootScope.currentUser.sapCode
        }
        for (let i = 0; i < dataList.length; i++) {
            model.arg.predicate = model.arg.predicate ? model.arg.predicate + `||id = @${i}` : `id = @${i}`;
            model.arg.predicateParameters.push(dataList[i]);
        }
        return model;

    }
    function GetDivisionsExitsSubmit(data) {
        let divisions = [];
        _.forEach(data, function (item) {
            if (item.divisions.length > 0) {
                _.forEach(item.divisions, function (i) {
                    divisions.push(i);
                });
            }
        });
        divisions = _.filter(divisions, function (item) { return item.submitSAPCodes.includes($rootScope.currentUser.sapCode) });
        $scope.divisionsSubmit = divisions;
        return divisions;
    }
    function generateQueryGetUser(divisionIds, option, status) {
        if (option && option.page) {
            $scope.page.current = option.page;
        }
        let model = {
            periodId: $scope.model.periodId,
            //ids: divisionIds,
            items: [],
            activeUsers: activeUsers,
            query: {
                predicate: "",
                predicateParameters: [],
                page: $scope.page.current,
                limit: $scope.page.limit / 2,
                order: "",
            },
        }
        if (!divisionIds.length) {
            let dept = _.find($scope.deptLineList, function (o) { return o.deptLine.id == $scope.model.deptId });
            if (dept.deptLine) {
                model.items.push({ id: $scope.model.deptId, isIncludeChildren: dept.deptLine.isIncludeChildren });
            }
        } else {
            _.forEach(divisionIds, function (id) {
                let isIncludeChildren = null;
                let dept = _.find($scope.deptLineList, function (o) { return o.deptLine.id == id });
                if (dept && _.isBoolean(dept.deptLine.isIncludeChildren)) {
                    isIncludeChildren = dept.deptLine.isIncludeChildren;
                } else if ($scope.divisionsSubmit && $scope.divisionsSubmit.length > 0) {
                    var currentDivision = _.find($scope.divisionsSubmit, function (o) { return o.id == id });
                    isIncludeChildren = currentDivision ? currentDivision.isIncludeChildren : null;
                }

                model.items.push({ id: id, isIncludeChildren: isIncludeChildren });
            });
        }
        if (status && status != "all") {
            model.query.predicate = "isSent = @0";
            if (status == "isHave") {
                model.query.predicateParameters = ["true"];
            } else if (status == "isNotHave") {
                model.query.predicateParameters = ["false"];
            }
        }
        model.type = 2; // Add Target plan , mapping backend
        return model;
    }
    //getValidUserSAPForTargetPlan
    $scope.allValidUsers = [];
    async function getSapCodeByDivision(divisionIds, option, allowToGetData = false) {
        if (divisionIds) {
            $scope.currentDivisonIds = divisionIds;
            let arg = generateQueryGetUser(divisionIds, option, $scope.checkStatus.value);
            let result = await settingService.getInstance().users.getUsersForTargetPlanByDeptId(arg).$promise;
            let allValidUserRes = await settingService.getInstance().users.getValidUsersForSubmitTargetPlan({ periodId: arg.periodId, items: arg.items, activeUsers: arg.activeUsers, query: { predicate: "", predicateParameters: [], page: 1, limit: 1000, order: "" } }).$promise;
            if (allValidUserRes && allValidUserRes.object) {
                $scope.allValidUsers = allValidUserRes.object.data;
                //console.log($scope.allValidUsers);
            }
            if (result.isSuccess) {
                $scope.total = result.object.count;
                $scope.allUsers = result.object.data;
                checkButtonCancel();
                await getValidUserSAPForTargetPlan();


                console.log($scope.allUsers);
            }
            await checkShowAction();
            // if (allowToGetData) {

            // }
            currentData = await getDetailData();
            detailTargetPlan = currentData;
            renderData(currentData);
            if (currentData.length) {

            } else {
                let periodData = initPeriod($scope.model);
                let currentData = initData(periodData);
                renderData(currentData);
            }
        }

    }
    async function filterDivisionByUserNotSubmit(model) {
        if (model) {
            let result = await settingService.getInstance().shiftPlanSubmitPerson.getFilterDivisionByUserNotSubmit(model).$promise;
            if (result.isSuccess && result.object.data)
                return result.object.data;
            return [];
        }
    }
    $scope.dialog = {};
    $scope.warningDialogOpts = {
        width: "600px",
        buttonLayout: "normal",
        closable: true,
        modal: true,
        visible: false,
        content: "",
        actions: [{
            text: $translate.instant('COMMON_BUTTON_OK'),
            action: function (e) {
                $scope.warningDialog.close();
            },
            primary: true
        }],
        close: function (e) {
            $scope.warningDialog.open();
        }
    };
    $scope.onClickSelectFile = function () {
        $(".attachmentDetail").kendoUpload({
            multiple: false,
            validation: {
                allowedExtensions: ['XLS', 'XLSX']
            }
        }).getKendoUpload();
    }
    $scope.onSelect = function (e) {
        let message = $.map(e.files, function (file) {
            $scope.attachmentDetails.push(file);
            return file.name;
        }).join(", ");
    };

    $scope.sendData = async function () {
        let attachmentFilesJson = null;
        if ($scope.attachmentDetails.length || $scope.removeFileDetails.length) {
            attachmentFilesJson = await mergeAttachment();
        }
        if (!attachmentFilesJson) {
            Notification.error($translate.instant('TARGET_PLAN_FILE_FORMAT_NOT_OK'));
        }
    }
    $scope.importDialogOpts = {
        width: "600px",
        buttonLayout: "normal",
        closable: true,
        modal: true,
        visible: false,
        content: "",
        multiple: false
    };
    $scope.closeImportDialog = function () {
        $scope.attachmentDetails = [];
        var upload = $("#attachmentDetail").data("kendoUpload");
        upload.removeAllFiles();
        setTimeout(function () {
            if ($scope.dialogVisible) {
                let dialog = $("#dialog-import").data("kendoDialog");
                dialog.close();
                $scope.dialogVisible = false;
            }
        }, 0);
        return false;
    }
    async function getPeriods() {
        var res = await cbService.getInstance().targetPlan.getTargetPlanPeriods().$promise;
        if (res.isSuccess) {
            targetPlanData = res.object.data;
            let currentPeriod = _.orderBy(targetPlanData, ['fromDate'], ['desc'])[0];
            if (currentPeriod) {
                $scope.model.periodId = currentPeriod.id;
                $scope.model.periodName = currentPeriod.name;
                $scope.model.periodFromDate = currentPeriod.fromDate;
                $scope.model.periodToDate = currentPeriod.toDate;
            }
            setDataSourceForDropdownList('#period_source_id', targetPlanData);
        }
    }
    async function getDetailData() {
        // Get target plan bởi departmentId, và Period
        var currentUsers = [];
        var viewUsers = _.map($scope.allUsers, 'sapCode');
        let res = await cbService.getInstance().targetPlan.getPendingTargetPlanFromDepartmentAndPeriod({ departmentId: $scope.model.deptId, divisionId: $scope.model.divisionId, periodId: $scope.model.periodId, sapCodes: viewUsers }).$promise;
        if (res && res.isSuccess) {
            if (!$scope.model.divisionId) {
                res.object.divisionId = '';
                res.object.divisionName = '';
                res.object.divisionCode = '';
            }
            $scope.model = res.object;

        }
        // get target plan detail 

        let result = await cbService.getInstance().targetPlan.getPendingTargetPlanDetailFromSAPCodesAndPeriod({ deptId: $scope.model.deptId, divisionId: $scope.model.divisionId, periodId: $scope.model.periodId, sapCodes: viewUsers, allSAPCodes: $scope.allValidUsers, visibleSubmit: $scope.visibleSubmitData }).$promise;
        if (result.isSuccess && result.object && result.object.count > 0) {
            targetPlanDetailItems = result.object.data;
            renderTargetForAllUser(result.object.allData);
            let validUsers = _.filter(targetPlanDetailItems, x => {
                return _.findIndex($scope.allUsers, y => {
                    if ($scope.visibleSubmitData) {
                        if (_.findIndex(activeUsers, t => { return t == y.sapCode }) > -1) {

                            return y.sapCode == x.sapCode
                        } else {
                            return x.isSent
                        }
                    } else if (!$scope.visibleSubmitData && !$scope.visibleSendData) {
                        if (_.findIndex(activeUsers, t => { return t != y.sapCode }) > -1) {
                            return x.isSent;
                        } else {
                            return y.sapCode == x.sapCode;
                        }
                    } else {
                        return y.sapCode == x.sapCode
                    }
                }) > -1;
            });
            //let validUsers = targetPlanDetailItems;
            if (validUsers && validUsers.length) {
                let currentUsers = [];
                _.forEach(validUsers, x => {
                    var currentObject = { 'sapCode': x.sapCode, 'fullName': x.fullName, 'departmentName': x.departmentName, 'departmentCode': x.departmentCode, 'type': `${commonData.targetPlanType[`target${x.type}`]}`, erdQuality: x.erdQuality ? x.erdQuality : 0, prdQuality: x.prdQuality ? x.prdQuality : 0, alhQuality: x.alhQuality ? x.alhQuality : 0, doflQuality: x.doflQuality ? x.doflQuality : 0, isSent: x.isSent, isSubmitted: x.isSubmitted };
                    let list = JSON.parse(x.jsonData);
                    _.forEach(list, y => {
                        currentObject[`targetField${y.date}`] = y.value;
                    })
                    currentUsers.push(currentObject);
                });

                currentUsers = updateTargetPlanDetails(currentUsers);
                return currentUsers;
            }
        } else {
            targetPlanDetailItems = [];
        }
        return currentUsers;

    }
    function renderTargetForAllUser(data) {
        let AllUsers = [];
        _.forEach(data, x => {
            var currentObject = { 'sapCode': x.sapCode, 'fullName': x.fullName, 'departmentName': x.departmentName, 'departmentCode': x.departmentCode, 'type': `${commonData.targetPlanType[`target${x.type}`]}`, erdQuality: x.erdQuality ? x.erdQuality : 0, prdQuality: x.prdQuality ? x.prdQuality : 0, alhQuality: x.alhQuality ? x.alhQuality : 0, doflQuality: x.doflQuality ? x.doflQuality : 0, isSent: x.isSent, isSubmitted: x.isSubmitted };
            let list = JSON.parse(x.jsonData);
            _.forEach(list, y => {
                currentObject[`targetField${y.date}`] = y.value;
            })
            AllUsers.push(currentObject);
        });
        $scope.allUserData = AllUsers;
        return AllUsers;
    }
    function updateTargetPlanDetails(listUsers) {
        let currentUsers = [];
        var columnInPeriods = getColumnFromPeriod({
            id: $scope.model.periodId,
            name: $scope.model.periodName,
            fromDate: $scope.model.periodFromDate,
            toDate: $scope.model.periodToDate
        });
        if (columnInPeriods.length) {
            let tempColumns = columnsDaysInPeriod.concat(columnInPeriods);
            refreshGridColumns(tempColumns);
        };
        _.forEach($scope.allUsers, x => {
            let createdTargetItems = _.filter(listUsers, y => { return y.sapCode == x.sapCode });
            if (!createdTargetItems.length) {
                for (let i = 1; i <= 2; i++) {
                    var currentObject = { 'sapCode': x.sapCode, 'fullName': x.fullName, 'departmentName': x.department, 'departmentCode': x.departmentCode, 'type': `${commonData.targetPlanType[`target${i}`]}`, erdQuality: x.erdQuality ? x.erdQuality : 0, prdQuality: x.prdQuality ? x.prdQuality : 0, alhQuality: x.alhQuality ? x.alhQuality : 0, doflQuality: x.doflQuality ? x.doflQuality : 0 };
                    for (let i = 1; i < columnInPeriods.length; i += 2) {
                        columnInPeriods[i].isUserLastRecord = true;
                    }
                    _.forEach(columnInPeriods, y => {
                        currentObject[`${y.field}`] = '';
                    });
                    currentUsers.push(currentObject);
                }
            } else {
                let orderItems = _.sortBy(createdTargetItems, ['sapCode', 'type']);
                for (let i = 1; i < orderItems.length; i += 2) {
                    orderItems[i].isUserLastRecord = true;
                }
                _.forEach(orderItems, t => {
                    currentUsers.push(t);
                });
            }
        });
        return currentUsers;
    }
    function initData(periodData) {
        let currentUsers = [];
        $scope.model.periodId = periodData.id;
        $scope.model.periodName = periodData.periodName ? periodData.periodName : periodData.name;
        $scope.model.periodFromDate = periodData.fromDate;
        $scope.model.periodToDate = periodData.toDate;
        var columnInPeriods = getColumnFromPeriod(periodData);
        if (columnInPeriods.length) {
            let tempColumns = columnsDaysInPeriod.concat(columnInPeriods);
            refreshGridColumns(tempColumns);
        };
        _.forEach($scope.allUsers, x => {
            for (let i = 1; i <= 2; i++) {
                var currentObject = { 'sapCode': x.sapCode, 'fullName': x.fullName, 'departmentName': x.department, 'departmentCode': x.departmentCode, 'type': `${commonData.targetPlanType[`target${i}`]}`, erdQuality: x.erdQuality ? x.erdQuality : 0, prdQuality: x.prdQuality ? x.prdQuality : 0, alhQuality: x.alhQuality ? x.alhQuality : 0, doflQuality: x.doflQuality ? x.doflQuality : 0 };
                _.forEach(columnInPeriods, y => {
                    currentObject[`${y.field}`] = '';
                });
                currentUsers.push(currentObject);
            }
        });
        return currentUsers;
    }
    function renderData(currentData) {
        $timeout(function () {
            if (currentData.length) {
                let sortItems = _.filter(currentData, x => { return activeUsers.includes(x.sapCode) || x.sapCode == $rootScope.currentUser.sapCode });
                let unSortItems = _.filter(currentData, x => { return !activeUsers.includes(x.sapCode) && x.sapCode != $rootScope.currentUser.sapCode });
                let sortedUsers = sortItems.concat(unSortItems);
                for (let i = 1; i < sortedUsers.length; i += 2) {
                    sortedUsers[i].isUserLastRecord = true;
                }
                setDataForUser(sortedUsers)
                //setDataSourceForGrid('#grid_user_id', sortedUsers);
            } else {
                setDataSourceForGrid('#grid_user_id', []);
            }
        }, 0);
    }
    $scope.onChangeDivision = async function (divisionId) {
        console.log(divisionId);
        if (divisionId) {
            await getSapCodeByDivision([divisionId], null, true);
        } else {
            $timeout(function () {
                $scope.dialog = $rootScope.showConfirmDelete($translate.instant('COMMON_NOTIFY'), $translate.instant('TARGET_PLAN_NOTIFY'), $translate.instant('COMMON_BUTTON_CONFIRM'));
                $scope.dialog.bind("close", confirm);
            }, 0);
        }
    }
    confirm = async function (e) {
        if (e.data && e.data.value) {
            $scope.isShowAllDivision = true;
            let dropdownlist = $("#dept_line_id").data("kendoDropDownList");
            let dataItem = dropdownlist.dataItem(e.node)
            await loadUserByDeptLine(dataItem);
        } else {
            let dropdownlist = $("#division_id").data("kendoDropDownTree");
            if (dropdownlist) {
                if ($scope.currentDivisionId) {
                    dropdownlist.value($scope.currentDivisionId);
                } else {
                    dropdownlist.value($rootScope.currentUser.divisionId);
                }
            }
        }
    }
    function setDataForUser(data) {
        let grid = $('#grid_user_id').data("kendoGrid");
        let dataSource = new kendo.data.DataSource({
            data: data,
            pageSize: $scope.page.limit,
            serverPaging: true,
            page: $scope.page.current,
            schema: {
                total: function () {
                    return $scope.total * 2;
                },
            },
        });
        if (grid) {
            grid.setDataSource(dataSource);

        }
    }
    function initPeriod(model) {
        let periodData = {
            id: model.periodId,
            periodName: model.periodName,
            fromDate: model.periodFromDate,
            toDate: model.periodToDate,
        }
        return periodData;
    }
    $scope.save = async function (isSend = false) {
        $scope.errors = [];
        let data = null;
        // if (isSend) {
        //     data = $scope.allUserData;
        // }
        // else {
        //     if ($scope.visibleSubmitData) {
        //         data = $('#grid_user_id').data('kendoGrid').dataSource._data;
        //     } else {
        //         data = _.filter($('#grid_user_id').data('kendoGrid').dataSource._data, x => { return activeUsers.includes(x.sapCode) });
        //     }
        // }
        if ($scope.visibleSubmitData) {
            data = $('#grid_user_id').data('kendoGrid').dataSource._data;
        } else {
            data = _.filter($('#grid_user_id').data('kendoGrid').dataSource._data, x => { return activeUsers.includes(x.sapCode) });
        }
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
                let res = await cbService.getInstance().targetPlan.savePendingTargetPlan({ isSend: isSend }, saveModel).$promise;
                if (res.isSuccess) {
                    Notification.success($translate.instant('COMMON_SAVE_SUCCESS'));
                    $state.go($state.current.name, {}, { reload: isSend });
                } else {
                    $timeout(function () {
                        if (res.errorCodes && res.errorCodes.length) {
                            $scope.errors = [];
                            for (let i = 0; i < res.errorCodes.length; i++) {
                                if (res.errorCodes[i] == 3) { // Code 3: Validate cho TargetPlan Holidays 
                                    $scope.errors.push({ controlName: res.messages[i], message: $translate.instant('TARGET_PLAN_VALIDATE_HOLIDAYS') });
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
                                }
                                else if (res.errorCodes[i] == 10) { // Startdate is null - jira 593
                                    $scope.errors.push({ controlName: res.messages[i], message: $translate.instant('TARGET_PLAN_START_DATE_NULL') });
                                }
                                else if (res.errorCodes[i] == 11) { // jira 253
                                    $scope.errors.push({ controlName: res.messages[i], message: $translate.instant('TARGET_PLAN_SHIFT_CODE_HQ') });
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

    // ADD
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
        $scope.errors = [];
        var payload = new FormData();
        $scope.attachmentDetails.forEach(item => {
            payload.append('file', item.rawFile, item.name);
        });
        if (!$scope.allUsers.length) {
            Notification.error('You made Target Plan');
        } else {
            let argData = {
                deptId: '',
                divisionId: null,
                periodId: '',
                visibleSubmit: $scope.visibleSubmitData,
                sapCodes: []
            };
            argData.deptId = $scope.model.deptId;
            argData.periodId = $scope.model.periodId;
            if ($scope.checkStatus.value == 'isNotHave') {
                argData.sapCodes = _.map($scope.allUsers, 'sapCode').join(',');
            }
            else {
                if ($scope.visibleSubmitData) {
                    argData.sapCodes = $scope.allValidUsers.join(',');
                } else {
                    argData.sapCodes = activeUsers.join(',');
                }
            }
            argData.divisionId = $scope.model.divisionId;
            var header = buildCustomBlobHeader();
            kendo.ui.progress($("#loading"), true);
            let res = await $http({
                method: 'POST',
                url: baseUrlApi + "/TargetPlan/PostFileWithData",
                headers: header,
                transformRequest: function (data) {
                    var formData = new FormData();
                    formData.append("argData", angular.toJson(data.model));
                    data.files.forEach(item => {
                        formData.append('payload', item.rawFile, item.name);
                    });
                    return formData;
                },
                data: { model: argData, files: $scope.attachmentDetails }
            }).then(res => {
                let result = res.data;
                if (result.isSuccess && result.object && result.object.data.length) {
                    Notification.success('Upload Data Successfully');
                    if (result.object && result.object.data) {
                        $scope.model.id = result.object.data[0].pendingTargetPlanId;
                    }
                } else {
                    if (result.errorCodes.length) {

                        for (let i = 0; i < result.errorCodes.length; i++) {
                            if (result.errorCodes[i] == 1) {
                                Notification.error($translate.instant('INVALID_SHIFT_CODE_TARGET_1') + `: ${result.messages[i]}`);
                            } else if (result.errorCodes[i] == 2) {
                                Notification.error($translate.instant('INVALID_SHIFT_CODE_TARGET_2') + `: ${result.messages[i]}`);
                            } else if (result.errorCodes[i] == 3) { // Code 3: Validate cho TargetPlan Holidays 
                                $scope.errors.push({ controlName: result.messages[i], message: $translate.instant('TARGET_PLAN_VALIDATE_HOLIDAYS') });
                            }
                            else if (result.errorCodes[i] == 3) { // Check Holidays
                                $scope.errors.push({ controlName: `${result.messages[i]}`, message: `${$translate.instant('TARGET_PLAN_VALIDATE_HOLIDAYS')}` })
                            }
                            else if (result.errorCodes[i] == 4) { // Thiếu target 1 or target 2
                                var messArray = result.messages[i].split("|");
                                //var message = `${$translate.instant('COMMON_SAP_CODE')}(${messArray[0]}) : ${messArray[1] + ' ' + $translate.instant('COMMON_FIELD_IS_REQUIRED')}`;
                                $scope.errors.push({ controlName: `${messArray[0]} :`, message: `${messArray[1] + ' ' + $translate.instant('COMMON_FIELD_IS_REQUIRED')}` });
                            }
                            else if (result.errorCodes[i] == 5) { //
                                var messArray = result.messages[i].split("|");
                                $scope.errors.push({ controlName: `${messArray[0]}`, message: `${$translate.instant(messArray[1] == 'Target1' ? 'SHIFT_PLAN_TARGET_1' : 'SHIFT_PLAN_TARGET_2') + ' ' + $translate.instant('COMMON_IS_NOT_INVALID')}` })
                            }
                            else if (result.errorCodes[i] == 6) { //
                                var messArray = result.messages[i].split("|");
                                $scope.errors.push({ controlName: `${messArray[0]}`, message: `${$translate.instant(messArray[1] == 'Target1' ? 'SHIFT_PLAN_TARGET_1' : 'SHIFT_PLAN_TARGET_2') + ' ' + $translate.instant('TARGET_PLAN_DUPLICATE_TARGET')}` })
                            }
                            else if (result.errorCodes[i] == 7) { //
                                $scope.errors.push({ controlName: `${result.messages[i]}`, message: `${$translate.instant('TARGET_PLAN_VALIDATE_HALFDAY')}` })
                            }
                            else if (result.errorCodes[i] == 8) { //
                                _.forEach(result.object.data, item => {
                                    // $scope.errors.push({ controlName: `${x.sapCode}`, message: `${$translate.instant('TARGET_PLAN_VALIDATE_STORE_G2')}` })

                                    if (item.typeName == commonData.validateTargetPlan.ERD) {
                                        let text = item.sapCode + ' ' + $translate.instant('TARGET_PLAN_VALIDATE_ERD')
                                        let textError = {
                                            controlName: $translate.instant('COMMON_ENTER_FIELD'),
                                            message: text
                                        }
                                        $scope.errors.push(textError);
                                    }
                                    else if (item.typeName == commonData.validateTargetPlan.PRD) {
                                        let text = item.sapCode + ' ' + $translate.instant('TARGET_PLAN_VALIDATE_PRD')
                                        let textError = {
                                            controlName: $translate.instant('COMMON_ENTER_FIELD'),
                                            message: text
                                        }
                                        $scope.errors.push(textError);
                                    }
                                    else if (item.typeName == commonData.validateTargetPlan.G2) {
                                        let text = item.sapCode + ' ' + $translate.instant('TARGET_PLAN_VALIDATE_STORE_G2')
                                        let textError = {
                                            controlName: $translate.instant('COMMON_ENTER_FIELD'),
                                            message: text
                                        }
                                        $scope.errors.push(textError);
                                    }
                                    else if (item.typeName == commonData.validateTargetPlan.AL) {
                                        let text = item.sapCode + ' ' + $translate.instant('TARGET_PLAN_VALIDATE_AL')
                                        let textError = {
                                            controlName: $translate.instant('COMMON_ENTER_FIELD'),
                                            message: text
                                        }
                                        $scope.errors.push(textError);
                                    }
                                    else if (item.typeName == commonData.validateTargetPlan.DOFL) {
                                        let text = item.sapCode + ' ' + $translate.instant('TARGET_PLAN_VALIDATE_DOFL')
                                        let textError = {
                                            controlName: $translate.instant('COMMON_ENTER_FIELD'),
                                            message: text
                                        }
                                        $scope.errors.push(textError);
                                    }
                                })
                            }
                            else if (result.errorCodes[i] == 9) { //
                                $scope.errors.push({ controlName: `${result.messages[i]}`, message: `${$translate.instant('TARGET_PLAN_NO_PRD')}` })
                            }
                            else {
                                Notification.error($translate.instant(item));
                            }
                        }
                    } else {
                        Notification.error($translate.instant('TARGET_PLAN_NOT_FOUND_DATA_IMPORT'))
                    }
                    $scope.$applyAsync();
                }
                return result;
            });
            kendo.ui.progress($("#loading"), false);
            return res;
            // let result = await cbService.getInstance().targetPlan.import(argData, payload).$promise;           
        }

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
                allattachmentDetails = allattachmentDetails.concat(['Import Target Plan.xlsx']);
                // let currentData = loadDataAfterImport(uploadResult);
                // if (currentData.length) {
                //     renderData(currentData);
                // }
                await $scope.onCheckStatus();
                $scope.closeImportDialog();
            }
            attachmentFilesJson = JSON.stringify(allattachmentDetails);
            return attachmentFilesJson;
        } catch (e) {
            console.log(e);
            return '';
        }
    }
    function updateDataAfterImport(importData) {
        let result = [];
        console.log(importData);
        console.log(detailTargetPlan);
        return result;
    }
    function loadDataAfterImport(result) {
        let currentUsers = [];
        let data = [];
        if (result.isSuccess && result.object && result.object.count > 0) {
            targetPlanDetailItems = result.object.data;
            data = result.object.data;
            let newData = [];
            if (targetPlanDetailItems && targetPlanDetailItems.length) {
                data.forEach(item => {
                    let updateItem = _.find(targetPlanDetailItems, x => { return x.sapCode == item.sapCode && x.type == item.type });
                    if (updateItem) {
                        updateItem.alhQuality = item.alhQuality;
                        updateItem.prdQuality = item.prdQuality;
                        updateItem.doflQuality = item.doflQuality;
                        updateItem.erdQuality = item.erdQuality;
                        updateItem.jsonData = item.jsonData;
                    } else {
                        newData.push(item);
                    }
                });
                targetPlanDetailItems = targetPlanDetailItems.concat(newData);
            } else {
                targetPlanDetailItems = data;
            }
            validUsers = targetPlanDetailItems;
            if (validUsers && validUsers.length) {
                let currentUsers = [];
                _.forEach(validUsers, x => {
                    var currentObject = { 'sapCode': x.sapCode, 'fullName': x.fullName, 'departmentName': x.departmentName, 'departmentCode': x.departmentCode, 'type': `${commonData.targetPlanType[`target${x.type}`]}`, erdQuality: x.erdQuality ? x.erdQuality : 0, prdQuality: x.prdQuality ? x.prdQuality : 0, alhQuality: x.alhQuality ? x.alhQuality : 0, doflQuality: x.doflQuality ? x.doflQuality : 0, isSent: x.isSent, isSubmitted: x.isSubmitted };
                    let list = JSON.parse(x.jsonData);
                    _.forEach(list, y => {
                        currentObject[`targetField${y.date}`] = y.value;
                    })
                    currentUsers.push(currentObject);
                });
                currentUsers = updateTargetPlanDetails(currentUsers);
                return currentUsers;
            }
        }
        return currentUsers;
    }
    //
    // CHANGING
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
    //
    $scope.import = function () {
        $scope.importDialog.title('IMPORT')
        $scope.importDialog.open();
        //$rootScope.confirmVoteDialog = $scope.importDialog;
    }
    $scope.submit = async function () {
        $scope.errors = [];
        let currentData = [];
        currentData = $('#grid_user_id').data('kendoGrid').dataSource._data;
        validateTargetPlan(currentData);
        $scope.$applyAsync();
        if (!$scope.errors.length) {
            $scope.model.jsonData = JSON.stringify(currentData);
            $scope.model.createdById = $rootScope.currentUser.id;
            let saveModel = convertJsonData(true);
            $scope.errors = await validateTargetPlanDetail(saveModel);
            $scope.$applyAsync();
            console.log(saveModel);
            if (!$scope.errors.length) {
                let res = await cbService.getInstance().targetPlan.savePendingTargetPlan({ isSend: true }, saveModel).$promise;
                if (!res.isSuccess && res.errorCodes) {
                    $scope.errors = [];
                    for (let i = 0; i < res.errorCodes.length; i++) {
                        if (res.errorCodes[i] == 3) { // Code 3: Validate cho TargetPlan Holidays 
                            $scope.errors.push({ controlName: res.messages[i], message: $translate.instant('TARGET_PLAN_VALIDATE_HOLIDAYS') });
                        } else if (res.errorCodes[i] == 4) {

                            $scope.errors.push({ controlName: res.messages[i], message: $translate.instant('TARGET_PLAN_EMPLOYEE_NOT_FILL_SHIFT_CODE') });
                        }
                        else if (res.errorCodes[i] == 5) { //
                            var messArray = res.messages[i].split("|");
                            $scope.errors.push({ controlName: `${messArray[0]}`, message: `: ${$translate.instant(messArray[1] == 'Target1' ? 'SHIFT_PLAN_TARGET_1' : 'SHIFT_PLAN_TARGET_2') + ' ' + $translate.instant('COMMON_IS_NOT_INVALID')}` });
                        }
                        else if (res.errorCodes[i] == 7) { //
                            $scope.errors.push({ controlName: res.messages[i], message: $translate.instant('TARGET_PLAN_VALIDATE_HALFDAY') });
                        }
                        else if (res.errorCodes[i] == 9) { // Thiếu PRD
                            $scope.errors.push({ controlName: `${res.messages[i]}`, message: `${$translate.instant('TARGET_PLAN_NO_PRD')}` })
                        }
                        else if (res.errorCodes[i] == 10) { // Startdate is null - jira 593
                            $scope.errors.push({ controlName: res.messages[i], message: $translate.instant('TARGET_PLAN_START_DATE_NULL') });
                        }
                        else if (res.errorCodes[i] == 11) { // jira 253
                            $scope.errors.push({ controlName: res.messages[i], message: $translate.instant('TARGET_PLAN_SHIFT_CODE_HQ') });
                        }
                        else {
                            $scope.errors.push({ controlName: $translate.instant(`SHIFT_PLAN_TARGET_${res.errorCodes[i]}`), message: $translate.instant(res.messages[i]) });
                        }
                    }
                    $scope.$applyAsync();
                } else {
                    _.forEach(currentData, x => {
                        if (_.findIndex(activeUsers, y => { return x.sapCode == y }) > -1) {
                            x.isSent = true;
                        }
                    })
                }
            }
        }
        if (!$scope.errors.length) {
            let submitArg = {
                deptId: $scope.model.deptId,
                deptCode: $scope.model.deptCode,
                deptName: $scope.model.deptName,
                divisionId: $scope.model.divisionId,
                divisionCode: $scope.model.divisionCode,
                divisionName: $scope.model.divisionName,
                periodId: $scope.model.periodId,
                periodName: $scope.model.periodName,
                periodFromDate: $scope.model.periodFromDate,
                periodToDate: $scope.model.periodToDate,
                userSAPCode: $rootScope.currentUser.sapCode,
                userFullName: $rootScope.currentUser.fullName,
                pendingTargetPlanId: $scope.model.id,
                listSAPCode: $scope.allValidUsers.join(',')
            }
            let res = await cbService.getInstance().targetPlan.submit(submitArg).$promise;
            if (res && res.isSuccess && res.object) {
                let itemId = '';
                if (res.object.id) {
                    itemId = res.object.id;
                    let workflowStatusResult = await workflowService.getInstance().workflows.getWorkflowStatus({ itemId: itemId }).$promise;
                    if (workflowStatusResult && workflowStatusResult.object) {
                        let workflowId = workflowStatusResult.object.workflowButtons[0].id;
                        var result = await workflowService.getInstance().workflows.startWorkflow({ workflowId: workflowId, itemId: itemId }, null).$promise;
                        if (result && result.isSuccess) {
                            Notification.success($translate.instant('COMMON_WORKFLOW_STARTED'));
                            $scope.visibleSubmitData = false;
                            let arg = { periodId: $scope.model.periodId, listSAPCode: submitArg.listSAPCode };
                            await cbService.getInstance().targetPlan.setSubmittedStateForDetailPendingTargetPlan(arg).$promise;
                            // ADD PERMISSTION
                            await cbService.getInstance().targetPlan.updatePermissionUserInTargetPlanItem({ id: itemId }, null).$promise;

                            workflowStatusResult = await workflowService.getInstance().workflows.getWorkflowStatus({ itemId: itemId }).$promise;
                            dataService.workflowStatus = workflowStatusResult;
                        }
                    }
                }
                if (res.object.itemId) {
                    itemId = res.object.itemId;
                    var result = await workflowService.getInstance().workflows.startWorkflow({ workflowId: res.object.workflowId, itemId: res.object.itemId }, null).$promise;
                    if (result && result.isSuccess) {
                        $state.go('home.targetPlan.item', { id: itemId, referenceValue: res.object.referenceNumber }, { reload: true });
                    }
                }
                if (itemId) {

                    $state.go('home.targetPlan.item', { id: itemId, referenceValue: res.object.referenceNumber }, { reload: true });
                }
            }
            else {
                $timeout(function () {
                    if (res.errorCodes && res.errorCodes.length) {
                        $scope.errors = [];
                        for (let i = 0; i < res.errorCodes.length; i++) {
                            if (res.errorCodes[i] == 3) { // Code 3: Validate cho TargetPlan Holidays 
                                $scope.errors.push({ controlName: res.messages[i], message: $translate.instant('TARGET_PLAN_VALIDATE_HOLIDAYS') });
                            } else if (res.errorCodes[i] == 4) {
                                debugger;
                                $scope.errors.push({ controlName: res.messages[i], message: $translate.instant('TARGET_PLAN_EMPLOYEE_NOT_FILL_SHIFT_CODE') });
                            }
                            else if (res.errorCodes[i] == 5) { //
                                var messArray = res.messages[i].split("|");
                                $scope.errors.push({ controlName: `${messArray[0]}`, message: `: ${$translate.instant(messArray[1] == 'Target1' ? 'SHIFT_PLAN_TARGET_1' : 'SHIFT_PLAN_TARGET_2') + ' ' + $translate.instant('COMMON_IS_NOT_INVALID')}` });
                            }
                            else if (res.errorCodes[i] == 7) { //
                                $scope.errors.push({ controlName: res.messages[i], message: $translate.instant('TARGET_PLAN_VALIDATE_HALFDAY') });
                            }
                            else if (res.errorCodes[i] == 9) { // Thiếu PRD
                                $scope.errors.push({ controlName: `${res.messages[i]}`, message: `${$translate.instant('TARGET_PLAN_NO_PRD')}` })
                            }
                            else if (res.errorCodes[i] == 10) { // Startdate is null - jira 593
                                $scope.errors.push({ controlName: res.messages[i], message: $translate.instant('TARGET_PLAN_START_DATE_NULL') });
                            }
                            else if (res.errorCodes[i] == 11) { // jira 253
                                $scope.errors.push({ controlName: res.messages[i], message: $translate.instant('TARGET_PLAN_SHIFT_CODE_HQ') });
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
    function validateDataBeforeSubmit(data) {
        Object.keys(data).forEach(fieldName => {
            if (fieldName.includes('targetField') && !data[fieldName]) {
                return true;
            }
        })
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
                        //Check !x.isSent
                        //If user send target plan- no need to check this user any more
                        if (!haveError && !x.isSent && fieldName.includes('targetField') && (x[fieldName] == null || x[fieldName] == undefined || x[fieldName].length == 0) && _.findIndex(res, t => { return t == fieldName }) > -1) {
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
        let currentUsers = res.object.data;
        //console.log(currentUsers);
        let haveError = false;
        _.forEach(list, x => {
            let item = _.find(currentUsers, y => { return x.sapCode == y.sapCode });
            if ((item.jobGradeValue == 1 || item.jobGradeValue == 2) && item.isStore) {
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
        $scope.$applyAsync();
    }
    $scope.$on("targetPlan", async function (evt, data) {
        $scope.model = data;
        await getPeriods();
        await getValidUserSAPForTargetPlan();
        await getDepartmentByUserId($rootScope.currentUser.id, $rootScope.currentUser.deptId, $scope.currentUser.divisionId);
        await checkShowAction();
        let currentPeriod = initPeriod($scope.model);
        let currentData = initData(currentPeriod);
        currentData = await getDetailData();
        detailTargetPlan = currentData;
        if (currentData.length) {
            renderData(currentData);
        } else {
            let periodData = initPeriod($scope.model);
            let currentData = initData(periodData);
            renderData(currentData);
        }

    });
    async function getValidUserSAPForTargetPlan() {
        if ($rootScope.currentUser.divisionId) {
            departmentId = $rootScope.currentUser.divisionId;
        } else if ($rootScope.currentUser.deptId) {
            departmentId = $rootScope.currentUser.deptId;
        }
        let res = await cbService.getInstance().targetPlan.getValidUserSAPForTargetPlan({ id: departmentId, userId: $rootScope.currentUser.id, periodId: $scope.model.periodId }, null).$promise;
        if (res && res.isSuccess && res.object) {
            validUserMappings = res.object.data;
            activeUsers = _.map(res.object.data, 'sapCode');
        } else {
            $timeout(function () {
                activeUsers = [];
            });
        }
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
    $rootScope.$watch('currentUser', async function (newValue, oldValue) {
        if ($rootScope.currentUser && (!oldValue && newValue || oldValue && newValue) && !isGetData) {
            let initModel = {
                deptId: $rootScope.currentUser.deptId,
                deptCode: $rootScope.currentUser.deptCode,
                deptName: $rootScope.currentUser.deptName,
                divisionId: $rootScope.currentUser.divisionId,
                divisionCode: $rootScope.currentUser.divisionCode,
                divisionName: $rootScope.currentUser.divisionName
            }
            isGetData = true;
            $rootScope.$broadcast("targetPlan", initModel);
        }
    });
    async function checkShowAction() {
        let divisionIds = [];
        if ($scope.isShowAllDivision) {
            var currentItem = _.find($scope.deptLineList, x => { return x.deptLine.id == $scope.model.deptId });
            let divisionSubmit = getCurrentDivisionByCurrentUserSubmit(currentItem);
            if (!divisionSubmit.length) {
                divisionIds.push($scope.model.divisionId)
            } else {
                divisionIds = _.map(divisionSubmit, "id");
            }
        } else {
            divisionIds.push($scope.model.divisionId)
        }
        let res = await cbService.getInstance().targetPlan.getPermissionInfoOnPendingTargetPlan({ departmentId: $scope.model.deptId, divisionIds: divisionIds, periodId: $scope.model.periodId, sapCodes: _.map($scope.allUsers, 'sapCode'), activeUsers: activeUsers, validUsersSubmit: $scope.allValidUsers },).$promise;
        if (res && res.isSuccess && res.object) {
            $scope.visibleSendData = res.object.allowToSendData;
            $scope.visibleSubmitData = res.object.allowToSubmit;
        }
        $scope.$applyAsync();
    }
    async function validateTargetPlanDetail(model) {
        let errors = [];
        //validate ERD vs PRD
        let errorERDAndPRD = await validateERDUserSAP(model);
        errors = errors.concat(errorERDAndPRD);
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

    $scope.buttonCancel = false;
    $scope.cancelPropress = async function () {
        // if ($scope.model.id) {
        //     const currentUser = $rootScope.currentUser;
        //     let args = {
        //         pendingTargetId: $scope.model.id,
        //         deptId: currentUser.divisionId,
        //         periodId: $scope.model.periodId,
        //         sapCode: currentUser.sapCode
        //     }
        //     let res = await cbService.getInstance().targetPlan.cancelPendingTargetPLan(args).$promise;
        //     if (res && res.isSuccess) {
        //         Notification.error($translate.instant('TARGET_PLAN_CANCEL'));
        //         $scope.visibleSendData = true;
        //         $scope.$applyAsync();
        //         $state.go($state.current.name, {}, { reload: true });
        //     }
        // } else {
        //     Notification.error("Error System");
        // }
        const currentUser = $rootScope.currentUser;
        let args = {
            pendingTargetId: $scope.model.id,
            deptId: currentUser.divisionId,
            periodId: $scope.model.periodId,
            sapCode: currentUser.sapCode
        }
        let res = await cbService.getInstance().targetPlan.cancelPendingTargetPLan(args).$promise;
        if (res && res.isSuccess) {
            Notification.success($translate.instant('TARGET_PLAN_CANCEL'));
            $scope.visibleSendData = true;
            $scope.$applyAsync();
            $state.go($state.current.name, {}, { reload: true });
        }
    }

    function checkButtonCancel() {
        var result = $scope.allUsers.find(x => x.sapCode == $rootScope.currentUser.sapCode && x.isSent && !x.isSubmitted);
        if (result) {
            $scope.buttonCancel = true;
        }
        else {
            $scope.buttonCancel = false;
        }
        $scope.$applyAsync();
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

    $scope.downloadTemplate = async function () {
        //var viewUsers = _.map($scope.allUsers, 'sapCode');
        var periodFrom = moment($scope.model.periodFromDate).format("DD/MM/YYYY")

        let model = {
            departmentId: $scope.model.deptId,
            divisionId: $scope.model.divisionId,
            periodFromDate: periodFrom,
            listSAPCodes: $scope.allValidUsers
        }

        let res = await cbService.getInstance().targetPlan.downloadTemplate(model).$promise;
        if (res.isSuccess) {
            exportToExcelFile(res.object);
            Notification.success(appSetting.notificationExport.success);
        } else {
            Notification.error(appSetting.notificationExport.error);
        }
    }
});