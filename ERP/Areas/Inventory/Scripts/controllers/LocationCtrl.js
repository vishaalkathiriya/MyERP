/// <reference path="../libs/angular/angular.js" />


(function () {

    'use strict';
    //Define controller signature
    angular.module("ERPApp.Controllers").controller("LocationCtrl", [
        "$scope",
        "$rootScope",
        "$timeout",
        "LocationService",
        "$filter",
        "ngTableParams", locationCtrl]);

    //Main controller function
    function locationCtrl($scope, $rootScope, $timeout, LS, $filter, ngTableParams) {
        $scope.editData = $scope.editData || {};
        $rootScope.isFormVisible = false;
        $scope.isFirstFocus = false;
        $scope.lastLocation = $scope.lastLocation || {};
        $scope.mode = "Add";
        $scope.saveText = "Save";
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes

        //Load All Locations
        $scope.loadAllLocations = function () {
            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,  // Total No. of records per page
                sorting: {
                    LocationName: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    //$rootScope.IsAjaxLoading = true;
                    LS.GetLocationList($scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().LocationName).success(function (result) {
                        if (result.IsValidUser) {
                            //display no data message
                            if (result.MessageType != 0) { // 0:Error
                                //display no data message
                                if (result.DataList.total == 0) {
                                    $scope.noRecord = true;
                                    $scope.filterText = params.filter().LocationName;
                                } else {
                                    $scope.noRecord = false;
                                }
                                params.total(result.DataList.total);
                                $defer.resolve(result.DataList = result.DataList.result);
                                $rootScope.IsAjaxLoading = false;
                            }else {
                                toastr.error(result.data.Message, 'Opps, Something went wrong');
                            }
                        }
                        else {
                            $rootScope.redirectToLogin();
                        }
                    });
                }
            });
        }

        /*add new Location click event*/
        $scope.addLocation = function () {
            $scope.mode = "Add";
            $scope.saveText = "Save";
            $scope.editData = {};
            $scope.lastLocation = {};
            $rootScope.isFormVisible = true;
            $scope.editData.IsActive = true;
            $scope.SetFocus();
            $scope.locform.$setPristine();
            //$scope.catNameTarget.focus();

        };

        //Create and Update Location
        $scope.SaveLocation = function (location) {
            LS.SaveLocation(location).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType === 1) {
                        toastr.success('Your data is successfully submitted', 'Success');
                        $scope.editData = { LocationName: '', IsActive: true };
                        $scope.refreshTable();
                        $scope.lastLocation = {};
                        $scope.isFirstFocus = false;
                        $scope.locform.$setPristine();
                        if ($scope.mode == "Edit") {
                            $rootScope.isFormVisible = false;
                            $scope.saveText = "Save";
                        }
                        else { $scope.SetFocus(); }
                    } else if (result.data.MessageType == 2) { // 2:Warning
                        toastr.warning(result.data.Message, 'Record already exists');
                        $scope.SetFocus();
                    }
                    else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
            });
        }
        // Binding Data for edit mode
        $scope.EditLocation = function (location) {
            $scope.lastLocation = location;
            $rootScope.isFormVisible = true;
            $scope.mode = "Edit";
            $scope.saveText = "Update";
            $scope.editData.LocationId = location.LocationId;
            $scope.editData.LocationName = location.LocationName;
            $scope.editData.IsActive = location.IsActive;
            $scope.locform.$setPristine();
            $scope.SetFocus();
        }

        /*delete Location*/
        $scope.DeleteLocation = function (location) {
            LS.DeleteLocation(location).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 0:Error
                        toastr.success(result.data.Message, 'Success');
                        $scope.cancel();
                        $scope.refreshTable();
                    } else if (result.data.MessageType == 2) { // 1:Warning
                        toastr.warning(result.data.Message, 'Warning!');
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
            });
        }

        /* Change the Status of Location */
        $scope.IsActive = function (location) {
            LS.IsActive(location).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success('Status changed successfully.', 'Success');
                        $timeout(function () {
                            $scope.cancel();
                            $scope.refreshTable();
                        }, 50);
                    } else if (result.data.MessageType == 2) { // 2:Warning
                        toastr.warning(result.data.Message, 'Warning!');
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
            });
        }

        /*reset the form*/
        $scope.reset = function () {
            $scope.location = $scope.lastLocation;
            if ($scope.location.LocationId) {
                $scope.EditLocation($scope.location);
            } else {
                $scope.editData = { LocationName: '', IsActive: true };
                $scope.saveText = "Save";
                $scope.locform.$setPristine();
                $scope.SetFocus();
            }

        }

        /*cancel button click event*/
        $scope.cancel = function () {
            $scope.editData = {};
            $scope.mode = "Add";
            $rootScope.isFormVisible = false;
        }
        // For Exporting Excel
        $scope.ExportToExcel = function () {
            document.location.href = "../../Handler/Location.ashx?timezone=" + $scope.timeZone
        };

        /*refresh table after */
        $scope.refreshTable = function () {
            $scope.tableParams.reload();
        };
        // Reset Focus
        $scope.SetFocus = function () {
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }

    };

})();