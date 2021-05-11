/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
    .service("PMSReportService", [
        "$http",
        function ($http) {

            var list = {};

            /*get user list*/
            list.GetUserList = function () {
                return $http({
                    method: "GET",
                    url: "./../api/PMSReport/GetUserList?ts=" + new Date().getTime()
                });
            };

            /*get project list*/
            list.GetProjectList = function () {
                return $http({
                    method: "GET",
                    url: "./../api/PMSReport/GetProjectList?ts=" + new Date().getTime()
                });
            };

            list.reportUserAndProjectwise = function (LoadType, filterUserProject, date, counter, startDate, endDate) {
                return $http({
                    method: "GET",
                    url: "./../api/PMSReport/reportUserAndProjectwise?ts=" + new Date().getTime() + "&LoadType=" + LoadType + "&reportProjectId=" + filterUserProject + "&date=" + date._i + "&counter=" + counter + "&startDate=" + startDate + "&endDate=" + endDate
                });
            }

            return list;
        }
    ]);