/// <reference path="../libs/angular/angular.min.js" />


(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("DesignationCtrl", [
            "$scope", "$rootScope", "$timeout", "DesignationService", "$http", "$filter", "ngTableParams",
            designationCtrl
        ]);


    //Main controller function
    function designationCtrl($scope, $rootScope, $timeout, DesignationService, $http, $filter, ngTableParams) {
        $scope.editData = $scope.editData || {};
        $scope.mode = "Add";
        $scope.saveText = "Save";
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes
        $scope.isFocusFirst = false;
        $scope.storage = { lastRecord: "" };

        $rootScope.$on('onDesignationGroupTabSelected', function () {
            $rootScope.isDesignationFormVisible = false;
        });
        $rootScope.$on('onDesignationParentTabSelected', function () {
            $rootScope.isDesignationFormVisible = false;
        });
        $rootScope.$on('onEscape', function () {
            $rootScope.isDesignationFormVisible = false;
        });

        /*reset the form*/
        function ResetForm() {
            $scope.editData = {
                Id: 0,
                Designation: "",
                DesignationGroupId: 0,
                DesignationParentId: 0,
                IsActive: true
            };
            $scope.desform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }

        /*validate designation group dropdown*/
        $scope.validateDropGroup = function () {
            if ($scope.editData.DesignationGroupId && $scope.editData.DesignationGroupId != 0) return false;
            return true;
        };

        /*validate designation parent dropdown*/
        $scope.validateDropParent = function () {
            if ($scope.editData.DesignationParentId && $scope.editData.DesignationParentId != 0) return false;
            return true;
        };


        /* getting list of designation group*/
        function loadDesignationGroupDrop() {
            DesignationService.RetrieveDesignationGroup().then(function (result) {
                $scope.DesignationGroup = result.data.DataList;
            });
        };
        loadDesignationGroupDrop();

        /* getting list of designation parent*/
        function loadDesignationParentDrop() {
            DesignationService.RetrieveDesignationParent().then(function (result) {
                $scope.DesignationParent = result.data.DataList;
            });
        };
        loadDesignationParentDrop();


        /*add new designation*/
        $scope.AddDesignation = function () {
            $rootScope.isDesignationFormVisible = true;
            $scope.mode = "Add";
            $scope.saveText = "Save";
            ResetForm(); //reset form
        };

        /*save designation*/
        $scope.CreateUpdateDesignation = function (doc) {
            DesignationService.CreateUpdateDesignation(doc).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.editData = {
                            Id: 0,
                            Designation: "",
                            DesignationGroupId: 0,
                            DesignationParentId: 0,
                            IsActive: true
                        };
                        $scope.RefreshTable();
                        $scope.storage.lastRecord = {};
                        $scope.isFirstFocus = false;
                        $scope.desform.$setPristine();
                        if ($scope.mode === "Edit") {
                            $rootScope.isDesignationFormVisible = false;
                            $scope.saveText = "Save";
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
        }

        /*refresh table after */
        $scope.RefreshTable = function () {
            $scope.tableParams.reload();
        };

        /*active/inactive designation group*/
        $scope.ChangeStatus = function (id, status) {
            var oper = status == true ? "InActive" : "Active";
            $rootScope.IsAjaxLoading = true;
            DesignationService.ChangeStatus(id, status).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.RefreshTable();
                    } else if (result.data.MessageType == 2) { // 2:Warning
                        toastr.warning(result.data.Message, 'Warning!');
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;
            });
        };

        /*reset the form*/
        $scope.ResetDesignation = function () {
            if ($scope.mode == "Edit") {
                $scope.editData.Id = $scope.storage.lastRecord.Id;
                $scope.editData.Designation = $scope.storage.lastRecord.DesignationName;
                $scope.editData.DesignationGroupId = $scope.storage.lastRecord.DesignationGroupId;
                $scope.editData.DesignationParentId = $scope.storage.lastRecord.DesignationParentId;
                $scope.editData.IsActive = $scope.storage.lastRecord.IsActive;
            } else { //mode == add
                ResetForm();
            }
        };

        /*cancel button click event*/
        $scope.CloseDesignation = function () {
            $scope.mode = "Add";
            ResetForm();
            $rootScope.isDesignationFormVisible = false;
        };

        /*get record for edit designation*/
        $scope.UpdateDesignation = function (_des) {
            $scope.storage.lastRecord = _des;
            $rootScope.isDesignationFormVisible = true;
            $scope.mode = "Edit";
            $scope.saveText = "Update";

            $scope.editData.Id = _des.Id;
            $scope.editData.Designation = _des.DesignationName;
            $scope.editData.DesignationGroupId = _des.DesignationGroupId;
            $scope.editData.DesignationParentId = _des.DesignationParentId;
            $scope.editData.IsActive = _des.IsActive;
            $scope.desform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        };

        /*delete designation*/
        $scope.DeleteDesignation = function (id) {
            $rootScope.IsAjaxLoading = true;
            DesignationService.DeleteDesignation(id).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) {  //1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.RefreshTable();
                    } else if (result.data.MessageType == 2) { // 2:Warning
                        toastr.warning(result.data.Message, 'Warning!');
                    } else {  // 0:Error
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;
            });
        };

        /*export to excel*/
        $scope.ExportToExcel = function () {
            document.location.href = "../../Handler/Designations.ashx?timezone=" + $scope.timeZone
        };

        /*datatable*/
        $scope.RetrieveDesignation = function () {
            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,
                sorting: {
                    DesignationName: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;
                    DesignationService.GetDesignationList($scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().DesignationName, params.filter().DesignationGroup, params.filter().DesignationParent).then(function (result) {
                        if (result.data.IsValidUser) {
                            if (result.data.MessageType != 0) { // 0:Error
                                //display no data message
                                if (result.data.DataList.total == 0) {
                                    $scope.noRecord = true;
                                } else {
                                    $scope.noRecord = false;
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
    };


})();

