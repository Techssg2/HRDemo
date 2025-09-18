<%@ Assembly Name="Aeon.ManagementPortalUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6d3ecc968062f0d0" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Aeon.ManagementPortalUI.Layouts.AeonHR.Default" MasterPageFile="Blank.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <link type="text/css" rel="stylesheet" href="ClientApp/assets/lib/kendo/styles/kendo.common.min.css?v=<%= Version  %>42" />
    <link type="text/css" rel="stylesheet" href="ClientApp/assets/lib/kendo/styles/kendo.office365.min.css?v=<%= Version  %>42" />
    <link type="text/css" rel="stylesheet" href="ClientApp/assets/lib/kendo/styles/fx.framework.core.css?v=<%= Version  %>42" />
    <link type="text/css" rel="stylesheet" href="ClientApp/assets/css/aeon.edocument.css?v=<%= Version  %>42" />
    <link type="text/css" rel="stylesheet" href="ClientApp/assets/css/shared.css?v=<%= Version  %>42" />
    <link type="text/css" rel="stylesheet" href="ClientApp/assets/css/angular-ui-notification.css?v=<%= Version  %>42" />
    <link type="text/css" rel="stylesheet" href="ClientApp/assets/css/styles.css?v=<%= Version  %>42" />
    <script src="ClientApp/assets/lib/kendo/js/jquery.min.js"></script>
    <script src="ClientApp/assets/lib/kendo/js/jszip.min.js"></script>
    <script src="ClientApp/assets/lib/kendo/js/angular.min.js"></script>
    <script src="ClientApp/assets/lib/kendo/js/kendo.all.min.js"></script>
    <script src="ClientApp/assets/lib/ui-router/angular-ui-router.min.js"></script>
    <script src="ClientApp/assets/lib/kendo/js/angular-cookies.min.js"></script>
    <script src="ClientApp/assets/lib/angular-translate/angular-translate.min.js"></script>
    <script src="ClientApp/assets/lib/angular-translate/angular-translate-loader-static-files.min.js"></script>
    <script src="ClientApp/assets/lib/angular-resource/angular-resource.min.js"></script>
    <script src="ClientApp/assets/lib/lodash/lodash.min.js"></script>
    <script src="ClientApp/assets/lib/file/FileSaver.js"></script>
    <script src="ClientApp/assets/lib/kendo/js/angular-local-storage.min.js"></script>
    <script src="ClientApp/assets/lib/fns/date_fns.min.js"></script>
    <base href="">
</asp:Content>

