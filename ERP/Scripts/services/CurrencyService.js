/// <reference path="../libs/angular/angular.js" />
angular.module("ERPApp.Services")
    .service("CurrencyService", [
        "$http",
        function ($http) {
            var currencylist = {};

            currencylist.CountryList = function () {
                return $http({
                    method: 'GET',
                    url: "/api/Currency/CountryList?ts=" + new Date().getTime(),
                });
            }

            // BEGIN GET CURRENCY LIST      
            currencylist.GetCurrencyList = function (timezone, page, count, orderby, CurrencyName, CurrencyCode, CountryName) {
                return $http({
                    method: "GET",
                    url: "/api/Currency/GetCurrencyList?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&CurrencyName=" + CurrencyName + "&CurrencyCode=" + CurrencyCode + "&CountryName=" + CountryName
                });
            };
            // END GET CURRENCY LIST 

            // BEGIN ADD AND UPDATE CURRENCY  INFORMATION 
            currencylist.CreateUpdateCurrency = function (data) {
                return $http({
                    method: "POST",
                    url: "/api/Currency/SaveCurrency?ts=" + new Date().getTime(),
                    data: data,
                    contentType: "application/json"
                });
            };
            // END ADD AND UPDATE CURRENCY  INFORMATION 

            //BEGIN DELETE  CURRENCY INFORMATION
            currencylist.DeleteCurrency = function (id) {
                return $http({
                    method: "GET",
                    url: "/api/Currency/DeleteCurrency?ts=" + new Date().getTime() + "&id=" + id,
                });
            };
            //END DELETE  CURRENCY INFORMATION

            //BEGIN  CHANGE CURRENCY STATUS
            currencylist.ChangeStatus = function (id) {
                return $http({
                    method: "GET",
                    url: "/api/Currency/ChangeStatus?ts=" + new Date().getTime() + "&id=" + id
                });
            };
            //END CHANGE CURRENCY STATUS

            currencylist.IsCurrencyCodeExists = function (code) {
                return $http({
                    method: "GET",
                    url: "/api/Currency/IsCurrencyCodeExists?ts=" + new Date().getTime() + "&code=" + code
                });
            };

            return currencylist;
        }
    ]);