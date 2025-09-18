var ssgApp = angular.module("ssg.settingRecruitmentPromoteTransferPrintModule", ["kendo.directives"]);
ssgApp.controller("settingRecruitmentPromoteTransferPrintController", function (
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
    $scope.numeric = {
        format: '',
        min: 0
    }

    $scope.total = 0;
    $scope.keyword = '';
    $scope.dataPromoteAndTranferPrint = [];
    //API
    async function getListPromoteAndTranferPrint(option) {
        let args = buildArgs($scope.currentQuery.Page, appSetting.pageSizeDefault);
        if (option) {
            $scope.currentQuery.limit = option.data.take;
            $scope.currentQuery.page = option.data.page;
        }
        $scope.currentQuery.predicate = args.predicate;
        $scope.currentQuery.predicateParameters = args.predicateParameters;
        var result = await settingService.getInstance().recruitment.getPromoteAndTranferPrintValue($scope.currentQuery).$promise;
        console.log(result);
        if (result.isSuccess) {
            $scope.dataPromoteAndTranferPrint = result.object.data;
            var count = (($scope.currentQuery.page - 1) * $scope.currentQuery.limit) + 1;
            $scope.dataPromoteAndTranferPrint.forEach(x => {
                x["select"] = false;
                x["no"] = count;
                count = count + 1;
            });
            let nValue = {
                id: '',
                removingValue: ""
            };
            $scope.dataPromoteAndTranferPrint.push(nValue);
            $scope.total = result.object.count;
            if (option) {
                option.success($scope.dataPromoteAndTranferPrint);
            }
            else {
                let grid = $("#grid").data("kendoGrid");
                grid.dataSource.read();
                grid.dataSource.page($scope.currentQuery.page);
            }
        }
    }

    function buildArgs(pageIndex, pageSize) {
        let predicate = [];
        let predicateParameters = [];
        if ($scope.keyword) {
            predicate.push('(removingValue.contains(@0))');
            predicateParameters.push($scope.keyword);
        }
        var option = QueryArgs(predicate, predicateParameters, appSetting.ORDER_GRID_DEFAULT, pageIndex, pageSize);
        return option;
    }

    // Get list
    $scope.PromoteAndTranferPrintOption = {
        dataSource: {
            serverPaging: true,
            pageSize: 20,
            transport: {
                read: async function (e) {
                    await getListPromoteAndTranferPrint(e);
                }
            },
            schema: {
                total: () => { return $scope.total },
                data: () => { return $scope.dataPromoteAndTranferPrint }
            }
        },
        sortable: false,
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
                width: "50px",
                editor: function (container, options) {
                    $(`<label>${options.model[options.field]}</label>`).appendTo(
                        container
                    );
                }
            },
            {
                field: "removingValue",
                title: "Removing Value",
                width: "100px",
                template: function (dataItem) {
                    return `<input class="k-textbox w100" autoComplete="off" name="code" ng-model="dataItem.removingValue"/>`;
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
    function checkEdit() {
        var result = false;
        $scope.dataPromoteAndTranferPrint.forEach(item => {
            if (item.select === true && item.id !== '') {
                result = true;
                return result;
            }
        });
        return result
    }
    let requiredFields = [
        {
            fieldName: "removingValue",
            title: "RemovingValue"
        }
    ];
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
            else if (errors.length == 0) {
                var checkCode = await findPromoteAndTranferPrintByName(model.removingValue);
                if (checkCode) {
                    Notification.error("Value " + model.code + " has existed in Promote and Tranfer Print.");
                }
                else {
                    settingService.getInstance().recruitment.createPromoteAndTranferPrint({ Id: model.id, RemovingValue: model.removingValue}).$promise.then(function (result) {
                        if (result.isSuccess) {
                            Notification.success("Data Successfully Saved");
                            loadPageOne($scope.keyword = '');
                            return true;
                        }
                    });
                }
            }
        }
    };
    async function findPromoteAndTranferPrintByName(name, id) {
        let queryArgs = {
            predicate: '',
            predicateParameters: [],
            order: appSetting.ORDER_GRID_DEFAULT,
            page: 0,
            limit: appSetting.pageSizeDefault
        };

        queryArgs.predicateParameters.push(name);
        queryArgs.predicateParameters.push(id);
        var result = await settingService.getInstance().recruitment.getPromoteAndTranferPrintByName(queryArgs).$promise;
        if (result.object.count >= 1) {
            return true;
        }
        return false;
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
        let grid = $("#grid").data("kendoGrid");
        grid.dataSource.fetch(() => grid.dataSource.page(1));
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
    function validateEdit() {
        var result = false;
        $scope.dataPromoteAndTranferPrint.forEach(item => {
            if (item.select === true && item.id !== '') {
                result = true;
                return result;
            }
        });
        return result
    }

    confirm = function (e) {
        let grid = $("#grid").data("kendoGrid");
        if (e.data && e.data.value) {
            settingService.getInstance().recruitment.deletePromoteAndTranferPrint({ Id: $scope.dataDelete.id }).$promise.then(function (result) {
                if (result.isSuccess) {
                    Notification.success("Data Successfully Saved");
                    getListPromoteAndTranferPrint();
                    grid.refresh();
                }
                else {
                    Notification.error(result.messages[0]);
                }
            });
        }
    }
    async function save(model) {
        let errors = $rootScope.validateInRecruitment(requiredFields, model);
        if (errors.length > 0) {
            let errorList = errors.map(x => {
                return x.controlName + " " + x.errorDetail;
            });
            Notification.error(`Some fields are required: </br>
            <ul>${errorList.join('<br/>')}</ul>`);

        }
        else if (errors.length == 0) {
            var checkCode = await findPromoteAndTranferPrintByName(model.removingValue);
            if (checkCode) {
                Notification.error("Value " + model.code + " has existed in Promote and Tranfer Print.");
            }
            else {
                settingService.getInstance().recruitment.updatePromoteAndTranferPrint({ Id: model.id, RemovingValue: model.removingValue }).$promise.then(function (result) {
                    if (result.isSuccess) {
                        Notification.success("Data Successfully Saved");
                        getListPromoteAndTranferPrint();
                        return true;
                    }
                });
            }
        }
    }
    function cancel(model) {
        model['select'] = false;
        var item = $scope.dataPromoteAndTranferPrint.find(x => x.id === model.id);
        item.select = false;
        model.removingValue = item.removingValue;
        let grid = $("#grid").data("kendoGrid");
        grid.refresh();
    }

    function edit(model) {
        var result = validateEdit();
        if (result) {
            Notification.error("Please save selected item before edit/delete other item");
        }
        else {
            model['select'] = true;
            var item = $scope.dataPromoteAndTranferPrint.find(x => x.id === model.id);
            item.select = true;
            let grid = $("#grid").data("kendoGrid");
            grid.refresh();
        }

    }

    $scope.export = async function () {
        let model = {
            predicate: '',
            predicateParameters: [],
            order: "Modified desc",
        }

        if ($scope.keyword) {
            model.predicate = '(removingValue.contains(@0))';
            model.predicateParameters.push($scope.keyword);
        }

        var res = await fileService.getInstance().processingFiles.export({
            type: commonData.exportType.MAINTAINPROMOTEANDTRANFERPRINT
        }, model).$promise;
        if (res.isSuccess) {
            exportToExcelFile(res.object);
        }
    }
});