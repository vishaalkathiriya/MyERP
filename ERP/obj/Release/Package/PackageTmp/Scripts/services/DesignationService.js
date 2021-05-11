/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
    .service("DesignationService", [
        "$http",
        function ($http) {

            var designationlist = {};

            /*retrieve designation group list*/
            designationlist.RetrieveDesignationGroup = function () {
                return $http({
                    method: 'GET',
                    url: "/api/Designation/RetrieveDesignationGroupList?ts=" + new Date().getTime(),
                });
            }

            /*retrieve designation parent list*/
            designationlist.RetrieveDesignationParent = function () {
                return $http({
                    method: 'GET',
                    url: "/api/Designation/RetrieveDesignationParentList?ts=" + new Date().getTime(),
                });
            }


            /*get designation list*/
            designationlist.GetDesignationList = function (timezone, page, count, orderby, filter1, filter2, filter3) {
                return $http({
                    method: "GET",
                    url: "/api/Designation/GetDesignationList?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&filter1=" + filter1 + "&filter2=" + filter2 + "&filter3=" + filter3
                });
            };

            /*add edit designation*/
            designationlist.CreateUpdateDesignation = function (data) {
                return $http({
                    method: "POST",
                    url: "/api/Designation/SaveDesignation?ts=" + new Date().getTime(),
                    data: data,
                    contentType: "application/json"
                });
            };

            /*delete designation*/
            designationlist.DeleteDesignation = function (id) {
                return $http({
                    method: "POST",
                    url: "/api/Designation/DeleteDesignation?ts=" + new Date().getTime(),
                    data: id,
                    contentType: "application/json"
                });
            };

            /*active inactive designation*/
            designationlist.ChangeStatus = function (id, status) {
                return $http({
                    method: "POST",
                    url: "/api/Designation/ChangeStatus?ts=" + new Date().getTime(),
                    data: JSON.stringify({ id: id, isactive: status }),
                    contentType: "application/json"
                });
            };

            return designationlist;
        }
    ]);