angular
  .module("ssg.module.base.ctrl", [])
  .controller(
    "BaseController",
    function (
      $rootScope,
      $scope,
      $stateParams,
      $state,
      $window,
      $translate,
      appSetting,
      Notification,
      workflowService
    ) {
      $scope.id = $stateParams.id;
      $scope.formMode = $scope.id ? "Edit" : "New";
      $rootScope.isParentMenu = false;
      $rootScope.title = "Academy";
      $scope.errors = [];

      $scope.validateForm = validateForm;
      $scope.addError = addError;
      $scope.clearErrors = clearErrors;
      $scope.validateRequired = validateRequired;
      $scope.validatePhoneNumber = validatePhoneNumber;
      $scope.findElementByFieldName = findElementByFieldName;
      $scope.validateTrainingHour = validateTrainingHour;
      $scope.validateEmail = validateEmail;

      function validateForm() {
        return $scope.errors.length === 0;
      }

      function validateRequired(field) {
        if (!field) return;
        if (!field.value) {
          addError({
            fieldName: field.name,
            error:
              field.message || $translate.instant("COMMON_FIELD_IS_REQUIRED"),
          });
        }
      }

      function validatePhoneNumber(field) {
        if (!field) return;
        if (field.value.length > 0) {
          let regex = new RegExp(
            /([\+84|84|0]+(3|5|7|8|9|1[2|6|8|9]))+([0-9]{8})\b/
          );
          field.value.forEach((item) => {
            if (!regex.test(item.phoneNumber)) {
              addError({
                fieldName: field.name + " Of " + item.sapCode,
                error: "Invalid Phone Number",
              });
            }
          });
        }
      }
      function validateEmail(field) {
        if (!field) return;
        if (field.value.length > 0) {
          let regex = new RegExp(
            /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
          );
          field.value.forEach((item) => {
            if (!regex.test(item.email)) {
              addError({
                fieldName: field.name + " Of " + item.sapCode,
                error: "Invalid Email",
              });
            }
          });
        }
      }
      $scope.validateDateRange = (field) => {
        if (field.name == "Date of Submission" && !field.value)
          addError({
            fieldName: field.name,
            error: $translate.instant("COMMON_FIELD_IS_REQUIRED"),
          });
        if (!field.value) return;
        let currDate = field.currentDate
          ? new Date(field.currentDate)
          : new Date();
        currDate.setHours(0, 0, 0, 0);
        if (field.name == "Working Commitment") {
          if (moment(currDate).isAfter(moment(field.value), "day")) {
            addError({
              fieldName: field.name,
              error: field.message || "",
            });
          }
          return;
        }
        if (field.name == "Month") {
          if (moment(currDate).isAfter(moment(field.value), "month")) {
            addError({
              fieldName: field.name,
              error: field.message || "",
            });
          }
          return;
        }
        if (moment(currDate).isAfter(moment(field.value), "day")) {
          addError({
            fieldName: field.name,
            error: field.message || "",
          });
        }
      };
      function validateTrainingHour(field) {
        if (!field) return;
        if (field.value == null || field.value < 0 || field.value > 999) {
          addError({
            fieldName: field.name,
            error: "Invalid Total Training Hour Min: 0, Max 999",
          });
        }
      }
      function addError(err) {
        if (!err.fieldName) throw "field name is missing.";
        if (!err.error) throw "error is missing.";

        $scope.errors.push(err);

        findElementByFieldName(err.fieldName).addClass("ng-invalid");
      }
      function clearErrors() {
        $.each($scope.errors, function (index) {
          findElementByFieldName(this.fieldName).removeClass("ng-invalid");
        });
        $scope.errors = [];
      }
      function findElementByFieldName(fieldName) {
        var searchName = '[name="' + fieldName + '"]';
        return $(document).find(searchName);
      }
    }
  );
