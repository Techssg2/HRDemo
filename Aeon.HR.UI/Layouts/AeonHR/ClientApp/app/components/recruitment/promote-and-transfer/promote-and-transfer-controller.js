var ssgApp = angular.module("ssg.promoteAndTransferModule", ["kendo.directives"]);
ssgApp.controller("promoteAndTransferController", function ($rootScope, $scope, $location, appSetting, $stateParams, $state, commonData, moment, Notification, $timeout, recruitmentService, masterDataService, settingService, attachmentFile, workflowService, ssgexService, fileService, localStorageService, $translate, attachmentService) {
    var ssg = this;
    $scope.title = '';
    $scope.value = 0;
    var currentAction = null;
    isItem = true;
    $scope.title = $stateParams.id ? $translate.instant('PROMOTE_TRANSFER_MENU') + $stateParams.referenceValue : $state.current.name == 'home.promoteAndTransfer.item' ? "New item: PROMOTION & TRANSFER RECOMMENDATION" : $stateParams.action.title;
    $scope.titleDetail = '';
    var id = $stateParams.id;
    $scope.model = {};
    $scope.disabled = true;
    $rootScope.isParentMenu = false;
    isItem = true;
    stateItem = "home.promoteAndTransfer.item";
    $scope.mode = $stateParams.id ? 'Edit' : 'New';
    var allDepartments = JSON.parse(sessionStorage.getItemWithSafe("departments"));
    var resetAllDepartments = JSON.parse(sessionStorage.getItemWithSafe("departments"));

    $scope.filter = {
        keyword: "",
        createdDate: "",
        status: "",
        fromDate: null,
        toDate: null
    };
    $rootScope.isParentMenu = false;

    $scope.itemForm = {
        // newJobGradeName: '',
        // newDeptOrLineName: '',
        // newWorkLocationName: '',
    };
    $scope.total = 0;
    $scope.currentQuery = {
        predicate: "",
        predicateParameters: [],
        order: "Status desc, Created desc",
        limit: appSetting.pageSizeDefault,
        page: 1
    };
    $scope.clearSearch = function () {
        $scope.filter = {
            keyword: "",
            createdDate: "",
            status: "",
            fromDate: null,
            toDate: null
        };
        $scope.currentQuery = {
            predicate: "",
            predicateParameters: [],
            order: "Status desc, Created desc",
            limit: appSetting.pageSizeDefault,
            page: 1
        };
        $scope.currentQueryExport = {
            predicate: "",
            predicateParameters: [],
            order: "Created desc",
            limit: appSetting.pageSizeDefault,
            page: 1
        }
        $scope.$broadcast('resetToDate', $scope.filter.toDate);
        reloadGrid();
    }
    var requiredFields = [{
        fieldName: 'typeCode',
        title: "Type"
    },
    {
        fieldName: 'effectiveDate',
        title: 'Effective Date'
    },
    {
        fieldName: 'requestFrom',
        title: 'Request From'
    },
    // {
    //     fieldName: 'newSalaryOrBenefit',
    //     title: 'New Salary/ Benefits'
    // },
    {
        fieldName: 'userId',
        title: 'SAP Code'
    },
    {
        fieldName: 'positionName',
        title: 'New Position'
    },
    {
        fieldName: 'newDeptOrLineId',
        title: 'New Department'
    },
    {
        fieldName: 'reportToId',
        title: 'Report To'
    },
    {
        fieldName: 'requestFrom',
        title: 'Request From'
    }
    ];

    async function ngOnit() {
        if ($state.current.name === 'home.promoteAndTransfer.item') {
            //await getDepartments();
            //await getJobGrades();
            await getDepartment();
            //await getPosition();
            if (id) {
                $scope.model.id = id;
                await getDepartmentByReferenceNumber();
                await getPromoteAndTransferById();
                $timeout(function () {
                    $scope.titleDetail = "PROMOTION & TRANSFER RECOMMENDATION: " + $scope.itemForm.referenceNumber;
                }, 10);
            }

        } else {
            await getListPromoteAndTransfers();
        }
    }

    async function getListPromoteAndTransfers() {
        $scope.data = [];
        $scope.promoteAndTransferGridOptions = {
            dataSource: {
                serverPaging: true,
                pageSize: 20,
                transport: {
                    read: async function (e) {
                        await getPromoteAndTransfers(e);
                    }
                },
                schema: {
                    total: () => {
                        return $scope.total
                    },
                    data: () => {
                        return $scope.data
                    }
                }
            },

            sortable: false,
            autoBind: true,
            valuePrimitive: false,
            pageable: {
                alwaysVisible: true,
                pageSizes: appSetting.pageSizesArray,
            },
            columns: [{
                field: "status",
                title: "Status",
                width: "350px",
                locked: true,
                template: function (dataItem) {
                    var statusTranslate = $rootScope.getStatusTranslate(dataItem.status);
                    //return `<workflow-status status="${statusTranslate}"></workflow-status>`;
                    return `<workflow-status status="${dataItem.status}"></workflow-status>`;
                }
            },
            {
                field: "referenceNumber",
                title: "Reference Number",
                width: "180px",
                locked: true,
                template: function (dataItem) {

                    if (dataItem.status === commonData.Status.Waiting) {
                        return `<a ui-sref="home.promoteAndTransfer.approve({referenceValue: '${dataItem.referenceNumber}', id: '${dataItem.id}'})" ui-sref-opts="{ reload: true }">${dataItem.referenceNumber}</a>`;
                    }
                    return `<a ui-sref="home.promoteAndTransfer.item({referenceValue: '${dataItem.referenceNumber}', id: '${dataItem.id}' })" ui-sref-opts="{ reload: true }">${dataItem.referenceNumber}</a>`;
                }
            },
            {
                field: "userSAPCode",
                title: "SAP Code",
                width: "200px"
            },
            {
                field: "userFullName",
                title: "Full Name",
                width: "200px"
            },
            {
                field: "currentDepartment",
                title: "Current Dept/ Line",
                width: "350px"
            },
            {
                field: "regionName",
                title: "Region",
                width: "100px"
            },
            {
                field: "currentWorkLocation",
                title: "Current Work Location",
                width: "200px"
            },
            {
                field: "created",
                title: 'Created Date',
                width: "150px",
                template: function (dataItem) {
                    if (dataItem && dataItem.created !== null) {
                        return moment(dataItem.created).format('DD/MM/YYYY HH:mm');
                    }
                    return '';
                }
            }
            ]
        }
    };

    //Get data list promote and transfer
    async function getPromoteAndTransfers(option) {
        if (option) {
            $scope.currentQuery.limit = option.data.take;
            $scope.currentQuery.page = option.data.page;
        }

        if ($state.current.name == commonData.myRequests.PromoteAndTranfer) {
            $scope.currentQuery.predicate = ($scope.currentQuery.predicate ? $scope.currentQuery.predicate + ' and ' : $scope.currentQuery.predicate) + `CreatedById = @${$scope.currentQuery.predicateParameters.length}`;
            $scope.currentQuery.predicateParameters.push($rootScope.currentUser.id);
        }
        var res = await recruitmentService.getInstance().promoteAndTransfers.getListPromoteAndTransfers($scope.currentQuery).$promise;
        if (res.isSuccess) {
            $scope.data = [];
            var n = 1;
            res.object.data.forEach(element => {
                element.no = n++;
                $scope.data.push(element);
            });
        }
        $scope.total = res.object.count;
        if (option) {
            option.success($scope.data);
        } else {
            var grid = $("#grid").data("kendoGrid");
            grid.dataSource.read();
        }
    }

    $scope.jobGradeTemporyId = '';
    //Get data detail promote and transfer
    async function getPromoteAndTransferById() {
        let res = await recruitmentService.getInstance().promoteAndTransfers.getPromoteAndTransferById({
            id
        }).$promise;
        if (res.isSuccess) {
            $scope.itemForm.typeCode = res.object.object.typeCode;
            $scope.jobGradeValue = parseInt(res.object.object.currentJobGradeValue);
            await getPosition(parseInt(res.object.object.currentJobGradeValue));
            await getEmployeeData(res.object.object.employeeSubgroup);
            $timeout(function () {
                $scope.itemForm = res.object.object;
                $scope.selectedUserId = $scope.itemForm.userId;
                $scope.selectedReportToUserId = $scope.itemForm.reportToId;
                $scope.jobGradeTemporyId = $scope.itemForm.newJobGradeId;
                $scope.title = $scope.itemForm.referenceNumber;

                $scope.checkManager = false;
                if (res.object.object.requestFrom == $scope.managerRequestFrom) {
                    $scope.checkManager = true;
                }

                try {
                    $scope.oldAttachments = JSON.parse(res.object.object.documents);
                } catch (e) {
                    console.log(e);
                }

            }, 0);
        }
    }

    $scope.positions = [];
    $scope.jobTitlesDataSource = {
        //optionLabel: "Please Select...",
        dataTextField: 'nameShow',
        dataValueField: 'name',
        template: '#: nameShow #',
        autoBind: true,
        valuePrimitive: true,
        filter: "contains",
        dataSource: $scope.positionsGroupByName,
        change: function (e) {
            let data = e.sender.text();
            onChangePosition(data)
        }
    };
    $scope.positionsGroupByName = [];
    positionActing = [];
    async function getPosition(jobgrade) {
        //reset
        $scope.positionsGroupByName = [];
        $scope.itemForm.newDeptOrLineId = '';
        $scope.itemForm.newJobGradeName = '';
        $scope.itemForm.newWorkLocationName = '';

        //
        var result = await recruitmentService.getInstance().position.getOpenPositions().$promise;
        if (result.isSuccess) {
            $scope.positions = result.object.data;
            if (jobgrade) {
                let dataFilter = [];
                if ($scope.itemForm.typeCode == $scope.typeCodeTranfer) {
                    if (result.isSuccess) {
                        $scope.jobGradeDataSource = result.object.data;
                    }
                    var result = _.filter(result.object.data, y => { return y.positionGrade == jobgrade });
                    if (result) {
                        dataFilter = result;
                    }
                    //code moi : Map data Position/ Setting
                    let modelPosition = {
                        predicate: "MetadataType.Value = @0 && JobGrade.Grade = @1 && (Code.contains(@2))",
                        predicateParameters: ["Position", jobgrade, "ACT"],
                        order: "Modified desc",
                        limit: 100000,
                        page: 1
                    }

                    var resultPosition = await settingService.getInstance().recruitment.getPositionLists(modelPosition).$promise;
                    if (resultPosition.isSuccess) {
                        positionActing = [];
                        resultPosition.object.data.forEach(item => {
                            let value = {
                                id: item.id,
                                positionName: item.name,
                                positionGrade: item.jobGradeGrade,
                                jobGradeCaption: item.jobGradeCaption,
                                jobGradeGrade: item.jobGradeGrade,
                                jobGradeId: item.jobGradeId
                            }
                            dataFilter.push(value);
                            $scope.positions.push(value);
                        });
                        positionActing = resultPosition.object.data;
                    }
                    //
                } else {
                    result.object.data.forEach(item => {
                        if (item.positionGrade > jobgrade) {
                            dataFilter.push(item);
                        }
                    });
                }
                let data = _.groupBy(dataFilter, 'positionName');
                Object.keys(data).forEach(x => {
                    let valuePositionGroupByName = {
                        name: x,
                        nameShow: x
                    }
                    $scope.positionsGroupByName.push(valuePositionGroupByName);
                });
                setDataPosition($scope.positionsGroupByName);
            }
        }
    }
    $scope.typeCodeTranfer = 'Tran';
    $scope.typeCodePromotion = 'Pro';
    $scope.typeCodePromotionAndTranfer = 'ProAndTran';

    function setDataPosition(dataPosition) {
        var dataSource = new kendo.data.HierarchicalDataSource({
            data: dataPosition
        });
        var dropdownlist = $("#positionId").data("kendoDropDownList");
        if (dropdownlist) {
            dropdownlist.setDataSource(dataSource);
        }
    }

    function findJobGradeByCode(code) {
        var result;
        result = $scope.jobGrades.find(item => item.caption == code);
        return result.id;
    }

    //reportToDataSource
    $scope.reportToDataSource = {
        dataTextField: 'sapCode',
        dataValueField: 'id',
        filter: "contains",
        template: '#: sapCode # - #: fullName #',
        valueTemplate: '#: sapCode # - #: fullName #',
        autoBind: false,
        valuePrimitive: false,
        dataSource: {
            serverFiltering: true,
            transport: {
                read: async function (e) {
                    await getReportToUsers(e);
                }
            },
        },
        customFilterFields: ['sapCode', 'fullName'],
        filtering: filterMultiField,
        change: async function (e) {
            if (e.sender.dataItem().sapCode) {
                //let SAPCode = e.sender.text();
                let SAPCode = e.sender.dataItem().sapCode;
                let result = await settingService.getInstance().users.seachEmployee({
                    SAPCode: SAPCode
                }).$promise;
                if (result.isSuccess) {
                    $timeout(async function () {
                        $scope.itemForm.reportToFullName = result.object.fullName;
                        $scope.itemForm.reportToId = result.object.id;
                        $scope.itemForm.reportToSAPCode = result.object.sapCode;
                    }, 0);


                }
            }
        },
    };
    //newWorkLocationDataSource
    $scope.workLocations = [];
    $scope.newWorkLocationDataSource = {
        dataTextField: 'name',
        dataValueField: 'code',
        autoBind: false,
        valuePrimitive: false,
        filter: "contains",
        dataSource: $scope.workLocations,
    };

    // departmentOptions
    $scope.departmentData = [];

    $scope.employeeGroupArray = [];
    $scope.employeeSubGroupDataSource = {
        dataTextField: 'employeeGroupDescription',
        dataValueField: 'employeeGroup',
        autoBind: true,
        valuePrimitive: true,
        dataSource: $scope.employeeGroupArray,
        change: async function (e) {
            if (e.sender.text() !== "") {
                var result = _.filter($scope.employeeGroupArray, y => { return y.employeeGroup === e.sender.text() });
                if (result) {
                    var employeeData = result[0];
                    $scope.itemForm.employeeGroup = employeeData.employeeGroup;
                    $scope.itemForm.employeeGroupDescription = employeeData.employeeGroupDescription;
                    $scope.itemForm.employeeSubgroupDescription = employeeData.employeeSubgroupDescription;
                } else {
                    $scope.itemForm.employeeGroup = "";
                    $scope.itemForm.employeeGroupDescription = "";
                    $scope.itemForm.employeeSubgroupDescription = "";
                }
            }
        }
    };

    async function getNewWorkLocation(newWorkLocationText) {
        var returnValue = await settingService.getInstance().users.getNewWorkLocation({ newWorkLocationText: newWorkLocationText }).$promise;
        if (returnValue.isSuccess) {
            $scope.itemForm.newWorkLocationCode = returnValue.object.personnelSubArea;
            $scope.itemForm.personnelAreaText = returnValue.object.personnelAreaText;
            $scope.itemForm.personnelArea = returnValue.object.personnelArea;
        }
    }

    async function getEmployeeData(employeeSubGroup) {
        if (!$.isEmptyObject(employeeSubGroup)) {
            var returnValue = await settingService.getInstance().users.getSubGroupEmployees({ empSubgroup: employeeSubGroup }).$promise;
            if (returnValue.isSuccess) {
                $scope.employeeGroupArray = returnValue.object.data
            }
        } else {
            $scope.employeeGroupArray = [];
        }
        setDataEmployeesGroup($scope.employeeGroupArray);
    }

    function setDataEmployeesGroup(dataSubGroup) {
        var dataSource = new kendo.data.HierarchicalDataSource({
            data: dataSubGroup
        });
        var dropdownlist = $("#employeeGroup").data("kendoDropDownList");
        if (dropdownlist) {
            dropdownlist.setDataSource(dataSource);
        }
    }

    $scope.departmentOptions = {
        placeholder: "",
        dataTextField: "name",
        dataValueField: "id",
        //template: `<span class="#: data.item.enable == false ? 'k-state-disabled': ''#">#:data.item.code# - #:data.item.name#</span>`,
        template: function (dataItem) {
            return `<span class="${dataItem.item.enable == false ? 'k-state-disabled' : ''}">${showCustomDepartmentTitle(dataItem)}</span>`;
        },
        valueTemplate: (e) => showCustomField(e, ['name']),
        valuePrimitive: true,
        checkboxes: false,
        autoBind: true,
        filter: "contains",
        loadOnDemand: true,
        filtering: async function (e) {
            await getDepartmentByFilter(e);
        },
        dataSource: $scope.departmentData,
        select: function (e) {
            let dropdownlist = $("#newDeptOrLineId").data("kendoDropDownTree");
            let dataItem = dropdownlist.dataItem(e.node)
            if (dataItem) {
                $scope.itemForm.newDeptOrLineCode = dataItem.code;
                $scope.itemForm.newDeptOrLineName = dataItem.name;
                $scope.itemForm.isStoreNewDepartment = dataItem.isStore;
            }
            if (!dataItem.enable) {
                e.preventDefault();
            }
        },
        change: async function (e) {
            if (!e.sender.value()) {
                watch(true);
                setDataDepartment(allDepartments);
                $timeout(function () {
                    watch(false);
                }, 0);
                $scope.itemForm.newJobGradeId = ''
                $scope.itemForm.newJobGradeName = '';
                $scope.itemForm.newWorkLocationCode = '';
                $scope.itemForm.newWorkLocationName = '';
                $scope.itemForm.payScaleArea = '';
                $scope.itemForm.employeeSubgroup = ''
                $scope.itemForm.personnelAreaText = '';
                $scope.itemForm.personnelArea = '';
                $scope.itemForm.employeeGroup = "";
                $scope.itemForm.employeeGroupDescription = "";
                $scope.itemForm.employeeSubgroupDescription = "";
                //Thanh add code CR222
                setDataEmployeesGroup([]);
            }
            if ($scope.itemForm.newDeptOrLineId) {
                if ($scope.itemForm.positionName.includes('Acting')) {
                    var position = _.find(departmentRTH, { deptDivisionId: $scope.itemForm.newDeptOrLineId });
                    var result = _.find(positionActing, { name: $scope.itemForm.positionName });
                    $scope.itemForm.positionId = result.id;
                    $scope.itemForm.newJobGradeId = position.jobGradeId
                    $scope.itemForm.newJobGradeName = position.jobGradeGrade;
                }
                else {
                    // cheat code
                    if ($scope.itemForm.newDeptOrLineCode == '40002333' || $scope.itemForm.newDeptOrLineCode == 'DEPT_17032022' || $scope.itemForm.newDeptOrLineCode == '50022551' || $scope.itemForm.newDeptOrLineCode == '40001053-Transfer' || $scope.itemForm.newDeptOrLineCode == '50033940') {
                        $scope.itemForm.newJobGradeId = "f8a9694f-d369-43f5-8625-9c38fa36abe4";
                        $scope.itemForm.newJobGradeName = "G2";
                        if ($scope.itemForm.newDeptOrLineCode == 'DEPT_17032022' || $scope.itemForm.newDeptOrLineCode == '40001053-Transfer' || $scope.itemForm.newDeptOrLineCode == '50033940') {
                            $scope.itemForm.newJobGradeId = "53E9BC34-3F92-45B3-B1EE-606E7EE6A77C";
                            $scope.itemForm.newJobGradeName = "G3";
                        }
                    }
                    else {
                        var position = _.find($scope.positions, { positionName: $scope.itemForm.positionName, deptDivisionId: $scope.itemForm.newDeptOrLineId });
                        $scope.itemForm.positionId = position.id;
                        $scope.itemForm.newJobGradeId = position.deptDivisionJobGradeId
                        $scope.itemForm.newJobGradeName = position.deptDivisionJobGradeCaption;
                    }
                }

                var intSubGroup = 0;
                if ($scope.itemForm.newJobGradeName && $scope.itemForm.newJobGradeName !== "") {
                    intSubGroup = parseInt($scope.itemForm.newJobGradeName.replace("G", ""));
                }

                $scope.itemForm.payScaleArea = intSubGroup > 0 ? "0" + intSubGroup : ""

                $scope.itemForm.employeeSubgroup = "";
                if (intSubGroup > 0) {
                    if (intSubGroup > 4) {
                        intSubGroup = intSubGroup - 1;
                    }
                    $scope.itemForm.employeeSubgroup = "0" + intSubGroup;
                }

                // cheat code
                if ($scope.itemForm.newDeptOrLineCode == '40002333' || $scope.itemForm.newDeptOrLineCode == 'DEPT_17032022' || $scope.itemForm.newDeptOrLineCode == '50022551' || $scope.itemForm.newDeptOrLineCode == '40001053-Transfer' || $scope.itemForm.newDeptOrLineCode == '50033940') {
                    if ($scope.itemForm.newDeptOrLineCode == '40002333') {
                        $scope.itemForm.newWorkLocationName = 'CANARY - GMS';
                    }
                    if ($scope.itemForm.newDeptOrLineCode == '50022551') {
                        $scope.itemForm.newWorkLocationName = 'HA DONG - GMS';
                    }
                    if ($scope.itemForm.newDeptOrLineCode == 'DEPT_17032022' || $scope.itemForm.newDeptOrLineCode == '40001053-Transfer' || $scope.itemForm.newDeptOrLineCode == '50033940') {
                        $scope.itemForm.newWorkLocationName = 'HQ';
                    }
                }
                else {
                    //$scope.itemForm.newWorkLocationCode = position.locationCode;
                    $scope.itemForm.newWorkLocationName = position.locationName;
                }

                //Thanh add code CR222
                if ($scope.itemForm.employeeSubgroup && $scope.itemForm.employeeSubgroup !== "") {
                    await getEmployeeData($scope.itemForm.employeeSubgroup);
                }

                if ($scope.itemForm.newWorkLocationName && $scope.itemForm.newWorkLocationName) {
                    await getNewWorkLocation($scope.itemForm.newWorkLocationName);
                }

                $scope.$apply();
            }
        }
    };

    function filterDepartmentHasNoSAPCodeAndRequestToHire(departmentInfo) {
        let returnValue = departmentInfo;
        try {

            if (!$.isEmptyObject(departmentInfo) && !$.isEmptyObject(departmentInfo)) {
                ////cheat code
                if (returnValue.sapCode == '40002333' || returnValue.sapCode == 'DEPT_17032022' || returnValue.sapCode == '50022551' || returnValue.sapCode == '40001053-Transfer' || returnValue.sapCode == '50033940') {
                    x.enable = true;
                }

                if ($scope.departmentids.indexOf(returnValue.id) > -1 && !$.isEmptyObject(returnValue.sapCode) && !$.isEmptyObject(returnValue.requestToHireId)) {
                    x.enable = true;
                }
                departmentInfo.items = departmentInfo.items.filter(x => !$.isEmptyObject(x.sapCode));

                let deptCount = departmentInfo.items.length;
                for (var i = 0; i < deptCount; i++) {
                    departmentInfo.items[i] = filterDepartmentHasNoSAPCodeAndRequestToHire(departmentInfo.items[i]);
                }
            }
        } catch (e) {

        }
        return returnValue;
    }

    async function getDepartmentByFilter(option) {
        if (!option.filter) {
            option.preventDefault();
        } else {
            let filter = option.filter && option.filter.value ? option.filter.value : "";
            //code moi
            if (filter) {
                arg = {
                    predicate: "((name.contains(@0) or code.contains(@1)) and  SAPCode != null and RequestToHireId.Value != null)",
                    predicateParameters: [filter, filter],
                    page: 1,
                    limit: appSetting.pageSizeDefault,
                    order: ""
                }
                if ($scope.itemForm.currentJobGradeValue) {
                    if ($scope.itemForm.typeCode == $scope.typeCodeTranfer) {
                        arg.predicate += ` and (jobGrade.grade == @2)`;
                        arg.predicateParameters.push($scope.itemForm.currentJobGradeValue);
                    } else if ($scope.itemForm.typeCode == $scope.typeCodePromotion) {
                        arg.predicate += ` and (jobGrade.grade > @2)`;
                        arg.predicateParameters.push($scope.itemForm.currentJobGradeValue);
                    }
                }
                watch(true);
                res = await settingService.getInstance().departments.getDepartmentByFilter(arg).$promise;
                if (res.isSuccess) {

                    res.object.data.forEach(x => {
                        x['enable'] = false;
                        x = filterDepartmentHasNoSAPCodeAndRequestToHire(x);
                        if (x.items.length > 0) {
                            checkEnable(x.items);
                        }
                    });
                    $scope.departmentData = res.object.data;
                    setDataDepartment(res.object.data);
                    if ($scope.itemForm.positionName) {
                        onChangePosition($scope.itemForm.positionName);
                    }
                }
                $timeout(function () {
                    watch(false);
                }, 10)
            }
            else {
                setDataDepartment(resetAllDepartments);
            }
            //
        }

    }

    function watch(value) {
        $rootScope.$watch('isLoading', function (newValue, oldValue) {
            kendo.ui.progress($("#loading"), value);
        });
    }

    async function getDepartment() {
        $scope.departmentData = allDepartments;
        if ($scope.departmentData && $scope.departmentData.length) {
            $scope.departmentData.forEach(x => {
                x = filterDepartmentHasNoSAPCodeAndRequestToHire(x);
                x['enable'] = false;
                if (x.items.length > 0) {
                    checkEnable(x.items);
                }
            });


            if (resetAllDepartments && resetAllDepartments.length) {
                resetAllDepartments.forEach(x => {
                    x['enable'] = false;
                    if (x.items.length > 0) {
                        checkEnable(x.items);
                    }
                });
            }
        }
    }

    function checkEnable(dataList) {
        checkEnableById(dataList, $scope.departmentids)
    }

    function setDataDepartment(dataDepartment) {
        var dataSource = new kendo.data.HierarchicalDataSource({
            data: dataDepartment
        });
        var dropdownlist = $("#newDeptOrLineId").data("kendoDropDownTree");
        dropdownlist.setDataSource(dataSource);
    }

    $scope.jobGrades = [];
    $scope.newJobGradeDataSource = {
        dataTextField: 'name',
        dataValueField: 'id',
        autoBind: false,
        valuePrimitive: false,
        filter: "contains",
        dataSource: $scope.jobGrades,
    };

    async function getJobGrades() {
        var res = await settingService.getInstance().jobgrade.getJobGradeList({
            predicate: "",
            predicateParameters: [],
            order: "Caption asc",
            limit: 10000,
            page: 1
        }).$promise;
        if (res.isSuccess) {
            $scope.jobGrades = res.object.data;
        }
    }

    function initJobGrades(data) {
        var dropdownlist = $("#newJobGradeId").data("kendoDropDownList");
        dropdownlist.setDataSource(data);
    }

    function initWorkLocation(data) {
        var dropdownlist = $("#newWorkLocationCode").data("kendoDropDownList");
        dropdownlist.setDataSource(data);
    }
    $scope.jobGradeValue = '';
    // SapCode
    $scope.usersDataSource = {
        dataTextField: 'sapCode',
        dataValueField: 'id',
        filter: "contains",
        template: '#: sapCode # - #: fullName #',
        valueTemplate: '#: sapCode #',
        autoBind: false,
        valuePrimitive: false,
        dataSource: {
            serverFiltering: true,
            transport: {
                read: async function (e) {
                    await getUsers(e);
                }
            },
        },
        customFilterFields: ['sapCode', 'fullName'],
        filtering: filterMultiField,
        change: async function (e) {
            if (e.sender.text()) {
                setDataDepartment(resetAllDepartments);
                let SAPCode = e.sender.text();
                let result = await settingService.getInstance().users.seachEmployee({
                    SAPCode: SAPCode
                }).$promise;
                if (result.isSuccess) {
                    $scope.jobGradeValue = result.object.jobGradeValue;
                    if ($scope.itemForm.typeCode) {
                        await getPosition(result.object.jobGradeValue);
                    }
                    $timeout(async function () {
                        $scope.itemForm.fullName = result.object.fullName;
                        $scope.itemForm.currentTitle = result.object.jobTitle;
                        $scope.itemForm.currentJobGrade = result.object.jobGradeName ? result.object.jobGradeName : result.object.jobGradeCode;
                        //$scope.itemForm.currentDepartment = result.object.deptName ? result.object.deptName : result.object.devisionName;
                        $scope.itemForm.currentDepartment = result.object.divisionName && result.object.jobGradeCode ? result.object.divisionName : result.object.deptName && result.object.jobGradeCode ? result.object.deptName : result.object.divisionName ? result.object.divisionName : result.object.deptName;
                        $scope.itemForm.currentWorkLocation = result.object.workLocationName;
                        $scope.itemForm.currentJobGradeValue = result.object.jobGradeValue;
                        $scope.itemForm.positionName = '';
                        $scope.itemForm.newDeptOrLineId = '';
                        clearSearchTextOnDropdownTree('newDeptOrLineId');
                        clearSearchTextOnDropdownList('positionId');
                    }, 0);
                }
            }
        },
    };

    $scope.newJobGradeId = '';

    async function onChangePosition(name) {
        //refresh value 
        let resultDepartmentIds = await settingService.getInstance().departments.getAllDepartmentsByPositonName({ posiontionName: name }).$promise;
        var departmentids = []
        if (resultDepartmentIds.isSuccess) {
            $scope.departmentids = resultDepartmentIds.object.data
        }


        let dataEmpty = [];
        setDataDepartment(dataEmpty);
        $scope.departmentData.forEach(y => {
            y.enable = false;

            if (y.items.length > 0) {
                checkEnable(y.items);
            }
        });
        $scope.jobGrades = [];
        $scope.workLocations = [];

        //code moi 
        if (name.includes('Acting')) {
            await getDepartmentRTH(name);

            let allDepartmentRTHIds = departmentRTH.map(x => x.deptDivisionId);
            $scope.departmentData.forEach(x => {

                if (x.code == 'DEP208253_Promo') {
                    x.enable = true;
                }
                else {
                    if (allDepartmentRTHIds.indexOf(x.id) > -1 && !$.isEmptyObject(x.sapCode) && !$.isEmptyObject(x.requestToHireId)) {
                        x.enable = true;
                    }
                }

                if (x.items.length > 0) {
                    checkEnableById(x.items, allDepartmentRTHIds);
                }
            });
            $scope.itemForm.newJobGradeName = '';
            $scope.itemForm.newWorkLocationName = '';
        }
        else {
            $scope.departmentData.forEach(x => {
                if (x.code == 'DEP208253_Promo' || x.code == '40002333' || x.code == 'DEPT_17032022' || x.code == '50022551' || x.code == '40001053-Transfer' || x.code == '50033940') {
                    x.enable = true;
                }
                else {
                    if ($scope.departmentids.indexOf(x.id) > -1 && !$.isEmptyObject(x.sapCode) && !$.isEmptyObject(x.requestToHireId)) {
                        x.enable = true;
                    }
                }
                if (x.items.length > 0) {
                    checkEnableById(x.items, $scope.departmentids)
                }
            });
            $scope.itemForm.newJobGradeName = '';
            $scope.itemForm.newWorkLocationName = '';
        }

        //
        setDataDepartment($scope.departmentData);
        $timeout(function () {
            watch(false);
        }, 0)
    }

    departmentRTH = [];
    async function getDepartmentRTH() {
        departmentRTH = [];
        let model = {
            predicate: "RequestToHire.JobGradeGrade=@0 && RequestToHire.Status=@1 && Status=@2",
            predicateParameters: [$scope.itemForm.currentJobGradeValue, "Completed", 1],
            order: "Modified desc",
            limit: 100000,
            page: 1
        }
        var res = await recruitmentService.getInstance().position.getListPosition(model).$promise;
        if (res.isSuccess) {
            res.object.data.forEach(item => {
                let value = {
                    deptDivisionId: item.requestToHireDeptDivisionId,
                    deptDivisionName: item.requestToHireDeptDivisionName,
                    deptDivisionCode: item.requestToHireDeptDivisionCode,
                    deptDivisionGrade: item.requestToHireDeptDivisionGrade,
                    jobGradeGrade: item.requestToHireJobGradeCaption,
                    jobGradeId: item.requestToHireJobGradeId,
                    locationCode: item.requestToHireLocationCode,
                    locationName: item.requestToHireLocationName,
                }
                departmentRTH.push(value);
            });
        }
    }

    function checkEnableById(dataList, departmentids) {
        if ($.isEmptyObject(departmentids)) {
            departmentids = new Array();
        }
        dataList.forEach(x => {
            // cheat code 
            if (x.code == 'DEP208253_Promo' || x.code == '40002333' || x.code == 'DEPT_17032022' || x.code == '50022551' || x.code == '40001053-Transfer' || x.code == '50033940' ) {
                x.enable = true;
            }
            else {
                if (departmentids.indexOf(x.id) > -1 && !$.isEmptyObject(x.sapCode) && !$.isEmptyObject(x.requestToHireId)) {
                    x.enable = true;
                }
                else {
                    x.enable = false;
                }
            }

            if (x.items.length > 0) {
                return checkEnableById(x.items, departmentids);
            }
        });
    }

    function findValueDeptLine(id) {
        var res;
        $scope.departmentData.forEach(item => {
            if (item.id == id) {
                res = item;
            }
            else {
                res = checkValueChildDeptLine(item.items, id);
            }
        });
        return res
    }

    function checkValueChildDeptLine(dataList, id) {
        var res;
        dataList.forEach(x => {
            if (x.id == id) {
                res = x;
                return res;
            }
            if (x.items.length > 0) {
                checkValueChildDeptLine(x.items, id);
            }
        });
        return res;
    }

    $scope.submit = async function (form) {
        var res = await save(form);
        if (res.isSuccess) {
            var resSubmitWorkflow = await workflowService.getInstance().workflows.startWorkflow({
                itemId: res.object.id
            }, null).$promise;
            if (resSubmitWorkflow && resSubmitWorkflow.isSuccess) {
                Notification.success("Data Successfully Saved");
            } else {
                Notification.error("Error");
            }
        }
        return res;
    }
    $scope.save = async function (form) {
        var res = await save(form);
        if (res) {
            if (res.isSuccess) {
                $scope.model.id = res.object.id;
                $scope.itemForm.id = res.object.id;
                try {
                    $scope.oldAttachments = JSON.parse(res.object.documents);
                    $timeout(function () {
                        $scope.attachments = [];
                        $(".k-upload-files.k-reset").find("li").remove();
                    }, 0);
                } catch (e) {
                    console.log(e);
                }
                Notification.success("Data successfully saved");
                if ($scope.removeFiles) {
                    await attachmentService.getInstance().attachmentFile.deleteMultiFile($scope.removeFiles).$promise;
                }
                if (!$stateParams.id) {
                    $state.go('home.promoteAndTransfer.item', { id: $scope.model.id, referenceValue: res.object.referenceNumber }, { reload: true });
                }
            } else { }
        }
        return res;
    }

    async function save(form) {
        //check validate
        $scope.errors = $rootScope.validateForm(form, requiredFields, $scope.itemForm);
        if (!$scope.itemForm.positionName) {
            let error = { controlName: "New Position", fieldName: "positionName" }
            $scope.errors.push(error);
        }
        if (!$scope.itemForm.newDeptOrLineId) {
            let error = { controlName: "New Department", fieldName: "newDeptOrLineId" }
            $scope.errors.push(error);
        }
        if ($scope.itemForm.requestFrom == $scope.managerRequestFrom) {
            if (!$scope.itemForm.reasonOfPromotion) {
                let error = { controlName: "Reason of Promotion/ Transfer", fieldName: "reasonOfPromotion" }
                $scope.errors.push(error);
            }
        }
        //
        if ($scope.errors.length > 0) {
            return;
        }
        let data;
        if ($scope.attachments.length || $scope.removeFiles.length) {
            let attachmentFilesJson = await mergeAttachment();
            $scope.itemForm.documents = attachmentFilesJson;
        }

        var resultDeptLine = findValueDeptLine($scope.itemForm.newDeptOrLineId);
        if (resultDeptLine) {
            $scope.itemForm.newDeptOrLineCode = resultDeptLine.code;
            $scope.itemForm.newDeptOrLineName = resultDeptLine.name;
        }
        if ($scope.itemForm.positionName.includes('Acting')) {
            $scope.itemForm.actingPositionId = _.cloneDeep($scope.itemForm.positionId);
            $scope.itemForm.positionId = null;
        }
        var res;
        if ($scope.itemForm.id) {
            res = await recruitmentService.getInstance().promoteAndTransfers.updatePromoteAndTransfer(
                $scope.itemForm
            ).$promise;
        } else {
            res = await recruitmentService.getInstance().promoteAndTransfers.createPromoteAndTransfer(
                $scope.itemForm
            ).$promise;
        }
        if (res.isSuccess) {
            $state.go('home.promoteAndTransfer.item', { id: res.object.id, referenceValue: res.object.referenceNumber });
        }
        return res;
    }

    //Status Data
    $scope.statusOptions = {
        placeholder: "",
        valuePrimitive: true,
        checkboxes: false,
        autoBind: false,
        filter: "contains",
        dataSource: {
            // serverFiltering: true,
            data: commonData.itemStatusesHR
        }
    };

    //Code cu cua huy
    $scope.toggleFilterPanel = function (value) {
        $scope.advancedSearchMode = value;
    };
    $scope.requestFromDataSource = [{
        name: "Employee",
        code: "EMP"
    },
    {
        name: "Manager",
        code: "MNG"
    }
    ];

    //Ngan
    $scope.typesDataSource = {
        dataTextField: "name",
        dataValueField: "code",
        valuePrimitive: true,
        autoBind: true,
        dataSource: [{
            code: 'Pro',
            name: 'Promotion'
        },
        {
            code: 'Tran',
            name: 'Transfer'
        },
        {
            code: 'ProAndTran',
            name: 'Promote and Transfer'
        }
        ],
        change: function (e) {
            $scope.itemForm.typeName = e.sender.text();
        }
    }
    //end

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

    var customDateNow = new Date();
    if (customDateNow.getDate() < 26) {
        customDateNow.setDate(26);
        customDateNow.getMonth() + 1;
    } else { // > 26
        if (customDateNow.getMonth() == 11) {
            var customDateNow = new Date(customDateNow.getFullYear() + 1, 0, 1);
            customDateNow.setDate(26);
        } else {
            var customDateNow = new Date(customDateNow.getFullYear(), customDateNow.getMonth() + 1, 1);
            customDateNow.setDate(26);
        }
    }

    $scope.itemForm.effectiveDate = customDateNow;

    async function uploadAction() {

        var payload = new FormData();

        $scope.attachments.forEach(item => {
            payload.append('file', item.rawFile, item.name);
        });

        let result = await attachmentFile.uploadFile(payload);
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
            console.log(e);
            return '';
        }
    }

    $scope.isApprove = false;
    $scope.ActionForPromoteAndTransfer = commonData.ActionForPromoteAndTransfer;
    $scope.action = [commonData.ActionForPromoteAndTransfer.Approve];


    $scope.errors = [];

    // tên hàm của angular x


    //Init default value for AddOrUpdate
    function InitDefaultValueForAddOrUpdate() {
        let now = new Date();
        let day = now.getDate();
        if (day < 26) {
            $scope.itemForm.effectiveDate = new Date(now.getFullYear(), now.getMonth(), 26);
        } else {
            $scope.itemForm.effectiveDate = moment(new Date().setDate(26)).add(1, 'M').toDate();
        }
    }

    function getItem(model, code) {
        // tìm ra giá trị của sap code trước đã
        model.sapCodeId = $scope.data.find(x => x.referenceNumber === code).sapCode;
        // tìm ra employee
        findEmployee(model, model.sapCodeId, $scope.employeeDataSource);
    }

    function findEmployee(model, code, listSource) {
        let item = listSource.find(x => x.code === code);
        // chuyển đổi dữ liệu thành dữ liệu mong muốn
        $timeout(function () {
            mapObject(model, item);
        }, 0);
    }

    function mapObject(model, object) {
        model.staffFullName = object.firstName + " " + object.lastName;
        model.currentTitle = getValueById(
            object.title,
            $scope.currentTitleDataSource,
            "name"
        );
        model.currentJobGrade = getValueById(
            object.jobGrade,
            $scope.currentJobGradeDataSource,
            "name"
        );
        model.currentDeptLine = getValueById(
            object.department_Division,
            $scope.currentDeptLineDataSource,
            "name"
        );
        model.currentWorkLocation = getValueById(
            object.workLocation,
            $scope.currentWorkLocationDataSource,
            "name"
        );
    }

    function isAction(actions) {
        //  user sẽ có một danh sách permission
        // kiểm tra xem permission đó có nằm trong mảng gửi vào hay không
        if (actions.some(x => x === $scope.action)) {
            return true;
        }
        return false;
    }

    function hasPermission(actions) {
        //  user sẽ có một danh sách permission
        // kiểm tra xem permission đó có nằm trong mảng gửi vào hay không
        // giả sử cái permission của thèn nhân viên này là một array

        let hasRight = false;
        $scope.action.forEach(p => {
            if (actions.some(x => x === p)) {
                hasRight = true;
                return;
            }
        });
        // return về true thì là có quyền, còn false là không có quyền
        // action đầu vào là một mảng permission muốn cho phép
        return hasRight;
    }

    // id: id của object cần lấy value
    // list: danh sách item
    function getValueById(id, list, property) {
        let item = list.find(x => x.code === id);
        if (item) {
            return item[property];
        }
        return "";
    }

    function getStatusClass(dataItem) {
        let classStatus = "";
        switch (dataItem.status) {
            case commonData.Status.Waiting:
                classStatus = "fa-circle font-yellow-lemon";
                break;
            case commonData.Status.Completed:
                classStatus = "fa-check-circle font-green-jungle";
                break;
            case commonData.Status.Rejected:
                classStatus = "fa-ban font-red";
                break;
            default:
        }
        return classStatus;
    }

    // phần dùng chung
    function setDataSourceDropdown(idDropdown, items) {
        let dropdownlist = $(idDropdown).data("kendoDropDownList");
        dropdownlist.setDataSource(items);
    }

    // Phần danh cho dropdown employee (SAP)
    function onSelectSAPCode(e) {
        let control = e.sender;
        $scope.itemForm.sapCodeId = control.value();
        findEmployee($scope.itemForm, $scope.itemForm.sapCodeId, $scope.employeeDataSource);
    }

    // Phần dành cho dropdown new jobGrade
    //$scope.newJobGradeOptions = createOptionDropdown($scope.newJobGradeDataSource, 'name', 'code', filteringOfNewJobGradeDropdown, function() {});

    function filteringOfNewJobGradeDropdown(e) {
        let filter = e.filter;

        if (!filter.value) {
            //prevent filtering if the filter does not value
            e.preventDefault();
        } else {
            // call api lấy danh sách data
            let items = $scope.newJobGradeDataSource;

            //--DELETE-CODE thông báo để cho biết là chỗ này đang chạy, lúc ra sản phẩm thì xoá đi 
            Notification.info(`Đang call api lấy danh sách của dropdown newJobGradeDropdown với keyword là: "${filter.value}"`);

            // set lại dataSource cho dropdown 
            setDataSourceDropdown('#newJobGradeDropdown', items);
        }
    }

    function requestedToChange() {
        Notification.success("RequestedToChange");
        return true;
    }

    function approve() {
        Notification.success("Approve");
        return true;
    }

    function reject() {
        Notification.success("Reject");
        return true;
    }
    // hàm này sẽ chạy sau khi view được render lên
    $scope.$on('$viewContentLoaded', function () {
        if ($state.current.name === 'home.promoteAndTransfer.item' && !$stateParams.referenceValue) {
            let employeeDrp = $("#employeeDropdownList").data("kendoDropDownList");
            if (employeeDrp) {
                employeeDrp.select(-1);
            }
        }
    });
    var columsSearch = ['ReferenceNumber.contains(@0)', 'FullName.contains(@1)', 'User.SAPCode.contains(@2)', 'CurrentDepartment.contains(@3)', 'CurrentWorkLocation.contains(@4)']
    $scope.search = async function () {
        $scope.currentQuery = {
            predicate: "",
            predicateParameters: [],
            order: "Status desc, Created desc",
            limit: appSetting.pageSizeDefault,
            page: 1
        };
        var option = $scope.currentQuery;
        if ($scope.filter.keyword) {
            option.predicate = `(${columsSearch.join(" or ")})`;
            for (let index = 0; index < columsSearch.length; index++) {
                option.predicateParameters.push($scope.filter.keyword);
            }
            option.predicateParameters.push($scope.filter.keyword);
        }
        if ($scope.filter.fromDate) {
            //var date = moment($scope.filter.fromDate, 'DD/MM/YYYY').format('MM/DD/YYYY');
            option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `Created >= @${option.predicateParameters.length}`;
            option.predicateParameters.push(moment($scope.filter.fromDate, 'DD/MM/YYYY').format('YYYY-MM-DD'));
        }
        if ($scope.filter.toDate) {
            //var date = moment($scope.filter.toDate, 'DD/MM/YYYY').format('MM/DD/YYYY');
            option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `Created < @${option.predicateParameters.length}`;
            option.predicateParameters.push(moment($scope.filter.toDate, 'DD/MM/YYYY').add(1, 'days').format('YYYY-MM-DD'));
        }

        if ($scope.filter.status && $scope.filter.status.length) {
            generatePredicateWithStatus(option, $scope.filter.status);
        }

        $scope.currentQueryExport = option;


        reloadGrid();
    }

    function reloadGrid() {
        let grid = $("#grid").data("kendoGrid");
        if (grid) {
            grid.dataSource.read();
            if ($scope.advancedSearchMode) {
                grid.dataSource.page(1);
            }
        }

    }
    async function getUsers(option) {
        if (option.data.filter) {
            var filter = option.data.filter.filters.length ? capitalize(option.data.filter.filters[0].value) : "";
        }
        else {
            var filter = '';
        }
        var res = await settingService.getInstance().users.getUsers({
            predicate: "sapcode.contains(@0) || fullName.contains(@1)",
            predicateParameters: [filter, filter],
            page: 1,
            limit: appSetting.pageSizeDefault,
            order: "sapcode asc"
        }).$promise;
        if (res.isSuccess) {
            $scope.dataUser = [];
            res.object.data.forEach(element => {
                element['showCodeName'] = element.sapCode + ' - ' + element.fullName;
                $scope.dataUser.push(element);
            });
            if ($scope.selectedUserId) {
                let index = _.findIndex($scope.dataUser, x => {
                    return x.id == $scope.selectedUserId;
                });
                if (index == -1) {
                    $scope.dataUser.push({ id: $scope.itemForm.userId, fullName: $scope.itemForm.fullName, sapCode: $scope.itemForm.sapCode });
                }
                $scope.selectedUserId = '';
            }
        }
        if (option) {
            option.success($scope.dataUser);
        }
    }
    async function getReportToUsers(option) {
        if (option.data.filter) {
            var filter = option.data.filter.filters.length ? capitalize(option.data.filter.filters[0].value) : "";
        }
        else {
            var filter = '';
        }
        var res = await settingService.getInstance().users.getUsers({
            predicate: "sapcode.contains(@0) || fullName.contains(@1)",
            predicateParameters: [filter, filter],
            page: 1,
            limit: appSetting.pageSizeDefault,
            order: "sapcode asc"
        }).$promise;
        if (res.isSuccess) {
            $scope.dataReportToUser = [];
            res.object.data.forEach(element => {
                element['showCodeName'] = element.sapCode + ' - ' + element.fullName;
                $scope.dataReportToUser.push(element);
            });
            if ($scope.selectedReportToUserId) {
                let index = _.findIndex($scope.dataReportToUser, x => {
                    return x.id == $scope.selectedReportToUserId;
                });
                if (index == -1) {
                    $scope.dataReportToUser.push({ id: $scope.itemForm.reportToId, fullName: $scope.itemForm.reportToFullName, sapCode: $scope.itemForm.reportToSAPCode });
                }
                $scope.selectedReportToUserId = '';
            }
        }
        if (option) {
            option.success($scope.dataReportToUser);
        }
    }
    $scope.showProcessingStages = function () {
        $rootScope.visibleProcessingStages($translate);
    }

    $scope.currentQueryExport = {
        predicate: "",
        predicateParameters: [],
        order: "Created desc",
        limit: appSetting.pageSizeDefault,
        page: 1
    }

    $scope.export = async function () {
        let option = $scope.currentQueryExport
        var res = await fileService.getInstance().processingFiles.export({ type: commonData.exportType.PROMOTEANDTRANSFER }, option).$promise;
        if (res.isSuccess) {
            exportToExcelFile(res.object);
            Notification.success(appSetting.notificationExport.success);
        } else {
            Notification.error(appSetting.notificationExport.error);
        }
    }
    $scope.managerRequestFrom = 'MNG';
    $scope.checkManager = false;

    $scope.onChangeRequestFrom = function (data) {
        if (data == $scope.managerRequestFrom) {
            $scope.checkManager = true;
        }
        else {
            $scope.checkManager = false;
            $scope.itemForm.reasonOfPromotion = '';
        }
    }

    $scope.printForm = async function () {
        let res = await recruitmentService.getInstance().promoteAndTransfers.printForm({ id: id }).$promise;
        if (res.isSuccess) {
            exportToPdfFile(res.object);
        }
    }

    $scope.onchangeType = async function (data) {
        setDataDepartment(resetAllDepartments)
        $scope.itemForm.positionName = '';
        await getPosition($scope.jobGradeValue);
    }

    async function getDepartmentByReferenceNumber() {
        let referencePrefix = $stateParams.referenceValue.split('-')[0];
        let res = await settingService.getInstance().departments.getDepartmentByReferenceNumber({ prefix: referencePrefix, itemId: $stateParams.id }).$promise;
        if (res.isSuccess) {
            setDataDepartment(res.object.data);
        }

    }
    ngOnit();

    $rootScope.$on("isEnterKeydown", function (event, data) {
        if ($scope.advancedSearchMode && data.state == $state.current.name) {
            $scope.search();
        }
    });
    //Khiem - task 512
    $scope.$on('workflowStatus', async function (event, data) {
        if (data.isAttachmentFile) {
            $timeout(function () {
                $("#attachmentDetail").removeAttr("disabled");
            }, 0);
        }

    });

    //QUI - task JIRA 84
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
});