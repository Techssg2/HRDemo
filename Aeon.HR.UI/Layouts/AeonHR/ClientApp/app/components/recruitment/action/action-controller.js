var ssgApp = angular.module('ssg.actionModule', ["kendo.directives"]);
ssgApp.controller('actionController', function ($rootScope, $scope, $location, appSetting, $stateParams, $state, moment, Notification, $timeout, recruitmentService, commonData, settingService, employeeService, masterDataService, workflowService, ssgexService, fileService, localStorageService, $translate) {
    // create a message to display in our view
    var ssg = this;
    $scope.title = '';
    isItem = true;
    $scope.title = $stateParams.id ? "Acting: " + $stateParams.referenceValue : $state.current.name == 'home.action.item' ? "NEW ITEM: ACTING" : $stateParams.action.title;
    id = $stateParams.id;
    $rootScope.isParentMenu = false;
    $scope.filter = {};
    $scope.detail = {};
    $scope.errors = [];
    $scope.mode = $stateParams.referenceValue ? 'Edit' : 'New';
    var allDepartments = JSON.parse(sessionStorage.getItemWithSafe("departments"));
    var resetAllDepartments = JSON.parse(sessionStorage.getItemWithSafe("departments"));
    $scope.saveItem = async function (form) {
        var res = await save(form);
        return res;
    }

    function beforeSave() {
        var goalGridCurrent = $("#goalGrid").data("kendoGrid");
        let compulsoryGrid = $("#compulsoryGrid").data("kendoGrid");
        if (compulsoryGrid) {
            $scope.tableCompulsoryTraining = compulsoryGrid._data;
        } else {
            $scope.tableCompulsoryTraining = [];
        }
        if (goalGridCurrent) {
            $scope.goalDataSource = new kendo.data.ObservableArray(goalGridCurrent._data);
        } else {
            $scope.goalDataSource = new kendo.data.ObservableArray([]);
        }
        syncAppraising();
    }
    async function save(form) {
        let result = { isSuccess: false }
        beforeSave();
        $scope.errors = validateForm($scope.detail, requiredFields);
        var errors2 = validateFormatDetail();
        $scope.errors = $scope.errors.concat(errors2);
        if ($scope.errors.length > 0) {
            return result;
        } else {
            mapDataSource('goal');
            mapDataSource('compulsoryTraining');
            mapDataSource('appraising');
            $scope.detail.templateGoal = JSON.stringify($scope.model.goalDataSource);
            $scope.detail.tableCompulsoryTraining = JSON.stringify($scope.model.tableCompulsoryTraining);
            // $scope.detail.titleInActingPeriodName = $scope.detail.position.positionName;
            var modelPeriod = [];
            $scope.model.appraisingDataSource.forEach(item => {
                modelPeriod.push(item);
            });
            var modelRequest = {
                acting: $scope.detail,
                periods: modelPeriod,
            };

            var res = await recruitmentService.getInstance().actings.createActing(modelRequest).$promise;
            if (res.isSuccess && res.object) {
                Notification.success('Data Successfully Save');
                $timeout(function () {
                    $scope.actingId = res.object.id;
                    $scope.model = _.cloneDeep(res.object);
                    if (!$stateParams.id) {
                        $state.go('home.action.item', { id: $scope.model.id, referenceValue: res.object.referenceNumber }, { reload: true });
                    }
                }, 0);

            } else {
                Notification.error('Error System');
            }
            return res;
        }
    }

    function findValueByField(dataSource, nameFieldCompare, nameFieldGetValue, valueCheck) {
        var result = dataSource.find(x => x[nameFieldCompare] === valueCheck);
        return result[nameFieldGetValue];
    }

    var validateFormatDetail = function () {
        var errors = [];
        var period1 = moment($scope.detail.period1From, 'DD/MM/YYYY').isValid();
        var period2 = moment($scope.detail.period1To, 'DD/MM/YYYY').isValid();
        if (!$scope.detail.period1From || !$scope.detail.period1To) {
            errors.push({
                controlName: 'Period 1',
            });
        }

        if ($scope.goalDataSource.length < 1) {
            errors.push({
                controlName: 'Goal',
            });
        } else {
            let n = 0;
            $scope.goalDataSource.forEach(element => {
                n++;
                if (!element.goal) {
                    errors.push({
                        controlName: `Goal of row ${n}`,
                    });
                }
                else {
                    if (!element.weight) {
                        errors.push({
                            controlName: `Weight of row ${n}`,
                        });
                    }
                }
            });

            // if (!$scope.goalDataSource[0].goal) {
            //     errors.push({
            //         controlName: 'Goal first row',
            //     });
            // } else {
            //     if (!$scope.goalDataSource[0].weight) {
            //         errors.push({
            //             controlName: 'Weight',
            //         });
            //     }
            // }
            let sumWeght = 0.00;
            $scope.goalDataSource.forEach(x => {
                sumWeght += parseFloat(x.weight);
            });
            if (sumWeght != 100) {
                errors.push({
                    controlName: 'Weight',
                    message: 'Total weight of the goals have to be equal 100'
                });
            }
        }

        if ($scope.tableCompulsoryTraining.length > 0) {
            let m = 0;
            $scope.tableCompulsoryTraining.forEach(element => {
                m++;
                if (!element.courseName) {
                    errors.push({
                        controlName: `Compulsory Training of row ${m}`,
                    });
                }
            });
        }

        if ($scope.detail.jobGradeValue == 1) {
            if (!$scope.detail.isCompletedTranning && $scope.tableCompulsoryTraining.length == 0) {
                errors.push({
                    controlName: 'Compulsory Training',
                });
            }
        }


        var periodError = [];
        var p1t = moment($scope.detail.period1To, 'DD/MM/YYYY');
        var p1f = moment($scope.detail.period1From, 'DD/MM/YYYY');
        var p2f = moment($scope.detail.period2From, 'DD/MM/YYYY');
        var p2t = moment($scope.detail.period2To, 'DD/MM/YYYY');
        var p3f = moment($scope.detail.period3From, 'DD/MM/YYYY');
        var p3t = moment($scope.detail.period3To, 'DD/MM/YYYY');
        var p4f = moment($scope.detail.period4From, 'DD/MM/YYYY');
        var p4t = moment($scope.detail.period4To, 'DD/MM/YYYY');

        if (p1f.isValid() && p1t.isValid()) {
            periodError.push([
                p1f,
                p1t,
                'Period 1',
            ]);
        }
        if (p2f.isValid() && p2t.isValid()) {
            periodError.push([
                p2f,
                p2t,
                'Period 2',
            ]);
        }
        if (p3f.isValid() && p3t.isValid()) {
            periodError.push([
                p3f,
                p3t,
                'Period 3',
            ]);
        }
        if (p4f.isValid() && p4t.isValid()) {
            periodError.push([
                p4f,
                p4t,
                'Period 4',
            ]);
        }
        for (let i = 0; i < periodError.length - 1; i++) {
            if (periodError[i][1] > periodError[i + 1][0]) {
                errors.push({
                    controlName: 'Period',
                    message: `${periodError[i][2]} must less than ${periodError[i + 1][2]}`,
                });
            }
        }

        return errors;
    }

    $scope.toggleFilterPanel = async function (value) {
        $scope.advancedSearchMode = value;
        if (value) {
            setDataDepartment(allDepartments, "departmentId");
            setDataDepartment(allDepartments, "departmentSAPId");
        }

    }

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
                    $scope.detail.employeeGroup = employeeData.employeeGroup;
                    $scope.detail.employeeGroupDescription = employeeData.employeeGroupDescription;
                    $scope.detail.employeeSubgroupDescription = employeeData.employeeSubgroupDescription;
                } else {
                    $scope.detail.employeeGroup = "";
                    $scope.detail.employeeGroupDescription = "";
                    $scope.detail.employeeSubgroupDescription = "";
                }
            }
        }
    };

    async function getNewWorkLocation(newWorkLocationText) {
        var returnValue = await settingService.getInstance().users.getNewWorkLocation({ newWorkLocationText: newWorkLocationText }).$promise;
        if (returnValue.isSuccess) {
            $scope.detail.newWorkLocationCode = returnValue.object.personnelSubArea;
            $scope.detail.personnelAreaText = returnValue.object.personnelAreaText;
            $scope.detail.personnelArea = returnValue.object.personnelArea;
        }
    }

    async function getAllWorkLocation() {
        var returnValue = await settingService.getInstance().users.getNewWorkLocation({ newWorkLocationText: "" }).$promise;
        if (returnValue.isSuccess) {
            $scope.allPersonelArea = returnValue.object;
            $scope.newPersonelAreaCollection = _.uniqBy($scope.allPersonelArea, cItem => `${cItem.personnelArea}${cItem.personnelAreaText}`);
            setNewPersonelArea();
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

    $scope.Init = function () {
        $scope.filter = {
            keyWord: '',
            departmentId: '',
            status: '',
            createdDateObject: '',
            fromDate: null,
            toDate: null
        }
    }

    var requiredFields = [{
        fieldName: 'userId',
        title: "SAP Code",
    },
    {
        fieldName: 'firstAppraiserId',
        title: "1st Appraiser",
    },
    {
        fieldName: 'secondAppraiserId',
        title: "2nd Appraiser",
    },
    // {
    //     fieldName: 'titleInActingPeriodCode',
    //     title: "Position in Acting Period",
    // },
    {
        fieldName: 'positionId',
        title: "Position in Acting Period",
    },
    {
        fieldName: 'departmentId',
        title: "Department in Acting Period",
    },
    ];

    //Data
    //model save database
    $scope.model = {
        isCompletedTranning: false,
        isCompletedTranning: false,
        tableCompulsoryTraining: [],
        goalDataSource: [],
        appraisingDataSource: []
    }

    //show len UI 
    $scope.tableCompulsoryTraining = new kendo.data.ObservableArray([]);
    $scope.goalDataSource = new kendo.data.ObservableArray([]);
    $scope.appraisingDataSource = new kendo.data.ObservableArray([
        { period: '1', timeRange: '', appraising: new kendo.data.ObservableArray([]) },
        { period: '2', timeRange: '', appraising: new kendo.data.ObservableArray([]) },
        { period: '3', timeRange: '', appraising: new kendo.data.ObservableArray([]) },
        { period: '4', timeRange: '', appraising: new kendo.data.ObservableArray([]) },
    ]);



    $scope.dateRange = {
        period1FromMin: moment('01/01/2000').format('MM/DD/YYYY'),
        period1FromMax: moment('12/31/2030').format('MM/DD/YYYY'),
        period1ToMin: moment('01/01/2000').format('MM/DD/YYYY'),
        period1ToMax: moment('12/31/2030').format('MM/DD/YYYY'),

        period2FromMin: moment('01/01/2000').format('MM/DD/YYYY'),
        period2FromMax: moment('12/31/2030').format('MM/DD/YYYY'),
        period2ToMin: moment('01/01/2000').format('MM/DD/YYYY'),
        period2ToMax: moment('12/31/2030').format('MM/DD/YYYY'),

        period3FromMin: moment('01/01/2000').format('MM/DD/YYYY'),
        period3FromMax: moment('12/31/2030').format('MM/DD/YYYY'),
        period3ToMin: moment('01/01/2000').format('MM/DD/YYYY'),
        period3ToMax: moment('12/31/2030').format('MM/DD/YYYY'),

        period4FromMin: moment('01/01/2000').format('MM/DD/YYYY'),
        period4FromMax: moment('12/31/2030').format('MM/DD/YYYY'),
        period4ToMin: moment('01/01/2000').format('MM/DD/YYYY'),
        period4ToMax: moment('12/31/2030').format('MM/DD/YYYY'),
    }

    $scope.inst = {
        appraising: '',
    };

    //Department
    $scope.departmentOptions = {
        dataTextField: "name",
        dataValueField: "id",
        //template: showCustomDepartmentTitle,
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
            await getDepartmentByFilter(e, "departmentId");
        },
        dataSource: $scope.departments,
        select: async function (e) {
            let dropdownlist = $("#departmentId").data("kendoDropDownTree");
            let dataItem = dropdownlist.dataItem(e.node);

            if (!dataItem.enable) {
                e.preventDefault();
            }
            else {
                if (dataItem) {
                    $scope.detail.newDeptOrLineCode = dataItem.code;
                    $scope.detail.newDeptOrLineName = dataItem.name;
                    $scope.detail.isStoreNewDepartment = dataItem.isStore;

                    //Thanh add code CR222
                    $scope.detail.newJobGradeId = dataItem.jobGradeId
                    $scope.detail.newJobGradeName = dataItem.jobGradeCaption;

                    var intSubGroup = 0;
                    if ($scope.detail.newJobGradeName && $scope.detail.newJobGradeName !== "") {
                        intSubGroup = parseInt($scope.detail.newJobGradeName.replace("G", ""));
                    }

                    $scope.detail.payScaleArea = intSubGroup > 0 ? "0" + intSubGroup : ""
                    $scope.detail.employeeSubgroup = "";
                    if (intSubGroup > 0) {
                        if (intSubGroup > 4) {
                            intSubGroup = intSubGroup - 1;
                        }
                        $scope.detail.employeeSubgroup = "0" + intSubGroup;
                    }

                    if ($scope.detail.employeeSubgroup && $scope.detail.employeeSubgroup !== "") {
                        await getEmployeeData($scope.detail.employeeSubgroup);
                    }
                    if ($scope.detail.workLocationName && $scope.detail.workLocationName !== "") {
                        await getNewWorkLocation($scope.detail.workLocationName);
                    }
                    $scope.$apply();
                }
            }
        }
    };

    //personel Area
    $scope.newPersonelAreaOptions = {
        dataTextField: "personnelAreaText",
        dataValueField: "personnelArea",
        valuePrimitive: true,
        autoBind: true,
        filter: "contains",
        dataSource: $scope.newPersonelAreaCollection,
        select: async function (e) {
            let dataItem = e.dataItem;
            if (dataItem) {
                $scope.detail.newPersonnelAreaText = dataItem.personnelAreaText;
                $scope.detail.newPersonnelArea = dataItem.personnelArea;
                setSubPersonelArea(dataItem.personnelArea);
            }
        }
    };

    //personel Area
    $scope.personelSubareaOptions = {
        dataTextField: "personnelSubAreaText",
        dataValueField: "personnelSubArea",
        valuePrimitive: true,
        autoBind: true,
        filter: "contains",
        loadOnDemand: true,
        select: async function (e) {
            let dataItem = e.dataItem;
            if (dataItem) {
                $scope.detail.newWorkLocationName = dataItem.personnelSubAreaText;
                $scope.detail.newWorkLocationCode = dataItem.personnelSubArea;
            }
        }
    };
    //Department SAP ID
    $scope.departmentSAPOptions = {
        dataTextField: "name",
        dataValueField: "id",
        valueTemplate: (e) => showCustomField(e, ['name']),
        template: function (dataItem) {
            let allowDisabled = $.isEmptyObject(dataItem.item.sapCode);
            return `<span class="${allowDisabled ? 'k-state-disabled' : ''}">${showCustomDepartmentTitle(dataItem)}</span>`;
        },
        valuePrimitive: true,
        checkboxes: false,
        autoBind: true,
        filter: "contains",
        loadOnDemand: true,
        filtering: async function (e) {
            await getDepartmentByFilter(e, "departmentSAPId");
        },
        dataSource: $scope.departments,
        select: async function (e) {
            let dropdownlist = $("#departmentSAPId").data("kendoDropDownTree");
            let dataItem = dropdownlist.dataItem(e.node);
            if (dataItem) {
                $scope.detail.departmentSAPId = dataItem.id;
                $scope.detail.newDeptOrLineCode = dataItem.code;
                $scope.detail.newDeptOrLineName = dataItem.name;
                $scope.detail.isStoreNewDepartment = dataItem.isStore;

                //Thanh add code CR222
                $scope.detail.newJobGradeId = dataItem.jobGradeId
                $scope.detail.newJobGradeName = dataItem.jobGradeCaption;

                var intSubGroup = 0;
                if ($scope.detail.newJobGradeName && $scope.detail.newJobGradeName !== "") {
                    intSubGroup = parseInt($scope.detail.newJobGradeName.replace("G", ""));
                }

                $scope.detail.payScaleArea = intSubGroup > 0 ? "0" + intSubGroup : ""
                $scope.detail.employeeSubgroup = "";
                if (intSubGroup > 0) {
                    if (intSubGroup > 4) {
                        intSubGroup = intSubGroup - 1;
                    }
                    $scope.detail.employeeSubgroup = "0" + intSubGroup;
                }

                if ($scope.detail.employeeSubgroup && $scope.detail.employeeSubgroup !== "") {
                    await getEmployeeData($scope.detail.employeeSubgroup);
                }
                //if ($scope.detail.workLocationName && $scope.detail.workLocationName !== "") {
                //    await getNewWorkLocation($scope.detail.workLocationName);
                //}
                $scope.$apply();
            }
        }
    };

    $scope.departmentForGetDetail = [];
    async function getDepartmentByFilter(option, departmentControlId) {
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
                $scope.departmentForGetDetail = res.object.data;
                setDataDepartment(res.object.data, departmentControlId);
            }
        }

    }
    function filterDepartmentHasNoSAPCode(departmentInfo) {
        let returnValue = departmentInfo;
        try {

            if (!$.isEmptyObject(departmentInfo) && !$.isEmptyObject(departmentInfo)) {
                if (!$.isEmptyObject(returnValue.sapCode)) {
                    returnValue.enable = true;
                }
                departmentInfo.items = departmentInfo.items.filter(x => !$.isEmptyObject(x.sapCode));

                let deptCount = departmentInfo.items.length;
                for (var i = 0; i < deptCount; i++) {
                    departmentInfo.items[i] = filterDepartmentHasNoSAPCode(departmentInfo.items[i]);
                }
            }
        } catch (e) {

        }
        return returnValue;
    }
    function setDataDepartment(dataDepartment, departmentControlId) {
        $scope.departments = dataDepartment;
        //Jira - 551
        if (departmentControlId === 'departmentId') {
            $scope.departments.forEach(x => {
                x['enable'] = false;
                x = filterDepartmentHasNoSAPCode(x);
            });
        }

        var dataSource = new kendo.data.HierarchicalDataSource({
            data: dataDepartment,
            schema: {
                model: {
                    children: "items"
                }
            }
        });
        var dropdownlist = $("#" + departmentControlId).data("kendoDropDownTree");
        if (dropdownlist) {
            dropdownlist.setDataSource(dataSource);
        }
    }

    function setNewPersonelArea() {
        var dataSource = new kendo.data.HierarchicalDataSource({
            data: $scope.newPersonelAreaCollection
        });
        var dropdownlist = $("#newPersonnelArea").data("kendoDropDownList");
        if (dropdownlist) {
            dropdownlist.setDataSource(dataSource);
        }
    }

    function setSubPersonelArea(personnelArea) {
        if (personnelArea === null || personnelArea === undefined || personnelArea.trim() === '') {
            $scope.subPersonelAreaCollection = null;
        }
        else {
            $scope.subPersonelAreaCollection = $scope.allPersonelArea.filter(x => x.personnelArea == personnelArea);
        }

        var dataSource = new kendo.data.HierarchicalDataSource({
            data: $scope.subPersonelAreaCollection
        });
        var dropdownlist = $("#newWorkLocationCode").data("kendoDropDownList");
        if (dropdownlist) {
            dropdownlist.setDataSource(dataSource);
        }
    }


    $scope.departments = [];

    //Data SAP CODE
    function showCustomSapCodeTitle(model) {
        if (model.sapCode) {
            return `${model.sapCode} - ${model.fullName}`
        } else {
            return `${model.fullName}`
        }
    }
    $scope.jobGradeCaption = '';
    $scope.sapCodesDataSource = {
        //optionLabel: "Please Select...",
        dataTextField: 'sapCode',
        dataValueField: 'id',
        template: showCustomSapCodeTitle,
        valueTemplate: '#: sapCode #',
        valuePrimitive: true,
        autoBind: true,
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
            if (e.sender.text()) {
                let SAPCode = e.sender.text();
                let result = await settingService.getInstance().users.seachEmployee({
                    SAPCode: SAPCode
                }).$promise;
                if (result.isSuccess) {
                    $scope.jobGradeCaption = result.object.jobGradeName;
                    //không dùng read dropdown được do ko thể load data lên được khi get detail
                    // var positionSelect = $("#titleInActingPeriodCode").data("kendoDropDownList");
                    // positionSelect.dataSource.read();
                    await getPositions();
                    setDataDepartment(allDepartments, "departmentId");
                    $timeout(function () {
                        $scope.detail.userId = result.object.id;
                        $scope.detail.fullName = result.object.fullName;
                        $scope.detail.userSAPCode = result.object.sapCode;
                        $scope.detail.currentPosition = result.object.jobTitle;
                        $scope.detail.currentJobGrade = result.object.jobGradeName;
                        $scope.detail.deptCode = result.object.deptCode;
                        $scope.detail.deptName = result.object.deptName;
                        $scope.detail.divisionName = result.object.divisionName;
                        $scope.detail.divisionCode = result.object.divisionCode;
                        $scope.detail.workLocationCode = result.object.workLocationCode;
                        $scope.detail.workLocationName = result.object.workLocationName;
                        $scope.detail.startingDate = result.object.startDate ? result.object.startDate : null;
                        $scope.detail.deptLine = result.object.divisionCode ? result.object.divisionName : result.object.deptName;
                        $scope.detail.jobGradeName = result.object.jobGradeName;
                        $scope.detail.jobGradeValue = result.object.jobGradeValue;

                        $scope.detail.newDeptOrLineCode = '';
                        $scope.detail.newDeptOrLineName = '';
                        $scope.detail.isStoreNewDepartment = '';
                        $scope.detail.newJobGradeId = ''
                        $scope.detail.newJobGradeName = '';
                        $scope.detail.payScaleArea = '';
                        $scope.detail.employeeSubgroup = ''
                        $scope.detail.personnelAreaText = '';
                        $scope.detail.personnelArea = '';
                        $scope.detail.employeeGroup = "";
                        $scope.detail.employeeGroupDescription = "";
                        $scope.detail.employeeSubgroupDescription = "";
                        $scope.detail.departmentSAPId = "";
                        setDataEmployeesGroup([]);

                    }, 0)
                    if (result.object.deptId) {
                        $scope.detail.userDepartmentId = result.object.deptId;
                    }
                    else {
                        $scope.detail.userDepartmentId = result.object.divisionId;
                    }
                }
            }
        }
    };

    function findUserById(userId) {
        var result;
        $scope.users.data.forEach(item => {
            if (item.id === userId) {
                result = item;
                return;
            }
        });
        return result;
    }

    function initSapCode(data) {
        var dropdownlist = $("#sapCode").data("kendoDropDownList");
        dropdownlist.setDataSource(data);
    }
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
                $scope.dataUser.push(element);
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
    async function getAppraiser1s(option) {
        var filter = option.data.filter && option.data.filter.filters.length ? option.data.filter.filters[0].value : "";
        var res = await settingService.getInstance().users.getUsers({
            predicate: "(sapcode.contains(@0) or fullName.contains(@1)) and IsActivated = @2",
            predicateParameters: [filter, filter, true],
            page: 1,
            limit: appSetting.pageSizeDefault,
            order: "sapcode asc"
        }).$promise;
        if (res.isSuccess) {
            $scope.Appraiser1s = [];
            res.object.data.forEach(element => {
                $scope.Appraiser1s.push(element);
            });

            if ($scope.firstAppraiserId) {
                let index = _.findIndex($scope.Appraiser1s, x => {
                    return x.id == $scope.detail.firstAppraiserId;
                });
                if (index == -1) {
                    $scope.Appraiser1s.push({ id: $scope.detail.firstAppraiserId, fullName: $scope.detail.firstAppraiserFullName, sapCode: $scope.detail.firstAppraiserSAPCode });
                }
                $scope.firstAppraiserId = '';
            }
        }
        if (option) {
            option.success($scope.Appraiser1s);
        }
    }
    async function getAppraiser2s(option) {
        var filter = option.data.filter && option.data.filter.filters.length ? option.data.filter.filters[0].value : "";
        var res = await settingService.getInstance().users.getUsers({
            predicate: "(sapcode.contains(@0) or fullName.contains(@1)) and IsActivated = @2",
            predicateParameters: [filter, filter, true],
            page: 1,
            limit: appSetting.pageSizeDefault,
            order: "sapcode asc"
        }).$promise;
        if (res.isSuccess) {
            $scope.Appraiser2s = [];
            res.object.data.forEach(element => {
                $scope.Appraiser2s.push(element);
            });
            if ($scope.secondAppraiserId) {
                let index = _.findIndex($scope.Appraiser2s, x => {
                    return x.id == $scope.detail.secondAppraiserId;
                });
                if (index == -1) {
                    $scope.Appraiser2s.push({ id: $scope.detail.secondAppraiserId, fullName: $scope.detail.secondAppraiserFullName, sapCode: $scope.detail.secondAppraiserSAPCode });
                }
                $scope.secondAppraiserId = '';
            }
        }
        if (option) {
            option.success($scope.Appraiser2s);
        }
    }

    $scope.titleInActingPeriodss = [];
    //Data Title in Acting Period
    $scope.titleInActingPeriodsDataSource = {
        //optionLabel: "Select location...",
        dataTextField: 'positionName',
        dataValueField: 'id',
        template: '<span><label>#: data.positionName#</label></span>',
        valueTemplate: '#: positionName #',
        filter: "contains",
        dataSource: $scope.titleInActingPeriods,
        // customFilterFields: ['code', 'name'],
        // filtering: filterMultiField
    };

    $scope.jobTitlesDataSource = {
        //optionLabel: "Please Select...",
        dataTextField: 'name',
        dataValueField: 'id',
        autoBind: true,
        valuePrimitive: true,
        filter: "contains",
        // customFilterFields: ['code', 'name'],
        // filtering: filterMultiField,
        dataSource: {
            serverPaging: false,
            pageSize: 1000,
            transport: {
                read: async function (e) {
                    if ($scope.jobGradeCaption) {
                        let queryArgs = {
                            predicate: '',
                            predicateParameters: [],
                            order: appSetting.ORDER_GRID_DEFAULT,
                            page: 1,
                            limit: appSetting.pageSizeDefault
                        };

                        queryArgs.predicate = 'MetadataType.Value = @0 and JobGrade.Caption = @1';
                        queryArgs.predicateParameters.push('Position');
                        queryArgs.predicateParameters.push($scope.jobGradeCaption);
                        queryArgs.predicate = queryArgs.predicate + ' and ' + '(Name.contains(@2))';
                        queryArgs.predicateParameters.push('acting');
                        queryArgs.limit = 1000;


                        var result = await settingService.getInstance().recruitment.getPositionLists(queryArgs).$promise;
                        if (result && result.isSuccess) {
                            e.success(result.object.data);
                        }
                    }
                    else {
                        let dataEmpty = [];
                        e.success(dataEmpty);
                    }
                }
            },
        },
        change: async function (e) {
            $scope.detail.titleInActingPeriodName = e.sender.text();
        }
    };

    $scope.actingPeriods = [];

    function initTitleInActingPeriod(data) {
        var dropdownlist = $("#titleInActingPeriodCode").data("kendoDropDownList");
        dropdownlist.setDataSource(data);
    }
    //Data 1stAppraiser
    $scope.appraiser1stDataSource = {
        dataTextField: 'sapCode',
        dataValueField: 'id',
        template: showCustomSapCodeTitle,
        valueTemplate: '#: sapCode # - #: fullName #',
        autoBind: false,
        valuePrimitive: true,
        filter: "contains",
        dataSource: {
            serverFiltering: true,
            transport: {
                read: async function (e) {
                    await getAppraiser1s(e);
                }
            },
            schema: {
                data: () => { return $scope.Appraiser1s }
            }
        }
    };

    function initAppraiser1st(data) {
        var dropdownlist = $("#appraiser1st").data("kendoDropDownList");
        dropdownlist.setDataSource(data);
    }

    //Data 2nd Appraiser
    $scope.appraiser2ndDataSource = {
        dataTextField: 'sapCode',
        dataValueField: 'id',
        template: showCustomSapCodeTitle,
        valueTemplate: '#: sapCode # - #: fullName #',
        autoBind: false,
        valuePrimitive: true,
        filter: "contains",
        dataSource: {
            serverFiltering: true,
            transport: {
                read: async function (e) {
                    await getAppraiser2s(e);
                }
            },
            schema: {
                data: () => { return $scope.Appraiser2s }
            }
        }
    };

    function initAppraiser2nd(data) {
        var dropdownlist = $("#appraiser2nd").data("kendoDropDownList");
        dropdownlist.setDataSource(data);
    }

    //Get User 
    $scope.users = [];

    async function getUsersFromEdoc() {
        var res = await settingService.getInstance().users.getAllUser().$promise;
        if (res.isSuccess) {
            $scope.users = res.object;
            if ($stateParams.action.state !== commonData.stateActing.approve) {
                initAppraiser1st($scope.users);
                initAppraiser2nd($scope.users);
            }
        }
    }

    // List Data 
    $scope.data = [];


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
    $scope.currentQuery = {
        predicate: "",
        predicateParameters: [],
        order: "Status desc, Created desc",
        limit: appSetting.pageSizeDefault,
        page: 1
    };
    async function getActings(option) {
        //var userId = $scope.userId;    
        $scope.currentQuery = $scope.buildQuery();;
        if (option) {
            $scope.currentQuery.Limit = option.data.take;
            $scope.currentQuery.Page = option.data.page;
        } 

        var result = await recruitmentService.getInstance().actings.getActings($scope.currentQuery).$promise;
        if (result.isSuccess && result) {
            $scope.data = result.object.data;
            $scope.data.forEach(item => {
                item.created = new Date(item.created);
                // item['titleInActingPeriodName'] = item.titleInActingPeriodCode + ' - ' + item.titleInActingPeriodName;
                if (item.mapping) {
                    item['userDeptName'] = item.mapping.departmentName;
                }
            })
            $scope.total = result.object.count;

            if (option) {
                option.success($scope.data);
            }
            else {
                let grid = $("#gridRequests").data("kendoGrid").setDataSource({
                    serverPaging: true,
                    pageSize: 20,
                    data: $scope.data,
                    transport: {
                        read: async function (e) {
                            await getActings(e);
                        }
                    },
                    schema: {
                        total: () => { return $scope.total },
                        data: () => { return $scope.data }
                    }
                });
            }
        }
        else {
            let grid = $("#gridRequests").data("kendoGrid");
            grid.dataSource.read();
            grid.dataSource.page($scope.currentQuery.page);
        } 
    }

    //function initGridRequests(dataSource, total, pageIndex, pageSize) {
    //    //let grid = $("#gridRequests").data("kendoGrid");
    //    //let dataSourceRequests = new kendo.data.DataSource({
    //    //    data: dataSource,
    //    //    //pageSize: appSetting.pageSizeDefault,
    //    //    pageSize: pageSize,
    //    //    page: pageIndex,
    //    //    serverPaging: true, 
    //    //    //transport: {
    //    //    //    read: async function (e) {
    //    //    //        $scope.optionSave = e;
    //    //    //        await getActings(e.page, $scope.userId, e.pageSize);
    //    //    //    }
    //    //    //},
    //    //    schema: {
    //    //        total: function () {
    //    //            return total;
    //    //        }
    //    //    },
    //    //});
    //    //grid.setDataSource(dataSourceRequests);
    //    //if ($scope.advancedSearchMode) {
    //    //    grid.dataSource.page(1);
    //    //}
    //    let grid = $("#gridRequests").data("kendoGrid");
    //    let dataSourceRequests = new kendo.data.DataSource({
    //        data: dataSource,
    //        serverPaging: true, 
    //        // pageSize: appSetting.pageSizeDefault,
    //        pageSize: pageSize,
    //        page: pageIndex,
    //        //transport: {
    //        //    read: async function (e) {
    //        //        $scope.optionSave = e; 
    //        //        await getActings(e, $scope.userId);
    //        //    }
    //        //},
    //        //schema: {
    //        //    total: function () {
    //        //        return total;
    //        //    }
    //        //},
    //    });
    //    //grid.kendoGrid({
    //    //    dataSource: dataSourceRequests,
    //    //    pageable: true
    //    //});
    //    grid.setDataSource(dataSourceRequests);
    //    if ($scope.advancedSearchMode) {
    //        grid.dataSource.page(1);
    //    }
    //}

    $scope.positionId = ''
    $scope.actingId = '';
    $scope.isCreated = true;
    async function getActingByReferenceNumber(referenceNumber) {
        let QueryArgs = {
            predicate: '',
            predicateParameters: [],
            order: appSetting.ORDER_GRID_DEFAULT,
            page: '',
            limit: ''
        };
        //
        let model = {
            queryArgs: QueryArgs,
            id: $stateParams.id
        }
        //
        var result = await recruitmentService.getInstance().actings.getActingByReferenceNumber(model).$promise;
        if (result.isSuccess) {
            $scope.model.id = model.id;
            $scope.jobGradeCaption = result.object.data.acting.jobGradeName;
            //không dùng read dropdown được do ko thể load data lên được khi get detail
            // var positionSelect = $("#titleInActingPeriodCode").data("kendoDropDownList");
            // positionSelect.dataSource.read();
            await getPositions();
            $scope.positionId = result.object.data.acting.positionId;

            if (result.object.data.acting.status !== "Draft") {
                $scope.isCreated = false;
                if (result.object.data.acting.employeeSubgroup && result.object.data.acting.employeeSubgroup !== "") {
                    await getEmployeeData(result.object.data.acting.employeeSubgroup);
                }
            }

            $timeout(async function () {
                $scope.detail = _.cloneDeep(result.object.data.acting);
                if ($scope.detail.departmentSAPId == '00000000-0000-0000-0000-000000000000') {
                    $scope.detail.departmentSAPId = null;
                }
                // $scope.detail.userId = result.object.data.acting.userId;
                $scope.selectedUserId = $scope.detail.userId;
                $scope.firstAppraiserId = $scope.detail.firstAppraiserId;
                $scope.secondAppraiserId = $scope.detail.secondAppraiserId;
                //$scope.detail.deptLine = $scope.detail.deptName ? $scope.detail.deptName : $scope.detail.divisionName;
                $scope.detail.deptLine = $scope.detail.divisionCode ? $scope.detail.divisionName : $scope.detail.deptCode ? $scope.detail.deptName : '';
                $scope.detail.period1From = $scope.detail.period1From ? new Date($scope.detail.period1From) : null;
                $scope.detail.period1To = $scope.detail.period1To ? new Date($scope.detail.period1To) : null;
                $scope.detail.period2From = $scope.detail.period2From ? new Date($scope.detail.period2From) : null;
                $scope.detail.period2To = $scope.detail.period2To ? new Date($scope.detail.period2To) : null;
                $scope.detail.period3From = $scope.detail.period3From ? new Date($scope.detail.period3From) : null;
                $scope.detail.period3To = $scope.detail.period3To ? new Date($scope.detail.period3To) : null;
                $scope.detail.period4From = $scope.detail.period4From ? new Date($scope.detail.period4From) : null;
                $scope.detail.period4To = $scope.detail.period4To ? new Date($scope.detail.period4To) : null;
                var cloneWorkLocationCode = $scope.detail.newWorkLocationCode;
                var dropdownlist = $("#sapCode").data("kendoDropDownList");
                dropdownlist.dataSource.add({ id: $scope.detail.userId, fullName: $scope.detail.fullName, sapCode: $scope.detail.employeeCode });
                if ($stateParams.id) {
                    mapGoalGrid(JSON.parse(result.object.data.acting.templateGoal));
                    initGridGoal($scope.goalDataSource);
                    if (!$scope.model.isCompletedTranning) {
                        mapCompulsoryTrainingGrid(JSON.parse(result.object.data.acting.tableCompulsoryTraining));
                        initGridCompulsoryTraining($scope.tableCompulsoryTraining);
                    }
                    mapPeriod(result.object.data.period);
                    mapAppraising(result.object.data.period);
                    $scope.titleEdit = "ACTING: " + result.object.data.acting.referenceNumber;
                }

                let actingItem = result.object.data.acting;
                if (actingItem.departmentSAPId != null && actingItem.departmentSAPId != '00000000-0000-0000-0000-000000000000') {
                    await getDepartmentSAPById(actingItem.departmentSAPId);
                    $scope.detail.departmentSAPId = actingItem.departmentSAPId;
                }
                else {
                    setDataDepartment(allDepartments, "departmentSAPId");
                }
                if ($scope.detail.newPersonnelArea !== null || $scope.detail.newPersonnelArea !== '') {
                    setSubPersonelArea($scope.detail.newPersonnelArea);
                    $scope.detail.newWorkLocationCode = cloneWorkLocationCode
                }
            }, 0);
        }
    }

    function findTitleInActingPeriodById(id) {
        var result;
        $scope.titleInActingPeriods.forEach(item => {
            if (item.id === id) {
                result = item;
                return result;
            }
        });
        return result;
    }

    function mapGoalGrid(data) {
        $timeout(function () {
            $scope.goalDataSource = [];
            data = JSON.parse(data);
            // data.forEach(item => {
            //     $scope.goalDataSource.push(item);
            // });
            var goalGridCurrent = $("#goalGrid").data("kendoGrid");
            var dataSource = new kendo.data.DataSource({
                data: data
            });
            goalGridCurrent.setDataSource(dataSource);
            //$scope.goalDataSource = new kendo.data.ObservableArray(data);

        }, 0);
    }

    function mapCompulsoryTrainingGrid(data) {
        data = JSON.parse(data);
        data.forEach(item => {
            $scope.tableCompulsoryTraining.push(item);
        });
    }

    $scope.actingPeriodFrom = '';
    $scope.actingPeriodTo = '';

    async function mapPeriod(periods) {
        // periods.forEach(item => {
        //     switch (item.priority) {
        //         case 1:
        //             $scope.detail.period1From = new Date(item.fromDate).toLocaleDateString("en-GB");
        //             $scope.actingPeriodFrom = $scope.detail.period1From;
        //             $scope.detail.period1To = new Date(item.toDate).toLocaleDateString("en-GB");
        //             //initDatePicker('#period1From', $scope.detail.period1From, '#period1To', $scope.detail.period1To)
        //             break;
        //         case 2:
        //             $scope.detail.period2From = new Date(item.fromDate).toLocaleDateString("en-GB");
        //             $scope.detail.period2To = new Date(item.toDate).toLocaleDateString("en-GB");
        //             //initDatePicker('#period2From', $scope.detail.period2From, '#period2To', $scope.detail.period2To)
        //             break;
        //         case 3:
        //             $scope.detail.period3From = new Date(item.fromDate).toLocaleDateString("en-GB");
        //             $scope.detail.period3To = new Date(item.toDate).toLocaleDateString("en-GB");
        //             //initDatePicker('#period3From', $scope.detail.period3From, '#period3To', $scope.detail.period3To)
        //             break;
        //         case 4:
        //             $scope.detail.period4From = new Date(item.fromDate).toLocaleDateString("en-GB");
        //             $scope.detail.period4To = new Date(item.toDate).toLocaleDateString("en-GB");
        //             //initDatePicker('#period4From', $scope.detail.period4From, '#period4To', $scope.detail.period4To)
        //             break;
        //         default:
        //             break;
        //     }
        // });
        $scope.actingPeriodTo = checkActingPeriodTo();
    }

    function checkActingPeriodTo() {

        if ($scope.detail.period4To != '') {
            return $scope.detail.period4To;
        }
        if ($scope.detail.period3To != '') {
            return $scope.detail.period3To;
        }
        if ($scope.detail.period2To != '') {
            return $scope.detail.period2To;
        }
        if ($scope.detail.period1To != '') {
            return $scope.detail.period1To;
        }
    }

    async function initDatePicker(idFrom, valueFrom, idTo, valueTo) {
        if ($stateParams.action.state === commonData.stateActing.item) {
            var datepickerFrom = $(idFrom).data("kendoDatePicker");
            datepickerFrom.value(valueFrom);
            //
            var datepickerTo = $(idTo).data("kendoDatePicker");
            datepickerTo.value(valueTo);
        }
    }

    //Form detail
    // $scope.detail = {
    //     userId: '',
    //     sapCode: "",
    //     position: "",
    //     titleInActingPeriodName: "",
    //     appraiser1st: "",
    //     appraiser2nd: "",
    //     fullName: '',
    //     deptLine: '',
    //     workLocation: '',
    //     currentTitle: '',
    //     workLocationCode: '',
    //     period1From: '',
    //     period1To: '',
    //     period2From: '',
    //     period2To: '',
    //     period3From: '',
    //     period3To: '',
    //     period4From: '',
    //     period4To: '',
    // };

    function mapAppraising(periods) {
        $scope.appraisingDataSource.forEach(item => {
            var result = periods.find(x => x.priority === parseInt(item.period));
            if (result) {
                item.timeRange = ': ' + new Date(result.fromDate).toLocaleDateString("en-GB") + ' - ' + new Date(result.toDate).toLocaleDateString("en-GB");
                var list = JSON.parse(result.appraising);
                list.forEach(x => {
                    item.appraising.push(x)
                });
            }
        });

        if ($stateParams.action.state === commonData.stateActing.item) {
            initGridMainAppraisingGridOptions($scope.appraisingDataSource);
        }
        if ($stateParams.action.state === commonData.stateActing.approve) {
            initGridMainAppraisingGridOptionsApprove($scope.appraisingDataSource);
        }
    }

    function initGridMainAppraisingGridOptions(dataSource) {
        let grid = $("#mainAppraisingGridOptions").data("kendoGrid");
        let dataSourceRequests = new kendo.data.DataSource({
            data: dataSource,
        });
        grid.setDataSource(dataSourceRequests);
    }

    function initGridMainAppraisingGridOptionsApprove(dataSource) {
        let grid = $("#mainAppraisingGridOptionsApprove").data("kendoGrid");
        let dataSourceRequests = new kendo.data.DataSource({
            data: dataSource,
        });
        grid.setDataSource(dataSourceRequests);
    }

    function initGridGoal(dataSource) {
        let grid = $("#goalGrid").data("kendoGrid");
        if (dataSource.length > 0) {
            let dataSourceRequests = new kendo.data.DataSource({
                data: dataSource,
                pageSize: appSetting.pageSizeDefault,
                schema: {
                    total: function () {
                        return dataSource.length;
                    }
                },
            });
            grid.setDataSource(dataSourceRequests);
        }
    }

    function initGridCompulsoryTraining(dataSource) {
        let grid = $("#compulsoryGrid").data("kendoGrid");
        if (dataSource.length > 0) {
            let dataSourceRequests = new kendo.data.DataSource({
                data: dataSource,
            });
            grid.setDataSource(dataSourceRequests);
        }
    }

    //List Acting

    $scope.actingDataSource = {
        serverPaging: true,
        pageSize: 20,
        transport: {
            read: async function (e) {
                await getActings(e);
            }
        },
        schema: {
            total: () => { return $scope.total },
            data: () => { return $scope.data }
        }
    };
    $scope.actingGridOptions = {
        dataSource: {
            serverPaging: true,
            pageSize: 20,
            transport: {
                read: async function (e) {
                    await getActings(e);
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
            title: "Status",
            locked: true,
            lockable: false,
            width: "350px",
            template: function (dataItem) {
                var statusTranslate = $rootScope.getStatusTranslate(dataItem.status);
                //return `<workflow-status status="${statusTranslate}"></workflow-status>`;
                return `<workflow-status status="${dataItem.status}"></workflow-status>`;
            }
        },
        {
            field: "referenceNumber",
            title: "Reference Number",
            locked: true,
            width: "180px",
            template: function (dataItem) {
                if (
                    dataItem.status === commonData.StatusMissingTimeLock.WaitingCMD ||
                    dataItem.status === commonData.StatusMissingTimeLock.WaitingHOD
                ) {
                    return `<a ui-sref="home.action.itemApprove({referenceValue:'${dataItem.referenceNumber}', id: '${dataItem.id}'})" ui-sref-opts="{ reload: true }">${dataItem.referenceNumber}</a>`;
                }
                return `<a ui-sref="home.action.item({referenceValue:'${dataItem.referenceNumber}', id: '${dataItem.id}'})" ui-sref-opts="{ reload: true }">${dataItem.referenceNumber}</a>`;

                // return `<a ui-sref= home.action.item({referenceValue:'${dataItem.referenceNumber}'}) ui-sref-opts="{ reload: true }">${dataItem.referenceNumber}</a>`;
            }
        },
        {
            field: "userSAPCode",
            title: "SAP Code",
            width: "200px"
        }, {
            field: "userFullName",
            title: "Full Name",
            width: "250px",
        },
        {
            field: "divisionName",
            // title: "Dept/Line",
            title: "Department", // fix ticket 135
            width: "350px",
            template: function (dataItem) {
                if (dataItem.deptName) {
                    return dataItem.deptName;
                } else if (dataItem.divisionName) {
                    return dataItem.divisionName;
                } else {
                    return '';
                }
            }
        },
        {
            field: "regionName",
            title: "Region",
            width: "100px"
        },
        {
            field: "workLocationName",
            title: 'Work Location',
            width: "200px"
        }, {
            field: "titleInActingPeriodName",
            title: 'Position in Acting Period',
            width: "250px"
        }, {
            field: "created",
            title: 'Created Date',
            width: "180px",
            template: function (dataItem) {
                if (dataItem && dataItem.created !== null) {
                    return moment(dataItem.created).format('DD/MM/YYYY HH:mm');
                }
                return '';
            }
        }
        ],
        //page: function (e) {
        //    getActings(e.page, $scope.userId);
        //}
    };

    // GOAL

    $scope.goalDataSource = new kendo.data.ObservableArray([{
        id: $scope.goalDataSource.length + 1,
        goal: '',
        weight: '',
        code: ''
    }]);
    $scope.goalGridOptions = {
        dataSource: {
            // autoSync: true, 
            data: $scope.goalDataSource,
            pageSize: 20,
            schema: {
                model: {
                    id: "goal",
                    fields: {
                        // no: { editable: false },
                        goal: { type: "string", editable: true },
                        weight: { type: "number", editable: true }
                    }
                }
            }
        },
        sortable: false,
        editable: true,
        pageable: false,
        columns: [{
            field: "goal",
            title: "Goal",
            width: "250px",
            template: function (dataItem) {
                return `<input class="k-textbox w100" ng-change="onChangeGoal(dataItem)" type="text" ng-model="dataItem.goal"/>`
                //return `<input class="k-textbox w100" type="text" ng-model="dataItem.goal"/>`
            }
        },
        {
            field: "weight",
            title: "Weight(%)",
            width: "130px",
            template: function (dataItem) {
                return `<input id="weightId" kendo-numeric-text-box class="w100" k-ng-model="dataItem.weight"/>`
            }
        },
        {
            field: "",
            title: "",
            width: "150px",
            template: function (dataItem) {
                if (!dataItem.selected) {
                    return `<a class="btn btn-sm default red-stripe" ng-click="deleteRow('goal', dataItem)">Delete</a>`
                }
            }
        }
        ],
        // selectable: true,
        // cellClose: function (e) {
        //     console.log(e);
        //     syncAppraising(e);
        //     // e.preventDefault();
        // },
    };

    $scope.appraisingDetailGridOptions = function (dataItem) {
        return {
            dataSource: {
                data: dataItem.appraising,
                pageSize: 500,
                schema: {
                    model: {
                        id: "goal",
                        fields: {
                            goal: { editable: false },
                            actual: { editable: false },
                        }
                    }
                }
            },
            editable: true,
            columns: [{
                field: "goal",
                title: "Goal",
                width: "280px"
            },
            {
                field: "target",
                title: "Target",
                width: "180px",
                template: function (dataItem) {
                    return `<input class="k-textbox w100" type="text" ng-model="dataItem.target"/>`
                }
            },
            {
                field: "actual",
                title: 'Actual',
                width: "180px",
                template: function (dataItem) {
                    return `<input class="k-textbox form-control w100 input-default input-sm" aria-owns="actual" id='actual' type="text" ng-model="dataItem.actual" disabled="disabled"/>`
                }
            }
            ],
        };
    }

    $scope.appraisingDetailGridOptionsApprove = function (dataItem) {
        return {
            dataSource: {
                data: dataItem.appraising,
                pageSize: 500,
                schema: {
                    model: {
                        id: "goal",
                        fields: {
                            actual: { editable: false },
                        }
                    }
                }
            },
            columns: [{
                field: "goal",
                title: "Goal",
                width: "280px"
            },
            {
                field: "target",
                title: "Target",
                width: "180px"
            },
            {
                field: "actual",
                title: 'Actual',
                width: "180px"
            }
            ],
        };
    }

    //Compulsory training
    $scope.onChange = function (value) {
        if (value) {
            $scope.tableCompulsoryTraining = [];
        }
    }
    //CR210=============
    $scope.confirmDataSource1 = [{
        name: "Passed",
        code: "Passed1"
    },
    {
        name: "Failed",
        code: "Failed1"
    }
    ];
    $scope.confirmDataSource2 = [{
        name: "Passed",
        code: "Passed2"
    },
    {
        name: "Failed",
        code: "Failed2"
    }
    ];
    $scope.detail.firstAppraiserNote = null;
    $scope.detail.secondAppraiserNote = null;

    //=============

    $scope.model.tableCompulsoryTraining = [];

    $scope.compulsoryGridOptions = {
        dataSource: $scope.model.tableCompulsoryTraining,
        editable: true,
        columns: [{
            field: "no",
            title: "No.",
            width: "30px",
            editable: function () {
                return false;
            },
        }, {
            field: "courseName",
            title: "Course Name",
            width: "180px",
            template: function (dataItem) {
                return `<input class="k-textbox w100" type="text" ng-model="dataItem.courseName"/>`
            }
        },
        {
            field: "duration",
            title: "Duration",
            width: "100px",
            template: function (dataItem) {
                return `<input class="k-textbox w100" type="text" ng-model="dataItem.duration"/>`
            }
        }, {
            field: "actualResult",
            title: 'Actual Result',
            width: "100px",
            template: function (dataItem) {
                return `<input class="k-textbox w100 readonly input-default" disabled  type="text" ng-model="dataItem.actualResult"/>`
            }
        },
        {
            field: "",
            title: "",
            width: "150px",
            template: function (dataItem) {
                if (!dataItem.selected) {
                    return `<a class="btn btn-sm default red-stripe" ng-click="deleteRow('compulsoryTraining', dataItem)">Delete</a>`
                }
            }
        }
        ]
    };

    //Main Appraising
    $scope.mainAppraisingGridOptions = {
        dataSource: {
            data: $scope.appraisingDataSource,
            pageSize: 20,
        },
        scrollable: false,
        // detailInit: detailInit,
        dataBound: function (e) {
            // e.sender.thead.hide();
            // this.expandRow(this.tbody.find("tr.k-master-row").first());
            this.expandRow(this.tbody.find("tr.k-master-row"));
        },
        columns: [{
            field: "",
            title: "",
            width: "900px",
            template: function (dataItem) {
                return `<div>Period ${dataItem.period}${dataItem.timeRange}</div>`
            }
        },]
    };

    $scope.mainAppraisingGridOptionsApprove = {
        dataSource: {
            data: $scope.appraisingDataSource,
            pageSize: 20,
        },
        scrollable: false,
        // detailInit: detailInit,
        dataBound: function (e) {
            // e.sender.thead.hide();
            // this.expandRow(this.tbody.find("tr.k-master-row").first());
            this.expandRow(this.tbody.find("tr.k-master-row"));
        },
        columns: [{
            field: "",
            title: "",
            width: "900px",
            template: function (dataItem) {
                return `<div>Period ${dataItem.period}${dataItem.timeRange}</div>`
            }
        },]
    };

    //Reset Form Search
    $scope.resetSearch = async function () {
        $scope.filter.keyWord = '';
        $scope.filter.departmentId = '';
        $scope.filter.status = '';
        $scope.filter.fromDate = null;
        $scope.filter.toDate = null;
        $scope.$broadcast('resetToDate', $scope.filter.toDate);
        $scope.currentQuery = {
            predicate: "",
            predicateParameters: [],
            order: "Status desc, Created desc",
            limit: appSetting.pageSizeDefault,
            page: 1
        };
        await getActings();
        clearSearchTextOnDropdownTree("departmentId");
        setDataDepartment(allDepartments, "departmentId");
        setDataDepartment(allDepartments, "departmentSAPId");
    }

    $scope.addRow = function (item) {
        if (item === 'goal') {
            var goalGridCurrent = $("#goalGrid").data("kendoGrid");
            $scope.goalDataSource = goalGridCurrent.dataSource._data;
            let value = {
                id: $scope.goalDataSource.length + 1,
                goal: '',
                weight: '',
                code: ''
            };
            $scope.goalDataSource.push(value);
        }
        if (item === 'appraising') {
            var grid = $("#appraisingGrid").data("kendoGrid");
            grid.dataSource.add({
                goal: '',
                target: '',
                actual: ''
            });
            // $scope.appraisingDataSource
        }
        if (item === 'compulsoryTraining') {
            var gridcompulsoryTrainingCurrent = $("#compulsoryGrid").data("kendoGrid");
            $scope.tableCompulsoryTraining = gridcompulsoryTrainingCurrent.dataSource._data;
            let value = {
                no: $scope.tableCompulsoryTraining.length + 1,
                courseName: '',
                duration: '',
                actualResult: ''
            };
            $scope.tableCompulsoryTraining.push(value);
            var grid = $("#compulsoryGrid").data("kendoGrid");
            let dataSource = new kendo.data.DataSource({
                data: $scope.tableCompulsoryTraining,
            });
            grid.setDataSource(dataSource);
        }
    };

    $scope.id;
    $scope.type;

    confirm = function (e) {
        if (e.data && e.data.value) {
            switch ($scope.type) {
                case 'goal':
                    var currentGoalDataSource = $("#goalGrid").data("kendoGrid").dataSource._data;
                    var code;
                    for (var i = 0; i < currentGoalDataSource.length; i++) {
                        if (currentGoalDataSource[i].code === $scope.id) {
                            code = currentGoalDataSource[i].code;
                            currentGoalDataSource.splice(i, 1);
                            syncAppraising(code);
                            break;
                        }
                    }
                    Notification.success("Data Sucessfully Deleted");
                    break;
                case 'compulsoryTraining':
                    var grid = $("#compulsoryGrid").data("kendoGrid");
                    let currentLength = grid.dataSource._data.length;
                    grid.dataSource._data.splice($scope.id - 1, 1);
                    if ($scope.id < currentLength) {
                        grid.dataSource._data.forEach(item => {
                            if (item.no > $scope.id) item.no--;
                        });
                    }
                    Notification.success("Data Sucessfully Deleted");
                    break;
                default:
                    break;
            }

        }
    }

    $scope.deleteRow = function (title, dataItem) {
        if (title === 'goal') {
            $scope.id = dataItem.code;
            $scope.type = 'goal';
            $scope.dialog = $rootScope.showConfirmDelete("CLOSE", 'Are you sure to delete this Goal ?', 'Confirm');
            $scope.dialog.bind("close", confirm);
        }

        if (title === 'compulsoryTraining') {
            $scope.id = dataItem.no;
            $scope.type = 'compulsoryTraining';
            $scope.dialog = $rootScope.showConfirmDelete("CLOSE", 'Are you sure to delete this Compulsory Training ?', 'Confirm');
            $scope.dialog.bind("close", confirm);

        }
    }

    $scope.goBack = function () {
        $state.go('home.dashboard', {}, { reload: true });
    }
    async function checkCompulsoryTraining(item) {
        var res = await settingService.getInstance().users.seachEmployee({ SAPCode: item }).$promise;
        $timeout(function () {
            if (res.object.jobGradeCode === commonData.jobGrade) {
                $scope.model.isCompletedTranning = false;
            } else {
                $scope.model.isCompletedTranning = true;
                $scope.tableCompulsoryTraining = new kendo.data.ObservableArray([]);
            }
        }, 0);
    }

    $scope.onChangePeriod = function (event, model, range, isFrom) {
        syncAppraising();
        if (moment($scope.detail[model], 'DD/MM/YYYY').isValid()) {
            $scope.dateRange[range] = moment($scope.detail[model], 'DD/MM/YYYY').format('MM/DD/YYYY');
        } else {
            if (isFrom)
                $scope.dateRange[range] = '01/01/2000';
            else
                $scope.dateRange[range] = '12/31/2030';
        }
    }
    $scope.onChangeGoal = function (e) {
        syncAppraising();
    }

    function syncAppraising(goalCode) {
        for (let i = 0; i < $scope.appraisingDataSource.length; i++) {
            if (moment($scope.detail['period' + (i + 1) + 'From'], 'DD/MM/YYYY').isValid() && moment($scope.detail['period' + (i + 1) + 'To'], 'DD/MM/YYYY').isValid()) {
                $scope.appraisingDataSource[i].timeRange = ': ' + moment($scope.detail['period' + (i + 1) + 'From']).format('DD/MM/YYYY') + ' - ' + moment($scope.detail['period' + (i + 1) + 'To']).format('DD/MM/YYYY');
                refreshAppraisingGrid($scope.appraisingDataSource[i], goalCode);
            } else {
                $scope.appraisingDataSource[i].timeRange = '';
                $scope.appraisingDataSource[i].appraising = new kendo.data.ObservableArray([]);
            }
        }

        $scope.inst.appraising.dataSource.read();
    }

    function refreshAppraisingGrid(target, goalCode) {
        //ngan
        let currentGoalDataSource = $("#goalGrid").data("kendoGrid").dataSource._data;
        currentGoalDataSource.map(function (e) {
            if (e.code == "") {
                return e.code = e.uid;
            }
            return e.code;
        });
        if (goalCode) {
            for (var i = 0; i < target.appraising.length; i++) {
                if (target.appraising[i].code == goalCode) {
                    target.appraising.splice(i, 1);
                    break;
                }
            }
        }
        else {
            currentGoalDataSource.forEach(goal => {
                let found = target.appraising.find(appr => appr.code === goal.code);
                if (found) {
                    found.goal = goal.goal;
                    found.weight = goal.weight;
                } else {
                    target.appraising.push({
                        code: goal.code,
                        goal: goal.goal,
                        weight: '',
                        target: '',
                        actual: '',
                    });
                }
            });
        }
        //end
    }

    //Map vao model 
    function mapDataSource(nameType) {
        switch (nameType) {
            case 'goal':
                let mappings = [];
                $scope.goalDataSource.forEach(x => {
                    mappings.push({ goal: x.goal, weight: x.weight, code: x.code });
                });
                $scope.model.goalDataSource = JSON.stringify(mappings);
                break;
            case 'appraising':
                $scope.model.appraisingDataSource = [];
                $scope.appraisingDataSource.forEach(item => {
                    let appraisingData = {};
                    if (item.timeRange) {
                        var fromDate = moment(splidStringDate(item.timeRange, 1), "DD/MM/YYYY");
                        var toDate = moment(splidStringDate(item.timeRange, 2), "DD/MM/YYYY")
                        appraisingData['fromDate'] = fromDate.toDate();
                        appraisingData['toDate'] = toDate.toDate();
                        appraisingData['priority'] = item.period;
                        let mappingsAppraisings = [];
                        item.appraising.forEach(x => {
                            mappingsAppraisings.push({
                                goal: x.goal,
                                target: x.target,
                                actual: x.actual,
                                code: x.code,
                            })
                        })
                        appraisingData["appraising"] = JSON.stringify(mappingsAppraisings);
                    } else {
                        item['fromDate'] = '';
                        item['toDate'] = '';
                        item['priority'] = item.period;
                        item.appraising = item.appraising;
                    }
                    $scope.model.appraisingDataSource.push(appraisingData);
                });
                break;
            case 'compulsoryTraining':
                let tableCompulsoryTrainingMappings = [];
                $scope.tableCompulsoryTraining.forEach(x => {
                    tableCompulsoryTrainingMappings.push({ courseName: x.courseName, duration: x.duration, actualResult: x.actualResult });
                })
                $scope.model.tableCompulsoryTraining = JSON.stringify(tableCompulsoryTrainingMappings);
                break;
            default:
                break;
        }
    }

    //tách chuỗi để lấy 2 giá trị dateTime của period
    function splidStringDate(array, no) {
        var temporary = array.split("-");
        switch (no) {
            case 1:
                return temporary[0].split(" ", 2)[1];
            case 2:
                return temporary[1].split(" ")[1];
            default:
                break;
        }
    }

    $scope.employees = [];

    async function getEmployees(skip) {
        var res = await employeeService.getInstance().employee.getEmployees({
            functionName: 'Search_empSet',
            top: 10000,
            // filter: `Vorna eq 'Thụy*'`,
            filter: ``,
            skip: skip,
            select: '',
        }).$promise;

        if (res.isSuccess) {
            $scope.employees = res.object;
            $scope.employees.forEach(item => {
                item['fullName'] = item.lastName + ' ' + item.firstName;
            });
            initAppraiser1st($scope.employees);
            initAppraiser2nd($scope.employees);
        }
    }

    //get User By Id 
    $scope.user = '';

    async function getUserById(userid) {
        var model = {
            userId: userid
        }

        if (userid) {

            var result = await settingService.getInstance().users.getUserById(model).$promise;
            var res = await settingService.getInstance().users.seachEmployee({ SAPCode: result.object.sapCode }).$promise;
            if (res.isSuccess) {
                $scope.user = result.object;
                return $scope.user;
            }
        }
    }
    //Map form 
    function mapUser(data) {
        $timeout(function () {
            $scope.detail = {
                sapCode: data.id,
                fullName: data.fullName,
                deptLine: data.deptName,
                divisionName: data.divisionName,
                workLocationName: data.workLocationName,
                currentTitle: data.jobTitle
            }
        }, 0);
    }

    // var columsSearch = ['ReferenceNumber.contains(@0)', 'User.SapCode.contains(@1)', 'User.FullName.contains(@2)', 'WorkLocationName.contains(@3)', 'TitleInActingPeriodName.contains(@4)', 'User.UserDepartmentMappings.Where(y => y.IsHeadCount == true).Any(Department.Name.contains(@5))'];
    var columsSearch = ['ReferenceNumber.contains(@0)', 'UserSAPCode.contains(@1)', 'FullName.contains(@2)', 'WorkLocationName.contains(@3)', 'TitleInActingPeriodName.contains(@4)', 'DeptName.contains(@5)'];

    $scope.buildQuery = function () {
        var option = {
            predicate: "",
            predicateParameters: [],
            order: "Status desc, Created desc",
            limit: appSetting.pageSizeDefault,
            page: 1
        };
         

        if ($state.current.name == commonData.myRequests.Acting) {
            option.predicate = option.predicate ? `${option.predicate} and (CreatedById = @${option.predicateParameters.length})` : `(CreatedById =@${option.predicateParameters.length})`;
            option.predicateParameters.push($scope.userId);
        }
        if ($scope.filter.keyWord) {
            option.predicate = `(${columsSearch.join(" or ")})`;
            for (let index = 0; index < columsSearch.length; index++) {
                option.predicateParameters.push($scope.filter.keyWord);
            }
        }

        if ($scope.filter.departmentId) {
            // option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `User.UserDepartmentMappings.Where(x => x.IsHeadCount == true).Any(Department.id = @${option.predicateParameters.length})`;
            option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `(UserDepartmentId =@${option.predicateParameters.length})`;
            option.predicateParameters.push($scope.filter.departmentId);

        }
        if ($scope.filter.status && $scope.filter.status.length) {
            generatePredicateWithStatus(option, $scope.filter.status);
        }

        if ($scope.filter.fromDate) {
            option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `Created >= @${option.predicateParameters.length}`;
            //option.predicateParameters.push(moment($scope.filter.fromDate).format('YYYY-MM-DD'));
            option.predicateParameters.push((moment($scope.filter.fromDate, 'DD/MM/YYYY').format('YYYY-MM-DD')));

        }
        if ($scope.filter.toDate) {
            option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `Created < @${option.predicateParameters.length}`;
            //option.predicateParameters.push(moment($scope.filter.toDate).add(1, 'days').format('YYYY-MM-DD'));
            option.predicateParameters.push((moment($scope.filter.toDate, 'DD/MM/YYYY').add(1, 'days').format('YYYY-MM-DD')));
        }

        //$scope.currentQuery = option; 
        return option;
    }

    $scope.search = async function () {
        await getActings();
    }

    //$scope.search = async function () {
    //    $scope.currentQuery = {
    //        predicate: "",
    //        predicateParameters: [],
    //        order: "Status desc, Created desc",
    //        limit: appSetting.pageSizeDefault,
    //        page: 1
    //    };
    //    var option = $scope.currentQuery;
    //    if ($scope.filter.keyWord) {
    //        option.predicate = `(${columsSearch.join(" or ")})`;
    //        for (let index = 0; index < columsSearch.length; index++) {
    //            option.predicateParameters.push($scope.filter.keyWord);
    //        }
    //    }

    //    if ($scope.filter.departmentId) {
    //        // option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `User.UserDepartmentMappings.Where(x => x.IsHeadCount == true).Any(Department.id = @${option.predicateParameters.length})`;
    //        option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `(UserDepartmentId =@${option.predicateParameters.length})`;
    //        option.predicateParameters.push($scope.filter.departmentId);

    //    }

    //    if ($state.current.name == commonData.myRequests.Acting) {
    //        option.predicate = option.predicate ? `${option.predicate} and (CreatedById = @${option.predicateParameters.length})` : `(CreatedById =@${option.predicateParameters.length})`;
    //        option.predicateParameters.push($scope.userId);
    //    }

    //    // if ($scope.filter.statusId && $scope.filter.statusId.length > 0) {
    //    //     let statusArray = $scope.filter.statusId.map(x => x.statusName);
    //    //     let statuspredicateArray = $scope.filter.statusId.map((x,index) => `Status = @${option.predicateParameters.length+index}`);
    //    //     let statuspredicate = `(${statuspredicateArray.join(" or ")})`;
    //    //     for (let index = 0; index < statusArray.length; index++) {
    //    //         option.predicateParameters.push(statusArray[index]);
    //    //     }
    //    //     option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + statuspredicate;
    //    // }
    //    if ($scope.filter.status && $scope.filter.status.length) {
    //        generatePredicateWithStatus(option, $scope.filter.status);
    //    }

    //    if ($scope.filter.fromDate) {
    //        option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `Created >= @${option.predicateParameters.length}`;
    //        option.predicateParameters.push(moment($scope.filter.fromDate).format('YYYY-MM-DD'));
    //    }
    //    if ($scope.filter.toDate) {
    //        option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `Created < @${option.predicateParameters.length}`;
    //        option.predicateParameters.push(moment($scope.filter.toDate).add(1, 'days').format('YYYY-MM-DD'));
    //    }

    //    $scope.currentQueryExport = option;

    //    $scope.currentQuery.predicateParameters = option.predicateParameters
    //    $scope.currentQuery.predicate = option.predicate;  

    //    await GetActingByFilter();
    //}

    //async function GetActingByFilter() {
    //    var result = await recruitmentService.getInstance().actings.getActings($scope.currentQuery).$promise;
    //    if (result.isSuccess) {
    //        $scope.data = result.object.data;
    //        $scope.data.forEach(item => {
    //            item.created = new Date(item.created);
    //            item['titleInActingPeriod'] = item.titleInActingPeriodCode + ' - ' + item.titleInActingPeriodName;
    //            if (item.mapping) {
    //                item['userDeptName'] = item.mapping.departmentName;
    //            }
    //        })
    //        initGridRequests($scope.data, result.object.count, 1, 20);
    //        $scope.total = result.object.count
    //    } 
    //}

    async function getDepartmentByReferenceNumber() {
        let referencePrefix = $stateParams.referenceValue.split('-')[0];
        let res = await settingService.getInstance().departments.getDepartmentByReferenceNumber({ prefix: referencePrefix, itemId: $stateParams.id }).$promise;
        if (res.isSuccess) {
            setDataDepartment(res.object.data, "departmentId");
        }
    }

    async function getDepartmentSAPById(departmentSAPId) {
        let res = await settingService.getInstance().departments.getDepartmentById({ id: departmentSAPId }, null).$promise;;
        if (res.isSuccess) {
            setDataDepartment([res.object], "departmentSAPId");
            var dropdowntree = $("#departmentSAPId").data("kendoDropDownTree");
            dropdowntree.value(departmentSAPId);
            dropdowntree.trigger("change");
        }
    }

    //
    $scope.userId = '';
    $scope.ngOnit = async function () {

        if ($state.current.name == commonData.stateActing.myRequests || $state.current.name == commonData.stateActing.allRequests) {
            $scope.userId = $rootScope.currentUser.id;
            //getActings(null, $scope.userId);
            //await getActings(null);
            //$scope.total = $scope.data.length ;
        }
        else {
            if ($stateParams.referenceValue) {
                await getAllWorkLocation();
                await getDepartmentByReferenceNumber();
                await getActingByReferenceNumber($stateParams.referenceValue);
            }
            else {
                $timeout(function () {
                    setDataDepartment(allDepartments);
                }, 0);
            }
            var result = await recruitmentService.getInstance().position.getOpenPositions().$promise;
            if (result.isSuccess) {
                $scope.positions = result.object.data;
            }
        }
        //$scope.$apply();
    }

    $scope.showProcessingStages = function () {
        $rootScope.visibleProcessingStages($translate);
    }
    $scope.ngOnit();

    $scope.currentQueryExport = {
        predicate: "",
        predicateParameters: [],
        order: "Created desc",
        limit: appSetting.pageSizeDefault,
        page: 1
    }

    $scope.export = async function () {
        //let option = $scope.currentQueryExport;
        let option = $scope.currentQuery;
        var res = await fileService.getInstance().processingFiles.export({ type: commonData.exportType.ACTING }, option).$promise; 
        if (res.isSuccess) {
            exportToExcelFile(res.object);
            Notification.success(appSetting.notificationExport.success);
        } else {
            Notification.error(appSetting.notificationExport.error);
        }
    }
    $scope.printForm = async function () {
        let res = await recruitmentService.getInstance().actings.printForm({ id: id }).$promise;
        if (res.isSuccess) {
            exportToExcelFile(res.object);
        }
    }

    $scope.positions = [];
    async function getPositions() {
        if ($scope.jobGradeCaption) {
            let queryArgs = {
                predicate: '',
                predicateParameters: [],
                order: appSetting.ORDER_GRID_DEFAULT,
                page: 1,
                limit: appSetting.pageSizeDefault
            };

            queryArgs.predicate = 'MetadataType.Value = @0 and JobGrade.Caption = @1';
            queryArgs.predicateParameters.push('Position');
            queryArgs.predicateParameters.push($scope.jobGradeCaption);
            queryArgs.predicate = queryArgs.predicate + ' and ' + '(Name.contains(@2))';
            queryArgs.predicateParameters.push('acting');
            queryArgs.limit = 1000;


            var result = await settingService.getInstance().recruitment.getPositionLists(queryArgs).$promise;
            if (result && result.isSuccess) {
                var positionSelect = $("#titleInActingPeriodCode").data("kendoDropDownList");
                positionSelect.setDataSource(result.object.data);
            }
        }
    }

    $rootScope.$on("isEnterKeydown", function (event, data) {
        if ($scope.advancedSearchMode && data.state == $state.current.name) {
            $scope.search();
        }
    });

    //QUI - task JIRA 84
    $scope.getFilePath = async function (id) {
        let result = await attachmentFile.getFilePath({
            id
        });

        return result.object;
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
                            }).data("kendoDialog");
                        }
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