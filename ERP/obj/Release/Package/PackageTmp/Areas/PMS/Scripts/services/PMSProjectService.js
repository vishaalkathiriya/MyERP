/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
    .service("PMSProjectService", [
        "$http",
        function ($http) {

            var list = {};

            /*get status list*/
            list.GetStatusList = function () {
                return  $http({
                    method: "GET",
                    url: "./../api/PMSProject/GetStatusList?ts=" + new Date().getTime()
                });
            };

            /*GET TECHNOLOGIES  LIST */
            list.GetTechnologies = function () {
                return $http({
                    method: "GET",
                    url: "./../api/PMSProject/GetTechnologies?ts=" + new Date().getTime()
                });
            };

            /*  GET TECHNOLOGY GROUP LIST */
            list.GetTechnologiesGroup = function () {
                return $http({
                    method: "GET",
                    url: "./../api/PMSProject/GetTechnologiesGroup?ts=" + new Date().getTime
                });
            };
            

            /*get  project type */
            list.GetProjectTypeList = function () {
                return $http({
                    method: "GET",
                    url: "./../api/PMSProject/GetProjectTypeList?ts=" + new Date().getTime()
                });
            };

            /*get team lead list*/
            list.GetTLList = function () {
                return $http({
                    method: "GET",
                    url: "./../api/PMSProject/GetTLList?ts=" + new Date().getTime()
                });
            };

            /*get user list*/
            list.GetUserList = function (leadId) {
                return $http({
                    method: "GET",
                    url: "./../api/PMSProject/GetUserList?ts=" + new Date().getTime() + "&leadId=" + leadId
                });
            };

            /*get project list*/
            list.GetProjectList = function (timezone, filterId, filterTechnology, filterProjectType, filterUserList) {
                return $http({
                    method: "GET",
                    url: "/api/PMSProject/GetProjectList?ts=" + new Date().getTime() + "&timezone=" + timezone + "&filterData=" + filterId + "&technology=" + filterTechnology + "&projectType=" + filterProjectType + "&filterUserList=" + filterUserList
                });
            };

            /*add edit project*/
            list.CreateUpdateProject = function (data) {
                return $http({
                    method: "POST",
                    url: "/api/PMSProject/SaveProject?ts=" + new Date().getTime(),
                    data: data,
                    contentType: "application/json"
                });
            };

            list.CreateProjectUsers = function (data) {
                return $http({
                    method: "POST",
                    url: "/api/PMSProject/SaveProjectUsers?ts=" + new Date().getTime(),
                    data: data,
                    contentType: "application/json"
                });
            };

            /*get user list*/
            list.SendMail = function (projectId, mode, projData) {
                return $http({
                    method: "POST",
                    url: "/api/PMSProject/SendMail?ts=" + new Date().getTime() + "&projectId=" + projectId + "&mode=" + mode,
                    data: projData,
                    contentType: "application/json"
                });
            };

          

            return list;
        }
    ]);