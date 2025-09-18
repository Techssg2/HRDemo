angular
  .module("ssg.module.academy.course.svc", [])
  .factory("CourseService", function ($resource) {
    return $resource(null, null, {
      get: {
        method: "GET",
        url: baseUrlApi + "/Course/Get/:id",
      },
      getAll: {
        method: "GET",
        url: baseUrlApi + "/Course/GetAll",
        isArray: true,
      },
      save: {
        method: "POST",
        url: baseUrlApi + "/Course/Save",
      },
      list: {
        method: "GET",
        isArray: true,
        url: baseUrlApi + "/Course/List",
      },
      delete: {
        method: "DELETE",
        isArray: true,
        url: baseUrlApi + "/Course/Delete",
      }

    });
  });
