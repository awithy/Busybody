var app = angular.module('busybodyApp', []);

app.controller('hostsController', function($scope, $http) {
    $http.get("/hosts")
    .success(function (response) {
        $scope.hosts = response;
    });
});

app.controller('eventLogApiController', function($scope, $http) {
    $http.get("/eventLogApi")
    .success(function (response) {
        $scope.events = response;
    });
});

app.filter('fromNow', function() {
  return function(date) {
    return moment(date).fromNow();
  }
});