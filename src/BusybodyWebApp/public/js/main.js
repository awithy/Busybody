angular.module('app', ['ngRoute']);

angular.module('app').config(function($routeProvider, $locationProvider) {

    $routeProvider.when('/hosts', {templateUrl: 'app/hosts/hosts.html', controller:'mvHostsCtrl'});
    //$routeProvider.when('/hosts/:hostId', {templateUrl: 'templates/host.html', controller:'hostController'});
    //$routeProvider.when('/eventLog', {templateUrl: 'templates/eventLog.html', controller:'eventLogController'});
    //$routeProvider.when('/systemStatus', {templateUrl: 'templates/systemStatus.html', controller:'systemStatusController'});
    //$routeProvider.when('/config', {templateUrl: 'templates/config.html', controller:'configController'});
    //$routeProvider.when('/test', {templateUrl: 'templates/test.html', controller:'viewsController'});
    //$routeProvider.otherwise({redirectTo: '/hosts'});

});

