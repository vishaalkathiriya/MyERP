/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
.service("BoilPlantCleaningService", [
    "$http",
    function($http)
    {
        var list = {};
        //BEGIN ADD AND UPDATE BOIL PLANT CLEANING RECORDS 
        list.CreateUpdatBoilPlantCleaning = function (data, _timezone) {
            return $http({
                method: "POST",
                url: "/api/HRDBoilPlantCleaningRecords/SaveBoilPlantClearingRecords?ts=" + new Date().getTime() + "&timezone=" + _timezone,
                data: data,
                contentType: "application/json"
            });
        };
        //END ADD AND UPDATE BOIL PLANT CLEANING RECORDS 

        //BEGIN GET BOIL PLANT CLEANING RECORDS
        list.GetBoilPlant = function (timezone, page, count, orderby, BoilPlantLocation, DateOfCleaining, NameOfCleaner, PlantIncharge, Remark, startDate, endDate) {
            return $http({
                method: "GET",
                url: "/api/HRDBoilPlantCleaningRecords/GetBoilPlantClearingRecords?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&BoilPlantLocation=" + BoilPlantLocation + "&DateOfCleaining=" + DateOfCleaining + "&NameOfCleaner=" + NameOfCleaner + "&PlantIncharge=" + PlantIncharge + "&Remark=" + Remark + "&startDate=" + startDate + "&endDate=" + endDate
            });
        };
        //END GET BOIL PLANT CLEANING RECORDS

        //BEGIN DELETE BOIL PLANT CLEANING RECORDS
        list.DeleteBoilPlantCleaning = function (id) {
            return $http({
                method: "POST",
                url: "/api/HRDBoilPlantCleaningRecords/DeleteBoilPlantClearingRecords?ts=" + new Date().getTime(),
                data: id,
                contentType: "application/json"
            });
        };
        //END DELETE BOIL PLANT CLEANING RECORDS
        return list;
    }]);