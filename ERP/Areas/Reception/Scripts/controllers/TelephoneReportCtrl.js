/// <reference path="../libs/angular/angular.min.js" />


(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("TelephoneReportCtrl", [
            "$scope", "$rootScope", "$timeout", "TelephoneReportService", "$http", "$filter", "ngTableParams", "$q",
            TelephoneReportCtrl
        ])
    //.directive('enterAsTab', function () {
    //    return {
    //        restrict: 'A',
    //        link: function ($scope, elem, attrs) {
    //            debugger;
    //            elem.bind('keydown', function (e) {
    //                debugger;
    //                var code = e.keyCode || e.which;
    //                if (code === 13) {
    //                    e.preventDefault();
    //                    elem.next().focus();
    //                }
    //            });
    //        }
    //    }
    //});


    //Main controller function
    function TelephoneReportCtrl($scope, $rootScope, $timeout, TelephoneReportService, $http, $filter, ngTableParams, $q) {
        $scope.master = $scope.master || {};
        $scope.editData = $scope.editData || {};
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes
        $scope.isFirstFocus = true;


        function setDefaultDate() {
            var myDate = new Date();
            var defDate = (myDate.getDate()) + '/' + (myDate.getMonth() + 1) + '/' +
                    myDate.getFullYear();
            $scope.defaultDate = defDate;
        }
        setDefaultDate();

        $scope.filterDate = {
            dateRange: { startDate: $scope.defaultDate, endDate: $scope.defaultDate }
        };

        $scope.editData = {
            ExtType: "0",
            ExtNo: "",
            OutNo: ""
        }

        $scope.filterDateInter = {
            dateRange: { startDate: $scope.defaultDate, endDate: $scope.defaultDate }
        };

        $scope.interData = {
            FromExt: "",
            ToExt: ""
        }

        /*reset the form*/
        function ResetFormOuter() {
            $scope.editData = {
                ExtType: "0",
                ExtNo: "",
                OutNo: ""
            }

            $('#txtdate').val($scope.defaultDate);
            setDefaultDate();
            $scope.filterDate = {
                dateRange: { startDate: $scope.defaultDate, endDate: $scope.defaultDate }
            };

            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }

        /*reset the form*/
        function ResetFormInter() {
            $scope.interData = {
                FromExt: "",
                ToExt: ""
            }

            $('#txtDateI').val($scope.defaultDate);
            setDefaultDate();
            $scope.filterDateInter = {
                dateRange: { startDate: $scope.defaultDate, endDate: $scope.defaultDate }
            };

            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }

        /* getting list of ext types*/
        function loadExtTypes() {
            TelephoneReportService.RetrieveExtType().then(function (result) {
                $scope.ExtTypes = result.data.DataList;
            });
        };
        loadExtTypes();

        $scope.$watch('master.isInterTabActive', function () {
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        })

        $scope.GetTelephoneReportData = function () {
            $scope.RefreshTable();
        }

        /*refresh table after */
        $scope.RefreshTable = function () {
            if ($scope.master.isOuterTabActive == true) {
                $scope.editData.startDate = $scope.filterDate.dateRange.startDate;
                $scope.editData.endDate = $scope.filterDate.dateRange.endDate;
                $scope.tableParamsOut.reload();
            }
            else if ($scope.master.isInterTabActive == true) {
                $scope.interData.startDate = $scope.filterDateInter.dateRange.startDate;
                $scope.interData.endDate = $scope.filterDateInter.dateRange.endDate;
                $scope.tableParamsInter.reload();
            }

        };

        $scope.lastSort = '-pdate';

        /*datatable*/
        $scope.GetTelephoneReportData_Out = function () {
            setDefaultDate();
            $scope.editData.startDate = !$scope.filterDate.dateRange.startDate ? $scope.defaultDate : $scope.filterDate.dateRange.startDate;
            $scope.editData.endDate = !$scope.filterDate.dateRange.endDate ? $scope.defaultDate : $scope.filterDate.dateRange.endDate;

            $scope.tableParamsOut = new ngTableParams({
                page: 1,
                count: 100,
                sorting: {
                    pdate: 'desc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {

                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;
                    TelephoneReportService.GetTelephoneReportOuter($scope.timeZone, params.page(), params.count(), params.orderBy(), $scope.editData, params.filter().Type, params.filter().Outline, params.filter().Duration, params.filter().ExtNo, params.filter().OutNo).then(function (result) {
                        
                        //Reset Paging when sorting
                        if ($scope.lastSort != params.orderBy())
                            $scope.tableParamsOut.$params.page = 1;

                        $scope.lastSort = params.orderBy().toString();
                        //End Reset Paging when sorting

                        if (result.data.IsValidUser) {
                            if (result.data.MessageType != 0) { // 0:Error
                                //display no data message
                                if (result.data.DataList.total == 0) {
                                    $scope.noRecord = true;
                                } else {
                                    $scope.noRecord = false;
                                }
                                params.total(result.data.DataList.total);
                                if (result.data.DataList.total>=1000)
                                    $scope.totalOutRecords = "More than 1000";
                                else
                                    $scope.totalOutRecords = result.data.DataList.total;
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

        /*datatable*/
        $scope.GetTelephoneReportData_Inter = function () {
            setDefaultDate();
            $scope.interData.startDate = !$scope.filterDateInter.dateRange.startDate ? $scope.defaultDate : $scope.filterDateInter.dateRange.startDate;
            $scope.interData.endDate = !$scope.filterDateInter.dateRange.endDate ? $scope.defaultDate : $scope.filterDateInter.dateRange.endDate;
            $scope.tableParamsInter = new ngTableParams({
                page: 1,
                count: 100,
                sorting: {
                    pdate: 'desc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;
                    TelephoneReportService.GetTelephoneReportInter($scope.timeZone, params.page(), params.count(), params.orderBy(), $scope.interData, params.filter().Line, params.filter().FromExt, params.filter().ToExt, params.filter().Duration).then(function (result) {

                        //Reset Paging when sorting
                        if ($scope.lastSort != params.orderBy())
                            $scope.tableParamsInter.$params.page = 1;

                        $scope.lastSort = params.orderBy().toString();
                        //End Reset Paging when sorting

                        if (result.data.IsValidUser) {
                            if (result.data.MessageType != 0) { // 0:Error
                                //display no data message
                                if (result.data.DataList.total == 0) {
                                    $scope.noRecord = true;
                                } else {
                                    $scope.noRecord = false;
                                }
                                params.total(result.data.DataList.total);
                                if (result.data.DataList.total >= 1000)
                                    $scope.totalInterRecords = "More than 1000";
                                else
                                    $scope.totalInterRecords = result.data.DataList.total;

                               // $scope.totalInterRecords = result.data.DataList.total;
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
        $scope.ResetTelephoneReport = function () {
            if ($scope.master.isOuterTabActive == true)
                ResetFormOuter();
            else if ($scope.master.isInterTabActive == true)
                ResetFormInter();
        };

        /*export to excel*/
        $scope.ExportToExcel = function () {
            if ($scope.master.isOuterTabActive == true) {
                setDefaultDate();
                $scope.editData.startDate = !$scope.filterDate.dateRange.startDate ? $scope.defaultDate : $scope.filterDate.dateRange.startDate;
                $scope.editData.endDate = !$scope.filterDate.dateRange.endDate ? $scope.defaultDate : $scope.filterDate.dateRange.endDate;

                document.location.href = "../../Handler/TelephoneReport.ashx?timezone=" + $scope.timeZone
            + "&ExtType=" + $scope.editData.ExtType
            + "&ExtNo=" + $scope.editData.ExtNo
            + "&OutNo=" + $scope.editData.OutNo
            + "&StartDate=" + $scope.editData.startDate
            + "&EndDate=" + $scope.editData.endDate
            }
            else if ($scope.master.isInterTabActive == true) {
                setDefaultDate();
                $scope.interData.startDate = !$scope.filterDateInter.dateRange.startDate ? $scope.defaultDate : $scope.filterDateInter.dateRange.startDate;
                $scope.interData.endDate = !$scope.filterDateInter.dateRange.endDate ? $scope.defaultDate : $scope.filterDateInter.dateRange.endDate;

                document.location.href = "../../Handler/TelephoneReportIntercom.ashx?timezone=" + $scope.timeZone
            + "&ExtType=" + $scope.editData.FromExt
            + "&ExtNo=" + $scope.editData.ToExt
            + "&StartDate=" + $scope.interData.startDate
            + "&EndDate=" + $scope.interData.endDate
            }

        };

        $scope.clearDateBox = function (e) {
            var target = $(e.target).parent().find("input[type=text]").val("");
            $scope.filterDate = {
                dateRange: { startDate: "", endDate: "" }
            };
        };

        $scope.clearDateBoxInter = function (e) {
            var target = $(e.target).parent().find("input[type=text]").val("");
            $scope.filterDateInter = {
                dateRange: { startDate: "", endDate: "" }
            };
        };

    };

})();

