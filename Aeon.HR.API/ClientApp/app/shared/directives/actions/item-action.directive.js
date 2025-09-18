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
            controller: ["$rootScope", "$scope", "$cacheFactory", "dataService", "workflowService", "permissionService", "Notification", "$translate", "$timeout", "trackingHistoryService", "appSetting", "attachmentService", "$state", "$stateParams", "ssgexService", "itService", "$sce", "cbService", "settingService",
                async function ($rootScope, $scope, $cacheFactory, dataService, workflowService, permissionService, Notification, $translate, $timeout, trackingHistoryService, appSetting, attachmentService, $state, $stateParams, ssgexService, itService, $sce, cbService, settingService) {
                    let statusNotIn = ['Rejected', 'Cancelled', 'Out Of Period', 'Completed'];
                    let editStatusNotIn = ['Rejected', 'Cancelled', 'Out Of Period'];
                    $scope.voteModel = { isValid: true };
                    $scope.updateStatusModel = { isValid: true };
                    $scope.updateModel = { isValid: true };
                    $scope.oldModel = {};
                    $scope.isEditing = false;
                    $scope.revokeModel = { isValid: true };
                    $scope.voteDialog = {};
                    $scope.updateStatusDialog = {};
                    $scope.updateModelDialog = {};
                    $scope.revokeDialog = {};
                    $scope.status = dataService.workflowStatus;
                    $scope.perm = dataService.permission;
                    $rootScope.waringOTs = [];
                    $scope.perm = 0;
                    $scope.isLoaded = false;
                    $scope.approveFieldText = '';
                    $scope.WorkflowName = '';
                    // Attachments
                    $scope.attachments = [];
                    $scope.removeFiles = [];
                    $scope.allHolidays = null;
                    setTimeout(() => {
                        $scope.isLoaded = true;
                        $scope.$apply();
                    }, 1000);
                    $scope.$watch("model.id", async function () {
                        if ($scope.model && $scope.model.id) {
                            await $scope.init();
                        }
                    });
                    $scope.isShowButtonITHelpdesk = function () {
                        return $rootScope.currentUser && $rootScope.currentUser.isITHelpDesk && $scope.model && $scope.model.id;
                    }
                    $scope.isCompleteOrCanceled = function () {
                        return !statusNotIn.includes($scope.model.status)//Requested To Change?
                    }

                    $scope.isShowButtonEditITHelpdesk = function () {
                        if ($scope.model && $scope.model.referenceNumber && $scope.model.referenceNumber.startsWith("BTA-")) {
                            editStatusNotIn.push("Completed");
                        }
                        return $rootScope.currentUser && $rootScope.currentUser.isITHelpDesk && $scope.model && $scope.model.id && !editStatusNotIn.includes($scope.model.status);
                    }

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
                        $scope.model.status = result.object && result.object.currentStatus ? result.object.currentStatus : '';
                        if (result.object.workflowName !== null)
                            $scope.WorkflowName = result.object.workflowName;
                        angular.copy(result.object, dataService.workflowStatus);

                        // clone model before updated
                        if ($scope.model) $scope.oldModel = _.cloneDeep($scope.model);
                        else if ($scope.form) $scope.oldModel = _.cloneDeep($scope.form);

                        //Load trackingHistory
                        var resultTrackingHistory = await trackingHistoryService.getInstance().trackingHistory.getTrackingHistoryByItemId({ ItemId: $scope.model.id }).$promise;
                        if (resultTrackingHistory.isSuccess) {
                            //resultTrackingHistory.object.forEach(x => { x.created = moment(x.created).format(appSetting.longDateFormat) });
                            // resultTrackingHistory.object.forEach(x => { x.documents = (x.documents !== '' && JSON.parse(x.documents)) ? JSON.parse(x.documents) : [] });
                            angular.copy(resultTrackingHistory.object, dataService.trackingHistory);
                        }

                        var permResult = await permissionService.getInstance().permission.getPerm({ itemId: $scope.model.id }).$promise;
                        dataService.permission.right = permResult.object;
                        //Permission
                        if ((2 & dataService.permission.right) == 2) {
                            disableElement(false);
                        } else {
                            disableElement(true);
                        }
                        // enable chuc nang cua roleIThelpsesk
                        if ($rootScope.currentUser && $rootScope.currentUser.isITHelpDesk) {
                            enableElementWithRoleITHelpdesk();
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
                        //$rootScope.loadingDialog(null, true);
                        $rootScope.showLoading();
                        CheckIsStore($scope);
                        $scope.revokeModel.workflowId = workflowId;
                        if ($scope.status && $scope.status.currentStatus == 'Completed') {
                            let errorRange_existRE = [];
                            if (actionButtonName == 'Revoke') {
                                $scope.revokeModel.errorMessage = '';
                                $scope.revokeModel.comment = '';
                                let errorRanges_Deadline = validateDateRange_RevokeDeadline($rootScope.salaryDayConfiguration);
                                let errorRanges_PeriodClose = [];
                                let allowRevokeLeaveApplication = true;
                                if (!$.isEmptyObject($scope.model.referenceNumber) && ($scope.model.referenceNumber.indexOf("BTA-") == 0)) {
                                    errorRanges_PeriodClose = [];  // not validate period close BTA
                                    if (!$.isEmptyObject($scope.model.referenceNumberRE) && $scope.model.isCheckRe)
                                        errorRange_existRE.push($scope.model.referenceNumberRE)
                                }
                                if (!$.isEmptyObject($scope.model.referenceNumber) && ($scope.model.referenceNumber.indexOf("LEA-") == 0 || $scope.model.referenceNumber.indexOf("SHI-") == 0)
                                    || $scope.model.referenceNumber.indexOf("MIS-") == 0 || $scope.model.referenceNumber.indexOf("OVE-") == 0) {
                                    allowRevokeLeaveApplication = await $scope.$parent.checkAllowRevoke();
                                }

                                if (!allowRevokeLeaveApplication || errorRanges_Deadline.length > 0 || errorRanges_PeriodClose.length > 0 || errorRange_existRE.length > 0) {
                                    if (errorRanges_Deadline.length > 0) {
                                        Notification.error(errorRanges_Deadline[0].message);
                                    }
                                    else if (errorRanges_PeriodClose.length > 0) {
                                        Notification.error(errorRanges_PeriodClose[0].message);
                                    } else if (errorRange_existRE.length > 0) {
                                        Notification.error($translate.instant('BTA_TRIP_NOT_REVOKE_1') + ": " + errorRange_existRE[0]);
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
                                // BTA Change business Trip
                                if (!$.isEmptyObject($scope.model.referenceNumber) && ($scope.model.referenceNumber.indexOf("BTA-") == 0)) {
                                    errorRanges_PeriodClose = [];  // not validate period close BTA
                                    if (!$.isEmptyObject($scope.model.referenceNumberRE) && $scope.model.isCheckRe)
                                        errorRange_existRE.push($scope.model.referenceNumberRE)
                                }
                                if (errorRange_existRE.length > 0) {
                                    Notification.error($translate.instant('BTA_TRIP_NOT_REVOKE_1') + ": " + errorRange_existRE[0]);
                                } else {
                                    $scope.$emit(actionButtonName, { voteModel: $scope.revokeModel });
                                }
                            }
                        } else {
                            if ($scope.model) {
                                var currentItem = await workflowService.getInstance().common.getItemById({ Id: $scope.model.id }, null).$promise
                                if ($scope.model.status && currentItem.object.status && currentItem.object.status !== $scope.model.status) {
                                    //$rootScope.loadingDialog(null, false);
                                    $rootScope.hideLoading();
                                    Notification.error($translate.instant('STATUS_ALREADY_CHANGE'));
                                    return;
                                }
                            }
                            if ($scope.form && $scope.form.overtimeList
                                && (($scope.model && $scope.model.referenceNumber && $scope.model.referenceNumber.startsWith("OVE")) || ($scope.form.overtimeList.length > 0))) {
                                let content = [];
                                let currentShiftCodeList = await getShiftCodeAllUser($scope.form.overtimeList, $scope.model.userSAPCode);
                                for (const x of $scope.form.overtimeList) {
                                    x.date = moment(x.dateOfOT, "DD/MM/YYYY").toDate();
                                    const userSAPCode = (x.sapCode == null ? $scope.model.userSAPCode : x.sapCode);
                                    var currentShiftCode = _.find(currentShiftCodeList, item => {
                                        let otDate = new Date(x.date);
                                        if (isNaN(otDate.getTime()) && x.dateOfOT) {
                                            otDate = converStringToDate(x.dateOfOT);
                                        }
                                        return item.sapCode == userSAPCode && isSameDay(otDate, new Date(item.shiftExchangeDate));
                                    });

                                    await loadHolidays();
                                    var check_isHoliday = $scope.isHoliday(x.dateOfOT);

                                    let hours = 0;
                                    if (currentShiftCode) {
                                        var otHours = parseFloat(x.OTProposalHour);
                                        if (x.actualOTHour) {
                                            otHours = parseFloat(x.actualOTHour);
                                        }
                                        if (currentShiftCode.currentCode != "PRD" && check_isHoliday == false) {
                                            if (currentShiftCode.currentCode.startsWith("V")) {
                                                hours = calculateHoursOT(currentShiftCode.currentCode);
                                                otHours += parseFloat(hours);

                                            }
                                        }

                                        if (currentShiftCode.currentCode == "PRD" || check_isHoliday == true) {
                                            if (otHours > 12) {
                                                let validateContent = "<br>- <b>" + (x.sapCode == null ? $scope.model.userSAPCode : x.sapCode) + "</b>: ";
                                                validateContent = validateContent.concat(" ", x.dateOfOT)
                                                validateContent = validateContent.concat(" ", $translate.instant('CANNOT_EXCEED_12H'))
                                                content.push(validateContent);
                                            }
                                        }
                                        else {
                                            if (otHours > 12) {
                                                let validateContent = "<br>- <b>" + (x.sapCode == null ? $scope.model.userSAPCode : x.sapCode) + "</b>: ";
                                                validateContent = validateContent.concat(" ", x.dateOfOT)
                                                validateContent = validateContent.concat(" ", $translate.instant('CANNOT_EXCEED_4H'))
                                                content.push(validateContent);
                                            }
                                        }
                                    }
                                }
                                if (content.length > 0) {
                                    let message =
                                        "<b>" + $translate.instant('COMMON_WARNING').concat(" ", $translate.instant('OVERTIME_APPLICATION_OT_PROPOSAL_HOURS')) + ": </b>" +
                                        content.join();
                                    $scope.$apply();
                                    $scope.dialog = $rootScope.showConfirmWarning(
                                        $translate.instant('COMMON_WARNING'),
                                        message,
                                        $translate.instant('COMMON_BUTTON_CONFIRM'),
                                        true
                                    );
                                    $scope.$emit('waringOTs', { message });
                                    $scope.dialog.bind("close", async function (e) {
                                        if (e.data && e.data.value) {
                                            var errorsinMonth = await $scope.errorMessageOTHourAMonth();
                                            if (errorsinMonth.length > 0) {
                                                message =
                                                    "<b>" + ("", $translate.instant('OVERTIME_MANAGEMENT_USEREXCEDED_LIMIT_MONTH')) + ": </b>" +
                                                    errorsinMonth.join();
                                                $scope.$apply();
                                                $scope.dialog = $rootScope.showConfirmWarning(
                                                    $translate.instant('COMMON_WARNING'),
                                                    message,
                                                    $translate.instant('COMMON_BUTTON_CONFIRM'),
                                                    true
                                                );
                                                $scope.$emit('waringOTs', { message });
                                            }
                                            else {
                                                var errosWarning = await $scope.errorMessageOvertimeRemainBalance();
                                                if (errosWarning.length > 0) {
                                                    message =
                                                        "<b>" + ("", $translate.instant('OVERTIME_MANAGEMENT_USEREXCEDED_LIMIT')) + ": </b>" +
                                                        errosWarning.join();
                                                    $scope.$apply();
                                                    $scope.dialog = $rootScope.showConfirmWarning(
                                                        $translate.instant('COMMON_WARNING'),
                                                        message,
                                                        $translate.instant('COMMON_BUTTON_CONFIRM'),
                                                        true
                                                    );
                                                    $scope.$emit('waringOTs', { message });
                                                    /*$scope.dialog.bind("close", async function (e) {
                                                        if (e.data && e.data.value) {
                                                            var result = await $scope.save($scope.form, dataService.permission.right, actionButtonName);
                                                            if (result && result.isSuccess) {
                                                                var result = await workflowService.getInstance().workflows.startWorkflow($scope.revokeModel, null).$promise;
                                                                if (result.messages.length == 0) {
                                                                    $scope.$emit("startWorkflow", $scope.model.id);
                                                                    Notification.success($translate.instant('COMMON_WORKFLOW_STARTED'));
                                                                    try {
                                                                        if ($scope.oldModel && $scope.oldModel.referenceNumber && $scope.oldModel.referenceNumber.startsWith("RES-")) {
                                                                            let dataStrModel = {
                                                                                SubmitDate: $scope.oldModel.submitDate,
                                                                                ContractTypeCode: $scope.oldModel.contractTypeCode,
                                                                                IsExpiredLaborContractDate: $scope.oldModel.isExpiredLaborContractDate,
                                                                                OfficialResignationDate: $scope.oldModel.officialResignationDate,
                                                                                RoundNum: result.object.roundNum,
                                                                            }
                                                                            let modelLog = {
                                                                                ItemRefereceNumberOrCode: $scope.form.referenceNumber,
                                                                                ItemType: $rootScope.getItemTypeByReferenceNumber($scope.oldModel.referenceNumber),
                                                                                Type: appSetting.TrackingType.SendData,
                                                                                InstanceId: result.object.id,
                                                                                RoundNum: result.object.roundNum,
                                                                                DataStr: JSON.stringify(dataStrModel)
                                                                            };
                                                                            await trackingHistoryService.getInstance().trackingHistory.saveTrackingHistory(modelLog).$promise;
                                                                        }
                                                                    } catch { }
                                                                    $rootScope.loadingDialog(null, false);
                                                                    await $scope.init();
                                                                    $scope.$apply();
                                                                } else {
                                                                    if (result.messages && result.messages.length) {
                                                                        $rootScope.loadingDialog(null, false);
                                                                        Notification.error(result.messages[0]);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    });*/
                                                } else {
                                                    var result = await $scope.save($scope.form, dataService.permission.right, actionButtonName);
                                                    if (result && result.isSuccess) {
                                                        var result = await workflowService.getInstance().workflows.startWorkflow($scope.revokeModel, null).$promise;
                                                        if (result.messages.length == 0) {
                                                            $scope.$emit("startWorkflow", $scope.model.id);
                                                            Notification.success($translate.instant('COMMON_WORKFLOW_STARTED'));
                                                            try {
                                                                if ($scope.oldModel && $scope.oldModel.referenceNumber && $scope.oldModel.referenceNumber.startsWith("RES-")) {
                                                                    let dataStrModel = {
                                                                        SubmitDate: $scope.oldModel.submitDate,
                                                                        ContractTypeCode: $scope.oldModel.contractTypeCode,
                                                                        IsExpiredLaborContractDate: $scope.oldModel.isExpiredLaborContractDate,
                                                                        OfficialResignationDate: $scope.oldModel.officialResignationDate,
                                                                        RoundNum: result.object.roundNum,
                                                                    }
                                                                    let modelLog = {
                                                                        ItemRefereceNumberOrCode: $scope.form.referenceNumber,
                                                                        ItemType: $rootScope.getItemTypeByReferenceNumber($scope.oldModel.referenceNumber),
                                                                        Type: appSetting.TrackingType.SendData,
                                                                        InstanceId: result.object.id,
                                                                        RoundNum: result.object.roundNum,
                                                                        DataStr: JSON.stringify(dataStrModel)
                                                                    };
                                                                    await trackingHistoryService.getInstance().trackingHistory.saveTrackingHistory(modelLog).$promise;
                                                                }
                                                            } catch { }
                                                            //$rootScope.loadingDialog(null, false);
                                                            $rootScope.hideLoading();
                                                            await $scope.init();
                                                            $scope.$apply();
                                                            $state.reload();
                                                        } else {
                                                            if (result.messages && result.messages.length) {
                                                                //$rootScope.loadingDialog(null, false);
                                                                $rootScope.hideLoading();
                                                                Notification.error(result.messages[0]);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    });
                                } else {
                                    var errorsinMonth = await $scope.errorMessageOTHourAMonth();
                                    if (errorsinMonth.length > 0) {
                                        message =
                                            "<b>" + ("", $translate.instant('OVERTIME_MANAGEMENT_USEREXCEDED_LIMIT_MONTH')) + ": </b>" +
                                            errorsinMonth.join();
                                        $scope.$apply();
                                        $scope.dialog = $rootScope.showConfirmWarning(
                                            $translate.instant('COMMON_WARNING'),
                                            message,
                                            $translate.instant('COMMON_BUTTON_CONFIRM'),
                                            true
                                        );
                                        $scope.$emit('waringOTs', { message });
                                    }
                                    else {
                                        var errosWarning = await $scope.errorMessageOvertimeRemainBalance();
                                        if (errosWarning.length > 0) {
                                            message =
                                                "<b>" + ("", $translate.instant('OVERTIME_MANAGEMENT_USEREXCEDED_LIMIT')) + ": </b>" +
                                                errosWarning.join();
                                            $scope.$apply();
                                            $scope.dialog = $rootScope.showConfirmWarning(
                                                $translate.instant('COMMON_WARNING'),
                                                message,
                                                $translate.instant('COMMON_BUTTON_CONFIRM'),
                                                true
                                            );
                                            $scope.$emit('waringOTs', { message });
                                            /*$scope.dialog.bind("close", async function (e) {
                                                if (e.data && e.data.value) {
                                                    var result = await $scope.save($scope.form, dataService.permission.right, actionButtonName);
                                                    if (result && result.isSuccess) {
                                                        var result = await workflowService.getInstance().workflows.startWorkflow($scope.revokeModel, null).$promise;
                                                        if (result.messages.length == 0) {
                                                            $scope.$emit("startWorkflow", $scope.model.id);
                                                            Notification.success($translate.instant('COMMON_WORKFLOW_STARTED'));
    
                                                            try {
                                                                if ($scope.oldModel && $scope.oldModel.referenceNumber && $scope.oldModel.referenceNumber.startsWith("RES-")) {
                                                                    let dataStrModel = {
                                                                        SubmitDate: $scope.oldModel.submitDate,
                                                                        ContractTypeCode: $scope.oldModel.contractTypeCode,
                                                                        IsExpiredLaborContractDate: $scope.oldModel.isExpiredLaborContractDate,
                                                                        OfficialResignationDate: $scope.oldModel.officialResignationDate,
                                                                        RoundNum: result.object.roundNum,
                                                                    }
                                                                    let modelLog = {
                                                                        ItemRefereceNumberOrCode: $scope.form.referenceNumber,
                                                                        ItemType: $rootScope.getItemTypeByReferenceNumber($scope.oldModel.referenceNumber),
                                                                        Type: appSetting.TrackingType.SendData,
                                                                        InstanceId: result.object.id,
                                                                        RoundNum: result.object.roundNum,
                                                                        DataStr: JSON.stringify(dataStrModel)
                                                                    };
                                                                    await trackingHistoryService.getInstance().trackingHistory.saveTrackingHistory(modelLog).$promise;
                                                                }
                                                            } catch { }
    
                                                            $rootScope.loadingDialog(null, false);
                                                            await $scope.init();
                                                            $scope.$apply();
                                                        } else {
                                                            if (result.messages && result.messages.length) {
                                                                $rootScope.loadingDialog(null, false);
                                                                Notification.error(result.messages[0]);
                                                            }
                                                        }
                                                    }
                                                }
                                            });*/
                                        } else {
                                            var result = await $scope.save($scope.form, dataService.permission.right, actionButtonName);
                                            if (result && result.isSuccess) {
                                                var result = await workflowService.getInstance().workflows.startWorkflow($scope.revokeModel, null).$promise;
                                                if (result.messages.length == 0) {
                                                    $scope.$emit("startWorkflow", $scope.model.id);
                                                    Notification.success($translate.instant('COMMON_WORKFLOW_STARTED'));

                                                    try {
                                                        if ($scope.oldModel && $scope.oldModel.referenceNumber && $scope.oldModel.referenceNumber.startsWith("RES-")) {
                                                            let dataStrModel = {
                                                                SubmitDate: $scope.oldModel.submitDate,
                                                                ContractTypeCode: $scope.oldModel.contractTypeCode,
                                                                IsExpiredLaborContractDate: $scope.oldModel.isExpiredLaborContractDate,
                                                                OfficialResignationDate: $scope.oldModel.officialResignationDate,
                                                                RoundNum: result.object.roundNum,
                                                            }
                                                            let modelLog = {
                                                                ItemRefereceNumberOrCode: $scope.form.referenceNumber,
                                                                ItemType: $rootScope.getItemTypeByReferenceNumber($scope.oldModel.referenceNumber),
                                                                Type: appSetting.TrackingType.SendData,
                                                                InstanceId: result.object.id,
                                                                RoundNum: result.object.roundNum,
                                                                DataStr: JSON.stringify(dataStrModel)
                                                            };
                                                            await trackingHistoryService.getInstance().trackingHistory.saveTrackingHistory(modelLog).$promise;
                                                        }
                                                    } catch { }

                                                    //$rootScope.loadingDialog(null, false);
                                                    $rootScope.hideLoading();
                                                    await $scope.init();
                                                    $scope.$apply();
                                                    $state.reload();
                                                } else {
                                                    if (result.messages && result.messages.length) {
                                                        //$rootScope.loadingDialog(null, false);
                                                        $rootScope.hideLoading();
                                                        Notification.error(result.messages[0]);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            } else {
                                var result = await $scope.save($scope.form, dataService.permission.right, actionButtonName);
                                if (result && result.isSuccess) {
                                    var result = await workflowService.getInstance().workflows.startWorkflow($scope.revokeModel, null).$promise;
                                    if (result.messages.length == 0) {
                                        $scope.$emit("startWorkflow", $scope.model.id);
                                        Notification.success($translate.instant('COMMON_WORKFLOW_STARTED'));

                                        try {
                                            if ($scope.oldModel && $scope.oldModel.referenceNumber && $scope.oldModel.referenceNumber.startsWith("RES-")) {
                                                let dataStrModel = {
                                                    SubmitDate: $scope.oldModel.submitDate,
                                                    ContractTypeCode: $scope.oldModel.contractTypeCode,
                                                    IsExpiredLaborContractDate: $scope.oldModel.isExpiredLaborContractDate,
                                                    OfficialResignationDate: $scope.oldModel.officialResignationDate,
                                                    RoundNum: result.object.roundNum,
                                                }
                                                let modelLog = {
                                                    ItemRefereceNumberOrCode: $scope.form.referenceNumber,
                                                    ItemType: $rootScope.getItemTypeByReferenceNumber($scope.oldModel.referenceNumber),
                                                    Type: appSetting.TrackingType.SendData,
                                                    InstanceId: result.object.id,
                                                    RoundNum: result.object.roundNum,
                                                    DataStr: JSON.stringify(dataStrModel)
                                                };
                                                await trackingHistoryService.getInstance().trackingHistory.saveTrackingHistory(modelLog).$promise;
                                            }
                                        } catch { }

                                        //$rootScope.loadingDialog(null, false);
                                        $rootScope.hideLoading();
                                        await $scope.init();
                                        $scope.$apply();
                                        $state.reload();

                                    } else {
                                        if (result.messages && result.messages.length) {
                                            //$rootScope.loadingDialog(null, false);
                                            $rootScope.hideLoading();
                                            Notification.error(result.messages[0]);
                                        }
                                    }
                                }
                            }
                        }
                        //$rootScope.loadingDialog(null, false);
                        $rootScope.hideLoading();
                    }

                    async function getShiftCodeAllUser(overtimeApplicationList, userSapcCode) {
                        var _returnValue = [];
                        let args = [];
                        for (const x of overtimeApplicationList) {
                            let exchangeDate = x.date;
                            if (x.dateOfOT) {
                                // line not save
                                exchangeDate = converStringToDate(x.dateOfOT);
                            }
                            args.push({
                                deptId: null,
                                divisionId: null,
                                sapCode: (x.sapCode == null ? userSapcCode : x.sapCode),
                                shiftExchangeDate: exchangeDate
                            });
                        }

                        let resCurrentShift = await cbService.getInstance().shiftExchange.getCurrentShiftCodeFromShiftPlan(args).$promise;
                        if (resCurrentShift.isSuccess && resCurrentShift.object && resCurrentShift.object.data) {
                            _returnValue = resCurrentShift.object.data;
                        }

                        return _returnValue;
                    }

                    function converStringToDate(dateStr) {
                        var dateParts = dateStr.split("/");
                        var day = parseInt(dateParts[0], 10);
                        var month = parseInt(dateParts[1] - 1, 10);
                        var year = parseInt(dateParts[2], 10);
                        return new Date(year, month, day);
                    }

                    function calculateHoursOT(currentShiftCode) {
                        let _returnValue = 0;
                        if (currentShiftCode.startsWith("V")) {
                            if (currentShiftCode.startsWith("V9")) {
                                _returnValue = 8;
                            } else if (currentShiftCode.startsWith("VX")) {
                                _returnValue = 10;
                            } else {
                                _returnValue = currentShiftCode.substring(1, 2);
                            }
                        } else {
                            // ma ca khac mac dinh la 0
                        }
                        return _returnValue;
                    }

                    async function revoke() {
                        //$rootScope.loadingDialog(null, true);
                        $rootScope.showLoading();
                        if ($scope.status && $scope.status.currentStatus == 'Completed') {
                            var result = await workflowService.getInstance().workflows.startWorkflow($scope.revokeModel, null).$promise;
                            if (result.messages.length == 0) {
                                Notification.success($translate.instant('COMMON_WORKFLOW_STARTED'));
                                //$rootScope.loadingDialog(null, false);
                                $rootScope.hideLoading();
                                await $scope.init();
                                $state.reload();
                                $scope.$apply();
                            } else {
                                if (result.messages && result.messages.length) {
                                    //$rootScope.loadingDialog(null, false);
                                    $rootScope.hideLoading();
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
                                if (voteId == 1 && $scope.form && $scope.form.overtimeList
                                    && (($scope.model && $scope.model.referenceNumber && $scope.model.referenceNumber.startsWith("OVE")) || ($scope.form.overtimeList.length > 0))) {
                                    let content = [];
                                    let currentShiftCodeList = await getShiftCodeAllUser($scope.form.overtimeList, $scope.model.userSAPCode);
                                    for (const x of $scope.form.overtimeList) {
                                        const userSAPCode = (x.sapCode == null ? $scope.model.userSAPCode : x.sapCode);
                                        x.date = moment(x.dateOfOT, "DD/MM/YYYY").toDate();
                                        var currentShiftCode = _.find(currentShiftCodeList, item => {
                                            let otDate = new Date(x.date);
                                            if (isNaN(otDate.getTime()) && x.dateOfOT) {
                                                otDate = converStringToDate(x.dateOfOT);
                                            }
                                            return item.sapCode == userSAPCode && isSameDay(otDate, new Date(item.shiftExchangeDate));
                                        });

                                        await loadHolidays();
                                        var check_isHoliday = $scope.isHoliday(x.dateOfOT);

                                        let hours = 0;
                                        if (currentShiftCode) {
                                            var otHours = parseFloat(x.OTProposalHour);
                                            if (x.actualOTHour) {
                                                otHours = parseFloat(x.actualOTHour);
                                            }
                                            if (currentShiftCode.currentCode != "PRD" && check_isHoliday == false) {
                                                if (currentShiftCode.currentCode.startsWith("V")) {
                                                    hours = calculateHoursOT(currentShiftCode.currentCode);
                                                    otHours += parseFloat(hours);

                                                }
                                            }

                                            if (currentShiftCode.currentCode == "PRD" || check_isHoliday == true) {
                                                if (otHours > 12) {
                                                    let validateContent = "<br>- <b>" + (x.sapCode == null ? $scope.model.userSAPCode : x.sapCode) + "</b>: ";
                                                    validateContent = validateContent.concat(" ", x.dateOfOT)
                                                    validateContent = validateContent.concat(" ", $translate.instant('CANNOT_EXCEED_12H'))
                                                    content.push(validateContent);
                                                }
                                            }
                                            else {
                                                if (otHours > 12) {
                                                    let validateContent = "<br>- <b>" + (x.sapCode == null ? $scope.model.userSAPCode : x.sapCode) + "</b>: ";
                                                    validateContent = validateContent.concat(" ", x.dateOfOT)
                                                    validateContent = validateContent.concat(" ", $translate.instant('CANNOT_EXCEED_4H'))
                                                    content.push(validateContent);
                                                }
                                            }
                                        }
                                    }
                                    if (content.length > 0) {
                                        let message =
                                            "<b>" + $translate.instant('COMMON_WARNING').concat(" ", $translate.instant('OVERTIME_APPLICATION_OT_PROPOSAL_HOURS')) + ": </b>" +
                                            content.join();
                                        $scope.$apply();
                                        $scope.dialog = $rootScope.showConfirmWarning(
                                            $translate.instant('COMMON_WARNING'),
                                            message,
                                            $translate.instant('COMMON_BUTTON_CONFIRM'),
                                            true
                                        );
                                        $scope.$emit('waringOTs', { message });
                                        $scope.dialog.bind("close", async function (e) {
                                            var check_revoke = false;
                                            if ($scope.WorkflowName.includes("Revoke")) {
                                                check_revoke = true;
                                            }
                                            var errorsinMonth = await $scope.errorMessageOTHourAMonth();
                                            if (errorsinMonth.length > 0 && !check_revoke) {
                                                message =
                                                    "<b>" + ("", $translate.instant('OVERTIME_MANAGEMENT_USEREXCEDED_LIMIT_MONTH')) + ": </b>" +
                                                    errorsinMonth.join();
                                                $scope.$apply();
                                                $scope.dialog = $rootScope.showConfirmWarning(
                                                    $translate.instant('COMMON_WARNING'),
                                                    message,
                                                    $translate.instant('COMMON_BUTTON_CONFIRM'),
                                                    true
                                                );
                                                $scope.$emit('waringOTs', { message });
                                            }
                                            else {
                                                var errosWarning = await $scope.errorMessageOvertimeRemainBalance();
                                                if (errosWarning.length > 0 && !check_revoke) {
                                                    message =
                                                        "<b>" + ("", $translate.instant('OVERTIME_MANAGEMENT_USEREXCEDED_LIMIT')) + ": </b>" +
                                                        errosWarning.join();
                                                    $scope.$apply();
                                                    $scope.dialog = $rootScope.showConfirmWarning(
                                                        $translate.instant('COMMON_WARNING'),
                                                        message,
                                                        $translate.instant('COMMON_BUTTON_CONFIRM'),
                                                        true
                                                    );
                                                    $scope.$emit('waringOTs', { message });
                                                    /*$scope.dialog.bind("close", async function (e) {
                                                        if (e.data && e.data.value) {
                                                            $scope.voteDialog.title(title);
                                                            $scope.voteDialog.open();
                                                            $rootScope.confirmVoteDialog = $scope.voteDialog;
                                                        }
                                                    });*/
                                                } else {
                                                    $scope.voteDialog.title(title);
                                                    $scope.voteDialog.open();
                                                    $rootScope.confirmVoteDialog = $scope.voteDialog;
                                                }
                                            }
                                        });
                                    } else {
                                        var check_revoke = false;
                                        if ($scope.WorkflowName.includes("Revoke")) {
                                            check_revoke = true;
                                        }

                                        var errorsinMonth = await $scope.errorMessageOTHourAMonth();
                                        if (errorsinMonth.length > 0 && !check_revoke) {
                                            message =
                                                "<b>" + ("", $translate.instant('OVERTIME_MANAGEMENT_USEREXCEDED_LIMIT_MONTH')) + ": </b>" +
                                                errorsinMonth.join();
                                            $scope.$apply();
                                            $scope.dialog = $rootScope.showConfirmWarning(
                                                $translate.instant('COMMON_WARNING'),
                                                message,
                                                $translate.instant('COMMON_BUTTON_CONFIRM'),
                                                true
                                            );
                                            $scope.$emit('waringOTs', { message });
                                        }
                                        else {
                                            var errosWarning = await $scope.errorMessageOvertimeRemainBalance();
                                            if (errosWarning.length > 0 && !check_revoke) {
                                                message =
                                                    "<b>" + ("", $translate.instant('OVERTIME_MANAGEMENT_USEREXCEDED_LIMIT')) + ": </b>" +
                                                    errosWarning.join();
                                                $scope.$apply();
                                                $scope.dialog = $rootScope.showConfirmWarning(
                                                    $translate.instant('COMMON_WARNING'),
                                                    message,
                                                    $translate.instant('COMMON_BUTTON_CONFIRM'),
                                                    true
                                                );
                                                $scope.$emit('waringOTs', { message });
                                                /*$scope.dialog.bind("close", async function (e) {
                                                    if (e.data && e.data.value) {
                                                        $scope.voteDialog.title(title);
                                                        $scope.voteDialog.open();
                                                        $rootScope.confirmVoteDialog = $scope.voteDialog;
                                                    }
                                                });*/
                                            } else {
                                                $scope.voteDialog.title(title);
                                                $scope.voteDialog.open();
                                                $rootScope.confirmVoteDialog = $scope.voteDialog;
                                            }
                                        }
                                    }
                                } else {
                                    $scope.voteDialog.title(title);
                                    $scope.voteDialog.open();
                                    $rootScope.confirmVoteDialog = $scope.voteDialog;
                                }
                            }
                        }
                    }

                    $scope.dSave = async function () {
                        if ($scope.model && $scope.model.id && $scope.model.id !== null && $scope.model.id !== "") {
                            var currentItem = await workflowService.getInstance().common.getItemById({ Id: $scope.model.id }, null).$promise
                            if ($scope.model.status && currentItem.object.status && currentItem.object.status !== $scope.model.status) {
                                //$rootScope.loadingDialog(null, false);
                                $rootScope.hideLoading();
                                Notification.error($translate.instant('STATUS_ALREADY_CHANGE'));
                                return;
                            }
                        }

                        if ($scope.form && $scope.form.overtimeList
                            && (($scope.model && $scope.model.referenceNumber && $scope.model.referenceNumber.startsWith("OVE")) || ($scope.form.overtimeList.length > 0))) {
                            let content = [];
                            // validate OT/1d 
                            let currentShiftCodeList = await getShiftCodeAllUser($scope.form.overtimeList, $scope.model.userSAPCode);
                            for (const x of $scope.form.overtimeList) {
                                x.date = moment(x.dateOfOT, "DD/MM/YYYY").toDate();
                                const userSAPCode = (x.sapCode == null ? $scope.model.userSAPCode : x.sapCode);
                                var currentShiftCode = _.find(currentShiftCodeList, item => {
                                    let otDate = new Date(x.date);
                                    if (isNaN(otDate.getTime()) && x.dateOfOT) {
                                        otDate = converStringToDate(x.dateOfOT);
                                    }
                                    return item.sapCode == userSAPCode && isSameDay(otDate, new Date(item.shiftExchangeDate));
                                });

                                await loadHolidays();
                                var check_isHoliday = $scope.isHoliday(x.dateOfOT);

                                let hours = 0;
                                if (currentShiftCode) {
                                    var otHours = parseFloat(x.OTProposalHour);
                                    if (x.actualOTHour) {
                                        otHours = parseFloat(x.actualOTHour);
                                    }
                                    if (currentShiftCode.currentCode != "PRD" && check_isHoliday == false) {
                                        if (currentShiftCode.currentCode.startsWith("V")) {
                                            hours = calculateHoursOT(currentShiftCode.currentCode);
                                            otHours += parseFloat(hours);

                                        }
                                    }

                                    if (currentShiftCode.currentCode == "PRD" || check_isHoliday == true) {
                                        if (otHours > 12) {
                                            let validateContent = "<br>- <b>" + (x.sapCode == null ? $scope.model.userSAPCode : x.sapCode) + "</b>: ";
                                            validateContent = validateContent.concat(" ", x.dateOfOT)
                                            validateContent = validateContent.concat(" ", $translate.instant('CANNOT_EXCEED_12H'))
                                            content.push(validateContent);
                                        }
                                    }
                                    else {
                                        if (otHours > 12) {
                                            let validateContent = "<br>- <b>" + (x.sapCode == null ? $scope.model.userSAPCode : x.sapCode) + "</b>: ";
                                            validateContent = validateContent.concat(" ", x.dateOfOT)
                                            validateContent = validateContent.concat(" ", $translate.instant('CANNOT_EXCEED_4H'))
                                            content.push(validateContent);
                                        }
                                    }
                                }
                            }

                            let message = "";
                            if (content.length > 0) {
                                message =
                                    "<b>" + $translate.instant('COMMON_WARNING').concat(" ", $translate.instant('OVERTIME_APPLICATION_OT_PROPOSAL_HOURS')) + ": </b>" +
                                    content.join();
                                $scope.$apply();
                                $scope.dialog = $rootScope.showConfirmWarning(
                                    $translate.instant('COMMON_WARNING'),
                                    message,
                                    $translate.instant('COMMON_BUTTON_CONFIRM'),
                                    true
                                );
                                $scope.$emit('waringOTs', { message });
                                $scope.dialog.bind("close", async function (e) {
                                    if (e.data && e.data.value) {
                                        var errorsinMonth = await $scope.errorMessageOTHourAMonth();
                                        if (errorsinMonth.length > 0) {
                                            message =
                                                "<b>" + ("", $translate.instant('OVERTIME_MANAGEMENT_USEREXCEDED_LIMIT_MONTH')) + ": </b>" +
                                                errorsinMonth.join();
                                            $scope.$apply();
                                            $scope.dialog = $rootScope.showConfirmWarning(
                                                $translate.instant('COMMON_WARNING'),
                                                message,
                                                $translate.instant('COMMON_BUTTON_CONFIRM'),
                                                true
                                            );
                                            $scope.$emit('waringOTs', { message });
                                        }
                                        else {
                                            var errosWarning = await $scope.errorMessageOvertimeRemainBalance();
                                            if (errosWarning.length > 0) {
                                                message =
                                                    "<b>" + ("", $translate.instant('OVERTIME_MANAGEMENT_USEREXCEDED_LIMIT')) + ": </b>" +
                                                    errosWarning.join();
                                                $scope.$apply();
                                                $scope.dialog = $rootScope.showConfirmWarning(
                                                    $translate.instant('COMMON_WARNING'),
                                                    message,
                                                    $translate.instant('COMMON_BUTTON_CONFIRM'),
                                                    true
                                                );
                                                $scope.$emit('waringOTs', { message });
                                                /*$scope.dialog.bind("close", async function (e) {
                                                    if (e.data && e.data.value) {
                                                        $rootScope.loadingDialog(null, true);
                                                        await $scope.save($scope.form);
                                                        $rootScope.loadingDialog(null, false);
                                                    }
                                                });*/
                                            } else {
                                                $scope.$emit('waringOTs', { message });
                                                //$rootScope.loadingDialog(null, true);
                                                $rootScope.showLoading();
                                                await $scope.save($scope.form);
                                                //$rootScope.loadingDialog(null, false);
                                                $rootScope.hideLoading();
                                            }
                                        }
                                    }
                                });
                            } else {
                                var errorsinMonth = await $scope.errorMessageOTHourAMonth();
                                if (errorsinMonth.length > 0) {
                                    message =
                                        "<b>" + ("", $translate.instant('OVERTIME_MANAGEMENT_USEREXCEDED_LIMIT_MONTH')) + ": </b>" +
                                        errorsinMonth.join();
                                    $scope.$apply();
                                    $scope.dialog = $rootScope.showConfirmWarning(
                                        $translate.instant('COMMON_WARNING'),
                                        message,
                                        $translate.instant('COMMON_BUTTON_CONFIRM'),
                                        true
                                    );
                                    $scope.$emit('waringOTs', { message });
                                }
                                else {
                                    var errosWarning = await $scope.errorMessageOvertimeRemainBalance();
                                    if (errosWarning.length > 0) {
                                        message =
                                            "<b>" + ("", $translate.instant('OVERTIME_MANAGEMENT_USEREXCEDED_LIMIT')) + ": </b>" +
                                            errosWarning.join();
                                        $scope.$apply();
                                        $scope.dialog = $rootScope.showConfirmWarning(
                                            $translate.instant('COMMON_WARNING'),
                                            message,
                                            $translate.instant('COMMON_BUTTON_CONFIRM'),
                                            true
                                        );
                                        $scope.$emit('waringOTs', { message });
                                        /* $scope.dialog.bind("close", async function (e) {
                                         if (e.data && e.data.value) {
                                             $rootScope.loadingDialog(null, true);
                                             await $scope.save($scope.form);
                                             $rootScope.loadingDialog(null, false);
                                         }
                                     });*/
                                    } else {
                                        $scope.$emit('waringOTs', { message });
                                        //$rootScope.loadingDialog(null, true);
                                        $rootScope.showLoading();
                                        await $scope.save($scope.form);
                                        //$rootScope.loadingDialog(null, false);
                                        $rootScope.hideLoading();
                                    }
                                }
                            }
                        } else {
                            //$rootScope.loadingDialog(null, true);
                            $rootScope.showLoading();
                            await $scope.save($scope.form);
                            //$rootScope.loadingDialog(null, false);
                            $rootScope.hideLoading();
                        }
                    }
                    $scope.cancel = function () {
                        //$rootScope.loadingDialog(null, true);
                        $rootScope.showLoading();
                        $rootScope.cancel();
                        //$rootScope.loadingDialog(null, false);
                        $rootScope.hideLoading();
                        //$rootScope.gotoDashboard();
                    }

                    $scope.errorMessageOvertimeRemainBalance = async function () {
                        let errosWarning = [];

                        let groupData = [];
                        var sapCodeList = [];
                        for (const x of $scope.form.overtimeList) {
                            const userSAPCode = (x.sapCode == null ? $scope.model.userSAPCode : x.sapCode);
                            if (!groupData[userSAPCode]) {
                                groupData[userSAPCode] = [];
                                sapCodeList.push({ sapCode: userSAPCode, overtimeApplicationid: $scope.model.id });
                            }
                            groupData[userSAPCode].push(x);
                        }

                        let currentShiftCodeList = await getAllOvertimeBalanceSet(sapCodeList);

                        // lap tung loop de xu lu
                        for (const groupBySAPCode in groupData) {
                            if (groupData.hasOwnProperty(groupBySAPCode)) {
                                let totalHour = 0;
                                const group = groupData[groupBySAPCode];
                                group.forEach(item => {
                                    var otHours = parseFloat(item.OTProposalHour);
                                    if (item.actualOTHour) {
                                        otHours = parseFloat(item.actualOTHour);
                                    }
                                    totalHour += otHours;
                                });

                                //let currentShiftCodeList = await getAllOvertimeBalanceSet(sapCodeList);
                                var remainOT = _.find(currentShiftCodeList, x => { return x.employeeCode == groupBySAPCode });
                                if (!remainOT || remainOT == null || remainOT == '') {
                                    let checkQuota = "<br>- <b>" + (groupBySAPCode) + "</b>: ";
                                    checkQuota = checkQuota.concat("User don't have Quota OT");
                                    errosWarning.push(checkQuota);
                                }
                                if (remainOT && ((totalHour + remainOT.edocInUsed) > (remainOT.otRemain))) {
                                    //if (remainOT) {
                                    let validateContent = "<br>- <b>" + (groupBySAPCode) + "</b>: ";
                                    validateContent = validateContent.concat($translate.instant('HAS_EXCEEDED') + " ", (totalHour + remainOT.edocInUsed - (remainOT.otRemain)), " " + $translate.instant('HOURS'));
                                    //validateContent = validateContent.concat("  (In used: ", remainOT.edocInUsed).concat("/Remain: ", remainOT.newRemain).concat(")");
                                    errosWarning.push(validateContent);
                                }
                            }
                        }
                        return errosWarning;
                    }

                    async function getAllOvertimeBalanceSet(sapCodeList) {
                        var _returnValue = [];
                        /*for (const x of overtimeApplicationList) {
                            var sapCode = (x.sapCode == null ? userSapcCode : x.sapCode);
                            var res = await ssgexService.getInstance().remoteDatas.getOvertimeBalanceSet({ sapCode: sapCode, OvertimeApplicationid: $scope.model.id }, null).$promise;
                            if (res.isSuccess && res.object) {
                                _returnValue = res.object;
                            }
                        }*/
                        var res = await ssgexService.getInstance().remoteDatas.getMultiOvertimeBalanceSet(sapCodeList).$promise;
                        if (res.isSuccess && res.object) {
                            _returnValue = res.object;
                        }
                        return _returnValue;
                    }

                    $scope.errorMessageOTHourAMonth = async function () {
                        let errosWarning = [];

                        //let groupData = [];
                        // Group overtimeList by month
                        let groupData = {};
                        var sapCodeAndMonthList = [];
                        for (const x of $scope.form.overtimeList) {
                            if (!x.isNoOT) {
                                const userSAPCode = (x.sapCode == null ? $scope.model.userSAPCode : x.sapCode);
                                var month = moment(x.date, "DD/MM/YYYY").format("MM/YYYY");
                                if (moment(x.date, "DD/MM/YYYY").date() >= 26) {
                                    month = moment(x.date, "DD/MM/YYYY").add(1, 'months').format("MM/YYYY");
                                }

                                if (!groupData[userSAPCode]) {
                                    groupData[userSAPCode] = {};
                                }
                                if (!groupData[userSAPCode][month]) {
                                    groupData[userSAPCode][month] = [];
                                    sapCodeAndMonthList.push({ sapCode: userSAPCode, month: month, overtimeApplicationid: $scope.model.id });
                                }
                                groupData[userSAPCode][month].push(x);
                            }
                        }

                        var ListOTHasCurrentOTHours = await getOTHourAMonth(sapCodeAndMonthList);

                        // lap tung loop de xu lu
                        for (const groupBySAPCode in groupData) {
                            if (groupData.hasOwnProperty(groupBySAPCode)) {
                                const group = groupData[groupBySAPCode];
                                for (const month in group) {
                                    let totalHour = 0;
                                    if (group.hasOwnProperty(month)) {
                                        const overtimeList = group[month];
                                        overtimeList.forEach(item => {
                                            var otHours = parseFloat(item.OTProposalHour);
                                            if (item.actualOTHour) {
                                                otHours = parseFloat(item.actualOTHour);
                                            }
                                            totalHour += otHours;
                                        });

                                        var currentOTHour = ListOTHasCurrentOTHours.find(item => item.sapCode === groupBySAPCode && item.month === month);

                                        if ((totalHour + currentOTHour.otHour) > 40) {
                                            //if (remainOT) {
                                            let validateContent = "<br>- <b>" + (groupBySAPCode) + "</b> (" + month + "): ";
                                            validateContent = validateContent.concat($translate.instant('HAS_EXCEEDED') + " ", totalHour + currentOTHour.otHour - 40, " " + $translate.instant('HOURS'));
                                            errosWarning.push(validateContent);
                                        }
                                    }
                                }
                            }
                        }
                        return errosWarning;
                    }

                    async function getOTHourAMonth(sapCodeAndMonthList) {
                        var _returnValue = [];
                        var res = await ssgexService.getInstance().remoteDatas.getOTHourInMonth(sapCodeAndMonthList).$promise;
                        if (res.isSuccess && res.object) {
                            _returnValue = res.object;
                        }
                        return _returnValue;
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
                                //$rootScope.loadingDialog(null, true);
                                $rootScope.showLoading();
                                if ($scope.voteModel.vote == 1 || $scope.voteModel.comment) {
                                    $scope.voteModel.isValid = true;
                                    if ($scope.voteModel.vote == 1) {
                                        workflowService.getInstance().common.getItemById({ Id: $scope.voteModel.itemId }, null).$promise.then(function (result) {
                                            if ($scope.model.status && result.object.status && result.object.status !== $scope.model.status) {
                                                //$rootScope.loadingDialog(null, false);
                                                $rootScope.hideLoading();
                                                Notification.error($translate.instant('STATUS_ALREADY_CHANGE'));
                                                $scope.init();
                                            } else {
                                                // if (!dataService.workflowStatus.ignoreValidation && (dataService.workflowStatus.restrictedProperties || (2 & dataService.permission.right) == 2)) {
                                                if (!dataService.workflowStatus.ignoreValidation) {
                                                    if (dataService.workflowStatus.restrictedProperties) {
                                                        var errorFields = isRestrictedPropertiesEmpty(dataService.workflowStatus.restrictedProperties);
                                                        $scope.voteModel.errorMessage = '';
                                                        if (errorFields.length > 0) {
                                                            $scope.voteModel.isValid = false;
                                                            $scope.voteModel.errorMessage = "Please fill the following fields in the Form: " + errorFields.join(", ");
                                                            //$rootScope.loadingDialog(null, false);
                                                            $rootScope.hideLoading();
                                                            $scope.$apply();
                                                            return false;
                                                        }
                                                    }

                                                    if ($scope.model?.referenceNumber?.startsWith("TAR") && $scope.model.status?.toLowerCase() != "requested to change") {
                                                        workflowService.getInstance().workflows.vote($scope.voteModel).$promise.then(function (result) {
                                                            if (result.messages.length == 0) {
                                                                // Notification.success("Workflow has been processed");
                                                                Notification.success($translate.instant('COMMON_WORKFLOW_PROCESSED'));
                                                                $scope.init();
                                                                $state.reload();
                                                            } else {
                                                                if (result.messages && result.messages.length) {
                                                                    Notification.error(result.messages[0]);
                                                                }
                                                                $scope.voteDialog.close();
                                                            }
                                                            //$rootScope.loadingDialog(null, false);
                                                            $rootScope.hideLoading();
                                                        });

                                                    }
                                                    else if ($scope.model?.referenceNumber?.startsWith("LEA")) {
                                                        workflowService.getInstance().workflows.vote($scope.voteModel).$promise.then(function (result) {
                                                            if (result.messages.length == 0) {
                                                                // Notification.success("Workflow has been processed");
                                                                Notification.success($translate.instant('COMMON_WORKFLOW_PROCESSED'));
                                                                $scope.init();
                                                                $state.reload();
                                                            } else {
                                                                if (result.messages && result.messages.length) {
                                                                    Notification.error(result.messages[0]);
                                                                }
                                                                $scope.voteDialog.close();
                                                            }
                                                            //$rootScope.loadingDialog(null, false);
                                                            $rootScope.hideLoading();
                                                        });

                                                    }
                                                    else {
                                                        $scope.save($scope.form, dataService.permission.right).then(function (result) {
                                                            if (result && result.isSuccess) {
                                                                workflowService.getInstance().workflows.vote($scope.voteModel).$promise.then(function (result) {
                                                                    if (result.messages.length == 0) {
                                                                        // Notification.success("Workflow has been processed");
                                                                        Notification.success($translate.instant('COMMON_WORKFLOW_PROCESSED'));
                                                                        $scope.init();
                                                                        $state.reload();
                                                                    } else {
                                                                        if (result.messages && result.messages.length) {
                                                                            Notification.error(result.messages[0]);
                                                                        }
                                                                        $scope.voteDialog.close();
                                                                    }
                                                                    //$rootScope.loadingDialog(null, false);
                                                                    $rootScope.hideLoading();
                                                                });
                                                            } else {
                                                                if (result.messages && result.messages.length) {
                                                                    Notification.error(result.messages[0]);
                                                                }
                                                                //$rootScope.loadingDialog(null, false);
                                                                $rootScope.hideLoading();
                                                                $scope.voteDialog.close();
                                                            }
                                                            $scope.$apply();
                                                        });
                                                    }
                                                } else {
                                                    workflowService.getInstance().workflows.vote($scope.voteModel).$promise.then(function (result) {
                                                        if (result.messages.length == 0) {
                                                            // Notification.success("Workflow has been processed");
                                                            Notification.success($translate.instant('COMMON_WORKFLOW_PROCESSED'));
                                                            $scope.init();
                                                            $state.reload();
                                                        } else {
                                                            if (result.messages && result.messages.length) {
                                                                Notification.error(result.messages[0]);
                                                            }
                                                            $scope.voteDialog.close();
                                                        }
                                                        //$rootScope.loadingDialog(null, false);
                                                        $rootScope.hideLoading();
                                                    });
                                                }
                                            }
                                        });
                                    } else {
                                        if ($scope.voteModel && $scope.voteModel.vote == 3 && $scope.voteModel && $scope.voteModel.itemId != null && $scope.voteModel.itemId !== "") {
                                            workflowService.getInstance().common.getItemById({ Id: $scope.voteModel.itemId }, null).$promise.then(function (result) {
                                                if ($scope.model.status && result.object.status && result.object.status !== $scope.model.status) {
                                                    //$rootScope.loadingDialog(null, false);
                                                    $rootScope.hideLoading();
                                                    Notification.error($translate.instant('STATUS_ALREADY_CHANGE'));
                                                    $scope.init();
                                                } else {
                                                    workflowService.getInstance().workflows.vote($scope.voteModel).$promise.then(function (result) {
                                                        if (result.messages.length == 0) {
                                                            // Notification.success("Workflow has been processed");
                                                            Notification.success($translate.instant('COMMON_WORKFLOW_PROCESSED'));
                                                            $scope.init();
                                                            $state.reload();
                                                        } else {
                                                            if (result.messages && result.messages.length) {
                                                                Notification.error(result.messages[0]);
                                                            }
                                                            $scope.voteDialog.close();
                                                        }
                                                        //$rootScope.loadingDialog(null, false);
                                                        $rootScope.hideLoading();
                                                    });
                                                }
                                            });
                                        } else {
                                            workflowService.getInstance().workflows.vote($scope.voteModel).$promise.then(function (result) {
                                                if (result.messages.length == 0) {
                                                    // Notification.success("Workflow has been processed");
                                                    Notification.success($translate.instant('COMMON_WORKFLOW_PROCESSED'));
                                                    $scope.init();
                                                    $state.reload();
                                                } else {
                                                    if (result.messages && result.messages.length) {
                                                        Notification.error(result.messages[0]);
                                                    }
                                                    $scope.voteDialog.close();
                                                }
                                                //$rootScope.loadingDialog(null, false);
                                                $rootScope.hideLoading();
                                            });
                                        }
                                    }
                                }
                                else {
                                    $scope.voteModel.isValid = false;
                                    // $scope.voteModel.errorMessage = "Please enter comment";
                                    $scope.voteModel.errorMessage = $translate.instant('COMMON_COMMENT_VALIDATE');
                                    //$rootScope.loadingDialog(null, false);
                                    $rootScope.hideLoading();
                                    $scope.$apply();
                                    return false;
                                }
                            },
                            primary: true
                        }],
                        close: async function (e) {
                        }
                    };

                    $scope.updateStatusItem = async function (status) {
                        $scope.updateStatusModel.status = status;
                        var title = '';
                        if (status == 'Completed') {
                            title = $translate.instant('COMMON_BUTTON_IT_COMPLETE');
                        } else {
                            title = $translate.instant('COMMON_BUTTON_IT_CANCEL');
                        }
                        $scope.updateStatusDialog.title(title);
                        $scope.updateStatusDialog.open();
                        $rootScope.confirmVoteDialog = $scope.updateStatusDialog;
                    }

                    $scope.updateStatusDialogOpts = {
                        width: "1200px",
                        buttonLayout: "normal",
                        closable: true,
                        modal: true,
                        visible: false,
                        content: "",
                        actions: [{
                            text: $translate.instant('COMMON_BUTTON_OK'),
                            action: function (e) {
                                if ($scope.updateStatusModel.comment) {
                                    $scope.updateStatusModel.isValid = true;
                                    let dialogUpdateStatusWorkflow = $rootScope.showConfirmYNDialog("COMMON_NOTIFICATION", "COMMON_SAVE_CONFIRM");
                                    $scope.$apply();
                                    dialogUpdateStatusWorkflow.bind("close", async function (e) {
                                        if (e.data && e.data.value) {
                                            //$rootScope.loadingDialog(null, true);
                                            $rootScope.showLoading();
                                            // upload attachment
                                            if ($scope.attachments.length || $scope.removeFiles.length) {
                                                let attachmentFilesJson = await mergeAttachmentV2(attachmentService, $scope.oldAttachments, $scope.attachments);
                                                $scope.model.documents = attachmentFilesJson;
                                            }

                                            var modelUpdateStatus = {
                                                Id: $stateParams.id,
                                                Status: $scope.updateStatusModel.status,
                                                Comment: $scope.updateStatusModel.comment,
                                                ReferenceNumber: $stateParams.referenceValue
                                            };
                                            workflowService.getInstance().common.updateStatusByReferenceNumber(modelUpdateStatus).$promise.then(function (result) {
                                                if (result.isSuccess) {
                                                    // save log
                                                    var modelLog = {
                                                        ItemRefereceNumberOrCode: $stateParams.referenceValue,
                                                        ItemType: $rootScope.getItemTypeByReferenceNumber($stateParams.referenceValue),
                                                        Type: appSetting.TrackingType.UpdateStatus,
                                                        Comment: $scope.updateStatusModel.comment,
                                                        DataStr: JSON.stringify({ Status: $scope.updateStatusModel.status }),
                                                        InstanceId: ($scope.status && $scope.status.lastHistory && $scope.status.lastHistory.instanceId) ? $scope.status.lastHistory.instanceId : "",
                                                        Documents: $scope.model.documents,
                                                        RoundNum: ($scope.status && $scope.status.workflowInstances && $scope.status.workflowInstances.length > 0) ? $scope.status.workflowInstances.length : "",
                                                    };
                                                    trackingHistoryService.getInstance().trackingHistory.saveTrackingHistory(modelLog).$promise.then(function (resultLog) {
                                                        if (resultLog.isSuccess) {
                                                            Notification.success($translate.instant('COMMON_SAVE_SUCCESS'));
                                                            $scope.init();
                                                            $state.reload();
                                                        }
                                                    });
                                                } else {
                                                    if (result.messages && result.messages.length) Notification.error(result.messages[0]);
                                                }
                                                //$rootScope.loadingDialog(null, false);
                                                $rootScope.hideLoading();
                                                $scope.updateStatusDialog.close();
                                            });
                                        }
                                    });
                                }
                                else {
                                    $scope.updateStatusModel.isValid = false;
                                    $scope.updateStatusModel.errorMessage = $translate.instant('COMMON_COMMENT_VALIDATE');
                                    //$rootScope.loadingDialog(null, false);
                                    $rootScope.hideLoading();
                                    $scope.$apply();
                                    return false;
                                }
                            },
                            primary: true
                        }],
                        close: async function (e) {
                            $scope.updateStatusModel.errorMessage = null;
                            $scope.updateStatusModel.isValid = true;
                        }
                    };

                    $scope.onSelect = function (e) {
                        let message = $.map(e.files, function (file) {
                            $scope.attachments.push(file);
                            return file.name;
                        }).join(", ");
                    };

                    $scope.isPerAdminHrAdmin = function () {
                        var _return = false;
                        if ($state.current && $state.current.name && ($state.current.name == "home.requestToHire.item" || $state.current.name == "home.promoteAndTransfer.item" || $state.current.name == "home.action.item")) {
                            if ($rootScope.currentUser) {
                                _return = ((($rootScope.currentUser.role & appSetting.role.HRAdmin) == appSetting.role.HRAdmin) || (($rootScope.currentUser.role & appSetting.role.Admin) == appSetting.role.Admin));
                            }
                        } else {
                            _return = true;
                        }
                        return _return;
                    }

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
                        ActionHistory = [];
                    });

                    $scope.saveForm = async function () {
                        $scope.updateModelDialog.title($translate.instant('COMMON_BUTTON_IT_EDIT'));
                        $scope.updateModelDialog.open();
                        $rootScope.confirmVoteDialog = $scope.updateModelDialog;
                        /*await $scope.save($scope.form);
                        $state.reload();*/
                    }

                    function convertJsonData(model, jsonData, period) {
                        let removeFieldFromModels = ['createdById', 'jsonData'];
                        let list = [];
                        var arrayDataFromJson = _.groupBy(JSON.parse(jsonData), 'sapCode');
                        if (arrayDataFromJson) {
                            Object.keys(arrayDataFromJson).forEach(function (targetPlan) {
                                tempTarget1 = createData(arrayDataFromJson[targetPlan][0]);
                                tempTarget2 = createData(arrayDataFromJson[targetPlan][1]);

                                if (tempTarget1) {
                                    tempTarget1.periodId = period;
                                    list.push(tempTarget1);
                                }
                                if (tempTarget2) {
                                    tempTarget2.periodId = period;
                                    list.push(tempTarget2);
                                }
                            });
                        }
                        let viewModel = {};
                        Object.keys(model).forEach(function (fieldName) {
                            if (_.findIndex(removeFieldFromModels, x => { return x == fieldName }) == -1) {
                                viewModel[fieldName] = model[fieldName];
                            }
                        });
                        viewModel.targetPlanDetailAfterConvert = list;
                        console.log(viewModel);
                        return viewModel;
                    }

                    function createData(item) {
                        if (item) {
                            var convertModel = { modified: new Date(), modifiedById: null, responseStatus: null, jsonData: '' };
                            let tempJsonData = [];
                            Object.keys(item).forEach(function (fieldName) {
                                if (fieldName.includes('targetField')) {
                                    tempJsonData.push({ date: fieldName.substring(fieldName.length - 8, fieldName.length), value: item[fieldName] });
                                } else {
                                    if (fieldName == 'type') {
                                        convertModel[fieldName] = item[fieldName].substring(item[fieldName].length - 1, item[fieldName].length);
                                    } else {
                                        convertModel[fieldName] = item[fieldName];
                                    }
                                }

                            });
                            convertModel.jsonData = JSON.stringify(tempJsonData);
                            return convertModel;
                        }
                    }

                    $scope.updateModelDialogOpts = {
                        width: "1000px",
                        buttonLayout: "normal",
                        closable: true,
                        modal: true,
                        visible: false,
                        content: "",
                        actions: [{
                            text: $translate.instant('COMMON_BUTTON_OK'),
                            action: function (e) {
                                if ($scope.updateModel.comment) {
                                    $scope.updateModel.isValid = true;
                                    let dialogUpdateModelsWorkflow = $rootScope.showConfirmYNDialog("COMMON_NOTIFICATION", "COMMON_SAVE_CONFIRM");
                                    $scope.$apply();
                                    dialogUpdateModelsWorkflow.bind("close", async function (e) {
                                        if (e.data && e.data.value) {
                                            //$rootScope.loadingDialog(null, true);
                                            $rootScope.showLoading();
                                            $scope.isEditing = false;
                                            // upload attachment
                                            if ($scope.attachments.length || $scope.removeFiles.length) {
                                                let attachmentFilesJson = await mergeAttachmentV2(attachmentService, $scope.oldAttachments, $scope.attachments);
                                                $scope.updateModel.documents = attachmentFilesJson;
                                            }
                                            let refererenNumber = $stateParams.referenceValue ? $stateParams.referenceValue : $scope.model.referenceNumber;
                                            let result = null;
                                            let model = "";
                                            if (refererenNumber) {
                                                switch (refererenNumber.toString().substring(0, 3)) {
                                                    case "REQ":
                                                        model = {
                                                            referenceNumber: refererenNumber,
                                                            WorkingAddressRecruitmentId: $scope.model.workingAddressRecruitmentId,
                                                            HasBudget: $scope.model.hasBudget,
                                                        }
                                                        result = await itService.getInstance().it.saveRequestToHire(model).$promise;
                                                        // model Log
                                                        dataStrModel = {
                                                            HasBudget: $scope.oldModel.hasBudget,
                                                            WorkingAddressRecruitmentId: $scope.oldModel.workingAddressRecruitmentId
                                                        };
                                                        break;
                                                    case "RES":
                                                        model = {
                                                            isUpdatePayload: $scope.updateModel.applyPayload ? true : false,
                                                            referenceNumber: refererenNumber,
                                                            startingDate: $scope.model.startingDate,
                                                            officialResignationDate: $scope.model.officialResignationDate,
                                                            suggestionForLastWorkingDay: $scope.model.suggestionForLastWorkingDay
                                                        }
                                                        result = await itService.getInstance().it.saveResignation(model).$promise;
                                                        // luu log
                                                        dataStrModel = {
                                                            StartingDate: $scope.oldModel.startingDate,
                                                            OfficialResignationDate: $scope.oldModel.officialResignationDate,
                                                            SuggestionForLastWorkingDay: $scope.oldModel.suggestionForLastWorkingDay
                                                        };
                                                    case "PRO":
                                                        break;
                                                    case "SHI":
														var gridUserId = $('#shiftExchangeEditListGrid').data('kendoGrid').dataSource._data;
                                                        gridUserId.forEach(function (item, index) {
                                                            if (item.currentShift != item.currentShiftCode) {
                                                                var listShiftCode = item.shiftSet;
                                                                item.currentShiftCode = item.currentShift;
                                                                item.currentShiftName = listShiftCode.find(x => x.code == item.currentShift).name;
                                                            }
                                                        });

                                                        model = {
                                                            isUpdatePayload: $scope.updateModel.applyPayload ? true : false,
                                                            referenceNumber: refererenNumber,
                                                            shiftExchangeItemsData: gridUserId,
                                                        }
                                                        result = await itService.getInstance().it.saveShiftExchange(model).$promise;
                                                        dataStrModel = $scope.oldModel;
                                                        break;
                                                    case "TAR":
                                                        var gridUserId = $('#grid_user_id').data('kendoGrid').dataSource._data;
                                                        var models = JSON.stringify(gridUserId);
                                                        var data = convertJsonData($scope.model, models, $scope.model.periodId);
                                                        model = {
                                                            isUpdatePayload: $scope.updateModel.applyPayload ? true : false,
                                                            referenceNumber: refererenNumber,
                                                            targetPlanDetails: data.targetPlanDetailAfterConvert,
                                                            periodId: $scope.model.periodId,
                                                            periodFromDate: $scope.model.periodFromDate,
                                                            periodToDate: $scope.model.periodToDate

                                                        }
                                                        result = await itService.getInstance().it.saveTargetPlan(model).$promise;
                                                        dataStrModel = $scope.oldModel;
                                                        break;
                                                    case "BTA":
                                                        var gridBTA = $('#btaListDetailGrid').data('kendoGrid').dataSource._data;
                                                        var modelBTADetails = JSON.stringify(gridBTA);
                                                        model = {
                                                            referenceNumber: refererenNumber,
                                                            isRoundTrip: $scope.model.isRoundTrip,
                                                            requestorNote: $scope.model.requestorNote,
                                                            requestorNoteDetail: $scope.model.requestorNoteDetail,
                                                            btaDetails: modelBTADetails
                                                        };
                                                        result = await itService.getInstance().it.saveBTA(model).$promise;
                                                        dataStrModel = $scope.oldModel;
                                                        break;
                                                    default:
                                                        dataStrModel = $scope.oldModel;
                                                }
                                            }
                                            if (result != null && result.isSuccess) {
                                                // edit payload
                                                var modelLog = {
                                                    ItemRefereceNumberOrCode: $scope.oldModel.referenceNumber,
                                                    ItemType: $rootScope.getItemTypeByReferenceNumber($scope.model.referenceNumber),
                                                    Type: appSetting.TrackingType.UpdateInformation,
                                                    Comment: $scope.updateModel.comment,
                                                    Documents: $scope.updateModel.documents,
                                                    DataStr: JSON.stringify(dataStrModel)
                                                };

                                                trackingHistoryService.getInstance().trackingHistory.saveTrackingHistory(modelLog).$promise.then(function (resultLog) {
                                                    if (resultLog.isSuccess) {
                                                        Notification.success($translate.instant('COMMON_SAVE_SUCCESS'));
                                                        $state.go($state.current.name, { id: $scope.model.id, referenceValue: $scope.model.referenceNumber }, { reload: true });
                                                    }
                                                });
                                                $scope.init();
                                            }
                                            $rootScope.hideLoading();
                                            $scope.updateModelDialog.close();
                                        }
                                    });
                                }
                                else {
                                    $scope.updateModel.isValid = false;
                                    $scope.updateModel.errorMessage = $translate.instant('COMMON_COMMENT_VALIDATE');
                                    $rootScope.loadingDialog(null, false);
                                    $rootScope.hideLoading();
                                    $scope.$apply();
                                    return false;
                                }
                            },
                            primary: true
                        }],
                        close: async function (e) {
                            $scope.updateModel.errorMessage = null;
                            $scope.updateModel.isValid = true;
                        }
                    };

                    $scope.appearButtonEdit = function () {
                        let refererenNumber = $stateParams.referenceValue ? $stateParams.referenceValue : $scope.model.referenceNumber;
                        var list = ["REQ", "RES", "SHI", "TAR", "BTA"];
                        if (refererenNumber) {
                            return list.includes(refererenNumber.toString().substring(0, 3));
                        }
                        return false;
                    }

                    function getDataGrid(id) {
                        let gridRoom = $(id).data("kendoGrid");
                        if (gridRoom) {
                            return gridRoom.dataSource._data.toJSON();
                        }
                        return null;
                    }

                    $scope.editField = function () {
                        enableElementWithRoleITHelpdesk();
                        $scope.isEditing = true;
                        let refererenNumber = $stateParams.referenceValue ? $stateParams.referenceValue : $scope.model.referenceNumber;
                        if (refererenNumber) {
                            switch (refererenNumber.toString().substring(0, 3)) {
                                case "REQ":
                                    $("#workingAddressRecruitment_id").removeAttr("disabled");
                                    $("#hasBudget").removeAttr("disabled");
                                    break;
                                case "RES":
                                    $("#startingDate").removeAttr("disabled").removeAttr('readonly');
                                    $("#official_resignation_date_id").removeAttr("disabled").removeAttr('readonly');
                                case "SHI":
                                    var shiftExchangeEditList = getDataGrid('#shiftExchangeEditListGrid');
                                    if (shiftExchangeEditList && shiftExchangeEditList.length > 0) {
                                        shiftExchangeEditList.forEach(item => {
                                            $('#currentShiftSelect' + item.no).removeAttr("disabled").removeAttr('readonly');
                                        })
                                    }
                                case "TAR":
                                    var targetPlanEditList = getDataGrid('#grid_user_id');
                                    if (targetPlanEditList && targetPlanEditList.length > 0) {
                                        targetPlanEditList.forEach(item => {
                                            $('.targetSelect').removeAttr("disabled").removeAttr('readonly');
                                        })
                                    }
                                case "BTA":
                                    var btaListDetailGridEdit = getDataGrid('#btaListDetailGrid');
                                    if (btaListDetailGridEdit && btaListDetailGridEdit.length > 0) {
                                        for (let index = 0; index < btaListDetailGridEdit.length; index++) {
                                            // let item = btaListDetailGridEdit[index];
                                            //if (item.isCommitBooking) {
                                            $(`#hotel${index + 1}`).removeAttr("disabled").removeAttr("readonly");
                                            $(`#stayHotel${index + 1}`).removeAttr("disabled").removeAttr("readonly");
                                            $(`#checkInHotelDate${index + 1}`).removeAttr("disabled").removeAttr("readonly");
                                            $(`#checkOutHotelDate${index + 1}`).removeAttr("disabled").removeAttr("readonly");
                                            $(`#dataRoundTripId`).removeAttr("disabled").removeAttr("readonly");
                                            // continue;
                                            //}
                                            $(`#dataRoundTripId`).removeAttr("disabled").removeAttr("readonly");
                                            $(`#reasonDropdown`).removeAttr("disabled").removeAttr("readonly");
                                            $(`#reasonOfPromotion`).removeAttr("disabled").removeAttr("readonly");
                                            $(`#partitionSelect${index + 1}`).removeAttr("disabled").removeAttr("readonly");
                                            $(`#departureSelect${index + 1}`).removeAttr("disabled").removeAttr("readonly");
                                            $(`#arrivalSelect${index + 1}`).removeAttr("disabled").removeAttr("readonly");
                                            $(`#gender${index + 1}`).removeAttr("disabled").removeAttr("readonly");
                                            $(`#stayHotel${index + 1}`).removeAttr("disabled").removeAttr("readonly");
                                            $(`#hotel${index + 1}`).removeAttr("disabled").removeAttr("readonly");
                                            $(`#firstName${index + 1}`).removeAttr("disabled").removeAttr("readonly");
                                            $(`#lastName${index + 1}`).removeAttr("disabled").removeAttr("readonly");
                                            $(`#email${index + 1}`).removeAttr("disabled").removeAttr("readonly");
                                            $(`#mobile${index + 1}`).removeAttr("disabled").removeAttr("readonly");
                                            $(`#passport${index + 1}`).removeAttr("disabled").removeAttr("readonly");
                                            $(`#hasBudget${index + 1}`).removeAttr("disabled").removeAttr("readonly");
                                            $(`#idCard${index + 1}`).removeAttr("disabled").removeAttr("readonly");
                                            $(`#dateOfBirth${index + 1}`).removeAttr("disabled").removeAttr("readonly");
                                            $(`#passport${index + 1}`).removeAttr("disabled").removeAttr("readonly");
                                            $(`#departureSelect${index + 1}`).removeAttr("disabled").removeAttr("readonly");
                                            $(`#passportDateOfIssue${index + 1}`).removeAttr("disabled").removeAttr("readonly");
                                            $(`#passportExpiryDate${index + 1}`).removeAttr("disabled").removeAttr("readonly");
                                            $(`#fromDate${index + 1}`).removeAttr("disabled").removeAttr("readonly");
                                            $(`#toDate${index + 1}`).removeAttr("disabled").removeAttr("readonly");
                                            $(`#isForeigner${index + 1}`).removeAttr("disabled").removeAttr("readonly");
                                            $(`#stayHotel${index + 1}`).removeAttr("disabled").removeAttr("readonly");
                                            $(`#countrySelect${index + 1}`).removeAttr("disabled").removeAttr("readonly");
                                            /*$(`#checkInHotelDate${index + 1}`).removeAttr("disabled").removeAttr("readonly");
                                            $(`#checkOutHotelDate${index + 1}`).removeAttr("disabled").removeAttr("readonly");*/
                                        }
                                    }
                                    setTimeout(() => {
                                        $('.k-textbox').removeAttr('disabled').removeAttr('readonly');
                                        $('.k-input').removeAttr('disabled').removeAttr('readonly');
                                        $('.k-dropdown').removeClass('k-state-disabled');
                                        $('.k-combobox').removeClass('k-state-disabled');
                                        $('.k-autocomplete').removeClass('k-state-disabled');
                                        $('[aria-disabled="true"]').attr('aria-disabled', 'false');
                                    }, 100);
                                    $scope.$emit('ITEdit', { isEdit: true });
                                    break;
                            }
                        }
                    }

                    $scope.hidenCancel = function () {
                        var check = ($stateParams.referenceValue && ($stateParams.referenceValue.startsWith('PRO-') || $stateParams.referenceValue.startsWith('MIS-')) && $scope.status.currentStatus == 'Completed') ? false : true
                        if (check) {
                            return check
                        }

                    }

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

                }
            ],
        };
    },
]);