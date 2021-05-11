/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
    .service("DocumentService", [
        "$http",
        function ($http) {

            var documentlist = {};

            /*get document list*/
            documentlist.GetDocumentList = function (timezone, page, count, orderby, filter) {
                return  $http({
                    method: "GET",
                    url: "/api/Documents/GetDocumentList?ts=" + new Date().getTime()+"&timezone=" + timezone + "&page=" + page + "&count=" + count +"&orderby="+orderby+"&filter="+filter
                });
            };

            /*get document type list*/
            documentlist.GetDocumentTypeList = function () {
                return $http({
                    method: "GET",
                    url: "/api/Documents/GetDocumentTypeList?ts=" + new Date().getTime()
                });
            };

            /*add edit document*/
            documentlist.CreateUpdateDocument = function (data) {
                return $http({
                    method: "POST",
                    url: "/api/Documents/SaveDocument?ts=" + new Date().getTime(),
                    data: data,
                    contentType: "application/json"
                });
            };

            /*delete document*/
            documentlist.DeleteDocument = function (id) {
                return $http({
                    method: "POST",
                    url: "/api/Documents/DeleteDocument?ts=" + new Date().getTime(),
                    data: id,
                    contentType: "application/json"
                });
            };
            
            /*active inactive document*/
            documentlist.ChangeStatus = function (id, status) {
                return $http({
                    method: "POST",
                    url: "/api/Documents/ChangeStatus?ts=" + new Date().getTime(),
                    data: JSON.stringify({ id: id, isactive:status }),
                    contentType: "application/json"
                });
            };

            return documentlist;
        }
    ]);