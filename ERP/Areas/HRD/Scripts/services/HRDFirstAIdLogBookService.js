/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
.service("FirstAIdLogBookService", [
          "$http",
          function($http)
          {
              var list = {};

              //BEGIN ADD AND UPDATE FIRST AID LOGBOOK 
              list.CreateUpdateFirstAIdLogBook = function (data, _timeZone) {
                  return $http({
                      method: "POST",
                      url: "/api/HRDFirstAIdLogBook/SaveFirstAIdLogBook?ts=" + new Date().getTime() + "&timezone=" + _timeZone,
                      data: data,
                      contentType: "application/json"
                  });
              };
              //END ADD UPDATE FIRST AID LOGBOOK

              //BEGIN GET FIRST AID LOGBOOK
              list.GetFirstAIdLogBook = function (timezone, page, count, orderby, NameOfIssuer, NameOfReceiver, NameOfFirstAIdItems, DateOfIssue, Quanity, Size, ManagerName, LocationOfFirstAIdBox, Price, ExpiryDate, Remarks, startDate, endDate) {
                  return $http({
                      method: "GET",
                      url: "/api/HRDFirstAIdLogBook/GetFirstAIdLogBook?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&NameOfIssuer=" + NameOfIssuer + "&NameOfReceiver=" + NameOfReceiver + "&NameOfFirstAIdItems=" + NameOfFirstAIdItems + "&DateOfIssue=" + DateOfIssue + "&Quanity=" + Quanity + "&Size=" + Size + "&ManagerName=" + ManagerName + "&LocationOfFirstAIdBox=" + LocationOfFirstAIdBox + "&Price=" + Price + "&ExpiryDate=" + ExpiryDate + "&Remarks=" + Remarks + "&startDate=" + startDate + "&endDate=" + endDate
                  });
              };
              //END GET FIRST AID LOGBOOK

              //BEGIN DELETE FIRST AID LOGBOOK
              list.DeleteFirstAIdLogBook = function (id) {
                  return $http({
                      method: "POST",
                      url: "/api/HRDFirstAIdLogBook/DeleteFirstAIdLogBook?ts=" + new Date().getTime(),
                      data: id,
                      contentType: "application/json"
                  });
              };
              //END DELETE AID LOG BOOK
              return list;
          }]);