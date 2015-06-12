angular.module('app').factory('mvAuth', function($http, mvIdentity, $q) {
    return {
        authenticateUser: function(username, password) {
            var dfd = $q.defer();
            $http.post('/api/login/dologin', { username:username, password:password }).then(function(response){
                if(response.data.success) {
                    mvIdentity.currentUser = response.data;
                    console.log(mvIdentity.isAuthenticated());
                    dfd.resolve(true);
                } else {
                    dfd.resolve(false);
                }
            })
            return dfd.promise;
        }
    }
});
