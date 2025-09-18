angular
  .module("ssg.module.academy.training-invitation.svc", [])
  .factory(
    "TrainingInvitationService",
    function ($resource, interceptorService) {
      return $resource(null, null, {
        save: {
          method: "POST",
          url: baseUrlApi + "/TrainingInvitation/Save",
          interceptor: interceptorService.getInstance().interceptor,
        },
        listMyInvitations: {
          method: "POST",
          url: baseUrlApi + "/TrainingInvitation/ListMyInvitations",
        },
        listAllInvitations: {
          method: "POST",
          url: baseUrlApi + "/TrainingInvitation/ListAllInvitations",
        },
        get: {
          method: "GET",
          url: baseUrlApi + "/TrainingInvitation/Get",
          interceptor: interceptorService.getInstance().interceptor,
        },
        getByRequest: {
          method: "GET",
          url: baseUrlApi + "/TrainingInvitation/GetByRequest",
          interceptor: interceptorService.getInstance().interceptor,
        },
        getByParticipant: {
          method: "GET",
          url: baseUrlApi + "/TrainingInvitation/GetByParticipant/:id",
          interceptor: interceptorService.getInstance().interceptor,
        },
        accept: {
          method: "POST",
          url: baseUrlApi + "/TrainingInvitation/Accept/",
          interceptor: interceptorService.getInstance().interceptor,
        },
        decline: {
          method: "POST",
          url: baseUrlApi + "/TrainingInvitation/Decline/",
          interceptor: interceptorService.getInstance().interceptor,
        },
        sendInvitation: {
          method: "POST",
          url: baseUrlApi + "/TrainingInvitation/SendInvitation",
          interceptor: interceptorService.getInstance().interceptor,
        },
        getAll: {
          method: "POST",
          url: baseUrlApi + "/TrainingInvitation/GetAll",
          interceptor: interceptorService.getInstance().interceptor,
        },
        cancelInvitation: {
          method: "POST",
          url: baseUrlApi + "/TrainingInvitation/CancelInvitation",
          interceptor: interceptorService.getInstance().interceptor,
        },
      });
    }
  );
