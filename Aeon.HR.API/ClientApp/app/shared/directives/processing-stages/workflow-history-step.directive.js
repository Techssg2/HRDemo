var ssgApp = angular.module('ssg.directiveWorkflowStepModule', []);
ssgApp.directive("workflowStep", [
    function ($rootScope) {
        return {
            restrict: "E",
            templateUrl: "ClientApp/app/shared/directives/processing-stages/workflow-history-step.template.html?v=" + edocV,
            scope: {
                step: "=",
                flow: "=",
                model: "=",
                directivevalue: "="
            },
            link: function ($scope, element, attr, modelCtrl) { },
            controller: [
                "$rootScope", "$scope", "dataService", "$translate", "workflowService", "Notification", "$state", "trackingHistoryService", "$timeout", "attachmentService", "appSetting","$stateParams",
                function ($rootScope, $scope, dataService, $translate, workflowService, Notification, $state, trackingHistoryService, $timeout, attachmentService, appSetting, $stateParams) {

                    $scope.allowShowThisStep = function () {
                        let returnValue = true;
                        if ($scope.step.isStepWithConditions == true) {
                            let failedResult = $($scope.step.stepConditions)
                                .map(function (cIndex, cCondition) {
                                    return $scope.doesMatchStepConditions(cCondition);
                                })
                                .filter(function (cIndex, cItem) {
                                    return cItem == false;
                                });

                            //If has any case mot match this step will not be display
                            returnValue = failedResult.length == 0;

                        }
                        return returnValue;
                    }
                    $scope.doesMatchStepConditions = function (stepCondition) {
                        let returnValue = true;
                        if (!$.isEmptyObject(stepCondition) && !$.isEmptyObject(stepCondition.fieldValues)) {

                            let getFieldValueIgnoreCaseSensitive = function (obj, keyName) {
                                let allKeys = Object.keys(obj);
                                let matchKeys = $(allKeys).filter((cIndex, cKey) => cKey.toLowerCase() == keyName.toLowerCase());
                                if (!$.isEmptyObject(matchKeys) && matchKeys.length > 0) {
                                    return obj[matchKeys[0]] + "";
                                }
                            }

                            let fieldValue = getFieldValueIgnoreCaseSensitive($scope.model, stepCondition.fieldName);
                            if (fieldValue) {
                                fieldValue = fieldValue.charAt(0).toUpperCase() + fieldValue.slice(1);
                            }
                            returnValue = stepCondition.fieldValues.indexOf(fieldValue) > -1;
                        }
                        return returnValue;
                    }

                    $scope.init = function () {
                        console.log("Flow old:"+JSON.stringify($scope.flow));
                        var instances = dataService.workflowStatus.workflowInstances;
                        if (instances) {
                            var index = _.findIndex(instances, x => { return x.id == $scope.flow.id });
                            var rounds = [];
                            var round = _.find(instances[index].histories, { stepNumber: $scope.step.stepNumber });
                            if (round) {
                                if (round.stepNumber < instances[index].histories.length) {
                                    round.isStepCompleted = true;
                                }
                                rounds.push(round);
                            } else {
                                if ($scope.allowShowThisStep()) {
                                    rounds.push({})
                                }
                            }
                            $scope.step.rounds = rounds;
                            $scope.step.stepName = $scope.step.stepName.trim();
                        }
                    }
                    $scope.init();
                    $scope.modelChangeStep = { isValid: true };
                    // Attachments
                    $scope.attachments = [];
                    $scope.removeFiles = [];
                    $scope.isBTA = $stateParams.referenceValue.startsWith('BTA-') ? true : false;
                    $scope.skipBTA = { isValid: true };
                    $scope.isITHelpDesk = $rootScope.currentUser && $rootScope.currentUser.isITHelpDesk;
                    if ($scope.isITHelpDesk) {
                        $scope.changeStepWorkflow = async function (model) {
                            enableElementWithRoleITHelpdesk();
                            $scope.directivevalue.newstep = { stepNumber: model.stepNumber };
                            let title = $translate.instant('CHANGE_STEP');
                            $scope.dialogChangeStepDialog.title(title);
                            $scope.dialogChangeStepDialog.open();
                            $rootScope.confirmVoteDialog = $scope.dialogChangeStepDialog;
                            $scope.$emit("closeProcessModal", true);
                        }
                        $scope.dialogChangeStepDialogOpts = {
                            width: "1000px",
                            buttonLayout: "normal",
                            closable: true,
                            modal: true,
                            visible: false,
                            content: "",
                            actions: [{
                                text: $translate.instant('COMMON_BUTTON_OK'),
                                action: function (e) {
                                    if ($scope.modelChangeStep.comment) {
                                        $scope.modelChangeStep.isValid = true;
                                        let dialogChangeStepWorkflow = $rootScope.showConfirmYNDialog("COMMON_NOTIFICATION", "COMMON_SAVE_CONFIRM");
                                        $scope.$apply();
                                        dialogChangeStepWorkflow.bind("close", async function (e) {
                                            if (e.data && e.data.value) {
                                                // upload attachment
                                                if ($scope.attachments.length || $scope.removeFiles.length) {
                                                    let attachmentFilesJson = await mergeAttachmentV2(attachmentService, $scope.oldAttachments, $scope.attachments);
                                                    $scope.modelChangeStep.documents = attachmentFilesJson;
                                                }

                                                var modelChangeStep = {
                                                    CurrentStepNumber: $scope.directivevalue.currentstep.stepNumber,
                                                    NewStepNumber: $scope.directivevalue.newstep.stepNumber,
                                                    InstanceId: $scope.directivevalue.currentstep.instanceId,
                                                    ItemReferenceNumber: $scope.directivevalue.currentstep.itemReferenceNumber
                                                };
                                                let updateDepartmentResponse = await workflowService.getInstance().workflows.changeStepWorkflow(modelChangeStep).$promise;
                                                if (updateDepartmentResponse.isSuccess) {
                                                    // save log
                                                    modelChangeStep.templateId = $scope.directivevalue.currentstep.templateId;
                                                    modelChangeStep.workflowDataStr = $scope.directivevalue.currentstep.workflowDataStr;
                                                    var modelLog = {
                                                        ItemRefereceNumberOrCode: $scope.directivevalue.currentstep.itemReferenceNumber,
                                                        ItemType: $rootScope.getItemTypeByReferenceNumber($scope.directivevalue.currentstep.itemReferenceNumber),
                                                        Type: appSetting.TrackingType.ChangeStepWorkflow,
                                                        DataStr: JSON.stringify(modelChangeStep),
                                                        Comment: $scope.modelChangeStep.comment,
                                                        InstanceId: $scope.directivevalue.currentstep.instanceId,
                                                        Documents: $scope.modelChangeStep.documents,
                                                        RoundNum: $scope.directivevalue.currentstep.roundNum
                                                    };
                                                    let saveLog = await trackingHistoryService.getInstance().trackingHistory.saveTrackingHistory(modelLog).$promise;
                                                    if (saveLog.isSuccess) {
                                                        Notification.success($translate.instant("COMMON_SAVE_SUCCESS"));
                                                        let dialog = $("#dialog").data("kendoDialog");
                                                        dialog.close();
                                                        $('.modal-backdrop').remove();
                                                        $state.reload();
                                                    }
                                                }
                                            }
                                        });
                                    } else {
                                        $scope.modelChangeStep.isValid = false;
                                        $scope.modelChangeStep.errorMessage = $translate.instant('COMMON_COMMENT_VALIDATE');
                                        $scope.$apply();
                                        return false;
                                    }

                                },
                                primary: true
                            }],
                            close: async function (e) {
                                $scope.modelChangeStep.errorMessage = null;
                                $scope.modelChangeStep.isValid = true;
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
                        /*$scope.skipBTA = async function () {
                            let dialogs = $("#dialog_skipBTA").data("kendoDialog");
                            dialogs.title($translate.instant('SkipBTA'))
                            dialogs.open();
                        }*/

                        $scope.dialogSkipBTADialogOpts = {
                            width: "1000px",
                            buttonLayout: "normal",
                            closable: true,
                            modal: true,
                            visible: false,
                            content: "",
                            actions: [{
                                text: $translate.instant('COMMON_BUTTON_OK'),
                                action:  function (e){
                                    if ($scope.skipBTA.comment) {
                                        $scope.skipBTA.isValid = true;
                                        let dialogSkipStepBTA = $rootScope.showConfirmYNDialog("COMMON_NOTIFICATION", "COMMON_SAVE_CONFIRM");
                                        $scope.$apply();
                                        dialogSkipStepBTA.bind("close",async  function (e) {
                                            if (e.data && e.data.value) {
                                                // upload attachment
                                                if ($scope.attachments.length || $scope.removeFiles.length) {
                                                    let attachmentFilesJson = await mergeAttachmentV2(attachmentService, $scope.oldAttachments, $scope.attachments);
                                                    $scope.skipBTA.documents = attachmentFilesJson;
                                                }

                                                $scope.wfVoteModel = {
                                                    itemId: $stateParams.id,
                                                    Comment: $scope.skipBTA.comment,
                                                    Vote: 1
                                                }
                                                let skipBTA = await workflowService.getInstance().workflows.voteAdminBTA($scope.wfVoteModel).$promise;

                                                if (skipBTA.isSuccess) {
                                                    // save log
                                                    skipBTA.templateId = $scope.directivevalue.currentstep.templateId;
                                                    skipBTA.workflowDataStr = $scope.directivevalue.currentstep.workflowDataStr;
                                                    var modelLog = {
                                                        ItemRefereceNumberOrCode: $scope.directivevalue.currentstep.itemReferenceNumber,
                                                        ItemType: $rootScope.getItemTypeByReferenceNumber($scope.directivevalue.currentstep.itemReferenceNumber),
                                                        Type: appSetting.TrackingType.ChangeStepWorkflow,
                                                        DataStr: JSON.stringify(skipBTA),
                                                        Comment: $scope.skipBTA.comment,
                                                        InstanceId: $scope.directivevalue.currentstep.instanceId,
                                                        Documents: $scope.skipBTA.documents,
                                                        RoundNum: $scope.directivevalue.currentstep.roundNum
                                                    };
                                                    let saveLog = await trackingHistoryService.getInstance().trackingHistory.saveTrackingHistory(modelLog).$promise;
                                                    if (saveLog.isSuccess) {
                                                        Notification.success($translate.instant("COMMON_SAVE_SUCCESS"));
                                                        let dialog = $("#dialog_skipBTA").data("kendoDialog");
                                                        dialog.close();
                                                        $('.modal-backdrop').remove();
                                                        $state.reload();
                                                    }
                                                }
                                            }
                                        });
                                    }else {
                                        $scope.skipBTA.isValid = false;
                                        $scope.skipBTA.errorMessage = $translate.instant('COMMON_COMMENT_VALIDATE');
                                        $scope.$apply();
                                        return false;
                                    }
                                },
                                primary: true
                            }],
                            close: async function (e) {
                                $scope.skipBTA.errorMessage = null;
                                $scope.skipBTA.isValid = true;
                                $scope.$emit("closeProcessModal", false);
                            }
                        };
                    }
                   
                }
            ],
        };
    },
]);