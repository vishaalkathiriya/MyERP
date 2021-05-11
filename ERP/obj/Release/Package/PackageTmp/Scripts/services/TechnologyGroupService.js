/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
    .service("TechnologyGroupService", [
        "$http",
        function ($http) {

            var techgrouplist = {};

            /*get tech group list*/
            techgrouplist.GetTechGroupList = function (timezone, page, count, orderby, filter) {
                return $http({
                    method: "GET",
                    url: "/api/TechGroup/GetTechGroupList?ts="+new Date().getTime()+"&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&filter=" + filter
                });
            };

            /*add edit tech group*/
            techgrouplist.CreateUpdateTechGroup = function (data) {
                return $http({
                    method: "POST",
                    url: "/api/TechGroup/SaveTechGroup?ts=" + new Date().getTime(),
                    data: data,
                    contentType: "application/json"
                });
            };

            /*delete tech group*/
            techgrouplist.DeleteTechGroup = function (id) {
                return $http({
                    method: "POST",
                    url: "/api/TechGroup/DeleteTechGroup?ts=" + new Date().getTime(),
                    data: id,
                    contentType: "application/json"
                });
            };

            /*change status of tech group*/
            techgrouplist.ChangeStatus = function (id, status) {
                return $http({
                    method: "POST",
                    url: "/api/TechGroup/ChangeStatus?ts=" + new Date().getTime(),
                    data: JSON.stringify({ id: id, isactive: status }),
                    contentType: "application/json"
                });
            };

            return techgrouplist;
        }
    ]);