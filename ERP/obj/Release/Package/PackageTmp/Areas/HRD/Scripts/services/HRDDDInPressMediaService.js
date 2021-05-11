/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
          .service("DDPressMediaService", [
          "$http",
          function ($http) {
              var list = {};
              list.CreateUpdateDDPressMedia = function (data, _timezone) {
                  return $http({
                      method: "POST",
                      url: "/api/HRDDDInPressMedia/SaveDDPressMedia?ts=" + new Date().getTime() + "&timezone=" + _timezone,
                      data: data,
                      contentType: "application/json"
                  });
              };

              list.DeleteDDPressMedia = function (id) {
                  return $http({
                      method: "POST",
                      url: "/api/HRDDDInPressMedia/DeleteDDPressMedia?ts=" + new Date().getTime(),
                      data: id,
                      contentType: "application/json"
                  });
              };

              list.GetDDPressMediaList = function (timezone, page, count, orderby, NameOfNewspaper, EventName, Website,startDate, endDate) {
                  return $http({
                      method: "GET",
                      url: "/api/HRDDDInPressMedia/GetDDPressMediaList?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&NameOfNewspaper=" + NameOfNewspaper + "&EventName=" + EventName + "&Website=" + Website + "&startDate=" + startDate + "&endDate=" + endDate
                  });
              };

              return list;

          }]);