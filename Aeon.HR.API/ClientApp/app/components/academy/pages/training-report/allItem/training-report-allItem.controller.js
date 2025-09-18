angular
  .module("ssg.module.academy.training-report.all-items.ctrl", [
    "kendo.directives",
  ])
  .controller(
    "TrainingReportAllItemsController",
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
      TrainingReportService
    ) {
      $controller("BaseController", { $scope: $scope });

      $scope.title = $translate.instant("ALL_TRAINING_REPORT");

        $scope.selectedTab = 0;
        $('#mySelect').on('change', function () {
            var selectedValue = $(this).val();
            if (selectedValue === "1") {
                $scope.selectedTab = 1;
                $state.go('home.academy-myReport')

            } else if (selectedValue === "0") {
                $state.go('home.academy-allReport')
            }
        });


      $scope.currentQuery = {
        pageSize: appSetting.pageSizeDefault,
        pageNumber: 1,
        status: [],
        createdBy: "",
        sapCode: "",
        courseName: "",
      };
      $scope.query = {
        status: [],
        createdBy: "",
        sapCode: "",
        courseName: "",
      };
      const cloneStatus = [
        { name: "Pending", code: "Pending" },
        { name: "Completed", code: "Completed" },
        { name: "Draft", code: "Draft" },
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
            fileName: "TrainingReport.xlsx",
            proxyURL: "https://demos.telerik.com/kendo-ui/service/export",
            filterable: true,
            allPages: true,
          },
          dataSource: {
            serverPaging: true,
            pageSize: 20,
            transport: {
              read: async function (e) {
                await getTrainingReportList(e);
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
              width: "170px",
            },
            {
              field: "referenceNumber",
              title: $translate.instant("REFERENCE_ID"),
              width: "170px",
              template: function (data) {
                return `<a ui-sref="home.academy-supplierReport({referenceValue: '${data.itemId}', id: '${data.itemId}'})" ui-sref-opts="{ reload: true }">${data.referenceNumber}</a>`;
              },
            },
            {
              field: "requestorFullName",
              title: $translate.instant("COMMON_REFERENCE_REQUESTOR"),
              width: "170px",
            },
            {
              field: "courseName",
              title: $translate.instant("TRAINING_REQUEST_COURSE_NAME"),
              width: "100px",
            },
            {
              field: "dateSubmit",
              title: $translate.instant("COMMON_CREATED_DATE"),
              template: function (dataItem) {
                return moment(dataItem.dateSubmit).format(
                  appSetting.longDateFormat
                );
              },
              width: "100px",
            },
            {
              field: "typeOfTraining",
              title: $translate.instant("COURSE_TYPE"),
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
          status: [],
          createdBy: "",
          sapCode: "",
          courseName: "",
        };
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
      async function getTrainingReportList(option) {
        $scope.data = [];

        $scope.currentQuery.status = $scope.query.status;
        $scope.currentQuery.sapCode = $scope.query.sapCode;
        $scope.currentQuery.courseName = $scope.query.courseName;
        $scope.currentQuery.createdBy = $scope.query.createdBy;
        if (option) {
          $scope.currentQuery.pageSize = option.data.take;
          $scope.currentQuery.pageNumber = option.data.page;
        }

        if (option) {
          let result = await TrainingReportService.listAllReports(
            $scope.currentQuery
          ).$promise;
          console.log($scope.currentQuery);
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
