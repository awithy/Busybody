var app = angular.module('testApp', []);

app.controller('testController', function($scope, $http) {
    $http.get("/hosts")
    .success(function (response) {
        $scope.hosts = response;
    });
});

angular.module('testApp').filter('fromNow', function() {
  return function(date) {
    return moment(date).fromNow();
  }
});