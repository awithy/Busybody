angular.module('app').controller('mvEventLogCtrl', function($rootScope, $scope, $http, $interval, mvIdentity) {
    $scope.reverseSort = false;
    var update = function () {
        if(!mvIdentity.isAuthenticated()) {
            return;
        }
        $http.get("/api/eventLog")
            .success(function(response) {
                $scope.events = response;
            });
    };
    $interval(update, 1000);
    update();
});
