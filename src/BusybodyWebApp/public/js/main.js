angular.module('app', ['ngRoute']);

angular.module('app').config(function($routeProvider, $locationProvider) {
    $routeProvider.when('/hosts', {templateUrl: 'app/hosts/hosts.html', controller:'mvHostsCtrl'});
    $routeProvider.when('/eventLog', {templateUrl: 'app/eventLog/eventLog.html', controller:'mvEventLogCtrl'});
    $routeProvider.when('/systemStatus', {templateUrl: 'app/systemStatus/systemStatus.html', controller:'mvSystemStatusCtrl'});
    $routeProvider.when('/hosts/:hostId', {templateUrl: 'app/hosts/host.html', controller:'mvHostCtrl'});
    $routeProvider.when('/config', {templateUrl: 'app/config/config.html', controller:'mvConfigCtrl'});
    $routeProvider.otherwise({redirectTo: '/hosts'});
});
