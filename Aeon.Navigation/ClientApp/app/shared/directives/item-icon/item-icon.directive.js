var ssgApp = angular.module('ssg.directiveItemIconModule', []);
ssgApp.directive("itemIcon", [
    function($rootScope) {
        return {
            restrict: "E",
            templateUrl: "ClientApp/app/shared/directives/item-icon/item-icon.template.html?v=" + edocV,
            scope: {
                type: "@type"
            },
            link: function($scope, element, attr, modelCtrl) {
                $scope.type = attr.type
            },
            controller: [
                "$rootScope", "$scope",'$translate',
                function($rootScope, $scope, $translate) {
                    $scope.icons = {};
                    $scope.names = {};
                    // Edoc 01
                    $scope.icons["Contract"] = "ClientApp/assets/images/icon/request1.png";
                    $scope.icons["NonExpenseContract"] = "ClientApp/assets/images/icon/non-expense.png";
                    $scope.icons["Payment"] = "ClientApp/assets/images/icon/requisition1.png";
                    $scope.icons["Advance"] = "ClientApp/assets/images/icon/advance.png";
                    $scope.icons["Reimbursement"] = "ClientApp/assets/images/icon/reimbursement.png";
                    $scope.icons["ReimbursementPayment"] = "ClientApp/assets/images/icon/icon-NewReimbursementModels.png";
                    $scope.icons["Purchase"] = "ClientApp/assets/images/icon/proposal1.png";
                    $scope.icons["CreditNote"] = "ClientApp/assets/images/icon/creditnote.png";
                    $scope.icons["TransferCash"] = "ClientApp/assets/images/icon/creditnote.png";
                    $scope.icons["RefundCard"] = "ClientApp/assets/images/icon/creditnote.png";
                    // Edoc 02
                    $scope.icons["RequestToHire"] = "ClientApp/assets/images/icon/request-to-hire.jpg";
                    $scope.icons["PromoteAndTransfer"] = "ClientApp/assets/images/icon/promote.png";
                    $scope.icons["Acting"] = "ClientApp/assets/images/icon/Acting.jpg";
                    $scope.icons["LeaveApplication"] = "ClientApp/assets/images/icon/Leave-management.jpg";
                    $scope.icons["MissingTimeClock"] = "ClientApp/assets/images/icon/Missing-Timelock.png";
                    $scope.icons["OvertimeApplication"] = "ClientApp/assets/images/icon/OverTime.jpg";
                    $scope.icons["ShiftExchangeApplication"] = "ClientApp/assets/images/icon/Shift-Exchange.jpg";
                    $scope.icons["ResignationApplication"] = "ClientApp/assets/images/icon/Resignation.jpg";
                    $scope.icons["BusinessTripApplication"] = "ClientApp/assets/images/icon/business_trip.png";
                    $scope.icons["TargetPlan"] = "ClientApp/assets/images/icon/SHIFTPLAN.png";
                    $scope.icons["TrainingRequest"] = "ClientApp/assets/images/icon/academy.png";
                    $scope.icons["TrainingReport"] ="ClientApp/assets/images/icon/bta-report.png";
                    $scope.icons["TrainingInvitation"] = "ClientApp/assets/images/icon/non-expense.png";
                    $scope.icons["BusinessTripOverBudget"] = "ClientApp/assets/images/icon/over_budget.png";

                    //Facilitys
                    $scope.icons["Stationery"] = "ClientApp/assets/images/icon/icon-stationery.png";
                    $scope.icons["Material"] = "ClientApp/assets/images/icon/icon-mateial.png";


                    // Trade Contract
                    $scope.icons["TradeContract"] = "ClientApp/assets/images/icon/TradeContract.png";
                    $scope.icons["Supplier Application Form"] = "ClientApp/assets/images/icon/TradeContract.png";
                    $scope.icons["Supplier Agreement"] = "ClientApp/assets/images/icon/TradeContract.png";
                    $scope.icons["Supplier Audit"] = "ClientApp/assets/images/icon/TradeContract.png";
                    $scope.icons["Document Set Appendix"] = "ClientApp/assets/images/icon/TradeContract.png";
                    $scope.icons["Document Set Liquidation"] = "ClientApp/assets/images/icon/TradeContract.png";
                    $scope.icons["Document Set Announcement"] = "ClientApp/assets/images/icon/TradeContract.png";
                    $scope.icons["Document Set Agreement"] = "ClientApp/assets/images/icon/TradeContract.png";
                    $scope.icons["Freeze Contract Code"] = "ClientApp/assets/images/icon/TradeContract.png";
                    $scope.icons["Extend trading term"] = "ClientApp/assets/images/icon/TradeContract.png";

                    let tradeContractDocumentSet = ["Supplier Application Form", "Supplier Audit", "Document Set Agreement", "Document Set Appendix", "Document Set Liquidation", "Document Set Announcement", "Freeze Contract Code", "Extend trading term"];

                    $scope.icons["Liquor/Document"] = "ClientApp/assets/images/icon/liquor_menu.jpg";
                    $scope.icons["Liquor/Task"] = "ClientApp/assets/images/icon/liquor_menu.jpg";
                    $scope.icons["Liquor/Project"] = "ClientApp/assets/images/icon/liquor_menu.jpg";
                    $scope.icons["Liquor/RetailLicense"] = "ClientApp/assets/images/icon/liquor_menu.jpg";
                    // SKU
                    $scope.icons["SKU"] = "ClientApp/assets/images/icon/sku.jpg";

                    // Edoc 01
                    $scope.names["Contract"] = "Contract";
                    $scope.names["NonExpenseContract"] = "Non-Expense Contract";
                    $scope.names["Payment"] = "Payment";
                    $scope.names["Advance"] = "Advance";
                    $scope.names["Reimbursement"] = "Business Trip Reimbursement";
                    $scope.names["ReimbursementPayment"] = "Reimbursement Payment";
                    $scope.names["Purchase"] = "Purchasing";
                    $scope.names["CreditNote"] = "Credit Note";
                    $scope.names["TransferCash"] = "Transfer Cash";
                    $scope.names["RefundCard"] = "Refund Card";
                    // Edoc 02
                    $scope.names["RequestToHire"] = "Request To Hire";
                    $scope.names["PromoteAndTransfer"] = "Promote And Transfer";
                    $scope.names["Acting"] = "Acting";
                    $scope.names["LeaveApplication"] = "Leave Management";
                    $scope.names["MissingTimeClock"] = "Missing TimeClock";
                    $scope.names["OvertimeApplication"] = "Overtime Application";
                    $scope.names["ShiftExchangeApplication"] = "Shift Exchange Application";
                    $scope.names["ResignationApplication"] = "Resignation Application";
                    $scope.names["BusinessTripApplication"] = "Business Trip Application";
                    $scope.names["TargetPlan"] = "Target Plan";
                    $scope.names["TrainingRequest"] = "Training Request";
                    $scope.names["BusinessTripOverBudget"] = "Business Trip Over Budget";
                    $scope.names["Liquor/Document"] = "Liquor/Document";
                    $scope.names["Liquor/Task"] = "Liquor/Task";
                    $scope.names["Liquor/Project"] = "Liquor/Project";
                    $scope.names["Liquor/RetailLicense"] = "Liquor/RetailLicense";

                    // Sku
                    $scope.names["SKU"] = "SKU";

                    // Facility
                    $scope.names["Stationery"] = "Stationery";
                    $scope.names["Material"] = "Material";

                    $scope.iconUrl = $scope.icons[$scope.type];
                    $scope.name = $scope.names[$scope.type];
                    switch($scope.type){
                        case "RequestToHire":
                            $scope.name = $translate.instant('REQUEST_TO_HIRE_MENU');
                            break;
                        case "Acting":
                            $scope.name = $translate.instant('ACTING_MENU');
                            break;
                        case "PromoteAndTransfer":
                            $scope.name = $translate.instant('PROMOTE_TRANSFER_MENU');
                            break;
                        case "MissingTimeClock":
                            $scope.name = $translate.instant('MISSING_TIMECLOCK_MENU');
                            break;
                        case "LeaveApplication":
                            $scope.name = $translate.instant('LEAVE_MANAGEMENT_MENU');
                            break;
                        case "ShiftExchangeApplication":
                            $scope.name = $translate.instant('SHIFT_EXCHANGE_MENU');
                            break;
                        case "ResignationApplication":
                            $scope.name = $translate.instant('RESIGNATION_APPLICATION_MENU');
                            break;
                        case "OvertimeApplication":
                            $scope.name = $translate.instant('OVERTIME_TIMECLOCK_MENU');
                            break;
                        case "BusinessTripApplication":
                            $scope.name = $translate.instant('BUSINESS_TRIP_APPLICATION');
                            break;
                        case "Target Plan":
                            $scope.name = $translate.instant('TARGET_PLAN');
                            break;
                        case "TrainingRequest":
                            $scope.name = $translate.instant('TRAINING_REQUEST');
                            break;
                        case "TrainingReport":
                            $scope.name = $translate.instant("TRAINING_REPORT");
                            break;
                        case "TrainingInvitation":
                            $scope.name = $translate.instant("TRAINING_INVITATION");
                            break;
                        case "Stationery":
                            $scope.name = $translate.instant("COMMON_STATIONERY");
                            break;
                        case "Material":
                            $scope.name = $translate.instant("COMMON_MATERIAL");
                            break;
                        case "BusinessTripOverBudget":
                            $scope.name = $translate.instant('BUSINESS_TRIP_OVER_BUDGET');
                            break;
                        case "SKU":
                            $scope.name = $translate.instant('SKU');
                            break;
                        default:
                            // Trade Contract
                            if (tradeContractDocumentSet.includes($scope.type)) {
                                $scope.name = $scope.type;
                            } else {
                                $scope.name = $scope.type;
                            }
                    }
                }


            ]
        };
    },
]);