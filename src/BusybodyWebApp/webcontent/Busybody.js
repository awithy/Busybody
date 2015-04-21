﻿var app = angular.module('busybodyApp', ['ngRoute']);

app.controller('hostsController', function($scope, $http, $interval) {
    $scope.orderByField = 'Name';
    $scope.reverseSort = false;
    var update = function () {
        $http.get("/hosts")
            .success(function(response) {
                $scope.hosts = response;
            });
    };
    $interval(update, 1000);
    update();
});

app.controller('eventLogApiController', function($scope, $http, $interval) {
    var update = function () {
        $http.get("/eventLogApi")
            .success(function(response) {
                $scope.events = response;
            });
    };
    $interval(update, 1000);
    update();
});

app.controller('timeController', function($scope, $http, $interval) {
    var update = function () {
        $scope.now = moment().format('HH:mm:ss');
    };
    $interval(update, 1000);
    update();
});

app.filter('fromNow', function() {
  return function(date) {
    return moment(date).fromNow();
  }
});

app.controller('viewsController', function($rootScope, $scope, $location, $routeParams) {
    //$scope.showView = function(pathUrl) {
         $location.path("/view");
    //}
});

app.config(['$routeProvider', function($routeProvider, viewsController) {
    $routeProvider.when('/view', {templateUrl: 'views/test.html', controller:'viewsController'});
    $routeProvider.otherwise({redirectTo: '/'});
}]);
