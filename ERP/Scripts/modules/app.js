/// <reference path="../libs/angular/angular.min.js" />

//Declare Main and sub modules
angular.module("ERPApp", [
    "ngRoute",
    "ngSanitize",
    "ngTable",
    "ui.calendar",
    "ui.bootstrap",
    "colorpicker.module",
    "ngDateRange",
    "ngTagsInput",
    "textAngular",
    "ERPApp.Controllers",
    "ERPApp.Filters",
    "ERPApp.Services",
    "ERPApp.Directives",
    "drc.cookie",
    "duScroll",
    "ngCkeditor",
    "ngFileUpload"
]);
angular.module("ERPApp.Controllers", []);
angular.module("ERPApp.Filters", []);
angular.module("ERPApp.Services", []);
angular.module("ERPApp.Directives", []);

//Initialize variable when dom is ready
angular.module("ERPApp").run([
    "$rootScope",
    function ($rootScope) {

        $rootScope.$safeApply = function ($scope, fn) {
            fn = fn || function () { };
            if ($scope.$$phase) {
                fn();
            }
            else {
                $scope.$apply(fn);
            }
        };

        $rootScope.IsAjaxLoading = false;
        $rootScope.isFormVisible = false;

        $rootScope.keydown = function ($event) {
            var a = $event.keyCode;
            if (a == 27) {
                if ($rootScope.isFormVisible == true) {
                    $rootScope.isFormVisible = false;
                }
                $rootScope.$emit('onEscape');
            }
        }

        $rootScope.redirectToLogin = function () {
            window.location.href = "/login";
        };

        $rootScope.range = function (n) {
            return new Array(n);
        };

    }

]);


//App configuration
angular.module("ERPApp").config([
    '$httpProvider',
    function ($httpProvider) {
        $httpProvider.interceptors.push(['$q', function ($q) {
            return {
                'request': function (config) {
                    var url = window.liveAPIPath ? window.liveAPIPath + config.url : config.url;
                    config.url = url;
                    return config || $q.when(config);
                }
            }
        }]);
    }
]);