/// <reference path="../libs/angular/angular.min.js" />


(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("SRRepairCtrl", [
            "$scope", "$rootScope", "$timeout", "SRRepairService", "$http", "$filter", "ngTableParams",
            SRRepairCtrl
        ]);


    //Main controller function
    function SRRepairCtrl($scope, $rootScope, $timeout, SRRepairService, $http, $filter, ngTableParams) {
        $scope.editData = $scope.editData || {};
        $scope.mode = "Add";
        $scope.saveText = "Save";
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes
        $scope.isFocusFirst = false;
        $scope.storage = { lastRecord: "" };
        $scope.isOther = true;
        $('#txtRemarks').elastic();

        $scope.$watch('editData.RepairedBy', function (newValue) {
            if (newValue == "O") {
                $scope.isOther = true;
            }
            else {
                $scope.isOther = false;
            }
        });

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
        $scope.issueDate = new Date();

        $scope.calendarOpenIssueDate = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.calOpenIssueDate = true;
            $scope.calOpenReceiveDate = false;
        };

        $scope.calendarOpenReceiveDate = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.calOpenIssueDate = false;
            $scope.calOpenReceiveDate = true;
        };

        $scope.$watch('issueDate', function (newValue) {
            $scope.editData.IssueDate = $filter('date')(newValue, 'dd-MM-yyyy');
            $scope.minDate = newValue;
            if ($scope.IsIssueDateBig()) {
                $scope.editData.ReceiveDate = $scope.editData.IssueDate;
                $scope.receiveDate = newValue;
            }
        });
        $scope.$watch('receiveDate', function (newValue) {
            $scope.editData.ReceiveDate = $filter('date')(newValue, 'dd-MM-yyyy');
        });

        $scope.ValidateIssueDate = function (issueDate) {
            if (issueDate.length == 10) {
                if ($scope.ValidateDate(issueDate)) {
                    $scope.issueDate = $scope.StringToDateString(issueDate);
                    $scope.repairform.txtIssueDate.$setValidity("invalidIssueDate", true);

                    if ($scope.IsIssueDateBig()) {
                        $scope.editData.ReceiveDate = issueDate;
                        $scope.receiveDate = $scope.StringToDateString(issueDate);
                    }
                } else {
                    $scope.repairform.txtIssueDate.$setValidity("invalidIssueDate", false);
                }
            } else {
                $scope.repairform.txtIssueDate.$setValidity("invalidIssueDate", false);
            }
        };

        //$scope.ValidateReceiveDate = function (receiveDate) {
        //    if (receiveDate.length == 10) {
        //        if ($scope.ValidateDate(receiveDate)) {
        //            $scope.receiveDate = $scope.StringToDateString(receiveDate);
        //            $scope.repairform.txtReceiveDate.$setValidity("invalidReceiveDate", true);

        //            if ($scope.IsIssueDateBig()) {
        //                $scope.editData.ReceiveDate = $scope.editData.IssueDate;
        //                $scope.receiveDate = $scope.StringToDateString($scope.editData.IssueDate);
        //            }
        //        } else {
        //            $scope.repairform.txtReceiveDate.$setValidity("invalidReceiveDate", false);
        //        }
        //    } else {
        //        //$scope.repairform.txtReceiveDate.$setValidity("invalidReceiveDate", false);
        //    }
        //};

        $scope.ValidateReceiveDate = function (receiveDate) {
            if (!receiveDate) {
                $scope.repairform.txtReceiveDate.$setValidity("invalidReceiveDate", true);
                return;
            } else if (receiveDate.length == 10) {
                if ($scope.ValidateDate(receiveDate)) {
                    $scope.receiveDate = $scope.StringReceiveDateString(receiveDate);
                    $scope.repairform.txtReceiveDate.$setValidity("invalidReceiveDate", true);

                    if ($scope.IsIssueDateBig()) {
                        $scope.editData.ReceiveDate = $scope.editData.IssueDate;
                        $scope.receiveDate = $scope.StringReceiveDateString($scope.editData.IssueDate);
                    }
                } else {
                    $scope.repairform.txtReceiveDate.$setValidity("invalidReceiveDate", false);
                }
            } else {
                $scope.repairform.txtReceiveDate.$setValidity("invalidReceiveDate", false);
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

        $scope.IsIssueDateBig = function () {
            if ($scope.editData.IssueDate && $scope.editData.ReceiveDate) {
                var fDT = $scope.editData.IssueDate.split('-');
                var tDT = $scope.editData.ReceiveDate.split('-');

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
                RepairId: 0,
                MachineId: 0,
                PartId: 0,
                Problem: "",
                RepairedBy: "",
                Others: "",
                IssueDate: "",
                ReceiveDate: "",
                Remarks: ""
            };
            $scope.repairform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }

        /* getting list of machine*/
        function loadMachine() {
            SRRepairService.RetrieveSRMachine().then(function (result) {
                $scope.SRMachines = result.data.DataList;
            });
        };
        loadMachine();

        /* getting list of parts*/
        function loadParts() {
            SRRepairService.RetrieveSRPart().then(function (result) {
                $scope.SRParts = result.data.DataList;
            });
        };
        loadParts();

        /*validate machine dropdown*/
        $scope.validateDropMachine = function () {
            if ($scope.editData.MachineId && $scope.editData.MachineId != 0) return false;
            return true;
        };

        /*validate Part dropdown*/
        $scope.validateDropPart = function () {
            if ($scope.editData.PartId && $scope.editData.PartId != 0) return false;
            return true;
        };
        /*validate Repaired By*/
        $scope.validateDropRepairBy = function () {
            if ($scope.editData.RepairedBy && $scope.editData.RepairedBy != 0) return false;
            return true;
        };

        /*add new SR-Repair*/
        $scope.AddSRRepair = function () {
            $rootScope.isFormVisible = true;
            $scope.mode = "Add";
            $scope.saveText = "Save";
            ResetForm(); //reset form
        };

        /*save Sr-Repair*/
        $scope.CreateUpdateSRRepair = function (doc) {

            var IDate = $filter('date')(doc.IssueDate, 'dd-MM-yyyy').split('-');
            var temp_IssueDate = new Date(parseInt(IDate[2]), parseInt(IDate[1]) - 1, parseInt(IDate[0]), 0, 0, 0);
            doc.IssueDate = $filter('date')(temp_IssueDate, 'MM-dd-yyyy HH:mm:ss');

            if (doc.ReceiveDate) {
                IDate = $filter('date')(doc.ReceiveDate, 'dd-MM-yyyy').split('-');
                var temp_IssueDate = new Date(parseInt(IDate[2]), parseInt(IDate[1]) - 1, parseInt(IDate[0]), 0, 0, 0);
                doc.ReceiveDate = $filter('date')(temp_IssueDate, 'MM-dd-yyyy HH:mm:ss');
            }
            SRRepairService.CreateUpdateSRRepair(doc).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.editData = {
                            RepairId: 0,
                            MachineId: 0,
                            PartId: 0,
                            Problem: "",
                            RepairedBy: "",
                            Others: "",
                            IssueDate: "",
                            ReceiveDate: "",
                            Remarks: ""
                        };
                        $scope.RefreshTable();
                        $scope.storage.lastRecord = {};
                        $scope.isFirstFocus = false;
                        $scope.repairform.$setPristine();
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
        $scope.CloseSRRepair = function () {
            $scope.mode = "Add";
            $rootScope.isFormVisible = false;
            ResetForm();
        };

        /*reset the form*/
        $scope.ResetSRRepair = function () {
            if ($scope.mode == "Edit") {

                $scope.editData.RepairId = $scope.storage.lastRecord.RepairId;
                $scope.editData.MachineId = $scope.storage.lastRecord.MachineId;
                $scope.editData.PartId = $scope.storage.lastRecord.PartId;
                $scope.editData.Problem = $scope.storage.lastRecord.Problem;
                $scope.editData.RepairedBy = $scope.storage.lastRecord.RepairedBy;
                $scope.editData.Others = $scope.storage.lastRecord.Others;
                $scope.editData.IssueDate = $filter('date')($scope.storage.lastRecord.IssueDate, 'dd-MM-yyyy')
                $scope.editData.ReceiveDate = $filter('date')($scope.storage.lastRecord.ReceiveDate, 'dd-MM-yyyy')
                $scope.editData.Remarks = $scope.storage.lastRecord.Remarks;

            } else { //mode == add
                ResetForm();
            }
        };

        /*get record for edit SR-Repair*/
        $scope.UpdateSRRepair = function (_rep) {
            $scope.storage.lastRecord = _rep;
            $rootScope.isFormVisible = true;
            $scope.mode = "Edit";
            $scope.saveText = "Update";

            $scope.editData.RepairId = _rep.RepairId;
            $scope.editData.MachineId = _rep.MachineId;
            $scope.editData.PartId = _rep.PartId;
            $scope.editData.Problem = _rep.Problem;
            $scope.editData.RepairedBy = _rep.RepairedBy;
            $scope.editData.Others = _rep.Others;
            $scope.issueDate = _rep.IssueDate;
            $scope.editData.IssueDate = $filter('date')(_rep.IssueDate, 'dd-MM-yyyy')
            $scope.receiveDate = _rep.ReceiveDate;
            $scope.editData.ReceiveDate = $filter('date')(_rep.ReceiveDate, 'dd-MM-yyyy')
            $scope.editData.Remarks = _rep.Remarks;

            $scope.repairform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        };

        /*delete SR-Repair*/
        $scope.DeleteSRRepair = function (id) {
            $rootScope.IsAjaxLoading = true;
            SRRepairService.DeleteSRRepair(id).then(function (result) {
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
            document.location.href = "../../Handler/SRRepair.ashx?timezone=" + $scope.timeZone
        };

        /*datatable*/
        $scope.RetrieveSRRepair = function () {
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
                    SRRepairService.GetSRRepairList($scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().MachineName, params.filter().PartName, params.filter().Problem, params.filter().RepairMansName, params.filter().SerialNo).then(function (result) {
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

