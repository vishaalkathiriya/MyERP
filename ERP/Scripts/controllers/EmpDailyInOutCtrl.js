/// <reference path="../libs/angular/angular.min.js" />

(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("EmpDailyInOutCtrl", [
           "$scope", "$rootScope", "EmpDailyInOutService", "$timeout",
           EmpDailyInOutCtrl
        ]);

    //Main controller function
    function EmpDailyInOutCtrl($scope, $rootScope, EIO, $timeout) {

        $scope.mode = {
            userIn: false,
            userOut: true
        };

        //Check initial status
        $scope.checkLoginStatusLogin = false;
        $scope.savedInDateTime = "";
        var getStatus = function () {
            $scope.checkLoginStatusLogin = true;
            EIO.checkLoginStatus().then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) {
                        var data = result.data.DataList;
                        refreshStatus(data);
                    } else {
                        toastr.error(result.data.Message, 'Something wrong while getting login status');
                    }
                }
                $scope.checkLoginStatusLogin = false;
            });
        }
        getStatus();

        function refreshStatus(data) {
            var isLogin = data.isLogin,
                inDateTime = data.inDateTime,
                loginTypeName = data.loginTypeName;

            var timerClassAndTypeItems = getTimerClassAndTypeItems(loginTypeName);
            $scope.visibleInOutItems = timerClassAndTypeItems.visibleInOutItems;
            $scope.timerCountDownClass = timerClassAndTypeItems.className;
            $scope.inOutCurrentStatusTitle = timerClassAndTypeItems.inOutCurrentStatusTitle;

            if (isLogin) {
                var mDate = new moment(inDateTime);
                var finalDate = new Date(mDate.year(), mDate.month(), mDate.date(), mDate.hour(), mDate.minute(), mDate.second(), mDate.millisecond());
                $scope.savedInDateTime = null;
                $scope.savedInDateTime = finalDate.getTime();

                $scope.mode.userIn = true;
                $scope.mode.userOut = false;
            } else {
                $scope.savedInDateTime = "";
                $scope.mode.userIn = false;
                $scope.mode.userOut = true;
            }
        }


        //get employee daily in-out working hours information
        $scope.getWorkHoursInfo = function () {
            EIO.getWorkHoursInfo().then(function (result) {
                if (result.data.IsValidUser) {
                    $scope.inoutHourInfo = result.data.DataList;
                }
                else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;
            });
        }



        //Save In/Out entries
        $scope.inOutComment = "";
        $scope.inOutType = "0";
        $scope.SaveInOut = function () {
            $rootScope.IsAjaxLoading = true;

            EIO.SaveInOut($scope.inOutComment, $scope.inOutType).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 0:Error
                        toastr.success('You are ' + ($scope.mode.userIn ? "out" : "in") + " successfully!", 'Success');
                        getStatus();
                    } else if (result.data.MessageType == 2) { // 1:Warning
                        toastr.warning(result.data.Message, 'Warning!');
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                    $rootScope.IsAjaxLoading = false;
                    $scope.inOutComment = "";
                    $scope.inOutType = "0";
                } else {
                    $rootScope.redirectToLogin();
                }
            });
        };

        //Validate InOut Drop
        $scope.validateInOutDrop = function () {
            if ($scope.inOutType && $scope.inOutType != 0) return false;
            return true;
        };

        function getTimerClassAndTypeItems(loginTypeName) {
            var visibleInOutItems = [],
                className = "",
                inOutCurrentStatusTitle = "";
            switch (loginTypeName) {
                case "CompanyOut":
                case "":
                    visibleInOutItems.push({ key: 'Company In', value: 1 });
                    className = "timer-count-down";
                    $scope.inOutType = 1;
                    inOutCurrentStatusTitle = "";
                    break;
                case "LunchBreackIn":
                    visibleInOutItems.push({ key: 'Lunch Break End', value: 4 });
                    className = "timer-count-down timer-lunch";
                    $scope.inOutType = 4;
                    inOutCurrentStatusTitle = "Lunch Break";
                    break;
                case "CompanyWorkIn":
                    visibleInOutItems.push({ key: 'Company work End', value: 6 });
                    className = "timer-count-down timer-company-work";
                    $scope.inOutType = 6;
                    inOutCurrentStatusTitle = "Company Work";
                    break;
                case "PersonalWorkIn":
                    visibleInOutItems.push({ key: 'Personal Work End', value: 8 });
                    className = "timer-count-down timer-personal-work";
                    $scope.inOutType = 8;
                    inOutCurrentStatusTitle = "Personal Work";
                    break;
                default:
                    visibleInOutItems.push({ key: 'Company Out', value: 2 });
                    visibleInOutItems.push({ key: 'Lunch Break Start', value: 3 });
                    visibleInOutItems.push({ key: 'Company Work Start', value: 5 });
                    visibleInOutItems.push({ key: 'Personal Work Start', value: 7 });
                    className = "timer-count-down";
                    $scope.inOutType = 0;
                    inOutCurrentStatusTitle = "";
                    break;
            }

            return {
                visibleInOutItems: visibleInOutItems,
                className: className,
                inOutCurrentStatusTitle: inOutCurrentStatusTitle
            };
        };

    };

})();