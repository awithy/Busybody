angular.module('app').controller('mvNavbarCtrl', function($rootScope, $scope, $location, mvIdentity) {
    $scope.identity = mvIdentity;
    $scope.showView = function(pathUrl) {
        if(!mvIdentity.isAuthenticated()) {
            return;
        }
        console.debug('showView:' + pathUrl);
        $scope.page = pathUrl.split('/')[1];
        $location.path(pathUrl);
    };
});
