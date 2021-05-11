/// <reference path="../libs/angular/angular.min.js" />


(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("SRParameterCtrl", [
            "$scope", "$rootScope", "$timeout", "SRParameterService", "$http", "$filter", "ngTableParams",
            SRParameterCtrl
        ]);


    //Main controller function
    function SRParameterCtrl($scope, $rootScope, $timeout, SRParameterService, $http, $filter, ngTableParams) {
        $scope.editData = $scope.editData || {};
        $scope.mode = "Add";
        $scope.saveText = "Save";
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes
        $scope.isFocusFirst = false;
        $scope.storage = { lastRecord: "" };


        $rootScope.$on('onTypeTabSelected', function () {
            $rootScope.isParameterFormVisible = false;
        });
        $rootScope.$on('onSubTypeTabSelected', function () {
            $rootScope.isParameterFormVisible = false;
        });
        $rootScope.$on('onPartsTabSelected', function () {
            $rootScope.isParameterFormVisible = false;
        });
        $rootScope.$on('onEscape', function () {
            $rootScope.isParameterFormVisible = false;
        });


        /*reset the form*/
        function ResetForm() {
            $scope.editData = {
                ParameterId: 0,
                ParameterName: "",
                SubTypeId: 0,
                Remarks: ""
            };
            $scope.parameterform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }

        /*validate type dropdown*/
        $scope.validateDropGroup = function () {
            if ($scope.editData.SubTypeId && $scope.editData.SubTypeId != 0) return false;
            return true;
        };

        /* getting list of types*/
        function loadMachineSubTypes() {
            SRParameterService.RetrieveSRSubType().then(function (result) {
                $scope.SRSubTypes = result.data.DataList;
                $scope.editData.SubTypeId = "0";
            });
        };
        loadMachineSubTypes();

        /*add new SR-Parameter*/
        $scope.AddSRParameter = function () {
            $rootScope.isParameterFormVisible = true;
            $scope.mode = "Add";
            $scope.saveText = "Save";
            ResetForm(); //reset form
        };

        /*cancel button click event*/
        $scope.CloseSRParameter = function () {
            $scope.mode = "Add";
            $rootScope.isParameterFormVisible = false;
            ResetForm();
        };

        /*save sub parameter*/
        $scope.CreateUpdateSRParameter = function (doc) {
            SRParameterService.CreateUpdateSRParameter(doc).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');

                        // Perform in order to kip SubTypeId selected after save in "Add" mode
                        $scope.editData.ParameterId = 0;
                        $scope.editData.ParameterName = "";
                        $scope.editData.Remarks = "";

                        $scope.RefreshTable();
                        $scope.storage.lastRecord = {};
                        $scope.isFirstFocus = false;
                        $scope.parameterform.$setPristine();
                        if ($scope.mode === "Edit") {
                            $rootScope.isParameterFormVisible = false;
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

        /*reset the form*/
        $scope.ResetSRParameter = function () {
            if ($scope.mode == "Edit") {
                $scope.editData.ParameterId = $scope.storage.lastRecord.ParameterId;
                $scope.editData.ParameterName = $scope.storage.lastRecord.ParameterName;
                $scope.editData.SubTypeId = $scope.storage.lastRecord.SubTypeId;
                $scope.editData.Remarks = $scope.storage.lastRecord.Remarks;
            } else { //mode == add
                ResetForm();
            }
        };

        /*get record for edit sub type*/
        $scope.UpdateSRParameter = function (_para) {
            $scope.storage.lastRecord = _para;
            $rootScope.isParameterFormVisible = true;
            $scope.mode = "Edit";
            $scope.saveText = "Update";

            $scope.editData.ParameterId = _para.ParameterId;
            $scope.editData.ParameterName = _para.ParameterName;
            $scope.editData.SubTypeId = _para.SubTypeId;
            $scope.editData.Remarks = _para.Remarks;
            $scope.parameterform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        };

        /*delete Parameter*/
        $scope.DeleteSRParameter = function (id) {
            $rootScope.IsAjaxLoading = true;
            SRParameterService.DeleteSRParameter(id).then(function (result) {
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
            document.location.href = "../../Handler/SRParameters.ashx?timezone=" + $scope.timeZone
        };

        /*datatable*/
        $scope.RetrieveSRParameter = function () {
            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,
                sorting: {
                    ParameterName: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;
                    SRParameterService.GetSRParameterList($scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().ParameterName, params.filter().SubTypeName).then(function (result) {
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

        $scope.$watch('master.SubTypeChanged', function (newValue) {
            loadMachineSubTypes();
        });

    };


})();
