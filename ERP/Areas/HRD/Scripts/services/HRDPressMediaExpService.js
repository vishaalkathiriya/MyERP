/// <reference path="../libs/angular/angular.js" />

//PressMediaExpService

angular.module("ERPApp.Services")
          .service("PressMediaExpService", [
          "$http",
          function ($http) {

              var list = {};

              //BEGIN ADD AND UPDATE SOCIAL WELFARE EXPENSE INFORMATION
              list.CreateUpdatePressMediaExp = function (data, _timezone) {

                  return $http({
                      method: "POST",
                      url: "/api/HRDPressMediaExp/SavePressMediaExp?ts=" + new Date().getTime() + "&timezone=" + _timezone,
                      data: data,
                      contentType: "application/json"
                  });
              };
              //END ADD AND UPDATE SOCIAL WELFARE EXPENSE INFORMATION

              //BEGIN GET A LIST SOCIAL WELFARE EXPENSE INFOMATION 
              list.GetPressMediaExpList = function (timezone, page, count, orderby, NameOfPressMedia, RepresentativeName, Date1, MobileNumber, Amount, ApprovedBy, Occasion, startDate, endDate) {
                  return $http({
                      method: "GET",
                      url: "/api/HRDPressMediaExp/GetPressmediaExpList?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&NameOfPressMedia=" + NameOfPressMedia + "&RepresentativeName=" + RepresentativeName + "&Date1=" + Date1 + "&MobileNumber=" + MobileNumber + "&Amount=" + Amount + "&ApprovedBy=" + ApprovedBy + "&Occasion=" + Occasion + "&startDate=" + startDate + "&endDate=" + endDate
                  });
              };
              //END GET A LIST SOCIAL WELFARE EXPENSE INFOMATION

              //BEGIN DELETE SOCIAL WELFARE EXPENSE INFOMATION
              list.DeletePressMediaExp = function (id) {
                  return $http({
                      method: "POST",
                      url: "/api/HRDPressMediaExp/DeletePressMediaExp?ts=" + new Date().getTime(),
                      data: id,
                      contentType: "application/json"
                  });
              };
              //END DELETE SOCIAL WELFARE EXPENSE HELP INFOMATION

              return list;

          }]);