var ssgApp = angular.module(
  "ssg.module.academy.training-request-management.ctrl",
  ["kendo.directives"]
);
ssgApp.controller(
  "TrainingRequestManagementController",
  function (
    $rootScope,
    $scope,
    $location,
    $controller,
    $translate,
    appSetting,
    localStorageService,
    Notification,
    commonData,
    $stateParams,
    settingService,
    fileService,
    $timeout,
    dashboardService,
    TrainingRequestService
  ) {
    $controller("BaseController", { $scope: $scope });

    var ssg = this;
    $scope.title = "Training Request Management";
    ssg.requiredValueFields = [
      {
        fieldName: "userSAPCode",
        title: "Employee Code",
      },
      {
        fieldName: "userFullName",
        title: "Employee Name",
      },
    ];
    $scope.currentQuery = {
      // predicate: "",
      // predicateParameters: [],
      // order: "Modified DESC",
      // limit: appSetting.pageSizeDefault,
      // page: 1,
      departmentId: "",
      pageNumber: 1,
      pageSize: appSetting.pageSizeDefault,
    };
    $scope.currentParentId = null;
    $scope.tempEditingUser = {};
    $scope.currentUserDepartmentId = -1;
    $scope.currentUserDepartmentGrade = -1;
    $scope.loadColor = false;
    $scope.editingUser = {};
    allDepartments = JSON.parse(sessionStorage.getItemWithSafe("departments"));
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
    $scope.departmentTree = [];
    $scope.departmentTreeByGrade = [];

    $scope.searchText = "";
    $scope.total = 0;
    $scope.totalPending = 0;
    $scope.totalApproved = 0;
    $scope.totalAmountPendingApproval = "0 VND";
    $scope.totalAmountApproved = "0 VND";
    $scope.data = [];
    $scope.regionDataSource = [];
    $scope.currentColor = "#b60081";
    $scope.errors = [];
    $scope.currentDepartment = {};
    $scope.departmentModel = {
      id: "",
      code: "",
      name: "",
      positionCode: "",
      positionName: "",
      parentId: "",
      parentIdCreate: "",
      type: 1,
      jobGradeId: "",
      sapCode: "",
      color: "#b60081",
      isStore: false,
      isHr: false,
      isCB: false,
      isPerfomance: false,
      hrDepartmentId: "",
      hrDepartmentIdCreate: "",
      isFacility: false,
    };
    $scope.userInDepartmentList = [];
    $scope.allUserList = [];
    $scope.departmentName = "AEON";
    $scope.availableEmployees = [];
    $scope.hasAnyUserChange = false;

    $scope.test = "######";

    async function getData(option) {
      $scope.hasAnyUserChange = false;
      if (option) {
        $scope.currentQuery.pageSize = option.data.take;
        $scope.currentQuery.pageNumber = option.data.page;
      }

      if ($scope.selectedItem && $scope.selectedItem.id != "0") {
        $scope.currentQuery.departmentId = $scope.selectedItem.id;
      } else {
        $scope.currentQuery.departmentId = "";
      }

      let departRes = await TrainingRequestService.listByDepartment(
        $scope.currentQuery
      ).$promise;
      if (departRes) {
        if (
          departRes.data &&
          departRes.data.length > 1 &&
          $scope.selectedItem
        ) {
          departRes.data = _.filter(departRes.data, (x) => {
            return x.id != $scope.selectedItem.id;
          });
        }
        if (option && option.data.page > 1) {
          $scope.data = departRes.data.map((item, index) => {
            return {
              ...item,
              no:
                index +
                (option.data.take * option.data.page - option.data.take) +
                1,
            };
          });
        } else {
          $scope.data = departRes.data.map((item, index) => {
            return {
              ...item,
              no: index + 1,
            };
          });
        }
        $scope.total = departRes.count;
        $scope.totalPending = departRes.totalPending;
        $scope.totalApproved = departRes.totalApproved;
        $scope.totalAmountPendingApproval = formatAsCurrency(
          departRes.totalAmountPendingApproval
        );
        $scope.totalAmountApproved = formatAsCurrency(
          departRes.totalAmountApproved
        );
      }
      if (option) {
        option.success($scope.data);
      } else {
        let grid = $("#departmentGrid").data("kendoGrid");
        grid.dataSource.read();
        grid.dataSource.page(1);
      }
    }
    function formatAsCurrency(number) {
      return number.toLocaleString("it-IT", {
        style: "currency",
        currency: "VND",
        minimumFractionDigits: 0,
      });
    }

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

    $scope.resetDepartmentModel = function () {
      $scope.departmentModel = {
        id: "",
        code: "",
        name: "",
        positionCode: "",
        positionName: "",
        caption: "",
        parentId: "",
        parentIdCreate: "",
        type: null,
        jobGradeId: null,
        userSAPCode: "",
        color: "#b60081",
        isStore: false,
        isHr: false,
        isCB: false,
        isPerfomance: false,
        hrDepartmentIdCreate: "",
        hrDepartmentId: "",
      };
    };
    $scope.dialogVisible = false;
    $scope.userInDepartment = function (data) {
      $scope.dialogVisible = true;
    };

    $scope.departmentGridOptions = {
      dataSource: {
        pageSize: appSetting.pageSizeDefault,
        serverPaging: true,
        transport: {
          read: async function (e) {
            await getData(e);
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
      autoBind: true,
      sortable: false,
      pageable: {
        alwaysVisible: true,
        pageSizes: appSetting.pageSizesArray,
        //refresh: true,
      },
      columns: [
        {
          field: "no",
          title: "No.",
          width: "50px",
          editor: function (container, options) {
            $(
              `<label style="padding-top:.4em">${
                options.model[options.field]
              }</label>`
            ).appendTo(container);
          },
        },
        {
          field: "referenceNumber",
          title: "Reference Number",
          width: "170px",
          template: function (data) {
            return `<a ui-sref="home.academy-editRequest({referenceValue: '${data.referenceNumber}', id: '${data.id}'})" ui-sref-opts="{ reload: true }">${data.referenceNumber}</a>`;
          },
        },
        {
          field: "status",
          //title: "Status",
          headerTemplate: $translate.instant("COMMON_STATUS"),
          width: "250px",
          template: function (data) {
            // return `<label>${dataItem.status}</label>`;
            var statusTranslate = $rootScope.getStatusTranslate(data.status);
            return `<workflow-status status="${statusTranslate}"></workflow-status>`;
          },
        },
        {
          field: "requestor",
          title: "Requestor",
          width: "150px",
          template: function (dataItem) {
            return `<label>${dataItem.requesterName}</label>`;
          },
        },
        {
          field: "department",
          title: "Department",
          width: "150px",
          template: function (dataItem) {
            return `<label>${dataItem.departmentName}</label>`;
          },
        },
        {
          field: "type",
          title: "Type",
          width: "150px",
          template: function (dataItem) {
            return `<label>${dataItem.typeOfTraining}</label>`;
          },
        },
        {
          field: "trainingFee",
          title: "Training Fee",
          width: "150px",
          template: function (dataItem) {
            return formatAsCurrency(dataItem.trainingFee);
          },
        },
      ],
      editable: false,
    };

    async function getRegionList(option) {
      let result = await settingService
        .getInstance()
        .departments.getRegionList({
          predicate: "",
          predicateParameters: [],
          order: "RegionName asc",
          limit: 200,
          page: 1,
        }).$promise;
      $scope.regionDataSource = [];
      if (result.isSuccess) {
        $scope.regionDataSource = result.object.data;
      }
      if (option) {
        option.success($scope.regionDataSource);
      }
    }

    $scope.dataDepartment = new kendo.data.HierarchicalDataSource({
      data: [
        {
          id: "0",
          name: "Aeon",
          jobGradeCaption: "",
          isRoot: true,
          expanded: true,
          items: allDepartments,
        },
      ],
    });

    $scope.treeViewDepartment = {
      loadOnDemand: true,
      dataSource: $scope.dataDepartment,
    };

    $scope.temporaryDepartmentAdd;
    $scope.selectedItem = undefined;

    $scope.createAdd = false;
    $scope.isAeon = true;

    $scope.onchangeItemTreeView = async function (dataItem) {
      $scope.selectedItem = dataItem;
      $scope.searchText = "";
      var treeView = $("#treeview").data("kendoTreeView");
      treeView.expand(treeView.findByUid($scope.selectedItem.uid));
      $scope.createAdd = true;
      if ($scope.selectedItem.id != "0") {
        await getData();
        $scope.isAeon = false;
        $scope.departmentName = $scope.selectedItem.name;
      } else {
        $scope.data = _.filter(
          JSON.parse(sessionStorage.getItem("departments")),
          (x) => {
            return x.jobGradeGrade == 9;
          }
        );
        $scope.total = $scope.data.length;
        let grid = $("#departmentGrid").data("kendoGrid");
        grid.dataSource.read();
        if ($scope.searchText) {
          grid.dataSource.page(1);
        }
        $scope.isAeon = true;
      }
    };

    async function ngOnit() {
      await getJobGradeList();
      await getRegionList();
      var treeview = $("#treeview").data("kendoTreeView");
      if (treeview) {
        treeview.expand(".k-item");
      }
      var firstNode = $("#treeview").find(".k-first");
      treeview.select(firstNode);
      setDataDepartmentSearch(allDepartments);
    }
    ngOnit();
    $scope.arraySearchDepartment = [];
    function findDepartment(departmentId, list) {
      var result;
      for (var i = 0; i < list.length; i++) {
        $scope.arraySearchDepartment.push(list[i].id);
        if (list[i].id == departmentId) {
          result = list[i];
          return result;
        } else {
          if (list[i].items.length > 0) {
            result = findDepartment(departmentId, list[i].items);
            if (result) {
              return result;
            }
          }
        }
      }
      return result;
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
      var dropdowntree = $("#departmentId").data("kendoDropDownTree");
      if (dropdowntree) {
        dropdowntree.setDataSource(dataSource);
      }
    }

    $scope.path = [];
    function findPathNode(data, departments) {
      var node = "";
      for (var i = 0; i < departments.length; i++) {
        if (departments[i].id == data.parentId) {
          node = departments[i].id;
          $scope.path.push(node);
          if (departments[i].parentId) {
            node = findPathNode(
              departments[i],
              JSON.parse(sessionStorage.getItemWithSafe("departments"))
            );
          } else {
            $scope.path.push("0");
          }
        } else {
          if (
            departments[i].items.length > 0 &&
            data.parentId != allDepartments[0].id
          ) {
            node = findPathNode(data, departments[i].items);
            if (node) {
              $scope.path.push(node);
            }
          }
        }
      }
      return node;
    }
    $scope.departmentTreeId = "";
    $scope.departmentOptions = {
      placeholder: "",
      dataTextField: "name",
      dataValueField: "id",
      template: showCustomDepartmentTitle,
      valuePrimitive: true,
      checkboxes: false,
      autoBind: true,
      filter: "contains",
      filtering: async function (option) {
        await getDepartmentByFilter(option);
      },
      loadOnDemand: true,
      // valueTemplate: (e) => showCustomField(e, ['name']),
      change: function (e) {
        if (!e.sender.value()) {
          clearSearchTextOnDropdownTree("departmentId");
          setDataDepartmentSearch(allDepartments);
        } else {
          $scope.dataDepartment = new kendo.data.HierarchicalDataSource({
            data: [
              {
                id: "0",
                name: "Aeon",
                jobGradeCaption: "",
                isRoot: true,
                expanded: true,
                items: JSON.parse(
                  sessionStorage.getItemWithSafe("departments")
                ),
              },
            ],
          });
          var treeview = $("#treeview").data("kendoTreeView");
          treeview.setDataSource($scope.dataDepartment);
          var result = findDepartment(
            $scope.departmentTreeId,
            JSON.parse(sessionStorage.getItemWithSafe("departments"))
          );
          $scope.path.push($scope.departmentTreeId);
          findPathNode(
            result,
            JSON.parse(sessionStorage.getItemWithSafe("departments"))
          );
          treeview.expandPath($scope.path.reverse());
          //trỏ vào node đó
          var nodeTree = findNodeInTree(result, treeview.dataSource._data);
          treeview.select(treeview.findByUid(nodeTree.uid));
          //$timeout(function() {
          // var test = $(`span[id=${nodeTree.uid}]`)[0].offsetTop;
          // var test = $(`span[id=${nodeTree.uid}]`)[0].offsetTop;
          // $("div.col-md-4").scrollTop(test);
          //}, 300);
          $scope.path = [];
        }
      },
    };

    function findNodeInTree(data, departments) {
      var node = "";
      for (var i = 0; i < departments.length; i++) {
        if (departments[i].id == data.id) {
          node = departments[i];
          break;
        } else {
          if (departments[i].items.length > 0) {
            node = findNodeInTree(data, departments[i].items);
            if (node) {
              break;
            }
          }
        }
      }
      return node;
    }

    async function getDepartmentByFilter(option) {
      if (!option.filter) {
        option.preventDefault();
      } else {
        let filter =
          option.filter && option.filter.value ? option.filter.value : "";
        if (filter) {
          arg = {
            predicate:
              "(name.contains(@0) or code.contains(@1)) or UserDepartmentMappings.Any(User.FullName.contains(@2))",
            predicateParameters: [filter.trim(), filter.trim(), filter.trim()],
            page: 1,
            limit: appSetting.pageSizeDefault,
            order: "",
          };
          res = await settingService
            .getInstance()
            .departments.getDepartmentByFilter(arg).$promise;
          if (res.isSuccess) {
            setDataDepartmentSearch(res.object.data);
          }
        } else {
          setDataDepartmentSearch(
            JSON.parse(sessionStorage.getItemWithSafe("departments"))
          );
        }
      }
    }
  }
);
