/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
    .service("AdminMasterService", [
        "$http",
        function ($http) {

            var list = {};

            /*check for page permission after login*/
            list.HasPermission = function (ctrl) {
                return $http({
                    method: "GET",
                    url: "/api/AdminMaster/HasPermission?ts=" + new Date().getTime()+"&ctrl="+ctrl
                });
            };

            /*change password*/
            list.ChangePassword = function (newPassword) {
                return $http({
                    method: "GET",
                    url: "/api/AdminMaster/ChangePassword?ts=" + new Date().getTime() + "&newPassword=" + newPassword
                });
            };

            /*switch view*/
            list.SwitchMenuView = function () {
                return $http({
                    method: "GET",
                    url: "/api/AdminMaster/SwitchMenuView?ts=" + new Date().getTime()
                });
            };

            list.activeModuleTodoList = function ()
            {
                return $http({
                    method: "GET",
                    url: "/api/AdminMaster/activeModuleTodoList?ts=" + new Date().getTime()
                });
            }

        

            return list;
        }
    ]);