/// <reference path="../libs/angular/angular.js" />

(function () {

    angular.module("ERPApp.Controllers").controller("EmpWorkExperienceCtrl", [
        "$scope",
        "$rootScope",
        "$timeout",
        "EmployeeCreateService",
        "$http",
        "$filter",
        "ngTableParams",
        "$q",
        EmpWorkExperience
    ]);

    function EmpWorkExperience($scope, $rootScope, $timeout, ES, $http, $filter, ngTableParams, $q) {

        // BEGIN VARIABLES
        $scope.mode = "Add";
        $scope.saveText = "Save";
        $scope.timeZone = new Date().getTimezoneOffset().toString();
        $scope.months = [
            { key: 1, val: 'Jan' },
            { key: 2, val: 'Feb' },
            { key: 3, val: 'Mar' },
            { key: 4, val: 'Apr' },
            { key: 5, val: 'May' },
            { key: 6, val: 'Jun' },
            { key: 7, val: 'Jul' },
            { key: 8, val: 'Aug' },
            { key: 9, val: 'Sep' },
            { key: 10, val: 'Oct' },
            { key: 11, val: 'Nov' },
            { key: 12, val: 'Dec' }
        ];
        $scope.years = [];
        for (var i = 1980; i <= new Date().getFullYear() ; i++) {
            $scope.years.push(i);
        }

        $scope.workExperience = {
            SrNo: 0,
            EmployeeId: 0,
            CompanyName: "",
            Designation: "",
            Skills: [],
            FromMonth: 0,
            FromYear: 0,
            ToMonth: 0,
            ToYear: 0,
            Salary: 0,
            Comments: "",
            IsActive: true
        };
        $scope.lastWorkExperience = {
            SrNo: 0,
            EmployeeId: 0,
            CompanyName: "",
            Designation: "",
            Skills: [],
            FromMonth: 0,
            FromYear: 0,
            ToMonth: 0,
            ToYear: 0,
            Salary: 0,
            Comments: "",
            IsActive: true
        };
        $scope.isFirstFocus = true;
        // END VARIABLES

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
        $scope.maxDate = $scope.maxDate || new Date();

        $scope.calendarOpenWorkExpFromDate = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.workExperience.calOpenWorkExpFromDate = true;
            $scope.workExperience.calOpenWorkExpToDate = false;
        };

        $scope.calendarOpenWorkExpToDate = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.workExperience.calOpenWorkExpFromDate = false;
            $scope.workExperience.calOpenWorkExpToDate = true;
        };

        $scope.$watch('workExperience.fromDate', function (newValue) {
            $scope.workExperience.FromDate = $filter('date')(newValue, 'dd-MM-yyyy');
            $scope.minDate = newValue;
            if ($scope.CheckDate()) {
                $scope.workExperience.ToDate = $scope.workExperience.FromDate;
                $scope.workExperience.toDate = newValue;
            }
        });
        $scope.$watch('workExperience.toDate', function (newValue) {
            $scope.workExperience.ToDate = $filter('date')(newValue, 'dd-MM-yyyy');
        });

        $scope.ValidateWorkExpFromDate = function (fromDate) {
            if (!fromDate) {
                $scope.frmWorkExp.txtFromDate.$setValidity("invalidWorkExpFromDate", true);
                return;
            } else if (fromDate.length == 10) {
                if ($scope.ValidateDate(fromDate)) {
                    $scope.workExperience.fromDate = $scope.StringToDateString(fromDate);
                    $scope.frmWorkExp.txtFromDate.$setValidity("invalidWorkExpFromDate", true);

                    if ($scope.IsGreterThanToday(fromDate)) {
                        $scope.workExperience.FromDate = $filter('date')(new Date(), 'dd-MM-yyyy');
                        $scope.workExperience.fromDate = $scope.StringToDateString($filter('date')(new Date(), 'dd-MM-yyyy'));
                    }

                    if ($scope.CheckDate()) {
                        $scope.workExperience.ToDate = fromDate;
                        $scope.workExperience.toDate = $scope.StringToDateString(fromDate);
                    }
                } else {
                    $scope.frmWorkExp.txtFromDate.$setValidity("invalidWorkExpFromDate", false);
                }
            } else {
                $scope.frmWorkExp.txtFromDate.$setValidity("invalidWorkExpFromDate", false);
            }
        };

        $scope.ValidateWorkExpToDate = function (toDate) {
            if (!toDate) {
                $scope.frmWorkExp.txtToDate.$setValidity("invalidWorkExpToDate", true);
                return;
            } else if (toDate.length == 10) {
                if ($scope.ValidateDate(toDate)) {
                    $scope.workExperience.toDate = $scope.StringToDateString(toDate);
                    $scope.frmWorkExp.txtToDate.$setValidity("invalidWorkExpToDate", true);

                    if ($scope.IsGreterThanToday(toDate)) {
                        $scope.workExperience.ToDate = $filter('date')(new Date(), 'dd-MM-yyyy');
                        $scope.workExperience.toDate = $scope.StringToDateString($filter('date')(new Date(), 'dd-MM-yyyy'));
                    }

                    if ($scope.CheckDate()) {
                        $scope.workExperience.ToDate = $scope.workExperience.FromDate;
                        $scope.workExperience.toDate = $scope.StringToDateString($scope.workExperience.FromDate);
                    }
                } else {
                    $scope.frmWorkExp.txtToDate.$setValidity("invalidWorkExpToDate", false);
                }
            } else {
                $scope.frmWorkExp.txtToDate.$setValidity("invalidWorkExpToDate", false);
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

        $scope.CheckDate = function () {
            var fromDate = new Date($filter('date')($scope.workExperience.FromDate, 'MM-dd-yyyy'));
            var toDate = new Date($filter('date')($scope.workExperience.ToDate, 'MM-dd-yyyy'));

            if (fromDate > toDate) {
                return true;
            }
            return false;
        };

        $scope.StringToDateString = function (dtValue) {
            var dt = dtValue.split('-');
            return dt[2] + "-" + dt[1] + "-" + dt[0] + "T00:00:00";
        };

        // END DATE PICKER

        // BEGIN FUNCTIONS
        $scope.CreateUpdateWorkExperience = function (workExperience) {
            
            var newWorkExperience = angular.copy(workExperience);
            var temp_Skills = workExperience.Skills;
            var temp = [];
            for (var i = 0 ; i < temp_Skills.length; i++)
            { temp.push(temp_Skills[i].text); }
            newWorkExperience.Skills = temp.toString();

            ES.CreateUpdateWorkExperience(newWorkExperience, $scope.timeZone, $scope.master.EmployeeId).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType === 1) {
                        $scope.workExperience = {
                            SrNo: 0,
                            EmployeeId: 0,
                            CompanyName: "",
                            Designation: "",
                            Skills: [],
                            FromDate: "",
                            ToDate: "",
                            Salary: "",
                            Comments: "",
                            IsActive: true
                        };
                        $scope.frmWorkExp.$setPristine();
                        $scope.mode = "Add";
                        $scope.saveText = "Save";
                        $scope.RefreshTable();
                        $scope.SetFocus();
                        toastr.success(result.data.Message, 'Success');
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

        $scope.RetriveTechnology = function ($query) {
            var deferred = $q.defer();
            ES.RetriveTechnology($query).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) {
                        deferred.resolve(result.data.DataList);
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                        deferred.reject(result.data.Message);
                    }
                }
                else {
                    deferred.reject("Error loading tag data.");
                    $rootScope.redirectToLogin();
                }
            });

            return deferred.promise;
        }

        $scope.RetrieveEmpWorkExperience = function () {
            $scope.mode = "Add";
            $scope.ResetEmpWorkExperience();
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
                    ES.RetrieveEmpWorkExperience($scope.master.EmployeeId, $scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().EmployeeName, params.filter().CompanyName, params.filter().Designation, params.filter().Skills).then(function (result) {
                        if (result.data.IsValidUser) {
                            //display no data message
                            if (result.data.MessageType != 0) {
                                if (result.data.DataList.total == 0) {
                                    $scope.noRecord = true;
                                    $scope.filterText = params.filter().EmployeeName;
                                } else {
                                    $scope.noRecord = false;
                                }
                                params.total(result.data.DataList.total);
                                //$defer.resolve(function(){
                                //    $scope.we = result.data.DataList;
                                //    $scope.we.Skills = (result.data.DataList.length > 0 ? result.data.DataList.Skills.split(',') : []);
                                //});
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
        var lastWorkExperience = {};
        $scope.UpdateEmpWorkExperience = function (_workExperience) {
            $scope.lastWorkExperience = _workExperience;
            $scope.workExperience = {
                SrNo: _workExperience.SrNo,
                EmployeeId: _workExperience.EmployeeId,
                CompanyName: _workExperience.CompanyName,
                Designation: _workExperience.Designation,
                //  Skills: _workExperience.Skills.length > 0 ? _workExperience.Skills.split(',') : [],
                Skills: _workExperience.Skills.toString().length > 0 ? _workExperience.Skills.toString().split(',') : [],
                FromDate: $filter('date')(_workExperience.FromDate, 'dd-MM-yyyy'),
                ToDate: $filter('date')(_workExperience.ToDate, 'dd-MM-yyyy'),
                Salary: _workExperience.Salary.toString(),
                Comments: _workExperience.Comments,
                IsActive: _workExperience.IsActive
            }

            $scope.workExperience.fromDate = _workExperience.FromDate;
            $scope.workExperience.toDate = _workExperience.ToDate;

            $rootScope.isFormVisible = true;
            $scope.mode = "Edit";
            $scope.saveText = "Update";
            $scope.SetFocus();
        }

        $scope.ResetEmpWorkExperience = function () {
            if ($scope.mode === "Add") {
                $scope.workExperience = {
                    SrNo: 0,
                    EmployeeId: 0,
                    CompanyName: "",
                    Designation: "",
                    Skills: [],
                    FromDate: "",
                    ToDate: "",
                    Salary: 0,
                    Comments: "",
                    IsActive: true
                };
                $scope.frmWorkExp.$setPristine();
            }
            else if ($scope.mode === "Edit") {
                $scope.workExperience = $scope.lastWorkExperience;
                $scope.UpdateEmpWorkExperience($scope.workExperience);
            }
            $scope.SetFocus();
        }

        $scope.ClearEmpWorkExperience = function () {
            $scope.workExperience = {
                SrNo: 0,
                EmployeeId: 0,
                CompanyName: "",
                Designation: "",
                Skills: [],
                FromDate: "",
                ToDate: "",
                Salary: "",
                Comments: "",
                IsActive: true
            };
            $scope.frmWorkExp.$setPristine();
            $scope.mode = "Add";
            $scope.saveText = "Save";
            $scope.SetFocus();
        }

        $scope.DeleteEmpWorkExperience = function (detail) {
            ES.DeleteEmpWorkExperience(detail).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType === 1) {
                        toastr.success(result.data.Message, 'Success');
                        $scope.RefreshTable();
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                }
                else {
                    $rootScope.redirectToLogin();
                }
            });
        }

        $scope.ChangeIsActive = function (detail) {
            ES.ChangeIsActive(detail).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType === 1) {
                        toastr.success(result.data.Message, 'Success');
                        $scope.RefreshTable();
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                }
                else {
                    $rootScope.redirectToLogin();
                }
            });
        }

        $scope.RefreshTable = function () {
            $scope.tableParams.reload();
        };

        $scope.SetFocus = function () {
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }
        // END FUNCTIONS
    }
})();

