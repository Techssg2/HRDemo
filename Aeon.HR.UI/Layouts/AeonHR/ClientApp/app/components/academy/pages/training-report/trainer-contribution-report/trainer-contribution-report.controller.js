angular
  .module("ssg.academy.trainer-contribution-report.module", [
    "kendo.directives",
  ])
  .controller(
    "TrainerContributionReport",
    function (
      $scope,
      $rootScope,
      $window,
      $translate,
      $controller,
      appSetting,
      Notification,
      $state,
      $stateParams,
      TrainingReportService
    ) {
      $controller("BaseController", { $scope: $scope });
      $scope.title = $translate.instant("TRAINER_CONTRIBUTION_REPORT");
      $scope.titleEdit = $translate.instant("TRAINER_CONTRIBUTION_REPORT");
      $scope.query = {
        sapCode: "",
        courseName: "",
        trainerName: "",
      };
      $scope.currentQuery = {
        pageSize: appSetting.pageSizeDefault,
        pageNumber: 1,
        sapCode: "",
        courseName: "",
        trainerName: "",
      };
      function loadPageOne() {
        let grid = $("#trainerContributionReport").data("kendoGrid");
        if (grid) {
          grid.dataSource.fetch(() => grid.dataSource.page(1));
        }
      }
      async function loadData(option) {
        $scope.data = [];
        $scope.currentQuery = { ...$scope.query };
        if (option) {
          $scope.currentQuery.pageSize = option.data.take;
          $scope.currentQuery.pageNumber = option.data.page;
        }

        if (option) {
          let result =
            await TrainingReportService.getTrainerContributionReports(
              $scope.currentQuery
            ).$promise;
          if (result) {
            $scope.data = result.data.map((item, index) => {
              item.no = index + 1;
              return item;
            });
            $scope.total = result.count;
            option.success($scope.data);
          }
        } else {
          let grid = $("#trainerContributionReport").data("kendoGrid");
          grid.dataSource.read();
          grid.dataSource.page($scope.currentQuery.pageNumber);
        }
      }
      this.$onInit = async function () {
        $scope.advancedSearchMode = false;
        $scope.total = 0;
        $scope.data = [];
        $scope.trainerContributionGridOptions = {
          dataSource: {
            serverPaging: true,
            pageSize: 20,
            transport: {
              read: async function (e) {
                await loadData(e);
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
          toolbar: [
            { name: "excel", text: $translate.instant("COMMON_BUTTON_EXPORT") },
          ],
          excel: {
            fileName: "Trainer Contribution Report.xlsx",
            proxyURL: "https://demos.telerik.com/kendo-ui/service/export",
            filterable: true,
            allPages: true,
          },
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

          sortable: true,
          columnMenu: false,
          resizable: false,
          columns: [
            {
              field: "no",
              title: $translate.instant("COMMON_NO"),
              width: 20,
              editable: function (e) {
                return false;
              },
            },
            {
              field: "sapCode",
              title: $translate.instant("COMMON_SAP_CODE"),
              width: 80,
            },
            {
              field: "fullName",
              title: $translate.instant("COMMON_TRAINER_NAME"),
              width: 100,
            },
            {
              field: "department",
              title: $translate.instant("COMMON_DEPARTMENT"),
              width: 150,
            },
            {
              field: "courseName",
              title: $translate.instant("TRAINING_REQUEST_COURSE_NAME"),
              width: 100,
            },
            {
              field: "startDate",
              title: $translate.instant("COURSE_START_DATE"),
              width: 100,

              template: function (dataItem) {
                return dataItem.startDate
                  ? moment(dataItem.startDate).format(appSetting.longDateFormat)
                  : "";
              },
            },
            {
              field: "endDate",
              title: $translate.instant("COURSE_END_DATE"),
              width: 100,

              template: function (dataItem) {
                return dataItem.endDate
                  ? moment(dataItem.endDate).format(appSetting.longDateFormat)
                  : "";
              },
            },
            {
              field: "courseDuration",
              title: $translate.instant("TRAINING_HOURS_PER_DAY"),
              width: 60,
            },

            {
              field: "totalCourseAttended",
              title: $translate.instant("TRAINING_NUMBER_OF_DATE"),
              width: 60,
            },
            {
              field: "totalTimeAttended",
              title: $translate.instant("TRAINING_TOTOAL_HOURS"),
              width: 60,
            },
          ],
        };
      };
      $scope.toggleFilterPanel = function (value) {
        $scope.advancedSearchMode = value;
      };
      $scope.search = function () {
        loadPageOne();
      };
      $scope.clearSearch = function () {
        $scope.query = {
          sapCode: "",
          courseName: "",
          trainerName: "",
        };
        loadPageOne();
      };
      $rootScope.$on("isEnterKeydown", function (result, data) {
        if ($scope.advancedSearchMode && data.state == $state.current.name) {
          $scope.search();
        }
      });
    }
  );
