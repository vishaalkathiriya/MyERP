/// <reference path="../libs/angular/angular.min.js" />


(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("SRTypeCtrl", [
            "$scope", "$rootScope", "$timeout", "SRTypeService", "$http", "$filter", "ngTableParams",
            SRTypeCtrl
        ]);


    //Main controller function
    function SRTypeCtrl($scope, $rootScope, $timeout, SRTypeService, $http, $filter, ngTableParams) {
        $scope.editData = $scope.editData || {};
        $scope.mode = "Add";
        $scope.saveText = "Save";
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes
        $scope.isFocusFirst = false;
        $scope.storage = { lastRecord: "" };
        
        $rootScope.$on('onSubTypeTabSelected', function () {
            $rootScope.isTypeFormVisible = false;
        });
        $rootScope.$on('onParameterTabSelected', function () {
            $rootScope.isTypeFormVisible = false;
        });
        $rootScope.$on('onPartsTabSelected', function () {
            $rootScope.isTypeFormVisible = false;
        });
        $rootScope.$on('onEscape', function () {
            $rootScope.isTypeFormVisible = false;
        });


        /*reset the form*/
        function ResetForm() {
            $scope.editData = {
                TypeId: 0,
                TypeName: "",
                TypePrefix: "",
                Remarks: ""
            };
            $scope.typeForm.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }

        /*add new SR-Type*/
        $scope.AddSRType = function () {
            $rootScope.isTypeFormVisible = true;
            $scope.mode = "Add";
            $scope.saveText = "Save";
            ResetForm(); //reset form
        };

        /*save SR-Type*/
        $scope.CreateUpdateSRType = function (doc) {
            SRTypeService.CreateUpdateSRType(doc).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.editData = {
                            TypeId: 0,
                            TypeName: "",
                            TypePrefix: "",
                            Remarks: ""
                        };
                        $scope.master.TypeChanged = !$scope.master.TypeChanged;
                        $scope.RefreshTable();
                        $scope.storage.lastRecord = {};
                        $scope.isFirstFocus = false;
                        $scope.typeForm.$setPristine();
                        if ($scope.mode === "Edit") {
                            $rootScope.isTypeFormVisible = false;
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

        /*cancel button click event*/
        $scope.CloseSRType = function () {
            $scope.mode = "Add";
            $rootScope.isTypeFormVisible = false;
            ResetForm();
        };


        /*refresh table after */
        $scope.RefreshTable = function () {
            $scope.tableParams.reload();
        };

        /*reset the form*/
        $scope.ResetSRType = function () {
            if ($scope.mode == "Edit") {
                $scope.editData.TypeId = $scope.storage.lastRecord.TypeId;
                $scope.editData.TypeName = $scope.storage.lastRecord.TypeName;
                $scope.editData.TypePrefix = $scope.storage.lastRecord.TypePrefix;
                $scope.editData.Remarks = $scope.storage.lastRecord.Remarks;
            } else { //mode == add
                ResetForm();
            }
        };

        /*get record for edit SR-Type*/
        $scope.UpdateSRType = function (_sr) {
            $scope.storage.lastRecord = _sr;
            $rootScope.isTypeFormVisible = true;
            $scope.mode = "Edit";
            $scope.saveText = "Update";

            $scope.editData.TypeId = _sr.TypeId;
            $scope.editData.TypeName = _sr.TypeName;
            $scope.editData.TypePrefix = _sr.TypePrefix;
            $scope.editData.Remarks = _sr.Remarks;
            $scope.typeForm.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        };

        /*delete SR-Type*/
        $scope.DeleteSRType = function (id) {
            $rootScope.IsAjaxLoading = true;
            SRTypeService.DeleteSRType(id).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 0) { // 0:Error
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                    else if (result.data.MessageType == 2) { // 2:Warning
                        toastr.warning(result.data.Message, 'This record is in use');
                    }
                    else {
                        toastr.success(result.data.Message, 'Success');
                        $scope.master.TypeChanged = !$scope.master.TypeChanged;
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
            document.location.href = "../../Handler/SRType.ashx?timezone=" + $scope.timeZone
        };

        /*datatable*/
        $scope.RetrieveSRType = function () {
            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,
                sorting: {
                    TypeName: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;
                    SRTypeService.GetSRTypeList($scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().TypeName, params.filter().TypePrefix).then(function (result) {
                        if (result.data.IsValidUser) {
                            if (result.data.MessageType != 0) { // 0:Error
                                //display no data message
                                if (result.data.DataList.total == 0) {
                                    $scope.noRecord = true;
                                    $scope.filterText = params.filter().TypeName;
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

