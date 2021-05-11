/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services").service("LocationService", [
    "$http",
    function ($http) {
        var Location = {};

        //Get Location List
        Location.GetLocationList = function (timezone, page, count, orderby, filter) {
            return $http({
                method: "GET",
                url: "/api/Location/GetLocList??ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&filter=" + filter
            });
        };

        //add Location
        Location.SaveLocation = function (_location) {
            return $http({
                method: "POST",
                url: "/api/Location/SaveLocation?ts=" + new Date().getTime(),
                data: _location,
                contentType: "application/json;charset=utf-8"
            });
        };

        /*Delete Location*/
        Location.DeleteLocation = function (_location) {
            return $http({
                method: "POST",
                url: "/api/Location/DeleteLocation?ts=" + new Date().getTime(),
                data: _location,
                contentType: 'application/json; charset=utf-8'
            });
        }

        Location.IsActive = function (_location) {
            
            return $http({
                method: "POST",
                url: "/api/Location/ChangeStatus?ts=" + new Date().getTime(),
                data: _location,
                contentType: "application/json"
            });
        }

        return Location;
    }
])