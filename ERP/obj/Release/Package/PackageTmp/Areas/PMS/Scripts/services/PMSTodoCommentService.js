/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
    .service("PMSTodoCommentService", [
        "$http",
        function ($http) {

            var list = {};

            /*get project id*/
            list.GetProjectId = function (todoId) {
                return $http({
                    method: "GET",
                    url: "/api/PMSTodoComment/GetProjectId?ts=" + new Date().getTime() + "&todoId=" + todoId
                });
            };

            /* Get Todo Staus for Active Todo */
            list.GetTodoStatus = function (todoId) {
                return $http({
                    method: "GET",
                    url: "/api/PMSTodoComment/GetTodoStatus?ts=" + new Date().getTime() + "&todoId=" + todoId
                });
            }

            /* Get Status List for binding when adding new comment */
            list.GetStatusList = function () {
                return $http({
                    method: "GET",
                    url: "/api/PMSTodoComment/GetStatusList?ts=" + new Date().getTime()
                });
            }

            /*get comment list*/
            list.GetCommentList = function (timezone, todoId) {
                return $http({
                    method: "GET",
                    url: "/api/PMSTodoComment/GetCommentList?ts=" + new Date().getTime() + "&timezone=" + timezone + "&todoId=" + todoId
                });
            };

            /*get assigned user list*/
            list.GetAssignedUserList = function (todoId) {
                return $http({
                    method: "GET",
                    url: "/api/PMSTodoComment/GetAssignedUserList?ts=" + new Date().getTime() + "&todoId=" + todoId
                });

            };

            /*save comment*/
            list.SaveComment = function (data) {
                return $http({
                    method: "POST",
                    url: "/api/PMSTodoComment/SaveComment?ts=" + new Date().getTime() + "&status=" + data.Status,
                    data: data,
                    contentType: "application/json"
                });
            };


            /* delete comment */
            list.DeleteModuleTodoComments = function (CommentId) {
                return $http({
                    method: "GET",
                    url: "/api/PMSTodoComment/DeleteModuleTodoComment?ts=" + new Date().getTime() + "&CommentId=" + CommentId
                });
            };



            /*save uploaded file*/
            list.SaveUploadedFile = function (data, projectId) {
                return $http({
                    method: "POST",
                    url: "/api/PMSTodoComment/SaveUploadedFile?ts=" + new Date().getTime() + "&projectId=" + projectId,
                    data: data,
                    contentType: "application/json"
                });
            };

            /*send update mail*/
            list.SendMail = function (commentId, bcc) {
                return $http({
                    method: "GET",
                    url: "/api/PMSTodoComment/SendMail?ts=" + new Date().getTime() + "&commentId=" + commentId + "&bcc=" + bcc
                });
            };


            /*USED FOR SET MODEL NAME INIT TIME */
            list.setModuleName = function (todoId) {
                return $http({
                    method: "GET",
                    url: "/api/PMSTodoComment/setModuleName?ts=" + new Date().getTime() + "&todoId=" + todoId
                });

            }

            return list;
        }
    ]);