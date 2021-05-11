/// <reference path="../libs/angular/angular.js" />
angular.module("ERPApp.Services")
          .service("AccidentRecordsService", [
          "$http",
           function ($http) {
               var list = {};

               //BEGIN ADD AND UPDATE ACCIDENT HELP INFORMATION
               list.CreateUpdateAccidentRecords = function (data, _timezone) {
                   return $http({
                       method: "POST",
                       url: "/api/HRDAccidentRecords/SaveAccidentRecords?ts=" + new Date().getTime() + "&timezone=" + _timezone,
                       data: data,
                       contentType: "application/json"
                   });
               };
               //END ADD AND UPDATE ACCIDENT HELP INFORMATION

               //BEGIN GET A LIST ACCIDENT HELP INFOMATION 
               list.GetAccidentRecords = function (timezone, page, count, orderby, TypeOfAccident, Department, ManagerName, NameOfInjuredPerson, RootCauseOfAccident, NoOfCasualities, CorrectiveActionTaken, Hospitalized, startDate, endDate) {
                   console.log(startDate +"  ----  "+ endDate)
                   return $http({
                       method: "GET",
                       url: "/api/HRDAccidentRecords/GetAccidentRecords?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&TypeOfAccident=" + TypeOfAccident + "&Department=" + Department + "&ManagerName=" + ManagerName + "&NameOfInjuredPerson=" + NameOfInjuredPerson + "&RootCauseOfAccident=" + RootCauseOfAccident + "&NoOfCasualities=" + NoOfCasualities + "&CorrectiveActionTaken=" + CorrectiveActionTaken + "&Hospitalized=" + Hospitalized + "&startDate=" + startDate + "&endDate=" + endDate
                   });
               };
               //END GET A LIST ACCIDENT HELP INFOMATION

               //BEGIN DELETE ACCIDENT HELP INFOMATION
               list.DeleteAccidentsRecords = function (id) {
                   return $http({
                       method: "POST",
                       url: "/api/HRDAccidentRecords/DeleteAccidentRecords?ts=" + new Date().getTime(),
                       data: id,
                       contentType: "application/json"
                   });
               };
               //END DELETE ACCIDENT HELP INFOMATION

               return list;
           }]);

