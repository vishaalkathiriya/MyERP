/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Directives")
    .directive('drcInputMask', ["$compile", "$http","$timeout", "$rootScope", function ($compile, $http, $timeout, $rootScope) {
        return {
            restrict: 'A',
            scope: true,
            link: function (scope, element, attrs) {
                element.inputmask(scope.$eval(attrs.drcInputMask));
                element.on('change', function () {
                    scope.$eval(attrs.ngModel + "='" + element.val() + "'");
                });
                
                $timeout(function () {
                    //var elPicker = $('#'+attrs.parentid +' .dtPicker').detach();
                    //$(element).after(elPicker);

                    var elPicker = $('#'+attrs.parentid +' .dtPicker');
                    elPicker.addClass('ulPicker');
                });
            }
        }
    }]);