(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("DesignationMasterCtrl", [
            "$scope", "$rootScope", "$timeout", "DesignationService", "$http", "$filter", "ngTableParams",
            DesignationMasterCtrl
        ]);

    function DesignationMasterCtrl($scope, $rootScope, $timeout, DesignationService, $http, $filter, ngTableParams) {

        $scope.SetFocus = function (currentTabName) {
            switch (currentTabName) {
                case 'DesignationGroup':
                    $rootScope.$emit('onDesignationGroupTabSelected');
                    break;
                case 'DesignationParent':
                    $rootScope.$emit('onDesignationParentTabSelected');
                    break;
                case 'Designation':
                    $rootScope.$emit('onDesignationTabSelected');
                    break;
            }
        }
    }
})();