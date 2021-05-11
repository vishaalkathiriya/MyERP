/// <reference path="../libs/angular/angular.min.js" />


(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("EmpLoginInfoCtrl", [
            "$scope", "$rootScope", "$timeout", "EmployeeCreateService", "$http", "$filter",
            empLoginInfoCtrl
        ]);


    //Main controller function
    function empLoginInfoCtrl($scope, $rootScope, $timeout, EmployeeCreateService, $http, $filter) {
        $scope.login = $scope.login || {};
        $scope.saveText = "Save";
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes
        $scope.isFocusFirst = false;
        $scope.storage = { lastRecord: "" };
        $scope.login.IsActive = true;

        /*save login information*/
        $scope.CreateUpdateEmpLogin = function (q) {
            $scope.login.EmployeeId = $scope.master.EmployeeId;
            EmployeeCreateService.CreateUpdateEmpLogin(q).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.saveText = "Update";
                        $scope.login.LoginInfoId = result.data.DataList;
                        SetFocus();
                        $scope.empLform.$setPristine();
                    } else if (result.data.MessageType == 2) { // 2:Warning
                        toastr.warning(result.data.Message, 'Record already exists');
                    }
                    else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;
            });
        };

        /*fetch payroll information to display in form*/
        $scope.FetchLoginInfo = function () {
            EmployeeCreateService.FetchLoginInfo($scope.master.EmployeeId).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 0) { // 0:Error
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    } else if (result.data.MessageType == 1) { //1:Success
                        if (result.data.DataList.length > 0) {
                            $scope.saveText = "Update";
                            $scope.storage.lastRecord = result.data.DataList[0];
                            BindData(result.data.DataList[0]);
                        }
                    }
                }
                else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;
            });
        };


        /*reset the form*/
        $scope.ResetLoginInfo = function () {
            if ($scope.storage.lastRecord) {
                BindData($scope.storage.lastRecord);
            } else {
                ResetForm();
            }
        };

        function ResetForm() {
            $scope.login = {
                LoginInfoId: 0,
                UserName: "",
                Password: "",
                PasswordExpiresDays: "",
                IsRemoteLogin: false,
                IsPermit: false,
                IsActive: true
            };

            $scope.empLform.$setPristine();
            SetFocus();
        }


        function BindData(p) {
            $scope.login = {
                LoginInfoId: p.LoginInfoId,
                UserName: p.UserName,
                Password: p.Password,
                PasswordExpiresDays: p.PasswordExpiresDays,
                IsRemoteLogin: p.IsRemoteLogin,
                IsPermit: p.IsPermit,
                IsActive: p.IsActive
            };
            SetFocus();
        }

        function SetFocus() {
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }

    };


})();

