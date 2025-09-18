angular
    .module("ssg.module.academy.training-request.ctrl", ["kendo.directives"])
    .controller(
        "TrainingRequestController",
        function (
            $rootScope,
            $scope,
            $stateParams,
            $state,
            $window,
            $translate,
            $controller,
            appSetting,
            Notification,
            workflowService,
            TrainingRequestService,
            settingService,
            CategoryService,
            CourseService,
            AccountService,
            TrainingReasonService,
            commonData,
            F2IntegrationService
        ) {
            $controller("BaseController", { $scope: $scope });

            const edocItUrl = "https://hrapi.aeon.com.vn"
            const edocItApiToken = "Bearer eyJzdWIiOiJTU0ciLCJuYW1lIjoiQUVPTiJ9.RumX95b8ljvb7t00CO6CX7kVy2rNK3ccs455Zoaj8SA";

            $scope.title = $translate.instant("TRAINING_REQUEST_FORM");
            $scope.titleEdit = $translate.instant("TRAINING_REQUEST_FORM_EDIT");
            $scope.status = "Draft";
            $scope.realStatus = "Draft";
            $scope.enabledActions = ["Save"];
            $scope.vm = {
                currentUser: $scope.currentUser,
            };
            $scope.categoryList = [];
            $scope.workflowInstances = {};
            $scope.isEditable = false;
            $scope.isCheckerAcademy = false;
            $scope.isHODAcademy = false;
            $scope.deletedParticipant = null;
            $scope.isAcademyUser = false;
            $scope.isCategorySelected = false;
            $scope.typeofTrainingOptions = [
                { text: "External Training", value: "External" },
                { text: "Internal Training", value: "Internal" },
            ];
            $scope.durationObj = {
                online: "Online",
                offLine: "Offline",
                onlAndOff: "Online & Offline",
                isOnline: function () {
                    if (!$scope.trainingDuration?.trainingMethod) return;
                    return $scope.trainingDuration.trainingMethod == "Online";
                },
                isOffline: function () {
                    if (!$scope.trainingDuration?.trainingMethod) return;
                    return $scope.trainingDuration.trainingMethod == "Offline";
                },
                isOnlineAndOffline: function () {
                    if (!$scope.trainingDuration?.trainingMethod) return;
                    return $scope.trainingDuration.trainingMethod == "Online & Offline";
                },
            };
            //attachmen
            $scope.attachmentDetailsCurrent;
            $scope.attachmentDetails = [];
            $scope.removeFileDetails = [];
            $scope.oldAttachmentDetails = [];
            $scope.isInprocess = false;
            $scope.isViewOnline = false;
            const departments = sessionStorage.getItemWithSafe("departments");
            if (departments) {
                allDepartments = JSON.parse(departments);
            }
            
            f2Error = false;
            $scope.isADUser = $rootScope?.currentUser?.type == 0;
            $scope.performAction = function (name) {
               
                switch (name) {
                    case "Save":
                        save();
                        break;
                    case "Submit":
                        submit();
                        break;
                    case "Approve":
                        approve();
                        break;
                    case "Reject":
                    case "RequestToChange":
                    case "Cancel":
                        showCommentDialog(name);
                        break;
                    case "CreateTrainingInvitation":
                        createTrainingInvitation();
                        break;
                    case "Close":
                        close();
                        break;
                }
              


            };
            $scope.ALL_FORM_ACTIONS = [
                {
                    name: "Submit",
                    displayName: "COMMON_BUTTON_SEND_REQUEST",
                    icon: "k-icon k-i-hyperlink-open font-green-jungle",
                    statuses: ["Draft", "Requested To Change"],
                },
                {
                    name: "Save",
                    displayName: "COMMON_BUTTON_SAVE",
                    icon: "k-icon k-i-save font-green-jungle",
                    statuses: ["Draft", "Requested To Change", "Pending"],
                },
                {
                    name: "Approve",
                    displayName: "COMMON_BUTTON_APPROVE",
                    icon: "fa fa-check font-green-jungle",
                    statuses: ["Pending"],
                },
                {
                    name: "Reject",
                    displayName: "COMMON_BUTTON_REJECT",
                    icon: "fa fa-minus-circle font-red",
                    statuses: ["Pending"],
                },
                {
                    name: "RequestToChange",
                    displayName: "COMMON_BUTTON_REQUEST_TO_CHANGE",
                    icon: "fa fa-minus-circle font-red",
                    statuses: ["Pending"],
                },
                {
                    name: "Cancel",
                    displayName: "Cancel",
                    icon: "fa fa-minus-circle font-red",
                    statuses: ["Draft", "Requested To Change", "Pending"],
                },
                {
                    name: "Close",
                    displayName: "COMMON_BUTTON_CLOSE",
                    icon: "k-icon k-i-close font-green-jungle",
                    statuses: [],
                },


            ];
            $scope.EDITABLE_STATUS = ["Draft", "Requested To Change"];
            $scope.budgetPlanOptions = [
                {
                    text: $translate.instant("TRAINING_REQUEST_PLANNED"),
                    value: "Planned",
                },
                {
                    text: $translate.instant("TRAINING_REQUEST_UNPLANNED"),
                    value: "Unplanned",
                },
            ];
            $scope.percentReimbursement = [
                {
                    text: "10",
                    value: 10,
                },
                {
                    text: "20",
                    value: 20,
                },
                {
                    text: "30",
                    value: 30,
                },
                {
                    text: "40",
                    value: 40,
                },
                {
                    text: "50",
                    value: 50,
                },
                {
                    text: "60",
                    value: 60,
                },
                {
                    text: "70",
                    value: 70,
                },
                {
                    text: "80",
                    value: 80,
                },
                {
                    text: "90",
                    value: 90,
                },
                {
                    text: "100",
                    value: 100,
                },
            ];
            $scope.monthSelectorOptions = {
                start: "year",
                depth: "year",
            };
            $scope.vatPercentageOptions = [
                {
                    text: "0",
                    value: 0,
                },
                {
                    text: "3.5",
                    value: 3.5,
                },
                {
                    text: "5",
                    value: 5,
                },
                {
                    text: "7",
                    value: 7,
                },
                {
                    text: "8",
                    value: 8,
                },
                {
                    text: "10",
                    value: 10,
                },
            ];
            $scope.methodOfChoosingContractorOptions = [
                {
                    text: "Appointing",
                    value: "Appointing",
                },
                {
                    text: "Calling Tender",
                    value: "Calling Tender",
                },
                {
                    text: "Staff Cost",
                    value: "Staff Cost",
                },
                {
                    text: "Others",
                    value: "Others",
                },
            ];
            $scope.theProposalForOptions = [
                {
                    text: "Principle Contract",
                    value: "Principle Contract",
                },
                {
                    text: "Individual Contract",
                    value: "Individual Contract",
                },
                {
                    text: "PO without Principle Contract",
                    value: "PO without Principle Contract",
                },
                {
                    text: "PO with Principle Contract",
                    value: "PO with Principle Contract",
                },
                {
                    text: "Staff Cost",
                    value: "Staff Cost",
                },
                {
                    text: "Others",
                    value: "Others",
                },
            ];
            $scope.budgetPlanOptions = [
                {
                    text: "Planned",
                    value: "Planned",
                },
                {
                    text: "Unplanned",
                    value: "Unplanned",
                },
            ];
            $scope.departmentInChargesOptions = [];
            $scope.budgetInformation = [];
            $scope.supplierOptions = [];
            $scope.requestedDepartmentOptions = [];
            $scope.totalTotal = 0;
            $scope.totalAmount = 0;
            $scope.budgetYearsOptions = [];

            //popup add user
            $scope.dialogDetailOption = {
                buttonLayout: "normal",
                animation: {
                    open: {
                        effects: "fade:in",
                    },
                },
                schema: {
                    model: {
                        id: "no",
                    },
                },
                actions: [
                    {
                        text: $translate.instant("TRAINING_REQUEST_ADD_USER"),
                        action: function (e) {
                            $scope.addUser($scope.idGrid, "#dialog_Detail");
                            return false;
                        },
                        primary: true,
                    },
                ],
            };
            $scope.addItemsUser = function () {
                $scope.keyWorkTemporary = "";
                $scope.limitDefaultGrid = 20;
                $scope.userGrid = [];
                $scope.arrayCheck = [];
                $scope.allCheck = false;
                $scope.gridUser.keyword = "";
                $scope.idGrid = "#userGrid";
                let grid = $("#userGrid").data("kendoGrid");
                grid.dataSource.data([]);
                $scope.searchGridUser(null, "#userGrid");
                // set title cho cái dialog
                let dialog = $("#dialog_Detail").data("kendoDialog");
                //let dialog1 = $("#dialog_Detail_Department").data("kendoDialog");
                dialog.title($translate.instant("COMMON_BUTTON_ADDUSER"));
                dialog.open();
                $rootScope.confirmDialogAddItemsUser = dialog;
            };

            $scope.searchGridUser = async function (departmentId = "", idGrid) {
                let result = {};
                if (idGrid == "#userGrid") {
                    departmentId = await getDepartmentLineIds();
                    var model = {
                        depLineIds: departmentId,
                        limit: 100000,
                        page: 1,
                        searchText: $scope.gridUser.keyword
                            ? $scope.gridUser.keyword.trim()
                            : "",
                    };
                    result = await settingService
                        .getInstance()
                        .users.getUsersByDeptLines(model).$promise;
                } else {
                    result = await settingService
                        .getInstance()
                        .users.getUsersByOnlyDeptLine({
                            depLineId: departmentId,
                            limit: 100000,
                            page: 1,
                            searchText: "",
                        }).$promise;
                }

                if (result.isSuccess) {
                    let dataFilter = result.object.data.map(function (item) {
                        return {
                            ...item,
                            showtextCode: item.sapCode,
                            department:
                                item.userDepartmentMappingsDepartmentName ||
                                item.departmentName,
                            position: item.jobGradeTitle,
                            userId: item.id,
                            name: item.fullName,
                        };
                    });
                    $scope.total = result.object.count;
                    let count = 0;
                    let countCheck = 0;
                    dataFilter.forEach((item) => {
                        item["no"] = count;
                        item["isCheck"] = false;
                        if ($scope.arrayCheck.length > 0) {
                            var result = $scope.arrayCheck.find((x) => x.id == item.id);
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
                        } else {
                            if (
                                $scope.arrayCheck.length == $scope.total &&
                                countCheck == $scope.userGrid.length
                            ) {
                                $scope.allCheck = true;
                            } else {
                                $scope.allCheck = false;
                            }
                        }
                    } else {
                        $scope.allCheck = false;
                    }
                    setGridUser(
                        dataFilter,
                        idGrid,
                        dataFilter.length,
                        1,
                        $scope.limitDefaultGrid
                    );
                }
            };

            async function getHighestDepartmentLineId() {
                try {
                    let opt = { id: $rootScope.currentUser?.id }
                    let result = await settingService.getInstance().departments.getDepartmentUpToG4ByUserId({ ...opt }, null).$promise;
                    if (result.isSuccess) {
                        let lstDept = [...result.object.data];
                        if (lstDept.length > 0) {
                            let maxDeptGrade = Math.max(...lstDept.map(d => d.deptLine.jobGradeGrade));
                            let foundDept = lstDept.find(d => d.deptLine.jobGradeGrade == maxDeptGrade);
                            return foundDept.deptLine.id;
                        }
                    }
                }
                catch (e) {
                    console.error(e);
                    Notification.error("Error System");
                }
            }

            $scope.addDeparmentUser = async function () {
                let dialog = $("#dialog_Detail_Department").data("kendoDialog");
                dialog.title($translate.instant("COMMON_BUTTON_ADDUSER"));
                dialog.open();
                $rootScope.confirmDialogAddDeparmentUser = dialog;
                $scope.idGrid = "#userByDeptGrid";
                $scope.dataDepartment = new kendo.data.HierarchicalDataSource({
                    data: [
                        {
                            id: "0",
                            name: "Aeon",
                            jobGradeCaption: "",
                            isRoot: true,
                            expanded: true,
                            items: allDepartments || null,
                        },
                    ],
                });
                $scope.keyWorkTemporary = "";
                $scope.limitDefaultGrid = 20;
                $scope.userGrid = [];
                $scope.arrayCheck = [];
                $scope.allCheck = false;
                $scope.gridUser.keyword = "";
                let grid = $($scope.idGrid).data("kendoGrid");
                grid.dataSource.data([]);
            };

            $scope.selectedDeparment = function (dataItem) {
                if (dataItem) {
                    $scope.selectedItem = dataItem;
                    $scope.searchText = "";
                    var treeView = $("#treeview").data("kendoTreeView");
                    treeView.expand(treeView.findByUid($scope.selectedItem.uid));
                    $scope.createAdd = true;
                    if ($scope.selectedItem.id != "0") {
                        $scope.searchGridUser(dataItem.id, $scope.idGrid);
                    }
                }
            };

            $scope.userListGridOptions = {
                dataSource: $scope.gridUser,
                sortable: true,
                autoBind: true,
                resizable: false,
                scrollable: true,
                height: 300,
                pageable: {
                    alwaysVisible: true,
                    pageSizes: [5, 10, 20, 30, 40],
                    responsive: false,
                    messages: {
                        display:
                            "{0}-{1} " +
                            $translate.instant("PAGING_OF") +
                            " {2} " +
                            $translate.instant("PAGING_ITEM"),
                        itemsPerPage: $translate.instant("PAGING_ITEM_PER_PAGE"),
                        empty: $translate.instant("PAGING_NO_ITEM"),
                    },
                },
                columns: [
                    {
                        field: "",
                        title:
                            "<input type='checkbox' style='margin-left:1px'  ng-model='allCheck' name='allCheck' id='allCheck' class='form-check-input' ng-change='onChange(allCheck, idGrid)'/> <label class='form-check-label cbox' for='allCheck' style='padding-bottom: 10px;'></label>",
                        width: "50px",
                        template: function (dataItem) {
                            return `<input type="checkbox" ng-model="dataItem.isCheck" name="isCheck{{dataItem.no}}"
                    id="isCheck{{dataItem.no}}" class="form-check-input" style="width: 2%; margin-left:1px"'/>
                    <label class="form-check-label cbox" for="isCheck{{dataItem.no}}"></label>`;
                        },
                    },
                    {
                        field: "sapCode",
                        // title: "SAP Code",
                        headerTemplate: $translate.instant("COMMON_SAP_CODE"),
                        width: "150px",
                    },
                    {
                        field: "fullName",
                        // title: "Full Name",
                        headerTemplate: $translate.instant("COMMON_FULL_NAME"),
                        width: "200px",
                    },
                    {
                        field: "department",
                        // title: "Department",
                        headerTemplate: $translate.instant("COMMON_DEPARTMENT"),
                        width: "200px",
                    },
                    {
                        field: "position",
                        // title: "Position",
                        headerTemplate: $translate.instant("COMMON_POSITION"),
                        width: "200px",
                    },
                ],
            };

            $scope.userListByDeptGridOptions = {
                dataSource: $scope.gridUser,
                sortable: true,
                autoBind: true,
                resizable: false,
                scrollable: true,
                height: 350,
                pageable: {
                    alwaysVisible: true,
                    pageSizes: [5, 10, 20, 30, 40],
                    responsive: true,
                    messages: {
                        display:
                            "{0}-{1} " +
                            $translate.instant("PAGING_OF") +
                            " {2} " +
                            $translate.instant("PAGING_ITEM"),
                        itemsPerPage: $translate.instant("PAGING_ITEM_PER_PAGE"),
                        empty: $translate.instant("PAGING_NO_ITEM"),
                    },
                },
                columns: [
                    {
                        field: "",
                        title:
                            "<input type='checkbox' ng-model='allCheck' name='allCheck' id='allCheck' class='k-checkbox' ng-change='onChange(allCheck, idGrid)'/> <label class='k-checkbox-label cbox' for='allCheck' style='padding-bottom: 10px;'></label>",
                        width: "50px",
                        template: function (dataItem) {
                            return `<input type="checkbox" ng-model="dataItem.isCheck" name="isCheck{{dataItem.no}}"
                    id="isCheck{{dataItem.no}}" class="k-checkbox" style="width: 100%;"'/>
                    <label class="k-checkbox-label cbox" for="isCheck{{dataItem.no}}"></label>`;
                        },
                    },
                    {
                        field: "sapCode",
                        // title: "SAP Code",
                        headerTemplate: $translate.instant("COMMON_SAP_CODE"),
                        width: "150px",
                    },
                    {
                        field: "fullName",
                        // title: "Full Name",
                        headerTemplate: $translate.instant("COMMON_FULL_NAME"),
                        width: "200px",
                    },
                    {
                        field: "department",
                        // title: "Department",
                        headerTemplate: $translate.instant("COMMON_DEPARTMENT"),
                        width: "200px",
                    },
                    {
                        field: "position",
                        // title: "Position",
                        headerTemplate: $translate.instant("COMMON_POSITION"),
                        width: "200px",
                    },
                ],
            };

            $scope.dialogDeptOption = {
                buttonLayout: "normal",
                animation: {
                    open: {
                        effects: "fade:in",
                    },
                },
                schema: {
                    model: {
                        id: "no",
                    },
                },
                actions: [
                    {
                        text: $translate.instant("TRAINING_REQUEST_ADD_USER"),
                        action: function (e) {
                            $scope.addUser($scope.idGrid, "#dialog_Detail_Department");
                            return false;
                        },
                        primary: true,
                    },
                ],
            };
            $scope.treeViewDepartment = {
                loadOnDemand: true,
                dataSource: $scope.dataDepartment,
            };
            $scope.dataDepartment = new kendo.data.HierarchicalDataSource({
                data: [
                    {
                        id: "0",
                        name: "Aeon",
                        jobGradeCaption: "",
                        isRoot: true,
                        expanded: true,
                        items: allDepartments || null,
                    },
                ],
            });
            $scope.selectedItem = undefined;
            $scope.path = [];
            function findPathNode(data, departments) {
                var node = "";
                for (var i = 0; i < departments.length; i++) {
                    if (departments[i].id == data.parentId) {
                        node = departments[i].id;
                        $scope.path.push(node);
                        if (departments[i].parentId) {
                            node = findPathNode(
                                departments[i],
                                JSON.parse(sessionStorage.getItemWithSafe("departments"))
                            );
                        } else {
                            $scope.path.push("0");
                        }
                    } else {
                        if (
                            departments[i].items.length > 0 &&
                            data.parentId != allDepartments[0].id
                        ) {
                            node = findPathNode(data, departments[i].items);
                            if (node) {
                                $scope.path.push(node);
                            }
                        }
                    }
                }
                return node;
            }
            $scope.departmentTreeId = "";
            $scope.departmentOptions = {
                placeholder: "",
                dataTextField: "name",
                dataValueField: "id",
                template: showCustomDepartmentTitle,
                valuePrimitive: true,
                checkboxes: false,
                autoBind: true,
                filter: "contains",
                filtering: async function (option) {
                    await getDepartmentByFilter(option);
                },
                loadOnDemand: true,
                // valueTemplate: (e) => showCustomField(e, ['name']),
                change: function (e) {
                    if (!e.sender.value()) {
                        clearSearchTextOnDropdownTree("departmentId");
                        setDataDepartmentSearch(allDepartments);
                    } else {
                        $scope.dataDepartment = new kendo.data.HierarchicalDataSource({
                            data: [
                                {
                                    id: "0",
                                    name: "Aeon",
                                    jobGradeCaption: "",
                                    isRoot: true,
                                    expanded: true,
                                    items: JSON.parse(
                                        sessionStorage.getItemWithSafe("departments")
                                    ),
                                },
                            ],
                        });
                        var treeview = $("#treeview").data("kendoTreeView");
                        treeview.setDataSource($scope.dataDepartment);
                        var result = findDepartment(
                            e.sender.value(),
                            JSON.parse(sessionStorage.getItemWithSafe("departments"))
                        );
                        $scope.path.push($scope.departmentTreeId);
                        findPathNode(
                            result,
                            JSON.parse(sessionStorage.getItemWithSafe("departments"))
                        );
                        treeview.expandPath($scope.path.reverse());
                        var nodeTree = findNodeInTree(result, treeview.dataSource._data);
                        treeview.select(treeview.findByUid(nodeTree.uid));
                        $scope.path = [];
                    }
                },
            };
            async function getDepartmentByFilter(option) {
                if (!option.filter) {
                    option.preventDefault();
                } else {
                    let filter =
                        option.filter && option.filter.value ? option.filter.value : "";
                    if (filter) {
                        arg = {
                            predicate:
                                "(name.contains(@0) or code.contains(@1)) or UserDepartmentMappings.Any(User.FullName.contains(@2))",
                            predicateParameters: [
                                filter.trim(),
                                filter.trim(),
                                filter.trim(),
                            ],
                            page: 1,
                            limit: appSetting.pageSizeDefault,
                            order: "",
                        };
                        res = await settingService
                            .getInstance()
                            .departments.getDepartmentByFilter(arg).$promise;
                        if (res.isSuccess) {
                            setDataDepartmentSearch(res.object.data);
                        }
                    } else {
                        setDataDepartmentSearch(
                            JSON.parse(sessionStorage.getItemWithSafe("departments"))
                        );
                    }
                }
            }
            function setDataDepartmentSearch(dataDepartment) {
                let dataSource = new kendo.data.HierarchicalDataSource({
                    data: dataDepartment,
                    schema: {
                        model: {
                            children: "items",
                        },
                    },
                });
                var dropdowntree = $("#departmentId").data("kendoDropDownTree");
                if (dropdowntree) {
                    dropdowntree.setDataSource(dataSource);
                }
            }
            function findNodeInTree(data, departments) {
                var node = "";
                for (var i = 0; i < departments.length; i++) {
                    if (departments[i].id == data.id) {
                        node = departments[i];
                        break;
                    } else {
                        if (departments[i].items.length > 0) {
                            node = findNodeInTree(data, departments[i].items);
                            if (node) {
                                break;
                            }
                        }
                    }
                }
                return node;
            }
            function findDepartment(departmentId, list) {
                var result;
                for (var i = 0; i < list.length; i++) {
                    if (list[i].id == departmentId) {
                        result = list[i];
                        return result;
                    } else {
                        if (list[i].items.length > 0) {
                            result = findDepartment(departmentId, list[i].items);
                            if (result) {
                                return result;
                            }
                        }
                    }
                }
                return result;
            }

            $scope.onChange = function (isCheckAll, idGrid) {
                let grid = $(idGrid).data("kendoGrid");
                let data = grid.dataSource._data;
                if (isCheckAll) {
                    data.forEach((item) => {
                        item.isCheck = true;
                    });
                    $scope.allCheck = true;
                } else {
                    data.forEach((item) => {
                        item.isCheck = false;
                    });
                    $scope.allCheck = false;
                }
                setGridUser(data, idGrid, data.length, 1, $scope.limitDefaultGrid);
            };

            $scope.addUser = function (idGrid, idDialog) {
                let grid = $(idGrid).data("kendoGrid");
                let participantsGrid = $("#participantGrid").data("kendoGrid");
                let addedUsers = grid.dataSource._data.filter((item) => item.isCheck);
                if (addedUsers.length < 1) return;
                else {
                    let participants = [
                        ...participantsGrid.dataSource._data,
                        ...addedUsers,
                    ];
                    $scope.participantBudget.participants = _.uniqBy(
                        participants,
                        "sapCode"
                    );
                    $scope.participantBudget.participants.forEach((item, index) => {
                        item["no"] = index + 1;
                    });
                    let dialog = $(idDialog).data("kendoDialog");
                    dialog.close();
                    setGridUser(
                        $scope.participantBudget.participants,
                        "#participantGrid",
                        $scope.participantBudget.participants.length,
                        1,
                        $scope.limitDefaultGrid
                    );
                    $scope.$apply(function () {
                        updateParticipantsAmount();
                        $scope.handleAllocationCost();
                    });
                }
            };

            $scope.delete = function (dataItem = null) {
                if ($scope.participantBudget.participants.length <= 0) return;
                $scope.dialog = $rootScope.showConfirmDelete(
                    $translate.instant("COMMON_BUTTON_DELETE"),
                    $translate.instant("COMMON_DELETE_VALIDATE"),
                    $translate.instant("COMMON_BUTTON_CONFIRM")
                );
                $scope.deletedParticipant = dataItem;
                $scope.dialog.bind("close", $scope.deleteParticipant);
                // let dialog = $("#dialog_Confirm").data("kendoDialog");
                // dialog.open();
            };

            $scope.deleteParticipant = function (e) {
                if (e.data && e.data.value) {
                    let grid = $("#participantGrid").data("kendoGrid");
                    let deleteUser = [];
                    if ($scope.deletedParticipant) {
                        deleteUser = grid.dataSource._data.filter(
                            (item) => item.sapCode != $scope.deletedParticipant.sapCode
                        );
                    }
                    $scope.participantBudget.participants = deleteUser.map((i, index) => {
                        return {
                            ...i,
                            no: index + 1,
                        };
                    });
                    $scope.deletedParticipant = null;
                    setGridUser(
                        $scope.participantBudget.participants,
                        "#participantGrid",
                        $scope.participantBudget.participants.length,
                        1,
                        $scope.limitDefaultGrid
                    );
                    updateParticipantsAmount();
                    $scope.handleAllocationCost();
                    Notification.success($translate.instant("COMMON_DELETE_SUCCESS"));
                }
            };

            $scope.onParticipantsImport = function (element) {
                if (element.files[0]) {
                    var reader = new FileReader();
                    //For Browsers other than IE.
                    if (reader.readAsBinaryString) {
                        reader.onload = function (e) {
                            ProcessExcel(e.target.result);
                        };
                        reader.onerror = function (ex) {
                            console.log(ex);
                        };
                        reader.readAsBinaryString(
                            document.querySelector("#fileInput").files[0]
                        );
                    } else {
                        //For IE Browser.
                        reader.onload = function (e) {
                            var data = "";
                            var bytes = new Uint8Array(e.target.result);
                            for (var i = 0; i < bytes.byteLength; i++) {
                                data += String.fromCharCode(bytes[i]);
                            }
                            ProcessExcel(data);
                        };
                        reader.onerror = function (ex) {
                            console.log(ex);
                        };
                        reader.readAsArrayBuffer(
                            document.querySelector("#fileInput").files[0]
                        );
                    }
                    element.value = null;
                }
            };

            $scope.performRequestAction = function (actionName) {
                if (!validateComment(actionName)) return;
                let commentDialog = $("#commentWindow").data("kendoDialog");
                commentDialog.close();
                switch (actionName) {
                    case "Approve":
                        approve();
                        break;
                    case "Reject":
                        reject();
                        break;
                    case "RequestToChange":
                        requestChange();
                        break;
                    case "Cancel":
                        cancel();
                        break;
                }
            };

            $scope.removeAll = function () {
                $scope.delete();
            };

            // $scope.downloadTemplate = function () {
            //   let fileContent = [
            //     {
            //       SAPNumber: "11111111",
            //       PhoneNumber: "0981234567",
            //     },
            //     {
            //       SAPNumber: "22222222",
            //       PhoneNumber: "0912345678",
            //     },
            //   ];
            //   let ws = XLSX.utils.json_to_sheet(fileContent);
            //   let wb = { Sheets: { data: ws }, SheetNames: ["data"] };
            //   let excelBuffer = XLSX.write(wb, {
            //     bookType: "xlsx",
            //     type: "array",
            //   });
            //   let data = new Blob([excelBuffer], {
            //     type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=utf-8",
            //   });
            //   saveAs(data, "ImportTemplate");
            //};

            async function ProcessExcel(data) {
                //Read the Excel File data.
                var workbook = XLSX.read(data, {
                    type: "binary",
                });

                //Fetch the name of First Sheet.
                var firstSheet = workbook.SheetNames[0];

                //Read all rows from First Sheet into an JSON array.
                var excelRows = XLSX.utils.sheet_to_row_object_array(
                    workbook.Sheets[firstSheet]
                );

                //Display the data from Excel file in Table.
                $scope.$apply(function () {
                    $scope.sapCodeList = excelRows;
                    setParticipants(excelRows);
                });
            }

            async function setParticipants(userList) {
                let sapCodes = [];
                userList.forEach((item) => sapCodes.push(item.SAPNumber));
                let sapCode = _.uniq(sapCodes).join(",");
                let result = await settingService
                    .getInstance()
                    .users.getUsersByList({ sapCodes: sapCode }, null).$promise;
                if (result.isSuccess) {
                    let dataFilter = result.object.data.map(function (item) {
                        let phoneNo = userList.find((i) => i.SAPNumber == item.sapCode);
                        return {
                            ...item,
                            showtextCode: item.sapCode,
                            department: item.departmentName,
                            position: item.jobGrade,
                            phoneNumber: phoneNo.PhoneNumber,
                            name: item.fullName,
                            userId: item.id,
                        };
                    });
                    let participants = [
                        ...$scope.participantBudget.participants,
                        ...dataFilter,
                    ];
                    $scope.participantBudget.participants = _.uniqBy(
                        participants,
                        "sapCode"
                    );
                    let count = 1;
                    $scope.participantBudget.participants.forEach((item) => {
                        item["no"] = count;
                        count++;
                    });
                    setGridUser(
                        $scope.participantBudget.participants,
                        "#participantGrid",
                        $scope.participantBudget.participants.length,
                        1,
                        $scope.limitDefaultGrid
                    );
                    $scope.$apply(function () {
                        updateParticipantsAmount();
                        $scope.handleAllocationCost();
                    });
                }
            }

            function setGridUser(data, idGrid, total, pageIndex, pageSizes) {
                let grid = $(idGrid).data("kendoGrid");
                let dataSource = new kendo.data.DataSource({
                    data: data,
                    pageSize: pageSizes,
                    page: pageIndex,
                    schema: {
                        total: function () {
                            return total;
                        },
                    },
                });
                if (grid) {
                    grid.setDataSource(dataSource);
                    //$scope.$apply();
                }
            }

            function updateParticipantsAmount() {
                let userList = $scope.participantBudget.participants;
                $scope.participantBudget.g1 = userList.filter(
                    (x) => x.position == "G1"
                ).length;
                $scope.participantBudget.g2 = userList.filter(
                    (x) => x.position == "G2"
                ).length;
                $scope.participantBudget.g3 = userList.filter(
                    (x) => x.position == "G3"
                ).length;
                $scope.participantBudget.g4 = userList.filter(
                    (x) => x.position == "G4"
                ).length;
                $scope.participantBudget.g5plus =
                    userList.length -
                    $scope.participantBudget.g1 -
                    $scope.participantBudget.g2 -
                    $scope.participantBudget.g3 -
                    $scope.participantBudget.g4;
                //$scope.$apply();
            }

            function showCommentDialog(actionName) {
                let title = "";
                switch (actionName) {
                    case "Approve":
                        title = $translate.instant("COMMON_BUTTON_APPROVE");
                        break;
                    case "Reject":
                        title = $translate.instant("COMMON_BUTTON_REJECT");
                        break;
                    case "RequestToChange":
                        title = $translate.instant("COMMON_BUTTON_REQUEST_TO_CHANGE");
                        break;
                    case "Cancel":
                        title = $translate.instant("Cancel");
                        $scope.comment = '';
                        if ($scope.status == 'Draft') {
                            cancel();
                            return;
                        }
                        break;
                }
                let commentDialog = $("#commentWindow").kendoDialog({
                    buttonLayout: "normal",
                    width: 600,
                    visible: false,
                    title: title,
                    animation: {
                        open: {
                            effects: "fade:in",
                        },
                    },
                    actions: [
                        {
                            text: $translate.instant("COMMON_BUTTON_OK"),
                            action: function (e) {
                                $scope.performRequestAction(actionName);
                                return false;
                            },
                            primary: true,
                        },
                    ],
                });
                let boxReason = commentDialog.data("kendoDialog");
                boxReason.open();
            }

            function validateComment(actionName) {
                if (actionName == "Approve") return true;
                if (!$scope.comment) {
                    $scope
                        .findElementByFieldName("workflowComment")
                        .addClass("ng-invalid");
                    $scope.commentModel = {
                        isValid: false,
                        errorMessage: $translate.instant("COMMON_FIELD_IS_REQUIRED"),
                    };
                    $scope.$apply();
                    return false;
                } else {
                    $scope
                        .findElementByFieldName("workflowComment")
                        .removeClass("ng-invalid");
                    $scope.commentModel = {
                        isValid: true,
                        errorMessage: "",
                    };
                    $scope.$apply();
                    return true;
                }
            }
            //new code for megre

            $scope.handleChangeCategory = async function (event, contextName) {
                if (!event) {
                    if (contextName == "Request New Course") {
                        await getSupliers();
                    }
                    return;
                }
                if (event.sender.element.context.name == "Course Category") {
                    $scope.trainingDetails.courseId = "";
                    try {
                        let courseObj = await CourseService.list({
                            categoryId: $scope.trainingDetails?.categoryId,
                        }).$promise;
                        if (courseObj?.length > 0) {
                            $scope.$apply(function () {
                                $scope.courseObj = courseObj.filter(
                                    (item) => item.type == $scope.trainingDetails?.typeOfTraining
                                );
                                if ($scope.courseObj.length < 1) {
                                    Notification.error("Selected Category has no course");
                                }
                                // $scope.trainingDuration.estimatedTrainingDays = courseObj.duration;
                            });
                        } else {
                            $scope.courseObj = [];
                            Notification.error("Selected Category has no course");
                        }
                    } catch (e) {
                        console.error(e);
                        Notification.error("Error System");
                    }
                    $scope.$apply(function () {
                        $scope.isCategorySelected = $scope.courseObj.length > 0;
                    })
                }
                if (event?.sender?.element.context.name == "Type Of Training") {
                    $scope.trainingDetails.categoryId = "";
                    $scope.trainingDetails.courseId = "";
                    $scope.trainingDetails.existingCourse = "";
                    if (
                        $scope?.trainingDetails?.typeOfTraining &&
                        $scope.trainingDetails.typeOfTraining == "Internal"
                    ) {
                        $scope.trainingDetails.existingCourse = "Existing Course";
                        // if (
                        //   $scope?.trainingDetails?.existingCourse &&
                        //   $scope?.trainingDetails?.existingCourse == "Request New Course"
                        // ) {
                        //   $scope.trainingDetails.existingCourse = "Existing Course";
                        // }
                    } else {
                        await getDepartmentInCharges();
                        await getRequestedDepartments();
                    }
                }
                if (event.sender.element.context.name == "Course Name") {
                    try {
                        let course = await CourseService.get({
                            id: $scope.trainingDetails?.courseId,
                        }).$promise;
                        if (course) {
                            $scope.$apply(function () {
                                $scope.trainingDuration.estimatedTrainingDays = course.duration;
                                $scope.trainingDetails.supplierName = course.serviceProvider;
                                $scope.trainingDetails.supplierCode =
                                    course.serviceProviderCode;
                            });
                        }
                    } catch (e) {
                        console.error(e);
                        Notification.error("Error System");
                    }
                }
            };
            function getTrainingDurationItem(trainingMethod, items) {
                if (!items) return {};
                let item = items.find(function (i) {
                    return i.trainingMethod == trainingMethod;
                });
                if (item) return item;
                return {};
            }

            async function getCategory() {
                try {
                    let res = await CategoryService.list().$promise;
                    return res;
                } catch (e) {
                    console.log(e);
                }
            }
            function setCategory(data) {
                if (!data) return;
                $scope.$apply(function () {
                    $scope.categoryObj = data;
                });
            }
            async function populateTrainingDetails(trainingDetails) {
                if (!trainingDetails) return;
                $scope.trainingDetails = {
                    typeOfTraining: trainingDetails.typeOfTraining,
                    typeOfExternalSupplier: trainingDetails.typeOfExternalSupplier,
                    existingCourse: trainingDetails.existingCourse,
                    categoryId: trainingDetails.categoryId,
                    categoryName: trainingDetails.categoryName,
                    courseId: trainingDetails.courseId,
                    courseName: trainingDetails.courseName,
                    supplierName: trainingDetails.supplierName,
                    trainingNotTotalFee: trainingDetails.trainingNotTotalFee,
                    accommodationIfAny: trainingDetails.accommodationIfAny,
                    airTicketFeeIfAny: trainingDetails.airTicketFeeIfAny,
                    trainingFee: trainingDetails.trainingFee,
                    currency: trainingDetails.currency,
                    currencyJson: trainingDetails.currencyJson,
                    attachments: trainingDetails.attachments,
                    reasonOfTrainingRequest: trainingDetails.reasonOfTrainingRequest,
                    supplierCode: trainingDetails.supplierCode,
                };
                $scope.attachmentDetails = trainingDetails?.attachments?.length
                    ? trainingDetails.attachments
                    : [];
                $scope.oldAttachmentDetails = [...$scope.attachmentDetails];
            }
            function populateTrainingDuration(trainingDuration) {
                if (!trainingDuration) return;
                $scope.trainingDuration = {
                    id: trainingDuration.id,
                    trainingMethod: trainingDuration.trainingMethod,
                    estimatedTrainingDays: trainingDuration.estimatedTrainingDays,
                    estimatedTrainingHours: trainingDuration.estimatedTrainingHours,
                    trainingDurationItems: trainingDuration.trainingDurationItems,
                };
                let durationOnlineMethod = trainingDuration.trainingDurationItems.find(
                    (item) => item.trainingMethod == "Online"
                );
                let durationOfflineMethod = trainingDuration.trainingDurationItems.find(
                    (item) => item.trainingMethod == "Offline"
                );
                if (durationOnlineMethod) {
                    $scope.durationOnlineMethod = {
                        trainingMethod: durationOnlineMethod.trainingMethod,
                        id: durationOnlineMethod.id,
                        from: durationOnlineMethod.from,
                        to: durationOnlineMethod.to,
                        duration: durationOnlineMethod.duration,
                        trainingLocation: durationOnlineMethod.trainingLocation,
                    };
                } else {
                    $scope.durationOnlineMethod = {
                        trainingMethod: "Online",
                        trainingLocation: "",
                    };
                }
                if (durationOfflineMethod) {
                    $scope.durationOfflineMethod = {
                        trainingMethod: durationOfflineMethod.trainingMethod,
                        id: durationOfflineMethod.id,
                        from: durationOfflineMethod.from,
                        to: durationOfflineMethod.to,
                        duration: durationOfflineMethod.duration,
                        trainingLocation: durationOfflineMethod.trainingLocation,
                    };
                } else {
                    $scope.durationOfflineMethod = {
                        trainingMethod: "Offline",
                        trainingLocation: "",
                    };
                }
            }
            //code for attachment
            function getBase64(file) {
                return new Promise((resolve, reject) => {
                    const reader = new FileReader();
                    reader.readAsDataURL(file);
                    reader.onload = () => resolve(reader.result);
                    reader.onerror = (error) => reject(error);
                });
            }
            $scope.onSelectTrainingDetails = async function (e) {
                let files = e.files;
                if (files?.length > 0) {
                    files.map(async (item, index) => {
                        let objAtt = {
                            file: await getBase64(item.rawFile),
                            state: "Added",
                            fileName: item.name,
                        };
                        $scope.attachmentDetails.push(objAtt);
                    });
                }
            };
            $scope.removeAttach = function (e) {
                let file = e.files[0];
                $scope.attachmentDetails = $scope.attachmentDetails.filter((item) => {
                    if (!item.state) return true;
                    if (item.fileName != file.name) return true;
                    return false;
                });
            };

            $scope.removeOldAttach = function (doc) {
                $scope.attachmentDetails = $scope.attachmentDetails.map((item) => {
                    if (item.fileRef == doc.fileRef) {
                        item.state = "Deleted";
                    }
                    return item;
                });
                $scope.oldAttachmentDetails = $scope.oldAttachmentDetails.filter(
                    (item) => item.fileRef != doc.fileRef
                );
            };
            $scope.dowloadAttachment = function (doc) {
                let url = baseUrlApi + "/Attachment/DownloadDocument?filePath=";
                const initSoucse = doc;
                const dowloadLink = document.createElement("a");
                const fileName = initSoucse.fileName;
                dowloadLink.href = url + initSoucse.fileRef;
                dowloadLink.download = fileName;
                // dowloadLink.target = "_blank";
                dowloadLink.click();
            };
            $scope.openAttachment = async function (doc) {
                const dowloadLink = document.createElement("a");
                dowloadLink.href = doc.linkView;
                dowloadLink.target = "_blank";
                dowloadLink.click();
            };
            $scope.owaClose = function () {
                if (!$.isEmptyObject($("#attachmentViewDialog").data("kendoDialog"))) {
                    attachmentViewDialog = $("#attachmentViewDialog").data("kendoDialog");
                    attachmentViewDialog.close();
                }
                $scope.isViewOnline = false;
            };
            $scope.viewFileOnline = async function (doc) {
                if (!doc.linkView) {
                    Notification.error(
                        $translate.instant("NOT_SUPPORT_VIEW_ONLINE_TYPE")
                    );
                    return;
                }
                let filePath = window.location.origin + doc.linkView;
                $scope.isViewOnline = true;

                if (filePath.length > 0) {
                    let attachmentViewDialog = null;
                    if (
                        !$.isEmptyObject($("#attachmentViewDialog").data("kendoDialog"))
                    ) {
                        attachmentViewDialog = $("#attachmentViewDialog").data(
                            "kendoDialog"
                        );
                    } else {
                        attachmentViewDialog = $("#attachmentViewDialog")
                            .kendoDialog({
                                width: "90%",
                                height: "85%",
                            })
                            .data("kendoDialog");
                    }
                    $("#attachment_owa")[0].setAttribute("src", filePath);
                    $("#attachmentViewDialog").css("height", "93%");

                    attachmentViewDialog.open();
                } else {
                    Notification.error($translate.instant("NOT_SUPPORT_VIEW_ONLINE"));
                    $scope.$apply();
                }
            };
            $scope.showProcessingStages = function () {
                $rootScope.visibleProcessingStages($translate);
            };
            //end
            $scope.onReimbursementChange = function (event) {
                if (
                    event?.sender?.element?.context?.name ==
                    "Actual Tuition Reimbursement Amount"
                ) {
                    $scope.trainingSponsorshipContract.sponsorshipPercentage = 0;
                }
                if (
                    event?.sender?.element?.context?.name ==
                    "Tuition Reimbursement Percentage"
                ) {
                    $scope.trainingSponsorshipContract.actualTuitionReimbursementAmount =
                        null;
                }
                let trainingFee = $scope.trainingDetails.trainingFee;
                let actualAmount =
                    $scope.trainingSponsorshipContract.actualTuitionReimbursementAmount;
                let percent = $scope.trainingSponsorshipContract.sponsorshipPercentage;
                let totalAmount = 0;
                if (!trainingFee) totalAmount = 0;
                else {
                    if ($scope.trainingSponsorshipContract.applySponsorship) {
                        if (percent) {
                            totalAmount = trainingFee * (1 - percent / 100);
                        }
                        if (actualAmount) {
                            totalAmount = trainingFee - actualAmount;
                        }
                    } else {
                        totalAmount = trainingFee;
                    }
                }

                $scope.trainingSponsorshipContract.totalAfterApply = $scope.trainingDetails.currency === 'VND' ? Math.ceil(totalAmount / 1000) * 1000 : totalAmount.toFixed(2);
                $scope.handleAllocationCost();
                if (($scope.status === 'Requested To Change' || $scope.status === 'Draft') && $scope.trainingSponsorshipContract.applySponsorship) {
                    $scope.handleAmountCostcenter();
                }
            };
            $scope.changeTrainingFee = function () {
                $scope.trainingDetails.trainingNotTotalFee = ($scope.trainingDetails.trainingNotTotalFee === null || $scope.trainingDetails.trainingNotTotalFee === undefined) ? 0.00 : $scope.trainingDetails.trainingNotTotalFee;
                $scope.trainingDetails.accommodationIfAny = ($scope.trainingDetails.accommodationIfAny === null || $scope.trainingDetails.accommodationIfAny === undefined) ? 0.00 : $scope.trainingDetails.accommodationIfAny;
                $scope.trainingDetails.airTicketFeeIfAny = ($scope.trainingDetails.airTicketFeeIfAny === null || $scope.trainingDetails.airTicketFeeIfAny === undefined) ? 0.00 : $scope.trainingDetails.airTicketFeeIfAny;
                $scope.trainingDetails.trainingFee = Number($scope.trainingDetails.trainingNotTotalFee) + Number($scope.trainingDetails.airTicketFeeIfAny) + Number($scope.trainingDetails.accommodationIfAny);
                $scope.onReimbursementChange();
            }
            $scope.changeCurrency = async function () {
                await getCurrency($scope.trainingDetails.currency);
                $scope.onReimbursementChange();
            }
            $scope.handleAmountCostcenter = function () {
                let budgetGrid = $("#budgetGrid").data("kendoGrid");
                let data = budgetGrid?.dataSource._data;
                if (!data) {
                    data = $scope.costCenters;
                }
                if (data.length > 0) {
                    if ($scope.trainingSponsorshipContract.totalAfterApply != null) {
                        let amount = ($scope.trainingDetails.trainingFee ? $scope.trainingDetails.trainingFee : 0) - $scope.trainingSponsorshipContract.totalAfterApply;
                        data.forEach((item) => {
                            item["amount"] = amount;
                            let vat = (amount * item["vatPercentage"]) / 100;
                            item["vat"] = vat;
                            let total = Number(vat) + Number(amount);
                            item["total"] = total;
                            //let budgetBalance = item["remainingBalance"] - total;
                            let budgetBalance = calBudgetBalance(item);
                            item["budgetBalance"] = budgetBalance;
                            let type = budgetBalance < 0 ? "Non-budget" : "Budget";
                            item["type"] = type;
                        });

                        $scope.totalTotal = 0;
                        $scope.totalAmount = 0;
                        if (data.length > 0) {
                            data.forEach((item) => {
                                $scope.totalTotal += item.total;
                                $scope.totalAmount += item.amount;
                            });
                        }
                        setGridUser(
                            data,
                            "#budgetGrid",
                            data.length,
                            1,
                            $scope.limitDefaultGrid
                        );
                        return;
                    }
                }
            };
            $scope.handleAllocationCost = function () {
                let participantsGrid = $("#participantGrid").data("kendoGrid");
                let data = participantsGrid?.dataSource._data;
                if (!data) {
                    data = $scope.participantBudget.participants;
                }
                if (data.length > 0) {
                    if ($scope.trainingSponsorshipContract.totalAfterApply) {
                        //let fee =
                        //    $scope.trainingSponsorshipContract.totalAfterApply / data.length;

                        let fee = Number($scope.trainingDetails.trainingFee) > Number($scope.trainingSponsorshipContract.totalAfterApply) ?
                            (($scope.trainingDetails.currency === 'VND' ? Math.ceil($scope.trainingDetails.trainingFee / 1000) * 1000 - Math.ceil($scope.trainingSponsorshipContract.totalAfterApply / 1000) * 1000
                                : $scope.trainingDetails.trainingFee - $scope.trainingSponsorshipContract.totalAfterApply)) / data.length
                            : ($scope.trainingDetails.currency === 'VND' ? Math.ceil($scope.trainingSponsorshipContract.totalAfterApply / 1000) * 1000 : $scope.trainingSponsorshipContract.totalAfterApply) / data.length;
                        data.forEach((item) => {
                            item["allocationCost"] = fee;
                        });
                        setGridUser(
                            data,
                            "#participantGrid",
                            data.length,
                            1,
                            $scope.limitDefaultGrid
                        );
                        $scope.getAllocationCost(
                            $scope.workingCommitment.workingCommitment
                        );
                        return;
                    }
                    if ($scope.trainingDetails.trainingFee) {
                        let fee = $scope.trainingDetails.trainingFee / data.length;
                        data.forEach((item) => {
                            item["allocationCost"] = fee;
                        });
                        setGridUser(
                            data,
                            "#participantGrid",
                            data.length,
                            1,
                            $scope.limitDefaultGrid
                        );
                        $scope.getAllocationCost(
                            $scope.workingCommitment.workingCommitment
                        );
                        return;
                    }
                }
            };
            $scope.getAllocationCost = function (workingCommitment) {
                if (workingCommitment) {
                    let participantsGrid = $("#participantGrid").data("kendoGrid");
                    let data = participantsGrid?.dataSource._data;
                    if (!data) {
                        data = $scope.participantBudget.participants;
                    }
                    $scope.workingCommitment.compensateAmount =
                        data.length > 0 ? data[0].allocationCost : 0;
                }
            };
            $scope.onDateChange = function (event) {
                if (event != undefined && $scope.participantBudget.fromObj != null && $scope.participantBudget.fromObj != undefined) {
                    let id = "#participantBudgetToId";
                    $(id).data('kendoDatePicker').min(new Date($scope.participantBudget.fromObj));
                    if (new Date($scope.participantBudget.fromObj) > new Date($scope.participantBudget.toObj))
                        $scope.participantBudget.toObj = $scope.participantBudget.fromObj;
                }
                if (event != undefined && $scope.participantBudget.toObj != null && $scope.participantBudget.toObj != undefined) {
                    let id = "#participantBudgetFromId";
                    $(id).data('kendoDatePicker').max(new Date($scope.participantBudget.toObj));
                }
                let fromMonth = $scope.participantBudget.fromObj;
                let toMonth = $scope.participantBudget.toObj;
                if (!fromMonth || !toMonth) return;
                fromMonth = new Date(fromMonth).setHours(0, 0, 0, 0);
                toMonth = new Date(toMonth).setHours(0, 0, 0, 0);
                let monthOfUse = moment(toMonth).diff(moment(fromMonth), "months");
                $scope.participantBudget.monthOfUse =
                    monthOfUse < 0 ? null : monthOfUse + 1;
            };
            idGridAction = "";
            $scope.executeAction = async function (typeAction, dataItem, idGrid) {
                idGridAction = idGrid;
                switch (typeAction) {
                    case "Delete":
                        itemDeleteId = dataItem.uid;
                        $scope.dialog = $rootScope.showConfirmDelete(
                            "DELETE",
                            commonData.confirmContents.remove,
                            "Confirm"
                        );
                        $scope.dialog.bind("close", confirm);
                        break;
                    default:
                        break;
                }
            };
            itemDeleteId = "";
            $scope.budgetPlan = [];
            confirm = async function (e) {
                let grid = $(idGridAction).data("kendoGrid");
                if (e.data && e.data.value) {
                    let data = grid.dataSource._data.filter(
                        (item) => item.uid != itemDeleteId
                    );
                    $scope.totalTotal = 0;
                    $scope.totalAmount = 0;
                    if (data.length > 0) {
                        data.forEach((item) => {
                            $scope.totalTotal += item.total;
                            $scope.totalAmount += item.amount;
                        });
                    }
                    setGridUser(
                        data,
                        idGridAction,
                        data.length,
                        1,
                        $scope.limitDefaultGrid
                    );
                    Notification.success($translate.instant("COMMON_DELETE_SUCCESS"));
                }
            };
            $scope.addNewBudgetPlan = function (value) {
                let grid = $("#budgetGrid").data("kendoGrid");
                let data = [...grid.dataSource._data];
                if (!value)
                    var amount1 = null;
                if ($scope.trainingSponsorshipContract.applySponsorship && data.length == 0) {
                    //amount1 = $scope.trainingSponsorshipContract.totalAfterApply;
                    amount1 = ($scope.trainingDetails.trainingFee ? ($scope.trainingDetails.currency === 'VND' ? Math.ceil($scope.trainingDetails.trainingFee / 1000) * 1000 : $scope.trainingDetails.trainingFee) : 0) - $scope.trainingSponsorshipContract.totalAfterApply;
                }
                value = {
                    budgetCode: "",
                    budgetName: "",
                    costCenterCode: "",
                    vatPercentage: null,
                    vat: null,
                    amount: amount1,
                    total: null,
                    type: "",
                    budgetBalanced: null
                };
                $scope.budgetPlan = [...data, value];
                $scope.total = $scope.budgetPlan.length;
                setGridUser(
                    $scope.budgetPlan,
                    "#budgetGrid",
                    $scope.budgetPlan.length,
                    1,
                    $scope.limitDefaultGrid
                );
            };

            async function getBudgetPlan(option) {
                await getBudgetInformation();
                $scope.budgetPlan = bindingBudgetInformation($scope.costCenters);
                $scope.totalCostCenters = $scope.budgetPlan.length;
                option.success($scope.dataItemList);
                if ($scope.budgetPlan.length > 0)
                    $scope.totalTotal = 0;
                $scope.totalAmount = 0;
                $scope.budgetPlan.forEach((item) => {
                    $scope.totalTotal += item.total;
                    $scope.totalAmount += item.amount;
                });
                setGridUser(
                    $scope.budgetPlan,
                    "#budgetGrid",
                    $scope.budgetPlan.length,
                    1,
                    $scope.limitDefaultGrid
                );
            }
            function bindingBudgetInformation(budgets) {
                return budgets && budgets.length > 0
                    ? budgets.map((item) => {
                        let lstCostCenter = $scope.budgetInformation.filter(
                            (bg) => bg.budgetCode == item.budgetCode
                        );
                        let costCenterOption = lstCostCenter.map((code) => {
                            return {
                                text: code.costCenterCode,
                                value: code.costCenterCode,
                            };
                        });
                        let lstBudget = $scope.budgetInformation.filter(bug => bug.budgetPlan == item.budgetPlan);
                        let lstBudgetName = Object.keys(_.groupBy(lstBudget, "budgetName"));
                        let newBugetOptons = lstBudgetName.map(i => {
                            return {
                                text: i,
                                value: i,
                            }
                        });
                        if (
                            item.amount &&
                            (item.vatPercentage || item.vatPercentage == 0)
                        ) {
                            let vat = (item.amount * item.vatPercentage) / 100;
                            let total = Number(vat) + Number(item.amount);
                            //let budgetBalance = item.remainingBalance - total;
                            let budgetBalance = calBudgetBalance(item);
                            let type = budgetBalance < 0 ? "Non-budget" : "Budget";
                            return {
                                ...item,
                                amount: item.amount,
                                costCenterOptions: costCenterOption,
                                budgetNameOptions: newBugetOptons,
                                vat: vat,
                                total: total,
                                budgetBalance: budgetBalance,
                                type: type,
                                budgetName: lstBudget.filter(b => b.budgetCode == item.budgetCode)[0].budgetName
                            };
                        }
                    })
                    : [];
            }
            //get BudgetInformation
            $scope.handleBudgetInformation = async function (event) {
                if (!event) return;
                if (event.sender.element.context.name == "Year") {
                    let selectedYear = event.sender.element.context.value;
                    let currentMonth = new Date().getMonth();
                    let today = new Date().getDate();
                    $scope.participantBudget.fromObj = new Date(selectedYear, currentMonth, today);
                    $scope.participantBudget.toObj = new Date(selectedYear, currentMonth + 1, today);
                }
                if (
                    $scope.participantBudget.year &&
                    $scope.participantBudget.departmentInChargeCode
                ) {
                    await getBudgetInformation();
                }
            };
            budgetId = "";
            selectedBudgetPlan = '';
            $scope.handleChangeBudgetInformation = function (event, dataItem) {
                if (!event) return;
                budgetId = dataItem.uid;
                let grid = $("#budgetGrid").data("kendoGrid");
                let dataGrid = [...grid.dataSource._data];
                selectedBudgetPlan = dataGrid.find(i => i.uid == budgetId).budgetPlan;
                if (event.sender.element.context.name == "Bugdet Plan") {
                    selectedBudgetPlan = event.sender.element.context.value;
                    let lstBudget = $scope.budgetInformation.filter(item => item.budgetPlan == selectedBudgetPlan);
                    let lstBudgetName = Object.keys(_.groupBy(lstBudget, "budgetName"));
                    if (lstBudget.length > 0) {
                        let newBugetOptons = lstBudgetName.map(item => {
                            return {
                                text: item,
                                value: item,
                            }
                        });
                        if (dataGrid && dataGrid.length > 0) {
                            $scope.totalTotal = 0;
                            $scope.totalAmount = 0;
                            let newDataGrid = dataGrid.map((item) => {
                                if (item.uid == budgetId) {
                                    var amount1 = null;
                                    if ($scope.trainingSponsorshipContract.applySponsorship && dataGrid.length == 1) {
                                        // amount1 = $scope.trainingSponsorshipContract.totalAfterApply;
                                        amount1 = ($scope.trainingDetails.trainingFee ? $scope.trainingDetails.trainingFee : 0) - $scope.trainingSponsorshipContract.totalAfterApply;
                                    }
                                    return {
                                        ...item,
                                        budgetNameOptions: newBugetOptons,
                                        budgetName: null,
                                        budgetCode: null,
                                        costCenterCode: null,
                                        vat: null,
                                        vatPercentage: null,
                                        amount: amount1,
                                        total: null,
                                        budgetBalance: null,
                                        type: null,
                                    };
                                }
                                return item;
                            });
                            setGridUser(
                                newDataGrid,
                                "#budgetGrid",
                                newDataGrid.length,
                                1,
                                $scope.limitDefaultGrid
                            );
                        }
                    } else {
                        Notification.error(
                            $translate.instant("TRAINING_REQUEST_BUDGET_PLANNED_ERROR")
                        );
                        if (dataGrid && dataGrid.length > 0) {
                            $scope.totalTotal = 0;
                            $scope.totalAmount = 0;
                            let newDataGrid = dataGrid.map((item) => {
                                if (item.uid == budgetId) {
                                    var amount1 = null;
                                    if ($scope.trainingSponsorshipContract.applySponsorship && dataGrid.length == 1) {
                                        //amount1 = $scope.trainingSponsorshipContract.totalAfterApply;
                                        amount1 = ($scope.trainingDetails.trainingFee ? $scope.trainingDetails.trainingFee : 0) - $scope.trainingSponsorshipContract.totalAfterApply;
                                    }
                                    return {
                                        ...item,
                                        budgetNameOptions: [],
                                        budgetCode: null,
                                        costCenterOptions: [],
                                        budgetName: null,
                                        costCenterCode: null,
                                        vat: null,
                                        vatPercentage: null,
                                        amount: amount1,
                                        total: null,
                                        budgetBalance: null,
                                        type: null,
                                    };
                                }
                                return item;
                            });
                            setGridUser(
                                newDataGrid,
                                "#budgetGrid",
                                newDataGrid.length,
                                1,
                                $scope.limitDefaultGrid
                            );
                        }
                    }
                }
                else if (event.sender.element.context.name == "Bugdet Name") {
                    let budgetName = event.sender.element.context.value;
                    let lstCostCenter = $scope.budgetInformation.filter(
                        (item) => item.budgetName == budgetName && item.budgetPlan == selectedBudgetPlan
                    );
                    let newOption = lstCostCenter.map((item) => {
                        return {
                            text: item.costCenterCode,
                            value: item.costCenterCode,
                        };
                    });
                    if (dataGrid && dataGrid.length > 0) {
                        $scope.totalTotal = 0;
                        $scope.totalAmount = 0;
                        let newDataGrid = dataGrid.map((item) => {
                            if (item.uid == budgetId) {
                                var amount1 = null;
                                if ($scope.trainingSponsorshipContract.applySponsorship && dataGrid.length == 1) {
                                    //amount1 = $scope.trainingSponsorshipContract.totalAfterApply;
                                    amount1 = ($scope.trainingDetails.trainingFee ? $scope.trainingDetails.trainingFee : 0) - $scope.trainingSponsorshipContract.totalAfterApply;
                                }
                                return {
                                    ...item,
                                    costCenterOptions: newOption,
                                    budgetCode: null,
                                    costCenterCode: null,
                                    vat: null,
                                    vatPercentage: null,
                                    amount: amount1,
                                    total: null,
                                    budgetBalance: null,
                                    type: null,
                                };
                            }
                            return item;
                        });
                        setGridUser(
                            newDataGrid,
                            "#budgetGrid",
                            newDataGrid.length,
                            1,
                            $scope.limitDefaultGrid
                        );
                    }
                } else if (event.sender.element.context.name == "Cost Center") {
                    let costCenter = event.sender.element.context.value;
                    if (dataGrid && dataGrid.length > 0) {
                        let budget = dataGrid.find((item) => item.uid == budgetId);
                        let foundBudget = $scope.budgetInformation.find(
                            (item) =>
                                item.budgetName == budget?.budgetName &&
                                item.costCenterCode == costCenter &&
                                item.budgetPlan == selectedBudgetPlan
                        );
                        if (foundBudget) {
                            $scope.totalTotal = 0;
                            $scope.totalAmount = 0;
                            let newDataGrid = dataGrid.map((item) => {
                                if (item.uid == budgetId) {
                                    var amount1 = null;
                                    if ($scope.trainingSponsorshipContract.applySponsorship && dataGrid.length == 1) {
                                        //amount1 = $scope.trainingSponsorshipContract.totalAfterApply;
                                        amount1 = ($scope.trainingDetails.trainingFee ? $scope.trainingDetails.trainingFee : 0) - $scope.trainingSponsorshipContract.totalAfterApply;
                                    }
                                    return {
                                        ...item,
                                        ...foundBudget,
                                        vat: null,
                                        vatPercentage: null,
                                        amount: amount1,
                                        total: null,
                                        budgetBalance: null,
                                        type: null,
                                    };
                                }
                                return item;
                            });
                            setGridUser(
                                newDataGrid,
                                "#budgetGrid",
                                newDataGrid.length,
                                1,
                                $scope.limitDefaultGrid
                            );
                        }
                    }
                } else {
                    if (
                        dataItem.amount &&
                        (dataItem.vatPercentage || dataItem.vatPercentage == 0)
                    ) {
                        let vat = (dataItem.amount * dataItem.vatPercentage) / 100;
                        let total = Number(vat) + Number(dataItem.amount);
                        //CR9.3 Tinh lai BudgetBallance khi co Ngoai te 
                        let budgetBalance = calBudgetBalance(dataItem);

                        //let budgetBalance = dataItem.remainingBalance - total;
                        let type = budgetBalance < 0 ? "Non-budget" : "Budget";
                        $scope.totalTotal += total;
                        $scope.totalAmount += dataItem.amount;
                        let newDataGrid = dataGrid.map((item) => {
                            if (item.uid == budgetId) {
                                return {
                                    ...item,
                                    vat: vat,
                                    total: total,
                                    budgetBalance: budgetBalance,
                                    type: type,
                                };
                            }
                            return item;
                        });
                        $scope.totalTotal = 0;
                        $scope.totalAmount = 0;
                        if (newDataGrid.length > 0) {
                            newDataGrid.forEach((item) => {
                                $scope.totalTotal += item.total;
                                $scope.totalAmount += item.amount;
                            });
                        }
                        setGridUser(
                            newDataGrid,
                            "#budgetGrid",
                            newDataGrid.length,
                            1,
                            $scope.limitDefaultGrid
                        );
                    }
                }
            };
            async function getBudgetInformation() {
                if (
                    $scope.participantBudget.year &&
                    $scope.participantBudget.departmentInChargeCode
                ) {
                    try {
                        if (!f2Error) {
                            let option = {
                                year: $scope.participantBudget.year,
                                dicCode: $scope.participantBudget.departmentInChargeCode,
                            };
                            watch(true);
                            var result = await F2IntegrationService.getBudgetInformations(
                                option
                            ).$promise;
                            watch(false);
                            if (result) {
                                //let emptyItem = { id: "", value: "" };
                                $scope.budgetInformation = [
                                    //emptyItem,
                                    ...JSON.parse(JSON.stringify(result)),
                                ];
                                if ($scope.budgetInformation.length < 1) {
                                    Notification.error(
                                        $translate.instant("TRAINING_REQUEST_BUDGET_ERROR")
                                    );
                                }
                            }
                        }
                    } catch (e) {
                        f2Error = true;
                        watch(false);
                        console.error(e);
                        Notification.error("Error System");
                    }
                }
            }

            async function getDepartmentInCharges() {
                try {
                    if (!f2Error) {
                        watch(true);
                        let request = {
                            "Predicate": "",
                            "PredicateParameters": [],
                            "Order": "",
                            "Page": 1,
                            "Limit": 100,
                            "IsGetAllOrg": false
                        };
                        let ret = [];
                        await $.ajax({
                            type: "POST",
                            beforeSend: function (request) {
                                request.setRequestHeader("Authorization", edocItApiToken);
                            },
                            contentType: "application/json",
                            dataType: "json",
                            data: JSON.stringify(request),
                            url: `${edocItUrl}/it/api/Partner/GetDepartmentDIC`,
                            success: function (response) {
                                ret = response?.object?.data;
                            },
                            error: function (e) {
                                console.log("getDepartmentDIC error: ", e);
                            }
                        });
                        if (ret) {
                            $scope.$apply(function () {
                                $scope.departmentInChargesOptions = ret;
                            });
                        }
                        watch(false);
                    }
                } catch (e) {
                    f2Error = true;
                    watch(false);
                    console.error(e);
                    Notification.error("Error System");
                }
            }
            async function getSupliers() {
                try {
                    if (!f2Error) {
                        watch(true);
                        var result = await F2IntegrationService.getSuppliers().$promise;
                        if (result) {
                            //let emptyItem = { id: "", value: "" };
                            $scope.$apply(function () {
                                $scope.supplierOptions = [
                                    //emptyItem,
                                    ...JSON.parse(JSON.stringify(result)),
                                ].filter(item => item.name && item.code);
                            });
                        }
                        watch(false);
                    }
                } catch (e) {
                    f2Error = true;
                    watch(false);
                    console.error(e);
                    Notification.error("Error System");
                }
            }

            async function getRequestedDepartments() {
                try {
                    if (!f2Error) {
                        if ($rootScope.currentUser) {
                            watch(true);

                            const  getAllDepartmentRequest = {
                                "Predicate": "",
                                "PredicateParameters": [],
                                "Order": "",
                                "Page": 1,
                                "Limit": 10000
                            }

                            await $.ajax({
                                type: "POST",
                                beforeSend: function(request) {
                                    request.setRequestHeader("Authorization", edocItApiToken);
                                },
                                contentType: "application/json",
                                dataType: "json",
                                data: JSON.stringify(getAllDepartmentRequest),
                                url: edocItUrl + `/it/api/Partner/GetDepartmentTreeByFilter`,
                                success: function (response) {
                                    getDepartmentTreeResponse = response;
                                },
                                error: function (e) {
                                    console.log("getDepartmentTree error: ", e);
                                }
                            });

                            $scope.flattedDept = makeFlatDepartmentData(getDepartmentTreeResponse?.object?.data[0]);
                            $scope.cleanDept = makeCleanDepartmentData($scope.flattedDept);

                            const dataSource = new kendo.data.HierarchicalDataSource({
                                data: processTable($scope.cleanDept, "id", "parent", "#")
                            })

                            $("#requested-department-ddt").kendoDropDownTree({
                                filter: "contains",
                                height: "auto",
                                dataSource: dataSource,
			                    dataValueField: "text",
			                    dataTextField: "text",
                                change: function(e) {
                                    const value = this.value();
                                    $scope.participantBudget.requestedDepartment = value;
                                  }
                            });
                            watch(false);
                        }
                    }
                } catch (e) {
                    f2Error = true;
                    watch(false);
                    console.error(e);
                    Notification.error("Error System");
                }
            }
            async function getCurrency(symbol) {
                try {
                    if (!f2Error) {
                        if ($rootScope.currentUser) {
                            watch(true);
                            var result = await F2IntegrationService.getCurrency({
                                symbol: symbol
                            }).$promise;
                            if (result) {
                                $scope.trainingDetails.currencyJson = result[0];
                            }
                            watch(false);
                        }
                    }
                } catch (e) {
                    f2Error = true;
                    watch(false);
                    console.error(e);
                    Notification.error("Error System");
                }
            }
            async function getYears() {
                try {
                    if (!f2Error) {
                        if ($rootScope.currentUser) {
                            watch(true);
                            var result = await F2IntegrationService.getYears().$promise;
                            if (result) {
                                let data = [
                                    ...JSON.parse(JSON.stringify(result)),
                                ];
                                $scope.$apply(function () {
                                    $scope.budgetYearsOptions = data.map(item => {
                                        return {
                                            name: item,
                                            value: item
                                        }
                                    });
                                });
                            }
                            watch(false);
                        }
                    }
                } catch (e) {
                    f2Error = true;
                    watch(false);
                    console.error(e);
                    Notification.error("Error System");
                }
            }
            async function getCurrencies() {
                try {
                    loadJSON(function (response) {
                        // Parse JSON string into object
                        $scope.currencyOption = _.values(JSON.parse(response));
                    });
                } catch (e) {
                }
            }
            function loadJSON(callback) {

                var xobj = new XMLHttpRequest();
                xobj.overrideMimeType("application/json");
                xobj.open('GET', './ClientApp/app/components/academy/pages/training-request/currency.txt', true); // Replace 'my_data' with the path to your file
                xobj.onreadystatechange = function () {
                    if (xobj.readyState == 4 && xobj.status == "200") {
                        // Required use of an anonymous callback as .open will NOT return a value but simply returns undefined in asynchronous mode
                        callback(xobj.responseText);
                    }
                };
                xobj.send(null);
            }
            function calBudgetBalance(dataItem) {
                let budgetBalance = 0;
                if (dataItem.totalBudget && dataItem.totalBudget > 0) {
                    let balance = dataItem.totalBudget - (dataItem.remainingBalance || 0) - (dataItem.transferOut || 0) + (dataItem.transferIn || 0)
                        + (dataItem.refund || 0);
                    budgetBalance = balance - (dataItem.amount * $scope.trainingDetails.currencyJson.amountInVND);
                } else {
                    let balance = (dataItem.transferIn || 0) - (dataItem.remainingBalance || 0) - (dataItem.transferOut || 0)
                        + (dataItem.refund || 0);
                    budgetBalance = balance - (dataItem.amount * $scope.trainingDetails.currencyJson.amountInVND);
                }
                return budgetBalance;
            }

            async function getJobGradeList() {
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

            this.$onInit = async function () {
                $window.scrollTo(0, 0);
                $rootScope.showLoading();
                await getJobGradeList();
                var today = new Date();
                var model = {
                    requester: {
                        dateOfSubmission: new Date(),
                        regionName: "",
                    },
                    participantBudget: {
                        year: null,
                        departmentInCharge: null,
                        from: today.toISOString(),
                        to: new Date(today.setMonth(today.getMonth() + 1)).toISOString(),
                        participants: [],
                        requestedDepartment: null,
                        departmentInChargeName: null,
                        requestedDepartmentCode: null,
                        methodOfChoosingContractor: "Appointing",
                        theProposalFor: null,
                        reference: "",
                        requestedDepartmentId: null,
                        departmentInChargeId: null,
                        dicDepartmentCode: null
                    },
                    trainingSponsorshipContract: {
                        applySponsorship: false,
                        sponsorshipPercentage: 50,
                        actualTuitionReimbursementAmount: null,
                        totalAfterApply: null,
                    },
                    workingCommitment: {
                        workingCommitment: false,
                        from: new Date(),
                        to: "",
                        trainingLocation: "",
                        sponsorshipContractNumber: "",
                        compensateAmount: 0.0,
                    },
                    purpose: "",
                    reason: "",
                    howApply: "",
                    whatExpectedKPI: "",
                    whenExcute: "",
                    trainingDetails: {
                        typeOfTraining: $scope.isADUser ? "External" : "Internal",
                        typeOfExternalSupplier: null,
                        existingCourse: !$scope.isADUser ? "Existing Course" : "",
                        categoryId: "",
                        categoryName: "",
                        courseId: "",
                        courseName: "",
                        supplierName: null,
                        supplierCode: null,
                        trainingNotTotalFee: 0.0,
                        accommodationIfAny: 0.0,
                        airTicketFeeIfAny: 0.0,
                        trainingFee: 0.0,
                        currency: 'VND',
                        currencyJson: null,
                        attachments: null,
                        reasonOfTrainingRequest: "",
                    },
                    trainingDuration: {
                        trainingMethod: "Online",
                        estimatedTrainingDays: "",
                        estimatedTrainingHours: "",
                        trainingDurationItems: [],
                    },
                    realStatus: "Draft",
                    costCenters: [],
                    f2ReferenceNumber: null,
                    f2Url: null,
                };
                try {
                    $scope.isInprocess = true;
                    let res = await getCategory();
                    setCategory(res);
                    await getTrainingReasons();
                    await getCurrencies();
                    if (!$scope.id) await getAffiliate();
                    if ($scope.id) {
                       
                        model = await TrainingRequestService.get({ id: $scope.id })
                            .$promise;
                        if (model.trainingDetails.typeOfTraining == "External") {
                            await getSupliers();
                            await getRequestedDepartments(model.requester);
                        }
                        let checker = await AccountService.IsCheckerAcademy().$promise;
                        if (checker.isAuthorized && model.realStatus != "Completed") {
                            $scope.ALL_FORM_ACTIONS = [
                                {
                                    name: "CreateTrainingInvitation",
                                    displayName: "TRAINING_INVITATION_CREATE",
                                    icon: "k-icon font-green-jungle k-i-check",
                                    statuses: ["Completed"],
                                },
                                ...$scope.ALL_FORM_ACTIONS,
                            ];
                        }
                        if (model.trainingDetails?.categoryId) {
                            let courseObj = await CourseService.list({
                                categoryId: model.trainingDetails.categoryId,
                            }).$promise;
                            $scope.courseObj = courseObj.filter(
                                (item) => item.type == model.trainingDetails.typeOfTraining
                            );
                        }
                        if (model.trainingDetails?.currencyJson)
                            model.trainingDetails.currencyJson = JSON.parse(model.trainingDetails.currencyJson);

                        // get workflow
                        let workflowInstances =
                            await TrainingRequestService.progressingStage({ id: $scope.id })
                                .$promise;
                        //await getWorkflowProcessingStage($scope.id);
                        if (workflowInstances) {
                            $scope.workflowInstances = workflowInstances;
                            $scope.comment = workflowInstances.histories[0]?.comment;
                        }

                        $scope.workflowInstances1 = angular.copy($scope.workflowInstances);
                        if ($scope.workflowInstances1.workflowData && $scope.workflowInstances1.workflowData.steps) {
                            for (let i = 0; i < $scope.workflowInstances1.workflowData.steps.length; i++) {
                                let history = $scope.workflowInstances1.histories.find(h => h.stepNumber === $scope.workflowInstances1.workflowData.steps[i].stepNumber);
                                if (history) {
                                    $scope.workflowInstances1.workflowData.steps[i].history = history;
                                }
                            }
                        }
                        if (workflowInstances.histories.length > 0) {
                            let sortedHistories = [...workflowInstances.histories].sort((a, b) => a.stepNumber - b.stepNumber);
                            if (sortedHistories[0].stepNumber === 0) {
                                workflowInstances.histories.forEach(history => {
                                    history.stepNumber += 1;
                                });
                                if (workflowInstances.workflowData.steps) {
                                    workflowInstances.workflowData.steps.forEach(step => {
                                        step.stepNumber += 1;
                                    });
                                }
                            }
                        }

                        $scope.roundWF = Math.max(...$scope.workflowInstances1.histories.map(history => history.roundNumber));
                        

                    }
                    $scope.isInprocess = false;
                    await getDepartmentInCharges();
                    await getYears();
                    populateModel(model);
                    if (!$scope.id)
                        await getCurrency('VND');
                    if ($scope.requestedDepartmentOptions.length < 1)
                        await getRequestedDepartments(model.requester);
                    // set requested department
                    if (model.participantBudget.requestedDepartment) {
                        var tree = $("#requested-department-ddt").data("kendoDropDownTree");
                        tree.value(model.participantBudget.requestedDepartment);
                        $scope.$apply();
                    }
                } catch (error) {
                    console.log(error);
                    $scope.isInprocess = false;
                    $rootScope.hideLoading();
                }
                $rootScope.hideLoading();
            };

            /* Populate model and validation functions */
            async function populateModel(model) {
                if (!model) return;
                $scope.referenceNumber = model.referenceNumber;
                $scope.requestedDepartment = model.participantBudget.requestedDepartment;
                if (model.status) $scope.status = model.status;
                if (model.realStatus) $scope.realStatus = model.realStatus;
                if (model.enabledActions) $scope.enabledActions = model.enabledActions;

                if (model.status === "Completed") {
                    $scope.enabledActions.push("CreateTrainingInvitation");
                }

                if ($scope.enabledActions.indexOf("Save") != -1) {
                    $scope.isEditable = true;
                }
                $scope.budgetGridOptions = {
                    dataSource: {
                        serverPaging: true,
                        pageSize: 20,
                        transport: {
                            read: async function (e) {
                                await getBudgetPlan(e);
                            },
                        },
                        schema: {
                            total: () => {
                                return $scope.totalCostCenters;
                            },
                            data: () => {
                                return $scope.costCenters;
                            },
                        },
                    },
                    sortable: false,
                    editable: false,
                    scrollable: true,
                    selectable: true,
                    pageable: {
                        alwaysVisible: true,
                        pageSizes: [5, 10, 20, 30, 40],
                        responsive: true,
                        messages: {
                            display:
                                "{0}-{1} " +
                                $translate.instant("PAGING_OF") +
                                " {2} " +
                                $translate.instant("PAGING_ITEM"),
                            itemsPerPage: $translate.instant("PAGING_ITEM_PER_PAGE"),
                            empty: $translate.instant("PAGING_NO_ITEM"),
                        },
                    },
                    columns: [
                        {
                            field: "budgetPlan",
                            title: "Plan",
                            headerTemplate: $translate.instant(
                                "TRAINING_REQUEST_BUDGET_PLAN"
                            ),
                            width: "125px",
                            template: function (dataItem) {
                                return `<select
                        name="Bugdet Plan"
                        kendo-drop-down-list
                        k-data-source="budgetPlanOptions"
                        data-k-ng-model="dataItem.budgetPlan"
                        k-data-text-field="'text'"
                        k-data-value-field="'value'"
                        class="w100"
                        k-value-primitive="'false'"
                        k-filter="'contains'"
                        k-template="'<span><label>#: data.text# </label></span>'"
                        ng-disabled="!isEditable"
                        k-on-change="handleChangeBudgetInformation(kendoEvent, dataItem)"
                      ></select>`;
                            },
                        },
                        {
                            field: "budgetName",
                            title: "Budget Name",
                            headerTemplate: $translate.instant(
                                "TRAINING_REQUEST_BUDGET_NAME"
                            ),
                            width: "200px",
                            template: function (dataItem) {
                                return `<select
                        name="Bugdet Name"
                        kendo-drop-down-list
                        k-data-source="dataItem.budgetNameOptions"
                        data-k-ng-model="dataItem.budgetName"
                        k-data-text-field="'text'"
                        k-data-value-field="'value'"
                        class="w100"
                        k-value-primitive="'false'"
                        k-filter="'contains'"
                        k-template="'<span><label>#: data.text# </label></span>'"
                        ng-disabled="!isEditable"
                        k-on-change="handleChangeBudgetInformation(kendoEvent, dataItem)"
                      ></select>`;
                            },
                        },
                        {
                            field: "budgetCode",
                            title: "Budget Code",
                            headerTemplate: $translate.instant(
                                "TRAINING_REQUEST_BUDGET_CODE"
                            ),
                            width: "80px",
                            // template: function (dataItem) {
                            //   return `<select
                            //           name="Bugdet Code"
                            //           kendo-drop-down-list
                            //           k-data-source="budgetCodeOptions"
                            //           data-k-ng-model="dataItem.budgetCode"
                            //           k-data-text-field="'name'"
                            //           k-data-value-field="'code'"
                            //           class="w100"
                            //           k-value-primitive="'false'"
                            //           k-filter="'contains'"
                            //           k-template="'<span><label>#: data.name# </label></span>'"
                            //           ng-disabled="!isEditable"
                            //           k-on-change="handleChangeBudgetInformation(kendoEvent, dataItem)"
                            //         ></select>`;
                            // },
                        },
                        {
                            field: "costCenterCode",
                            title: "Cost Center",
                            headerTemplate: $translate.instant(
                                "TRAINING_REQUEST_COST_CENTER"
                            ),
                            width: "100px",
                            template: function (dataItem) {
                                return `<select
                        name="Cost Center"
                        kendo-drop-down-list
                        id="costCenter{{dataItem.index}}"
                        k-data-source="dataItem.costCenterOptions"
                        k-data-text-field="'text'"
                        k-data-value-field="'value'"
                        data-k-ng-model="dataItem.costCenterCode"
                        k-filter="'contains'"
                        k-template="'<span><label>#: data.value# </label></span>'"
                        class="w100"
                        k-value-primitive="'true'"
                        ng-disabled="!isEditable"
                        k-on-change="handleChangeBudgetInformation(kendoEvent, dataItem)"
                      ></select>`;
                            },
                        },
                        {
                            field: "amount",
                            title: "Amount",
                            headerTemplate: $translate.instant("BUDGET_AMOUNT"),
                            width: "150px",
                            template: function (dataItem) {
                                return `<input kendo-numeric-text-box
                                         k-format="'#,0'"
                                         data-role="numerictextbox"
                                         aria-valuemin="1"
                                         aria-valuemax="3"
                                         class="w100"
                                         k-ng-model="dataItem.amount"
						                 k-ng-readonly="trainingSponsorshipContract.applySponsorship"
                                         ng-disabled="!isEditable"
                                         k-on-change="handleChangeBudgetInformation(kendoEvent, dataItem)" />`;
                                //      ` <input
                                //                  kendo-numeric-text-box
                                //                  k-format="'#,'"
                                //                  decimals="0"
                                //                  restrict-decimals="true"
                                //                  data-role="numerictextbox"
                                //                  class="w100"
                                //                  k-ng-model="dataItem.amount"
                                //k-ng-readonly="trainingSponsorshipContract.applySponsorship"
                                //                  ng-disabled="!isEditable"
                                //                  k-on-change="handleChangeBudgetInformation(kendoEvent, dataItem)"
                                //                />`;
                            },
                            footerTemplate: function () {
                                return `<span><strong>{{"BUDGET_TOTAL"| translate}}: {{totalAmount | currency:"":2}} ({{trainingDetails.currency}})`;
                            },
                        },
                        {
                            field: "vatPercentage",
                            title: "% VAT",
                            //headerTemplate: $translate.instant("COMMON_SAP_CODE"),
                            width: "120px",
                            template: function (dataItem) {
                                return `<select
                        kendo-drop-down-list
                        k-data-source="vatPercentageOptions"
                        data-k-ng-model="dataItem.vatPercentage"
                        k-data-text-field="'text'"
                        k-data-value-field="'value'"
                        class="w100"
                        k-value-primitive="'false'"
                        k-filter="'contains'"
                        k-template="'<span><label>#: data.text# </label></span>'"
                        ng-disabled="!isEditable"
                        k-on-change="handleChangeBudgetInformation(kendoEvent, dataItem)"
                      ></select>`;
                            },
                        },
                        {
                            field: "vat",
                            title: "VAT",
                            //headerTemplate: $translate.instant("TRAINING_REQUEST_BUDGET_CODE"),
                            width: "100px",
                            template: function (dataItem) {
                                return `<span>{{dataItem.vat | currency:"":2}}</span>`;
                            },
                        },
                        {
                            field: "total",
                            title: "Total",
                            headerTemplate: $translate.instant("BUDGET_TOTAL"),
                            width: "150px",
                            template: function (dataItem) {
                                return `<span>{{dataItem.total | currency:"":2}}</span>`;
                            },
                            footerTemplate: function () {
                                return `<span><strong>{{"BUDGET_TOTAL"| translate}}: {{totalTotal | currency:"":2}} ({{trainingDetails.currency}})`;
                            },
                        },
                        {
                            field: "currency",
                            title: "Currency",
                            headerTemplate: $translate.instant("BUDGET_CURRENCY"),
                            width: "75px",
                            template: function (dataItem) {
                                return `<span>{{trainingDetails.currency}}</span>`;
                            },
                        },
                        {
                            field: "type",
                            title: "Type",
                            headerTemplate: $translate.instant("COMMON_TYPE"),
                            width: "100px",
                        },
                        {
                            field: "budgetBalance",
                            title: "Budget Balance",
                            headerTemplate: $translate.instant(
                                "TRAINING_REQUEST_BUDGET_BALANCE"
                            ),
                            width: "120px",
                            template: function (dataItem) {
                                return `<span>{{dataItem.budgetBalance | currency:"":2}}</span>`;
                            },
                        },
                        {
                            headerTemplate: $translate.instant("COMMON_ACTION"),
                            width: "75px",
                            //locked: true,
                            template: function (dataItem) {
                                return `
                                        <a ng-click="executeAction('Delete', dataItem, '#budgetGrid')"  class='btn-delete-upgrade btn-border-upgrade' > </a>
                                `;
                            },
                            hidden: !$scope.isEditable,
                        },
                    ],
                };
                $scope.$apply(function () {
                    populateRequester(model.requester);
                    populateParticipant(model.participantBudget);
                    populateTrainingSponsorshipContract(
                        model.trainingSponsorshipContract
                    );
                    populateWokingCommitment(model.workingCommitment);
                    populateTrainingDetails(model.trainingDetails);
                    populateTrainingDuration(model.trainingDuration);
                    $scope.purposeAndReason = {
                        purpose: model.purpose,
                        reason: model.reason,
                        howApply: model.howApply,
                        whenExcute: model.whenExcute,
                        whatExpectedKPI: model.whatExpectedKPI,
                    };
                    //const f2UrlOld = model.f2Url.replace("home.aspx", "default.aspx");
                    $scope.commentModel = { isValid: true, errorMessage: "" };
                    $scope.f2 = {
                        f2Url: model.f2Url,
                        f2ReferenceNumber: model.f2ReferenceNumber,
                    };
                    $scope.costCenters = model.costCenters;
                    $scope.isAcademyUser =
                        deptAcademyCode == $rootScope.currentUser.deptCode;
                    $scope.onReimbursementChange();
                });
            }
            function populateRequester(requester) {
                if (!requester) return;
                $scope.requester = {
                    requesterName: $scope.id
                        ? requester.requesterName
                        : $scope.vm?.currentUser?.fullName,
                    sapNumber: $scope.id
                        ? requester.sapNumber
                        : $scope.vm?.currentUser?.sapCode,
                    affiliate: $scope.id ? requester.affiliate : $scope.affiliateName,
                    departmentName: $scope.id
                        ? requester.departmentName
                        : $scope.currentUser?.divisionName
                            ? $scope.currentUser?.divisionName
                            : $scope.currentUser?.deptName,
                    position: $scope.id ? requester.position : getPosition(),
                    extension: requester.extension,
                    dateOfSubmission: requester.dateOfSubmission,
                    departmentId:
                        $scope.id || requester.departmentId
                            ? requester.departmentId
                            : $scope.vm?.currentUser.divisionId
                                ? $scope.vm?.currentUser.divisionId
                                : $scope.vm?.currentUser.deptId,
                    regionName: "",
                };
            }
            function populateParticipant(participantBudget) {
                if (!participantBudget) return;
                $scope.participantBudget = {
                    year: participantBudget.year,
                    from: participantBudget.from,
                    to: participantBudget.to,
                    fromObj: participantBudget.from,
                    toObj: participantBudget.to,
                    departmentInCharge: participantBudget.departmentInCharge,
                    requestedDepartmentCode: participantBudget.requestedDepartmentCode,
                    departmentInChargeCode: participantBudget.departmentInChargeCode,
                    requestedDepartment: participantBudget.requestedDepartment,
                    methodOfChoosingContractor:
                        participantBudget.methodOfChoosingContractor,
                    theProposalFor: participantBudget.theProposalFor,
                    reference: participantBudget.reference,
                    participants: participantBudget.participants.map((item, index) => {
                        return {
                            ...item,
                            no: index + 1,
                            departmentName: item.department,
                        };
                    }),
                    g1: null,
                    g2: null,
                    g3: null,
                    g4: null,
                    g5plus: null,
                };
                $scope.onDateChange();
                updateParticipantsAmount();
                $scope.participantsGridOptions = {
                    dataSource: {
                        data: $scope.participantBudget.participants,
                        pageSize: 20,
                    },
                    sortable: false,
                    editable: false,
                    scrollable: true,
                    selectable: true,
                    pageable: {
                        alwaysVisible: true,
                        pageSizes: [5, 10, 20, 30, 40],
                        responsive: true,
                        messages: {
                            display:
                                "{0}-{1} " +
                                $translate.instant("PAGING_OF") +
                                " {2} " +
                                $translate.instant("PAGING_ITEM"),
                            itemsPerPage: $translate.instant("PAGING_ITEM_PER_PAGE"),
                            empty: $translate.instant("PAGING_NO_ITEM"),
                        },
                    },
                    columns: [
                        {
                            field: "no",
                            headerTemplate: $translate.instant("COMMON_NO"),
                            width: "40px",
                            editable: function (e) {
                                return false;
                            },
                        },
                        {
                            field: "sapCode",
                            headerTemplate: $translate.instant("COMMON_SAP_CODE"),
                            width: "100px",
                        },
                        {
                            field: "name",
                            title: "name",
                            headerTemplate: $translate.instant("COMMON_FULL_NAME"),
                            width: "100px",
                        },
                        {
                            field: "email",
                            title: "Email",
                            headerTemplate: $translate.instant("COMMON_EMAIL"),
                            width: "150px",
                            template: function (dataItem) {
                                return `<input type='text'
                        name="Email Of {{dataItem.sapCode}}"
                        style="width: 100%;" ng-model="dataItem.email" 
                        class="k-textbox form-control input-sm"
                        ng-disabled="!isEditable" />`;
                            },
                        },
                        {
                            field: "position",
                            title: "Job Grade",
                            headerTemplate: $translate.instant("JOBGRADE_MENU"),
                            width: "100px",
                        },
                        {
                            field: "departmentName",
                            title: "Department",
                            headerTemplate: $translate.instant("COMMON_DEPARTMENT"),
                            width: "150px",
                        },
                        {
                            field: "phoneNumber",
                            title: "Phone Number",
                            headerTemplate: $translate.instant(
                                "TRAINING_REQUEST_PHONE_NUMBER"
                            ),
                            width: "100px",
                            template: function (dataItem) {
                                return `<input type='text'
                        name="PhoneNumber Of {{dataItem.sapCode}}"
                        style="width: 100%;" ng-model="dataItem.phoneNumber" 
                        class="k-textbox form-control input-sm" onkeypress="return event.keyCode < 58 && event.keyCode > 47"
                        ng-disabled="!isEditable"
                        maxlength="12"/>`;
                            },
                        },
                        {
                            field: "allocationCost",
                            title: "Allocation Cost",
                            headerTemplate: $translate.instant(
                                "TRAINING_REQUEST_ALLOCATION_COST"
                            ),
                            width: "150px",
                            template: function (dataItem) {
                                return `<span>{{dataItem.allocationCost | currency:'':0}}</span>`;
                            },
                        },
                        {
                            headerTemplate: $translate.instant("COMMON_ACTION"),
                            width: "100px",
                            //locked: true,
                            template: function (dataItem) {
                                return `
                                <a ng-click="delete(dataItem)"  class='btn-delete-upgrade btn-border-upgrade' > </a>


                              

                                `;
                            },
                            hidden: !$scope.isEditable,
                        },
                    ],
                };
                $scope.userGrid = [];
                $scope.gridUser = {
                    keyword: "",
                };
            }
            function populateTrainingSponsorshipContract(
                trainingSponsorshipContract
            ) {
                if (!trainingSponsorshipContract) return;
                $scope.trainingSponsorshipContract = {
                    applySponsorship: trainingSponsorshipContract.applySponsorship,
                    sponsorshipPercentage:
                        trainingSponsorshipContract.sponsorshipPercentage,
                    actualTuitionReimbursementAmount:
                        trainingSponsorshipContract.actualTuitionReimbursementAmount,
                };
            }
            function populateWokingCommitment(workingCommitment) {
                if (!workingCommitment) return;
                $scope.workingCommitment = {
                    workingCommitment: workingCommitment.workingCommitment,
                    from: workingCommitment.from,
                    to: workingCommitment.to,
                    trainingLocation: workingCommitment.trainingLocation,
                    sponsorshipContractNumber:
                        workingCommitment.sponsorshipContractNumber,
                    compensateAmount: workingCommitment.compensateAmount,
                };
            }
            function isFormValid() {
                let durationObj = new Object($scope.durationObj);
                $scope.clearErrors();
                if (!$scope.requester) $scope.requester = {};
                $scope.validatePhoneNumber({
                    value: $scope.participantBudget.participants,
                    name: "PhoneNumber",
                });
                $scope.validateEmail({
                    value: $scope.participantBudget.participants,
                    name: "Email",
                });
                $scope.validateRequired({
                    value: $scope.trainingDetails.typeOfTraining,
                    name: "Type Of Training",
                });
                $scope.validateRequired({
                    value: $scope.trainingDetails.existingCourse,
                    name: "Existing Course",
                });
                if ($scope.trainingDetails.existingCourse)
                    $scope.validateRequired({
                        value: $scope.trainingDetails.reasonOfTrainingRequest,
                        name: "Reason Of Training Request",
                    });
                $scope.validateDateRange({
                    value: $scope.requester.dateOfSubmission,
                    name: "Date of Submission",
                    message: $translate.instant("TRAINING_REQUEST_SUBMITSSION_VALIDATE"),
                });
                if ($scope?.trainingDetails?.existingCourse == "Request New Course") {
                    $scope.validateRequired({
                        value: $scope.trainingDetails.courseName,
                        name: "Course Name",
                    });
                    $scope.validateRequired({
                        value: $scope.trainingDetails.trainingFee,
                        name: "Training Fee",
                        message: $translate.instant("TRAINING_FEE_VALIDATION"),
                    });
                    $scope.validateRequired({
                        value: $scope.trainingDetails.currency,
                        name: "Currency",
                    });
                    $scope.validateRequired({
                        value: $scope.trainingDetails.supplierName,
                        name: "Supplier Name",
                    });
                }
                //lamnl add require trainingFee has value require Currency
                if ($scope?.trainingDetails?.trainingFee != null) {
                    $scope.validateRequired({
                        value: $scope.trainingDetails.currency,
                        name: "Currency",
                    });
                }
                if ($scope?.trainingDetails?.existingCourse == "Existing Course") {
                    $scope.validateRequired({
                        value: $scope.trainingDetails.categoryId,
                        name: "Course Category",
                    });
                    $scope.validateRequired({
                        value: $scope.trainingDetails.courseId,
                        name: "Course Name",
                    });
                }
                $scope.validateRequired({
                    value: $scope.trainingDuration.estimatedTrainingDays,
                    name: "Estimated Training Days",
                });
                if (durationObj.isOnline() || durationObj.isOnlineAndOffline()) {
                    $scope.validateDateRange({
                        value: $scope.durationOnlineMethod.to,
                        currentDate: $scope.durationOnlineMethod.from,
                        name: "Online Duration",
                        message: $translate.instant("TRAINING_DURATION_VALIDATION"),
                    });
                    // $scope.validateRequired({
                    //   value: $scope.durationOnlineMethod.from,
                    //   name: "Online Duration To",
                    // });
                    // $scope.validateRequired({
                    //   value: $scope.durationOnlineMethod.to,
                    //   name: "Online Duration From",
                    // });
                }
                if (durationObj.isOffline() || durationObj.isOnlineAndOffline()) {
                    $scope.validateDateRange({
                        value: $scope.durationOfflineMethod.to,
                        currentDate: $scope.durationOfflineMethod.from,
                        name: "Offline Duration",
                        message: $translate.instant("TRAINING_DURATION_VALIDATION"),
                    });
                    // $scope.validateRequired({
                    //   value: $scope.durationOfflineMethod.from,
                    //   name: "Offline Duration From",
                    // });
                    // $scope.validateRequired({
                    //   value: $scope.durationOfflineMethod.to,
                    //   name: "Offline Duration To",
                    // });
                }
                $scope.validateRequired({
                    value: $scope.purposeAndReason.purpose,
                    name: "What does learner do to execute?",
                });
                $scope.validateRequired({
                    value: $scope.purposeAndReason.reason,
                    name: "Why does learner need to attend this course?",
                });
                $scope.validateRequired({
                    value: $scope.purposeAndReason.howApply,
                    name: "How does learner will apply?",
                });
                $scope.validateRequired({
                    value: $scope.purposeAndReason.whenExcute,
                    name: "When does learner execute the plan?",
                });
                $scope.validateRequired({
                    value: $scope.purposeAndReason.whatExpectedKPI,
                    name: "What are the expected KPI of the application?",
                });
                if ($scope.participantBudget.participants.length <= 0) {
                    $scope.addError({
                        fieldName: "Participants",
                        error: $translate.instant("PARTICIPANTS_VALIDATION"),
                    });
                }
                if ($scope.trainingSponsorshipContract.applySponsorship) {
                    if (
                        !$scope.trainingSponsorshipContract.sponsorshipPercentage &&
                        !$scope.trainingSponsorshipContract.actualTuitionReimbursementAmount
                    ) {
                        $scope.addError({
                            fieldName: "Actual Tuition Reimbursement Amount",
                            error: $translate.instant("COMMON_FIELD_IS_REQUIRED"),
                        });
                        $scope.addError({
                            fieldName: "Tuition Reimbursement Percentage",
                            error: $translate.instant("COMMON_FIELD_IS_REQUIRED"),
                        });
                    }
                }

                if ($scope.workingCommitment.workingCommitment) {
                    $scope.validateRequired({
                        value: $scope.workingCommitment.from,
                        name: "FromDate",
                    });
                    $scope.validateRequired({
                        value: $scope.workingCommitment.to,
                        name: "ToDate",
                    });
                    $scope.validateDateRange({
                        value: $scope.workingCommitment.to,
                        currentDate: $scope.workingCommitment.from,
                        name: "ToDate",
                        message: $translate.instant(
                            $translate.instant("TRAINING_COMMITMENT_VALIDATION")
                        ),
                    });
                    $scope.validateRequired({
                        value: $scope.workingCommitment.sponsorshipContractNumber,
                        name: "Sponsorship ContractNumber",
                    });
                }
                if ($scope.trainingDetails.typeOfTraining == "External") {
                    $scope.validateRequired({
                        value: $scope.participantBudget.departmentInChargeCode,
                        name: "Department In Charge",
                    });
                    $scope.validateRequired({
                        value: $scope.participantBudget.requestedDepartment,
                        name: "Requested Department",
                    });
                    $scope.validateRequired({
                        value: $scope.participantBudget.year,
                        name: "Year",
                    });
                    $scope.validateRequired({
                        value: $scope.participantBudget.from,
                        name: "From",
                    });
                    $scope.validateRequired({
                        value: $scope.participantBudget.to,
                        name: "To",
                    });
                    $scope.validateDateRange({
                        value: $scope.participantBudget.toObj,
                        currentDate: $scope.participantBudget.fromObj,
                        name: "Month",
                        message: $translate.instant("COSTCENTER_MONTH_VALIDATION"),
                    });
                    $scope.validateRequired({
                        value: $scope.participantBudget.theProposalFor,
                        name: "The Proposal for",
                    });
                    if ($scope.participantBudget.theProposalFor == "PO with Principle Contract") {
                        $scope.validateRequired({
                            value: $scope.participantBudget.reference,
                            name: "Reference Number",
                        });
                    }
                    if ($scope.costCenters.length <= 0) {
                        $scope.addError({
                            fieldName: "Cost Center",
                            error: $translate.instant("COSTCENTER_REQUIRED"),
                        });
                    } else {
                        $scope.costCenters.forEach((item) => {
                            $scope.validateRequired({
                                value: item.budgetCode,
                                name: "Budget Code",
                            });
                            $scope.validateRequired({
                                value: item.costCenterCode,
                                name: "Cost Center Code",
                            });
                            $scope.validateRequired({
                                value: item.amount,
                                name: "Amount",
                            });
                            if (item.vatPercentage != 0) {
                                $scope.validateRequired({
                                    value: item.vatPercentage,
                                    name: "VAT Percentage",
                                });
                            }
                        });
                    }
                }
                return $scope.validateForm();
            }
            function formatNumberToInt(num) {
                if (!num) return num;
                return Number.parseFloat(num).toFixed(0);
            }
            function watch(value) {
                kendo.ui.progress($("#loading"), value);
            }
            /* End */

            /* Form action functions */
            async function mappingDataModel() {
                let durationObj = new Object($scope.durationObj);
                if (!$scope.status) $scope.status = "Draft";
                let requester = { ...$scope.requester };
                let trainingDetails = {
                    ...$scope.trainingDetails,
                    trainingNotTotalFee: $scope.trainingDetails.trainingNotTotalFee || 0.0,
                    accommodationIfAny: $scope.trainingDetails.accommodationIfAny || 0.0,
                    airTicketFeeIfAny: $scope.trainingDetails.airTicketFeeIfAny || 0.0,
                    trainingFee: $scope.trainingDetails.trainingFee || 0.0,
                };
                if ($scope.trainingDetails.existingCourse == "Request New Course") {
                    let supplier = $scope.supplierOptions.find(
                        (item) => item.name == $scope.trainingDetails.supplierName
                    );
                    trainingDetails = {
                        ...$scope.trainingDetails,
                        supplierName: supplier?.name || $scope.trainingDetails.supplierName,
                        supplierCode: supplier?.code || null,
                    };
                }
                if ($scope.attachmentDetails.length) {
                    trainingDetails.attachments = $scope.attachmentDetails;
                } else {
                    trainingDetails.attachments = [];
                }
                if ($scope.trainingDetails.currencyJson) {
                    trainingDetails.currencyJson = JSON.stringify($scope.trainingDetails.currencyJson);
                }
                let trainingDuration = { ...$scope.trainingDuration };
                trainingDuration.estimatedTrainingHours = formatNumberToInt(
                    trainingDuration.estimatedTrainingHours
                );
                trainingDuration.estimatedTrainingDays = formatNumberToInt(
                    trainingDuration.estimatedTrainingDays
                );
                trainingDuration.trainingDurationItems = [];
                if (
                    $scope.durationOnlineMethod &&
                    (durationObj.isOnline() || durationObj.isOnlineAndOffline())
                ) {
                    trainingDuration.trainingDurationItems.push(
                        $scope.durationOnlineMethod
                    );
                }
                if (
                    $scope.durationOfflineMethod &&
                    (durationObj.isOffline() || durationObj.isOnlineAndOffline())
                ) {
                    trainingDuration.trainingDurationItems.push(
                        $scope.durationOfflineMethod
                    );
                }
                let requestedDept = $scope.cleanDept.find(
                    (item) => item.text == $scope.participantBudget.requestedDepartment
                );
                let dic = $scope.departmentInChargesOptions.find(
                    (item) => item.dicCode == $scope.participantBudget.departmentInChargeCode
                );
                let participantBudget = {
                    ...$scope.participantBudget,
                    departmentInCharge: dic?.name || null,
                    requestedDepartment: requestedDept?.text || null,
                    requestedDepartmentId: requestedDept?.id || null,
                    departmentInChargeCode: dic?.dicCode || null,
                    departmentInChargeId: dic?.id || null,
                    dicDepartmentCode: dic?.code || null,
                    requestedDepartmentCode: requestedDept?.code != undefined ? requestedDept.code : null,
                    participants: getRevertParticipants(
                        $scope.participantBudget.participants
                    ),
                    to: $scope.participantBudget.toObj,
                    from: $scope.participantBudget.fromObj,
                    reference: $scope.participantBudget.theProposalFor == "PO with Principle Contract" ? $scope.participantBudget.reference : '',
                };
                let trainingSponsorshipContract = {
                    ...$scope.trainingSponsorshipContract,
                };
                let workingCommitment = {
                    ...$scope.workingCommitment,
                    from: $scope.workingCommitment.workingCommitment
                        ? $scope.workingCommitment.from
                        : null,
                    to: $scope.workingCommitment.workingCommitment
                        ? $scope.workingCommitment.to
                        : null,
                };
                let costCenters = [...$scope.costCenters];
                var dataModel = {
                    id: $scope.id,
                    status: $scope.status,
                    referenceNumber: $scope.referenceNumber,
                    AcademyDepartmentId: "9811A736-240F-4E6F-B708-1673BD55238A",
                    requester: requester,
                    trainingDetails: trainingDetails,
                    trainingDuration: trainingDuration,
                    participantBudget: participantBudget,
                    trainingSponsorshipContract: trainingSponsorshipContract,
                    workingCommitment: workingCommitment,
                    purpose: $scope.purposeAndReason.purpose,
                    reason: $scope.purposeAndReason.reason,
                    howApply: $scope.purposeAndReason.howApply,
                    whenExcute: $scope.purposeAndReason.whenExcute,
                    whatExpectedKPI: $scope.purposeAndReason.whatExpectedKPI,
                    costCenters: costCenters,
                };
                return dataModel;
            }
            function getRevertParticipants(participants) {
                return participants.length > 0
                    ? participants.map((item) => {
                        return {
                            userId: item.userId,
                            sapCode: item.sapCode,
                            email: item.email,
                            name: item.name,
                            phoneNumber: item.phoneNumber,
                            position: item.position,
                            department: item.departmentName,
                        };
                    })
                    : [];
            }

            async function getAffiliate() {
                if ($rootScope.currentUser) {
                    let deptId = $rootScope.currentUser.divisionId
                        ? $rootScope.currentUser.divisionId
                        : $rootScope.currentUser.deptId;
                    if (deptId) {
                        let result = await settingService
                            .getInstance()
                            .departments.getDepartmentById({ id: deptId }).$promise;
                        if (result.isSuccess) {
                            var valueTitleG5 = _.find($scope.jobGradeList, item => item.title === 'G5');
                            var valueTitleG4 = _.find($scope.jobGradeList, item => item.title === 'G4');
                            if (result.object.isStore && result.object.jobGradeGrade < valueTitleG5.grade) {
                                let jG = valueTitleG5.grade;
                                let parentId = result.object.parentId;
                                let dept = result;
                                while (jG < valueTitleG4.grade) {
                                    dept = await settingService
                                        .getInstance()
                                        .departments.getDepartmentById({ id: parentId }).$promise;
                                    if (dept.isSuccess) {
                                        jG = dept.object.jobGradeGrade;
                                        parentId = dept.object.parentId;
                                    }
                                }
                                $scope.affiliateName = dept.object.name;
                            } else {
                                if (result.object.parentId) {
                                    await settingService
                                        .getInstance()
                                        .departments.getDepartmentById({
                                            id: result.object.parentId,
                                        })
                                        .$promise.then(function (res) {
                                            if (res.isSuccess) {
                                                $scope.affiliateName = res.object.name;
                                            }
                                        });
                                } else {
                                    $scope.affiliateName = result.object.name;
                                }
                            }
                        }
                    }
                }
            }
            function getPosition() {
                let jobGradeRegex = RegExp(/\(G\d\)/g);
                if (jobGradeRegex.test($scope.currentUser?.positionName)) {
                    return $scope.currentUser?.positionName;
                } else {
                    return (
                        $scope.currentUser?.positionName +
                        " (" +
                        $scope.currentUser?.jobGradeName +
                        ")"
                    );
                }
            }
            async function internalSave() {
                $scope.participantBudget.participants =
                    $("#participantGrid").data("kendoGrid").dataSource._data;
                
                $scope.costCenters =
                    $("#budgetGrid").data("kendoGrid")?.dataSource._data || [];

                if (!isFormValid()) {
                    $window.scrollTo(0, 0);
                    return;
                }
                var model = await mappingDataModel();
                $scope.isInprocess = true;
                watch(true);
                var result = await TrainingRequestService.save(model).$promise;
                watch(false);
                return result;
            }
            async function save() {
                try {
                    var result = await internalSave();
                    if (!result) return;
                    Notification.success($translate.instant("COMMON_SAVE_SUCCESS"));
                    $scope.isInprocess = false;
                    $state.go(
                        appSetting.ACADAMY_ROUTES.editRequest,
                        { id: result.id },
                        { reload: true }
                    );
                } catch (e) {
                    watch(false);
                    console.error(e);
                    Notification.error("Error System");
                    $scope.isInprocess = false;
                }
            }

            async function submit() {
                try {
                    var itemId = $scope.id;
                    var saveResult = await internalSave();
                    if (!saveResult) return;
                    itemId = saveResult.id;
                    var req = { requestId: itemId, comment: "" };
                    watch(true);
                    await TrainingRequestService.submit(req).$promise;
                    Notification.success("Your request has been submitted successfully.");
                    $scope.isInprocess = false;
                    watch(false);

                    $state.go(
                        appSetting.ACADAMY_ROUTES.editRequest,
                        { id: itemId },
                        { reload: true }
                    );
                } catch (e) {
                    watch(false);
                    console.error(e);
                    $scope.isInprocess = false;
                    try {
                        var errorData = e.data;
                        var match = errorData.match(/\[.*\]/s);
                        if (match) {
                            var parsed = JSON.parse(match[0]);
                            if (parsed.length > 0) {
                                Notification.error(parsed[0].ErrorMessage || "Error System");
                                return;
                            }
                        }
                        Notification.error(errorData || "Error System");
                    } catch (err) {
                        Notification.error("Error System");
                    }
                }
            }
            async function approve() {
                try {
                    var itemId = $scope.id;
                    $scope.isInprocess = true;
                    watch(true);
                    var req = { requestId: itemId, comment: $scope.comment };
                    await TrainingRequestService.approve(req).$promise;
                    Notification.success("The request has been approved successfully.");
                    $scope.isInprocess = false;
                    watch(false);
                    $state.go(
                        appSetting.ACADAMY_ROUTES.editRequest,
                        { id: itemId },
                        { reload: true }
                    );
                } catch (e) {
                    watch(false);
                    console.error(e);
                    $scope.isInprocess = false;

                    Notification.error("Error System");
                }
            }

            async function reject() {
                try {
                    var itemId = $scope.id;
                    $scope.isInprocess = true;
                    watch(true);
                    var req = { requestId: itemId, comment: $scope.comment };
                    await TrainingRequestService.reject(req).$promise;
                    Notification.success("The request has been rejected successfully.");
                    $scope.isInprocess = false;
                    watch(false);
                    $state.go(
                        appSetting.ACADAMY_ROUTES.editRequest,
                        { id: itemId },
                        { reload: true }
                    );
                } catch (e) {
                    watch(false);
                    console.error(e);
                    $scope.isInprocess = false;
                    Notification.error("Error System");
                }
            }

            async function requestChange() {
                try {
                    var itemId = $scope.id;
                    $scope.isInprocess = true;
                    watch(true);
                    var req = { requestId: itemId, comment: $scope.comment };
                    await TrainingRequestService.requestToChange(req).$promise;
                    Notification.success(
                        "The request has been requested to change successfully."
                    );
                    $scope.isInprocess = false;
                    watch(false);
                    $state.go(
                        appSetting.ACADAMY_ROUTES.editRequest,
                        { id: itemId },
                        { reload: true }
                    );
                } catch (e) {
                    watch(false);
                    console.error(e);
                    $scope.isInprocess = false;
                    Notification.error("Error System");
                }
            }

            async function cancel() {
                try {
                    var itemId = $scope.id;
                    $scope.isInprocess = true;
                    watch(true);
                    var req = { requestId: itemId, comment: $scope.comment };
                    await TrainingRequestService.cancel(req).$promise;
                    Notification.success(
                        "The request has been cancelled successfully."
                    );
                    $scope.isInprocess = false;
                    watch(false);
                    $state.go(
                        appSetting.ACADAMY_ROUTES.editRequest,
                        { id: itemId },
                        { reload: true }
                    );
                } catch (e) {
                    watch(false);
                    console.error(e);
                    $scope.isInprocess = false;
                    Notification.error("Error System");
                }
            }

            function close() {
                $state.go(appSetting.ACADAMY_ROUTES.home);
            }

            function createTrainingInvitation() {
                $state.go(
                    appSetting.ACADAMY_ROUTES.trainingInvitation,
                    { id: $scope.id },
                    { reload: true }
                );
            }

            $scope.createTrainingInvitation = function () {
                $state.go(
                    appSetting.ACADAMY_ROUTES.trainingInvitation,
                    { id: $scope.id },
                    { reload: true }
                );
            };

            async function getTrainingReasons() {
                var result = await TrainingReasonService.list().$promise;
                if (result) {
                    //let emptyItem = { id: "", value: "" };
                    $scope.trainingReasons = [
                        //emptyItem,
                        ...JSON.parse(JSON.stringify(result)),
                    ];
                }
            }
            $scope.filterCharacter = function (e) {
                var regex = new RegExp(/^[a-zA-Z0-9 .,()-]+$/g);
                var str = String.fromCharCode(!e.charCode ? e.which : e.charCode);
                if (regex.test(str)) {
                    return true;
                }

                e.preventDefault();
                return false;
            };
            /* End */

            async function getWorkflowProcessingStage(itemId) {

                $scope.currentInstanceProcessingStage.workflowData.histories = $scope.currentInstanceProcessingStage.histories
                var result = await workflowService.getInstance().workflows.getWorkflowStatus({ itemId: itemId }).$promise;
                if (result.isSuccess && result.object) {
                
                    $scope.currentInstanceProcessingStage = result.object.workflowInstances[result.object.workflowInstances.length - 1];
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

            async function getDepartmentLineIds() {
                try {
                    let opt = { id: $rootScope.currentUser?.id }
                    let result = await settingService.getInstance().departments.getDepartmentUpToG4ByUserId({ ...opt }, null).$promise;
                    if (result.isSuccess) {
                        let lstDept = [...result.object.data];
                        let deptIds = [];
                        if (lstDept) {
                            lstDept.forEach(x => {
                                deptIds.push(x.deptLine.id);
                            });
                        }
                        return deptIds;
                    }
                }
                catch (e) {
                    console.error(e);
                    Notification.error("Error System");
                }
            }

            function makeFlatDepartmentData(obj) {
                let result = [];
                let item = {"id": obj?.id, "parent": obj?.parentId != null ? obj?.parentId : '#', "text": obj?.name};
                for (let key in obj) {
                    if (key !== "id" && key !== "parentId" && key !== "name" && obj.hasOwnProperty(key)) {
                        item[key] = obj[key];
                    }
                }
                result.push(item);
                if (obj?.items && obj?.items?.length > 0) {
                    for (let item of obj?.items) {
                        let newItem = {"id": item.id, "parent": item.parentId || obj.id, "text": item.name};
                        for (let key in item) {
                            if (key !== "id" && key !== "parentId" && key !== "name" && item.hasOwnProperty(key)) {
                                newItem[key] = item[key];
                            }
                        }
                        result.push(newItem);
                        if (item.items && item.items.length > 0) {
                            result = result.concat(makeFlatDepartmentData(item));
                        }
                    }
                }

                return result;
            }

            function makeCleanDepartmentData(dept) {
                 const temp = dept.map(obj => {
                    const newObj = { ...obj };
                    delete newObj.items;
                    return newObj;
                 });

                 return temp.filter((obj, index, self) => index === self.findIndex((t) => t.id === obj.id));
            }

            function processTable(data, idField, foreignKey, rootLevel) {
              var hash = {};

              for (var i = 0; i < data.length; i++) {
                var item = data[i];
                var id = item[idField];
                var parentId = item[foreignKey];

                hash[id] = hash[id] || [];
                hash[parentId] = hash[parentId] || [];

                item.items = hash[id];
                hash[parentId].push(item);
              }

              return hash[rootLevel];
            }
        }
    );
