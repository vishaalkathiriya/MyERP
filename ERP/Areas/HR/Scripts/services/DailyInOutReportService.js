/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
    .service("DailyInOutReportService", [
        "$http",
        function ($http) {

            var list = {};

            /*get project list*/
            list.GetDailyInOutReprotList = function (timezone, date, groupName) {
                if (!groupName)
                    groupName = null;
                return $http({
                    method: "GET",
                    url: "/api/DailyInOutReport/GetDailyInOutReprotData?ts=" + new Date().getTime() + "&timezone=" + timezone + "&date=" + date + "&groupName=" + groupName
                });
            };

            /*save report data*/
            list.SaveData = function (data) {
                return $http({
                    method: "POST",
                    url: "/api/DailyInOutReport/SaveData?ts=" + new Date().getTime(),
                    data: data,
                    contentType: 'application/json; charset=utf-8'
                });
            };

            /*get InOut Detail entries Of Employee for selected date */
            list.GetInOutDetailsOfEmployee = function (timezone, date, EmpId) {
                return $http({
                    method: "GET",
                    url: "/api/DailyInOutReport/GetInOutDetailsOfEmployee?ts=" + new Date().getTime() + "&timezone=" + timezone + "&date=" + date + "&EmpId=" + EmpId
                });
            };

            /*update InOut Detial entry Of Employee for selected date*/
            list.UpdateInOutDetailsOfEmployee = function (data) {
                return $http({
                    method: "POST",
                    url: "/api/DailyInOutReport/UpdateInOutDetailsOfEmployee?ts=" + new Date().getTime(),
                    data: data,
                    contentType: 'application/json; charset=utf-8'
                });
            };


            /*Delete InOut Detial entry Of Employee for selected date*/
            list.DeleteEmployeeInOutDelete = function (data) {
                return $http({
                    method: "POST",
                    url: "/api/DailyInOutReport/DeleteEmployeeInOutDelete?ts=" + new Date().getTime(),
                    data: data,
                    contentType: 'application/json; charset=utf-8'
                });
            };

            // Pass data to popup's controller
            var data;
            list.SetData = function (empData) {
                data = empData;
            };

            list.GetData = function () {
                return data;
            };


            return list;
        }
    ]);