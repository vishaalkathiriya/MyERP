/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
    .service("TelephoneReportService", [
        "$http",
        function ($http) {

            var list = {};

            /*retrieve ext type list*/
            list.RetrieveExtType = function () {
                return $http({
                    method: 'GET',
                    url: "./../api/TelephoneReport/RetrieveExtTypeList?ts=" + new Date().getTime(),
                });
            }

            /*Retrieve telephone report*/
            list.GetTelephoneReportOuter = function (timezone, page, count, orderby, filter, filter1, filter2, filter3, filter4, filter5) {
                return $http({
                    method: "POST",
                    data: filter,
                    contentType: "application/json",
                    url: "/api/TelephoneReport/GetTelephoneReportOuter?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&filter1=" + filter1 + "&filter2=" + filter2 + "&filter3=" + filter3 + "&filter4=" + filter4 + "&filter5=" + filter5
                });
            };

            /*Retrieve telephone intercom report*/
            list.GetTelephoneReportInter = function (timezone, page, count, orderby, filter, filter1, filter2, filter3, filter4) {
                return $http({
                    method: "POST",
                    data: filter,
                    contentType: "application/json",
                    url: "/api/TelephoneReport/GetTelephoneReportInter?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&filter1=" + filter1 + "&filter2=" + filter2 + "&filter3=" + filter3 + "&filter4=" + filter4
                });
            };
            return list;
        }
    ]);