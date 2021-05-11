/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
    .service("FestivalService", [
        "$http",
        function ($http) {

            var festivallist = {};

            /*retrieve festival type list*/
            festivallist.RetrieveFestivalType = function () {
                return $http({
                    method: 'GET',
                    url: "/api/Festival/RetrieveFestivalTypeList?ts=" + new Date().getTime()
                });
            }

            /*get festival list*/
            festivallist.GetFestivalList = function (topFilter,timezone, page, count, orderby, filter) {
                return $http({
                    method: "GET",
                    url: "/api/Festival/GetFestivalList?ts=" + new Date().getTime() + "&topfilter=" + topFilter + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&filter=" + filter
                });
            };

            /*add edit festival type*/
            festivallist.CreateUpdateFestival = function (data) {
                return $http({
                    method: "POST",
                    url: "/api/Festival/SaveFestival?ts=" + new Date().getTime(),
                    data: data,
                    contentType: "application/json"
                });
            };

            /*delete festival*/
            festivallist.DeleteFestival = function (id) {
                return $http({
                    method: "POST",
                    url: "/api/Festival/DeleteFestival?ts=" + new Date().getTime(),
                    data: JSON.stringify({ FestivalGroupId: id }),
                    contentType: "application/json"
                });
            };

            /*change festival status*/
            festivallist.ChangeStatus = function (id, status) {
                return $http({
                    method: "POST",
                    url: "/api/Festival/ChangeStatus?ts=" + new Date().getTime(),
                    data: JSON.stringify({ FestivalGroupId: id, IsActive: status }),
                    contentType: "application/json"
                });
            };

            return festivallist;
        }
    ]);