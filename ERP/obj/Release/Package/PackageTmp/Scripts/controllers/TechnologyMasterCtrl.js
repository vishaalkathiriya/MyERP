(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("TechnologyMasterCtrl", [
            "$scope", "$rootScope", "$timeout", "TechnologyGroupService", "$http", "$filter", "ngTableParams",
            TechnologyMasterCtrl
        ]);

    function TechnologyMasterCtrl($scope, $rootScope, $timeout, TechnologyGroupService, $http, $filter, ngTableParams) {

        $scope.SetFocus = function (currentTabName) {
            switch (currentTabName) {
                case 'TechnologyGroup':
                    $rootScope.$emit('onTechnologyGroupTabSelected');
                    break;
                case 'Technology':
                    $rootScope.$emit('onTechnologyTabSelected');
                    break;
            }
        }
    }
})();