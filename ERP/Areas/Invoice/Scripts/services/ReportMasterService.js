/// <reference path="../../../../Scripts/libs/angular/angular.js" />

angular.module("ERPApp.Services")
    .service("ReportMasterService", [
        "$http",
        function ($http) {
            var app = {};

            app.GetClientList = function () {
                return $http({
                    method: "GET",
                    url: "/api/Report/GetClientList?ts=" + new Date().getTime(),
                    contentType: "application/json"
                });
            };

            return app;
        }
    ]);