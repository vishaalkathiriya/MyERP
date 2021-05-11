/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
.service("FireHydrantSpprinklerService", [
    "$http",
    function ($http) {
        var list = {};
        //BEGIN ADD AND UPDATE   FIRE HYDRANT SPRINKLER  
        list.CreateUpdateFireHydrantSprinkler = function (data, _timezone) {
            return $http({
                method: "POST",
                url: "/api/HRDFireHydrantSprinklerSystem/SaveFireHydrantSprinklerSystem?ts=" + new Date().getTime() + "&timezone=" + _timezone,
                data: data,
                contentType: "application/json"
            });
        };
        //END ADD AND UPDATE   FIRE HYDRANT SPRINKLER  

        //BEGIN GET   FIRE HYDRANT SPRINKLER  
        list.GetFireHydrantSprinklerSystemLog = function (timezone, page, count, orderby, BuildingName, DateOfInspectione, CheckedBy, Findings, RootCause, CorrectiveActionTaken, Remark, startDate, endDate) {
            return $http({
                method: "GET",
                url: "/api/HRDFireHydrantSprinklerSystem/GetFireHydrantSprinklerSystem?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&BuildingName=" + BuildingName + "&DateOfInspectione=" + DateOfInspectione + "&CheckedBy=" + CheckedBy + "&Findings=" + Findings + "&RootCause=" + RootCause + "&CorrectiveActionTaken=" + CorrectiveActionTaken + "&Remark=" + Remark + "&startDate=" + startDate + "&endDate=" + endDate
            });
        };
        //END GET   FIRE HYDRANT SPRINKLER  

        //BEGIN DELETE   FIRE HYDRANT SPRINKLER  
        list.DeleteFireHydrantSprinkler = function (id) {
            return $http({
                method: "POST",
                url: "/api/HRDFireHydrantSprinklerSystem/DeleteFireHydrantSprinklerSystem?ts=" + new Date().getTime(),
                data: id,
                contentType: "application/json"
            });
        };
        //END DELETE   FIRE HYDRANT SPRINKLER  
        return list;
    }]);