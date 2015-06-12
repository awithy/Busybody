angular.module('app').controller('mvWelcomeCtrl', function($scope, mvIdentity) {
    $scope.identity = mvIdentity;
});