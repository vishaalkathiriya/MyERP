angular.module("ERPApp.Filters")
    .filter('fromNow', function () {
    return function (date) {
        return moment(date).fromNow();
    }
});