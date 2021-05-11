/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
    .service("DesignationParentService", [
        "$http",
        function ($http) {

            var designationlist = {};

            /*get designation parent list*/
            designationlist.GetDesignationParentList = function (timezone, page, count, orderby, filter) {
                return $http({
                    method: "GET",
                    url: "/api/DesignationParent/GetDesignationParentList?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&filter=" + filter
                });
            };

            /*add edit designation parent*/
            designationlist.CreateUpdateDesignationParent = function (data) {
                return $http({
                    method: "POST",
                    url: "/api/DesignationParent/SaveDesignationParent?ts=" + new Date().getTime(),
                    data: data,
                    contentType: "application/json"
                });
            };

            /*delete designation parent*/
            designationlist.DeleteDesignationParent = function (id) {
                return $http({
                    method: "POST",
                    url: "/api/DesignationParent/DeleteDesignationParent?ts=" + new Date().getTime(),
                    data: id,
                    contentType: "application/json"
                });
            };

            /*active inactive designation parent*/
            designationlist.ChangeStatus = function (id, status) {
                return $http({
                    method: "POST",
                    url: "/api/DesignationParent/ChangeStatus?ts=" + new Date().getTime(),
                    data: JSON.stringify({ id: id, isactive: status }),
                    contentType: "application/json"
                });
            };

            return designationlist;
        }
    ]);