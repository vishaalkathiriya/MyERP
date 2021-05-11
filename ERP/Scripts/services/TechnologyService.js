/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
    .service("TechnologyService", [
        "$http",
        function ($http) {

            var techlist = {};

            /*retrieve tech group list*/
            techlist.RetrieveTechGroup = function () {
                return $http({
                    method: 'GET',
                    url: "/api/Technology/RetrieveTechGroupList?ts=" + new Date().getTime(),
                    //contentType: 'application/json; charset=utf-8'
                });
            }

            /*get technology list*/
            techlist.GetTechnologyList = function (topFilter, timezone, page, count, orderby, filter1, filter2) {
                return $http({
                    method: "GET",
                    url: "/api/Technology/GetTechnologyList?ts=" + new Date().getTime() +"&topFilter="+topFilter+"&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&filter1=" + filter1 + "&filter2=" + filter2
                });
            };

            /*add edit technology*/
            techlist.CreateUpdateTechnology = function (data) {
                return $http({
                    method: "POST",
                    url: "/api/Technology/SaveTechnology?ts=" + new Date().getTime(),
                    data: data,
                    contentType: "application/json"
                });
            };

            /*delete technology*/
            techlist.DeleteTechnology = function (id) {
                return $http({
                    method: "POST",
                    url: "/api/Technology/DeleteTechnology?ts=" + new Date().getTime(),
                    data: id,
                    contentType: "application/json"
                });
            };

            /*change status of technology*/
            techlist.ChangeStatus = function (id, status) {
                return $http({
                    method: "POST",
                    url: "/api/Technology/ChangeStatus?ts=" + new Date().getTime(),
                    data: JSON.stringify({ id: id, isactive: status }),
                    contentType: "application/json"
                });
            };

            return techlist;
        }
    ]);