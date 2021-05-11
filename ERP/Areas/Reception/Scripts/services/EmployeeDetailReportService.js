/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
    .service("EmployeeDetailReportService", [
        "$http",
        function ($http) {

            var list = {};

            /*retrieve Dept list*/
            list.RetrieveDept= function () {
                return $http({
                    method: 'GET',
                    url: "./../api/EmployeeDetailReport/RetrieveDeptList?ts=" + new Date().getTime(),
                });
            }

            /*Retrieve telephone report*/
            list.GetEmployeeDetailReport = function (timezone, page, count, orderby, filter, filter1, filter2, filter3, filter4, filter5, filter6, filter7, filter8) {
                return $http({
                    method: "POST",
                    data: filter,
                    contentType: "application/json",
                    url: "/api/EmployeeDetailReport/GetEmployeeDetailReport?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&filter1=" + filter1 + "&filter2=" + filter2 + "&filter3=" + filter3 + "&filter4=" + filter4 + "&filter5=" + filter5 + "&filter6=" + filter6 + "&filter7=" + filter7 + "&filter8=" + filter8
                });
            };
            return list;
        }
    ]);