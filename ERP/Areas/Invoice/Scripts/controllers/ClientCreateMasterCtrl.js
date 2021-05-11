/// <reference path="../libs/angular/angular.min.js" />


(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("ClientCreateMasterCtrl", [
            "$scope", "$rootScope", "$timeout", "ClientCreateService", "$http", "$filter",
            clientCreateMasterCtrl
        ]);


    //Main controller function
    function clientCreateMasterCtrl($scope, $rootScope, $timeout, ClientCreateService, $http, $filter) {
        $scope.master = $scope.master || {};
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes

        $scope.InitMasterDataLoad = function (id, mode) {
            //intialize variables for both mode here
            if (mode == "Add") {
                $scope.master.mode = mode;
                $scope.master.clientId = 0;
                $scope.master.isPartialTabDisabled = false;
                $scope.master.isFullTabDisabled = false;
                $scope.master.isPartialTabActive = true;
                $scope.master.isFullTabActive = false;
                $scope.master.isConfirmButtonDisabled = true;
            } else if (mode == "Edit") {
                $scope.master.clientId = id;
                $scope.master.mode = mode;
                ClientCreateService.IsClientConfirmed($scope.master.clientId).then(function (result) { //check for IsConfirm in db table
                    var isConfirmed = result.data.DataList;
                    if (isConfirmed) { //Mode = Edit + isConfirmed == true > full tab
                        $scope.master.isPartialTabActive = false;
                        $scope.master.isFullTabActive = true;
                        $scope.master.isPartialTabDisabled = true;
                        $scope.master.isFullTabDisabled = false;
                    } else {//Mode = Edit + isConfirmed == false > partial tab
                        $scope.master.isPartialTabActive = true;
                        $scope.master.isFullTabActive = false;
                        $scope.master.isPartialTabDisabled = false;
                        $scope.master.isFullTabDisabled = true;
                        $scope.master.isConfirmButtonDisabled = false;
                    }
                });
            }
        };

        $scope.ValidateDate = function (date) {
            if (date) {
                var isError = false;
                var dates = date.split('-');
                if (dates[0].search("_") > 0 || dates[1].search("_") > 0 || dates[2].search("_") > 0) {
                    isError = true;
                }
                else {
                    if (!parseInt(dates[0]) || parseInt(dates[0]) > 31) { isError = true; }
                    if (!parseInt(dates[1]) || parseInt(dates[1]) > 12) { isError = true; }
                    if (!parseInt(dates[2]) || dates[2].length != 4) { isError = true; }
                }

                if (!isError) { return true; } // date is validated
                return false; // error in validation
            }
            return true;
        };

        /* getting list of countries*/
        $scope.loadCountryDrop = function() {
            return ClientCreateService.RetrieveCountry();
        };
    };
})();

