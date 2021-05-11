/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
    .service("SRAmcService", [
        "$http",
        function ($http) {

            var amclist = {};

            /*retrieve SR Machine*/
            amclist.RetrieveSRMachine = function () {
                return $http({
                    method: 'GET',
                    url: "./../api/SRAmc/RetrieveSRMachineList?ts=" + new Date().getTime(),
                });
            }

            /*get SR-AMC list*/
            amclist.GetSRAmcList = function (timezone, page, count, orderby, filter1) {
                return $http({
                    method: "GET",
                    url: "./../api/SRAmc/GetSRAmcList?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&filter1=" + filter1
                });
            };

            /*add edit SR-AMC*/
            amclist.CreateUpdateSRAmc = function (data, _selectedMachine) {
                return $http({
                    method: "POST",
                    url: "./../api/SRAmc/SaveSRAmc?ts=" + new Date().getTime() + "&selectedMachine=" + _selectedMachine.join(),
                    data: data,
                    contentType: "application/json"
                });
            };

            /*delete SR-AMC*/
            amclist.DeleteSRAmc = function (id) {
                return $http({
                    method: "POST",
                    url: "./../api/SRAmc/DeleteSRAmc?ts=" + new Date().getTime(),
                    data: id,
                    contentType: "application/json"
                });
            };

            return amclist;
        }
    ]);