/// <reference path="../libs/angular/angular.min.js" />


(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("EmpPayRollCtrl", [
            "$scope", "$rootScope", "$timeout", "EmployeeCreateService", "$http", "$filter",
            empPayRollCtrl
        ]);


    //Main controller function
    function empPayRollCtrl($scope, $rootScope, $timeout, EmployeeCreateService, $http, $filter) {
        $scope.payRoll = $scope.payRoll || {};
        $scope.saveText = "Save";
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes
        $scope.isFocusFirst = false;
        $scope.storage = { lastRecord: "" };
        $scope.payRoll.IsActive = true;

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

        $scope.calendarOpenJoinDate = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.calendarOpenedJoinDate = true;
            $scope.calendarOpenedRelDate = false;
            $scope.calendarOpenedPerDate = false;
        };

        $scope.calendarOpenRelDate = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.calendarOpenedRelDate = true;
            $scope.calendarOpenedJoinDate = false;
            $scope.calendarOpenedPerDate = false;
        };

        $scope.calendarOpenPerDate = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.calendarOpenedPerDate = true;
            $scope.calendarOpenedRelDate = false;
            $scope.calendarOpenedJoinDate = false;
        };

        $scope.$watch('JoiningDate', function (newValue) {
            $scope.payRoll.JoiningDate = $filter('date')(newValue, 'dd-MM-yyyy');
            $scope.empPform.txtPayJoiningFrom.$setValidity("invalidJoinDate", true);
        });

        $scope.$watch('releavingDate', function (newValue) {
            $scope.payRoll.ReLeavingDate = $filter('date')(newValue, 'dd-MM-yyyy');
            $scope.empPform.txtPayReLeavingFrom.$setValidity("invalidRelDate", true);
        });

        $scope.$watch('permanentDate', function (newValue) {
            $scope.payRoll.PermanentFromDate = $filter('date')(newValue, 'dd-MM-yyyy');
            $scope.empPform.txtPayPermanentFrom.$setValidity("invalidPerDate", true);
        });

        $scope.ValidateJoinDate = function (jDate) {
            if (!jDate) {
                $scope.empPform.txtPayJoiningFrom.$setValidity("invalidJoinDate", true);
                return;
            } else if (jDate.length == 10) {
                if ($scope.ValidateDate(jDate)) {
                    $scope.empPform.txtPayJoiningFrom.$setValidity("invalidJoinDate", true);
                    var dt = jDate.split('-');
                    $scope.joinDate = dt[2] + "-" + dt[1] + "-" + dt[0] + "T00:00:00";
                } else {
                    $scope.empPform.txtPayJoiningFrom.$setValidity("invalidJoinDate", false);
                }
            } else {
                $scope.empPform.txtPayJoiningFrom.$setValidity("invalidJoinDate", false);
            }
        };

        $scope.ValidateReleavingDate = function (rDate) {
            if (!rDate) {
                $scope.empPform.txtPayReLeavingFrom.$setValidity("invalidRelDate", true);
                return;
            } else if (rDate.length == 10) {
                if ($scope.ValidateDate(rDate)) {
                    $scope.empPform.txtPayReLeavingFrom.$setValidity("invalidRelDate", true);
                    var dt = rDate.split('-');
                    $scope.releavingDate = dt[2] + "-" + dt[1] + "-" + dt[0] + "T00:00:00";
                }
                else {
                    $scope.empPform.txtPayReLeavingFrom.$setValidity("invalidRelDate", false);
                }
            } else {
                $scope.empPform.txtPayReLeavingFrom.$setValidity("invalidRelDate", false);
            }
        };

        $scope.ValidatePermanentDate = function (pDate) {
            if (!pDate) {
                $scope.empPform.txtPayPermanentFrom.$setValidity("invalidPerDate", true);
                return;
            } else if (pDate.length == 10) {
                if ($scope.ValidateDate(pDate)) {
                    $scope.empPform.txtPayPermanentFrom.$setValidity("invalidPerDate", true);
                    var dt = pDate.split('-');
                    $scope.permanentDate = dt[2] + "-" + dt[1] + "-" + dt[0] + "T00:00:00";
                } else {
                    $scope.empPform.txtPayPermanentFrom.$setValidity("invalidPerDate", false);
                }
            } else {
                $scope.empPform.txtPayPermanentFrom.$setValidity("invalidPerDate", false);
            }
        };

        //END DATE PICKER

        /*validate dropdown*/
        $scope.validateDropEmpStatus = function () {
            if ($scope.payRoll.EmploymentStatus && $scope.payRoll.EmploymentStatus != 0) return false;
            return true;
        };
        $scope.validateDropSalaryBasedOn = function () {
            if ($scope.payRoll.SalaryBasedOn && $scope.payRoll.SalaryBasedOn != 0) return false;
            return true;
        };


        $scope.validateDropGroupName = function () {
            if ($scope.payRoll.GroupName && $scope.payRoll.GroupName != 0) return false;
            return true;
        };

        /*save payroll information*/
        $scope.CreateUpdateEmpPayRoll = function (q) {
            var JoinDate = $filter('date')(q.JoiningDate, 'dd-MM-yyyy').split('-');
            var temp_JoinDate = new Date(parseInt(JoinDate[2]), parseInt(JoinDate[1]) - 1, parseInt(JoinDate[0]), 0, 0, 0);
            q.JoiningDate = $filter('date')(temp_JoinDate, 'MM-dd-yyyy HH:mm:ss');

            var PerDate = $filter('date')(q.PermanentFromDate, 'dd-MM-yyyy').split('-');
            var temp_PreDate = new Date(parseInt(PerDate[2]), parseInt(PerDate[1]) - 1, parseInt(PerDate[0]), 0, 0, 0);
            q.PermanentFromDate = $filter('date')(temp_PreDate, 'MM-dd-yyyy HH:mm:ss');

            if ($filter('date')(q.ReLeavingDate, 'dd-MM-yyyy')) {
                var RelivingDate = $filter('date')(q.ReLeavingDate, 'dd-MM-yyyy').split('-');
                var temp_RelivingDate = new Date(parseInt(RelivingDate[2]), parseInt(RelivingDate[1]) - 1, parseInt(RelivingDate[0]), 0, 0, 0);
                q.ReLeavingDate = $filter('date')(temp_RelivingDate, 'MM-dd-yyyy HH:mm:ss');
            }

            $scope.payRoll.EmployeeId = $scope.master.EmployeeId;
            EmployeeCreateService.CreateUpdatePayRoll(q).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.saveText = "Update";
                        $scope.payRoll.PayRollId = result.data.DataList;
                        SetFocus();
                        $scope.empPform.$setPristine();
                        $scope.FetchPayRollInfo();
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
        };

        /*fetch payroll information to display in form*/
        $scope.FetchPayRollInfo = function () {
            EmployeeCreateService.FetchPayRollInfo($scope.master.EmployeeId).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 0) { // 0:Error
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    } else if (result.data.MessageType == 1) { //1:Success
                        if (result.data.DataList.length > 0) {
                            $scope.saveText = "Update";
                            $scope.storage.lastRecord = result.data.DataList[0];
                            BindData(result.data.DataList[0]);
                        }
                    }
                }
                else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;
            });
        };

        /*reset the form*/
        $scope.ResetPayRollInfo = function () {
            if ($scope.storage.lastRecord) {
                BindData($scope.storage.lastRecord);
            } else {
                ResetForm();
            }
        };

        function ResetForm() {
            $scope.payRoll = {
                PayRollId: 0,
                CTC: "",
                BasicSalary: "",
                EmploymentTax: "",
                ESIC: "",
                LeavesAllowedPerYear: "",
                PFAccountNumber: "",
                PF: "",
                CompanyBankName: "",
                CompanyBankAccount: "",
                PersonalBankName: "",
                PersonalBankAccount: "",
                AllocatedPassNo: "",
                JoiningDate: $filter('date')(new Date(), 'dd-MM-yyyy'),
                ReLeavingDate: $filter('date')(new Date(), 'dd-MM-yyyy'),
                EmploymentStatus: "",
                PermanentFromDate: $filter('date')(new Date(), 'dd-MM-yyyy'),
                GroupName: "",
                IsActive: true
            };
            $scope.joinDate = $scope.StringToDateString($scope.payRoll.JoiningDate);;
            $scope.releavingDate = $scope.StringToDateString($scope.payRoll.ReLeavingDate);;
            $scope.permanentDate = $scope.StringToDateString($scope.payRoll.PermanentFromDate);;


            $scope.issueDate = $scope.StringToDateString($scope.editData.IssueDate);


            $scope.empPform.$setPristine();
            SetFocus();
        }

        function BindData(p) {
            console.log("Sample Applasjhasdfasdfasdrui");
            console.log(p);
            $scope.payRoll = {
                PayRollId: p.PayRollId,
                CTC: p.CTC,
                BasicSalary: p.BasicSalary,
                EmploymentTax: p.EmploymentTax != 0 ? p.EmploymentTax : "",
                ESIC: p.ESIC != 0 ? p.ESIC : "",
                LeavesAllowedPerYear: p.LeavesAllowedPerYear,
                PFAccountNumber: p.PFAccountNumber,
                PF: p.PF != 0 ? p.PF : "",
                CompanyBankName: p.CompanyBankName,
                CompanyBankAccount: p.CompanyBankAccount,
                PersonalBankName: p.PersonalBankName,
                PersonalBankAccount: p.PersonalBankAccount,
                AllocatedPassNo: p.AllocatedPassNo,
                JoiningDate: $filter('date')(p.JoiningDate, 'dd-MM-yyyy'),
                ReLeavingDate: $filter('date')(p.ReLeavingDate, 'dd-MM-yyyy'),
                EmploymentStatus: p.EmploymentStatus,
                GroupName: p.GName,
                PermanentFromDate: $filter('date')(p.PermanentFromDate, 'dd-MM-yyyy'),
                IsActive: p.IsActive,
                SalaryBasedOn: p.SalaryBasedOn
            };
            //$scope.joinDate = $scope.StringToDateString(p.JoiningDate);
            //$scope.releavingDate = $scope.StringToDateString(p.ReLeavingDate);
            //$scope.permanentDate = $scope.StringToDateString(p.PermanentFromDate);
            SetFocus();
        }

        function SetFocus() {
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }

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

    };
})();

