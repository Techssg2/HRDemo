angular
  .module("ssg.academy.training-budget-balance-report.module", [
    "kendo.directives",
  ])
  .controller(
    "TrainingBudgetBalanceReport",
    function (
      $scope,
      $rootScope,
      $window,
      $translate,
      $controller,
      appSetting,
      Notification,
      $state,
      settingService,
      $stateParams,
      TrainingReportService
    ) {
      $controller("BaseController", { $scope: $scope });
      $scope.title = $translate.instant("TRAINING_BUDGET_BALANCE_REPORT");
      $scope.titleEdit = $translate.instant("TRAINING_BUDGET_BALANCE_REPORT");
      $scope.query = {
        pageNumber: 1,
        pageSize: 20,
        courseName: "",
        supplierName: "",
        requestedDateFrom: null,
        requestedDateTo: null,
        requestForDeptId: null,
        requestedDeptId: null,
      };
      $scope.currentQuery = {
        pageNumber: 1,
        pageSize: appSetting.pageSizeDefault,
        courseName: "",
        supplierName: "",
        requestedDateFrom: null,
        requestedDateTo: null,
        requestForDeptId: null,
        requestedDeptId: null,
      };
      $scope.departments = [];
      $scope.no = 1;
      $scope.advancedSearchMode = false;
      $scope.total = 0;
      $scope.data = [];
      allDepartments = JSON.parse(
        sessionStorage.getItemWithSafe("departments")
      );
      $scope.jobGrades = [];
      $scope.mapperSpanColumns = [];
      $scope.model = {
        typeList: [
          {
            id: 2,
            title: "Department",
          },
          {
            id: 1,
            title: "Division",
          },
        ],
        jobGradeList: [],
      };

      function loadPageOne() {
        let grid = $("#trainingBudgetBalanceReport").data("kendoGrid");
        if (grid) {
          grid.dataSource.fetch(() => grid.dataSource.page(1));
        }
      }

      function populateData() {
        $scope.trainingBudgetBalanceReportGridOptions = {
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
            fileName: "Training Budget Balance Report.xlsx",
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
            var datas = e.data;
            let maxNo = getMaxNo(datas);
            if (maxNo > 0) {
              for (let no = 1; no <= maxNo; no++) {
                let allRowsData = datas.filter((row) => row.no == no);
                let noRowData = allRowsData?.length;
                if (noRowData > 1) {
                  let isFirstRow = true;
                  sheets.rows = sheets.rows.map((row, i) => {
                    if (i == 0 || i == 1) return row;
                    if (row.cells[0]?.value == no && isFirstRow) {
                      row.cells[0].rowSpan = noRowData;
                      row.cells[1].rowSpan = noRowData;
                      row.cells[3].rowSpan = noRowData;
                      row.cells[10].rowSpan = noRowData;
                      row.cells[11].rowSpan = noRowData;
                      row.cells[12].rowSpan = noRowData;
                      row.cells[14].rowSpan = noRowData;
                      row.cells[15].rowSpan = noRowData;
                      isFirstRow = false;
                      return row;
                    }
                    if (row.cells[0].value == no && !isFirstRow) {
                      var removeValFrom = [0, 1, 3, 10, 11, 12, 14, 15];
                      row.cells = row.cells.filter(function (value, index) {
                        return removeValFrom.indexOf(index) == -1;
                      });

                      return row;
                    }
                    return row;
                  });
                  console.log(sheets);
                }
              }
            }
          },
          sortable: true,
          columnMenu: false,
          resizable: false,
          columns: [
            {
              field: "no",
              title: $translate.instant("COMMON_NO"),
              width: "40px",
            },
            {
              field: "requestFor",
              title: $translate.instant("REQUEST_FOR"),
              width: 250,
              // locked: true,
              //textAlign: "center",
            },
            {
              field: "departmentName",
              title: $translate.instant("REQUESTED_DEPARTMENT"),
              width: 250,
              // locked: true,
              //textAlign: "center",
            },
            {
              field: "courseName",
              title: $translate.instant("COURSE_NAME"),
              width: 100,
              textAlign: "center",
            },
            {
              field: "participants",
              title: $translate.instant("PARTICIPANTS"),
              width: 4,
              columns: [
                {
                  field: "g1",
                  title: "G1",
                  width: 100,
                  textAlign: "center",
                },
                {
                  field: "g2",
                  title: "G2",
                  width: 100,
                  textAlign: "center",
                },
                {
                  field: "g3",
                  title: "G3",
                  width: 100,
                  textAlign: "center",
                },
                {
                  field: "g4",
                  title: "G4",
                  width: 100,
                  textAlign: "center",
                },
                {
                  field: "g5",
                  title: "G5",
                  width: 100,
                  textAlign: "center",
                },
                {
                  field: "g6",
                  title: "G6",
                  width: 100,
                  textAlign: "center",
                },
              ],
            },
            {
              field: "budgetGroup",
              title: $translate.instant("BUDGET_GROUP"),
              width: 100,
              textAlign: "center",
            },
            {
              field: "plannedBudget",
              title: $translate.instant("PLANNED_BUDGET"),
              width: 200,
              textAlign: "center",
            },
            {
              field: "actualUsedBudget",
              title: $translate.instant("ACTUAL_USED_BUDGET"),
              width: 200,
              textAlign: "center",
            },
            {
              field: "actualUsedBudgetByDepartment",
              title: $translate.instant("ACTUAL_USED_BUDGET_BY_DEPARTMENT"),
              width: 250,
              textAlign: "center",
            },
            {
              field: "supplierName",
              title: $translate.instant("SUPPLIER_NAME"),
              width: 250,
              textAlign: "center",
            },
            {
              field: "requestedDate",
              title: $translate.instant("REQUESTED_DATE"),
              width: 250,
              textAlign: "center",
              template: function (dataItem) {
                return moment(dataItem.requestedDate).format(
                  appSetting.longDateFormat
                );
              },
            },
          ],
          dataBound: function (e) {
            let colMerge = [
              {
                fieldText: $translate.instant("REQUESTED_DATE"),
                fieldId: "requestedDate",
              },
              {
                fieldText: $translate.instant("SUPPLIER_NAME"),
                fieldId: "supplierName",
              },
              {
                fieldText: $translate.instant("ACTUAL_USED_BUDGET"),
                fieldId: "actualUsedBudget",
              },
              {
                fieldText: $translate.instant("PLANNED_BUDGET"),
                fieldId: "plannedBudget",
              },
              {
                fieldText: $translate.instant("BUDGET_GROUP"),
                fieldId: "budgetGroup",
              },
              {
                fieldText: $translate.instant("REQUEST_FOR"),
                fieldId: "requestFor",
              },
              {
                fieldText: $translate.instant("COMMON_NO"),
                fieldId: "no",
              },
            ];
            let col = [];
            // colMerge.forEach((element) => {
            //   col = MergeGridRows("trainingBudgetBalanceReport", element);
            // });

            MergeCommonRows(
              $("#trainingBudgetBalanceReport>.k-grid-content>table"),
              $(
                "#trainingBudgetBalanceReport>.k-grid-header>.k-grid-header-wrap>table"
              )
            );
          },
        };
      }
      function getMaxNo(datas) {
        let maxNo = 0;
        if (datas && datas.length) {
          datas.map((item) => {
            if (item.no > maxNo) {
              maxNo = item.no;
            }
          });
        }
        return maxNo;
      }
      this.$onInit = async function () {
        await getJobGradeList();
        populateData();
        setDataDepartmentSearch(allDepartments);
      };

      async function getJobGradeList() {
        let result = await settingService.getInstance().headcount.getJobGrades({
          predicate: "",
          predicateParameters: [],
          order: "Grade asc",
          limit: 200,
          page: 1,
        }).$promise;
        if (result.isSuccess) {
          $scope.model.jobGradeList = result.object.data;
          let dataSource = new kendo.data.DataSource({
            data: $scope.model.jobGradeList,
          });
        } else {
          Notification.error(result.messages[0]);
        }
      }

      $scope.departmentOptions = {
        dataTextField: "name",
        dataValueField: "id",
        //template: showCustomDepartmentTitle,
        template: function (dataItem) {
          return `<span class="${
            dataItem.item.enable == false ? "k-state-disabled" : ""
          }">${showCustomDepartmentTitle(dataItem)}</span>`;
        },
        checkboxes: false,
        autoBind: true,
        valuePrimitive: true,
        filter: "contains",
        filtering: async function (e) {
          await getDepartmentByFilter(e, "#departmentId");
        },
        loadOnDemand: true,
        // valueTemplate: (e) => showCustomField(e, ['name', 'userCheckedHeadCount'], 'jobGradeCaption')
        valueTemplate: (e) => showCustomField(e, ["name"]),
        select: function (e) {
          let dropdownlist = $("#departmentId").data("kendoDropDownTree");
          let dataItem = dropdownlist.dataItem(e.node);
          if (dataItem && dataItem.id) {
            $scope.query.requestForDeptId = dataItem.id;
            clearSearchTextOnDropdownTree("departmentId");
          }
        },
      };

      $scope.department2Options = {
        dataTextField: "name",
        dataValueField: "id",
        //template: showCustomDepartmentTitle,
        template: function (dataItem) {
          return `<span class="${
            dataItem.item.enable == false ? "k-state-disabled" : ""
          }">${showCustomDepartmentTitle(dataItem)}</span>`;
        },
        checkboxes: false,
        autoBind: true,
        valuePrimitive: true,
        filter: "contains",
        filtering: async function (e) {
          await getDepartmentByFilter(e, "#department2Id");
        },
        loadOnDemand: true,
        // valueTemplate: (e) => showCustomField(e, ['name', 'userCheckedHeadCount'], 'jobGradeCaption')
        valueTemplate: (e) => showCustomField(e, ["name"]),
        select: function (e) {
          let dropdownlist = $("#department2Id").data("kendoDropDownTree");
          let dataItem = dropdownlist.dataItem(e.node);
          if (dataItem && dataItem.id) {
            $scope.query.requestedDeptId = dataItem.id;
            clearSearchTextOnDropdownTree("department2Id");
          }
        },
      };

      async function getDepartmentByFilter(option, dropdownId) {
        let arg = {};
        if (option) {
          if (!option.filter) {
            option.preventDefault();
          } else {
            let filter =
              option.filter && option.filter.value ? option.filter.value : "";
            if (filter) {
              arg = {
                predicate:
                  "name.contains(@0) or code.contains(@1) or UserDepartmentMappings.Any(User.FullName.contains(@2))",
                predicateParameters: [filter, filter, filter],
                page: 1,
                limit: appSetting.pageSizeDefault,
                order: "",
              };
              res = await settingService
                .getInstance()
                .departments.getDepartmentByFilter(arg).$promise;
              if (res.isSuccess) {
                setDataWithDepartmentNameSearch(res.object.data, dropdownId);
              }
            } else {
              setDataWithDepartmentNameSearch(
                JSON.parse(
                  sessionStorage.getItemWithSafe("departments"),
                  dropdownId
                )
              );
            }
          }
        }
      }

      function setDataDepartmentSearch(dataDepartment) {
        let dataSource = new kendo.data.HierarchicalDataSource({
          data: dataDepartment,
          schema: {
            model: {
              children: "items",
            },
          },
        });
        let dataSource2 = new kendo.data.HierarchicalDataSource({
          data: dataDepartment,
          schema: {
            model: {
              children: "items",
            },
          },
        });
        var dropdowntree = $("#departmentId").data("kendoDropDownTree");
        if (dropdowntree) {
          dropdowntree.setDataSource(dataSource);
        }
        var dropdowntree2 = $("#department2Id").data("kendoDropDownTree");
        if (dropdowntree2) {
          dropdowntree2.setDataSource(dataSource2);
        }
      }

      function setDataWithDepartmentNameSearch(
        dataDepartment,
        departmentNameId
      ) {
        let dataSource = new kendo.data.HierarchicalDataSource({
          data: dataDepartment,
          schema: {
            model: {
              children: "items",
            },
          },
        });
        var dropdowntree = $(departmentNameId).data("kendoDropDownTree");
        if (dropdowntree) {
          dropdowntree.setDataSource(dataSource);
        }
      }

      function formatTo_MMDDYYYY(day) {
        var initial = day.split(/\//);
        return [initial[1], initial[0], initial[2]].join("/");
      }
      function formatAsCurrency(number) {
        return number.toLocaleString("it-IT", {
          style: "currency",
          currency: "VND",
          minimumFractionDigits: 0,
        });
      }
      async function loadData(option) {
        let dataObj = {
          sapCode: "",
          fullName: "",
          courseName: "",
          trainerName: "",
          departmentName: "",
        };
        $scope.data = [];
        $scope.currentQuery = { ...$scope.query };
        if (option) {
          $scope.currentQuery.pageSize = option.data.take;
          $scope.currentQuery.pageNumber = option.data.page;
        }
        if ($scope.currentQuery.requestedDateFrom) {
          $scope.currentQuery.requestedDateFrom = formatTo_MMDDYYYY(
            $scope.currentQuery.requestedDateFrom
          );
        }
        if ($scope.currentQuery.requestedDateTo) {
          $scope.currentQuery.requestedDateTo = formatTo_MMDDYYYY(
            $scope.currentQuery.requestedDateTo
          );
        }

        if (option) {
          let result =
            await TrainingReportService.getTrainingBudgetBalanceReport(
              $scope.currentQuery
            ).$promise;
          if (result) {
            let rebuildObj = [];
            result.data?.map((item, index) => {
              let objRoot = {
                ...item,
                no: index + 1,
                rowSpan: item.requestedDepartments?.length,
              };
              item.requestedDepartments?.map((subItem, y) => {
                let objChild = {
                  ...objRoot,
                };
                objChild.departmentName = subItem.departmentName;
                objChild.departmentId = subItem.departmentId;
                objChild.actualUsedBudgetByDepartment =
                  subItem.actualUsedBudgetByDepartment;
                subItem.participants?.map((subparticipantsItem, z) => {
                  if (subparticipantsItem.jobgrade === "G1") {
                    objChild.g1 = subparticipantsItem.noParticipant;
                  }
                  if (subparticipantsItem.jobgrade === "G2") {
                    objChild.g2 = subparticipantsItem.noParticipant;
                  }
                  if (subparticipantsItem.jobgrade === "G3") {
                    objChild.g3 = subparticipantsItem.noParticipant;
                  }
                  if (subparticipantsItem.jobgrade === "G4") {
                    objChild.g4 = subparticipantsItem.noParticipant;
                  }
                  if (subparticipantsItem.jobgrade === "G5") {
                    objChild.g5 = subparticipantsItem.noParticipant;
                  }
                  if (subparticipantsItem.jobgrade === "G6") {
                    objChild.g6 = subparticipantsItem.noParticipant;
                  }
                });
                rebuildObj.push(objChild);
              });
            });
            $scope.data = rebuildObj;
            //mockdata
            //$scope.data = $scope.testData.data;
            $scope.data.forEach((item) => {
              item.actualUsedBudgetByDepartment = formatAsCurrency(
                item.actualUsedBudgetByDepartment
              );
              item.actualUsedBudget = formatAsCurrency(item.actualUsedBudget);
              item.plannedBudget = formatAsCurrency(item.plannedBudget);
            });

            $scope.total = result.count;
            option.success($scope.data);
          }
        } else {
          let grid = $("#trainingBudgetBalanceReport").data("kendoGrid");
          grid.dataSource.read();
          grid.dataSource.page($scope.currentQuery.pageNumber);
        }
      }

      $scope.toggleFilterPanel = function (value) {
        $scope.advancedSearchMode = value;
      };
      $scope.search = function () {
        loadPageOne();
      };
      $scope.clearSearch = function () {
        $scope.query = {
          PageNumber: 1,
          PageSize: 20,
          courseName: "",
          supplierName: "",
          requestedDateFrom: null,
          requestedDateTo: null,
          requestForDeptId: null,
          requestedDeptId: null,
        };
        setDataDepartmentSearch(allDepartments);
        loadPageOne();
      };
      $rootScope.$on("isEnterKeydown", function (result, data) {
        if ($scope.advancedSearchMode && data.state == $state.current.name) {
          $scope.search();
        }
      });

      function MergeGridRows(gridId, colTitle) {
        let arr = [];
        $("#" + gridId + ">.k-grid-content>table").each(function (index, item) {
          var dimension_col = 1;
          // First, scan first row of headers for the "Dimensions" column.
          $("#" + gridId + ">.k-grid-header>.k-grid-header-wrap>table")
            .find("th")
            .each(function () {
              var _this = $(this);
              if (_this.text() == colTitle.fieldText) {
                if (
                  colTitle.fieldId == "budgetGroup" ||
                  colTitle.fieldId == "supplierName" ||
                  colTitle.fieldId == "requestedDate" ||
                  colTitle.fieldId == "plannedBudget" ||
                  colTitle.fieldId == "actualUsedBudget"
                ) {
                  dimension_col = dimension_col + 5;
                }
                // first_instance holds the first instance of identical td
                var first_instance = null;
                var cellText = "";
                var arrCells = [];
                let className = "";
                $(item)
                  .find("tr")
                  .each(function (i, tr) {
                    // console.log(tr);
                    var dimension_td = $(this).find(
                      "td:nth-child(" + dimension_col + ")"
                    );

                    if (first_instance == null) {
                      first_instance = dimension_td;
                      cellText = first_instance.text();
                      let obj = $scope.data.find((o) => {
                        if (colTitle.fieldId == "requestedDate") {
                          return (
                            moment(o[colTitle.fieldId]).format(
                              appSetting.longDateFormat
                            ) == cellText
                          );
                        }
                        return o[colTitle.fieldId] + "" === cellText;
                      });
                      if (obj) className = "rowSpan-" + obj.rowSpan;
                    } else if (dimension_td.text() == cellText) {
                      // if current td is identical to the previous
                      dimension_td.css("border-top", "0px");
                    } else {
                      // this cell is different from the last
                      arr.push(arrCells.length);
                      arrCells = ChangeMergedCells(arrCells, cellText, true);
                      first_instance = dimension_td;
                      cellText = dimension_td.text();
                    }
                    arrCells.push(dimension_td);
                    dimension_td.text("");
                    dimension_td
                      .css("background-color", "white")
                      .css("color", "black")
                      .css("border-bottom-color", "transparent")
                      .addClass(className);
                  });
                arrCells = ChangeMergedCells(arrCells, cellText, true);
                return;
              }
              dimension_col++;
            });
        });
        console.log(arr);
        return arr;
      }

      function ChangeMergedCells(arrCells, cellText, addBorderToCell) {
        var cellsCount = arrCells.length;
        if (cellsCount == 0) {
          return [];
        }
        if (addBorderToCell) {
          arrCells[cellsCount - 1].css("border-bottom", "solid 1px #ddd");
        }

        let cell = arrCells[0];
        cell.text(cellText);
        arrCells[0].css("border-bottom", "solid 1px #ddd");
        arrCells.forEach((item, index) => {
          if (index == 0) {
            item.attr("rowspan", cellsCount);
          } else {
            item.remove();
          }
        });
        arrCells = [];
        return arrCells;
      }

      function MergeCommonRows(table, header) {
        var firstColumnBrakes = [];
        let col = [1, 2, 4, 11, 12, 13, 15, 16]; //this is selected col need to merge start with 1
        let allCol = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16]; //this is number of col in one row
        // iterate through the columns instead of passing each column as function parameter:
        for (var i = 1; i <= 16; i++) {
          let isValidCol = col.filter((item) => item == i);
          if (isValidCol.length > 0) {
            var previous = null,
              cellToExtend = null,
              rowspan = 1;
            table.find("td:nth-child(" + i + ")").each(function (index, e) {
              var jthis = $(this),
                content = jthis.text();
              // check if current row "break" exist in the array. If not, then extend rowspan:
              if (
                previous == content &&
                content !== "" &&
                $.inArray(index, firstColumnBrakes) === -1
              ) {
                // hide the row instead of remove(), so the DOM index won't "move" inside loop.
                jthis.addClass("hidden");
                cellToExtend.attr("rowspan", (rowspan = rowspan + 1));
              } else {
                // store row breaks only for the first column:
                if (i === 1) firstColumnBrakes.push(index);
                rowspan = 1;
                previous = content;
                cellToExtend = jthis;
              }
            });
          }
        }
        // now remove hidden td's (or leave them hidden if you wish):
        $("td.hidden").remove();
      }

      $scope.testData = {
        count: 12,
        totalpages: 1,
        data: [
          {
            no: 1,
            requestFor: "STRATEGIC HRM, CORPERATE COM & EX AFR (G6)",
            requestForDeptId: "2127fd55-42cd-4fbc-a15a-da9a51691402",
            departmentName: "CHIEF MERCHANDISNG OFFICER (G8)",
            departmentId: "f87d2994-6b37-4786-9bda-cb8d8afe2c39",
            courseName: "Dev",
            g1: 2,
            g2: 3,
            g3: 4,
            g4: 5,
            g5: 6,
            g6: 7,
            actualUsedBudgetByDepartment: 60000.0,

            budgetGroup: "Doumatic Training",
            plannedBudget: 1000000,
            actualUsedBudget: 100000000,
            supplierName: "DXC",
            requestedDate: "2022-01-05T09:44:42.06+07:00",
          },
          {
            no: 1,
            requestFor: "STRATEGIC HRM, CORPERATE COM & EX AFR (G6)",
            requestForDeptId: "2127fd55-42cd-4fbc-a15a-da9a51691402",
            departmentName: "CONSTRUCTION (G6)",
            departmentId: "a937288c-683b-4a9d-a682-2366d41d0509",
            courseName: "Dev",
            g1: 7,
            g2: 6,
            g3: 5,
            g4: 4,
            g5: 3,
            g6: 2,
            actualUsedBudgetByDepartment: 60009.0,

            budgetGroup: "Doumatic Training",
            plannedBudget: 1000000,
            actualUsedBudget: 100000000,
            supplierName: "DXC",
            requestedDate: "2022-01-05T09:44:42.06+07:00",
          },
          {
            no: 1,
            requestFor: "STRATEGIC HRM, CORPERATE COM & EX AFR (G6)",
            requestForDeptId: "2127fd55-42cd-4fbc-a15a-da9a51691402",
            departmentName: "CONSTRUCTION (G6)",
            departmentId: "a937288c-683b-4a9d-a682-2366d41d0509",
            courseName: "Dev",
            g1: 2,
            g2: 3,
            g3: 4,
            g4: 5,
            g5: 6,
            g6: 7,
            actualUsedBudgetByDepartment: 60009.0,

            budgetGroup: "Doumatic Training",
            plannedBudget: 1000000,
            actualUsedBudget: 100000000,
            supplierName: "DXC",
            requestedDate: "2022-01-05T09:44:42.06+07:00",
          },
          {
            no: 1,
            requestFor: "STRATEGIC HRM, CORPERATE COM & EX AFR (G6)",
            requestForDeptId: "2127fd55-42cd-4fbc-a15a-da9a51691402",
            departmentName: "CONSTRUCTION (G6)",
            departmentId: "a937288c-683b-4a9d-a682-2366d41d0509",
            courseName: "Dev",
            g1: 7,
            g2: 6,
            g3: 5,
            g4: 4,
            g5: 3,
            g6: 2,
            actualUsedBudgetByDepartment: 60009.0,

            budgetGroup: "Doumatic Training",
            plannedBudget: 1000000,
            actualUsedBudget: 100000000,
            supplierName: "DXC",
            requestedDate: "2022-01-05T09:44:42.06+07:00",
          },
          {
            no: 2,
            requestFor: "CONSTRUCTION (G6)",
            requestForDeptId: "2127fd55-42cd-4fbc-a15a-da9a51691402",
            departmentName: "CONSTRUCTION (G6)",
            departmentId: "a937288c-683b-4a9d-a682-2366d41d0509",
            courseName: "Tester",
            g1: 7,
            g2: 6,
            g3: 5,
            g4: 4,
            g5: 3,
            g6: 2,
            actualUsedBudgetByDepartment: 60009.0,

            budgetGroup: "Doumatic Training",
            plannedBudget: 2000000,
            actualUsedBudget: 200000000,
            supplierName: "DXC",
            requestedDate: "2022-01-05T09:44:42.06+07:00",
          },
          {
            no: 2,
            requestFor: "CONSTRUCTION (G6)",
            requestForDeptId: "2127fd55-42cd-4fbc-a15a-da9a51691402",
            departmentName: "CONSTRUCTION (G6)",
            departmentId: "a937288c-683b-4a9d-a682-2366d41d0509",
            courseName: "Tester",
            g1: 7,
            g2: 6,
            g3: 5,
            g4: 4,
            g5: 3,
            g6: 2,
            actualUsedBudgetByDepartment: 60009.0,

            budgetGroup: "Doumatic Training",
            plannedBudget: 2000000,
            actualUsedBudget: 200000000,
            supplierName: "DXC",
            requestedDate: "2022-01-05T09:44:42.06+07:00",
          },
          {
            no: 2,
            requestFor: "CONSTRUCTION (G6)",
            requestForDeptId: "2127fd55-42cd-4fbc-a15a-da9a51691402",
            departmentName: "CONSTRUCTION (G6)",
            departmentId: "a937288c-683b-4a9d-a682-2366d41d0509",
            courseName: "Tester",
            g1: 7,
            g2: 6,
            g3: 5,
            g4: 4,
            g5: 3,
            g6: 2,
            actualUsedBudgetByDepartment: 60009.0,

            budgetGroup: "Doumatic Training",
            plannedBudget: 2000000,
            actualUsedBudget: 200000000,
            supplierName: "DXC",
            requestedDate: "2022-01-05T09:44:42.06+07:00",
          },
          {
            no: 2,
            requestFor: "CONSTRUCTION (G6)",
            requestForDeptId: "2127fd55-42cd-4fbc-a15a-da9a51691402",
            departmentName: "CONSTRUCTION (G6)",
            departmentId: "a937288c-683b-4a9d-a682-2366d41d0509",
            courseName: "Tester",
            g1: 7,
            g2: 6,
            g3: 5,
            g4: 4,
            g5: 3,
            g6: 2,
            actualUsedBudgetByDepartment: 60009.0,

            budgetGroup: "Doumatic Training",
            plannedBudget: 2000000,
            actualUsedBudget: 200000000,
            supplierName: "DXC",
            requestedDate: "2022-01-05T09:44:42.06+07:00",
          },
          {
            no: 2,
            requestFor: "CONSTRUCTION (G6)",
            requestForDeptId: "2127fd55-42cd-4fbc-a15a-da9a51691402",
            departmentName: "CONSTRUCTION (G6)",
            departmentId: "a937288c-683b-4a9d-a682-2366d41d0509",
            courseName: "Tester",
            g1: 7,
            g2: 6,
            g3: 5,
            g4: 4,
            g5: 3,
            g6: 2,
            actualUsedBudgetByDepartment: 60009.0,

            budgetGroup: "Doumatic Training",
            plannedBudget: 2000000,
            actualUsedBudget: 200000000,
            supplierName: "DXC",
            requestedDate: "2022-01-05T09:44:42.06+07:00",
          },
          {
            no: 3,
            requestFor: "AOEN VIETNAM",
            requestForDeptId: "2127fd55-42cd-4fbc-a15a-da9a51691402",
            departmentName: "CONSTRUCTION (G6)",
            departmentId: "a937288c-683b-4a9d-a682-2366d41d0509",
            courseName: "Sales",
            g1: 7,
            g2: 6,
            g3: 5,
            g4: 4,
            g5: 3,
            g6: 2,
            actualUsedBudgetByDepartment: 60009.0,

            budgetGroup: "Doumatic Training 3",
            plannedBudget: 3000000,
            actualUsedBudget: 300000000,
            supplierName: "CSC",
            requestedDate: "2022-01-05T09:44:42.06+07:00",
          },
          {
            no: 4,
            requestFor: "AOEN",
            requestForDeptId: "2127fd55-42cd-4fbc-a15a-da9a51691402",
            departmentName: "CONSTRUCTION (G6)",
            departmentId: "a937288c-683b-4a9d-a682-2366d41d0509",
            courseName: "Sercurity",
            g1: 7,
            g2: 6,
            g3: 5,
            g4: 4,
            g5: 3,
            g6: 2,
            actualUsedBudgetByDepartment: 60009.0,

            budgetGroup: "Doumatic Training 4",
            plannedBudget: 4000000,
            actualUsedBudget: 400000000,
            supplierName: "DXC",
            requestedDate: "2022-01-05T09:44:42.06+07:00",
          },
          {
            no: 4,
            requestFor: "AOEN",
            requestForDeptId: "2127fd55-42cd-4fbc-a15a-da9a51691402",
            departmentName: "CONSTRUCTION (G6)",
            departmentId: "a937288c-683b-4a9d-a682-2366d41d0509",
            courseName: "Sercurity",
            g1: 7,
            g2: 6,
            g3: 5,
            g4: 4,
            g5: 3,
            g6: 2,
            actualUsedBudgetByDepartment: 60009.0,

            budgetGroup: "Doumatic Training 4",
            plannedBudget: 4000000,
            actualUsedBudget: 400000000,
            supplierName: "DXC",
            requestedDate: "",
          },
          {
            no: 5,
            requestFor: "AOEN",
            requestForDeptId: "2127fd55-42cd-4fbc-a15a-da9a51691402",
            departmentName: "CONSTRUCTION (G6)",
            departmentId: "a937288c-683b-4a9d-a682-2366d41d0509",
            courseName: "Sercurity5",
            g1: 7,
            g2: 6,
            g3: 5,
            g4: 4,
            g5: 3,
            g6: 2,
            actualUsedBudgetByDepartment: 60009.0,

            budgetGroup: "Doumatic Training 45",
            plannedBudget: 4000000,
            actualUsedBudget: 400000000,
            supplierName: "DXC5",
            requestedDate: "5",
          },
        ],
      };
    }
  );
