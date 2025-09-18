angular
  .module("ssg.module.academy.training-reson.ctrl", ["kendo.directives"])
  .controller(
    "TrainingReasonController",
    function (
      $rootScope,
      $scope,
      $stateParams,
      commonData,
      $state,
      $window,
      $translate,
      $controller,
      appSetting,
      Notification,
      TrainingReasonService
    ) {
      $controller("BaseController", { $scope: $scope });
      $scope.title = "Academy - Reason of Training Request";

      $scope.keyword = "";
      $scope.trainingReasons = [];
      $scope.total = 0;
      $scope.trainingReasonOptions = {
        dataSource: {
          serverPaging: true,
          pageSize: 20,
          transport: {
            read: async function (e) {
              await getReasons(e);
            },
          },
          schema: {
            total: () => {
              return $scope.total;
            },
            data: () => {
              return $scope.trainingReasons;
            },
          },
        },
        sortable: false,
        pageable: true,
        editable: {
          mode: "inline",
          confirmation: false,
        },
        pageable: {
          alwaysVisible: true,
          pageSizes: appSetting.pageSizesArray,
        },
        columns: [
          {
            field: "no",
            title: "NO.",
            width: "20px",
            template: function (dataItem) {
              return `<span>{{dataItem.no}}</span>`;
            },
          },
          {
            field: "value",
            title: "Reason",
            width: "200px",
            template: function (dataItem) {
              if (dataItem.select) {
                return `<input class="k-textbox w100" autoComplete="off" name="Reason" ng-model="dataItem.value"/>`;
              } else {
                return `<span>{{dataItem.value}}</span>`;
              }
            },
          },
          {
            title: "ACTION",
            width: "150px",
            template: function (dataItem) {
              if (!dataItem.id) {
                  return `
                        <a ng-click="executeAction('Create', dataItem)"  class='btn-create-upgrade btn-border-upgrade' > </a>


                `;
              }
              if (dataItem.select) {
                  return `
                         <a ng-click="executeAction('Save', dataItem)"  class='btn-save-upgrade btn-border-upgrade' > </a>
                         <a ng-click="executeAction('Cancel', dataItem)"  class='btn-cancel-upgrade btn-border-upgrade' > </a>
                `;
              } else {
                  return `
                        <a ng-click="executeAction('Edit', dataItem)"  class='btn-edit-upgrade btn-border-upgrade' > </a>
                        <a ng-click="executeAction('Delete', dataItem)"  class='btn-delete-upgrade btn-border-upgrade' > </a>`;
              }
            },
          },
        ],
      };
      let requiredFields = [
        {
          fieldName: "Reason",
          title: "Reason",
        },
      ];

      dataTemporary = "";
      $scope.executeAction = async function (typeAction, dataItem) {
        let grid = $("#trainingReasonGrid").data("kendoGrid");
        switch (typeAction) {
          case "Create":
            var checkEdit = validateEdit(grid.dataSource._data);
            if (!checkEdit) {
              let errors = $rootScope.validateInRecruitment(
                requiredFields,
                dataItem
              );
              if (errors.length > 0) {
                let errorList = errors.map((x) => {
                  return x.controlName + " " + x.errorDetail;
                });
                Notification.error(
                  `Some fields are required: </br><ul>${errorList.join(
                    "<br/>"
                  )}</ul>`
                );
              } else {
                let model = {
                  value: dataItem.value,
                };
                var result = await TrainingReasonService.save(model).$promise;
                if (result.id) {
                  Notification.success("Data Successfully Saved");
                  loadPageOne();
                }
              }
            } else {
              Notification.error(
                "Please save selected item before edit/delete other item"
              );
            }
            break;
          case "Save":
            let errors = $rootScope.validateInRecruitment(
              requiredFields,
              dataItem
            );
            if (errors.length > 0) {
              let errorList = errors.map((x) => {
                return x.controlName + " " + x.errorDetail;
              });
              Notification.error(
                `Some fields are required: </br><ul>${errorList.join(
                  "<br/>"
                )}</ul>`
              );
            } else {
              let model = {
                id: dataItem.id,
                value: dataItem.value,
              };
              var result = await TrainingReasonService.save(model).$promise;
              if (result) {
                Notification.success("Data Successfully Saved");
                page = grid.pager.dataSource._page;
                pageSize = grid.pager.dataSource._pageSize;
                grid.dataSource.fetch(
                  () => grid.dataSource.page(page),
                  grid.dataSource.take(pageSize)
                );
              }
            }
            break;
          case "Cancel":
            dataItem.select = false;
            dataItem.no = dataTemporary.no;
            dataItem.id = dataTemporary.id;
            dataItem.reason = dataTemporary.code;
            grid.refresh();
            break;
          case "Edit":
            var result = validateEdit(grid.dataSource._data);
            if (result) {
              Notification.error(
                "Please save selected item before edit/delete other item"
              );
            } else {
              dataTemporary = _.clone(dataItem);
              dataItem.select = true;
              grid.refresh();
            }
            break;
          case "Delete":
            var result = validateEdit(grid.dataSource._data);
            if (result) {
              Notification.error(
                "Please save selected item before edit/delete other item"
              );
            } else {
              itemDeleteId = dataItem.id;
              $scope.dialog = $rootScope.showConfirmDelete(
                "DELETE",
                commonData.confirmContents.remove,
                "Confirm"
              );
              $scope.dialog.bind("close", confirm);
            }
            break;
          default:
            break;
        }
        actionSearch = false;
      };

      itemDeleteId = "";
      confirm = async function (e) {
        let grid = $("#trainingReasonGrid").data("kendoGrid");
        if (e.data && e.data.value) {
          let model = {
            id: itemDeleteId,
          };
          var result = await TrainingReasonService.delete(model).$promise;
          if (result) {
            Notification.success("Data Sucessfully Deleted");
            page = grid.pager.dataSource._page;
            pageSize = grid.pager.dataSource._pageSize;
            grid.dataSource.fetch(
              () => grid.dataSource.page(page),
              grid.dataSource.take(pageSize)
            );
          }
        }
      };

      async function getReasons(option) {
        var result = await TrainingReasonService.list().$promise;
        if (result) {
          let reasons = JSON.parse(JSON.stringify(result));
          if (reasons.length > 0) {
            reasons = _.sortBy(reasons, function (item) {
              return new Date(item.createdDate);
            });
          }
          $scope.trainingReasons = reasons;
          $scope.trainingReasons.forEach((x, index) => {
            x["select"] = false;
            x["no"] = index + 1;
          });
          let value = {
            id: "",
            value: "",
            select: true,
            };
          $scope.total = $scope.trainingReasons.length;
          $scope.trainingReasons.push(value);
          
          option.success($scope.dataItemList);
        }
      }

      function validateEdit(data) {
        var result = false;
        data.forEach((item) => {
          if (item.select === true && item.id !== "") {
            result = true;
            return result;
          }
        });
        return result;
      }
      function loadPageOne() {
        let grid = $("#trainingReasonGrid").data("kendoGrid");
        grid.dataSource.fetch(() => grid.dataSource.page(1));
      }
    }
  );
