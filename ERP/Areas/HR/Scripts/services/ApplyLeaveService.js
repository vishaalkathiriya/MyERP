/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
    .service("ApplyLeaveService", [
        "$http",
        function ($http) {

            var leavelist = {};

            /*add leave*/
            leavelist.SaveApplyLeave = function (data) {
                return $http({
                    method: "POST",
                    url: "/api/ApplyLeave/SaveApplyLeave?ts=" + new Date().getTime(),
                    data: data,
                    contentType: "application/json"
                });
            };

            /*get festival list*/
            leavelist.GetFestivalList = function () {
                return $http({
                    method: "GET",
                    url: "/api/ApplyLeave/GetFestivalList?ts=" + new Date().getTime()
                });
            };

            /*check if entry is exists or not*/
            //leavelist.IsEntryExists = function (employeeId, startdate, enddate) {
            //    return $http({
            //        method: "GET",
            //        url: "/api/ApplyLeave/IsEntryExists?ts=" + new Date().getTime() + "&startDate=" + startdate + "&endDate=" + enddate + "&employeeId=" + employeeId
            //    });
            //};

            /*get full calendar list*/
            leavelist.GetCalendarLeaveList = function (employeeId, timezone) {
                var isAll = false;
                if (employeeId == 0) {
                    isAll = true;
                    employeeId = window.erpuid;
                }
                return $http({
                    method: "GET",
                    url: "/api/ApplyLeave/GetCalendarLeaveList?ts=" + new Date().getTime() + "&employeeId=" + employeeId + "&IsAll=" + isAll + "&timeZone=" + timezone
                });
            };

            /*get user list*/
            leavelist.GetUserList = function () {
                return $http({
                    method: "GET",
                    url: "/api/ApplyLeave/GetUserList?ts=" + new Date().getTime()
                });
            };

            leavelist.CheckTeamLead = function (employeeId) {
                return $http({
                    method: "GET",
                    url: "/api/ApplyLeave/CheckTeamLead?ts=" + new Date().getTime() + "&employeeId=" + employeeId
                });
            }

            /*get absent & leave list*/
            leavelist.GetLeaveAbsentList = function (employeeId) {
                return $http({
                    method: "GET",
                    url: "/api/ApplyLeave/GetLeaveAbsentList?ts=" + new Date().getTime() + "&employeeId=" + employeeId
                });
            };

            /*cancel leave*/
            leavelist.CancelLeave = function (date, employeeId) {
                return $http({
                    method: "GET",
                    url: "/api/ApplyLeave/CancelLeave?ts=" + new Date().getTime() + "&date=" + date + "&employeeId=" + employeeId
                });
            };

            /*approve disapprove leave*/
            leavelist.ApproveDisapproveLeave = function (leaveDate, employeeId, isApproved, reason) {
                return $http({
                    method: "GET",
                    url: "/api/ApplyLeave/ApproveDisapproveLeave?ts=" + new Date().getTime() + "&leaveDate=" + leaveDate + "&isApproved=" + isApproved + "&reason=" + reason + "&employeeId=" + employeeId
                });
            };

            /*send mail after applying for leave*/
            leavelist.SendMail = function (comments, employeeId, startdate, enddate) {
                return $http({
                    method: "GET",
                    url: "/api/ApplyLeave/SendMail?ts=" + new Date().getTime() + "&startDate=" + startdate + "&endDate=" + enddate + "&employeeId=" + employeeId + "&comments=" + comments
                });
            };

            //---------------use for deshboard-------------------------
            //leavelist.GetInformationList = function () {
            //    return $http({
            //        method: "GET",
            //        url: "/api/ApplyLeave/GetInformationList?ts=" + new Date().getTime()
            //    });
            //};

            //-----------------------------------------------------

            return leavelist;
        }
    ]);