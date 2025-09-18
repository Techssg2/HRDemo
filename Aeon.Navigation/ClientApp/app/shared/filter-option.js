var app = angular.module("ssg.filterModule", []);
app.filter('roleFilter', function (appSetting) {
    return function (values, element) {
        values.forEach(element => {
            var item = _.find(appSetting.permission, x => {
                return x.code === element;
            })
            if (item) {
                return item.name;
            }
        });
    }
});
app.filter('targetPlanFilter', function (appSetting) {
    return function (target) {
        return target == appSetting.targets.Target1 ? 'Target 1' : 'Target 2';
    };
})
