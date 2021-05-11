/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
    .service("EmpDailyInOutService", [
        "$http",
        function ($http) {
            var empDailyInOut = {};

            empDailyInOut.FetchDailyInOut = function () {
                return $http({
                    method: 'GET',
                    url: "/api/EmpDailyInOut/GetDailyInOut?ts=" + new Date().getTime() 
                });
            };

            empDailyInOut.checkLoginStatus = function () {
                return $http({
                    method: 'GET',
                    url: "/api/EmpDailyInOut/checkLoginStatus?ts=" + new Date().getTime()
                });
            };

            empDailyInOut.SaveInOut = function (comment,inOutType) {
                var data = {
                    Comment: comment,
                    InOutType: inOutType
                };
                return $http({
                    method: "POST",
                    url: "/api/EmpDailyInOut/SaveInOut?ts=" + new Date().getTime(),
                    data: data,
                    contentType: 'application/json; charset=utf-8'
                });
            };

            empDailyInOut.getWorkHoursInfo = function () {
                return $http({
                    method: "GET",
                    url: "/api/EmpDailyInOut/GetInOutInformation?ts=" + new Date().getTime()
                });
            }


            return empDailyInOut;
        }
    ]);