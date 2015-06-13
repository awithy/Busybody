angular.module('app').run(function(mvAuth){
    mvAuth.refreshLogin();
});