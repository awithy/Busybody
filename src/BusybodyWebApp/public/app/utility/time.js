angular.module('app').filter('fromNow', function() {
    return function(date) {
        return moment(date).fromNow();
    }
});

angular.module('app').filter('longDateTime', function() {
    return function(date) {
        return moment(date).format('ddd HH:mm:ss');
    }
});
