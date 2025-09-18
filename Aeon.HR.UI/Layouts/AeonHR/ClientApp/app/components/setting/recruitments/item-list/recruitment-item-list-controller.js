var ssgApp = angular.module("ssg.settingRecruitmentItemListModule", [
    "kendo.directives"
]);
ssgApp.controller("settingRecruitmentItemListController", function (
    $rootScope,
    $scope,
    $location,
    $stateParams,
    appSetting,
    commonData,
    Notification,
    settingService,
    fileService
) {
    // create a message to display in our view
    var ssg = this;
    $scope.advancedSearchMode = false;
    $scope.DateOfBirth = new Date();
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
    $scope.dataItemList = [];
    //API
    async function getAllItemList(option) {
        let args = buildArgs($scope.currentQuery.Page, appSetting.pageSizeDefault);
        if (option) {
            $scope.currentQuery.limit = option.data.take;
            $scope.currentQuery.page = option.data.page;
        }
        $scope.currentQuery.predicate = args.predicate;
        $scope.currentQuery.predicateParameters = args.predicateParameters;

        var result = await settingService.getInstance().recruitment.getItemList($scope.currentQuery).$promise;
        if (result.isSuccess) {
            $scope.dataItemList = result.object.data;
            var count = (($scope.currentQuery.page - 1) * $scope.currentQuery.limit) + 1;
            $scope.dataItemList.forEach(x => {
                x["select"] = false;
                x["no"] = count;
                count = count + 1;
            });
            let nValue = {
                id: '',
                code: "",
                name: "",
                unit: '',
                select: true,
            };
            $scope.dataItemList.push(nValue);
            $scope.total = result.object.count;
            if (option) {
                option.success($scope.dataItemList);
            }
            else {
                let gridItemList = $("#gridItemList").data("kendoGrid");
                gridItemList.dataSource.read();
                gridItemList.dataSource.page($scope.currentQuery.page);
            }
        }
    }

    function buildArgs(pageIndex, pageSize) {
        let predicate = [];
        let predicateParameters = [];
        if ($scope.keyword) {
            predicate.push('(Name.contains(@0) or Code.contains(@0))');
            predicateParameters.push($scope.keyword);  
        }
        var option = QueryArgs(predicate, predicateParameters, appSetting.ORDER_GRID_DEFAULT, pageIndex, pageSize);
        return option;
    }

    async function findItemListByCode(code, id) {
        let queryArgs = {
            predicate: '',
            predicateParameters: [],
            order: appSetting.ORDER_GRID_DEFAULT,
            page: 0,
            limit: appSetting.pageSizeDefault
        };

        queryArgs.predicateParameters.push(code);
        queryArgs.predicateParameters.push(id);
        var result = await settingService.getInstance().recruitment.getItemListByCode(queryArgs).$promise;
        if (result.object.count >= 1) {
            return true;
        }
        return false;
    }

    //
    $scope.itemListOptions = {
        dataSource: {
            serverPaging: true,
            pageSize: 20,
            transport: {
                read: async function (e) {
                    await getAllItemList(e);
                }
            },
            schema: {
                total: () => { return $scope.total },
                data: () => { return $scope.dataItemList }
            }
        },
        sortable: false,
        pageable: true,
        editable: {
            mode: "inline",
            confirmation: false
        },
        pageable: {
            alwaysVisible: true,
            pageSizes: appSetting.pageSizesArray
        },
        columns: [
            {
                field: "no",
                title: "NO.",
                width: "20px",
                editor: function (container, options) {
                    $(`<label>${options.model[options.field]}</label>`).appendTo(
                        container
                    );
                }
            },
            {
                field: "code",
                title: "Code",
                width: "70px",
                template: function (dataItem) {
                    if (dataItem.select) {
                        return `<input class="k-textbox w100" autoComplete="off" name="code" ng-model="dataItem.code"/>`;
                    } else {
                        return `<span>{{dataItem.code}}</span>`;
                    }
                }
            },
            {
                field: "name",
                title: "Name",
                width: "200px",
                template: function (dataItem) {

                    if (dataItem.select) {
                        return `<input class="k-textbox w100" autoComplete="off" name="name" ng-model="dataItem.name"/>`;
                    } else {
                        return `<span>{{dataItem.name}}</span>`;
                    }
                }
            },
            {
                field: "unit",
                title: "Unit",
                width: "100px",
                template: function (dataItem) {
                    if (dataItem.select) {
                        return `<input class="k-textbox w100" autoComplete="off" name="unit" ng-model="dataItem.unit"/>`;
                    } else {
                        return `<span>{{dataItem.unit}}</span>`;
                    }
                }
            },
            {
                title: "ACTION",
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
        let gridItemList = $("#gridItemList").data("kendoGrid");
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
                var checkCode = await findItemListByCode(model.code);
                if (checkCode) {
                    Notification.error("Code " + model.code + " has existed in Item List");
                }
                else {
                    settingService.getInstance().recruitment.addItemList({ Id: model.id, Code: model.code, Name: model.name, Unit: model.unit }).$promise.then(function (result) {
                        if (result.isSuccess) {
                            Notification.success("Data Successfully Saved");
                            loadPageOne($scope.keyword = '');
                            gridItemList.dataSource.page(1);
                        }
                    });
                }
            }
        }
    };

    function checkEdit() {
        var result = false;
        $scope.dataItemList.forEach(item => {
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
            fieldName: "code",
            title: "Code"
        },
        {
            fieldName: "name",
            title: "Name"
        },
        {
            fieldName: "unit",
            title: "Unit"
        },
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
            var checkCode = await findItemListByCode(model.code);
            if (checkCode) {
                Notification.error("Code " + model.code + " has existed in Item List");
            }
            else {
                settingService.getInstance().recruitment.editItemList({ Id: model.id, Code: model.code, Name: model.name, Unit: model.unit }).$promise.then(function (result) {
                    if (result.isSuccess) {
                        Notification.success("Data Successfully Saved");
                        getAllItemList();
                        return true;
                    }
                });
            }
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
        let grid = $("#gridItemList").data("kendoGrid");
        grid.dataSource.fetch(() => grid.dataSource.page(1));
    }

    function cancel(model) {
        model['select'] = false;
        var item = $scope.dataItemList.find(x => x.id === model.id);
        item.select = false;
        model.code = item.code;
        model.name = item.name;
        model.unit = item.unit;
        let grid = $("#gridItemList").data("kendoGrid");
        grid.refresh();
    }

    function edit(model) {
        var result = validateEdit();
        if (result) {
            Notification.error("Please save selected item before edit/delete other item");
        }
        else {
            model['select'] = true
            var item = $scope.dataItemList.find(x => x.id === model.id);
            item.select = true;
            let grid = $("#gridItemList").data("kendoGrid");
            grid.refresh();
        }

    }

    function validateEdit() {
        var result = false;
        $scope.dataItemList.forEach(item => {
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
        let gridItemList = $("#gridItemList").data("kendoGrid");
        if (e.data && e.data.value) {
            settingService.getInstance().recruitment.deleteItemList({ Id: $scope.dataDelete.id }).$promise.then(function (result) {
                if (result.isSuccess) {
                    Notification.success("Data Sucessfully Deleted");
                    getAllItemList();
                    gridItemList.refresh();
                }
                else {
                    Notification.error(result.messages[0]);
                }
            });
        }
    }

    function initGrid(pageIndex, total) {
        var count = ((pageIndex - 1) * appSetting.pageSizeDefault) + 1;
        let gridItemList = $("#gridItemList").data("kendoGrid");
        $scope.dataItemList.forEach(x => {
            x["select"] = false;
            x["no"] = count;
            count = count + 1;
        });
        let nValue = {
            id: '',
            code: "",
            name: "",
            unit: '',
            select: true,
        };
        $scope.dataItemList.push(nValue);
        let dataSourceItemList = new kendo.data.DataSource({
            data: $scope.dataItemList,
            pageSize: appSetting.pageSizeDefault,
            page: pageIndex,
            schema: {
                total: function () {
                    return total;
                }
            },
            serverPaging: true
        });
        gridItemList.setDataSource(dataSourceItemList);
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
            type: 22
        }, model).$promise;
        if (res.isSuccess) {
            exportToExcelFile(res.object);
        }
    }



    $scope.import = async function () {

    }


    function ngInit() {
        //getAllItemList(1);
    }

    ngInit();

    $scope.$on('$locationChangeStart', function (event, next, current) {
        $scope.errors = [];
    });

    $scope.$on("$viewContentLoaded", function () {
    });


});
