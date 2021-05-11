
(function () {

    'use strict';

    angular.module("ERPApp.Directives")
        .directive("drcTooltip", [function () {
            return {
                restrict: "A",
                priority: 2,
                link: function (scope, element, attrs) {
                    //apply bootstrap UI tooltip on element
                    var generateTooltip = function () {
                        $(element).tooltip({
                            title: attrs.drcTooltip,
                            container: "body"
                        });
                    };
                    attrs.$observe("helpTooltip", generateTooltip);
                }
            };
        }]);


})();

