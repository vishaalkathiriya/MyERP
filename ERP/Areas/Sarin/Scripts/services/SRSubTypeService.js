/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
    .service("SRSubTypeService", [
        "$http",
        function ($http) {

            var subtypelist = {};

            /*retrieve SR Type list*/
            subtypelist.RetrieveSRType = function () {
                return $http({
                    method: 'GET',
                    url: "./../api/SRSubType/RetrieveSRTypeList?ts=" + new Date().getTime(),
                });
            }

            /*get SR-Sub-Type list*/
            subtypelist.GetSRSubTypeList = function (timezone, page, count, orderby, filter1, filter2, filter3) {
                return $http({
                    method: "GET",
                    url: "./../api/SRSubType/GetSRSubTypeList?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&filter1=" + filter1 + "&filter2=" + filter2 + "&filter3=" + filter3
                });
            };

            /*add edit sub type*/
            subtypelist.CreateUpdateSRSubType = function (data) {
                return $http({
                    method: "POST",
                    url: "./../api/SRSubType/SaveSRSubType?ts=" + new Date().getTime(),
                    data: data,
                    contentType: "application/json"
                });
            };

            /*delete sub type*/
            subtypelist.DeleteSRSubType = function (id) {
                return $http({
                    method: "POST",
                    url: "./../api/SRSubType/DeleteSRSubType?ts=" + new Date().getTime(),
                    data: id,
                    contentType: "application/json"
                });
            };

            return subtypelist;
        }
    ]);