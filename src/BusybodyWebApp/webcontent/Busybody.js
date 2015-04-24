var app = angular.module('busybodyApp', ['ngRoute']);

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
    $scope.showView = function(pathUrl) {
         console.debug('showView:' + pathUrl);
         $scope.page = pathUrl.split('/')[1];
         $location.path(pathUrl);
    }
    $scope.page = "hosts";
});

app.config(['$routeProvider', function($routeProvider, viewsController) {
    $routeProvider.when('/hosts', {templateUrl: 'templates/hosts.html', controller:'hostsController'});
    $routeProvider.when('/eventLog', {templateUrl: 'templates/eventLog.html', controller:'viewsController'});
    $routeProvider.when('/test', {templateUrl: 'templates/test.html', controller:'viewsController'});
    $routeProvider.otherwise({redirectTo: '/hosts'});
}]);
