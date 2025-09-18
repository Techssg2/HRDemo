var ssgApp = angular.module('ssg.jobGradeModule', ["kendo.directives"]);
ssgApp.controller('jobGradeController', function ($rootScope, $scope, $location, appSetting, $stateParams, $state, moment, commonData, Notification, $sce, settingService, $timeout, fileService) {
    // create a message to display in our view
    var ssg = this;
    $scope.value = 0;
    var currentAction = null;
    isItem = true;
    $scope.title = 'Job Grade';

    // phần code dành cho myrequest và all request
    //Status Data
    $scope.statusOptions = {
        placeholder: "",
        dataTextField: "statusName",
        valuePrimitive: true,
        autoBind: false,
        dataSource: {
            serverFiltering: true,
            data: [{
                statusName: "Completed"
            },
            {
                statusName: "Waiting for CMD Checker Approval"
            },
            {
                statusName: "Waiting for HOD Approval"
            },
            {
                statusName: "Cancelled"
            }
            ]
        }
    };

    $scope.data = [];
    //Search
    $scope.searchInfo = {
        keyword: ''
    };

    $scope.titleModal = 'Title modal';
    $scope.jobGradeIdNeedAddItem = null;

    $scope.allOrMyRequestGridOptions = {
        dataSource: {
            data: $scope.data,
            pageSize: 20,
            schema: {
                model: {
                    id: "no"
                }
            },
        },
        sortable: false,
        // pageable: true,
        pageable: {
            alwaysVisible: true,
            pageSizes: appSetting.pageSizesArray
        },
        columns: [{
            field: "no",
            title: "No.",
            width: "50px"
        },
        {
            field: "grade",
            title: 'Grade',
            width: "80px"
            // template: function(dataItem) {
            //     if (dataItem.canEdit || !dataItem['id']) {
            //         return `<input required name="grade" kendo-numeric-text-box k-min="0" class="w100" k-format="'#,0'" ng-model="dataItem.grade"/>`
            //     } else {
            //         return `<span>{{dataItem.grade}}</span>`
            //     }
            // }
        },
        {
            field: "caption",
            title: "Caption",
            width: "150px",
            template: function (dataItem) {
                if ((dataItem.canEdit || !dataItem['id']) && dataItem.grade > 9) {
                    return `<input class="k-textbox w100" name="caption" ng-model="dataItem.caption"/>`;
                } else {
                    return `<span>{{dataItem.caption}}</span>`
                }
            }
        },
        {
            field: "title",
            title: "Title",
            width: "200px",
            template: function (dataItem) {
                if (dataItem.canEdit || !dataItem['id']) {
                    return `<input class="k-textbox w100" name="title" ng-model="dataItem.title"/>`;
                } else {
                    return `<span>{{dataItem.title}}</span>`
                }
            }
        },
        {
            field: "expiredDayPosition",
            title: "Expired Day For Positions",
            width: "200px",
            template: function (dataItem) {
                if (dataItem.canEdit || !dataItem['id']) {
                    return `<input required name="expiredDayPosition" kendo-numeric-text-box k-min="0" class="w100" k-format="'#,0'" ng-model="dataItem.expiredDayPosition"/>`
                } else {
                    return `<span>{{dataItem.expiredDayPosition}}</span>`
                }
            }
        },
        {
            title: "Actions",
            width: "250px",
            template: function (dataItem) {
                if (!dataItem['no']) {
                    return `
                        <a class="btn btn-sm btn-primary" ng-click="actions.createJobGrade(dataItem)"><i class="fa fa-plus right-5"></i>Create</a>
                        `
                }
                if (dataItem.canEdit) {
                    return `
                        <a class="btn btn-sm default green-stripe" ng-click="actions.saveJobGrade(dataItem)">Save</a>
                        <a class="btn btn-sm default" ng-click="actions.cancelEditJobGrade(dataItem)">Cancel</a>
                        `
                } else {
                    return `
                        <a class="btn btn-sm default blue-stripe" ng-click="actions.editJobGrade(dataItem)">Edit</a>
                        <a class="btn btn-sm default red-stripe" ng-if="dataItem.grade > 9" ng-click="actions.deleteJobGrade(dataItem)">Delete</a>
                        <a class="btn btn-sm default blue-stripe" ng-click="actions.addItemsJobGrade(dataItem)">Default Asset Items</a>
                        `
                }
            }
        }
        ],
        editable: "inline"
    };

    $scope.dialogDetailOption = {
        buttonLayout: "normal",
        animation: {
            open: {
                effects: "fade:in"
            }
        },
        schema: {
            model: {
                id: "no"
            }
        },
        actions: [{
            text: "Save",
            action: function (e) {
                // e.sender is a reference to the dialog widget object
                // OK action was clicked
                // Returning false will prevent the closing of the dialog
                saveItems('#itemsOnGrid');
                return false;
            },
            primary: true
        }]
    };
    // phần dành cho trang add/edit
    // các biến sẽ khai báo ở đây
    $scope.idEditing = 0;
    $scope.errors = [];
    $scope.errorsTwo = {};

    let requiredFieldsOfTableItems = [{
        fieldName: 'itemCode',
        title: "Code"
    },
    {
        fieldName: 'itemName',
        title: "Item"
    }
    ];

    let requiredFieldsOfJobGrade = [{
        fieldName: 'grade',
        title: "Grade"
    },
    {
        fieldName: 'caption',
        title: "Caption"
    },
    {
        fieldName: 'title',
        title: "Title"
    }
    ];

    // xử lý add more
    function addItem() {
        let grid = $("#itemsOnGrid").data("kendoGrid");
        let nValue = {
            no: grid.dataSource._data.length + 1,
            itemCode: '',
            itemName: ''
        };
        grid.dataSource.add(nValue);
    }

    function cancelEditJobGrade(model) {
        model['canEdit'] = false;
        refreshGrid('#allOrMyRequestGrid', true);
    }

    async function updateJobGrade(model) {

        return result;
    }

    async function editJobGrade(model) {
        var result = validateEdit();
        if (result) {
            Notification.error("Please save selected item before edit/delete other item");
        } else {
            model['canEdit'] = true;
            refreshGrid('#allOrMyRequestGrid');
        }
    }

    function validateEdit() {
        var result = false;
        let grid = $(`#allOrMyRequestGrid`).data("kendoGrid");
        let dataGrid = grid.dataSource._data;
        dataGrid.forEach(item => {
            if (item.canEdit === true && item.id !== '') {
                result = true;
                return result;
            }
        });
        return result
    }

    async function deleteJobGrade(model) {
        // call api ở đây
        let result = await settingService.getInstance().jobgrade.deleteJobGrade({
            id: model.id
        }).$promise;
        // thành công thì hiện ra thông báo và call api để refresh lại lưới
        if (result.isSuccess) {
            $timeout(function() {
                $scope.dialog = $rootScope.showConfirmDelete("DELETE", commonData.confirmContents.remove, 'Confirm');
                $scope.dialog.bind("close", confirmRemove);
            }, 0);
        }
    }

    confirmRemove = function (e) {
        if (e.data && e.data.value) {
            Notification.success("Data Successfully Deleted");
            refreshGrid('#allOrMyRequestGrid', true);
            // id = id của kendo grid = itemsOnGrid
            let grid = $(`#itemsOnGrid`).data("kendoGrid");
            let currentLength = grid.dataSource._data.length;
            grid.dataSource._data.splice($scope.no - 1, 1);
            if ($scope.no < currentLength) {
                grid.dataSource._data.forEach(item => {
                    if (item.no > $scope.no) item.no--;
                });
            }
        }
    }

    async function createJobGrade(model) {
        var resultEdit = validateEdit();
        if (resultEdit) {
            Notification.error("Please save selected item before create other item");
        } else {
            let result = await saveJobGrade(model);
            if (result) {
                refreshGrid('#allOrMyRequestGrid', true);
            }
        }
    }

    async function excuteSettingService(nameProperty, model) {
        let res = await settingService.getInstance().jobgrade[nameProperty](model).$promise;
        return res;
    }

    async function getItemOfJobGrade(jobGradeId) {
        return await excuteSettingService('getItemRecruitmentsOfJobGrade', { jobGradeId });
    }

    async function getAllItemRecruitments() {
        return await excuteSettingService('getAllItemRecruitments', {});
    }

    async function addOrUpdateItemsOfJobGrade(model) {
        return await excuteSettingService('addOrUpdateItemsOfJobGrade', model);
    }

    function generateNumberOrder(items) {
        if (items && items.length) {
            let startNum = 1;
            items.forEach(i => i['no'] = startNum++);
        }       
        return new kendo.data.DataSource({
            data: items,
            pageSize: 20,
            page: 1,
            schema: {
                total: function(){
                    return items.length;
                }
            }
        }) 
    }

    // xử lý add item
    async function addItemsJobGrade(model) {
        $scope.jobGradeIdNeedAddItem = model.id;
        // call api lấy thông tin về jobgrade
        const itemOfJobGradeResult = await getItemOfJobGrade(model.id);

        // lấy list item để đổ vào dropdown
        const itemRecruitmentsResult = await getAllItemRecruitments();
        itemRecruitmentsResult.object.data.forEach(item => {
            item['showCode'] = item.code;
        });
        $scope.allItemOfJobGrade = itemRecruitmentsResult.object.data;
        $scope.itemsDatasource = $scope.allItemOfJobGrade;
       

        // set lại dataSource cho grid
        let grid = $("#itemsOnGrid").data("kendoGrid");
        grid.setDataSource(generateNumberOrder(itemOfJobGradeResult.object.data));


        // set title cho cái dialog
        let dialog = $("#dialogDetail").data("kendoDialog");
        dialog.title(`Add Items For Grade ${model.grade}`);
        dialog.open();
        $rootScope.defaultAssetDialog = dialog;

        // sửa cái css cái button và thêm icon
        let result = document.getElementsByClassName("k-dialog-buttongroup k-dialog-button-layout-normal");
        let wrappedResult = angular.element(result);
        wrappedResult
            .find(".k-button.k-primary")
            .removeClass("k-primary")
            .removeClass("k-button")
            .addClass("btn btn-sm default")
            .prepend(`<i class="k-icon k-i-save font-green-jungle"></i> `);

        $scope.errorsTwo = {};
    }

    function refreshGrid(idGrid, isCallApi = false) {
        let grid = $(idGrid).data("kendoGrid");
        if (isCallApi) {
            getList(false, grid.pager.dataSource._page);
        }
        grid.refresh();
    }

    $scope.deleteRecord = function (no, id) {
        $scope.no = no;
        $scope.id = id;

        $scope.dialog = $rootScope.showConfirmDelete("CLOSE", commonData.confirmContents.remove, 'Confirm');
        $scope.dialog.bind("close", confirmDelete);
    }

    confirmDelete = function (e) {
        if (e.data && e.data.value) {
            Notification.success('Data Successfully Deleted');
            refreshGrid('#allOrMyRequestGrid', true);
            // id = id của kendo grid = itemsOnGrid
            let grid = $(`#itemsOnGrid`).data("kendoGrid");
            let currentLength = grid.dataSource._data.length;
            grid.dataSource._data.splice($scope.no - 1, 1);
            if ($scope.no < currentLength) {
                grid.dataSource._data.forEach(item => {
                    if (item.no > $scope.no) item.no--;
                });
            }
        }
    }

    // phần dành cho items
    $scope.itemsDatasource = [
        // {
        //     code: 'DELL',
        //     item: 'Laptop Dell Vostro A5370'
        // },
        // {
        //     code: 'Card',
        //     item: 'Card Over'
        // },
        // {
        //     code: 'ASUS',
        //     item: 'Laptop ASUS VivoBook 14 A412FA-EK377T'
        // },
        // {
        //     code: 'ASUS-01',
        //     item: 'Laptop ASUS ROG Strix G G531GT-AL007T'
        // },
        // {
        //     code: 'MSI',
        //     item: 'Laptop MSI GS65 Stealth 9SE 1000VN'
        // }
    ];

    // phần dùng chung
    function setDataSourceDropdown(idDropdown, items) {
        let dropdownlist = $(idDropdown).data("kendoDropDownList");
        dropdownlist.setDataSource(items);
    }

    $scope.getLengthObjectKeys = function (obj) {
        return Object.keys(obj).length;
    }

    $scope.renderHtml = function (htmlCode) {
        return $sce.trustAsHtml(htmlCode);
    };


    function itemDropDownEditor(container, options) {
        $scope.uidOfItemEditing = options.model.uid;
        $(`<input required name="${options.field}" id="${options.field + options.model.uid}"/>`)
            .appendTo(container)
            .kendoDropDownList({
                autoBind: true,
                dataTextField: "code",
                dataValueField: "code",
                dataSource: {
                    data: $scope.itemsDatasource,
                },
                filter: "contains",
                // filtering: function (e) {
                //     let filter = e.filter;
                //     if (!filter.value) {
                //         //prevent filtering if the filter does not value
                //         e.preventDefault();
                //     } else {
                //         // call api lấy danh sách data
                //         let items = [{
                //             code: 'DELL',
                //             item: 'Laptop Dell Vostro A5370'
                //         }];

                //         $scope.itemsDatasource = items;
                //         //--DELETE-CODE thông báo để cho biết là chỗ này đang chạy, lúc ra sản phẩm thì xoá đi 
                //         Notification.info(`Đang call api lấy danh sách của reportToDropdown với keyword là: "${filter.value}"`);
                //         // set lại dataSource cho dropdown 
                //         setDataSourceDropdown(`#${options.field + options.model.uid}`, items);
                //     }
                // }
            });
    }

    $scope.itemsEditListGridOptions = {
        dataSource: {
            data: [],
            pageSize: 5,
        },
        sortable: false,
        scrollable: false,
        pageable: {
            alwaysVisible: true,
            responsive: false
        },
        //editable: true,
        columns: [{
            field: "no",
            title: "No",
            width: "50px",
            editable: function (dataItem) {
                return false;
            }
        },
        {
            field: "itemCode",
            title: "Code",
            width: "200px",
            //editor: itemDropDownEditor,
            template: function(dataItem) {
                return `<select kendo-drop-down-list style="width: 100%;" id="typeActualTimeId"
                data-k-ng-model="dataItem.itemCode"
                k-data-text-field="'showCode'"
                k-data-value-field="'code'"
                k-template="'<span><label>#: data.showCode# </label></span>'",
                k-valueTemplate="'#: value #'",
                k-auto-bind="'true'"
                k-value-primitive="'false'"
                k-data-source="itemsDatasource"
                k-on-change="onchangeItem(kendoEvent, dataItem)"
                > </select>`;
            }
        },
        {
            field: "itemName",
            title: "Item",
            width: "200px",
            // editable: function (dataItem) {
            //     return false;
            // }
            template: function(dataItem) {
                return `<input name="others" class="k-input" type="text" ng-model="dataItem.itemName" style="width: 100%" readonly />`;
            }
        },
        {
            title: "Actions",
            width: "80px",
            template: function (dataItem) {
                return `<a class="btn btn-sm default red-stripe" ng-click="deleteRecord(dataItem.no, 'itemsOnGrid')">Delete</a>`
            }
        }
        ],
        // save: function (e) {
        //     if (e.values['itemCode']) {
        //         let item = $scope.itemsDatasource.find(x => x.code === e.values['itemCode']);
        //         if (item) {
        //             e.model['itemRecruitmentId'] = item.id;
        //             e.model.itemName = item.name;
        //         }
        //     }
        //     $timeout(function () {
        //         let grid = $('#itemsOnGrid').data("kendoGrid");
        //         grid.saveChanges();
        //     }, 0)
        // }
    };

    $scope.onchangeItem = function(e, dataItem) {
        let result = $scope.itemsDatasource.find(x => x.code == dataItem.itemCode);
        if(result) {
            dataItem.itemName = result.name;
            dataItem.itemRecruitmentId = result.id;
        }
        let grid = $("#itemsOnGrid").data("kendoGrid");
        grid.refresh();
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
                // controlName
                errors.push({
                    controlName: nMessage
                });
            }
        });
        return errors;
    }

    function ValidationForTableItems(items) {
        let errors = [];
        items.forEach(item => {
            errors = errors.concat(validationRequired(item, requiredFieldsOfTableItems, '<span class="bold">[field] of No [index]</span>: Field is required'));
        });

        if (errors.length) {
            errors = errors.map(({ controlName }) => ({ message: controlName, groupName: 'Please enter all required fields' }));
            errors = _.groupBy(errors, 'groupName');
        }
        return errors;
    }

    function ValidationForJobGrade(model) {
        let errors = validationRequired(model, requiredFieldsOfJobGrade, '[field] : Field is required');
        return errors;
    }

    function showNotification(messages) {
        let messageShow = messages.map(x => x.controlName).join('</br>');
        Notification.error(messageShow);
    }

    async function saveJobGrade(model) {
        $scope.errors = ValidationForJobGrade(model);
        if ($scope.errors.length) {
            showNotification($scope.errors);
            return false;
        } else {

            let result = { isSuccess: false };

            if (!model.id) {
                result = await settingService.getInstance().jobgrade.createJobGrade(model).$promise;
            } else {
                result = await settingService.getInstance().jobgrade.updateJobGrade(model).$promise;
            }

            if (result.isSuccess) {
                Notification.success("Data Successfully Saved");
                model['canEdit'] = false;
                refreshGrid('#allOrMyRequestGrid');
                return true;
            } else {
                Notification.error("Fail");
                return false;
            }

        }
    }

    async function saveItems(idInHtml) {
        let grid = $(idInHtml).data("kendoGrid");

        // bỏ validation do meeting yêu cầu dòng nào chọn thì sẽ cho vào
        // $scope.$apply(function () {
        //     $scope.errorsTwo = ValidationForTableItems(grid.dataSource._data);
        // });  

        if ($scope.getLengthObjectKeys($scope.errorsTwo)) {
            // showNotification($scope.errors);
            return false;
        } else {

            // chuẩn bị data
            const itemListRecruitmentIds = grid.dataSource._data.map(i => i['itemRecruitmentId']);
            const model = { id: $scope.jobGradeIdNeedAddItem, itemListRecruitmentIds };
            // call api lưu vào db
            const saveResult = await addOrUpdateItemsOfJobGrade(model);

            if (saveResult.isSuccess) {
                Notification.success(" Data Successfully Saved");
                return false;
            } else {
                Notification.error("Fail");
                return false;
            }
        }
    }

    // get list
    function buildArgs(reset = false) {
        if (reset) {

        }
    }

    // call api ở đây
    async function getDataFromApi(reset) {
        // buildArgs();
        let model = {
            Predicate: "",
            PredicateParameters: [],
            Order: "Grade asc",
            Limit: appSetting.pageSizeDefault,
            Page: 1
        }

        if ($scope.searchInfo.keyword) {
            model.Predicate = "Caption.contains(@0) or Title.contains(@0)";
            model.PredicateParameters.push($scope.searchInfo.keyword);
            model.Limit = 10000;
        }

        let data = await settingService.getInstance().jobgrade.getJobGradeList(model).$promise;
        let result = {
            count: data.object.count,
            items: data.object.data
        };
        return result;
    }

    async function getList(reset, pageIndex) {
        // call api tại đây rồi return ra cái list
        let result = await getDataFromApi(reset);
        let items = result.items;
        let total = result.count;
        // biến đổi dữ liệu cho phù hợp
        // lấy pageIndex hiện tại 
        if (items.length) {
            let noFirst = ((pageIndex - 1) * appSetting.pageSizeDefault) + 1;
            items.forEach(item => {
                item['no'] = noFirst;
                item['canEdit'] = false
                noFirst++;
            });
        }

        // thêm một dòng dữ liệu rỗng
        let nValue = {
            no: '',
            grade: total + 1,
            caption: '',
            title: '',
            expiredDayPosition: 0,
            items: [],
            canEdit: false
        };
        items.push(nValue);
        // setdata vào lại grid
        let grid = $("#allOrMyRequestGrid").data("kendoGrid");
        let dataSource = new kendo.data.DataSource({
            data: items,
            pageSize: appSetting.pageSizeDefault,
            page: pageIndex,
            schema: {
                total: function () {
                    return total;
                }
            }
        });
        grid.setDataSource(dataSource);
    }

    $scope.export = async function () {
        var res = await fileService.getInstance().processingFiles.export({
            type: 26
        }, {
            Predicate: '',
            PredicateParameters: []
        }).$promise;
        if (res.isSuccess) {
            exportToExcelFile(res.object);
        }
    }

    // $scope.import = async function () {

    // }

    // phần khai báo chung
    $scope.actions = {
        addItem: addItem,
        createJobGrade: createJobGrade,
        deleteJobGrade: deleteJobGrade,
        editJobGrade: editJobGrade,
        cancelEditJobGrade: cancelEditJobGrade,
        addItemsJobGrade: addItemsJobGrade,
        saveJobGrade: saveJobGrade,
        ifEnter: ifEnter
    };

    function ifEnter($event) {
        var keyCode = $event.which || $event.keyCode;
        if (keyCode === 13) {
            // Do that thing you finally wanted to do
            getList(true, 1);
        }
    }

    $scope.search = function () {
        getList(true, 1);
    }

    $scope.$on('$locationChangeStart', function (event, next, current) {
        $scope.errors = [];
    });

    // thêm vào 1 dòng dữ liệu rỗng để có thể hiện cuối danh sách

    // hàm này sẽ chạy sau khi view được render lên
    $scope.$on('$viewContentLoaded', function () {
        getList(true, 1);
    });

});