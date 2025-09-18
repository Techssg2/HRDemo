
var ssgApp = angular.module('ssg.leaveManagementModule', ["kendo.directives"]);
ssgApp.controller('leaveManagementController', function ($rootScope, $scope, $location, appSetting, localStorageService, $stateParams, $state, moment, Notification, settingService, $timeout, cbService, masterDataService, workflowService, ssgexService, fileService, commonData, $translate, attachmentService, attachmentFile) {

    var ssg = this;
    $scope.title = '';
    $scope.advancedSearchMode = false;
    $scope.currentMonth = new Date().getMonth();
    isItem = true;
    //$scope.title = $stateParams.id ? "LEAVE MANAGEMENT: " + $stateParams.referenceValue : $stateParams.action.title;
    $scope.title = $stateParams.id ? $translate.instant('LEAVE_MANAGEMENT_EDIT_TITLE') + $stateParams.referenceValue : $state.current.name == 'home.leavesManagement.item' ? $translate.instant('LEAVE_MANAGEMENT_NEW_TITLE') : $state.current.name == 'home.leavesManagement.myRequests' ? $translate.instant('LEAVE_MANAGEMENT_MY_REQUETS') : $translate.instant('LEAVE_MANAGEMENT_ALL_REQUETS');

    $scope.searchForm = {
        keyword: '',
    }
    $rootScope.isParentMenu = false;
    $scope.workflowInstances = [];
    var requiredFields = [{
        fieldName: 'sapCode',
        title: "SAP Code"
    },
    {
        fieldName: 'leaveKindCode',
        title: 'Leave Kind'
    },
    {
        fieldName: 'reason',
        title: 'Reason'
    },
    ];
    var mode = $stateParams.id ? 'Edit' : 'New';

    $scope.leaveApplicationData = new kendo.data.ObservableArray([]);
    $scope.thisYear = moment().year();
    $scope.shiftCodes = [];
    var leaveBalances = [];
    var leaveBalances_PrevYear  = [];
    var allDepartments = JSON.parse(sessionStorage.getItemWithSafe("departments"));
    $scope.query = {
        keyword: '',
        department: '',
        fromDate: '',
        toDate: '',
        status: [],
        createdDate: "",
    }
    var halfDateCheck = true;

    $scope.resignationInProgress = null;
    this.$onInit = async () => {
        await GetLeaveKind();
        if (mode == 'New') {
            $timeout(function () {
                $scope.model = _.cloneDeep($rootScope.currentUser);
                $scope.model.id = '';
                $scope.model.userSAPCode = $rootScope.currentUser.sapCode;
                $scope.model.createdByFullName = $rootScope.currentUser.fullName;
                $scope.model.startingDate = $rootScope.currentUser.startDate ? $rootScope.currentUser.startDate : null;
                // $scope.model.deptName = $scope.model.deptName && $scope.model.jobGradeCode ? $scope.model.deptName + "(" + $scope.model.jobGradeCode + ")" : $scope.model.deptName ? $scope.model.deptName : '';
                // $scope.model.divisionName = $scope.model.divisionName && $scope.model.jobGradeCode ? $scope.model.divisionName + "(" + $scope.model.jobGradeCode + ")" : $scope.model.divisionName ? $scope.model.divisionName : '';
                $scope.model.deptName = $scope.model.deptName && $scope.model.jobGradeCode ? $scope.model.deptName : $scope.model.deptName ? $scope.model.deptName : '';
                $scope.model.divisionName = $scope.model.divisionName && $scope.model.jobGradeCode ? $scope.model.divisionName : $scope.model.divisionName ? $scope.model.divisionName : '';
                //$scope.model.userId = $rootScope.currentUser.id;
            }, 0);
        } else {
            await getLeaveApplicationById($stateParams.id);
        }
        if ($state.current.name === 'home.leavesManagement.item') {
            await fetApplicantDetailInfo($stateParams.id);
            await getLeaveBalance();
            
            if ($scope.currentMonth == 0) {
                await getLeaveBalance_PrevYear();
            }
            if ($stateParams.id) {
                checkInProgressResignationApplicantion($scope.leaveApplicantDetails);
            }
        }

    };


    async function checkInProgressResignationApplicantion(leaveManagementDetails) {
        let errors = [];
        $scope.warning = '';
        $scope.resignationInProgress = false;
        let userSapCode = '';
        if (mode == 'New') {
            userSapCode = $rootScope.currentUser.sapCode;
        } else {
            userSapCode = $scope.model.employeeCode;
        }
        if (_.findIndex(leaveManagementDetails, x => {
            return x.leaveCode == 'NPL'
                || x.leaveCode == 'NPH1'
                || x.leaveCode == 'NPH2'
                || x.leaveCode == 'NPA'
                || x.leaveCode == 'NPA1'
                || x.leaveCode == 'NPA2'
                || x.leaveCode == 'NPLP'
        }) > -1) {
            //var res = await cbService.getInstance().resignationApplication.checkInProgressResignationApplicantion({ userSapCode: userSapCode }).$promise;
            var res = await cbService.getInstance().resignationApplication.checkInProgressResignationWithIsActive({ userSapCode: userSapCode }).$promise;
            if (res.isSuccess) {
                $scope.resignationInProgress = res.object;
                $timeout(function () {
                    if ($scope.resignationInProgress) {
                        if (mode == 'New') {
                            // errors.push({ controlName: $translate.instant('LEAVE_MANAGEMENT_RESIGNATION_WARNING_CREATOR') });
                            $scope.warning = $translate.instant('LEAVE_MANAGEMENT_RESIGNATION_WARNING_CREATOR');

                        } else {
                            if ($rootScope.currentUser.id == $scope.model.createdById) {
                                // errors.push({ controlName: $translate.instant('LEAVE_MANAGEMENT_RESIGNATION_WARNING_CREATOR') });
                                $scope.warning = $translate.instant('LEAVE_MANAGEMENT_RESIGNATION_WARNING_CREATOR');
                            }
                            else {
                                //errors.push({ controlName: $scope.model.userNameCreated + ' ' + $translate.instant('LEAVE_MANAGEMENT_RESIGNATION_WARNING_APPROVER') });
                                $scope.warning = $scope.model.userNameCreated + ' ' + $translate.instant('LEAVE_MANAGEMENT_RESIGNATION_WARNING_APPROVER');
                            }

                        }
                    }
                }, 0);

            }
        }
        return errors;
    }
    // async function getAllLeaveManagementByUserId() {
    //     let userId = $rootScope.currentUser.id;
    //     var res = await cbService.getInstance().resignationApplication.getAllLeaveManagementByUserId({ id:$scope.model.userId }).$promise;
    //     if (res.isSuccess) {
    //         $scope.leaveManagementToChecks = res.object.data;
    //     }
    // }

    async function getLeaveApplicationById(id) {
        if (id) {
            var res = await cbService.getInstance().leaveManagement.getLeaveApplicationById({
                id: id,
            }).$promise;
            if (res.isSuccess) {
                $scope.model = _.cloneDeep(res.object);
                $scope.model.moduleTitle = 'Leave Management';
                try {
                    $scope.oldAttachments = JSON.parse(res.object.documents);
                } catch (e) {
                }
            }
        }
    }
    async function getLeaveBalance() {
        let userSapCode = '';
        if (mode == 'New') {
            userSapCode = $rootScope.currentUser.sapCode;
        } else {
            userSapCode = $scope.model.employeeCode;
        }
        if (userSapCode) {
            var res = await ssgexService.getInstance().remoteDatas.getLeaveBalanceSet({ sapCode: userSapCode }, null).$promise;
            if (res.isSuccess && res.object) {
                leaveBalances = res.object;
            } else {
                leaveBalances = [];
            }
            setLeaveBalance(leaveBalances);
        }
    }


    async function getLeaveBalance_PrevYear() {
        let userSapCode = '';
        if (mode == 'New') {
            userSapCode = $rootScope.currentUser.sapCode;
        } else {
            userSapCode = $scope.model.employeeCode;
        }
        if (userSapCode) {
            var res = await ssgexService.getInstance().remoteDatas.getLeaveBalanceSet({
                sapCode: userSapCode ,
                year: new Date().getFullYear() - 1
            }, null).$promise;
            if (res.isSuccess && res.object) {
                leaveBalances_PrevYear = res.object;
            } else {
                leaveBalances_PrevYear = [];
            }
            setLeaveBalance_PrevYear(leaveBalances_PrevYear);
        }
    }

    function setLeaveBalance(data) {
        let grid = $("#gridLeaveTitle").data("kendoGrid");
        let dataSourceLeaveManagement = new kendo.data.DataSource({
            data: data,
        });
        grid.setDataSource(dataSourceLeaveManagement);
    }
    function setLeaveBalance_PrevYear(data) {
        let grid = $("#gridLeaveTitle_PrevYear").data("kendoGrid");
        let dataSourceLeaveManagement = new kendo.data.DataSource({
            data: data,
        });
        grid.setDataSource(dataSourceLeaveManagement);
    }
    async function GetLeaveKind() {
        var res = await masterDataService.getInstance().masterData.GetMasterDataInfo({
            name: "LeaveKind",
            parentCode: '',
        }).$promise;
        if (res.isSuccess) {
            $scope.shiftCodes = res.object.data
                .filter(function (i) { return this.indexOf(i.code) < 0; }, ['CE', 'CEH1', 'CEH1', 'TRN', 'TRN1', 'TRN2'])
                .map(function (item) {
                    return { ...item, showtext: `${item.code}-${item.name} (${item.nameVN})` }
                });
        }
    }
    async function BindDataLeaveKindToDataSource(option) {
        var res = await masterDataService.getInstance().masterData.GetMasterDataInfo({
            name: "LeaveKind",
            parentCode: '',
        }).$promise;
        if (res.isSuccess) {
            option.success(res.object.data)
        }
    }
    $scope.createdByUser = {};
    async function getLeaveApplicationDetails(option, id) {
        if (id) {
            var res = await cbService.getInstance().leaveManagement.getLeaveApplicantDetail({
                id: id,
            }).$promise;
            if (res.isSuccess) {

                var data = res.object.data.map(function (item) {
                    item.fromDate = new Date(item.fromDate);
                    item.toDate = new Date(item.toDate);
                    var leaveKind = { code: item.leaveCode, name: item.leaveName };
                    return { ...item, leaveKind: leaveKind }
                });
                $scope.leaveApplicantDetails = data;
                $scope.model.leaveApplicantDetails = data;
                option.success(data)
            } else {
                option.success([]);
            }
        } else {
            option.success([{
                no: $scope.leaveApplicationData.length + 1,
                fromDate: null,
                toDate: null,
                leaveKind: '',
                quantity: '',
                reason: ''
            }]);
        }

    }
    async function getListDepartmentTree() {
        var res = await settingService.getInstance().departments.getDepartmentTree().$promise;
        if (res.isSuccess) {
            $scope.widget.department.setDataSource(new kendo.data.HierarchicalDataSource({
                data: res.object.data,
            }));
        }
    }

    $scope.listDepartment = [];
    async function getListDepartment() {
        var res = await settingService.getInstance().departments.getDepartments({
            predicate: "",
            predicateParameters: [],
            Order: "Created desc",
            Limit: 10000,
            Page: 1
        }).$promise;
        if (res.isSuccess) {
            $scope.listDepartment = res.object.data;
        }
    }

    // Grid bên phải UI
    $scope.leaveTitleGridOptions = {
        dataSource: {
            // transport: {
            //     read: async function (e) {
            //         await getLeaveBalance(e);
            //     }
            // },
            data: leaveBalances
        },
        sortable: false,
        autoBind: true,
        valuePrimitive: false,
        columns: [{
            field: "absenceQuotaName",
            //title: "Leave Kind",
            headerTemplate: $translate.instant('LEAVE_MANAGEMENT_LEAVE_KIND') + ($scope.currentMonth == 0 ? " - " + (new Date().getFullYear()) : "") ,
            width: "80px"
        }, {
            field: "currentYearBalance",
            //title: 'Leave Quota',
            headerTemplate: $translate.instant('LEAVE_MANAGEMENT_LEAVE_QUOTA'),
            width: "80px"
        }, {
            field: "newRemain",
            //title: 'Leave Remains',
            headerTemplate: $translate.instant('LEAVE_MANAGEMENT_LEAVE_REMAIN'),
            width: "80px"
        }]
    }

    // Grid bên phải UI
    $scope.leaveTitleGridOptions_PrevYear = {
        dataSource: {
            // transport: {
            //     read: async function (e) {
            //         await getLeaveBalance(e);
            //     }
            // },
            data: leaveBalances_PrevYear
        },
        sortable: false,
        autoBind: true,
        valuePrimitive: false,
        columns: [{
            field: "absenceQuotaName",
            //title: "Leave Kind",
            headerTemplate: $translate.instant('LEAVE_MANAGEMENT_LEAVE_KIND') + ($scope.currentMonth == 0 ? " - " + (new Date().getFullYear() - 1) :""),
            width: "80px"
        }, {
            field: "currentYearBalance",
            //title: 'Leave Quota',
            headerTemplate: $translate.instant('LEAVE_MANAGEMENT_LEAVE_QUOTA'),
            width: "80px"
        }, {
            field: "newRemain",
            //title: 'Leave Remains',
            headerTemplate: $translate.instant('LEAVE_MANAGEMENT_LEAVE_REMAIN'),
            width: "80px"
        }]
    }

    //Grid phía cuối cùng
    function fetApplicantDetailInfo(id) {
        $scope.leaveApplicationDetailGridOptions = {
            dataSource: {
                serverPaging: false,
                transport: {
                    read: async function (e) {
                        await getLeaveApplicationDetails(e, id);
                    }
                },
            },
            sortable: false,
            pageable: false,
            columns: [{
                field: "fromDate",
                //title: "From Date",
                headerTemplate: $translate.instant('LEAVE_MANAGEMENT_FROM_DATE'),
                width: "80px",
                template: function (dataItem) {
                    return `<input style="width: 100%;" kendo-date-picker id="fromDate${dataItem.no}" name="fromDate${dataItem.no}"
                    k-ng-model="dataItem.fromDate"
                                k-format="'dd/MM/yyyy'"
                                k-on-change="onchangeShiftCode(kendoEvent, dataItem)"                             
                                k-date-input="true" />`;
                }
            },
            {
                field: "toDate",
                //title: "To Date",
                headerTemplate: $translate.instant('LEAVE_MANAGEMENT_TO_DATE'),
                width: "80px",
                template: function (dataItem) {
                    return `<input style="width: 100%;" kendo-date-picker id="toDate${dataItem.no}" name="toDate${dataItem.no}"
                    k-ng-model="dataItem.toDate"
                                k-format="'dd/MM/yyyy'"       
                                k-on-change="onchangeShiftCode(kendoEvent, dataItem)"                      
                                k-date-input="true" />`;
                }
            },
            {
                field: "leaveKind",
                //title: "Leave Kind",
                headerTemplate: $translate.instant('LEAVE_MANAGEMENT_LEAVE_KIND'),
                width: "180px",
                template: function (dataItem) {
                    return `<select kendo-drop-down-list style="width: 100%;"
                data-k-ng-model="dataItem.leaveKind"
                k-data-text-field="'showtext'"
                k-data-value-field="'code'"
                k-template="'<span><label>#: data.code# - #: data.name# (#:data.nameVN#) </label></span>'",
                k-valueTemplate="'#: code #'",
                k-auto-bind="'true'"           
                k-data-source="shiftCodes"
                k-filter="'contains'",
                k-customFilterFields="['code', 'name']",
                k-filtering="'filterMultiField'"   
                k-on-change="onchangeShiftCode(kendoEvent, dataItem)"       
                > </select>`;
                }
            },
            {
                field: "quantity",
                //title: 'Quantity',
                headerTemplate: $translate.instant('COMMON_QUANTITY'),
                width: "50px",
                editable: "inline",
                template: function (dataItem) {
                    return `<input style="background: #eee; border: none" class="k-input w100" ng-model="dataItem.quantity" readonly/>`;
                }
            }, {
                field: "reason",
                //title: 'Reason',
                headerTemplate: $translate.instant('COMMON_REASON'),
                width: "120px",
                template: function (dataItem) {
                    return `<input class="k-textbox w100" id="reason${dataItem.no}" name="reason${dataItem.no}" ng-model="dataItem.reason"/>`;
                }
            },
            {
                //title: "Actions",
                headerTemplate: $translate.instant('COMMON_ACTION'),
                width: "70px",
                headerAttributes: {
                    class: "bg-table-head"
                },
                template: function (dataItem) {
                    if ($scope.model.status == "Draft" || !dataItem.id) {
                        return `<a class="btn btn-sm default red-stripe" ng-click="deleteRecord(dataItem)">{{'COMMON_BUTTON_DELETE'|translate}}</a>`;
                    }
                    else {
                        return ``;
                    }
                }
            }
            ],
            selectable: true
        };
    }

    confirm = function (e) {
        if (e.data && e.data.value) {
            let resetId = 1;

            let currentDataOnGrid = $("#leaveApplicationDetailGrid").data("kendoGrid").dataSource._data;
            var index = _.findIndex(currentDataOnGrid, x => {
                if (x.id) {
                    return x.id == $scope.itemDetailId
                } else if (x.uid) {
                    return x.uid == $scope.itemDetailId
                }
            });
            currentDataOnGrid.splice(index, 1);
            // currentDataOnGrid.pop(index);
            // Notification.success("Data Sucessfully Deleted");
            Notification.success($translate.instant('COMMON_DELETE_SUCCESS'));
        }
    }

    function initGridLeason(dataSource) {
        let grid = $("#leaveApplicationDetailGrid").data("kendoGrid");
        let dataSourceLeaveManagement = new kendo.data.DataSource({
            data: dataSource,
        });
        grid.setDataSource(dataSourceLeaveManagement);
    }

    $scope.deleteRecord = function (dataItem) {
        if (dataItem.id) {
            $scope.itemDetailId = dataItem.id;
        } else if (dataItem.uid) {
            $scope.itemDetailId = dataItem.uid;
        }
        $scope.dialog = $rootScope.showConfirmDelete($translate.instant('COMMON_BUTTON_DELETE'), $translate.instant('COMMON_DELETE_VALIDATE'), $translate.instant('COMMON_BUTTON_CONFIRM'));
        $scope.dialog.bind("close", confirm);
    };

    $scope.onchangeShiftCode = function (e, dataItem) {
        if (dataItem.toDate < dataItem.fromDate) {
            dataItem.toDate = dataItem.fromDate;
            // Notification.error('From Date must be less than To Date');
            //Notification.error($translate.instant('COMMON_FROMDATE_LESS_TODATE'));

        }
        var start = moment(dataItem.fromDate, "YYYY-MM-DD");
        var end = moment(dataItem.toDate, "YYYY-MM-DD");
        let interval = Math.abs(moment.duration(start.diff(end)).asDays()) + 1;
        var quantiesForHafts = [
            "ERD1",
            "ERD2",
            "DOH1",
            "DOH2",
            "ALH1",
            "ALH2",
            "NPH1",
            "NPH2",
            "TRN1",
            "TRN2",
            "RAC1",
            "RAC2"
        ]
        if (dataItem.leaveKind) {
            var itemIndex = _.findIndex(quantiesForHafts, x => { return x == dataItem.leaveKind.code });
            if (itemIndex > -1) {
                dataItem.quantity = 0.5 * interval;
            } else {
                dataItem.quantity = interval;
            }
        }
        refreshGrid();

    }

    function getQuantityFromLeaveCode(code) {
        let quantity = 0;
        var quantiesForHafts = [
            "ERD1",
            "ERD2",
            "DOH1",
            "DOH2",
            "ALH1",
            "ALH2",
            "NPH1",
            "NPH2",
            "TRN1",
            "TRN2",
            "RAC1",
            "RAC2"
        ]
        if (code) {
            var itemIndex = _.findIndex(quantiesForHafts, x => { return x == code });
            if (itemIndex > -1) {
                quantity = 0.5;
            } else {
                quantity = 1;
            }
        }
        return quantity;
    }

    // add more
    $scope.addRow = async function (item) {
        let currentLeaveGrid = $("#leaveApplicationDetailGrid").data("kendoGrid");
        let nValue = {
            no: currentLeaveGrid._data.length + 1,
            fromDate: null,
            toDate: null,
            leaveKind: '',
            quantity: '',
            reason: ''
        }
        $scope.leaveApplicationData.push({
            nValue
        });

        let grid = $("#leaveApplicationDetailGrid").data("kendoGrid");
        grid.dataSource.add(nValue);
    };

    // data dropdown leave kind in leaveApplication detail grid
    $scope.leaveKindsDataSource = {
        dataTextField: 'name',
        dataValueField: 'code',
        autoBind: false,
        valuePrimitive: false,
        filter: "contains",
        filtering: $rootScope.dropdownFilter,
        dataSource: {
            serverPaging: false,
            transport: {
                read: async function (e) {
                    await BindDataLeaveKindToDataSource(e);
                }
            },
        }
    };
    $scope.departmentOptions = {
        placeholder: "",
        dataTextField: "name",
        dataValueField: "code",
        //template: '#: item.code # - #: item.name #',
        template: showCustomDepartmentTitle,
        //valueTemplate: '#: item.id #',
        valuePrimitive: true,
        checkboxes: false,
        autoBind: true,
        filter: "contains",
        filtering: async function (option) {
            await getDepartmentByFilter(option);
        },
        loadOnDemand: true,
        valueTemplate: (e) => showCustomField(e, ['name']),
        change: function (e) {
            if (!e.sender.value()) {
                clearSearchTextOnDropdownTree('departmentId');
                setDataDepartment(allDepartments);
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

    $scope.toggleFilterPanel = function (value) {
        $scope.advancedSearchMode = value;
        if (value) {
            setDataDepartment(allDepartments);
            setLeaveKind($scope.shiftCodes);
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

    let columsToSearchOnKeyword = [
        'ReferenceNumber.contains(@0)',
        'userSAPCode.contains(@1)',
        'createdByFullName.contains(@2)',
        'deptName.contains(@3)',
        'divisionName.contains(@4)',
        'status.contains(@5)',
    ];

    async function getListLeaveApplication(option) {
        $scope.currentQuery = buildArgForSearch(appSetting.pageSizeDefault, $scope.query, columsToSearchOnKeyword);

        if (option) {
            $scope.currentQuery.limit = option.data.take;
            $scope.currentQuery.page = option.data.page;
        }

        // if ($state.current.name == commonData.myRequests.LeaveManagement) {
        //     $scope.currentQuery.predicate = 'CreatedById = @0';
        //     $scope.currentQuery.predicateParameters.push($rootScope.currentUser.id);
        // }

        if ($state.current.name == commonData.leaveManagement.myRequests) {
            $scope.currentQuery.predicate = ($scope.currentQuery.predicate ? $scope.currentQuery.predicate + ' and ' : $scope.currentQuery.predicate) + `CreatedById = @${$scope.currentQuery.predicateParameters.length}`;
            $scope.currentQuery.predicateParameters.push($rootScope.currentUser.id);
        }

        var res = await cbService.getInstance().leaveManagement.getListLeaveApplication({},
            $scope.currentQuery
        ).$promise;
        if (res.isSuccess) {
            $scope.data = [];
            var n = 1;
            res.object.data.forEach(element => {
                element.no = n++;
                $scope.data.push(element);
            });;
        }
        $scope.total = res.object.count;
        if (option) {
            option.success($scope.data);
        } else {
            $scope.widget.leaveManagementGrid.dataSource.read();
        }
    }


    function createPredicateMuti(predicate, predicateParams) {

        if (!predicateParams.length)
            return '';

        let predicateResultArrray = [];
        predicateParams.forEach(param => {
            predicateResultArrray.push(predicate.replace('[value]', `"${param}"`));
        });

        return `(${predicateResultArrray.join(' || ')})`;
    }

    $scope.leaveManagementGridOptions = {
        dataSource: {
            serverPaging: true,
            pageSize: 20,
            transport: {
                read: async function (e) {
                    await getListLeaveApplication(e);
                }
            },
            schema: {
                total: () => { return $scope.total },
                data: () => { return $scope.data }
            }
        },
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
            field: "status",
            //title: "Status",
            headerTemplate: $translate.instant('COMMON_STATUS'),
            width: "350px",
            locked: true,
            template: function (data) {
                var statusTranslate = $rootScope.getStatusTranslate(data.status);
                return `<workflow-status status="${statusTranslate}"></workflow-status>`;
                //return `<workflow-status status="${data.status}"></workflow-status>`;
            }
        },
        {
            field: "referenceNumber",
            //title: "Reference Number",
            headerTemplate: $translate.instant('COMMON_REFERENCE_NUMBER'),
            width: "180px",
            locked: true,
            template: function (data) {
                return `<a ui-sref="home.leavesManagement.item({referenceValue: '${data.referenceNumber}', id: '${data.id}'})" ui-sref-opts="{ reload: true }">${data.referenceNumber}</a>`;
            }
        },
        {
            field: "userSAPCode",
            //title: "Sap Code",
            headerTemplate: $translate.instant('COMMON_SAP_CODE'),
            width: "150px",
            // template: function(data) {
            //     return `<a ui-sref="home.leavesManagement.item({referenceValue: ${data.referenceNumber}})" ui-sref-opts="{ reload: true }">${data.userSAPCode}</a>`;
            // }
        },
        {
            field: "createdByFullName",
            //title: "Full Name",
            headerTemplate: $translate.instant('COMMON_FULL_NAME'),
            width: "200px",
        },
        {
            field: "deptName",
            //title: "Dept/ Line",
            headerTemplate: $translate.instant('COMMON_DEPT_LINE'),
            sortable: false,
            width: "350px"
        },
        {
            field: "divisionName",
            //title: "Division/ Group",
            headerTemplate: $translate.instant('COMMON_DIVISION_GROUP'),
            sortable: false,
            width: "350px"
        },
        {
            field: "created",
            //title: "Created Date",
            headerTemplate: $translate.instant('COMMON_CREATED_DATE'),
            width: "200px",
            template: function (dataItem) {
                return moment(dataItem.created).format(appSetting.longDateFormat);
            }
        }
        ]
    };
    var gridLeaveManagementData = [];
    $scope.temporyDataGrid = []
    async function checkValidLeaveKind() {
        var errors = [];
        let userSapCode = '';
        let id = null;
        if (mode == 'New') {
            userSapCode = $rootScope.currentUser.sapCode;
        } else {
            userSapCode = $scope.model.employeeCode;
            id = $scope.model.id;
        }
        var objectToCheck = { leaveDetails: gridLeaveManagementData, myLeaveBalances: leaveBalances.length > 0 ? leaveBalances : [], userSapCode: userSapCode, id: id };
        var res = await cbService.getInstance().leaveManagement.checkValidLeaveKind(objectToCheck).$promise;
        if (res.isSuccess)
            return errors;
        if (res.errorCodes.length) {
            res.messages.forEach(x => {
                errors.push({
                    controlName: `${x}`
                });
            })

        }
        return errors;
    }
    function getDataDetailsOnGrid() {
        gridLeaveManagementData = [];
        let currentDataOnGrid = $("#leaveApplicationDetailGrid").data("kendoGrid").dataSource._data;
        currentDataOnGrid.forEach(x => {
            if (!x.id || !x.isApproved) {
                gridLeaveManagementData.push({ leaveCode: x.leaveKind.code, leaveName: x.leaveKind.name, quantity: x.quantity, fromDate: x.fromDate, toDate: x.toDate, reason: x.reason })
            }
        });
        return _.cloneDeep(gridLeaveManagementData);
    }
    $scope.save = async function (form, perm) {
        $scope.errors = [];
        var result = { isSuccess: false };
        $scope.temporyDataGrid = getDataDetailsOnGrid();
        //dung cho validate
        // var isValidation = false;
        // if (!$scope.model || perm == 'undifined' || perm != 1) {
        //     isValidation = true;
        // } else {
        //     isValidation = false;
        // }
        // if (isValidation) {
        //     $scope.errors = validateForm(gridLeaveManagementData, $scope.properties);
        // }

        //Kiểm tra xem LEAVE MANAGEMENT table đã có dữ liệu nào chưa
        $scope.errors = $scope.validateForm(gridLeaveManagementData, $scope.properties);
        if ($scope.errors.length > 0) {
            return result;
        } else {
            var test = await checkValidLeaveKind();
            $scope.errors = translateCheckValidLeaveKind(test);
            if (!$scope.errors.length) {
                $scope.errors = await checkInProgressResignationApplicantion($scope.temporyDataGrid);
            }
            $scope.$applyAsync()
            if ($scope.errors.length > 0) {
                return result;
            } else {
                var res = await save(form);
                if (res && res.isSuccess) {
                    Notification.success($translate.instant('COMMON_SAVE_SUCCESS'));
                    $scope.model = _.cloneDeep(res.object);
                    try {
                        $scope.oldAttachments = JSON.parse(res.object.documents);
                        $timeout(function () {
                            $scope.attachments = [];
                            $(".k-upload-files.k-reset").find("li").remove();
                        }, 0);
                    } catch (e) {
                    }
                    if ($scope.removeFiles) {
                        await attachmentService.getInstance().attachmentFile.deleteMultiFile($scope.removeFiles).$promise;
                    }
                    if (!$stateParams.id) {
                        $state.go($state.current.name, { id: $scope.model.id, referenceValue: $scope.model.referenceNumber }, { reload: true });
                    }

                    // if (!$scope.model.id && res.object) {
                    //     $scope.model = _.cloneDeep(res.object);
                    //     $state.go($state.current.name, { id: $scope.model.id, referenceValue: $scope.model.referenceNumber }, { reload: true });
                    //     if ($scope.removeFiles) {
                    //         await attachmentService.getInstance().attachmentFile.deleteMultiFile($scope.removeFiles).$promise;
                    //     }
                    // }
                } else {
                    //Jira 632
                    $timeout(function () {
                        if (res.errorCodes && res.errorCodes.length) {
                            for (let i = 0; i < res.errorCodes.length; i++) {
                                if (res.errorCodes[i] == 1) {
                                    $scope.errors.push({ controlName: "Leave application: ", message: "A Leave Application already exists for 'From Date': " + moment(res.object.fromDate).format(appSetting.sortDateFormat) + " under " + res.object.leaveApplicationReferenceNumber });
                                }
                            }
                        }
                        else {
                            Notification.error("Error System");
                        }
                    }, 0);
                    //============
                    
                }
                return res;
            }
            //$scope.errors = test;

        }
    }

    function translateCheckValidLeaveKind(arrayError) {
        let error = [];
        arrayError.forEach(item => {
            if (item.controlName.includes('Leave quantity in a day is not more than 1: ')) {
                var str = item.controlName.replace("Leave quantity in a day is not more than 1: ", " ");
                item.controlName = $translate.instant('LEAVE_MANAGEMENT_LEAVE_ADAY_VALIDATE') + ': ' + str;
                error.push(item);
            }
            if (item.controlName.includes('Leave quantity Exceeds Quota: ')) {
                var str = item.controlName.replace("Leave quantity Exceeds Quota: ", " ");
                item.controlName = $translate.instant('LEAVE_MANAGEMENT_LEAVE_QUOTA_VALIDATE') + ': ' + str;
                error.push(item);
            }
            if (item.controlName.includes('You cannot apply leave in the same half day: ')) {
                var str = item.controlName.replace("You cannot apply leave in the same half day: ", " ");
                item.controlName = $translate.instant('LEAVE_MANAGEMENT_HALFDAY_VALIDATE') + ': ' + str;
                error.push(item);
            }
            if (item.controlName.includes('You have not been granted this leave kind: ')) {
                var str = item.controlName.replace("You have not been granted this leave kind: ", " ");
                item.controlName = $translate.instant('LEAVE_MANAGEMENT_GRANTED_VALIDATE') + ': ' + str;
                error.push(item);
            }

            if (item.controlName.includes('You cannot apply leave in the same day: ')) {
                var str = item.controlName.replace("You cannot apply leave in the same day: ", " ");
                item.controlName = $translate.instant('LEAVE_MANAGEMENT_SAMEDAY_VALIDATE') + ': ' + str;
                error.push(item);
            }
        });
        return error;
    }

    async function save(form) {
        if ($scope.attachments.length || $scope.removeFiles.length) {
            let attachmentFilesJson = await mergeAttachment();
            $scope.model.documents = attachmentFilesJson;
        }
        let data = _.cloneDeep($scope.model);
        var leaveApplicationDataDetails = JSON.stringify($scope.temporyDataGrid);
        data.leaveApplicantDetails = leaveApplicationDataDetails;
        var res;
        if (mode == 'Edit') {
            res = await cbService.getInstance().leaveManagement.updateLeaveApplication(
                data
            ).$promise;
        } else {
            res = await cbService.getInstance().leaveManagement.createNewLeaveApplication(
                data
            ).$promise;
        }
        return res;
    }

    $scope.properties = [{
        property: 'fromDate',
        // title: 'From Date',
        title: $translate.instant('LEAVE_MANAGEMENT_FROM_DATE_REQUIRED'),
    },
    {
        property: 'toDate',
        // title: 'To Date',
        title: $translate.instant('LEAVE_MANAGEMENT_TO_DATE_REQUIRED'),
    },
    {
        property: "leaveCode",
        // title: 'Leave Kind',
        title: $translate.instant('LEAVE_MANAGEMENT_LEAVE_KIND_REQUIRED'),
    },
    {
        property: "reason",
        // title: 'Reason',
        title: $translate.instant('LEAVE_MANAGEMENT_REASON_REQUIRED'),
    }
    ]

    $scope.validateForm = function (arrayLeaveDetails, properties) {
        CheckIsStore($scope);
        let error = [];
        let no = 1;
        // arrayLeaveDetails.forEach(x => {
        //     x['fromDate'] = moment(x.fromDate).format('DD/MM/YYYY');
        //     x['toDate'] = moment(x.toDate).format('DD/MM/YYYY');
        // });
        if (arrayLeaveDetails.length > 0) {
            arrayLeaveDetails.forEach(item => {
                error = error.concat(validationForTable(item, properties, no));
                //error = error.concat(validateDateRange(appSetting.rangeDate, item.fromDate, item.toDate, no, 'Leave Management ',  $translate));

                let errorRanges = validateDateRange($rootScope.salaryDayConfiguration, item.fromDate, item.toDate, no, 'LEAVE_MANAGEMENT_MENU', $translate, 'LEAVE_MANAGEMENT_PERIOD_VALIDATE_1', 'LEAVE_MANAGEMENT_PERIOD_VALIDATE_2')
                errorRanges.forEach(x => {
                    error.push({ controlName: x.controlName, message: ': ' + x.message, isRule: true });
                })
                no = no + 1;
            });
            if (!error.length) {
                //check salary Range 
                error = validateSalaryRangeDate($translate, arrayLeaveDetails, appSetting.salaryRangeDate, 'LEAVE_MANAGEMENT_MENU', 'LEAVE_MANAGEMENT_PERIOD_VALIDATE_2', true);
            }
            if (error.length == 0) {
                //check quantity
                error = error.concat(validationQuantity(arrayLeaveDetails));
                // //check leave kind co trung nhau tren leave date 
                error = error.concat(validationLeaveKind(arrayLeaveDetails));
                //await checkValidLeaveKind();
                // error = error.concat(await checkValidLeaveKind());               
                error = error.concat(validationHaftKind(arrayLeaveDetails));
            }
        } else {
            // error.push({ controlName: 'Table Leave Management', message: ': Field is required' });
            error.push({ controlName: $translate.instant('LEAVE_MANAGEMENT_TABLE_LEAVE'), message: ': ' + $translate.instant('COMMON_FIELD_IS_REQUIRED') });
        }
        return error;
    }

    function validationForTable(model, requiredFields, no) {
        let errors = [];
        requiredFields.forEach(field => {
            if (!model[field.property]) {
                $scope.errors.push({
                    controlName: `${field.title} ${no}: `,
                    message: $translate.instant('COMMON_FIELD_IS_REQUIRED')
                });
            }
        });
        return errors;
    }

    function isLeaveKindDuplicate(arrayModel) {
        // var result = false;
        // var count;
        // for (var i = 0; i < arrayModel.length; i++) {
        //     count = 0;
        //     for (var j = 0; j < arrayModel.length; j++) {
        //         if (arrayModel[i].leaveCode == arrayModel[j].leaveCode) {
        //             count += 1;
        //         }
        //     }
        // }
        // if (count > 1) {
        //     result = true;
        // }
        return result;
    }

    function validationLeaveKind(arrayModel) {
        let errors = [];
        let groupbyLeaveDate = _.groupBy(arrayModel, function (item) {
            return moment(item.fromDate).format('DD/MM/YYYY') + ',' + moment(item.toDate).format('DD/MM/YYYY');
        });
        Object.keys(groupbyLeaveDate).forEach(key => {
            // var result = isLeaveKindDuplicate(groupbyLeaveDate[key]);
            let groupbyLeaveCode = _.groupBy(groupbyLeaveDate[key], 'leaveCode');
            Object.keys(groupbyLeaveCode).forEach(x => {
                if (groupbyLeaveCode[x].length > 1) {
                    errors.push({
                        //controlName: `Dupicate Leave Kind on From Date, To Date : ${key}, ${x}`
                        controlName: $translate.instant('LEAVE_MANAGEMENT_DUPLICATE_VALIDATE') + ` : ${key}, ${x}`
                    });
                }
            });
        });
        return errors;
    }

    function validationHaftKind(arrayModel) {
        let errors = [];
        let minDate = arrayModel[0].fromDate;
        let maxDate = arrayModel[0].toDate;
        let quantityByDates = [];
        arrayModel.forEach(x => {
            if (x.fromDate < minDate) {
                minDate = x.fromDate;
            }
            if (x.toDate > maxDate) {
                maxDate = x.toDate;
            }
            let startDate = x.fromDate;
            while (startDate <= x.toDate) {
                let fromDateToString = moment(startDate).format('DD/MM/YYYY');
                let quantity = getQuantityFromLeaveCode(x.leaveCode);
                quantityByDates.push({ date: fromDateToString, quantity: quantity, leaveCode: x.leaveCode });
                startDate = addDays(startDate, 1);
            }

        });
        var groupByDates = _.groupBy(quantityByDates, 'date');

        Object.keys(groupByDates).forEach(key => {
            let sumQuantityByDateFor1 = _.sumBy(groupByDates[key], function (o) {
                if ((o.quantity == 0.5 && o.leaveCode.includes('1')))
                    return o.quantity;
            });
            let sumQuantityByDateFor2 = _.sumBy(groupByDates[key], function (o) {
                if (o.quantity == 0.5 && o.leaveCode.includes('2')) {
                    return o.quantity;
                }
            });
            if (sumQuantityByDateFor1 >= 1 || sumQuantityByDateFor2 >= 1) {
                // errors.push({ controlName: `You cannot apply leave in the same half day: ${key}` });
                errors.push({ controlName: $translate.instant('LEAVE_MANAGEMENT_HALFDAY_VALIDATE') + `: ${key}` });
            }
        });
        return errors;
    }

    function isQuantitySmallThanOne(arrayModel) {
        let count = 0;
        arrayModel.forEach(item => {
            count += parseFloat(item.quantity);
        });
        // let count = arrayModel.reduce(( accumulator, currentValue ) => accumulator + currentValue.quantity, 0);
        return count <= 1 ? true : false
    }

    function validationQuantity(arrayModel) {
        let errors = [];
        let minDate = arrayModel[0].fromDate;
        let maxDate = arrayModel[0].toDate;
        let quantityByDates = [];
        arrayModel.forEach(x => {
            if (x.fromDate < minDate) {
                minDate = x.fromDate;
            }
            if (x.toDate > maxDate) {
                maxDate = x.toDate;
            }
            let startDate = x.fromDate;
            while (startDate <= x.toDate) {
                let fromDateToString = moment(startDate).format('DD/MM/YYYY');
                let quantity = getQuantityFromLeaveCode(x.leaveCode);
                let existItem = _.find(quantityByDates, item => {
                    return item.date == fromDateToString;
                });
                if (existItem) {
                    existItem.quantity += quantity;
                } else {
                    quantityByDates.push({ date: fromDateToString, quantity: quantity });
                }
                startDate = addDays(startDate, 1);
            }
        });
        var filterQuantityOver1PerDate = _.filter(quantityByDates, x => {
            return x.quantity > 1;
        });
        if (filterQuantityOver1PerDate && filterQuantityOver1PerDate.length) {
            filterQuantityOver1PerDate.forEach(x => {

            });
            // errors.push({ controlName: ` Leave quantity in a day is not more than 1: ${filterQuantityOver1PerDate.map(x => { return x.date }).join(', ')}` });
            errors.push({ controlName: $translate.instant('LEAVE_MANAGEMENT_LEAVE_ADAY_VALIDATE') + ` : ${filterQuantityOver1PerDate.map(x => { return x.date }).join(', ')}` });
        }
        return errors;
    }

    $scope.model = {
        leaveKindCode: '',
    };

    $scope.mDatePicker = {
        fromDateMin: new Date(2000, 0, 1, 0, 0, 0),
        fromDateMax: new Date(2030, 0, 1, 0, 0, 0),
        toDateMin: new Date(2000, 0, 1, 0, 0, 0),
        toDateMax: new Date(2030, 0, 1, 0, 0, 0),
    }

    $scope.mCheckbox = {
        halfDay1: false,
        halfDay2: false,
    };

    $scope.collectionIndex = function (list, value) {
        var index = _.findIndex(list, x => {
            return x === 'Part Time';
        });
        return index;
    };
    $scope.addSubItem = function (list) {
        list.push({})
    }
    $scope.removeSubItem = function (list, $index) {
        list.splice($index, 1);
    }
    $scope.errors = [];
    $scope.submit = async function (form) {
        var res = await save(form);
        if (res.isSuccess) {
            var resSubmitWorkflow = await workflowService.getInstance().workflows.startWorkflow({ itemId: res.object.id }, null).$promise;
            if (resSubmitWorkflow && resSubmitWorkflow.isSuccess) {
                Notification.success($translate.instant('COMMON_SAVE_SUCCESS'));
            } else {
                Notification.error("Error");
            }
        }
    }
    $scope.changeData = function () {
        alert($scope.model.workConditions);
    }

    $scope.statusesDataSource = [{
        id: '1',
        name: 'Completed',
        class: 'fa-check-circle font-green-jungle',
    },
    {
        id: '2',
        name: 'Waiting for CMD Checker Approval',
        class: 'fa-circle font-yellow-lemon',
    },
    {
        id: '2',
        name: 'Waiting for HOD Approval',
        class: 'fa-circle font-yellow-lemon',
    },
    {
        id: '3',
        name: 'Rejected',
        class: 'fa-ban font-red',
    },
    ];

    $scope.settingOptions = {
        leaveKindOptions: {
            placeholder: "",
            dataTextField: "name",
            dataValueField: "code",
            template: '#: code # - #: name #',
            valueTemplate: '#: name #',
            valuePrimitive: true,
            checkboxes: false,
            autoBind: true,
            filter: "contains",
            dataSource: {
                serverPaging: false,
                //pageSize: 20,
                transport: {
                    read: async function (e) {
                        var res = await masterDataService.getInstance().masterData.GetMasterDataInfo({
                            name: "LeaveKind",
                            parentCode: '',
                        }).$promise;
                        if (res.isSuccess && res.object.count > 0) {
                            e.success(res.object.data);
                        }
                    }
                },
            },
            change: function (e) {
                $scope.model.leaveKindName = e.sender.text();
            },
            dataBound: function () {
                this.select(-1);
                this.trigger("change");
            }
        },
    }

    $scope.statusOptions = {
        placeholder: "",
        dataTextField: "name",
        dataValueField: "code",
        valuePrimitive: true,
        checkboxes: false,
        autoBind: false,
        filter: "contains",
        dataSource: {
            // serverFiltering: true,
            //data: commonData.itemStatuses
            data: translateStatus($translate, commonData.itemStatuses)
        }
    };

    $scope.leaveKindData = [];
    $scope.leaveKindSearchOptions = {
        dataTextField: "name",
        dataValueField: "code",
        template: '#: code # - #: name # (#:data.nameVN#)',
        tagTemplate: '#: code # - #: name # (#:data.nameVN#)',
        valuePrimitive: true,
        checkboxes: false,
        autoBind: false,
        filter: "contains",
        dataSource: $scope.leaveKindData
    };

    function setLeaveKind(data) {
        var multiselect = $("#leaveKindId").data("kendoMultiSelect");
        let dataSourceLeaveKind = new kendo.data.DataSource({
            data: data,
        });
        multiselect.setDataSource(dataSourceLeaveKind);
    }

    function getStatusClass(name) {
        var found = $scope.statusesDataSource.find(x => x.name === name);
        return found.class;
    }

    $scope.goBackList = function () {
        $state.go('home.leavesManagement.allRequests');
    }

    function caculateLeaveDay() {
        if (!moment($scope.model.fromDate, 'DD/MM/YYYY').isValid() || !moment($scope.model.toDate, 'DD/MM/YYYY').isValid()) {
            $scope.model.leaveDay = '';
            return;
        }

        let from = moment($scope.model.fromDate, 'DD/MM/YYYY');
        let to = moment($scope.model.toDate, 'DD/MM/YYYY');
        if (!from.isValid() || !to.isValid())
            return;

        $scope.model.leaveDay = to.diff(from, 'days') + 1;

        if (moment($scope.model.halfDate, 'DD/MM/YYYY').isValid())
            $scope.model.leaveDay -= 0.5;
    }

    $scope.fromDateChanged = function () {
        if (moment($scope.model.fromDate, 'DD/MM/YYYY').isValid()) {
            $scope.mDatePicker.toDateMin = moment($scope.model.fromDate, 'DD/MM/YYYY').format('MM/DD/YYYY');
        } else {
            $scope.mDatePicker.toDateMin = '01/01/2000';
        }
        //$scope.model.halfDate = '';
        caculateLeaveDay();
        refreshHalfDateOption();
    }

    $scope.toDateChanged = function () {
        if (moment($scope.model.toDate, 'DD/MM/YYYY').isValid()) {
            $scope.mDatePicker.fromDateMax = moment($scope.model.toDate, 'DD/MM/YYYY').format('MM/DD/YYYY');
        } else {
            $scope.mDatePicker.fromDateMax = '12/31/2030';
        }
        //$scope.model.halfDate = '';
        caculateLeaveDay();
        refreshHalfDateOption();
    }

    $scope.halfDateDatepickerChange = function () {
        caculateLeaveDay();
    }

    $scope.halfDateOptions = {};

    function refreshHalfDateOption() {
        if (!moment($scope.model.fromDate, 'DD/MM/YYYY').isValid() || !moment($scope.model.toDate, 'DD/MM/YYYY').isValid()) {
            $scope.halfDateOptions = {
                value: '',
            };
            return;
        }

        let value = $scope.model.halfDate;
        if (moment($scope.model.fromDate, 'DD/MM/YYYY').isAfter(moment($scope.model.halfDate, 'DD/MM/YYYY')) || moment($scope.model.toDate, 'DD/MM/YYYY').isBefore(moment($scope.model.halfDate, 'DD/MM/YYYY'))) {
            value == '';
        }


        $scope.halfDateOptions = {
            value: $scope.model.halfDate,
            disableDates: function (date) {
                // if (!$scope.mCheckbox.halfDay2 || !$scope.model.halfDay2)
                //     return false;

                // let disableDate = new Date(moment($scope.model.halfDay2, 'DD/MM/YYYY').format('MM/DD/YYYY'));
                // if (date && date.getTime() === disableDate.getTime()) {
                //     return true;
                // }
                // else {
                //     return false;
                // }
                return false;
            },
            min: moment($scope.model.fromDate, 'DD/MM/YYYY').format('MM/DD/YYYY'),
            max: moment($scope.model.toDate, 'DD/MM/YYYY').format('MM/DD/YYYY'),
            month: {
                empty: '<span class="k-state-disabled">#= data.value #</span>'
            },
        };
    }

    $scope.disableHalfDate = function () {
        if (!moment($scope.model.fromDate, 'DD/MM/YYYY').isValid() || !moment($scope.model.toDate, 'DD/MM/YYYY').isValid()) {
            $scope.model.halfDate = null;
            return true;
        }
        return false;
    }

    $scope.disableRadioHalfDate = function () {
        if (!moment($scope.model.fromDate, 'DD/MM/YYYY').isValid() || !moment($scope.model.toDate, 'DD/MM/YYYY').isValid() || !moment($scope.model.halfDate, 'DD/MM/YYYY').isValid()) {
            $scope.model.halfDayOption = null;
            return true;
        }
        return false;
    }

    $scope.month = {
        empty: '<span class="k-state-disabled">#= data.value #</span>'
    };

    function approve() {
        // value thì lấy trong biến $rootScope.reasonObject.comment
        // nếu thành công thì return về true để ẩn modal đi
        Notification.success("Approve leave management");
        return true;
    }

    function requestedToChange() {
        // value thì lấy trong biến $rootScope.reasonObject.comment
        // nếu thành công thì return về true để ẩn modal đi
        Notification.success("RequestedToChange leave management");
        return true;
    }

    function reject() {
        // value thì lấy trong biến $rootScope.reasonObject.comment
        // nếu thành công thì return về true để ẩn modal đi
        Notification.success("Reject leave management");
        return true;
    }

    // phần khai báo chung
    $scope.actions = {
        approve: approve,
        requestedToChange: requestedToChange,
        reject: reject
    };

    function refreshGrid() {
        let grid = $("#leaveApplicationDetailGrid").data("kendoGrid");
        grid.refresh();
    }

    function loadPageOne() {
        let grid = $("#grid").data("kendoGrid");
        if (grid) {
            grid.dataSource.fetch(() => grid.dataSource.page(1));
        }
    }

    $scope.search = async function (reset) {
        // let option = buildArgForSearch(appSetting.pageSizeDefault, $scope.query, columsToSearchOnKeyword);
        // if ($state.current.name == commonData.leaveManagement.myRequests) {
        //     option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `CreatedById = @${option.predicateParameters.length}`;
        //     option.predicateParameters.push($rootScope.currentUser.id);
        // }
        // // await getListLeaveApplicationByFilter(option);
        loadPageOne();
    }

    async function getListLeaveApplicationByFilter(option) {
        //get limit in grid 
        let grid = $("#grid").data("kendoGrid");
        option.limit = grid.pager.dataSource._pageSize
        //
        var res = await cbService.getInstance().leaveManagement.getListLeaveApplication(option).$promise;
        if (res.isSuccess) {
            $scope.data = [];
            var n = 1;
            res.object.data.forEach(element => {
                element.no = n++;
                $scope.data.push(element);
            });;
            $scope.total = res.object.count;
            initGridRequests(res.object.data, $scope.total, 1, option.limit);
        }
    }

    function initGridRequests(dataSource, total, pageIndex, pageSize) {
        let grid = $("#grid").data("kendoGrid");
        let dataSourceRequests = new kendo.data.DataSource({
            data: dataSource,
            // pageSize: appSetting.pageSizeDefault,
            pageSize: pageSize,
            page: pageIndex,
            schema: {
                total: function () {
                    return total;
                }
            },
        });
        grid.setDataSource(dataSourceRequests);
    }

    $scope.clearSearch = async function () {
        $scope.query = {
            keyword: '',
            departmentCode: '',
            fromDate: '',
            toDate: '',
            status: [],
            createdDate: "",
            leaveDateFrom: null,
            leaveDateTo: null
        }

        $scope.currentQuery = {
            predicate: "",
            predicateParameters: [],
            Order: "Created desc",
            Limit: appSetting.pageSizeDefault,
            Page: 1
        }
        let option = $scope.currentQuery;
        $scope.$broadcast('resetToDate', $scope.query.toDate);
        loadPageOne();
        // await getListLeaveApplicationByFilter(option);
    }

    $scope.export = async function () {
        let option = buildArgForSearch(appSetting.numberRowPerSheets, $scope.query, columsToSearchOnKeyword);
        if ($state.current.name == commonData.leaveManagement.myRequests) {
            option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `CreatedById = @${option.predicateParameters.length}`;
            option.predicateParameters.push($rootScope.currentUser.id);
        }
        var res = await fileService.getInstance().processingFiles.export({ type: commonData.exportType.LEAVEMANAGEMENT }, option).$promise;
        if (res.isSuccess) {
            exportToExcelFile(res.object);
            Notification.success(appSetting.notificationExport.success);
        } else {
            Notification.error(appSetting.notificationExport.error);
        }

    }
    $scope.showProcessingStages = function () {
        $rootScope.visibleProcessingStages($translate);
    }

    $scope.attachments = [];
    $scope.oldAttachments = [];
    $scope.removeFiles = [];
    $scope.onSelect = function (e) {
        let message = $.map(e.files, function (file) {
            $scope.attachments.push(file);
            return file.name;
        }).join(", ");
    };

    $scope.removeAttach = function (e) {
        let file = e.files[0];
        $scope.attachments = $scope.attachments.filter(item => item.name !== file.name);
    }

    $scope.removeOldAttachment = function (model) {
        $scope.oldAttachments = $scope.oldAttachments.filter(item => item.id !== model.id);
        $scope.removeFiles.push(model.id);
    }

    $scope.downloadAttachment = async function (id) {
        let result = await attachmentFile.downloadAndSaveFile({
            id
        });
    }

    async function getShiftSetByDate(userSapCode, date) {
        let returnValue = new Array();
        let paramObj = {
            "SAPCode": userSapCode,
            "date": date
        };
        let shiftSetResult = await ssgexService.getInstance().remoteDatas.getShiftSetByDate(paramObj).$promise;
        if (shiftSetResult.isSuccess) {

            let shiftSet = new Array();
            let shiftSetOfCurrentDate = shiftSetResult.object;
            if (!$.isEmptyObject(shiftSetOfCurrentDate.shift1)) {
                shiftSet.push(shiftSetOfCurrentDate.shift1);
            }
            if (!$.isEmptyObject(shiftSetOfCurrentDate.shift2)) {
                shiftSet.push(shiftSetOfCurrentDate.shift2);
            }
            returnValue = shiftSet;
        }
        return returnValue;
    }

    async function checkShiftSetByDateRange(userSapCode, leaveShiftCode, startDate, endDate) {
        let returnValue = new Array();
        try {
            var checkDate = startDate;
            while (checkDate <= endDate) {
                let currentShiftSet = await getShiftSetByDate(userSapCode, checkDate);
                if (currentShiftSet.indexOf(leaveShiftCode) == -1) {
                    returnValue.push({
                        "status": false,
                        "shiftSet": currentShiftSet,
                        "leaveShiftCode": leaveShiftCode,
                        "date": checkDate
                    });
                }
                checkDate = moment(checkDate).add(1, 'days').toDate();
            }
        } catch (e) {

        }
        return returnValue;
    }

    $scope.checkAllowRevoke = async function () {
        let returnValue = true;
        if (!$.isEmptyObject($scope.model) && !$.isEmptyObject($scope.model.leaveApplicantDetails)) {
            var userSAPCode = $scope.model.employeeCode;
            var totalCount = $scope.model.leaveApplicantDetails.length;
            let checkingResults = new Array();
            for (var i = 0; i < totalCount; i++) {
                let cLeaveInfo = $scope.model.leaveApplicantDetails[i];
                let leaveShiftCode = cLeaveInfo.leaveCode;
                let cCheckingResults = await checkShiftSetByDateRange(userSAPCode, leaveShiftCode, cLeaveInfo.fromDate, cLeaveInfo.toDate);
                if (!$.isEmptyObject(cCheckingResults)) {
                    checkingResults = checkingResults.concat(cCheckingResults);
                }
            }

            let errors = [];
            $(checkingResults).filter(function (index, cItem) {
                if (!$.isEmptyObject(cItem) && !cItem.status) {
                    returnValue = false;
                    //Show error message
                    errors.push({
                        controlName: "",
                        message: $translate.instant('LEAVE_MANAGEMENT_NOT_ABLE_REVOKE_LEAVE_CODE_CHANGED', { leaveDate: moment(cItem.date).format('DD/MMM/YYYY'), leaveCode: cItem.leaveShiftCode })

                    });
                }
            });

            $scope.errors = errors;
            $scope.$apply();
        }
        return returnValue;
    }

    async function uploadAction() {
        var payload = new FormData();
        $scope.attachments.forEach(item => {
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
            let allAttachments = $scope.oldAttachments && $scope.oldAttachments.length ? $scope.oldAttachments.map(({
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
                allAttachments = allAttachments.concat(attachmentFiles);
            }
            attachmentFilesJson = JSON.stringify(allAttachments);
            return attachmentFilesJson;
        } catch (e) {
            return '';
        }
    }
    $rootScope.$on("isEnterKeydown", function (result, data) {
        if ($scope.advancedSearchMode && data.state == $state.current.name) {
            $scope.search();
        }
    });
});