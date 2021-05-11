/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
    .service("SRPartService", [
        "$http",
        function ($http) {

            var partlist = {};

            /*get SR-Part list*/
            partlist.GetSRPartList = function (timezone, page, count, orderby, filter) {
                return $http({
                    method: "GET",
                    url: "./../api/SRParts/GetSRPartList?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&filter=" + filter
                });
            };

            /*add edit SR-Part*/
            partlist.CreateUpdateSRPart = function (data) {
                return $http({
                    method: "POST",
                    url: "./../api/SRParts/SaveSRPart?ts=" + new Date().getTime(),
                    data: data,
                    contentType: "application/json"
                });
            };

            /*delete SR-Part*/
            partlist.DeleteSRPart = function (id) {
                return $http({
                    method: "POST",
                    url: "./../api/SRParts/DeleteSRPart?ts=" + new Date().getTime(),
                    data: id,
                    contentType: "application/json"
                });
            };

            return partlist;
        }
    ]);