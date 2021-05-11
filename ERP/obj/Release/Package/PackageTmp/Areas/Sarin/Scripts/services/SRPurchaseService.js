/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
    .service("SRPurchaseService", [
        "$http",
        function ($http) {

            var purchaselist = {};

            /*retrieve SR Parts list*/
            purchaselist.RetrieveSRParts = function () {
                return $http({
                    method: 'GET',
                    url: "./../api/SRPurchase/RetrieveSRPartList?ts=" + new Date().getTime(),
                });
            }

            /*get SR-Purchase list*/
            purchaselist.GetSRPurchaseList = function (timezone, page, count, orderby, filter1, filter2, filter3) {
                return $http({
                    method: "GET",
                    url: "./../api/SRPurchase/GetSRPurchaseList?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&filter1=" + filter1 + "&filter2=" + filter2 + "&filter3=" + filter3
                });
            };

            /*add edit SR-Purchase*/
            purchaselist.CreateUpdateSRPurchage = function (data) {
                return $http({
                    method: "POST",
                    url: "./../api/SRPurchase/SaveSRPurchase?ts=" + new Date().getTime(),
                    data: data,
                    contentType: "application/json"
                });
            };

            /*delete SR-Purchase*/
            purchaselist.DeleteSRPurchase = function (id) {
                return $http({
                    method: "POST",
                    url: "./../api/SRPurchase/DeleteSRPurchase?ts=" + new Date().getTime(),
                    data: id,
                    contentType: "application/json"
                });
            };

            return purchaselist;
        }
    ]);