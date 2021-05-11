/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
    .service("SRParameterService", [
        "$http",
        function ($http) {

            var parameterList = {};

            /*retrieve SR-Sub Type list*/
            parameterList.RetrieveSRSubType = function () {
                return $http({
                    method: 'GET',
                    url: "./../api/SRParameters/RetrieveSRSubTypeList?ts=" + new Date().getTime(),
                });
            }

            /*get SR-Parameter list*/
            parameterList.GetSRParameterList = function (timezone, page, count, orderby, filter1, filter2) {
                return $http({
                    method: "GET",
                    url: "./../api/SRParameters/GetSRParameterList?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&filter1=" + filter1 + "&filter2=" + filter2
                });
            };

            /*add edit Parameter*/
            parameterList.CreateUpdateSRParameter = function (data) {
                return $http({
                    method: "POST",
                    url: "./../api/SRParameters/SaveSRParameters?ts=" + new Date().getTime(),
                    data: data,
                    contentType: "application/json"
                });
            };

            /*delete SR-Parameter*/
            parameterList.DeleteSRParameter = function (id) {
                return $http({
                    method: "POST",
                    url: "./../api/SRParameters/DeleteSRParameter?ts=" + new Date().getTime(),
                    data: id,
                    contentType: "application/json"
                });
            };

            return parameterList;
        }
    ]);