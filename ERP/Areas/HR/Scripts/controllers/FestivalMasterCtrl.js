/// <reference path="../libs/angular/angular.min.js" />


(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("FestivalMasterCtrl", [
            "$scope", "$rootScope", "$timeout", "FestivalService", "$http", "$filter",
            FestivalMasterCtrl
        ]);


    //Main controller function
    function FestivalMasterCtrl($scope, $rootScope, $timeout, FestivalService, $http, $filter) {

        $scope.SetFocus = function (currentTabName) {
            switch (currentTabName) {
                case 'FestivalType':
                    $rootScope.$emit('onFestivalTypeTabSelected');
                    break;
                case 'Festival':
                    $rootScope.$emit('onFestivalTabSelected');
                    break;

            }
        }
    };

})();

