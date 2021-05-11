(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("MastersMainController", [
            "$scope", "$rootScope", "$timeout", "SRTypeService", "$http", "$filter", "ngTableParams",
            MastersMainController
        ]);

    function MastersMainController($scope, $rootScope, $timeout, SRTypeService, $http, $filter, ngTableParams) {

        $scope.master = {
            TypeChanged: false,
            SubTypeChanged: false
        };

        $scope.SetFocus = function (currentTabName) {
            switch (currentTabName) {
                case 'Type':
                    $rootScope.$emit('onTypeTabSelected');
                    break;
                case 'SubType':
                    $rootScope.$emit('onSubTypeTabSelected');
                    break;
                case 'Parameter':
                    $rootScope.$emit('onParameterTabSelected');
                    break;
                case 'Parts':
                    $rootScope.$emit('onPartsTabSelected');
                    break;
            }
        }
    }
})();