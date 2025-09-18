var ssgApp = angular.module('ssg.cAndBReasonModule', ["kendo.directives"]);
ssgApp.controller('cAndBReasonController', function ($rootScope, $scope, $location, appSetting, $stateParams, $state, moment, commonData, Notification, settingService, $q, fileService) {
    // create a message to display in our view
    var ssg = this;
    $scope.value = 0;
    var currentAction = null;
    isItem = true;
    $scope.title = $stateParams.action.title;
    ActionTypes = {
        ADD: 'ADD',
        DELETE: 'DELETE',
        UPDATE: 'UPDATE'
    };

    TYPE_MASTERDATA = 'MISSING_TIMECLOCK_REASON_TYPE';

    function initController() {
        if ($stateParams && $stateParams.type) {
            TYPE_MASTERDATA = $stateParams.type;
        }
    }

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
        Predicate: "",
        PredicateParameters: [],
        Order: "Modified desc",
        Limit: appSetting.pageSizeDefault,
        Page: 1
    };

    initController();

    // phần code dành cho myrequest và all request
    $scope.data = [];
    //Search
    $scope.searchInfo = {
        keyword: ''
    };

    $scope.idEditing = 0;
    $scope.errors = [];

    let requiredFields = [
        {
            fieldName: 'code',
            title: "Code"
        },
        {
            fieldName: 'name',
            title: "Name"
        }
    ];

    $scope.allOrMyRequestGridOptions = {
        dataSource: {
            pageSize: appSetting.pageSizeDefault,
            serverPaging: true,
            transport: {
                read: async function (e) {
                    await getList(e);
                }
            },
            schema: {
                total: () => { return $scope.total },
                data: () => { return $scope.data }
            }
        },
        sortable: false,
        autoBind: true,
        valuePrimitive: false,
        pageable: {
            alwaysVisible: true,
            pageSizes: appSetting.pageSizesArray
        },
        columns: [{
            field: "no",
            title: 'No.',
            width: "50px",
        },
        {
            field: "code",
            title: 'Code',
            width: "70px",
            template: function (dataItem) {
                if (dataItem.canEdit || !dataItem.id) {
                    return `<input class="k-textbox w100" name="code" ng-model="dataItem.code" autocomplete="off"/>`;
                } else {
                    return `<span>{{dataItem.code}}</span>`
                }
            }
        },
        {
            field: "name",
            title: "Name",
            width: "200px",
            template: function (dataItem) {
                if (dataItem.canEdit || !dataItem.id) {
                    return `<input class="k-textbox w100" name="name" ng-model="dataItem.name" autocomplete="off"/>`;
                } else {
                    return `<span>{{dataItem.name}}</span>`
                }
            }
        },
        {
            title: "Actions",
            width: "50px",
            template: function (dataItem) {
                if (!dataItem.id) {
                    return `
                        <a class="btn btn-sm btn-primary" ng-click="actions.create(dataItem)"><i class="fa fa-plus right-5"></i>Create</a>
                        `
                }
                if (dataItem.canEdit) {
                    return `
                        <a class="btn btn-sm default green-stripe" ng-click="actions.save(dataItem)">Save</a>
                        <a class="btn btn-sm default" ng-click="actions.cancel(dataItem)">Cancel</a>
                        `
                } else {
                    return `
                        <a class="btn btn-sm default blue-stripe" ng-click="actions.edit(dataItem)">Edit</a>
                        <a class="btn btn-sm default red-stripe" ng-click="actions.deleteRecord(dataItem)">Delete</a>
                        `
                }
            }
        }
        ],
    };

    async function getList(option) {
        if ($scope.searchInfo.keyword) {
            $scope.currentQuery.Predicate = "Code.contains(@0) || Name.contains(@0)";
            $scope.currentQuery.PredicateParameters.push($scope.searchInfo.keyword);
        }
        else{
            $scope.currentQuery.Predicate = "";
            $scope.currentQuery.PredicateParameters = [];
        }

        if (option) {
            $scope.currentQuery.limit = option.data.take;
            $scope.currentQuery.page = option.data.page;
        }

        // let arg = {
        //     queryArgs: $scope.currentQuery,
        //     type: TYPE_MASTERDATA
        // };

        var result = await settingService.getInstance().cabs.getReasons({queryArgs: $scope.currentQuery,type: TYPE_MASTERDATA}).$promise;
        if (result.isSuccess) {
            $scope.data = result.object.data;
            var count = (($scope.currentQuery.page - 1) * $scope.currentQuery.limit) + 1;
            $scope.data.forEach(x => {
                x["canEdit"] = false;
                x["no"] = count;
                count = count + 1;
            });

            let nValue = {
                code: '',
                name: '',
                canEdit: false,
                id: ''
            };
            $scope.data.push(nValue);
            $scope.total = result.object.count;

            if (option) {
                option.success($scope.data);
            }
            else {
                let grid = $("#allOrMyRequestGrid").data("kendoGrid");
                grid.dataSource.read();
                grid.dataSource.page($scope.currentQuery.page);
            }
        }
    }

    function validationRequired(model, rqFields, message) {
        let errors = [];
        rqFields.forEach(field => {
            if (!model[field.fieldName]) {
                let nMessage = message;
                if (model['no']) {
                    nMessage = nMessage.replace('[index]', model['no']);
                }
                nMessage = nMessage.replace('[field]', field.title);
                errors.push({
                    controlName: nMessage
                });
            }
        });
        return errors;
    }

    function Validation(model) {
        let errors = validationRequired(model, requiredFields, '[field] : Field is required');
        return errors;
    }

    function showNotification(messages) {
        let messageShow = messages.map(x => x.controlName).join('</br>');
        Notification.error(messageShow);
    }

    async function save(model) {
        $scope.errors = Validation(model);
        if ($scope.errors.length) {
            showNotification($scope.errors);
            return false;
        } else {
            let result = {
                isSuccess: false
            };
            let messageNotification = '';
            if (model.id) {
                result = await settingService.getInstance().cabs.updateReason(model).$promise;
                messageNotification = 'Data Successfully Saved';
            } else {
                model.type = TYPE_MASTERDATA;
                result = await settingService.getInstance().cabs.addReason(model).$promise;
                messageNotification = 'Data Successfully Created';
            }

            if (result.isSuccess) {
                Notification.success(messageNotification);
                refreshGrid('#allOrMyRequestGrid', true, model.id ? ActionTypes.UPDATE : ActionTypes.ADD);
                return true;
            } else {
                if (result.messages.length) {
                    Notification.error(result.messages[0]);
                } else {
                    Notification.error("Error");
                }
                return false;
            }
        }
    }

    function cancel(model) {
        model['canEdit'] = false;
        refreshGrid('#allOrMyRequestGrid', true);
    }

    function hasBeenEditing(idGrid, message) {
        let grid = $(idGrid).data("kendoGrid");
        let isEditing = grid.pager.dataSource._data.some(item => item['canEdit']);

        if (isEditing) {
            Notification.error(message);
        }
        return isEditing;
    }

    function canDoActionUpdateOrDelete() {
        // Kiểm tra xem có chỗ nào đang chỉnh sửa hay không, nếu có thì show message cảnh báo
        return !hasBeenEditing('#allOrMyRequestGrid', 'Please save selected item before edit/delete other item');
    }

    function edit(model) {
        // Kiểm tra xem có chỗ nào đang chỉnh sửa hay không, nếu có thì show message cảnh báo
        let canDoAction = canDoActionUpdateOrDelete();
        if (canDoAction) {
            model['canEdit'] = true;
            refreshGrid('#allOrMyRequestGrid');
        }
    }

    function deleteRecord(model) {
        // Kiểm tra xem có chỗ nào đang chỉnh sửa hay không, nếu có thì show message cảnh báo
        let canDoAction = canDoActionUpdateOrDelete();
        if (canDoAction) {
            $scope.dataDelete = model;
            $scope.dialog = $rootScope.showConfirmDelete("DELETE", commonData.confirmContents.remove, 'Confirm');
            $scope.dialog.bind("close", confirm);
        }
    }

    confirm = function (e) {
        let grid = $("#allOrMyRequestGrid").data("kendoGrid");
        if (e.data && e.data.value) {
            settingService.getInstance().cabs.deleteReason({ Id: $scope.dataDelete.id }).$promise.then(function (result) {
                if (result.isSuccess) {
                    Notification.success("Data Sucessfully Deleted");
                    getList();
                    grid.refresh();
                }
                else {
                    Notification.error(result.messages[0]);
                }
            });
        }
    }

    function create(model) {
        let isEditing = hasBeenEditing('#allOrMyRequestGrid', 'Please save selected item before create other item');
        if (!isEditing) {
            save(model);
        }
    }

    function refreshGrid(idGrid, isCallApi = false, action = '') {
        let grid = $(idGrid).data("kendoGrid");
        if (isCallApi) {

            // Thứ tự switch case quan trọng, đừng có đổi vị trí nha
            switch (action) {
                case ActionTypes.ADD:
                    loadPageOne($scope.searchInfo.keyword = '');
                    //getList();
                    break;
                case ActionTypes.DELETE:
                    // Nếu hành động xóa và trang đó không phải là trạng 1, thì nếu nó delete dòng cuối cùng thì phải trả dữ liệu của pageIndex kế trước đó 
                    // còn nếu không phải nó sẽ nhảy vào default lấy dữ liệu bình thường
                    if (grid.pager.dataSource._page != 1 && grid.pager.dataSource._data.length === 2) {
                        getList();
                        break;
                    }
                default:
                    getList();
                    break;
            }

        }
        grid.refresh();
    }

    function ifEnter($event) {
        var keyCode = $event.which || $event.keyCode;
        if (keyCode === 13) {
            loadPageOne();
        }
    }

    $scope.search = function () {
        $scope.currentQuery = {
            Predicate: "",
            PredicateParameters: [],
            Order: "Created desc",
            Limit: appSetting.pageSizeDefault,
            Page: 1
        };
        //getList();
        loadPageOne();
    }

    $scope.export = async function () {
        let option = $scope.currentQuery;

        if(option.Predicate){
            option.Predicate += ' && MetadataType.Value = @0';
        }
        else{
            option.Predicate += 'MetadataType.Value = @0';
        }
            
        var Type = 0;
        switch ($stateParams.type) {
            case commonData.reasonType.MISSING_TIMECLOCK_REASON:
                option.PredicateParameters.push(commonData.reasonType.MISSING_TIMECLOCK_REASON);
                Type = commonData.exportType.MISSINGTIMECLOCKREASON;
                break;
            case commonData.reasonType.OVERTIME_REASON:
                option.PredicateParameters.push(commonData.reasonType.OVERTIME_REASON);
                Type = commonData.exportType.OVERTIMEREASON;
                break;
            case commonData.reasonType.SHIFT_EXCHANGE_REASON:
                option.PredicateParameters.push(commonData.reasonType.SHIFT_EXCHANGE_REASON);
                Type = commonData.exportType.SHIFTEXCHANGEREASON;
                break;
            case commonData.reasonType.RESIGNATION_REASON:
                option.PredicateParameters.push(commonData.reasonType.RESIGNATION_REASON);
                Type = commonData.exportType.RESIGNATIONREASON;
                break;
            default:
                break;
        }
        
        var res = await fileService.getInstance().processingFiles.export({ type: Type }, option).$promise;
        if (res.isSuccess) {
            exportToExcelFile(res.object);
            Notification.success(appSetting.notificationExport.success);
        } else {
            Notification.error(appSetting.notificationExport.error);
        }
    }

    $scope.import = async function () {

    }

    $scope.$on('$locationChangeStart', function (event, next, current) {
        $scope.errors = [];
    });

    // thêm vào 1 dòng dữ liệu rỗng để có thể hiện cuối danh sách
    // hàm này sẽ chạy sau khi view được render lên
    $scope.$on('$viewContentLoaded', function () {
        
    });

    function loadPageOne() {
        let grid = $("#allOrMyRequestGrid").data("kendoGrid");
        grid.dataSource.fetch(() => grid.dataSource.page(1));
    }

});