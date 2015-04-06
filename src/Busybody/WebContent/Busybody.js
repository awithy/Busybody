var app = angular.module('testApp', []);

app.controller('testController', function($scope, $http) {
    $http.get("/api/hosts")
    .success(function (response) {
        $scope.hosts = response;
    });
});