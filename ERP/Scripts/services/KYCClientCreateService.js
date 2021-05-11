/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
    .service("KYCClientCreateService", [
        "$http",
        function ($http) {

            var list = {};

            list.RetrieveCountry = function () {
                return $http({
                    method: 'GET',
                    url: "/api/KYCClient/RetrieveCountry?ts=" + new Date().getTime()
                });
            };

            list.LoadClientSource = function () {
                return $http({
                    method: 'GET',
                    url: "/api/KYCClient/LoadClientSource?ts=" + new Date().getTime()
                });
            };

            list.ValidateURL = function (urlkey) {
                return $http({
                    method: 'GET',
                    url: "/api/KYCClient/ValidateURL?ts=" + new Date().getTime() + "&key=" + urlkey
                });
            };


            list.RetrieveState = function (countryId) {
                return $http({
                    method: 'GET',
                    url: "/api/KYCClient/RetrieveState?ts=" + new Date().getTime() + "&countryId=" + countryId
                });
            };

            list.GetBusinessTypeList = function () {
                return $http({
                    method: 'GET',
                    url: "/api/KYCClient/GetBusinessTypeList?ts=" + new Date().getTime()
                });
            };

            list.GetDocuments = function () {
                return $http({
                    method: 'GET',
                    url: "/api/KYCClient/GetDocuments?ts=" + new Date().getTime()
                });
            };

            list.CreateUpdatePartialClient = function (data) {
                return $http({
                    method: "POST",
                    url: "/api/KYCClient/SaveInvoiceClientPartial?ts=" + new Date().getTime(),
                    data: data,
                    contentType: "application/json"
                });
            };

            list.CreateUpdateFullClient = function (data) {
                return $http({
                    method: "POST",
                    url: "/api/KYCClient/SaveInvoiceClientFull?ts=" + new Date().getTime(),
                    data: data,
                    contentType: "application/json"
                });
            };

            list.SaveDirector = function (data) {
                return $http({
                    method: "POST",
                    url: "/api/KYCClient/SaveDirector?ts=" + new Date().getTime(),
                    data: data,
                    contentType: "application/json"
                });
            };

            list.SaveDocument = function (data) {
                return $http({
                    method: "POST",
                    url: "/api/KYCClient/SaveDocument?ts=" + new Date().getTime(),
                    data: data,
                    contentType: "application/json"
                });
            };

            list.DeleteDocument = function (docId) {
                return $http({
                    method: 'GET',
                    url: "/api/KYCClient/DeleteDocument?ts=" + new Date().getTime() + "&docId=" + docId
                });
            };

            list.DeleteDirector = function (Id) {
                return $http({
                    method: 'GET',
                    url: "/api/KYCClient/DeleteDirector?ts=" + new Date().getTime() + "&Id=" + Id
                });
            };
           

            return list;
        }
    ]);