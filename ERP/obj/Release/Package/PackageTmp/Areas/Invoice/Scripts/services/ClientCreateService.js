/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
    .service("ClientCreateService", [
        "$http",
        function ($http) {

            var list = {};

            list.RetrieveCountry = function () {
                return $http({
                    method: 'GET',
                    url: "/api/InvoiceClient/RetrieveCountry?ts=" + new Date().getTime()
                });
            };

            list.LoadClientSource = function () {
                return $http({
                    method: 'GET',
                    url: "/api/InvoiceClient/LoadClientSource?ts=" + new Date().getTime()
                });
            };

            list.RetrieveState = function (countryId) {
                return $http({
                    method: 'GET',
                    url: "/api/InvoiceClient/RetrieveState?ts=" + new Date().getTime() + "&countryId=" + countryId
                });
            };

            list.GetBusinessTypeList = function () {
                return $http({
                    method: 'GET',
                    url: "/api/InvoiceClient/GetBusinessTypeList?ts=" + new Date().getTime()
                });
            };

            list.GetDocuments = function () {
                return $http({
                    method: 'GET',
                    url: "/api/InvoiceClient/GetDocuments?ts=" + new Date().getTime()
                });
            };

            list.CreateUpdatePartialClient = function (data) {
                return $http({
                    method: "POST",
                    url: "/api/InvoiceClient/SaveInvoiceClientPartial?ts=" + new Date().getTime(),
                    data: data,
                    contentType: "application/json"
                });
            };

            list.GetClient = function (clientId) {
                return $http({
                    method: 'GET',
                    url: "/api/InvoiceClient/GetClient?ts=" + new Date().getTime() + "&cId=" + clientId
                });
            };

            list.CreateUpdateFullClient = function (data) {
                return $http({
                    method: "POST",
                    url: "/api/InvoiceClient/SaveInvoiceClientFull?ts=" + new Date().getTime(),
                    data: data,
                    contentType: "application/json"
                });
            };

            list.SaveDirector = function (data) {
                return $http({
                    method: "POST",
                    url: "/api/InvoiceClient/SaveDirector?ts=" + new Date().getTime(),
                    data: data,
                    contentType: "application/json"
                });
            };

            list.SaveDocument = function (data) {
                return $http({
                    method: "POST",
                    url: "/api/InvoiceClient/SaveDocument?ts=" + new Date().getTime(),
                    data: data,
                    contentType: "application/json"
                });
            };

            list.DeleteDocument = function (docId) {
                return $http({
                    method: 'GET',
                    url: "/api/InvoiceClient/DeleteDocument?ts=" + new Date().getTime() + "&docId=" + docId
                });
            };

            list.DeleteDirector = function (Id) {
                return $http({
                    method: 'GET',
                    url: "/api/InvoiceClient/DeleteDirector?ts=" + new Date().getTime() + "&Id=" + Id
                });
            };
            
            list.IsClientConfirmed = function (cId) {
                return $http({
                    method: 'GET',
                    url: "/api/InvoiceClient/IsClientConfirmed?ts=" + new Date().getTime() + "&cId=" + cId
                });
            };

            list.GetClientList = function (filterKYCApproved, filterCountryId, timezone, page, count, orderby, filter) {
                return $http({
                    method: "POST",
                    data: filter,
                    contentType: "application/json",
                    url: "/api/InvoiceClient/GetClientList?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&KYCApproved=" + filterKYCApproved + "&countryId=" + filterCountryId
                });
            };

            list.DeleteClient = function (id) {
                return $http({
                    method: "POST",
                    url: "/api/InvoiceClient/DeleteClient?ts=" + new Date().getTime(),
                    data: id,
                    contentType: "application/json"
                });
            };

            list.ChangeStatus = function (id, status) {
                return $http({
                    method: "POST",
                    url: "/api/InvoiceClient/ChangeStatus?ts=" + new Date().getTime(),
                    data: JSON.stringify({ PKClientId: id, IsActive: status }),
                    contentType: "application/json"
                });
            };

            list.GenerateLink = function (cId) {
                return $http({
                    method: 'GET',
                    url: "/api/InvoiceClient/GenerateLink?ts=" + new Date().getTime() + "&cId=" + cId
                });
            };

            list.DeleteLink = function (cId) {
                return $http({
                    method: 'GET',
                    url: "/api/InvoiceClient/DeleteLink?ts=" + new Date().getTime() + "&cId=" + cId
                });
            };

            list.ApproveKYC = function (cId) {
                return $http({
                    method: 'GET',
                    url: "/api/InvoiceClient/ApproveKYC?ts=" + new Date().getTime() + "&cId=" + cId
                });
            };

            list.CheckClientCode = function (cCode) {
                return $http({
                    method: 'GET',
                    url: "/api/InvoiceClient/CheckClientCode?ts=" + new Date().getTime() + "&code=" + cCode
                });
            };

            list.GetClientOverView = function (cId) {
                return $http({
                    method: 'GET',
                    url: "/api/InvoiceClient/GetClientOverView?ts=" + new Date().getTime() + "&cId=" + cId
                });
            };

            return list;
        }
    ]);