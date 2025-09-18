var ssgApp = angular.module("ssg.settingRecruitmentCategoriesModule", [
    "kendo.directives"
]);
ssgApp.controller("settingRecruitmentCategoriesController", function (
    $rootScope,
    $scope,
    $location,
    $stateParams,
    appSetting,
    commonData,
    Notification,
    settingService,
    fileService,
    massService
) {
    // create a message to display in our view
    var ssg = this;
    $scope.advancedSearchMode = false;
    $rootScope.isParentMenu = false;
    $scope.title = $stateParams.action.title;

    // phần khai báo chung
    $scope.actions = {
        save: save,
        cancel: cancel,
        edit: edit,
        deleteRecord: deleteRecord,
        create: create,
        ifEnter: ifEnter
    };

    $scope.currentQuery = {
        predicate: "",
        predicateParameters: [],
        order: "Modified desc",
        limit: appSetting.pageSizeDefault,
        page: 1
    };
    $scope.total = 0;
    $scope.keyword = '';
    $scope.dataCategories = [];
    $scope.totalCategories = [];
    //API
    async function getCategories(option) {
        let args = buildArgs($scope.currentQuery.Page, appSetting.pageSizeDefault);
        if (option) {
            $scope.currentQuery.limit = option.data.take;
            $scope.currentQuery.page = option.data.page;
        }
        $scope.currentQuery.predicate = args.predicate;
        $scope.currentQuery.predicateParameters = args.predicateParameters;

        var result = await settingService.getInstance().recruitment.getCategories($scope.currentQuery).$promise;
        if (result.isSuccess) {
            $scope.dataCategories = result.object.data;
            var count = (($scope.currentQuery.page - 1) *  $scope.currentQuery.limit) + 1;
            if ($scope.dataCategories.length) {
                $scope.dataCategories.forEach(x => {
                    x["select"] = false;
                    x["no"] = count;
                    count = count + 1;
                });
            }
            let nValue = {
                id: '',
                code: "",
                name: "",
                select: true,
            };
            $scope.dataCategories.push(nValue);
            $scope.total = result.object.count;
            if (option) {
                option.success($scope.dataCategories);
            }
            else {
                let categoryGrid = $("#categoryGrid").data("kendoGrid");
                categoryGrid.dataSource.read();
                categoryGrid.dataSource.page($scope.currentQuery.page);
            }
        }
    }
    // lấy tất cả categories
    async function getAllCategories(){
        let args = buildArgs(1, 1000);
        // $scope.currentQuery.limit = 1000;
        // $scope.currentQuery.page = 1;
        $scope.currentQuery.predicate = args.predicate;
        $scope.currentQuery.predicateParameters = args.predicateParameters;

        var result = await settingService.getInstance().recruitment.getCategories($scope.currentQuery).$promise;
        if (result.isSuccess) {
            $scope.totalCategories = result.object.data;
        }
    }
    function buildArgs(pageIndex, pageSize) {
        let predicate = [];
        let predicateParameters = [];
        if ($scope.keyword) {
            predicate.push('Name.contains(@0)');
            predicateParameters.push($scope.keyword);  
        }
        var option = QueryArgs(predicate, predicateParameters, appSetting.ORDER_GRID_DEFAULT, pageIndex, pageSize);
        return option;
    }
    
    //update mass
    async function updateMass(operation, data) {
        data.isEdoc2 = true;
        switch (operation) {
            case 'add':
                massService.getInstance().mass.addCategory(data)
                .$promise.then(function (result) {
                    if (result.isSuccess) {
                        Notification.success("Data saved to Mass");
                    }
                });
                break;
            case 'update':
                massService.getInstance().mass.updateCategory(data)
                .$promise.then(function (result) {
                    if (result.isSuccess) {
                        Notification.success("Data saved to Mass");
                    }
                });
                break;
            case 'delete':
                massService.getInstance().mass.deleteCategory(data)
                .$promise.then(function (result) {
                    if (result.isSuccess) {
                        Notification.success("Data saved to Mass");
                    }
                });
                break;
        }
    }


    $scope.categoryOptions = {
        dataSource: {
            serverPaging: true,
            pageSize: 20,
            transport: {
                read: async function (e) {
                    await getCategories(e);
                }
            },
            schema: {
                total: () => { return $scope.total },
                data: () => { return $scope.dataCategories }
            }
        },
        sortable: false,
        pageable: {
            alwaysVisible: true,
            pageSizes: appSetting.pageSizesArray
        },
        columns: [{
            field: "no",
            title: "NO.",
            width: "50px",
            editor: function (container, options) {
                $(`<label>${options.model[options.field]}</label>`).appendTo(
                    container
                );
            }
        },
        {
            field: "name",
            title: "Name",
            width: "125px",
            template: function (dataItem) {
                if (dataItem.select) {
                    return `<input class="k-textbox w100" autoComplete="off" name="name"  ng-model="dataItem.name"/>`;
                } else {
                    return `<span>{{dataItem.name}}</span>`;
                }
            }
        },
        {
            field: "priority",
            title: "Priority",
            width: "50px",
            template: function (dataItem) {
                if (dataItem.select) {
                    return `<input kendo-numeric-text-box k-min="0" class="w100" k-format="'#,0'" autoComplete="off" name="code"  ng-model="dataItem.priority"/>`;
                } else {
                    return `<span>{{dataItem.priority}}</span>`;
                }
            }
        },
        {
            field: "parentCategory",
            title: "Parent Item",
            width: "125px",
            template: function (dataItem) {
                if (dataItem.select || !dataItem.id) {
                    return `<select kendo-combo-box style="width: 100%;" name="parentId" 
                        data-k-ng-model="dataItem.parentId"
                        k-data-text-field="'name'"
                        k-data-value-field="'id'"
                        filter = "'contains'"
                        k-data-source="totalCategories"
                        k-auto-bind="'false'"
                        k-value-primitive="'false'"
                        > </select>`
                } else {
                    return `<span>${dataItem.parentName ? dataItem.parentName : ""}</span>`
                }
            }
        },
        {
            title: "ACTIONS",
            width: "150px",
            template: function (dataItem) {
                if (!dataItem.id) {
                    return `
                        <a class="btn btn-sm btn-primary " ng-click="actions.create(dataItem)"><i class="fa fa-plus right-5"></i>Create</a>
                        `
                }
                if (dataItem.select) {
                    return `
                        <a class="btn btn-sm default green-stripe " ng-click="actions.save(dataItem)">Save</a>
                        <a class="btn btn-sm default " ng-click="actions.cancel(dataItem)">Cancel</a>
                `;
                } else {
                    return `
                        <a class="btn btn-sm default blue-stripe " ng-click="actions.edit(dataItem)">Edit</a>
                        <a class="btn btn-sm default red-stripe " ng-click="actions.deleteRecord(dataItem)">Delete</a>
                `;
                }
            }
        }
        ]
    };

    async function create(model) {
        var editExist = checkEdit();
        if (editExist) {
            Notification.error("Please save selected item before create other item");
        }
        else {
            let errors = $rootScope.validateInRecruitment(requiredFields, model);
            if (errors.length > 0) {
                let errorList = errors.map(x => {
                    return x.controlName + " " + x.errorDetail;
                });
                Notification.error(`Some fields are required: </br>
                <ul>${errorList.join('<br/>')}</ul>`);
                
            }
            else if(errors.length == 0){
                settingService.getInstance().recruitment.addCategory(
                    { 
                        Id: model.id, Priority: model.priority, Name: model.name, ParentId: model.parentId
                    }
                    ).$promise.then(function (result) {
                    if (result.isSuccess) {
                        Notification.success("Data Successfully Saved");
                        updateMass('add',result.object);
                        loadPageOne($scope.keyword = '');
                        getAllCategories();
                        return true;
                    }
                });
            }
        }
    };

    function checkEdit() {
        var result = false;
        $scope.dataCategories.forEach(item => {
            if (item.select === true && item.id !== '') {
                result = true;
                return result;
            }
        });
        return result
    }

    $scope.errors = [];

    let requiredFields = [
    {
        fieldName: "name",
        title: "Name"
    }
    ];

    async function save(model) {
        let errors = $rootScope.validateInRecruitment(requiredFields, model);
        if (errors.length > 0) {
            let errorList = errors.map(x => {
                return x.controlName + " " + x.errorDetail;
            });
            Notification.error(`Some fields are required: </br>
            <ul>${errorList.join('<br/>')}</ul>`);
            
        }
        else if(errors.length == 0){
            settingService.getInstance().recruitment.editCategory(
                { 
                    Id: model.id, Priority: model.priority, Name: model.name, ParentId: model.parentId
                }
                ).$promise.then(function (result) {
                if (result.isSuccess) {
                    Notification.success("Data Successfully Saved");
                    updateMass('update',result.object);
                    loadPageOne($scope.keyword = '');
                    getAllCategories();
                    return true;
                }
            });
        }
    }

    async function ifEnter($event) {
        var keyCode = $event.which || $event.keyCode;
        if (keyCode === 13) {
            loadPageOne();
        }
    }

    $scope.search = async function () {
        loadPageOne();
    }

    function loadPageOne() {
        let grid = $("#categoryGrid").data("kendoGrid");
        grid.dataSource.fetch(() => grid.dataSource.page(1));
    }


    function cancel(model) {
        model['select'] = false
        var item = $scope.dataCategories.find(x => x.id === model.id);
        item.select = false;
        model.code = item.code;
        model.name = item.name;
        let grid = $("#categoryGrid").data("kendoGrid");
        grid.refresh();
    }

    function edit(model) {
        var result = validateEdit();
        if (result) {
            Notification.error("Please save selected item before edit/delete other item");
        }
        else {
            model['select'] = true
            var item = $scope.dataCategories.find(x => x.id === model.id);
            item.select = true;
            let grid = $("#categoryGrid").data("kendoGrid");
            grid.refresh();
        }
    }

    function validateEdit() {
        var result = false;
        $scope.dataCategories.forEach(item => {
            if (item.select === true && item.id !== '') {
                result = true;
                return result;
            }
        });
        return result
    }

    $scope.dataDelete = [];
    function deleteRecord(model) {
        var result = validateEdit();
        if (result) {
            Notification.error("Please save selected item before edit/delete other item");
        }
        else {
            $scope.dataDelete = model;
            $scope.dialog = $rootScope.showConfirmDelete("DELETE", commonData.confirmContents.remove, 'Confirm');
            $scope.dialog.bind("close", confirm);
        }
    }

    confirm = function (e) {
        let categoryGrid = $("#categoryGrid").data("kendoGrid");
        if (e.data && e.data.value) {
            settingService.getInstance().recruitment.deleteCategory({ Id: $scope.dataDelete.id }).$promise.then(function (result) {
                if (result.isSuccess) {
                    Notification.success("Data Successfully Saved");
                    updateMass('delete',result.object);
                    getCategories();
                    getAllCategories();
                    categoryGrid.refresh();
                }
                else {
                    Notification.error(result.messages[0]);
                }
            });
        }
    }

    $scope.export = async function () {
        let model = {
            predicate: '',
            predicateParameters: [],
            order: "Modified desc",
        }

        if ($scope.keyword) {
            model.predicate = '(Name.contains(@0) or Code.contains(@0))';
            model.predicateParameters.push($scope.keyword);
        }

        var res = await fileService.getInstance().processingFiles.export({
            type: 21
        }, model).$promise;
        if (res.isSuccess) {
            exportToExcelFile(res.object);
        }
    }



    $scope.import = async function () {

    }


    function ngInit() {
        getAllCategories();
    }

    ngInit();

    $scope.$on('$locationChangeStart', function (event, next, current) {
        $scope.errors = [];
    });

    $scope.$on("$viewContentLoaded", function () {
    });


});