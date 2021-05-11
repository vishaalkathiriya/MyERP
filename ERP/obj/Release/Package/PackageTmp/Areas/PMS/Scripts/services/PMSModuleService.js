/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
    .service("PMSModuleService", [
        "$http",
        function ($http) {

            var list = {};

            /*get module list*/
            list.GetModuleList = function (timezone, projectId) {
                return  $http({
                    method: "GET",
                    url: "/api/PMSModule/GetModuleList?ts=" + new Date().getTime() + "&timezone=" + timezone + "&projectId=" + projectId
                });
            };

            /*get module todo list*/
            list.GetModuleTodoList = function (timezone, moduleId) {
                return $http({
                    method: "GET",
                    url: "/api/PMSModule/GetModuleTodoList?ts=" + new Date().getTime() + "&timezone=" + timezone + "&moduleId=" + moduleId
                });
            };

            /*update module*/
            list.UpdateModule = function (moduleName, moduleId) {
                return $http({
                    method: "GET",
                    url: "/api/PMSModule/UpdateModule?ts=" + new Date().getTime() + "&moduleName=" + moduleName + "&moduleId=" + moduleId
                });
            };

            /*do sorting for module*/
            list.DoSortingForModule = function (sortOrder) {
                return $http({
                    method: "GET",
                    url: "/api/PMSModule/DoSortingForModule?ts=" + new Date().getTime() + "&sortOrder=" + sortOrder
                });
            };
            /*do sorting for todo*/
            list.DoSortingForTodo = function (sortOrder) {
                return $http({
                    method: "GET",
                    url: "/api/PMSModule/DoSortingForTodo?ts=" + new Date().getTime() + "&sortOrder=" + sortOrder
                });
            };

            /*add pms module*/
            list.CreateUpdateModule = function (data) {
                return $http({
                    method: "POST",
                    url: "/api/PMSModule/CreateUpdateModule?ts=" + new Date().getTime(),
                    data: data,
                    contentType: "application/json"
                });
            };

            /*delete module*/
            list.DeleteModule = function (moduleId) {
                return $http({
                    method: "GET",
                    url: "/api/PMSModule/DeleteModule?ts=" + new Date().getTime() + "&moduleId=" + moduleId
                });
            };

            /*save todo item*/
            list.SaveTodoItem = function (data) {
                return $http({
                    method: "POST",
                    url: "/api/PMSModule/SaveTodoItem?ts=" + new Date().getTime(),
                    data: data,
                    contentType: "application/json"
                });
            };

            /*hold todo item*/
            list.HoldTodoItem = function (todoId) {
                return $http({
                    method: "GET",
                    url: "/api/PMSModule/HoldTodoItem?ts=" + new Date().getTime() + "&todoId=" + todoId
                });
            };

            /*delete todo item*/
            list.DeleteTodoItem = function (todoId) {
                return $http({
                    method: "GET",
                    url: "/api/PMSModule/DeleteTodoItem?ts=" + new Date().getTime() + "&todoId=" + todoId
                });
            };

            /*finish todo item*/
            list.FinishTodoItem = function (todoId) {
                return $http({
                    method: "GET",
                    url: "/api/PMSModule/FinishTodoItem?ts=" + new Date().getTime() + "&todoId=" + todoId
                });
            };

            /*get assigned user*/
            list.GetAssignedUserList = function (projectId) {
                return $http({
                    method: "GET",
                    url: "/api/PMSModule/GetAssignedUserList?ts=" + new Date().getTime() + "&projectId=" + projectId
                });
            };
            
      
            /*Assign user*/
            list.AssignUser = function (filteredDate, userId, projectId, todoId, iscanfinish) {
                return $http({
                    method: "GET",
                    url: "/api/PMSModule/AssignUser?ts=" + new Date().getTime() + "&filteredDate=" + filteredDate + "&userId=" + userId + "&projectId=" + projectId + "&todoId=" + todoId + "&iscanfinish=" + iscanfinish
                });
            };
            


            /*Active todo item from unhold, finished and archived list*/
            list.ActiveTodoItem = function (todoId, isFor) {
                
                return $http({
                    method: "GET",
                    url: "/api/PMSModule/ActiveTodoItem?ts=" + new Date().getTime() + "&todoId=" + todoId + "&for=" + isFor
                });
            };

            /*GET project name*/
            list.GetProjectName = function (projectId) {
                return $http({
                    method: "GET",
                    url: "/api/PMSModule/GetProjectName?ts=" + new Date().getTime() + "&projectId=" + projectId
                });
            };

            /*GET module type list*/
            list.GetModuleTypeList = function () {
                return $http({
                    method: "GET",
                    url: "/api/PMSModule/GetModuleTypeList?ts=" + new Date().getTime()
                });
            };

            
            /*get all project name  */
            list.getAllProjectName = function ()
            {
                return $http({
                    method: "GET",
                    url: "/api/PMSModule/getAllProjectName?ts=" + new Date().getTime()
                });
            }

            return list;
        }
    ]);