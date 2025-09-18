angular
    .module('ssg.module.academy.category.svc', [])
    .factory('CategoryService', function ($resource) {
        return $resource(null, null, {
            get: {
                method: 'GET',
                url: baseUrlApi + "/Category/Get/:id"
            },
            save: {
                method: 'POST',
                url: baseUrlApi + "/Category/Save"
            },
            list: {
                method: 'GET',
                isArray: true,
                url: baseUrlApi + "/Category/List"
            },
            delete: {
                method: 'DELETE',
                isArray: true,
                url: baseUrlApi + "/Category/Delete"
            }
        });
    });