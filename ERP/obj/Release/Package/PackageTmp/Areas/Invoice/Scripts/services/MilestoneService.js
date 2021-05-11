/// <reference path="../../../../Scripts/libs/angular/angular.js" />


angular.module("ERPApp.Services")
    .service("MilestoneService", [
        "$http",
        function ($http) {
            var api = {};

            api.RetrieveProjectMilestones = function (projectId) {
                return $http({
                    method: 'GET',
                    url: "/api/Milestone/RetrieveProjectMilestones?ts=" + new Date().getTime() + "&projectId=" + projectId
                });
            };

            api.DeleteProjectMilestone = function (milestoneId) {
                return $http({
                    method: 'GET',
                    url: "/api/Milestone/DeleteProjectMilestone?ts=" + new Date().getTime() + "&milestoneId=" + milestoneId
                });
            };

            api.CreateUpdateMilestone = function (milestoneData, milestoneId, timeZone) {
                return $http({
                    method: 'POST',
                    url: "/api/Milestone/CreateUpdateMilestone?ts=" + new Date().getTime() + "&timezone=" + timeZone+"&milestoneId="+milestoneId,
                    data: milestoneData,
                    contentType: 'application/json; charset=utf-8'
                });
            };

            return api;
        }
    ]);