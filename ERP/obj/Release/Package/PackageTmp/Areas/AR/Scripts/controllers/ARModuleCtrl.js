/// <reference path="../libs/angular/angular.min.js" />


(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("ARModuleCtrl", [
            "$scope", "$rootScope", "$timeout", "ARModuleService", "$http", "$filter", "ngTableParams",
            moduleCtrl
        ]);


    //Main controller function
    function moduleCtrl($scope, $rootScope, $timeout, ARModuleService, $http, $filter, ngTableParams) {
        $scope.editData = $scope.editData || {};
        $scope.mode = "Add";
        $scope.saveText = "Save";
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes
        $scope.isFocusFirst = false;
        $scope.storage = { lastRecord: "" };

        /*reset the form*/
        function ResetForm() {
            $scope.editData = {
                ModuleName: "",
                SeqNo: "",
                IsActive: true
            };
            //$scope.isChecked = true;
            $scope.modform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }

        /*add new module*/
        $scope.AddModule = function () {
            $rootScope.isFormVisible = true;
            $scope.mode = "Add";
            $scope.saveText = "Save";
            ResetForm(); //reset form
        };

        /*change sequence number from grid*/
        $scope.isEdit = [];
        $scope.oldSeqNo = [];
        $scope.ChangeSeqNo = function (seqNo, moduleId, index) {
            ARModuleService.UpdateSequenceNo(seqNo, moduleId).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.isEdit[index] = false;
                        $scope.oldSeqNo[index] = 0;
                        $scope.RefreshTable();
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;
            });
        };

        $scope.CancelSeqChange = function (index, $data) {
            $scope.isEdit[index] = false;
            $data[index].SeqNo = $scope.oldSeqNo[index];
            $scope.oldSeqNo[index] = 0;
        };

        /*save document*/
        $scope.CreateUpdateModule = function (mod) {
            ARModuleService.CreateUpdateModule(mod).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.editData = { ModuleName: '',SeqNo:'', IsActive: true };
                        $scope.RefreshTable();
                        $scope.storage.lastRecord = {};
                        $scope.isFirstFocus = false;
                        $scope.modform.$setPristine();
                        if ($scope.mode === "Edit") {
                            $rootScope.isFormVisible = false;
                            $scope.saveText = "Save";
                        }
                    }else if (result.data.MessageType == 2) { // 2:Warning
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

        /*active/inactive module*/
        $scope.ChangeStatus = function (id, status) {
            var oper = status == true ? "InActive" : "Active";
            $rootScope.IsAjaxLoading = true;
            ARModuleService.ChangeStatus(id, status).then(function (result) {
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
        $scope.ResetModule = function () {
            if ($scope.mode == "Edit") {
                $scope.editData.ModuleName = $scope.storage.lastRecord.ModuleName;
                $scope.editData.SeqNo = $scope.storage.lastRecord.SeqNo;
                $scope.editData.IsActive = $scope.storage.lastRecord.IsActive;
            } else { //mode == add
                ResetForm();
            }
        };

        /*cancel button click event*/
        $scope.CloseModule = function () {
            $scope.mode = "Add";
            ResetForm();
            $rootScope.isFormVisible = false;
        };

        /*get record for edit Module*/
        $scope.UpdateModule = function (_mod) {
            $scope.storage.lastRecord = _mod;
            $rootScope.isFormVisible = true;
            $scope.mode = "Edit";
            $scope.saveText = "Update";

            $scope.editData.ModuleId = _mod.ModuleId;
            $scope.editData.ModuleName = _mod.ModuleName;
            $scope.editData.SeqNo = _mod.SeqNo;
            $scope.editData.IsActive = _mod.IsActive;
            $scope.modform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        };

        /*delete module*/
        $scope.DeleteModule = function (id) {
            $rootScope.IsAjaxLoading = true;
            ARModuleService.DeleteModule(id).then(function (result) {
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

        /*datatable*/
        $scope.RetrieveModule = function () {
            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,
                sorting: {
                    ModuleName: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;
                    ARModuleService.GetModuleList($scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().ModuleName).then(function (result) {
                        if (result.data.IsValidUser) {
                            if (result.data.MessageType != 0) { // 0:Error
                                //display no data message
                                if (result.data.DataList.total == 0) {
                                    $scope.noRecord = true;
                                    $scope.filterText = params.filter().ModuleName;
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

