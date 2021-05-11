/// <reference path="../libs/angular/angular.min.js" />

(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("EmpCredentialCtrl", [
            "$scope", "$rootScope", "$timeout", "EmployeeCreateService", "$http", "$filter", "ngTableParams",
            empCredentialCtrl
        ]);

    //Main controller function
    function empCredentialCtrl($scope, $rootScope, $timeout, EmployeeCreateService, $http, $filter, ngTableParams) {
        $scope.compCred = $scope.compCred || {};

        $scope.saveText = "Save";
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes
        $scope.isFirstFocus = false;
        $scope.storage = { lastRecord: "" };

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
        $scope.maxDate = $scope.maxDate || new Date();

        $scope.calendarOpenBDate = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.calendarOpenedCreBirthDate = true;
        };

        $scope.$watch('CredBirthDate', function (newValue) {
            $scope.compCred.BirthDate = $filter('date')(newValue, 'dd-MM-yyyy');
            $scope.empCCform.txtCDOB.$setValidity("invalidBirthDate", true);
        });

        $scope.ValidateCreBirthDate = function (bDate) {
            if (!bDate) {
                $scope.empCCform.txtCDOB.$setValidity("invalidBirthDate", true);
                return;
            } else if (bDate.length == 10) {
                if ($scope.ValidateDate(bDate)) {
                    if ($scope.IsGreterThanToday(bDate)) {
                        $scope.empCCform.txtCDOB.$setValidity("invalidBirthDate", false);
                    } else {
                        $scope.empCCform.txtCDOB.$setValidity("invalidBirthDate", true);
                        var dt = bDate.split('-');
                        $scope.CredBirthDate = dt[2] + "-" + dt[1] + "-" + dt[0] + "T00:00:00";
                    }
                } else {
                    $scope.empCCform.txtCDOB.$setValidity("invalidBirthDate", false);
                }
            } else {
                $scope.empCCform.txtCDOB.$setValidity("invalidBirthDate", false);
            }
        };
        // END DATE PICKER

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

        /*validate dropdown*/
        $scope.validateSource = function () {
            if ($scope.compCred.SourceId && $scope.compCred.SourceId != 0) return false;
            return true;
        };

        /* getting list of Source*/
        function loadSources() {
            EmployeeCreateService.GetActiveSources().then(function (result) {
                $scope.DSource = result.data.DataList;
                $timeout(function () {
                    $scope.compCred.SourceId = 0;
                });
            });
        };
        loadSources();

        $scope.$watch('compCred.SecurityQuestion1', function (newValue) {
            if (!newValue) {
                $scope.compCred.SecurityAnswer1 = "";
            }
        });
        $scope.$watch('compCred.SecurityQuestion2', function (newValue) {
            if (!newValue) {
                $scope.compCred.SecurityAnswer2 = "";
            }
        });

        /*restrict other keyword in few textboxes*/
        $scope.$watch('compCred.SourceOther', function (newValue) {
            var lower = angular.lowercase(newValue);
            if (lower == "other" || lower == "others") {
                $scope.empCCform.txtSourceOther.$setValidity("errorSourceOther", false);
            } else if (newValue) {
                $scope.empCCform.txtSourceOther.$setValidity("errorSourceOther", true);
            }
        });

        $scope.copyEmail = function () {
            var re = /^[a-zA-Z0-9.!#$%&’*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$/;
            if (re.test($scope.compCred.UserName))
            { $scope.compCred.EmailId = $scope.compCred.UserName; }            
        }

        $scope.GetEmpCredentialsList = function () {
            //$scope.empCCform.$setPristine();
            //$scope.clearControl();
            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,
                sorting: {
                    EmployeeName: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;
                    EmployeeCreateService.GetEmpCredentialsList($scope.master.EmployeeId, $scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().SourceName, params.filter().UserName, params.filter().EmailId).then(function (result) {
                        if (result.data.IsValidUser) {
                            //display no data message
                            if (result.data.MessageType != 0) {
                                if (result.data.DataList.total == 0) {
                                    $scope.noRecord = true;
                                } else {
                                    $scope.noRecord = false;
                                }
                                params.total(result.data.DataList.total);
                                $defer.resolve($scope.we = result.data.DataList.result);
                                $rootScope.IsAjaxLoading = false;
                            }
                            else {
                                toastr.error(result.data.Message, 'Opps, Something went wrong');
                            }
                        }
                        else {
                            $rootScope.redirectToLogin();
                        }
                    });
                }
            })
        }
        //$scope.GetEmpCredentialsList();

        /*save Employee Company Credentials Information*/
        $scope.CreateUpdateEmpCredentials = function (q) {

            var IDate = $filter('date')(q.BirthDate, 'dd-MM-yyyy').split('-');
            var temp_Date = new Date(parseInt(IDate[2]), parseInt(IDate[1]) - 1, parseInt(IDate[0]), 0, 0, 0);
            q.BirthDate = $filter('date')(temp_Date, 'MM-dd-yyyy HH:mm:ss');

            q.EmployeeId = $scope.master.EmployeeId;
            EmployeeCreateService.CreateUpdateEmpCredentials(q).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.RefreshTable();
                        $scope.storage.lastRecord = {};
                        $scope.clearControl();
                        loadSources();
                        $scope.SetFocus();
                        $scope.empCCform.$setPristine();
                        if ($scope.mode === "Edit") {
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

        /*delete Employee Company Credentials Information*/
        $scope.DeleteEmpCredentials = function (_empCre) {
            $rootScope.IsAjaxLoading = true;
            EmployeeCreateService.DeleteEmpCredentials(_empCre).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType != 0) { // 0:Error
                        toastr.success(result.data.Message, 'Success');
                        $scope.clearControl();
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

        $scope.UpdateEmpCredentials = function (_empCre) {
            $scope.storage.lastRecord = _empCre;
            $scope.mode = "Edit";
            $scope.saveText = "Update";
            $scope.compCred = {
                SrNo: _empCre.SrNo,
                EmployeeId: _empCre.EmployeeId,
                SourceId: _empCre.SourceId,
                UserName: _empCre.UserName,
                Password: _empCre.Password,
                //BirthDate: _empCre.BirthDate,
                BirthDate: $filter('date')(_empCre.BirthDate, 'dd-MM-yyyy'),
                EmailId: _empCre.EmailId,
                SecurityQuestion1: _empCre.SecurityQuestion1,
                SecurityAnswer1: _empCre.SecurityAnswer1,
                SecurityQuestion2: _empCre.SecurityQuestion2,
                SecurityAnswer2: _empCre.SecurityAnswer2,
                Comments: _empCre.Comments,
            }
            $scope.CredBirthDate = _empCre.BirthDate;
            $scope.SetFocus();
        }

        /*reset the form*/
        $scope.ResetEmpCredentials = function () {
            $scope.compCred = $scope.storage.lastRecord;
            if ($scope.compCred.SrNo) {
                $scope.UpdateEmpCredentials($scope.compCred);
                //$scope.DocFileName = "";
            } else {
                $scope.clearControl();
            }
        };

        /*Clear all controls*/
        $scope.clearControl = function () {
            $scope.compCred = {
                SrNo: 0,
                SourceId: 0,
                SourchOther: '',
                UserName: '',
                Password: '',
                BirthDate: '',
                EmailId: '',
                SecurityQuestion1: '',
                SecurityAnswer1: '',
                SecurityQuestion2: '',
                SecurityAnswer2: '',
                Comments: ''
            };
            $scope.CredBirthDate = "";
            $scope.mode = "Add";
            $scope.saveText = "Save";
            $scope.storage.lastRecord = {};
            $scope.empCCform.$setPristine();
            $scope.SetFocus();
        };

        $scope.RefreshTable = function () {
            $scope.tableParams.reload();
        };
        /*reset the form*/

        $scope.SetFocus = function () {
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }
    };

})();