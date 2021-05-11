(function () {

    'use strict';

    angular.module("ERPApp.Directives")
        .directive('drcScrollTop', [
            '$window',
            function ($window) {
                return {
                    restrict: 'A',
                    scope: {
                        el: '@',
                        padd: '@'
                    },
                    link: function (scope, element, attrs) {
                        $(scope.el).hide();
                        $($window).on('scroll', function () {
                            if ($(this).scrollTop() > Number(scope.padd)) {
                                $(scope.el).show();
                            } else {
                                $(scope.el).hide();
                            }
                        });
                    }
                }
            }
        ]);


})();