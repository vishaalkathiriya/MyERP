/// <reference path="../libs/angular/angular.min.js" />


(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("SRFloorCtrl", [
            "$scope", "$rootScope", "$timeout", "SRFloorService", "$http", "$filter", "ngTableParams",
            SRFloorCtrl
        ]);


    //Main controller function
    function SRFloorCtrl($scope, $rootScope, $timeout, SRFloorService, $http, $filter, ngTableParams) {
        $scope.editData = $scope.editData || {};
        $scope.mode = "Add";
        $scope.saveText = "Save";
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes
        $scope.isFocusFirst = false;
        $scope.storage = { lastRecord: "" };
        $scope.selection = [];
        $scope.machineError = false;
        $('#txtRemarks').elastic();
        /*reset the form*/
        function ResetForm() {
            $scope.selectedList = [];
            $scope.selection = [];
            $scope.editData = {
                FloorId: 0,
                LocationId: 0,
                Manager: "",
                Remarks: ""
            };

            $scope.floorform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }
        /* getting list of machine*/
        function loadMachine() {
            SRFloorService.RetrieveSRMachine().then(function (result) {
                $scope.MachineList = result.data.DataList;
            });
        };
        loadMachine();

        /* getting list of types*/
        function loadLocations() {
            SRFloorService.RetrieveLocations().then(function (result) {
                $scope.Locations = result.data.DataList;
                //$scope.editData.LocationId = "0";
            });
        };
        loadLocations();


        /*validate type dropdown*/
        $scope.validateDropLocation = function () {
            if ($scope.editData.LocationId && $scope.editData.LocationId != 0) return false;
            return true;
        };

        /*add new SR-Floor*/
        $scope.AddSRFloor = function () {
            $rootScope.isFormVisible = true;
            $scope.mode = "Add";
            $scope.saveText = "Save";
            ResetForm(); //reset form
        };

        /*save floor */
        $scope.CreateUpdateSRFloor = function (doc) {
            angular.forEach($scope.selectedList, function (value, key) {
                var idx = $scope.selection.indexOf(value.Id);
                if (idx > -1) {
                    $scope.selection.splice(idx, 1);
                }
                else {
                    $scope.selection.push(value.Id);
                }
            });

            if ($scope.selection == null || $scope.selection == "" || $scope.selection == "undefined") {
                $scope.machineError = true;
            }
            else {
                $scope.machineError = false;
                var selectedMachines = $scope.selection;
                SRFloorService.CreateUpdateSRFloor(doc, selectedMachines).then(function (result) {
                    if (result.data.IsValidUser) {
                        if (result.data.MessageType == 1) { // 1:Success
                            toastr.success(result.data.Message, 'Success');
                            $scope.editData = {
                                FloorId: 0,
                                LocationId: 0,
                                Manager: "",
                                Remarks: ""
                            };
                            loadMachine();
                            $scope.RefreshTable();
                            $scope.storage.lastRecord = {};
                            $scope.isFirstFocus = false;
                            $scope.selectedList = [];
                            $scope.selection = [];

                            $scope.floorform.$setPristine();
                            if ($scope.mode === "Edit") {
                                $rootScope.isFormVisible = false;
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
        }

        /*refresh table after */
        $scope.RefreshTable = function () {
            $scope.tableParams.reload();
        };

        /*cancel button click event*/
        $scope.CloseSRFloor = function () {
            $scope.mode = "Add";
            $rootScope.isFormVisible = false;
            ResetForm();
        };

        /*reset the form*/
        $scope.ResetSRFloor = function () {
            if ($scope.mode == "Edit") {
                $scope.editData.FloorId = $scope.storage.lastRecord.FloorId;
                $scope.editData.LocationId = $scope.storage.lastRecord.LocationId;
                $scope.editData.Manager = $scope.storage.lastRecord.Manager;
                $scope.editData.Remarks = $scope.storage.lastRecord.Remarks;
            } else { //mode == add
                ResetForm();
            }
        };

        /*get record for edit Floor */
        $scope.UpdateSRFloor = function (_floor) {

            SRFloorService.RetrieveSRMachine().then(function (result) {
                $scope.MachineList = result.data.DataList;
                var new_obj = { 'Id': _floor.MachineId, 'Label': _floor.MachineName };
                $scope.MachineList.push(new_obj);
                $scope.selectedList = [{ Id: _floor.MachineId, Label: _floor.MachineName }];
            })

            $scope.storage.lastRecord = _floor;
            $rootScope.isFormVisible = true;
            $scope.mode = "Edit";
            $scope.saveText = "Update";

            $scope.editData.FloorId = _floor.FloorId;
            $scope.editData.LocationId = _floor.LocationId;
            $scope.editData.Manager = _floor.Manager;
            $scope.editData.Remarks = _floor.Remarks;

            $scope.floorform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });

        };

        /*delete SR-Floor*/
        $scope.DeleteSRFloor = function (id) {
            $rootScope.IsAjaxLoading = true;
            SRFloorService.DeleteSRFloor(id).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) {  //1:Success
                        toastr.success(result.data.Message, 'Success');
                        loadMachine();
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
            document.location.href = "../../Handler/SRFloor.ashx?timezone=" + $scope.timeZone
        };

        /*datatable*/
        $scope.RetrieveSRFloor = function () {
            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,
                sorting: {
                    Floor: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;
                    SRFloorService.GetSRFloorList($scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().MachineName, params.filter().LocationName, params.filter().Manager, params.filter().SerialNo).then(function (result) {
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
    };


})();

