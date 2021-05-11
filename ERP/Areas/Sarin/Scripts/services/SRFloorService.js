/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
    .service("SRFloorService", [
        "$http",
        function ($http) {

            var floorlist = {};

            /*retrieve SR Machine list*/
            floorlist.RetrieveSRMachine = function () {
                return $http({
                    method: 'GET',
                    url: "/api/SRFloor/RetrieveSRMachineList?ts=" + new Date().getTime(),
                });
            }

            /*retrieve Locations list*/
            floorlist.RetrieveLocations = function () {
                return $http({
                    method: 'GET',
                    url: "/api/SRFloor/RetrieveLocationList?ts=" + new Date().getTime(),
                });
            }

            /*get SR-Floor list*/
            floorlist.GetSRFloorList = function (timezone, page, count, orderby, filter1, filter2, filter3, filter4) {
                return $http({
                    method: "GET",
                    url: "/api/SRFloor/GetSRFloorList?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&filter1=" + filter1 + "&filter2=" + filter2 + "&filter3=" + filter3 + "&filter4=" + filter4
                });
            };

            /*add edit SR-Floor*/
            floorlist.CreateUpdateSRFloor = function (data, _selectedMachine) {
                return $http({
                    method: "POST",
                    url: "/api/SRFloor/SaveSRFloor?ts=" + new Date().getTime() + "&selectedMachine=" + _selectedMachine.join(),
                    data: data,
                    contentType: "application/json"
                });
            };

            /*delete SR-Floor*/
            floorlist.DeleteSRFloor = function (id) {
                return $http({
                    method: "POST",
                    url: "/api/SRFloor/DeleteSRFloor?ts=" + new Date().getTime(),
                    data: id,
                    contentType: "application/json"
                });
            };

            return floorlist;
        }
    ]);