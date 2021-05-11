/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
          .service("PPEIssueRegisterService", [
          "$http",
          function ($http) {

              var list = {};

              //BEGIN ADD AND UPDATE PPE ISSUE REGISTER INFORMATION
              list.CreateUpdatePPEIssueRegister = function (data, _timezone) {
                  return $http({
                      method: "POST",
                      url: "/api/HRDPPEIssueRegister/SavePPEIssueRegister?ts=" + new Date().getTime() + "&timezone=" + _timezone,
                      data: data,
                      contentType: "application/json"
                  });
              };
              //END ADD AND UPDATEPPE ISSUE REGISTER  INFORMATION


              //BEGIN GET A LIST PPE ISSUE REGISTER  INFOMATION 
              list.GetPPEIssueRegister = function (timezone, page, count, orderby, NameOfIssuer, NameOfRecievr, TypeOfPPE, Quanity, Department, ManagerName, Price, Remarks, startDate, endDate) {
                  return $http({
                      method: "GET",
                      url: "/api/HRDPPEIssueRegister/GetPPEIssueRegister?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&NameOfIssuer=" + NameOfIssuer + "&NameOfRecievrr=" + NameOfRecievr + "&TypeOfPPE=" + TypeOfPPE + "&Quanity=" + Quanity + "&Department=" + Department + "&ManagerName=" + ManagerName + "&Price=" + Price + "&Remarks=" + Remarks + "&startDate=" + startDate + "&endDate=" + endDate
                  });
              };
              //END GET A LIST PPE ISSUE REGISTER INFOMATION


              //BEGIN DELETE PPE ISSUE REGISTER  INFOMATION
              list.DeletePPEIssueRegister = function (id) {
                  return $http({
                      method: "POST",
                      url: "/api/HRDPPEIssueRegister/DeletePPEIssueRegister?ts=" + new Date().getTime(),
                      data: id,
                      contentType: "application/json"
                  });
              };
              //END DELETE PPE ISSUE REGISTER  INFOMATION

              return list;
          }]);