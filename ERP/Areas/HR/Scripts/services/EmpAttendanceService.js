/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
    .service("EmpAttendanceService", [
        "$http",
        function ($http) {

            var list = {};

            /*Get Employees Attendance Report Data*/
            list.GetEmpAttendanceReport = function (timezone, filter1, GroupName) {
                return $http({
                    method: "GET",
                    url: "/api/EmpAttendance/GetEmpAttendanceReport?ts=" + new Date().getTime() + "&timezone=" + timezone + "&filter1=" + filter1 + "&groupName=" + GroupName
                });
            };

            /*Get Employees Attendance Report Data In Month Format*/
            list.GetEmpAttendanceReportMonthFormat = function (timezone, filter1, GroupName, filter2) {
                return $http({
                    method: "GET",
                    url: "/api/EmpAttendance/GetEmpAttendanceReportMonthFormat?ts=" + new Date().getTime() + "&timezone=" + timezone + "&filter1=" + filter1 + "&groupName=" + GroupName + "&filter2=" + filter2
                });
            };

            /*Get Particular Employee's Month's Detail Attendance Data*/
            list.GetEmployeeMonthDetail = function (EmpId, Mnth, sYear) {
                return $http({
                    method: "GET",
                    url: "/api/EmpAttendance/GetEmployeeMonthDetail?EmpId=" + EmpId + "&Mnth=" + Mnth + "&sYear=" + sYear
                });
            };

            return list;
        }
    ]);