<asp:Content ID="JsHolder" ContentPlaceHolderID="PlaceCustomJsHolder" runat="server">

	<script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/xlsx/0.13.5/xlsx.full.min.js"></script>

    <!-- Academy -->
    <script src="ClientApp/app/components/academy/utils/academy.router.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/academy/utils/academy.navigation.js?v=<%= Version  %>42"></script>
    <base href="" />
    <!-- / -->

    <!-- My js files -->
    <script src="ClientApp/app/app.module.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/recruitment/recruitment-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/cb/cb-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/setting/setting-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/dashboard/dashboard-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/dashboard/orgchart/orgchart-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/app-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/recruitment/applicant/applicant-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/recruitment/position/position-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/recruitment/promote-and-transfer/promote-and-transfer-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/recruitment/new-staff-onboard/new-staff-onboard-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/recruitment/request-to-hire/request-to-hire-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/cb/overtime-application/overtime-application-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/cb/resignation-application/resignation-application-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/cb/shift-exchange-application/shift-exchange-application-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/cb/leave-management/leave-management-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/setting/departments/department-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/cb/missing-timelock/missing-timelock-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/setting/job-grade/job-grade-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/recruitment/handover/handover-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/setting/recruitments/working-time/recruitment-working-time-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/setting/recruitments/promoteTransferPrint/recruitment-promote-transfer-print-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/setting/recruitments/category/category-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/setting/recruitments/item-list/recruitment-item-list-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/recruitment/action/action-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/setting/cb/c-a-b-reason-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/setting/reference-number/reference-number-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/setting/recruitments/status/recruitment-applicant-status-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/setting/recruitments/appreciation/recruitment-appreciation-list-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/setting/tracking-logs/tracking-log-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/setting/recruitments/position/recruitment-position-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/errors-page/not-found.controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/redirect-page-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/setting/days-configuration/days-configuration-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/setting/recruitments/working-address/recruitment-working-address-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/recruitment/applicant/applicant-directives/applicant-item-g1.directive.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/recruitment/applicant/applicant-directives/applicant-item-g2.directive.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/setting/recruitments/cost-center/recruitment-cost-center-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/setting/users/user-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/setting/budgets/budget-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/setting/workflows/workflow-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/setting/recruitments/category/category-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/cb/shift-plan/pending-target-plan/pending-target-plan-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/cb/shift-plan/target-plan-list/target-plan-list-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/cb/shift-plan/shift-plan-item/shift-plan-item-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/cb/shift-plan/target-plan-list/target-plan-item/target-plan-item-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/setting/cb/holiday-schedule/cb-holiday-schedule-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/setting/shift-plan-submit-person/shift-plan-submit-person-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/cb/shift-plan/report-target-plan/report-target-plan-controller.js?v=<%= Version  %>42"></script>
	
    <script src="ClientApp/app/components/setting/navigation/navigation-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/navigation-home/navigation-home-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/shared/directives/sub-menu-nav-section/sub-menu-nav-section.directive.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/setting/bta/partition/partition-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/bta/business-trip-application/over-budget/over-budget-item-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/bta/business-trip-application/over-budget/over-budget-list-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/setting/maintenant/maintenant-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/errors-page/access-denied.controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/view-log-detail/workflow-view-log-detail-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/view-log-detail/shift-exchange-view-log-detail-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/view-log-detail/target-plan-view-log-detail-controller.js?v=<%= Version  %>42"></script>

    <!-- Shared -->
    <script src="ClientApp/app/app.setting.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/shared/utilities.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/shared/btaHelper.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/shared/SPHandler_Helper.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/shared/moment.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/shared/history.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/shared/angular-ui-notification.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/shared/fileSaver.js?v=<%= Version  %>42"></script>

    <!-- Directive -->
    <script src="ClientApp/app/shared/directives/user-info/user-info.directive.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/shared/directives/sub-menu-section/sub-menu-section.directive.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/shared/directives/excute-jq.directive.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/shared/directives/actions/item-action.directive.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/shared/directives/notification/notification.directive.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/shared/directives/workflow-status/workflow-status.directive.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/shared/directives/item-icon/item-icon.directive.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/shared/directives/workflow-comment/workflow-comment.directive.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/shared/directives/processing-stages/processing-statges.directive.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/shared/directives/processing-stages/workflow-history-task.directive.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/shared/directives/processing-stages/workflow-history-step.directive.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/shared/directives/learn-more/learn-more.directive.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/shared/directives/bta/search-flight.directive.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/shared/directives/bta/flight-ticket/flight-ticket.directive.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/shared/directives/bta/view-change-cancel-ticket/view-change-cancel-ticket.directive.js?v=<%= Version  %>42"></script>
    <!-- Service -->
    <script src="ClientApp/app/components/setting/setting-service.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/cb/cb-service.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/recruitment/recruitment-service.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/cb/cb-service.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/shared/interceptor-service.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/master-data/master-data-service.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/employee/employee-service.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/shared/services/attachmentFile-service.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/services/data-service.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/services/workflow-service.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/services/permission-service.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/services/dashboard-service.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/services/booking-flight-service.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/integration-remote-data/ssgex-service.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/services/file-service.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/shared/services/common-service.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/services/mass-service.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/services/api-service.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/services/navigation-service.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/services/tracking-history-service.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/services/it-service.js?v=<%= Version  %>42"></script>

    <!-- Filter -->
    <script src="ClientApp/app/shared/filter-option.js?v=<%= Version  %>42"></script>

    <script src="ClientApp/app/shared/modules/attachment-file-module.js?v=<%= Version  %>42"></script>
    <!--BTA-Business Trip Application -->
    <script src="ClientApp/app/components/bta/bta-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/bta/bta-service.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/bta/business-trip-application/business-trip-item/business-trip-item-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/bta/business-trip-application/business-trip-list/business-trip-list-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/bta/business-trip-report/business-trip-report-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/setting/bta/airline/airline-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/setting/bta/flight-number/flight-number-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/setting/bta/hotel/hotel-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/setting/bta/location/location-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/setting/bta/room-type/room-type-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/setting/bta/bta-policy/bta-policy-controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/setting/bta/bta-policy-special-case/bta-policy-special-case-controller.js?v=<%= Version  %>42"></script>
	
	<!-- Academy -->
    <!-- MODULES -->
    <script src="ClientApp/app/components/academy/academy.module.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/shared/directives/tracking-history/tracking-history.directive.js?v=<%= Version  %>42"></script>

    <!-- / -->

    <!-- Base -->
    <script src="ClientApp/app/components/academy/common/base.controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/academy/common/interceptor.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/overwrites/dashboard.controller.js?v=<%= Version  %>42"></script>

    <!-- / -->

     <!-- CONTROLLERS -->
    <script src="ClientApp/app/components/academy/pages/training-request/request-item.controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/academy/pages/category/category-management.controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/academy/pages/course/course-management.controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/academy/pages/training-request/request-management.controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/academy/pages/training-invitation/training-invitation.controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/academy/pages/training-request/request-list.controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/academy/pages/training-invitation/myItem/training-invitation-myItem.controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/academy/pages/training-invitation/view-invitation.controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/academy/pages/training-invitation/allItem/training-invitation-allItem.controller.js?v=<%= Version  %>42"></script>	
    <script src="ClientApp/app/components/academy/pages/training-reason/training-reason.controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/academy/pages/training-report/external-training-report/external-training-report.controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/academy/pages/training-report/training-tracker-report/training-tracker-report.controller.js?v=<%= Version  %>42"></script>
	<script src="ClientApp/app/components/academy/pages/training-report/training-survey-report/training-survey-report.controller.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/academy/pages/training-report/training-budget-balance-report/training-budget-balance-report.controller.js?v=<%= Version  %>42"></script>
	<script src="ClientApp/app/components/academy/pages/training-report/trainer-contribution-report/trainer-contribution-report.controller.js?v=<%= Version  %>42"></script>
	<script src="ClientApp/app/components/academy/pages/training-invitation/manageItem/training-invitation-manageItem.controller.js?v=<%= Version  %>42"></script>	
	<script src="ClientApp/app/components/academy/pages/training-report/allItem/training-report-allItem.controller.js?v=<%= Version  %>42"></script>
	<script src="ClientApp/app/components/academy/pages/training-report/myItem/training-report-myItem.controller.js?v=<%= Version  %>42"></script>
	<script src="ClientApp/app/components/setting/bta/global-location/global-location-controller.js?v=<%= Version  %>42"></script>
    <!-- / -->
    <!-- SERVICES -->
    <script src="ClientApp/app/components/academy/services/training-request.service.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/academy/services/category.service.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/academy/services/course.service.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/academy/services/account.service.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/academy/services/training-invitation.service.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/academy/services/training-reason.service.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/academy/services/training-report.service.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/academy/services/f2-integration.service.js?v=<%= Version  %>42"></script>
    <!-- / -->
    <!-- DIRECTIVES -->
    <script src="ClientApp/app/components/academy/directives/common/customCurrency.directive.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/academy/directives/course-duration/course-duration-detail.directive.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/academy/directives/processing-stages/academy-processing-statges.directive.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/academy/directives/processing-stages/academy-workflow-history-step.directive.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/academy/directives/processing-stages/academy-workflow-history-task.directive.js?v=<%= Version  %>42"></script>
    <script src="ClientApp/app/components/academy/directives/workflow-comment/academy-workflow-comment.directive.js?v=<%= Version  %>42"></script>

    <!-- / -->
	<!-- Academy Role -->
	<script type="text/javascript">
		var deptAcademyCode =50022367
	</script>
    <!-- / -->
</asp:Content>
<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
</asp:Content>

