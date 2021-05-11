/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
          .service("SocialWelExpService", [
          "$http",
          function ($http) {

              var list = {};

              //BEGIN ADD AND UPDATE SOCIAL WELFARE EXPENSE INFORMATION
              list.CreateUpdateSocialWelExp = function (data , _timezone) {
                  return $http({
                      method: "POST",
                      url: "/api/HRDSocialWebExp/SaveSocialWebExp?ts=" + new Date().getTime() + "&timezone=" + _timezone,
                      data: data,
                      contentType: "application/json"
                  });
              };
              //END ADD AND UPDATE SOCIAL WELFARE EXPENSE INFORMATION

              //BEGIN GET A LIST SOCIAL WELFARE EXPENSE INFOMATION 
              list.GetSocialWelExpList = function (timezone, page, count, orderby, ProgrammeName, Venue, Date1, Time, ExpenseAmount, GuestName, startDate, endDate) {
                  return $http({
                      method: "GET",
                      url: "/api/HRDSocialWebExp/GetSocialWebExpList?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&ProgrammeName=" + ProgrammeName + "&Venue=" + Venue + "&Date=" + Date1 + "&Time=" + Time + "&ExpenseAmount=" + ExpenseAmount + "&GuestName=" + GuestName + "&startDate=" + startDate + "&endDate=" + endDate
                  });
              };
              //END GET A LIST SOCIAL WELFARE EXPENSE INFOMATION

              //BEGIN DELETE SOCIAL WELFARE EXPENSE INFOMATION
              list.DeleteSocialWelExp = function (id) {
                  return $http({
                      method: "POST",
                      url: "/api/HRDSocialWebExp/DeleteSocialWebExp?ts=" + new Date().getTime(),
                      data: id,
                      contentType: "application/json"
                  });
              };
              //END DELETE SOCIAL WELFARE EXPENSE HELP INFOMATION

              return list;

          }]);