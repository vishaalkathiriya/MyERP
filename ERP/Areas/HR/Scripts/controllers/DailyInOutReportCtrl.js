/// <reference path="../libs/angular/angular.min.js" />


(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("DailyInOutReportCtrl", [
            "$scope", "$rootScope", "$timeout", "DailyInOutReportService", "$http", "$filter", "$q", "$modal",
            DailyInOutReportCtrl
        ]);


    //Main controller function
    function DailyInOutReportCtrl($scope, $rootScope, $timeout, DailyInOutReportService, $http, $filter, $q, $modal) {
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes
        $scope.isFocusFirst = false;
        $scope.storage = { lastRecord: "" };
        $scope.filterData = { date: "", pickerDate: new Date(), initialDate: new Date(), IsDisabled: true };
        $scope.IsAjaxLoadingPMS = false;


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
        //$scope.maxDate = $scope.maxDate || new Date();

        $scope.calendarOpenDate = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.calOpenDate = true;
        };

        $scope.$watch('filterData.pickerDate', function (newValue) {
            $scope.filterData.date = $filter('date')(newValue, 'dd-MM-yyyy');
            $scope.filterData.IsDisabled = false;
        });

        $scope.ValidateFilterDate = function (bDate) {
            if (!bDate) {
                $scope.filterData.IsDisabled = true;
            } else if (bDate.length == 10) {
                if ($scope.ValidateDate(bDate)) {
                    var dt = bDate.split('-');
                    $scope.filterData.pickerDate = dt[2] + "-" + dt[1] + "-" + dt[0] + "T00:00:00";
                    $scope.filterData.IsDisabled = false;
                } else {
                    $scope.filterData.IsDisabled = true;
                }
            } else {
                $scope.filterData.IsDisabled = true;
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
        // END DATE PICKER

        /*getting list of project*/
        $scope.GetDailyInOutReprotList = function (date, groupName) {
            $scope.IsAjaxLoadingPMS = true;
            DailyInOutReportService.GetDailyInOutReprotList($scope.timeZone, date, groupName).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        $scope.DailyInOutReportList = result.data.DataList;
                        angular.forEach($scope.DailyInOutReportList, function (value, key) {
                            value.PresenceList = $scope.GetPresenceList();
                            value.LeaveList = $scope.GetLeaveList();
                            value.AbsenceList = $scope.GetAbsenceList();
                            value.OTList = $scope.GetOTList();
                            value.btnText = "Insert";
                            value.IsHide = false;

                            //Set Is-Holiday to true if Selected Date is Holiday
                            value.IsHoliday = value.isHolidayDay == true ? true : value.IsHoliday;

                            $timeout(function () {
                                if (value.Attendance) { //Edit
                                    value.IsHoliday = value.Attendance.IsHoliday;
                                    value.Remark = value.Attendance.Remark;
                                    value.btnText = "Update";
                                    var pValue = value.Attendance.Presence > 0 ? ((value.Attendance.Presence == 0.5) ? "HP" : "FP") : 0;
                                    if (value.SalaryBasedOn == "H") {
                                        value.IsHourly = true;
                                        value.nofHours = value.Attendance.WorkingHours;
                                    } else {
                                        value.IsHourly = false;
                                        value.nofHours = "";

                                    }



                                    if (pValue == "HP") {
                                        value.LeaveList = value.LeaveList.splice(0, 1);
                                        value.AbsenceList = value.AbsenceList.splice(0, 1);
                                    } else if (pValue == "FP") {
                                        value.LeaveList = [];
                                        value.AbsenceList = [];
                                    }

                                    $timeout(function () {
                                        value.Presence = pValue;
                                        value.Leave = value.Attendance.Leave > 0 ? ((value.Attendance.Leave == 0.5) ? "HL" : "FL") : 0;
                                        value.Absence = value.Attendance.Absence > 0 ? ((value.Attendance.Absence == 0.5) ? "HA" : "FA") : 0;
                                        value.OT = value.Attendance.OT > 0 ? ((value.Attendance.OT == 0.5) ? "HO" : "FO") : 0;
                                    }, 1000);

                                } else { //Add
                                    //list out dropdown options
                                    if (value.AppliedLeaveStatus) {
                                        if (value.AppliedLeaveStatus == "P") {
                                            //value.PresenceList = value.PresenceList.splice(0, 1);
                                            value.LeaveList = value.LeaveList.splice(0, 1);
                                            //value.AbsenceList = [];
                                        } else if (value.AppliedLeaveStatus == "F") {
                                            //value.PresenceList = [];
                                            //value.AbsenceList = [];
                                            value.LeaveList = value.LeaveList.splice(1, 2);
                                        }
                                    }
                                    //Set Is-Holiday to true if Selected Date is Holiday
                                    value.IsHoliday = value.isHolidayDay == true ? true : false;
                                    value.Remark = "";
                                    value.Presence = value.AppliedLeaveStatus ? (value.AppliedLeaveStatus == "P" ? "HP" : 0) : 0;
                                    value.Leave = value.AppliedLeaveStatus ? (value.AppliedLeaveStatus == "P" ? "HL" : "FL") : 0;
                                    value.Absence = 0;
                                    value.OT = 0;

                                  
                                }

                            });

                        });
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                $scope.IsAjaxLoadingPMS = false;
            });
        }
        $scope.GetDailyInOutReprotList($filter('date')($scope.filterData.initialDate, 'dd-MM-yyyy'));

        /*save employee attandance*/
        $scope.SaveData = function (data, index) {
            var pValue = $.isNumeric(data.Presence) ? data.Presence : data.Presence != 0 && data.Presence ? ((data.Presence == "HP") ? 0.5 : 1) : 0;
            var lValue = $.isNumeric(data.Leave) ? data.Leave : data.Leave != 0 && data.Leave ? ((data.Leave == "HL") ? 0.5 : 1) : 0;
            var aValue = $.isNumeric(data.Absence) ? data.Absence : data.Absence != 0 && data.Absence ? ((data.Absence == "HA") ? 0.5 : 1) : 0;

            // Total Of Presence,leave and absense should be = 1
            var total = Number(pValue) + Number(lValue) + Number(aValue);
            if (data.IsHourly) {

                var line = {
                    Id: data.Attendance ? data.Attendance.Id : 0,
                    EmployeeId: data.EmployeeId,
                    PDateInString: $filter('date')(data.Edate, "dd-MM-yyyy hh:mm"),
                    Presence: 1,
                    Absence: 0,
                    Leave: 0,
                    OT: 0,
                    WorkingHours: data.nofHours,
                    PersonalWorkHours: data.InOutDetail.personalWorkHrs,
                    CompanyWorkHours: data.InOutDetail.CompanyWorkHrs,
                    LunchBreakHours: data.InOutDetail.lunchBreakHrs,
                    IsHoliday: data.IsHoliday,
                    Remark: data.Remark
                };

                $scope.IsAjaxLoadingPMS = true;
                DailyInOutReportService.SaveData(line).then(function (result) {
                    if (result.data.IsValidUser) {
                        if (result.data.MessageType == 1) { // 1:Success
                            toastr.success(result.data.Message, 'Success');
                            $scope.DailyInOutReportList[index].IsHide = true;
                            //$scope.GetDailyInOutReprotList($scope.filterData.date, $scope.filterData.GroupName);
                        }
                        else {
                            toastr.error(result.data.Message, 'Opps, Something went wrong');
                        }
                    }
                    else {
                        $rootScope.redirectToLogin();
                    }
                    $scope.IsAjaxLoadingPMS = false;
                });

            } else if ((total == 1) || data.IsHoliday == true) {

                var line = {
                    Id: data.Attendance ? data.Attendance.Id : 0,
                    EmployeeId: data.EmployeeId,
                    PDateInString: $filter('date')(data.Edate, "dd-MM-yyyy hh:mm"),
                    Presence: Number(pValue),
                    Absence: Number(aValue),
                    Leave: Number(lValue),
                    OT: data.OT != 0 && data.OT ? ((data.OT == "HO") ? 0.5 : 1) : 0,
                    WorkingHours: data.InOutDetail.companyHrs,
                    PersonalWorkHours: data.InOutDetail.personalWorkHrs,
                    CompanyWorkHours: data.InOutDetail.CompanyWorkHrs,
                    LunchBreakHours: data.InOutDetail.lunchBreakHrs,
                    IsHoliday: data.IsHoliday,
                    Remark: data.Remark
                };

                $scope.IsAjaxLoadingPMS = true;
                DailyInOutReportService.SaveData(line).then(function (result) {
                    if (result.data.IsValidUser) {
                        if (result.data.MessageType == 1) { // 1:Success
                            toastr.success(result.data.Message, 'Success');
                            $scope.DailyInOutReportList[index].IsHide = true;
                            //$scope.GetDailyInOutReprotList($scope.filterData.date, $scope.filterData.GroupName);
                        }
                        else {
                            toastr.error(result.data.Message, 'Opps, Something went wrong');
                        }
                    }
                    else {
                        $rootScope.redirectToLogin();
                    }
                    $scope.IsAjaxLoadingPMS = false;
                });
            } else {
                toastr.warning('Please select proper value', "Make proper selection");
            }
        };

        $scope.OnChangePresenceList = function (pValue, index) {
            var leavelist = $scope.GetLeaveList();
            var absencelist = $scope.GetAbsenceList();

            if (pValue == "HP") {
                $scope.DailyInOutReportList[index].LeaveList = leavelist.splice(0, 1);
                $scope.DailyInOutReportList[index].AbsenceList = absencelist.splice(0, 1);
            } else if (pValue == "FP") {
                $scope.DailyInOutReportList[index].LeaveList = [];
                $scope.DailyInOutReportList[index].AbsenceList = [];
            } else {
                $scope.DailyInOutReportList[index].LeaveList = leavelist;
                $scope.DailyInOutReportList[index].AbsenceList = absencelist;
            }

            $scope.DailyInOutReportList[index].Leave = 0;
            $scope.DailyInOutReportList[index].Absence = 0;
        };


        $scope.OnChangeLeaveList = function (lValue, index) {
            // NOTE: HOT FIX TO ALLOW HALF LEAVE AND HALF ABSENCE 
            //var absencelist = $scope.GetAbsenceList();
            //$scope.DailyInOutReportList[index].AbsenceList = absencelist;
            //$scope.DailyInOutReportList[index].Absence = 0;
        };

        $scope.OnChangeAbsenceList = function (aValue, index) {
            // NOTE: HOT FIX TO ALLOW HALF LEAVE AND HALF ABSENCE
            //var leavelist = $scope.GetLeaveList();
            //$scope.DailyInOutReportList[index].LeaveList = leavelist;
            //$scope.DailyInOutReportList[index].Leave = 0;
        };

        $scope.GetPresenceList = function () {
            var list = [];
            list.push({ val: "HP", key: "Half Presence" });
            list.push({ val: "FP", key: "Full Presence" });

            return list;
        };
        $scope.GetLeaveList = function () {
            var list = [];
            list.push({ val: "HL", key: "Half Leave" });
            list.push({ val: "FL", key: "Full Leave" });

            return list;
        };
        $scope.GetAbsenceList = function () {
            var list = [];
            list.push({ val: "HA", key: "Half Absence" });
            list.push({ val: "FA", key: "Full Absence" });

            return list;
        };
        $scope.GetOTList = function () {
            var list = [];
            list.push({ val: "HO", key: "Half OT" });
            list.push({ val: "FO", key: "Full OT" });

            return list;
        };

        /*filter by date*/
        $scope.FilterByDate = function (filterData) {
            $scope.GetDailyInOutReprotList(filterData.date, filterData.GroupName);
        };

        /*clear filter and load current date data*/
        $scope.ClearFilter = function () {
            $scope.GetDailyInOutReprotList($filter('date')($scope.filterData.initialDate, 'dd-MM-yyyy'));
            $scope.filterData.pickerDate = $scope.filterData.initialDate;
            $scope.filterData.GroupName = "";
        };

        $scope.CheckIsHoliday = function (isHoliday, index) {
            if (!isHoliday) {
                $scope.DailyInOutReportList[index].Presence = 0;
                $scope.DailyInOutReportList[index].Absence = 0;
                $scope.DailyInOutReportList[index].Leave = 0;
            }
        };

        /*********************************************** Edit In-Out Detail Popup ***********************************************/

        

        var InOutDetailCtrl = function ($scope, $rootScope, DetailResult, $modalInstance) {
            $scope.editData = $scope.editData || {};
            $scope.EmpData = DailyInOutReportService.GetData();
            $scope.isFormvisible = false;

            $scope.InTypeOptions = [
                    { name: 'Company In', value: 1 },
                    { name: 'Lunch-Break End', value: 4 },
                    { name: 'Company-Work End', value: 6 },
                    { name: 'Personal-Work End', value: 8 }
            ];

            $scope.OutTypeOptions = [
                    { name: 'Company Out', value: 2 },
                    { name: 'Lunch-Break Start', value: 3 },
                    { name: 'Company-Work Start', value: 5 },
                    { name: 'Personal-Work Start', value: 7 }
            ];

            //Load In-Out Detail Grid-Data
            function refreshData() {
                DailyInOutReportService.GetInOutDetailsOfEmployee($scope.timeZone, $scope.filterData.date, $scope.EmpData.EmployeeId).then(function (result) {
                    $scope.EmployeeInOutDetailList = result.data.DataList;
                })
            }
            refreshData();

            /*get record to edit In-Out Detail*/
            $scope.UpdateInOutDetail = function (_io) {
                $scope.storage.lastRecord = _io;
                $scope.mode = "Edit";
                $scope.saveText = "Update";

                $scope.editData.SrNo = _io.SrNo;
                $scope.editData.EmployeeId = _io.EmployeeId;
                $scope.editData.InTime = _io.Intime;
                $scope.editData.OutTime = _io.OutTime;
                $scope.editData.InType = _io.InType;
                $scope.editData.OutType = _io.OutType;

                $scope.isFormvisible = true;
                $scope.isFirstFocus = true;
                $timeout(function () {
                    $scope.isFirstFocus = true;
                });
            };

            //Save In-Out Detail
            $scope.CreateUpdateInOutTime = function () {
                if (($scope.editData.InTime != null && $scope.editData.InType != null) || ($scope.editData.OutTime != null && $scope.editData.OutType != null)) {
                    DailyInOutReportService.UpdateInOutDetailsOfEmployee($scope.editData).then(function (result) {
                        if (result.data.IsValidUser) {
                            if (result.data.MessageType == 1) { // 1:Success
                                toastr.success(result.data.Message, 'Success');
                                refreshData();
                                $scope.editData = {};
                                $scope.isFormvisible = false;
                            }
                            else {
                                toastr.error(result.data.Message, 'Opps, Something went wrong');
                            }
                        }
                        else {
                            $rootScope.redirectToLogin();
                        }
                        $scope.IsAjaxLoadingPMS = false;
                    });
                }
                else {
                    toastr.warning('Please Enter proper value', "Enter Time");
                }
            };

            //Delete In-Out Entry
            $scope.DeleteEmployeeInOutDelete = function (data) {
                DailyInOutReportService.DeleteEmployeeInOutDelete(data).then(function (result) {
                    if (result.data.IsValidUser) {
                        if (result.data.MessageType == 1) { // 1:Success
                            toastr.success(result.data.Message, 'Success');
                            refreshData();
                        }
                        else {
                            toastr.error(result.data.Message, 'Opps, Something went wrong');
                        }
                    }
                    else {
                        $rootScope.redirectToLogin();
                    }
                    $scope.IsAjaxLoadingPMS = false;
                });
            }

            //Close Edit-Form
            $scope.CloseEditForm = function () {
                $scope.editData = {};
                $scope.isFormvisible = false;
            };

            //Close Popup
            $scope.CloseDetailPopup = function () {
                $modalInstance.close();
                $scope.GetDailyInOutReprotList($scope.filterData.date, $scope.filterData.GroupName);
            };

        };


        $scope.EditInOutDetails = function (data) {
            DailyInOutReportService.SetData(data);
            var detailModelInstance = $modal.open({
                templateUrl: 'InOutDetailPopup.html',
                controller: InOutDetailCtrl,
                scope: $scope,
                size: 'lg',
                resolve: {
                    DetailResult: function () {
                        return DailyInOutReportService.GetInOutDetailsOfEmployee($scope.timeZone, $scope.filterData.date, data.EmployeeId).then(function (result) {
                            return result;
                        });
                    }
                }
            });
        }
    };
})();

