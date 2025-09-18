var ssgApp = angular.module("ssg.settingRecruitmentPositionModule", [
    "kendo.directives"
]);
ssgApp.controller("settingRecruitmentPositionController", function (
    $rootScope,
    $scope,
    $location,
    $stateParams,
    appSetting,
    commonData,
    Notification,
    settingService,
    fileService,
    $timeout
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
    $scope.keyword = '';
    $scope.dataPosition = [];

    function buildArgs(pageIndex, pageSize) {
        let predicate = ['MetadataType.Value = @0'];
        let predicateParameters = ['Position'];
        if ($scope.keyword) {
            predicate.push('(Name.contains(@1) or Code.contains(@1))');
            predicateParameters.push($scope.keyword);  
        }
        var option = QueryArgs(predicate, predicateParameters, appSetting.ORDER_GRID_DEFAULT, pageIndex, pageSize);
        return option;
    }

    async function getPositions(option) {
        let args = buildArgs($scope.currentQuery.Page, appSetting.pageSizeDefault);
        if (option) {
            $scope.currentQuery.limit = option.data.take;
            $scope.currentQuery.page = option.data.page;
        }

        $scope.currentQuery.predicate = args.predicate;
        $scope.currentQuery.predicateParameters = args.predicateParameters;
        var result = await settingService.getInstance().recruitment.getPositionLists($scope.currentQuery).$promise;
        if (result.isSuccess) {
            $scope.dataPosition = result.object.data;
            var count = (($scope.currentQuery.page - 1) * $scope.currentQuery.limit) + 1;
            $scope.dataPosition.forEach(x => {
                x["select"] = false;
                x["no"] = count;
                count = count + 1;
            });
            let nValue = {
                id: '',
                code: "",
                name: "",
                select: true,
            };
            $scope.dataPosition.push(nValue);
            $scope.total = result.object.count;
            if(option) {
                option.success($scope.dataPosition);
            }
            else {
                let grid = $("#gridPosition").data("kendoGrid");
                grid.dataSource.read();
                grid.dataSource.page($scope.currentQuery.page);
            }
        }
    }

    async function findPositiontByCode(code, id) {
        let queryArgs = {
            predicate: '',
            predicateParameters: [],
            order: appSetting.ORDER_GRID_DEFAULT,
            page: 0,
            limit: appSetting.pageSizeDefault
        };

        queryArgs.predicateParameters.push(code);
        queryArgs.predicateParameters.push("Position");
        queryArgs.predicateParameters.push(id);
        var result = await settingService.getInstance().recruitment.getPositionListRecruimentByCode(queryArgs).$promise;
        if (result.object.count >= 1) {
            return true;
        }
        return false;
    }
    //
    $scope.total = 0;
    $scope.positionOptions = {
        dataSource: {
            serverPaging: true,
            pageSize: 20,
            transport: {
                read: async function (e) {
                    await getPositions(e);
                }
            },
            schema: {
                total: () => { return $scope.total },
                data: () => { return $scope.dataPosition }
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
        columns: [{
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
            field: "jobGradeId",
            title: "Job Grade",
            width: "200px",
            template: function (dataItem) {
                if (dataItem.select) {
                    return `<select kendo-drop-down-list style="width: 100%;"
                            id="jobGradeId"
                            name="jobGradeId"
                            k-ng-model="dataItem.jobGradeId"       
                            k-data-source="jobGradeDataSource"        
                            k-options="jobGradeOptions"
                            ></select>`;
                } else {
                    return `<label>${dataItem.jobGradeCaption}</label>`
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
                var checkCode = await findPositiontByCode(model.code);
                if (checkCode) {
                    Notification.error("Code " + model.code + " has existed in Position List");
                }
                else {
                    var result = await settingService.getInstance().recruitment.addPositionList({ Id: model.id, Code: model.code, Name: model.name, JobGradeId: model.jobGradeId }).$promise;
                    if (result.isSuccess) {
                        Notification.success("Data Successfully Saved");
                        loadPageOne($scope.keyword = '');
                        return true;
                    }
                }
            }
        }
    };

    function checkEdit() {
        var result = false;
        $scope.dataPosition.forEach(item => {
            if (item.select === true && item.id !== '') {
                result = true;
                return result;
            }
        });
        return result
    }

    $scope.errors = [];

    let requiredFields = [{
        fieldName: "code",
        title: "Code"
    },
    {
        fieldName: "name",
        title: "Name"
    },
    {
        fieldName: "jobGradeId",
        title: "job Grade"
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
            var checkCode = await findPositiontByCode(model.code, model.id);
            if (checkCode) {
                Notification.error("Code " + model.code + " has existed in Position List");
            }
            else {
                settingService.getInstance().recruitment.editPositionList({ Id: model.id, Code: model.code, Name: model.name, JobGradeId: model.jobGradeId }).$promise.then(function (result) {
                    if (result.isSuccess) {
                        Notification.success("Data Successfully Saved");
                        getPositions();
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
        let grid = $("#gridPosition").data("kendoGrid");
        grid.dataSource.fetch(() => grid.dataSource.page(1));
    }


    function cancel(model) {
        model['select'] = false;
        var item = $scope.dataPosition.find(x => x.id === model.id);
        item.select = false;
        model.code = item.code;
        model.name = item.name;
        let grid = $("#gridPosition").data("kendoGrid");
        grid.refresh();
    }

    function edit(model) {
        var result = validateEdit();
        if (result) {
            Notification.error("Please save selected item before edit/delete other item");
        } else {
            model['select'] = true
            var item = $scope.dataPosition.find(x => x.id === model.id);
            item.select = true;
            let grid = $("#gridPosition").data("kendoGrid");
            grid.refresh();
        }

    }

    function validateEdit() {
        var result = false;
        $scope.dataPosition.forEach(item => {
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
        } else {
            $scope.dataDelete = model;
            $scope.dialog = $rootScope.showConfirmDelete("DELETE", commonData.confirmContents.remove, 'Confirm');
            $scope.dialog.bind("close", confirm);
        }
    }

    confirm = function (e) {
        let grid = $("#gridPosition").data("kendoGrid");
        if (e.data && e.data.value) {
            settingService.getInstance().recruitment.deletePositionList({ Id: $scope.dataDelete.id }).$promise.then(function (result) {
                if (result.isSuccess) {
                    Notification.success("Data Sucessfully Deleted");
                    getPositions();
                    grid.refresh();
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

        model.predicate = 'MetadataType.Value = @0';
        model.predicateParameters.push('Position')

        if ($scope.keyword) {
            model.predicate = model.predicate + ' and ' + `(Name.contains(@${model.predicateParameters.length}) or Code.contains(@${model.predicateParameters.length}))`;
            model.predicateParameters.push($scope.keyword);
        }

        var res = await fileService.getInstance().processingFiles.export({
            type: 25
        }, model).$promise;
        if (res.isSuccess) {
            exportToExcelFile(res.object);
        }
    }

    $scope.jobGradeOptions = {
        dataTextField: "caption",
        dataValueField: "id",
        valuePrimitive: true,
        checkboxes: false,
        autoBind: true,
        filter: "contains",
        dataSource: $scope.jobGradeDataSource
    };

    $scope.jobGradeDataSource = [];
    async function getJobGrade() {
        let model = {
            Predicate: "",
            PredicateParameters: [],
            Order: "Grade asc",
            Limit: appSetting.pageSizeDefault,
            Page: 1
        }

        let result = await settingService.getInstance().jobgrade.getJobGradeList(model).$promise;
        if (result.isSuccess) {
            $scope.jobGradeDataSource = result.object.data;
        }
    }


    $scope.import = async function () {

    }

    async function ngInit() {
        //await getPositions(1);
        await getJobGrade();
    }

    ngInit();

});