/// <reference path="../../../../Scripts/libs/angular/angular.js" />

angular.module("ERPApp.Services")
    .service("ConversationService", [
        "$http",
        function ($http) {
            var list = {};

            list.CreateUpdateConversation = function (data, timeZone) {

                return $http({
                    method: "POST",
                    url: "/api/Conversation/CreateUpdateConversation?ts=" + new Date().getTime() + "&timezone=" + timeZone,
                    data: data,
                    contentType: "application/json"
                });
            };

            list.UploadDocument = function (document) {
                return $http({
                    method: "POST",
                    url: "/api/Conversation/UploadDocument?ts=" + new Date().getTime(),
                    data: document,
                    contentType: "application/json"
                });
            };

            list.RetrieveConversations = function (clientId, refId, timeZone, conversationType) {
                return $http({
                    method: "GET",
                    url: "/api/Conversation/RetrieveConversations?clientId=" + clientId + "&timezone=" + timeZone + "&conversationType=" + conversationType+"&refId="+refId,
                    contentType: "application/json"
                });
            };

            list.DeleteConversation = function (id) {
                return $http({
                    method: "GET",
                    url: "/api/Conversation/DeleteConversation?ConversationId=" + id,
                    contentType: "application/json"
                });
            };

            list.GetClientList = function () {
                return $http({
                    method: "GET",
                    url: "/api/Conversation/GetClientList?ts=" + new Date().getTime(),
                    contentType: "application/json"
                });
            };

            list.GetClient = function (clientId) {
                return $http({
                    method: 'GET',
                    url: "/api/Conversation/GetClient?ts=" + new Date().getTime() + "&cId=" + clientId
                });
            };

            list.GetCurrencyList = function () {
                return $http({
                    method: 'GET',
                    url: "/api/Conversation/GetCurrencyList?ts=" + new Date().getTime()
                });
            };

            return list;
        }
    ]);