var ssgApp = angular.module('ssg.overtimeApplicationModule', ["kendo.directives"]);
ssgApp.controller('overtimeApplicationController',
    function (
        $rootScope,
        $compile,
        $scope,
        $location,
        appSetting,
        Notification,
        $stateParams,
        $state,
        moment,
        cbService,
        commonData,
        settingService,
        $timeout,
        workflowService,
        fileService,
        $translate,
        dataService,
        $sce,
        masterDataService,
        attachmentService,
        localStorageService,
        $window,
        ssgexService) {
        // create a message to display in our view
        var ssg = this;
        $scope.isITHelpdesk = $rootScope.currentUser && $rootScope.currentUser.isITHelpDesk;
        $scope.value = 0;
        $rootScope.isParentMenu = false;
        $scope.refValue = $stateParams.referenceValue;
        var currentAction = null;
        $scope.errors = [];
        isItem = true;
        $scope.warningDialog = {};
        $scope.importDialog = {};
        $scope.allUsers = [];
        $scope.allHolidays = null;
        $scope.waringOTs = [];

        var isStoreCurrentUser = false;
        $scope.titleHeader = 'OVERTIME APPLICATION';
        //$scope.title = $stateParams.id ? "OVERTIME APPLICATION: " + $stateParams.referenceValue : $stateParams.action.title;
        $scope.title = $stateParams.id ? /*$translate.instant('OVERTIME_TIMECLOCK_EDIT_TITLE') +*/ $stateParams.referenceValue : $state.current.name == 'home.overtimeApplication.item' ? /*$translate.instant('OVERTIME_APPLICATION_NEW_TITLE') */'' : $state.current.name == 'home.overtimeApplication.myRequests' ? $translate.instant('OVERTIME_APPLICATION_MY_REQUETS').toUpperCase() : $translate.instant('OVERTIME_APPLICATION_ALL_REQUETS').toUpperCase();
        var mode = $stateParams.id ? 'Edit' : 'New';
        $scope.mode = $stateParams.id ? 'Edit' : 'New';
        dataService.trackingHistory = [];
        //let strDepartments = sessionStorage.getItemWithSafe("departments");
        //var allDepartments = !$.isEmptyObject(strDepartments) ? JSON.parse(strDepartments) : [];

        var allDepartments = JSON.parse(sessionStorage.getItemWithSafe("departments"));

        $scope.form = {
            id: null,
            overtimeList: [],
        }

        $scope.processingStageRound = 0;
        //// Duc lam
        $scope.sendData = async function () {
            let attachmentFilesJson = null;
            if ($scope.attachmentDetails.length || $scope.removeFileDetails.length) {
                attachmentFilesJson = await mergeAttachment();
                if (attachmentFilesJson !== "" && attachmentFilesJson !== "[]") {
                    var res = await save();
                    if (res && res.isSuccess) {
                        Notification.success('Upload Data Successfully');
                    }
                }
            }
            if (!attachmentFilesJson) {
                Notification.error($translate.instant('OVERTIME_APPLICATION_FILE_FORMAT_NOT_OK'));
            }
        }

        $scope.sendActualData = async function () {
            let result = null;
            if ($scope.attachmentActualDetails.length) {
                result = await uploadActualAction();
                if (!$.isEmptyObject(result) && result.isSuccess) {
                    $scope.closeImportActualDialog();
                    var res = await save();
                    if (res && res.isSuccess) {
                        Notification.success('Upload Data Successfully');
                        $state.go($state.current.name, { id: $scope.model.id, referenceValue: $scope.model.referenceNumber }, { reload: true });
                    }
                    else {
                        $scope.$applyAsync();
                    }
                }
                else {
                    Notification.error($translate.instant('OVERTIME_APPLICATION_FILE_FORMAT_NOT_OK'));
                }
            }
        }
        $scope.closeImportDialog = function () {
            $scope.attachmentDetails = [];
            var upload = $("#attachmentDetail").data("kendoUpload");
            upload.removeAllFiles();
            setTimeout(function () {
                if ($scope.dialogVisible) {
                    let dialog = $("#dialog_import").data("kendoDialog");
                    dialog.close();
                    $scope.dialogVisible = false;
                }
            }, 0);
            return false;
        }
        $scope.closeImportActualDialog = function () {
            $scope.attachmentActualDetails = [];
            var upload = $("#attachmentActualDetail").data("kendoUpload");
            upload.removeAllFiles();
            setTimeout(function () {
                if ($scope.dialogVisible) {
                    let dialog = $("#dialog_import_actual").data("kendoDialog");
                    dialog.close();
                    $scope.dialogVisible = false;
                }
            }, 0);
            return false;
        }
        $scope.attachmentDetails = [];
        $scope.attachmentActualDetails = [];
        $scope.oldAttachmentDetails = [];
        $scope.removeFileDetails = [];
        $scope.onSelect = function (e) {
            let message = $.map(e.files, function (file) {
                $scope.attachmentDetails.push(file);
                return file.name;
            }).join(", ");
        };
        $scope.onActualSelect = function (e) {
            let message = $.map(e.files, function (file) {
                $scope.attachmentActualDetails.push(file);
                return file.name;
            }).join(", ");
        };
        $scope.removeAttach = function (e) {
            let file = e.files[0];
            $scope.attachmentDetails = $scope.attachmentDetails.filter(item => item.name !== file.name);
        }
        $scope.removeActualAttach = function (e) {
            let file = e.files[0];
            $scope.attachmentActualDetails = $scope.attachmentActualDetails.filter(item => item.name !== file.name);
        }
        $scope.removeOldAttachment = function (model) {
            $scope.oldAttachmentDetails = $scope.oldAttachmentDetails.filter(item => item.id !== model.id);
            $scope.removeFileDetails.push(model.id);
        }
        $scope.downloadAttachment = async function (id) {
            let result = await attachmentFile.downloadAndSaveFile({
                id
            });
        }
        async function uploadAction() {
            let errors = [];
            var payload = new FormData();
            $scope.attachmentDetails.forEach(item => {
                payload.append('file', item.rawFile, item.name);
            });
            let argData = {
                sapCodes: []
            };
            let resultgetUser = null
            if ($scope.model.divisionId) {
                if ($scope.model.divisionId === '00000000-0000-0000-0000-000000000000') {
                    resultgetUser = await settingService.getInstance().users.getUserCheckedHeadCount({ departmentId: $rootScope.currentUser.deptId, textSearch: $scope.gridUser.keyword }).$promise;
                }
                else {
                    resultgetUser = await settingService.getInstance().users.getChildUsers({ departmentId: $scope.model.divisionId, limit: 1000000, page: 1, searchText: $scope.gridUser.keyword, isAll: true }).$promise;
                }
                //let resultgetUser = await settingService.getInstance().users.getChildUsers({ departmentId: $scope.model.divisionId, isAll: true, limit: 10000, page: 1, searchText: '' }).$promise;
                if (resultgetUser.isSuccess) {
                    $scope.allUsers = resultgetUser.object.data;
                    argData.sapCodes = _.map($scope.allUsers, 'sapCode').join(',');
                }
            }
            else {
                resultgetUser = await settingService.getInstance().users.getUserCheckedHeadCount({ departmentId: $rootScope.currentUser.deptId, textSearch: '' }).$promise;
                if (resultgetUser.isSuccess) {
                    $scope.allUsers = resultgetUser.object.data;
                    argData.sapCodes = _.map($scope.allUsers, 'sapCode').join(',');
                }
            }
            let result = await cbService.getInstance().overtimeApplication.import(argData, payload).$promise;
            if (result.isSuccess && result.object && result.object.data.length) {
                //code moi 
                $scope.model.overtimeList = result.object.data;
                $scope.form.overtimeList = $scope.model.overtimeList.map(x => {
                    var _data = {
                        sapCode: x.sapCode,
                        userId: x.userId,
                        fullName: x.fullName,
                        department: x.department,
                        dateOfOT: moment(x.date).utcOffset(7).format('DD/MM/YYYY'),
                        OTProposalHour: mapOTHourV2(x.isStore, x.proposalHoursFrom, x.proposalHoursTo),
                        from: moment(x.proposalHoursFrom, 'HH:mm').format('HH:mm'),
                        to: moment(x.proposalHoursTo, 'HH:mm').format('HH:mm'),
                        //to: x.proposalHoursTo,
                        dayOffInLieu: x.dateOffInLieu,
                        isNoOT: x.isNoOT
                    }
                    return _data;
                });
                $scope.$apply();
            }
            else {
                $scope.form.overtimeList.length = "";
                var i = 0;
                result.messages.forEach(item => {
                    switch (result.errorCodes[i]) {
                        case 1002:
                            errors.push({
                                controlName: $translate.instant('OVERTIME_APPLICATION_OT_PROPOSAL_FROM') + ' ' + $translate.instant('OVERTIME_APPLICATION_IN_LINE') + ' ' + item,
                                errorDetail: ' :' + $translate.instant('OVERTIME_APPLICATION_WRONG_FORMAT_FROM_OR_TO'),
                                isRule: true
                            });
                            break;
                        case 1003:
                            errors.push({
                                controlName: $translate.instant('OVERTIME_APPLICATION_DATE_OF_OT') + ' ' + $translate.instant('OVERTIME_APPLICATION_IN_LINE') + ' ' + item,
                                errorDetail: ' :' + $translate.instant('COMMON_IS_NOT_INVALID'),
                                isRule: true
                            });
                            break;
                        case 1004:
                            errors.push({
                                controlName: $translate.instant('COMMON_SAP_CODE') + ' ' + $translate.instant('OVERTIME_APPLICATION_IN_LINE') + ' ' + item,
                                errorDetail: ' :' + $translate.instant('OVERTIME_APPLICATION_NOT_FOUND'),
                                isRule: true
                            });
                            break;
                        case 1005:
                            errors.push({
                                controlName: $translate.instant('OVERTIME_APPLICATION_OT_PROPOSAL_TO') + ' ' + $translate.instant('OVERTIME_APPLICATION_IN_LINE') + ' ' + item,
                                errorDetail: ' :' + $translate.instant('OVERTIME_APPLICATION_WRONG_FORMAT_FROM_OR_TO'),
                                isRule: true
                            });
                            break;
                        case 1006:
                            errors.push({
                                controlName: $translate.instant('COMMON_SAP_CODE') + ' ' + $translate.instant('OVERTIME_APPLICATION_IN_LINE') + ' ' + item,
                                errorDetail: ' :' + $translate.instant('OVERTIME_APPLICATION_EMPTY_VALIDATE'),
                                isRule: true
                            });
                            break;
                        default:
                            break;
                    }
                    i++;
                });
                if (errors.length > 0) {
                    $scope.errors = errors;
                    return result;
                }
            }
            return result;
        }
        async function uploadActualAction() {
            let errors = [];
            var payload = new FormData();
            $scope.attachmentActualDetails.forEach(item => {
                payload.append('file', item.rawFile, item.name);
            });
            let argData = {
                sapCodes: []
            };
            let resultgetUser = null
            if ($scope.model.divisionId) {
                if ($scope.model.divisionId === '00000000-0000-0000-0000-000000000000') {
                    resultgetUser = await settingService.getInstance().users.getUserCheckedHeadCount({ departmentId: $rootScope.currentUser.deptId, textSearch: $scope.gridUser.keyword }).$promise;
                }
                else {
                    resultgetUser = await settingService.getInstance().users.getChildUsers({ departmentId: $scope.model.divisionId, limit: 1000000, page: 1, searchText: $scope.gridUser.keyword, isAll: true }).$promise;
                }
                //let resultgetUser = await settingService.getInstance().users.getChildUsers({ departmentId: $scope.model.divisionId, isAll: true, limit: 10000, page: 1, searchText: '' }).$promise;
                if (resultgetUser.isSuccess) {
                    $scope.allUsers = resultgetUser.object.data;
                    argData.sapCodes = _.map($scope.allUsers, 'sapCode').join(',');
                }
            }
            else {
                resultgetUser = await settingService.getInstance().users.getUserCheckedHeadCount({ departmentId: $rootScope.currentUser.deptId, textSearch: '' }).$promise;
                if (resultgetUser.isSuccess) {
                    $scope.allUsers = resultgetUser.object.data;
                    argData.sapCodes = _.map($scope.allUsers, 'sapCode').join(',');
                }
            }
            let result = await cbService.getInstance().overtimeApplication.importActual(argData, payload).$promise;
            if (result.isSuccess && result.object && result.object.data.length) {
                //code moi 
                var myList = result.object.data;
                let getOverItemBySAPCodeAndDate = function (item) {
                    if (!$.isEmptyObject(item) && !$.isEmptyObject(item.date)) {
                        return myList.filter(function (x, y) {
                            if (!$.isEmptyObject(x) && !$.isEmptyObject(x.date)) {
                                return x.sapCode == item.sapCode && moment(x.date).format('DD/MM/YYYY') == moment(item.date).format('DD/MM/YYYY')
                            }
                            else {
                                return false;
                            }
                        });
                    }
                    else {
                        return null;
                    }
                };

                $scope.form.overtimeList = $scope.form.overtimeList.map(x => {
                    var currentItem = getOverItemBySAPCodeAndDate(x);
                    if (!$.isEmptyObject(currentItem) && currentItem.length > 0) {
                        currentItem = currentItem[0];
                        x.fromActual = moment(currentItem.actualHoursFrom, 'HH:mm').format('HH:mm');
                        x.toActual = moment(currentItem.actualHoursTo, 'HH:mm').format('HH:mm');
                        x.dayOffInLieu = currentItem.dateOffInLieu;
                        x.isPrevDay = currentItem.isPrevDay;
                        x.isNoOT = currentItem.isNoOT;
                    }
                    return x;
                });
            }
            else {
                var i = 0;
                result.messages.forEach(item => {
                    switch (result.errorCodes[i]) {
                        case 1001:
                            errors.push({
                                controlName: "",
                                errorDetail: ' :' + $translate.instant(item),
                                isRule: true
                            });
                            break;
                        case 1002:
                            errors.push({
                                controlName: $translate.instant('OVERTIME_APPLICATION_OT_PROPOSAL_FROM') + ' ' + $translate.instant('OVERTIME_APPLICATION_IN_LINE') + ' ' + item,
                                errorDetail: ' :' + $translate.instant('OVERTIME_APPLICATION_WRONG_FORMAT_FROM_OR_TO'),
                                isRule: true
                            });
                            break;
                        case 1003:
                            errors.push({
                                controlName: $translate.instant('OVERTIME_APPLICATION_DATE_OF_OT') + ' ' + $translate.instant('OVERTIME_APPLICATION_IN_LINE') + ' ' + item,
                                errorDetail: ' :' + $translate.instant('COMMON_IS_NOT_INVALID'),
                                isRule: true
                            });
                            break;
                        case 1004:
                            errors.push({
                                controlName: $translate.instant('COMMON_SAP_CODE') + ' ' + $translate.instant('OVERTIME_APPLICATION_IN_LINE') + ' ' + item,
                                errorDetail: ' :' + $translate.instant('OVERTIME_APPLICATION_NOT_FOUND'),
                                isRule: true
                            });
                            break;
                        case 1005:
                            errors.push({
                                controlName: $translate.instant('OVERTIME_APPLICATION_OT_PROPOSAL_TO') + ' ' + $translate.instant('OVERTIME_APPLICATION_IN_LINE') + ' ' + item,
                                errorDetail: ' :' + $translate.instant('OVERTIME_APPLICATION_WRONG_FORMAT_FROM_OR_TO'),
                                isRule: true
                            });
                            break;
                        case 1006:
                            errors.push({
                                controlName: $translate.instant('COMMON_SAP_CODE') + ' ' + $translate.instant('OVERTIME_APPLICATION_IN_LINE') + ' ' + item,
                                errorDetail: ' :' + $translate.instant('OVERTIME_APPLICATION_EMPTY_VALIDATE'),
                                isRule: true
                            });
                            break;
                        default:
                            break;
                    }
                    i++;
                });
                if (errors.length > 0) {
                    $scope.errors = errors;
                    return result;
                }
            }
            return result;
        }
        async function mergeAttachment() {
            try {
                // Upload file lên server rồi lấy thông tin lưu thành chuỗi json
                let uploadResult = await uploadAction();
                let attachmentFilesJson = '';
                let allattachmentDetails = $scope.oldAttachmentDetails && $scope.oldAttachmentDetails.length ? $scope.oldAttachmentDetails.map(({
                    id,
                    fileDisplayName
                }) => ({
                    id,
                    fileDisplayName
                })) : [];
                if (uploadResult.isSuccess) {
                    allattachmentDetails = allattachmentDetails.concat(['Import Overtime Application.xlsx']);
                    $scope.closeImportDialog();
                }
                $scope.closeImportDialog();
                attachmentFilesJson = JSON.stringify(allattachmentDetails);
                return attachmentFilesJson;
            } catch (e) {
                console.log(e);
                return '';
            }
        }
        $scope.attachmentChanges = [];
        $scope.oldAttachmentChanges = [];
        $scope.removeFileChanges = [];
        $scope.onSelectChange = function (e) {
            let message = $.map(e.files, function (file) {
                $scope.attachmentChanges.push(file);
                return file.name;
            }).join(", ");
        };

        $scope.removeAttachChange = function (e) {
            let file = e.files[0];
            $scope.attachmentChanges = $scope.attachmentChanges.filter(item => item.name !== file.name);
        }

        $scope.removeOldAttachmentChange = function (model) {
            $scope.oldAttachmentChanges = $scope.oldAttachmentChanges.filter(item => item.id !== model.id);
            $scope.removeFileChanges.push(model.id);
        }

        $scope.downloadAttachmentChange = async function (id) {
            let result = await attachmentFile.downloadAndSaveFile({
                id
            });
        }
        $scope.importDialogOpts = {
            width: "600px",
            buttonLayout: "normal",
            closable: true,
            modal: true,
            visible: false,
            content: "",
            multiple: false
        };
        $scope.onClickSelectFile = function () {
            $(".attachmentDetail").kendoUpload({
                multiple: false,
                validation: {
                    allowedExtensions: ['XLS', 'XLSX']
                }
            }).getKendoUpload();
        }
        $scope.import = function () {
            let dialog = $("#dialog_import").data("kendoDialog");
            dialog.title('IMPORT');
            dialog.open();
        }
        $scope.importActual = function () {
            let dialog = $("#dialog_import_actual").data("kendoDialog");
            dialog.title('IMPORT');
            dialog.open();
        }
        ///Het Import

        $scope.gridUser = {
            keyword: '',
        }

        $scope.gridUserReview = {
            keyword: '',
        }

        $scope.reasons = [];
        $scope.workflowInstances = [];
        $scope.isShow = false;
        $scope.reasonOfOTListOptions = {
            dataTextField: "name",
            dataValueField: "code",
            template: '<span><label>#: data.name# </label></span>',
            valueTemplate: '#: name #',
            filter: "contains",
            dataSource: $scope.reasons,
            filtering: function (ev) {
                var filterValue = ev.filter != undefined ? ev.filter.value : "";
                ev.preventDefault();

                this.dataSource.filter({
                    logic: "or",
                    filters: [{
                        field: "name",
                        operator: "contains",
                        value: filterValue
                    },
                    {
                        field: "code",
                        operator: "contains",
                        value: filterValue
                    }
                    ]
                });
            },
        }
        $scope.viewData = {
            overtimeList: [],
        }
        $scope.validateFields = [
            {
                fieldName: 'dateOfOT',
                // title: 'Date Of OT',
                title: $translate.instant('OVERTIME_APPLICATION_DATE_OF_OT'),
            },
            {
                fieldName: 'from',
                // title: 'From',
                title: $translate.instant('OVERTIME_APPLICATION_OT_PROPOSAL_FROM'),
            },
            {
                fieldName: 'to',
                //title: 'To',
                title: $translate.instant('OVERTIME_APPLICATION_OT_PROPOSAL_TO'),
            },
            {
                fieldName: 'fromActual',
                //title: 'From Actual',
                title: $translate.instant('OVERTIME_APPLICATION_OT_ACTUAL_FROM'),
            },
            {
                fieldName: 'toActual',
                //title: 'To Actual',
                title: $translate.instant('OVERTIME_APPLICATION_OT_ACTUAL_TO'),
            },
            {
                fieldName: 'OTProposalHour',
                //title: 'OT Proposal Hour',
                title: $translate.instant('OVERTIME_APPLICATION_OT_PROPOSAL_HOURS'),
            },
            {
                fieldName: 'actualOTHour',
                //title: 'Actual OT Hour',
                title: $translate.instant('OVERTIME_APPLICATION_OT_ACTUAL_HOURS'),
            },
            {
                fieldName: 'reasonOfOT',
                title: $translate.instant('OVERTIME_APPLICATION_REASON_REQUIRED'),
            },
            // {
            //     fieldName: 'detailReasonOfOT',
            //     title: 'Detail Reason',
            // },
        ]
        $scope.dateEditor = function (container, options) {
            let dateformatted = moment(options.model[options.field], 'DD/MM/YYYY').toDate();
            $('<input type="text" name="' + options.field + '" />')
                .appendTo(container)
                .kendoDatePicker({
                    format: "dd/MM/yyyy",
                    value: dateformatted
                });
        }
        async function getTimeConfiguration() {
            let args = {
                type: commonData.reasonType.TIME_CONFIGURATION,
            }
            var result = await settingService.getInstance().timeConfiguration.getTimeConfigurations(args).$promise;
            if (result.isSuccess) {
                _.forEach(result.object.data, function (item) {
                    $scope[item.name.replace(/\s+/g, '')] = item.code;
                })
            }
        }
        getTimeConfiguration();
        $scope.timeEditor = function (container, options) {
            $('<input type="text" name="' + options.field + '" />')
                .appendTo(container)
                .kendoTimePicker({
                    format: "HH:mm",
                    interval: 15,
                    value: kendo.toString(options.model[options.field], 'HH:mm')
                });
        }
        $scope.overtimeReadListGridOptions = {
            dataSource: {
                data: $scope.viewData.overtimeList,
                pageSize: 20,
            },
            sortable: false,
            // pageable: true,
            editable: false,
            pageable: {
                alwaysVisible: true,
                pageSizes: appSetting.pageSizesArray,
            },
            columns: [{
                field: "overtimeApplicationId",
                hidden: true
            },
            {
                field: "date",
                //title: "Date Of OT",
                headerTemplate: $translate.instant('OVERTIME_APPLICATION_DATE_OF_OT'),
                width: "180px",
                format: "{0:dd/MM/yyyy}"
            },
            {
                field: "reasonName",
                //title: 'Reason Of OT',
                headerTemplate: $translate.instant('OVERTIME_APPLICATION_REASON_OF_OT'),
                width: "150px",
            },
            {
                field: "detailReason",
                //title: 'Detail Reason Of OT',
                headerTemplate: $translate.instant('COMMON_OTHER_REASON'),
                width: "200px",
            },
            {
                field: "OTProposalHour",
                //title: "OT Proposal Hour",
                headerTemplate: $translate.instant('OVERTIME_APPLICATION_OT_PROPOSAL_HOURS'),
                width: "150px",
                format: "{0:d}",
                defaultValue: 1,
            },
            {
                field: "proposalHoursFrom",
                //title: "From",
                headerTemplate: $translate.instant('OVERTIME_APPLICATION_OT_PROPOSAL_FROM'),
                width: "100px",
                format: "{0:HH:mm}",
            }, {
                field: "proposalHoursTo",
                //title: "To",
                headerTemplate: $translate.instant('OVERTIME_APPLICATION_OT_PROPOSAL_TO'),
                width: "100px",
                format: "{0:HH:mm}",
            },
            {
                field: "actualOTHour",
                //title: 'Actual OT Hour',
                headerTemplate: $translate.instant('OVERTIME_APPLICATION_OT_ACTUAL_HOURS'),
                width: "150px",
                format: "{0:d}",
                defaultValue: 1,
            }, {
                field: "actualHoursFrom",
                //title: 'From',
                headerTemplate: $translate.instant('OVERTIME_APPLICATION_OT_ACTUAL_FROM'),
                format: "{0:HH:mm}",
                width: "100px",
            },
            {
                field: "actualHoursTo",
                //title: 'To',
                headerTemplate: $translate.instant('OVERTIME_APPLICATION_OT_ACTUAL_TO'),
                format: "{0:HH:mm}",
                width: "100px",
            },
            {
                field: "dateOffInLieu",
                //title: 'Day Off In Lieu',
                headerTemplate: $translate.instant('OVERTIME_APPLICATION_DATE_OFF_IN_LIEU'),
                width: "200px",
                defaultValue: false,
                template: "<input name='#:date#' id='#:date#' class='k-checkbox' type='checkbox' #= (dateOffInLieu == true) ? checked ='checked' : '' # disabled/> <label class='k-checkbox-label' for='#:date#'></label>",
            }
            ]
        };
        $scope.data = [];
        //Search
        $scope.departments = [];
        $scope.departmentOptions = {
            placeholder: "",
            dataTextField: "name",
            dataValueField: "code",
            //template: '#: item.code # - #: item.name #',
            template: showCustomDepartmentTitle,
            //valueTemplate: '#: item.id #',
            valuePrimitive: true,
            checkboxes: false,
            autoBind: true,
            filter: "contains",
            filtering: async function (option) {
                await getDepartmentByFilter(option);
            },
            loadOnDemand: true,
            valueTemplate: (e) => showCustomField(e, ['name']),
            change: function (e) {
                if (!e.sender.value()) {
                    clearSearchTextOnDropdownTree('departmentId');
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
                if (res.isSuccess) {
                    setDataDepartment(res.object.data);
                }
            }
        }
        function setDataDepartment(dataDepartment) {
            var dataSource = new kendo.data.HierarchicalDataSource({
                data: dataDepartment,
                schema: {
                    model: {
                        children: "items"
                    }
                }
            });
            var departmentTree = $("#departmentId").data("kendoDropDownTree");
            if (departmentTree) {
                departmentTree.setDataSource(dataSource);
            }
        }

        $scope.reasonManagers = [];
        async function getReasons() {
            var res = await settingService.getInstance().cabs.getAllReason({ nameType: commonData.reasonType.OVERTIME_REASON }).$promise;
            if (res.isSuccess) {
                $scope.reasonOfOTListOptions.dataSource = res.object.data;
                $scope.reasons = res.object.data;
                $scope.reasons.forEach(item => {
                    $scope.reasonManagers.push(item);
                });
                setDataReasonOfOT($scope.reasonManagers);
            }
        }

        function getStatusClass(dataItem) {
            let classStatus = "";
            switch (dataItem.status) {
                case commonData.StatusMissingTimeLock.WaitingCMD:
                    classStatus = "fa-circle font-yellow-lemon";
                    break;
                case commonData.StatusMissingTimeLock.WaitingHOD:
                    classStatus = "fa-circle font-yellow-lemon";
                    break;
                case commonData.StatusMissingTimeLock.Completed:
                    classStatus = "fa-check-circle font-green-jungle";
                    break;
                case commonData.StatusMissingTimeLock.Rejected:
                    classStatus = "fa-ban font-red";
                    break;
                default:
            }
            return classStatus;
        }
        $scope.query = {
            keyword: '',
            userSAPCode: '',
            departmentId: null,
            fromDate: null,
            toDate: null
        }
        $scope.optionSave = ''
        $scope.model = {
            overtimeApplicationGridOptions: {
                dataSource: {
                    serverPaging: true,
                    data: $scope.data,
                    pageSize: 20,
                    transport: {
                        read: async function (e) {
                            $scope.optionSave = e;
                            await getAllOvertime(e);
                        }
                    },
                    schema: {
                        total: () => { return $scope.total },
                        data: () => { return $scope.data }
                    }
                },
                sortable: false,
                // pageable: true,
                pageable: {
                    alwaysVisible: true,
                    pageSizes: appSetting.pageSizesArray,
                    messages: {
                        display: "{0}-{1} " + $translate.instant('PAGING_OF') + " {2} " + $translate.instant('PAGING_ITEM'),
                        itemsPerPage: $translate.instant('PAGING_ITEM_PER_PAGE'),
                        empty: $translate.instant('PAGING_NO_ITEM')
                    }
                },
                columns: [{
                    hidden: true,
                    field: "id"
                },
                {
                    field: "status",
                    //title: "Status",
                    headerTemplate: $translate.instant('COMMON_STATUS'),
                    locked: true,
                    lockable: false,
                    width: "350px",
                    template: function (dataItem) {
                        var statusTranslate = $rootScope.getStatusTranslate(dataItem.status);
                        return `<workflow-status status="${statusTranslate}"></workflow-status>`;
                        //return `<workflow-status status="${dataItem.status}"></workflow-status>`;
                    }
                },
                {
                    field: "referenceNumber",
                    //title: 'Reference Number',
                    headerTemplate: $translate.instant('COMMON_REFERENCE_NUMBER'),
                    locked: true,
                    lockable: false,
                    width: "180px",
                    template: function (dataItem) {
                        // if (
                        //     dataItem.status === commonData.StatusMissingTimeLock.WaitingCMD ||
                        //     dataItem.status === commonData.StatusMissingTimeLock.WaitingHOD
                        // ) {
                        //     return `<a ui-sref= "${viewState}({referenceValue:'${dataItem.referenceNumber}', id: '${dataItem.id}}')" ui-sref-opts="{ reload: true }">${dataItem.referenceNumber}</a>`;
                        // }
                        return `<a ui-sref= "home.overtimeApplication.item({referenceValue:'${dataItem.referenceNumber}', id: '${dataItem.id}'})" ui-sref-opts="{ reload: true }">${dataItem.referenceNumber}</a>`;

                        // return `<a ui-sref=${viewState}({referenceValue:'${dataItem.referenceNumber}'}) 
                        //             ui-sref-opts="{ reload: true }">${dataItem.referenceNumber}</a>`;
                    }
                },
                {
                    field: "userSAPCode",
                    //title: 'SAP Code',
                    headerTemplate: $translate.instant('COMMON_SAP_CODE'),
                    locked: false,
                    lockable: false,
                    width: "150px",
                    // template: function(dataItem) {
                    //     if (
                    //         dataItem.status === commonData.StatusMissingTimeLock.WaitingCMD ||
                    //         dataItem.status === commonData.StatusMissingTimeLock.WaitingHOD
                    //     ) {
                    //         return `<a ui-sref= "${viewState}({referenceValue:'${dataItem.referenceNumber}', id: '${dataItem.id}'})" ui-sref-opts="{ reload: true }">${dataItem.userSAPCode}</a>`;
                    //     }
                    //     return `<a ui-sref= "home.overtimeApplication.item({referenceValue:'${dataItem.referenceNumber}', id: '${dataItem.id}'})" ui-sref-opts="{ reload: true }">${dataItem.userSAPCode}</a>`;

                    //     // return `<a ui-sref=${viewState}({referenceValue:'${dataItem.referenceNumber}'}) 
                    //     //             ui-sref-opts="{ reload: true }">${dataItem.userSAPCode}</a>`;
                    // }
                },
                {
                    field: "createdByFullName",
                    //title: "Full Name",
                    headerTemplate: $translate.instant('COMMON_FULL_NAME'),
                    width: "180px",
                },
                {
                    field: "deptName",
                    //title: "Dept/ Line",
                    headerTemplate: $translate.instant('COMMON_DEPT_LINE'),
                    width: "350px"
                }, {
                    field: "divisionName",
                    //title: "Division/ Group",
                    headerTemplate: $translate.instant('COMMON_DIVISION_GROUP'),
                    width: "350px"
                },
                {
                    field: "workLocationName",
                    //title: 'Work Location',
                    headerTemplate: $translate.instant('COMMON_WORK_LOCATION'),
                    width: "200px",
                },
                {
                    field: "created",
                    //title: 'Created Date',
                    headerTemplate: $translate.instant('COMMON_CREATED_DATE'),
                    width: "150px",
                    template: function (dataItem) {
                        return moment(dataItem.created).format(appSetting.longDateFormat);
                    }
                }
                ]
            }
        };

        $scope.statusOptions = {
            placeholder: "",
            dataTextField: "name",
            dataValueField: "code",
            valuePrimitive: true,
            checkboxes: false,
            autoBind: false,
            filter: "contains",
            dataSource: {
                // serverFiltering: true,
                //data: commonData.itemStatuses
                data: translateStatus($translate, commonData.itemStatuses)
            }
        };
        $scope.isModalVisible = false;
        $scope.toggleFilterPanel = async function (value) {
            $scope.advancedSearchMode = value;
            $scope.isModalVisible = value;
            if (value) {
                if (!$scope.query.departmentCode || $scope.query.departmentCode == '') {
                    $timeout(function () {
                        setDataDepartment(allDepartments);
                    }, 0);
                }
            }
        }
        // Event
        $scope.readReason = false;
        $scope.isHoliday = function (strDate) {
            let returnValue = false;
            try {
                if ($scope.allHolidays != null) {
                    var dateParts = strDate.split("/");
                    var date = new Date(dateParts[2], dateParts[1] - 1, dateParts[0]);
                    var selectedHoliday = $($scope.allHolidays).filter(function (index, currentHoliday) {
                        return !$.isEmptyObject(currentHoliday) && currentHoliday.fromDate <= date && date <= currentHoliday.toDate;
                    });

                    returnValue = !$.isEmptyObject(selectedHoliday) && selectedHoliday.length > 0;
                }
            } catch (e) {
                returnValue = false;
            }
            return returnValue;
        }
        $scope.changeReason = function (item) {
            if (item) {
                let code = $scope.model.type == commonData.OverTimeType.EmployeeSeftService ? item.reasonOfOT : item.reasonCode;
                item.reasonOfOT ? item.detailReasonOfOT = "" : item.contentOfOtherReason = "";
                if (code) {
                    var result = findReasonName(code);
                    if (result && result.toLowerCase().includes("Other Reason".toLowerCase())) {
                        $scope.readReason = true;
                    } else {
                        $scope.readReason = false;
                    }
                }
            }
        }
        $scope.changeOTDate = function (item, index) {
            if (item) {
                let date = item.dateOfOT;
                if ($scope.isHoliday(date)) {
                    $scope.form.overtimeList[index].dayOffInLieu = true;
                }
                else {
                    $scope.form.overtimeList[index].dayOffInLieu = false;
                }
            }
        }

        $scope.checkPublicHoliday = async function () {
            let cDef = jQuery.Deferred();
            let returnValue = true;
            let checkDOFLStatus = ["Draft", "Requested To Change", "Waiting for Fill Actual Hour"];
            let errors = [];
            if ($.isEmptyObject($scope.model) || $.isEmptyObject($scope.model.status) || checkDOFLStatus.indexOf($scope.model.status) != -1) {
                let hasOT_In_Holiday = false;

                let errorItems = $($scope.form.overtimeList).filter(function (index, item) {

                    let date = item.dateOfOT;
                    if (!hasOT_In_Holiday) {
                        hasOT_In_Holiday = $scope.isHoliday(date) && item.dayOffInLieu == true;
                    }

                    //Check case Holiday but not check DOFL
                    //let status = $scope.isHoliday(date) && !item.dayOffInLieu;
                    ////Check OT with out tick DOFL
                    //if (status) {
                    //    errors.push(
                    //        {
                    //            controlName: (!$.isEmptyObject(item.sapCode) ? item.sapCode + '-' : "") + item.dateOfOT,
                    //            errorDetail: $translate.instant("OVERTIME_APPLICATION_PUBLIC_HOLIDAY_NOT_CHECK_DOFL"),
                    //            isRule: false
                    //        }
                    //    );
                    //}

                    ////Check case not Holiday but check DOFL
                    //status = !$scope.isHoliday(date) && item.dayOffInLieu;
                    //if (status) {
                    //    errors.push(
                    //        {
                    //            controlName: (!$.isEmptyObject(item.sapCode) ? item.sapCode + '-' : "") + item.dateOfOT,
                    //            errorDetail: $translate.instant("OVERTIME_APPLICATION_NOT_PUBLIC_HOLIDAY_BUT_CHECK_DOFL"),
                    //            isRule: false
                    //        }
                    //    );
                    //}
                    return status;
                });
                if (!$.isEmptyObject(errorItems) && errorItems.length > 0) {
                    returnValue = false;
                    kendo.alert($translate.instant("OVERTIME_APPLICATION_SAVE_FAILED"));
                    $scope.errors = errors;
                    $scope.$applyAsync();
                    cDef.resolve(returnValue);
                }
                else {
                    //Confirm OT in Holiday
                    if (hasOT_In_Holiday) {
                        kendo.confirm($translate.instant("OVERTIME_APPLICATION_PUBLIC_HOLIDAY_OT_CONFIRM")).then(function () {
                            //OK button
                            cDef.resolve(true);
                        }, function () {
                            //Cancle button
                            errors.push(
                                {
                                    errorDetail: $translate.instant("OVERTIME_APPLICATION_SAVE_FAILED"),
                                    isRule: false
                                }
                            );
                            kendo.alert($translate.instant("OVERTIME_APPLICATION_SAVE_FAILED"));
                            $scope.errors = errors;
                            $scope.$applyAsync();
                            cDef.resolve(false);
                        });
                    }
                    else {
                        $scope.errors = [];
                        $scope.$applyAsync();
                        cDef.resolve(true);
                    }
                }
            }
            else {
                $scope.errors = [];
                $scope.$applyAsync();
                cDef.resolve(true);
            }
            return cDef.promise();
        }

        $scope.applyClassTextbox = function (item) {
            let strClass = "k-textbox readonly ";
            if (item) {
                let code = $scope.model.type == commonData.OverTimeType.EmployeeSeftService ? item.reasonOfOT : commonData.OverTimeType.ManagerApplyForEmployee ? item.reasonCode : '';
                if (code) {
                    var result = findReasonName(code);
                    if (result && result.toLowerCase().includes("Other Reason".toLowerCase())) {
                        strClass = strClass.replace("readonly", " ");
                        $scope.readReason = true;
                    } else {
                        $scope.readReason = false;
                    }
                }
            }
            return strClass;
        }
        $scope.addSubItem = function (list) {
            list.push({
                userId: '',
                fullName: '',
                dateOfOT: null,
                reasonOfOT: '',
                OTProposalHour: 0,
                from: '',
                to: '',
                actualOTHour: '',
                fromActual: '',
                toActual: '',
                dayOffInLieu: false,
                isPrevDay: false,
                isNoOT: false
            });
        }
        // $scope.removeSubItem = function(list, $index) {
        //     list.splice($index, 1);
        // }

        $scope.index = '';
        $scope.removeSubItem = function (list, $index) {
            $scope.index = $index;
            $scope.dialog = $rootScope.showConfirmDelete($translate.instant('COMMON_BUTTON_DELETE'), $translate.instant('COMMON_DELETE_VALIDATE'), $translate.instant('COMMON_BUTTON_CONFIRM'));
            $scope.dialog.bind("close", confirm);
        }

        confirm = function (e) {
            if (e.data && e.data.value) {
                $scope.form.overtimeList.splice($scope.index, 1);
                Notification.success($translate.instant('COMMON_DELETE_SUCCESS'));
            }
        }

        $scope.overtimeApplicationId = '';

        $scope.submitOvertimeRequirsion = async function () {
            var res = await save();
            if (res.isSuccess) {
                var resSubmitWorkflow = await workflowService.getInstance().workflows.startWorkflow({ itemId: res.object.id }, null).$promise;
                if (resSubmitWorkflow && resSubmitWorkflow.isSuccess) {
                    // Notification.success("Data Successfully Saved");
                    Notification.success($translate.instant('COMMON_SAVE_SUCCESS'));
                } else {
                    Notification.error("Error");
                }
            }
        }

        $scope.saveOvertimeRequirsion = async function (form, isRequiredValidation) {
            let checkStatus = await $scope.checkPublicHoliday();
            if (checkStatus) {
                var res = await save(form, isRequiredValidation);
                return res;
            }
        }

        $scope.$on("waringOTs", async function (evt, message) {
            if (message != null && message != "")
                $scope.waringOTs = [message];
            else 
                $scope.waringOTs = []
            $scope.$apply();
        })

        $scope.renderHtml = function (htmlCode) {
            return $sce.trustAsHtml(htmlCode);
        };

        $scope.disableEmployee = false;
        $scope.disableManager = false;
        async function save(form, perm) {
            $scope.errors = [];
            var result = { isSuccess: false };
            $scope.errors = $scope.validateOvertimeList();
            if ($scope.errors.length > 0) {
                $scope.$apply();
                return result;
            } else {
                $scope.model.overtimeList = $scope.form.overtimeList.map(x => {
                    if ($scope.model.type == commonData.OverTimeType.EmployeeSeftService) {
                        return {
                            id: x.id,
                            overtimeApplicationId: $scope.model.id,
                            fullName: x.fullName,
                            date: x.dateOfOT,
                            reasonCode: x.reasonOfOT,
                            reasonName: findReasonName(x.reasonOfOT),
                            detailReason: x.detailReasonOfOT,
                            proposalHoursFrom: x.from,
                            proposalHoursTo: x.to,
                            actualHoursFrom: x.fromActual,
                            actualHoursTo: x.toActual,
                            dateOffInLieu: x.dayOffInLieu,
                            isPrevDay: x.isPrevDay,
                            isNoOT: x.isNoOT,
                            isStore: isStoreCurrentUser
                        }
                    }
                    if ($scope.model.type == commonData.OverTimeType.ManagerApplyForEmployee) {
                        $scope.model.reasonName = findReasonName($scope.model.reasonCode);
						var applyDateTimeZone = new Date($scope.model.applyDate);
                        var timezoneOffset = applyDateTimeZone.getTimezoneOffset() * -1;
                        if (timezoneOffset != $scope.localtimezone) {
                            var diff = $scope.localtimezone - timezoneOffset;
                            applyDateTimeZone.setMinutes(applyDateTimeZone.getMinutes() + diff);
                            $scope.model.applyDate = moment(applyDateTimeZone).format('YYYY-MM-DD HH:mm:ss') + ' +07:00';
                            $scope.model.applyDate = moment(applyDateTimeZone).utc().format('YYYY-MM-DD HH:mm:ss') + ' +00:00';
                        }
                        $scope.localtimezone = timezoneOffset;
                        return {
                            id: x.id,
                            overtimeApplicationId: $scope.model.id,
                            sapCode: x.sapCode,
                            userId: x.userId,
                            fullName: x.fullName,
                            department: x.department,
                            position: x.position,
                            jobGrade: x.jobGrade,
                            date: x.dateOfOT,
                            reasonCode: x.reasonOfOT,
                            reasonName: findReasonName(x.reasonOfOT),
                            detailReason: x.detailReasonOfOT,
                            proposalHoursFrom: moment(x.from, 'HH:mm').format('HH:mm'),
                            proposalHoursTo: moment(x.to, 'HH:mm').format('HH:mm'),
                            actualHoursFrom: x.fromActual,
                            actualHoursTo: x.toActual,
                            dateOffInLieu: x.dayOffInLieu,
                            isPrevDay: x.isPrevDay,
                            isNoOT: x.isNoOT,
                            isStore: x.isStore
                        }
                    }
                });
                let res = await cbService.getInstance().overtimeApplication.saveOvertimeApplication($scope.model).$promise;
                if (res.isSuccess) {
                    Notification.success($translate.instant('COMMON_SAVE_SUCCESS'));
                    $scope.model.id = res.object.id;
                    $scope.model.referenceNumber = res.object.referenceNumber;
                    $scope.title = $scope.model.id ? /*$translate.instant('OVERTIME_TIMECLOCK_EDIT_TITLE') +*/ $scope.model.referenceNumber : $stateParams.action.title;
                    await getOvertimeDetail(res.object.id);
                    $scope.waringOTs = [];
                    if (res.object.id) {
                        $state.go($state.current.name, { id: res.object.id, referenceValue: res.object.referenceNumber }, { reload: true });
                    }
                } else {
                    var errorInfos = res.object;
                    if (!$.isEmptyObject(errorInfos) && $.type(errorInfos) == "array") {
                        $(errorInfos).map(function (index, currentErrorInfo) {
                            if (!$.isEmptyObject(currentErrorInfo) && !currentErrorInfo.success) {
                                $scope.errors.push(
                                    {
                                        controlName: currentErrorInfo.message,
                                        errorDetail: $translate.instant(currentErrorInfo.errorCode) + (currentErrorInfo.description ? ('(' + currentErrorInfo.description + ')') : ''),
                                        isRule: false
                                    }
                                );
                            }
                        });
                        $scope.$apply();
                    }
                }
                return res;
            }
            return result;
        }

        async function loadHolidays() {
            let returnValue = false;
            try {
                if ($scope.allHolidays == null) {
                    arg = {
                        predicate: "title.contains(@0)",
                        predicateParameters: [''],
                        page: 1,
                        limit: 1000,
                        order: "title"
                    }
                    var res = await settingService.getInstance().cabs.getHolidaySchedules(arg).$promise;
                    if (res && res.isSuccess) {
                        $scope.allHolidays = res.object.data;
                    }
                }
            } catch (e) {
                returnValue = false;
            }
            return returnValue;
        }

        function findReasonName(code) {
            var result;
            $scope.reasons.forEach(item => {
                if (item.code === code) {
                    result = item.name;
                    return result;
                }
            });
            return result;
        }

        $scope.reasonOtherCode = 'OT0005';

        $scope.validateOvertimeList = function () {
            CheckIsStore($scope);
            let errors = [];
            let currentOvertimeList = $scope.form.overtimeList;
            if (currentOvertimeList.length > 0) {
                for (let i in currentOvertimeList) {
                    let item = currentOvertimeList[i];
                    if (item.dateOfOT && item.dateOfOT != 'Invalid date') {
                        let dateOfOT = moment(item.dateOfOT, "DD/MM/YYYY").toDate();
                        let error = validateDateRange($rootScope.salaryDayConfiguration, dateOfOT, null, (parseInt(i) + 1), 'OVERTIME_TIMECLOCK_MENU', $translate, 'OVERTIME_APPLICATION_PERIOD_VALIDATE_1', 'OVERTIME_APPLICATION_PERIOD_VALIDATE_2');

                        if (error.length) {
                            errors.push({ controlName: error[0].controlName + ': ', errorDetail: error[0].message, isRule: true });
                        }
                    }
                    Object.keys(item).forEach(function (fieldName) {
                        let propValue = item[fieldName];
                        //có trong mảng cần validate hay k
                        let requiredFieldIndex = _.findIndex($scope.validateFields, x => {
                            return x.fieldName === fieldName
                        });
                        //Đã có error cho trường này hay chưa
                        let hasErrorBefore = _.findIndex(errors, x => {
                            return x.fieldName === fieldName
                        });
                        if (requiredFieldIndex !== -1 && hasErrorBefore === -1) {
                            //không để trống trường nào đã quy định
                            if ((!propValue || propValue == 'Invalid date' || !isValidDate(propValue)) && fieldName != 'OTProposalHour' && fieldName != 'detailReasonOfOT' && fieldName != 'fromActual' && fieldName != 'toActual' && fieldName != 'actualOTHour') {
                                errors.push({
                                    fieldName: fieldName,
                                    controlName: $scope.validateFields[requiredFieldIndex].title,
                                    errorDetail: $translate.instant('COMMON_FIELD_IS_REQUIRED'),
                                    isRule: false,
                                });
                            } else {
                                //Check không cho giờ bắt đầu lớn hơn giờ kết thúc
                                switch (fieldName) {
                                    case 'detailReasonOfOT':
                                        if (!propValue && item['reasonOfOT'] == 5) {
                                            errors.push({
                                                fieldName: fieldName,
                                                controlName: $scope.validateFields[requiredFieldIndex].title,
                                                errorDetail: $translate.instant('OVERTIME_APPLICATION_EMPTY_VALIDATE'),
                                                isRule: false,
                                            });
                                        }
                                        break;
                                    case 'OTProposalHour':
                                        if (propValue == 0) {
                                            errors.push({
                                                fieldName: fieldName,
                                                controlName: $scope.validateFields[requiredFieldIndex].title,
                                                errorDetail: $translate.instant('OVERTIME_APPLICATION_HOUR_0'),
                                                isRule: true
                                            });
                                        }
                                        break;

                                    case 'fromActual':
                                        /*if (propValue > item['toActual'] && ((propValue > "12:00" && item['toActual'] > "12:00") || ((propValue < "12:00" && item['toActual'] < "12:00"))) && item['toActual'] != '00:00') {
                                            errors.push({
                                                fieldName: fieldName,
                                                controlName: $translate.instant('OVERTIME_APPLICATION_OT_ACTUAL_FROM'),
                                                errorDetail: $translate.instant('OVERTIME_APPLICATION_ACTUAL_GREATER_TO'),
                                                isRule: true
                                            });
                                        }*/
                                        break;
                                }
                            }
                        }
                        if (fieldName == 'detailReasonOfOT' && item['reasonOfOT'] === $scope.reasonOtherCode && !item.detailReasonOfOT) {
                            errors.push({
                                fieldName: 'detailReasonOfOT',
                                controlName: $translate.instant('COMMON_OTHER_REASON'),
                                errorDetail: $translate.instant('COMMON_FIELD_IS_REQUIRED'),
                                isRule: false
                            });
                        }
                    });
                }
                if (!errors.length) {
                    currentOvertimeList = currentOvertimeList.map(function (item) { return { ...item, date: moment(item.dateOfOT, "DD/MM/YYYY").toDate(), sapCode: item.sapCode } });
                    errors = validateSalaryRangeDate($translate, currentOvertimeList, appSetting.salaryRangeDate, 'OVERTIME_TIMECLOCK_MENU', 'OVERTIME_APPLICATION_PERIOD_VALIDATE_2', false);
                    if (errors.length) {
                        errors = errors.map(function (item) { return { ...item, errorDetail: item.message, isRule: true } });
                    }
                }
                //Check nếu có bản ghi trùng ngày
                let groupedOvertimeList = [];
                if ($scope.model.type == commonData.OverTimeType.EmployeeSeftService) {
                    groupedOvertimeList = Object.values(_.groupBy(currentOvertimeList, function (time) {
                        return time.dateOfOT;

                    }));
                }
                if ($scope.model.type == commonData.OverTimeType.ManagerApplyForEmployee) {
                    groupedOvertimeList = Object.values(_.groupBy(currentOvertimeList.map(i => ({ ...i, group: i.dateOfOT + i.sapCode })), 'group'));
                }

                groupedOvertimeList.forEach(function (groupedDate) {
                    //let totalHourInDay = 0;
                    if (groupedDate.length > 1) {
                        errors.push({
                            isRule: true,
                            fieldName: 'dateOfOT',
                            controlName: $translate.instant('OVERTIME_APPLICATION_DATE_OF_OT') + '-' + groupedDate[0].sapCode,
                            errorDetail: ':' + groupedDate[0].dateOfOT + ' ' + $translate.instant('OVERTIME_TIMECLOCK_DUPLICATE_VALIDATE'),
                        })
                    }
                });

            } else {
                errors.push({
                    fieldName: 'List overtime application',
                    controlName: $translate.instant('OVERTIME_APPLICATION_TABLE'),
                    errorDetail: $translate.instant('COMMON_FIELD_IS_REQUIRED'),
                    isRule: false,
                });
            }

            if ($scope.model.type == commonData.OverTimeType.ManagerApplyForEmployee) {
                if (!$scope.model.reasonCode) {
                    errors.push({
                        fieldName: 'reasonCode',
                        controlName: $translate.instant('OVERTIME_APPLICATION_REASON_REQUIRED'),
                        errorDetail: $translate.instant('COMMON_FIELD_IS_REQUIRED'),
                    });
                }
                let reasonName = findReasonName($scope.model.reasonCode)
                if (reasonName && reasonName.toLowerCase().includes("Other Reason".toLowerCase()) && !$scope.model.contentOfOtherReason) {
                    errors.push({
                        fieldName: 'contentOfOtherReason',
                        controlName: $translate.instant('OVERTIME_APPLICATION_CONTENT_OF_OT'),
                        errorDetail: $translate.instant('COMMON_FIELD_IS_REQUIRED'),
                        isRule: false
                    });
                }
            }

            return errors;
        }

        function mapOTHour(from, to) {
            let startTime = moment(from, "HH:mm");
            let endTime = moment(to, "HH:mm");
            if (to == "00:00") {
                endTime = moment($scope.temporaryTo, "HH:mm");
            }
            let duration = moment.duration(endTime.diff(startTime));
            let hours = duration.asHours();
            if (hours < 0) {
                hours = hours + 24;
            }
            //Làm tròn xuống 15 phút
            //return result = (Math.floor(hours / 0.25) * 0.25).toFixed(2);
            let result = (Math.floor(hours / 0.25) * 0.25).toFixed(2);
            if (isStoreCurrentUser == false && result > 4) {
                result = (result - 1).toFixed(2);
            }
            return result;
        }

        function mapOTHourV2(isStore, from, to) {
            let startTime = moment(from, "HH:mm");
            let endTime = moment(to, "HH:mm");
            if (to == "00:00") {
                endTime = moment($scope.temporaryTo, "HH:mm");
            }
            let duration = moment.duration(endTime.diff(startTime));
            let hours = duration.asHours();
            if (hours < 0) {
                hours = hours + 24;
            }
            //Làm tròn xuống 15 phút
            //return result = (Math.floor(hours / 0.25) * 0.25).toFixed(2);
            let result = (Math.floor(hours / 0.25) * 0.25).toFixed(2);
            if (isStore == false && result > 4) {
                result = (result - 1).toFixed(2);
            }
            return result;
        }

        $scope.caculatePOTHour = function (index) {
            //$scope.form.overtimeList.forEach(x => {
            //    x.fromActual = x.from;
            //    x.toActual = x.to;
            //});
            $scope.caculateOTHour(index);
        }

        $scope.caculateOTHour = function (index) {
            let currentRow = $scope.form.overtimeList[index];
            // start time and end time
            let startTime = moment(currentRow.from, "HH:mm");
            let endTime = moment(currentRow.to, "HH:mm");
            if (currentRow.to == "00:00") {
                endTime = moment($scope.temporaryTo, "HH:mm");
            }
            // let endTime = moment(currentRow.to, "HH:mm");
            let duration = moment.duration(endTime.diff(startTime));
            let hours = duration.asHours();
            // check trường hop ngan viet ne
            if (hours < 0) {
                hours = hours + 24;
            }
            //Làm tròn xuống 15 phút
            let proposalHour = (Math.floor(hours / 0.25) * 0.25).toFixed(2);
            // duration in minutes
            //var minutes = parseInt(duration.asMinutes())%60;
            startTime = moment(currentRow.fromActual, "HH:mm");
            endTime = moment(currentRow.toActual, "HH:mm");
            if (currentRow.toActual == "00:00") {
                endTime = moment($scope.temporaryTo, "HH:mm");
            }
            duration = moment.duration(endTime.diff(startTime));
            let hoursActual = duration.asHours();
            if (hoursActual < 0) {
                hoursActual = hoursActual + 24;
            }
            let actualHour = (Math.floor(hoursActual / 0.25) * 0.25).toFixed(2);
            //Chỉ lấy giá trị lớn hơn 30 phút
            $scope.form.overtimeList[index].OTProposalHour = proposalHour >= 0.50 ? proposalHour : 0;
            $scope.form.overtimeList[index].actualOTHour = actualHour >= 0.50 ? actualHour : '';
            /*if (isStoreCurrentUser == false && $scope.form.overtimeList[index].OTProposalHour >= 5) {
                $scope.form.overtimeList[index].OTProposalHour = ($scope.form.overtimeList[index].OTProposalHour - 1).toFixed(2);
            }*/
            if ($scope.model && $scope.model.type) {
                if ($scope.model.type == 1) {
                    // ma hinh current user
                    if ((isStoreCurrentUser == false || $scope.form.overtimeList[index].isStore == false) && $scope.form.overtimeList[index].OTProposalHour > 4) {
                        $scope.form.overtimeList[index].OTProposalHour = ($scope.form.overtimeList[index].OTProposalHour - 1).toFixed(2);
                    }

                    if ((isStoreCurrentUser == false || $scope.form.overtimeList[index].isStore == false) && $scope.form.overtimeList[index].actualOTHour != '' && $scope.form.overtimeList[index].actualOTHour > 4) {
                        $scope.form.overtimeList[index].actualOTHour = ($scope.form.overtimeList[index].actualOTHour - 1).toFixed(2);
                    }
                } else if ($scope.model.type == 2) {
                    // man hinh manage
                    if ($scope.form.overtimeList[index].isStore == false && $scope.form.overtimeList[index].OTProposalHour > 4) {
                        $scope.form.overtimeList[index].OTProposalHour = ($scope.form.overtimeList[index].OTProposalHour - 1).toFixed(2);
                    }

                    if ((isStoreCurrentUser == false || $scope.form.overtimeList[index].isStore == false) && $scope.form.overtimeList[index].actualOTHour != '' && $scope.form.overtimeList[index].actualOTHour > 4) {
                        $scope.form.overtimeList[index].actualOTHour = ($scope.form.overtimeList[index].actualOTHour - 1).toFixed(2);
                    }
                }
            } else {
                // old
                if (isStoreCurrentUser == false && $scope.form.overtimeList[index].OTProposalHour >= 5) {
                    $scope.form.overtimeList[index].OTProposalHour = ($scope.form.overtimeList[index].OTProposalHour - 1).toFixed(2);
                }

                if (isStoreCurrentUser == false  && $scope.form.overtimeList[index].actualOTHour != '' && $scope.form.overtimeList[index].actualOTHour >= 5) {
                    $scope.form.overtimeList[index].actualOTHour = ($scope.form.overtimeList[index].actualOTHour - 1).toFixed(2);
                }
            }
            //$scope.form.overtimeList[index].actualOTHour = actualHour >= 0.50 ? actualHour : 0;
            //$scope.form.overtimeList[index].actualOTHour = actualHour >= 0.50 ? actualHour : '';

            $scope.onchangePrevDay(index);
        }

        function checkPrevDay(from, to) {
            if (from != null && from != '' && to != null && to != '') {
                let startTime = moment(from, "HH:mm");
                let endTime = moment(to, "HH:mm");
                if (startTime > endTime) {
                    _data.isPrevDay = true;
                } else {
                    _data.isPrevDay = false;
                }
            }
        }

        $scope.onchangePrevDay = function (index) {
            var currentRow = $scope.form.overtimeList[index];
            if (currentRow.fromActual != null && currentRow.fromActual != '' && currentRow.toActual != null && currentRow.toActual != '') {
                if (currentRow.toActual < currentRow.fromActual) {
                    $scope.form.overtimeList[index].isPrevDay = true;
                } else {
                    $scope.form.overtimeList[index].isPrevDay = false;
                }
            } else {
                var currentRow = $scope.form.overtimeList[index];
                if (currentRow.from != null && currentRow.from != '' && currentRow.to != null && currentRow.to != '') {
                    if (currentRow.to < currentRow.from) {
                        $scope.form.overtimeList[index].isPrevDay = true;
                    } else {
                        $scope.form.overtimeList[index].isPrevDay = false;
                    }
                }
            }
        }


        $scope.mouseLeaveTimeInput = function (item, fieldName) {
            let isValidTime = moment(item[fieldName], 'HH:mm', true).isValid();
            let currentTime = moment(new Date()).format('HH:mm');
            if (!isValidTime) {
                item[fieldName] = currentTime;
            }
        }
        $scope.selectedsapCodeChange = function () {
            for (let i in $scope.data) {
                let newInfo = $scope.data[i];
                if (newInfo.sapCode == $scope.form.sapCode) {
                    $scope.form.fullName = newInfo.fullName;
                    $scope.form.dept_Line = newInfo.dept_Line;
                    $scope.form.division_Group = newInfo.division_Group;
                    $scope.form.workLocation = newInfo.workLocation;
                    break;
                }
            }
        }

        function loadPageOne() {
            let grid = $("#gridRequests").data("kendoGrid");
            if (grid) {
                grid.dataSource.fetch(() => grid.dataSource.page(1));
            }
        }

        $scope.applySearch = function () {
            // getAllOvertime();
            loadPageOne();
            $scope.toggleFilterPanel(false);
        }

        $scope.clearSearch = function () {
            $scope.query = {
                keyword: '',
                userSAPCode: '',
                departmentCode: '',
                fromDate: null,
                toDate: null
            }
            $scope.$broadcast('resetToDate', $scope.query.toDate);
            // getAllOvertime($scope.optionSave);
            loadPageOne();
        }

        // $scope.resetSearch = function () {
        //     $scope.model.keyword = "";
        //     $scope.model.createdDateFrom = null;
        //     $scope.model.createdDateFromString = null;
        //     $scope.model.createdDateToString = null;
        //     $scope.model.createdDateTo = null;
        //     $scope.model.departmentId = "";
        //     $scope.model.statusIds = [];
        // }    
        function approve() {
            // value thì lấy trong biến $rootScope.reasonObject.comment
            // nếu thành công thì return về true để ẩn modal đi
            Notification.success("Approve overtime application");
            return true;
        }

        function requestedToChange() {
            // value thì lấy trong biến $rootScope.reasonObject.comment
            // nếu thành công thì return về true để ẩn modal đi
            Notification.success("RequestedToChange overtime application");
            return true;
        }

        function reject() {
            // value thì lấy trong biến $rootScope.reasonObject.comment
            // nếu thành công thì return về true để ẩn modal đi
            Notification.success("Reject overtime application");
            return true;
        }

        $scope.statusCompleted = false;
        valueReasonCode = '';
        valueReasonCodeTabManager = '';
        async function getOvertimeDetail(id) {
            var result = await cbService.getInstance().overtimeApplication.getOvertimeApplication({ id: id }).$promise;
            if (result.isSuccess) {
				$scope.model = _.cloneDeep(result.object);
                $timeout(async function () {
                    $scope.statusTranslate = $rootScope.getStatusTranslate($scope.model.status);
                    $scope.model.moduleTitle = 'Overtime';
                    if ($scope.model && $scope.model.status.toLowerCase() == "Waiting For Fill Actual Hour".toLowerCase()) {
                        if (!$scope.model.timeInRound) {
                            $scope.model.timeInRound = $scope.TimeInRound;
                        }
                        if (!$scope.model.timeOutRound) {
                            $scope.model.timeOutRound = $scope.TimeOutRound;
                        }
                    }
                    //await getSapCodeByDivison(1, $scope.limitDefaultGrid);    
                    if ($scope.model.divisionCode || $scope.model.deptCode) {
                        var res = await settingService.getInstance().users.checkUserIsStore({ departmentCode: $scope.model.divisionCode ? $scope.model.divisionCode : $scope.model.deptCode }, null).$promise;
                        if (res && res.isSuccess) {
                            isStoreCurrentUser = res.object;
                        }
                    }
                    if ($stateParams.type === commonData.stateOvertime.item) {
                        //result.object.overtimeItems = _.orderBy(result.object.overtimeItems, ['dateOfOT'], ['asc']);
                        $scope.overtimeDetailItems = result.object.overtimeItems;
                        $scope.model.overtimeDetailItems = result.object.overtimeItems;
                        $scope.warning = '';
                        $scope.dateOfOT = [];
                        $scope.overtimeDetailItems.forEach(element => {
                            if ((moment(element.date)).diff(moment(result.object.created), 'days') < 0) {
                                let item = moment(element.date).format('DD/MM/YYYY');
                                if ($scope.dateOfOT.length && _.findIndex($scope.dateOfOT, x => { return x == item }) == -1) {
                                    $scope.dateOfOT.push(item);
                                }

                            }
                        });
                        //ngan
                        if ($scope.overtimeDetailItems.length) {
                            for (var i = 0; i < $scope.overtimeDetailItems.length; i++) {
                                if ($scope.overtimeDetailItems[i].reasonCode) {
                                    let index = _.findIndex($scope.reasons, x => {
                                        return x.code == $scope.overtimeDetailItems[i].reasonCode;
                                    });
                                    if (index == -1) {
                                        $scope.reasons.push({ code: $scope.overtimeDetailItems[i].reasonCode, name: $scope.overtimeDetailItems[i].reasonName });
                                    }
                                    valueReasonCode = $scope.overtimeDetailItems[i].reasonCode;
                                    setDataDropdownList("#reasonDropdown", $scope.reasons, valueReasonCode);
                                }
                            }
                        }

                        if ($scope.model.reasonCode) {
                            let index = _.findIndex($scope.reasons, x => {
                                return x.code == $scope.model.reasonCode;
                            });
                            if (index == -1) {
                                $scope.reasons.push({ code: $scope.model.reasonCode, name: $scope.model.reasonName });
                            }
                            valueReasonCodeTabManager = $scope.model.reasonCode;
                            setDataDropdownList("#reasonDropdownManager", $scope.reasons, valueReasonCodeTabManager);
                        }
                        //end

                        $scope.warning = $translate.instant('OVERTIME_APPLICATION_WARNING') + ' ' + $scope.dateOfOT.join(', ');

                        result.object.overtimeItems.forEach(item => {
                            item['id'] = item.overtimeApplicationId;
                            item['dateOfOT'] = moment.utc(item.date).utcOffset(7).format('DD/MM/YYYY');//moment(item.date).format('DD/MM/YYYY');
                            item['reasonOfOT'] = item.reasonCode;
                            item['dayOffInLieu'] = item.dateOffInLieu;
                            item['isNoOT'] = item.isNoOT;
                            item['isPrevDay'] = item.isPrevDay;
                            item['from'] = item.proposalHoursFrom;
                            item['to'] = item.proposalHoursTo;
                            item['fromActual'] = item.actualHoursFrom;
                            item['toActual'] = item.actualHoursTo;
                            item['detailReasonOfOT'] = item.detailReason;
                            item['OTProposalHour'] = mapOTHourV2(item.isStore, item.proposalHoursFrom, item.proposalHoursTo);
                            item['actualOTHour'] = item.actualHoursFrom && item.actualHoursTo ? mapOTHourV2(item.isStore, item.actualHoursFrom, item.actualHoursTo) : '';
                            item['userId'] = item.userId;
                            item['isStore'] = item.isStore;
                        });
                        $scope.form.overtimeList = result.object.overtimeItems;
                        $scope.warning = '';
                        $scope.dateOfOT = [];
                        //$scope.form.overtimeList = _.orderBy($scope.form.overtimeList, ['dateOfOT'], ['asc']);
                        $scope.form.overtimeList.forEach(element => {
                            if ((moment(element.date)).diff(moment(result.object.created), 'days') < 0) {
                                let item = moment(element.date).format('DD/MM/YYYY');
                                if ($scope.dateOfOT.length && _.findIndex($scope.dateOfOT, x => { return x == item }) == -1) {
                                    $scope.dateOfOT.push(item);
                                }
                            }
                            if ($scope.model && $scope.model.status.toLowerCase() == "Waiting For Fill Actual Hour".toLowerCase()) {
                                // SET Actual Hour BY TIMES CONFIG
                                if (element.fromActual) {
                                    element.fromActual = setTimeByTimeInRound(element.fromActual, $scope.model.timeInRound);
                                }
                                if (element.toActual) {
                                    element.toActual = setTimeByTimeOutRound(element.toActual, $scope.model.timeOutRound);
                                }
                            }
                            if (element.fromActual && element.toActual) {
                                element.actualOTHour = mapOTHourV2(element.isStore, element.fromActual, element.toActual);
                            }
                        });
                        $scope.warning = $translate.instant('OVERTIME_APPLICATION_WARNING') + ' ' + $scope.dateOfOT.join(', ');
                        if (result.object.status == 'Completed') {
                            $scope.statusCompleted = true;
                        }
                        $timeout(function () {
                            var noOtCheckBox = $(".noOTCB:checked");
                            noOtCheckBox.trigger('click');
                        }, 800);
                    }
                    if ($scope.model.type == commonData.OverTimeType.EmployeeSeftService) {
                        $scope.type = 'Employee';
                        $scope.disableManager = true;
                        var tabToActivate = $("#tab1");
                        $("#tabstrip").kendoTabStrip().data("kendoTabStrip").activateTab(tabToActivate);
                    }

                    if ($scope.model.type == commonData.OverTimeType.ManagerApplyForEmployee) {
                        $scope.type = 'Manager';
                        $scope.disableEmployee = true;
                        if ($scope.model.applyDate && $scope.model.applyDate != 'Invalid Date') {
                            $scope.model.applyDate = new Date($scope.model.applyDate);
                            if ($scope.model.applyDate.getTimezoneOffset() * -1 != 420) {
                                $scope.model.applyDate.setMinutes($scope.model.applyDate.getMinutes() + 420 - $scope.model.applyDate.getTimezoneOffset() * -1);
                            }
                        } else {
                            let applyDatePicker = $('#apply-date').data('kendoDatePicker');
                            if (applyDatePicker) {
                                applyDatePicker.value(null);
                            }
                        }
                        //them dieu kien cho validate
                        $scope.validateFields.pop();
                        $scope.validateFields.push(
                            {
                                fieldName: 'userId',
                                title: $translate.instant('COMMON_SAP_CODE'),
                            }
                        );
                        var tabToActivate = $("#tab2");
                        $("#tabstrip").kendoTabStrip().data("kendoTabStrip").activateTab(tabToActivate);
                    }
                }, 0);
                $scope.model.type = result.object.type;
                if ($stateParams.type === commonData.stateOvertime.approve) {
                    result.object.overtimeItems.forEach(item => {
                        item.date = new Date(item.date);
                        item['OTProposalHour'] = mapOTHourV2(item.isStore, item.proposalHoursFrom, item.proposalHoursTo);
                        item['actualOTHour'] = mapOTHourV2(item.isStore, item.actualHoursFrom, item.actualHoursTo);
                    });
                    // initOvertimeReadListGrid(result.object.overtimeItems);
                }
            }
        }

        //function initOvertimeReadListGrid(data) {
        //    let grid = $("#overtimeReadListGrid").data("kendoGrid");
        //    let dataSource = new kendo.data.DataSource({
        //        data: data
        //    });
        //    grid.setDataSource(dataSource);
        //}

        async function getUserById(userid) {
            var model = {
                userId: userid
            }

            if (userid) {
                var result = await settingService.getInstance().users.getUserById(model).$promise;
                if (result.isSuccess) {
                    return result.object;
                }
            }
        }

        function buildArgs(pageIndex, pageSize) {
            let conditions = [
                'ReferenceNumber.contains(@{i})',
                'userSAPCode.contains(@{i})',
                'createdByFullName.contains(@{i})',
                'deptName.contains(@{i})',
                'divisionName.contains(@{i})',
                'status.contains(@{i})',
                //'workLocation.contains(@{i})'
            ];
            let stateArgs = {
                currentState: $stateParams.type,
                myRequestState: commonData.stateOvertime.myRequests,
                currentUserId: $rootScope.currentUser.id
            };

            return createQueryArgsForCAB({ pageIndex, pageSize, order: appSetting.ORDER_GRID_DEFAULT }, conditions, $scope.query, stateArgs);

        }

        $scope.total = 0;
        $scope.data = [];
        $scope.currentQuery = {
            predicate: "",
            predicateParameters: [],
            Order: "Created desc",
            Limit: appSetting.pageSizeDefault,
            Page: 1
        }
        async function getAllOvertime(option) {
            $scope.currentQuery = {
                predicate: "",
                predicateParameters: [],
                Order: "Created desc",
                Limit: appSetting.pageSizeDefault,
                Page: 1
            }

            let args = buildArgs($scope.currentQuery.Page, appSetting.pageSizeDefault);
            if ($scope.query.status && $scope.query.status.length) {
                generatePredicateWithStatus(args, $scope.query.status);
            }

            if ($scope.query.userSAPCode) {
                if (args.predicateParameters.length) {
                    args.predicate += ` and `;
                }
                args.predicate += `((OvertimeItems.any(x => x.User.SAPCode.contains(@` + args.predicateParameters.length + `)) and Type == 2) or (Type == 1 and UserSAPCode.contains(@` + args.predicateParameters.length +`)))`;
                args.predicateParameters.push($scope.query.userSAPCode);
            }


            if (option) {
                $scope.currentQuery.Limit = option.data.take;
                $scope.currentQuery.Page = option.data.page;
            }
            $scope.currentQuery.predicate = args.predicate;
            $scope.currentQuery.predicateParameters = args.predicateParameters;

            // if ($state.current.name == commonData.myRequests.OvertimeApplication) {
            //     $scope.currentQuery.predicate = 'CreatedById = @0'; 
            // }

            //get limit in grid 
            let grid = $("#gridRequests").data("kendoGrid");
            $scope.currentQuery.Limit = grid.pager.dataSource._pageSize
            //

            var result = await cbService.getInstance().overtimeApplication.getOvertimeApplicationList($scope.currentQuery).$promise;
            if (result.isSuccess) {
                $scope.data = result.object.data;
                if ($scope.data.length !== 0) {
                    $scope.data.forEach(item => {
                        item.created = new Date(item.created);
                        if (item.mapping) {
                            if (item.mapping.userJobGradeGrade >= 5) {
                                item['userDeptName'] = item.mapping.departmentName;
                            } else {
                                item['userDivisionName'] = item.mapping.departmentName;
                            }
                        }
                    });
                }
                $scope.total = result.object.count;
            }
            if (option) {
                option.success($scope.data);
            } else {
                // $scope.model.overtimeApplicationGridOptions.dataSource.read();
                initGridRequests($scope.data, $scope.total, $scope.currentQuery.Page, $scope.currentQuery.Limit);
                // let grid = $("#grid").data("kendoGrid");
                // grid.dataSource.fetch();
            }
        }

        // function getAllOvertime(pageIndex, option) {

        //     let args = buildArgs(pageIndex, appSetting.pageSizeDefault);
        //     if ($scope.searchInfo.status && $scope.searchInfo.status.length) {
        //         generatePredicateWithStatus(args, $scope.searchInfo.status);
        //     }
        //     cbService.getInstance().overtimeApplication.getOvertimeApplicationList(args).$promise.then(function (result) {
        //         if (result.isSuccess) {
        //             $scope.data = result.object.data;
        //             if ($scope.data.length !== 0) {
        //                 $scope.data.forEach(item => {
        //                     item.created = new Date(item.created);
        //                     if (item.mapping) {
        //                         if (item.mapping.userJobGradeGrade >= 5) {
        //                             item['userDeptName'] = item.mapping.departmentName;
        //                         } else {
        //                             item['userDivisionName'] = item.mapping.departmentName;
        //                         }
        //                     }
        //                 });
        //             }
        //             initGridRequests($scope.data, result.object.count, pageIndex);
        //         }
        //     });
        // }

        function initGridRequests(dataSource, total, pageIndex, pageSize) {
            let grid = $("#gridRequests").data("kendoGrid");
            let dataSourceRequests = new kendo.data.DataSource({
                data: dataSource,
                // pageSize: appSetting.pageSizeDefault,
                pageSize: pageSize,
                page: pageIndex,
                schema: {
                    total: function () {
                        return total;
                    }
                },
            });
            grid.setDataSource(dataSourceRequests);
        }

        // phần khai báo chung
        $scope.actions = {
            approve: approve,
            requestedToChange: requestedToChange,
            reject: reject
        };

        $scope.user = {
            sapCode: '',
            fullName: '',
            deptLine: '',
            divisionGroup: '',
            workLocation: ''
        }

        $scope.getSapCodeByDivison = async function () {
            if ($scope.model.divisionId) {
                let result = await settingService.getInstance().users.getChildUsers({ departmentId: $scope.model.divisionId, isAll: true }).$promise;
                if (result.isSuccess) {
                    $scope.employeesDataSource = result.object.data.map(function (item) {
                        return { ...item, showtextCode: item.sapCode }
                    });
                    //setDataDeptSAPCode($scope.employeesDataSource);
                }
            } else {
                if ($rootScope.currentUser.deptId) {
                    // nếu chọn division thì lấy những sapcode có trong divs đó
                    if ($scope.model.divisionId) {
                        let result = await settingService.getInstance().users.getChildUsers({ departmentId: $rootScope.currentUser.deptId, isAll: true }).$promise;
                        if (result.isSuccess) {
                            $scope.employeesDataSource = result.object.data.map(function (item) {
                                return { ...item, showtextCode: item.sapCode }
                            });
                        }
                    } // nếu ko thì select hết những sap code nằm trong những divs trong dropdown
                    else { // nếu không chọn devision thì lấy user của toàn bộ dept/line và của toàn bộ division con
                        let result = await settingService.getInstance().users.getUsersByOnlyDeptLine({ depLineId: $rootScope.currentUser.deptId }).$promise;
                        if (result.isSuccess) {
                            $scope.employeesDataSource = result.object.data.map(function (item) {
                                return { ...item, showtextCode: item.sapCode }
                            });
                        }
                    }
                }
            }
            $scope.sapCodeOptions.dataSource = $scope.employeesDataSource;
        }


        async function getSapCodeByDivison(page, limit, searchText = "") {
            if ($scope.model.divisionId) {
                let result = null;
                if ($scope.model.divisionId === '00000000-0000-0000-0000-000000000000') {
                    result = await settingService.getInstance().users.getUserCheckedHeadCount({ departmentId: $rootScope.currentUser.deptId, textSearch: $scope.gridUser.keyword }).$promise;
                }
                else {
                    result = await settingService.getInstance().users.getChildUsers({ departmentId: $scope.model.divisionId, limit: $scope.limitDefaultGrid, page: 1, searchText: $scope.gridUser.keyword, isAll: true }).$promise;
                }
                //let result = await settingService.getInstance().users.getChildUsers({ departmentId: $scope.model.divisionId, limit: limit, page: page, searchText: searchText, isAll: true }).$promise;
                if (result.isSuccess) {
                    $scope.employeesDataSource = result.object.data.map(function (item) {
                        return { ...item, showtextCode: item.sapCode }
                    });
                    $scope.total = result.object.count;
                    //setDataDeptSAPCode($scope.employeesDataSource);
                }
            }

            $scope.sapCodeOptions.dataSource = $scope.employeesDataSource;
        }
        async function getUsersByDeptLine(deptId, limit, searchText, page = 1) {
            let result = await settingService.getInstance().users.getChildUsers({ departmentId: deptId, limit: limit, page: page, searchText: searchText, isAll: true }).$promise;
            if (result.isSuccess) {
                $scope.employeesDataSource = result.object.data.map(function (item) {
                    return { ...item, showtextCode: item.sapCode }
                });
                $scope.total = result.object.count;
            }
        }

        $scope.limitDefaultGrid = 20;
        async function ngOnit() {
            $rootScope.showLoading();
            if ($state.current.name === 'home.overtimeApplication.myRequests') {
                $scope.selectedTab = "1";
            }
            else if ($state.current.name === 'home.overtimeApplication.allRequests') {
                $scope.selectedTab = "0";
            }

            if ($rootScope.currentUser) {
                /*$rootScope.blockAccess()*/
                isStoreCurrentUser = $rootScope.currentUser.isStore;
                if ($state.current.name === commonData.stateOvertime.item || $state.current.name === commonData.stateOvertime.approve) {
                    await getReasons();
                    $timeout(function () {
                        $scope.model = _.cloneDeep($rootScope.currentUser);
                        $scope.model.id = '';
                        $scope.model.userSAPCode = $rootScope.currentUser.sapCode;
                        $scope.model.createdByFullName = $rootScope.currentUser.fullName;
                        $scope.model.startingDate = $rootScope.currentUser.startDate ? $rootScope.currentUser.startDate : null;
                        // $scope.model.deptName = $scope.model.deptName && $scope.model.jobGradeCode ? $scope.model.deptName + "(" + $scope.model.jobGradeCode + ")" : $scope.model.deptName ? $scope.model.deptName : '';
                        $scope.model.deptName = $scope.model.deptName ? $scope.model.deptName : '';
                        $scope.model.divisionName = $scope.model.divisionName ? $scope.model.divisionName : '';
                       /* if (mode == 'New') {
                            $scope.type = 'Employee';
                            $scope.model.type = commonData.OverTimeType.EmployeeSeftService;
                            $scope.form.overtimeList = [{
                                userId: '',
                                fullName: '',
                                dateOfOT: null,
                                reasonOfOT: '',
                                OTProposalHour: 0,
                                from: '',
                                to: '',
                                actualOTHour: '',
                                fromActual: '',
                                toActual: '',
                                dayOffInLieu: false,
                                isPrevDay: false,
                                isNoOT: false,
                            }];
                            if ($rootScope.currentUser.jobGradeValue == 1) {
                                $scope.disableManager = true;
                            }

                            $scope.currentInstanceProcessingStage = null;
                        }*/
                    }, 0);
                    //check xem user có dept/line ko 
                    await loadHolidays();
                    if ($rootScope.currentUser.deptName && !$rootScope.currentUser.divisionId) {
                        $scope.model.divisionId = null;
                        $scope.form.dept_Line = $rootScope.currentUser.deptId;
                        getDeptLine();
                        await getDepartmentByUserId($rootScope.currentUser.id, $rootScope.currentUser.deptId, $rootScope.currentUser.divisionId);
                        await getDeptLineByDeptName($rootScope.currentUser.deptName);
                        //await $scope.getChildDivison();
                    } else {
                        await getDepartmentByUserId($rootScope.currentUser.id, $rootScope.currentUser.deptId, $rootScope.currentUser.divisionId);
                        await getJobGradeG5();
                        //await getDeptDivision();
                    }
                    //await $scope.getSapCodeByDivison();

                    if (mode == 'New') {
                        $scope.type = 'Employee';
                        $scope.model.type = commonData.OverTimeType.EmployeeSeftService;
                        $scope.form.overtimeList = [{
                            userId: '',
                            fullName: '',
                            dateOfOT: null,
                            reasonOfOT: '',
                            OTProposalHour: 0,
                            from: '',
                            to: '',
                            actualOTHour: '',
                            fromActual: '',
                            toActual: '',
                            dayOffInLieu: false,
                            isPrevDay: false,
                            isNoOT: false,
                        }];
                        if ($rootScope.currentUser.jobGradeValue == 1) {
                            $scope.disableManager = true;
                        }
                        $scope.currentInstanceProcessingStage = null;
                    }
                    else if (mode == 'Edit') {
                        await getOvertimeDetail($stateParams.id);
                        await getDivisionsByDeptLine($rootScope.currentUser.deptId, $scope.model.divisionId);
						if ($scope.model?.id != null) {
							await getDepartmentByReferenceNumber($stateParams.referenceValue, $stateParams.id, $scope.model.divisionId);
						} else {
							await getDepartmentByReferenceNumber($stateParams.referenceValue, $stateParams.id);
						}
                        //await getDivisionsByDeptLine($rootScope.currentUser.deptId, $rootScope.currentUser.divisionId);
                        await getWorkflowProcessingStage($stateParams.id);
                    }
                }
            }
            var currentDate = new Date();
            $scope.localtimezone = -1 * currentDate.getTimezoneOffset();

            setTimeout(function () {
                document.querySelectorAll('.tooltip-custom').forEach((tooltip) => {
                    const tooltipText = tooltip.querySelector('.tooltip-text');
                    const parentRect = document.querySelector('.portlet-body').getBoundingClientRect();
                    tooltip.addEventListener('mouseenter', function () {
                        const tooltipRect = tooltipText.getBoundingClientRect();
                        if (tooltipRect.right > parentRect.right) {
                            tooltip.classList.add('tooltip-left');
                        }
                    });
                });
            }, 10);

            $rootScope.hideLoading();
        }
        
        $scope.onTabChange = function () {
            localStorage.setItem('selectedTab', $scope.selectedTab);
            if ($scope.selectedTab === "1") {
                $state.go('home.overtimeApplication.myRequests');
            } else if ($scope.selectedTab === "0") {
                $state.go('home.overtimeApplication.allRequests');
            }
        };
        async function getWorkflowProcessingStage(itemId) {
            var result = await workflowService.getInstance().workflows.getWorkflowStatus({ itemId: itemId }).$promise;
            if (result.isSuccess && result.object) {
                $scope.currentInstanceProcessingStage = result.object.workflowInstances[0];
                $scope.processingStageRound = result.object.workflowInstances.length;
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
        $scope.export = async function () {
            let args = buildArgs(appSetting.numberSheets, appSetting.numberRowPerSheets);
            if ($scope.query.status && $scope.query.status.length) {
                generatePredicateWithStatus(args, $scope.query.status);
            }
            var res = await fileService.getInstance().processingFiles.export({
                type: commonData.exportType.OVERTIME
            }, args).$promise;



            if (res.isSuccess) {
                exportToExcelFile(res.object);
                Notification.success(appSetting.notificationExport.success);
            } else {
                Notification.error(appSetting.notificationExport.error);
            }
        }
        $scope.exportOT = async function () {
            let args = buildArgs(appSetting.numberSheets, appSetting.numberRowPerSheets);
            if ($scope.query.status && $scope.query.status.length) {
                generatePredicateWithStatus(args, $scope.query.status);
            }
            var res = await fileService.getInstance().processingFiles.export({
                type: commonData.exportType.OVERTIME_FILL_ACTUAL_DETAILS,
            }, { "Predicate": $scope.model.referenceNumber }).$promise;



            if (res.isSuccess) {
                exportToExcelFile(res.object);
                Notification.success(appSetting.notificationExport.success);
            } else {
                Notification.error(appSetting.notificationExport.error);
            }
        }
        $scope.showProcessingStages = function () {
            $rootScope.visibleProcessingStages($translate);
        }
        $scope.showTrackingHistory = function () {
            $rootScope.visibleTrackingHistory($translate, appSetting.TrackingLogDialogDefaultWidth);
        }
        ngOnit();

        $scope.stripOptions = {
            animation: false,
        }
        $scope.dataTemporaryDivision = [];
        $scope.divisionOptions = {
            dataValueField: "id",
            dataTextField: "name",
            dataSource: $scope.deptDivision,
            valuePrimitive: true,
            checkboxes: false,
            autoBind: true,
            filter: "contains",
            filtering: async function (option) {
                await getDeptDivisionByFilter(option);
            },
            template: function (dataItem) {
                //return `<span class="${dataItem.item.jobGradeGrade > 4 ? 'k-state-disabled' : ''}">${showCustomDepartmentTitle(dataItem)}</span>`;
                return `<span class="${dataItem.item.type == 2 ? 'k-state-disabled' : ''}">${showCustomDepartmentTitle(dataItem)}</span>`;
            },
            loadOnDemand: false,
            valueTemplate: (e) => showCustomField(e, ['name']),
            select: function (e) {
                let dropdownlist = $("#deptDivision").data("kendoDropDownTree");
                let dataItem = dropdownlist.dataItem(e.node)
                $scope.model.divisionId = dataItem.id;
                $scope.model.divisionName = dataItem.name;
                $scope.model.divisionCode = dataItem.code;
                // if (dataItem.jobGradeGrade > 4) {
                if (dataItem.type == 2) {
                    e.preventDefault();
                }
            },
            change: async function (e) {
                if (!e.sender.value()) {
                    clearSearchTextOnDropdownTree('deptDivision');
                    setDataDeptDivision($scope.dataTemporaryDivision);
                }
                if ($scope.form.overtimeList.length && $scope.perm) {
                    $scope.dialog = $rootScope.showConfirmDelete($translate.instant('COMMON_NOTIFY'), $translate.instant('SHIFT_EXCHANGE_NOTIFY'), $translate.instant('COMMON_BUTTON_CONFIRM'));
                    $scope.dialog.bind("close", confirmDivision);
                }
                dropdownlist.close();

            },
            dataBound: function (e) {
                $(e.sender.element).attr('autocomplete', 'off');
            }
        }
        $scope.onChangeDivision = async function (value) {
            if ($scope.form.overtimeList.length > 0 && $scope.perm && !value) {
                $timeout(function () {
                    $scope.dialog = $rootScope.showConfirmDelete($translate.instant('COMMON_NOTIFY'), $translate.instant('SHIFT_EXCHANGE_NOTIFY'), $translate.instant('COMMON_BUTTON_CONFIRM'));
                    $scope.dialog.bind("close", confirm);
                }, 0);
            }
            else {
                if (!value) {
                    kendo.ui.progress($("#loading"), true);
                    clearSearchTextOnDropdownTree('deptDivision');
                    setDataDeptDivision($scope.dataTemporaryArrayDivision);
                    await getUserCheckedHeadCount(value);
                }
                kendo.ui.progress($("#loading"), false);
            }
        }
        confirmDivision = async function (e) {
            if (e.data && e.data.value) {
                if ($scope.model.divisionId) {
                    $scope.temporaryDivision = $scope.model.divisionId;
                    $scope.form.overtimeList = [];
                    await getSapCodeByDivison(1, $scope.limitDefaultGrid);
                }
                else {
                    $scope.temporaryDivision = $scope.model.divisionId;
                    $scope.form.overtimeList = [];
                    await getSapCodeByDivison(1, $scope.limitDefaultGrid);
                }
            }
        }

        async function getDeptDivisionByFilter(option) {
            if (!option.filter) {
                option.preventDefault();
            } else {
                let res = {};
                let filter = option.filter && option.filter.value ? option.filter.value : "";
                if (filter) {
                    arg = {
                        predicate: "name.contains(@0) or code.contains(@1)",
                        predicateParameters: [filter, filter],
                        page: 1,
                        limit: appSetting.pageSizeDefault,
                        order: ""
                    }
                    res = await settingService.getInstance().departments.getDepartmentByFilter(arg).$promise;
                    if (res.isSuccess) {
                        setDataDeptDivision(res.object.data);
                    }
                } else {
                    setDataDeptDivision($scope.dataTemporaryDivision);
                }

            }
        }
        function setDataReasonOfOT(dataPosition) {
            var dataSource = new kendo.data.HierarchicalDataSource({
                data: dataPosition
            });
            var dropdownlist = $("#reasonDropdownManager").data("kendoDropDownList");
            dropdownlist.setDataSource(dataSource);
        }

        $scope.deptLine = [];
        function getDeptLine() {
            $scope.deptLine = allDepartments;
        }

        async function getDeptLineByDeptName(name) {
            let queryArgs = {
                predicate: 'Name.contains(@0)',
                predicateParameters: [name],
                order: appSetting.ORDER_GRID_DEFAULT,
                page: 1,
                limit: 10000
            };


            var result = await settingService.getInstance().departments.getDepartments(queryArgs).$promise;
            if (result.isSuccess) {
                $scope.form.deptLineId = result.object.data[0].id;
                $scope.form.deptName = result.object.data[0].name && result.object.data[0].jobGradeCaption ? result.object.data[0].name + "(" + result.object.data[0].jobGradeCaption + ")" : result.object.data[0].name ? result.object.data[0].name : '';
                $scope.$applyAsync()
            }
        }


        $scope.getChildDivison = async function () {
            if ($scope.form.deptLineId) {

                var result = await settingService.getInstance().departments.getDivisionTree({
                    departmentId: $scope.form.deptLineId
                }).$promise;
                if (result.isSuccess) {
                    $scope.dataTemporaryDivision = result.object.data;
                    setDataDeptDivision(result.object.data)
                } else {
                    Notification.error(result.messages[0]);
                }
            }
        }

        function setDataDeptDivision(dataDepartment) {
            var dataSource = new kendo.data.HierarchicalDataSource({
                data: dataDepartment,
                schema: {
                    model: {
                        children: "items"
                    }
                }
            });
            var dropdownlist = $("#deptDivision").data("kendoDropDownTree");
            if (dropdownlist) {
                dropdownlist.setDataSource(dataSource);
            }
        }

        $scope.jobGrade = '';
        async function getJobGradeG5() {
            let queryArgs = {
                predicate: 'Grade = 5',
                predicateParameters: [],
                order: appSetting.ORDER_GRID_DEFAULT,
                page: 1,
                limit: 10000
            };
            //
            var result = await settingService.getInstance().jobgrade.getJobGradeList(queryArgs).$promise;
            if (result.isSuccess) {
                $scope.jobGrade = result.object.data[0];
            }
        }

        $scope.deptDivision = [];
        async function getDeptDivision(departmentId) {
            arg = {
                predicate: `id =@${0}`,
                predicateParameters: [departmentId],
                page: 1,
                limit: appSetting.pageSizeDefault,
                order: ""
            }
            result = await settingService.getInstance().departments.getDepartmentByFilter(arg).$promise;
            if (result.isSuccess) {
                $scope.deptDivision = result.object.data;
                $scope.dataTemporaryDivision = _.filter(result.object.data, x => { return x.items.length || x.jobGradeGrade <= 4 });
                setDropDownTree('deptDivision', $scope.dataTemporaryDivision);
            }
            $timeout(function () {
                $scope.model.divisionId = $rootScope.currentUser.divisionId;
            }, 10)
        }

        $scope.employeesDataSource = [];
        $scope.sapCodeOptions = {
            //dataTextField: "showtextCode",
            dataValueField: "id",
            template: '<span><label>#: data.sapCode# - #: data.fullName# </label></span>',
            valueTemplate: '#: sapCode #',
            filter: "contains",
            dataSource: $scope.employeesDataSource,
            filtering: function (ev) {
                var filterValue = ev.filter != undefined ? ev.filter.value : "";
                ev.preventDefault();

                this.dataSource.filter({
                    logic: "or",
                    filters: [{
                        field: "name",
                        operator: "contains",
                        value: filterValue
                    },
                    {
                        field: "code",
                        operator: "contains",
                        value: filterValue
                    }
                    ]
                });
            },
        }



        $scope.changeSapCode = function (dataItem) {
            // dataItem.fullName = findFullNameForUser(dataItem.userId);
            $scope.userGrid.push(dataItem);
            let count = 1;
            $scope.userGrid.forEach(item => {
                item['no'] = count;
                count++;
            });
            setGridUser($scope.userGrid);
        }

        function findFullNameForUser(userId) {
            var result = $scope.employeesDataSource.find(x => x.id == userId);
            if (result) {
                return result.fullName;
            }
        }
        $scope.temporaryDivision = '';
        confirmDivision = async function (e) {
            if (e.data && e.data.value) {
                if ($scope.model.divisionId) {
                    $scope.temporaryDivision = $scope.model.divisionId;
                    $scope.form.overtimeList = [];
                    //await $scope.getSapCodeByDivison()
                    await getSapCodeByDivison(1, $scope.limitDefaultGrid);
                }
                else {
                    //check xem user có dept/line ko 
                    $scope.temporaryDivision = $scope.model.divisionId;
                    $scope.form.overtimeList = [];
                    if ($rootScope.currentUser.deptName && !$rootScope.currentUser.divisionId) {
                        $scope.model.divisionId = null;
                        $scope.form.dept_Line = $rootScope.currentUser.deptId;
                        //getDeptLine();
                        await getDeptLineByDeptName($rootScope.currentUser.deptName);
                        //$scope.getChildDivison();
                        //await getDeptDivision($rootScope.currentUser.deptId);
                        //await getSapCodeByDivison(1, $scope.limitDefaultGrid);
                    } else {
                        await getJobGradeG5();
                        //await getDeptDivision($rootScope.currentUser.divisionId);

                    }
                    //await $scope.getSapCodeByDivison();
                    await getSapCodeByDivison(1, $scope.limitDefaultGrid);
                }
            } else {
                //$scope.model.divisionId = $scope.temporaryDivision;
                //var dropdownlist = $("#deptDivision").data("kendoDropDownTree");
                //dropdownlist.value($scope.temporaryDivision)
            }
        }

        $scope.type = '';
        $scope.typeTemporary = '';
        $scope.tabIdGo = '';
        $scope.tabIdBack = '';
        $scope.changTab = async function (type, tabIdGo, tabIdBack) {
            if ($scope.type != type) {
                //$scope.type = type;
                $scope.typeTemporary = type;
                $scope.tabIdGo = tabIdGo;
                $scope.tabIdBack = tabIdBack;
                $scope.dialog = $rootScope.showConfirmDelete($translate.instant('COMMON_NOTIFY'), $translate.instant('OVERTIME_APPLICATION_TAB_NOTIFY'), $translate.instant('COMMON_BUTTON_CONFIRM'));
                $scope.dialog.bind("close", confirmTab);
            }

        }

        confirmTab = async function (e) {
            if (e.data && e.data.value) {
                $scope.type = $scope.typeTemporary;
                switch ($scope.type) {
                    case 'Employee':
                        $scope.form.overtimeList = [{
                            userId: '',
                            fullName: '',
                            dateOfOT: null,
                            reasonOfOT: '',
                            OTProposalHour: 0,
                            from: '',
                            to: '',
                            actualOTHour: '',
                            fromActual: '',
                            toActual: '',
                            dayOffInLieu: false,
                            isPrevDay: false,
                            isNoOT: false,
                        }]
                        $scope.errors = [];
                        //them dieu kien cho validate
                        $scope.validateFields.pop();
                        $scope.validateFields.push(
                            {
                                fieldName: 'reasonOfOT',
                                title: $translate.instant('OVERTIME_APPLICATION_REASON_REQUIRED'),
                            }
                        );
                        //
                        $scope.model.type = commonData.OverTimeType.EmployeeSeftService;
                        break;
                    case 'Manager':
                        $scope.form.overtimeList = [];
                        $scope.errors = [];
                        //them dieu kien cho validate
                        $scope.validateFields.pop();
                        $scope.validateFields.push(
                            {
                                fieldName: 'userId',
                                title: $translate.instant('COMMON_SAP_CODE'),
                            }
                        );
                        $scope.model.type = commonData.OverTimeType.ManagerApplyForEmployee;
                        let applyDatePicker = $('#apply-date').data('kendoDatePicker');
                        if (applyDatePicker) {
                            applyDatePicker.value(null);
                        }
                        if ($rootScope.currentUser) {
                            if ($rootScope.currentUser.divisionId) {
                                $scope.model.divisionId = $rootScope.currentUser.divisionId;
                                //await getDeptDivision($rootScope.currentUser.divisionId);
                                await getDepartmentByUserId($rootScope.currentUser.id, $rootScope.currentUser.deptId, $rootScope.currentUser.divisionId);
                                //var dropdownlist = $("#deptDivision").data("kendoDropDownTree");
                                //if (dropdownlist) {
                                //    dropdownlist.value($rootScope.currentUser.divisionId);
                                //}
                            } else {
                                await getDepartmentByUserId($rootScope.currentUser.id, $rootScope.currentUser.deptId, $rootScope.currentUser.divisionId);
                                //await getDeptDivision($rootScope.currentUser.deptId);
                            }
                        }
                        break;
                    default:
                        break;
                }
                var tabToActivate = $("#" + $scope.tabIdGo);
                $("#tabstrip").kendoTabStrip().data("kendoTabStrip").activateTab(tabToActivate);
            }
            else {
                $scope.type = $scope.typeTemporary;
                var tabToActivate = $("#" + $scope.tabIdBack);
                $("#tabstrip").kendoTabStrip().data("kendoTabStrip").activateTab(tabToActivate);
            }
        }

        async function getDepartmentByReferenceNumber(referenceNumber, id, value = null) {
            let referencePrefix = referenceNumber.split('-')[0];
            let res = await settingService.getInstance().departments.getDepartmentByReferenceNumber({ prefix: referencePrefix, itemId: id }).$promise;
            if (res.isSuccess) {
                setDropDownTree('deptDivision', res.object.data, value);
            }
        }

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
                text: $translate.instant('COMMON_BUTTON_REVIEWUSER'),
                enable: false,
                action: function (e) {

                    // $(".k-dialog-title").append(
                    //     `
                    //     <div class="row">
                    //         <div class="col-md-4" style="margin-top: -30px;">
                    //             <div class="form-group">
                    //                 <div class="col-md-2">
                    //                     <kendo-button class="btn btn-default btn-sm" ng-click="back()">
                    //                         <i class="k-icon k-i-arrow-chevron-left"></i>
                    //                     </kendo-button>
                    //                 </div>       
                    //                 <div class="col-md-7 ">
                    //                     <span style="color: #b42e83 !important;font-family: Nova-Regular !important;font-size: 18px !important;font-weight: 700; margin: 10px 0;margin: 0;=line-height: 1.42857143;=text-transform: uppercase !important;">${$translate.instant('REVIEW_USER_TITLE_DIALOG')}</span>
                    //                 </div>
                    //             </div>
                    //         </div>
                    //     </div>
                    //     `
                    // )
                    $scope.reviewUser();
                    $scope.reviewOk = false;
                    return false;
                },
                primary: true
            }]
        };

        $scope.dialogDetailReviewOption = {
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
            actions: [
                {
                    text: $translate.instant('COMMON_BUTTON_OK'),
                    action: function (e) {
                        $scope.reviewOk = true;
                        $scope.isShow = false;
                        $scope.closeReviewDialog(false);
                        $scope.allCheck = false;
                        $scope.arrayCheck = [];
                        $scope.userGridReview.forEach(item => {
                            let value = {
                                userId: item.id,
                                sapCode: item.sapCode,
                                fullName: item.fullName,
                                department: item.department,
                                dateOfOT: moment($scope.userOTReview.dateOfOT).format('DD/MM/YYYY'),
                                reasonOfOT: '',
                                OTProposalHour: caculateOTHourUserReviewV2(item.isStore, $scope.userOTReview.from, $scope.userOTReview.to),
                                from: $scope.userOTReview.from,
                                to: $scope.userOTReview.to,
                                actualOTHour: '',
                                fromActual: '',
                                toActual: '',
                                dayOffInLieu: false,
                                isPrevDay: false,
                                isNoOT: false,
                                isStore: item.isStore
                            }
                            $scope.form.overtimeList.push(value);
                        });
                        let dialog = $("#dialog_Detail").data("kendoDialog");
                        dialog.close();
                        return true;
                    },
                    primary: true
                }
            ]
        };
        $scope.closeReviewDialog = function (openDialog_Detail) {
            $scope.dialogVisible = false;
            if (openDialog_Detail && !$scope.reviewOk) {
                let dialog = $("#dialog_Detail").data("kendoDialog");
                dialog.title($translate.instant('COMMON_BUTTON_ADDUSER'));
                dialog.open();
            }
        }



        $scope.sapCodeDropDownUserPopup = {
            //dataTextField: "showtextCode",
            dataValueField: "id",
            template: '<span><label>#: data.sapCode# - #: data.fullName# </label></span>',
            valueTemplate: '#: sapCode #',
            filter: "contains",
            dataSource: $scope.employeesDataSource,
            filtering: function (ev) {
                var filterValue = ev.filter != undefined ? ev.filter.value : "";
                ev.preventDefault();

                this.dataSource.filter({
                    logic: "or",
                    filters: [{
                        field: "name",
                        operator: "contains",
                        value: filterValue
                    },
                    {
                        field: "code",
                        operator: "contains",
                        value: filterValue
                    }
                    ]
                });
            },
        }

        $scope.addItemsUser = async function (model) {
            $scope.keyWorkTemporary = '';
            $scope.limitDefaultGrid = 20;
            $scope.userOTReview.dateOfOT = null;
            $scope.userOTReview.from = null;
            $scope.userOTReview.to = null;
            $scope.employeesDataSource = []
            $scope.userGrid = [];
            $scope.arrayCheck = [];
            $scope.userGridReview = [];
            $scope.allCheck = false;
            let count = 0;
            $scope.gridUser.keyword = '';
            $scope.isShow = true;
            // await getSapCodeByDivison(1, $scope.limitDefaultGrid);
            // $scope.employeesDataSource.forEach(item => {
            //     item['no'] = count;
            //     item['isCheck'] = false;
            //     $scope.userGrid.push(item);
            //     count++;
            // });
            // setGridUser($scope.userGrid, "#userGrid", $scope.total, 1);
            let grid = $('#userGrid').data("kendoGrid");
            grid.dataSource.data($scope.userGridReview);
            // grid.dataSource.fetch(() => grid.dataSource.page(1));
            $scope.searchGridUser();
            // set title cho cái dialog
            let dialog = $("#dialog_Detail").data("kendoDialog");
            dialog.title($translate.instant('COMMON_BUTTON_ADDUSER'));
            dialog.open();
            $rootScope.confirmDialogAddItemsUser = dialog;
        }

        $scope.backPopupReview = function () {
            let dialog = $("#dialog_Detail_Review").data("kendoDialog");
            dialog.close();
            $scope.closeReviewDialog(true);
        }

        $scope.reviewUser = function () {
            $scope.userGridReview = [];
            let dialog_Detail = $("#dialog_Detail").data("kendoDialog");
            dialog_Detail.close();
            // let grid = $("#userGrid").data("kendoGrid");
            // let userGrid = grid.dataSource._data;
            $scope.arrayCheck.forEach(item => {
                if (item.isCheck) {
                    $scope.userGridReview.push(item);
                }
            });
            setGridUser($scope.userGridReview, '#userGridReview', $scope.userGridReview.length, 1, $scope.limitDefaultGrid);
            // set title cho cái dialog
            let dialog = $("#dialog_Detail_Review").data("kendoDialog");
            //reset title 
            dialog.title("");
            //set title mới
            angular.element('.k-dialog-title').append($compile(`
        <div class="row">
            <div class="d-flex align-items-center">
                <div class="col-md-4" style="margin-top: -25px;">
                    <i class="k-icon k-i-arrow-chevron-left" style="margin-left: -10px;" ng-click="backPopupReview()"></i>
                    <span style="color: #006EFA !important;font-family: Nova-Regular !important;font-size: 18px !important;font-weight: 700; margin: 10px 0;margin: 0;=line-height: 1.42857143;=text-transform: uppercase !important;">${$translate.instant('COMMON_BUTTON_REVIEWUSER')}</span>
                </div>
            </div>
        </div>
        `)($scope)
            )
            dialog.open();
        }



        function setGridUser(data, idGrid, total, pageIndex, pageSizes) {
            let grid = $(idGrid).data("kendoGrid");
            if (idGrid == '#userGrid') {
                let dataSource = new kendo.data.DataSource({
                    transport: {
                        read: async function (e) {
                            await getSAPCode(e);
                        }
                    },
                    pageSize: pageSizes,
                    page: pageIndex,
                    serverPaging: true,
                    schema: {
                        total: function () {
                            return total;
                        }
                    },
                });
                grid.setDataSource(dataSource);
            }
            if (idGrid == '#userGridReview') {
                let dataSource = new kendo.data.DataSource({
                    data: data,
                    pageSize: pageSizes,
                    page: 1,
                    //serverPaging: true,
                    schema: {
                        total: function () {
                            return total;
                        }
                    },
                });
                grid.setDataSource(dataSource);
            }
        }

        $scope.onChange = async function (isCheckAll) {
            let count = 1;
            //await getSapCodeByDivison(1, 10000, $scope.gridUser.keyword);
            await getSapCodeByDivison(1, 10000, $scope.keyWorkTemporary);
            if (isCheckAll) {
                $scope.employeesDataSource.forEach(item => {
                    item['no'] = count;
                    item['isCheck'] = true;
                    count++;
                    //add vô arrayCheck
                    var result = $scope.arrayCheck.find(x => x.id == item.id);
                    if (!result) {
                        $scope.arrayCheck.push(item);
                    }
                });

                $scope.userGrid.forEach(item => {
                    item.isCheck = true;
                });

                $scope.allCheck = true;
            }
            else {
                $scope.employeesDataSource.forEach(item => {
                    item.isCheck = false;
                });

                $scope.userGrid.forEach(item => {
                    item.isCheck = false;
                });
                let arrayTemporary = [];
                $scope.arrayCheck.forEach(item => {
                    var result = $scope.employeesDataSource.find(x => x.id == item.id);
                    if (!result) {
                        arrayTemporary.push(item);
                    }
                });
                $scope.allCheck = false;
                $scope.arrayCheck = arrayTemporary;
            }
            let grid = $("#userGrid").data("kendoGrid");
            page = grid.pager.dataSource._page;
            pageSize = grid.pager.dataSource._pageSize;
            setGridUser($scope.userGrid, '#userGrid', $scope.total, page, pageSizeChange);
        }

        $scope.keyWorkTemporary = '';
        $scope.searchGridUser = async function () {
            $scope.userGrid = [];
            $scope.keyWorkTemporary = $scope.gridUser.keyword;
            if ($scope.gridUser.keyword != null) {
                let result = {};
                if ($scope.model.divisionId) {
                    if ($scope.model.divisionId === '00000000-0000-0000-0000-000000000000') {
                        result = await settingService.getInstance().users.getUserCheckedHeadCount({ departmentId: $rootScope.currentUser.deptId, textSearch: $scope.gridUser.keyword }).$promise;
                    }
                    else {
                        result = await settingService.getInstance().users.getChildUsers({ departmentId: $scope.model.divisionId, limit: $scope.limitDefaultGrid, page: 1, searchText: $scope.gridUser.keyword, isAll: true }).$promise;
                    }

                } else {
                    //result = await settingService.getInstance().users.getUsersByOnlyDeptLine({ depLineId: $rootScope.currentUser.deptId, limit: $scope.limitDefaultGrid, page: 1, searchText: $scope.gridUser.keyword }).$promise;
                    if (!$stateParams.id || ($scope.perm)) {
                        if ($rootScope.currentUser) {
                            result = await settingService.getInstance().users.getUserCheckedHeadCount({ departmentId: $rootScope.currentUser.deptId, textSearch: $scope.gridUser.keyword }).$promise;
                        }
                    }
                }
                if (result.isSuccess) {
                    let dataFilter = result.object.data.map(function (item) {
                        return { ...item, showtextCode: item.sapCode }
                    });
                    $scope.total = result.object.count;
                    let count = 1;
                    let countCheck = 0;
                    //$scope.userGrid = [];
                    dataFilter.forEach(item => {
                        item['no'] = count;
                        item['isCheck'] = false;
                        if ($scope.arrayCheck.length > 0) {
                            var result = $scope.arrayCheck.find(x => x.id == item.id);
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
                        }
                        else {
                            if ($scope.arrayCheck.length == $scope.total && countCheck == $scope.userGrid.length) {
                                $scope.allCheck = true;
                            }
                            else {
                                $scope.allCheck = false;
                            }
                        }
                    } else {
                        $scope.allCheck = false;
                    }
                    setGridUser(dataFilter, '#userGrid', $scope.total, 1, $scope.limitDefaultGrid);
                }
            }
            else {
                let count = 1;
                let countCheck = 0;
                $scope.userGrid.forEach(item => {
                    item['no'] = count;
                    item['isCheck'] = false;
                    if ($scope.arrayCheck.length > 0) {
                        var result = $scope.arrayCheck.find(x => x.id == item.id);
                        if (result) {
                            item.isCheck = true;
                            countCheck++;
                        }
                    }
                    count++;
                });
                setGridUser($scope.userGrid, '#userGrid', $scope.total, 1, $scope.limitDefaultGrid);
            }
            //code moi
            // let grid = $('#userGrid').data("kendoGrid");
            // grid.dataSource.fetch(() => grid.dataSource.page(1));
            //
        }

        $scope.searchGridUserReview = function () {
            let grid = $("#userGridReview").data("kendoGrid");
            pageSize = grid.pager.dataSource._pageSize;
            if ($scope.gridUserReview.keyword) {
                let dataTemporary = _.cloneDeep($scope.userGridReview);
                // let dataFilter = dataTemporary.filter(item => {
                //     if (item.fullName.toLowerCase().includes($scope.gridUserReview.keyword.toLowerCase())) {
                //         return item;
                //     }
                // });
                let dataFilter = _.filter(dataTemporary, item => {
                    return item.fullName.toLowerCase().includes($scope.gridUserReview.keyword.toLowerCase().replace(/  +/g, ' '))
                        || $scope.gridUserReview.keyword.toLowerCase().replace(/  +/g, ' ').includes(item.fullName.toLowerCase().replace(/  +/g, ' '))
                        || item.sapCode.includes($scope.gridUserReview.keyword)
                })
                setGridUser(dataFilter, '#userGridReview', dataFilter.length, 1, pageSize);
            }
            else {
                setGridUser($scope.userGridReview, '#userGridReview', $scope.userGridReview.length, 1, pageSize);
            }
        }
        $scope.ifEnter = function ($event, dialog) {
            var keyCode = $event.which || $event.keyCode;
            if (keyCode === 13) {
                if (dialog == 'review') {
                    $scope.searchGridUserReview();
                } else {
                    $scope.searchGridUser();
                }
            }
        }

        $scope.userGrid = [];

        $scope.userListGridOptions = {
            dataSource: {
                serverPaging: true,
                pageSize: 10,
                transport: {
                    read: async function (e) {
                        await getSAPCode(e);
                    }
                },
                schema: {
                    total: () => { return $scope.total },
                    data: () => { return $scope.userGrid }
                }
            },
            sortable: false,
            autoBind: true,
            resizable: true,
            pageable: {
                // pageSize: appSetting.pageSizeWindow,
                alwaysVisible: true,
                pageSizes: [5, 10, 20, 30, 40],
                responsive: false,
                messages: {
                    display: "{0}-{1} " + $translate.instant('PAGING_OF') + " {2} " + $translate.instant('PAGING_ITEM'),
                    itemsPerPage: $translate.instant('PAGING_ITEM_PER_PAGE'),
                    empty: $translate.instant('PAGING_NO_ITEM')
                }
            },
            columns: [
                {
                    field: "isCheck",
                    title: "<input type='checkbox' ng-model='allCheck' name='allCheck' id='allCheck' class='form-check-input' ng-change='onChange(allCheck)' style='margin-left: 10px;'/> <label class='form-check-label' for='allCheck' style='padding-bottom: 10px;'></label>",
                    width: "50px",
                    template: function (dataItem) {
                        return `<input type="checkbox" ng-model="dataItem.isCheck" name="isCheck{{dataItem.no}}"
                    id="isCheck{{dataItem.no}}" class="form-check-input" style="margin-left: 10px; vertical-align: middle;" ng-change='changeCheck(dataItem)'/>
                    <label class="form-check-label" for="isCheck{{dataItem.no}}"></label>`
                    }
                },
                {
                    field: "sapCode",
                    // title: "SAP Code",
                    headerTemplate: $translate.instant('COMMON_SAP_CODE'),
                    width: "150px",
                },
                {
                    field: "fullName",
                    // title: "Full Name",
                    headerTemplate: $translate.instant('COMMON_FULL_NAME'),
                    width: "200px",
                },
                {
                    field: "department",
                    // title: "Department",
                    headerTemplate: $translate.instant('COMMON_DEPARTMENT'),
                    width: "200px",
                },
                {
                    field: "position",
                    // title: "Position",
                    headerTemplate: $translate.instant('COMMON_POSITION'),
                    width: "200px",
                },
                {
                    //field: "jobGrade",
                    field: "jobGradeTitle",
                    // title: "Job Grade",
                    headerTemplate: $translate.instant('JOBGRADE_MENU'),
                    width: "100px",
                },
            ]
        }


        $scope.arrayCheck = [];
        $scope.disableReview = true;
        $scope.changeCheck = async function (dataItem) {
            if ($scope.allCheck) {
                $scope.allCheck = false;
            }
            $scope.userGrid.forEach(item => {
                if (item.id == dataItem.id) {
                    if (item.isCheck) {
                        item.isCheck = false;
                        var arrayTemporary = $scope.arrayCheck.filter(x => x.id != item.id);
                        $scope.arrayCheck = arrayTemporary;
                    }
                    else {
                        item.isCheck = true;
                        $scope.arrayCheck.push(item);
                        $scope.disableReview = true;
                    }
                }
            });
            // if($scope.arrayCheck.length == $scope.total) {
            //     $scope.allCheck = true;
            // }

            if ($scope.gridUser.keyword) {
                let arrayCheckTemporary = [];
                let grid = $("#userGrid").data("kendoGrid");
                pageSize = grid.pager.dataSource._pageSize;
                if (pageSize < $scope.total) {
                    await getSapCodeByDivison(1, 10000, $scope.gridUser.keyword);
                }
                $scope.employeesDataSource.forEach(x => {
                    let result = $scope.arrayCheck.find(y => y.id == x.id);
                    if (result) {
                        arrayCheckTemporary.push(result);
                    }
                });

                if (arrayCheckTemporary.length == $scope.total) {
                    $timeout(function () {
                        $scope.allCheck = true;
                    }, 0);
                }
            }
            else {
                if ($scope.arrayCheck.length == $scope.total) {
                    $scope.allCheck = true;
                }
            }
        }

        $scope.userGridReview = [];
        $scope.userListGridReviewOptions = {
            dataSource: {
                serverPaging: true,
                pageSize: 10,
                schema: {
                    total: () => { return $scope.userGridReview.length },
                    data: () => { return $scope.userGridReview }
                }
            },
            sortable: false,
            autoBind: true,
            resizable: true,
            pageable: {
                // pageSize: appSetting.pageSizeWindow,
                alwaysVisible: true,
                pageSizes: [5, 10, 20, 30, 40],
                responsive: false,
                messages: {
                    display: "{0}-{1} " + $translate.instant('PAGING_OF') + " {2} " + $translate.instant('PAGING_ITEM'),
                    itemsPerPage: $translate.instant('PAGING_ITEM_PER_PAGE'),
                    empty: $translate.instant('PAGING_NO_ITEM')
                }
            },
            columns: [
                {
                    field: "sapCode",
                    // title: "SAP Code",
                    headerTemplate: $translate.instant('COMMON_SAP_CODE'),
                    width: "100px",
                },
                {
                    field: "fullName",
                    // title: "Full Name",
                    headerTemplate: $translate.instant('COMMON_FULL_NAME'),
                    width: "200px",
                },
                {
                    field: "department",
                    // title: "Department",
                    headerTemplate: $translate.instant('COMMON_DEPARTMENT'),
                    width: "200px",
                },
                {
                    field: "position",
                    // title: "Position",
                    headerTemplate: $translate.instant('COMMON_POSITION'),
                    width: "200px",
                },
                {
                    field: "jobGrade",
                    // title: "Job Grade",
                    headerTemplate: $translate.instant('JOBGRADE_MENU'),
                    width: "100px",
                },
            ],
        }

        $scope.allCheck = false;

        $scope.no = '';
        confirmUserGrid = function (e) {
            if (e.data && e.data.value) {
                let grid = $("#userGrid").data("kendoGrid");
                $scope.userGrid = grid.dataSource._data;
                let resetId = 1;
                $scope.userGrid.splice($scope.no - 1, 1);
                $scope.userGrid.forEach(item => {
                    item.id = resetId;
                    resetId += 1;
                });
                setGridUser($scope.userGrid);
                // Notification.success("Data Sucessfully Deleted"); 
                Notification.success($translate.instant('COMMON_DELETE_SUCCESS'));
            }
        }

        $scope.deleteRecordUserGrid = function (dataItem) {
            $scope.no = dataItem.no;
            $scope.dialog = $rootScope.showConfirmDelete($translate.instant('COMMON_BUTTON_DELETE'), $translate.instant('COMMON_DELETE_VALIDATE'), $translate.instant('COMMON_BUTTON_CONFIRM'));
            $scope.dialog.bind("close", confirmUserGrid);
        };

        $scope.userOTReview = {
            dateOfOT: null,
            from: null,
            to: null
        }
        $scope.temporaryTo = "24:00"
        function caculateOTHourUserReview(dateFrom, dateTo) {
            // start time and end time
            let startTime = moment(dateFrom, "HH:mm");
            let endTime = moment(dateTo, "HH:mm");
            if (dateTo == "00:00") {
                endTime = moment($scope.temporaryTo, "HH:mm");
            }
            let duration = moment.duration(endTime.diff(startTime));
            let hours = duration.asHours();
            if (hours < 0) {
                hours = hours + 24;
            }
            //Làm tròn xuống 15 phút
            let proposalHour = (Math.floor(hours / 0.25) * 0.25).toFixed(2);
            // duration in minutes
            //var minutes = parseInt(duration.asMinutes())%60;

            //duration = moment.duration(endTime.diff(startTime));
            //hours = duration.asHours();
            //let actualHour = (Math.floor(hours / 0.25) * 0.25).toFixed(2);
            //Chỉ lấy giá trị lớn hơn 30 phút
            let result = proposalHour >= 0.50 ? proposalHour : 0;
            if (isStoreCurrentUser == false && result > 4) {
                result = (result - 1).toFixed(2);
            }
            return result;
            //$scope.form.overtimeList[index].actualOTHour = actualHour >= 0.50 ? actualHour : 0;
        }

        function caculateOTHourUserReviewV2(isStore, dateFrom, dateTo) {
            // start time and end time
            let startTime = moment(dateFrom, "HH:mm");
            let endTime = moment(dateTo, "HH:mm");
            if (dateTo == "00:00") {
                endTime = moment($scope.temporaryTo, "HH:mm");
            }
            let duration = moment.duration(endTime.diff(startTime));
            let hours = duration.asHours();
            if (hours < 0) {
                hours = hours + 24;
            }
            //Làm tròn xuống 15 phút
            let proposalHour = (Math.floor(hours / 0.25) * 0.25).toFixed(2);
            // duration in minutes
            //var minutes = parseInt(duration.asMinutes())%60;

            //duration = moment.duration(endTime.diff(startTime));
            //hours = duration.asHours();
            //let actualHour = (Math.floor(hours / 0.25) * 0.25).toFixed(2);
            //Chỉ lấy giá trị lớn hơn 30 phút
            let result = proposalHour >= 0.50 ? proposalHour : 0;
            if (isStore == false && result > 4) {
                result = (result - 1).toFixed(2);
            }
            return result;
            //$scope.form.overtimeList[index].actualOTHour = actualHour >= 0.50 ? actualHour : 0;
        }

        $rootScope.$on("isEnterKeydown", function (event, data) {
            if ($scope.advancedSearchMode && data.state == $state.current.name) {
                $scope.applySearch();
            }
        });
        $scope.printForm = async function () {
            let res = await cbService.getInstance().overtimeApplication.printForm({ id: $stateParams.id }).$promise;
            if (res.isSuccess) {
                exportToPdfFile(res.object);
            }
        }
        $rootScope.$watch(function () { return dataService.permission }, function (newValue, oldValue) {

            if ($stateParams.id) {
                $scope.perm = (2 & dataService.permission.right) == 2;
            } else {
                $scope.perm = true;
            }

        }, true);

        $scope.checkAllowRevoke = async function () {
            let returnValue = true;
            $scope.errors = $scope.validateOvertimeList();
            if ($scope.errors.length > 0) {
                returnValue = false;
            }
            return returnValue;
        }

        async function getUserCheckedHeadCount(deptId, textSearch = "") {
            if (deptId) {
                let result = await settingService.getInstance().users.getUserCheckedHeadCount({ departmentId: deptId, textSearch: textSearch }).$promise;
                if (result.isSuccess) {
                    $scope.employeesDataSource = result.object.data.map(function (item) {
                        return { ...item, showtextCode: item.sapCode }
                    });
                    $scope.total = result.object.count;
                }
            }

        }

        pageSizeChange = 20;
        async function getSAPCode(option) {
            $scope.userGrid = [];
            $scope.limitDefaultGrid = option.data.take;
            let countCheck = 0;

            if ($scope.keyWorkTemporary) {
                await getSapCodeByDivison(1, 10000, $scope.keyWorkTemporary);
                $scope.employeesDataSource.forEach(item => {
                    if ($scope.arrayCheck.length > 0) {
                        var result = $scope.arrayCheck.find(x => x.id == item.id);
                        if (result) {
                            item.isCheck = true;
                            countCheck++;
                        }
                    }
                });
            }

            if ($scope.model.divisionId) {
                await getSapCodeByDivison(option.data.page, option.data.take, $scope.keyWorkTemporary);
            } else {
                if (!$stateParams.id || ($scope.perm)) {
                    if ($rootScope.currentUser) {
                        await getUserCheckedHeadCount($rootScope.currentUser.deptId, $scope.keyWorkTemporary);
                    }
                }

            }
            let grid = $("#userGrid").data("kendoGrid");
            pageSizeChange = grid.pager.dataSource._pageSize;
            if (grid) {
                let count = 1;

                if ($scope.allCheck && $scope.arrayCheck.length === $scope.total) {
                    $scope.employeesDataSource.forEach(item => {
                        item['no'] = count;
                        item['isCheck'] = true;
                        $scope.userGrid.push(item);
                        count++;
                        //add vô arrayCheck
                        var result = $scope.arrayCheck.find(x => x.id == item.id);
                        if (!result) {
                            $scope.arrayCheck.push(item);
                        }
                    });
                    countCheck = $scope.arrayCheck.length;
                }
                else {
                    $scope.employeesDataSource.forEach(item => {
                        item['no'] = count;
                        item['isCheck'] = false;
                        if ($scope.arrayCheck.length > 0) {
                            var result = $scope.arrayCheck.find(x => x.id == item.id);
                            if (result) {
                                item.isCheck = true;
                                //countCheck++;
                            }
                        }
                        $scope.userGrid.push(item);
                        count++;
                    });
                }
                //code moi
                if ($scope.total <= $scope.arrayCheck.length) {
                    if (countCheck == $scope.total && countCheck > 0) {
                        $scope.allCheck = true;
                    }
                    else {
                        if ($scope.arrayCheck.length == $scope.total && countCheck == $scope.arrayCheck.length && $scope.arrayCheck.length > 0) {
                            $scope.allCheck = true;
                        }
                        else {
                            $scope.allCheck = false;
                        }
                    }
                } else {
                    $scope.allCheck = false;
                }
                //
            }
            option.success($scope.userGrid);
        }
        $scope.$on('Revoke Actual', async function (event, data) {
            var result = await workflowService.getInstance().workflows.startWorkflow(data.voteModel, null).$promise;
            if (result.messages.length == 0) {
                Notification.success($translate.instant('COMMON_WORKFLOW_STARTED'));
                $state.go($state.current.name, { id: $scope.model.id, referenceValue: $scope.model.referenceNumber }, { reload: true });
            } else {
                Notification.error(result.messages[0]);
            }
        })
        //fix Overtime Division
        async function getDepartmentByUserId(userId, deptId, divisionId) {
            $scope.deptLineList = [];
            $scope.dept_Line = deptId;
            let res = await settingService.getInstance().departments.getDepartmentsByUserId({ id: userId }, null).$promise;
            if (res && res.isSuccess) {
                $scope.deptLineList = res.object.data;
                var deptLines = _.map($scope.deptLineList, x => { return x['deptLine'] });
                var dropdownlist = $("#deptLineOptionId").data("kendoDropDownList");
                if (dropdownlist)
                    dropdownlist.setDataSource(deptLines);
                if (deptId) {
                    if (dropdownlist) {
                        dropdownlist.value(deptId);
                    }
                }
                if (!$stateParams.id) {
                    if (!divisionId) {
                        await getDivisionsByDeptLine(deptId);
                    }
                    else {
                        await getDivisionsByDeptLine(deptId, divisionId);
                    }
                } else {
                    if (!divisionId) {
                        await getDivisionsByDeptLine(deptId);
                    }
                }
            }
        }
        async function getDivisionsByDeptLine(deptId, divisionId = '') {
            var currentItem = _.find($scope.deptLineList, x => { return x.deptLine.id == deptId });
            var dropdownlist = $("#deptDivision").data("kendoDropDownTree");
            if (dropdownlist) {
                if (currentItem && currentItem.divisions && currentItem.divisions.length) {
                    var ids = _.map(currentItem.divisions, 'id');
                    await getDivisionTreeByIds(ids);
                } else {
                    await getDivisionTreeByIds([deptId]);
                }
                if (divisionId) {
                    dropdownlist.value(divisionId);
                    $scope.model.divisionId = divisionId;
                }
            }

        }
        async function getDivisionTreeByIds(ids) {
            arg = {
                predicate: "",
                predicateParameters: [],
                page: 1,
                limit: appSetting.pageSizeDefault,
                order: ""
            }
            for (let i = 0; i < ids.length; i++) {
                arg.predicate = arg.predicate ? arg.predicate + `||id = @${i}` : `id = @${i}`;
                arg.predicateParameters.push(ids[i]);
            }
            result = await settingService.getInstance().departments.getDepartmentByFilter(arg).$promise;
            if (result.isSuccess) {
                $scope.deptDivision = result.object.data;
                // $scope.dataTemporaryArrayDivision = _.filter(result.object.data, x => { return x.items.length || x.jobGradeGrade <= 4 });
                $scope.dataTemporaryArrayDivision = _.filter(result.object.data, x => { return x.items.length || x.type == 1 });
                setDataDeptDivision($scope.dataTemporaryArrayDivision);
                if (_.findIndex(ids, x => { return x == $rootScope.currentUser.divisionId }) > -1) {
                    var dropdownlist = $("#deptDivision").data("kendoDropDownTree");
                    if (!$stateParams.id) {
                        if (dropdownlist) {
                            dropdownlist.value($rootScope.currentUser.divisionId);
                        }
                        $scope.model.divisionId = $rootScope.currentUser.divisionId;
                    }
                }

            } else {
                Notification.error(result.messages[0]);
            }
        }
        //CR220
        $scope.isCheckNoOT = function () {
            if (this.item.isNoOT) {
                //disable fromActual, toActual, dayOffInLieu
                this.item.dayOffInLieu = false;
                this.item.fromActual = this.item.from;
                this.item.toActual = this.item.to;
                this.item.actualOTHour = this.item.OTProposalHour;
            }
        }
    });
/*
Tự động resize textarea lý do khác
*/
function auto_grow(element) {
    element.style.maxHeight = "auto";
    if (element.scrollHeight < 27 || element.textLength < 50) {
        element.rows = 1
        element.style.maxHeight = 27.63 + "px";
    }
    else {
        element.rows = 3
        element.style.maxHeight = (element.scrollHeight) + "px";
    }
}