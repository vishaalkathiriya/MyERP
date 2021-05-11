/// <reference path="../libs/angular/angular.min.js" />


(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("EmployeeDetailReportCtrl", [
            "$scope", "$rootScope", "$timeout", "EmployeeDetailReportService", "$http", "$filter", "ngTableParams", "$q",
            EmployeeDetailReportCtrl
        ]);


    //Main controller function
    function EmployeeDetailReportCtrl($scope, $rootScope, $timeout, EmployeeDetailReportService, $http, $filter, ngTableParams, $q) {
        $scope.editData = $scope.editData || {};
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes
        $scope.isFocusFirst = false;

        $scope.editData = {
            FirstName: "",
            MiddleName: "",
            LastName: "",
            Department: "0",
        }

        /*reset the form*/
        function ResetForm() {
            $scope.editData = {
                FirstName: "",
                MiddleName: "",
                LastName: "",
                Department: "0",
            }

            //$scope.employeedetailreportform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }

        /* getting list of Depts*/
        function loadDepts() {
            EmployeeDetailReportService.RetrieveDept().then(function (result) {
                $scope.DeptList = result.data.DataList;
            });
        };
        loadDepts();


        $scope.GetEmployeeReportData = function () {
            if (!$scope.editData.FirstName && !$scope.editData.MiddleName && !$scope.editData.LastName && $scope.editData.Department == "0")
            //{
                toastr.warning("Please,select either Department or First,Middle and Last Name of employee");
            //    return;
            //}
            //else {
                $scope.RefreshTable();
            //}
        }

        /*refresh table after */
        $scope.RefreshTable = function () {
            $scope.tableParams.reload();
        };

        /*datatable*/
        $scope.GetEmployeeReportData_Table = function () {
            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 100,
                sorting: {
                    ECode: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;
                    EmployeeDetailReportService.GetEmployeeDetailReport($scope.timeZone, params.page(), params.count(), params.orderBy(), $scope.editData, params.filter().ECode, params.filter().FullName, params.filter().Designation, params.filter().Department, params.filter().Manager, params.filter().ManCode, params.filter().ICom, params.filter().Present).then(function (result) {
                        if (result.data.IsValidUser) {
                            if (result.data.MessageType != 0) { // 0:Error
                                //display no data message
                                if (result.data.DataList.total == 0) {
                                    $scope.noRecord = true;
                                } else {
                                    $scope.noRecord = false;
                                }
                                params.total(result.data.DataList.total);
                                $scope.totalRecords = result.data.DataList.total;
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
        };

        /*reset the form*/
        $scope.ResetEmployeeReport = function () {
            //Reset ngTableParam filters
            $scope.tableParams.$params.filter = {
                ECode: "", FullName: "", Designation: "", Department: "", Manager: "", ManCode: "", ICom: "", Present: ""
            };
            
            ResetForm();
        };

        /*export to excel*/
        $scope.ExportToExcel = function () {
            document.location.href = "../../Handler/EmployeeDetailReport.ashx?timezone=" + $scope.timeZone
            + "&FirstName=" + $scope.editData.FirstName
            + "&MiddleName=" + $scope.editData.MiddleName
            + "&LastName=" + $scope.editData.LastName
            + "&Department=" + $scope.editData.Department

        };

        $scope.PresentTypes = function (column) {
            var def = $q.defer(),
            docNames = [{ id: 'No', title: 'No' }, { id: 'Yes', title: 'Yes' }];
            def.resolve(docNames);
            return def;
        };

    };

})();

