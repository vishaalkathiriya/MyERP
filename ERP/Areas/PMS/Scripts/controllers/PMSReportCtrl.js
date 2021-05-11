/// <reference path="../libs/angular/angular.min.js" />
(function () {
    'use strict';
    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("PMSReportCtrl", [
            "$scope", "$modal", "$rootScope", "$timeout", "PMSReportService", "$http", "$filter", "$q", "$document", "$window",
            pmsReportCtrl
        ]);

    //Main controller function
    function pmsReportCtrl($scope, $modal, $rootScope, $timeout, PMSReportService, $http, $filter, $q, $document, $window) {
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes
        $scope.filterData = {
            LoadType: 1
        };

        $scope.filterDate = {
            dateRange: { startDate: "", endDate: "" }
        };

        //use  for read more 
        //$scope.readMoreCounter = 2;
        $scope.readMoreDate = "";
        $scope.readMoreDisable = true;
        //date default value to be null
        $scope.date1 = moment("01-01-1900", "yyyy-MM-dd");

        $scope.filterData.UserId = -1;
        $scope.filterData.ProjectId = -1;

        //filter button enable flag
        $scope.filterBtnFlag = true;
        $scope.filterEnable = function () {
            $scope.filterBtnFlag = true;
        };

        $scope.filterEnableOnProj = function (id) {
            if (id > 0) {
                $scope.filterBtnFlag = false;
            } else {
                $scope.filterBtnFlag = true;
            }
        }

        $scope.GetUserList = function () {
            $rootScope.IsAjaxLoading = true;
            PMSReportService.GetUserList().then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        $scope.UserList = result.data.DataList;
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;
            });
        };
        $scope.GetUserList();

        $scope.GetProjectList = function () {
            $rootScope.IsAjaxLoading = true;
            PMSReportService.GetProjectList().then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        $scope.ProjectList = result.data.DataList;
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;
            });
        };
        $scope.GetProjectList();


        $scope.test = function () {
            $scope.data = null;
        }

        $scope.setReadMoreCounter = function (readCounter) {
            $scope.readMoreCounter = readCounter;

        }

        $scope.$watch('filterData', function () {
            $scope.data = [];
            $scope.readMoreDisable = true;
        }, true);

        //$scope.weekListReportClear = function () {
        //    $scope.data = [];
        //};


        $scope.$watch('filterDate.dateRange', function (newVaue) {
                $scope.data = [];
                $scope.readMoreDisable = true;
        }, true);

        $scope.getNumberOFHr = function (date) {

            for (var i = 0; i < $scope.data.length; i++) {
                var resultdate = new moment($scope.data[i].workDate).format("YYYY-MM-DD");
                if (resultdate == date) {
                    return $scope.data[i].userTotalwork;
                }
            }
            return 0;
        };

        $scope.FilterReport = function (filterData) {

            $rootScope.IsAjaxLoading = true;
            $scope.filterData.filterUserProject = "";
            if (filterData.LoadType == 1) {
                $scope.filterData.filterUserProject = filterData.UserId;
                $scope.readMoreDisable = true;
            } else {
                $scope.filterData.filterUserProject = filterData.ProjectId;
                $scope.readMoreDisable = true;
            }
            $scope.data = [];
            $scope.Weeks = [];

            $scope.PMSWeekReport = [];

            PMSReportService.reportUserAndProjectwise(filterData.LoadType, filterData.filterUserProject, $scope.date1, $scope.readMoreCounter, $scope.filterDate.dateRange.startDate, $scope.filterDate.dateRange.endDate).then(function (result) {

                if (result.data.IsValidUser) {
                   
                    if (result.data.DataList.length != 0) {
                        $scope.data = result.data.DataList;
                        $scope.readMoreDisable = false;
                        if ($scope.filterDate.dateRange.startDate != "") {
                            $scope.readMoreDisable = true;

                            //var comp_start_date = new moment($scope.filterDate.dateRange.startDate, 'DD/MM/YYYY').format('YYYY-MM-DD');
                            //var comp_end_date = new moment($scope.filterDate.dateRange.endDate, 'DD/MM/YYYY').format('YYYY-MM-DD');
                            //var grant_total = 0;
                            //var week_total = 0;

                            //while (comp_start_date < comp_end_date) {
                            //    week_total = 0;
                            //    var temp = new moment(comp_start_date, 'YYYY-MM-DD').format('DD-MMM-YYYY');
                            //    var date = new Date(temp);
                            //    var day = date.getDay();
                            //    var start = new Date(date);
                            //    start.setDate(start.getDate() - day);
                            //    var end = new Date(date);
                            //    end.setDate(end.getDate() - day + 6);

                            //    var refArr = [];
                            //    //refArr.Title = new moment(start).format("YYYY-MM-DD") + " To " + new moment(end).format("YYYY-MM-DD");
                            //    refArr.Title = new moment(start).format("DD-MMM-YYYY") + " To " + new moment(end).format("DD-MMM-YYYY");
                              
                            //    var temp_Sdate = new moment(start).format("YYYY-MM-DD");
                            //    var test = new Date(temp_Sdate);

                            //    for (var i = 0; i < 7; i++) {

                            //        test.setDate(test.getDate() + 1);
                                   
                            //        var day_hr = $scope.getNumberOFHr(new moment(test).format("YYYY-MM-DD"));
                            //            refArr.push({
                            //                day: test.getDayName(),
                            //                date: new Date(test),
                            //                hr: day_hr
                            //            });

                            //            week_total = week_total + day_hr;
                            //            grant_total = grant_total + day_hr;
                            //    }
                            //    refArr.weekTotal = week_total;
                            //    refArr.grantTotal = grant_total;
                            //    $scope.Weeks.push(refArr);
                            //    end.setDate(end.getDate() + 1);
                            //    comp_start_date = new moment(end).format('YYYY-MM-DD');
                            //}

                        }
                    } else {
                        $scope.readMoreDisable = true;
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;
            });
        }

        $scope.readMoreProject = function (filterData) {
            $scope.readMoreDate = moment($scope.data[$scope.data.length - 1].workDate, "yyyy-MM-dd");
            $rootScope.IsAjaxLoading = true;
            PMSReportService.reportUserAndProjectwise(filterData.LoadType, filterData.filterUserProject, $scope.readMoreDate, $scope.readMoreCounter, $scope.filterDate.dateRange.startDate, $scope.filterDate.dateRange.endDate).then(function (result) {

                if (result.data.IsValidUser) {
                    if (result.data.DataList.length != 0) {
                        angular.forEach(result.data.DataList, function (value, key) {
                            $scope.data.push(value);
                        });
                    } else {
                        $scope.readMoreDisable = true;
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;

            });
        }



        /*Code by nik*/
        $scope.moveToSpecificTimelineArea = function (weekTitle, daysAdd) {
            var dateRanges = weekTitle.split('-');
            var duration = 1000;
            var offset = 20;
            var id = "#" + new moment(dateRanges[0]).add('days', daysAdd).format("DD-MMM-YYYY");
            var someElement = angular.element(document.getElementById(id));
            $document.scrollToElement(someElement, offset, duration);
        };

        $scope.createLinkForGoToSpecificArea = function (weekTitle, daysAdd) {
            var dateRanges = weekTitle.split('-');
            return "#" + new moment(dateRanges[0]).add('days', daysAdd).format("DD-MMM-YYYY");
        };

        $scope.getTooltipDay = function (weekTitle, daysAdd) {
            var dateRanges = weekTitle.split('-');
            return new moment(dateRanges[0]).add('days', daysAdd).format("DD-MMM-YYYY");
        };

        $scope.convertToShortDateTitle = function (weekTitle) {
            var dateRanges = weekTitle.split('-');
            return new moment(dateRanges[0]).format("DD MMM") + ' - ' + new moment(dateRanges[1]).format("DD MMM");
        };

        $scope.clearDateBox = function (e) {
            var target = $(e.target).parent().find("input[type=text]").val("");
            $scope.filterDate = {
                dateRange: { startDate: "", endDate: "" }
            };
        };
    }
})();