angular
  .module("ssg.module.academy.request-list.ctrl", ["kendo.directives"])
  .controller(
    "RequestListController",
    function (
      $rootScope,
      $scope,
      $stateParams,
      $state,
      $window,
      $translate,
      $controller,
      appSetting,
      Notification,
      TrainingRequestService,
      dashboardService
    ) {
      stateItem = "home.academy-editRequest";
      $scope.title = $stateParams.action.title;
      $scope.realTitle =
        $stateParams.action.title === "All Academy Requests"
          ? $translate.instant("TRAINING_REQUEST_COMMON_ALL_REQUEST")
          : $translate.instant("TRAINING_REQUEST_COMMON_MY_REQUEST");
      $scope.data = [];
      $scope.currentQuery = {};
      $scope.total = 0;
      $scope.showColumn = $stateParams.action.title === "All Academy Requests";
      $scope.currentQuery = {
        pageSize: appSetting.pageSizeDefault,
        pageNumber: 1,
        status: [],
        courseName: "",
        type: [],
        status: [],
        keyword: "",
        createDateFrom: null,
        createDateTo: null,
      };
      $scope.query = {
        courseName: "",
        type: [],
        status: [],
      };
      const cloneStatus = [
        { name: "Pending", code: "Pending" },
        { name: "Completed", code: "Completed" },
        { name: "Draft", code: "Draft" },
        { name: "Rejected", code: "Rejected" },
        { name: "Requested To Change", code: "Requested To Change" },
      ];
      $scope.typeofStatus = {
        placeholder: "",
        dataTextField: "name",
        dataValueField: "code",
        valuePrimitive: true,
        checkboxes: false,
        autoBind: false,
        filter: "contains",
        dataSource: {
          data: translateStatus($translate, cloneStatus),
        },
      };
      const cloneTypes = [
        { name: "External", code: "External" },
        { name: "Internal", code: "Internal" },
      ];
      $scope.trainingTypesOptions = {
        placeholder: "",
        dataTextField: "name",
        dataValueField: "code",
        valuePrimitive: true,
        checkboxes: false,
        autoBind: false,
        filter: "contains",
        dataSource: {
          data: translateStatus($translate, cloneTypes),
        },
      };

      this.$onInit = async function () {
        $scope.advancedSearchMode = false;

        $scope.showColumn =
          $stateParams.action.title === "All Academy Requests";
        $scope.allOrMyRequestGridOptions = {
          toolbar: [
            { name: "excel", text: $translate.instant("COMMON_BUTTON_EXPORT") },
          ],
          excel: {
            fileName: "Training Request.xlsx",
            proxyURL: "https://demos.telerik.com/kendo-ui/service/export",
            filterable: true,
            allPages: true,
          },
          dataSource: {
            serverPaging: true,
            pageSize: 20,
            transport: {
              read: async function (e) {
                await getAllTrainingRequest(e);
              },
            },
            schema: {
              total: () => {
                return $scope.total;
              },
              data: () => {
                return $scope.data;
              },
            },
          },
          resizable: true,
          sortable: true,
          autoBind: true,
          pageable: {
            alwaysVisible: true,
            pageSizes: appSetting.pageSizesArray,
            messages: {
              display:
                "{0}-{1} " +
                $translate.instant("PAGING_OF") +
                " {2} " +
                $translate.instant("PAGING_ITEM"),
              itemsPerPage: $translate.instant("PAGING_ITEM_PER_PAGE"),
              empty: $translate.instant("PAGING_NO_ITEM"),
            },
          },
          columns: [
            {
              title: "",
              width: "250px",
              template: function (data) {
                return `<item-icon type="${data.itemType}"></item-icon>`;
              },
            },
            {
              field: "status",
              headerTemplate: $translate.instant("COMMON_STATUS"),
              width: "200px",
              template: function (dataItem) {
                var statusTranslate = $rootScope.getStatusTranslate(
                  dataItem.status
                );
                return `<workflow-status status="${statusTranslate}"></workflow-status>`;
              },
            },
            {
              field: "referenceNumber",
              headerTemplate: $translate.instant("COMMON_REFERENCE_NUMBER"),
              width: "180px",
              template: function (dataItem) {
                return `<a ui-sref= "${stateItem}({referenceValue:'${dataItem.referenceNumber}', id: '${dataItem.itemId}'})" ui-sref-opts="{ reload: true }">${dataItem.referenceNumber}</a>`;
              },
            },
            {
              field: $scope.showColumn
                ? "requestorFullName"
                : "createdByFullName",
              headerTemplate: $translate.instant("COMMON_REFERENCE_REQUESTOR"),
              width: "200px",
            },
            {
              field: "requestedDepartmentName",
              headerTemplate: $translate.instant("COMMON_DEPT_LINE"),
              width: "350px",
              hidden: !$scope.showColumn,
              template: function (data) {
                return `<div kendo-tooltip k-content="'${data.requestedDepartmentCode}'" >${data.requestedDepartmentName}</div>`;
              },
            },
            {
              field: "regionName",
              headerTemplate: $translate.instant("COMMON_REGION"),
              width: "100px",
              hidden: !$scope.showColumn,
              template: function (data) {
                return `${data.regionName}`;
              },
            },
            {
              field: "created",
              headerTemplate: $translate.instant("COMMON_CREATED_DATE"),
              width: "200px",
              lockable: false,
              sortable: {
                initialDirection: "desc",
              },
              template: function (dataItem) {
                return moment(dataItem.created).format(
                  appSetting.longDateFormat
                );
              },
            },
            {
              field: "modified",
              headerTemplate: $translate.instant("COMMON_MODIFIED_DATE"),
              width: "200px",
              lockable: false,
              template: function (dataItem) {
                return moment(dataItem.modified).format(
                  appSetting.longDateFormat
                );
              },
            },
          ],
        };
        $window.scrollTo(0, 0);
      };
      $scope.toggleFilterPanel = function (value) {
        $scope.advancedSearchMode = value;
      };
      function loadPageOne() {
        let grid = $("#gridTrainingRequests").data("kendoGrid");
        if (grid) {
          grid.dataSource.fetch(() => grid.dataSource.page(1));
        }
      }
      $scope.applySearch = function () {
        loadPageOne();
      };
      $scope.clearSearch = function () {
        $scope.query = {
          courseName: "",
          type: [],
          status: [],
          keyword: '',
          createDateFrom: null,
          createDateTo: null,
        };
        loadPageOne();
      };
      async function getAllTrainingRequest(option) {
        var optionGet = {
          pageSize: 10,
          pageNumber: 1,
        };
        $scope.data = [];
        $scope.currentQuery = { 
          ...$scope.query,
          createDateFrom: convertDate($scope.query.createDateFrom),
          createDateTo: convertDate($scope.query.createDateTo),
        };
        if (option) {
          $scope.currentQuery.pageSize = option.data.take;
          $scope.currentQuery.pageNumber = option.data.page;
        }
        if ($stateParams.action.title === "All Academy Requests") {
          var result = await TrainingRequestService.allRequest(
            $scope.currentQuery
          ).$promise;
          $scope.total = result.count;
          $scope.data = JSON.parse(JSON.stringify(result.data));
          option.success(JSON.parse(JSON.stringify(result.data)));
        } else {
          var result = await TrainingRequestService.myRequest(
            $scope.currentQuery
          ).$promise;
          $scope.total = result.count;
          $scope.data = JSON.parse(JSON.stringify(result.data));
          option.success(JSON.parse(JSON.stringify(result.data)));
        }
      }
      $rootScope.$on("isEnterKeydown", function (result, data) {
        if ($scope.advancedSearchMode && data.state == $state.current.name) {
          $scope.search();
        }
      });
      function convertDate(date) {
        if (date) {
          let arrDate = date.split("/");
          let newDate = arrDate[1] + "/" + arrDate[0] + "/" + arrDate[2];
          return new Date(newDate);
        } else return "";
      }
    }
  );
