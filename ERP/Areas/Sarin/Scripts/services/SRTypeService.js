/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
    .service("SRTypeService", [
        "$http",
        function ($http) {

            var typelist = {};

            /*get SR-Type list*/
            typelist.GetSRTypeList = function (timezone, page, count, orderby, filter, filter1) {
                return $http({
                    method: "GET",
                    url: "./../api/SRType/GetSRTypeList?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&filter=" + filter + "&filter1=" + filter1
                });
            };

            /*add edit SR-Type*/
            typelist.CreateUpdateSRType = function (data) {
                return $http({
                    method: "POST",
                    url: "./../api/SRType/SaveSRType?ts=" + new Date().getTime(),
                    data: data,
                    contentType: "application/json"
                });
            };

            /*delete SR-Type*/
            typelist.DeleteSRType = function (id) {
                return $http({
                    method: "POST",
                    url: "./../api/SRType/DeleteSRType?ts=" + new Date().getTime(),
                    data: id,
                    contentType: "application/json"
                });
            };


            //typelist.FetchType = function (typeId) {
            //    return $http({
            //        method: 'GET',
            //        url: "./../../../api/SRType/FetchType?ts=" + new Date().getTime() + "&TypeId=" + typeId
            //    });
            //};

            return typelist;
        }
    ]);