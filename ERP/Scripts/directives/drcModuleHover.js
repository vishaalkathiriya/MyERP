/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Directives")
    .directive('drcModuleHover', ["$compile", "$http","$timeout", "$rootScope", function ($compile, $http, $timeout, $rootScope) {
        return {
            restrict: 'A',
            scope: true,
            link: function (scope, element, attrs) {
                var $this = $(element),
                    optionsClass = "." + attrs.drcModuleHover;
                $this.hover(function () {
                    $this.find(optionsClass).removeClass('invisible').addClass('visible');
                }, function () {
                    $this.find(optionsClass).removeClass('visible').addClass('invisible');
                });
            }
        }
    }]);