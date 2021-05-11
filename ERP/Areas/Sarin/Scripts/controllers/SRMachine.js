/// <reference path="../libs/angular/angular.min.js" />


(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("SRMachineCtrl", [
            "$scope", "$rootScope", "$timeout", "SRMachineService", "$http", "$filter", "ngTableParams",
            SRMachineCtrl
        ]);


    //Main controller function
    function SRMachineCtrl($scope, $rootScope, $timeout, SRMachineService, $http, $filter, ngTableParams) {
        $scope.editData = $scope.editData || {};
        $scope.mode = "Add";
        $scope.saveText = "Save";
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes
        $scope.isFocusFirst = false;
        $scope.storage = { lastRecord: "" };
        $scope.editData.TypeId = 0;
        $scope.editData.SubTypeId = 0;
        $scope.editData.ParameterId = 0;
        $('#txtRemarks').elastic();

        // BEGIN DATE PICKER
        $scope.dateOptions = { 'year-format': "'yy'", 'starting-day': 1 };
        $scope.formats = ['dd-MM-yyyy', 'yyyy/MM/dd', 'shortDate'];
        $scope.format = $scope.formats[0];
        $scope.today = function () {
            $scope.currentDate = new Date();
        };
        $scope.today();
        $scope.showWeeks = true;
        $scope.toggleWeeks = function () {
            $scope.showWeeks = !$scope.showWeeks;
        };
        $scope.clear = function () {
            $scope.currentDate = null;

        };
        $scope.calendarOpenIssueDate = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.editData.calOpenIssueDate = true;
        };

        $scope.$watch('editdata.InstallationDate', function (newvalue) {
            $scope.issueDate = $filter('date')(newvalue, 'dd-mm-yyyy'); // give this value to calandar 
        });

        $scope.ValidateDate = function (date) {
            if (date) {
                var isError = false;
                var dates = date.split('-');
                if (dates[0].search("_") > 0 || dates[1].search("_") > 0 || dates[2].search("_") > 0) {
                    isError = true;
                }
                else {
                    if (!parseInt(dates[0]) || parseInt(dates[0]) > 31) { isError = true; }
                    if (!parseInt(dates[1]) || parseInt(dates[1]) > 12) { isError = true; }
                    if (!parseInt(dates[2]) || dates[2].length != 4) { isError = true; }
                }

                if (!isError) { return true; } // date is validated
                return false; // error in validation
            }
            return true;
        };

        $scope.ValidateIssuedDate = function (issueDate) {
            if (!issueDate) {
                $scope.machineform.txtIssueDate.$setValidity("invalidIssueDate", true);
                return;
            } else if (issueDate.length == 10) {
                if ($scope.ValidateDate(issueDate)) {
                    $scope.issueDate = $scope.StringToDateString(issueDate);
                    $scope.machineform.txtIssueDate.$setValidity("invalidIssueDate", true);
                } else {
                    $scope.machineform.txtIssueDate.$setValidity("invalidIssueDate", false);
                }
            } else {
                $scope.machineform.txtIssueDate.$setValidity("invalidIssueDate", false);
            }
        };  

        $scope.$watch('issueDate', function (newValue) {
            $scope.editData.InstallationDate = $filter('date')(newValue, 'dd-MM-yyyy');
        });

        $scope.StringToDateString = function (dtValue) {
            if (dtValue) {
                var dt = dtValue.split('-');
                return dt[2] + "-" + dt[1] + "-" + dt[0] + "T00:00:00";
            }
            return dtValue;
        };
        // End DATE PICKER

        /*reset the form*/
        function ResetForm() {
            $scope.editData = {
                MachineId: 0,
                MachineName: "",
                SerialNo: "",
                InstallationDate: "",
                TypeId: 0,
                SubTypeId: 0,
                ParameterId: 0,
                Remarks: ""
            };
            $scope.machineform.$setPristine();
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

        /*validate sub-type dropdown*/
        $scope.validateDropSubType = function () {
            if ($scope.editData.SubTypeId && $scope.editData.SubTypeId != 0) return false;
            return true;
        };

        /*validate parameter dropdown*/
        $scope.validateDropParameter = function () {
            if ($scope.editData.ParameterId && $scope.editData.ParameterId != 0) return false;
            return true;
        };

        /* getting list of types*/
        function loadMachineTypes() {
            SRMachineService.RetrieveSRType().then(function (result) {
                $scope.SRTypes = result.data.DataList;
            });
        };
        loadMachineTypes();

        $scope.$watch('editData.TypeId', function (newValue) {
            if (newValue != 0) {
                $scope.GetSubTypeByType(newValue).then(function (subTypes) {
                    $scope.SRSubTypes = subTypes;
                });
            } else {
                $scope.SRSubTypes = {};
                $scope.editData.SubTypeId = 0;
            }
        });

        /*getting list of sub-type by type */
        $scope.GetSubTypeByType = function (typeId) {
            if (typeId != null) {
                return SRMachineService.RetrieveSRSubTypeListByTypeId(typeId).then(function (result) {
                    return result.data.DataList;
                });
            }
        };

        $scope.$watch('editData.SubTypeId', function (newValue) {
            if (newValue != 0) {
                $scope.GetParameterBySubType(newValue).then(function (parameters) {
                    $scope.SRParameters = parameters;

                });
            } else {
                $scope.SRParameters = {};
                $scope.editData.ParameterId = 0;
            }
        });

        /*getting list of parameter by sub-type */
        $scope.GetParameterBySubType = function (subTypeId) {
            if (subTypeId != null) {
                return SRMachineService.RetrieveSRParameterBySubTypeId(subTypeId).then(function (result) {
                    return result.data.DataList;
                });
            }
        };

        /*add new SR-Machine*/
        $scope.AddSRMachine = function () {
            $rootScope.isFormVisible = true;
            $scope.mode = "Add";
            $scope.saveText = "Save";
            ResetForm(); //reset form
        };

        /*save SR-Machine*/
        $scope.CreateUpdateSRMachine = function (doc) {

            var IDate = $filter('date')(doc.InstallationDate, 'dd-MM-yyyy').split('-');
            var temp_IssueDate = new Date(parseInt(IDate[2]), parseInt(IDate[1]) - 1, parseInt(IDate[0]), 0, 0, 0);
            doc.InstallationDate = $filter('date')(temp_IssueDate, 'MM-dd-yyyy HH:mm:ss');

            SRMachineService.CreateUpdateSRMachine(doc).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.editData = {
                            MachineId: 0,
                            MachineName: "",
                            SerialNo: "",
                            IntallationDate: "",
                            TypeId: 0,
                            SubTypeId: 0,
                            ParameterId: 0,
                            Remarks: ""
                        };
                        $scope.RefreshTable();
                        $scope.storage.lastRecord = {};
                        $scope.isFirstFocus = false;
                        $scope.machineform.$setPristine();
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

        /*refresh table after */
        $scope.RefreshTable = function () {
            $scope.tableParams.reload();
        };

        /*cancel button click event*/
        $scope.CloseSRParameter = function () {
            $scope.mode = "Add";
            $rootScope.isFormVisible = false;
            ResetForm();
        };

        /*reset the form*/
        $scope.ResetSRParameter = function () {
            if ($scope.mode == "Edit") {
                $scope.editData.MachineId = $scope.storage.lastRecord.MachineId;
                $scope.editData.SerialNo = $scope.storage.lastRecord.SerialNo;
                $scope.editData.MachineName = $scope.storage.lastRecord.MachineName;
                $scope.editData.InstallationDate = $filter('date')($scope.storage.lastRecord.InstallationDate, 'dd-MM-yyyy')
                $scope.editData.TypeId = $scope.storage.lastRecord.TypeId;
                $scope.editData.SubTypeId = $scope.storage.lastRecord.SubTypeId;
                $scope.editData.ParameterId = $scope.storage.lastRecord.ParameterId;
                $scope.editData.Remarks = $scope.storage.lastRecord.Remarks;
            } else { //mode == add
                ResetForm();
            }
        };

        /*cancel button click event*/
        $scope.SRSubType = function () {
            $scope.mode = "Add";
            ResetForm();
            $rootScope.isFormVisible = false;
        };

        /*get record for edit sr machine*/
        $scope.UpdateSRMachine = function (_machine) {
            $scope.storage.lastRecord = _machine;
            $rootScope.isFormVisible = true;
            $scope.mode = "Edit";
            $scope.saveText = "Update";

            $scope.editData.MachineId = _machine.MachineId;
            $scope.editData.SerialNo = _machine.SerialNo;
            $scope.editData.MachineName = _machine.MachineName;
            $scope.issueDate= _machine.InstallationDate;
            $scope.editData.InstallationDate = $filter('date')(_machine.InstallationDate, 'dd-MM-yyyy');
            $scope.editData.TypeId = _machine.TypeId;
            $scope.editData.SubTypeId = _machine.SubTypeId;
            $scope.editData.ParameterId = _machine.ParameterId;
            $scope.editData.Remarks = _machine.Remarks;

            $scope.machineform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        };

        /*delete mahcine*/
        $scope.DeleteSRMachine = function (id) {
            $rootScope.IsAjaxLoading = true;
            SRMachineService.DeleteSRMachine(id).then(function (result) {
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
            document.location.href = "../../Handler/SRMachine.ashx?timezone=" + $scope.timeZone
        };

        /*datatable*/
        $scope.RetrieveSRMachine = function () {
            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,
                sorting: {
                    MachineName: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;
                    SRMachineService.GetSRMachineList($scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().MachineName, params.filter().SerialNo, params.filter().TypeName, params.filter().SubTypeName, params.filter().ParameterName).then(function (result) {
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

