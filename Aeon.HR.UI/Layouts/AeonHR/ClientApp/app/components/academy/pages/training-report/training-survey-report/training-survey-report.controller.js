angular
  .module("ssg.academy.training-survey-report.module", ["kendo.directives"])
  .controller(
    "TrainingSurveyReport",
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
      $scope.title = $translate.instant("TRAINING_SURVEY_REPORT");
      $scope.titleEdit = $translate.instant("TRAINING_SURVEY_REPORT");
      $scope.query = {
        sapCode: "",
        fullName: "",
        courseName: "",
        trainerName: "",
        startDateFrom: null,
        startDateTo: null,
        endDateFrom: null,
        endDateTo: null,
      };
      $scope.currentQuery = {
        pageSize: appSetting.pageSizeDefault,
        pageNumber: 1,
        sapCode: "",
        fullName: "",
        courseName: "",
        trainerName: "",
        startingDateFrom: null,
        startDateTo: null,
        endDateFrom: null,
        endDateTo: null,
      };
      function loadPageOne() {
        let grid = $("#trainingSurveyReport").data("kendoGrid");
        if (grid) {
          grid.dataSource.fetch(() => grid.dataSource.page(1));
        }
      }
      this.$onInit = async function () {
        $scope.advancedSearchMode = false;
        $scope.total = 0;
        $scope.data = [];
        $scope.surveyRepotGridOptions = {
          dataSource: {
            serverPaging: true,
            pageSize: 20,
            transport: {
              read: async function (e) {
                await filterSurveyReport(e);
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
            fileName: "Training Survery Report.xlsx",
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
          excelExport: function (e) {
            var sheets = e.workbook.sheets[0];
            sheets = sheets.rows.map((item, i) => {
              item.cells?.forEach((cell) => {
                cell.wrap = true;
                cell.whiteSpace = "normal";
                cell.verticalAlign = "center";
                cell.height = "auto";
                cell.background = "#fff";
                cell.color = "black";
                cell.borderRight = "#ccc";
                cell.borderBottom = "#ccc";
                cell.textAlign = "center";
              });
              item.textAlign = "center";
              if (i == 0) item.height = 20;
              if (i == 1) item.height = 120;

              return item;
            });
            console.log(sheets);
          },
          sortable: true,
          columnMenu: false,
          resizable: false,
          columns: [
            {
              field: "sapCode",
              title: $translate.instant("COMMON_SAP_CODE"),
              width: 80,
              // locked: true,
              textAlign: "center",
            },
            {
              field: "fullName",
              title: $translate.instant("COMMON_FULL_NAME"),
              width: 120,
              // locked: true,
              textAlign: "center",
            },
            {
              field: "departmentName",
              title: $translate.instant("COMMON_DEPARTMENT"),
              width: 120,
              textAlign: "center",
            },
            {
              field: "courseName",
              title: $translate.instant("TRAINING_REQUEST_COURSE_NAME"),
              width: 120,
              textAlign: "center",
            },
            {
              field: "trainerName",
              title: $translate.instant("COMMON_TRAINER_NAME"),
              width: 120,
              textAlign: "center",
            },
            {
              field: "NCC",
              title: $translate.instant("TRAINING_REQUEST_SUPPLIER_NAME"),
              width: 120,
              textAlign: "center",
            },

            {
              field: "A11",
              title: $translate.instant("TRAINING_SURVEY_QUESTION_A11"),
              width: 100,
              maxWidth: "20px",
              textAlign: "center",
            },
            {
              field: "A12",
              title: $translate.instant("TRAINING_SURVEY_QUESTION_A12"),
              width: 100,
              maxWidth: "20px",
              textAlign: "center",
            },
            {
              field: "A13",
              title: $translate.instant("TRAINING_SURVEY_QUESTION_A13"),
              width: 100,
              maxWidth: "100px",
              textAlign: "center",
            },
            {
              field: "A14",
              title: $translate.instant("TRAINING_SURVEY_QUESTION_A14"),
              width: 100,
              maxWidth: "100px",
              textAlign: "center",
            },
            {
              field: "A15",
              title: $translate.instant("TRAINING_SURVEY_QUESTION_A15"),
              width: 100,
              maxWidth: "100px",
              textAlign: "center",
            },
            {
              field: "A16",
              title: $translate.instant("TRAINING_SURVEY_QUESTION_A16"),
              width: 100,
              maxWidth: "100px",
              textAlign: "center",
            },
            {
              field: "A17",
              title: $translate.instant("TRAINING_SURVEY_QUESTION_A17"),
              width: 100,
              maxWidth: "100px",
              textAlign: "center",
            },
            {
              field: "A21",
              title: $translate.instant("TRAINING_SURVEY_QUESTION_A21"),
              width: 4,
              columns: [
                {
                  field: "A211",
                  title: $translate.instant("TRAINING_SURVEY_QUESTION_A211"),
                  width: 100,
                  textAlign: "center",
                },
                {
                  field: "A212",
                  title: $translate.instant("TRAINING_SURVEY_QUESTION_A212"),

                  width: 100,
                  textAlign: "center",
                },
                {
                  field: "A213",
                  title: $translate.instant("TRAINING_SURVEY_QUESTION_A213"),

                  width: 100,
                  textAlign: "center",
                },
                {
                  field: "A214",
                  title: $translate.instant("TRAINING_SURVEY_QUESTION_A214"),

                  width: 100,
                  textAlign: "center",
                },
              ],
            },
            {
              field: "A22",
              title: $translate.instant("TRAINING_SURVEY_QUESTION_A22"),
              width: 4,
              columns: [
                {
                  field: "A222",
                  title: $translate.instant("TRAINING_SURVEY_QUESTION_A222"),

                  width: 100,
                  textAlign: "center",
                },
                {
                  field: "A223",
                  title: $translate.instant("TRAINING_SURVEY_QUESTION_A223"),

                  width: 100,
                  textAlign: "center",
                },
                {
                  field: "A224",
                  title: $translate.instant("TRAINING_SURVEY_QUESTION_A224"),

                  width: 100,
                  textAlign: "center",
                },
                {
                  field: "A225",
                  title: $translate.instant("TRAINING_SURVEY_QUESTION_A225"),
                  width: 100,
                  textAlign: "center",
                },
                {
                  field: "A226",
                  title: $translate.instant("TRAINING_SURVEY_QUESTION_A226"),
                  width: 100,
                  textAlign: "center",
                },
              ],
            },
            {
              field: "A23",
              title: $translate.instant("TRAINING_SURVEY_QUESTION_A23"),
              width: 4,
              columns: [
                {
                  field: "A232",
                  title: $translate.instant("TRAINING_SURVEY_QUESTION_A232"),

                  width: 100,
                  textAlign: "center",
                },
                {
                  field: "A233",
                  title: $translate.instant("TRAINING_SURVEY_QUESTION_A233"),

                  width: 100,
                  textAlign: "center",
                },
                {
                  field: "A234",
                  title: $translate.instant("TRAINING_SURVEY_QUESTION_A234"),

                  width: 100,
                  textAlign: "center",
                },
              ],
            },
            {
              field: "B1",
              title: $translate.instant("TRAINING_SURVEY_QUESTION_B1"),

              width: 100,
              textAlign: "center",
            },
            {
              field: "B11",
              title: $translate.instant("TRAINING_SURVEY_QUESTION_B11"),
              width: 400,
              textAlign: "center",
            },
          ],
        };
      };
      async function filterSurveyReport(option) {
        let dataObj = {
          sapCode: "",
          fullName: "",
          courseName: "",
          trainerName: "",
          departmentName: "",
          NCC: "",
          A11: "",
          A12: "",
          A13: "",
          A14: "",
          A15: "",
          A16: "",
          A17: "",
          A211: "",
          A212: "",
          A213: "",
          A214: "",
          A222: "",
          A223: "",
          A224: "",
          A225: "",
          A226: "",
          A232: "",
          A233: "",
          A234: "",
          A235: "",
          B11: "",
        };
        $scope.data = [];
        $scope.currentQuery = { ...$scope.query };
        if (option) {
          $scope.currentQuery.pageSize = option.data.take;
          $scope.currentQuery.pageNumber = option.data.page;
        }

        if (option) {
          let result = await TrainingReportService.getSurveyReports(
            $scope.currentQuery
          ).$promise;
          if (result) {
            result.data?.map((sur) => {
              let obj = { ...dataObj };
              obj.sapCode = sur.employeeInfo.sapCode;
              obj.fullName = sur.employeeInfo.fullName;
              obj.trainerName = sur.trainerName;
              obj.departmentName = sur.employeeInfo?.departmentName;
              obj.NCC = sur.supplierName || "";
              obj.courseName = sur.courseName;
              if (sur.trainingSurveyQuestions?.length) {
                obj.A11 = getAnswer("A11", sur.trainingSurveyQuestions);
                obj.A12 = getAnswer("A12", sur.trainingSurveyQuestions);
                obj.A13 = getAnswer("A13", sur.trainingSurveyQuestions);
                obj.A14 = getAnswer("A14", sur.trainingSurveyQuestions);
                obj.A15 = getAnswer("A15", sur.trainingSurveyQuestions);
                obj.A16 = getAnswer("A16", sur.trainingSurveyQuestions);
                obj.A17 = getAnswer("A17", sur.trainingSurveyQuestions);
                obj.A211 = getAnswer("A211", sur.trainingSurveyQuestions, true);
                obj.A212 = getAnswer("A212", sur.trainingSurveyQuestions, true);
                obj.A213 = getAnswer("A213", sur.trainingSurveyQuestions, true);
                obj.A214 = getAnswer("A214", sur.trainingSurveyQuestions, true);
                obj.A222 = getAnswer("A222", sur.trainingSurveyQuestions, true);
                obj.A223 = getAnswer("A223", sur.trainingSurveyQuestions, true);
                obj.A224 = getAnswer("A224", sur.trainingSurveyQuestions, true);
                obj.A225 = getAnswer("A225", sur.trainingSurveyQuestions, true);
                obj.A226 = getAnswer("A226", sur.trainingSurveyQuestions, true);
                obj.A232 = getAnswer("A232", sur.trainingSurveyQuestions, true);
                obj.A232 = getAnswer("A232", sur.trainingSurveyQuestions, true);
                obj.A233 = getAnswer("A233", sur.trainingSurveyQuestions, true);
                obj.A234 = getAnswer("A234", sur.trainingSurveyQuestions, true);
                // obj.A235 = getAnswer("A235", sur.trainingSurveyQuestions, true);
                obj.B1 = getAnswer("B1", sur.trainingSurveyQuestions, true);
                obj.B11 = getAnswerForB11(
                  sur.trainingSurveyQuestions,
                  sur.otherReasons
                );
              }
              $scope.data.push(obj);
              console.log($scope.data);
            });
            // $scope.data = result.data;
            $scope.total = result.count;
            option.success($scope.data);
          }
        } else {
          let grid = $("#trainingSurveyReport").data("kendoGrid");
          grid.dataSource.read();
          grid.dataSource.page($scope.currentQuery.pageNumber);
        }
      }
      function getAnswer(question, questionLst, format) {
        if (!questionLst) return;
        let q = questionLst.find((sq) => sq.surveyQuestion == question);
        if (!q) return "";

        if (format) {
          if (q.value == "1") {
            return "Không đồng ý";
          }
          if (q.value == "2") {
            return "Không chắc";
          }
          if (q.value == "3") {
            return "Đồng ý";
          }
        }
        return q.value;
      }
      $scope.surveyReasons = {
        B111: "Nội dung khoá học không như mong đợi",
        B112: "Khoá học không mang tính ứng dụng cao",
        B113: "Phương pháp truyền đạt của giảng viên chưa tốt ",
        B114: "Kiến thức, năng lựcchuyên môn của giảng viên chưa như mong đợi",
        B115: "Học liệu sơ sài; không cung cấp học liệu",
        B116: "Cơ sở vật chất kém",
        B117: "Quy trình tổ chức, ban tổ chức thiếu chuyên nghiệp",
      };
      function getAnswerForB11(questionLst, otherReasons) {
        if (!questionLst) return;
        let result = otherReasons;
        let qs = questionLst.filter((q) => q.parentQuestion == "B.1.1");
        if (qs?.length) {
          qs.map((cq) => {
            if (cq.value == "true" && cq.surveyQuestion != "B118") {
              result = ` ${$scope.surveyReasons[cq.surveyQuestion]},${result}`;
            }
          });
        }
        return result;
      }
      $scope.toggleFilterPanel = function (value) {
        $scope.advancedSearchMode = value;
      };
      $scope.search = function () {
        loadPageOne();
      };
      $scope.clearSearch = function () {
        $scope.query = {
          sapCode: "",
          fullName: "",
          courseName: "",
          trainerName: "",
          startDateFrom: null,
          startDateTo: null,
          endDateFrom: null,
          endDateTo: null,
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
