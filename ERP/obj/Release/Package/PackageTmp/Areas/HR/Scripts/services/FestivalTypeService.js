/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
    .service("FestivalTypeService", [
        "$http",
        function ($http) {

            var festivallist = {};

            /*get festival type list*/
            festivallist.GetFestivalTypeList = function (timezone, page, count, orderby, filter) {
                return  $http({
                    method: "POST",
                    data: filter,
                    contentType: "application/json",
                    url: "/api/FestivalType/GetFestivalTypeList?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby
                });
            };

            /*add edit festival type*/
            festivallist.CreateUpdateFestivalType = function (data) {
                return $http({
                    method: "POST",
                    url: "/api/FestivalType/SaveFestivalType?ts=" + new Date().getTime(),
                    data: data,
                    contentType: "application/json"
                });
            };

            /*delete festival type*/
            festivallist.DeleteFestivalType = function (id) {
                return $http({
                    method: "POST",
                    url: "/api/FestivalType/DeleteFestivalType?ts=" + new Date().getTime(),
                    data: id,
                    contentType: "application/json"
                });
            };
            
            /*active inactive festival type*/
            festivallist.ChangeStatus = function (id, status) {
                return $http({
                    method: "POST",
                    url: "/api/FestivalType/ChangeStatus?ts=" + new Date().getTime(),
                    data: JSON.stringify({ FestivalTypeId: id, isactive: status }),
                    contentType: "application/json"
                });
            };
            return festivallist;
        }
    ]);