angular.module('app').controller('mvHostCtrl', function($rootScope, $scope, $http, $interval, $routeParams, mvIdentity) {
    $scope.hostId = $routeParams.hostId;
    var update = function () {
        if(!mvIdentity.isAuthenticated()) {
            return;
        }
        if($scope.hostId) {
            $http.get("api/hosts/" + $scope.hostId)
                .success(function(response) {
                    $scope.host = response;
                });
        }
    };
    $interval(update, 1000);
    update();
});
