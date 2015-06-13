angular.module('app').controller('mvConfigCtrl', function($scope, $http, $interval, mvIdentity) {
    var update = function () {
        if(!mvIdentity.isAuthenticated()) {
            return;
        }
        $http.get("/api/config")
            .success(function(response) {
                $scope.response = response;
            });
    };
    $interval(update, 1000);
    update();
});
