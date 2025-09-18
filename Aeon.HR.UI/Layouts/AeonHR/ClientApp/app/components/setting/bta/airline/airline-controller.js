var ssgApp = angular.module('ssg.airlineModule', ['kendo.directives']);
ssgApp.controller('airlineController', function ($rootScope, $scope, $location, appSetting, Notification, commonData, $stateParams, settingService, fileService, $timeout) {
    var ssg = this;
    $scope.title = 'Airline';
    isItem = true;

    $scope.keyword = '';
    keyWordOld = '';
    $scope.airlines = [];
    $scope.total = 0;
    $scope.airlineOptions = {
        dataSource: {
            serverPaging: true,
            pageSize: 20,
            transport: {
                read: async function (e) {
                    await getAirlines(e);
                }
            },
            schema: {
                total: () => { return $scope.total },
                data: () => { return $scope.airlines }
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
                template: function (dataItem) {
                    return `<span>{{dataItem.no}}</span>`;
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
                title: "ACTION",
                width: "150px",
                template: function (dataItem) {
                    if (!dataItem.id) {
                        return `<a class="btn btn-sm btn-primary " ng-click="executeAction('Create', dataItem)"><i class="fa fa-plus right-5"></i>Create</a>`;
                    }
                    if (dataItem.select) {
                        return `
                        <a class="btn btn-sm default green-stripe " ng-click="executeAction('Save', dataItem)">Save</a>
                        <a class="btn btn-sm default " ng-click="executeAction('Cancel', dataItem)">Cancel</a>`;
                    } else {
                        return `
                        <a class="btn btn-sm default blue-stripe " ng-click="executeAction('Edit', dataItem)">Edit</a>
                        <a class="btn btn-sm default red-stripe " ng-click="executeAction('Delete', dataItem)">Delete</a>`;
                    }
                }
            }
        ]
    };

    let requiredFields = [
        {
            fieldName: "code",
            title: "Code"
        },
        {
            fieldName: "name",
            title: "Name"
        }
    ];

    dataTemporary = '';
    $scope.executeAction = async function (typeAction, dataItem) {
        let grid = $("#airlineGrid").data("kendoGrid");
        switch (typeAction) {
            case 'Create':
                var checkEdit = validateEdit(grid.dataSource._data);
                if (!checkEdit) {
                    let errors = $rootScope.validateInRecruitment(requiredFields, dataItem);
                    if (errors.length > 0) {
                        let errorList = errors.map(x => {
                            return x.controlName + " " + x.errorDetail;
                        });
                        Notification.error(`Some fields are required: </br><ul>${errorList.join('<br/>')}</ul>`);
                    }
                    else {
                        let model = {
                            code: dataItem.code,
                            name: dataItem.name
                        }
                        var resultValidateCode = await settingService.getInstance().airline.checkValidateCode(model).$promise;
                        if (resultValidateCode.object.count > 0) {
                            Notification.error("Code " + dataItem.code + " has existed");
                        }
                        else {
                            var result = await settingService.getInstance().airline.saveAirline(model).$promise;
                            if (result.isSuccess) {
                                Notification.success("Data Successfully Saved");
                                loadPageOne();
                            }
                        }
                    }
                }
                else {
                    Notification.error("Please save selected item before edit/delete other item");
                }
                break;
            case 'Save':
                let errors = $rootScope.validateInRecruitment(requiredFields, dataItem);
                if (errors.length > 0) {
                    let errorList = errors.map(x => {
                        return x.controlName + " " + x.errorDetail;
                    });
                    Notification.error(`Some fields are required: </br><ul>${errorList.join('<br/>')}</ul>`);
                }
                else {
                    let flag = false;
                    let model = {
                        id: dataItem.id,
                        code: dataItem.code,
                        name: dataItem.name
                    }
                    if (dataItem.code != dataTemporary.code) {
                        var resultValidateCode = await settingService.getInstance().airline.checkValidateCode(model).$promise;
                        if (resultValidateCode.object.count > 0) {
                            flag = true;
                        }
                    }
                    if (flag) {
                        Notification.error("Code " + dataItem.code + " has existed");
                    }
                    else {
                        var result = await settingService.getInstance().airline.saveAirline(model).$promise;
                        if (result.isSuccess) {
                            Notification.success("Data Successfully Saved");
                            page = grid.pager.dataSource._page;
                            pageSize = grid.pager.dataSource._pageSize;
                            grid.dataSource.fetch(() => grid.dataSource.page(page), grid.dataSource.take(pageSize));
                        }
                    }
                }
                break;
            case 'Cancel':
                dataItem.select = false;
                dataItem.id = dataTemporary.id;
                dataItem.no = dataTemporary.no;
                dataItem.code = dataTemporary.code;
                dataItem.name = dataTemporary.name;
                grid.refresh();
                break;
            case 'Edit':
                var result = validateEdit(grid.dataSource._data);
                if (result) {
                    Notification.error("Please save selected item before edit/delete other item");
                }
                else {
                    dataTemporary = _.clone(dataItem);
                    dataItem.select = true;
                    grid.refresh();
                }
                break;
            case 'Delete':
                var result = validateEdit(grid.dataSource._data);
                if (result) {
                    Notification.error("Please save selected item before edit/delete other item");
                }
                else {
                    itemDeleteId = dataItem.id;
                    $scope.dialog = $rootScope.showConfirmDelete("DELETE", commonData.confirmContents.remove, 'Confirm');
                    $scope.dialog.bind("close", confirm);
                }
                break;
            default:
                break;
        }
        actionSearch = false;
    }

    itemDeleteId = '';
    confirm = async function (e) {
        let grid = $("#airlineGrid").data("kendoGrid");
        if (e.data && e.data.value) {
            let model = {
                id: itemDeleteId
            }
            var resultValidate = await settingService.getInstance().airline.validateWhenFlightNumberUsed(model).$promise;
            if (resultValidate.object.count > 0) {
                Notification.error("This data has been used in Flight Number. You have to change Airline in Flight Number first.");
            }
            else {
                var result = await settingService.getInstance().airline.deleteAirline(model).$promise
                if (result.isSuccess) {
                    Notification.success("Data Sucessfully Deleted");
                    page = grid.pager.dataSource._page;
                    pageSize = grid.pager.dataSource._pageSize;
                    grid.dataSource.fetch(() => grid.dataSource.page(page), grid.dataSource.take(pageSize));
                }
            }
        }
    }

    function validateEdit(data) {
        var result = false;
        data.forEach(item => {
            if (item.select === true && item.id !== '') {
                result = true;
                return result;
            }
        });
        return result
    }

    async function getAirlines(option) {
        let currentQuery = {
            predicate: "",
            predicateParameters: [],
            order: "Modified desc",
            limit: appSetting.pageSizeDefault,
            page: 1
        };

        if (option) {
            currentQuery.limit = option.data.take;
            currentQuery.page = option.data.page;
        }

        if (actionSearch) {
            if ($scope.keyword) {
                currentQuery.predicate = 'Name.contains(@0) or Code.contains(@0)';
                currentQuery.predicateParameters.push($scope.keyword);
            }
        }
        else {
            currentQuery.predicate = 'Name.contains(@0) or Code.contains(@0)';
            currentQuery.predicateParameters.push(keyWordOld);
        }

        var result = await settingService.getInstance().airline.getAirlines(currentQuery).$promise;
        if (result.isSuccess) {
            $scope.airlines = result.object.data;
            var count = ((currentQuery.page - 1) * currentQuery.limit) + 1;
            $scope.airlines.forEach(x => {
                x["select"] = false;
                x["no"] = count;
                count = count + 1;
            });
            let value = {
                id: '',
                code: "",
                name: "",
                select: true,
            };
            $scope.airlines.push(value);
            $scope.total = result.object.count;
            option.success($scope.dataItemList);
        }
    }

    actionSearch = false;
    $scope.search = function () {
        actionSearch = true;
        keyWordOld = $scope.keyword;
        loadPageOne();
    }

    $scope.ifEnter = async function ($event) {
        var keyCode = $event.which || $event.keyCode;
        if (keyCode === 13) {
            actionSearch = true;
            keyWordOld = $scope.keyword;
            loadPageOne();
        }
    }

    function loadPageOne() {
        let grid = $("#airlineGrid").data("kendoGrid");
        grid.dataSource.fetch(() => grid.dataSource.page(1));
    }

    $scope.export = async function () {
        let model = {
            predicate: '',
            predicateParameters: [],
            order: "Modified desc",
        }

        if ($scope.keyword) {
            model.predicate = '(Code.contains(@0) or Name.contains(@0))';
            model.predicateParameters.push($scope.keyword);
        }

        var res = await fileService.getInstance().processingFiles.export({
            type: commonData.exportType.AIRLINESETTING
        }, model).$promise;
        if (res.isSuccess) {
            exportToExcelFile(res.object);
        }
    }
})