/// <reference path="../libs/angular/angular.min.js" />


(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("DesignationParentCtrl", [
            "$scope", "$rootScope", "$timeout", "DesignationParentService", "$http", "$filter", "ngTableParams",
            designationParentCtrl
        ]);


    //Main controller function
    function designationParentCtrl($scope, $rootScope, $timeout, DesignationParentService, $http, $filter, ngTableParams) {
        $scope.editData = $scope.editData || {};
        $scope.mode = "Add";
        $scope.saveText = "Save";
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes
        $scope.isFocusFirst = false;
        $scope.storage = { lastRecord: "" };

        $rootScope.$on('onDesignationGroupTabSelected', function () {
            $rootScope.isDesignationParentFormVisible = false;
        });
        $rootScope.$on('onDesignationTabSelected', function () {
            $rootScope.isDesignationParentFormVisible = false;
        });
        $rootScope.$on('onEscape', function () {
            $rootScope.isDesignationParentFormVisible = false;
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

        /*add new designation parent*/
        $scope.AddDesignationParent = function () {
            $rootScope.isDesignationParentFormVisible = true;
            $scope.mode = "Add";
            $scope.saveText = "Save";
            ResetForm(); //reset form
        };

        /*save designation parent*/
        $scope.CreateUpdateDesignationParent = function (doc) {
            DesignationParentService.CreateUpdateDesignationParent(doc).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.editData = {
                            Id: 0,
                            DesignationParent: "",
                            IsActive: true
                        };
                        $scope.RefreshTable();
                        $scope.storage.lastRecord = {};
                        $scope.isFirstFocus = false;
                        $scope.desform.$setPristine();
                        if ($scope.mode === "Edit") {
                            $rootScope.isDesignationParentFormVisible = false;
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
            DesignationParentService.ChangeStatus(id, status).then(function (result) {
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
        $scope.ResetDesignationParent = function () {
            if ($scope.mode == "Edit") {
                $scope.editData.Id = $scope.storage.lastRecord.Id;
                $scope.editData.DesignationParent = $scope.storage.lastRecord.DesignationParent;
                $scope.editData.IsActive = $scope.storage.lastRecord.IsActive;
            } else { //mode == add
                ResetForm();
            }
        };

        /*cancel button click event*/
        $scope.CloseDesignationParent = function () {
            $scope.mode = "Add";
            ResetForm();
            $rootScope.isDesignationParentFormVisible = false;
        };

        /*get record for edit designation parent*/
        $scope.UpdateDesignationParent = function (_des) {
            $scope.storage.lastRecord = _des;
            $rootScope.isDesignationParentFormVisible = true;
            $scope.mode = "Edit";
            $scope.saveText = "Update";

            $scope.editData.Id = _des.Id;
            $scope.editData.DesignationParent = _des.DesignationParent;
            $scope.editData.IsActive = _des.IsActive;
            $scope.desform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        };

        /*delete designation parent*/
        $scope.DeleteDesignationParent = function (id) {
            $rootScope.IsAjaxLoading = true;
            DesignationParentService.DeleteDesignationParent(id).then(function (result) {
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
            document.location.href = "../../Handler/DesignationParent.ashx?timezone=" + $scope.timeZone
        };

        /*datatable*/
        $scope.RetrieveDesignationParent = function () {
            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,
                sorting: {
                    DesignationParent: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;
                    DesignationParentService.GetDesignationParentList($scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().DesignationParent).then(function (result) {
                        if (result.data.IsValidUser) {
                            if (result.data.MessageType != 0) { // 0:Error
                                //display no data message
                                if (result.data.DataList.total == 0) {
                                    $scope.noRecord = true;
                                    $scope.filterText = params.filter().DesignationParent;
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

