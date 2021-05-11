/// <reference path="../libs/angular/angular.min.js" />


(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("AdminMasterCtrl", [
            "$scope", "$modal", "$rootScope", "$timeout", "AdminMasterService", "$http", "$filter", "$compile", "$q", "drcCookie",
            adminMasterCtrl
        ]);


    //Main controller function
    function adminMasterCtrl($scope, $modal, $rootScope, $timeout, AdminMasterService, $http, $filter, $compile, $q, drcCookie) {
        $scope.MenuListGrouped = [];
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes

        //$timeout(function () {
        //    $scope.collaps = drcCookie("isCollapseSideMenu");
        //}, 1000);

        /*passing ctrl from mvc controller*/
        $scope.AuthURL = function (ctrl) {
            $scope.HasPermission(ctrl);
        }

        /*switch menu view*/
        $scope.SwitchMenuView = function () {
            $rootScope.IsAjaxLoading = true;
            AdminMasterService.SwitchMenuView().then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) {
                        window.location.reload(); //reload page to take it in action
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;
            });
        };

        /*check for page permission after login*/
        $scope.HasPermission = function (ctrl) {
            //$rootScope.IsAjaxLoading = true;
            AdminMasterService.HasPermission(ctrl).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) {
                        $scope.ModuleList = result.data.DataList;
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                //$rootScope.IsAjaxLoading = false;
            });
        };

        /*Show popup*/
        $scope.ShowChangePasswordPopup = function () {
            var modalInstance = $modal.open({
                templateUrl: 'ChangePassword.html',
                controller: ModalInstanceCtrl,
                scope: $scope
            });
        };

        //View Employee Profile As PDF File
        $scope.ViewEmployeeProfile = function (id) {
            document.location.href = "../../Handler/EmployeeViewProfilePDF.ashx?EmployeeId=" + id
        };

        //$scope.sidebarCollapsed = function () {
        //    if (drcCookie("isCollapseSideMenu") == true) {
        //        drcCookie("isCollapseSideMenu", false, { expires: 30 });
        //        $scope.collaps = false;
        //    } else {
        //        drcCookie("isCollapseSideMenu", true, { expires: 30 });
        //        $scope.collaps = true;
        //    }
        //};

        $scope.activeModuleTodoList = function () {
            //$rootScope.IsAjaxLoading = true;
            AdminMasterService.activeModuleTodoList().then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        $scope.activeEmployeeModuleTodoList = result.data.DataList;
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                //$rootScope.IsAjaxLoading = false;
            });
        };
    };

    var ModalInstanceCtrl = function ($scope, $rootScope, AdminMasterService, $filter, $modalInstance) {
        $scope.Close = function () {
            $modalInstance.close();
        };

        /*change password*/
        $scope.ChangePassword = function (nPwd, cPwd) {
            if (nPwd != cPwd) {
                toastr.warning("New Password doesn't match with Confirm Password", "Password not matched");
            } else {
                $rootScope.IsAjaxLoading = true;
                AdminMasterService.ChangePassword(nPwd).then(function (result) {
                    if (result.data.IsValidUser) {
                        if (result.data.MessageType == 1) {//SUCCESS
                            toastr.success(result.data.Message, 'Success');
                            $modalInstance.close();
                        } else if (result.data.MessageType == 0) {//ERROR
                            toastr.error(result.data.Message, 'Opps, Something went wrong');
                        }
                    } else {
                        $rootScope.redirectToLogin();
                    }
                    $rootScope.IsAjaxLoading = false;
                });
            }
        };
    };
})();

