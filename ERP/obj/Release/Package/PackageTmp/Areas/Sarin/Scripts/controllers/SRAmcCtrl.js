/// <reference path="../libs/angular/angular.min.js" />


(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("SRAmcCtrl", [
            "$scope", "$rootScope", "$timeout", "SRAmcService", "$http", "$filter", "ngTableParams",
            SRAmcCtrl
        ]);


    //Main controller function
    function SRAmcCtrl($scope, $rootScope, $timeout, SRAmcService, $http, $filter, ngTableParams) {
        $scope.editData = $scope.editData || {};
        $scope.mode = "Add";
        $scope.saveText = "Save";
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes
        $scope.isFocusFirst = false;
        $scope.storage = { lastRecord: "" };
        $scope.selection = [];
        $scope.machineError = false;
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
        $scope.minDate = $scope.minDate || new Date();
        $scope.startDate = new Date();

        $scope.calendarOpenStartDate = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.calOpenStartDate = true;
            $scope.calOpenEndDate = false;
        };

        $scope.calendarOpenEndDate = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.calOpenStartDate = false;
            $scope.calOpenEndDate = true;
        };

        $scope.$watch('startDate', function (newValue) {
            $scope.editData.StartDate = $filter('date')(newValue, 'dd-MM-yyyy');
            $scope.minDate = newValue;
            if ($scope.IsStartDateBig()) {
                $scope.editData.EndDate = $scope.editData.StartDate;
                $scope.endDate = newValue;
            }
        });
        $scope.$watch('endDate', function (newValue) {
            $scope.editData.EndDate = $filter('date')(newValue, 'dd-MM-yyyy');
        });

        $scope.ValidateStartDate = function (startDate) {
            if (startDate.length == 10) {
                if ($scope.ValidateDate(startDate)) {
                    $scope.startDate = $scope.StringToDateString(startDate);
                    $scope.amcform.txtStartDate.$setValidity("invalidStartDate", true);

                    if ($scope.IsStartDateBig()) {
                        $scope.editData.EndDate = startDate;
                        $scope.endDate = $scope.StringToDateString(startDate);
                    }
                } else {
                    $scope.amcform.txtStartDate.$setValidity("invalidStartDate", false);
                }
            } else {
                $scope.amcform.txtStartDate.$setValidity("invalidStartDate", false);
            }
        };

        $scope.ValidateEndDate = function (endDate) {
            if (endDate.length == 10) {
                if ($scope.ValidateDate(endDate)) {
                    $scope.endDate = $scope.StringToDateString(endDate);
                    $scope.amcform.txtEndDate.$setValidity("invalidEndDate", true);

                    if ($scope.IsStartDateBig()) {
                        $scope.editData.EndDate = $scope.editData.StartDate;
                        $scope.endDate = $scope.StringToDateString($scope.editData.StartDate);
                    }
                } else {
                    $scope.amcform.txtEndDate.$setValidity("invalidEndDate", false);
                }
            } else {
                $scope.amcform.txtEndDate.$setValidity("invalidEndDate", false);
            }
        };


        $scope.IsGreterThanToday = function (date) { //date should be in dd-MM-yyyy format
            var tDT = date.split('-');
            var todayDT = $filter('date')(new Date(), 'dd-MM-yyyy').split('-');

            var tDate = new Date(parseInt(tDT[2]), parseInt(tDT[1]) - 1, parseInt(tDT[0]));
            var todayDate = new Date(parseInt(todayDT[2]), parseInt(todayDT[1]) - 1, parseInt(todayDT[0]));

            if (tDate > todayDate) {
                return true;
            }
            return false;
        };

        $scope.IsStartDateBig = function () {
            if ($scope.editData.StartDate && $scope.editData.EndDate) {
                var fDT = $scope.editData.StartDate.split('-');
                var tDT = $scope.editData.EndDate.split('-');

                var fDate = new Date(parseInt(fDT[2]), parseInt(fDT[1]) - 1, parseInt(fDT[0]));
                var tDate = new Date(parseInt(tDT[2]), parseInt(tDT[1]) - 1, parseInt(tDT[0]));

                if (fDate > tDate) {
                    return true;
                }
            }
            return false;
        };

        $scope.StringToDateString = function (dtValue) {
            var dt = dtValue.split('-');
            return dt[2] + "-" + dt[1] + "-" + dt[0] + "T00:00:00";
        };

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

        // END DATE PICKER

        /*reset the form*/
        function ResetForm() {
            $scope.editData = {
                AMCId: 0,
                StartDate: "",
                EndDate: "",
                Remarks: ""
            };
            $scope.amcform.$setPristine();
            $scope.selection = [];
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }

        /* getting list of Machine*/
        function loadMachines() {
            SRAmcService.RetrieveSRMachine().then(function (result) {
                $scope.MachineList = result.data.DataList;
            });
        };
        loadMachines();

        /*add new SR-AMC*/
        $scope.AddSRAmc = function () {
            $rootScope.isFormVisible = true;
            $scope.mode = "Add";
            $scope.saveText = "Save";
            ResetForm(); //reset form
        };

        /*save SR-AMC*/
        $scope.CreateUpdateSRAmc = function (doc) {

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
                var IDate = $filter('date')(doc.StartDate, 'dd-MM-yyyy').split('-');
                var temp_IssueDate = new Date(parseInt(IDate[2]), parseInt(IDate[1]) - 1, parseInt(IDate[0]), 0, 0, 0);
                doc.StartDate = $filter('date')(temp_IssueDate, 'MM-dd-yyyy HH:mm:ss');

                IDate = $filter('date')(doc.EndDate, 'dd-MM-yyyy').split('-');
                var temp_IssueDate = new Date(parseInt(IDate[2]), parseInt(IDate[1]) - 1, parseInt(IDate[0]), 0, 0, 0);
                doc.EndDate = $filter('date')(temp_IssueDate, 'MM-dd-yyyy HH:mm:ss');

                SRAmcService.CreateUpdateSRAmc(doc, selectedMachines).then(function (result) {
                    if (result.data.IsValidUser) {
                        if (result.data.MessageType == 1) { // 1:Success
                            toastr.success(result.data.Message, 'Success');
                            $scope.editData = {
                                AMCId: 0,
                                StartDate: "",
                                EndDate: "",
                                Remarks: ""
                            };
                            $scope.amcform.$setPristine();
                            $scope.selection = [];
                            $('input[name="chkMachine"]').each(function () {
                                $(this).prop('checked', false);
                            });

                            $scope.RefreshTable();
                            $scope.storage.lastRecord = {};
                            $scope.isFirstFocus = false;
                            $scope.amcform.$setPristine();
                            if ($scope.mode === "Edit") {
                                $rootScope.isFormVisible = false;
                                $scope.saveText = "Save";
                            }
                            loadMachines();
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
        $scope.CloseSRAmc = function () {
            $scope.mode = "Add";
            $rootScope.isFormVisible = false;
            ResetForm();
        };

        /*reset the form*/
        $scope.ResetSRAmc = function () {
            if ($scope.mode == "Edit") {
                $scope.editData.AMCId = $scope.storage.lastRecord.AMCId;
                $scope.editData.StartDate = $filter('date')($scope.storage.lastRecord.StartDate, 'dd-MM-yyyy')
                $scope.editData.EndDate = $filter('date')($scope.storage.lastRecord.EndDate, 'dd-MM-yyyy')
                $scope.editData.Remarks = $scope.storage.lastRecord.Remarks;
            } else { //mode == add
                ResetForm();
            }
        };

        /*get record for edit SR-AMC*/
        $scope.UpdateSRAmc = function (_amc) {

            SRAmcService.RetrieveSRMachine().then(function (result) {
                $scope.MachineList = result.data.DataList;
                var new_obj = { 'Id': _amc.MachineId, 'Label': _amc.MachineName };
                $scope.MachineList.push(new_obj);
                $scope.selectedList = [{ Id: _amc.MachineId, Label: _amc.MachineName }];
            })

            $scope.storage.lastRecord = _amc;
            $rootScope.isFormVisible = true;
            $scope.mode = "Edit";
            $scope.saveText = "Update";

            $scope.editData.AMCId = _amc.AMCId;
            $scope.startDate = _amc.StartDate;
            $scope.editData.StartDate = $filter('date')(_amc.StartDate, 'dd-MM-yyyy');
            $scope.endDate = _amc.EndDate;
            $scope.editData.EndDate = $filter('date')(_amc.EndDate, 'dd-MM-yyyy');
            $scope.editData.Remarks = _amc.Remarks;
            
            $scope.amcform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        };

        /*delete SR-AMC*/
        $scope.DeleteSRAmc = function (id) {
            $rootScope.IsAjaxLoading = true;
            SRAmcService.DeleteSRAmc(id).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) {  //1:Success
                        toastr.success(result.data.Message, 'Success');
                        loadMachines();
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
            document.location.href = "../../Handler/SRAmc.ashx?timezone=" + $scope.timeZone
        };

        /*datatable*/
        $scope.RetrieveSRAmc = function () {
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
                    SRAmcService.GetSRAmcList($scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().MachineName).then(function (result) {
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

