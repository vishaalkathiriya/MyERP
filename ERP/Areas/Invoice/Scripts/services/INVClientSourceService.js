/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
    .service("INVClientSourceService", [
        "$http",
        function ($http) {

            var clientSourcelist = {};

            /*get designation group list*/
            clientSourcelist.GetClientSourceList = function (timezone, page, count, orderby, filter) {
                return $http({
                    method: "GET",
                    url: "/api/INVClientSource/GetClientSourceList?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&filter=" + filter
                });
            };

            /*add edit designation group*/
            clientSourcelist.CreateUpdateClientSource = function (data) {
                return $http({
                    method: "POST",
                    url: "/api/INVClientSource/SaveClientSource?ts=" + new Date().getTime(),
                    data: data,
                    contentType: "application/json"
                });
            };

            /*delete designation group*/
            clientSourcelist.DeleteClientSource = function (id) {
                return $http({
                    method: "POST",
                    url: "/api/INVClientSource/DeleteClientSource?ts=" + new Date().getTime(),
                    data: id,
                    contentType: "application/json"
                });
            };

            /*active inactive designation group*/
            clientSourcelist.ChangeStatus = function (id, status) {
                return $http({
                    method: "POST",
                    url: "/api/INVClientSource/ChangeStatus?ts=" + new Date().getTime(),
                    data: JSON.stringify({ PKSourceId: id, IsActive: status }),
                    contentType: "application/json"
                });
            };

            return clientSourcelist;
        }
    ]);