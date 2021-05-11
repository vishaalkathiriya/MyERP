/// <reference path="../../../../Scripts/libs/angular/angular.js" />

angular.module("ERPApp.Services")
    .service("ReportInvoiceService", [
        "$http",
        function ($http) {
            var app = {};

            app.RetrieveInvoices = function (filterClientId, filterInvoiceType, filterCurrency, startDate, endDate, filterStatus, timezone, page, count, orderby, code, currency) {
                return $http({
                    method: "GET",
                    url: "/api/Report/RetrieveInvoices?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&code=" + code + "&currency=" + currency + "&filterClientId=" + filterClientId + "&filterInvoiceType=" + filterInvoiceType + "&filterCurrency=" + filterCurrency + "&startDate=" + startDate + "&endDate=" + endDate + "&filterStatus=" + filterStatus,
                    contentType: "application/json; charset=utf-8"
                });
            };

            app.GetClientList = function () {
                return $http({
                    method: "GET",
                    url: "/api/Report/GetClientList?ts=" + new Date().getTime(),
                    contentType: "application/json"
                });
            };

            app.RetrieveInvoiceMilestones = function (milestoneIds) {
                return $http({
                    method: "GET",
                    url: "/api/Report/RetrieveInvoiceMilestones?ts=" + new Date().getTime() + "&milestoneIds=" + milestoneIds,
                    contentType: "application/json"
                });
            };

            app.GetCurrencyList = function () {
                return $http({
                    method: 'GET',
                    url: "/api/Conversation/GetCurrencyList?ts=" + new Date().getTime()
                });
            };

            return app;
        }
    ]);