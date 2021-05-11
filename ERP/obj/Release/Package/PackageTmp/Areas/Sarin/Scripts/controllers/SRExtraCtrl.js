/// <reference path="../libs/angular/angular.min.js" />


(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("SRExtraCtrl", [
            "$scope", "$rootScope", "$timeout", "SRExtraService", "$http", "$filter", "ngTableParams",
            SRSubTypeCtrl
        ]);


    //Main controller function
    function SRSubTypeCtrl($scope, $rootScope, $timeout, SRExtraService, $http, $filter, ngTableParams) {
        $scope.editData = $scope.editData || {};
        $scope.mode = "Add";
        $scope.saveText = "Save";
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes
        $scope.isFocusFirst = false;
        $scope.storage = { lastRecord: "" };
        $scope.FilterType = "";
        $scope.filterDate = {
            dateRange: { startDate: "", endDate: "" }
        };
        $('#txtRemark').elastic();

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

        $scope.$watch('editdata.ExtraDate', function (newvalue) {
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
                $scope.extraform.txtIssueDate.$setValidity("invalidIssueDate", true);
                return;
            } else if (issueDate.length == 10) {
                if ($scope.ValidateDate(issueDate)) {
                    $scope.issueDate = $scope.StringToDateString(issueDate);
                    $scope.extraform.txtIssueDate.$setValidity("invalidIssueDate", true);
                } else {
                    $scope.extraform.txtIssueDate.$setValidity("invalidIssueDate", false);
                }
            } else {
                $scope.extraform.txtIssueDate.$setValidity("invalidIssueDate", false);
            }
        };

        $scope.$watch('issueDate', function (newValue) {
            $scope.editData.ExtraDate = $filter('date')(newValue, 'dd-MM-yyyy');
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
                ExtraId: 0,
                Type: 0,
                MachineNo: "",
                ExtraDate: "",
                Remark: ""
            };
            $scope.extraform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }


        /*validate Repaired By*/
        $scope.validateDropType = function () {
            if ($scope.editData.Type && $scope.editData.Type != 0) return false;
            return true;
        };

        /*add new SR-Extra*/
        $scope.AddSRExtra = function () {
            $rootScope.isFormVisible = true;
            $scope.mode = "Add";
            $scope.saveText = "Save";
            ResetForm(); //reset form
        };

        /*save sub type*/
        $scope.CreateUpdateSRExtra = function (doc) {

            var IDate = $filter('date')(doc.ExtraDate, 'dd-MM-yyyy').split('-');
            var temp_IssueDate = new Date(parseInt(IDate[2]), parseInt(IDate[1]) - 1, parseInt(IDate[0]), 0, 0, 0);
            doc.ExtraDate = $filter('date')(temp_IssueDate, 'MM-dd-yyyy HH:mm:ss');

            SRExtraService.CreateUpdateSRExtra(doc).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.editData = {
                            SubTypeId: 0,
                            TypeId: 0,
                            SubTypeName: "",
                            Remarks: ""
                        };
                        $scope.RefreshTable();
                        $scope.storage.lastRecord = {};
                        $scope.isFirstFocus = false;
                        $scope.extraform.$setPristine();
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
        $scope.CloseSRExtra = function () {
            $scope.mode = "Add";
            $rootScope.isFormVisible = false;
            ResetForm();
        };

        /*reset the form*/
        $scope.ResetSRExtra = function () {
            if ($scope.mode == "Edit") {
                $scope.editData.ExtraId = $scope.storage.lastRecord.ExtraId;
                $scope.editData.Type = $scope.storage.lastRecord.Type;
                $scope.editData.MachineNo = $scope.storage.lastRecord.MachineNo;
                $scope.editData.ExtraDate = $filter('date')($scope.storage.lastRecord.ExtraDate, 'dd-MM-yyyy')
                $scope.editData.Remark = $scope.storage.lastRecord.Remark;
            } else { //mode == add
                ResetForm();
            }
        };

        /*cancel button click event*/
        $scope.SRExtra = function () {
            $scope.mode = "Add";
            ResetForm();
            $rootScope.isFormVisible = false;
        };

        /*get record for edit sub type*/
        $scope.UpdateSRExtra = function (_ex) {
            $scope.storage.lastRecord = _ex;
            $rootScope.isFormVisible = true;
            $scope.mode = "Edit";
            $scope.saveText = "Update";

            $scope.editData.ExtraId = _ex.ExtraId;
            $scope.editData.Type = _ex.Type;
            $scope.editData.MachineNo = _ex.MachineNo;
            $scope.issueDate = _ex.ExtraDate;
            $scope.editData.ExtraDate = $filter('date')(_ex.ExtraDate, 'dd-MM-yyyy');
            $scope.editData.ExtraDate = _ex.ExtraDate;
            $scope.editData.Remark = _ex.Remark;
            $scope.extraform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        };

        /*delete SR-Extra*/
        $scope.DeleteSRExtra = function (id) {
            $rootScope.IsAjaxLoading = true;
            SRExtraService.DeleteSRExtra(id).then(function (result) {
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
            document.location.href = "../../Handler/SRExtra.ashx?timezone=" + $scope.timeZone
        };

        /*datatable*/
        $scope.RetrieveSRExtra = function () {
            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,
                sorting: {
                    MachineNo: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;
                    SRExtraService.GetSRExtraList($scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().TypeName, params.filter().MachineNo, $scope.FilterType, $scope.filterDate.dateRange.startDate, $scope.filterDate.dateRange.endDate).then(function (result) {
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

        $scope.clearDateBox = function (e) {
            var target = $(e.target).parent().find("input[type=text]").val("");
            $scope.filterDate = {
                dateRange: { startDate: "", endDate: "" }
            };
        };

        $scope.FilterData = function () {
            $scope.RefreshTable();
        };

    };


})();

