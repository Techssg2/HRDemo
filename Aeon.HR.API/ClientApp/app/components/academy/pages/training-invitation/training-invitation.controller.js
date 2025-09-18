angular
  .module("ssg.module.academy.training-invitation.ctrl", ["kendo.directives"])
  .controller(
    "TrainingInvitationController",
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
      Notification,
      CourseService,
      TrainingRequestService,
      TrainingInvitationService,
      $stateParams
    ) {
      $controller("BaseController", { $scope: $scope });

      $scope.title = $translate.instant("TRAINING_INVITATION");
      $scope.enabledActions = ["Save", "Close"];
      $scope.vm = {
        currentUser: $scope.currentUser,
      };
      $scope.invitationId = $stateParams.invitationId;
      $scope.workflowInstances = {};
      $scope.showColumn = false;
      $scope.email =
        "<p>English below<br>---<br>Chào [Employee Name],<br>Academy HQ xin kính mời các anh chị tham dự khóa học [Course Name] tổ chức bởi [Supplier Name]<br>Thông tin khóa học:<br>Ngày giờ: [Course Start Date]<br>Địa điểm: [Location]<br>Chi tiết tham khảo xem file đính kèm<br>Anh chị vui lòng Accept thông báo mời học này trên hệ thống eDOC để xác nhận tham dự khóa học<br>Sau khóa học, anh chị vui lòng gửi các báo cáo sau:<br>Bản scan bằng hoàn tất khóa học (nếu có)<br>Deadline gửi report: [After Training Report Deadline]<br><br>Cảm ơn các anh chị.<br>---<br>Dear [Employee Name],<br>Academy HQ invited you to [Course Name] course organized by [Supplier Name]<br>Training course information:<br>Date and time: [Course Start Date]<br>Location: [Location]<br>For further details please check the attachment in eDOC<br>Please help to confirm you will be joining the training course in eDOC<br>After training, please help to complete the External Supplier report<br>Including completed training certificate or confirmation (if available)<br>Please done your after-training report before [After Training Report Deadline]<br><br>Best regards,<br>Academy department<br></p>";
      $scope.emailNotReport =
        "<p>English below<br>---<br>Chào [Employee Name],<br>Academy HQ xin kính mời các anh chị tham dự khóa học [Course Name] tổ chức bởi [Supplier Name]<br>Thông tin khóa học:<br>Ngày giờ: [Course Start Date]<br>Địa điểm: [Location]<br>Chi tiết tham khảo xem file đính kèm<br>Anh chị vui lòng Accept thông báo mời học này trên hệ thống eDOC để xác nhận tham dự khóa học<br><br><br>Cảm ơn các anh chị.<br>---<br>Dear [Employee Name],<br>Academy HQ invited you to [Course Name] course organized by [Supplier Name]<br>Training course information:<br>Date and time: [Course Start Date]<br>Location: [Location]<br>For further details please check the attachment in eDOC<br>Please help to confirm you will be joining the training course in eDOC<br><br><br>Best regards,<br>Academy department<br></p>";
      $scope.models = {
        courseCategory: "",
        courseName: "",
        displayServiceProvider: false,
        serviceProvider: "",
        startDate: new Date(),
        endDate: new Date(),
        reportDeadLine: new Date(new Date().setDate(new Date().getDate() + 7)),
        totalOnlineTrainingHours: 0,
        totalOfflineTrainingHours: 0,
        location: "",
        afterTrainingReportNotRequired: false,
        trainerId: "",
        trainerName: "",
        categoryId: "",
        courseId: "",
        content: $scope.email,
        trainingHoursPerDay: 1,
        numberOfDays: 1,
        totalHour: 1,
        note: "",
        numberOfParticipant: 0,
      };
      $scope.ALL_EXPORT_STATUS = [
        "Waiting for Responses",
        "Waiting for After training report",
        "Completed",
        "Cancelled",
      ];
      //attachmen
      $scope.attachmentDetails = [];
      $scope.removeFileDetails = [];
      $scope.oldAttachmentDetails = [];
      $scope.isViewOnline = false;
      $scope.sta = {
        disabledField: false,
      };

      function disableKendoEditor() {
        var editor = $("#editor").data().kendoEditor;
        var editorBody = $(editor.body);
        editorBody.attr("contenteditable", false);
      }

      this.$onInit = async function () {
        $scope.sta.disabledField = false;

        if ($scope.id) {
          let existingTrainingInvitaion = await TrainingInvitationService.get({
            id: $scope.id,
          }).$promise;
          if (existingTrainingInvitaion && existingTrainingInvitaion.id) {
            if (existingTrainingInvitaion.status == "Draft") {
              displaySaveButton();
            } else {
              $scope.sta.disabledField = true;
            }
            if (existingTrainingInvitaion.status == "Waiting for Responses") {
              displayCancelButton();
            }
            if (existingTrainingInvitaion.status == "Cancelled") {
              removeButton();
            }
            mappExistingData(existingTrainingInvitaion);
          } else {
            //this case is create new at training-request
            let trainingModels = await TrainingRequestService.get({
              id: $scope.id,
            }).$promise.then(function (results) {
              populateModel(results);
              displaySaveButton();
            });
          }
          disableKendoEditor();
        }
      };

      function displaySaveButton() {
        $scope.ALL_FORM_ACTIONS = [
          {
            name: "Save",
            icon: "k-icon font-green-jungle k-i-save",
            displayName: "COMMON_BUTTON_SAVE",
            statuses: [],
          },
          ...$scope.ALL_FORM_ACTIONS,
        ];
      }

      function displayCancelButton() {
        $scope.ALL_FORM_ACTIONS = [
          {
            name: "Cancel",
            icon: "k-icon font-green-jungle k-i-cancel",
            displayName: "COMMON_BUTTON_CANCEL",
            statuses: [],
          },
          ...$scope.ALL_FORM_ACTIONS,
        ];
      }

      $scope.onCheckReport = function (e) {
        if ($scope.models.afterTrainingReportNotRequired) {
          $scope.models.content = $scope.emailNotReport;
        } else {
          $scope.models.content = $scope.email;
        }
      };
      function getBase64(file) {
        return new Promise((resolve, reject) => {
          const reader = new FileReader();
          reader.readAsDataURL(file);
          reader.onload = () => resolve(reader.result);
          reader.onerror = (error) => reject(error);
        });
      }
      $scope.onSelectTrainingDetails = async function (e) {
        // let base64 = await getBase64(e.files[0]);
        // console.log(base64);
        // let file = e.files[0];
        // let objAtt = {
        //   file: await getBase64(file.rawFile),
        //   fileName: file.name,
        //   state: "Added",
        // };
        // $scope.attachmentDetails.push(objAtt);

        console.log(e.files);
        let files = e.files;
        let typeAtt = e.sender.name;
        if (files?.length > 0) {
          files.map(async (item, index) => {
            let objAtt = {
              file: await getBase64(item.rawFile),
              state: "Added",
              fileName: item.name,
            };
            $scope.attachmentDetails.push(objAtt);
          });
        }
      };
      $scope.removeAttach = function (e) {
        let file = e.files[0];
        $scope.attachmentDetails = $scope.attachmentDetails.filter((item) => {
          if (!item.state) return true;
          if (item.fileName != file.name) return true;
          return false;
        });
      };
      $scope.removeOldAttach = function (doc) {
        $scope.attachmentDetails = $scope.attachmentDetails.map((item) => {
          if (item.fileRef == doc.fileRef) {
            item.state = "Deleted";
          }
          return item;
        });
        $scope.oldAttachmentDetails = $scope.oldAttachmentDetails.filter(
          (item) => item.fileRef != doc.fileRef
        );
      };
      $scope.openAttachment = async function (doc) {
        const dowloadLink = document.createElement("a");
        dowloadLink.href = doc.linkView;
        dowloadLink.target = "_blank";
        dowloadLink.click();
      };
      $scope.dowloadAttachment = function (doc) {
        let url = baseUrlApi + "/Attachment/DownloadDocument?filePath=";
        const initSoucse = doc;
        const dowloadLink = document.createElement("a");
        const fileName = initSoucse.fileName;
        dowloadLink.href = url + initSoucse.fileRef;
        dowloadLink.download = fileName;
        // dowloadLink.target = "_blank";
        dowloadLink.click();
      };
      $scope.owaClose = function () {
        if (!$.isEmptyObject($("#attachmentViewDialog").data("kendoDialog"))) {
          attachmentViewDialog = $("#attachmentViewDialog").data("kendoDialog");
          attachmentViewDialog.close();
        }
        $scope.isViewOnline = false;
      };
      $scope.viewFileOnline = async function (doc) {
        let filePath = window.location.origin + doc.linkView;
        $scope.isViewOnline = true;

        if (filePath.length > 0) {
          let attachmentViewDialog = null;
          if (
            !$.isEmptyObject($("#attachmentViewDialog").data("kendoDialog"))
          ) {
            attachmentViewDialog = $("#attachmentViewDialog").data(
              "kendoDialog"
            );
          } else {
            attachmentViewDialog = $("#attachmentViewDialog")
              .kendoDialog({
                width: "90%",
                height: "85%",
              })
              .data("kendoDialog");
          }
          $("#attachment_owa")[0].setAttribute("src", filePath);
          $("#attachmentViewDialog").css("height", "93%");

          attachmentViewDialog.open();
        } else {
          Notification.error($translate.instant("NOT_SUPPORT_VIEW_ONLINE"));
          $scope.isViewOnline = false;
          $scope.$apply();
        }
      };

      $scope.showProcessingStages = function () {
        $rootScope.visibleProcessingStages($translate);
      };

      $scope.totalHour = function () {
        return $scope.models.numberOfDays * $scope.models.trainingHoursPerDay;
      };

      $scope.performAction = function (name) {
        switch (name) {
          case "Save":
            save();
            break;
          case "Close":
            close();
            break;
          case "SendInvitation":
            sendInvitation();
            break;
          case "Cancel":
            cancel();
            break;
        }
      };

      $scope.ALL_FORM_ACTIONS = [
        {
          name: "SendInvitation",
          icon: "k-icon font-green-jungle k-i-email",
          displayName: "BUTTON_SEND_INVITATION",
          statuses: ["Draft"],
        },
        {
          name: "Close",
          icon: "k-icon font-green-jungle k-i-close",
          displayName: "COMMON_BUTTON_CLOSE",
          statuses: [],
        },
      ];

      async function cancel() {
        var model = await mappingDataModel();
        model.id = $scope.invitationId;
        var result = await TrainingInvitationService.cancelInvitation(model)
          .$promise;
        Notification.success($translate.instant("STATUS_COMPLETED"));
        removeButton();
      }
      function close() {
        $state.go(appSetting.ACADAMY_ROUTES.home);
      }
      function removeButton() {
        $scope.ALL_FORM_ACTIONS = $scope.ALL_FORM_ACTIONS.filter(function (i) {
          return i.name != "SendInvitation" && i.name != "Save";
        });
      }

        $scope.createTrainingInvitation = function () {
            sendInvitation()
        };
        
      async function sendInvitation() {
        try {
          if (!isFormValid()) {
            $window.scrollTo(0, 0);
            return;
          }
          var model = await mappingDataModel();
          $scope.participantBudget.participants =
            $("#participantGrid").data("kendoGrid").dataSource._data;
          model.id = $scope.invitationId;
          var result = await TrainingInvitationService.sendInvitation(model)
            .$promise;
          Notification.success($translate.instant("STATUS_COMPLETED"));
          displayCancelButton();
          removeButton();
        } catch (e) {
          console.error(e);
          Notification.error("Error System");
        }
      }
      function mappingDataModel() {
        let participantsArray = {
          participants: getRevertParticipants(
            $scope.participantBudget.participants
          ),
        };
        let deadLine = $scope.models.afterTrainingReportNotRequired
          ? new Date(0)
          : $scope.models.reportDeadLine;
        let totalHours =
          $scope.models.numberOfDays * $scope.models.trainingHoursPerDay;
        if ($scope.attachmentDetails.length) {
          $scope.models.attachments = $scope.attachmentDetails;
        } else {
          $scope.models.attachments = [];
        }
        var dataModel = {
          TrainingRequestId: $scope.id,
          ReferenceNumber: $scope.referenceNumber,
          CategoryId: $scope.models.categoryId,
          CategoryName: $scope.models.courseCategory,
          CourseId: $scope.models.courseId,
          CourseName: $scope.models.courseName,
          ServiceProvider: $scope.models.serviceProvider,
          TrainerId: $scope.models.trainerId,
          TrainerName: $scope.models.trainerName,
          StartDate: $scope.models.startDate,
          EndDate: $scope.models.endDate,
          TrainingLocation: $scope.models.location,
          AfterTrainingReportNotRequired:
            $scope.models.afterTrainingReportNotRequired,
          AfterTrainingReportDeadline: deadLine,
          Content: $scope.models.content,
          note: $scope.models.note,
          participants: participantsArray.participants,
          HoursPerDay: $scope.models.trainingHoursPerDay,
          NumberOfDays: $scope.models.numberOfDays,
          TotalHours: totalHours,
          numberOfParticipant: $scope.models.numberOfParticipant,
          totalOfflineTrainingHours: $scope.models.totalOfflineTrainingHours,
          totalOnlineTrainingHours: $scope.models.totalOnlineTrainingHours,
          Attachments: $scope.models.attachments,
        };
        return dataModel;
      }
      function isFormValid() {
        $scope.clearErrors();
        $scope.validatePhoneNumber({
          value: $scope.participantBudget.participants,
          name: "PhoneNumber",
        });
        $scope.validateRequired({
          value: $scope.models.startDate,
          name: "Course Start Date",
        });
        $scope.validateRequired({
          value: $scope.models.endDate,
          name: "Course End Date",
        });
        $scope.validateRequired({
          value: $scope.models.reportDeadLine,
          name: "After Training Report Deadline",
        });

        validationCustomField();
        return $scope.validateForm();
      }

      function findElementByFieldName(fieldName) {
        var searchName = '[name="' + fieldName + '"]';
        return $(document).find(searchName);
      }
      function addError(err) {
        if (!err.fieldName) throw "field name is missing.";
        if (!err.error) throw "error is missing.";

        $scope.errors.push(err);

        findElementByFieldName(err.fieldName).addClass("ng-invalid");
      }
      function validationCustomField() {
        let obj = [];
        //validate course date
        if (
          new Date($scope.models.endDate) <= new Date($scope.models.startDate)
        ) {
          addError({
            fieldName: "Course End Date",
            error: $translate.instant("VALIDATE_INVITATION_DATE"),
          });
        }

        //validate location
        if ($scope.models.location && $scope.models.location.length > 30) {
          addError({
            fieldName: "Location",
            error: $translate.instant("VALIDATE_INVITATION_LOCATION"),
          });
        }

        //validate Training Hours Per Day should be at least one hour
        validateRequiredAndMinMax(
          "Training Hours Per Day",
          $scope.models.trainingHoursPerDay,
          1,
          24
        );

        //validate numberOfDays should be at least one hour
        validateRequiredAndMinMax(
          "Number of Days",
          $scope.models.numberOfDays,
          1,
          999
        );
        //validate training hours
        validateRequiredAndMinMaxIncludeZero(
          "Total Offline Training Hours",
          $scope.models.totalOfflineTrainingHours,
          0,
          999
        );
        validateRequiredAndMinMaxIncludeZero(
          "Total Online Training Hours",
          $scope.models.totalOnlineTrainingHours,
          0,
          999
        );

        return obj;
      }

      function validateRequiredAndMinMax(fieldName, value, min, max) {
        let msgRequired = $translate.instant("MSG_REQUIRED_HOURS");

        if (!value) {
          addError({
            fieldName: fieldName,
            error: msgRequired,
          });
        } else {
          if (value && value > max) {
            let msg = $translate.instant("VALIDATE_MIN_MAX");
            addError({
              fieldName: fieldName,
              error: msg.replace("{0}", min).replace("{1}", max),
            });
          }
        }
      }

      function validateRequiredAndMinMaxIncludeZero(
        fieldName,
        value,
        min,
        max
      ) {
        if (value === null || value > max) {
          let msg = $translate.instant("VALIDATE_MIN_MAX");
          addError({
            fieldName: fieldName,
            error: msg.replace("{0}", min).replace("{1}", max),
          });
        }
      }

      async function internalSave() {
        $scope.participantBudget.participants =
          $("#participantGrid").data("kendoGrid").dataSource._data;
        if (!isFormValid()) {
          $window.scrollTo(0, 0);
          return;
        }
        var model = await mappingDataModel();
        var result = await TrainingInvitationService.save(model).$promise;
        return result;
      }
      async function save() {
        try {
          var result = await internalSave();
          if (!result) return;
          $scope.id = result.id;
          $scope.invitationId = result.id;
          Notification.success($translate.instant("COMMON_SAVE_SUCCESS"));
          $state.go(
            appSetting.ACADAMY_ROUTES.trainingInvitation,
            { id: $scope.id },
            { reload: true }
          );
        } catch (e) {
          console.error(e);
          Notification.error("Error System");
        }
      }
      $scope.onChangeEndDate = function (event) {
        let endDate = new Date($scope.models.endDate);
        let deadLine = endDate.setDate(endDate.getDate() + 7);
        $scope.models.reportDeadLine = new Date(deadLine);
      };

      function populateModel(model) {
        if (!model) return;
        $scope.referenceNumber = model.referenceNumber;
        $scope.minDate = new Date();

        populateTrainingLocation(model.trainingDuration);
        populateParticipant(model.participantBudget);
        populateTrainingDetails(model.trainingDetails);
      }
      function populateTrainingLocation(model) {
        if (model && model.trainingMethod !== "Online") {
          model.trainingDurationItems.forEach((item) => {
            if (item && item.trainingMethod == "Offline") {
              $scope.models.location = item.trainingLocation;
            }
          });
        }
      }

      function mappExistingData(model) {
        $scope.participantBudget = {
          participants: model.participants,
          g1: null,
          g2: null,
          g3: null,
          g4: null,
          g5plus: null,
        };
        if (model.status === "Draft") {
          $scope.status = model.status;
          $scope.enabledActions.push("SendInvitation");
        }
        if (model.status) {
          $scope.status = model.status;
        }
        populateParticipant($scope.participantBudget);

        $scope.id = model.trainingRequestId;
        $scope.invitationId = model.id;
        $scope.referenceNumber = model.referenceNumber;
        $scope.models.categoryId = model.categoryId;
        $scope.models.courseCategory = model.categoryName;
        $scope.models.courseId = model.courseId;
        $scope.models.courseName = model.courseName;
        $scope.models.serviceProvider = model.serviceProvider;
        $scope.models.trainerId = model.trainerId;
        $scope.models.trainerName = model.trainerName;
        $scope.models.startDate = new Date(model.startDate);
        $scope.models.endDate = new Date(model.endDate);
        $scope.models.totalOfflineTrainingHours =
          model.totalOfflineTrainingHours;
        $scope.models.totalOnlineTrainingHours = model.totalOnlineTrainingHours;
        $scope.models.location = model.trainingLocation;
        $scope.models.reportDeadLine = model.afterTrainingReportDeadline;

        $scope.models.afterTrainingReportNotRequired =
          model.afterTrainingReportNotRequired;
        $scope.models.content = model.content;
        if (model.serviceProvider) {
          $scope.models.displayServiceProvider = true;
        } else {
          $scope.models.displayServiceProvider = false;
        }
        $scope.models.trainingHoursPerDay = model.hoursPerDay;
        $scope.models.numberOfDays = model.numberOfDays;
        $scope.models.totalHour = model.totalHours;
        $scope.models.note = model.note;

        $scope.attachmentDetails = model?.attachments?.length
          ? model.attachments
          : [];
        $scope.oldAttachmentDetails = [...$scope.attachmentDetails];
        if (model.startDate) {
          $scope.minDate = new Date(model.startDate);
        }
        $scope.$apply();
      }

      async function populateTrainingDetails(trainingDetails) {
        if (!trainingDetails) return;
        $scope.models.categoryId = trainingDetails.categoryId;
        $scope.models.courseId = trainingDetails.courseId;
        $scope.models.courseCategory = trainingDetails.categoryName;
        $scope.models.courseName = trainingDetails.courseName;
        if (
          trainingDetails.typeOfTraining
            .trim()
            .toUpperCase()
            .includes("EXTERNAL")
        ) {
          $scope.models.displayServiceProvider = true;
          $scope.models.serviceProvider = trainingDetails.typeOfTraining;
        } else {
          $scope.models.displayServiceProvider = false;
        }
      }

      function getRevertParticipants(participants) {
        return participants.length > 0
          ? participants.map((item) => {
              return {
                participantId: item.participantId,
                trainingRequestId: item.trainingRequestId,
                sapCode: item.sapCode,
                email: item.email,
                name: item.name,
                phoneNumber: item.phoneNumber,
                position: item.position,
                department: item.department,
              };
            })
          : [];
      }

      $scope.dialogDeptOption = {
        buttonLayout: "normal",
        animation: {
          open: {
            effects: "fade:in",
          },
        },
        schema: {
          model: {
            id: "no",
          },
        },
        actions: [
          {
            text: "Add User",
            action: function (e) {
              $scope.addUser($scope.idGrid, "#dialog_Detail_Department");
              return false;
            },
            primary: true,
          },
        ],
      };

      $scope.selectedDeparment = function (departmentItem) {
        if (departmentItem) {
          $scope.searchGridUser(departmentItem.id, $scope.idGrid);
        }
      };

      $scope.searchGridUser = async function (departmentId = "", idGrid) {
        let result = {};
        if (idGrid == "#userGrid") {
          if ($scope.vm.currentUser.divisionId) {
            result = await settingService.getInstance().users.getChildUsers({
              departmentId: "0d8f8e94-6615-4531-ac6e-89ec6f5b1331",
              limit: 100000,
              page: 1,
              searchText: $scope.gridUser.keyword,
            }).$promise;
          } else {
            result = await settingService
              .getInstance()
              .users.getUsersByOnlyDeptLine({
                depLineId: $scope.vm.currentUser.deptId,
                limit: 100000,
                page: 1,
                searchText: $scope.gridUser.keyword
                  ? $scope.gridUser.keyword.trim()
                  : "",
              }).$promise;
          }
        } else {
          result = await settingService
            .getInstance()
            .users.getUsersByOnlyDeptLine({
              depLineId: departmentId,
              limit: 100000,
              page: 1,
              searchText: "",
            }).$promise;
        }

        if (result.isSuccess) {
          let dataFilter = result.object.data.map(function (item) {
            return {
              ...item,
              showtextCode: item.sapCode,
              department:
                item.userDepartmentMappingsDepartmentName ||
                item.departmentName,
              position:
                item.userDepartmentMappingsJobGradeCaption || item.jobGrade,
              userId: item.id,
              name: item.fullName,
            };
          });
          $scope.total = result.object.count;
          let count = 1;
          let countCheck = 1;
          dataFilter.forEach((item) => {
            item["no"] = count;
            item["isCheck"] = false;
            if ($scope.arrayCheck.length > 0) {
              var result = $scope.arrayCheck.find((x) => x.id == item.id);
              if (result) {
                item.isCheck = true;
                countCheck++;
              }
            }
            $scope.userGrid.push(item);
            count++;
          });
          if ($scope.total) {
            if (countCheck == $scope.total) {
              $scope.allCheck = true;
            } else {
              if (
                $scope.arrayCheck.length == $scope.total &&
                countCheck == $scope.userGrid.length
              ) {
                $scope.allCheck = true;
              } else {
                $scope.allCheck = false;
              }
            }
          } else {
            $scope.allCheck = false;
          }
          setGridUser(
            dataFilter,
            idGrid,
            dataFilter.length,
            1,
            $scope.limitDefaultGrid
          );
        }
      };

      function setGridUser(data, idGrid, total, pageIndex, pageSizes) {
        let grid = $(idGrid).data("kendoGrid");
        let dataSource = new kendo.data.DataSource({
          data: data,
          pageSize: pageSizes,
          page: pageIndex,
          schema: {
            total: function () {
              return total;
            },
          },
        });
        if (grid) {
          grid.setDataSource(dataSource);
        }
      }

      $scope.addItemsUser = function () {
        $scope.keyWorkTemporary = "";
        $scope.limitDefaultGrid = 20;
        $scope.userGrid = [];
        $scope.arrayCheck = [];
        $scope.allCheck = false;
        $scope.gridUser.keyword = "";
        $scope.idGrid = "#userGrid";
        let grid = $("#userGrid").data("kendoGrid");
        grid.dataSource.data([]);
        $scope.searchGridUser(null, "#userGrid");
        // set title cho cái dialog
        let dialog = $("#dialog_Detail").data("kendoDialog");
        //let dialog1 = $("#dialog_Detail_Department").data("kendoDialog");
        dialog.title($translate.instant("COMMON_BUTTON_ADDUSER"));
        dialog.open();
        $rootScope.confirmDialogAddItemsUser = dialog;
        grid.tbody.on("click", ".k-checkbox", onClickAddTrainer);
      };

      function onClickAddTrainer(e) {
        var grid = $("#userGrid").data("kendoGrid");
        var row = $(e.target).closest("tr");

        if (row.hasClass("k-state-selected")) {
          setTimeout(function (e) {
            var grid = $("#userGrid").data("kendoGrid");
            grid.clearSelection();
          });
        } else {
          grid.clearSelection();
        }

        var gridDataSource = $("#userGrid").data("kendoGrid").dataSource._data;
        var rowData = grid.dataItem($(this).closest("tr"));
        gridDataSource.forEach((data) => {
          var { id } = data;
          if (id !== rowData.id) {
            data.isCheck = false;
          } else {
            data.isCheck = true;
          }
        });
      }

      $scope.addUser = function (idGrid, idDialog) {
        let grid = $(idGrid).data("kendoGrid");
        let addedUsers = grid.dataSource._data
          .filter((item) => item.isCheck)
          .map((user) => {
            return { ...user, participantId: user.id };
          });
        if (addedUsers.length < 1) return;
        else {
          if (idGrid == "#userGrid") {
            $scope.models.trainerId = addedUsers[0].id;
            $scope.models.trainerName = addedUsers[0].fullName;
            let dialog = $(idDialog).data("kendoDialog");
            dialog.close();
            return;
          }
          let participants = [
            ...$scope.participantBudget.participants,
            ...addedUsers,
          ];
          $scope.participantBudget.participants = _.uniqBy(
            participants,
            "sapCode"
          );
          let count = 1;
          $scope.participantBudget.participants.forEach((item) => {
            item["no"] = count;
            count++;
          });
          let dialog = $(idDialog).data("kendoDialog");
          dialog.close();
          setGridUser(
            $scope.participantBudget.participants,
            "#participantGrid",
            $scope.participantBudget.participants.length,
            1,
            $scope.limitDefaultGrid
          );
          $scope.$apply(function () {
            updateParticipantsAmount();
          });
        }
      };

      $scope.dialogDetailOption = {
        buttonLayout: "normal",
        animation: {
          open: {
            effects: "fade:in",
          },
        },
        schema: {
          model: {
            id: "no",
          },
        },
        actions: [
          {
            text: $translate.instant("TRAINING_REQUEST_ADD_USER"),
            action: function (e) {
              $scope.addUser($scope.idGrid, "#dialog_Detail");
              return false;
            },
            primary: true,
          },
        ],
      };

      $scope.userListByDeptGridOptions = {
        dataSource: $scope.gridUser,
        sortable: true,
        autoBind: true,
        resizable: false,
        scrollable: true,
        height: 500,
        pageable: {
          alwaysVisible: true,
          pageSizes: [5, 10, 20, 30, 40],
          responsive: true,
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
            field: "",
            title:
              "<input style='margin-left:10px' type='checkbox' ng-model='allCheck' name='allCheck' id='allCheck' class='form-check-input' ng-change='onChange(allCheck, idGrid)'/> <label class='form-checkbox-label cbox' for='allCheck' style='padding-bottom: 10px;'></label>",
            width: "50px",
            template: function (dataItem) {
              return `<input type="checkbox" ng-model="dataItem.isCheck" name="isCheck{{dataItem.no}}"
                    id="isCheck{{dataItem.no}}" class="form-check-input" style="width: 100%;margin-left:10px"'/>
                    <label class="form-checkbox-label cbox" for="isCheck{{dataItem.no}}"></label>`;
            },
            },
          {
            field: "sapCode",
            // title: "SAP Code",
            headerTemplate: $translate.instant("COMMON_SAP_CODE"),
            width: "150px",
          },
          {
            field: "fullName",
            // title: "Full Name",
            headerTemplate: $translate.instant("COMMON_FULL_NAME"),
            width: "200px",
          },
          {
            field: "department",
            // title: "Department",
            headerTemplate: $translate.instant("COMMON_DEPARTMENT"),
            width: "200px",
          },
          {
            field: "position",
            // title: "Position",
            headerTemplate: $translate.instant("JOBGRADE_MENU"),
            width: "200px",
          },
        ],
      };

      $scope.addDeparmentUser = async function (clickedAddTrainer) {
        let dialog = $("#dialog_Detail_Department").data("kendoDialog");
        dialog.title($translate.instant("COMMON_BUTTON_ADDUSER"));
        dialog.open();
        $rootScope.confirmDialogAddDeparmentUser = dialog;
        $scope.idGrid = "#userByDeptGrid";
        let result = {};
        result = await settingService
          .getInstance()
          .departments.getDepartmentTree().$promise;
        $scope.departmentList = result.object.data;
        $scope.$apply(function () {
          $scope.departments = new kendo.data.HierarchicalDataSource({
            data: $scope.departmentList,
          });
        });
        $scope.keyWorkTemporary = "";
        $scope.limitDefaultGrid = 20;
        $scope.userGrid = [];
        $scope.arrayCheck = [];
        $scope.allCheck = false;
        $scope.gridUser.keyword = "";
        let grid = $($scope.idGrid).data("kendoGrid");
        grid.dataSource.data([]);
      };

      $scope.addDeparmentTrainer = async function () {
        let dialog = $("#dialog_Detail_Department").data("kendoDialog");
        dialog.title($translate.instant("COMMON_BUTTON_ADDUSER"));
        dialog.open();
        $rootScope.confirmDialogAddDeparmentUser = dialog;
        //$scope.idGrid = "#userByDeptGrid";
        let result = {};
        result = await settingService
          .getInstance()
          .departments.getDepartmentTree().$promise;
        $scope.departmentList = result.object.data;
        $scope.$apply(function () {
          $scope.departments = new kendo.data.HierarchicalDataSource({
            data: $scope.departmentList,
          });
        });
        $scope.keyWorkTemporary = "";
        $scope.limitDefaultGrid = 20;
        $scope.userGrid = [];
        $scope.arrayCheck = [];
        $scope.allCheck = false;
        $scope.gridUser.keyword = "";
        let grid = $($scope.idGrid).data("kendoGrid");
        grid.dataSource.data([]);
      };

      function updateParticipantsAmount() {
        let userList = $scope.participantBudget.participants;
        $scope.participantBudget.g1 = userList.filter(
          (x) => x.position == "G1"
        ).length;
        $scope.participantBudget.g2 = userList.filter(
          (x) => x.position == "G2"
        ).length;
        $scope.participantBudget.g3 = userList.filter(
          (x) => x.position == "G3"
        ).length;
        $scope.participantBudget.g4 = userList.filter(
          (x) => x.position == "G4"
        ).length;
        $scope.participantBudget.g5plus =
          userList.length -
          $scope.participantBudget.g1 -
          $scope.participantBudget.g2 -
          $scope.participantBudget.g3 -
          $scope.participantBudget.g4;
      }

      function populateParticipant(participantBudget) {
        if (!participantBudget) return;
        $scope.showColumn =
          $scope.ALL_EXPORT_STATUS.indexOf($scope.status) != -1;
        $scope.models.numberOfParticipant =
          participantBudget.participants.length;
        $scope.participantBudget = {
          totalAmountWithinBudget: participantBudget.totalAmountWithinBudget,
          totalAmountNonBudget: participantBudget.totalAmountNonBudget,
          actualAmount: participantBudget.actualAmount,
          budgetBalanced: participantBudget.budgetBalanced,
          participants: participantBudget.participants.map((item, index) => {
            return {
              ...item,
              participantId: item.userId ? item.userId : item.participantId,
              no: index + 1,
              response: $scope.showColumn
                ? item.response == "Created"
                  ? "Pending"
                  : item.response
                : item.response,
            };
          }),
          g1: null,
          g2: null,
          g3: null,
          g4: null,
          g5plus: null,
        };
        updateParticipantsAmount();
        $scope.participantsGridOptions = {
          toolbar: ["excel"],
          excel: {
            fileName: "InvitationParticipants.xlsx",
            proxyURL: "https://demos.telerik.com/kendo-ui/service/export",
            filterable: true,
            allPages: true,
          },
          dataBound: function () {
            if (!$scope.showColumn) {
              $(".k-grid-excel").hide();
            }
          },
          dataSource: {
            data: $scope.participantBudget.participants,
            pageSize: 20,
          },
          sortable: false,
          editable: false,
          scrollable: true,
          selectable: true,
          pageable: {
            alwaysVisible: true,
            pageSizes: [5, 10, 20, 30, 40],
            responsive: true,
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
              field: "no",
              title: "No",
              headerTemplate: $translate.instant("COMMON_NO"),
              exportable: { excel: false },
              width: "40px",
            },
            {
              field: "sapCode",
              title: "Sap Code",
              headerTemplate: $translate.instant("COMMON_SAP_CODE"),
              width: "100px",
            },
            {
              field: "name",
              title: "Name",
              headerTemplate: $translate.instant("COMMON_FULL_NAME"),
              width: "100px",
            },
            {
              field: "email",
              title: "Email",
              headerTemplate: $translate.instant("COMMON_EMAIL"),
              width: "100px",
            },
            {
              field: "phoneNumber",
              title: "Phone Number",
              headerTemplate: $translate.instant(
                "TRAINING_REQUEST_PHONE_NUMBER"
              ),
              width: "100px",
              template: function (dataItem) {
                return `<input type='text'
                        name="PhoneNumber Of {{dataItem.sapCode}}"
                        style="width: 100%;" ng-model="dataItem.phoneNumber" 
                        class="k-textbox form-control input-sm" onkeypress="return event.keyCode < 58 && event.keyCode > 47" 
                        ng-disabled="showColumn || status == 'Cancelled'"
                        maxlength="10"/>`;
              },
            },
            {
              field: "department",
              title: "Department",
              headerTemplate: $translate.instant("COMMON_DEPARTMENT"),
              width: "150px",
            },
            {
              field: "position",
              title: "Position",
              headerTemplate: $translate.instant("JOBGRADE_MENU"),
              width: "100px",
            },
            {
              headerTemplate: $translate.instant("COMMON_ACTION"),
              width: "100px",
              hidden: $scope.showColumn,
              //locked: true,
              template: function (dataItem) {
                  return `
                        <a ng-click="delete(dataItem)" ng-disabled="showColumn || status == 'Cancelled'"  class='btn-delete-upgrade btn-border-upgrade' > </a>
                        `;
              },
            },
            {
              field: "response",
              title: "Reponse",
              headerTemplate: $translate.instant("COMMON_RESPONSE"),
              width: "100px",
              hidden: !$scope.showColumn,
              template: function (dataItem) {
                return `<label ng-if="dataItem.response=='Accept'">{{'STATUS_ACCEPT'|translate}}</label>
                        <label ng-if="dataItem.response=='Pending'">{{'STATUS_PENDING_INV'|translate}}</label>
                        <label ng-if="dataItem.response=='Decline'">{{'STATUS_DECLINE'|translate}}</label>`;
              },
            },
            {
              field: "reasonOfDecline",
              title: "Reason of Decline",
              hidden: !$scope.showColumn,
              headerTemplate: $translate.instant("COMMON_REASON_DECLINE"),
              width: "100px",
            },
            {
              field: "statusOfReport",
              title: "Status of Report",
              hidden: !$scope.showColumn,
              headerTemplate: $translate.instant("COMMON_REPORT_STATUS"),
              width: "100px",
            },
          ],
        };
        $scope.userGrid = [];
        $scope.gridUser = {
          keyword: "",
        };
      }

      $scope.dialogConfirmOption = {
        buttonLayout: "normal",
        animation: {
          open: {
            effects: "fade:in",
          },
        },
        schema: {
          model: {
            id: "no",
          },
        },
        actions: [
          {
            text: "Delete",
            action: function (e) {
              $scope.deleteParticipant();
              return false;
            },
            primary: true,
          },
          {
            text: "Cancel",
            action: function (e) {
              let dialog = $("#dialog_Confirm").data("kendoDialog");
              dialog.close();
              return false;
            },
            primary: true,
          },
        ],
      };

      $scope.delete = function (dataItem) {
        let dialog = $("#dialog_Confirm").data("kendoDialog");
        $scope.deletedParticipant = dataItem;
        dialog.open();
      };

      $scope.deleteParticipant = function () {
        let grid = $("#participantGrid").data("kendoGrid");
        let deleteUser = grid.dataSource._data.filter(
          (item) => item.sapCode != $scope.deletedParticipant.sapCode
        );
        $scope.participantBudget.participants = deleteUser;
        setGridUser(
          $scope.participantBudget.participants,
          "#participantGrid",
          $scope.participantBudget.participants.length,
          1,
          $scope.limitDefaultGrid
        );
        updateParticipantsAmount();
        let dialog = $("#dialog_Confirm").data("kendoDialog");
        dialog.close();
      };

      $scope.onChange = function (isCheckAll, idGrid) {
        if (isCheckAll) {
          $scope.userGrid.forEach((item) => {
            item.isCheck = true;
          });
          $scope.allCheck = true;
        } else {
          $scope.userGrid.forEach((item) => {
            item.isCheck = false;
          });
          $scope.allCheck = false;
        }
        setGridUser(
          $scope.userGrid,
          idGrid,
          $scope.userGrid.length,
          1,
          $scope.limitDefaultGrid
        );
      };

      $scope.userListGridOptions = {
        dataSource: $scope.gridUser,
        sortable: true,
        autoBind: true,
        resizable: false,
        scrollable: true,
        height: 400,
        pageable: {
          alwaysVisible: true,
          pageSizes: [5, 10, 20, 30, 40],
          responsive: false,
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
            field: "",
            selectable: true,
            title: "",
            // title:
            //   "<input type='checkbox' ng-model='allCheck' name='allCheck' id='allCheck' class='k-checkbox' ng-change='onChange(allCheck, idGrid)'/> <label class='k-checkbox-label cbox' for='allCheck' style='padding-bottom: 10px;'></label>",
            width: "50px",
            template: function (dataItem) {
              return `<input type="checkbox" ng-model="dataItem.isCheck" name="isCheck{{dataItem.no}}"
                    id="isCheck{{dataItem.no}}" class="form-check-input" style="width: 100%;"'/>
                    <label class="form-check-label cbox" for="isCheck{{dataItem.no}}"></label>`;
            },
          },
          {
            field: "sapCode",
            // title: "SAP Code",
            headerTemplate: $translate.instant("COMMON_SAP_CODE"),
            width: "150px",
          },
          {
            field: "fullName",
            // title: "Full Name",
            headerTemplate: $translate.instant("COMMON_FULL_NAME"),
            width: "200px",
          },
          {
            field: "department",
            // title: "Department",
            headerTemplate: $translate.instant("COMMON_DEPARTMENT"),
            width: "200px",
          },
          {
            field: "position",
            // title: "Position",
            headerTemplate: $translate.instant("COMMON_POSITION"),
            width: "200px",
          },
        ],
      };
    } //end scope
  );
