angular
  .module("ssg.module.academy.training-tracker-report.ctrl", [
    "kendo.directives",
  ])
  .controller(
    "TrainingTrackerReportController",
    function (
      $rootScope,
      $scope,
      $stateParams,
      $state,
      $window,
      $translate,
      $controller,
      appSetting,
      TrainingReportService
    ) {
      $controller("BaseController", { $scope: $scope });

      $scope.title = $translate.instant("TRAINING_TRACKER_REPORT");

      $scope.currentQuery = {
        pageSize: appSetting.pageSizeDefault,
        pageNumber: 1,
        sapCode: "",
        fullName: "",
        courseType: "",
        supplierName: "",
        typeofTraining: "",
        startingDateFrom: null,
        startingDateTo: null,
        endingDateFrom: null,
        endingDateTo: null,
      };
      $scope.query = {
        sapCode: "",
        fullName: "",
        courseType: "",
        supplierName: "",
        typeofTraining: "",
        startingDateFrom: null,
        startingDateTo: null,
        endingDateFrom: null,
        endingDateTo: null,
      };

      const cloneStatus = [
        { name: "Internal", code: "Internal" },
        { name: "External", code: "External" },
      ];
      $scope.typeofTrainingOptions = {
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
            fileName: "TrainingTrackerReport.xlsx",
            proxyURL: "https://demos.telerik.com/kendo-ui/service/export",
            filterable: true,
            allPages: true,
          },
          dataSource: {
            serverPaging: true,
            pageSize: 20,
            transport: {
              read: async function (e) {
                await getTrackerReportList(e);
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
              field: "sapCode",
              title: $translate.instant("COMMON_SAP_CODE"),
              width: "170px",
            },
            {
              field: "fullName",
              title: $translate.instant("COMMON_FULL_NAME"),
              width: "170px",
            },
            {
              field: "deptLine",
              title: $translate.instant("LINE_DEPARTMENT"),
              width: "170px",
            },
            {
              field: "division",
              title: $translate.instant("DIVISION_SECTION"),
              width: "170px",
              template: function (dataItem) {
                let division = dataItem.division ? dataItem.division : "";
                return division;
              },
            },
            {
              field: "jobGrade",
              title: $translate.instant("JOBGRADE_MENU"),
              width: "170px",
            },
            {
              field: "courseName",
              title: $translate.instant("TRAINING_REQUEST_COURSE_NAME"),
              width: "170px",
            },
            {
              field: "startingDate",
              title: $translate.instant("STARTING_DATE"),
              template: function (dataItem) {
                return dataItem.startingDate
                  ? moment(dataItem.startingDate).format(
                      appSetting.longDateFormat
                    )
                  : "";
              },
              width: "170px",
            },
            {
              field: "endingDate",
              title: $translate.instant("ENDING_DATE"),
              template: function (dataItem) {
                return dataItem.endingDate
                  ? moment(dataItem.endingDate).format(
                      appSetting.longDateFormat
                    )
                  : "";
              },
              width: "170px",
            },
            {
              field: "actualAttendingDate",
              title: $translate.instant("ACTUAL_ATTENDING_DATE"),
              template: function (dataItem) {
                return dataItem.actualAttendingDate
                  ? moment(dataItem.actualAttendingDate).format(
                      appSetting.longDateFormat
                    )
                  : "";
              },
              width: "200px",
            },
            {
              field: "supplierName",
              title: $translate.instant("TRAINING_REQUEST_SUPPLIER_NAME"),
              width: "170px",
            },
            {
              field: "totalOnlineTrainingHours",
              title: $translate.instant("TOTAL_ONLINE_TRAINING_HOUR"),
              width: "200px",
            },
            {
              field: "totalOfflineTrainingHours",
              title: $translate.instant("TOTAL_OFFLINE_TRAINING_HOUR"),
              width: "200px",
            },
            {
              field: "typeOfTraining",
              title: $translate.instant("TRAINING_REQUEST_TYPE_OFF_TRAINING"),
              width: "170px",
            },
            {
              field: "trainingFee",
              title: $translate.instant("TRAINING_REQUEST_TRAINING_REE"),
              width: "170px",
              template: function (dataItem) {
                return `<span>{{dataItem.trainingFee |  currency:"":0}} VND</span>`;
              },
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
          sapCode: "",
          fullName: "",
          courseType: "",
          supplierName: "",
          typeofTraining: "",
          startingDateFrom: null,
          startingDateTo: null,
          endingDateFrom: null,
          endingDateTo: null,
        };
        $scope.$broadcast("resetToDate", $scope.query.startingDateFrom);
        $scope.$broadcast("resetToDate", $scope.query.startingDateTo);
        $scope.$broadcast("resetToDate", $scope.query.endingDateFrom);
        $scope.$broadcast("resetToDate", $scope.query.endingDateTo);
        loadPageOne();
      };

      function loadPageOne() {
        let grid = $("#grid").data("kendoGrid");
        if (grid) {
          grid.dataSource.fetch(() => grid.dataSource.page(1));
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
          let newDate = arrDate[2] + "/" + arrDate[1] + "/" + arrDate[0];
          return newDate;
        } else return "";
      }
      async function getTrackerReportList(option) {
        $scope.data = [];

        $scope.currentQuery.sapCode = $scope.query.sapCode;
        $scope.currentQuery.fullName = $scope.query.fullName;
        //$scope.currentQuery.courseType = $scope.query.courseType;
        $scope.currentQuery.supplierName = $scope.query.supplierName;
        $scope.currentQuery.typeofTraining = $scope.query.typeofTraining;
        $scope.currentQuery.startingDateFrom = convertDate(
          $scope.query.startingDateFrom
        );
        $scope.currentQuery.startingDateTo = convertDate(
          $scope.query.startingDateTo
        );
        $scope.currentQuery.endingDateFrom = convertDate(
          $scope.query.endingDateFrom
        );
        $scope.currentQuery.endingDateTo = convertDate(
          $scope.query.endingDateTo
        );

        console.log($scope.currentQuery);

        if (option) {
          $scope.currentQuery.pageSize = option.data.take;
          $scope.currentQuery.pageNumber = option.data.page;
        }

        if (option) {
          let result = await TrainingReportService.getTrackerReports(
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
