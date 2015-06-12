angular.module('app').controller('mvNavbarCtrl', function($rootScope, $scope, $location, mvNotifier) {
    $scope.showView = function(pathUrl) {
        console.debug('showView:' + pathUrl);
        $scope.page = pathUrl.split('/')[1];
        $location.path(pathUrl);
    };
});
