/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
    .service("SRExtraService", [
        "$http",
        function ($http) {

            var extralist = {};


            /*get SR-Extra list*/
            extralist.GetSRExtraList = function (timezone, page, count, orderby, filter1, filter2, filterType,startDate,endDate) {
                return $http({
                    method: "GET",
                    url: "./../api/SRExtra/GetSRExtraList?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&filter1=" + filter1 + "&filter2=" + filter2 + "&filterType=" + filterType + "&startDate=" + startDate + "&endDate=" + endDate
                });
            };

            /*add edit sub type*/
            extralist.CreateUpdateSRExtra = function (data) {
                return $http({
                    method: "POST",
                    url: "./../api/SRExtra/SaveSRExtra?ts=" + new Date().getTime(),
                    data: data,
                    contentType: "application/json"
                });
            };

            /*delete SR-Extra*/
            extralist.DeleteSRExtra = function (id) {
                return $http({
                    method: "POST",
                    url: "./../api/SRExtra/DeleteSRExtra?ts=" + new Date().getTime(),
                    data: id,
                    contentType: "application/json"
                });
            };

            return extralist;
        }
    ]);