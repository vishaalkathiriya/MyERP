/// <reference path="../../../../Scripts/libs/angular/angular.js" />

angular.module("ERPApp.Services")
    .service("InvoiceService", [
        "$http",
        function ($http) {
            var list = {};

            list.GetTaxList = function () {
                return $http({
                    method: "GET",
                    url: "/api/Invoice/GetTaxList?ts=" + new Date().getTime(),
                    contentType: "application/json"
                });
            };

            list.GetMilestoneList = function (milestoneIds) {
                return $http({
                    method: "GET",
                    url: "/api/Invoice/GetMilestoneList?ts=" + new Date().getTime() + "&milestoneIds=" + milestoneIds,
                    contentType: "application/json"
                });
            };

            list.GetInvoice = function (invoiceId) {
                return $http({
                    method: "GET",
                    url: "/api/Invoice/GetInvoice?ts=" + new Date().getTime() + "&invoiceId=" + invoiceId,
                    contentType: "application/json"
                });
            };

            list.GetClientCountry = function (clientId) {
                return $http({
                    method: "GET",
                    url: "/api/Invoice/GetClientCountry?ts=" + new Date().getTime() + "&clientId=" + clientId,
                    contentType: "application/json"
                });
            };

            list.GetClient = function (clientId) {
                return $http({
                    method: "GET",
                    url: "/api/Invoice/GetClient?ts=" + new Date().getTime() + "&clientId=" + clientId,
                    contentType: "application/json"
                });
            };

            list.DeleteInvoice = function (invoiceId) {
                return $http({
                    method: "GET",
                    url: "/api/Invoice/DeleteInvoice?ts=" + new Date().getTime() + "&invoiceId=" + invoiceId,
                    contentType: "application/json"
                });
            };

            list.DeleteInvoiceTax = function (invoiceTaxId) {
                return $http({
                    method: "GET",
                    url: "/api/Invoice/DeleteInvoiceTax?ts=" + new Date().getTime() + "&invoiceTaxId=" + invoiceTaxId,
                    contentType: "application/json"
                });
            };

            list.CopyInvoice = function (invoiceId) {
                return $http({
                    method: "GET",
                    url: "/api/Invoice/CopyInvoice?ts=" + new Date().getTime() + "&invoiceId=" + invoiceId,
                    contentType: "application/json"
                });
            };

            list.RetrieveInvoiceList = function (filterClientId, timezone, page, count, orderby, code, currency) {
                return $http({
                    method: "GET",
                    url: "/api/Invoice/RetrieveInvoiceList?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&code=" + code + "&currency=" + currency + "&clientId=" + filterClientId,
                    contentType: "application/json; charset=utf-8"
                });
            };

            list.CreateUpdateInvoice = function (invoice) {
                return $http({
                    method: "POST",
                    data: invoice,
                    url: "/api/Invoice/CreateUpdateInvoice?ts=" + new Date().getTime(),
                    contentType: "application/json"
                });
            };

            list.SaveInvoiceTax = function (tax) {
                return $http({
                    method: "POST",
                    data: tax,
                    url: "/api/Invoice/SaveInvoiceTax?ts=" + new Date().getTime(),
                    contentType: "application/json"
                });
            };

            return list;
        }
    ]);