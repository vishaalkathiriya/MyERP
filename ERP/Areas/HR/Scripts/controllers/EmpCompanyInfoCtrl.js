/// <reference path="../libs/angular/angular.js" />
(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("EmpCompanyInfoCtrl", [
            "$scope", "$modal", "$rootScope", "$timeout", "EmployeeCreateService",
            empCompanyInfoCtrl]);



    //Main controller function
    function empCompanyInfoCtrl($scope, $modal, $rootScope, $timeout, EmployeeCreateService) {
        $scope.companyinfo = $scope.companyinfo || {};

        $scope.isFirstFocus = false;
        $scope.lastcompanyinfo = $scope.lastcompanyinfo || {};
        $scope.mode = "Add";
        $scope.saveText = "Save";

        // Start Validation
        $scope.validateTeamGroup = function () {
            if ($scope.companyinfo.TeamId && $scope.companyinfo.TeamId != 0) return false;
            return true;
        };
        $scope.validateReportingTo = function () {
            if ($scope.companyinfo.ReportingTo && $scope.companyinfo.ReportingTo != 0) return false;
            return true;
        };
        $scope.validateDesignation = function () {
            if ($scope.companyinfo.DesignationId && $scope.companyinfo.DesignationId != 0) return false;
            return true;
        };
        $scope.validateRole = function () {
            if ($scope.companyinfo.RoleId && $scope.companyinfo.RoleId != 0) return false;
            return true;
        };
        $scope.validateIncrementCycle = function () {
            if ($scope.companyinfo.IncrementCycle && $scope.companyinfo.IncrementCycle != 0) return false;
            return true;
        };
        $scope.validateModuleUser = function () {
            if ($scope.companyinfo.ModuleUser && $scope.companyinfo.ModuleUser != 0) return false;
            return true;
        };

        // End Validation

        /*reset the form*/
        $scope.ResetComapnyInfo = function () {
            if ($scope.companyinfo.CompanyId) {
                $scope.BindCompanyInfo();
            } else {
                $scope.clearControl();
                $scope.saveText = "Save";
            }
        };

        /* getting list of Company Info*/
        $scope.BindCompanyInfo = function () {
            if ($scope.master.EmployeeId != 0) {
                EmployeeCreateService.FetchCompanyInfo($scope.master.EmployeeId).then(function (result) {
                    $scope.GetReporting().then(function (reporting) {
                        $scope.reporting = reporting;
                    });
                    if (result.data.DataList.length != 0) {
                        $scope.EditCompanyInfo(result.data.DataList[0]);
                        $scope.lastcompanyinfo = result.data.DataList[0];
                    } else {
                        $scope.clearControl();
                    }
                });
            }
        };

        $scope.EditCompanyInfo = function (_companyinfo) {
            $scope.mode = "Edit";
            $scope.saveText = "Update";
            $timeout(function () {
                $scope.companyinfo.CompanyId = parseInt(_companyinfo.CompanyId);
                $scope.companyinfo.TeamId = parseInt(_companyinfo.TeamId);
                $scope.companyinfo.ReportingTo = parseInt(_companyinfo.ReportingTo);
                $scope.companyinfo.DesignationId = parseInt(_companyinfo.DesignationId);
                $scope.companyinfo.RoleId = parseInt(_companyinfo.RolesId);
                $scope.companyinfo.IncrementCycle = _companyinfo.IncrementCycle;
            }, 0);

            $scope.companyinfo.IsTL = _companyinfo.IsTL;
            $scope.companyinfo.IsBillable = _companyinfo.IsBillable;
            $scope.companyinfo.ModuleUser = _companyinfo.ModuleUser;
            $scope.companyinfo.IsActive = _companyinfo.IsActive;

            $scope.lastcompanyinfo = _companyinfo;
            $scope.frmCompanyInfo.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        };

        /*Clear all controls*/
        $scope.clearControl = function () {
            $scope.companyinfo = {
                CompanyId: 0,
                TeamId: 0,
                ReportingTo: 0,
                DesignationId: 0,
                RoleId: 0,
                IncrementCycle: 0,
                IsTL: false,
                IsBillable: false,
                ModuleUser: 0,
                IsActive: true
            };
            $scope.companyinfo.IncrementCycle = 0;
            $scope.companyinfo.ModuleUser = 0;

            $scope.lastcompanyinfo = {};
            $scope.frmCompanyInfo.$setPristine();
            $scope.SetFocus();
        };
        $scope.CreateUpdateCompanyInfo = function (_company) {
            _company.EmployeeId = $scope.master.EmployeeId;
            EmployeeCreateService.CreateUpdateCompanyInfo(_company).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.companyinfo.CompanyId = result.data.DataList.CompanyId;
                        $scope.SetFocus();
                        $scope.frmCompanyInfo.$setPristine();
                        $scope.lastcompanyinfo = result.data.DataList;
                        if ($scope.mode === "Add") {
                            $scope.saveText = "Update";
                        }
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

        /* Start DropDown list*/
        $scope.GetActiveDesignations = function () {
            // BEGIN GETTING LIST OF DESIGNATIONS
            EmployeeCreateService.GetActiveDesignations().then(function (result) {
                $scope.designation = result.data;
                $scope.companyinfo.DesignationId = 0;
            });
        }

        $scope.GetActiveRoles = function () {
            // BEGIN GETTING LIST OF ROLES
            EmployeeCreateService.GetActiveRoles().then(function (result) {
                $scope.roles = result.data;
                $scope.companyinfo.RoleId = 0;
            });
        }

        $scope.GetActiveTechnologiesGroup = function () {
            // BEGIN GETTING LIST OF TECH. GROUP
            EmployeeCreateService.GetActiveTechnologiesGroup().then(function (result) {
                $scope.techonologygroup = result.data;
                $scope.companyinfo.TeamId = 0;
            });
        }

        $scope.GetReporting = function () {
            // BEGIN GETTING LIST OF ROLES
            return EmployeeCreateService.GetReporting().then(function (result) {
                return result.data.DataList;
            });
        }


        $scope.GetActiveDesignations();
        $scope.GetActiveRoles();
        $scope.GetActiveTechnologiesGroup();
        $scope.GetReporting();
        $scope.companyinfo.IncrementCycle = 0;
        $scope.companyinfo.ModuleUser = 0;

        $scope.SetFocus = function () {
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }
        /* End DropDown list*/
    }
})();