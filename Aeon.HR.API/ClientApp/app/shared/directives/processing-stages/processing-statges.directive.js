var ssgApp = angular.module('ssg.directiveStageModule', []);
ssgApp.directive("processingStage", [
    function($rootScope) {
        return {
            restrict: "E",
            templateUrl: "ClientApp/app/shared/directives/processing-stages/processing-stages.template.html?v=" + edocV,
            scope: {
                currentUser: "=",
                workflowInstances: "=",
                model: "="
            },
            link: function ($scope, element, attr, modelCtrl) { },
            controller: [
                "$rootScope", "$scope", "dataService", "appSetting", "settingService", "$translate", "trackingHistoryService", "workflowService", "$state", "Notification", "attachmentService", "$timeout","$stateParams",
                function ($rootScope, $scope, dataService, appSetting, settingService, $translate, trackingHistoryService, workflowService, $state, Notification, attachmentService, $timeout, $stateParams) {
                    let directivevalue = {};
                    $scope.roundIndex = 0;
                    $scope.workflowStatus = dataService.workflowStatus;
                    $scope.isITHelpDesk = $rootScope.currentUser && $rootScope.currentUser.isITHelpDesk;
                    $scope.selectedTab = 0;
                    $scope.reverseArray = function (arr) {
                        if ($scope.isITHelpDesk) {
                            getCurrentStep(arr); // get current
                        }
                        if (arr && $scope.selectedTab == 0) {
                            $scope.selectedTab = arr.length;
                        }
                        return _.orderBy(arr, 'created', 'asc');
                    }

                    //$scope.selectedTab = 1;
                    $scope.changeRound = function (round) {
                        $timeout(function () {
                            $scope.selectedTab = round;
                        });
                        
                    }

                    $scope.$on("closeProcessModal", async function (evt, data) {
                        if (data) {
                            var modalElement = document.getElementById('exampleModalView5');
                            if (modalElement) {
                                modalElement.style.display = 'none';
                            }
                        }
                        else {
                            var modalElement = document.getElementById('exampleModalView5');
                            if (modalElement) {
                                modalElement.style.display = 'block';
                            }
                        }
                    });

                    $scope.updateIndex = function (index) {
                        return $scope.roundIndex;
                    }

                    if ($scope.isITHelpDesk) {
                        function getCurrentStep(instanceArr) {
                            let currentInstance = _.find(instanceArr, x => {
                                return !x.isCompleted;
                            });

                            if (currentInstance) {
                                directivevalue.currentstep = _.first(_.orderBy(_.filter(currentInstance.histories, x => {
                                    return !x.outcome && x.voteType == 0;
                                }), 'created', 'desc'));
                                // luu log
                                if (directivevalue.currentstep) {
                                    directivevalue.currentstep.roundNum = (instanceArr.length); // lay ra round hien tai
                                    if (instanceArr[0] && instanceArr[0].itemReferenceNumber) {
                                        directivevalue.currentstep.itemReferenceNumber = instanceArr[0].itemReferenceNumber;
                                    }
                                    if (directivevalue.currentstep.assignedToDepartmentName && directivevalue.currentstep.assignedToDepartmentId && !directivevalue.currentstep.assignedToUserId) {
                                        // department
                                        directivevalue.isEditDepartment = true;
                                        directivevalue.currentdepartmentOptions = {
                                            placeholder: "",
                                            dataTextField: "name",
                                            dataValueField: "id",
                                            valuePrimitive: true,
                                            checkboxes: false,
                                            autoBind: false,
                                            filter: "contains",
                                            dataSource: {
                                                data: directivevalue.currentstep ? [{ id: directivevalue.currentstep.assignedToDepartmentId, name: directivevalue.currentstep.assignedToDepartmentName }] : []
                                            }
                                        }
                                    } else if (directivevalue.currentstep.assignedToUserFullName && directivevalue.currentstep.assignedToUserId) {
                                        // user
                                        directivevalue.isEditDepartment = false;
                                        directivevalue.currentUserOptions = {
                                            placeholder: "",
                                            dataTextField: "name",
                                            dataValueField: "id",
                                            valuePrimitive: true,
                                            checkboxes: false,
                                            autoBind: false,
                                            filter: "contains",
                                            dataSource: {
                                                data: directivevalue.currentstep ? [{ id: directivevalue.currentstep.assignedToUserId, name: directivevalue.currentstep.assignedToUserFullName }] : []
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        $scope.modelSync = { isValid: true };
                        let allDepartments = JSON.parse(sessionStorage.getItemWithSafe("departments"));
                        $scope.syncWorkflowDialog = {};
                        // Attachments
                        $scope.attachments = [];
                        $scope.removeFiles = [];
                        // set value vao deractive
                        directivevalue.newDepartmentGroupOptions = {
                            placeholder: "",
                            dataTextField: "title",
                            dataValueField: "id",
                            valuePrimitive: true,
                            checkboxes: false,
                            autoBind: false,
                            filter: "contains",
                            dataSource: {
                                data: appSetting.departmentGroup
                            }
                        };

                        directivevalue.currentDepartmentGroupOptions = {
                            placeholder: "",
                            dataTextField: "title",
                            dataValueField: "id",
                            valuePrimitive: true,
                            checkboxes: false,
                            autoBind: false,
                            filter: "contains",
                            dataSource: {
                                data: appSetting.departmentGroup
                            }
                        }

                        directivevalue.setDataNewDepartment = function (dataDepartment) {
                            var dataSource = new kendo.data.HierarchicalDataSource({
                                data: dataDepartment,
                                schema: {
                                    model: {
                                        children: "items"
                                    }
                                }
                            });
                            var currentDepartmentTree = $("#newDepartmentId").data("kendoDropDownTree");
                            if (currentDepartmentTree) {
                                currentDepartmentTree.setDataSource(dataSource);
                            }
                        }

                        async function getDepartmentByFilter(option) {
                            var filter = option.data.filter && option.data.filter.filters.length ? option.data.filter.filters[0].value : "";
                            arg = {
                                predicate: "name.contains(@0) or code.contains(@1) or UserDepartmentMappings.Any(User.FullName.contains(@2) or User.LoginName.contains(@3) or User.Email.contains(@4) or User.SAPCode.contains(@5))",
                                predicateParameters: [filter, filter, filter, filter, filter, filter],
                                page: 1,
                                limit: appSetting.pageSizeDefault,
                                order: "JobGrade.Caption desc"
                            }

                            var res = await settingService.getInstance().departments.getDepartmentByFilterV2(arg).$promise;
                            if (res.isSuccess) {
                                $scope.dataUser = [];
                                res.object.forEach(element => {
                                    $scope.dataUser.push(element);
                                });
                            }
                            if (option) {
                                option.success($scope.dataUser);
                            }
                        }

                        directivevalue.departmentOptions = {
                            dataTextField: 'code',
                            dataValueField: 'id',
                            template: '#: code # - #: name #',
                            valueTemplate: '#: code # - #: name #',
                            autoBind: true,
                            valuePrimitive: true,
                            filter: "contains",
                            customFilterFields: ['code', 'name'],
                            filtering: filterMultiField,
                            dataSource: {
                                serverFiltering: true,
                                transport: {
                                    read: async function (e) {
                                        await getDepartmentByFilter(e);
                                    }
                                },
                            },
                            change: function (e) {
                                if (e.sender.dataItem()) {

                                }
                            }
                        };

                        // get user
                        directivevalue.newUserOptions = {
                            dataTextField: 'fullName',
                            dataValueField: 'id',
                            template: '#: sapCode # - #: fullName #',
                            valueTemplate: '#: sapCode # - #: fullName #',
                            autoBind: true,
                            valuePrimitive: true,
                            filter: "contains",
                            customFilterFields: ['sapCode', 'fullName'],
                            filtering: filterMultiField,
                            dataSource: {
                                serverFiltering: true,
                                transport: {
                                    read: async function (e) {
                                        await getUsers(e);
                                    }
                                },
                            },
                            change: function (e) {
                                if (e.sender.dataItem()) {
                                   /* $timeout(function () {
                                        $scope.assigneeDetail.newAssigneeObject = {
                                            id: e.sender.dataItem().id,
                                            fullName: e.sender.dataItem().fullName,
                                            sapCode: e.sender.dataItem().sapCode
                                        };
                                    }, 0);*/
                                }
                            }
                        };
                        $scope.directivevalue = directivevalue;

                        async function getUsers(option) {
                            var filter = option.data.filter && option.data.filter.filters.length ? option.data.filter.filters[0].value : "";
                            var arg = {
                                predicate: "sapcode.contains(@0) or fullName.contains(@1) or loginName.contains(@2) or email.contains(@3)",
                                predicateParameters: [filter, filter, filter, filter],
                                page: 1,
                                limit: appSetting.pageSizeDefault,
                                order: "sapcode asc"
                            }
                            var res = await settingService.getInstance().users.getUsers(arg).$promise;
                            if (res.isSuccess) {
                                $scope.dataUser = [];
                                res.object.data.forEach(element => {
                                    $scope.dataUser.push(element);
                                });
                            }
                            if (option) {
                                option.success($scope.dataUser);
                            }
                        }

                        /* Action Sync */
                        $scope.syncWorkflow = async function (model, round) {
                            enableElementWithRoleITHelpdesk();
                            $scope.modelSync = {
                                instanceId: model.id,
                                itemReferenceNumber: model.itemReferenceNumber,
                                workflowDataStr: JSON.stringify(model.workflowData),
                                roundNum: round
                            }
                            let title = $translate.instant('SYNC_WORKFLOW');
                            $scope.syncWorkflowDialog.title(title);
                            $scope.syncWorkflowDialog.open();
                            $rootScope.confirmVoteDialog = $scope.syncWorkflowDialog;
                            $scope.$emit("closeProcessModal", true);
                        }

                        $scope.syncWorkflowDialogOpts = {
                            width: "1000px",
                            buttonLayout: "normal",
                            closable: true,
                            modal: true,
                            visible: false,
                            content: "",
                            actions: [{
                                text: $translate.instant('COMMON_BUTTON_OK'),
                                action: function (e) {
                                    if ($scope.modelSync.comment) {
                                        $scope.modelSync.isValid = true;
                                        let dialogSyncWorkflow = $rootScope.showConfirmYNDialog("COMMON_NOTIFICATION", "COMMON_SAVE_CONFIRM");
                                        $scope.$apply();
                                        dialogSyncWorkflow.bind("close", async function (e) {
                                            if (e.data && e.data.value) {
                                                // upload attachment
                                                if ($scope.attachments.length || $scope.removeFiles.length) {
                                                    let attachmentFilesJson = await mergeAttachmentV2(attachmentService, $scope.oldAttachments, $scope.attachments);
                                                    $scope.modelSync.documents = attachmentFilesJson;
                                                }

                                                let syncWorkflowResponse = await workflowService.getInstance().workflows.syncWorkflowById({ Id: $scope.modelSync.instanceId }).$promise;
                                                if (syncWorkflowResponse.isSuccess) {
                                                    // save log
                                                    let modelLog = {
                                                        ItemRefereceNumberOrCode: $scope.modelSync.itemReferenceNumber,
                                                        ItemType: $rootScope.getItemTypeByReferenceNumber($scope.modelSync.itemReferenceNumber),
                                                        Type: appSetting.TrackingType.SyncWorkflow,
                                                        Comment: $scope.modelSync.comment,
                                                        InstanceId: $scope.modelSync.instanceId,
                                                        WorkflowDataStr: $scope.modelSync.workflowDataStr,
                                                        Documents: $scope.modelSync.documents,
                                                        RoundNum: $scope.modelSync.roundNum
                                                    };
                                                    let saveLog = await trackingHistoryService.getInstance().trackingHistory.saveTrackingHistory(modelLog).$promise;
                                                    if (saveLog.isSuccess) {
                                                        Notification.success($translate.instant("COMMON_SAVE_SUCCESS"));
                                                        $("#syncWorkflowDialog").data("kendoDialog").close();
                                                        $state.go($state.current.name, {}, { reload: true });
                                                    }
                                                }
                                            }
                                        });
                                    } else {
                                        $scope.modelSync.isValid = false;
                                        $scope.modelSync.errorMessage = $translate.instant('COMMON_COMMENT_VALIDATE');
                                        $scope.$apply();
                                        return false;
                                    }
                                },
                                primary: true
                            }],
                            close: async function (e) {
                                $scope.modelSync.errorMessage = null;
                                $scope.modelSync.isValid = true;
                                $scope.$emit("closeProcessModal", false);
                            }
                        };

                        

                        $scope.onSelect = function (e) {
                            let message = $.map(e.files, function (file) {
                                $scope.attachments.push(file);
                                return file.name;
                            }).join(", ");
                        };

                        $scope.removeAttach = function (e) {
                            let file = e.files[0];
                            $scope.attachments = $scope.attachments.filter(item => item.name !== file.name);
                        }

                        $scope.removeOldAttachment = function (model) {
                            $scope.oldAttachments = $scope.oldAttachments.filter(item => item.id !== model.id);
                            $scope.removeFiles.push(model.id);
                        }

                        $scope.downloadAttachment = async function (id) {
                            await attachmentFile.downloadAndSaveFile({
                                id
                            });
                        }
                    }
                }
            ]
        };
    },
]);