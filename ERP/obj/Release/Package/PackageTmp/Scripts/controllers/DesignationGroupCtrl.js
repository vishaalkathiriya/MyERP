/// <reference path="../libs/angular/angular.min.js" />


(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("DesignationGroupCtrl", [
            "$scope", "$rootScope", "$timeout", "DesignationGroupService", "$http", "$filter", "ngTableParams",
            designationGroupCtrl
        ]);


    //Main controller function
    function designationGroupCtrl($scope, $rootScope, $timeout, DesignationGroupService, $http, $filter, ngTableParams) {
        $scope.editData = $scope.editData || {};
        $scope.mode = "Add";
        $scope.saveText = "Save";
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes
        $scope.isFocusFirst = false;
        $scope.storage = { lastRecord: "" };

        $rootScope.$on('onDesignationParentTabSelected', function () {
            $rootScope.isDesignationGroupFormVisible = false;
        });
        $rootScope.$on('onDesignationTabSelected', function () {
            $rootScope.isDesignationGroupFormVisible = false;
        });
        $rootScope.$on('onEscape', function () {
            $rootScope.isDesignationGroupFormVisible = false;
        });

        /*reset the form*/
        function ResetForm() {
            $scope.editData = {
                Id:0,
                DesignationGroup: "",
                IsActive: true
            };
            $scope.desform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }

        /*add new designation group*/
        $scope.AddDesignationGroup = function () {
            $rootScope.isDesignationGroupFormVisible = true;
            $scope.mode = "Add";
            $scope.saveText = "Save";
            ResetForm(); //reset form
        };

        /*save designation group*/
        $scope.CreateUpdateDesignationGroup = function (doc) {
            DesignationGroupService.CreateUpdateDesignationGroup(doc).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.editData = {
                            Id: 0,
                            DesignationGroup: "",
                            IsActive: true
                        };
                        $scope.RefreshTable();
                        $scope.storage.lastRecord = {};
                        $scope.isFirstFocus = false;
                        $scope.desform.$setPristine();
                        if ($scope.mode === "Edit") {
                            $rootScope.isDesignationGroupFormVisible = false;
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
            DesignationGroupService.ChangeStatus(id, status).then(function (result) {
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
        $scope.ResetDesignationGroup = function () {
            if ($scope.mode == "Edit") {
                $scope.editData.Id = $scope.storage.lastRecord.Id;
                $scope.editData.DesignationGroup = $scope.storage.lastRecord.DesignationGroup;
                $scope.editData.IsActive = $scope.storage.lastRecord.IsActive;
            } else { //mode == add
                ResetForm();
            }
        };

        /*cancel button click event*/
        $scope.CloseDesignationGroup = function () {
            $scope.mode = "Add";
            ResetForm();
            $rootScope.isDesignationGroupFormVisible = false;
        };

        /*get record for edit designation group*/
        $scope.UpdateDesignationGroup = function (_des) {
            $scope.storage.lastRecord = _des;
            $rootScope.isDesignationGroupFormVisible = true;
            $scope.mode = "Edit";
            $scope.saveText = "Update";

            $scope.editData.Id = _des.Id;
            $scope.editData.DesignationGroup = _des.DesignationGroup;
            $scope.editData.IsActive = _des.IsActive;
            $scope.desform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        };

        /*delete designation group*/
        $scope.DeleteDesignationGroup = function (id) {
            $rootScope.IsAjaxLoading = true;
            DesignationGroupService.DeleteDesignationGroup(id).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 0) { // 0:Error
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                    else if (result.data.MessageType == 2) { // 2:Warning
                        toastr.warning(result.data.Message, 'This record is in use');
                    }
                    else {
                        toastr.success(result.data.Message, 'Success');
                        $scope.RefreshTable();
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;
            });
        };

        /*export to excel*/
        $scope.ExportToExcel = function () {
            document.location.href = "../../Handler/DesignationGroup.ashx?timezone=" + $scope.timeZone
        };

        /*datatable*/
        $scope.RetrieveDesignationGroup = function () {
            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,
                sorting: {
                    DesignationGroup: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;
                    DesignationGroupService.GetDesignationGroupList($scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().DesignationGroup).then(function (result) {
                        if (result.data.IsValidUser) {
                            if (result.data.MessageType != 0) { // 0:Error
                                //display no data message
                                if (result.data.DataList.total == 0) {
                                    $scope.noRecord = true;
                                    $scope.filterText = params.filter().DesignationGroup;
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

