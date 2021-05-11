/// <reference path="../libs/angular/angular.min.js" />

(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("FestivalCtrl", [
            "$scope", "$rootScope", "$timeout", "FestivalService", "$http", "$filter", "ngTableParams",
            festivalCtrl
        ]);


    //Main controller function
    function festivalCtrl($scope, $rootScope, $timeout, FestivalService, $http, $filter, ngTableParams) {
        $scope.editData = $scope.editData || {};
        $scope.mode = "Add";
        $scope.saveText = "Save";
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes
        $scope.isFocusFirst = false;
        $scope.storage = { lastRecord: "" };
        $scope.filterData = {};
        $scope.topFilter = 0;

        $rootScope.$on('onFestivalTypeTabSelected', function () {
            $rootScope.isFestivalFormVisible = false;
            setScrollTableHeight();
        });
        $rootScope.$on('onEscape', function () {
            $rootScope.isFestivalFormVisible = false;
        });

        /*validate festival type dropdown*/
        $scope.validateDrop = function () {
            if ($scope.editData.FestivalTypeId && $scope.editData.FestivalTypeId != 0) return false;
            return true;
        };

        /*reset festival form*/
        function ResetForm() {
            $scope.editData = {
                FestivalId: 0, FestivalName: "", FestivalDate: "", FestivalGroupId: 0, FestivalTypeId: 0, IsActive: true
            };
            $scope.fesform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }

        /*filter festival list*/
        $scope.FilterByFestivalType = function (filter) {
            $scope.topFilter = filter.FestivalTypeId;
            $scope.RefreshTable();
        };

        /* getting list of festival type*/
        function loadFestivalTypeDrop() {
            FestivalService.RetrieveFestivalType().then(function (result) {
                $scope.FestivalType = result.data.DataList;
                $scope.filterData.FestivalTypeId = 0; //select default value of top filter dropdown
            });
        };
        loadFestivalTypeDrop();

        /*add new festival*/
        $scope.AddFestival = function () {
            $rootScope.isFestivalFormVisible = true;
            $scope.mode = "Add";
            $scope.saveText = "Save";
            ResetForm();
        };

        /*save festival*/
        $scope.CreateUpdateFestival = function (fes) {
            var dateRange = $filter("date")($scope.myDateRange.startDate._d, "yyyy-MM-dd") + "$" + $filter("date")($scope.myDateRange.endDate._d, "yyyy-MM-dd");
            $scope.editData.FestivalDate = dateRange;
            $rootScope.IsAjaxLoading = true;
            FestivalService.CreateUpdateFestival(fes).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.editData = { FestivalId: 0, FestivalName: "", FestivalDate: "", FestivalGroupId: 0, FestivalTypeId: 0, IsActive: true };
                        $scope.myDateRange = {};
                        $scope.RefreshTable();
                        $scope.storage.lastRecord = {};
                        $scope.isFirstFocus = false;
                        $scope.fesform.$setPristine();
                        if ($scope.mode === "Edit") {
                            $rootScope.isFestivalFormVisible = false;
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

        /*refresh festival list */
        $scope.RefreshTable = function () {
            $scope.tableParams.reload();
        };

        /*change festival status*/
        $scope.ChangeStatus = function (id, status) {
            var oper = status == true ? "InActive" : "Active";
            $rootScope.IsAjaxLoading = true;
            FestivalService.ChangeStatus(id, status).then(function (result) {
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

        /*reset form in add and edit mode*/
        $scope.ResetFestival = function () {
            if ($scope.mode == "Edit") {
                $scope.editData.FestivalId = $scope.storage.lastRecord.FestivalId;
                $scope.editData.FestivalName = $scope.storage.lastRecord.FestivalName;
                $scope.editData.FestivalDate = $scope.storage.lastRecord.FestivalDate;
                var dates = $scope.storage.lastRecord.FestivalDate.split(' - ');
                $scope.myDateRange = {
                    startDate: moment(dates[0], "DD-MM-YYYY"),
                    endDate: moment(dates[1], "DD-MM-YYYY")
                };

                $scope.editData.FestivalGroupId = $scope.storage.lastRecord.FestivalGroupId;
                $scope.editData.FestivalTypeId = $scope.storage.lastRecord.FestivalTypeId;
                $scope.editData.IsActive = $scope.storage.lastRecord.IsActive;
            } else { //mode == add
                ResetForm();
            }
        };

        /*cancel button click event*/
        $scope.CloseFestival = function () {
            $scope.mode = "Add";
            ResetForm();
            $rootScope.isFestivalFormVisible = false;
        };

        /*get record for edit festival*/
        $scope.UpdateFestival = function (_fes) {
            $scope.storage.lastRecord = _fes;
            $rootScope.isFestivalFormVisible = true;
            $scope.mode = "Edit";
            $scope.saveText = "Update";

            $scope.editData.FestivalId = _fes.FestivalId;
            $scope.editData.FestivalName = _fes.FestivalName;
            $scope.editData.FestivalDate = _fes.FestivalDate;
            var dates = _fes.FestivalDate.split(' - ');
            $scope.myDateRange = {
                startDate: moment(dates[0], "DD-MM-YYYY"),
                endDate: moment(dates[1], "DD-MM-YYYY")
            };
            $scope.editData.FestivalGroupId = _fes.FestivalGroupId;
            $scope.editData.FestivalTypeId = _fes.FestivalTypeId;
            $scope.editData.IsActive = _fes.IsActive;

            $scope.fesform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        };

        /*delete festival*/
        $scope.DeleteFestival = function (id) {
            $rootScope.IsAjaxLoading = true;
            FestivalService.DeleteFestival(id).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType != 0) { // 0:Error
                        toastr.success(result.data.Message, 'Success');
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

        /*export to excel*/
        $scope.ExportToExcel = function () {
            document.location.href = "../../Handler/Festival.ashx?timezone=" + $scope.timeZone + "&FestivalType=" + $scope.filterData.FestivalTypeId
        };

        /*getting list of festival*/
        $scope.RetrieveFestival = function () {
            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,
                sorting: {
                    FestivalDate: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;
                    FestivalService.GetFestivalList($scope.topFilter,$scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().FestivalName).then(function (result) {
                        if (result.data.IsValidUser) {
                            if (result.data.MessageType != 0) { // 0:Error
                                if (result.data.DataList.total == 0) {//display no data message
                                    $scope.noRecord = true;
                                    $scope.filterText = params.filter().FestivalName;
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

