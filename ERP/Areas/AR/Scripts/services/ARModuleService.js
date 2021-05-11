/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
    .service("ARModuleService", [
        "$http",
        function ($http) {

            var list = {};

            /*get module list*/
            list.GetModuleList = function (timezone, page, count, orderby, filter) {
                return  $http({
                    method: "GET",
                    url: "/api/ARModule/GetModuleList?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&filter=" + filter
                });
            };

            /*add edit module*/
            list.CreateUpdateModule = function (data) {
                return $http({
                    method: "POST",
                    url: "/api/ARModule/SaveModule?ts=" + new Date().getTime(),
                    data: data,
                    contentType: "application/json"
                });
            };

            /*update seq number*/
            list.UpdateSequenceNo = function (seqNo, moduleId) {
                return $http({
                    method: "GET",
                    url: "/api/ARModule/UpdateSequenceNo?ts=" + new Date().getTime() + "&seqNo=" + seqNo + "&moduleId=" + moduleId
                });
            };

            /*delete module*/
            list.DeleteModule = function (id) {
                return $http({
                    method: "POST",
                    url: "/api/ARModule/DeleteModule?ts=" + new Date().getTime(),
                    data: id,
                    contentType: "application/json"
                });
            };
            
            /*active inactive module*/
            list.ChangeStatus = function (id, status) {
                return $http({
                    method: "GET",
                    url: "/api/ARModule/ChangeStatus?ts=" + new Date().getTime() + "&moduleId=" + id + "&isActive=" + status
                });
            };

            return list;
        }
    ]);