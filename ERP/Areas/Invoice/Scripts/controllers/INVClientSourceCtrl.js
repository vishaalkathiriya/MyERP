/// <reference path="../libs/angular/angular.min.js" />

(function () {
    'use strict';
    //Define controller signature

    angular.module("ERPApp.Controllers")
    .controller("INVClientSourceCtrl", [
         "$scope", "$rootScope", "$timeout", "INVClientSourceService", "$http", "$filter", "ngTableParams",
        INVClientSourceCtrl
    ]);

    function INVClientSourceCtrl($scope, $rootScope, $timeout, INVClientSourceService, $http, $filter, ngTableParams) {

        $scope.editData = $scope.editData || {};
        $scope.mode = "Add";
        $scope.saveText = "Save";
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes
        $scope.isFocusFirst = false;
        $scope.storage = { lastRecord: "" };



        /*reset the form*/
        function ResetForm() {
            $scope.editData = {
                Id: 0,
                ClientSourceName: "",
                IsActive: true
            };
            $scope.clientSourceform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }

        /*add new  client source*/
        $scope.AddClientSource = function () {
            $rootScope.isClientSourceFormVisible = true;
            $scope.mode = "Add";
            $scope.saveText = "Save";
            ResetForm(); //reset form
        };

        /*save designation group*/
        $scope.CreateUpdateClientSource = function (doc) {
            doc = {
                PKSourceId: doc.Id,
                SourceName: doc.ClientSourceName,
                IsActive: doc.IsActive
            };
            INVClientSourceService.CreateUpdateClientSource(doc).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.editData = {
                            PKSourceId: 0,
                            SourceName: "",
                            IsActive: true
                        };
                        $scope.RefreshTable();
                        $scope.storage.lastRecord = {};
                        $scope.isFirstFocus = false;
                        $scope.clientSourceform.$setPristine();
                        if ($scope.mode === "Edit") {
                            $rootScope.isClientSourceFormVisible = false;
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
            INVClientSourceService.ChangeStatus(id, status).then(function (result) {
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
        $scope.ResetClientSource = function () {
            if ($scope.mode == "Edit") {
                $scope.editData.Id = $scope.storage.lastRecord.PKSourceId;
                $scope.editData.ClientSourceName = $scope.storage.lastRecord.SourceName;
                $scope.editData.IsActive = $scope.storage.lastRecord.IsActive;
            } else { //mode == add
                ResetForm();
            }
        };


        /*cancel button click event*/
        $scope.CloseClientSource = function () {
            $scope.mode = "Add";
            ResetForm();
            $rootScope.isClientSourceFormVisible = false;
        };

        /*get record for edit designation group*/
        $scope.UpdateClientSource = function (_ClientSource) {
            $scope.storage.lastRecord = _ClientSource;
            $rootScope.isClientSourceFormVisible = true;
            $scope.mode = "Edit";
            $scope.saveText = "Update";

            $scope.editData.Id = _ClientSource.PKSourceId;
            $scope.editData.ClientSourceName = _ClientSource.SourceName;
            $scope.editData.IsActive = _ClientSource.IsActive;
            $scope.clientSourceform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        };


        /*delete designation group*/
        $scope.DeleteClientSource = function (id) {
            $rootScope.IsAjaxLoading = true;
            INVClientSourceService.DeleteClientSource(id).then(function (result) {
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
            document.location.href = "../../Handler/INVClientSource.ashx?timezone=" + $scope.timeZone
        };

        /*datatable*/
        $scope.RetrieveClientSource = function () {
            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,
                sorting: {
                    SourceName: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;
                    INVClientSourceService.GetClientSourceList($scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().SourceName).then(function (result) {
                        if (result.data.IsValidUser) {
                            if (result.data.MessageType != 0) { // 0:Error
                                //display no data message
                                if (result.data.DataList.total == 0) {
                                    $scope.noRecord = true;
                                    $scope.filterText = params.filter().SourceName;
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