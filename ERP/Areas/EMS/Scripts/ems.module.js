; (function () {

    angular
        .module('ERPApp')
        .config(configFn);

    configFn.$inject = ['$routeProvider'];
    function configFn($routeProvider) {
        $routeProvider.when('/', {
            templateUrl: './Newsletter/Overview',
            controller: 'NewsletterOverviewController',
            controllerAs: 'vm',
            resolve: {
                newsletters: NewsletterOverview
            }
        })
        .when('/detail/new', {
            templateUrl: './Newsletter/Detail',
            controller: 'NewsletterDetailController',
            controllerAs: 'vm',
            resolve: {
                newsletter: angular.noop
            }
        })
        .when('/detail/:newsletterId', {
            templateUrl: './Newsletter/Detail',
            controller: 'NewsletterDetailController',
            controllerAs: 'vm',
            resolve: {
                newsletter: NewsletterDetail
            }
        })
        .otherwise({
            redirectTo: '/'
        });
    }

    NewsletterOverview.$inject = ['EMSService', '$http', '$q'];
    function NewsletterOverview(EMSService, $http, $q) {
        var defer = $q.defer();
        EMSService.getAllNewsletters()
            .then(function (data) {
                defer.resolve(data);
            });
        return defer.promise;
    }

    NewsletterDetail.$inject = ['EMSService', '$route', '$http', '$q'];
    function NewsletterDetail(EMSService, $route, $http, $q) {
        var newsletterId = $route.current.params.newsletterId;
        var defer = $q.defer();
        EMSService.getNewsletterById(newsletterId)
            .then(function (data) {
                defer.resolve(data);
            });
        return defer.promise;
    }

})();