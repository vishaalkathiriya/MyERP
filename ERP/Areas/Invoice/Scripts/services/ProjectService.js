/// <reference path="../../../../Scripts/libs/angular/angular.js" />


angular.module("ERPApp.Services")
    .service("ProjectService", [
        "$http",
        function ($http) {
            var api = {};

            api.RetrieveProject = function (projectId) {
                return $http({
                    method: 'GET',
                    url: "/api/Project/RetrieveProject?ts=" + new Date().getTime() + "&projectId=" + projectId
                });
            };

            api.RetrieveProjectStatus = function () {
                return $http({
                    method: 'GET',
                    url: "/api/Project/RetrieveProjectStatus?ts=" + new Date().getTime()
                });
            };

            api.RetrieveProjectTypes = function () {
                return $http({
                    method: 'GET',
                    url: "/api/Project/RetrieveProjectTypes?ts=" + new Date().getTime()
                });
            };

            api.CreateUpdateProject = function (projectData, timeZone) {
                var _StartDateArray = projectData.StartDate.split("-")
                var _StartDate = new Date(_StartDateArray[2], _StartDateArray[1] - 1, _StartDateArray[0], moment().hours(), moment().minute(), moment().second());
                var _EndDateArray = projectData.EndDate.split("-")
                var _EndDate = new Date(_EndDateArray[2], _EndDateArray[1] - 1, _EndDateArray[0], moment().hours(), moment().minute(), moment().second());
                var _data = {
                    FKInquiryId: projectData.FKInquiryId,
                    ProjectTitle: projectData.ProjectTitle,
                    ProjectStatus: projectData.ProjectStatus,
                    ProjectType: projectData.ProjectType,
                    Price: projectData.Price,
                    Currency: projectData.Currency,
                    TotalHours: projectData.TotalHours,
                    StartDate: _StartDate,
                    EndDate: _EndDate,
                    IsActive: projectData.IsActive,
                    IsDeleted: false,
                    Remarks: projectData.Remarks,
                };
                return $http({
                    method: 'POST',
                    url: "/api/Project/CreateUpdateProject?ts=" + new Date().getTime() + "&timezone=" + timeZone,
                    data: _data,
                    contentType: 'application/json; charset=utf-8'
                });
            };

            api.RetrieveConfirmedProjects = function (clientId) {
                return $http({
                    method: 'GET',
                    url: "/api/Project/RetrieveConfirmedProjects?ts=" + new Date().getTime() + "&clientId=" + clientId,
                    contentType: 'application/json; charset=utf-8'
                });
            };
            return api;
        }
    ]);