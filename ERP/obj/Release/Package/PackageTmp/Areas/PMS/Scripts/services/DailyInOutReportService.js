/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
    .service("DailyInOutReportService", [
        "$http",
        function ($http) {

            var list = {};

            /*get project list*/
            list.GetDailyInOutReprotList = function (timezone, date) {
                return $http({
                    method: "GET",
                    url: "/api/DailyInOutReport/GetDailyInOutReprotData?ts=" + new Date().getTime() + "&timezone=" + timezone+"&date="+date 
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

            return list;
        }
    ]);