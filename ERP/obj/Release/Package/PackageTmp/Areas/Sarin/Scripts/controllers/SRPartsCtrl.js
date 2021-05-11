/// <reference path="../libs/angular/angular.min.js" />


(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("SRPartCtrl", [
            "$scope", "$rootScope", "$timeout", "SRPartService", "$http", "$filter", "ngTableParams",
            SRPartCtrl
        ]);


    //Main controller function
    function SRPartCtrl($scope, $rootScope, $timeout, SRPartService, $http, $filter, ngTableParams) {
        $scope.editData = $scope.editData || {};
        $scope.mode = "Add";
        $scope.saveText = "Save";
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes
        $scope.isFocusFirst = false;
        $scope.storage = { lastRecord: "" };

        $rootScope.$on('onTypeTabSelected', function () {
            $rootScope.isPartFormVisible = false;
        });
        $rootScope.$on('onSubTypeTabSelected', function () {
            $rootScope.isPartFormVisible = false;
        });
        $rootScope.$on('onParameterTabSelected', function () {
            $rootScope.isPartFormVisible = false;
        });
        $rootScope.$on('onEscape', function () {
            $rootScope.isPartFormVisible = false;
        });

        /*reset the form*/
        function ResetForm() {
            $scope.editData = {
                PartId: 0,
                PartName: "",
                Remarks: ""
            };
            $scope.partform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }

        /*add new SR-Part*/
        $scope.AddSRPart = function () {
            $rootScope.isPartFormVisible = true;
            $scope.mode = "Add";
            $scope.saveText = "Save";
            ResetForm(); //reset form
        };

        /*save SR-Part*/
        $scope.CreateUpdateSRPart = function (doc) {
            SRPartService.CreateUpdateSRPart(doc).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.editData = {
                            PartId: 0,
                            PartName: "",
                            Remarks: ""
                        };
                        $scope.RefreshTable();
                        $scope.storage.lastRecord = {};
                        $scope.isFirstFocus = false;
                        $scope.partform.$setPristine();
                        if ($scope.mode === "Edit") {
                            $rootScope.isPartFormVisible = false;
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
        $scope.ResetSRPart = function () {
            if ($scope.mode == "Edit") {
                $scope.editData.PartId = $scope.storage.lastRecord.PartId;
                $scope.editData.PartName = $scope.storage.lastRecord.PartName;
                $scope.editData.Remarks = $scope.storage.lastRecord.Remarks;
            } else { //mode == add
                ResetForm();
            }
        };

        /*cancel button click event*/
        $scope.CloseSRPart = function () {
            $scope.mode = "Add";
            ResetForm();
            $rootScope.isPartFormVisible = false;
        };

        /*get record for edit SR-Part*/
        $scope.UpdateSRPart = function (_part) {
            $scope.storage.lastRecord = _part;
            $rootScope.isPartFormVisible = true;
            $scope.mode = "Edit";
            $scope.saveText = "Update";

            $scope.editData.PartId = _part.PartId;
            $scope.editData.PartName = _part.PartName;
            $scope.editData.Remarks = _part.Remarks;
            $scope.partform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        };

        /*delete SR-Part*/
        $scope.DeleteSRPart = function (id) {
            $rootScope.IsAjaxLoading = true;
            SRPartService.DeleteSRPart(id).then(function (result) {
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
            document.location.href = "../../Handler/SRPart.ashx?timezone=" + $scope.timeZone
        };

        /*datatable*/
        $scope.RetrieveSRPart = function () {
            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,
                sorting: {
                    PartName: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;
                    SRPartService.GetSRPartList($scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().PartName).then(function (result) {
                        if (result.data.IsValidUser) {
                            if (result.data.MessageType != 0) { // 0:Error
                                //display no data message
                                if (result.data.DataList.total == 0) {
                                    $scope.noRecord = true;
                                    $scope.filterText = params.filter().PartName;
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

