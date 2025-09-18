
var ssgApp = angular.module("ssg.addTargetPlanModule", [
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
    dataService
) {
    $scope.title = $stateParams.action.title;
    isItem = true;
    targetPlans = [];

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
        field: "department",
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
        width: 120
    },
    {
        title: "Count Off Day",
        width: "200px",
        columns: [{
            field: "ERD",
            width: 50
        }, {
            field: "PRD",
            width: 50
        },
        {
            field: "ALH",
            width: 50
        }]
    }];
    var currentUserList = [];
    $scope.allUsers = [{
        department: "SYSTEM INTERGRATION (G3)",
        fullName: "Cao Chánh Nguyên Hiển",
        id: "4ababce9-b826-4513-a052-ceab483b61d4",
        jobGrade: "G3",
        loginName: "hien.cao",
        position: "Executive (G3)",
        sapCode: "00403565"
    }, {
        department: "SYSTEM INTERGRATION (G3)",
        email: "",
        fullName: "Cao Chánh Nguyên Hiển",
        id: "830be62f-fe6c-48b3-b4fb-f2e92b91161c",
        jobGrade: "G3",
        loginName: "40356511",
        position: "Executive (G3)",
        sapCode: "0040356511"
    }]
    var holidayDates = [new Date('2020/06/30'), new Date('2020/07/07'), new Date('2020/08/07')];
    //$scope.allUserData = [];
    this.$onInit = function () {
        targetPlanData = [{ name: '26/06/2020 - 25/07/2020', fromDate: new Date('2020/06/26'), toDate: new Date('2020/07/25'), id: '9430a059-4670-4616-a862-4456719fdf06' }, { name: '2020/07/26 - 25/08/2020', fromDate: new Date('2020/07/26'), toDate: new Date('2020/08/25'), id: '6cd0a605-1d13-4e55-bd0f-eca5b4f746d8' }];
        $scope.targetPlans = {
            placeholder: "",
            dataTextField: "name",
            dataValueField: "id",
            autoBind: false,
            valuePrimitive: true,
            filter: "contains",
            dataSource: targetPlanData,
            change: function (e) {
                let currentUsers = [];
                var columnInPeriods = getColumnFromPeriod(e.sender.dataItem());
                if (columnInPeriods.length) {
                    let tempColumns = columnsDaysInPeriod.concat(columnInPeriods);
                    refreshGridColumns(tempColumns);
                };
                _.forEach($scope.allUsers, x => {
                    for (let i = 1; i <= 2; i++) {
                        var currentObject = { 'sapCode': x.sapCode, 'fullName': x.fullName, 'department': x.department, 'type': `Target ${i}`};
                        _.forEach(columnInPeriods, y=> {
                            currentObject[`${y.field}`] = '';
                        });
                        currentUsers.push(currentObject);
                    }
                });
                $timeout(function () {
                    setDataSourceForGrid('#grid_user_id', currentUsers);
                }, 0);
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
                let i = e.sender;
                let deptId = i.value();
                if (deptId) {
                }
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
                // return `<span class="${dataItem.item.jobGradeGrade > 4 ? 'k-state-disabled' : ''}">${showCustomDepartmentTitle(dataItem)}</span>`;
                return `<span class="${dataItem.item.type == 2 ? 'k-state-disabled' : ''}">${showCustomDepartmentTitle(dataItem)}</span>`;
            },
            loadOnDemand: false,
            valueTemplate: (e) => showCustomField(e, ['name']),
            select: async function (e) {
                let dropdownlist = $("#division_id").data("kendoDropDownTree");
                let dataItem = dropdownlist.dataItem(e.node)
                //if (dataItem.jobGradeGrade > 4) {
                if (dataItem.jobGradeGrade == 2) {
                    e.preventDefault();
                } else {
                    $scope.model.division = dataItem.id;
                }
                dropdownlist.close();
            }
        }
        $scope.gridUsers = {
            dataSource: {
                data: []
            },
            sortable: false,
            autoBind: true,
            valuePrimitive: true,
            columns: columnsDaysInPeriod
        }
    }
    function getColumnFromPeriod(period) {
        let result = [];
        if (period && period.fromDate && period.toDate) {
            var from = period.fromDate;
            var to = period.toDate;
            var ite = from;
            var temp = from;
            //var noTemp = 1;
            while (ite <= to) {
                result.push({
                    field: `code${getDayFromDate(ite)}${moment(ite).format('DDMMYYYY')}`,
                    //title: `${getDayFromDate(ite)} ${moment(ite).format('DD/MM/YYYY')}`,
                    width: 150,
                    headerTemplate: `<span ng-class="{'is-header-sun-day': ${getDayFromDate(ite) == 'Sun'}, 'is-header-holiday-day': ${isHoliday(ite)}}"> ${getDayFromDate(ite)} ${moment(ite).format('DD/MM/YYYY')}</span>`,
                    template: function (dataItem) {
                        var input_html = '';
                        input_html = `<input ng-class="{'is-sun': ${getDayFromDate(temp) == 'Sun'}, 'is-holiday': ${isHoliday(temp)}}" style="border: 1px solid #c9c9c9; text-align: center" class="k-input w100" ng-model="dataItem.code${getDayFromDate(temp)}${moment(temp).format('DDMMYYYY')}"/>`;
                        temp = temp >= to ? from : addDays(temp, 1);
                        //noTemp = temp >= to? noTemp: noTemp++;
                        return input_html;
                    }

                });
                ite = addDays(ite, 1);
            }
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
    async function getDepartmentByUserId(userId, deptId, divisionId) {
        $scope.deptLineList = [];
        let res = await settingService.getInstance().departments.getDepartmentById({ id: userId }, null).$promise;
        if (res && res.isSuccess) {
            $scope.deptLineList = res.object.data;
            var deptLines = _.map($scope.deptLineList, x => { return x['deptLine'] });
            var deptLineList = $("#dept_line_id").data("kendoDropDownList");
            if (deptLineList)
                deptLineList.setDataSource(deptLines);
            if (deptId) {
                if (deptLineList) {
                    deptLineList.value(deptId);
                }
                await getDivisionsByDeptLine(deptId, divisionId);
                if (divisionId)
                    await getSapCodeByDivision(divisionId);
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
        result = await settingService.getInstance().departments.getDepartmentByFilter(arg).$promise;
        if (result.isSuccess) {
            // $scope.dataTemporaryArrayDivision = _.filter(result.object.data, x => { return x.items.length || x.jobGradeGrade <= 4 });
            $scope.dataTemporaryArrayDivision = _.filter(result.object.data, x => { return x.items.length || x.type == 1 });
            setDropDownTree('division_id', $scope.dataTemporaryArrayDivision);
            if (_.findIndex(ids, x => { return x == $rootScope.currentUser.divisionId }) > -1) {
                var divisionTree = $("#division_id").data("kendoDropDownTree");
                if (divisionTree) {
                    divisionTree.value($rootScope.currentUser.divisionId);
                }
            }

        } else {
            Notification.error(result.messages[0]);
        }
    }
    async function getSapCodeByDivision(divisionId) {
        if (divisionId) {
            let result = await settingService.getInstance().users.getChildUsers({ departmentId: divisionId, limit: 1000, page: 1, searchText: '', isAll: true }).$promise;
            if (result.isSuccess) {
                $scope.allUsers = result.object.data;
                $scope.total = result.object.count;
                console.log($scope.allUsers);
            }
        }
    }

    $scope.save = function () {
        console.log($('#grid_user_id').data('kendoGrid').dataSource._data)
    }
    // $scope.$on("targetPlan", async function (evt, data) {
    //     await getDepartmentByUserId($rootScope.currentUser.id, $rootScope.currentUser.deptId, $scope.currentUser.divisionId);
    // });
    // $rootScope.$watch('currentUser', async function (newValue, oldValue) {
    //     if (!oldValue && newValue) {
    //         $rootScope.$broadcast("targetPlan", true);
    //     }
    // });
});