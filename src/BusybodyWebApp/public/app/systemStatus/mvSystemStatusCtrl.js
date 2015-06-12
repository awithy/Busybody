angular.module('app').controller('mvSystemStatusCtrl', function($scope, $http, $interval, mvIdentity) {
    var update = function () {
        if(!mvIdentity.isAuthenticated()) {
            return;
        }
        $http.get("/api/systemStatus")
            .success(function(response) {
                $scope.startTime = moment(response.StartTime).format('DD MMM YYYY HH:mm:ss');
                $scope.lastUpdate = moment(response.LastUpdate).fromNow();
                $scope.uptime = response.Uptime;
                $scope.systemHealth = response.SystemHealth;
                $scope.usedMemory = response.UsedMemory;
                $scope.cpu = response.Cpu;
                $scope.roleServices = response.RoleServices;
            });
    };
    $interval(update, 1000);
    update();
});
