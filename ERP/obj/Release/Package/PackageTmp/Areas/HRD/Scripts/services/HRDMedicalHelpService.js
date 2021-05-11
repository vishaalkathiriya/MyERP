/// <reference path="../libs/angular/angular.js" />
angular.module("ERPApp.Services")
          .service("MedicalHelpService", [
          "$http",
           function ($http) {
                 var list = {};

            //BEGIN ADD AND UPDATE MEDICAL HELP INFORMATION
                 list.CreateUpdateMedicalHelp = function (data, _timezone) {
                return $http({
                    method: "POST",
                    url: "/api/HRDMedicalHelp/SaveMedicalHelp?ts=" + new Date().getTime() + "&timezone=" + _timezone,
                    data: data,
                    contentType: "application/json"
                });
            };
            //END ADD AND UPDATE MEDICAL HELP INFORMATION

            //BEGIN GET A LIST MEDICAL HELP INFOMATION 
                 list.GetMedicalHelpList = function (timezone, page, count, orderby, ECode, EmployeeName, PatientName, Relation, HospitalName, ChequeIssueDate, ChequeNumber, ReceiverName, MobileNumber, Amount, IsPatelSocialGroup, startDate, endDate) {
                     return $http({
                    method: "GET",
                    url: "/api/HRDMedicalHelp/GetMedicalHelpList?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&ECode=" + ECode + "&EmployeeName=" + EmployeeName + "&PatientName=" + PatientName + "&Relation=" + Relation + "&HospitalName=" + HospitalName + "&ChequeIssueDate=" + ChequeIssueDate + "&ChequeNumber=" + ChequeNumber + "&ReceiverName=" + ReceiverName + "&MobileNumber=" + MobileNumber + "&Amount=" + Amount + "&IsPatelSocialGroup=" + IsPatelSocialGroup + "&startDate=" + startDate + "&endDate=" + endDate
                });
            };
            //END GET A LIST MEDICAL HELP INFOMATION

            //BEGIN DELETE MEDICAL HELP INFOMATION
            list.DeleteMedicalHelp = function (id) {
                return $http({
                    method: "POST",
                    url: "/api/HRDMedicalHelp/DeleteMedicalHelp?ts=" + new Date().getTime(),
                    data: id,
                    contentType: "application/json"
                });
            };
            //END DELETE MEDICAL HELP INFOMATION

            return list;
        }]);

