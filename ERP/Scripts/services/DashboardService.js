/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
    .service("DashboardService", [
        "$http",
        function ($http) {

            var list = {};

            ///*get festival list*/
            //list.GetFestivalList = function () {
            //    return $http({
            //        method: "GET",
            //        url: "/api/Dashboard/GetFestivalList?ts=" + new Date().getTime()
            //    });
            //};

            /*get full calendar list*/
            //list.GetCalendarLeaveList = function (timezone) {
            //    return $http({
            //        method: "GET",
            //        url: "/api/Dashboard/GetCalendarLeaveList?ts=" + new Date().getTime() + "&timeZone=" + timezone
            //    });
            //};

            /*get absent & leave list*/
            //list.GetLeaveAbsentList = function () {
            //    return $http({
            //        method: "GET",
            //        url: "/api/Dashboard/GetLeaveAbsentList?ts=" + new Date().getTime()
            //    });
            //};

            /*get anniversary, birthday and paid leave list of next 15 days*/
            list.GetInformationList = function () {
                return $http({
                    method: "GET",
                    url: "/api/Dashboard/GetInformationList?ts=" + new Date().getTime()
                });
            };


            list.GetEmpAttendanceReport = function (tempYear) {
                return $http({
                    method: "GET",
                    url: "/api/Dashboard/GetEmpAttendanceReport?ts=" + new Date().getTime() + "&tempYear=" + tempYear
                });
            };
           
            return list;
        }
    ]);