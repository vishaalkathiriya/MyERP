/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Directives")
    .directive('drcSlideUpDown', ["$compile", "$http","$timeout", "$rootScope", function ($compile, $http, $timeout, $rootScope) {
        return {
            restrict: 'A',
            scope: true,
            link: function (scope, element, attrs) {
                element.bind('click', function () {
                    var submenu_opened_class = 'opened';
                    //var submenu_options = { submenu_open_delay: 0.5, submenu_open_easing: Sine.easeInOut, submenu_opened_class: 'opened' };
                    var root_level_class = 'root-level';
                    $('#main-menu li').find('> ul').slideUp();
                    var $this = $(this);
                    //$this.addClass('has-sub opened');
                    $submenu = $this.find('> ul');

                    $($submenu).slideToggle('slow');
                });
            }
        }
    }]);