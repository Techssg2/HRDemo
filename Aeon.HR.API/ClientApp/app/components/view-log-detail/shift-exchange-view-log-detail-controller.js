var ssgApp = angular.module("ssg.shiftExchangeViewLogDetailModule", [
    "kendo.directives"
]);
ssgApp.controller("shiftExchangeViewLogDetailController", function (
    $rootScope,
    $scope,
    appSetting,
    $stateParams,
    $state,
    commonData,
    Notification,
    settingService,
    masterDataService,
    $timeout,
    $translate,
    trackingHistoryService
) {
    // create a message to display in our view
    var ssg = this;
    $scope.isITHelpdesk = $rootScope.currentUser && $rootScope.currentUser.isITHelpDesk;
    $scope.value = 0;
    isItem = true;
    $scope.titleHeader = "SHIFT EXCHANGE";
    $scope.title = $stateParams.referenceValue;
    stateItem = "home.shiftExchange.item";
    stateItemView = "home.shiftExchange.itemView";
    $scope.isShow = false;
    $scope.oldExchangingShiftItems = [];
    //Status Data
    $scope.query = {
        keyword: '',
        departmentId: null,
        fromDate: null,
        toDate: null,
        status: ''
    };
    $scope.review = {
        errorMessage: ''
    };
    
    $scope.model = {};
    $scope.workflowInstances = [];
    $scope.deptLine = [];
    $scope.deptDivision = [];
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
            $scope.deptDivision = result.object.data;
            $scope.dataTemporaryArrayDivision = _.filter(result.object.data, x => { return x.items.length || x.jobGradeGrade <= 4 });
            setDataDeptDivision($scope.dataTemporaryArrayDivision);
            if (_.findIndex(ids, x => { return x == $rootScope.currentUser.divisionId }) > -1) {
                var dropdownlist = $("#deptDivision").data("kendoDropDownTree");
                if (!$stateParams.id) {
                    if (dropdownlist) {
                        dropdownlist.value($rootScope.currentUser.divisionId);
                    }
                    $scope.form.dept_Division = $rootScope.currentUser.divisionId;
                }
                //await getSapCodeByDivison(1, $scope.limitDefaultGrid, '');
            }

        } else {
            Notification.error(result.messages[0]);
        }
    }

    function setDataDeptDivision(dataDepartment) {
        var dataSource = new kendo.data.HierarchicalDataSource({
            data: dataDepartment
        });
        var dropdownlist = $("#deptDivision").data("kendoDropDownTree");
        if (dropdownlist) {
            dropdownlist.setDataSource(dataSource);
        }
    }

    $scope.total = 0;
    $scope.data = [];
    $scope.currentQuery = {
        predicate: "",
        predicateParameters: [],
        Order: "Created desc",
        Limit: appSetting.pageSizeDefault,
        Page: 1
    }

    $scope.shiftCodes = [];

    async function getShiftCodes() {
        var res = await masterDataService.getInstance().masterData.GetMasterDataInfo({
            name: 'CurrentShift',
        }).$promise;
        if (res.isSuccess) {

            res.object.data = $.grep(res.object.data, function (v) {
                return v.code != "OT";
            });


            $scope.shiftExchangeDataSource = res.object.data.map(function (item) {

                return { ...item, showtextCode: item.code }
            });
        }
    }
    async function getNewShiftCodes() {
        var res = await masterDataService.getInstance().masterData.GetMasterDataInfo({
            name: 'ShiftCode',
            //name: 'CurrentShift',
        }).$promise;
        if (res.isSuccess)

            res.object.data = $.grep(res.object.data, function (v) {
                return v.code != "OT";
            });

        $scope.newShiftExchangeDataSource = res.object.data.map(function (item) {

            return { ...item, showtextCode: item.code }
        });
    }

    $scope.shiftExchangeId = '';
    $scope.referenceNumber = '';
    $scope.test = '';
    $scope.onChangeDivision = async function (value) {
        //if (e.sender.value() && $scope.form.exchangingShiftItems.length > 0) {
        if ($scope.form.exchangingShiftItems.length > 0 && $scope.perm && !value) {
            $timeout(function () {
                $scope.dialog = $rootScope.showConfirmDelete($translate.instant('COMMON_NOTIFY'), $translate.instant('SHIFT_EXCHANGE_NOTIFY'), $translate.instant('COMMON_BUTTON_CONFIRM'));
                $scope.dialog.bind("close", confirm);
            }, 0);
        }
        else {
            if (!value) {
                kendo.ui.progress($("#loading"), true);
                clearSearchTextOnDropdownTree('deptDivision');
                setDataDeptDivision($scope.dataTemporaryArrayDivision);
                await getUserCheckedHeadCount($scope.form.dept_Line);
                if ((!value || value == '') && $scope.shiftExchangeId && $scope.referenceNumber) {
                    if ($scope.oldExchangingShiftItems) {
                        $scope.form.exchangingShiftItems = $scope.oldExchangingShiftItems[0];
                        initShiftExchangeEditListGrid($scope.form.exchangingShiftItems);
                    }
                }
            } else {
            }
            kendo.ui.progress($("#loading"), false);
        }
    }
    async function getShiftExchangeById(id) {
        let result = await trackingHistoryService.getInstance().trackingHistory.getTrackingHistoryById({ id }).$promise;
        if (result.isSuccess) {
            //Khiem added new value of $scope
            var dataStr = JSON.parse(result.object.dataStr);
            $scope.model.moduleTitle = "Shift Exchange";
            $scope.model.referenceNumber = dataStr.referenceNumber;
            $scope.model.shiftExchangeItemsData = dataStr.shiftExchangeItemsData;
            $scope.model.id = result.object.id;
            $scope.form.applyDate = new Date(dataStr.applyDate);
            $scope.form.dept_Line = dataStr.deptLine;
            $scope.form.dept_Division = dataStr.deptDivision;
            $scope.model.created = dataStr.created;
            $scope.model.createdByFullName = dataStr.createdByFullName;
            await getDepartmentByUserId(dataStr.createdById, dataStr.deptLine, dataStr.deptDivision);
        }
        $timeout(function () {
            $scope.title = /*$translate.instant('SHIFT_EXCHANGE_APPLICATION_EDIT_TITLE') +*/ $scope.model.referenceNumber;
            $scope.exchangingShiftItems = dataStr.shiftExchangeItemsData;
            mapGridForItem($scope.exchangingShiftItems);
        }, 0)
    }

    $scope.shiftExchangeViewLogListGridOptions = {
        dataSource: {
            data: $scope.form.exchangingShiftItems,
            pageSize: 100
        },
        sortable: false,
        pageable: false,
        // editable: true,
        columns: [{
            field: "no",
            //title: "No.",
            headerTemplate: $translate.instant('COMMON_NO'),
            width: "80px",
            editable: function (e) {
                return false;
            }
        },
        {
            field: "employeeCode",
            //title: "SAP Code",
            headerTemplate: $translate.instant('COMMON_SAP_CODE'),
            width: "200px",
            // editor: employeeDropDownEditor,
            template: function (dataItem) {
                return `<input name="others" ng-readonly="true" class="form-control" type="text" ng-model="dataItem.employeeCode" style="width: 100%"/>`;
            }
        },
        {
            field: "fullName",
            //title: "Full Name",
            headerTemplate: $translate.instant('COMMON_FULL_NAME'),
            width: "200px",
            // editable: function(e) {
            //     return false;
            // }
            template: function (dataItem) {
                return `<input name="others" ng-readonly="true" class="form-control" type="text" ng-model="dataItem.fullName" style="width: 100%"/>`;
            }
        },
        {
            field: "quotaERD",
            title: "Quota ERD",
            width: "100px",
            // editable: function(e) {
            //     return false;
            // }
            template: function (dataItem) {
                return `<input name="others" ng-readonly="true" class="form-control" type="text" ng-model="dataItem.quotaERD" style="width: 100%"/>`;
            }
        },
        {
            field: "exchangeDate",
            //title: "Shift Exchange Date",
            headerTemplate: $translate.instant('SHIFT_EXCHANGE_APPLICATION_DATE'),
            width: "200px",
            format: "{0:dd/MM/yyyy}",
            // editor: shiftExchangeDateEditor
            template: function (dataItem) {
                return `<input kendo-date-picker
                    class = "form-control"
                    k-ng-model="dataItem.exchangeDate"
                    k-date-input="true"
                    k-ng-readonly="true""
                    k-format="'dd/MM/yyyy'"
                    k-on-change="onChangeExchangeDate(dataItem)"
                    style="width: 100%;" />`;
            }
        },
        {
            field: "currentShift",
            //title: "Current Shift",
            headerTemplate: $translate.instant('SHIFT_EXCHANGE_APPLICATION_CURRENT_SHIFT'),
            width: "200px",
            // editor: shiftExchangeDropDownEditor,
            template: function (dataItem) {
                return `<select kendo-drop-down-list style="width: 100%;"
                    id="currentShiftSelect${dataItem.no}"
                    data-k-ng-model="dataItem.currentShift"
                    k-ng-readonly="true"
                    k-data-text-field="'showtextCode'"
                    k-data-value-field="'code'"
                    k-template="'<span><label>#: data.code# - #: data.name# (#:data.nameVN#)</label></span>'",
                    valueTemplate="'#: code #'",
                    k-auto-bind="'false'"
                    k-value-primitive="'false'"
                    k-data-source="newShiftExchangeDataSource"
                    k-filter="'contains'",
                    k-customFilterFields="['code', 'name']",
                    k-filtering="'filterMultiField'",
                    k-ng-disabled="true"
                    > </select>`;
            }
        },
        {
            field: "newShift",
            //title: "New Shift",
            headerTemplate: $translate.instant('SHIFT_EXCHANGE_APPLICATION_NEW_SHIFT'),
            width: "200px",
            // editor: shiftExchangeDropDownEditor,
            template: function (dataItem) {
                //return findValue(item.shiftCode, $scope.shiftCodes, "code");
                return `<select kendo-drop-down-list style="width: 100%;"
                    data-k-ng-model="dataItem.newShift"
                    k-data-text-field="'showtextCode'"
                    k-ng-readonly="true"
                    k-data-value-field="'code'"
                    k-template="'<span><label>#: data.code# - #: data.name# (#:data.nameVN#)</label></span>'",
                    k-valueTemplate="'#: code #'",
                    k-auto-bind="'false'"
                    k-value-primitive="'false'"
                    k-data-source="newShiftExchangeDataSource"
                    k-filter="'contains'",
                    k-customFilterFields="['code', 'name']",
                    k-filtering="'filterMultiField'",
                    k-on-change="onChangeNewShift()"
                    > </select>`;
            }
        },
        {
            field: "reason",
            //title: "Reason",
            headerTemplate: $translate.instant('COMMON_REASON'),
            width: "200px",
            //editor: reasonDropDownEditor,
            template: function (dataItem) {
                return `<select kendo-drop-down-list id="reason_Id" style="width: 100%;"
                    data-k-ng-model="dataItem.reason"
                    k-data-text-field="'name'"
                    k-data-value-field="'code'"
                    k-ng-readonly="true"
                    k-template="'<span><label>#: data.name# </label></span>'",
                    k-valueTemplate="'#: name #'",
                    k-auto-bind="'true'"
                    k-on-change="changeReason(dataItem)"
                    k-value-primitive="'false'"
                    k-data-source="reasonsDataSource"
                    k-filter="'contains'",
                    > </select>`;
            }
        },
        {
            field: "otherReason",
            //title: "Other Reason",
            headerTemplate: $translate.instant('COMMON_OTHER_REASON'),
            width: "200px",
            template: function (dataItem) {
                return `<input name="otherReason"  ng-readonly="true" class="form-control" type="text" ng-model="dataItem.otherReason" style="width: 100%"/>`;
            }
        },
        {
            field: "isERD",
            headerTemplate: "ERD",
            width: "100px",
            template: function (dataItem) {
                return `<input type="checkbox" ng-model="dataItem.isERD" name="isERD{{dataItem.no}}"
                id="isERD{{dataItem.no}}" class="form-check-input" style="margin-left: 10px; vertical-align: middle;"/>
                <label class="form-check-label" for="isERD{{dataItem.no}}"></label>`;
                   
            }
        }
        ]
    };

    $scope.sapCodeDataSource = [];
    function mapGridForItem(dataSource) {
        var count = 0;
        //$scope.form.exchangingShiftItems = new kendo.data.ObservableArray([]);
        let arrayTemporary = []
        dataSource.forEach(item => {
            var simpleItem = _.find($scope.shiftExchangeSimpleDatas, x => { return x.id == item.id });
            let sapCode = simpleItem ? simpleItem.sapCode : '';
            let fullName = simpleItem ? simpleItem.fullName : '';
            item['no'] = count + 1;
            count = item['no'];
            item['fullName'] = fullName ? fullName : item.userFullName;
            item['exchangeDate'] = new Date(item.shiftExchangeDate);
            item['currentShift'] = item.currentShiftCode;
            item['newShift'] = item.newShiftCode;
            item['reason'] = item.reasonCode;
            item['otherReason'] = item.otherReason;
            if (absenceQuotas) {
                var resultQuotaERDSapCode = absenceQuotas.find(x => x.employeeCode == item.employeeCode && x.year == item.exchangeDate.getFullYear() && x.absenceQuotaType == commonData.typeAvailableLeaveBalance.ERD);
                if (resultQuotaERDSapCode) {
                    item['quotaERD'] = resultQuotaERDSapCode.remain;
                }
                else {
                    item['quotaERD'] = 0;
                }
            }
            else {
                item['quotaERD'] = 0;
            }
            arrayTemporary.push(item);
        });
        $scope.form.exchangingShiftItems = new kendo.data.ObservableArray(arrayTemporary);
        //$scope.form.exchangingShiftItems = _.orderBy($scope.form.exchangingShiftItems, ['shiftExchangeDate'], ['asc']);
        initShiftExchangeEditListGrid($scope.form.exchangingShiftItems);
    }

    function initShiftExchangeEditListGrid(dataSource) {
        let grid = $("#shiftExchangeEditListGrid").data("kendoGrid");
        let dataSourceShiftExchange = new kendo.data.DataSource({
            data: dataSource,
        });
        if (grid) {
            grid.setDataSource(dataSourceShiftExchange);
        }
        if (!$scope.perm) {
            disableElement(true);
        }

    }

    $scope.form = {
        applyDate: null,
        exchangingShiftItems: new kendo.data.ObservableArray([
        ]),
        dept_Line: '',
        dept_LineName: '',
        dept_Division: '',
        id: '',
    };

    async function getReasons() {
        var res = await settingService.getInstance().cabs.getAllReason({
            nameType: commonData.reasonType.SHIFT_EXCHANGE_REASON
        }).$promise;
        if (res.isSuccess) {
            res.object.data.forEach(element => {
                $scope.reasonsDataSource.push(element);
            });
        }
    }


    async function getDepartmentByUserId(userId, deptId, divisionId) {
        $scope.deptLineList = [];
        $scope.dept_Line = deptId;
        let res = await settingService.getInstance().departments.getDepartmentsByUserId({ id: userId }, null).$promise;
        if (res && res.isSuccess) {
            $scope.deptLineList = res.object.data;
            var deptLines = _.map($scope.deptLineList, x => { return x['deptLine'] });
            var dropdownlist = $("#deptLineOptionId").data("kendoDropDownList");
            if (dropdownlist)
                dropdownlist.setDataSource(deptLines);
            if (deptId) {
                if (dropdownlist) {
                    dropdownlist.value(deptId);
                }
            }
            await getDivisionsByDeptLine(deptId, divisionId);
        }
    }

    async function getDivisionsByDeptLine(deptId, divisionId = '') {
        var currentItem = _.find($scope.deptLineList, x => { return x.deptLine.id == deptId });
        var dropdownlist = $("#deptDivision").data("kendoDropDownTree");
        if (dropdownlist) {
            if (currentItem && currentItem.divisions && currentItem.divisions.length) {
                var ids = _.map(currentItem.divisions, 'id');
                await getDivisionTreeByIds(ids);
            } else {
                await getDivisionTreeByIds([deptId]);
            }
            if (divisionId) {
                dropdownlist.value(divisionId);
                $scope.form.dept_Division = divisionId;
            }
        }

    }
    this.$onInit = function () {
        $scope.deptLineOption = {
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
                $scope.errorsTwo = {};
                if (deptId) {
                    $scope.dept_Line = deptId;
                    $scope.previousDeptId = deptId;
                    $scope.form.exchangingShiftItems = [];
                    $scope.employeesDataSource = [];
                    clearGrid('shiftExchangeEditListGrid');
                    clearSearchTextOnDropdownTree('deptDivision');

                    var currentItem = _.find($scope.deptLineList, x => { return x.deptLine.id == deptId });
                    if (!currentItem.divisions.length) { // User là member của DeptLine                      
                        await $scope.getChildDivison();
                        await getUserCheckedHeadCount($scope.dept_Line);
                    } else {
                        var ids = _.map(currentItem.divisions, 'id');
                        await getDivisionTreeByIds(ids);
                    }
                }
            }
        };
    }
    async function ngOnInit() {
        await getShiftCodes();
        await getNewShiftCodes();
        await getReasons();
        if ($stateParams.id) {
            await getShiftExchangeById($stateParams.id);
        }
    }
    ngOnInit();
});