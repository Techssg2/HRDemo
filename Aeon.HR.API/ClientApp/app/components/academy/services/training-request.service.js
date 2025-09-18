angular
  .module("ssg.module.academy.training-request.svc", [])
  .factory("TrainingRequestService", function ($resource, interceptorService) {
    return $resource(null, null, {
      myItems: {
        method: "GET",
        isArray: true,
        url: baseUrlApi + "/TrainingRequest/MyItems",
      },
      toDoList: {
        method: "GET",
        isArray: true,
        url: baseUrlApi + "/TrainingRequest/ToDoList",
      },
      get: {
        method: "GET",
        url: baseUrlApi + "/TrainingRequest/Get/:id",
      },
      getAllRequest: {
        method: "GET",
        url: baseUrlApi + "/TrainingRequest/List",
      },
      save: {
        method: "POST",
        url: baseUrlApi + "/TrainingRequest/Save",
      },
      submit: {
        method: "POST",
        url: baseUrlApi + "/TrainingRequest/Submit",
      },
      approve: {
        method: "POST",
        url: baseUrlApi + "/TrainingRequest/Approve",
      },
      reject: {
        method: "POST",
        url: baseUrlApi + "/TrainingRequest/Reject",
      },
      requestToChange: {
        method: "POST",
        url: baseUrlApi + "/TrainingRequest/RequestToChange",
      },
      getByDepartment: {
        method: "GET",
        isArray: true,
        url: baseUrlApi + "/TrainingRequest/GetByDepartment/",
      },
      listByDepartment: {
        method: "GET",
        url: baseUrlApi + "/TrainingRequest/ListByDepartment/",
      },
      progressingStage: {
        method: "GET",
        url: baseUrlApi + "/TrainingRequest/progressingStage/:id",
      },
      downloadDocument: {
        method: "GET",
        url: baseUrlApi + "/Attachment/DownloadDocument/",
      },
      allRequest: {
        method: "POST",
        url: baseUrlApi + "/TrainingRequest/GetAllRequest",
      },
      myRequest: {
        method: "POST",
        url: baseUrlApi + "/TrainingRequest/GetMyRequest",
      },      
      cancel: {
        method: "POST",
        url: baseUrlApi + "/TrainingRequest/Cancel",
      },    
        saveCostCenter: {
            method: "GET",
          url: baseUrlApi + "/TrainingRequest/SaveCostCenter",
      },
    });
  });
