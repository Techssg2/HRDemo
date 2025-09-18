var ssgApp = angular.module('ssg.directiveWorkflowTaskModule', []);
ssgApp.directive("workflowTask", [

    function ($rootScope) {
        return {
            restrict: "E",
            templateUrl: "ClientApp/app/shared/directives/processing-stages/workflow-history-task.template.html?v=" + edocV,
            scope: {
                round: "=",
                directivevalue: "=",
                step: "=",
                onChangeStep: "&",
                flow:"="
            },
            link: function ($scope, element, attr, modelCtrl) { },
            controller: [
                "$rootScope", "$scope", "appSetting", "$translate", "settingService", "$timeout", "Notification", "workflowService", "trackingHistoryService", "$state", "attachmentService",
                async function ($rootScope, $scope, appSetting, $translate, settingService, $timeout, Notification, workflowService, trackingHistoryService, $state, attachmentService) {
                    $scope.formatDate = "dd/MM/yyyy HH:mm:ss";
                    console.log("Flow new:" + JSON.stringify($scope.flow));
                    //0:None
                    //1: Approve
                    //2: Reject
                    //3: Request To Change
                    if ($scope.round && $scope.round.id) {
                        $scope.roundStyle = "task-status-in-progress task-outcome-none";
                        if ($scope.round.voteType == 1 || $scope.round.voteType == 0 && $scope.round.isStepCompleted) {
                            $scope.roundStyle = "task-status-completed task-outcome-approved";
                        } else if ($scope.round.voteType == 2) {
                            $scope.roundStyle = "task-status-completed task-outcome-rejected";
                        } else if ($scope.round.voteType == 3) {
                            $scope.roundStyle = "task-status-completed task-outcome-requested-to-change";
                        } else if ($scope.round.voteType == 4) {
                            $scope.roundStyle = "task-status-completed task-outcome-cancelled";
                        } else if ($scope.round.voteType == 5) {
                            $scope.roundStyle = "task-status-completed task-outcome-cancelled";
                        }

                        if ($scope.round.assignedToDepartmentId) {
                            $scope.round.assignedToDepartmentInfo = $scope.round.assignedToDepartmentCode + " (" + ($scope.round.assignedToDepartmentType == 0 ? '' : _.find(appSetting.DepartmentGroup, x => { return $scope.round.assignedToDepartmentType == x.id }).title) +")";
                        }
                    } else {
                        $scope.roundStyle = "task-status-none";
                    }

                    $scope.isRefind = false;

                    $scope.isITHelpDesk = $rootScope.currentUser && $rootScope.currentUser.isITHelpDesk;

                    if ($scope.isITHelpDesk) {
                        $scope.model = { isValid: true };
                        $scope.editDepartmentDialog = {};
                        // Attachments
                        $scope.attachments = [];
                        $scope.removeFiles = [];
                        let allDepartments = JSON.parse(sessionStorage.getItemWithSafe("departments"));
                        $scope.isEditDepartment = true;
                        $scope.canEdit = function (type) {
                            let currentRoleUser = $rootScope.currentUser && $rootScope.currentUser.isITHelpDesk;
                            var departmentStepId;
                            if (type == 'EditDepartment') {
                                departmentStepId = $scope.round.assignedToDepartmentId && !$scope.round.assignedToUserId;
                            } else if (type == 'EditUser') {
                                departmentStepId = $scope.round.assignedToUserId;
                            }
                            return currentRoleUser && departmentStepId && !$scope.round.outcome && $scope.round.voteType == 0;
                        }

                        /*$scope.checkReFindDepartment = async function () {
                            let check = await workflowService.getInstance().workflows.checkReFindAssign({ InstanceId: $scope.round.instanceId }).$promise;
                            if (check.errorCodes.length > 0 && !$scope.round.outcome && $scope.round.voteType == 0) $scope.isRefind = true;
                        }

                        $scope.checkReFindDepartment();*/

                        function initDataSource() {
                            if ($scope.directivevalue && $scope.directivevalue.currentstep) {
								 $timeout(function () {
									$scope.directivevalue.setDataNewDepartment(allDepartments);
								}, 0);
                                $scope.isEditDepartment = $scope.directivevalue.isEditDepartment;
                                if ($scope.isEditDepartment) {
                                    // init source
                                    $scope.departmentOptions = $scope.directivevalue.departmentOptions;
                                    $scope.newDepartmentGroupOptions = $scope.directivevalue.newDepartmentGroupOptions;
                                    $scope.currentdepartmentOptions = $scope.directivevalue.currentdepartmentOptions
                                    $scope.currentDepartmentGroupOptions = $scope.directivevalue.currentDepartmentGroupOptions;;

                                    // init field
                                    $scope.model.currentDepartmentId = $scope.directivevalue.currentstep.assignedToDepartmentId;
                                    $scope.model.currentDepartmenGroup = $scope.directivevalue.currentstep.assignedToDepartmentType;
                                } else {
                                    $scope.currentUserOptions = $scope.directivevalue.currentUserOptions;
                                    $scope.newUserOptions = $scope.directivevalue.newUserOptions;
                                    

                                    $scope.model.currentUserId = $scope.directivevalue.currentstep.assignedToUserId;
                                }
                            }
                        }

                        initDataSource();
                        $scope.showEditDepartment = async function (round) {
                            enableElementWithRoleITHelpdesk();
                            $timeout(function () {
                                /*$scope.directivevalue.setDataDepartment(allDepartments);*/
                                $scope.directivevalue.setDataNewDepartment(allDepartments);
                            }, 0);

                            $scope.editDepartmentDialog.title($translate.instant('CHANGE_STEP'));
                            $scope.editDepartmentDialog.open();
                            $rootScope.confirmVoteDialog = $scope.dialogChangeStepDialog;
                            $scope.$emit("closeProcessModal", true);
                        }

                        $scope.dialogEditDepartmentDialogOpts = {
                            width: "1000px",
                            buttonLayout: "normal",
                            closable: true,
                            modal: true,
                            visible: false,
                            content: "",
                            actions: [{
                                text: $translate.instant('COMMON_BUTTON_OK'),
                                action: function (e) {
                                    if ($scope.isEditDepartment && (!$scope.model.newDepartmentId || $scope.model.newDepartmentId == '') && !$scope.model.newDepartmentGroup || $scope.model.newDepartmentGroup == '') {
                                        $scope.model.isValid = false;
                                        $scope.model.errorMessage = $translate.instant('COMMON_ASSIGN_TO_DEPARTMENT_AND_GROUP_VALIDATE');
                                        $scope.$apply();
                                        return false;
                                    } 

                                    if ($scope.model.comment) {
                                        $scope.model.isValid = true;
                                        let dialogChangeStepWorkflow = $rootScope.showConfirmYNDialog("COMMON_NOTIFICATION", "COMMON_SAVE_CONFIRM");
                                        $scope.$apply();
                                        dialogChangeStepWorkflow.bind("close", async function (e) {
                                            if (e.data && e.data.value) {
                                                // upload attachment
                                                if ($scope.attachments.length || $scope.removeFiles.length) {
                                                    let attachmentFilesJson = await mergeAttachmentV2(attachmentService, $scope.oldAttachments, $scope.attachments);
                                                    $scope.model.documents = attachmentFilesJson;
                                                }
                                                var modelUpdate = {};
                                                if ($scope.isEditDepartment) {
                                                    modelUpdate = {
                                                        AssignFromId: $scope.directivevalue.currentstep.assignedToDepartmentId,
                                                        AssignFromName: $scope.directivevalue.currentstep.assignedToDepartmentName,
                                                        AssignFromGroup: $scope.directivevalue.currentstep.assignedToDepartmentType,
                                                        AssignToId: $scope.model.newDepartmentId,
                                                        AssignToGroup: $scope.model.newDepartmentGroup,
                                                        Type: 'Department',
                                                        
                                                    };
                                                } else {
                                                    modelUpdate = {
                                                        AssignFromId: $scope.directivevalue.currentstep.assignedToUserId,
                                                        AssignFromName: $scope.directivevalue.currentstep.assignedToDepartmentName ? $scope.directivevalue.currentstep.assignedToDepartmentName : $scope.directivevalue.currentstep.assignedToUserFullName,
                                                        AssignToId: $scope.model.newUserId,
                                                        Type: 'User'
                                                    };
                                                }
                                                modelUpdate.StepNumber = $scope.directivevalue.currentstep.stepNumber;
                                                modelUpdate.InstanceId = $scope.directivevalue.currentstep.instanceId;

                                                let updateDepartmentResponse = await workflowService.getInstance().workflows.updateAssignToByItemId(modelUpdate).$promise;
                                                if (updateDepartmentResponse.isSuccess) {
                                                    // save log
                                                    var modelLog = {
                                                        ItemRefereceNumberOrCode: $scope.directivevalue.currentstep.itemReferenceNumber,
                                                        ItemType: $rootScope.getItemTypeByReferenceNumber($scope.directivevalue.currentstep.itemReferenceNumber),
                                                        Type: appSetting.TrackingType.UpdateApproval,
                                                        DataStr: JSON.stringify(modelUpdate),
                                                        Comment: $scope.model.comment,
                                                        InstanceId: $scope.directivevalue.currentstep.instanceId,
                                                        Documents: $scope.model.documents,
                                                        RoundNum: $scope.directivevalue.currentstep.roundNum
                                                    };
                                                    let saveLog = await trackingHistoryService.getInstance().trackingHistory.saveTrackingHistory(modelLog).$promise;
                                                    if (saveLog.isSuccess) {
                                                        Notification.success($translate.instant("COMMON_SAVE_SUCCESS"));
                                                        let dialog = $("#editDepartmentDialog").data("kendoDialog");
                                                        dialog.close();
                                                        $('.modal-backdrop').remove();
                                                        $state.reload();
                                                    }
                                                } else {
                                                    Notification.error(updateDepartmentResponse.messages[0]);
                                                    return false;
                                                }
                                            }
                                        });
                                    } 
                                    else {
                                            $scope.model.isValid = false;
                                            $scope.model.errorMessage = $translate.instant('COMMON_COMMENT_VALIDATE');
                                            $scope.$apply();
                                            return false;
                                    }
                                },
                                primary: true
                            }],
                            close: async function (e) {
                                $scope.model.errorMessage = null;
                                $scope.model.isValid = true;
                                $scope.$emit("closeProcessModal", false);
                            }
                        };

                        $scope.skipBTA = async function () {
                            let dialogs = $("#dialog_skipBTA").data("kendoDialog");
                            dialogs.title($translate.instant('SkipBTA'))
                            dialogs.open();
                            $scope.$emit("closeProcessModal", true);
                        }

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

                        $scope.reFindDepartment = async function () {
                            let refind = await workflowService.getInstance().workflows.reFindAssignToCurrentStep({ InstanceId: $scope.round.instanceId }).$promise;
                            if (refind.isSuccess && refind.errorCodes.length == 0) {
                                Notification.success($translate.instant("COMMON_SAVE_SUCCESS"));
                                $state.reload();
                            }
                            else {
                                Notification.error(refind.messages[0]);
                            }
                        }
                    }
                },
            ],
        };
    },
]);

