angular.module('app').controller('mvNavbarLoginCtrl', function($scope, $http, $location, mvIdentity, mvAuth, mvNotifier){
    $scope.identity = mvIdentity;
    $scope.user = {};
    $scope.user.username = "";
    $scope.user.password = "";

    $scope.update = function(user) {
        mvAuth.authenticateUser(user.username, user.password).then(function(success){
            if(success) {
                mvNotifier.notify('Logged in!')
            }
            else {
                mvNotifier.error('Not logged in');
            }
        })
    };

    $scope.signout = function() {
        mvAuth.logoutUser().then(function () {
            $scope.username = "";
            $scope.password = "";
            mvNotifier.notify('You have been logged out!');
            $location.path('/');
        })
    }

});