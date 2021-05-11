/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
    .service("DesignationGroupService", [
        "$http",
        function ($http) {

            var designationlist = {};

            /*get designation group list*/
            designationlist.GetDesignationGroupList = function (timezone, page, count, orderby, filter) {
                return $http({
                    method: "GET",
                    url: "/api/DesignationGroup/GetDesignationGroupList?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&filter=" + filter
                });
            };

            /*add edit designation group*/
            designationlist.CreateUpdateDesignationGroup = function (data) {
                return $http({
                    method: "POST",
                    url: "/api/DesignationGroup/SaveDesignationGroup?ts=" + new Date().getTime(),
                    data: data,
                    contentType: "application/json"
                });
            };

            /*delete designation group*/
            designationlist.DeleteDesignationGroup = function (id) {
                return $http({
                    method: "POST",
                    url: "/api/DesignationGroup/DeleteDesignationGroup?ts=" + new Date().getTime(),
                    data: id,
                    contentType: "application/json"
                });
            };

            /*active inactive designation group*/
            designationlist.ChangeStatus = function (id, status) {
                return $http({
                    method: "POST",
                    url: "/api/DesignationGroup/ChangeStatus?ts=" + new Date().getTime(),
                    data: JSON.stringify({ id: id, isactive: status }),
                    contentType: "application/json"
                });
            };

            return designationlist;
        }
    ]);