/// <reference path="../libs/angular/angular.min.js" />


(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("SRSubTypeCtrl", [
            "$scope", "$rootScope", "$timeout", "SRSubTypeService", "$http", "$filter", "ngTableParams",
            SRSubTypeCtrl
        ]);


    //Main controller function
    function SRSubTypeCtrl($scope, $rootScope, $timeout, SRSubTypeService, $http, $filter, ngTableParams) {
        $scope.editData = $scope.editData || {};
        $scope.mode = "Add";
        $scope.saveText = "Save";
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes
        $scope.isFocusFirst = false;
        $scope.storage = { lastRecord: "" };

        $rootScope.$on('onTypeTabSelected', function () {
            $rootScope.isSubTypeFormVisible = false;
        });
        $rootScope.$on('onParameterTabSelected', function () {
            $rootScope.isSubTypeFormVisible = false;
        });
        $rootScope.$on('onPartsTabSelected', function () {
            $rootScope.isSubTypeFormVisible = false;
        });
        $rootScope.$on('onEscape', function () {
            $rootScope.isSubTypeFormVisible = false;
        });

        /*reset the form*/
        function ResetForm() {
            $scope.editData = {
                SubTypeId: 0,
                TypeId: 0,
                SubTypeName: "",
                Remarks: ""
            };
            $scope.subtypeform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }

        /*validate type dropdown*/
        $scope.validateDropGroup = function () {
            if ($scope.editData.TypeId && $scope.editData.TypeId != 0) return false;
            return true;
        };

        /*validate selection dropdown*/
        $scope.validateDropSelection = function () {
            if ($scope.editData.Selection && $scope.editData.Selection != 0) return false;
            return true;
        };


        /* getting list of types*/
        function loadMachineTypes() {
            SRSubTypeService.RetrieveSRType().then(function (result) {
                $scope.SRTypes = result.data.DataList;
                $scope.editData.TypeId = "0";
            });
        };
        loadMachineTypes();

        /*add new SR-SubType*/
        $scope.AddSRSubType = function () {
            $rootScope.isSubTypeFormVisible = true;
            $scope.mode = "Add";
            $scope.saveText = "Save";
            ResetForm(); //reset form
        };

        /*save sub type*/
        $scope.CreateUpdateSRSubType = function (doc) {
            SRSubTypeService.CreateUpdateSRSubType(doc).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');

                        // Perform in order to kip TypeId selected after save in "Add" mode
                        $scope.editData.SubTypeId = 0;
                        $scope.editData.SubTypeName = "";
                        $scope.editData.Remarks = "";
                        $scope.editData.Selection = "";

                        $scope.master.SubTypeChanged = !$scope.master.SubTypeChanged;
                        $scope.RefreshTable();
                        $scope.storage.lastRecord = {};
                        $scope.isFirstFocus = false;
                        $scope.subtypeform.$setPristine();
                        if ($scope.mode === "Edit") {
                            $rootScope.isSubTypeFormVisible = false;
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

        /*cancel button click event*/
        $scope.CloseSRSubType = function () {
            $scope.mode = "Add";
            $rootScope.isSubTypeFormVisible = false;
            ResetForm();
        };

        /*reset the form*/
        $scope.ResetSRSubType = function () {
            if ($scope.mode == "Edit") {
                $scope.editData.SubTypeId = $scope.storage.lastRecord.SubTypeId;
                $scope.editData.SubTypeName = $scope.storage.lastRecord.SubTypeName;
                $scope.editData.TypeId = $scope.storage.lastRecord.TypeId;
                $scope.editData.Selection = $scope.storage.lastRecord.Selection;
                $scope.editData.Remarks = $scope.storage.lastRecord.Remarks;
            } else { //mode == add
                ResetForm();
            }
        };

        /*cancel button click event*/
        $scope.SRSubType = function () {
            $scope.mode = "Add";
            ResetForm();
            $rootScope.isSubTypeFormVisible = false;
        };

        /*get record for edit sub type*/
        $scope.UpdateSRSubType = function (_sub) {
            $scope.storage.lastRecord = _sub;
            $rootScope.isSubTypeFormVisible = true;
            $scope.mode = "Edit";
            $scope.saveText = "Update";

            $scope.editData.SubTypeId = _sub.SubTypeId;
            $scope.editData.SubTypeName = _sub.SubTypeName;
            $scope.editData.TypeId = _sub.TypeId;
            $scope.editData.Selection = _sub.Selection;
            $scope.editData.Remarks = _sub.Remarks;
            $scope.subtypeform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        };

        /*delete sub type*/
        $scope.DeleteSRSubType = function (id) {
            $rootScope.IsAjaxLoading = true;
            SRSubTypeService.DeleteSRSubType(id).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) {  //1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.master.SubTypeChanged = !$scope.master.SubTypeChanged;
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
            document.location.href = "../../Handler/SRSubType.ashx?timezone=" + $scope.timeZone
        };

        /*datatable*/
        $scope.RetrieveSRSubType = function () {
            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,
                sorting: {
                    SubTypeName: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;
                    SRSubTypeService.GetSRSubTypeList($scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().SubTypeName, params.filter().TypeName, params.filter().Selection).then(function (result) {
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

        $scope.$watch('master.TypeChanged', function (newValue) {
            loadMachineTypes();
        });


    };


})();

