angular
  .module("ssg.module.academy.training-invitation.manage-item.ctrl", [
    "kendo.directives",
  ])
  .controller(
    "TrainingInvitationManageItemController",
    function (
      $rootScope,
      $scope,
      $stateParams,
      $state,
      $window,
      $translate,
      $controller,
      appSetting,
      settingService,
      TrainingInvitationService
    ) {
      $controller("BaseController", { $scope: $scope });

      $scope.title = $translate.instant("TRAINING_INVITATION_MANAGEMENT");
      $scope.currentQuery = {
        pageSize: appSetting.pageSizeDefault,
        pageNumber: 1,
        status: [],
        referenceNumber: null,
        sapCode: null,
        courseType: null,
        createDateFrom: null,
        createDateTo: null,
      };
      $scope.query = {
        status: [],
        referenceNumber: null,
        sapCode: null,
        courseType: null,
        createDateFrom: null,
        createDateTo: null,
      };
      const cloneStatus = [
        { name: "Draft", code: "Draft" },
        { name: "Waiting for Responses", code: "Waiting for Responses" },
        {
          name: "Waiting for After Training Report",
          code: "Waiting for After Training Report",
        },
        { name: "Completed", code: "Completed" },
        { name: "Cancelled", code: "Cancelled" },
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

        $scope.exportToExcel = function () {
            var grid = $("#grid").data("kendoGrid");
            if (grid) {
                grid.saveAsExcel();
            }
        };
      this.$onInit = function () {
        $scope.advancedSearchMode = false;
        $scope.total = 0;
        $scope.data = [];

        $scope.allItemGridOptions = {
          toolbar: [
            { name: "excel", text: $translate.instant("COMMON_BUTTON_EXPORT") },
          ],
          excel: {
            fileName: "TrainingInvitationReport.xlsx",
            proxyURL: "https://demos.telerik.com/kendo-ui/service/export",
            filterable: true,
            allPages: true,
          },
          dataSource: {
            serverPaging: true,
            pageSize: 20,
            transport: {
              read: async function (e) {
                await getInvitationList(e);
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
          sortable: false,
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
              field: "status",
              title: $translate.instant("COMMON_STATUS"),
              template: function (data) {
                var statusTranslate = $rootScope.getStatusTranslate(
                  data.status
                );
                return `<workflow-status status="${statusTranslate}"></workflow-status>`;
              },
              width: "170px",
            },
            {
              field: "referenceNumber",
              title: $translate.instant("REFERENCE_ID"),
              width: "170px",
              template: function (data) {
                return `<a ui-sref="home.academy-trainingInvitation({referenceValue: '${data.id}', id: '${data.id}', invitationId: '${data.id}'})" ui-sref-opts="{ reload: true }">${data.referenceNumber}</a>`;
              },
            },
            {
              field: "numberOfParticipant",
              title: $translate.instant("TRAINING_REQUEST_NO_PARTICIPANTS"),
              width: "170px",
            },
            {
              field: "categoryName",
              title: $translate.instant("CATEGORY_NAME"),
              width: "100px",
            },
            {
              field: "courseName",
              title: $translate.instant("TRAINING_REQUEST_COURSE_NAME"),
              width: "100px",
            },
            {
              field: "serviceProvider",
              title: $translate.instant("SERVICE_PROVIDER"),
              width: "100px",
            },
            {
              field: "trainerName",
              title: $translate.instant("COMMON_TRAINER_NAME"),
              width: "100px",
            },
            {
              field: "startDate",
              title: $translate.instant("COURSE_START_DATE"),
              template: function (dataItem) {
                return moment(dataItem.startDate).format(
                  appSetting.longDateFormat
                );
              },
              width: "100px",
            },
            {
              field: "endDate",
              title: $translate.instant("COURSE_END_DATE"),
              template: function (dataItem) {
                return moment(dataItem.endDate).format(
                  appSetting.longDateFormat
                );
              },
              width: "100px",
            },
            {
              field: "createDate",
              title: $translate.instant("COMMON_CREATED_DATE"),
              template: function (dataItem) {
                return moment(dataItem.createDate).format(
                  appSetting.longDateFormat
                );
              },
              width: "100px",
            },
          ],
        };
      };
      $scope.toggleFilterPanel = function (value) {
        $scope.advancedSearchMode = value;
      };

      $scope.search = function () {
        functionType = "search";
        loadPageOne();
      };

      $scope.clearSearch = function () {
        $scope.query = {
          status: "",
          referenceNumber: "",
          sapCode: "",
          courseType: "",
          createDateFrom: null,
          createDateTo: null,
        };
        loadPageOne();
      };

      function loadPageOne() {
        let grid = $("#grid").data("kendoGrid");
        if (grid) {
          grid.dataSource.fetch(() => grid.dataSource.page(1));
        }
      }

      function convertDate(date) {
        if (date) {
          let arrDate = date.split("/");
          let newDate = arrDate[1] + "/" + arrDate[0] + "/" + arrDate[2];
          return new Date(newDate);
        } else return "";
      }

      $rootScope.$on("isEnterKeydown", function (result, data) {
        if ($scope.advancedSearchMode && data.state == $state.current.name) {
          $scope.search();
        }
      });
      async function getInvitationList(option) {
        $scope.data = [];

        $scope.currentQuery.status = $scope.query.status;
        $scope.currentQuery.referenceNumber = $scope.query.referenceNumber;
        $scope.currentQuery.sapCode = $scope.query.sapCode;
        $scope.currentQuery.courseType = $scope.query.courseType;
        $scope.currentQuery.createDateFrom = convertDate(
          $scope.query.createDateFrom
        );
        $scope.currentQuery.createDateTo = convertDate(
          $scope.query.createDateTo
        );

        if (option) {
          $scope.currentQuery.pageSize = option.data.take;
          $scope.currentQuery.pageNumber = option.data.page;
        }

        if (option) {
          let result = await TrainingInvitationService.getAll(
            $scope.currentQuery
          ).$promise;
          if (result) {
            $scope.data = result.data;
            $scope.total = result.count;
            option.success($scope.data);
          }
        } else {
          let grid = $("#grid").data("kendoGrid");
          grid.dataSource.read();
          grid.dataSource.page($scope.currentQuery.pageNumber);
        }
      }
    }
  );
