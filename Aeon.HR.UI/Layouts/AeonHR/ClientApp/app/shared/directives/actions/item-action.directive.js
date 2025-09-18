var ssgApp = angular.module('ssg.directiveActionsModule', []);
ssgApp.directive("actionItems", [
    function () {
        return {
            restrict: "E",
            templateUrl: "ClientApp/app/shared/directives/actions/item-action.template.html?v=" + edocV,
            scope: {
                model: "=",
                form: "=",
                save: "=",
                setRoom: "="
            },
            controller: ["$rootScope", "$scope", "$cacheFactory", "dataService", "workflowService", "permissionService", "Notification", "$translate", "$timeout",
                async function ($rootScope, $scope, $cacheFactory, dataService, workflowService, permissionService, Notification, $translate, $timeout) {
                    $scope.voteModel = { isValid: true };
                    $scope.revokeModel = { isValid: true };
                    $scope.voteDialog = {};
                    $scope.revokeDialog = {};
                    $scope.status = dataService.workflowStatus;
                    $scope.perm = dataService.permission;
                    $scope.perm = 0;
                    $scope.isLoaded = false;
                    $scope.approveFieldText = '';
                    setTimeout(() => {
                        $scope.isLoaded = true;
                        $scope.$apply();
                    }, 1000);
                    $scope.$watch("model.id", async function () {
                        if ($scope.model && $scope.model.id) {
                            await $scope.init();
                        }
                    });

                    $scope.hasPerm = function (right) {
                        return (right & dataService.permission.right) == right;
                    }
                    $scope.init = async function () {
                        $scope.voteModel.itemId = $scope.model.id;
                        $scope.revokeModel.itemId = $scope.model.id;
                        //Load workflow status
                        var result = await workflowService.getInstance().workflows.getWorkflowStatus({ itemId: $scope.model.id }).$promise;

                        $timeout(function () {
                            $scope.approveFieldText = ($scope.model.isRequestToChange && $scope.status.approveFieldText == 'Room Organization') ? 'Confirm' : $scope.status.approveFieldText;
                        }, 0);
                        if (result.isSuccess && !$.isEmptyObject(result.object)) {
                            $scope.$parent.allowToVote = result.object.allowToVote;
                        }
                        $scope.model.status = result.object.currentStatus;
                        angular.copy(result.object, dataService.workflowStatus);
                        var permResult = await permissionService.getInstance().permission.getPerm({ itemId: $scope.model.id }).$promise;
                        dataService.permission.right = permResult.object;
                        //Permission
                        if ((2 & dataService.permission.right) == 2) {
                            disableElement(false);
                        } else {
                            disableElement(true);
                        }
                        //REstricted Edit
                        if (dataService.workflowStatus.restrictedProperties) {
                            //check for RTH
                            setTimeout(function () {
                                enableRestrictedProperties(result.object.restrictedProperties, $scope.model.replacementFor);
                            }, 200);
                        }
                        //Load permission scope
                        $scope.$apply();
                        $scope.$emit('workflowStatus', dataService.workflowStatus);
                        await setTimeout(() => {
                            var $httpDefaultCache = $cacheFactory.get('$http');
                            $httpDefaultCache.removeAll();
                        }, 500);
                    }

                    $scope.submit = async function (workflowId, actionButtonName = '') {
                        CheckIsStore($scope);
                        $scope.revokeModel.workflowId = workflowId;
                        if ($scope.status && $scope.status.currentStatus == 'Completed') {
                            if (actionButtonName == 'Revoke') {
                                $scope.revokeModel.errorMessage = '';
                                $scope.revokeModel.comment = '';
                                let errorRanges_Deadline = validateDateRange_RevokeDeadline($rootScope.salaryDayConfiguration);
                                let errorRanges_PeriodClose = validateDateRange_PeriodClosed($rootScope.salaryDayConfiguration, $scope);
                                let allowRevokeLeaveApplication = true;
                                if (!$.isEmptyObject($scope.model.referenceNumber) && ($scope.model.referenceNumber.indexOf("LEA-") == 0 || $scope.model.referenceNumber.indexOf("SHI-") == 0)) {
                                    allowRevokeLeaveApplication = await $scope.$parent.checkAllowRevoke();
                                }

                                if (!allowRevokeLeaveApplication || errorRanges_Deadline.length > 0 || errorRanges_PeriodClose.length > 0) {
                                    if (errorRanges_Deadline.length > 0) {
                                        Notification.error(errorRanges_Deadline[0].message);
                                    }
                                    else if (errorRanges_PeriodClose.length > 0) {
                                        Notification.error(errorRanges_PeriodClose[0].message);
                                    }
                                }
                                else {
                                    if ($scope.model.referenceNumber.indexOf("BTA-") == 0) {
                                        $scope.$emit("revokeBookingFlight", $scope.revokeModel);
                                        //await $scope.init();
                                    }
                                    else {
                                        $scope.revokeDialog.title($translate.instant('COMMON_BUTTON_REVOKE'));
                                        $scope.revokeDialog.open();
                                        $rootScope.confirmVoteDialog = $scope.revokeDialog;
                                    }
                                }

                            } else {
                                $scope.$emit(actionButtonName, { voteModel: $scope.revokeModel });
                            }
                        } else {
                            var result = await $scope.save($scope.form, dataService.permission.right, actionButtonName);
                            if (result && result.isSuccess) {
                                var result = await workflowService.getInstance().workflows.startWorkflow($scope.revokeModel, null).$promise;
                                if (result.messages.length == 0) {
                                    $scope.$emit("startWorkflow", $scope.model.id);
                                    Notification.success($translate.instant('COMMON_WORKFLOW_STARTED'));
                                    await $scope.init();
                                    $scope.$apply();
                                } else {
                                    if (result.messages && result.messages.length) {
                                        Notification.error(result.messages[0]);
                                    }
                                }
                            }
                        }
                    }
                    async function revoke() {
                        if ($scope.status && $scope.status.currentStatus == 'Completed') {
                            var result = await workflowService.getInstance().workflows.startWorkflow($scope.revokeModel, null).$promise;
                            if (result.messages.length == 0) {
                                Notification.success($translate.instant('COMMON_WORKFLOW_STARTED'));
                                await $scope.init();
                                $scope.$apply();
                            } else {
                                if (result.messages && result.messages.length) {
                                    Notification.error(result.messages[0]);
                                }
                            }
                        }
                    }
                    var ignoreCustomEvent = ["viewBookingFlights"];
                    $scope.vote = async function (voteId) {
                        $scope.voteModel.vote = voteId;
                        var title = "";
                        if (voteId == 1) {
                            title = $translate.instant($scope.status.approveFieldText);
                        } else if (voteId == 2) {
                            title = $translate.instant($scope.status.rejectFieldText);
                        } else if (voteId == 3) {
                            title = $translate.instant("COMMON_BUTTON_REQUEST_TO_CHANGE");
                        } else {
                            // title = "Cancel";                           
                            title = $translate.instant('COMMON_BUTTON_CANCEL');
                        }
                        if (voteId == 3 && $scope.status.isCustomRequestToChange) {
                            $scope.$emit('customRequestToChange', { voteModel: $scope.voteModel });
                        } else {
                            if (!ignoreCustomEvent.includes($scope.status.customEventKey) && ($scope.status.isCustomEvent && voteId == 1 || $scope.status.approveFieldText == 'Change Business Trip')) {
                                if ($scope.status.customEventKey == null || $scope.status.customEventKey == undefined || $scope.status.approveFieldText == 'Change Business Trip') {
                                    $scope.$emit($scope.status.approveFieldText, { voteModel: $scope.voteModel }, $scope);
                                }
                                else {
                                    $scope.$emit($scope.status.customEventKey, { voteModel: $scope.voteModel }, $scope);
                                }
                                console.log($scope.status);
                            } else {
                                $scope.voteDialog.title(title);
                                $scope.voteDialog.open();
                                $rootScope.confirmVoteDialog = $scope.voteDialog;
                            }
                        }

                    }
                    $scope.dSave = async function () {
                        await $scope.save($scope.form);
                    }
                    $scope.cancel = function () {
                        $rootScope.cancel();
                        //$rootScope.gotoDashboard();
                    }

                    $scope.delete = async function () {
                        $scope.dialog = $rootScope.showConfirmDelete($translate.instant('COMMON_BUTTON_DELETE'), $translate.instant('COMMON_DELETE_VALIDATE'), $translate.instant('COMMON_BUTTON_CONFIRM'));
                        $scope.dialog.bind("close", async function (e) {
                            if (e.data && e.data.value) {
                                var result = await workflowService.getInstance().common.delete({ id: $scope.model.id }, null).$promise;
                                if (result && result.isSuccess) {
                                    Notification.success($translate.instant('COMMON_DELETE_SUCCESS'));
                                    $rootScope.gotoDashboard();
                                } else {
                                    Notification.success($translate.instant('COMMON_DELETE_SUCCESS'));
                                }
                            }
                        });
                    }


                    $scope.voteDialogOpts = {
                        width: "600px",
                        buttonLayout: "normal",
                        closable: true,
                        modal: true,
                        visible: false,
                        content: "",
                        actions: [{
                            text: $translate.instant('COMMON_BUTTON_OK'),
                            action: function (e) {
                                if ($scope.voteModel.vote == 1 || $scope.voteModel.comment) {
                                    $scope.voteModel.isValid = true;
                                    if ($scope.voteModel.vote == 1) {

                                        // if (!dataService.workflowStatus.ignoreValidation && (dataService.workflowStatus.restrictedProperties || (2 & dataService.permission.right) == 2)) {
                                        if (!dataService.workflowStatus.ignoreValidation) {
                                            if (dataService.workflowStatus.restrictedProperties) {
                                                var errorFields = isRestrictedPropertiesEmpty(dataService.workflowStatus.restrictedProperties);
                                                $scope.voteModel.errorMessage = '';
                                                if (errorFields.length > 0) {
                                                    $scope.voteModel.isValid = false;
                                                    $scope.voteModel.errorMessage = "Please fill the following fields in the Form: " + errorFields.join(", ");
                                                    $scope.$apply();
                                                    return false;
                                                }
                                            }
                                            $scope.save($scope.form, dataService.permission.right).then(function (result) {
                                                if (result && result.isSuccess) {
                                                    workflowService.getInstance().workflows.vote($scope.voteModel).$promise.then(function (result) {
                                                        if (result.messages.length == 0) {
                                                            // Notification.success("Workflow has been processed");
                                                            Notification.success($translate.instant('COMMON_WORKFLOW_PROCESSED'));
                                                            $scope.init();
                                                        } else {
                                                            if (result.messages && result.messages.length) {
                                                                Notification.error(result.messages[0]);
                                                            }
                                                            $scope.voteDialog.close();
                                                        }
                                                    });
                                                } else {
                                                    if (result.messages && result.messages.length) {
                                                        Notification.error(result.messages[0]);
                                                    }
                                                    $scope.voteDialog.close();
                                                }
                                                $scope.$apply();
                                            });
                                        } else {
                                            workflowService.getInstance().workflows.vote($scope.voteModel).$promise.then(function (result) {
                                                if (result.messages.length == 0) {
                                                    // Notification.success("Workflow has been processed");
                                                    Notification.success($translate.instant('COMMON_WORKFLOW_PROCESSED'));
                                                    $scope.init();
                                                } else {
                                                    if (result.messages && result.messages.length) {
                                                        Notification.error(result.messages[0]);
                                                    }
                                                    $scope.voteDialog.close();
                                                }
                                            });
                                        }
                                    } else {
                                        workflowService.getInstance().workflows.vote($scope.voteModel).$promise.then(function (result) {
                                            if (result.messages.length == 0) {
                                                // Notification.success("Workflow has been processed");
                                                Notification.success($translate.instant('COMMON_WORKFLOW_PROCESSED'));
                                                $scope.init();
                                            } else {
                                                if (result.messages && result.messages.length) {
                                                    Notification.error(result.messages[0]);
                                                }
                                                $scope.voteDialog.close();
                                            }
                                        });
                                    }
                                }
                                else {
                                    $scope.voteModel.isValid = false;
                                    // $scope.voteModel.errorMessage = "Please enter comment";
                                    $scope.voteModel.errorMessage = $translate.instant('COMMON_COMMENT_VALIDATE');
                                    $scope.$apply();
                                    return false;
                                }
                            },
                            primary: true
                        }],
                        close: async function (e) {
                        }
                    };
                    $scope.revokeDialogOpts = {
                        width: "600px",
                        buttonLayout: "normal",
                        closable: true,
                        modal: true,
                        visible: false,
                        content: "",
                        actions: [{
                            text: $translate.instant('COMMON_BUTTON_OK'),
                            action: function (e) {
                                if ($scope.revokeModel.comment) {
                                    $scope.revokeModel.errorMessage = '';
                                    revoke();
                                } else {
                                    $scope.revokeModel.isValid = false;
                                    $scope.revokeModel.errorMessage = $translate.instant('COMMON_COMMENT_VALIDATE');
                                    $scope.$apply();
                                    return false;
                                }
                            },
                            primary: true
                        }],
                        close: async function (e) {
                        }
                    };

                    $scope.$on('$destroy', function () {
                        dataService.workflowStatus = {};
                        dataService.permission = {};
                    });

                }
            ],
        };
    },
]);