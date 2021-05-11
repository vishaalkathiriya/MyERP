/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
    .service("SRRepairService", [
        "$http",
        function ($http) {

            var srrepairlist = {};

            /*retrieve SR Machine list*/
            srrepairlist.RetrieveSRMachine = function () {
                return $http({
                    method: 'GET',
                    url: "./../api/SRRepair/RetrieveSRMachineList?ts=" + new Date().getTime(),
                });
            }

            /*retrieve SR Part list*/
            srrepairlist.RetrieveSRPart = function () {
                return $http({
                    method: 'GET',
                    url: "./../api/SRRepair/RetrieveSRPartList?ts=" + new Date().getTime(),
                });
            }

            /*get SR-Repair list*/
            srrepairlist.GetSRRepairList = function (timezone, page, count, orderby, filter1, filter2, filter3, filter4, filter5) {
                return $http({
                    method: "GET",
                    url: "./../api/SRRepair/GetSRepairList?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&filter1=" + filter1 + "&filter2=" + filter2 + "&filter3=" + filter3 + "&filter4=" + filter4 + "&filter5=" + filter5
                });
            };

            /*add edit SR-Repair*/
            srrepairlist.CreateUpdateSRRepair = function (data) {
                return $http({
                    method: "POST",
                    url: "./../api/SRRepair/SaveSRRepair?ts=" + new Date().getTime(),
                    data: data,
                    contentType: "application/json"
                });
            };

            /*delete SR-Repair*/
            srrepairlist.DeleteSRRepair = function (id) {
                return $http({
                    method: "POST",
                    url: "./../api/SRRepair/DeleteSRRepair?ts=" + new Date().getTime(),
                    data: id,
                    contentType: "application/json"
                });
            };

            return srrepairlist;
        }
    ]);