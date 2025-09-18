angular.module("ssg.historyModule", ["ui.router", 'underscore']).service("$history", function ($state, $rootScope, $window) {
    var history = [{ state: 'home' }];
    angular.extend(this, {
        push: function (state) {
            var newState = angular.copy(state.current);
            newState.params = state.params;
            history.push(newState);
        },
        all: function () {
            return history;
        },
        go: function (step) {

            var currentStep = $state.current.name;
            var prev = this.previous(step || -1);
            while (currentStep == prev.name) {
                prev = this.previous(--step);
            }
            $state.params = prev.params;
            $rootScope.createPageComponent(prev.name);
            return $state.go(prev.name, prev.params);
        },
        previous: function (step) {
            var length = history.length - 1;
            return history[length - Math.abs(step || 1)];
        },
        back: function () {
            console.log(history);
            this.go(-1);
        },
        resetAll: function () {
            this.history = [{ state: 'home' }]
        }
    });
});