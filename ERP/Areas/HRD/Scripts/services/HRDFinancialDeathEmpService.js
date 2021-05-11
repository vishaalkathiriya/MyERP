/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
          .service("FinancialDeathEmpService", [
          "$http",
          function ($http) {

              var list = {};

              //BEGIN ADD AND UPDATE SOCIAL WELFARE EXPENSE INFORMATION
              list.CreateUpdateFinancialDeathEmp = function (data, _timezone) {
                  return $http({
                      method: "POST",
                      url: "/api/HRDFinancialDeathEmp/SaveFinancialDeathEmp?ts=" + new Date().getTime() + "&timezone=" + _timezone,
                      data: data,
                      contentType: "application/json"
                  });
              };
              //END ADD AND UPDATE SOCIAL WELFARE EXPENSE INFORMATION


              //BEGIN GET A LIST SOCIAL WELFARE EXPENSE INFOMATION 
              list.GetFinancialDeathEmpList = function (timezone, page, count, orderby, Ecode, EmployeeName, DateOfDeath, Amount, ChequeNumber,ChequeIssueDate, ReceiveBy, Relation, FamilyBackgroundDetail, startDate, endDate) {
                  return $http({
                      method: "GET",
                      url: "/api/HRDFinancialDeathEmp/GetFinancialDeathEmpList?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&Ecode=" + Ecode + "&EmployeeName=" + EmployeeName + "&DateOfDeath=" + DateOfDeath + "&Amount=" + Amount + "&ChequeNumber=" + ChequeNumber + "&ChequeIssueDate=" + ChequeIssueDate + "&ReceiveBy=" + ReceiveBy + "&Relation=" + Relation + "&FamilyBackgroundDetail=" + FamilyBackgroundDetail + "&startDate=" + startDate + "&endDate=" + endDate
                  });
              };
              //END GET A LIST SOCIAL WELFARE EXPENSE INFOMATION


              //BEGIN DELETE SOCIAL WELFARE EXPENSE INFOMATION
              list.DeleteFinancialDeathEmp = function (id) {
                  return $http({
                      method: "POST",
                      url: "/api/HRDFinancialDeathEmp/DeleteFinancialDeathEmp?ts=" + new Date().getTime(),
                      data: id,
                      contentType: "application/json"
                  });
              };
              //END DELETE SOCIAL WELFARE EXPENSE HELP INFOMATION

              return list;
          }]);