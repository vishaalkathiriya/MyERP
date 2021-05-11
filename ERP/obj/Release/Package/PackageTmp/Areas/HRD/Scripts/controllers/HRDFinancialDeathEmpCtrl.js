/// <reference path="../libs/angular/angular.min.js" />


(function () {
    'use strict';
    angular.module("ERPApp.Controllers")
       .controller("FinancialDeathEmpCtrl", [
            "$scope", "$rootScope", "$timeout", "$http", "$filter", "FinancialDeathEmpService", "ngTableParams","$modal",
           financialDeathEmpCtrl
       ]);

    function financialDeathEmpCtrl($scope, $rootScope, $timeout, $http, $filter, FinancialDeathEmpService, ngTableParams, $modal) {
        $scope.editData = $scope.editData || {};
        $scope.mode = "Add";
        $scope.saveText = "Save";
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes
        $scope.isFocusFirst = false;
        $scope.storage = { lastRecord: "" };
        $scope.ischanged = false;

        $scope.filterDate = {
            dateRange: { startDate: "", endDate: "" }
        };


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

        $scope.calendarOpenDateOfDeath = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.editData.calOpenDateOfDeath = true;
            $scope.editData.calOpenIssueDate = false;
        };


        $scope.calendarOpenIssueDate = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.editData.calOpenIssueDate = true;
            $scope.editData.calOpenDateOfDeath = false;
        };


        $scope.$watch('dateOfDeath', function (newValue) {
            $scope.editData.DateOfDeath = $filter('date')(newValue, 'dd-MM-yyyy');
        });

        $scope.$watch('issueDate', function (newValue) {
            $scope.editData.IssueDate = $filter('date')(newValue, 'dd-MM-yyyy');
        });


        $scope.ValidateDateOfDeath = function (dateOfDeath, FinancialDeathEmpform) {
            if (!dateOfDeath) {
                $scope.FinancialDeathEmpform.txtDateOfDeath.$setValidity("invalidIssueDate", true);
                return;
            } else if (dateOfDeath.length == 10) {
                if ($scope.ValidateDate(dateOfDeath)) {
                    //test
                    $scope.dateOfDeath = $scope.StringToDateString(dateOfDeath);
                    $scope.FinancialDeathEmpform.txtDateOfDeath.$setValidity("invalidIssueDate", true);
                } else {
                    $scope.FinancialDeathEmpform.txtDateOfDeath.$setValidity("invalidIssueDate", false);
                }
            } else {
                $scope.FinancialDeathEmpform.txtDateOfDeath.$setValidity("invalidIssueDate", false);
            }
        };




        $scope.ValidateIssuedDate = function (issueDate, FinancialDeathEmpform) {
            if (!issueDate) {
                $scope.FinancialDeathEmpform.txtIssueDate.$setValidity("invalidIssueDate", true);
                return;
            } else if (issueDate.length == 10) {
                if ($scope.ValidateDate(issueDate)) {
                    //test
                    $scope.issueDate = $scope.StringToDateString(issueDate);
                    $scope.FinancialDeathEmpform.txtIssueDate.$setValidity("invalidIssueDate", true);
                } else {
                    $scope.FinancialDeathEmpform.txtIssueDate.$setValidity("invalidIssueDate", false);
                }
            } else {
                $scope.FinancialDeathEmpform.txtIssueDate.$setValidity("invalidIssueDate", false);
            }
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

        $scope.StringToDateString = function (dtValue) {
            if (dtValue) {
                var dt = dtValue.split('-');
                return dt[2] + "-" + dt[1] + "-" + dt[0] + "T00:00:00";
            }
            return dtValue;
        };

        $scope.maxDate = $scope.maxDate || new Date();
        // End DATE PICKER


        //BEGIN RESET FORM
        function ResetForm() {
            $scope.editData = {
                ECode: "", EmployeeName: "", ReceiveBy: "", Relation: "", Amount: "", ChequeNumber: "", FamilyBackgroundDetail: ""
            };

            $scope.editData.DateOfDeath = $filter('date')(new Date(), 'dd-MM-yyyy');
            $scope.dateOfDeath = $scope.StringToDateString($scope.editData.DateOfDeath);

            $scope.editData.IssueDate = $filter('date')(new Date(), 'dd-MM-yyyy');
            $scope.issueDate = $scope.StringToDateString($scope.editData.IssueDate);

            // $scope.socialWelExpDate = "";
            $scope.FinancialDeathEmpform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }
        //END RESET FORM



        //BEGIN ADD SOCIAL WELFARE EXPENSE FORM BUTTON
        $scope.AddFinancialDeathEmp = function () {
            $rootScope.isFormVisible = true;
            $scope.mode = "Add";
            $scope.saveText = "Save";
            ResetForm(); //reset form
            $scope.editData.DateOfDeath = $filter('date')(new Date(), 'dd-MM-yyyy');
            $scope.dateOfDeath = $scope.StringToDateString($scope.editData.DateOfDeath);

            $scope.editData.IssueDate = $filter('date')(new Date(), 'dd-MM-yyyy');
            $scope.issueDate = $scope.StringToDateString($scope.editData.IssueDate);
        };
        //END ADD SOCIAL WELFARE EXPENSE FORM BUTTON


        $scope.FilterByDeathDate = function (filters) {
            $scope.filters = filters;
            $scope.RefreshTable();
        };

        //BEGIN CLOSE SOCIAL WELFARE EXPENSE FORM
        $scope.CloseFinancialDeathEmp = function () {
            $scope.mode = "Add";
            ResetForm();
            $rootScope.isFormVisible = false;
        };
        //END CLOSE SOCIAL WELFARE EXPENSE FORM

        //BEGIN RESET SOCIAL WELFARE EXPENSE FORM
        $scope.ResetFinancialDeathEmp = function () {

            if ($scope.mode == "Edit") {
                $scope.editData = {
                    ECode: $scope.storage.lastRecord.Ecode,
                    EmployeeName: $scope.storage.lastRecord.EmployeeName,
                    DateOfDeath: $filter('date')($scope.storage.lastRecord.DateOfDeath, 'dd-MM-yyyy'),
                    ReceiveBy: $scope.storage.lastRecord.ReceiveBy,
                    Relation: $scope.storage.lastRecord.Relation,
                    Amount: $scope.storage.lastRecord.Amount,
                    ChequeNumber: $scope.storage.lastRecord.ChequeNumber,
                    IssueDate: $filter('date')($scope.storage.lastRecord.ChequeIssueDate, 'dd-MM-yyyy'),
                    FamilyBackgroundDetail: $scope.storage.lastRecord.FamilyBackgroundDetail
                };

            } else { //mode == add
                ResetForm();
            }
        };
        //BEGIN RESET SOCIAL WELFARE EXPENSE FORM


        //BEGIN SAVE SOCIAL WELFARE EXPENSE INFORMATION
        $scope.CreateUpdateFinancialDeathEmp = function (financialDeathEmp) {

            var DeathDate = $filter('date')(financialDeathEmp.DateOfDeath, 'dd-MM-yyyy').split('-');
            var temp_Death_Date = new Date(parseInt(DeathDate[2]), parseInt(DeathDate[1]) - 1, parseInt(DeathDate[0]), 0, 0, 0);
            financialDeathEmp.DateOfDeath = $filter('date')(temp_Death_Date, 'MM-dd-yyyy HH:mm:ss');

            var IDate = $filter('date')(financialDeathEmp.IssueDate, 'dd-MM-yyyy').split('-');
            var temp_Date = new Date(parseInt(IDate[2]), parseInt(IDate[1]) - 1, parseInt(IDate[0]), 0, 0, 0);
            financialDeathEmp.ChequeIssueDate = $filter('date')(temp_Date, 'MM-dd-yyyy HH:mm:ss');


            FinancialDeathEmpService.CreateUpdateFinancialDeathEmp(financialDeathEmp, $scope.timeZone).then(function (result) {

                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.editData = { ECode: "", EmployeeName: "", ReceiveBy: "", Relation: "", Amount: "", ChequeNumber: "", FamilyBackgroundDetail: "" };
                        $scope.editData.DateOfDeath = $filter('date')(new Date(), 'dd-MM-yyyy');
                        $scope.dateOfDeath = $scope.StringToDateString($scope.editData.DateOfDeath);
                        $scope.editData.IssueDate = $filter('date')(new Date(), 'dd-MM-yyyy');
                        $scope.issueDate = $scope.StringToDateString($scope.editData.IssueDate);
                        $scope.RefreshTable();
                        $scope.storage.lastRecord = {};
                        $scope.isFirstFocus = false;
                        $scope.FinancialDeathEmpform.$setPristine();
                        if ($scope.mode === "Edit") {
                            $rootScope.isFormVisible = false;
                            $scope.saveText = "Save";
                        }
                    } else if (result.data.MessageType == 2) { // 2:Warning
                        toastr.warning(result.data.Message, 'Record already exists');
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                }
                else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;
            });
        }
        //END SAVE MEDICAL INFORMATION

        //BEGIN REFERESH TABLE AFTER INSERT,UPDATE AND DELETE
        $scope.RefreshTable = function () {
            $scope.tableParams.reload();
        };
        //END REFERESH TABLE AFTER INSERT,UPDATE AND DELETE

        //BEGIN RETRIVE SOCIAL WELFARE EXPENSE INFORMATION (DATATABLE)   
        $scope.RetrieveFinacialDeathEmp = function () {
            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,
                sorting: {
                    Ecode: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;

                    FinancialDeathEmpService.GetFinancialDeathEmpList($scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().Ecode, params.filter().EmployeeName, params.filter().DateOfDeath, params.filter().Amount, params.filter().ChequeNumber, params.filter().ChequeIssueDate, params.filter().ReceiveBy, params.filter().Relation, params.filter().FamilyBackgroundDetail, $scope.filterDate.dateRange.startDate , $scope.filterDate.dateRange.endDate).then(function (result) {
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
        //END RETRIVE SOCIAL WELFARE EXPENSE INFORMATION (DATATABLE)   

        //BEGIN SET VALUES BEFORE UPDATE OPERATION PERFORM
        $scope.UpdateFinancialDeathEmp = function (_financialDeathEmp) {

            $scope.storage.lastRecord = _financialDeathEmp;
            $rootScope.isFormVisible = true;
            $scope.mode = "Edit";
            $scope.saveText = "Update";

            $scope.editData = {
                SrNo: _financialDeathEmp.SrNo,
                ECode: _financialDeathEmp.Ecode,
                EmployeeName: _financialDeathEmp.EmployeeName,
                DateOfDeath: $filter('date')(_financialDeathEmp.DateOfDeath, 'dd-MM-yyyy'),
                ReceiveBy: _financialDeathEmp.ReceiveBy,
                Relation: _financialDeathEmp.Relation,
                Amount: _financialDeathEmp.Amount,
                ChequeNumber: _financialDeathEmp.ChequeNumber,
                IssueDate: $filter('date')(_financialDeathEmp.ChequeIssueDate, 'dd-MM-yyyy'),
                FamilyBackgroundDetail: _financialDeathEmp.FamilyBackgroundDetail
            };
            //  $scope.editData.fileName = _socialWelExp.Attachment;
            // $scope.editData.Attachment = _socialWelExp.Attachment;
            $scope.issueDate = _financialDeathEmp.ChequeIssueDate;
            $scope.dateOfDeath = _financialDeathEmp.DateOfDeath;


            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        };
        //END SET VALUES BEFORE UPDATE OPERATION PERFORM



        //change service name and button name
        //BEGIN DELETE SOCIAL WELFARE EXPENSE INFORMATION
        $scope.DeleteFinancialDeathEmp = function (id) {
            $rootScope.IsAjaxLoading = true;
            FinancialDeathEmpService.DeleteFinancialDeathEmp(id).then(function (result) {
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
        //END DELETE SOCIAL WELFARE EXPENSE INFORMATION


        //BEGIN EXPORT TO EXCEL
        $scope.ExportToExcel = function () {
            document.location.href = "../../Handler/HRDFinancialDeathEmp.ashx?timezone=" + $scope.timeZone + "&startDate=" + $filter('date')($scope.filterDate.dateRange.startDate, 'dd-MM-yyyy') + "&endDate=" + $filter('date')($scope.filterDate.dateRange.endDate, 'dd-MM-yyyy')
        };
        //BEGIN EXPORT TO EXCEL



        ///Model PopUp
        $scope.ShowFinancialDeathEmp = function (FinancialDeathEmp) {
            var modalInstance = $modal.open({
                templateUrl: 'FinancialDeathEmpPopup.html',
                controller: ModalInstanceCtrl,
                resolve: {
                    FinancialDeathEmpDetails: function () { return FinancialDeathEmp; }
                }
            });
        }
        var ModalInstanceCtrl = function ($scope, $modalInstance, FinancialDeathEmpDetails) {
            $scope.items = FinancialDeathEmpDetails;
            $scope.Close = function () {
                $modalInstance.close();
            };
        };




    }


})();
