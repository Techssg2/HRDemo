angular
  .module("ssg.module.academy.view-invitation.ctrl", ["kendo.directives"])
  .controller(
    "ViewInvitationController",
    function (
      $scope,
      $rootScope,
      TrainingInvitationService,
      $window,
      $translate,
      $controller,
      appSetting,
      Notification,
      $state
    ) {
      $controller("BaseController", { $scope: $scope });
      $scope.invitation = {};
      $scope.response = "";
      $scope.reasonOfDeclineText = "";
      $scope.enabledActions = ["Accept", "Decline"];
      //attachmen
      $scope.attachmentDetails = [];
      $scope.removeFileDetails = [];
      $scope.oldAttachmentDetails = [];
      $scope.isViewOnline = false;

      $scope.performAction = function (name) {
        switch (name) {
          case "Accept":
            accept();
            break;
          case "Decline":
            reasonofDeclineDialog();
            break;
          case "Close":
            cancel();
            break;
          case "Create After Training Report":
            createAfterTrainingReport();
            break;
        }
      };
      $scope.ALL_FORM_ACTIONS = [
        {
          name: "Accept",
          icon: "k-i-check font-green-jungle",
          displayName: "STATUS_ACCEPT",
          responses: ["Pending"],
        },
        {
          name: "Decline",
          icon: "k-i-close font-red",
          displayName: "STATUS_DECLINE",
          responses: ["Pending"],
        },
        {
          name: "Close",
          icon: "k-i-close",
          displayName: "COMMON_BUTTON_CLOSE",
          responses: [],
        },
      ];
      this.$onInit = async function () {
        $window.scrollTo(0, 0);
        let participant = await getInvitationByParticipant();
        if (participant) {
          if (participant.enabledActions)
            $scope.enabledActions = participant.enabledActions;
          if (participant.trainingInvitationId) {
            let invitation = await getInvitationToShowAttachment(
              participant.trainingInvitationId
            );
            if (invitation) {
              $scope.attachmentDetails = invitation?.attachments?.length
                ? invitation.attachments
                : [];
              $scope.oldAttachmentDetails = [...$scope.attachmentDetails];
            }
          }
          $scope.participant = participant;
          $scope.reasonOfDeclineModel = { isValid: true, errorMessage: "" };
          $("#emailContent").html(participant.emailContent);
          $scope.response = participant.response;
          if (participant.response && participant.response == "Accept") {
            if (
              participant.endDate &&
              participant.statusOfReport == "Not Submitted"
            ) {
              let today = new Date();
              let endday = new Date(participant.endDate);
              let enddayReport = new Date(
                participant.afterTrainingReportDeadline
              );
              if (today > enddayReport) return;
              if (today > endday) {
                $scope.ALL_FORM_ACTIONS = [
                  {
                    name: "Create After Training Report",
                    icon: "k-i-plus font-green-jungle",
                    displayName: "TRAINING_REPORT_CREATE",
                    responses: [],
                  },
                  ...$scope.ALL_FORM_ACTIONS,
                ];
              }
            }
          }
        }
        $scope.$apply();
      };
      function validateReasonOfDecline() {
        $scope.$apply(function () {
          if (!$scope.reasonOfDeclineText) {
            $scope
              .findElementByFieldName("reasonOfDecline")
              .addClass("ng-invalid");
            $scope.reasonOfDeclineModel = {
              isValid: false,
              errorMessage: $translate.instant("COMMON_FIELD_IS_REQUIRED"),
            };
          } else {
            $scope
              .findElementByFieldName("reasonOfDecline")
              .removeClass("ng-invalid");
            $scope.reasonOfDeclineModel = {
              isValid: true,
              errorMessage: "",
            };
          }
        });
      }
      function reasonofDeclineDialog() {
        let commentDialog = $("#reasonOfDecline").kendoDialog({
          buttonLayout: "normal",
          width: 600,
          visible: false,
          title: "Reason of Decline",
          animation: {
            open: {
              effects: "fade:in",
            },
          },
          actions: [
            {
              text: "Ok",
              action: function (e) {
                validateReasonOfDecline();
                decline();
                return false;
              },
              primary: true,
            },
            {
              text: "Close",
              action: function (e) {
                $scope.reasonOfDeclineText = "";
                return true;
              },
              primary: true,
            },
          ],
        });
        let boxReason = commentDialog.data("kendoDialog");
        boxReason.open();
      }
      async function getInvitationByParticipant() {
        if (!$scope.id) return;
        return await TrainingInvitationService.getByParticipant({
          id: $scope.id,
        }).$promise;
      }
      async function getInvitationToShowAttachment(invitationId) {
        if (!invitationId) return;
        return await TrainingInvitationService.get({
          id: invitationId,
        }).$promise;
      }
      async function accept() {
        try {
          var req = { ...$scope.participant };
          await TrainingInvitationService.accept(req).$promise;
          Notification.success("Accept successfully.");
          $state.go(
            appSetting.ACADAMY_ROUTES.viewInvitation,
            { id: $scope.id },
            { reload: true }
          );
        } catch (e) {
          console.error(e);
          Notification.error("Error System");
        }
      }
      async function decline() {
        try {
          let validate = $scope.reasonOfDeclineModel;
          if (!validate.isValid) return;
          var req = {
            ...$scope.participant,
            ReasonOfDecline: $scope.reasonOfDeclineText,
          };
          await TrainingInvitationService.decline(req).$promise;
          Notification.success("Decline successfully.");
          let commentDialog = $("#reasonOfDecline").data("kendoDialog");
          commentDialog.close();
          $state.go(
            appSetting.ACADAMY_ROUTES.viewInvitation,
            { id: $scope.id },
            { reload: true }
          );
        } catch (e) {
          console.error(e);
          Notification.error("Error System");
        }
      }
      async function createAfterTrainingReport() {
        $state.go(
          appSetting.ACADAMY_ROUTES.externalSupplierReport,
          { id: "", invitationId: $scope.participant.trainingInvitationId },
          { reload: true }
        );
      }
      function cancel() {
        $state.go(appSetting.ACADAMY_ROUTES.home);
      }
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
        let file = e.files[0];
        let objAtt = {
          file: await getBase64(file.rawFile),
          fileName: file.name,
          state: "Added",
        };
        $scope.attachmentDetails.push(objAtt);
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
        if (!doc.linkView) {
          Notification.error(
            $translate.instant("NOT_SUPPORT_VIEW_ONLINE_TYPE")
          );
          return;
        }
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
          $translate.instant("NOT_SUPPORT_VIEW_ONLINE_TYPE");
          $scope.isViewOnline = false;
          $scope.$apply();
        }
      };
    }
  );
