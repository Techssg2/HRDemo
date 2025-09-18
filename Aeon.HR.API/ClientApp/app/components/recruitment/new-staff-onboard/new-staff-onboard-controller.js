var ssgApp = angular.module('ssg.newStaffOnboardModule', ['kendo.directives']);
ssgApp.controller('newStaffOnboardController', function($rootScope, $scope, $location, $stateParams, appSetting, commonData, Notification, settingService, recruitmentService, fileService) {
    // create a message to display in our view
    var ssg = this;
    $scope.advancedSearchMode = false;
    $scope.DateOfBirth = new Date();
    $rootScope.isParentMenu = false;
    $scope.newStaffOnBoardList = [];
    var allDepartments = JSON.parse(sessionStorage.getItemWithSafe("departments"));
    //search
    $scope.toggleFilterPanel = function(value) {
        $scope.advancedSearchMode = value;
        if (value) {
            if (!$scope.searchNewStaffOnboard.Department || $scope.searchNewStaffOnboard.Department == '') {
                setDataDepartment(allDepartments);
            }
        }
    }



    $scope.searchNewStaffOnboard = {
        Keyword: '',
        Department: '',
        Position: '',
    }

    //reset
    $scope.resetSearch = async function() {
        $scope.searchNewStaffOnboard.Keyword = '';
        $scope.searchNewStaffOnboard.Department = '';
        //$scope.searchNewStaffOnboard.Position = '';
        await GetAllNewStaffOnboard(1);
        clearSearchTextOnDropdownTree("departmentDropdown");
        setDataDepartment(allDepartments);
    }

    $scope.positionOptions = {
        placeholder: "",
        dataTextField: "positionName",
        dataValueField: "id",
        valuePrimitive: true,
        checkboxes: false,
        autoBind: true,
        filter: "contains",
        dataSource: $scope.positions,
    };

    $scope.applicantStatus = [];
    $scope.departments = [];
    $scope.positions = [];
    //API
    async function GetAllApplicantStatusRecruiments(grid = false) {
        var result = await settingService.getInstance().recruitment.getAllApplicantStatus().$promise;
        if (result.isSuccess) {
            $scope.applicantStatus = result.object.data;
        }
    }

    async function GetDepartment() {
        var result = await settingService.getInstance().departments.getDepartmentTree().$promise;
        if (result.isSuccess) {
            $scope.departments = result.object.data;
            setDataDepartment($scope.departments);
        }
    }

    async function GetPosition() {
        let queryArgs = {
            predicate: "",
            predicateParameters: [],
        };
        var result = await recruitmentService.getInstance().position.getPositionMappingApplicant(queryArgs).$promise;
        if (result.isSuccess) {
            $scope.positions = result.object.data;
        }
    }

    async function GetAllNewStaffOnboard(pageIndex) {
        let queryArgs = {
            predicate: "applicantStatus.Name == @0",
            predicateParameters: ["Signed Offer"],
            order: "Created desc",
            limit: appSetting.pageSizeDefault,
            page: pageIndex
        };

        var result = await recruitmentService.getInstance().newStaffOnboard.GetAllNewStaffOnboard(queryArgs).$promise;
        console.log(result);
        if (result.isSuccess) {
            $scope.newStaffOnBoardList = result.object.data;
            $scope.total = result.object.count;            
            initGrid(pageIndex, result.object.count);
        }
    }

    function setDataStatusOptions(dataApplicantStatus) {
        var dataSource = new kendo.data.DataSource({
            data: dataApplicantStatus
        });
        var dropdownlist = $("#statusDropDow").data("kendoDropDownList");
        dropdownlist.setDataSource(dataSource);
    }

    function setDataPosition(dataPosition) {
        var dataSource = new kendo.data.DataSource({
            data: dataPosition
        });
        var dropdownlist = $("#position").data("kendoDropDownList");
        dropdownlist.setDataSource(dataSource);
    }

    function setDataDepartment(dataDepartment) {
        var dataSource = new kendo.data.HierarchicalDataSource({
            data: dataDepartment
        });
    }

    $scope.applicantStatusId = '';

    $scope.statusOptions = {
        placeholder: "",
        dataTextField: "name",
        dataValueField: "id",
        valuePrimitive: true,
        checkboxes: false,
        autoBind: true,
        filter: "contains",
        dataSource: $scope.applicantStatus,
        change: function(e) {
            var value = this.value(); // value dang la string 
            $scope.applicantStatusId = value;
        }
    };

    function showCustomDepartmente(e) {
        let model = e.item;
        return `${model.code} - ${model.name}`;
    }

    $scope.departmentOptions = {
        dataTextField: "name",
        dataValueField: "id",
        template: showCustomDepartmentTitle,
        valuePrimitive: true,
        checkboxes: false,
        autoBind: true,
        filter: "contains",
        loadOnDemand: true,    
        filtering: async function (e) {
            await getDepartmentByFilter(e);
        },     
        dataSource: {
            data: $scope.departments
        },
        valueTemplate: (e) => showCustomField(e, ['name']),
        change: function(e){
            if(!e.sender.value()){
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
            if(res.isSuccess){
                setDataDepartment(res.object.data);
            }
        }

    }
    function setDataDepartment(dataDepartment) {
        $scope.departments = dataDepartment;
        var dataSource = new kendo.data.HierarchicalDataSource({
            data: dataDepartment,
            schema: {
                model: {
                    children: "items"
                }
            }
        });
        var dropdownlist = $("#departmentDropdown").data("kendoDropDownTree");
        if (dropdownlist) {
            dropdownlist.setDataSource(dataSource);
        }
    }

    //LIST
    $scope.data = [];
    //show data in table 
    $scope.newStaffOnboardGridOptions = {
        dataSource: {
            // data: $scope.newStaffOnBoardList,
            // pageSize: 20,
            pageSize: appSetting.pageSizeDefault,
            serverPaging: true,
            transport: {
                read: async function (e) {
                    await GetNewStaffOnboardByFilter($scope.currentQuery);
                }
            },
            schema: {
                total: () => {
                    return $scope.total
                },
                data: () => {
                    return $scope.newStaffOnBoardList
                }
            }
        },
        sortable: false,
        pageable: {
            alwaysVisible: true,
            pageSizes: appSetting.pageSizesArray,
        },
        columns: [{
                field: "applicantStatusId",
                title: "STATUS", 
                width: "200px",
                locked: true,
                template: function(dataItem) {
                    if (dataItem.select) {
                        return `<kendo-drop-down-list id="statusDropDow" style="width: 100%;" k-options="statusOptions"
                        name="Status" ng-model="dataItem.applicantStatusId">
                        </kendo-drop-down-list>`

                    } else {
                        return `<span>${dataItem.applicantStatusName}</span>`
                    }
                }
            }, {
                field: "referenceNumber",
                title: "REFERENCE NUMBER",
                width: "200px",
                locked: true,
                template: function (dataItem) {
                    return `<a ui-sref="home.applicant.item({referenceValue: '${dataItem.referenceNumber}', id: '${dataItem.id}'})" ui-sref-opts="{ reload: true }">${dataItem.referenceNumber}</a>`;
                }
            }, {
                field: "fullName",
                title: "FULL NAME",
                width: "220px"
            }, {
                field: "positionName",
                title: "POSITION",
                width: "200px"
            }, {
                field: "idCard9Number",
                title: "Id Card (9 number)",
                width: "180px"
            }, {
                field: "idCard12Number",
                title: "Id Card (12 number)",
                width: "180px"
            }, {
                field: "deptDivision",
                title: "Department",
                width: "200px"
            }, {
                field: "dateOfBirth",
                title: "DATE OF BIRTH",
                width: "180px",
                template: function(dataItem) {
                    if (dataItem.dateOfBirth) {
                        return moment(dataItem.dateOfBirth).format('DD/MM/YYYY');
                    }
                    return '';
                }
            }
        ]
    };

    function save(data) {
        var model = {
            ApplicantId: data.id,
            ApplicantStatusId: $scope.applicantStatusId,
        };

        recruitmentService.getInstance().newStaffOnboard.UpdateStatusNewStaffOnBoard(model).$promise.then(async function(result) {
            if (result.isSuccess) {
                Notification.success('Update success');
                await GetAllNewStaffOnboard(1);
                return true;
            }
        });

        changeSelectById(model.id)
    }

    function cancel(model) {
        changeSelectById(model.id);
    }

    function edit(model) {
        changeSelectById(model.id, true);
        setDataStatusOptions($scope.applicantStatus);
    }

    function refreshGrid(idGrid, isCallApi = false) {
        let grid = $(idGrid).data("kendoGrid");
        if (isCallApi) {
            initGrid(false, grid.pager.dataSource._page);
        }
        grid.refresh();
    }

    function changeSelectById(id, valueSelect = false) {
        let grid = $("#grid").data("kendoGrid");
        $scope.newStaffOnBoardList.forEach(item => {
            if (item.id === id) {
                item.select = valueSelect;
            }
        });
        let dataSource = new kendo.data.DataSource({
            data: $scope.newStaffOnBoardList,
            pageSize: appSetting.pageSizeDefault,
        });
        grid.setDataSource(dataSource);
    }

    // phần khai báo chung
    $scope.actions = {
        save: save,
        cancel: cancel,
        edit: edit
    };

    //search
    //var columsSearch = ['ReferenceNumber.contains(@0)', 'PositionApplicantMappings.Where(IsSignedOffer == true).Any(Position.PositionName.contains(@1))', 'FullName.contains(@2)', 'PositionApplicantMappings.Where(IsSignedOffer == true).Any(Position.DeptDivision.Name.contains(@3))']
    var columsSearch = ['ReferenceNumber.contains(@1)', 'Position.PositionName.contains(@2)', 'FullName.contains(@3)', 'Position.DeptDivision.Name.contains(@4)', 'IDCard9Number.contains(@5)', 'IDCard12Number.contains(@6)']
    $scope.currentQuery = {
        predicate: "ApplicantStatus.Name == @0",
        predicateParameters: ["Signed Offer"],
        order: "Created desc",
        limit: appSetting.pageSizeDefault,
        page: 1
    };

    $scope.search = async function(value) {
        //$scope.advancedSearchMode = value;
        //
        $scope.currentQuery = {
            predicate: "ApplicantStatus.Name == @0",
            predicateParameters: ["Signed Offer"],
            order: "Created desc",
            limit: appSetting.pageSizeDefault,
            page: 1
        };
        var option = $scope.currentQuery;
        if ($scope.searchNewStaffOnboard.Keyword) {
            option.predicate = `(${columsSearch.join(" or ")})`;
            for (let index = 0; index < columsSearch.length; index++) {
                option.predicateParameters.push($scope.searchNewStaffOnboard.Keyword);
            }
        }

        if ($scope.searchNewStaffOnboard.Department) {
            //option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `PositionApplicantMappings.Where(Priority == 1).Any(Position.DeptDivision.Id = @${option.predicateParameters.length})`;
            option.predicate = (option.predicate ? option.predicate + ' and ' : option.predicate) + `(Position.DeptDivisionId = @${option.predicateParameters.length})`;
            option.predicateParameters.push($scope.searchNewStaffOnboard.Department);
        }

        //option.predicate = `${option.predicate} and (ApplicantStatus.Name == @${option.predicateParameters.length})`;
        option.predicate = option.predicate ?  `${option.predicate} and (ApplicantStatus.Name == @${option.predicateParameters.length})` : `ApplicantStatus.Name == @${option.predicateParameters.length}`;
        option.predicateParameters.push("Signed Offer");

        $scope.currentQueryExport = option;

        await GetNewStaffOnboardByFilter(option);

        $scope.toggleFilterPanel(false);
        $scope.$apply();
    }

    async function GetNewStaffOnboardByFilter(option) {
        var result = await recruitmentService.getInstance().newStaffOnboard.GetAllNewStaffOnboard(option).$promise;
        if (result.isSuccess) {
            $scope.newStaffOnBoardList = result.object.data;
            initGrid(1, result.count);
        }
    }



    function initGrid(pageIndex, total) {
        let gridItemList = $("#grid").data("kendoGrid");
        $scope.newStaffOnBoardList.forEach(x => (x["select"] = false));
        let dataSourceItemList = new kendo.data.DataSource({
            data: $scope.newStaffOnBoardList,
            pageSize: appSetting.pageSizeDefault,
            page: pageIndex,
            schema: {
                total: function() {
                    return total;
                }
            },
        });
        gridItemList.setDataSource(dataSourceItemList);
        if ($scope.advancedSearchMode){
            gridItemList.dataSource.page(1);
        }
    }

    function setData() {
        let gridItemList = $("#grid").data("kendoGrid");
        $scope.data.forEach(x => (x["select"] = false));
        let dataSourceItemList = new kendo.data.DataSource({
            data: $scope.newStaffOnBoardList,
            pageSize: appSetting.pageSizeDefault,
            page: 5,
            schema: {
                total: function() {
                    return $scope.data.length;
                }
            },
        });
        gridItemList.setDataSource(dataSourceItemList);
        gridItemList.refresh();
    }

    async function ngInit() {
        //await GetAllNewStaffOnboard(1);
        await GetAllApplicantStatusRecruiments();
        await GetPosition();
    }

    ngInit(); 

    $scope.$on("$viewContentLoaded", function() {

    });


    $scope.currentQueryExport = {
        predicate: "ApplicantStatus.Name == @0",
        predicateParameters: ["Signed Offer"],
        order: "Created desc",
        limit: appSetting.pageSizeDefault,
        page: 1
    }

    $scope.export = async function() {
        let option = $scope.currentQueryExport;
        var res = await fileService.getInstance().processingFiles.export({ type: commonData.exportType.NEWSTAFFONBOARD }, option).$promise;
        if (res.isSuccess) {
            exportToExcelFile(res.object);
            Notification.success(appSetting.notificationExport.success);
        } else {
            Notification.error(appSetting.notificationExport.error);
        }
    }

    $rootScope.$on("isEnterKeydown", function (event, data) {
        if ($scope.advancedSearchMode && data.state == $state.current.name) {
            $scope.search(true);
        }
    });
});