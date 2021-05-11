/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
    .service("RoleService", [
        "$http",
        function ($http) {

            var rolelist = {};

            /*get role list*/
            rolelist.GetRoleList = function (timezone, page, count, orderby, filter) {
                return  $http({
                    method: "GET",
                    url: "/api/Role/GetRoleList?ts=" + new Date().getTime()+"&timezone=" + timezone + "&page=" + page + "&count=" + count +"&orderby="+orderby+"&filter="+filter
                });
            };

            /*add edit role*/
            rolelist.CreateUpdateRole = function (data) {
                return $http({
                    method: "POST",
                    url: "/api/Role/SaveRole?ts=" + new Date().getTime(),
                    data: data,
                    contentType: "application/json"
                });
            };

            /*delete role*/
            rolelist.DeleteRole = function (id) {
                return $http({
                    method: "POST",
                    url: "/api/Role/DeleteRole?ts=" + new Date().getTime(),
                    data: id,
                    contentType: "application/json"
                });
            };
            
            /*active inactive role*/
            rolelist.ChangeStatus = function (id, status) {
                return $http({
                    method: "POST",
                    url: "/api/Role/ChangeStatus?ts=" + new Date().getTime(),
                    data: JSON.stringify({ RolesId: id, IsActive:status }),
                    contentType: "application/json"
                });
            };


            /*get module sub module list*/
            rolelist.GetModuleSubModuleList = function (roleId) {
                return $http({
                    method: "GET",
                    url: "/api/Role/GetModuleSubModuleList?ts=" + new Date().getTime() + "&roleId=" + roleId
                });
            };

            /*add edit access permission*/
            rolelist.SaveAccessPermission = function (data) {
                return $http({
                    method: "POST",
                    url: "/api/Role/SaveAccessPermission?ts=" + new Date().getTime(),
                    data: data,
                    contentType: "application/json"
                });
            };

            /*delete access permission*/
            rolelist.DeleteAccessPermission = function (id) {
                return $http({
                    method: "POST",
                    url: "/api/Role/DeleteAccessPermission?ts=" + new Date().getTime(),
                    data: id,
                    contentType: "application/json"
                });
            };

            /* Get user list by role Id*/
            rolelist.GetUsersByRole = function (roleId) {
                return $http({
                    method: "GET",
                    url: "/api/Role/GetUsersByRole?ts=" + new Date().getTime() + "&roleId=" + roleId
                });
            };

            return rolelist;
        }
    ]);