var ssgApp = angular.module('ssg.settingMaintenantModule', ['kendo.directives']);
ssgApp.controller('settingMaintenantController', function ($rootScope, $scope, $location, appSetting, $stateParams, $state, commonData, moment, Notification, $timeout, recruitmentService, masterDataService, settingService, attachmentFile, $anchorScroll) {
    // create a message to display in our view
    var ssg = this;
    $scope.title = '';
    $scope.unlockUser = {
        userSAPCode:""
    };
    $scope.syncUser = {
        userSAPCode:""
    };
    $scope.otPayload = {
        referenceCode: "",
        payloadDetails: ""
    };


    $scope.itemWrongStatusOptions = {
        dataSource: {
            serverPaging: false,
            pageSize: appSetting.pageSizeDefault,
            transport: {
                read: async function (e) {
                    await $scope.getItemsWrongStatus(e);
                }
            },
            schema: {
                data: () => { return $scope.itemWrongStatusData }
            }
        },
        noRecords: true,
        sortable: false,
        valuePrimitive: false,
        columns: [{
            field: "itemReferenceNumber",
            title: "Reference Number",
            width: "250px"
        }, {
            field: "itemId",
            title: "Item Id",
            width: "350px"
        },
        {
            field: "status",
            title: "Status",
            width: "250px"
        },
        {
            title: "Update Status",
            template: function (dataItem) {
                return `<input  autocomplete="off" class="k-input" type="text" ng-model="dataItem.status" style="color: #333;width:250px;" />`;
            }
        },
        {
            title: "Actions",
            width: "180px",
            template: function (dataItem) {
                var applicationList = new Array("overtimeApplication", "resignationApplication", "business-trip-application", "missingTimelock", "leavesManagement", "shiftExchange", "targetPlan", "action", "promoteAndTransfer", "handover", "requestToHire");
                var applicationPrefixList = new Array("OVE", "RES", "BTA", "MIS", "LEA", "SHI", "TAR", "ACT", "PRO", "HAN", "REQ");
                var itemRef = dataItem.itemReferenceNumber;
                var applicationName = applicationList[applicationPrefixList.indexOf(itemRef.substring(0, 3))];
                return `<a class='btn btn-sm default blue-stripe' ng-click="updateStatus('${itemRef}')">Update</a>` +
                    `<a class='btn btn-sm default blue-stripe' target='_blank' ui-sref="home.${applicationName}.item({referenceValue:'${itemRef}', id: '${dataItem.itemId}'})" ui-sref-opts="{ reload: true }">Go to</a>`;
            }
        }
        ]
    };

    $scope.updateStatus = function (itemRef) {
        let res = settingService.getInstance().maintenant.updateStatus({ referenceNumber: itemRef }).$promise;
        alert("Chay roi bam scan lai coi co mat khong :))")
    }

    $scope.itemNotHavePayloadOptions = {
        dataSource: {
            serverPaging: false,
            pageSize: 400,
            transport: {
                read: async function (e) {
                    await $scope.getItemsNotHavePayload(e);
                }
            },
            schema: {
                data: () => { return $scope.itemNotHavePayloadData }
            }
        },
        noRecords: true,
        sortable: false,
        valuePrimitive: false,
        columns: [{
            field: "itemReferenceNumber",
            title: "Reference Number",
            width: "250px"
        },
        {
            field: "itemId",
            title: "Item Id",
            width: "350px"
        },
        {
            title: "Actions",
            width: "180px",
            template: function (dataItem) {
                var applicationList = new Array("overtimeApplication", "resignationApplication", "business-trip-application", "missingTimelock", "leavesManagement", "shiftExchange", "targetPlan", "action", "promoteAndTransfer", "handover", "requestToHire");
                var applicationPrefixList = new Array("OVE", "RES", "BTA", "MIS", "LEA", "SHI", "TAR", "ACT", "PRO", "HAN", "REQ");
                var itemRef = dataItem.itemReferenceNumber;
                var applicationName = applicationList[applicationPrefixList.indexOf(itemRef.substring(0, 3))] ;
                return `<a class='btn btn-sm default blue-stripe' target='_blank' ui-sref="home.${applicationName}.item({referenceValue:'${itemRef}', id: '${dataItem.itemId}'})" ui-sref-opts="{ reload: true }">Go to</a>
                        <a style='display:none;' class='btn btn-sm default blue-stripe' ng-click="submitPayload('${dataItem.itemId}')">Submit Payload</a>`;
            }
        }
        ]
    };

    $scope.submitPayload = async function (itemId) {
        let res = await settingService.getInstance().maintenant.submitPayload({itemID: itemId }).$promise;
        if (res.isSuccess) {
            $scope.getItemsNotHavePayload();
        }
        else {
            alert('Failed');
        }
    }


    $scope.itemsHadPendingOptions = {
        dataSource: {
            serverPaging: false,
            pageSize: appSetting.pageSizeDefault,
            transport: {
                read: async function (e) {
                    await $scope.getItemsHadPending(e);
                }
            },
            schema: {
                data: () => { return $scope.itemsHadPendingData }
            }
        },
        noRecords: true,
        sortable: false,
        valuePrimitive: false,
        columns: [{
            field: "itemReferenceNumber",
            title: "Reference Number",
            width: "250px"
        }, {
            field: "itemId",
            title: "Item Id",
            width: "350px"
        },
        {
            title: "",
            template: function (dataItem) {
                return ``;
            }
        },
        {
            title: "Actions",
            width: "180px",
            template: function (dataItem) {
                var applicationList = new Array("overtimeApplication", "resignationApplication", "business-trip-application", "missingTimelock", "leavesManagement", "shiftExchange", "targetPlan", "action", "promoteAndTransfer", "handover", "requestToHire");
                var applicationPrefixList = new Array("OVE", "RES", "BTA", "MIS", "LEA", "SHI", "TAR", "ACT", "PRO", "HAN", "REQ");
                var itemRef = dataItem.itemReferenceNumber;
                var applicationName = applicationList[applicationPrefixList.indexOf(itemRef.substring(0, 3))];
                return `<a class='btn btn-sm default blue-stripe' target='_blank' ui-sref="home.${applicationName}.item({referenceValue:'${itemRef}', id: '${dataItem.itemId}'})" ui-sref-opts="{ reload: true }">Go to</a>`;
            }
        }
        ]
    };

    $scope.copyToClipboard = function (textNeedToCopy) {
        var $temp = $("<input>");
        $("body").append($temp);
        $temp.val(textNeedToCopy).select();
        document.execCommand("copy");
        $temp.remove();
    }

    $scope.viewPayload = function () {
        let payloadDetails= this.dataItem.payload;
        $scope.otPayload.payloadDetails = payloadDetails;
        let dialog = $("#payloadDetailsDialog").data("kendoDialog");
        dialog.open();
    }

    $scope.copyPayload = function () {
        let payloadDetails = this.dataItem.payload;
        $scope.copyToClipboard(payloadDetails);

        Notification.success("Payload copied successfully.");
    }

    $scope.payloadDetailsDialogOption = {
        buttonLayout: "normal",
        title:"Payload Details",
        animation: {
            open: {
                effects: "fade:in",
            },
        },
        schema: {
            model: {
                id: "no",
            },
        },
        actions: [
            {
                text: "Copy Payload",
                action: function (e) {
                    $scope.copyToClipboard($scope.otPayload.payloadDetails);
                    Notification.success("Payload copied successfully.");
                    return false;
                },
                primary: true,
            },
            {
                text: "OK",
                action: function (e) {
                    let dialog = $("#payloadDetailsDialog").data("kendoDialog");
                    dialog.close();
                    return false;
                },
                primary: true,
            }
        ],
    };

    $scope.userLockedStatusOptions = {
        dataSource: {
            serverPaging: false,
            pageSize: appSetting.pageSizeDefault,
            transport: {
                read: async function (e) {
                    await $scope.getUserLockedStatus(e);
                }
            },
            schema: {
                data: () => { return $scope.userLockedStatusData }
            }
        },
        noRecords: true,
        sortable: false,
        valuePrimitive: false,
        columns: [{
            field: "sapCode",
            title: "SAP Code",
            width: "100px"
        },
        {
            field: "fullName",
            title: "Full Name",
        },
        {
            field: "loginName",
            title: "Login Name",
        },
        {
        field: "type",
        title: "Login Type",
        },
        {
            field: "isLoginLocked",
            title: "Is Login Locked",
        },
        {
            title: "",
            template: function (dataItem) {
                return ``;
            }
        },
        {
            title: "Actions",
            width: "180px",
            template: function (dataItem) {
                var actionHTML = `<a class='btn btn-sm default blue-stripe' ng-click='unlockedUser("${dataItem.sapCode}")'>Unlock</a>`;
                if (!dataItem.isLoginLocked) {
                    actionHTML = "";
                }
                return actionHTML;
            }
        }
        ]
    };


    $scope.reGenerateOTPayloadOptions  = {
        dataSource: {
            serverPaging: false,
            pageSize: 200,
            transport: {
                read: async function (e) {
                    await $scope.getItemsWrongStatus(e);
                }
            },
            schema: {
                data: () => { return $scope.itemWrongStatusData }
            }
        },
        noRecords: true,
        sortable: false,
        valuePrimitive: false,
        columns: [
            {
                field: "referenceNumber",
                title: "Reference Number",
                width: "150px"
            },
            {
                field: "payload",
                title: "Payload",
                width: "150px"
            },
            {
                field: "status",
                title: "Status",
                width: "100px"
            },
            {
                field: "modified",
                title: "Modified",
                width: "150px"
            },
            {
                title: "Actions",
                width: "180px",
                template: function (dataItem) {
                    var actionHTML = `<a class='btn btn-sm default blue-stripe' ng-click='copyPayload()'>Copy Payload</a>`;
                    actionHTML += `<a class='btn btn-sm default blue-stripe' ng-click='viewPayload()'>View Payload</a>`;
                    return actionHTML;
                }
            }
        ]
    };

    $scope.generateOTPayload = async function () {
        let res = await settingService.getInstance().maintenant.generateOTPayload({ otReferenceNumber: $scope.otPayload.referenceCode }).$promise;
        if (res.isSuccess) {
            $scope.otPayloads = res.object;
            var grid = $("#otPayloadGrid").data("kendoGrid");
            grid.setDataSource($scope.otPayloads);
        }
    }

    $scope.getItemsWrongStatus = async function (option) {
        let res = await settingService.getInstance().maintenant.getItemsHasWrongStatus().$promise;
        if (res.isSuccess) {
            $scope.itemWrongStatusData = res.object;
        }
        if (option) {
            option.success($scope.itemWrongStatusData);
        } else {
            var grid = $("#itemWrongStatusGrid").data("kendoGrid");
            grid.dataSource.read();
        }
    }

    $scope.unlockedUser = async function (userSAPCode) {
        let res = await settingService.getInstance().maintenant.unlockedUser({ sapCode: userSAPCode }).$promise;
        if (res.isSuccess) {
            $scope.getUserLockedStatus();
        }
    }

    $scope.getItemsNotHavePayload = async function (option) {
        let res = await settingService.getInstance().maintenant.getItemsNotHavePayload().$promise;
        if (res.isSuccess) {
            $scope.itemNotHavePayloadData = res.object;
        }
        if (option) {
            option.success($scope.itemNotHavePayloadData);
        } else {
            var grid = $("#itemNotHavePayloadGrid").data("kendoGrid");
            grid.dataSource.read();
        }
    }

    $scope.getItemsHadPending = async function (option) {
        let res = await settingService.getInstance().maintenant.getItemsHadPending().$promise;
        if (res.isSuccess) {
            $scope.itemsHadPendingData = res.object;
        }
        if (option) {
            option.success($scope.itemsHadPendingData);
        } else {
            var grid = $("#itemHadPendingGrid").data("kendoGrid");
            grid.dataSource.read();
        }
    }

    $scope.stripOptions = {
        animation: false,
    }

    $scope.changTab = async function (tabIdGo) {
        var tabToActivate = $("#" + tabIdGo);
        $("#tabstrip").kendoTabStrip().data("kendoTabStrip").activateTab(tabToActivate);
    }

    $scope.getUserLockedStatus = async function (option) {
        let res = await settingService.getInstance().maintenant.getUserLockedStatus({ sapCode: $scope.unlockUser.userSAPCode }).$promise;


        if (res.isSuccess) {
            if ($.isEmptyObject(res.object)) {
                res.object = new Array();
            }
            $scope.userLockedStatusData = res.object;
        }
        if (option) {
            option.success($scope.userLockedStatusData);
        } else {
            var grid = $("#userLockedStatusGrid").data("kendoGrid");
            grid.dataSource.read();
        }
    }

    $scope.syncUserDataFromSAP = async function () {
        let res = await settingService.getInstance().maintenant.syncUserDataFromSAP({ sapCode: $scope.unlockUser.userSAPCode }).$promise;
        if (res.isSuccess) {
            alert('User data sync completed');
        }
        else {
            alert('User data sync failed');
        }
    }

    this.$onInit = async () => {
        
    }
});