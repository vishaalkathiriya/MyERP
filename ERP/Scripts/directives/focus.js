/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Directives")
    .directive('drcFocus', ["$parse", "$timeout", function ($parse, $timeout) {
        return {
            restrict: "A",
            scope: {
                targetbool: '=drcFocus'
            },
            link: function (scope, element, attr) {
                //apply focus on element on which we are using this directive
                var focusElement = function () {
                    $timeout(function () {
                        element[0].focus();
                    }, 0);
                };
                scope.$watch('targetbool', function (newValue, oldValue) {
                    if (newValue) {
                        focusElement();
                    }
                });
            }
        };
    }]);