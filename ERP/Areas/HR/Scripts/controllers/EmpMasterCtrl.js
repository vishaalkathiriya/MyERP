/// <reference path="../libs/angular/angular.min.js" />


(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("EmpMasterCtrl", [
            "$scope", "$rootScope", "$timeout", "EmployeeCreateService", "$http", "$filter",
            empMasterCtrl
        ]);


    //Main controller function
    function empMasterCtrl($scope, $rootScope, $timeout, EmployeeCreateService, $http, $filter) {

        $scope.master = {
            EmployeeId: 0,
            Mode: "Add",
            EmployeeName: "Employee"
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
    };

})();

