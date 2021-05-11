/// <reference path="../libs/angular/angular.min.js" />


(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("FestivalTypeCtrl", [
            "$scope", "$rootScope", "$timeout", "FestivalTypeService", "$http", "$filter", "ngTableParams","$q",
            festivalTypeCtrl
        ]);


    //Main controller function
    function festivalTypeCtrl($scope, $rootScope, $timeout, FestivalTypeService, $http, $filter, ngTableParams,$q) {
        $scope.editData = $scope.editData || {};
        $scope.mode = "Add";
        $scope.saveText = "Save";
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes
        $scope.isFocusFirst = false;
        $scope.storage = { lastRecord: "" };

        $rootScope.$on('onFestivalTabSelected', function () {
            $rootScope.isFestivalTypeFormVisible = false;
            setScrollTableHeight();
        });
        $rootScope.$on('onEscape', function () {
            $rootScope.isFestivalTypeFormVisible = false;
        });

        /*reset the form*/
        function ResetForm() {
            $scope.editData = {
                FestivalTypeId: 0, FestivalType: "", PartFullTime: "F", DisplayColorCode: "", IsWorkingDay: true, IsActive: true
            };
            $scope.fesform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }

        /*add new festival type*/
        $scope.AddFestivalType = function () {
            $rootScope.isFestivalTypeFormVisible = true;
            $scope.mode = "Add";
            $scope.saveText = "Save";
            ResetForm(); //reset form
        };

        /*save festival type*/
        $scope.CreateUpdateFestivalType = function (fes) {
            FestivalTypeService.CreateUpdateFestivalType(fes).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.editData = { FestivalTypeId: 0, FestivalType: "", PartFullTime: "F", DisplayColorCode: "", IsWorkingDay: true, IsActive: true };
                        $scope.RefreshTable();
                        $scope.storage.lastRecord = {};
                        $scope.isFirstFocus = false;
                        $scope.fesform.$setPristine();
                        if ($scope.mode === "Edit") {
                            $rootScope.isFestivalTypeFormVisible = false;
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

        /*active/inactive festival type*/
        $scope.ChangeStatus = function (id, status) {
            var oper = status == true ? "InActive" : "Active";
            $rootScope.IsAjaxLoading = true;
            FestivalTypeService.ChangeStatus(id, status).then(function (result) {
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
        $scope.ResetFestivalType = function () {
            if ($scope.mode == "Edit") {
                $scope.editData.FestivalTypeId = $scope.storage.lastRecord.FestivalTypeId;
                $scope.editData.FestivalType = $scope.storage.lastRecord.FestivalType;
                $scope.editData.PartFullTime = $scope.storage.lastRecord.PartFullTime;
                $scope.editData.DisplayColorCode = $scope.storage.lastRecord.DisplayColorCode;
                $scope.editData.IsWorkingDay = $scope.storage.lastRecord.IsWorkingDay;
                $scope.editData.IsActive = $scope.storage.lastRecord.IsActive;
            } else { //mode == add
                ResetForm();
            }
        };

        /*cancel button click event*/
        $scope.CloseFestivalType = function () {
            $scope.mode = "Add";
            ResetForm();
            $rootScope.isFestivalTypeFormVisible = false;
        };

        /*get record for edit role*/
        $scope.UpdateFestivalType = function (_fes) {
            $scope.storage.lastRecord = _fes;
            $rootScope.isFestivalTypeFormVisible = true;
            $scope.mode = "Edit";
            $scope.saveText = "Update";

            $scope.editData.FestivalTypeId = _fes.FestivalTypeId;
            $scope.editData.FestivalType = _fes.FestivalType;
            $scope.editData.PartFullTime = _fes.PartFullTime;
            $scope.editData.DisplayColorCode = _fes.DisplayColorCode;
            $scope.editData.IsWorkingDay = _fes.IsWorkingDay;
            $scope.editData.IsActive = _fes.IsActive;
            $scope.fesform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        };

        /*delete festival type*/
        $scope.DeleteFestivalType = function (id) {
            $rootScope.IsAjaxLoading = true;
            FestivalTypeService.DeleteFestivalType(id).then(function (result) {
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
            document.location.href = "../../Handler/FestivalType.ashx?timezone=" + $scope.timeZone
        };

        $scope.FestivalTimeType = function (column) {

            var def = $q.defer(),
            arr = [
                { id: "F", title: "FullTime" },
                { id: "P", title: "PartTime" }
            ];
            def.resolve(arr);
            return def;
        };

        /*datatable*/
        $scope.RetrieveFestivalType = function () {
            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,
                sorting: {
                    FestivalType: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;
                    FestivalTypeService.GetFestivalTypeList($scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter()).then(function (result) {
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
                                toastr.error(result.Message, 'Opps, Something went wrong');
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

