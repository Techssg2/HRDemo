angular
  .module("ssg.module.academy.account.svc", [])
  .factory("AccountService", function ($resource) {
    return $resource(null, null, {
      IsCheckerAcademy: {
        method: "GET",
        url: baseUrlApi + "/Account/IsCheckerAcademy",
      },
      IsHODAcademy: {
        method: "GET",
        url: baseUrlApi + "/Account/IsHODAcademy",
      },
    });
  });
