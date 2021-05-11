/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
    .service("IssuedDocumentService", [
        "$http",
        function ($http) {

            var documentlist = {};

            documentlist.RetrieveDocument = function () {
                return $http({
                    method: 'GET',
                    url: "/api/IssuedDocument/RetrieveDocument?ts=" + new Date().getTime()
                });
            };

            /*get document list*/
            documentlist.GetDocumentList = function (DocumentTypeId,startDate, endDate, timezone, page, count, orderby, filter) {
                return $http({
                    method: "POST",
                    data: filter,
                    url: "/api/IssuedDocument/GetDocumentList?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&DocumentTypeId=" + DocumentTypeId + "&startDate=" + startDate + "&endDate=" + endDate,
                    contentType: "application/json"
                });
            };

            /*add edit document*/
            documentlist.CreateUpdateDocument = function (data, timezone) {
                return $http({
                    method: "POST",
                    url: "/api/IssuedDocument/SaveDocument?ts=" + new Date().getTime() + "&timezone=" + timezone,
                    data: data,
                    contentType: "application/json"
                });
            };

            /*delete document*/
            documentlist.DeleteDocument = function (id) {
                return $http({
                    method: "POST",
                    url: "/api/IssuedDocument/DeleteDocument?ts=" + new Date().getTime(),
                    data: id,
                    contentType: "application/json"
                });
            };

            return documentlist;
        }
    ]);