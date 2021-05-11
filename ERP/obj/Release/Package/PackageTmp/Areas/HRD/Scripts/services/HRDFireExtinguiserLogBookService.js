/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
.service("FireExtinguiserService", [
          "$http",
          function ($http) {
              var list = {};

              //BEGIN ADD AND UPDATE FIRE EXTINGUISER  LOGBOOK 
              list.CreateFireExtinguiserLogBook = function (data, _timezone) {
                  return $http({
                      method: "POST",
                      url: "/api/HRDFireExtinguiserLogBook/SaveFireExtinuiserLogBook?ts=" + new Date().getTime() + "&timezone=" + _timezone,
                      data: data,
                      contentType: "application/json"
                  });
              };
              //END ADD UPDATE FIRE EXTINGUISER  LOGBOOK 

              //BEGIN GET FIRE EXTINGUISER  LOGBOOK 
              list.RetFireExtinguiser = function (timezone, page, count, orderby, TypeOfFireExtinguiser, Capacity, Location, DateOfInspection, UsedOfFireExtinguiser, DateOfRefilling, DueDateForNextRefilling, Reason, Remark, startDate, endDate) {
                  return $http({
                      method: "GET",
                      url: "/api/HRDFireExtinguiserLogBook/GetFireExtinguiserLogBook?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&TypeOfFireExtinguiser=" + TypeOfFireExtinguiser + "&Capacity=" + Capacity + "&Location=" + Location + "&DateOfInspection=" + DateOfInspection + "&UsedOfFireExtinguiser=" + UsedOfFireExtinguiser + "&DateOfRefilling=" + DateOfRefilling + "&DueDateForNextRefilling=" + DueDateForNextRefilling + "&Reason=" + Reason + "&Remark=" + Remark + "&startDate=" + startDate + "&endDate=" + endDate
                  });
              };
              //END GET FIRE EXTINGUISER  LOGBOOK 

              //BEGIN DELETEFIRE EXTINGUISER  LOGBOOK 
              list.DeleteFireExtinguiser = function (id) {
                  return $http({
                      method: "POST",
                      url: "/api/HRDFireExtinguiserLogBook/DeleteFireExtinuiserLogBook?ts=" + new Date().getTime(),
                      data: id,
                      contentType: "application/json"
                  });
              };
              //END DELETE FIRE EXTINGUISER  LOGBOOK 
              return list;
          }]);