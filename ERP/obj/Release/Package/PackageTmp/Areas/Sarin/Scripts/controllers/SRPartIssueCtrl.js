/// <reference path="../libs/angular/angular.min.js" />


(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("SRPartIssueCtrl", [
            "$scope", "$rootScope", "$timeout", "SRPartIssueService", "$http", "$filter", "ngTableParams",
            SRPartIssueCtrl
        ]);


    //Main controller function
    function SRPartIssueCtrl($scope, $rootScope, $timeout, SRPartIssueService, $http, $filter, ngTableParams) {
        $scope.editData = $scope.editData || {};
        $scope.mode = "Add";
        $scope.saveText = "Save";
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes
        $scope.isFocusFirst = false;
        $scope.storage = { lastRecord: "" };
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

        $scope.$watch('editdata.IssuedDate', function (newvalue) {
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
                $scope.partissueform.txtIssueDate.$setValidity("invalidIssueDate", true);
                return;
            } else if (issueDate.length == 10) {
                if ($scope.ValidateDate(issueDate)) {
                    $scope.issueDate = $scope.StringToDateString(issueDate);
                    $scope.partissueform.txtIssueDate.$setValidity("invalidIssueDate", true);
                } else {
                    $scope.partissueform.txtIssueDate.$setValidity("invalidIssueDate", false);
                }
            } else {
                $scope.partissueform.txtIssueDate.$setValidity("invalidIssueDate", false);
            }
        };

        $scope.$watch('issueDate', function (newValue) {
            $scope.editData.IssuedDate = $filter('date')(newValue, 'dd-MM-yyyy');
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
                PartIssueId: 0,
                MachineId: 0,
                PartId: 0,
                IssuedFrom: "",
                ChallanNo: "",
                IssuedDate: "",
                Problem: "",
                Remarks: ""
            };
            $scope.partissueform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }

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

        /*validate Issue From dropdown*/
        $scope.validateDropIssueFrom = function () {
            if ($scope.editData.IssuedFrom && $scope.editData.IssuedFrom != 0) return false;
            return true;
        };

        /* getting list of machine*/
        function loadMachine() {
            SRPartIssueService.RetrieveSRMachine().then(function (result) {
                $scope.SRMachines = result.data.DataList;
            });
        };
        loadMachine();

        /* getting list of Parts*/
        function loadParts() {
            SRPartIssueService.RetrieveSRParts().then(function (result) {
                $scope.SRParts = result.data.DataList;
            });
        };
        loadParts();

        /*add new SR-Part Issue*/
        $scope.AddSRPartIssue = function () {
            $rootScope.isFormVisible = true;
            $scope.mode = "Add";
            $scope.saveText = "Save";
            ResetForm(); //reset form
        };

        /*save Part-Issue*/
        $scope.CreateUpdateSRPartIssue = function (doc) {

            var IDate = $filter('date')(doc.IssuedDate, 'dd-MM-yyyy').split('-');
            var temp_IssueDate = new Date(parseInt(IDate[2]), parseInt(IDate[1]) - 1, parseInt(IDate[0]), 0, 0, 0);
            doc.IssuedDate = $filter('date')(temp_IssueDate, 'MM-dd-yyyy HH:mm:ss');

            SRPartIssueService.CreateUpdateSRPartIssue(doc).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.editData = {
                            PartIssueId: 0,
                            MachineId: 0,
                            PartId: 0,
                            IssuedFrom: "",
                            ChallanNo: "",
                            IssuedDate: "",
                            Problem: "",
                            Remarks: ""
                        };
                        $scope.RefreshTable();
                        $scope.storage.lastRecord = {};
                        $scope.isFirstFocus = false;
                        $scope.partissueform.$setPristine();
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
        $scope.CloseSRPartIssue = function () {
            $scope.mode = "Add";
            $rootScope.isFormVisible = false;
            ResetForm();
        };

        /*reset the form*/
        $scope.ResetSRPartIssue = function () {
            if ($scope.mode == "Edit") {
                $scope.editData.PartIssueId = $scope.storage.lastRecord.PartIssueId;
                $scope.editData.MachineId = $scope.storage.lastRecord.MachineId;
                $scope.editData.PartId = $scope.storage.lastRecord.PartId;
                $scope.editData.IssuedFrom = $scope.storage.lastRecord.IssuedFrom;
                $scope.editData.ChallanNo = $scope.storage.lastRecord.ChallanNo;
                $scope.editData.IssuedDate = $filter('date')($scope.storage.lastRecord.IssuedDate, 'dd-MM-yyyy')
                $scope.editData.Problem = $scope.storage.lastRecord.Problem;
                $scope.editData.Remarks = $scope.storage.lastRecord.Remarks;
            } else { //mode == add
                ResetForm();
            }
        };

        /*get record for edit Part-Issue*/
        $scope.UpdateSRPartIssue = function (_ps) {
            $scope.storage.lastRecord = _ps;
            $rootScope.isFormVisible = true;
            $scope.mode = "Edit";
            $scope.saveText = "Update";

            $scope.editData.PartIssueId = _ps.PartIssueId;
            $scope.editData.MachineId = _ps.MachineId;
            $scope.editData.PartId = _ps.PartId;
            $scope.editData.IssuedFrom = _ps.IssuedFrom;
            $scope.editData.ChallanNo = _ps.ChallanNo;
            $scope.issueDate = _ps.IssuedDate;
            $scope.editData.IssuedDate = $filter('date')(_ps.IssuedDate, 'dd-MM-yyyy');
            $scope.editData.Problem = _ps.Problem;
            $scope.editData.Remarks = _ps.Remarks;

            $scope.partissueform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        };

        /*delete Part-Issue*/
        $scope.DeleteSRPartIssue = function (id) {
            $rootScope.IsAjaxLoading = true;
            SRPartIssueService.DeleteSRPartIssue(id).then(function (result) {
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
            document.location.href = "../../Handler/SRPartIssue.ashx?timezone=" + $scope.timeZone
        };

        /*datatable*/
        $scope.RetrieveSRPartIssue = function () {
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
                    SRPartIssueService.GetSRPartIssueList($scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().MachineName, params.filter().PartName, params.filter().IssuedFrom, params.filter().ChallanNo, params.filter().Problem).then(function (result) {
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

