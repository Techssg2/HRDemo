angular
  .module("ssg.module.academy.category-management.ctrl", ["kendo.directives"])
  .controller(
    "CategoryManagementController",
    function (
      $rootScope,
      $scope,
      $stateParams,
      $state,
      $window,
      $translate,
      $controller,
      appSetting,
      Notification,
      CategoryService,
      CourseService,
      TrainingRequestService,
      F2IntegrationService
    ) {
      $controller("BaseController", { $scope: $scope });

      $scope.title = $translate.instant("COURSE_MANAGEMENT");;
      $scope.categoriesDataSource = null;
      $scope.categoriesList = null;
      $scope.course = null;
      $scope.isEditCourse = null;
      this.$onInit = async function () {
        getTreeListDataSourceCategory();
        renderTreeListCategory();

        getTreeListDataSourceCourse();
        renderTreeListCourse();

        $window.scrollTo(0, 0);
      };

      function getTreeListDataSourceCategory() {
        $scope.categoriesDataSource = new kendo.data.TreeListDataSource({
          transport: {
            read: fetchDataCategory,
            update: updateDataCategory,
            create: createNewCategory,
            destroy: deleteCategory,
          },
          sort: { field: "name", dir: "asc" },
          schema: {
            model: {
              id: "categoryId",
              fields: {
                categoryId: {
                  field: "id",
                },
                name: {
                  field: "name",
                },
              },
              expanded: false,
            },
          },
        });
        }

        $scope.addCate = async function () {
            var treeList = $("#treelist").data("kendoTreeList");
            treeList.addRow(); 
        }

        $scope.addCourse = async function () {
            var treeList = $("#treelistcourse").data("kendoTreeList");
            treeList.addRow(); 
        }

      function renderTreeListCategory() {
        $("#treelist").kendoTreeList({
          dataSource: $scope.categoriesDataSource,
          toolbar: [
            {
              name: "create",
              text: "Add New Category",
            },
          ],
          search: {
            fields: ["name"],
          },
          editable: true,
          selectable: true,
          change: selectCategory,
          edit: async function (e) {
            $(".k-grid-update").addClass(
              "btn btn-sm default blue-stripe no-icon"
            );
            $(".k-grid-cancel").addClass("btn btn-sm default no-icon");
          },
          columns: [
            { field: "name", title: "Category List" },
            {
              title: $translate.instant("COMMON_ACTION"),
              command: [
                {
                  name: "edit",
                  text: " ",
                  className: "btn btn-sm default blue-stripe",
                },
                {
                  name: "delete",
                  text: " ",
                  className: "btn btn-sm default red-stripe ",
                  click: deleteCategoryConfirm
                  }, {
                      name: "createchild",
                      text: $translate.instant("COMMON_SUBCATEGORY"),
                      className:
                          "add-child btn btn-sm default green-stripe  ",
                  },
              ],
            },
          ],
          messages: {
            commands: {
              update: "Save",
            },
          },
        });
        renderCategorySearch();
      }

      function getTreeListDataSourceCourse() {
        $scope.course = new kendo.data.TreeListDataSource({
          transport: {
            read: fetchDataCourse,
            update: updateDataCourse,
            create: addNewCourse,
            destroy: deleteCourse,
          },
          sort: { field: "name", dir: "asc" },
          schema: {
            model: {
              id: "courseId",
              fields: {
                courseId: { field: "id" },
                name: {
                  field: "name",
                  type: "string",
                  validation: {
                    required: true,
                  },
                },
                categoryId: {
                  field: "categoryId",
                  type: "string",
                  validation: {
                    required: true,
                  },
                },
                categoryName: {
                  field: "categoryName",
                  type: "string",
                },
                code: {
                  field: "code",
                  type: "string",
                  validation: {
                    required: true,
                    noSpecialChar: validationNoSpecialChar,
                    noMoreThanThree: validationNoMoreThanThree,

                  },
                },
                type: {
                  field: "type",
                  type: "string",
                  validation: {
                    required: true,
                    custom: showHideServiceProvider,
                  },
                },
                serviceProvider: {
                  field: "serviceProvider",
                  type: "string",
                },
                serviceProviderCode: {
                  field: "serviceProviderCode",
                  type: "string",
                },
                description: { field: "description" },
                duration: {
                  field: "duration",
                  type: "number",
                  validation: {
                    required: true,
                    moreThanZero: validationMoreThanZero,
                    onlyNumber: validationOnlyNumber,
                    format: "{0:d}"
                  },
                },
                courseDocuments: { field: "courseDocuments" },
                image: {
                  field: "image",
                  type: "string",
                },
              },
              expanded: false,
            },
          },
        });
      }

      function renderTreeListCourse() {
        var keyUpdateTranslate = {
          CourseName: $translate.instant("COURSE_NAME"),
          CourseCode: $translate.instant("COURSE_CODE"),
          SupplierName: $translate.instant("SUPPLIER_NAME"),
          Duration: $translate.instant("DURATION"),
          CourseType: $translate.instant("COURSE_TYPE"),
          Category: $translate.instant("CATEGORY"),
          Description: $translate.instant("DESCRIPTION"),
        }

        $("#treelistcourse").kendoTreeList({
          dataSource: $scope.course,
          toolbar: [
            {
              name: "create",
              text: "Add New Course",
            },
          ],
          search: {
            fields: ["name"],
          },
          editable: {
            mode: "popup",
            template: kendo.template($("#edit-course-form").html())(keyUpdateTranslate),
          },
          edit: courseEditorTemplate,
          columns: [
            { field: "name", title: $translate.instant("COURSE_NAME") },
            { field: "categoryName", title: $translate.instant("CATEGORY") },
            {
              field: $translate.instant("COURSE_CODE"),
              title: $translate.instant("COURSE_CODE"),
              template: function (dataItem) {
                return dataItem.code?.toUpperCase();
              },
            },
            { field: "type", title: $translate.instant("COURSE_TYPE") },
            { field: "serviceProvider", title: $translate.instant("SUPPLIER_NAME") },
            { field: "duration", title: $translate.instant("DURATION") },
            {
              title:  $translate.instant("COMMON_ACTION"),
              command: [
                {
                  name: "edit",
                  text: " ",
                  className: "btn btn-sm default blue-stripe",
                },
                {
                  name: "delete",
                  text: " ",
                  className: "btn btn-sm default red-stripe ",
                  click: deleteCourseConfirm
                },
              ],
            },
            ,
          ],
          messages: {
            commands: {
              update: "Save",
            },
          },
        });

        renderCourseSearch();
      }

      function renderCategorySearch() {
        var searchInput = `<span class="k-textbox k-grid-search k-display-flex">
        <input autocomplete="off" placeholder="Search..." title="Search..." class="k-input" id="search-category">
        <span class="k-input-icon">
            <span class="k-icon k-i-search"></span>
        </span>`;

        $(".k-grid-toolbar").append(searchInput);
        $("#search-category").on("change paste keyup", (e) => {
          var valueSearch = e.target.value;
          $("#treelist").data("kendoTreeList").dataSource.filter({
            field: "name",
            operator: "startswith",
            value: valueSearch,
          });
        });

        
      }

      var windowCategory = $("#windowCategory").kendoWindow({
        title: "To delete a Category, You have to delete all courses belong to that Category?",
        draggable: false,
        visible: false, //the window will not appear before its .open method is called
        modal: true,
        width: "555px",
        height: "100px",
      }).data("kendoWindow");

      var windowCourse = $("#windowCourse").kendoWindow({
        title: "Are you sure you want to delete this Course?",
        draggable: false,
        visible: false, //the window will not appear before its .open method is called
        modal: true,
        width: "350px",
        height: "100px",
      }).data("kendoWindow");
      
      function deleteCategoryConfirm(e) {
        e.preventDefault();
        var categoryDataSource =
          $("#treelist").data("kendoTreeList").dataSource;
          var tr = $(e.target).closest("tr");
          var data = this.dataItem(tr);
          var windowTemplateCategory = kendo.template($("#confirmTemplateCategory").html());
          windowCategory.content(windowTemplateCategory(data));
          windowCategory.center().open();
          if (data.hasChildren) {
            $("#yesButtonCat").prop('disabled', true);
            $("#deleteCategoryText").text('Can not delete Category with SubCategory!')
          }
          $("#yesButtonCat").click(function(){
            categoryDataSource.remove(data)  
            categoryDataSource.sync()
            windowCategory.close();
        })
        $("#noButtonCat").click(function(){
          windowCategory.close();
        })
      }

      function deleteCourseConfirm(e) {
        e.preventDefault();
        var courseDataSource =
          $("#treelistcourse").data("kendoTreeList").dataSource;
          var tr = $(e.target).closest("tr");
          var data = this.dataItem(tr);
          var windowTemplateCourse = kendo.template($("#confirmTemplateCourse").html());
          windowCourse.content(windowTemplateCourse(data));
          windowCourse.center().open();
 
          $("#yesButton").click(function(){
            courseDataSource.remove(data)  
            courseDataSource.sync()
            windowCourse.close();
        })
        $("#noButton").click(function(){
          windowCourse.close();
        })
      }

      function dowloadAttachment(fileRef) {
        let url = baseUrlApi + "/Attachment/DownloadDocument?filePath=";
        const dowloadLink = document.createElement("a");
        dowloadLink.href = url + fileRef;
        dowloadLink.click();
      }

      function openAttachment(linkView) {
        const iframeDocument = $("<iframe>", {
          src: linkView,
          id: "iframe-document",
        });
        iframeDocument
          .kendoWindow({
            draggable: false,
            title: "Document",
            visible: false,
            actions: ["Close"],
            width: "70vw",
            height: "70%",
          })
          .data("kendoWindow")
          .center()
          .open();
          iframeDocument.closest("div").addClass("have-close-button");
      }

      function deleteAttachment(fileName, course, idDoc) {
        var courseDataSource =
          $("#treelistcourse").data("kendoTreeList").dataSource;
        var data = courseDataSource.data();
        var editIndex = 0;
        if ($scope.isEditCourse) {
          data.forEach((row, index) => {
            if (row.id === $scope.isEditCourse.id) {
              editIndex = index;
            }
          });
        }

        course.documents.forEach((document) => {
          if (document.fileName === fileName) {
            document.state = "Deleted";
          }
        });

        data[editIndex].documents = course.documents;
        data[editIndex].dirty = true;
        $(`#${idDoc}`).hide();
        // console.log("origin: ", data[editIndex], " using: ", course);
      }

      function renderDocumentList(course) {
        course.documents.forEach((document, index) => {
          const { fileName, fileRef, linkView } = document;

          const containerList = $("<div>", {
            id: `${course.id}_${index}`,
          });
          // const documentName = $("<p>");
          // documentName.html(fileName);
          const downloadButton = $("<a>");
          downloadButton
            .html(fileName)
            .addClass("document-command file-name")
            .click(() => dowloadAttachment(fileRef));
          const openButton = $("<a>");
          openButton
            .html("Open")
            .addClass("document-command")
            .click(() => openAttachment(linkView));
          const deleteButton = $("<a>");
          deleteButton
            .html("Delete")
            .addClass("document-command")
            .click(() =>
              deleteAttachment(fileName, course, `${course.id}_${index}`)
            );
          // containerList.append(documentName);
          containerList.append(downloadButton);
          containerList.append(openButton);
          containerList.append(deleteButton);
          $("#DocumentList").append(containerList);
        });
      }

      async function courseEditorTemplate(e) {

        if (!e.model.isNew()) {
          if (!e.model.serviceProvider && !e.model.serviceProviderCode) {
            e.model.serviceProvider = "";
            e.model.serviceProviderCode = "";
          }
          var suppliersMapper = [];
          e.container.data("kendoWindow").title("Edit Course");
          e.container.data("kendoWindow")._draggable(false);
          try {
            $rootScope.isLoading = true;
            var course = await CourseService.get({
              id: e.model.id,
            }).$promise;
            var resultSuppliers = await F2IntegrationService.getSuppliers().$promise;
  
            if (course.$resolved && resultSuppliers.$resolved) {
              suppliersMapper = JSON.parse(JSON.stringify(resultSuppliers)).map(
                (option) => {
                  return {
                    nameSupplier: option.name,
                    codeSupplier: option.code
                  };
                }
              ).filter(result => result.nameSupplier && result.codeSupplier);
              $rootScope.isLoading = false;
            }
          } catch (e) {
            suppliersMapper = [
              {
                nameSupplier: "CÔNG TY TNHH SHINRYO VIỆT NAM",
                codeSupplier: "300089"
              },
              {
                nameSupplier: "CÔNG TY TNHH TƯ VẤN GIẢI PHÁP VIỆT NAM",
                codeSupplier: "100145"
              }
            ]
            $rootScope.isLoading = false;
          }
          
          if (course.documents?.length > 0) {
            renderDocumentList(course);
          }
        } else {
          
          try {
            var resultSuppliers = await F2IntegrationService.getSuppliers().$promise;
            $rootScope.isLoading = true;
            if (resultSuppliers.$resolved) {
              suppliersMapper = JSON.parse(JSON.stringify(resultSuppliers)).map(
                (option) => {
                  return {
                    nameSupplier: option.name,
                    codeSupplier: option.code
                  };
                }
              ).filter(result => result.nameSupplier && result.codeSupplier);
              $rootScope.isLoading = false;
            }
          } catch (e) {
            suppliersMapper = [
              {
                nameSupplier: "CÔNG TY TNHH SHINRYO VIỆT NAM",
                codeSupplier: "300089"
              },
              {
                nameSupplier: "CÔNG TY TNHH TƯ VẤN GIẢI PHÁP VIỆT NAM",
                codeSupplier: "100145"
              }
            ]
            $rootScope.isLoading = false;
          }
          
          e.container.data("kendoWindow").title("Add New Course");
          e.container.data("kendoWindow").draggable = false;
        }

        $scope.isEditCourse = e.model.id ? e.model : null;

        var categories = await CategoryService.list().$promise;
        var categoryMapper = JSON.parse(JSON.stringify(categories)).map(
          (option) => {
            return {
              name: option.name,
              categoryId: option.id,
              category: {
                id: option.id,
                name: option.name,
              },
            };
          }
        );
 
        
        var categoryList = new kendo.data.DataSource({
          data: categoryMapper,
        });
        var typeofCourse = new kendo.data.DataSource({
          data: [
            { name: "External Supplier", value: "External" },
            { name: "Internal Supplier", value: "Internal" },
          ],
        });
        var typeOfSupplier = new kendo.data.DataSource({
          data: suppliersMapper,
        });
        $(".k-grid-update").addClass("btn btn-sm default blue-stripe no-icon");
        $(".k-grid-cancel").addClass("btn btn-sm default no-icon");

        e.model.typeofCourse = typeofCourse;
        e.model.categoryList = categoryList;
        e.model.typeOfSupplier = typeOfSupplier;

        var editForm = $(
          ".k-popup-edit-form[data-uid=" + e.container.data("uid") + "]"
        );

        editForm.attr("id", "CoursePopup");

        if (e.model.type === "Internal") {
          $("#CourseServiceProvider").hide();
          $('#CourseServiceProviderInput').removeAttr('required');
        }

        if (e.model.image) {
          var courseAvatar = $("#CourseAvatar");
          var imageContainer = $("#remove-image");
          courseAvatar.attr("src", e.model.image);
          var removeBtn = $("<input/>").attr({
            class: "btn btn-sm default red-stripe",
            type: "button",
            id: "removeBtn",
            value: "Remove Course Image",
          });

          imageContainer.append(removeBtn);
          $("#removeBtn").on("click", removeImage);
        }

        $("#CourseImage").kendoUpload({
          async: {
            saveUrl: "save",
            removeUrl: "remove",
            autoUpload: false,
          },
          multiple: false,
          validation: {
            allowedExtensions: [".gif", ".jpg", ".png"],
          },
          select: function (e) {
            var fileInfo = e.files[0];
            var wrapper = this.wrapper;

            setTimeout(function () {
              addPreview(fileInfo, wrapper, $scope.isEditCourse);
            });
          },
        });

        $("#DocumentUpload").kendoUpload({
          async: {
            saveUrl: "save",
            removeUrl: "remove",
            autoUpload: false,
          },
          multiple: true,
          validation: {
            allowedExtensions: [".docx", ".xlsx", ".pdf", ".gif", ".jpg", ".png"],
          },
          select: function (e) {
            // var fileInfo = e.files[0];
            var wrapper = this.wrapper;

            setTimeout(function () {
              addPreviewDocumentUpload(e.files, wrapper, $scope.isEditCourse);
            });
          },
        });
        // if ($(".k-dropzone-hint")) {
        //   $(".k-dropzone-hint").text("Max 4Mb");
        // }
        kendo.bind(editForm, e.model);
      }

      function renderCourseSearch() {
        var searchInput = `<span class="k-textbox k-grid-search k-display-flex">
        <input autocomplete="off" placeholder="Search..." title="Search..." class="k-input" id="search-course">
        <span class="k-input-icon">
            <span class="k-icon k-i-search"></span>
        </span>`;

        $("#treelistcourse").find(".k-grid-toolbar").append(searchInput);
        $("#search-course").on("change paste keyup", (e) => {
          var valueSearch = e.target.value;
          $("#treelistcourse").data("kendoTreeList").dataSource.filter({
            field: "name",
            operator: "startswith",
            value: valueSearch,
          });
        });
      }

      function removeImage() {
        var courseDataSource =
          $("#treelistcourse").data("kendoTreeList").dataSource;
        var courseImageContainer = $(".course-image-container");
        var data = courseDataSource.data();
        var editIndex = 0;
        if ($scope.isEditCourse) {
          data.forEach((row, index) => {
            if (row.id === $scope.isEditCourse.id) {
              editIndex = index;
            }
          });
        }
        courseImageContainer.hide();
        data[editIndex].image = null;
        data[editIndex].dirty = true;
      }

      function addPreviewDocumentUpload(file, wrapper, model) {
        var courseDataSource =
          $("#treelistcourse").data("kendoTreeList").dataSource;
        var data = courseDataSource.data();
        var editIndex = 0;
        // var docArr = [];
        if (model) {
          data.forEach((row, index) => {
            if (row.id === model.id) {
              editIndex = index;
            }
          });
        }
        data[editIndex].documents =
          data[editIndex].documents?.length > 0
            ? data[editIndex].documents
            : [];
        data[editIndex].dirty = true;
        if (file?.length > 0) {
          file.forEach((doc) => {
            var fileName = doc.name;
            var reader = new FileReader();

            if (doc) {
              reader.onload = async function () {
                data[editIndex].documents.push({
                  file: this.result,
                  fileName: fileName,
                  state: "Added",
                });
              };

              reader.readAsDataURL(doc.rawFile);
            }
          });

          // data[editIndex].documentsName = fileName;
        }
      }

      function addPreview(file, wrapper, model) {
        var raw = file.rawFile;
        var reader = new FileReader();

        if (raw) {
          reader.onloadend = function () {
            var courseDataSource =
              $("#treelistcourse").data("kendoTreeList").dataSource;
            var data = courseDataSource.data();
            var editIndex = 0;
            if (model) {
              data.forEach((row, index) => {
                if (row.id === model.id) {
                  editIndex = index;
                }
              });
            }
            data[editIndex].image = this.result;
            data[editIndex].dirty = true;
          };
          reader.readAsDataURL(raw);
        }
      }

      async function fetchDataCourse(options) {
        var courseMapper = [];
        if ($scope.selectedCategoryId) {
          var course = await CourseService.list({
            categoryId: $scope.selectedCategoryId,
          }).$promise;
          courseMapper = JSON.parse(JSON.stringify(course)).map((item) => {
            item.parentId = null;
            return item;
          });
        } else {
          var course = await CourseService.getAll().$promise;
          courseMapper = JSON.parse(JSON.stringify(course)).map((item) => {
            item.parentId = null;
            return item;
          });
        }

        options.success(courseMapper);
      }

      async function updateDataCourse(options) {
        try {
          var categoryNameList = options.data.categoryList.options.data;
          var supplierNameList = options.data.typeOfSupplier.options.data;
          var chooseCategory = categoryNameList?.filter((category) => {
            return category.categoryId === options.data.categoryId;
          });
          var chooseSupplier = supplierNameList?.filter((supplier) => {
            return supplier.codeSupplier === options.data.serviceProviderCode;
          });
          var payload = {
            id: options.data.id,
            CategoryId: options.data.categoryId,
            CategoryName: chooseCategory[0].name,
            Name: options.data.name,
            Code: options.data.code,
            Type: options.data.type,
            ServiceProviderCode: options.data.serviceProviderCode,
            ServiceProvider: chooseSupplier ? chooseSupplier[0]?.nameSupplier : null,
            Description: options.data.description,
            Duration: options.data.duration,
            image: options.data.image,
            documents: options.data.documents,
            documentsName: options.data.documentsName,
          };
          if (payload.Type === "Internal") {
            payload.ServiceProviderCode = null;
            payload.ServiceProvider = null;
          }
          $rootScope.isLoading = true;
          var result = await CourseService.save(payload).$promise;
          if (result.$resolved) {
            $rootScope.isLoading = false;
            $("#treelistcourse").data("kendoTreeList").dataSource.read();
          }
          $("#treelist").data("kendoTreeList").dataSource.read();
          Notification.success($translate.instant("COMMON_SAVE_SUCCESS"));
          options.success();
        } catch (e) {
          $rootScope.isLoading = false;
          let nameCourse = options.data.name;
          if (e.data === "Name already exists") {
            Notification.error(`Input ${nameCourse} is duplicated`);
          } else {
            Notification.error("Error System");
          }
        }
      }

      async function addNewCourse(options) {
        try {
          var categoryNameList = options.data.categoryList.options.data;
          var supplierNameList = options.data.typeOfSupplier.options.data;
          var chooseCategory = categoryNameList?.filter((category) => {
            return category.categoryId === options.data.categoryId;
          });
          var chooseSupplier = supplierNameList?.filter((supplier) => {
            return supplier.codeSupplier === options.data.serviceProviderCode;
          });
          var payload = {
            id: options.data.id,
            CategoryId: options.data.categoryId,
            CategoryName: chooseCategory[0].name,
            Name: options.data.name,
            Code: options.data.code,
            Type: options.data.type,
            ServiceProviderCode: options.data.serviceProviderCode,
            ServiceProvider: chooseSupplier ? chooseSupplier[0]?.nameSupplier : null,
            Description: options.data.description,
            Duration: options.data.duration,
            image: options.data.image,
            documents: options.data.documents,
            documentsName: options.data.documentsName,
          };

          if (!payload.id) {
            delete payload.id;
          }

          if (payload.Type === "Internal") {
            payload.ServiceProviderCode = null;
            payload.ServiceProvider = null;
          }
          $rootScope.isLoading = true;
          let saveProcess = await CourseService.save(payload).$promise;
          if (saveProcess.$resolved) {
            $rootScope.isLoading = false;
            $("#treelistcourse").data("kendoTreeList").dataSource.read();
          }
          $("#treelist").data("kendoTreeList").dataSource.read();
          Notification.success($translate.instant("COMMON_SAVE_SUCCESS"));
          options.success();
        } catch (e) {
          $rootScope.isLoading = false;
          let nameCourse = options.data.name;
          if (e.data === "Name already exists") {
            Notification.error(`Input ${nameCourse} is duplicated`);
          } else {
            Notification.error("Error System");
          }
        }
      }

      async function deleteCourse(options) {
        try {
          $rootScope.isLoading = true;
          let saveProcess = await CourseService.delete(options.data).$promise;
          if (saveProcess.$resolved) {
            $rootScope.isLoading = false;
            $("#treelistcourse").data("kendoTreeList").dataSource.read();
          }
          $("#treelist").data("kendoTreeList").dataSource.read();
          Notification.success($translate.instant("COMMON_DELETE_SUCCESS"));
          options.success();
        } catch (e) {
          $rootScope.isLoading = false;
          if (e.data === "Can not delete") {
            Notification.error("This course is in used. Cannot be deleted");
          } else {
            Notification.error("Error System");
          }
          $("#treelistcourse").data("kendoTreeList").dataSource.read();
        }
      }

      function showHideServiceProvider() {
        var value = $("#CourseType").val();
        if (value === "Internal") {
          $("#CourseServiceProvider").hide();
          $('#CourseServiceProviderInput').removeAttr('required');
        } else {
          $("#CourseServiceProvider").show();
          $('#CourseServiceProviderInput').attr('required', 'required');
        }
        return true;
      }

      function validationNoMoreThanThree(e) {

        if (e.is("[name='CourseCode']") && e.val() != "") {
          e.attr(
            "data-noMoreThanThree-msg",
            "Course Code is not More Than 3 Characters!"
          );
          return e.val().length <= 3;
        }
        return true;
      }

      function validationNoSpecialChar(e) {
        var value = e.val();
        var regex = /^[a-zA-Z0-9- ]*$/;
        if (e.is("[name='CourseCode']") && e.val() != "") {
          e.attr(
            "data-noSpecialChar-msg",
            "Course Code is not allowed to have Special Characters!"
          );
          return regex.test(value);
        }
        return true;
      }

      function validationOnlyNumber(e) {
        var value = e.val();
        var regex = /^[0-9]*$/;

        if (e.is("[name='CourseDuration']") && e.val() != "") {
          e.attr(
            "data-onlyNumber-msg",
            "Course Duration allowed only number"
          );
          return regex.test(value);
        }
        return true;
      }

      function validationMoreThanZero(e) {
        var value = e.val();
        if (e.is("[name='CourseDuration']") && e.val() != "") {
          e.attr(
            "data-moreThanZero-msg",
            "Course Duration should be at least one day"
          );
          return +value > 0;
        }
        return true;
      }

      async function fetchDataCategory(options) {
        var categories = await CategoryService.list().$promise;
        $scope.categoriesList = JSON.parse(JSON.stringify(categories));
        options.success($scope.categoriesList);
      }

      async function updateDataCategory(options) {
        try {
          await CategoryService.save(options.data).$promise;
          Notification.success($translate.instant("COMMON_SAVE_SUCCESS"));
          options.success();
          $("#treelist").data("kendoTreeList").dataSource.read();
        } catch (e) {
          let nameCategory = options.data.name;
          console.log(e);
          if (e.data === "Name already exists") {
            Notification.error(`Input ${nameCategory} is duplicated`);
          } else {
            Notification.error("Error System");
          }
        }
      }

      async function createNewCategory(options) {
        try {
          if (!options.data.id) {
            delete options.data.id;
          }
          await CategoryService.save(options.data).$promise;
          $("#treelist").data("kendoTreeList").dataSource.read();
          options.success();
        } catch (e){
          let nameCategory = options.data.name;
          console.log(e);
          if (e.data === "Name already exists") {
            Notification.error(`Input ${nameCategory} is duplicated`);
          } else {
            Notification.error("Error System");
          }
        }
      }

      async function deleteCategory(options) {
        try {
          await CategoryService.delete(options.data).$promise;
          $("#treelist").data("kendoTreeList").dataSource.read();
          Notification.success($translate.instant("COMMON_DELETE_SUCCESS"));
          options.success();
        } catch (e) {
          if (e.data === "Can not delete") {
            Notification.error("All courses in Category should be removed before delete");
          } else {
            Notification.error("Error System");
          }
          $("#treelist").data("kendoTreeList").dataSource.read();
        }
      }

      function selectCategory() {
        let $selectedItem = this.select(),
          dataItem1 = this.dataItem($selectedItem);
        $scope.selectedCategoryId = dataItem1.categoryId;
        $("#treelistcourse").data("kendoTreeList").dataSource.read();
        $("#treelistcourse").data("kendoTreeList").refresh();
      }

    }
  );
