/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
.service("ChemicalStorageInspactionService", [
    "$http",
    function ($http) {
        var list = {};
        //BEGIN ADD AND UPDATE CHEMICAL STORAGE INSPACTION LOG 
        list.CreateUpdateChemicalInspactionRecordLog = function (data, _timezone) {
            return $http({
                method: "POST",
                url: "/api/HRDChemicalStorageInspactionLog/SaveChemicalStorageInspactionLog?ts=" + new Date().getTime() + "&timezone=" + _timezone,
                data: data,
                contentType: "application/json"
            });
        };
        //END ADD AND UPDATE CHEMICAL STORAGE INSPACTION LOG 

        //BEGIN GET CHEMICAL STORAGE INSPACTION LOG 
        list.GetChemicalInspactionLogRecord = function (timezone, page, count, orderby, DateOfInspectione, CheckedyBy, Findings, RootCause,CorrectiveAction, Remark, startDate, endDate) {
            return $http({
                method: "GET",
                url: "/api/HRDChemicalStorageInspactionLog/GetChemicalStorageInspactionLog?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&DateOfInspectione=" + DateOfInspectione + "&CheckedyBy=" + CheckedyBy + "&Findings=" + Findings + "&RootCause=" + RootCause +"&CorrectiveAction="+CorrectiveAction+"&Remark=" + Remark + "&startDate=" + startDate + "&endDate=" + endDate
            });
        };
        //END GET CHEMICAL STORAGE INSPACTION LOG 

        //BEGIN DELETE CHEMICAL STORAGE INSPACTION LOG 
        list.DeleteChemicalInspactionLog = function (id) {
            return $http({
                method: "POST",
                url: "/api/HRDChemicalStorageInspactionLog/DeleteChemicalStorageInspactionLog?ts=" + new Date().getTime(),
                data: id,
                contentType: "application/json"
            });
        };
        //END DELETE CHEMICAL STORAGE INSPACTION LOG 
        return list;
    }]);