angular
  .module("ssg.academy.external-training-report.module", ["kendo.directives"])
  .controller(
    "ExternalTrainingReport",
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
      TrainingReportService,
      TrainingInvitationService
    ) {
      $controller("BaseController", { $scope: $scope });
      $scope.title = $translate.instant("TRAINING_REPORT");
      $scope.titleEdit = $translate.instant("TRAINING_REPORT");
      $scope.invitationId = $stateParams.invitationId;
      $scope.isHODRemark = false;
      // $scope.invitationId = "94944a25-66f7-45aa-ab5f-2745bb2fe26b";
      $scope.status = "Draft";
      $scope.realStatus = "Draft";
      $scope.enabledActions = ["Save", "Submit"];
      $scope.performAction = function (name) {
        switch (name) {
          case "Save":
            save();
            break;
          case "Submit":
            submit();
            break;
          case "Approve":
          case "RequestToChange":
            showCommentDialog(name);
            break;
          case "Close":
            cancel();
            break;
        }
      };
      $scope.ALL_FORM_ACTIONS = [
        {
          name: "Submit",
          displayName: "COMMON_BUTTON_SUBMIT",
          icon: "k-icon k-i-hyperlink-open font-green-jungle",
          statuses: ["Draft", "Requested To Change"],
        },
        {
          name: "Save",
          displayName: "COMMON_BUTTON_SAVE",
          icon: "k-icon k-i-save font-green-jungle",
          statuses: ["Draft", "Requested To Change", "Pending"],
        },
        {
          name: "Approve",
          displayName: "COMMON_BUTTON_APPROVE",
          icon: "fa fa-check font-green-jungle",
          statuses: ["Pending"],
        },
        {
          name: "Reject",
          displayName: "COMMON_BUTTON_REJECT",
          icon: "fa fa-minus-circle font-red",
          statuses: ["Pending"],
        },
        {
          name: "RequestToChange",
          displayName: "COMMON_BUTTON_REQUEST_TO_CHANGE",
          icon: "fa fa-minus-circle font-red",
          statuses: ["Pending"],
        },
        {
          name: "Close",
          displayName: "COMMON_BUTTON_CLOSE",
          icon: "k-icon k-i-close font-green-jungle",
          statuses: [],
        },
      ];
      $scope.data = [];
      $scope.materialAttachmentDetails = [];
      $scope.certificateAttachmentDetails = [];
      $scope.oldAttachmentDetailsMaterial = [];
      $scope.oldAttachmentDetailsCertificate = [];
      // $scope.attachmentDetailsmaterial = [];
      // $scope.attachmentDetailsCertificate = [];

      //code for attachment
      $scope.isEditableTrainerName = true;
      $scope.enabledActionsButton = true;
      $scope.isViewOnline = false;
      function getBase64(file) {
        return new Promise((resolve, reject) => {
          const reader = new FileReader();
          reader.readAsDataURL(file);
          reader.onload = () => resolve(reader.result);
          reader.onerror = (error) => reject(error);
        });
      }
      $scope.onSelect = async function (e) {
        let files = e.files;
        let typeAtt = e.sender.name;
        let allowExtendionFile = [".docx", ".xlsx", ".pdf", ".csv", ".xls"];
        if (files?.length > 0) {
          files.map(async (item, index) => {
            if (!allowExtendionFile.includes(item.extension)) return;
            let objAtt = {
              file: await getBase64(item.rawFile),
              state: "Added",
              fileName: item.name,
            };
            if (typeAtt == "Material") {
              $scope.materialAttachmentDetails.push(objAtt);
            }
            if (typeAtt == "Certificate") {
              $scope.certificateAttachmentDetails.push(objAtt);
            }
          });
        }
        console.log($scope.certificateAttachmentDetails);
      };

      $scope.removeAttach = function (e) {
        let file = e.files[0];
        let typeAtt = e.sender.name;
        if (typeAtt == "Material") {
          $scope.materialAttachmentDetails =
            $scope.materialAttachmentDetails.filter((item) => {
              if (!item.state) return true;
              if (item.fileName != file.name) return true;
              return false;
            });
        }
        if (typeAtt == "Certificate") {
          $scope.certificateAttachmentDetails =
            $scope.certificateAttachmentDetails.filter((item) => {
              if (!item.state) return true;
              if (item.fileName != file.name) return true;
              return false;
            });
        }
      };

      $scope.removeOldAttach = function (doc, key) {
        switch (key) {
          case "Material":
            {
              $scope.materialAttachmentDetails =
                $scope.materialAttachmentDetails.map((item) => {
                  if (item.fileRef == doc.fileRef) {
                    item.state = "Deleted";
                  }
                  return item;
                });

              $scope.oldAttachmentDetailsMaterial =
                $scope.oldAttachmentDetailsMaterial.filter(
                  (item) => item.fileRef != doc.fileRef
                );
            }
            break;
          case "Certificate":
            {
              $scope.certificateAttachmentDetails =
                $scope.certificateAttachmentDetails.map((item) => {
                  if (item.fileRef == doc.fileRef) {
                    item.state = "Deleted";
                  }
                  return item;
                });

              $scope.oldAttachmentDetailsCertificate =
                $scope.oldAttachmentDetailsCertificate.filter(
                  (item) => item.fileRef != doc.fileRef
                );
            }
            break;
          default:
            break;
        }
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
      $scope.openAttachment = async function (doc) {
        const dowloadLink = document.createElement("a");
        dowloadLink.href = doc.linkView;
        dowloadLink.target = "_blank";
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
        if (!doc.linkView) {
          Notification.error(
            $translate.instant("NOT_SUPPORT_VIEW_ONLINE_TYPE")
          );
          return;
        }

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
          Notification.error(
            $translate.instant("NOT_SUPPORT_VIEW_ONLINE_TYPE")
          );
          $scope.isViewOnline = false;

          $scope.$apply();
        }
      };
      function isExitQuestion(arr, surveyQuestion) {
        if (!arr.length) return false;
        let q = arr.find((item) => item.surveyQuestion == surveyQuestion);
        if (q) return true;
        return false;
      }
      $scope.addMoreplan = function () {
        if (!$scope.isEditable) return;
        if ($scope.dataC && $scope.dataC.length < 10) {
          $scope.dataC = [
            ...$scope.dataC,
            {
              section: "C",
              subSection: "C.1",
              actionPlanCode: "",
              quarter1: false,
              quarter2: false,
              quarter3: false,
              quarter4: false,
            },
          ];
        }
      };
      $scope.deleteplan = function (index) {
        if (!$scope.isEditable) return;
        if ($scope.dataC?.length == 1) return;
        $scope.dialog = $rootScope.showConfirmDelete(
          $translate.instant("COMMON_BUTTON_DELETE"),
          $translate.instant("COMMON_DELETE_VALIDATE"),
          $translate.instant("COMMON_BUTTON_CONFIRM")
        );
        $scope.dialog.bind("close", function (e) {
          if (e.data && e.data.value)
            $scope.dataC = $scope.dataC.filter((val, i) => i !== index);
        });
      };
      $scope.changeEventForsubQuestionC = function (event, index, quater) {
        if (!event) return;

        let answer = event.target.checked;
        let newArr = $scope.dataC.map((q, i) => {
          if (i == index) {
            q[quater] = answer;
          }
          return q;
        });
        $scope.dataC = newArr;
      };
      $scope.changeEventForsubQuestionB1 = function (event) {
        if (!event) return;
        let surveyQuestion = event.target.id;
        let answer = event.target.checked;
        let arr = [];
        arr = $scope.data.map((question) => {
          if (question.surveyQuestion == "B11") {
            question.childQuestion = question.childQuestion?.map(
              (cquestion) => {
                if (cquestion.surveyQuestion == surveyQuestion) {
                  cquestion.value = answer;
                }
                if (cquestion.surveyQuestion == "B118" && !cquestion.value) {
                  $scope.surveyDataModel.otherReasons = "";
                }
                return cquestion;
              }
            );
          }
          return question;
        });
        var grid = $("#surveyQuestion").data("kendoGrid");
        grid.setDataSource($scope.data);
      };
      $scope.changeEvent = function (kendoEvent) {
        if (!kendoEvent) return;
        let questionName = kendoEvent.target.name;
        let answer = kendoEvent.target.value;
        let arr = [];
        arr = $scope.data.map((question) => {
          if (question.surveyQuestion == questionName) {
            question.value = answer;
          }
          return question;
        });
        if (questionName == "B1") {
          if (answer == "1" || answer == "3") {
            let isExit = isExitQuestion(arr, "B11");
            if (!isExit) arr = [...arr, $scope.subQuestionB1];
          }
          if (answer == "2") {
            arr = arr.filter((item) => item.surveyQuestion !== "B11");
          }
        }
        $scope.data = [...arr];
        var grid = $("#surveyQuestion").data("kendoGrid");
        grid.setDataSource($scope.data);
      };
      function watch(value) {
        $rootScope.$watch("isLoading", function (newValue, oldValue) {
          kendo.ui.progress($("#loading"), value);
        });
      }

      this.$onInit = async function () {
        // watch(true);

        $window.scrollTo(0, 0);
        if (!$scope.invitationId && !$scope.id) {
          $scope.enabledActionsButton = false;
          return;
        }
        var model = {
          employeeInfo: {
            sapCode: "",
            fullName: "",
            departmentName: "",
            regionName: "",
            position: "",
          },
          trainingInvitationId: "",
          courseName: "",
          trainerName: "",
          actualAttendingDate: "",
          otherReasons: "",
          otherFeedback: "",
          remark: "",
          referenceNumber: "",
          trainingReportAttachments: null,
          trainingActionPlans: [],
          trainingSurveyQuestions: [],
          realStatus: "Draft",
        };
        try {
          // go to from invitation view has invitationId
          if ($scope.invitationId) {
            let draftReport = await TrainingReportService.getReportInvitationId(
              {
                id: $scope.invitationId,
              }
            ).$promise;
            draftReport = JSON.parse(JSON.stringify(draftReport));
            if (!$.isEmptyObject(draftReport)) {
              model = draftReport;
            } else {
              if (!model.trainingInvitationId) {
                let invitationInfor = {};
                if ($scope.invitationId) {
                  invitationInfor = await TrainingInvitationService.get({
                    id: $scope.invitationId,
                  }).$promise;
                  model.trainingInvitationId = $scope.invitationId;
                  model.courseName = invitationInfor.courseName;
                  model.actualAttendingDate =
                    invitationInfor.actualAttendingDate;
                  model.trainerName = invitationInfor.trainerName;
                  model.referenceNumber = invitationInfor.referenceNumber;
                }
              }
            }
          }
          if ($scope.id) {
            model = await TrainingReportService.get({
              id: $scope.id,
            }).$promise;
            // get workflow
            let workflowInstances =
              await TrainingReportService.progressingStage({
                id: $scope.id,
              }).$promise;

            if (workflowInstances) {
              $scope.workflowInstances = workflowInstances;
              let currentState = workflowInstances.histories?.filter(
                (item) => !item.action
              );
              if (currentState.length) {
                $scope.isHODRemark = currentState[0].stepNumber == 1;
              }
            }
          }
          populateModel(model);

          $scope.$apply(function () {});
          // watch(false);
        } catch (error) {
          // watch(false);
          console.log(error);
        }
      };

      function populateModel(model) {
        if (!model) return;
        $scope.referenceNumber = model.referenceNumber;
        if (model.status) $scope.status = model.status;
        if (model.realStatus) $scope.realStatus = model.realStatus;
        if (model.enabledActions) $scope.enabledActions = model.enabledActions;

        if ($scope.enabledActions.indexOf("Save") != -1) {
          $scope.isEditable = true;
        }
        $scope.surveyDataModel = model;
        if ($scope.isEditable) {
          $scope.isEditableTrainerName = $scope.surveyDataModel?.trainerName
            ? false
            : true;
        }

        populateEmployeeInfor(model.employeeInfo);
        populateCourseInfor(model);
        populateSurveyQuestion(model.trainingSurveyQuestions);
        populateActionsPlan(model.trainingActionPlans);
        $scope.commentModel = { isValid: true, errorMessage: "" };
      }
      function populateActionsPlan(actionsPlan) {
        $scope.dataC = [
          {
            section: "C",
            subSection: "C.1",
            actionPlanCode: "",
            quarter1: false,
            quarter2: false,
            quarter3: false,
            quarter4: false,
          },
        ];
        if (!actionsPlan?.length) return;
        $scope.dataC = actionsPlan;
      }
      function populateEmployeeInfor(employeeInfo) {
        if (!employeeInfo) return;
        $scope.employeeInfo = {
          fullName: $scope.id
            ? employeeInfo.fullName
            : $scope.currentUser?.fullName,
          sapCode: $scope.id
            ? employeeInfo.sapCode
            : $scope.currentUser?.sapCode,
          location: $scope.id
            ? employeeInfo.location
            : $scope.currentUser?.workLocationName,
          departmentName: $scope.id
            ? employeeInfo.departmentName
            : $scope.currentUser?.divisionName
            ? $scope.currentUser?.divisionName
            : $scope.currentUser?.deptName,
          position: $scope.id ? employeeInfo.position : getPosition(),
          regionName: $scope.id ? employeeInfo.regionName : "",
          departmentId:
            $scope.id || employeeInfo.departmentId
              ? employeeInfo.departmentId
              : $scope.currentUser?.divisionId
              ? $scope.currentUser?.divisionId
              : $scope.currentUser?.deptId,
        };
      }
      function getPosition() {
        let jobGradeRegex = RegExp(/\(G\d\)/g);
        if (jobGradeRegex.test($scope.currentUser?.positionName)) {
          return $scope.currentUser?.positionName;
        } else {
          return (
            $scope.currentUser?.positionName +
            " (" +
            $scope.currentUser?.jobGradeName +
            ")"
          );
        }
      }
      function populateSurveyQuestion(surveyQuestion) {
        let dataModel = [
          //a1
          {
            id: "",
            trainingReportId: "",
            e_question: "Please rate your overall satisfaction on the course:",
            v_question:
              "Hãy chia sẻ nhận định chung về khoá học bạn đã tham gia:",
            surveyQuestion: "",
            parentQuestion: null,
            value: "noAnswer",
            section: "A",
            subSection: "A.1",
            isBold: true,
            questionSection: "A.1",
          },
          {
            id: "",
            trainingReportId: "",
            e_question: "Rating scale: 1- Not satisfied / 5- Very satisfied",
            v_question: "Thang điểm: 1- Không hài lòng / 5- Rất hài lòng",
            surveyQuestion: "",
            parentQuestion: null,
            value: "noAnswer",
            section: "",
            subSection: "",
            questionSection: "A.1",
          },
          {
            id: "",
            trainingReportId: "",
            e_question: "Training content",
            v_question: "Nội dung khóa học",
            surveyQuestion: "A11",
            parentQuestion: null,
            value: "",
            section: "",
            subSection: "",
            questionSection: "A.1",
          },
          {
            id: "",
            trainingReportId: "",
            e_question: "Activities in the training course",
            v_question: "Các hoạt động trong lớp học",
            surveyQuestion: "A12",
            parentQuestion: null,
            value: "",
            section: "",
            subSection: "",
            questionSection: "A.1",
          },
          {
            id: "",
            trainingReportId: "",
            e_question: "Assignments / Homework",
            v_question: "Bài tập thực hành / ứng dụng",
            surveyQuestion: "A13",
            parentQuestion: null,
            value: "",
            section: "",
            subSection: "",
            questionSection: "A.1",
          },
          {
            id: "",
            trainingReportId: "",
            e_question: "Training course's atmosphere",
            v_question: "Không khí lớp học",
            surveyQuestion: "A14",
            parentQuestion: null,
            value: "",
            section: "",
            subSection: "",
            questionSection: "A.1",
          },
          {
            id: "",
            trainingReportId: "",
            e_question: "Training material / tools",
            v_question: "Tài liệu / công cụ hỗ trợ học tập",
            surveyQuestion: "A15",
            parentQuestion: null,
            value: "",
            section: "",
            subSection: "",
            questionSection: "A.1",
          },
          {
            id: "",
            trainingReportId: "",
            e_question: "Trainer's competence / experience",
            v_question: "Trình độ / kinh nghiệm của giảng viên",
            surveyQuestion: "A16",
            parentQuestion: null,
            value: "",
            section: "",
            subSection: "",
            questionSection: "A.1",
          },
          {
            id: "",
            trainingReportId: "",
            e_question: "Facilities",
            v_question: "Cơ sở vật chất",
            surveyQuestion: "A17",
            parentQuestion: null,
            value: "",
            section: "",
            subSection: "",
            questionSection: "A.1",
          },
          //a2
          {
            id: "",
            trainingReportId: "",
            e_question: "Please share your opinion about following statments:",
            v_question: "Hãy chia sẻ ý kiến của bạn về các thông tin sau:",
            surveyQuestion: "",
            parentQuestion: null,
            value: "noAnswer",
            section: "",
            subSection: "A.2",
            isBold: true,
            questionSection: "A.2",
          },
          {
            id: "",
            trainingReportId: "",
            e_question: "Before attending the course:",
            v_question: "Trước khi tham gia khoá học:",
            surveyQuestion: "",
            parentQuestion: null,
            value: "noAnswer",
            section: "",
            subSection: "A.2.1",
            isBold: true,
            questionSection: "A.2",
          },
          {
            id: "",
            trainingReportId: "",
            e_question:
              "I was shared about the expectations before attending the course (from my direct supervisor)",
            v_question:
              "Tôi được chia sẻ về mong đợi trước khi tham gia khoá học (từ cấp trên trực tiếp)",
            surveyQuestion: "A211",
            parentQuestion: null,
            value: "",
            section: "",
            subSection: "",
            questionSection: "A.2",
          },
          {
            id: "",
            trainingReportId: "",
            e_question:
              "I was informed about the content before attending the course",
            v_question:
              "Tôi được chia sẻ thông tin về nội dung trước khi tham gia khoá học ",
            surveyQuestion: "A212",
            parentQuestion: null,
            value: "",
            section: "",
            subSection: "",
            questionSection: "A.2",
          },
          {
            id: "",
            trainingReportId: "",
            e_question:
              "I received the training announcement and instruction before attending the course ",
            v_question:
              "Tôi nhận được thông báo đi học và các hướng dẫn liên quan trước khi tham gia khóa học",
            surveyQuestion: "A213",
            parentQuestion: null,
            value: "",
            section: "",
            subSection: "",
            questionSection: "A.2",
          },
          {
            id: "",
            trainingReportId: "",
            e_question:
              "I received full study material before or during the training",
            v_question:
              "Tôi nhận được đầy đủ học liệu trước họặc trong quá trình học",
            surveyQuestion: "A214",
            parentQuestion: null,
            value: "",
            section: "",
            subSection: "",
            questionSection: "A.2",
          },
          //a22
          {
            id: "",
            trainingReportId: "",
            e_question: "During the course:",
            v_question: "Trong quá trình tham gia khoá học:",
            surveyQuestion: "A221",
            parentQuestion: null,
            value: "noAnswer",
            section: "",
            subSection: "A.2.2",
            questionSection: "A.2",
            isBold: true,
          },
          {
            id: "",
            trainingReportId: "",
            e_question:
              "Instructor shared/explaind about the objectives of the course",
            v_question:
              "Giảng viên có chia sẻ / giải thích về mục tiêu của khoá học",
            surveyQuestion: "A222",
            parentQuestion: null,
            value: "",
            section: "",
            subSection: "",
            questionSection: "A.2",
          },
          {
            id: "",
            trainingReportId: "",
            e_question: "I understood the objectives of the course",
            v_question: "Tôi hiểu rõ về mục tiêu của khoá học",
            surveyQuestion: "A223",
            parentQuestion: null,
            value: "",
            section: "",
            subSection: "",
            questionSection: "A.2",
          },
          {
            id: "",
            trainingReportId: "",
            e_question:
              "Trainer created opportunities/encouraged me to participate in class activities",
            v_question:
              "Giảng viên tạo cơ hội / khuyến khích tôi tham gia hoạt động trong lớp",
            surveyQuestion: "A224",
            parentQuestion: null,
            value: "",
            section: "",
            subSection: "",
            questionSection: "A.2",
          },
          {
            id: "",
            trainingReportId: "",
            e_question: "I learnt a lot from the other learners",
            v_question: "Tôi học hỏi được nhiều từ các học viên khác",
            surveyQuestion: "A225",
            parentQuestion: null,
            value: "",
            section: "",
            subSection: "",
            questionSection: "A.2",
          },
          {
            id: "",
            trainingReportId: "",
            e_question:
              "I received support from my direct supervisor while attending the course (e.g. support in arranging workload, approving shifts, etc.)",
            v_question:
              "Tôi nhận được sự hỗ trợ từ quản lý trực tiếp trong quá trình học (vd: hỗ trợ sắp xếp công việc, duyệt ca đi làm, v.v)",
            surveyQuestion: "A226",
            parentQuestion: null,
            value: "",
            section: "",
            subSection: "",
            questionSection: "A.2",
          },
          //a23
          {
            id: "",
            trainingReportId: "",
            e_question: "After attending the course:",
            v_question: "Sau khi tham gia khoá học:",
            surveyQuestion: "A231",
            parentQuestion: null,
            value: "noAnswer",
            section: "",
            subSection: "A.2.3",
            questionSection: "A.2",
            isBold: true,
          },
          {
            id: "",
            trainingReportId: "",
            e_question:
              "The trainer continues to support me after the course ended",
            v_question:
              "Giảng viên vẫn tiếp tục hỗ trợ tôi sau khi khoá học kết thúc",
            surveyQuestion: "A232",
            parentQuestion: null,
            value: "",
            section: "",
            subSection: "",
            questionSection: "A.2",
          },
          {
            id: "",
            trainingReportId: "",
            e_question: "I found this course very useful to me",
            v_question:
              "Tôi nhận thấy khoá học này thực tế và có tính ứng dụng",
            surveyQuestion: "A233",
            parentQuestion: null,
            value: "",
            section: "",
            subSection: "",
            questionSection: "A.2",
          },
          {
            id: "",
            trainingReportId: "",
            e_question:
              "I plan to share the knowledge I've learnt in this course with my colleagues/employees",
            v_question:
              "Tôi có kế hoạch chia sẻ lại kiến thức đã học cho đồng nghiệp / nhân viên của tôi",
            surveyQuestion: "A234",
            parentQuestion: null,
            value: "",
            section: "",
            subSection: "",
            questionSection: "A.2",
          },
          {
            id: "",
            trainingReportId: "",
            e_question: "Will you recommend this course to others?",
            v_question:
              "Bạn sẽ giới thiệu khoá học này cho những người có nhu cầu?",
            surveyQuestion: "B1",
            parentQuestion: null,
            value: "",
            section: "B",
            subSection: "B.1",
            questionSection: "B.1",
          },
        ];
        $scope.surveyQuestionOptions = {
          dataSource: {
            data: $scope.data,
          },
          sortable: false,
          editable: false,
          scrollable: false,
          selectable: false,

          columns: [
            {
              field: "section",
              headerTemplate: "",
              width: 1,
              editable: function (e) {
                return false;
              },
            },
            {
              field: "subSection",
              headerTemplate: "",
              width: 1,
            },
            {
              field: "v_question",
              headerTemplate: "VIETNAMESE",
              width: "80px",
              template: function (dataItem) {
                return `<strong ng-if="dataItem.isBold" >{{dataItem.v_question}}</strong>
              <span ng-if="!dataItem.isBold" >{{dataItem.v_question}}</span>`;
              },
              editable: function (e) {
                return false;
              },
            },
            {
              field: "e_question",
              headerTemplate: "ENGLISH",
              width: "80px",
              template: function (dataItem) {
                return `<strong ng-if="dataItem.isBold" >{{dataItem.e_question}}</strong>
              <span ng-if="!dataItem.isBold" >{{dataItem.e_question}}</span>`;
              },
            },
            {
              field: "answer",
              title: "",
              headerTemplate: "",
              template: function (dataItem) {
                if (dataItem.questionSection == "A.1") {
                  return ` <div ng-if="dataItem.value !='noAnswer'">
                          <div ng-include src="'training-report/survey-report-section-a1.partial.html'"></div>
                    </div>`;
                }
                if (dataItem.questionSection == "A.2") {
                  return ` <div ng-if="dataItem.value !='noAnswer'">
                        <div  ng-include src="'training-report/survey-report-section-a2.partial.html'"></div>
                    </div>`;
                }
                if (dataItem.questionSection == "B.1") {
                  return ` <div ng-if="dataItem.value !='noAnswer'">
                        <div  ng-include src="'training-report/survey-report-section-b1.partial.html'"></div>
                    </div>`;
                }
                if (
                  (dataItem.questionSection == "B.1.1" &&
                    dataItem.value == "1") ||
                  dataItem.value == "3"
                ) {
                  return ` <div ng-if="dataItem.value !='noAnswer'">
                        <div  ng-include src="'training-report/survey-report-section-b1-1.partial.html'"></div>
                    </div>`;
                }

                return ``;
              },
              width: "50px",
            },
          ],
        };

        $scope.subQuestionB1 = {
          id: "",
          trainingReportId: "",
          e_question:
            "The reasons why you do not want to recommend this course to others are: (you can choose serveral reasons)",
          v_question:
            "Lý do khiến bạn chưa muốn giới thiệu khoá học này cho người khác là: (có thể chọn nhiều lý do)",
          surveyQuestion: "B11",
          parentQuestion: null,
          value: "1",
          section: "",
          subSection: "B.1.1",
          questionSection: "B.1.1",
          childQuestion: [
            {
              id: "",
              trainingReportId: "",
              surveyQuestion: "B111",
              value: false,
              parentQuestion: "B.1.1",
              display: true,
              questionText:
                "Nội dung khoá học không như mong đợi / The content did not meet my expectations",
            },
            {
              id: "",
              trainingReportId: "",
              surveyQuestion: "B112",
              value: false,
              parentQuestion: "B.1.1",
              display: true,
              questionText:
                "Khoá học không mang tính ứng dụng cao / The course is not practical",
            },
            {
              id: "",
              trainingReportId: "",
              surveyQuestion: "B113",
              value: false,
              parentQuestion: "B.1.1",
              display: true,
              questionText:
                "Phương pháp truyền đạt của giảng viên chưa tốt / Content delivery's method from the trainer is not good",
            },
            {
              id: "",
              trainingReportId: "",
              surveyQuestion: "B114",
              value: false,
              parentQuestion: "B.1.1",
              display: true,
              questionText:
                "Kiến thức, năng lực chuyên môn của giảng viên chưa như mong đợi / Trainer's knowledge, competence are not as expected",
            },
            {
              id: "",
              trainingReportId: "",
              surveyQuestion: "B115",
              value: false,
              parentQuestion: "B.1.1",
              questionText:
                "Học liệu sơ sài; không cung cấp học liệu / Training material is not sufficient; not provide any material",
              display: true,
            },
            {
              id: "",
              trainingReportId: "",
              surveyQuestion: "B116",
              value: false,
              parentQuestion: "B.1.1",
              questionText: "Cơ sở vật chất kém / Low quality of facilities",
              display: true,
            },
            {
              id: "",
              trainingReportId: "",
              surveyQuestion: "B117",
              value: false,
              parentQuestion: "B.1.1",
              questionText:
                "Quy trình tổ chức, ban tổ chức thiếu chuyên nghiệp / Unprofessional process, organizer",
              display: true,
            },
            {
              id: "",
              trainingReportId: "",
              surveyQuestion: "B118",
              value: false,
              parentQuestion: "B.1.1",
              questionText: "Lý do khác / Other reasons",
              display: true,
              childQuestion: [],
            },
          ],
        };
        if (!surveyQuestion.length) {
          $scope.data = dataModel;
          $scope.surveyQuestionOptions.dataSource.data = $scope.data;
          return;
        }
        dataModel = mappingDataToModel(dataModel, surveyQuestion);
        $scope.data = dataModel;
        $scope.surveyQuestionOptions.dataSource.data = $scope.data;
      }
      $scope.showProcessingStages = function () {
        $rootScope.visibleProcessingStages($translate);
      };
      async function populateCourseInfor(model) {
        if (!model) return;

        $scope.trainingInvitationId = model.trainingInvitationId;
        $scope.courseName = model.courseName;
        $scope.referenceNumber = model.referenceNumber;
        $scope.materialAttachmentDetails = model?.trainingReportAttachments
          ?.material
          ? model?.trainingReportAttachments?.material
          : [];
        $scope.oldAttachmentDetailsMaterial = model?.trainingReportAttachments
          ?.material
          ? [...model?.trainingReportAttachments?.material]
          : [];
        $scope.oldAttachmentDetailsCertificate = model
          ?.trainingReportAttachments?.certificate
          ? [...model?.trainingReportAttachments?.certificate]
          : [];
        $scope.certificateAttachmentDetails = model?.trainingReportAttachments
          ?.certificate
          ? model?.trainingReportAttachments?.certificate
          : [];
      }
      function mappingDataToModel(model, surveyQuestion) {
        if (!model || !surveyQuestion) return [];
        model = model.map((q) => {
          let cq = surveyQuestion.find(
            (item) => item.surveyQuestion == q.surveyQuestion
          );
          if (cq) {
            q.value = cq.value;
            q.id = cq.id;
            q.trainingReportId = cq.trainingReportId;
            q.surveyQuestion = cq.surveyQuestion;
            q.parentQuestion = cq.parentQuestion;
          }

          return q;
        });
        let questionB = surveyQuestion.filter(
          (q) => q.parentQuestion == "B.1.1"
        );
        if (questionB.length) {
          $scope.subQuestionB1.childQuestion =
            $scope.subQuestionB1.childQuestion.map((q) => {
              questionB.map((cq) => {
                if (q.surveyQuestion == cq.surveyQuestion) {
                  q.value = cq.value == "true" ? true : false;
                  q.id = cq.id;
                  q.trainingReportId = cq.trainingReportId;
                  q.surveyQuestion = cq.surveyQuestion;
                  q.parentQuestion = cq.parentQuestion;
                }
                return cq;
              });
              return q;
            });
          model = [...model, $scope.subQuestionB1];
        }
        return model;
      }

      function mappingDataModelQuestion() {
        let trainingSurveyQuestions = [];

        let data = $scope.data;
        let surveyQuestion = data.filter((item) => item.value !== "noAnswer");
        if (surveyQuestion.length) {
          surveyQuestion.map((q) => {
            if (q.childQuestion) {
              q.childQuestion.map((cq) => {
                surveyQuestion.push(cq);
              });
            }
          });
        }

        surveyQuestion.forEach((q) => {
          let objModel = {
            surveyQuestion: "",
            parentQuestion: "",
            value: "",
          };
          if (q.id) objModel.id = q.id;
          if (q.trainingReportId)
            objModel.trainingReportId = q.trainingReportId;
          objModel.surveyQuestion = q.surveyQuestion;
          objModel.parentQuestion = q.parentQuestion;
          objModel.value = q.value;
          trainingSurveyQuestions.push(objModel);
        });
        return trainingSurveyQuestions;
      }

      function mappingDataModelToSave() {
        if (!$scope.status) $scope.status = "Draft";
        let employeeInfo = {
          ...$scope.employeeInfo,
        };
        let trainingSurveyQuestions = mappingDataModelQuestion();
        var dataModel = {
          status: $scope.status,
          referenceNumber: $scope.referenceNumber,
          employeeInfo: employeeInfo,
          trainingInvitationId: $scope.trainingInvitationId,
          courseName: $scope.courseName,
          trainerName: $scope.surveyDataModel.trainerName,
          actualAttendingDate: $scope.surveyDataModel.actualAttendingDate,
          otherReasons: $scope.surveyDataModel.otherReasons,
          otherFeedback: $scope.surveyDataModel.otherFeedback,
          remark: $scope.surveyDataModel.remark,
          trainingReportAttachments: {
            Certificate: [...$scope.certificateAttachmentDetails],
            Material: [...$scope.materialAttachmentDetails],
          },

          trainingActionPlans: $scope.dataC,
          trainingSurveyQuestions: trainingSurveyQuestions,
        };
        if ($scope.id) {
          dataModel.id = $scope.id;
        }
        console.log(dataModel);
        return dataModel;
      }

      $scope.performRequestAction = function (actionName) {
        if (!validateComment(actionName)) return;
        let commentDialog = $("#commentWindow").data("kendoDialog");
        commentDialog.close();
        switch (actionName) {
          case "Approve":
            approve();
            break;
          case "RequestToChange":
            requestChange();
            break;
        }
      };

      function validateComment(actionName) {
        if (actionName == "Approve" && !$scope.isHODRemark) return true;
        if (!$scope.comment) {
          $scope
            .findElementByFieldName("workflowComment")
            .addClass("ng-invalid");
          $scope.commentModel = {
            isValid: false,
            errorMessage: $translate.instant("COMMON_FIELD_IS_REQUIRED"),
          };
          $scope.$apply();
          return false;
        } else {
          $scope
            .findElementByFieldName("workflowComment")
            .removeClass("ng-invalid");
          $scope.commentModel = {
            isValid: true,
            errorMessage: "",
          };
          $scope.$apply();
          return true;
        }
      }

      function showCommentDialog(actionName) {
        let title = "";
        switch (actionName) {
          case "Approve":
            title = $translate.instant("COMMON_BUTTON_APPROVE");
            break;
          case "RequestToChange":
            title = $translate.instant("COMMON_BUTTON_REQUEST_TO_CHANGE");
            break;
        }
        let commentDialog = $("#commentWindow").kendoDialog({
          buttonLayout: "normal",
          width: 600,
          visible: false,
          title: title,
          animation: {
            open: {
              effects: "fade:in",
            },
          },
          actions: [
            {
              text: $translate.instant("COMMON_BUTTON_OK"),
              action: function (e) {
                $scope.performRequestAction(actionName);
                return false;
              },
              primary: true,
            },
          ],
        });
        let boxReason = commentDialog.data("kendoDialog");
        boxReason.open();
      }

      function isFormValid() {
        $scope.clearErrors();
        let data = $scope.data;
        // if ($scope.isEditableTrainerName) {
        //   $scope.validateRequired({
        //     value: $scope.surveyDataModel.trainerName,
        //     name: "Trainer Name",
        //   });
        // }
        $scope.validateRequired({
          value: $scope.surveyDataModel.actualAttendingDate,
          name: "Actual Attending Date",
        });
        // $scope.validateRequired({
        //   value: $scope.certificateAttachmentDetails.length,
        //   name: "Attachment of Training Material",
        // });
        // $scope.validateRequired({
        //   value: $scope.materialAttachmentDetails.length,
        //   name: "Attachment of Training Certificate",
        // });

        let surveyQuestion = data.filter((item) => item.value !== "noAnswer");
        if (surveyQuestion.length) {
          surveyQuestion.map((q) => {
            $scope.validateRequired({
              value: q.value,
              name: `${q.v_question}/ ${q.e_question}`,
            });
            if (q.surveyQuestion == "B11") {
              let flag = false;
              q.childQuestion &&
                q.childQuestion.map((cq) => {
                  if (cq.value) {
                    flag = true;
                  }
                });
              if (!flag) {
                $scope.validateRequired({
                  value: "",
                  name: `Lý do khiến bạn chưa muốn giới thiệu khoá học này cho người khác là: (có thể chọn nhiều lý do) / The reasons why you do not want to recommend this course to others are: (you can choose serveral reasons)`,
                });
              }
            }
          });
        }
        $scope.dataC?.map((dc, i) => {
          $scope.validateRequired({
            value: dc.actionPlanCode,
            name: `Action plan ${i + 1}`,
          });
          if (!dc.quarter1 && !dc.quarter2 && !dc.quarter3 && !dc.quarter4) {
            $scope.validateRequired({
              value: "",
              name: `Quarter for action plan ${i + 1}`,
            });
          }
        });

        return $scope.validateForm();
      }
      async function internalSave() {
        if (!isFormValid()) {
          $window.scrollTo(0, 0);
          return;
        }
        $scope.enabledActionsButton = false;
        // watch(true);
        var model = await mappingDataModelToSave();
        var result = await TrainingReportService.save(model).$promise;
        return result;
      }
      async function save() {
        try {
          var result = await internalSave();
          $scope.enabledActionsButton = true;
          // watch(false);
          if (!result) return;
          Notification.success($translate.instant("COMMON_SAVE_SUCCESS"));
          $state.go(
            appSetting.ACADAMY_ROUTES.externalSupplierReport,
            { id: result.id },
            { reload: true }
          );
        } catch (e) {
          console.error(e);
          $scope.enabledActionsButton = true;
          // watch(false);
          Notification.error("Error System");
        }
      }

      async function submit() {
        try {
          var itemId = $scope.id;
          var saveResult = await internalSave();
          if (!saveResult) return;
          itemId = saveResult.id;
          var req = { requestId: itemId, comment: "" };
          await TrainingReportService.submit(req).$promise;
          $scope.enabledActionsButton = true;
          // watch(false);
          Notification.success("Your report has been submitted successfully.");
          $state.go(
            appSetting.ACADAMY_ROUTES.externalSupplierReport,
            { id: itemId },
            { reload: true }
          );
        } catch (e) {
          console.error(e);
          // watch(false);
          $scope.enabledActionsButton = true;
          Notification.error("Error System");
        }
      }
      async function approve() {
        try {
          var itemId = $scope.id;
          var req = { requestId: itemId, comment: $scope.comment };
          $scope.enabledActionsButton = false;
          // watch(true);

          await TrainingReportService.approve(req).$promise;
          $scope.enabledActionsButton = true;
          // $timeout(function () {
          //   // watch(false);
          // }, 10);
          Notification.success("The report has been approved successfully.");
          $state.go(
            appSetting.ACADAMY_ROUTES.externalSupplierReport,
            { id: itemId },
            { reload: true }
          );
        } catch (e) {
          console.error(e);
          $scope.enabledActionsButton = true;
          Notification.error("Error System");
        }
      }

      async function requestChange() {
        try {
          var itemId = $scope.id;
          var req = { requestId: itemId, comment: $scope.comment };
          await TrainingReportService.requestToChange(req).$promise;
          Notification.success(
            "The report has been requested to change successfully."
          );
          $state.go(
            appSetting.ACADAMY_ROUTES.externalSupplierReport,
            { id: itemId },
            { reload: true }
          );
        } catch (e) {
          console.error(e);
          Notification.error("Error System");
        }
      }
      function cancel() {
        $state.go(appSetting.ACADAMY_ROUTES.home);
      }
    }
  );
