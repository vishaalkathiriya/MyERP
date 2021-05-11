/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
    .service("SRMachineService", [
        "$http",
        function ($http) {

            var machinelist = {};

            /*retrieve sr type list*/
            machinelist.RetrieveSRType = function () {
                return $http({
                    method: 'GET',
                    url: "./../api/SRMachine/RetrieveSRTypeList?ts=" + new Date().getTime(),
                });
            }
            
            /*retrieve sr sub type list by type id*/
            machinelist.RetrieveSRSubTypeListByTypeId = function (typeId) {
                return $http({
                    method: 'GET',
                    url: "./../api/SRMachine/RetrieveSRSubTypeListByTypeId?ts=" + new Date().getTime() + "&TypeId=" + typeId,
                });
            }

            /*retrieve sr parameter list by sub Type Id*/
            machinelist.RetrieveSRParameterBySubTypeId = function (subTypeId) {
                return $http({
                    method: 'GET',
                    url: "./../api/SRMachine/RetrieveSRParameterListBySubTypeId?ts=" + new Date().getTime() + "&SubTypeId=" + subTypeId,
                });
            }

            /*get SR-Machine list*/
            machinelist.GetSRMachineList = function (timezone, page, count, orderby, filter1, filter2, filter3, filter4, filter5) {
                return $http({
                    method: "GET",
                    url: "./../api/SRMachine/GetSRMachineList?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&filter1=" + filter1 + "&filter2=" + filter2 + "&filter3=" + filter3 + "&filter4=" + filter4 + "&filter5=" + filter5
                });
            };

            /*add edit SR-Machine*/
            machinelist.CreateUpdateSRMachine = function (data) {
                return $http({
                    method: "POST",
                    url: "./../api/SRMachine/SaveSRMachien?ts=" + new Date().getTime(),
                    data: data,
                    contentType: "application/json"
                });
            };

            /*delete SR-Machine*/
            machinelist.DeleteSRMachine = function (id) {
                return $http({
                    method: "POST",
                    url: "./../api/SRMachine/DeleteSRMachine?ts=" + new Date().getTime(),
                    data: id,
                    contentType: "application/json"
                });
            };

            return machinelist;
        }
    ]);