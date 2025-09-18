angular
  .module("ssg.module.academy.training-reason.svc", [])
  .factory("TrainingReasonService", function ($resource) {
    return $resource(null, null, {
      get: {
        method: "GET",
        url: baseUrlApi + "/Reason/Get/:id",
      },
      save: {
        method: "POST",
        url: baseUrlApi + "/Reason/Save",
      },
      list: {
        method: "GET",
        isArray: true,
        url: baseUrlApi + "/Reason/List",
      },
      delete: {
        method: "DELETE",
        isArray: true,
        url: baseUrlApi + "/Reason/Delete",
      },
    });
  });
