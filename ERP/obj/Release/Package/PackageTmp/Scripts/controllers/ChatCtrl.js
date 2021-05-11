/// <reference path="../libs/angular/angular.min.js" />


(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("ChatCtrl", [
            "$scope",
            ChatCtrl
        ]);

    function ChatCtrl($scope) {
        var chat = $.connection.chatHub;
        $scope.userList = [{ name: "nikhilrr" }, { name: "admin"}];
        $scope.chatList = [];

        chat.client.broadcastMessage = function (username, name, message) {
            $scope.chatList.push({ username: username, name: name, message: message });
            $scope.$apply();
        }

        $.connection.hub.start().done(function () {
            chat.server.Connect(window.erpuid, window.erpuname);
        });

        $scope.chattxt = "";
        $scope.sendChat = function () {
            chat.server.send("nikhil", $scope.chattxt);
        };
    }
})();