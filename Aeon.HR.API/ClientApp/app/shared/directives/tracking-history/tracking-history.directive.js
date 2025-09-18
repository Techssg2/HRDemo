var ssgApp = angular.module('ssg.directiveTrackingHistoryModule', []);
ssgApp.directive("trackingHistory", [
    function($rootScope) {
        return {
            restrict: "E",
            templateUrl: "ClientApp/app/shared/directives/tracking-history/tracking-history.template.html?v=" + edocV,
            scope: {
                currentUser: "=",
                trackingHistory: "=",
                model:"="
            },
            link: function($scope, element, attr, modelCtrl) {},
            controller: [
                "$rootScope", "$scope", "dataService", "appSetting", "$translate", "attachmentFile", "$window", "$state", "settingService", "commonData",
                function ($rootScope, $scope, dataService, appSetting, $translate, attachmentFile, $window, $state, settingService, commonData) {
                    $scope.detail = {};
                    $scope.query = {};
                    $scope.dialogActions = [
                        "close"
                    ];
                    $scope.departmentMode = {};
                    $scope.shiftCodeModel = {};
                    $scope.targetPlanSpecialModel = {};
                    $scope.removeFiles = [];
                    $scope.trackingHistory = dataService.trackingHistory;
                    $scope.stepDetail = {};
                    $scope.formatDate = function (created) {
                        return moment(created).format(appSetting.longDateFormat);
                    }

                    $scope.openCreateWindow = async function (type, itemType, dataStr) {
                        if (itemType) {
                            if (itemType == 'UserDepartmentMapping') {
                                $scope.viewHistoryDetail(type, itemType, dataStr);
                            } else if (itemType == 'Department') {
                                $scope.departmentModel = JSON.parse(dataStr);
                                $scope.departmentModel.typeName = $scope.departmentModel.Type == 1 ? 'Division' : 'Department'
                                $scope.departmentEditWin.title("Department dialog");
                                $scope.departmentEditWin.center();
                                $scope.departmentEditWin.open();
                            }
                            else if (itemType = 'TargetPlanSpecial') {
                                $scope.targetPlanSpecialModel = JSON.parse(dataStr);
                                $scope.targetPlanSpecialEditWin.title("Target Plan Special For PRD Dialog");
                                $scope.targetPlanSpecialEditWin.center();
                                $scope.targetPlanSpecialEditWin.open();
                            }
                        }
                    }

                    $scope.convertTypeUpdateStatus = function (dataStr) {
                        return dataStr && JSON.parse(dataStr) ? JSON.parse(dataStr).Status : ''
                    }

                    $scope.convertPermission = function (role) {
                        let returnPermission = [];
                        for (var i = 0; i < appSetting.permission.length; i++) {
                            if ((appSetting.permission[i].code & role) > 0) {
                                returnPermission.push(appSetting.permission[i].code);
                            }
                        }

                        var perms = [];
                        for (var i = 0; i < appSetting.permission.length; i++) {
                            for (var j = 0; j < returnPermission.length; j++) {
                                if (appSetting.permission[i].code == returnPermission[j]) {
                                    perms.push(appSetting.permission[i].name);
                                }
                            }
                        }
                        return `${perms.map(x => x)}`
                    }

                    $scope.hideAction = function (type) {
                        let trackingType =
                            [
                                appSetting.TrackingType.UpdatePayload,
                                appSetting.TrackingType.UpdateStatus,
                                appSetting.TrackingType.Update,
                                appSetting.TrackingType.Delete,
                                appSetting.TrackingType.Create,
                                appSetting.TrackingType.UpdateUser,
                                appSetting.TrackingType.DeleteUser,
                                appSetting.TrackingType.AddUser,
                                appSetting.TrackingType.UpdateStatus,
                                appSetting.TrackingType.SyncWorkflow
                            ];
                        return !trackingType.includes(type);
                    }
                    $scope.openTabViewDetail = async function (id, itemType) {
                        if (itemType == 'ShiftExchangeApplication') {
                            //$window.open($window.location.origin + "/#!/home/shift-exchange/viewlog/" + id , '_blank');
                            $window.open($window.location.origin + "/_layouts/15/AeonHRNewLayout/AeonHRNewLayout/Default.aspx#!/home/shift-exchange/viewlog/" + id, '_blank');
                        }
                        else if (itemType == 'TargetPlan') {
                            //$window.open($window.location.origin + "/#!/home/target-plan/viewlog/" + id, '_blank');
                            $window.open($window.location.origin + "/_layouts/15/AeonHRNewLayout/AeonHRNewLayout/Default.aspx#!/home/target-plan/viewlog/" + id, '_blank');
                        }
                        else {
                            //$window.open($window.location.origin + "/#!/home/workflowszzyyxx/viewlog/" + id, '_blank');
                            $window.open($window.location.origin + "/_layouts/15/AeonHRNewLayout/AeonHRNewLayout/Default.aspx#!/home/workflowszzyyxx/viewlog/" + id, '_blank');
                        }
                    }

                    $scope.hideAttachment = function (type) {
                        let trackingType =
                            [
                                appSetting.TrackingType.Update,
                                appSetting.TrackingType.Delete,
                                appSetting.TrackingType.Create,
                                appSetting.TrackingType.UpdateUser,
                                appSetting.TrackingType.DeleteUser,
                                appSetting.TrackingType.AddUser
                            ];
                        return !trackingType.includes(type);
                    }

                    $scope.hideComment = function (type) {
                        let trackingType =
                            [
                                appSetting.TrackingType.Update,
                                appSetting.TrackingType.UpdateUser,
                                appSetting.TrackingType.DeleteUser,
                                appSetting.TrackingType.AddUser,
                                appSetting.TrackingType.Delete,
                                appSetting.TrackingType.Create
                            ];
                        return !trackingType.includes(type);
                    }

                    $scope.hideRound = function (type) {
                        let trackingType =
                            [
                                appSetting.TrackingType.UpdatePayload,
                                appSetting.TrackingType.Update,
                                appSetting.TrackingType.Delete,
                                appSetting.TrackingType.AddUser,
                                appSetting.TrackingType.UpdateUser,
                                appSetting.TrackingType.DeleteUser,
                                appSetting.TrackingType.UpdateInformation,
                                appSetting.TrackingType.Create
                            ];
                        return !trackingType.includes(type);
                    }

                    function customWidthDialog (type, itemType) {
                        let withDialog = '';
                        switch (type) {
                            case appSetting.TrackingType.UpdateApproval:
                                withDialog = "1300px";
                                break;
                            case appSetting.TrackingType.ChangeStepWorkflow:
                            case appSetting.TrackingType.SyncWorkflow:
                                withDialog = "700px";
                                break;
                            case appSetting.TrackingType.UpdatePayload:
                                withDialog = "900px";
                                break;
                            default:
                                withDialog = '1000px';
                                break;
                        }
                        return withDialog;
                    }
                    $scope.oldtrackingHistory = $scope.trackingHistory;
                    $scope.filterDeparment = function (filter) {
                        if ($scope.trackingHistory.length) {
                            var resultFilter = $scope.trackingHistory;
                            /*if (filter.keyword) {
                                resultFilter = _.filter($scope.trackingHistory, x => {
                                    return (x.requestedDepartmentName && x.requestedDepartmentName.toLowerCase().includes($scope.query.keyword.toLowerCase()) || $scope.query.keyword.toLowerCase().includes(x.requestedDepartmentName.toLowerCase()))
                                        || (x.referenceNumber && x.referenceNumber.toLowerCase().includes($scope.query.keyword.toLowerCase()) || $scope.query.keyword.toLowerCase().includes(x.referenceNumber))
                                        || (x.requestorFullName && x.requestorFullName.toLowerCase().includes($scope.query.keyword.toLowerCase()) || $scope.query.keyword.includes(x.requestorFullName.toLowerCase()))
                                        || (x.requestorUserName && x.requestorUserName.toLowerCase().includes($scope.query.keyword.toLowerCase()) || $scope.query.keyword.includes(x.requestorUserName.toLowerCase()))
                                        || (x.status && x.status.toLowerCase().includes($scope.query.keyword.toLowerCase()) || $scope.query.keyword.toLowerCase().includes(x.status.toLowerCase()))
                                });
                            }*/

                            if (filter.departmentCode) {
                                resultFilter = _.filter($scope.trackingHistory, x => {
                                    return (x.itemReferenceNumberOrCode && x.itemReferenceNumberOrCode.includes(filter.departmentCode));
                                });
                            }

                            if (filter.departmentName) {
                                resultFilter = _.filter($scope.trackingHistory, x => {
                                    return (x.itemName && x.itemName.includes(filter.departmentName));
                                });
                            }
                        }
                        if (!resultFilter || resultFilter.length == 0) {
                            var ex = '[{"isJsonDefault":true,"itemType":"Department","type":"Delete"}]';
                            resultFilter = JSON.parse(ex);
                        }
                        $scope.trackingHistory = resultFilter;
                    }

                    $scope.clearSearch = function () {
                        $scope.query = {};
                        $scope.trackingHistory = $scope.oldtrackingHistory;
                    }

                    $scope.closePopup = function () {
                        $scope.query = {};
                        $scope.trackingHistory = $scope.oldtrackingHistory;
                        $("#trackingHistoryDialog").data("kendoDialog").close();
                    }

                    $scope.isWindowUserDepartmentDetail = function (dataType) {
                        var list = [
                            appSetting.TrackingType.AddUser,
                            appSetting.TrackingType.UpdateUser,
                            appSetting.TrackingType.DeleteUser
                        ]
                        return list.includes(dataType);
                    }

                    $scope.groupDataSource = [{
                        id: 1,
                        title: "HOD"
                    },
                    {
                        id: 2,
                        title: "Checker"
                    },
                    {
                        id: 4,
                        title: "Member"
                    },
                    {
                        id: 8,
                        title: "Assistant"
                    }
                    ];

                    $scope.viewHistoryDetail = function (type, itemType, dataStr) {
                        $scope.trackinHistoryDetail = JSON.parse(dataStr);
                        // convert group name for view detail
                        if ($scope.trackinHistoryDetail.AssignFromGroup) {
                            let group = _.find($scope.groupDataSource, x => {
                                return $scope.trackinHistoryDetail.AssignFromGroup == x.id;
                            })
                            if (group)
                                $scope.trackinHistoryDetail.AssignFromGroup = group.title;
                        }

                        if ($scope.trackinHistoryDetail.AssignToGroup) {
                            let group = _.find($scope.groupDataSource, x => {
                                return $scope.trackinHistoryDetail.AssignToGroup == x.id;
                            })
                            if (group)
                                $scope.trackinHistoryDetail.AssignToGroup = group.title;
                        }
                        
                        $scope.trackinHistoryDetail.dataType = type; 
                        $scope.trackinHistoryDetail.itemType = itemType;

                        let dialogReason = $("#trackingHistoryDetailDialog").kendoDialog({
                            title: $translate.instant('COMMON_TRACKING_HISTORY_DETAIL'),
                            width: customWidthDialog($scope.trackinHistoryDetail.dataType, itemType),
                            modal: true,
                            visible: true,
                            animation: {
                                open: {
                                    effects: "fade:in"
                                }
                            },
                            close: function (e) {
                                $scope.trackinHistoryDetail = {};
                            }
                        });
                        let boxReason = dialogReason.data("kendoDialog");
                        boxReason.open();
                        $rootScope.confirmProcessing = boxReason;
                        return $rootScope.confirmProcessing;
                    }
                    
                    $scope.owaClose = function () {
                        if (!$.isEmptyObject($("#attachmentViewDialog").data("kendoDialog"))) {
                            attachmentViewDialog = $("#attachmentViewDialog").data("kendoDialog");
                            attachmentViewDialog.close();
                        }
                    }

                    $scope.removeOldAttachment = function (model) {
                        $scope.trackinHistoryDetail.attachmentFiles = $scope.trackinHistoryDetail.attachmentFiles.filter(item => item.id !== model.id);
                        $scope.removeFiles.push(model.id);
                    }
                    
                    $scope.downloadAttachment = async function (id) {
                        await $rootScope.downloadAttachment(id);
                    }
                    $scope.viewFileOnline = async function (id) {
                        await $rootScope.viewFileOnline(id);
                    }

                    $scope.getValueInDataStr = function (fieldName, dataStr) {
                        let vReturn = "";
                        var dataItem = JSON.parse(dataStr.dataStr);
                        for (var propName in dataItem) {
                            if (fieldName === propName) {
                                if (fieldName === 'ContractTypeCode') {
                                    switch (dataItem[propName]) {
                                        case 1:
                                            vReturn = 'RESIGNATION_CONTRACT_TYPE_01'
                                            break;
                                        case 2:
                                            vReturn = 'RESIGNATION_CONTRACT_TYPE_02'
                                            break;
                                        case 3:
                                            vReturn = 'RESIGNATION_CONTRACT_TYPE_03'
                                            break;
                                        case 4:
                                            vReturn = 'RESIGNATION_CONTRACT_TYPE_04'
                                            break;
                                    }
                                } else {
                                    vReturn = dataItem[propName];
                                }
                                return vReturn
                                    ;
                            }
                        }
                    }
                }
            ]
        };
    }
]);