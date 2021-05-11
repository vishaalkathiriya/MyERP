/// <reference path="../libs/angular/angular.min.js" />


(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("SRMachineReportCtrl", [
            "$scope", "$rootScope", "$timeout", "SRMachineReportService", "$http", "$filter", "ngTableParams", "$q",
            SRMachineReportCtrl
        ]);


    //Main controller function
    function SRMachineReportCtrl($scope, $rootScope, $timeout, SRMachineReportService, $http, $filter, ngTableParams, $q) {
        $scope.editData = $scope.editData || {};
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes
        $scope.isFocusFirst = false;
        $scope.filterTypes = "";
        $scope.filterSubTypes = "";
        $scope.filterParameters = "";
        //$scope.filterFloorWings = "";
        $scope.filterLocations = "";
        $scope.TotalRecords = 0;

        $scope.filterDate = {
            dateRange: { startDate: "", endDate: "" }
        };

        /*reset the form*/
        function ResetForm() {
            $scope.selectedTypeList = [];
            $scope.selectedSubTypeList = [];
            $scope.selectedParameterList = [];
            $scope.selectedLocationList = [];

            $scope.machinereportform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }

        /* getting list of types*/
        function loadMachineTypes() {
            SRMachineReportService.RetrieveSRType().then(function (result) {
                $scope.SRTypes = result.data.DataList;
            });
        };
        loadMachineTypes();

        $scope.$watch('selectedTypeList', function (newValue) {
            $scope.filterTypes = "";
            angular.forEach($scope.selectedTypeList, function (value, key) {
                $scope.filterTypes += value.Id + ",";
            });

            SRMachineReportService.RetrieveSRSubTypeList($scope.filterTypes).then(function (result) {
                $scope.SRSubTypes = result.data.DataList;
                $scope.selectedSubTypeList = $scope.SRSubTypes;
            });
        });

        $scope.$watch('selectedSubTypeList', function (newValue) {
            $scope.filterSubTypes = "";
            angular.forEach($scope.selectedSubTypeList, function (value, key) {
                $scope.filterSubTypes += value.Id + ",";
            });
            SRMachineReportService.RetrieveSRParameterList($scope.filterSubTypes).then(function (result) {
                $scope.SRParameters = result.data.DataList;
                $scope.selectedParameterList = $scope.SRParameters;
            });
        });

        $scope.$watch('selectedParameterList', function (newValue) {
            $scope.filterParameters = "";
            angular.forEach($scope.selectedParameterList, function (value, key) {
                $scope.filterParameters += value.Id + ",";
            });
        });

       

        $scope.$watch('selectedLocationList', function (newValue) {
            $scope.filterLocations = "";
            angular.forEach($scope.selectedLocationList, function (value, key) {
                $scope.filterLocations += value.Id + ",";
            });
        });

        //$scope.$watch('selectedFloorWingList', function (newValue) {
        //    $scope.filterFloorWings = "";
        //    angular.forEach($scope.selectedFloorWingList, function (value, key) {
        //        $scope.filterFloorWings += value.Id + ",";
        //    });
        //});

        ///* getting list of Floor-Wings*/
        //function loadFloorWings() {
        //    SRMachineReportService.RetrieveSRFloorWingsList().then(function (result) {
        //        $scope.SRFloorWings = result.data.DataList;
        //        $scope.selectedFloorWingList = $scope.SRFloorWings;
        //    });
        //};
        //loadFloorWings();

        /* getting list of Locations*/
        function loadLocations() {
            SRMachineReportService.RetrieveSRLocationList().then(function (result) {
                $scope.SRLocations = result.data.DataList;
                $scope.selectedLocationList = $scope.SRLocations;
            });
        };
        loadLocations();

        $scope.GetMachineReportData = function () {
            $scope.RefreshTable();
        }
        /*refresh table after */
        $scope.RefreshTable = function () {
            $scope.tableParams.reload();
        };
        $scope.GetMachineReportData_Table = function () {
            debugger;
            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,
                sorting: {
                    MachineName: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;
                    SRMachineReportService.GetMachineReportData($scope.timeZone, params.page(), params.count(), params.orderBy(), $scope.filterTypes, $scope.filterSubTypes, $scope.filterParameters, $scope.filterLocations, $scope.filterDate.dateRange.startDate, $scope.filterDate.dateRange.endDate, params.filter().MachineName, params.filter().SerialNo, params.filter().TypeName, params.filter().SubTypeName, params.filter().ParameterName, params.filter().LocationName, params.filter().ManagerName).then(function (result) {
                        if (result.data.IsValidUser) {
                            if (result.data.MessageType != 0) { // 0:Error
                                //display no data message
                                if (result.data.DataList.total == 0) {
                                    $scope.noRecord = true;
                                    $scope.TotalRecords = 0;
                                } else {
                                    $scope.noRecord = false;
                                    $scope.TotalRecords = result.data.DataList.total;
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

        /*refresh table after */
        $scope.RefreshTable = function () {
            $scope.tableParams.reload();
        };

        /*reset the form*/
        $scope.ResetSRMachineReport = function () {
            ResetForm();
        };

        /*export to excel*/
        $scope.ExportToExcel = function () {
            document.location.href = "../../Handler/SRMachineReport.ashx?timezone=" + $scope.timeZone
        };

        $scope.clearDateBox = function (e) {
            var target = $(e.target).parent().find("input[type=text]").val("");
            $scope.filterDate = {
                dateRange: { startDate: "", endDate: "" }
            };
        };

        $scope.LocationTypes = function (column) {
            var qDocs = $q.defer();
            qDocs.resolve(SRMachineReportService.RetrieveSRLocationList());
            debugger;
            var def = $q.defer(),
            docNames = [];
            qDocs.promise.then(function (result) {
                angular.forEach(result.data.DataList, function (item) {
                    docNames.push({
                        'id': item.Id,
                        'title': item.Label
                    });
                });
            });
            def.resolve(docNames);
            return def;

        };

    };

})();

