/// <reference path="../libs/angular/angular.min.js" />

(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("DashboardCtrl", [
            "$scope", "$rootScope", "$timeout", "DashboardService", "$http", "$filter", "$compile", "$q", "EmpAttendanceService", "$modal", dashboardCtrl
        ]);

    //Main controller function
    function dashboardCtrl($scope, $rootScope, $timeout, DashboardService, $http, $filter, $compile, $q, EmpAttendanceService, $modal) {
        /*SECTION START - CALENDAR*/
        $scope.events = [];
        $scope.leaves = [];
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes
        $scope.filterYear = new Date().getFullYear();

        var date = new Date();
        var d = date.getDate();
        var m = date.getMonth();
        var y = date.getFullYear();

        $scope.isTl = false;
        $scope.isTeamLeader = function (isTl) {
            $scope.isTl = isTl;
        };

        $scope.$watch('filterYear', function (newValue) {
            $scope.getEmpAttendanceReport(newValue);
        });

        $scope.getEmpAttendanceReport = function (tempYear) {
            $scope.IsAjaxLoadingPMS = true;
            DashboardService.GetEmpAttendanceReport(tempYear).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        $scope.DisplayPandingLeave = result.data.DataList;
                        $scope.GetEmpAttendanceReportList = (_.sortBy(result.data.DataList, 'monthNumber')).reverse();
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                $scope.IsAjaxLoadingPMS = false;
            });

        };

        $scope.ShowDetails = function (emp, mnth, filterYear) {
            var curDate = new Date();
            var detailModelInstance = $modal.open({
                templateUrl: 'Content/AttandanceTemplate/AttandaceDetail.html',
                controller: DetailCtrl,
                scope: $scope,
                size: 'lg',
                resolve: {
                    DetailResult: function () {
                        return EmpAttendanceService.GetEmployeeMonthDetail(emp, mnth, filterYear).then(function (result) {
                            return result;
                        });
                    }
                }
            });
        };

        $scope.isSunday = function (date) {
            return $filter('date')(date, 'EEE') === 'Sun';
        };

        $scope.replaceZerotoEmpty = function (val) {
            return Number(val) === 0 ? "" : Number(val);
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
                for (var j = 15; j < result.DataList.length; j++) {
                    $scope.detailEmpHalf.push(result.DataList[j]);
                }
            }

            $scope.CloseUserPopup = function () {
                $modalInstance.close();
            };
        };

        /*SECTION START - Reminders*/
        $scope.InfoListGrouped = [];
        var LoadReminders = function () {
            $rootScope.IsAjaxLoading = true;
            DashboardService.GetInformationList().then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) {
                        $scope.reminders = GetRemindersList(result.data.DataList);

                        var sortedHolidayList = [];
                        var j = _.groupBy($scope.reminders.holidays, 'FestivalGroupId');
                        for (var groupIn in j) {
                            var temp = j[groupIn];
                            if (temp.length > 1) {
                                temp[0].lastDate = temp[temp.length - 1].OnDate;
                                temp[0].lastDateWeekName = temp[temp.length - 1].OnDateWeekName;
                                temp[0].EmployeeId = 0;
                                sortedHolidayList.push(temp[0]);
                            } else {
                                temp[0].EmployeeId = 0;
                                sortedHolidayList.push(temp[0]);
                            }
                        }
                        $scope.reminders.holidays = sortedHolidayList;
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;
            });
        };
        LoadReminders();
        /*SECTION END - Reminders*/
    }

    function GetRemindersList(data) {
        if (!data) return [];
        var reminders = {
            pendingLeaves: [], approveLeaves: [], ekadashies: [], holidays: [], anniversaries: [], birthDates: [], DisApproved: []
        };
        for (var i = 0, len = data.length; i < len; i++) {
            var currentReminderRec = data[i];
            switch (currentReminderRec.Type) {
                case "Approved Leave":
                    reminders.approveLeaves.push(currentReminderRec);
                    break;
                case "Pending Leave":
                    reminders.pendingLeaves.push(currentReminderRec);
                    break;
                case "DisApproved Leave":
                    reminders.DisApproved.push(currentReminderRec);
                    break;
                case "Ekadashi":
                    reminders.ekadashies.push(currentReminderRec);
                    break;
                case "Holiday":
                    reminders.holidays.push(currentReminderRec);
                    break;
                case "Anniversary":
                    reminders.anniversaries.push(currentReminderRec);
                    break;
                case "Birthdate":
                    reminders.birthDates.push(currentReminderRec);
                    break;
            }
        }
        return reminders;
    }

})();

