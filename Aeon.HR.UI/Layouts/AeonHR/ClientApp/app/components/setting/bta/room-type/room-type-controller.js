var ssgApp = angular.module('ssg.roomTypeModule', ['kendo.directives']);
ssgApp.controller('roomTypeController', function ($rootScope, $scope, $location, appSetting, Notification, commonData, $stateParams, settingService, fileService, $timeout) {
    var ssg = this;
    $scope.title = 'Room Type';

    // phần khai báo chung
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
            fieldName: "quota",
            title: "Quota"
        }
    ];

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

    // Get list
    $scope.roomTypeOptions = {
        dataSource: {
            serverPaging: true,
            pageSize: 20,
            transport: {
                read: async function (e) {
                    await getListRoomTypes(e);
                }
            },
            schema: {
                total: () => { return $scope.total },
                data: () => { return $scope.dataRoomType }
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
                width: "20px",
                template: function (dataItem) {
                    return `<span>{{dataItem.no}}</span>`;
                }
            },
            {
                field: "code",
                title: "Code",
                width: "100px",
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
                width: "300px",
                template: function (dataItem) {
                    if (dataItem.select) {
                        return `<input class="k-textbox w100" autoComplete="off" name="name" ng-model="dataItem.name"/>`;
                    } else {
                        return `<span>{{dataItem.name}}</span>`;
                    }
                }
            },
            {
                field: "quota",
                title: "Quota",
                width: "300px",
                template: function (dataItem) {
                    if (dataItem.select) {
                        return `<input kendo-numeric-text-box k-min="0" class="w100" name="quota" k-format="'#'" ng-model="dataItem.quota" restrict-input="{type: 'digitsOnly'}"/>`;
                    } else {
                        return `<span>{{dataItem.quota}}</span>`;
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

    //API
    async function getListRoomTypes(option) {
        let args = buildArgs($scope.currentQuery.page, appSetting.pageSizeDefault);
        if (option) {
            $scope.currentQuery.limit = option.data.take;
            $scope.currentQuery.page = option.data.page;
        }
        $scope.currentQuery.predicate = args.predicate;
        $scope.currentQuery.predicateParameters = args.predicateParameters;
        var result = await settingService.getInstance().roomType.getListRoomTypes($scope.currentQuery).$promise;
        if (result.isSuccess) {
            $scope.dataRoomType = result.object.data;
            var count = (($scope.currentQuery.page - 1) * $scope.currentQuery.limit) + 1;
            $scope.dataRoomType.forEach(x => {
                x["select"] = false;
                x["no"] = count;
                count = count + 1;
            });
            let nValue = {
                id: '',
                code: "",
                name: "",
                quota: "",
                select: true,
            };
            $scope.dataRoomType.push(nValue);
            $scope.total = result.object.count;
            if (option) {
                option.success($scope.dataRoomType);
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
        // if ($scope.keyword) {
        //     predicate.push('(Code.contains(@0) or Name.contains(@0))');
        //     predicateParameters.push($scope.keyword);  
        // }

        if(actionSearch) {
            if ($scope.keyword) {
                predicate.push('(Code.contains(@0) or Name.contains(@0))');
                predicateParameters.push($scope.keyword);
            }
        }
        else {
            predicate.push('(Code.contains(@0) or Name.contains(@0))');
            predicateParameters.push(keyWordOld);  
        }

        var option = QueryArgs(predicate, predicateParameters, appSetting.ORDER_GRID_DEFAULT, pageIndex, pageSize);
        return option;
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
        else if(errors.length == 0){
            var checkCode = await findRoomTypeByCode(model.code, model.id);
            if (checkCode) {
                Notification.error("Code " + model.code + " has existed in Room Type");
            }
            else {
                settingService.getInstance().roomType.updateRoomType({ Id: model.id, Code: model.code, Name: model.name, Quota: model.quota}).$promise.then(function (result) {
                    if (result.isSuccess) {
                        Notification.success("Data Successfully Saved");
                        getListRoomTypes();
                        return true;
                    }
                });
            }
        }
    }

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
                var checkCode = await findRoomTypeByCode(model.code);
                if (checkCode) {
                    Notification.error("Code " + model.code + " has existed in Room Type");
                }
                else{
                    settingService.getInstance().roomType.createRoomType({ Id: model.id, Code: model.code, Name: model.name, Quota: model.quota}).$promise.then(function (result) {
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

    async function findRoomTypeByCode(code, id) {
        let queryArgs = {
            predicate: '',
            predicateParameters: [],
            order: appSetting.ORDER_GRID_DEFAULT,
            page: 0,
            limit: appSetting.pageSizeDefault
        };

        queryArgs.predicateParameters.push(code);
        queryArgs.predicateParameters.push(id);
        var result = await settingService.getInstance().roomType.getRoomTypeByCode(queryArgs).$promise;
        if (result.object.count >= 1) {
            return true;
        }
        return false;
    }

    function checkEdit() {
        var result = false;
        $scope.dataRoomType.forEach(item => {
            if (item.select === true && item.id !== '') {
                result = true;
                return result;
            }
        });
        return result
    }

    function cancel(model) {
        model['select'] = false;
        var item = $scope.dataRoomType.find(x => x.id === model.id);
        item.select = false;
        model.code = item.code;
        model.name = item.name;
        model.quota = item.quota;
        let grid = $("#grid").data("kendoGrid");
        grid.refresh();
    }

    function edit(model) {
        var result = validateEdit();
        if (result) {
            Notification.error("Please save selected item before edit/delete other item");
        }
        else {
            actionSearch = false;
            model['select'] = true;
            var item = $scope.dataRoomType.find(x => x.id === model.id);
            item.select = true;
            let grid = $("#grid").data("kendoGrid");
            grid.refresh();
        }

    }

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
        let grid = $("#grid").data("kendoGrid");
        if (e.data && e.data.value) {
            settingService.getInstance().roomType.deleteRoomType({ Id: $scope.dataDelete.id }).$promise.then(function (result) {
                if (result.isSuccess) {
                    actionSearch = false;
                    Notification.success("Data Sucessfully Deleted");
                    getListRoomTypes();
                    grid.refresh();
                }
                else {
                    Notification.error(result.messages[0]);
                }
            });
        }
    }

    function validateEdit() {
        var result = false;
        $scope.dataRoomType.forEach(item => {
            if (item.select === true && item.id !== '') {
                result = true;
                return result;
            }
        });
        return result
    }

    actionSearch = false;
    keyWordOld = '';
    async function ifEnter($event) {
        var keyCode = $event.which || $event.keyCode;
        if (keyCode === 13) {
            actionSearch = true;
            keyWordOld = $scope.keyword;
            loadPageOne();
        }
    }

    $scope.search = async function () {
        actionSearch = true;
        keyWordOld = $scope.keyword;
        loadPageOne();
    }

    function loadPageOne() {
        let grid = $("#grid").data("kendoGrid");
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
            type: commonData.exportType.ROOMTYPESETTING
        }, model).$promise;
        if (res.isSuccess) {
            exportToExcelFile(res.object);
        }
    }
});