ssgApp.directive('showMore', function () {
    var templateShowMore = '<span class="{{class}}" ">{{truncate}}</span>' +
        '<a href="javascript:void(0);" class="no-underline" id="wordForTruncate"  ng-click="checkCondition()">{{replace && text.length > number  ? more : !replace && text.length > number ? less : ""}}</a>';
    return {
        restrict: "E",
        scope: {
            more: "@more",
            less: "@less",
            text: "=text",
            number: "@number",
            class: "@class",
        },
        template: templateShowMore,
        link: function ($scope, $element, attr) {
            var comment = $scope.text && $scope.text.substring($scope.number, $scope.text.length);
            $scope.truncate = $scope.text && $scope.text.replace(comment, $scope.text.length > $scope.number ? "..." : "");
            $scope.replace = true;
            $scope.checkCondition = function () {
                if ($scope.replace && $scope.text.length > $scope.number) {
                    $scope.showFullText();
                } else {
                    $scope.hideFullText();
                }
            };

            $scope.showFullText = function () {
                $scope.truncate = $scope.text;
                $scope.replace = false;
            };

            $scope.hideFullText = function () {
                $scope.text.substring($scope.number, $scope.text.length);
                $scope.truncate = $scope.text.replace(comment, "...");
                $scope.replace = true;
            };
        },
    };
});
