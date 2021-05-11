/// <reference path="../libs/angular/angular.min.js" />


(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("EmployeeListCtrl", [
            "$scope", "$rootScope", "$timeout", "EmployeeCreateService", "$http", "$filter", "ngTableParams",
            employeeListCtrl
        ]);


    //Main controller function
    function employeeListCtrl($scope, $rootScope, $timeout, EmployeeCreateService, $http, $filter, ngTableParams) {
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes

        /*refresh table after */
        $scope.RefreshTable = function () {
            $scope.tableParams.reload();
        };

        /*active/inactive employee*/
        $scope.ChangeStatus = function (id, status) {
            var oper = status == true ? "InActive" : "Active";
            $rootScope.IsAjaxLoading = true;
            EmployeeCreateService.ChangeStatus(id, status).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.RefreshTable();
                    } else if (result.data.MessageType == 2) { // 2:Warning
                        toastr.warning(result.data.Message, 'Warning!');
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;
            });
        };

        /*delete employee*/
        $scope.DeleteEmployee = function (id) {
            $rootScope.IsAjaxLoading = true;
            EmployeeCreateService.DeleteEmployee(id).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType != 0) { // 0:Error
                        toastr.success(result.data.Message, 'Success');
                        $scope.RefreshTable();
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;
            });
        };

        /*export to excel*/
        $scope.ExportToExcel = function () {
            document.location.href = "../../Handler/EmployeeList.ashx?timezone=" + $scope.timeZone
        };

        /*datatable*/
        $scope.RetrieveEmployee = function () {
            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,
                sorting: {
                    EmployeeRegisterCode: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;
                    EmployeeCreateService.GetEmployeeList($scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter()).then(function (result) {
                        if (result.data.IsValidUser) {
                            if (result.data.MessageType != 0) { // 0:Error
                                //display no data message
                                if (result.data.DataList.total == 0) {
                                    $scope.noRecord = true;
                                } else {
                                    $scope.noRecord = false;
                                }

                                params.total(result.data.DataList.total);
                                $defer.resolve($scope.documents = result.data.DataList.result);
                            } else {
                                toastr.error(result.data.Message, 'Opps, Something went wrong');
                            }
                        } else {
                            $rootScope.redirectToLogin();
                        }
                        $rootScope.IsAjaxLoading = false;
                    });
                }
            });
        }

    };
   
})();

