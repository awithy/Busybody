angular.module('app').controller('mvHostsCtrl', function($rootScope, $scope, $http, $interval) {
    $scope.orderByField = 'Name';
    $scope.reverseSort = false;
    var update = function () {
        $http.get("/hosts")
            .success(function(response) {
                $scope.hostGroups = response;
            });
    };
    $interval(update, 1000);
    update();
});
