
var ssgApp = angular.module("ssg.shiftPlanModule", [
    "kendo.directives"
]);

ssgApp.controller("shiftPlanController", function (
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
    dataService
) {
    $scope.titleHeader = 'SHIFT PLAN';
    $scope.title = $stateParams.action.title;
    $scope.title = $translate.instant('SHIFT_PLAN_MENU');
    targetPlans = [];
    $scope.model = {};
    isGetData = false;
    $scope.allUsers = []
    $scope.disableDivison = [];
    $scope.colorContent = "";
    var limit = 100000;
    var activeUsers = [];
    var holidayDates = [new Date('2020/06/30'), new Date('2020/07/07'), new Date('2020/08/07')];
    var targetPlanData = [];
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
        }]
    }];
    periodSelect = '';
    this.$onInit = async function () {
        setPagingDefault();
		var currentDate = new Date();
        $scope.localtimezone = -1 * currentDate.getTimezoneOffset();
        //targetPlanData = [{ name: '06/2020 - 07/2020', fromDate: new Date('2020/06/26'), toDate: new Date('2020/07/25'), id: '9430a059-4670-4616-a862-4456719fdf06' }, { name: '07/2020 - 08/2020', fromDate: new Date('2020/07/26'), toDate: new Date('2020/08/25'), id: '6cd0a605-1d13-4e55-bd0f-eca5b4f746d8' }];
        $scope.targetPlans = {
            placeholder: "",
            dataTextField: "name",
            dataValueField: "id",
            autoBind: true,
            valuePrimitive: true,
            filter: "contains",
            dataSource: targetPlanData,
            change: async function (e) {
                let dropdownlist = $("#period_source_id").data("kendoDropDownList");
                let dataItem = dropdownlist.dataItem(e.node);
                $scope.model.id = null;
                await getValidUserSAPForTargetPlan();
                periodSelect = dataItem;
                currentData = await getDetailData();

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
                $scope.total = 0;
                $scope.errorsTwo = {};
                let dropdownlist = $("#dept_line_id").data("kendoDropDownList");
                let dataItem = dropdownlist.dataItem(e.node)
                if (dataItem) {
                    $scope.model.deptId = dataItem.id;
                    $scope.model.deptCode = dataItem.code;
                    $scope.model.deptName = dataItem.name;
                    clearGrid('grid_user_id');
                    clearSearchTextOnDropdownTree('division_id');
                    var currentItem = _.find($scope.deptLineList, x => { return x.deptLine.id == $scope.model.deptId });
                    if (!currentItem.divisions.length) {
                        var dropdownTree = $(`#division_id`).data("kendoDropDownTree");
                        if (dropdownTree) {
                            dropdownTree.setDataSource([]);
                        }
                        await getValidUserSAPForTargetPlan();
                    } else {
                        var ids = _.map(currentItem.divisions, 'id');
                        await getDivisionTreeByIds(ids);
                    }
                }
            },
            select: function () {
                setPagingDefault();
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
                //return `<span class="${dataItem.item.jobGradeGrade > 4 ? 'k-state-disabled' : ''}">${showCustomDepartmentTitle(dataItem)}</span>`;
                return `<span class="${dataItem.item.type == 2 ? 'k-state-disabled' : ''}">${showCustomDepartmentTitle(dataItem)}</span>`;
            },
            loadOnDemand: false,
            valueTemplate: (e) => showCustomField(e, ['name']),
            select: async function (e) {
                setPagingDefault(1, $scope.page.limit);
                let dropdownlist = $("#division_id").data("kendoDropDownTree");
                let dataItem = dropdownlist.dataItem(e.node)
                //if (dataItem.jobGradeGrade > 4 || $scope.disableDivison.includes(dataItem.id)) {
                if (dataItem.type == 2 || $scope.disableDivison.includes(dataItem.id)) {
                    e.preventDefault();
                } else {
                    $scope.model.divisionId = dataItem.id;
                    $scope.model.divisionCode = dataItem.Code;
                    $scope.model.divisionName = dataItem.Name;
                }
                await getValidUserSAPForTargetPlan();
                dropdownlist.close();
            }
        }
        $scope.gridUsers = {
            dataSource: {
                serverPaging: false,
                pageSize: appSetting.pageSizeDefault,
            },
            sortable: false,
            pageable: {
                alwaysVisible: true,
                //pageSizes: appSetting.pageSizesArray,
                messages: {
                    display: "{0}-{1} " + $translate.instant('PAGING_OF') + " {2} " + $translate.instant('PAGING_ITEM'),
                    itemsPerPage: $translate.instant('PAGING_ITEM_PER_PAGE'),
                    empty: $translate.instant('PAGING_NO_ITEM')
                }
            },
            autoBind: true,
            columns: columnsDaysInPeriod,
            page: async function (option) {

                await BindingData(option);
            },
            dataBound: function (e) {
                var items = e.sender.items();
                items.each(function (idx, item) {
                    if (e.sender.dataItem(item).isUserLastRecord) {
                        $(item).addClass('separate');
                    }
                });
            },
        }
        $scope.createdByFullName = $rootScope.currentUser?.fullName;
        initTooltipContent();
        await getJobGradeList();
        findJobGrade();
        //await getTargetById();
    }

    let findJDG4;
    function findJobGrade() {
        findJDG4 = (_.find($scope.jobGradeList, x => x.title?.toLowerCase() === "g4")?.grade) ?? 4;
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
    function setPagingDefault(page, limit) {
        $scope.page = {
            current: page ? page : 1,
            limit: limit ? limit : appSetting.pageSizeDefault
        }
    }
    async function BindingData(option) {
        let divisionIds = !$scope.currentDivisonIds && $rootScope.currentUser ? $rootScope.currentUser.divisionId : $scope.currentDivisonIds;
        if (!divisionIds) {
            divisionIds = $scope.model.divisionId ? $scope.model.divisionId : $scope.model.deptId;
        }
        await getSapCodeByDivision([divisionIds], option);
        await getDetailData();
    }
    function getColumnFromPeriod(period, type) {
        let fieldName = type == 1 ? 'targetField' : 'actualField';
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
            //var noTemp = 1;
            while (ite <= to) {
                result.push({
                    field: `${fieldName}${moment(ite).format('YYYYMMDD')}`,
                    //title: `${getDayFromDate(ite)} ${moment(ite).format('DD/MM/YYYY')}`,
                    width: 100,
                    attributes: {
                        "class": "table-cell"
                    },
                    color: `${fieldName}${moment(ite).format('YYYYMMDD')}color`,
                    headerTemplate: `<div ng-class="{'is-header-sun-day': ${getDayFromDate(ite) == 'SHIFT_PLAN_SUN'}, 'is-header-holiday-day': ${isHoliday(ite)}}" style="text-align: center"> <div ng-class="{'is-header-sun-day': ${getDayFromDate(ite) == 'SHIFT_PLAN_SUN'}, 'is-header-holiday-day': ${isHoliday(ite)}}"> ${$translate.instant(getDayFromDate(ite))}</div> <div ng-class="{'is-header-sun-day': ${getDayFromDate(ite) == 'SHIFT_PLAN_SUN'}, 'is-header-holiday-day': ${isHoliday(ite)}}">${moment(ite).format('DD/MM/YYYY')}</div></div>`,
                    template: function (dataItem) {
                        //dataItem['isActive'] = _.findIndex(activeUsers, x => { return x == dataItem.sapCode }) > -1;
                        var input_html = '';
                        let checkTarget = getTargetType(dataItem);
                        //target 2 chỉ tô những ô có data - BA confirm
                        if (checkTarget == 2) {
                            let field = fieldName + moment(temp).format('YYYYMMDD')
                            if (dataItem[field]) {
                                input_html = `<input style="border: 1px solid #c9c9c9; text-align: center;" class="k-input w100" ng-model="dataItem.${fieldName}${moment(temp).format('YYYYMMDD')}" ng-readonly="{{!dataItem.isActive}}"/>`;
                            }
                            else {
                                input_html = `<input style="border: 1px solid #c9c9c9; text-align: center;" class="k-input w100" ng-model="dataItem.${fieldName}${moment(temp).format('YYYYMMDD')}" ng-readonly="{{!dataItem.isActive}}"/>`;
                            }
                        }
                        else if (checkTarget == 1 || checkTarget == 3 || checkTarget == 4) {
                            let propertyColor = fieldName + moment(temp).format('YYYYMMDD') + 'color';
                            input_html = `<input style="border: 1px solid #c9c9c9; text-align: center; background-color:${dataItem[propertyColor]} !important" class="k-input w100" ng-model="dataItem.${fieldName}${moment(temp).format('YYYYMMDD')}" ng-readonly="{{!dataItem.isActive}}"/>`;
                        }
                        temp = temp >= to ? from : addDays(temp, 1);
                        onChange(dataItem);
                        //noTemp = temp >= to? noTemp: noTemp++;
                        return input_html;
                    }

                });
                ite = addDays(ite, 1);
            }
        }
        return result;
    }
    function getTargetType(dataItem) {
        let result = 0;
        switch (dataItem.type) {
            case 'Target 1':
                result = 1;
                break;
            case 'Target 2':
                result = 2;
                break;
            case 'Actual 1':
                result = 3;
                break;
            case 'Actual 2':
                result = 4;
                break;
            default:
                break;
        }
        return result;
    }
    function isHoliday(date) {
        var index = _.findIndex(holidayDates, x => {
            return moment(x).format('DDMMYYYY') == moment(date).format('DDMMYYYY');
        });
        return index > -1;
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

    function onChange(item) {
        let ERDs = []
        let PRDs = [];
        let ALHs = [];
        let DOFLs = [];
        let DOHs = []; // target2
        Object.keys(item).forEach(function (fieldName) {
            if (fieldName.includes('targetField')) {
                if (item[fieldName]) {
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

            }
        });
        if (item.type == 'Target 1' || item.type == 'Actual 1') {
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

    function refreshGridColumns(columns) {
        $timeout(function () {
            $scope.gridUsers.columns = columns;
        }, 0);
    }
    async function getDepartmentByUserId(userId, deptId, divisionId) {
        $scope.deptLineList = [];
        //let res = await settingService.getInstance().departments.getDepartmentsByUserId({ id: userId }, null).$promise;
        let res = await settingService.getInstance().shiftPlanSubmitPerson.getDepartmentTargetPlansByUserId({ id: userId }, null).$promise;
        if (res && res.isSuccess) {
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
                await getDivisionsByDeptLine(deptId, divisionId);
                if (divisionId)
                    await getSapCodeByDivision([divisionId]);
            }
        }
    }
    async function getDivisionsByDeptLine(deptId, divisionId = '') {
        var currentItem = _.find($scope.deptLineList, x => { return x.deptLine.id == deptId });
        var divisionTree = $("#division_id").data("kendoDropDownTree");
        if (divisionTree) {
            if (currentItem && currentItem.divisions && currentItem.divisions.length) {
                var ids = _.map(currentItem.divisions, 'id');
                await getDivisionTreeByIds(ids);
            } else {
                await getDivisionTreeByIds([deptId]);
            }
            if (divisionId) {
                divisionTree.value(divisionId);
                $scope.model.divisionId = divisionId;
            }
        }

    }
    async function getDivisionTreeByIds(ids) {
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
        }
        result = await settingService.getInstance().departments.getDepartmentByArg(arg).$promise;
        if (result.isSuccess) {
            //$scope.dataTemporaryArrayDivision = _.filter(result.object.data, x => { return x.items.length || x.jobGradeGrade <= 4 });
            $scope.dataTemporaryArrayDivision = _.filter(result.object.data, x => { return x.items.length || x.type == 1 });
            let model = genarateDepartmentTargetPlan($scope.dataTemporaryArrayDivision, $rootScope.currentUser.sapCode);
            //$scope.disableDivison = await filterDivisionByUserNotSubmit(model);
            setDropDownTree('division_id', $scope.dataTemporaryArrayDivision);
            if (_.findIndex(ids, x => { return x == $rootScope.currentUser.divisionId }) > -1) {
                var divisionTree = $("#division_id").data("kendoDropDownTree");
                if (divisionTree) {
                    divisionTree.value($rootScope.currentUser.divisionId);
                }
            } else {
                $scope.visibleSubmitData = false;
            }
            await getSapCodeByDivision(ids);
            //await getDetailData();
        } else {
            Notification.error(result.messages[0]);
        }
    }

    function generateQueryGetUser(divisionIds, option) {
        if (option && option.page) {
            $scope.page.current = option.page;
        }
        let model = {
            periodId: $scope.model.periodId,
            items: [],
            activeUsers: activeUsers,
            query: {
                predicate: "",
                predicateParameters: [],
                page: $scope.page.current,
                limit: 500,
                order: "",
            },
            isNoDivisionChosen: false
        }
        if (!divisionIds.length) {
            //let dept = _.find($scope.deptLineList, function (o) { return o.deptLine.id == $scope.model.deptId });
            //if (dept.divisions && dept.divisions.length) {
            //    divisionIds = _.map(dept.divisions, 'id');
            //}
            //divisionIds.push($scope.deptLineList[0].deptLine.id);
        }
        if (divisionIds.length) {
            _.forEach(divisionIds, function (id) {
                let isIncludeChildren = null;
                let dept = _.find($scope.deptLineList, function (o) { return o.deptLine.id == $scope.model.deptId });
                if (dept) {
                    let divisions = dept.divisions;
                    var currentDivision = _.find(divisions, function (o) { return o.id == id });
                    isIncludeChildren = currentDivision ? currentDivision.isIncludeChildren : false;
                    model.items.push({ id: id, isIncludeChildren: isIncludeChildren });
                }
            });
        } else {
            if ($rootScope.currentUser.jobGradeValue <= findJDG4) {
                let isIncludeChildren = null;
                let dept = _.find($scope.deptLineList, function (o) { return o.deptLine.id == $scope.model.deptId });
                if (dept) {
                    let divisions = dept.divisions;
                    var currentDivision = _.find(divisions, function (o) { return o.id == $rootScope.currentUser.divisionId });
                    isIncludeChildren = currentDivision ? currentDivision.isIncludeChildren : false;
                    model.items.push({ id: currentDivision.id, isIncludeChildren: isIncludeChildren });
                    model.isNoDivisionChosen = true;
                }
            } else {
                let dept = _.find($scope.deptLineList, function (o) { return o.deptLine.id == $rootScope.currentUser.deptId });
                model.items.push({ id: $scope.model.deptId, isIncludeChildren: dept.deptLine.isIncludeChildren });
            }
        }
        model.type = 1; // Shift Plan de mapping backend
        return model;
    }
    var itemsTotal = 0;
    async function getSapCodeByDivision(divisionIds, option) {
        if (divisionIds) {
            /*let model = generateQueryGetUser(divisionIds, option);
            if ($scope.model.divisionId) {
                model.divisionId = $scope.model.divisionId;
            }*/
            //let result = await settingService.getInstance().users.getUsersForTargetPlanByDeptId(model).$promise;
            let result = null;
            let model = generateQueryGetUser(divisionIds, option);
            if (!$scope.model.divisionId) {
                result = await settingService.getInstance().users.getUsersByOnlyDeptLine({ depLineId: $scope.model.deptId, limit: limit, page: 1, searchText: "" }).$promise;
            } else {
                model.divisionId = $scope.model.divisionId;
                result = await settingService.getInstance().users.getUsersForTargetPlanByDeptId(model).$promise;
            }

            if (result.isSuccess) {
                $scope.allUsers = result.object.data;
                $scope.total = result.object.count;
                if (itemsTotal == 0) { // Khiem - 414
                    itemsTotal = result.object.count;
                }
            }
        }
    }
    period = '';
    async function getPeriods() {
        var res = await cbService.getInstance().targetPlan.getTargetPlanPeriods().$promise;
        if (res.isSuccess) {
            targetPlanData = res.object.data;
            let currentPeriod = _.filter(targetPlanData, x => {
                var currentDate = new Date();
                if (currentDate.getDate() >= 26) {
                    return x.fromDate.getMonth() == currentDate.getMonth() && x.fromDate.getYear() == currentDate.getYear();
                }
                return x.toDate.getMonth() == currentDate.getMonth() && x.toDate.getYear() == currentDate.getYear();
            });
            if (currentPeriod.length) {
                $scope.model.periodId = currentPeriod[0].id;
                $scope.model.periodName = currentPeriod[0].name;
                $scope.model.periodFromDate = currentPeriod[0].fromDate;
                $scope.model.periodToDate = currentPeriod[0].toDate;
            }
            setDataSourceForDropdownList('#period_source_id', targetPlanData);
            period = currentPeriod;
            //displayData(currentPeriod);
        }
    }
    // async function getDetailData() {
    //     // Get target plan bởi departmentId, và Period
    //     let res = await cbService.getInstance().targetPlan.getTargetPlanFromDepartmentAndPeriod({ departmentId: $scope.model.deptId, divisionId: $scope.model.divisionId, periodId: $scope.model.periodId }).$promise;
    //     if (res && res.isSuccess) {
    //         $scope.model = res.object;
    //     }
    //     // get target plan detail 
    //     var viewUsers = _.map($scope.allUsers, 'sapCode');
    //     let result = await cbService.getInstance().targetPlan.getTargetPlanDetailFromSAPCodesAndPeriod({ periodId: $scope.model.periodId, sapCodes: viewUsers }).$promise;
    //     if (result.isSuccess && result.object) {
    //         let targetPlanDetailItems = result.object.data;
    //         let validUsers = _.filter(targetPlanDetailItems, x => {
    //             return _.findIndex($scope.allUsers, y => { return y.sapCode == x.sapCode }) > -1;
    //         });
    //         if (validUsers && validUsers.length) {
    //             let currentUsers = [];
    //             _.forEach(validUsers, x => {
    //                 var currentObject = { 'sapCode': x.sapCode, 'fullName': x.fullName, 'departmentName': x.departmentName, 'departmentCode': x.departmentCode, 'type': `${commonData.targetPlanType[`target${x.type}`]}`, erdQuality: '', prdQuality: '', lhQuality: '' };
    //                 let list = JSON.parse(x.jsonData);
    //                 _.forEach(list, y => {
    //                     currentObject[`targetField${y.date}`] = y.value;
    //                 })
    //                 currentUsers.push(currentObject);
    //             });
    //             updateTargetPlanDetails(currentUsers);
    //         }

    //     }
    // }
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
        },1);

        $scope.localtimezone = timezoneOffset;


        if (columnInPeriods.length) {
            let tempColumns = columnsDaysInPeriod.concat(columnInPeriods);
            refreshGridColumns(tempColumns);
        };
        _.forEach($scope.allUsers, x => {
            let createdTargetItems = _.filter(listUsers, y => { return y.sapCode == x.sapCode });
            if (!createdTargetItems.length) {
                for (let i = 1; i <= 4; i++) {
                    var currentObject = { 'sapCode': x.sapCode, 'fullName': x.fullName, 'departmentName': x.department, 'departmentCode': x.departmentCode, 'type': `${commonData.targetPlanType[`target${i}`]}`, erdQuality: x.erdQuality, prdQuality: x.prdQuality, alhQuality: x.alhQuality, doflQuality: x.doflQuality };
                    _.forEach(columnInPeriods, y => {
                        currentObject[`${y.field}`] = '';
                        currentObject[`${y.color}`] = commonData.color.Active;
                    });
                    currentUsers.push(currentObject);
                }
            }
            else {
                let orderItems = _.sortBy(createdTargetItems, ['sapCode', 'type']);
                _.forEach(orderItems, t => {
                    currentUsers.push(t);
                });
                if (orderItems.length <= 2) {
                    for (let i = 3; i <= 4; i++) {
                        let currentObject = { 'sapCode': x.sapCode, 'fullName': x.fullName, 'departmentName': x.department, 'departmentCode': x.departmentCode, 'type': `${commonData.targetPlanType[`target${i}`]}`, erdQuality: x.erdQuality, prdQuality: x.prdQuality, alhQuality: x.alhQuality, doflQuality: x.doflQuality, color: '' };
                        _.forEach(columnInPeriods, y => {
                            let result = '';
                            if (currentObject.type == 'Actual 1') {
                                result = currentUsers.find(x => x.sapCode == currentObject.sapCode && x.type == 'Target 1');
                            }
                            else if (currentObject.type == 'Actual 2') {
                                result = currentUsers.find(x => x.sapCode == currentObject.sapCode && x.type == 'Target 2');
                            }
                            if (result) {
                                let itemExistActualShiftPlan = actualShiftPlans.find(x => x.sapCode == result.sapCode);
                                if (itemExistActualShiftPlan) {
                                    let itemExist = findItemActualShiftPlan(itemExistActualShiftPlan, currentObject.type, y.field);
                                    if (itemExist) {
                                        currentObject[`${y.field}`] = itemExist.Value;
                                        //nếu value ko có data thì ko tô màu
                                        if (itemExist.Value) {
                                            currentObject[`${y.color}`] = itemExist.backGroundColor;
                                        }
                                        else {
                                            currentObject[`${y.color}`] = commonData.color.Active;
                                        }
                                    }
                                    else {
                                        currentObject[`${y.field}`] = result[`${y.field}`];
                                    }
                                }
                                else {
                                    currentObject[`${y.field}`] = result[`${y.field}`];
                                }
                                // if (currentObject.type == 'Actual 1') {
                                //     if (result[`${y.field}`]) {
                                //         currentObject[`${y.color}`] = commonData.color.LeaveActual;
                                //     }
                                // }
                                if (currentObject.type == 'Actual 2') {
                                    if (result[`${y.field}`]) {
                                        //đối với actual 2 thì nếu ko có leave - shift mà có data thì tô mày vàng giống target 2 - BA đã confirm
                                        // currentObject[`${y.color}`] = commonData.color.LeaveActualOrShiftActualInProgress;
                                        // checkInprogressLeaveOrShift();
                                        let itemExistActualShiftPlan = actualShiftPlans.find(x => x.sapCode == result.sapCode);
                                        if (itemExistActualShiftPlan) {
                                            let itemExist = findItemActualShiftPlan(itemExistActualShiftPlan, currentObject.type, y.field);
                                            if (itemExist) {
                                                currentObject[`${y.color}`] = commonData.color.LeaveActualOrShiftActualInProgress;
                                                checkInprogressLeaveOrShift();
                                            }
                                        }
                                    }
                                }
                            }
                            else {
                                currentObject[`${y.field}`] = '';
                                currentObject[`${y.color}`] = commonData.color.Active;
                            }
                        });
                        currentUsers.push(currentObject);
                    }
                }
            }
        });
        if (currentUsers.length) {
            let sortItems = _.filter(currentUsers, x => { return activeUsers.includes(x.sapCode) });
            let unSortItems = _.filter(currentUsers, x => { return !activeUsers.includes(x.sapCode) });
            let sortedUsers = sortItems.concat(unSortItems);
            for (let i = 3; i < sortedUsers.length; i += 4) {
                sortedUsers[i].isUserLastRecord = true;
            }
            $timeout(function () {
                setDataForUser(sortedUsers);
            }, 0);
        }
    }
    function checkInprogressLeaveOrShift() {
        return console.log(actualShiftPlans);
    }
    function setDataForUser(data) {
        let grid = $('#grid_user_id').data("kendoGrid");
        let dataSource = new kendo.data.DataSource({
            data: data,
            pageSize: 500,
            serverPaging: true,
            page: $scope.page.current,
            schema: {
                total: function () {
                    return $scope.total * 4;
                },
                data: () => { return data }
            },
        });
        if (grid) {
            grid.setDataSource(dataSource);
        }
    }
    function findItemActualShiftPlan(item, type, fieldName) {
        let result = ''
        let data = []
        switch (type) {
            case 'Actual 1':
                data = JSON.parse(item.actual1);
                break;
            case 'Actual 2':
                data = JSON.parse(item.actual2);
                break;
            default:
                break;
        }
        if (data) {
            data.forEach(x => {
                if (x.Value) {
                    x.Date = 'targetField' + x.Date;
                    if (x.Type == 'Leave') {
                        x['backGroundColor'] = commonData.color.LeaveActual;
                    }
                    else if (x.Type == 'Shift') {
                        x['backGroundColor'] = commonData.color.ShiftActual;
                    }
                    if (x.Status != "Completed") {
                        x['backGroundColor'] = commonData.color.LeaveActualOrShiftActualInProgress;
                    }
                }

            });
            let itemExist = data.find(x => x.Date == fieldName);
            if (itemExist) {
                result = itemExist;
            }
        }
        return result;
    }

    function displayData(periodData) {
        kendo.ui.progress($("#loading"), true);
        let currentUsers = [];
        var columnInPeriods = getColumnFromPeriod(periodData, 1);
        if (columnInPeriods.length) {
            let tempColumns = columnsDaysInPeriod.concat(columnInPeriods);
            refreshGridColumns(tempColumns);
        };
        _.forEach($scope.allUsers, x => {
            for (let i = 1; i <= 4; i++) {
                var currentObject = { 'sapCode': x.sapCode, 'fullName': x.fullName, 'departmentName': x.department, 'departmentCode': x.departmentCode, 'type': `${commonData.targetPlanType[`target${i}`]}`, erdQuality: x.erdQuality, prdQuality: x.prdQuality, alhQuality: x.alhQuality, doflQuality: x.doflQuality };
                _.forEach(columnInPeriods, y => {
                    currentObject[`${y.field}`] = '';
                    currentObject[`${y.color}`] = '';
                });
                currentUsers.push(currentObject);
            }
        });
        if (currentUsers.length) {
            let sortItems = _.filter(currentUsers, x => { return activeUsers.includes(x.sapCode) });
            let unSortItems = _.filter(currentUsers, x => { return !activeUsers.includes(x.sapCode) });
            let sortedUsers = sortItems.concat(unSortItems);
            setDataSourceForGrid('#grid_user_id', sortedUsers);
            //setDataForUser(sortedUsers);
            $scope.model.periodId = periodData.id;
            $scope.model.periodName = periodData.name;
            $scope.model.periodFromDate = periodData.fromDate;
            $scope.model.periodToDate = periodData.toDate;
        }
        kendo.ui.progress($("#loading"), false);
    }
    $scope.onChangeDivision = async function (divisionId) {
        let divisionIds = [];
        if (divisionId) {
            divisionIds.push(divisionId);
        }
        await getSapCodeByDivision(divisionIds);
        // await displayData({
        //     id: $scope.model.periodId,
        //     name: $scope.model.periodName,
        //     fromDate: $scope.model.periodFromDate,
        //     toDate: $scope.model.periodToDate
        // });
        await getDetailData();
    }
    $scope.$on("targetPlan", async function (evt, data) {
        $scope.model = data;
        await getPeriods();
        activeUsers.push($rootScope.currentUser.sapCode);
        await getDepartmentByUserId($rootScope.currentUser.id, $rootScope.currentUser.deptId, $scope.currentUser.divisionId);
        await getDetailData();

    });

    $scope.downloadShiftCode = function () {
        window.location.href = 'ClientApp/assets/templates/Shift Codes.xlsx';
    };

    function convertJsonData() {
        let removeFieldFromModels = ['createdById', 'jsonData'];
        let model = $scope.model;
        let jsonData = model.jsonData;
        let list = [];
        var arrayDataFromJson = _.groupBy(JSON.parse(jsonData), 'sapCode');
        if (arrayDataFromJson) {
            Object.keys(arrayDataFromJson).forEach(function (targetPlan) {
                tempTarget1 = createData(arrayDataFromJson[targetPlan][0]);
                tempTarget2 = createData(arrayDataFromJson[targetPlan][1]);
                list.push(tempTarget1);
                list.push(tempTarget2);
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

    actualShiftPlans = [];
    async function getTargetById(targetPlanId) {
        let sapCodes = [];
        $scope.allUsers.forEach(item => {
            sapCodes.push(item.sapCode)
        });
        let model = {
            periodId: $scope.model.periodId,
            listSAPCode: JSON.stringify(sapCodes),
            notInShiftExchange: []
        }
        let res = await cbService.getInstance().targetPlan.getActualShiftPlan(model).$promise;
        if (res && res.isSuccess) {
            actualShiftPlans = res.object.data;
        }
    }

    $scope.cancel = function () {
        $rootScope.cancel();
    }

    $scope.export = async function () {
        //Khiem - 414
        var allItems = [];
        let modelQuery;
        var limit = 100000;
        if (!$scope.model.divisionId || $scope.model.divisionId == "") {
            let result = await settingService.getInstance().users.getUsersByOnlyDeptLine({ depLineId: $scope.model.deptId, limit: limit, page: 1, searchText: "" }).$promise;
            if (result.isSuccess) {
                allItems = allItems.concat(result.object.data);
            }
        }
        else {
            modelQuery = generateQueryGetUser([$scope.model.divisionId], null);
            modelQuery.divisionId = $scope.model.divisionId;
            modelQuery.query.limit = limit;
            let result = await settingService.getInstance().users.getUsersForTargetPlanByDeptId(modelQuery).$promise;
            if (result.isSuccess) {
                allItems = allItems.concat(result.object.data);
            }
        }
        var viewUsers = _.map(allItems, 'sapCode');

        let model = {
            departmentId: $scope.model.deptId,
            divisionId: $scope.model.divisionId,
            periodId: $scope.model.periodId,
            sapCodes: viewUsers
        }

        let res = await cbService.getInstance().targetPlan.printForm(model).$promise;
        if (res.isSuccess) {
            exportToExcelFile(res.object);
            Notification.success(appSetting.notificationExport.success);
        } else {
            Notification.error(appSetting.notificationExport.error);
        }
    }

    async function getDetailData() {
        // get target plan detail 
        var viewUsers = _.map($scope.allUsers, 'sapCode');
        let model = {
            departmentId: $scope.model.deptId,
            divisionId: $scope.model.divisionId,
            periodId: $scope.model.periodId,
            sapCodes: viewUsers
        }
        let result = await cbService.getInstance().targetPlan.getTargetPlanDetailIsCompleted(model).$promise;
        if (result.isSuccess && result.object) {
            kendo.ui.progress($("#loading"), true);
            let targetPlanDetailItems = result.object.data;
            if (targetPlanDetailItems.length) {
                await getTargetById(targetPlanDetailItems[0].targetPlanId);
            }
            let validUsers = _.filter(targetPlanDetailItems, x => {
                return _.findIndex($scope.allUsers, y => { return y.sapCode == x.sapCode }) > -1;
            });
            if (validUsers && validUsers.length) {
                if (periodSelect) {
                    displayData(periodSelect);
                }
                else {
                    let period = {
                        id: $scope.model.periodId,
                        name: $scope.model.periodName,
                        fromDate: $scope.model.periodFromDate,
                        toDate: $scope.model.periodToDate
                    }
                    displayData(period);
                    kendo.ui.progress($("#loading"), true);
                }
                // displayData(period);
                $timeout(function () {
                    kendo.ui.progress($("#loading"), true);
                    let currentUsers = [];
                    _.forEach(validUsers, x => {
                        var currentObject = { 'sapCode': x.sapCode, 'fullName': x.fullName, 'departmentName': x.departmentName, 'departmentCode': x.departmentCode, 'type': `${commonData.targetPlanType[`target${x.type}`]}`, erdQuality: x.erdQuality, prdQuality: x.prdQuality, alhQuality: x.alhQuality, doflQuality: x.doflQuality };
                        let list = JSON.parse(x.jsonData);
                        _.forEach(list, y => {
                            currentObject[`targetField${y.date}`] = y.value;
                        })
                        currentUsers.push(currentObject);
                    });
                    updateTargetPlanDetails(currentUsers);
                    kendo.ui.progress($("#loading"), false);
                }, 0);
            }
        }
        if (!result.object || result.object.count == 0) {
            kendo.ui.progress($("#loading"), false);
            let period = '';
            if (periodSelect) {
                period = periodSelect;
            }
            else {
                period = {
                    id: $scope.model.periodId,
                    name: $scope.model.periodName,
                    fromDate: $scope.model.periodFromDate,
                    toDate: $scope.model.periodToDate
                }
            }
            var columnInPeriods = getColumnFromPeriod(period, 1);
            if (columnInPeriods.length) {
                let tempColumns = columnsDaysInPeriod.concat(columnInPeriods);
                refreshGridColumns(tempColumns);
            };
            let grid = $("#grid_user_id").data("kendoGrid");
            grid.setDataSource([]);
        }
    }

    async function getValidUserSAPForTargetPlan() {
        let departmentId = '';
        if ($scope.model.divisionId) {
            departmentId = $scope.model.divisionId;
        } else if ($scope.model.deptId) {
            departmentId = $scope.model.deptId;
        }
        let res = await cbService.getInstance().targetPlan.getValidUserSAPForTargetPlan({ id: departmentId, userId: $rootScope.currentUser.id, periodId: $scope.model.periodId }, null).$promise;
        if (res && res.isSuccess && res.object) {
            activeUsers = _.map(res.object.data, 'sapCode');
        } else {
            $timeout(function () {
                activeUsers = [];
            });
        }
    }

    function genarateDepartmentTargetPlan(dataList, submitSapCode) {
        let model = {
            arg: {
                predicate: "",
                predicateParameters: []
            },
            submittedSapCode: submitSapCode
        }
        for (let i = 0; i < dataList.length; i++) {
            model.arg.predicate = model.arg.predicate ? model.arg.predicate + `||id = @${i}` : `id = @${i}`;
            model.arg.predicateParameters.push(dataList[i].id);
        }
        return model;
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
            case commonData.targetPlanType.target3:
                result = 'SHIFT_PLAN_ACTUAL_1';
                break;
            case commonData.targetPlanType.target4:
                result = 'SHIFT_PLAN_ACTUAL_2';
                break;
            default:
                break;
        }
        return result;
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