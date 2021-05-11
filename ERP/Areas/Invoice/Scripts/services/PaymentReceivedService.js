/// <reference path="../../../../Scripts/libs/angular/angular.js" />

angular.module("ERPApp.Services")
    .service("PaymentReceivedService", [
        "$http",
        function ($http) {
            var list = {};

            /*list.GetClient = function (clientId) {
                return $http({
                    method: "GET",
                    url: "/api/Invoice/GetClient?ts=" + new Date().getTime() + "&clientId=" + clientId,
                    contentType: "application/json"
                });
            };

            */

            list.CreateUpdateInvoicePayment = function (payment, timezone) {
                return $http({
                    method: "POST",
                    data: payment,
                    url: "/api/Payment/CreateUpdateInvoicePayment?ts=" + new Date().getTime() + "&timezone=" + timezone,
                    contentType: "application/json"
                });
            };

            list.RetrievePaymentReceivedInvoiceList = function (filterClientId, timezone, page, count, orderby, code, currency) {
                return $http({
                    method: "GET",
                    url: "/api/Payment/RetrievePaymentReceivedInvoiceList?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&code=" + code + "&currency=" + currency + "&clientId=" + filterClientId,
                    contentType: "application/json; charset=utf-8"
                });
            };

            list.RetrievePayments = function (invoiceId, timezone) {
                return $http({
                    method: "GET",
                    url: "/api/Payment/RetrievePayments?ts=" + new Date().getTime() + "&timezone=" + timezone + "&invoiceId=" + invoiceId,
                    contentType: "application/json; charset=utf-8"
                });
            };

            list.DeletePayment = function (paymentId) {
                return $http({
                    method: "GET",
                    url: "/api/Payment/DeletePayment?ts=" + new Date().getTime() + "&paymentId=" + paymentId,
                    contentType: "application/json"
                });
            };

            list.ChangePaymentStatus = function (paymentId) {
                return $http({
                    method: "GET",
                    url: "/api/Payment/ChangePaymentStatus?ts=" + new Date().getTime() + "&paymentId=" + paymentId,
                    contentType: "application/json"
                });
            };

            return list;
        }
    ]);