/// <reference path="../libs/angular/angular.min.js" />


(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("EmpAttendanceCtrl", [
            "$scope", "$modal", "$rootScope", "$timeout", "EmpAttendanceService", "$http", "$filter", "$q",
            EmpAttendanceCtrl
        ]);


    //Main controller function
    function EmpAttendanceCtrl($scope, $modal, $rootScope, $timeout, EmpAttendanceService, $http, $filter, $q) {
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes
        $scope.IsAjaxLoadingPMS = false;
        $scope.filterYears = [];
        $scope.selectedYear = "";
        // $scope.GroupName = "";

        $scope.FillYear = function () {
            for (var i = 2015; i <= new Date().getFullYear(); i++) {
                var obj = { "Id": i.toString(), "Label": i.toString() };
                $scope.filterYears.push(obj);
            }
            var n = new Date().getFullYear();
            var obj = { "Id": n.toString(), "Label": n.toString() };
            $scope.selectedYear = n.toString();
        };
        $scope.FillYear();

        /*getting Attendance Report Data*/
        $scope.GetEmpAttendanceReport = function (GroupName, selectMonth) {
            if (!selectMonth)
                selectMonth = null;

            if (!GroupName)
                GroupName = null;

            $scope.IsAjaxLoadingPMS = true;
            if (selectMonth == null) { // If Month is not selected
                $scope.ShowYearly = true;
                $scope.ShowMonthly = false;
                EmpAttendanceService.GetEmpAttendanceReport($scope.timeZone, $scope.selectedYear, GroupName).then(function (result) {
                    if (result.data.IsValidUser) {
                        if (result.data.MessageType == 1) { // 1:Success
                            $scope.GetEmpAttendanceReportList = result.data.DataList;
                        } else {
                            toastr.error(result.data.Message, 'Opps, Something went wrong');
                        }
                    } else {
                        $rootScope.redirectToLogin();
                    }
                    $scope.IsAjaxLoadingPMS = false;
                });
            }
            else { // If month is selected
                $scope.ShowMonthly = true;
                $scope.ShowYearly = false;
                EmpAttendanceService.GetEmpAttendanceReportMonthFormat($scope.timeZone, $scope.selectedYear, GroupName, selectMonth).then(function (result) {
                    if (result.data.IsValidUser) {
                        if (result.data.MessageType == 1) { // 1:Success
                            $scope.GetEmpAttendanceReportListMonthFormat = result.data.DataList;
                        } else {
                            toastr.error(result.data.Message, 'Opps, Something went wrong');
                        }
                    } else {
                        $rootScope.redirectToLogin();
                    }
                    $scope.IsAjaxLoadingPMS = false;
                });
            }

        }
        $scope.GetEmpAttendanceReport();

        // Show Modal for monthly attendance detail of employee
        $scope.ShowDetails = function (emp, mnth) {
            var detailModelInstance = $modal.open({
                templateUrl: '../../Content/AttandanceTemplate/AttandaceDetail.html',
                controller: DetailCtrl,
                scope: $scope,
                size: 'lg',
                resolve: {
                    DetailResult: function () {
                        return EmpAttendanceService.GetEmployeeMonthDetail(emp.EmployeeId, mnth, $scope.selectedYear).then(function (result) {
                            return result;
                        });
                    }
                }
            });
        };

        var DetailCtrl = function ($scope, $rootScope, DetailResult, $modalInstance) {
            $scope.detailEmp = [];
            $scope.detailEmpHalf = [];
            var result = DetailResult.data;
            if (result.IsValidUser) {
                $scope.EmpName = result.DataList[0].EmpName;
                $scope.MonthYear = $filter('date')(result.DataList[0].Edate, 'MMM') + "-" + $filter('date')(result.DataList[0].Edate, 'yyyy');
                for (var i = 0; i < 15; i++) {
                    $scope.detailEmp.push(result.DataList[i]);
                }
                for (var i = 15; i < result.DataList.length; i++) {
                    $scope.detailEmpHalf.push(result.DataList[i]);
                }
            }

            $scope.CloseUserPopup = function () {
                $modalInstance.close();
            };
        };

        /*export to excel*/
        $scope.ExportToExcel = function (GroupName, Month) {
            if (!Month)
                Month = null;

            if (!GroupName)
                GroupName = null;

            if (Month == null) {
                document.location.href = "../../Handler/EmpAttendance.ashx?timezone=" + $scope.timeZone + "&year=" + $scope.selectedYear + "&groupName=" + GroupName
            }
            else {
                document.location.href = "../../Handler/EmpAttendanceMonthFormatHandler.ashx?timezone=" + $scope.timeZone + "&year=" + $scope.selectedYear + "&groupName=" + GroupName + "&month=" + Month
            }
        };

        // Apply TD Class of P A L O
        $scope.getTdClass = function (obj, CUREENT_MONTH) {

            if (obj.REL_MONTH != 0 || obj.REL_YEAR != 0) {
                if ((obj.JOIN_MONTH > CUREENT_MONTH && obj.JOIN_YEAR >= $scope.selectedYear) || (obj.REL_MONTH < CUREENT_MONTH && obj.REL_YEAR <= $scope.selectedYear)) {
                    return "notjoin";
                }
            }
            else {
                if (obj.JOIN_MONTH > CUREENT_MONTH && obj.JOIN_YEAR >= $scope.selectedYear) {
                    return "notjoin";
                }
            }

        }

        // Apply class of Last TD (H)
        $scope.getLastTdClass = function (obj, CUREENT_MONTH, pos) {

            if (obj.REL_MONTH != 0 || obj.REL_YEAR != 0) {
                if ((obj.JOIN_MONTH > CUREENT_MONTH && obj.JOIN_YEAR >= $scope.selectedYear) || (obj.REL_MONTH < CUREENT_MONTH && obj.REL_YEAR <= $scope.selectedYear)) {
                    return "att-sep-nj att-" + pos + "-sep";
                }
                else {
                    return "att-sep att-" + pos + "-sep";
                }
            }
            else {
                if (obj.JOIN_MONTH > CUREENT_MONTH && obj.JOIN_YEAR >= $scope.selectedYear) {
                    return "att-sep-nj att-" + pos + "-sep";
                }
                else {
                    return "att-sep att-" + pos + "-sep";
                }
            }

        }

        // Apply SPAN class of P A L O H
        $scope.getSpanClass = function (obj, CUREENT_MONTH, pos) {

            if (obj.REL_MONTH != 0 || obj.REL_YEAR != 0) {
                if ((obj.JOIN_MONTH > CUREENT_MONTH && obj.JOIN_YEAR >= $scope.selectedYear) || (obj.REL_MONTH < CUREENT_MONTH && obj.REL_YEAR <= $scope.selectedYear)) {
                    return "att att-" + pos + "-nj";
                }
                else {
                    return "att att-" + pos;
                }
            } else {
                if (obj.JOIN_MONTH > CUREENT_MONTH && obj.JOIN_YEAR >= $scope.selectedYear) {
                    return "att att-" + pos + "-nj";
                }
                else {
                    return "att att-" + pos;
                }
            }
        }


        /*********************************** IF REPORT IS IN MONTH FORMAT ***********************************/

        // Get Td Background for Row (Present)
        $scope.getTdClassPRow = function (obj) {
            var className = "";
            if (obj.P == "A") {
                className = "abClass";
            }
            else if (obj.P == "L") {
                className = "lvClass";
            }
            else if (obj.isHoliday) { className = "hlClass"; }
            else {
                switch (obj.P) {
                    case "P":
                        className = "fpClass";
                        break;
                    case "H":
                        className = "hlClass";
                        break;
                    case "0.5":
                        className = "hpClass";
                        break;
                    default:
                }
            }

            if (obj.isJoined == false && obj.isHoliday == false) {//Set Backgroug of Td when Employee not joined
                return "notjoin";
            } else {
                return className == "" ? "content-zero" : className;
            }
        }

        // Get Td Background for Row (Leave-Absence)
        $scope.getTdClassLARow = function (obj, row) {
            var className = "";
            var issun = $filter('date')(obj.Edate, 'EEE') === 'Sun';
            if (issun) {
                className = "sundayClass";
            }
            else {
                switch (row) {
                    case "L":
                        if (obj.L == 0.5) {
                            className = "lvClass";
                        }
                        break;

                    case "A":
                        if (obj.A == 0.5) {
                            className = "abClass";
                        }
                        break;
                    default:
                }
            }
            return className == "" ? "content-zero" : className;
        }

        // Get Td Background for Row (Over-Time)
        $scope.getTdClassORow = function (obj) {
            var className = "";
            var issun = $filter('date')(obj.Edate, 'EEE') === 'Sun';
            if (obj.O == 1 || obj.O == 0.5) {
                className = "otClass";
            }
            else if (issun) {
                className = "sundayClass";
            }
            return className == "" ? "content-zero" : className;
        }


        $scope.isSunday = function (date) {
            return $filter('date')(date, 'EEE') === 'Sun';
        };

        $scope.replaceZerotoEmpty = function (val) {
            return Number(val) === 0 ? "" : Number(val);
        };

    };
})();

