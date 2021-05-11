/// <reference path="../libs/angular/angular.js" />
angular.module("ERPApp.Directives").directive('capitalInput', function () {
    return {
        require: 'ngModel',
        link: function (scope, element, attrs, modelCtrl) {
            var capitalize = function (inputValue) {
                var inpt = inputValue || '';
                var capitalized = inpt.toUpperCase();
                if (capitalized !== inpt) {
                    modelCtrl.$setViewValue(capitalized);
                    modelCtrl.$render();
                }
                return capitalized;
            }
            modelCtrl.$parsers.push(capitalize);
            capitalize(scope[attrs.ngModel]);  // capitalize initial value
        }
    };
});