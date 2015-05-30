var app = angular.module('busybodyApp', ['ngRoute']);

app.controller('hostsController', function($rootScope, $scope, $http, $interval) {
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

app.controller('hostController', function($rootScope, $scope, $http, $interval, $routeParams) {
    $scope.hostId = $routeParams.hostId;
    var update = function () {
        if($scope.hostId) {
            $http.get("/hosts/" + $scope.hostId)
                .success(function(response) {
                    $scope.host = response;
            });
        }
    };
    $interval(update, 1000);
    update();
});

app.controller('eventLogController', function($rootScope, $scope, $http, $interval) {
    $scope.reverseSort = false;
    var update = function () {
        $http.get("/eventLogApi")
            .success(function(response) {
                $scope.events = response;
            });
    };
    $interval(update, 1000);
    update();
});

app.controller('systemStatusController', function($scope, $http, $interval) {
    var update = function () {
        $http.get("/systemStatusApi")
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

app.controller('configController', function($scope, $http, $interval) {
    var update = function () {
        $http.get("/config")
            .success(function(response) {
                $scope.response = response;
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

app.filter('longDateTime', function() {
  return function(date) {
    return moment(date).format('ddd HH:mm:ss');
  }
});

app.controller('viewsController', function($rootScope, $scope, $location, $routeParams) {
    $scope.showView = function(pathUrl) {
         console.debug('showView:' + pathUrl);
         $scope.page = pathUrl.split('/')[1];
         $location.path(pathUrl);
    }
});

app.config(['$routeProvider', function($routeProvider, viewsController) {
    $routeProvider.when('/hosts', {templateUrl: 'templates/hosts.html', controller:'hostsController'});
    $routeProvider.when('/hosts/:hostId', {templateUrl: 'templates/host.html', controller:'hostController'});
    $routeProvider.when('/eventLog', {templateUrl: 'templates/eventLog.html', controller:'eventLogController'});
    $routeProvider.when('/systemStatus', {templateUrl: 'templates/systemStatus.html', controller:'systemStatusController'});
    $routeProvider.when('/config', {templateUrl: 'templates/config.html', controller:'configController'});
    $routeProvider.when('/test', {templateUrl: 'templates/test.html', controller:'viewsController'});
    $routeProvider.otherwise({redirectTo: '/hosts'});
}]);
