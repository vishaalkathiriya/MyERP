/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
    .service("ARSubModuleService", [
        "$http",
        function ($http) {

            var list = {};

            /*Save SubModule*/
            list.CreateUpdateSubModule = function (data) {
                return $http({
                    method: "POST",
                    url: "/api/ARSubModule/CreateUpdateSubModule?ts=" + new Date().getTime(),
                    data: data,
                    contentType: "application/json"
                });
            };

            /*get module list*/
            list.GetModuleList = function () {
                return $http({
                    method: "GET",
                    url: "/api/ARSubModule/GetModuleList?ts=" + new Date().getTime()
                });
            };

            /*update seq number*/
            list.UpdateSequenceNo = function (seqNo, subModuleId) {
                return $http({
                    method: "GET",
                    url: "/api/ARSubModule/UpdateSequenceNo?ts=" + new Date().getTime() + "&seqNo=" + seqNo + "&subModuleId=" + subModuleId
                });
            };

            /*get sub module list*/
            list.GetSubModuleList = function (timezone, page, count, orderby, filter) {
                return $http({
                    method: "POST",
                    data: filter,
                    url: "/api/ARSubModule/GetSubModuleList?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby,
                    contentType: "application/json"
                });
            };

            /*get access permission list*/
            list.GetAccessPermissionList = function (isDashboard, isEmployee, isInvoice) {
                return $http({
                    method: "GET",
                    url: "/api/ARSubModule/GetAccessPermissionList?ts=" + new Date().getTime() + "&isDashboard=" + isDashboard + "&isEmployee=" + isEmployee + "&isInvoice=" + isInvoice
                });
            };

            /*delete sub module*/
            list.DeleteSubModule = function (id) {
                return $http({
                    method: "POST",
                    url: "./../api/ARSubModule/DeleteSubModule?ts=" + new Date().getTime(),
                    data: JSON.stringify({ SubModuleId: id }),
                    contentType: "application/json"
                });
            };

            /*active inactive sub module*/
            list.ChangeStatus = function (id, status) {
                return $http({
                    method: "POST",
                    url: "/api/ARSubModule/ChangeStatus?ts=" + new Date().getTime(),
                    data: JSON.stringify({ SubModuleId: id, isactive: status }),
                    contentType: "application/json"
                });
            };

            return list;
        }
    ]);